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

namespace TechAsset.Application.Modulos.Equipos;

/// <summary>Casos de uso del módulo de equipos (RF-05 … RF-08).</summary>
public interface IEquipoServicio
{
    Task<Result<PagedResult<EquipoDto>>> ListarAsync(ConsultaEquipos consulta, CancellationToken cancelacion = default);
    Task<Result<EquipoDto>> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default);
    Task<Result<EquipoDto>> CrearAsync(CrearEquipoSolicitud solicitud, CancellationToken cancelacion = default);
    Task<Result<EquipoDto>> ActualizarAsync(Guid id, ActualizarEquipoSolicitud solicitud, CancellationToken cancelacion = default);
    Task<Result<EquipoDto>> CambiarEstadoAsync(Guid id, CambiarEstadoEquipoSolicitud solicitud, CancellationToken cancelacion = default);
    Task<Result> EliminarAsync(Guid id, CancellationToken cancelacion = default);
    Task<Result<IReadOnlyList<string>>> ObtenerCategoriasAsync(CancellationToken cancelacion = default);
    Task<Result<IReadOnlyList<string>>> ObtenerMarcasAsync(CancellationToken cancelacion = default);
}

/// <inheritdoc cref="IEquipoServicio" />
public sealed class EquipoServicio : IEquipoServicio
{
    private readonly IRepositorioEquipo _equipos;
    private readonly IRepositorioPrestamo _prestamos;
    private readonly IFechaActual _fecha;
    private readonly IUnitOfWork _unidadTrabajo;
    private readonly IMovimientoService _movimientos;

    public EquipoServicio(
        IRepositorioEquipo equipos,
        IRepositorioPrestamo prestamos,
        IFechaActual fecha,
        IUnitOfWork unidadTrabajo,
        IMovimientoService movimientos)
    {
        _equipos = equipos;
        _prestamos = prestamos;
        _fecha = fecha;
        _unidadTrabajo = unidadTrabajo;
        _movimientos = movimientos;
    }

    public async Task<Result<PagedResult<EquipoDto>>> ListarAsync(ConsultaEquipos consulta, CancellationToken cancelacion = default)
    {
        var (items, total) = await _equipos.ListarAsync(consulta, cancelacion);
        var dtos = items.Select(e => e.ADto()).ToList();
        return Result.Exito(new PagedResult<EquipoDto>(dtos, total, consulta.Pagina, consulta.TamanoPagina));
    }

