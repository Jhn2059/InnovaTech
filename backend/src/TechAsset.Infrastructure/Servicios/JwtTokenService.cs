using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Domain.Entities;
using TechAsset.Domain.Enums;

namespace TechAsset.Infrastructure.Servicios;

/// <summary>Opciones de configuración de la emisión de tokens JWT.</summary>
public sealed class JwtOpciones
{
    public const string Seccion = "Jwt";

    public string Emisor { get; set; } = "TechAsset";
    public string Audiencia { get; set; } = "TechAssetClientes";
    public string ClaveSecreta { get; set; } = string.Empty;
    public int MinutosExpiracion { get; set; } = 480;
}

/// <summary>Servicio de emisión de tokens JWT (implementación de <see cref="IJwtTokenService"/>).</summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOpciones _opciones;

    public JwtTokenService(IOptions<JwtOpciones> opciones) => _opciones = opciones.Value;

    public (string Token, DateTime ExpiraEn) Generar(Usuario usuario)
    {
        var expiraEn = DateTime.UtcNow.AddMinutes(_opciones.MinutosExpiracion);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Correo),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.NombreCompleto),
            new(ClaimTypes.Email, usuario.Correo),
            new(ClaimTypes.Role, usuario.Rol.ToString()),
            new("area", usuario.Area)
        };

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.ClaveSecreta)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opciones.Emisor,
            audience: _opciones.Audiencia,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiraEn,
            signingCredentials: credenciales);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEn);
    }
}
