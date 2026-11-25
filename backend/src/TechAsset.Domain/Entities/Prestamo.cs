using TechAsset.Domain.Enums;
using TechAsset.Domain.Exceptions;

namespace TechAsset.Domain.Entities;

/// <summary>
/// Préstamo de un equipo tecnológico a un colaborador.
/// Modela el ciclo completo: entrega (RF-09), devolución (RF-10) e historial (RF-11).
/// </summary>
public sealed class Prestamo : EntidadBase
{
    /// <summary>Fecha en la que se entrega el equipo (RN-04).</summary>
    public DateTime FechaPrestamo { get; private set; }

    /// <summary>Fecha de devolución efectiva; <c>null</c> mientras el préstamo está activo.</summary>
    public DateTime? FechaDevolucion { get; private set; }

    /// <summary>Observaciones del préstamo o de la devolución (RN-10: obligatorias al devolver).</summary>
    public string? Observacion { get; private set; }

    /// <summary>Observación registrada específicamente en la devolución.</summary>
    public string? ObservacionDevolucion { get; private set; }

    /// <summary>Usuario que recibe el equipo (solicitante).</summary>
    public Guid UsuarioId { get; private set; }

    /// <summary>Equipo entregado.</summary>
    public Guid EquipoId { get; private set; }

    /// <summary>Responsable que entrega y supervisa el equipo (RN-05).</summary>
    public Guid ResponsableId { get; private set; }

    public EstadoPrestamo Estado { get; private set; } = EstadoPrestamo.Activo;

    public Usuario? Usuario { get; private set; }
    public Equipo? Equipo { get; private set; }
    public Usuario? Responsable { get; private set; }

    private Prestamo() { }

    public Prestamo(
        Guid usuarioId,
        Guid equipoId,
        Guid responsableId,
        DateTime fechaPrestamo,
        string? observacion,
        DateTime ahora)
    {
        UsuarioId = usuarioId;
        EquipoId = equipoId;
        ResponsableId = responsableId;
        FechaPrestamo = fechaPrestamo.Date;
        Observacion = NormalizarOpcional(observacion);
        Estado = EstadoPrestamo.Activo;
        MarcarCreada(ahora);
    }

    /// <summary>Registra la devolución del equipo (RF-10 / RN-10).</summary>
    public void RegistrarDevolucion(DateTime fechaDevolucion, string observacion, DateTime ahora)
    {
        if (Estado == EstadoPrestamo.Devuelto)
        {
            throw new ReglaNegocioException("RN-10", "El préstamo ya fue devuelto anteriormente.");
        }

        if (string.IsNullOrWhiteSpace(observacion))
        {
            throw new ReglaNegocioException(
                "RN-10",
                "Toda devolución debe registrar observaciones (estado del equipo, incidencias, etc.).");
        }

        if (fechaDevolucion.Date <= FechaPrestamo.Date)
        {
            throw new ReglaNegocioException(
                "RN-04",
                "La fecha de devolución debe ser posterior a la fecha de préstamo.");
        }

        FechaDevolucion = fechaDevolucion.Date;
        ObservacionDevolucion = observacion.Trim();
        Estado = EstadoPrestamo.Devuelto;
        MarcarActualizada(ahora);
    }

    public bool EstaActivo => Estado == EstadoPrestamo.Activo;

    /// <summary>
    /// Vincula las entidades relacionadas con el préstamo (colaborador, equipo y
    /// responsable). Permite que la respuesta de la aplicación incluya la
    /// información completa sin consultas adicionales.
    /// </summary>
    public void VincularRelaciones(Usuario usuario, Equipo equipo, Usuario responsable)
    {
        Usuario = usuario;
        Equipo = equipo;
        Responsable = responsable;
    }

    /// <summary>Días transcurridos del préstamo (calculado sobre la fecha actual de registro).</summary>
    public int DiasDePrestamo(DateTime referencia) =>
        ((FechaDevolucion ?? referencia.Date) - FechaPrestamo.Date).Days;

    private static string? NormalizarOpcional(string? valor)
    {
        var limpio = valor?.Trim();
        return string.IsNullOrWhiteSpace(limpio) ? null : limpio;
    }
}
