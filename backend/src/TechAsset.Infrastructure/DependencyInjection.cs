using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Application.Common.Services;
using TechAsset.Infrastructure.Persistence;
using TechAsset.Infrastructure.Persistence.Repositorios;
using TechAsset.Infrastructure.Persistence.Sembrado;
using TechAsset.Infrastructure.Servicios;

namespace TechAsset.Infrastructure;

/// <summary>Registro de dependencias de la capa de infraestructura.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection servicios, IConfiguration configuracion)
    {
        // La cadena de conexión se resuelve de forma diferida (dentro del factory
        // de DbContext) para que la configuración pueda sobrescribirse en pruebas
        // o en otros entornos sin reconstruir el contenedor de dependencias.
        servicios.AddDbContext<AppDbContext>((proveedor, opciones) =>
        {
            var configuracionActual = proveedor.GetRequiredService<IConfiguration>();
            var cadenaConexion = configuracionActual.GetConnectionString("TechAsset")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'TechAsset'.");

            opciones.UseNpgsql(cadenaConexion, npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
        });

        servicios.Configure<JwtOpciones>(configuracion.GetSection(JwtOpciones.Seccion));

        servicios.AddScoped<IUnitOfWork, UnitOfWork>();
        servicios.AddScoped<IRepositorioUsuario, RepositorioUsuario>();
        servicios.AddScoped<IRepositorioEquipo, RepositorioEquipo>();
        servicios.AddScoped<IRepositorioPrestamo, RepositorioPrestamo>();
        servicios.AddScoped<IRepositorioMovimiento, RepositorioMovimiento>();
        servicios.AddScoped<IFechaActual, FechaActual>();
        servicios.AddScoped<IHashContrasena, HashContrasena>();
        servicios.AddScoped<IJwtTokenService, JwtTokenService>();
        servicios.AddScoped<IMovimientoService, MovimientoService>();

        return servicios;
    }

    /// <summary>
    /// Siembra datos de demostración cuando la base está vacía y la opción
    /// "SembrarDatos" está habilitada en la configuración.
    /// </summary>
    public static async Task SembrarDatosDemoAsync(this IServiceProvider proveedor, IConfiguration configuracion, CancellationToken cancelacion = default)
    {
        if (!configuracion.GetValue("SembrarDatos", false))
        {
            return;
        }

        using var alcance = proveedor.CreateScope();
        var contexto = alcance.ServiceProvider.GetRequiredService<AppDbContext>();
        var hash = alcance.ServiceProvider.GetRequiredService<IHashContrasena>();

        await contexto.Database.MigrateAsync(cancelacion);
        await SembradorDatos.SembrarAsync(contexto, hash, cancelacion);
    }
}
