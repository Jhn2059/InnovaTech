using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechAsset.Application.Common;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Application.Common.Mapping;
using TechAsset.Application.Dtos;
using TechAsset.Application.Modulos.Auth;

namespace TechAsset.API.Controllers;

/// <summary>Autenticación de usuarios (acceso al sistema).</summary>
[Route("api/auth")]
public sealed class AuthController : ApiControllerBase
{
    private readonly IAuthServicio _servicio;

    public AuthController(IAuthServicio servicio) => _servicio = servicio;

    /// <summary>Inicia sesión y devuelve el token JWT junto con el perfil del usuario.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<RespuestaSesion>> IniciarSesion([FromBody] IniciarSesionSolicitud solicitud, CancellationToken cancelacion)
    {
        var resultado = await _servicio.IniciarSesionAsync(solicitud, cancelacion);
        return Responder(resultado);
    }

    /// <summary>Devuelve el perfil del usuario autenticado (verificación de sesión).</summary>
    [HttpGet("perfil")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> Perfil(CancellationToken cancelacion)
    {
        var usuarioActual = HttpContext.RequestServices.GetRequiredService<IUsuarioActual>();
        var repositorio = HttpContext.RequestServices.GetRequiredService<IRepositorioUsuario>();
        var usuario = await repositorio.ObtenerPorIdAsync(usuarioActual.Id, cancelacion);

        if (usuario is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Recurso no encontrado",
                Detail = "El usuario de la sesión ya no existe.",
                Instance = HttpContext.Request.Path
            });
        }

        return Ok(usuario.ADto());
    }
}
