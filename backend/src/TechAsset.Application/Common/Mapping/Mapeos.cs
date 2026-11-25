using TechAsset.Application.Common.Extensions;
using TechAsset.Application.Dtos;
using TechAsset.Domain.Entities;

namespace TechAsset.Application.Common.Mapping;

/// <summary>
/// Mapeos explícitos entre entidades del dominio y DTOs de la aplicación.
/// Se prefieren mapeos manuales frente a reflexivos para conservar el control
/// de tipos, la trazabilidad y el rendimiento (ISO/IEC 25010).
/// </summary>
public static class Mapeos
{
    public static UsuarioDto ADto(this Usuario usuario) => new()
    {
        Id = usuario.Id,
        Nombres = usuario.Nombres,
        Apellidos = usuario.Apellidos,
        NombreCompleto = usuario.NombreCompleto,
        Correo = usuario.Correo,
        Area = usuario.Area,
        Rol = usuario.Rol,
        RolNombre = usuario.Rol.Descripcion(),
        Estado = usuario.Estado,
        FechaCreacion = usuario.FechaCreacion,
        FechaDeshabilitacion = usuario.FechaDeshabilitacion,
        MotivoDeshabilitacion = usuario.MotivoDeshabilitacion,
        UltimoAcceso = usuario.UltimoAcceso,
        TotalPrestamos = usuario.PrestamosRecibidos.Count
    };

    public static EquipoDto ADto(this Equipo equipo) => new()
    {
        Id = equipo.Id,
        CodigoPatrimonial = equipo.Codigo,
        Nombre = equipo.Nombre,
        Categoria = equipo.Categoria,
        Marca = equipo.Marca,
        NumeroSerie = equipo.NumeroSerie,
        Ubicacion = equipo.Ubicacion,
        FechaAdquisicion = equipo.FechaAdquisicion,
        Especificaciones = equipo.Especificaciones,
        Estado = equipo.Estado,
        EstadoNombre = equipo.Estado.Descripcion(),
        MotivoEstado = equipo.MotivoEstado,
        FechaCreacion = equipo.FechaCreacion,
        TienePrestamoActivo = equipo.Prestamos.Any(p => p.EstaActivo),
        UsuarioActual = equipo.Prestamos.FirstOrDefault(p => p.EstaActivo)?.Usuario?.NombreCompleto,
        UsuarioActualId = equipo.Prestamos.FirstOrDefault(p => p.EstaActivo)?.UsuarioId,
        FechaPrestamoActual = equipo.Prestamos.FirstOrDefault(p => p.EstaActivo)?.FechaPrestamo,
        TotalPrestamos = equipo.Prestamos.Count,
        PuedeEliminar = !equipo.TieneHistorial
    };

    public static PrestamoDto ADto(this Prestamo prestamo, DateTime referencia) => new()
    {
        Id = prestamo.Id,
        FechaPrestamo = prestamo.FechaPrestamo,
        FechaDevolucion = prestamo.FechaDevolucion,
        Observacion = prestamo.Observacion,
        ObservacionDevolucion = prestamo.ObservacionDevolucion,
        Estado = prestamo.Estado,
        EstadoNombre = prestamo.Estado.Descripcion(),
        DiasDePrestamo = prestamo.DiasDePrestamo(referencia),
        UsuarioId = prestamo.UsuarioId,
        UsuarioNombre = prestamo.Usuario?.NombreCompleto ?? "—",
        UsuarioArea = prestamo.Usuario?.Area ?? "—",
        EquipoId = prestamo.EquipoId,
        CodigoPatrimonial = prestamo.Equipo?.Codigo ?? "—",
        EquipoNombre = prestamo.Equipo?.Nombre ?? "—",
        EquipoCategoria = prestamo.Equipo?.Categoria ?? "—",
        ResponsableId = prestamo.ResponsableId,
        ResponsableNombre = prestamo.Responsable?.NombreCompleto ?? "—"
    };

    public static MovimientoDto ADto(this Movimiento movimiento) => new()
    {
        Id = movimiento.Id,
        Fecha = movimiento.Fecha,
        Tipo = movimiento.Tipo.ToString(),
        TipoNombre = movimiento.Tipo.Descripcion(),
        Descripcion = movimiento.Descripcion,
        Entidad = movimiento.Entidad,
        EntidadId = movimiento.EntidadId,
        UsuarioNombre = movimiento.UsuarioNombre
    };
}
