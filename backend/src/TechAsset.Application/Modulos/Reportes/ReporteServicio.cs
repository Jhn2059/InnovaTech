using TechAsset.Application.Common;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Application.Common.Mapping;
using TechAsset.Application.Dtos;

namespace TechAsset.Application.Modulos.Reportes;

/// <summary>Casos de uso de salida de información (RF-13 … RF-17).</summary>
public interface IReporteServicio
{
    Task<Result<DashboardDto>> ObtenerDashboardAsync(CancellationToken cancelacion = default);
    Task<Result<IReadOnlyList<EquiposPorEstadoDto>>> ObtenerEquiposPorEstadoAsync(CancellationToken cancelacion = default);
    Task<Result<IReadOnlyList<EquipoMasUtilizadoDto>>> ObtenerEquiposMasUtilizadosAsync(int tope = 10, CancellationToken cancelacion = default);
    Task<Result<IReadOnlyList<PrestamosPorAreaDto>>> ObtenerPrestamosPorAreaAsync(CancellationToken cancelacion = default);
    Task<Result<PagedResult<MovimientoDto>>> ObtenerMovimientosAsync(ConsultaMovimientos consulta, CancellationToken cancelacion = default);
}

/// <inheritdoc cref="IReporteServicio" />
public sealed class ReporteServicio : IReporteServicio
{
    private readonly IRepositorioEquipo _equipos;
    private readonly IRepositorioPrestamo _prestamos;
    private readonly IRepositorioUsuario _usuarios;
    private readonly IRepositorioMovimiento _movimientos;
    private readonly IFechaActual _fecha;

    public ReporteServicio(
        IRepositorioEquipo equipos,
        IRepositorioPrestamo prestamos,
        IRepositorioUsuario usuarios,
        IRepositorioMovimiento movimientos,
        IFechaActual fecha)
    {
        _equipos = equipos;
        _prestamos = prestamos;
        _usuarios = usuarios;
        _movimientos = movimientos;
        _fecha = fecha;
    }

    public async Task<Result<DashboardDto>> ObtenerDashboardAsync(CancellationToken cancelacion = default)
    {
        var equiposPorEstado = await _equipos.ObtenerEquiposPorEstadoAsync(cancelacion);
        var prestamosPorArea = await _prestamos.ObtenerPrestamosPorAreaAsync(cancelacion);
        var masUtilizados = await _prestamos.ObtenerEquiposMasUtilizadosAsync(5, cancelacion);
        var prestamosPorMes = await _prestamos.ObtenerPrestamosPorMesAsync(6, cancelacion);
        var (totalUsuarios, usuariosActivos) = await _usuarios.ObtenerTotalesAsync(cancelacion);
        var (items, _) = await _movimientos.ListarAsync(new ConsultaMovimientos { Pagina = 1, TamanoPagina = 8 }, cancelacion);

        var totalEquipos = equiposPorEstado.Sum(e => e.Total);
        var disponibles = equiposPorEstado.FirstOrDefault(e => e.Estado == Domain.Enums.EstadoEquipo.Disponible)?.Total ?? 0;
        var prestados = equiposPorEstado.FirstOrDefault(e => e.Estado == Domain.Enums.EstadoEquipo.Prestado)?.Total ?? 0;
        var mantenimiento = equiposPorEstado.FirstOrDefault(e => e.Estado == Domain.Enums.EstadoEquipo.Mantenimiento)?.Total ?? 0;
        var baja = equiposPorEstado.FirstOrDefault(e => e.Estado == Domain.Enums.EstadoEquipo.Baja)?.Total ?? 0;

        var prestamosPorAreaIndicadores = prestamosPorArea
            .Select(p => new IndicadorDto
            {
                Etiqueta = p.Area,
                Valor = p.TotalPrestamos,
                Porcentaje = CalcularPorcentaje(p.TotalPrestamos, prestamosPorArea.Sum(x => x.TotalPrestamos))
            })
            .OrderByDescending(p => p.Valor)
            .ToList();

        var areaConMayorUso = prestamosPorAreaIndicadores.FirstOrDefault();

        var dashboard = new DashboardDto
        {
            TotalEquipos = totalEquipos,
            EquiposDisponibles = disponibles,
            EquiposPrestados = prestados,
            EquiposMantenimiento = mantenimiento,
            EquiposBaja = baja,
            TotalPrestamos = prestamosPorArea.Sum(p => p.TotalPrestamos),
            PrestamosActivos = prestamosPorArea.Sum(p => p.PrestamosActivos),
            PrestamosDevueltos = prestamosPorArea.Sum(p => p.PrestamosDevueltos),
            TotalUsuarios = totalUsuarios,
            UsuariosActivos = usuariosActivos,
            TasaOcupacion = CalcularPorcentaje(prestados, totalEquipos),
            AreaConMayorUso = areaConMayorUso?.Etiqueta,
            PrestamosAreaMayorUso = areaConMayorUso?.Valor ?? 0,
            EquiposPorEstado = equiposPorEstado
                .Select(e => new IndicadorDto
                {
                    Etiqueta = e.EstadoNombre,
                    Valor = e.Total,
                    Porcentaje = e.Porcentaje
                })
                .ToList(),
            PrestamosPorArea = prestamosPorAreaIndicadores,
            PrestamosPorMes = prestamosPorMes,
            EquiposMasUtilizados = masUtilizados,
            UltimosMovimientos = items.Select(m => m.ADto()).ToList()
        };

        return Result.Exito(dashboard);
    }

    public async Task<Result<IReadOnlyList<EquiposPorEstadoDto>>> ObtenerEquiposPorEstadoAsync(CancellationToken cancelacion = default)
        => Result.Exito<IReadOnlyList<EquiposPorEstadoDto>>(await _equipos.ObtenerEquiposPorEstadoAsync(cancelacion));

    public async Task<Result<IReadOnlyList<EquipoMasUtilizadoDto>>> ObtenerEquiposMasUtilizadosAsync(int tope = 10, CancellationToken cancelacion = default)
    {
        var limite = tope is < 1 or > 50 ? 10 : tope;
        return Result.Exito<IReadOnlyList<EquipoMasUtilizadoDto>>(
            await _prestamos.ObtenerEquiposMasUtilizadosAsync(limite, cancelacion));
    }

    public async Task<Result<IReadOnlyList<PrestamosPorAreaDto>>> ObtenerPrestamosPorAreaAsync(CancellationToken cancelacion = default)
        => Result.Exito<IReadOnlyList<PrestamosPorAreaDto>>(await _prestamos.ObtenerPrestamosPorAreaAsync(cancelacion));

    public async Task<Result<PagedResult<MovimientoDto>>> ObtenerMovimientosAsync(ConsultaMovimientos consulta, CancellationToken cancelacion = default)
    {
        var (items, total) = await _movimientos.ListarAsync(consulta, cancelacion);
        var dtos = items.Select(m => m.ADto()).ToList();
        return Result.Exito(new PagedResult<MovimientoDto>(dtos, total, consulta.Pagina, consulta.TamanoPagina));
    }

    private static double CalcularPorcentaje(int valor, int total) =>
        total <= 0 ? 0 : Math.Round(valor * 100d / total, 1);
}
