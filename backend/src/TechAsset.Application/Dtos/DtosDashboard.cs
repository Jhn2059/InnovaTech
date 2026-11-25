using TechAsset.Domain.Enums;

namespace TechAsset.Application.Dtos;

/// <summary>
/// Indicadores mínimos exigidos por el caso práctico (sección 10) más los
/// indicadores de apoyo para la Gerencia de Tecnología.
/// </summary>
public sealed class DashboardDto
{
    public int TotalEquipos { get; init; }
    public int EquiposDisponibles { get; init; }
    public int EquiposPrestados { get; init; }
    public int EquiposMantenimiento { get; init; }
    public int EquiposBaja { get; init; }
    public int TotalPrestamos { get; init; }
    public int PrestamosActivos { get; init; }
    public int PrestamosDevueltos { get; init; }
    public int TotalUsuarios { get; init; }
    public int UsuariosActivos { get; init; }
    public double TasaOcupacion { get; init; }
    public string? AreaConMayorUso { get; init; }
    public int PrestamosAreaMayorUso { get; init; }
    public IReadOnlyList<IndicadorDto> EquiposPorEstado { get; init; } = Array.Empty<IndicadorDto>();
    public IReadOnlyList<IndicadorDto> PrestamosPorArea { get; init; } = Array.Empty<IndicadorDto>();
    public IReadOnlyList<IndicadorDto> PrestamosPorMes { get; init; } = Array.Empty<IndicadorDto>();
    public IReadOnlyList<EquipoMasUtilizadoDto> EquiposMasUtilizados { get; init; } = Array.Empty<EquipoMasUtilizadoDto>();
    public IReadOnlyList<MovimientoDto> UltimosMovimientos { get; init; } = Array.Empty<MovimientoDto>();
}

/// <summary>Equipo con mayor número de préstamos (RF-15).</summary>
public sealed class EquipoMasUtilizadoDto
{
    public Guid Id { get; init; }
    public string CodigoPatrimonial { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public string Marca { get; init; } = string.Empty;
    public int TotalPrestamos { get; init; }
    public double DiasPromedio { get; init; }
}

/// <summary>Reporte de préstamos agrupados por área (RF-16).</summary>
public sealed class PrestamosPorAreaDto
{
    public string Area { get; init; } = string.Empty;
    public int TotalPrestamos { get; init; }
    public int PrestamosActivos { get; init; }
    public int PrestamosDevueltos { get; init; }
    public double PromedioDias { get; init; }
}

/// <summary>Reporte de equipos por estado (RF-14).</summary>
public sealed class EquiposPorEstadoDto
{
    public EstadoEquipo Estado { get; init; }
    public string EstadoNombre { get; init; } = string.Empty;
    public int Total { get; init; }
    public double Porcentaje { get; init; }
}