    public async Task<Result<EquipoDto>> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default)
    {
        var equipo = await _equipos.ObtenerPorIdAsync(id, cancelacion);
        if (equipo is null)
        {
            return Result.Fallo<EquipoDto>("NOT_FOUND", "El equipo solicitado no existe.");
        }

        return Result.Exito(equipo.ADto());
    }

    public async Task<Result<EquipoDto>> CrearAsync(CrearEquipoSolicitud solicitud, CancellationToken cancelacion = default)
    {
        var codigo = solicitud.CodigoPatrimonial.Trim().ToUpperInvariant();

        // RN-07 · El código patrimonial debe ser único.
        if (await _equipos.ExisteCodigoAsync(codigo, null, cancelacion))
        {
            return Result.Fallo<EquipoDto>("RN-07", $"Ya existe un equipo registrado con el código patrimonial \"{codigo}\".");
        }

        var equipo = new Equipo(
            codigo,
            solicitud.Nombre,
            solicitud.Categoria,
            solicitud.Marca,
            solicitud.NumeroSerie,
            solicitud.Ubicacion,
            solicitud.FechaAdquisicion,
            solicitud.Especificaciones,
            _fecha.Ahora);

        _equipos.Agregar(equipo);
        await _movimientos.RegistrarAsync(
            TipoMovimiento.AltaEquipo,
            $"Se registró el equipo {equipo.Codigo} · {equipo.Nombre} ({equipo.Marca}).",
            nameof(Equipo),
            equipo.Id,
            cancelacion);

        await _unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return Result.Exito(equipo.ADto());
    }

    public async Task<Result<EquipoDto>> ActualizarAsync(Guid id, ActualizarEquipoSolicitud solicitud, CancellationToken cancelacion = default)
    {
        var equipo = await _equipos.ObtenerPorIdAsync(id, cancelacion);
        if (equipo is null)
        {
            return Result.Fallo<EquipoDto>("NOT_FOUND", "El equipo solicitado no existe.");
        }

        equipo.Actualizar(
            solicitud.Nombre,
            solicitud.Categoria,
            solicitud.Marca,
            solicitud.NumeroSerie,
            solicitud.Ubicacion,
            solicitud.FechaAdquisicion,
            solicitud.Especificaciones,
            _fecha.Ahora);

        _equipos.Actualizar(equipo);
        await _movimientos.RegistrarAsync(
            TipoMovimiento.ActualizacionEquipo,
            $"Se actualizaron los datos del equipo {equipo.Codigo} · {equipo.Nombre}.",
            nameof(Equipo),
            equipo.Id,
            cancelacion);

        await _unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return Result.Exito(equipo.ADto());
    }

    public async Task<Result<EquipoDto>> CambiarEstadoAsync(Guid id, CambiarEstadoEquipoSolicitud solicitud, CancellationToken cancelacion = default)
    {
        var equipo = await _equipos.ObtenerPorIdAsync(id, cancelacion);
        if (equipo is null)
        {
            return Result.Fallo<EquipoDto>("NOT_FOUND", "El equipo solicitado no existe.");
        }

        // Un equipo prestado solo puede volver a "Disponible" mediante la devolución del préstamo.
        if (equipo.Estado == EstadoEquipo.Prestado && solicitud.Estado != EstadoEquipo.Disponible)
        {
            return Result.Fallo<EquipoDto>(
                "RN-01",
                "El equipo tiene un préstamo activo. Registre la devolución para modificar su estado.");
        }

        var estadoAnterior = equipo.Estado;

        try
        {
            equipo.CambiarEstado(solicitud.Estado, solicitud.Motivo, _fecha.Ahora);
        }
        catch (ReglaNegocioException excepcion)
        {
            return Result.Fallo<EquipoDto>(excepcion.Codigo, excepcion.Message);
        }

        _equipos.Actualizar(equipo);
        await _movimientos.RegistrarAsync(
            TipoMovimiento.CambioEstadoEquipo,
            $"El equipo {equipo.Codigo} pasó de {estadoAnterior.Descripcion()} " +
            $"a {equipo.Estado.Descripcion()}. Motivo: {equipo.MotivoEstado ?? "sin observación"}.",
            nameof(Equipo),
            equipo.Id,
            cancelacion);

        await _unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return Result.Exito(equipo.ADto());
    }

    public async Task<Result> EliminarAsync(Guid id, CancellationToken cancelacion = default)
    {
        var equipo = await _equipos.ObtenerPorIdAsync(id, cancelacion);
        if (equipo is null)
        {
            return Result.Fallo("NOT_FOUND", "El equipo solicitado no existe.");
        }

        // RN-06 · No se puede eliminar un equipo con historial de préstamos.
        if (equipo.TieneHistorial)
        {
            return Result.Fallo(
                "RN-06",
                $"El equipo {equipo.Codigo} tiene historial de préstamos y no puede eliminarse. Debe pasar a estado \"Baja\".");
        }

        _equipos.Remover(equipo);
        await _movimientos.RegistrarAsync(
            TipoMovimiento.CambioEstadoEquipo,
            $"Se eliminó el equipo {equipo.Codigo} · {equipo.Nombre} (sin historial de préstamos).",
            nameof(Equipo),
            equipo.Id,
            cancelacion);

        await _unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return Result.Exito();
    }

    public async Task<Result<IReadOnlyList<string>>> ObtenerCategoriasAsync(CancellationToken cancelacion = default)
        => Result.Exito<IReadOnlyList<string>>(await _equipos.ObtenerCategoriasAsync(cancelacion));

    public async Task<Result<IReadOnlyList<string>>> ObtenerMarcasAsync(CancellationToken cancelacion = default)
        => Result.Exito<IReadOnlyList<string>>(await _equipos.ObtenerMarcasAsync(cancelacion));
}
