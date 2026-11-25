using TechAsset.Application.Common;

namespace TechAsset.API.Common;

/// <summary>
/// Base de los controladores: centraliza la traducción de <see cref="Result"/> a
/// respuestas HTTP coherentes (evita duplicación y garantiza respuestas uniformes).
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Traduce un resultado de aplicación a una respuesta HTTP.</summary>
    protected ActionResult<T> Responder<T>(Result<T> resultado)
    {
        if (resultado.EsExitoso)
        {
            return Ok(resultado.Valor);
        }

        return TraducirError<T>(resultado.Error!);
    }

    /// <summary>
    /// Traduce un resultado de creación a una respuesta 201 Created, indicando
    /// la ubicación del recurso recién creado (práctica REST y HTTPsemántica correcta).
    /// </summary>
    protected ActionResult<T> ResponderCreado<T>(Result<T> resultado, Func<T, string> ruta)
    {
        if (resultado.EsExitoso)
        {
            return Created(ruta(resultado.Valor), resultado.Valor);
        }

        return TraducirError<T>(resultado.Error!);
    }

    /// <summary>Traduce un resultado sin valor a una respuesta HTTP.</summary>
    protected ActionResult Responder(Result resultado)
    {
        if (resultado.EsExitoso)
        {
            return NoContent();
        }

        return TraducirError(resultado.Error!);
    }

    private ActionResult<T> TraducirError<T>(Error error) => StatusCode(DeterminarCodigoEstado(error.Codigo), ConstruirProblemDetails(error));

    private ActionResult TraducirError(Error error) => StatusCode(DeterminarCodigoEstado(error.Codigo), ConstruirProblemDetails(error));

    /// <summary>Construye la respuesta ProblemDetails con el código funcional del error.</summary>
    private ProblemDetails ConstruirProblemDetails(Error error)
    {
        var codigoEstado = DeterminarCodigoEstado(error.Codigo);

        var detalle = new ProblemDetails
        {
            Status = codigoEstado,
            Title = ObtenerTitulo(codigoEstado),
            Detail = error.Mensaje,
            Instance = HttpContext.Request.Path
        };

        detalle.Extensions["codigo"] = error.Codigo;
        detalle.Extensions["traceId"] = HttpContext.TraceIdentifier;

        if (error.Detalles is not null)
        {
            detalle.Extensions["errores"] = error.Detalles;
        }

        return detalle;
    }

    /// <summary>Determina el código HTTP según el código funcional del error.</summary>
    private static int DeterminarCodigoEstado(string codigo) => codigo switch
    {
        "NOT_FOUND" => StatusCodes.Status404NotFound,
        "FORBIDDEN" => StatusCodes.Status403Forbidden,
        "CREDENCIALES_INVALIDAS" => StatusCodes.Status401Unauthorized,
        "USUARIO_DESHABILITADO" => StatusCodes.Status403Forbidden,
        _ when codigo.StartsWith("RN-", StringComparison.Ordinal) => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status400BadRequest
    };

    private static string ObtenerTitulo(int codigoEstado) => codigoEstado switch
    {
        StatusCodes.Status400BadRequest => "Solicitud incorrecta",
        StatusCodes.Status401Unauthorized => "No autenticado",
        StatusCodes.Status403Forbidden => "Operación no permitida",
        StatusCodes.Status404NotFound => "Recurso no encontrado",
        StatusCodes.Status409Conflict => "Conflicto con las reglas del negocio",
        _ => "Error inesperado"
    };
}
