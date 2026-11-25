using TechAsset.Application.Common.Abstractions;
using TechAsset.Domain.Entities;

namespace TechAsset.Application.Common.Services;

/// <summary>
/// Servicio de bitácora. Centraliza el registro de auditoría para que ningún
/// caso de uso tenga que conocer la forma de persistir el movimiento (reutilización).
/// </summary>
public interface IMovimientoService
{
    Task RegistrarAsync(
        TipoMovimiento tipo,
        string descripcion,
        string entidad,
        Guid entidadId,
        CancellationToken cancelacion = default);
}

/// <inheritdoc cref="IMovimientoService" />
public sealed class MovimientoService : IMovimientoService
{
    private readonly IRepositorioMovimiento _repositorio;
    private readonly IUsuarioActual _usuarioActual;
    private readonly IFechaActual _fecha;

    public MovimientoService(
        IRepositorioMovimiento repositorio,
        IUsuarioActual usuarioActual,
        IFechaActual fecha)
    {
        _repositorio = repositorio;
        _usuarioActual = usuarioActual;
        _fecha = fecha;
    }

    public Task RegistrarAsync(
        TipoMovimiento tipo,
        string descripcion,
        string entidad,
        Guid entidadId,
        CancellationToken cancelacion = default)
    {
        var movimiento = new Movimiento(
            tipo,
            descripcion,
            entidad,
            entidadId,
            _usuarioActual.EstaAutenticado ? _usuarioActual.Id : null,
            _usuarioActual.EstaAutenticado ? _usuarioActual.Nombre : "Sistema",
            _fecha.Ahora);

        _repositorio.Agregar(movimiento);
        return Task.CompletedTask;
    }
}
