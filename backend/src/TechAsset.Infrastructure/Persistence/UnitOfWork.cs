using Microsoft.EntityFrameworkCore;
using TechAsset.Application.Common.Abstractions;

namespace TechAsset.Infrastructure.Persistence;

/// <summary>Implementación de la unidad de trabajo sobre EF Core.</summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _contexto;

    public UnitOfWork(AppDbContext contexto) => _contexto = contexto;

    public async Task<int> GuardarCambiosAsync(CancellationToken cancelacion = default) =>
        await _contexto.SaveChangesAsync(cancelacion);
}
