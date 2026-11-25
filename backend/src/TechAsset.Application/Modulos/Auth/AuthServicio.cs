using FluentValidation;
using TechAsset.Application.Common;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Application.Common.Extensions;
using TechAsset.Application.Common.Mapping;
using TechAsset.Application.Dtos;
using TechAsset.Domain.Entities;

namespace TechAsset.Application.Modulos.Auth;

/// <summary>Credenciales de acceso al sistema.</summary>
public sealed class IniciarSesionSolicitud
{
    public string Correo { get; init; } = string.Empty;
    public string Contrasena { get; init; } = string.Empty;
}

/// <summary>Respuesta de autenticación: token JWT y perfil del usuario.</summary>
public sealed class RespuestaSesion
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiraEn { get; init; }
    public UsuarioDto Usuario { get; init; } = null!;
}

/// <summary>Validador de las credenciales.</summary>
public sealed class IniciarSesionValidador : AbstractValidator<IniciarSesionSolicitud>
{
    public IniciarSesionValidador()
    {
        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.");

        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}

/// <summary>Caso de uso de autenticación.</summary>
public interface IAuthServicio
{
    Task<Result<RespuestaSesion>> IniciarSesionAsync(IniciarSesionSolicitud solicitud, CancellationToken cancelacion = default);
}

/// <inheritdoc cref="IAuthServicio" />
public sealed class AuthServicio : IAuthServicio
{
    private readonly IRepositorioUsuario _usuarios;
    private readonly IHashContrasena _hash;
    private readonly IJwtTokenService _tokens;
    private readonly IFechaActual _fecha;

    public AuthServicio(
        IRepositorioUsuario usuarios,
        IHashContrasena hash,
        IJwtTokenService tokens,
        IFechaActual fecha)
    {
        _usuarios = usuarios;
        _hash = hash;
        _tokens = tokens;
        _fecha = fecha;
    }

    public async Task<Result<RespuestaSesion>> IniciarSesionAsync(IniciarSesionSolicitud solicitud, CancellationToken cancelacion = default)
    {
        var correo = solicitud.Correo.Trim().ToLowerInvariant();
        var usuario = await _usuarios.ObtenerPorCorreoAsync(correo, cancelacion);

        // Mensaje genérico: no revela si el correo existe (seguridad de la información).
        if (usuario is null || string.IsNullOrEmpty(usuario.HashContrasena) ||
            !_hash.Verificar(solicitud.Contrasena, usuario.HashContrasena))
        {
            return Result.Fallo<RespuestaSesion>("CREDENCIALES_INVALIDAS", "El correo o la contraseña son incorrectos.");
        }

        if (!usuario.Estado)
        {
            return Result.Fallo<RespuestaSesion>(
                "USUARIO_DESHABILITADO",
                $"El usuario {usuario.NombreCompleto} está deshabilitado. Contacte al administrador.");
        }

        usuario.RegistrarAcceso(_fecha.Ahora);
        _usuarios.Actualizar(usuario);

        var (token, expiraEn) = _tokens.Generar(usuario);
        return Result.Exito(new RespuestaSesion
        {
            Token = token,
            ExpiraEn = expiraEn,
            Usuario = usuario.ADto()
        });
    }
}
