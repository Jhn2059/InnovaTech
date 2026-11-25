using System.Security.Claims;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Domain.Enums;

namespace TechAsset.API.Seguridad;

/// <summary>Implementación de <see cref="IUsuarioActual"/> a partir de los claims del JWT.</summary>
public sealed class UsuarioActualAccessor : IUsuarioActual
{
    private readonly IHttpContextAccessor _accesor;

    public UsuarioActualAccessor(IHttpContextAccessor accesor) => _accesor = accesor;

    private ClaimsPrincipal? Usuario => _accesor.HttpContext?.User;

    public Guid Id =>
        Guid.TryParse(Usuario?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

    public string Correo => Usuario?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    public string Nombre => Usuario?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    public string Rol => Usuario?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    public bool EsAdministrador => string.Equals(Rol, RolUsuario.Administrador.ToString(), StringComparison.OrdinalIgnoreCase);

    public bool EstaAutenticado => Usuario?.Identity?.IsAuthenticated ?? false;
}

/// <summary>Políticas de autorización por rol (actores del caso práctico).</summary>
public static class Politicas
{
    /// <summary>Acceso de cualquier usuario autenticado.</summary>
    public const string Autenticado = "Autenticado";

    /// <summary>Administrador del sistema.</summary>
    public const string Administrador = "Administrador";

    /// <summary>Administrador o Responsable de Tecnología.</summary>
    public const string GestionInventario = "GestionInventario";
}
