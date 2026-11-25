namespace TechAsset.Domain.Enums;

/// <summary>Rol del usuario dentro del sistema (actores del caso práctico).</summary>
public enum RolUsuario
{
    /// <summary>Administrador: gestiona usuarios, equipos, reportes y supervisión de préstamos.</summary>
    Administrador = 1,

    /// <summary>Responsable de Tecnología: registra equipos, aprueba préstamos y registra devoluciones.</summary>
    ResponsableTecnologia = 2,

    /// <summary>Colaborador: solicita equipos y consulta su historial y disponibilidad.</summary>
    Colaborador = 3
}

/// <summary>Estado operativo de un equipo tecnológico.</summary>
public enum EstadoEquipo
{
    /// <summary>Equipo listo para ser prestado (indicador "Equipos disponibles").</summary>
    Disponible = 1,

    /// <summary>Equipo asignado actualmente a un préstamo activo.</summary>
    Prestado = 2,

    /// <summary>Equipo fuera de servicio (indicador "Equipos en mantenimiento").</summary>
    Mantenimiento = 3,

    /// <summary>Equipo dado de baja; no puedeprestarse (RN-03).</summary>
    Baja = 4
}

/// <summary>Estado de un préstamo.</summary>
public enum EstadoPrestamo
{
    /// <summary>Préstamo vigente: el equipo está en poder del usuario.</summary>
    Activo = 1,

    /// <summary>Préstamo cerrado: el equipo fue devuelto.</summary>
    Devuelto = 2
}
