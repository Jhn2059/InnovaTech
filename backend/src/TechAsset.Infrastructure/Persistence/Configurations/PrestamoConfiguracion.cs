using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechAsset.Domain.Entities;

namespace TechAsset.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de mapeo y restricciones de la tabla Préstamos.
/// Las reglas RN-01 y RN-04 se replican como restricciones de base de datos
/// (defensa en profundidad frente a la capa de aplicación).
/// </summary>
public sealed class PrestamoConfiguracion : IEntityTypeConfiguration<Prestamo>
{
    public void Configure(EntityTypeBuilder<Prestamo> builder)
    {
        builder.ToTable("Prestamos");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("Id").ValueGeneratedNever();
        builder.Property(p => p.FechaPrestamo).HasColumnName("FechaPrestamo").HasColumnType("date").IsRequired();
        builder.Property(p => p.FechaDevolucion).HasColumnName("FechaDevolucion").HasColumnType("date");
        builder.Property(p => p.Observacion).HasColumnName("Observacion").HasMaxLength(500);
        builder.Property(p => p.ObservacionDevolucion).HasColumnName("ObservacionDevolucion").HasMaxLength(500);
        builder.Property(p => p.UsuarioId).HasColumnName("UsuarioId").IsRequired();
        builder.Property(p => p.EquipoId).HasColumnName("EquipoId").IsRequired();
        builder.Property(p => p.ResponsableId).HasColumnName("ResponsableId").IsRequired();
        builder.Property(p => p.Estado).HasColumnName("Estado").HasConversion<int>().IsRequired();
        builder.Property(p => p.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(p => p.FechaActualizacion).HasColumnName("FechaActualizacion").HasColumnType("timestamp with time zone").IsRequired();

        // RN-01 · Un equipo solo puede estar asociado a un préstamo activo.
        builder.HasIndex(p => p.EquipoId)
            .IsUnique()
            .HasFilter("\"FechaDevolucion\" IS NULL")
            .HasDatabaseName("IX_Prestamos_Equipo_Activo");

        // RN-04 · La fecha de devolución debe ser posterior a la fecha de préstamo.
        builder.ToTable("Prestamos", tabla =>
            tabla.HasCheckConstraint("CK_Prestamos_FechaDevolucion", "\"FechaDevolucion\" IS NULL OR \"FechaDevolucion\" > \"FechaPrestamo\""));

        builder.HasIndex(p => p.UsuarioId).HasDatabaseName("IX_Prestamos_Usuario");
        builder.HasIndex(p => p.FechaPrestamo).HasDatabaseName("IX_Prestamos_FechaPrestamo");
        builder.HasIndex(p => p.Estado).HasDatabaseName("IX_Prestamos_Estado");

        builder.HasOne(p => p.Usuario)
            .WithMany(u => u.PrestamosRecibidos)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Equipo)
            .WithMany(e => e.Prestamos)
            .HasForeignKey(p => p.EquipoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Responsable)
            .WithMany(u => u.PrestamosGestionados)
            .HasForeignKey(p => p.ResponsableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(p => p.EsNuevo);
    }
}
