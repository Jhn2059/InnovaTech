using Microsoft.EntityFrameworkCore;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Application.Common.Extensions;
using TechAsset.Application.Dtos;
using TechAsset.Domain.Entities;

namespace TechAsset.Infrastructure.Persistence.Repositorios;

/// <summary>Repositorio de equipos sobre EF Core.</summary>
public sealed class RepositorioEquipo : IRepositorioEquipo
{
    private readonly AppDbContext _contexto;

    public RepositorioEquipo(AppDbContext contexto) => _contexto = contexto;

    public Task<Equipo?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default) =>
        _contexto.Equipos
            .Include(e => e.Prestamos).ThenInclude(p => p.Usuario)
            .FirstOrDefaultAsync(e => e.Id == id, cancelacion);

    public Task<Equipo?> ObtenerPorCodigoAsync(string codigo, CancellationToken cancelacion = default) =>
        _contexto.Equipos.FirstOrDefaultAsync(e => e.Codigo == codigo, cancelacion);

    public Task<bool> ExisteCodigoAsync(string codigo, Guid? excluirId = null, CancellationToken cancelacion = default) =>
        _contexto.Equipos.AnyAsync(e => e.Codigo == codigo && (excluirId == null || e.Id != excluirId), cancelacion);

    public async Task<(IReadOnlyList<Equipo> Items, int Total)> ListarAsync(ConsultaEquipos consulta, CancellationToken cancelacion = default)
    {
        IQueryable<Equipo> query = _contexto.Equipos
            .AsNoTracking()
            .Include(e => e.Prestamos).ThenInclude(p => p.Usuario);

        if (!string.IsNullOrWhiteSpace(consulta.Estado) &&
            Application.Common.Extensions.EnumExtensions.TryParseEstadoEquipo(consulta.Estado, out var estado))
        {
            query = query.Where(e => e.Estado == estado);
        }

        if (!string.IsNullOrWhiteSpace(consulta.Categoria))
        {
            query = query.Where(e => e.Categoria == consulta.Categoria);
        }

        if (!string.IsNullOrWhiteSpace(consulta.Marca))
        {
            query = query.Where(e => e.Marca == consulta.Marca);
        }

        var busqueda = consulta.BusquedaNormalizada;
        if (busqueda is not null)
        {
            var patron = $"%{busqueda}%";
            query = query.Where(e =>
                EF.Functions.ILike(e.Codigo, patron) ||
                EF.Functions.ILike(e.Nombre, patron) ||
                EF.Functions.ILike(e.Marca, patron) ||
                EF.Functions.ILike(e.Categoria, patron) ||
                (e.Ubicacion != null && EF.Functions.ILike(e.Ubicacion, patron)));
        }

        var total = await query.CountAsync(cancelacion);

        query = consulta.OrdenarPor switch
        {
            "codigo" => consulta.Descendente ? query.OrderByDescending(e => e.Codigo) : query.OrderBy(e => e.Codigo),
            "nombre" => consulta.Descendente ? query.OrderByDescending(e => e.Nombre) : query.OrderBy(e => e.Nombre),
            "categoria" => consulta.Descendente ? query.OrderByDescending(e => e.Categoria) : query.OrderBy(e => e.Categoria),
            "marca" => consulta.Descendente ? query.OrderByDescending(e => e.Marca) : query.OrderBy(e => e.Marca),
            "estado" => consulta.Descendente ? query.OrderByDescending(e => e.Estado) : query.OrderBy(e => e.Estado),
            _ => consulta.Descendente ? query.OrderByDescending(e => e.FechaCreacion) : query.OrderBy(e => e.FechaCreacion)
        };

        var items = await query
            .Skip(consulta.Saltar)
            .Take(consulta.TamanoPagina)
            .ToListAsync(cancelacion);

        return (items, total);
    }

    public async Task<IReadOnlyList<string>> ObtenerCategoriasAsync(CancellationToken cancelacion = default) =>
        await _contexto.Equipos.AsNoTracking()
            .Select(e => e.Categoria)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancelacion);

    public async Task<IReadOnlyList<string>> ObtenerMarcasAsync(CancellationToken cancelacion = default) =>
        await _contexto.Equipos.AsNoTracking()
            .Select(e => e.Marca)
            .Distinct()
            .OrderBy(m => m)
            .ToListAsync(cancelacion);

    public async Task<IReadOnlyList<EquiposPorEstadoDto>> ObtenerEquiposPorEstadoAsync(CancellationToken cancelacion = default)
    {
        var filas = await _contexto.Equipos
            .AsNoTracking()
            .GroupBy(e => e.Estado)
            .Select(g => new { Estado = g.Key, Total = g.Count() })
            .ToListAsync(cancelacion);

        var totalGeneral = filas.Sum(f => f.Total);

        return filas
            .Select(f => new EquiposPorEstadoDto
            {
                Estado = f.Estado,
                EstadoNombre = f.Estado.Descripcion(),
                Total = f.Total,
                Porcentaje = totalGeneral == 0 ? 0 : Math.Round(f.Total * 100d / totalGeneral, 1)
            })
            .OrderBy(e => e.Estado)
            .ToList();
    }

    public void Agregar(Equipo equipo) => _contexto.Equipos.Add(equipo);

    public void Actualizar(Equipo equipo) => _contexto.Equipos.Update(equipo);

    public void Remover(Equipo equipo) => _contexto.Equipos.Remove(equipo);
}
