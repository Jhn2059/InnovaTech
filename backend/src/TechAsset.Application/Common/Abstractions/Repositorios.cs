using TechAsset.Application.Common;
using TechAsset.Application.Dtos;
using TechAsset.Domain.Entities;

namespace TechAsset.Application.Common.Abstractions;

/// <summary>Consultas paginadas de usuarios (RF-03).</summary>
public sealed class ConsultaUsuarios : ConsultaPaginada
{
    public string? Area { get; init; }
    public string? Rol { get; init; }
    public bool? SoloActivos { get; init; }
}

/// <summary>Consultas paginadas de equipos (RF-07 / RF-14).</summary>
public sealed class ConsultaEquipos : ConsultaPaginada
{
    public string? Estado { get; init; }
    public string? Categoria { get; init; }
    public string? Marca { get; init; }
}

/// <summary>Consultas paginadas de préstamos (RF-11 / RF-12).</summary>
public sealed class ConsultaPrestamos : ConsultaPaginada
{
    public Guid? UsuarioId { get; init; }
    public Guid? EquipoId { get; init; }
    public string? Area { get; init; }
    public string? Estado { get; init; }
    public DateTime? Desde { get; init; }
    public DateTime? Hasta { get; init; }
}

/// <summary>Consultas paginadas de movimientos históricos (RF-17).</summary>
public sealed class ConsultaMovimientos : ConsultaPaginada
{
    public string? Tipo { get; init; }
    public Guid? UsuarioId { get; init; }
    public DateTime? Desde { get; init; }
    public DateTime? Hasta { get; init; }
}

/// <summary>Repositorio de usuarios (contrato de persistencia, sin detalles de EF Core).</summary>
public interface IRepositorioUsuario
{
    Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default);
    Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancelacion = default);
    Task<bool> ExisteCorreoAsync(string correo, Guid? excluirId = null, CancellationToken cancelacion = default);
    Task<bool> CualquieraAsync(CancellationToken cancelacion = default);
    Task<(IReadOnlyList<Usuario> Items, int Total)> ListarAsync(ConsultaUsuarios consulta, CancellationToken cancelacion = default);
    Task<IReadOnlyList<string>> ObtenerAreasAsync(CancellationToken cancelacion = default);
    Task<(int Total, int Activos)> ObtenerTotalesAsync(CancellationToken cancelacion = default);
    void Agregar(Usuario usuario);
    void Actualizar(Usuario usuario);
    void Remover(Usuario usuario);
}

/// <summary>Repositorio de equipos (contrato de persistencia).</summary>
public interface IRepositorioEquipo
{
    Task<Equipo?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default);
    Task<Equipo?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancelacion = default);
    Task<bool> ExisteCodigoAsync(string codigo, Guid? excluirId = null, CancellationToken cancelacion = default);
    Task<(IReadOnlyList<Equipo> Items, int Total)> ListarAsync(ConsultaEquipos consulta, CancellationToken cancelacion = default);
    Task<IReadOnlyList<string>> ObtenerCategoriasAsync(CancellationToken cancelacion = default);
    Task<IReadOnlyList<string>> ObtenerMarcasAsync(CancellationToken cancelacion = default);
    Task<IReadOnlyList<EquiposPorEstadoDto>> ObtenerEquiposPorEstadoAsync(CancellationToken cancelacion = default);
    void Agregar(Equipo equipo);
    void Actualizar(Equipo equipo);
    void Remover(Equipo equipo);
}

/// <summary>Repositorio de préstamos (contrato de persistencia).</summary>
public interface IRepositorioPrestamo
{
    Task<Prestamo?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default);
    Task<bool> ExistePrestamoActivoParaEquipoAsync(Guid equipoId, CancellationToken cancelacion = default);
    Task<(IReadOnlyList<Prestamo> Items, int Total)> ListarAsync(ConsultaPrestamos consulta, CancellationToken cancelacion = default);
    Task<IReadOnlyList<Prestamo>> ListarActivosAsync(CancellationToken cancelacion = default);
    Task<IReadOnlyList<PrestamosPorAreaDto>> ObtenerPrestamosPorAreaAsync(CancellationToken cancelacion = default);
    Task<IReadOnlyList<EquipoMasUtilizadoDto>> ObtenerEquiposMasUtilizadosAsync(int tope, CancellationToken cancelacion = default);
    Task<IReadOnlyList<IndicadorDto>> ObtenerPrestamosPorMesAsync(int meses, CancellationToken cancelacion = default);
    void Agregar(Prestamo prestamo);
    void Actualizar(Prestamo prestamo);
}

/// <summary>Repositorio de movimientos (auditoría de acciones).</summary>
public interface IRepositorioMovimiento
{
    Task<(IReadOnlyList<Movimiento> Items, int Total)> ListarAsync(ConsultaMovimientos consulta, CancellationToken cancelacion = default);
    void Agregar(Movimiento movimiento);
}
