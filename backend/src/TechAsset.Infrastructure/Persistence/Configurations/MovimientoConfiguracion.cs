using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechAsset.Domain.Entities;

namespace TechAsset.Infrastructure.Persistence.Configurations;

/// <summary>Configuración de mapeo de la bitácora de movimientos (RF-17).</summary>
public sealed class MovimientoConfiguracion : IEntityTypeConfiguration<Movimiento>
{
    public void Configure(EntityTypeBuilder<Movimiento> builder)
    {
        builder.ToTable("Movimientos");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id).HasColumnName("Id").ValueGeneratedNever();
        builder.Property(m => m.Fecha).HasColumnName("Fecha").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(m => m.Tipo).HasColumnName("Tipo").HasConversion<int>().IsRequired();
        builder.Property(m => m.Descripcion).HasColumnName("Descripcion").HasMaxLength(500).IsRequired();
        builder.Property(m => m.Entidad).HasColumnName("Entidad").HasMaxLength(50).IsRequired();
        builder.Property(m => m.EntidadId).HasColumnName("EntidadId").IsRequired();
        builder.Property(m => m.UsuarioId).HasColumnName("UsuarioId");
        builder.Property(m => m.UsuarioNombre).HasColumnName("UsuarioNombre").HasMaxLength(150);
        builder.Property(m => m.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(m => m.FechaActualizacion).HasColumnName("FechaActualizacion").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasIndex(m => m.Fecha).IsDescending().HasDatabaseName("IX_Movimientos_Fecha");
        builder.HasIndex(m => m.Tipo).HasDatabaseName("IX_Movimientos_Tipo");

        builder.Ignore(m => m.EsNuevo);
    }
}
