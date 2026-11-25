namespace TechAsset.Domain.Entities;

/// <summary>Tipo de acción registrada en la bitácora de movimientos (RF-17).</summary>
public enum TipoMovimiento
{
    AltaUsuario = 1,
    ActualizacionUsuario = 2,
    CambioEstadoUsuario = 3,
    AltaEquipo = 4,
    ActualizacionEquipo = 5,
    CambioEstadoEquipo = 6,
    Prestamo = 7,
    Devolucion = 8
}

/// <summary>
/// Registro inmutable de auditoría. Permite trazar quién realizó cada acción
/// (criterio de trazabilidad y seguridad, ISO/IEC 25010 e ISO/IEC 27001).
/// </summary>
public sealed class Movimiento : EntidadBase
{
    public DateTime Fecha { get; private set; }
    public TipoMovimiento Tipo { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public string Entidad { get; private set; } = string.Empty;
    public Guid EntidadId { get; private set; }
    public Guid? UsuarioId { get; private set; }
    public string? UsuarioNombre { get; private set; }

    private Movimiento() { }

    public Movimiento(
        TipoMovimiento tipo,
        string descripcion,
        string entidad,
        Guid entidadId,
        Guid? usuarioId,
        string? usuarioNombre,
        DateTime fecha)
    {
        Tipo = tipo;
        Descripcion = (descripcion ?? string.Empty).Trim();
        Entidad = (entidad ?? string.Empty).Trim();
        EntidadId = entidadId;
        UsuarioId = usuarioId;
        UsuarioNombre = usuarioNombre?.Trim();
        Fecha = fecha;
        MarcarCreada(fecha);
    }
}
