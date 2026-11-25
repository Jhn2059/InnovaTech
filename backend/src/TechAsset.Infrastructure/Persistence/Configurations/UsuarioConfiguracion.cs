using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechAsset.Domain.Entities;

namespace TechAsset.Infrastructure.Persistence.Configurations;

/// <summary>Configuración de mapeo y restricciones de la tabla Usuarios.</summary>
public sealed class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("Id").ValueGeneratedNever();
        builder.Property(u => u.Nombres).HasColumnName("Nombres").HasMaxLength(100).IsRequired();
        builder.Property(u => u.Apellidos).HasColumnName("Apellidos").HasMaxLength(100).IsRequired();
        builder.Property(u => u.Correo).HasColumnName("Correo").HasMaxLength(150).IsRequired();
        builder.Property(u => u.Area).HasColumnName("Area").HasMaxLength(100).IsRequired();
        builder.Property(u => u.Rol).HasColumnName("Rol").HasConversion<int>().IsRequired();
        builder.Property(u => u.Estado).HasColumnName("Estado").IsRequired();
        builder.Property(u => u.HashContrasena).HasColumnName("HashContrasena").HasMaxLength(200);
        builder.Property(u => u.FechaDeshabilitacion).HasColumnName("FechaDeshabilitacion").HasColumnType("timestamp with time zone");
        builder.Property(u => u.MotivoDeshabilitacion).HasColumnName("MotivoDeshabilitacion").HasMaxLength(300);
        builder.Property(u => u.UltimoAcceso).HasColumnName("UltimoAcceso").HasColumnType("timestamp with time zone");
        builder.Property(u => u.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(u => u.FechaActualizacion).HasColumnName("FechaActualizacion").HasColumnType("timestamp with time zone").IsRequired();

        // RN-08 · El correo electrónico del usuario debe ser único.
        builder.HasIndex(u => u.Correo).IsUnique().HasDatabaseName("IX_Usuarios_Correo");
        builder.HasIndex(u => u.Area).HasDatabaseName("IX_Usuarios_Area");
        builder.HasIndex(u => u.Rol).HasDatabaseName("IX_Usuarios_Rol");

        builder.Ignore(u => u.EsNuevo);
    }
}
