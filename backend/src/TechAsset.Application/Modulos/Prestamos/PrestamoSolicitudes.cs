using FluentValidation;

namespace TechAsset.Application.Modulos.Prestamos;

/// <summary>Solicitud de registro de préstamo (RF-09).</summary>
public sealed class RegistrarPrestamoSolicitud
{
    public Guid UsuarioId { get; init; }
    public Guid EquipoId { get; init; }
    public DateTime? FechaPrestamo { get; init; }
    public string? Observacion { get; init; }
}

/// <summary>Solicitud de registro de devolución (RF-10).</summary>
public sealed class RegistrarDevolucionSolicitud
{
    public DateTime? FechaDevolucion { get; init; }
    public string Observacion { get; init; } = string.Empty;
}

/// <summary>Validador de registro de préstamo.</summary>
public sealed class RegistrarPrestamoValidador : AbstractValidator<RegistrarPrestamoSolicitud>
{
    public RegistrarPrestamoValidador()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("Debe seleccionar al colaborador que recibe el equipo.");

        RuleFor(x => x.EquipoId)
            .NotEmpty().WithMessage("Debe seleccionar el equipo a prestar.");

        // Si no se indica fecha de préstamo, el caso de uso registra la fecha actual.
        RuleFor(x => x.FechaPrestamo)
            .Must(f => f is null || f.Value.Date <= DateTime.Now.Date)
            .WithMessage("La fecha de préstamo no puede ser futura.");

        RuleFor(x => x.Observacion)
            .MaximumLength(500).WithMessage("La observación no puede superar los 500 caracteres.");
    }
}

/// <summary>Validador de registro de devolución (RN-10).</summary>
public sealed class RegistrarDevolucionValidador : AbstractValidator<RegistrarDevolucionSolicitud>
{
    public RegistrarDevolucionValidador()
    {
        RuleFor(x => x.FechaDevolucion)
            .NotNull().WithMessage("La fecha de devolución es obligatoria.");

        // RN-10 · Toda devolución deberá registrar observaciones.
        RuleFor(x => x.Observacion)
            .NotEmpty().WithMessage("Toda devolución debe registrar observaciones.")
            .MinimumLength(5).WithMessage("La observación debe tener al menos 5 caracteres.")
            .MaximumLength(500).WithMessage("La observación no puede superar los 500 caracteres.");
    }
}
