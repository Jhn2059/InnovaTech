using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechAsset.API.Seguridad;
using TechAsset.Application.Common;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Application.Dtos;
using TechAsset.Application.Modulos.Equipos;

namespace TechAsset.API.Controllers;

/// <summary>
/// Gestión del inventario de equipos (RF-05 … RF-08). La consulta está abierta a
/// todos los usuarios autenticados (el colaborador consulta disponibilidad) y la
/// escritura se reserva al Administrador y al Responsable de Tecnología.
/// </summary>
[Route("api/equipos")]
[Authorize]
public sealed class EquiposController : ApiControllerBase
{
    private readonly IEquipoServicio _servicio;

    public EquiposController(IEquipoServicio servicio) => _servicio = servicio;

    /// <summary>Consulta del inventario con filtros, búsqueda y paginación (RF-07, RF-14).</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<EquipoDto>>> Listar([FromQuery] ConsultaEquipos consulta, CancellationToken cancelacion)
        => Responder(await _servicio.ListarAsync(consulta, cancelacion));

    /// <summary>Obtiene un equipo por su identificador (RF-07).</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EquipoDto>> ObtenerPorId(Guid id, CancellationToken cancelacion)
        => Responder(await _servicio.ObtenerPorIdAsync(id, cancelacion));

    /// <summary>Registra un equipo tecnológico (RF-05, RN-07).</summary>
    [HttpPost]
    [Authorize(Policy = Politicas.GestionInventario)]
    public async Task<ActionResult<EquipoDto>> Crear([FromBody] CrearEquipoSolicitud solicitud, CancellationToken cancelacion)
        => ResponderCreado(await _servicio.CrearAsync(solicitud, cancelacion), equipo => $"/api/equipos/{equipo.Id}");

    /// <summary>Actualiza los datos de un equipo (RF-06).</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = Politicas.GestionInventario)]
    public async Task<ActionResult<EquipoDto>> Actualizar(Guid id, [FromBody] ActualizarEquipoSolicitud solicitud, CancellationToken cancelacion)
        => Responder(await _servicio.ActualizarAsync(id, solicitud, cancelacion));

    /// <summary>Actualiza el estado operativo del equipo (RF-08).</summary>
    [HttpPatch("{id:guid}/estado")]
    [Authorize(Policy = Politicas.GestionInventario)]
    public async Task<ActionResult<EquipoDto>> CambiarEstado(Guid id, [FromBody] CambiarEstadoEquipoSolicitud solicitud, CancellationToken cancelacion)
        => Responder(await _servicio.CambiarEstadoAsync(id, solicitud, cancelacion));

    /// <summary>Elimina un equipo sin historial de préstamos (RN-06).</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult> Eliminar(Guid id, CancellationToken cancelacion)
        => Responder(await _servicio.EliminarAsync(id, cancelacion));

    /// <summary>Lista las categorías registradas, para los filtros de la interfaz.</summary>
    [HttpGet("categorias")]
    public async Task<ActionResult<IReadOnlyList<string>>> Categorias(CancellationToken cancelacion)
        => Responder(await _servicio.ObtenerCategoriasAsync(cancelacion));

    /// <summary>Lista las marcas registradas, para los filtros de la interfaz.</summary>
    [HttpGet("marcas")]
    public async Task<ActionResult<IReadOnlyList<string>>> Marcas(CancellationToken cancelacion)
        => Responder(await _servicio.ObtenerMarcasAsync(cancelacion));
}
