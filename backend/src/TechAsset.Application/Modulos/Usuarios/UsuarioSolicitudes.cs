using TechAsset.Domain.Enums;
using FluentValidation;

namespace TechAsset.Application.Modulos.Usuarios;

/// <summary>Solicitud de alta de usuario (RF-01).</summary>
public sealed class CrearUsuarioSolicitud
{
    public string Nombres { get; init; } = string.Empty;
    public string Apellidos { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public string Area { get; init; } = string.Empty;
    public RolUsuario Rol { get; init; } = RolUsuario.Colaborador;
    public string Contrasena { get; init; } = string.Empty;
    public string? ConfirmarContrasena { get; init; }
}

/// <summary>Solicitud de modificación de usuario (RF-02).</summary>
public sealed class ActualizarUsuarioSolicitud
{
    public string Nombres { get; init; } = string.Empty;
    public string Apellidos { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public string Area { get; init; } = string.Empty;
    public RolUsuario Rol { get; init; } = RolUsuario.Colaborador;
}

/// <summary>Solicitud de deshabilitación/reactivación de usuario (RF-04 / RN-09).</summary>
public sealed class CambiarEstadoUsuarioSolicitud
{
    public bool Habilitar { get; init; }
    public string? Motivo { get; init; }
}

/// <summary>Validador de alta de usuario (validación de backend, Actividad 2).</summary>
public sealed class CrearUsuarioValidador : AbstractValidator<CrearUsuarioSolicitud>
{
    public CrearUsuarioValidador()
    {
        RuleFor(x => x.Nombres)
            .NotEmpty().WithMessage("Los nombres son obligatorios.")
            .MaximumLength(100).WithMessage("Los nombres no pueden superar los 100 caracteres.");

        RuleFor(x => x.Apellidos)
            .NotEmpty().WithMessage("Los apellidos son obligatorios.")
            .MaximumLength(100).WithMessage("Los apellidos no pueden superar los 100 caracteres.");

        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .MaximumLength(150).WithMessage("El correo no puede superar los 150 caracteres.")
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.");

        RuleFor(x => x.Area)
            .NotEmpty().WithMessage("El área es obligatoria.")
            .MaximumLength(100).WithMessage("El área no puede superar los 100 caracteres.");

        RuleFor(x => x.Rol)
            .IsInEnum().WithMessage("El rol seleccionado no es válido.");

        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .MaximumLength(100).WithMessage("La contraseña no puede superar los 100 caracteres.")
            .Matches("[A-Za-z]").WithMessage("La contraseña debe incluir al menos una letra.")
            .Matches("[0-9]").WithMessage("La contraseña debe incluir al menos un número.");

        RuleFor(x => x.ConfirmarContrasena)
            .NotEmpty().WithMessage("Debe confirmar la contraseña.")
            .Equal(x => x.Contrasena).WithMessage("Las contraseñas no coinciden.");
    }
}

/// <summary>Validador de modificación de usuario.</summary>
public sealed class ActualizarUsuarioValidador : AbstractValidator<ActualizarUsuarioSolicitud>
{
    public ActualizarUsuarioValidador()
    {
        RuleFor(x => x.Nombres)
            .NotEmpty().WithMessage("Los nombres son obligatorios.")
            .MaximumLength(100).WithMessage("Los nombres no pueden superar los 100 caracteres.");

        RuleFor(x => x.Apellidos)
            .NotEmpty().WithMessage("Los apellidos son obligatorios.")
            .MaximumLength(100).WithMessage("Los apellidos no pueden superar los 100 caracteres.");

        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .MaximumLength(150).WithMessage("El correo no puede superar los 150 caracteres.")
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.");

        RuleFor(x => x.Area)
            .NotEmpty().WithMessage("El área es obligatoria.")
            .MaximumLength(100).WithMessage("El área no puede superar los 100 caracteres.");

        RuleFor(x => x.Rol)
            .IsInEnum().WithMessage("El rol seleccionado no es válido.");
    }
}

/// <summary>Validador de cambio de estado de usuario (RN-09 / RN-10).</summary>
public sealed class CambiarEstadoUsuarioValidador : AbstractValidator<CambiarEstadoUsuarioSolicitud>
{
    public CambiarEstadoUsuarioValidador()
    {
        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("Debe registrar el motivo del cambio de estado.")
            .MaximumLength(300).WithMessage("El motivo no puede superar los 300 caracteres.")
            .When(x => !x.Habilitar);
    }
}
