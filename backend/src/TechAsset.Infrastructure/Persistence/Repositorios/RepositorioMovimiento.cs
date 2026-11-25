using Microsoft.EntityFrameworkCore;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Domain.Entities;

namespace TechAsset.Infrastructure.Persistence.Repositorios;

/// <summary>Repositorio de la bitácora de movimientos.</summary>
public sealed class RepositorioMovimiento : IRepositorioMovimiento
{
    private readonly AppDbContext _contexto;

    public RepositorioMovimiento(AppDbContext contexto) => _contexto = contexto;

    public async Task<(IReadOnlyList<Movimiento> Items, int Total)> ListarAsync(ConsultaMovimientos consulta, CancellationToken cancelacion = default)
    {
        var query = _contexto.Movimientos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(consulta.Tipo) &&
            Enum.TryParse<Domain.Entities.TipoMovimiento>(consulta.Tipo, true, out var tipo))
        {
            query = query.Where(m => m.Tipo == tipo);
        }

        if (consulta.UsuarioId is not null)
        {
            query = query.Where(m => m.UsuarioId == consulta.UsuarioId);
        }

        if (consulta.Desde is not null)
        {
            var desde = new DateTimeOffset(DateTime.SpecifyKind(consulta.Desde.Value, DateTimeKind.Utc));
            query = query.Where(m => m.Fecha >= desde);
        }

        if (consulta.Hasta is not null)
        {
            var hasta = new DateTimeOffset(DateTime.SpecifyKind(consulta.Hasta.Value, DateTimeKind.Utc));
            query = query.Where(m => m.Fecha <= hasta);
        }

        var busqueda = consulta.BusquedaNormalizada;
        if (busqueda is not null)
        {
            var patron = $"%{busqueda}%";
            query = query.Where(m =>
                EF.Functions.ILike(m.Descripcion, patron) ||
                (m.UsuarioNombre != null && EF.Functions.ILike(m.UsuarioNombre, patron)) ||
                EF.Functions.ILike(m.Entidad, patron));
        }

        var total = await query.CountAsync(cancelacion);

        query = consulta.Descendente
            ? query.OrderByDescending(m => m.Fecha)
            : query.OrderBy(m => m.Fecha);

        var items = await query
            .Skip(consulta.Saltar)
            .Take(consulta.TamanoPagina)
            .ToListAsync(cancelacion);

        return (items, total);
    }

    public void Agregar(Movimiento movimiento) => _contexto.Movimientos.Add(movimiento);
}
