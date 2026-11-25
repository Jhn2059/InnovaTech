using TechAsset.Domain.Entities;
using TechAsset.Domain.Enums;
using TechAsset.Domain.Exceptions;

namespace TechAsset.Domain.Rules;

/// <summary>
/// Catálogo ejecutable de reglas de negocio (RN-01 … RN-10) del caso práctico.
/// Cada regla vive en un único lugar y se referencia desde dominio, aplicación,
/// base de datos (restricciones) y pruebas, evitando duplicación de lógica.
/// </summary>
public static class ReglasNegocio
{
    // ── RN-01 · Un equipo solo puede estar asociado a un préstamo activo ─────────
    public static void ValidarEquipoSinPrestamoActivo(Equipo equipo, bool tienePrestamoActivo)
    {
        if (tienePrestamoActivo)
        {
            throw new ReglaNegocioException(
                "RN-01",
                $"El equipo {equipo.Codigo} ya tiene un préstamo activo. Debe registrar la devolución antes de volver a asignarlo.");
        }
    }

    // ── RN-02 · No se puede prestar un equipo en estado "Mantenimiento" ─────────
    public static void ValidarEquipoNoEnMantenimiento(Equipo equipo)
    {
        if (equipo.Estado == EstadoEquipo.Mantenimiento)
        {
            throw new ReglaNegocioException(
                "RN-02",
                $"El equipo {equipo.Codigo} está en mantenimiento y no puede ser prestado.");
        }
    }

    // ── RN-03 · No se puede prestar un equipo en estado "Baja" ─────────────────
    public static void ValidarEquipoNoEnBaja(Equipo equipo)
    {
        if (equipo.Estado == EstadoEquipo.Baja)
        {
            throw new ReglaNegocioException(
                "RN-03",
                $"El equipo {equipo.Codigo} está dado de baja y no puede ser prestado.");
        }
    }

    // ── RN-04 · La fecha de devolución debe ser posterior a la fecha de préstamo ─
    public static void ValidarFechasDePrestamo(DateTime fechaPrestamo, DateTime fechaDevolucion)
    {
        if (fechaDevolucion.Date <= fechaPrestamo.Date)
        {
            throw new ReglaNegocioException(
                "RN-04",
                "La fecha de devolución debe ser posterior a la fecha de préstamo.");
        }
    }

    // ── RN-05 · Todo préstamo debe registrar un responsable ────────────────────
    public static void ValidarResponsableRegistrado(Guid responsableId)
    {
        if (responsableId == Guid.Empty)
        {
            throw new ReglaNegocioException(
                "RN-05",
                "Todo préstamo debe registrar un responsable de la entrega.");
        }
    }

    // ── RN-06 · No se puede eliminar un equipo con historial de préstamos ───────
    public static void ValidarEquipoEliminable(Equipo equipo)
    {
        if (equipo.TieneHistorial)
        {
            throw new ReglaNegocioException(
                "RN-06",
                $"El equipo {equipo.Codigo} tiene historial de préstamos y no puede eliminarse. Debe pasar a estado \"Baja\".");
        }
    }

    // ── RN-07 · El código patrimonial debe ser único ───────────────────────────
    public static void ValidarCodigoPatrimonialUnico(string codigo, bool yaExiste)
    {
        if (yaExiste)
        {
            throw new ConflictoException(
                "RN-07",
                $"Ya existe un equipo registrado con el código patrimonial \"{codigo}\".");
        }
    }

    // ── RN-08 · El correo electrónico del usuario debe ser único ───────────────
    public static void ValidarCorreoUnico(string correo, bool yaExiste)
    {
        if (yaExiste)
        {
            throw new ConflictoException(
                "RN-08",
                $"Ya existe un usuario registrado con el correo \"{correo}\".");
        }
    }

    // ── RN-09 · Solo el Administrador podrá deshabilitar usuarios ──────────────
    public static void ValidarPermisoDeshabilitar(bool esAdministrador)
    {
        if (!esAdministrador)
        {
            throw new OperacionNoPermitidaException(
                "Solo el Administrador puede habilitar o deshabilitar usuarios (RN-09).");
        }
    }

    // ── RN-10 · Toda devolución deberá registrar observaciones ─────────────────
    public static void ValidarObservacionEnDevolucion(string? observacion)
    {
        if (string.IsNullOrWhiteSpace(observacion))
        {
            throw new ReglaNegocioException(
                "RN-10",
                "Toda devolución debe registrar observaciones.");
        }
    }
}
