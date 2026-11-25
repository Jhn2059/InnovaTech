using FluentValidation;
using TechAsset.Domain.Enums;

namespace TechAsset.Application.Modulos.Equipos;

/// <summary>Solicitud de alta de equipo tecnológico (RF-05).</summary>
public sealed class CrearEquipoSolicitud
{
    public string CodigoPatrimonial { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public string Marca { get; init; } = string.Empty;
    public string? NumeroSerie { get; init; }
    public string? Ubicacion { get; init; }
    public DateTime? FechaAdquisicion { get; init; }
    public string? Especificaciones { get; init; }
}

/// <summary>Solicitud de modificación de equipo (RF-06).</summary>
public sealed class ActualizarEquipoSolicitud
{
    public string Nombre { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public string Marca { get; init; } = string.Empty;
    public string? NumeroSerie { get; init; }
    public string? Ubicacion { get; init; }
    public DateTime? FechaAdquisicion { get; init; }
    public string? Especificaciones { get; init; }
}

/// <summary>Solicitud de actualización del estado del equipo (RF-08).</summary>
public sealed class CambiarEstadoEquipoSolicitud
{
    public EstadoEquipo Estado { get; init; }
    public string? Motivo { get; init; }
}

/// <summary>Validador de alta de equipo.</summary>
public sealed class CrearEquipoValidador : AbstractValidator<CrearEquipoSolicitud>
{
    public CrearEquipoValidador()
    {
        RuleFor(x => x.CodigoPatrimonial)
            .NotEmpty().WithMessage("El código patrimonial es obligatorio.")
            .MaximumLength(50).WithMessage("El código patrimonial no puede superar los 50 caracteres.")
            .Matches("^[A-Za-z0-9\\-]+$").WithMessage("El código patrimonial solo admite letras, números y guiones.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del equipo es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Categoria)
            .NotEmpty().WithMessage("La categoría es obligatoria.")
            .MaximumLength(50).WithMessage("La categoría no puede superar los 50 caracteres.");

        RuleFor(x => x.Marca)
            .NotEmpty().WithMessage("La marca es obligatoria.")
            .MaximumLength(50).WithMessage("La marca no puede superar los 50 caracteres.");

        RuleFor(x => x.NumeroSerie)
            .MaximumLength(60).WithMessage("El número de serie no puede superar los 60 caracteres.");

        RuleFor(x => x.Ubicacion)
            .MaximumLength(120).WithMessage("La ubicación no puede superar los 120 caracteres.");

        RuleFor(x => x.Especificaciones)
            .MaximumLength(500).WithMessage("Las especificaciones no pueden superar los 500 caracteres.");

        RuleFor(x => x.FechaAdquisicion)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de adquisición no puede ser futura.");
    }
}

/// <summary>Validador de modificación de equipo.</summary>
public sealed class ActualizarEquipoValidador : AbstractValidator<ActualizarEquipoSolicitud>
{
    public ActualizarEquipoValidador()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del equipo es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Categoria)
            .NotEmpty().WithMessage("La categoría es obligatoria.")
            .MaximumLength(50).WithMessage("La categoría no puede superar los 50 caracteres.");

        RuleFor(x => x.Marca)
            .NotEmpty().WithMessage("La marca es obligatoria.")
            .MaximumLength(50).WithMessage("La marca no puede superar los 50 caracteres.");

        RuleFor(x => x.NumeroSerie)
            .MaximumLength(60).WithMessage("El número de serie no puede superar los 60 caracteres.");

        RuleFor(x => x.Ubicacion)
            .MaximumLength(120).WithMessage("La ubicación no puede superar los 120 caracteres.");

        RuleFor(x => x.Especificaciones)
            .MaximumLength(500).WithMessage("Las especificaciones no pueden superar los 500 caracteres.");

        RuleFor(x => x.FechaAdquisicion)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de adquisición no puede ser futura.");
    }
}

/// <summary>Validador de cambio de estado de equipo.</summary>
public sealed class CambiarEstadoEquipoValidador : AbstractValidator<CambiarEstadoEquipoSolicitud>
{
    public CambiarEstadoEquipoValidador()
    {
        RuleFor(x => x.Estado).IsInEnum().WithMessage("El estado seleccionado no es válido.");

        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("Debe registrar el motivo del cambio de estado.")
            .MaximumLength(300).WithMessage("El motivo no puede superar los 300 caracteres.")
            .When(x => x.Estado is EstadoEquipo.Mantenimiento or EstadoEquipo.Baja);
    }
}
