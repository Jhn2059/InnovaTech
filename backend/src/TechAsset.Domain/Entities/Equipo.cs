using TechAsset.Domain.Enums;
using TechAsset.Domain.Exceptions;
using TechAsset.Domain.ValueObjects;

namespace TechAsset.Domain.Entities;

/// <summary>
/// Equipo tecnológico del inventario institucional (laptops, monitores, impresoras,
/// proyectores, tablets, desktops y equipos de red).
/// </summary>
public sealed class Equipo : EntidadBase
{
    private readonly List<Prestamo> _prestamos = new();

    /// <summary>Código patrimonial normalizado (mayúsculas) validado por el objeto valor <see cref="CodigoPatrimonial"/>.</summary>
    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public string Categoria { get; private set; } = string.Empty;
    public string Marca { get; private set; } = string.Empty;
    public string? NumeroSerie { get; private set; }
    public string? Ubicacion { get; private set; }
    public DateTime? FechaAdquisicion { get; private set; }
    public string? Especificaciones { get; private set; }
    public EstadoEquipo Estado { get; private set; } = EstadoEquipo.Disponible;
    public string? MotivoEstado { get; private set; }

    public IReadOnlyCollection<Prestamo> Prestamos => _prestamos.AsReadOnly();

    private Equipo() { }

    public Equipo(
        string codigoPatrimonial,
        string nombre,
        string categoria,
        string marca,
        string? numeroSerie,
        string? ubicacion,
        DateTime? fechaAdquisicion,
        string? especificaciones,
        DateTime ahora)
    {
        Codigo = CodigoPatrimonial.Crear(codigoPatrimonial).Valor;
        Nombre = Normalizar(nombre);
        Categoria = Normalizar(categoria);
        Marca = Normalizar(marca);
        NumeroSerie = NormalizarOpcional(numeroSerie);
        Ubicacion = NormalizarOpcional(ubicacion);
        FechaAdquisicion = fechaAdquisicion;
        Especificaciones = NormalizarOpcional(especificaciones);
        Estado = EstadoEquipo.Disponible;
        MarcarCreada(ahora);
    }

    /// <summary>Actualiza los datos descriptivos del equipo (RF-06).</summary>
    public void Actualizar(
        string nombre,
        string categoria,
        string marca,
        string? numeroSerie,
        string? ubicacion,
        DateTime? fechaAdquisicion,
        string? especificaciones,
        DateTime ahora)
    {
        Nombre = Normalizar(nombre);
        Categoria = Normalizar(categoria);
        Marca = Normalizar(marca);
        NumeroSerie = NormalizarOpcional(numeroSerie);
        Ubicacion = NormalizarOpcional(ubicacion);
        FechaAdquisicion = fechaAdquisicion;
        Especificaciones = NormalizarOpcional(especificaciones);
        MarcarActualizada(ahora);
    }

    /// <summary>Cambia el estado operativo del equipo (RF-08).</summary>
    public void CambiarEstado(EstadoEquipo nuevoEstado, string? motivo, DateTime ahora)
    {
        if (nuevoEstado == EstadoEquipo.Prestado)
        {
            throw new ReglaNegocioException(
                "RN-01",
                "El estado \"Prestado\" se asigna automáticamente al registrar o devolver un préstamo.");
        }

        Estado = nuevoEstado;
        MotivoEstado = NormalizarOpcional(motivo);
        MarcarActualizada(ahora);
    }

    /// <summary>
    /// Marca el equipo como prestado. Transición reservada al flujo de préstamos
    /// (registro de entrega y devolución), no a la edición manual del inventario.
    /// </summary>
    public void MarcarComoPrestado(DateTime ahora)
    {
        Estado = EstadoEquipo.Prestado;
        MotivoEstado = "Equipo entregado en préstamo";
        MarcarActualizada(ahora);
    }

    /// <summary>Indica si el equipo tiene historial de préstamos (RN-06).</summary>
    public bool TieneHistorial => _prestamos.Count > 0;

    public bool EstaDisponible => Estado == EstadoEquipo.Disponible;

    internal void AgregarPrestamo(Prestamo prestamo) => _prestamos.Add(prestamo);

    private static string Normalizar(string valor) => (valor ?? string.Empty).Trim();

    private static string? NormalizarOpcional(string? valor)
    {
        var limpio = valor?.Trim();
        return string.IsNullOrWhiteSpace(limpio) ? null : limpio;
    }
}
