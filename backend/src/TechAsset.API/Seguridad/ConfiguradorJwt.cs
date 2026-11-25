using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TechAsset.Infrastructure.Servicios;

namespace TechAsset.API.Seguridad;

/// <summary>
/// Configura la validación de tokens JWT a partir de la sección "Jwt".
/// Se registra como <see cref="IConfigureNamedOptions{TOptions}"/> para que el
/// Middleware y el emisor de tokens compartan exactamente la misma clave y
/// audiencia (fuente única de verdad, sin duplicar configuración).
/// </summary>
public sealed class ConfiguradorJwt : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IOptions<JwtOpciones> _opciones;

    public ConfiguradorJwt(IOptions<JwtOpciones> opciones) => _opciones = opciones;

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name is not null && name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        var configuracion = _opciones.Value;

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        // Se conservan los tipos de claim originales (ClaimTypes.*) para que la
        // política de autorización y el contexto de usuario lean el mismo claim.
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuracion.Emisor,
            ValidAudience = configuracion.Audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuracion.ClaveSecreta)),
            ClockSkew = TimeSpan.FromMinutes(1),
            // Se fija explícitamente el tipo de las claims para que la política de
            // autorización y el contexto de usuario leaned del mismo claim.
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            NameClaimType = System.Security.Claims.ClaimTypes.Name
        };
    }

    public void Configure(JwtBearerOptions options) => Configure(Options.DefaultName, options);
}
