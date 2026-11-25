using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechAsset.API.Seguridad;
using TechAsset.Application.Common;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Application.Dtos;
using TechAsset.Application.Modulos.Usuarios;

namespace TechAsset.API.Controllers;

/// <summary>
/// Gestión de usuarios (RF-01 … RF-04). El control de alta, modificación y
/// deshabilitación se restringe al Administrador (RN-09).
/// </summary>
[Route("api/usuarios")]
[Authorize]
public sealed class UsuariosController : ApiControllerBase
{
    private readonly IUsuarioServicio _servicio;

    public UsuariosController(IUsuarioServicio servicio) => _servicio = servicio;

    /// <summary>Consulta de usuarios con filtros, búsqueda y paginación (RF-03).</summary>
    [HttpGet]
    [Authorize(Policy = Politicas.GestionInventario)]
    public async Task<ActionResult<PagedResult<UsuarioDto>>> Listar([FromQuery] ConsultaUsuarios consulta, CancellationToken cancelacion)
        => Responder(await _servicio.ListarAsync(consulta, cancelacion));

    /// <summary>Obtiene un usuario por su identificador (RF-03).</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = Politicas.GestionInventario)]
    public async Task<ActionResult<UsuarioDto>> ObtenerPorId(Guid id, CancellationToken cancelacion)
        => Responder(await _servicio.ObtenerPorIdAsync(id, cancelacion));

    /// <summary>Registra un nuevo usuario (RF-01, RN-08).</summary>
    [HttpPost]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<UsuarioDto>> Crear([FromBody] CrearUsuarioSolicitud solicitud, CancellationToken cancelacion)
        => ResponderCreado(await _servicio.CrearAsync(solicitud, cancelacion), usuario => $"/api/usuarios/{usuario.Id}");

    /// <summary>Actualiza los datos de un usuario (RF-02, RN-08).</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<UsuarioDto>> Actualizar(Guid id, [FromBody] ActualizarUsuarioSolicitud solicitud, CancellationToken cancelacion)
        => Responder(await _servicio.ActualizarAsync(id, solicitud, cancelacion));

    /// <summary>Habilita o deshabilita un usuario (RF-04, RN-09).</summary>
    [HttpPatch("{id:guid}/estado")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<UsuarioDto>> CambiarEstado(Guid id, [FromBody] CambiarEstadoUsuarioSolicitud solicitud, CancellationToken cancelacion)
        => Responder(await _servicio.CambiarEstadoAsync(id, solicitud, cancelacion));

    /// <summary>Lista las áreas registradas, para los filtros de la interfaz.</summary>
    [HttpGet("areas")]
    [Authorize(Policy = Politicas.GestionInventario)]
    public async Task<ActionResult<IReadOnlyList<string>>> Areas(CancellationToken cancelacion)
        => Responder(await _servicio.ObtenerAreasAsync(cancelacion));
}
