using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechAsset.API.Seguridad;
using TechAsset.Application.Common;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Application.Dtos;
using TechAsset.Application.Modulos.Prestamos;

namespace TechAsset.API.Controllers;

/// <summary>
/// Gestión de préstamos y devoluciones (RF-09 … RF-12). El registro y la
/// devolución son actos del Responsable de Tecnología o del Administrador
/// (RN-05: todo préstamo registra responsable).
/// </summary>
[Route("api/prestamos")]
[Authorize]
public sealed class PrestamosController : ApiControllerBase
{
    private readonly IPrestamoServicio _servicio;

    public PrestamosController(IPrestamoServicio servicio) => _servicio = servicio;

    /// <summary>Consulta el historial de préstamos con filtros y paginación (RF-11).</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<PrestamoDto>>> Listar([FromQuery] ConsultaPrestamos consulta, CancellationToken cancelacion)
        => Responder(await _servicio.ListarAsync(consulta, cancelacion));

    /// <summary>Consulta los préstamos actualmente activos (RF-12).</summary>
    [HttpGet("activos")]
    public async Task<ActionResult<IReadOnlyList<PrestamoDto>>> Activos(CancellationToken cancelacion)
        => Responder(await _servicio.ListarActivosAsync(cancelacion));

    /// <summary>Obtiene el detalle de un préstamo (RF-11).</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PrestamoDto>> ObtenerPorId(Guid id, CancellationToken cancelacion)
        => Responder(await _servicio.ObtenerPorIdAsync(id, cancelacion));

    /// <summary>Registra el préstamo de un equipo a un colaborador (RF-09, RN-01 … RN-05).</summary>
    [HttpPost]
    [Authorize(Policy = Politicas.GestionInventario)]
    public async Task<ActionResult<PrestamoDto>> Registrar([FromBody] RegistrarPrestamoSolicitud solicitud, CancellationToken cancelacion)
        => ResponderCreado(await _servicio.RegistrarAsync(solicitud, cancelacion), prestamo => $"/api/prestamos/{prestamo.Id}");

    /// <summary>Registra la devolución de un equipo (RF-10, RN-04, RN-10).</summary>
    [HttpPost("{id:guid}/devolucion")]
    [Authorize(Policy = Politicas.GestionInventario)]
    public async Task<ActionResult<PrestamoDto>> RegistrarDevolucion(Guid id, [FromBody] RegistrarDevolucionSolicitud solicitud, CancellationToken cancelacion)
        => Responder(await _servicio.RegistrarDevolucionAsync(id, solicitud, cancelacion));
}
