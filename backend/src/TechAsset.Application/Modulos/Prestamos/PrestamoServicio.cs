using TechAsset.Application.Common;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Application.Common.Extensions;
using TechAsset.Application.Common.Mapping;
using TechAsset.Application.Common.Services;
using TechAsset.Application.Dtos;
using TechAsset.Domain.Entities;
using TechAsset.Domain.Enums;
using TechAsset.Domain.Exceptions;
using TechAsset.Domain.Rules;

namespace TechAsset.Application.Modulos.Prestamos;

/// <summary>Casos de uso del módulo de préstamos (RF-09 … RF-12).</summary>
public interface IPrestamoServicio
{
    Task<Result<PagedResult<PrestamoDto>>> ListarAsync(ConsultaPrestamos consulta, CancellationToken cancelacion = default);
    Task<Result<IReadOnlyList<PrestamoDto>>> ListarActivosAsync(CancellationToken cancelacion = default);
    Task<Result<PrestamoDto>> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default);
    Task<Result<PrestamoDto>> RegistrarAsync(RegistrarPrestamoSolicitud solicitud, CancellationToken cancelacion = default);
    Task<Result<PrestamoDto>> RegistrarDevolucionAsync(Guid id, RegistrarDevolucionSolicitud solicitud, CancellationToken cancelacion = default);
}

/// <inheritdoc cref="IPrestamoServicio" />
public sealed class PrestamoServicio : IPrestamoServicio
{
    private readonly IRepositorioPrestamo _prestamos;
    private readonly IRepositorioEquipo _equipos;
    private readonly IRepositorioUsuario _usuarios;
    private readonly IFechaActual _fecha;
    private readonly IUnitOfWork _unidadTrabajo;
    private readonly IMovimientoService _movimientos;
    private readonly IUsuarioActual _usuarioActual;

    public PrestamoServicio(
        IRepositorioPrestamo prestamos,
        IRepositorioEquipo equipos,
        IRepositorioUsuario usuarios,
        IFechaActual fecha,
        IUnitOfWork unidadTrabajo,
        IMovimientoService movimientos,
        IUsuarioActual usuarioActual)
    {
        _prestamos = prestamos;
        _equipos = equipos;
        _usuarios = usuarios;
        _fecha = fecha;
        _unidadTrabajo = unidadTrabajo;
        _movimientos = movimientos;
        _usuarioActual = usuarioActual;
    }

    public async Task<Result<PagedResult<PrestamoDto>>> ListarAsync(ConsultaPrestamos consulta, CancellationToken cancelacion = default)
    {
        var (items, total) = await _prestamos.ListarAsync(consulta, cancelacion);
        var dtos = items.Select(p => p.ADto(_fecha.Ahora)).ToList();
        return Result.Exito(new PagedResult<PrestamoDto>(dtos, total, consulta.Pagina, consulta.TamanoPagina));
    }

    public async Task<Result<IReadOnlyList<PrestamoDto>>> ListarActivosAsync(CancellationToken cancelacion = default)
    {
        var activos = await _prestamos.ListarActivosAsync(cancelacion);
        var dtos = activos.Select(p => p.ADto(_fecha.Ahora)).ToList();
        return Result.Exito<IReadOnlyList<PrestamoDto>>(dtos);
    }

