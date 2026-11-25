using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TechAsset.API.Common;
using TechAsset.API.Filtros;
using TechAsset.API.Middleware;
using TechAsset.API.Seguridad;
using TechAsset.Application;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Domain.Enums;
using TechAsset.Infrastructure;
using TechAsset.Infrastructure.Persistence;
using TechAsset.Infrastructure.Servicios;

var builder = WebApplication.CreateBuilder(args);

// ── Logging estructurado (Serilog) ──────────────────────────────────────────────
builder.Host.UseSerilog((contexto, configuracion) => configuracion
    .ReadFrom.Configuration(contexto.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// ── Servicios de la aplicación e infraestructura ────────────────────────────────
builder.Services.AddControllers(options => options.Filters.Add<ValidacionSolicitudFilter>());
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioActual, UsuarioActualAccessor>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── Autenticación JWT y políticas por rol ───────────────────────────────────────
var jwtOpciones = builder.Configuration.GetSection(JwtOpciones.Seccion).Get<JwtOpciones>()
    ?? throw new InvalidOperationException("Falta la sección de configuración 'Jwt'.");

if (string.IsNullOrWhiteSpace(jwtOpciones.ClaveSecreta) || jwtOpciones.ClaveSecreta.Length < 32)
{
    throw new InvalidOperationException("La clave secreta de JWT debe tener al menos 32 caracteres.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// La validación del token se configura a partir de la sección "Jwt" mediante un
// configurador dedicado: el middleware y el emisor comparten la misma clave,
// de modo que nunca puedan quedar desalineados.
builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfiguradorJwt>();

builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy(Politicas.Autenticado, politica => politica.RequireAuthenticatedUser());
    opciones.AddPolicy(Politicas.Administrador, politica => politica
        .RequireAuthenticatedUser()
        .RequireRole(RolUsuario.Administrador.ToString()));
    opciones.AddPolicy(Politicas.GestionInventario, politica => politica
        .RequireAuthenticatedUser()
        .RequireRole(RolUsuario.Administrador.ToString(), RolUsuario.ResponsableTecnologia.ToString()));
});

// ── Documentación de la API (Swagger / OpenAPI) ────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TechAsset · API de Inventario y Préstamos",
        Version = "v1",
        Description = """
        API del caso práctico TechAsset – Sistema de Gestión de Inventario y Préstamo
        de Equipos Tecnológicos de Innovatech Solutions S.A.C.

        **Módulos:** Usuarios (RF-01…RF-04), Equipos (RF-05…RF-08), Préstamos (RF-09…RF-12)
        y Reportes (RF-13…RF-17).

        **Reglas de negocio expuestas en los códigos de error:**
        RN-01 un equipo con un solo préstamo activo · RN-02/03 equipos en mantenimiento o baja
        no se prestan · RN-04 la devolución es posterior al préstamo · RN-05 todo préstamo
        registra responsable · RN-06 los equipos con historial no se eliminan · RN-07/RN-08
        unicidad de código patrimonial y correo · RN-09 solo el Administrador deshabilita
        usuarios · RN-10 toda devolución registra observaciones.
        """,
        Contact = new OpenApiContact { Name = "Innovatech Solutions S.A.C. · Gerencia de Tecnología" }
    });

    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduzca el token JWT obtenido en POST /api/auth/login."
    });

    opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    var archivoXml = Path.Combine(AppContext.BaseDirectory, "TechAsset.API.xml");
    if (File.Exists(archivoXml))
    {
        opciones.IncludeXmlComments(archivoXml);
    }
});

builder.Services.AddCors(opciones => opciones.AddPolicy("Frontend", politica =>
{
    var origenes = builder.Configuration.GetSection("Cors:Origenes").Get<string[]>()
        ?? new[] { "http://localhost:5173", "http://127.0.0.1:5173" };

    politica.WithOrigins(origenes)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
}));

builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>("postgresql");

var app = builder.Build();

// ── Tubería de peticiones ──────────────────────────────────────────────────────
app.UseSerilogRequestLogging();
app.UseMiddleware<ManejadorExcepcionesMiddleware>();
app.UseCors("Frontend");

if (app.Environment.IsDevelopment() || builder.Configuration.GetValue("Swagger:Activo", true))
{
    app.UseSwagger();
    app.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "TechAsset API v1");
        opciones.RoutePrefix = "swagger";
        opciones.DocumentTitle = "TechAsset API";
        opciones.DisplayRequestDuration();
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = async (contexto, informe) =>
    {
        contexto.Response.ContentType = "application/json";
        var respuesta = new
        {
            estado = informe.Status.ToString(),
            total = informe.TotalDuration.TotalMilliseconds,
            comprobaciones = informe.Entries.Select(e => new { e.Key, estado = e.Value.Status.ToString(), e.Value.Description })
        };
        await contexto.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(respuesta));
    }
});

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

// ── Datos de demostración (opcional, configurables) ─────────────────────────────
await app.Services.SembrarDatosDemoAsync(app.Configuration);

app.Run();

/// <summary>Punto de entrada accesible para las pruebas de integración.</summary>
public partial class Program { }
