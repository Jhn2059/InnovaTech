using TechAsset.Domain.Entities;
using TechAsset.Domain.Enums;

namespace TechAsset.Application.Common.Extensions;

/// <summary>
/// Extensiones de presentation para los enums del dominio. Centraliza las
/// etiquetas visibles al usuario (regla de consistencia de Nielsen) y evita
/// literales dispersos entre capas.
/// </summary>
public static class EnumExtensions
{
    public static string Descripcion(this RolUsuario rol) => rol switch
    {
        RolUsuario.Administrador => "Administrador",
        RolUsuario.ResponsableTecnologia => "Responsable de Tecnología",
        RolUsuario.Colaborador => "Colaborador",
        _ => "Desconocido"
    };

    public static string Descripcion(this EstadoEquipo estado) => estado switch
    {
        EstadoEquipo.Disponible => "Disponible",
        EstadoEquipo.Prestado => "Prestado",
        EstadoEquipo.Mantenimiento => "Mantenimiento",
        EstadoEquipo.Baja => "Baja",
        _ => "Desconocido"
    };

    public static string Descripcion(this EstadoPrestamo estado) => estado switch
    {
        EstadoPrestamo.Activo => "Activo",
        EstadoPrestamo.Devuelto => "Devuelto",
        _ => "Desconocido"
    };

    public static string Descripcion(this TipoMovimiento tipo) => tipo switch
    {
        TipoMovimiento.AltaUsuario => "Alta de usuario",
        TipoMovimiento.ActualizacionUsuario => "Actualización de usuario",
        TipoMovimiento.CambioEstadoUsuario => "Cambio de estado de usuario",
        TipoMovimiento.AltaEquipo => "Alta de equipo",
        TipoMovimiento.ActualizacionEquipo => "Actualización de equipo",
        TipoMovimiento.CambioEstadoEquipo => "Cambio de estado de equipo",
        TipoMovimiento.Prestamo => "Préstamo",
        TipoMovimiento.Devolucion => "Devolución",
        _ => "Movimiento"
    };

    /// <summary>
    /// Convierte el texto recibido por el usuario en el enum correspondiente.
    /// Acepta las etiquetas visibles ("Responsable de Tecnología") y los nombres
    /// técnicos ("ResponsableTecnologia"), ignorando espacios y tildes.
    /// </summary>
    public static bool TryParseRol(string? texto, out RolUsuario rol)
    {
        rol = RolUsuario.Colaborador;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        var normalizado = Normalizar(texto);
        if (normalizado.Equals("administrador", StringComparison.OrdinalIgnoreCase))
        {
            rol = RolUsuario.Administrador;
            return true;
        }

        if (normalizado.Equals("responsabledetecnologia", StringComparison.OrdinalIgnoreCase))
        {
            rol = RolUsuario.ResponsableTecnologia;
            return true;
        }

        if (normalizado.Equals("colaborador", StringComparison.OrdinalIgnoreCase))
        {
            rol = RolUsuario.Colaborador;
            return true;
        }

        return false;
    }

    /// <summary>Quita espacios y tildes para comparaciones tolerantes.</summary>
    private static string Normalizar(string valor)
    {
        var sinEspacios = valor.Replace(" ", string.Empty, StringComparison.Ordinal);
        var descompuesto = sinEspacios.Normalize(System.Text.NormalizationForm.FormD);
        var resultado = new System.Text.StringBuilder(sinEspacios.Length);
        foreach (var caracter in descompuesto)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(caracter) !=
                System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                resultado.Append(caracter);
            }
        }

        return resultado.ToString().Normalize(System.Text.NormalizationForm.FormC);
    }

    /// <summary>Convierte el texto recibido por el usuario en el enum de estado de equipo.</summary>
    public static bool TryParseEstadoEquipo(string? texto, out EstadoEquipo estado) =>
        Enum.TryParse(texto, ignoreCase: true, out estado);

    /// <summary>Convierte el texto recibido por el usuario en el enum de estado de préstamo.</summary>
    public static bool TryParseEstadoPrestamo(string? texto, out EstadoPrestamo estado) =>
        Enum.TryParse(texto, ignoreCase: true, out estado);
}
