using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TechAsset.Application.Common.Services;
using TechAsset.Application.Modulos.Auth;
using TechAsset.Application.Modulos.Equipos;
using TechAsset.Application.Modulos.Prestamos;
using TechAsset.Application.Modulos.Reportes;
using TechAsset.Application.Modulos.Usuarios;

namespace TechAsset.Application;

/// <summary>
/// Registro de dependencias de la capa de aplicación. Un único punto de
/// configuración garantiza la coherencia entre módulos (consistencia y
/// mantenibilidad, ISO/IEC 25010).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection servicios)
    {
        servicios.AddValidatorsFromAssemblyContaining<CrearUsuarioValidador>();
        servicios.AddScoped<IUsuarioServicio, UsuarioServicio>();
        servicios.AddScoped<IEquipoServicio, EquipoServicio>();
        servicios.AddScoped<IPrestamoServicio, PrestamoServicio>();
        servicios.AddScoped<IReporteServicio, ReporteServicio>();
        servicios.AddScoped<IAuthServicio, AuthServicio>();
        servicios.AddScoped<IMovimientoService, MovimientoService>();

        return servicios;
    }
}
