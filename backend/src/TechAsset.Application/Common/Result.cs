namespace TechAsset.Application.Common;

/// <summary>
/// Error funcional de la aplicación. Se transporta sin excepciones para que la
/// capa API pueda mapearlo a una respuesta ProblemDetails predecible.
/// </summary>
public sealed class Error
{
    public Error(string codigo, string mensaje, IReadOnlyDictionary<string, string[]>? detalles = null)
    {
        Codigo = codigo;
        Mensaje = mensaje;
        Detalles = detalles;
    }

    /// <summary>Código funcional (RN-xx, VALOR, NO_ENCONTRADO…).</summary>
    public string Codigo { get; }

    /// <summary>Mensaje orientado al usuario final.</summary>
    public string Mensaje { get; }

    /// <summary>Errores por campo, cuando provienen de validaciones.</summary>
    public IReadOnlyDictionary<string, string[]>? Detalles { get; }

    public static Error Crear(string codigo, string mensaje) => new(codigo, mensaje);
}

/// <summary>Resultado de una operación que puede fallar por motivos controlados.</summary>
public class Result
{
    protected Result(bool esExitoso, Error? error)
    {
        EsExitoso = esExitoso;
        Error = error;
    }

    public bool EsExitoso { get; }
    public bool EsFallo => !EsExitoso;
    public Error? Error { get; }

    public static Result Exito() => new(true, null);

    public static Result Fallo(string codigo, string mensaje) => new(false, Error.Crear(codigo, mensaje));

    public static Result Fallo(Error error) => new(false, error);

    public static Result<T> Exito<T>(T valor) => new Result<T>(true, null, valor);

    public static Result<T> Fallo<T>(string codigo, string mensaje) => new Result<T>(false, Error.Crear(codigo, mensaje));

    public static Result<T> Fallo<T>(Error error) => new Result<T>(false, error);
}

/// <summary>Resultado genérico con valor de retorno.</summary>
public sealed class Result<T> : Result
{
    private readonly T? _valor;

    internal Result(bool esExitoso, Error? error, T? valor = default) : base(esExitoso, error)
    {
        _valor = valor;
    }

    public T Valor => EsExitoso
        ? _valor!
        : throw new InvalidOperationException("No se puede leer el valor de un resultado fallido.");

    public static implicit operator Result<T>(T valor) => Exito(valor);
}
