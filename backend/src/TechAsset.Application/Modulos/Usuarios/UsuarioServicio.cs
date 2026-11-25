using TechAsset.Application.Common;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Application.Common.Extensions;
using TechAsset.Application.Common.Mapping;
using TechAsset.Application.Common.Services;
using TechAsset.Application.Dtos;
using TechAsset.Domain.Entities;
using TechAsset.Domain.Enums;
using TechAsset.Domain.Rules;

namespace TechAsset.Application.Modulos.Usuarios;

/// <summary>Casos de uso del módulo de usuarios (RF-01 … RF-04).</summary>
public interface IUsuarioServicio
{
    Task<Result<PagedResult<UsuarioDto>>> ListarAsync(ConsultaUsuarios consulta, CancellationToken cancelacion = default);
    Task<Result<UsuarioDto>> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default);
    Task<Result<UsuarioDto>> CrearAsync(CrearUsuarioSolicitud solicitud, CancellationToken cancelacion = default);
    Task<Result<UsuarioDto>> ActualizarAsync(Guid id, ActualizarUsuarioSolicitud solicitud, CancellationToken cancelacion = default);
    Task<Result<UsuarioDto>> CambiarEstadoAsync(Guid id, CambiarEstadoUsuarioSolicitud solicitud, CancellationToken cancelacion = default);
    Task<Result<IReadOnlyList<string>>> ObtenerAreasAsync(CancellationToken cancelacion = default);
}

/// <inheritdoc cref="IUsuarioServicio" />
public sealed class UsuarioServicio : IUsuarioServicio
{
    private readonly IRepositorioUsuario _usuarios;
    private readonly IHashContrasena _hash;
    private readonly IFechaActual _fecha;
    private readonly IUnitOfWork _unidadTrabajo;
    private readonly IMovimientoService _movimientos;
    private readonly IUsuarioActual _usuarioActual;

    public UsuarioServicio(
        IRepositorioUsuario usuarios,
        IHashContrasena hash,
        IFechaActual fecha,
        IUnitOfWork unidadTrabajo,
        IMovimientoService movimientos,
        IUsuarioActual usuarioActual)
    {
        _usuarios = usuarios;
        _hash = hash;
        _fecha = fecha;
        _unidadTrabajo = unidadTrabajo;
        _movimientos = movimientos;
        _usuarioActual = usuarioActual;
    }

    public async Task<Result<PagedResult<UsuarioDto>>> ListarAsync(ConsultaUsuarios consulta, CancellationToken cancelacion = default)
    {
        var (items, total) = await _usuarios.ListarAsync(consulta, cancelacion);
        var dtos = items.Select(u => u.ADto()).ToList();
        return Result.Exito(new PagedResult<UsuarioDto>(dtos, total, consulta.Pagina, consulta.TamanoPagina));
    }

    public async Task<Result<UsuarioDto>> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(id, cancelacion);
        if (usuario is null)
        {
            return Result.Fallo<UsuarioDto>("NOT_FOUND", "El usuario solicitado no existe.");
        }

        return Result.Exito(usuario.ADto());
    }

    public async Task<Result<UsuarioDto>> CrearAsync(CrearUsuarioSolicitud solicitud, CancellationToken cancelacion = default)
    {
        var correoNormalizado = solicitud.Correo.Trim().ToLowerInvariant();

        // RN-08 · El correo electrónico del usuario debe ser único.
        if (await _usuarios.ExisteCorreoAsync(correoNormalizado, null, cancelacion))
        {
            return Result.Fallo<UsuarioDto>("RN-08", $"Ya existe un usuario registrado con el correo \"{correoNormalizado}\".");
        }

        var usuario = new Usuario(
            solicitud.Nombres,
            solicitud.Apellidos,
            correoNormalizado,
            solicitud.Area,
            solicitud.Rol,
            _hash.Hash(solicitud.Contrasena),
            _fecha.Ahora);

        _usuarios.Agregar(usuario);
        await _movimientos.RegistrarAsync(
            TipoMovimiento.AltaUsuario,
            $"Se registró al usuario {usuario.NombreCompleto} ({usuario.Correo}) en el área {usuario.Area}.",
            nameof(Usuario),
            usuario.Id,
            cancelacion);

        await _unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return Result.Exito(usuario.ADto());
    }

    public async Task<Result<UsuarioDto>> ActualizarAsync(Guid id, ActualizarUsuarioSolicitud solicitud, CancellationToken cancelacion = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(id, cancelacion);
        if (usuario is null)
        {
            return Result.Fallo<UsuarioDto>("NOT_FOUND", "El usuario solicitado no existe.");
        }

        var correoNormalizado = solicitud.Correo.Trim().ToLowerInvariant();

        // RN-08 · El correo electrónico del usuario debe ser único.
        if (await _usuarios.ExisteCorreoAsync(correoNormalizado, id, cancelacion))
        {
            return Result.Fallo<UsuarioDto>("RN-08", $"Ya existe otro usuario registrado con el correo \"{correoNormalizado}\".");
        }

        usuario.Actualizar(
            solicitud.Nombres,
            solicitud.Apellidos,
            correoNormalizado,
            solicitud.Area,
            solicitud.Rol,
            _fecha.Ahora);

        _usuarios.Actualizar(usuario);
        await _movimientos.RegistrarAsync(
            TipoMovimiento.ActualizacionUsuario,
            $"Se actualizaron los datos del usuario {usuario.NombreCompleto}.",
            nameof(Usuario),
            usuario.Id,
            cancelacion);

        await _unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return Result.Exito(usuario.ADto());
    }

    public async Task<Result<UsuarioDto>> CambiarEstadoAsync(Guid id, CambiarEstadoUsuarioSolicitud solicitud, CancellationToken cancelacion = default)
    {
        // RN-09 · Solo el Administrador podrá deshabilitar usuarios.
        ReglasNegocio.ValidarPermisoDeshabilitar(_usuarioActual.EsAdministrador);

        var usuario = await _usuarios.ObtenerPorIdAsync(id, cancelacion);
        if (usuario is null)
        {
            return Result.Fallo<UsuarioDto>("NOT_FOUND", "El usuario solicitado no existe.");
        }

        if (solicitud.Habilitar)
        {
            usuario.Habilitar(_fecha.Ahora);
        }
        else
        {
            usuario.Deshabilitar(solicitud.Motivo ?? "Deshabilitado por el administrador", _fecha.Ahora);
        }

        _usuarios.Actualizar(usuario);
        await _movimientos.RegistrarAsync(
            TipoMovimiento.CambioEstadoUsuario,
            solicitud.Habilitar
                ? $"Se reactivó al usuario {usuario.NombreCompleto}."
                : $"Se deshabilitó al usuario {usuario.NombreCompleto}. Motivo: {usuario.MotivoDeshabilitacion}.",
            nameof(Usuario),
            usuario.Id,
            cancelacion);

        await _unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return Result.Exito(usuario.ADto());
    }

    public async Task<Result<IReadOnlyList<string>>> ObtenerAreasAsync(CancellationToken cancelacion = default)
    {
        var areas = await _usuarios.ObtenerAreasAsync(cancelacion);
        return Result.Exito<IReadOnlyList<string>>(areas);
    }
}
