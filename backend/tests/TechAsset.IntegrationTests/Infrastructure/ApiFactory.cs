using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Domain.Entities;
using TechAsset.Domain.Enums;
using TechAsset.Infrastructure.Persistence;

namespace TechAsset.IntegrationTests.Infrastructure;

/// <summary>
/// Fábrica de la API para pruebas de integración. Levanta la aplicación real
/// (con DI, JWT, EF Core y PostgreSQL) contra la base de datos de pruebas.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string CadenaConexionPruebas =
        "Host=localhost;Port=5432;Database=techasset_test;Username=techasset;Password=techasset";

    /// <summary>Credenciales del administrador base creado para las pruebas.</summary>
    public const string CorreoAdministrador = "admin.pruebas@techasset.pe";
    public const string ContrasenaAdministrador = "Admin1234";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuracion) =>
        {
            configuracion.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:TechAsset"] = CadenaConexionPruebas,
                ["Jwt:Emisor"] = "TechAsset",
                ["Jwt:Audiencia"] = "TechAssetClientes",
                ["Jwt:ClaveSecreta"] = "ClaveSecretaDePruebasIntegracion_TechAsset_2024_9f2a7c4e6b1",
                ["Jwt:MinutosExpiracion"] = "60",
                ["SembrarDatos"] = "false",
                ["Swagger:Activo"] = "false"
            });
        });
    }

    /// <summary>Crea un cliente HTTP con el token indicado (cliente independiente).</summary>
    public HttpClient CrearClienteAutenticado(string token)
    {
        var cliente = CreateClient();
        cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return cliente;
    }

    /// <summary>Aplica las migraciones sobre la base de datos de pruebas.</summary>
    public async Task PrepararBaseDeDatosAsync()
    {
        using var alcance = Services.CreateScope();
        var contexto = alcance.ServiceProvider.GetRequiredService<AppDbContext>();
        await contexto.Database.MigrateAsync();
    }

    /// <summary>
    /// Deja la base de pruebas en un estado conocido: vacía las tablas de negocio
    /// y crea el administrador base que permite autenticar las pruebas.
    /// </summary>
    public async Task ReiniciarDatosAsync()
    {
        using var alcance = Services.CreateScope();
        var contexto = alcance.ServiceProvider.GetRequiredService<AppDbContext>();
        var hash = alcance.ServiceProvider.GetRequiredService<IHashContrasena>();

        await contexto.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE \"Movimientos\", \"Prestamos\", \"Equipos\", \"Usuarios\" RESTART IDENTITY CASCADE");

        var administrador = new Usuario(
            "Administrador",
            "de Pruebas",
            CorreoAdministrador,
            "TI",
            RolUsuario.Administrador,
            hash.Hash(ContrasenaAdministrador),
            DateTime.UtcNow);

        contexto.Usuarios.Add(administrador);
        await contexto.SaveChangesAsync();
    }

    public async Task InitializeAsync()
    {
        await PrepararBaseDeDatosAsync();
        await ReiniciarDatosAsync();
    }

    public new Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }
}
