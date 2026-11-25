using TechAsset.Domain.Enums;

namespace TechAsset.Application.Dtos;

/// <summary>Proyección de usuario para la interfaz (RF-03).</summary>
public sealed class UsuarioDto
{
    public Guid Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string Apellidos { get; init; } = string.Empty;
    public string NombreCompleto { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public string Area { get; init; } = string.Empty;
    public RolUsuario Rol { get; init; }
    public string RolNombre { get; init; } = string.Empty;
    public bool Estado { get; init; }
    public DateTime FechaCreacion { get; init; }
    public DateTime? FechaDeshabilitacion { get; init; }
    public string? MotivoDeshabilitacion { get; init; }
    public DateTime? UltimoAcceso { get; init; }
    public int TotalPrestamos { get; init; }
}

/// <summary>Proyección de equipo para la interfaz (RF-07 / RF-14).</summary>
public sealed class EquipoDto
{
    public Guid Id { get; init; }
    public string CodigoPatrimonial { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public string Marca { get; init; } = string.Empty;
    public string? NumeroSerie { get; init; }
    public string? Ubicacion { get; init; }
    public DateTime? FechaAdquisicion { get; init; }
    public string? Especificaciones { get; init; }
    public EstadoEquipo Estado { get; init; }
    public string EstadoNombre { get; init; } = string.Empty;
    public string? MotivoEstado { get; init; }
    public DateTime FechaCreacion { get; init; }
    public bool TienePrestamoActivo { get; init; }
    public Guid? UsuarioActualId { get; init; }
    public string? UsuarioActual { get; init; }
    public DateTime? FechaPrestamoActual { get; init; }
    public int TotalPrestamos { get; init; }
    public bool PuedeEliminar { get; init; }
}

/// <summary>Proyección de préstamo para la interfaz (RF-11 / RF-12).</summary>
public sealed class PrestamoDto
{
    public Guid Id { get; init; }
    public DateTime FechaPrestamo { get; init; }
    public DateTime? FechaDevolucion { get; init; }
    public string? Observacion { get; init; }
    public string? ObservacionDevolucion { get; init; }
    public EstadoPrestamo Estado { get; init; }
    public string EstadoNombre { get; init; } = string.Empty;
    public int DiasDePrestamo { get; init; }
    public Guid UsuarioId { get; init; }
    public string UsuarioNombre { get; init; } = string.Empty;
    public string UsuarioArea { get; init; } = string.Empty;
    public Guid EquipoId { get; init; }
    public string CodigoPatrimonial { get; init; } = string.Empty;
    public string EquipoNombre { get; init; } = string.Empty;
    public string EquipoCategoria { get; init; } = string.Empty;
    public Guid ResponsableId { get; init; }
    public string ResponsableNombre { get; init; } = string.Empty;
}

/// <summary>Registro de auditoría expuesto en la bitácora de movimientos (RF-17).</summary>
public sealed class MovimientoDto
{
    public Guid Id { get; init; }
    public DateTime Fecha { get; init; }
    public string Tipo { get; init; } = string.Empty;
    public string TipoNombre { get; init; } = string.Empty;
    public string Descripcion { get; init; } = string.Empty;
    public string Entidad { get; init; } = string.Empty;
    public Guid EntidadId { get; init; }
    public string? UsuarioNombre { get; init; }
}

/// <summary>Elemento de los reportes agregados.</summary>
public sealed class IndicadorDto
{
    public string Etiqueta { get; init; } = string.Empty;
    public int Valor { get; init; }
    public double Porcentaje { get; init; }
}