    public async Task<Result<PrestamoDto>> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default)
    {
        var prestamo = await _prestamos.ObtenerPorIdAsync(id, cancelacion);
        if (prestamo is null)
        {
            return Result.Fallo<PrestamoDto>("NOT_FOUND", "El préstamo solicitado no existe.");
        }

        return Result.Exito(prestamo.ADto(_fecha.Ahora));
    }

    public async Task<Result<PrestamoDto>> RegistrarAsync(RegistrarPrestamoSolicitud solicitud, CancellationToken cancelacion = default)
    {
        var equipo = await _equipos.ObtenerPorIdAsync(solicitud.EquipoId, cancelacion);
        if (equipo is null)
        {
            return Result.Fallo<PrestamoDto>("NOT_FOUND", "El equipo seleccionado no existe.");
        }

        var usuario = await _usuarios.ObtenerPorIdAsync(solicitud.UsuarioId, cancelacion);
        if (usuario is null)
        {
            return Result.Fallo<PrestamoDto>("NOT_FOUND", "El colaborador seleccionado no existe.");
        }

        if (!usuario.Estado)
        {
            return Result.Fallo<PrestamoDto>(
                "USUARIO_DESHABILITADO",
                $"El usuario {usuario.NombreCompleto} está deshabilitado y no puede recibir equipos.");
        }

        // RN-05 · Todo préstamo debe registrar un responsable.
        ReglasNegocio.ValidarResponsableRegistrado(_usuarioActual.Id);

        // RN-02 / RN-03 · No se puede prestar un equipo en mantenimiento ni dado de baja.
        try
        {
            ReglasNegocio.ValidarEquipoNoEnMantenimiento(equipo);
            ReglasNegocio.ValidarEquipoNoEnBaja(equipo);
        }
        catch (ReglaNegocioException excepcion)
        {
            return Result.Fallo<PrestamoDto>(excepcion.Codigo, excepcion.Message);
        }

        // RN-01 · Un equipo solo puede estar asociado a un préstamo activo.
        var yaPrestado = await _prestamos.ExistePrestamoActivoParaEquipoAsync(equipo.Id, cancelacion);
        try
        {
            ReglasNegocio.ValidarEquipoSinPrestamoActivo(equipo, yaPrestado);
        }
        catch (ReglaNegocioException excepcion)
        {
            return Result.Fallo<PrestamoDto>(excepcion.Codigo, excepcion.Message);
        }

        var fechaPrestamo = (solicitud.FechaPrestamo ?? _fecha.Ahora).Date;
        if (fechaPrestamo > _fecha.Ahora.Date)
        {
            return Result.Fallo<PrestamoDto>("FECHA_FUTURA", "La fecha de préstamo no puede ser futura.");
        }

        var prestamo = new Prestamo(
            usuario.Id,
            equipo.Id,
            _usuarioActual.Id,
            fechaPrestamo,
            solicitud.Observacion,
            _fecha.Ahora);

        // RN-05 · El responsable que entrega el equipo queda registrado en el préstamo.
        var responsable = await _usuarios.ObtenerPorIdAsync(_usuarioActual.Id, cancelacion);
        if (responsable is null)
        {
            return Result.Fallo<PrestamoDto>("RESPONSABLE_NO_ENCONTRADO", "El responsable de la entrega no está registrado en el sistema.");
        }

        prestamo.VincularRelaciones(usuario, equipo, responsable);

        _prestamos.Agregar(prestamo);
        equipo.MarcarComoPrestado(_fecha.Ahora);
        _equipos.Actualizar(equipo);

        await _movimientos.RegistrarAsync(
            TipoMovimiento.Prestamo,
            $"Se prestó el equipo {equipo.Codigo} · {equipo.Nombre} a {usuario.NombreCompleto} ({usuario.Area}).",
            nameof(Prestamo),
            prestamo.Id,
            cancelacion);

        await _unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return Result.Exito(prestamo.ADto(_fecha.Ahora));
    }

    public async Task<Result<PrestamoDto>> RegistrarDevolucionAsync(Guid id, RegistrarDevolucionSolicitud solicitud, CancellationToken cancelacion = default)
    {
        var prestamo = await _prestamos.ObtenerPorIdAsync(id, cancelacion);
        if (prestamo is null)
        {
            return Result.Fallo<PrestamoDto>("NOT_FOUND", "El préstamo solicitado no existe.");
        }

        if (!prestamo.EstaActivo)
        {
            return Result.Fallo<PrestamoDto>("RN-10", "El préstamo ya fue devuelto anteriormente.");
        }

        var fechaDevolucion = (solicitud.FechaDevolucion ?? _fecha.Ahora).Date;

        // RN-04 · La fecha de devolución debe ser posterior a la fecha de préstamo.
        try
        {
            ReglasNegocio.ValidarFechasDePrestamo(prestamo.FechaPrestamo, fechaDevolucion);
        }
        catch (ReglaNegocioException excepcion)
        {
            return Result.Fallo<PrestamoDto>(excepcion.Codigo, excepcion.Message);
        }

        // RN-10 · Toda devolución deberá registrar observaciones.
        try
        {
            ReglasNegocio.ValidarObservacionEnDevolucion(solicitud.Observacion);
            prestamo.RegistrarDevolucion(fechaDevolucion, solicitud.Observacion, _fecha.Ahora);
        }
        catch (ReglaNegocioException excepcion)
        {
            return Result.Fallo<PrestamoDto>(excepcion.Codigo, excepcion.Message);
        }

        var equipo = await _equipos.ObtenerPorIdAsync(prestamo.EquipoId, cancelacion);
        if (equipo is not null)
        {
            equipo.CambiarEstado(EstadoEquipo.Disponible, "Equipo devuelto", _fecha.Ahora);
            _equipos.Actualizar(equipo);
        }

        _prestamos.Actualizar(prestamo);
        await _movimientos.RegistrarAsync(
            TipoMovimiento.Devolucion,
            $"Se registró la devolución del equipo {prestamo.Equipo?.Codigo ?? string.Empty} " +
            $"por {prestamo.Usuario?.NombreCompleto ?? string.Empty}. Observación: {solicitud.Observacion.Trim()}",
            nameof(Prestamo),
            prestamo.Id,
            cancelacion);

        await _unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return Result.Exito(prestamo.ADto(_fecha.Ahora));
    }
}
