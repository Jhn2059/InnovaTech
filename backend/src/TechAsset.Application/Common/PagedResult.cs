namespace TechAsset.Application.Common;

/// <summary>
/// Resultado paginado uniforme para todos los listados (evita duplicar la lógica
/// de paginación en cada módulo y favorece la reutilización).
/// </summary>
public sealed class PagedResult<T>
{
    public PagedResult(IReadOnlyList<T> items, int totalRegistros, int pagina, int tamanoPagina)
    {
        Items = items;
        TotalRegistros = totalRegistros;
        Pagina = pagina;
        TamanoPagina = tamanoPagina;
    }

    public IReadOnlyList<T> Items { get; }
    public int TotalRegistros { get; }
    public int Pagina { get; }
    public int TamanoPagina { get; }
    public int TotalPaginas => TamanoPagina <= 0 ? 0 : (int)Math.Ceiling(TotalRegistros / (double)TamanoPagina);
    public bool TienePaginaAnterior => Pagina > 1;
    public bool TienePaginaSiguiente => Pagina < TotalPaginas;

    public static PagedResult<T> Vacio(int pagina, int tamanoPagina) => new(Array.Empty<T>(), 0, pagina, tamanoPagina);
}

/// <summary>Parámetros comunes de consulta paginada, con normalización defensiva.</summary>
public abstract class ConsultaPaginada
{
    private int _pagina = 1;
    private int _tamanoPagina = 10;

    public int Pagina
    {
        get => _pagina;
        init => _pagina = value < 1 ? 1 : value;
    }

    public int TamanoPagina
    {
        get => _tamanoPagina;
        init => _tamanoPagina = value switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => value
        };
    }

    /// <summary>Término de búsqueda libre.</summary>
    public string? Busqueda { get; init; }

    /// <summary>Campo por el que se ordena el resultado.</summary>
    public string? OrdenarPor { get; init; }

    /// <summary>Indica si el ordenamiento es descendente.</summary>
    public bool Descendente { get; init; } = true;

    /// <summary>Devuelve la búsqueda normalizada (sin espacios sobrantes) o <c>null</c>.</summary>
    public string? BusquedaNormalizada =>
        string.IsNullOrWhiteSpace(Busqueda) ? null : Busqueda.Trim();

    public int Saltar => (Pagina - 1) * TamanoPagina;
}
