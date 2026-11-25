using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechAsset.Domain.Entities;

namespace TechAsset.Infrastructure.Persistence.Configurations;

/// <summary>Configuración de mapeo y restricciones de la tabla Equipos.</summary>
public sealed class EquipoConfiguracion : IEntityTypeConfiguration<Equipo>
{
    public void Configure(EntityTypeBuilder<Equipo> builder)
    {
        builder.ToTable("Equipos");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
        builder.Property(e => e.Codigo).HasColumnName("CodigoPatrimonial").HasMaxLength(50).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("Nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Categoria).HasColumnName("Categoria").HasMaxLength(50).IsRequired();
        builder.Property(e => e.Marca).HasColumnName("Marca").HasMaxLength(50).IsRequired();
        builder.Property(e => e.NumeroSerie).HasColumnName("NumeroSerie").HasMaxLength(60);
        builder.Property(e => e.Ubicacion).HasColumnName("Ubicacion").HasMaxLength(120);
        builder.Property(e => e.FechaAdquisicion).HasColumnName("FechaAdquisicion").HasColumnType("date");
        builder.Property(e => e.Especificaciones).HasColumnName("Especificaciones").HasMaxLength(500);
        builder.Property(e => e.Estado).HasColumnName("Estado").HasConversion<int>().IsRequired();
        builder.Property(e => e.MotivoEstado).HasColumnName("MotivoEstado").HasMaxLength(300);
        builder.Property(e => e.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(e => e.FechaActualizacion).HasColumnName("FechaActualizacion").HasColumnType("timestamp with time zone").IsRequired();

        // RN-07 · El código patrimonial debe ser único.
        builder.HasIndex(e => e.Codigo).IsUnique().HasDatabaseName("IX_Equipos_CodigoPatrimonial");
        builder.HasIndex(e => e.Estado).HasDatabaseName("IX_Equipos_Estado");
        builder.HasIndex(e => e.Categoria).HasDatabaseName("IX_Equipos_Categoria");

        builder.Ignore(e => e.EsNuevo);
    }
}
