using Microsoft.EntityFrameworkCore;
using TechAsset.Domain.Entities;
using TechAsset.Infrastructure.Persistence.Configurations;

namespace TechAsset.Infrastructure.Persistence;

/// <summary>
/// Contexto de datos de EF Core. Centraliza el modelo de persistencia y
/// traduce los objetos valor del dominio a columnas (conversores explícitos).
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opciones) : base(opciones) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<Prestamo> Prestamos => Set<Prestamo>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Los objetos valor (Email, CodigoPatrimonial) validan y normalizan el dato
        // en la construcción de la entidad; la columna almacena su forma canónica.
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Correo)
            .HasMaxLength(150);

        modelBuilder.Entity<Equipo>()
            .Property(e => e.Codigo)
            .HasMaxLength(50);
    }
}
