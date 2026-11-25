using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechAsset.API.Seguridad;
using TechAsset.Application.Common;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Application.Dtos;
using TechAsset.Application.Modulos.Reportes;

namespace TechAsset.API.Controllers;

/// <summary>
/// Módulos de salida de información: dashboard y reportes (RF-13 … RF-17).
/// </summary>
[Route("api/reportes")]
[Authorize]
public sealed class ReportesController : ApiControllerBase
{
    private readonly IReporteServicio _servicio;

    public ReportesController(IReporteServicio servicio) => _servicio = servicio;

    /// <summary>Dashboard general con los indicadores mínimos exigidos (RF-13).</summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardDto>> Dashboard(CancellationToken cancelacion)
        => Responder(await _servicio.ObtenerDashboardAsync(cancelacion));

    /// <summary>Equipos agrupados por estado (RF-14).</summary>
    [HttpGet("equipos-por-estado")]
    public async Task<ActionResult<IReadOnlyList<EquiposPorEstadoDto>>> EquiposPorEstado(CancellationToken cancelacion)
        => Responder(await _servicio.ObtenerEquiposPorEstadoAsync(cancelacion));

    /// <summary>Equipos con mayor número de préstamos (RF-15).</summary>
    [HttpGet("equipos-mas-utilizados")]
    public async Task<ActionResult<IReadOnlyList<EquipoMasUtilizadoDto>>> EquiposMasUtilizados([FromQuery] int tope = 10, CancellationToken cancelacion = default)
        => Responder(await _servicio.ObtenerEquiposMasUtilizadosAsync(tope, cancelacion));

    /// <summary>Préstamos agrupados por área (RF-16).</summary>
    [HttpGet("prestamos-por-area")]
    public async Task<ActionResult<IReadOnlyList<PrestamosPorAreaDto>>> PrestamosPorArea(CancellationToken cancelacion)
        => Responder(await _servicio.ObtenerPrestamosPorAreaAsync(cancelacion));

    /// <summary>Historial de movimientos del sistema (RF-17).</summary>
    [HttpGet("movimientos")]
    [Authorize(Policy = Politicas.GestionInventario)]
    public async Task<ActionResult<PagedResult<MovimientoDto>>> Movimientos([FromQuery] ConsultaMovimientos consulta, CancellationToken cancelacion)
        => Responder(await _servicio.ObtenerMovimientosAsync(consulta, cancelacion));
}
