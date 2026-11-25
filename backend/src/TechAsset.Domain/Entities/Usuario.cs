using TechAsset.Domain.Enums;
using TechAsset.Domain.Exceptions;
using TechAsset.Domain.ValueObjects;

namespace TechAsset.Domain.Entities;

/// <summary>
/// Colaborador de Innovatech Solutions S.A.C. Puede solicitar y recibir equipos
/// tecnológicos y ser responsable de la entrega de los mismos.
/// </summary>
public sealed class Usuario : EntidadBase
{
    private readonly List<Prestamo> _prestamosRecibidos = new();
    private readonly List<Prestamo> _prestamosGestionados = new();

    public string Nombres { get; private set; } = string.Empty;
    public string Apellidos { get; private set; } = string.Empty;
    /// <summary>Correo normalizado (minúsculas) validado por el objeto valor <see cref="Email"/>.</summary>
    public string Correo { get; private set; } = string.Empty;
    public string Area { get; private set; } = string.Empty;
    public RolUsuario Rol { get; private set; }

    /// <summary>Estado del usuario: <c>true</c> activo, <c>false</c> deshabilitado (RF-04).</summary>
    public bool Estado { get; private set; } = true;

    public string? HashContrasena { get; private set; }
    public DateTime? FechaDeshabilitacion { get; private set; }
    public string? MotivoDeshabilitacion { get; private set; }
    public DateTime? UltimoAcceso { get; private set; }

    public IReadOnlyCollection<Prestamo> PrestamosRecibidos => _prestamosRecibidos.AsReadOnly();
    public IReadOnlyCollection<Prestamo> PrestamosGestionados => _prestamosGestionados.AsReadOnly();

    private Usuario() { }

    public Usuario(
        string nombres,
        string apellidos,
        string correo,
        string area,
        RolUsuario rol,
        string hashContrasena,
        DateTime ahora)
    {
        Nombres = Normalizar(nombres);
        Apellidos = Normalizar(apellidos);
        Correo = Email.Crear(correo).Valor;
        Area = Normalizar(area);
        Rol = rol;
        HashContrasena = hashContrasena;
        Estado = true;
        MarcarCreada(ahora);
    }

    /// <summary>Actualiza los datos editables del usuario (RF-02).</summary>
    public void Actualizar(string nombres, string apellidos, string correo, string area, RolUsuario rol, DateTime ahora)
    {
        Nombres = Normalizar(nombres);
        Apellidos = Normalizar(apellidos);
        Correo = Email.Crear(correo).Valor;
        Area = Normalizar(area);
        Rol = rol;
        MarcarActualizada(ahora);
    }

    /// <summary>Deshabilita el usuario (RF-04 / RN-09).</summary>
    public void Deshabilitar(string motivo, DateTime ahora)
    {
        if (!Estado)
        {
            throw new ReglaNegocioException("RN-09", "El usuario ya se encuentra deshabilitado.");
        }

        Estado = false;
        MotivoDeshabilitacion = Normalizar(motivo);
        FechaDeshabilitacion = ahora;
        MarcarActualizada(ahora);
    }

    /// <summary>Reactiva un usuario previamente deshabilitado.</summary>
    public void Habilitar(DateTime ahora)
    {
        Estado = true;
        MotivoDeshabilitacion = null;
        FechaDeshabilitacion = null;
        MarcarActualizada(ahora);
    }

    public void RegistrarAcceso(DateTime ahora) => UltimoAcceso = ahora;

    public bool EsAdministrador => Rol == RolUsuario.Administrador;

    /// <summary>Permisos de gestión de inventario y préstamos (actor "Responsable de Tecnología").</summary>
    public bool PuedeGestionarInventario => Rol is RolUsuario.Administrador or RolUsuario.ResponsableTecnologia;

    public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();

    internal void AgregarPrestamoRecibido(Prestamo prestamo) => _prestamosRecibidos.Add(prestamo);

    internal void AgregarPrestamoGestionado(Prestamo prestamo) => _prestamosGestionados.Add(prestamo);

    private static string Normalizar(string valor) => (valor ?? string.Empty).Trim();
}
