namespace TechAsset.Domain.Entities;

/// <summary>
/// Base común de todas las entidades persistidas. Centraliza la trazabilidad
/// (alta y última modificación) exigida por los criterios de calidad ISO/IEC 25010
/// (mantenibilidad y trazabilidad de la información).
/// </summary>
public abstract class EntidadBase
{
    /// <summary>Identificador único (UUID v4) del registro.</summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    /// <summary>Fecha y hora de registro del documento.</summary>
    public DateTime FechaCreacion { get; protected set; }

    /// <summary>Fecha y hora de la última modificación.</summary>
    public DateTime FechaActualizacion { get; protected set; }

    /// <summary>Marca la entidad como persistida (asignado por EF Core).</summary>
    public bool EsNuevo => Id == Guid.Empty;

    protected void MarcarActualizada(DateTime fechaActual) => FechaActualizacion = fechaActual;

    protected void MarcarCreada(DateTime fechaActual)
    {
        FechaCreacion = fechaActual;
        FechaActualizacion = fechaActual;
    }
}
