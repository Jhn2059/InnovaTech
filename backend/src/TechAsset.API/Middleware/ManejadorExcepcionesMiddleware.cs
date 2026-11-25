using System.Net;
using System.Text.Json;
using FluentValidation;
using TechAsset.Domain.Exceptions;

namespace TechAsset.API.Middleware;

/// <summary>
/// Middleware global de manejo de excepciones. Traduce los errores de dominio y
/// de validación a respuestas ProblemDetails (RFC 9457) con un código funcional
/// rastreable, garantizando mensajes comprensibles y consistentes.
/// </summary>
public sealed class ManejadorExcepcionesMiddleware
{
    private readonly RequestDelegate _siguiente;
    private readonly ILogger<ManejadorExcepcionesMiddleware> _logger;

    public ManejadorExcepcionesMiddleware(RequestDelegate siguiente, ILogger<ManejadorExcepcionesMiddleware> logger)
    {
        _siguiente = siguiente;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (ValidationException excepcion)
        {
            await EscribirProblemDetailsAsync(
                contexto,
                StatusCodes.Status400BadRequest,
                "Validación",
                "Los datos enviados no cumplen las reglas de validación.",
                new Dictionary<string, string[]>(excepcion.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),
                "VALIDACION");
        }
        catch (ReglaNegocioException excepcion)
        {
            _logger.LogWarning("Regla de negocio incumplida {Codigo}: {Mensaje}", excepcion.Codigo, excepcion.Message);
            await EscribirProblemDetailsAsync(
                contexto,
                StatusCodes.Status409Conflict,
                "Regla de negocio",
                excepcion.Message,
                null,
                excepcion.Codigo);
        }
        catch (ConflictoException excepcion)
        {
            _logger.LogWarning("Conflicto de unicidad {Codigo}: {Mensaje}", excepcion.Codigo, excepcion.Message);
            await EscribirProblemDetailsAsync(
                contexto,
                StatusCodes.Status409Conflict,
                "Conflicto de datos",
                excepcion.Message,
                null,
                excepcion.Codigo);
        }
        catch (OperacionNoPermitidaException excepcion)
        {
            await EscribirProblemDetailsAsync(
                contexto,
                StatusCodes.Status403Forbidden,
                "Operación no permitida",
                excepcion.Message,
                null,
                excepcion.Codigo);
        }
        catch (EntidadNoEncontradaException excepcion)
        {
            await EscribirProblemDetailsAsync(
                contexto,
                StatusCodes.Status404NotFound,
                "Recurso no encontrado",
                excepcion.Message,
                null,
                excepcion.Codigo);
        }
        catch (OperationCanceledException) when (contexto.RequestAborted.IsCancellationRequested)
        {
            // El cliente canceló la petición: no se registra como error.
        }
        catch (Exception excepcion)
        {
            _logger.LogError(excepcion, "Error no controlado al procesar {Ruta}", contexto.Request.Path);
            await EscribirProblemDetailsAsync(
                contexto,
                StatusCodes.Status500InternalServerError,
                "Error interno",
                "Se produjo un error inesperado al procesar la solicitud. Intente nuevamente o contacte al administrador.",
                null,
                "ERROR_INTERNO");
        }
    }

    private static async Task EscribirProblemDetailsAsync(
        HttpContext contexto,
        int codigoEstado,
        string titulo,
        string detalle,
        IDictionary<string, string[]>? errores,
        string codigo)
    {
        if (contexto.Response.HasStarted)
        {
            return;
        }

        contexto.Response.Clear();
        contexto.Response.StatusCode = codigoEstado;
        contexto.Response.ContentType = "application/problem+json";

        var payload = new Dictionary<string, object?>
        {
            ["type"] = $"https://techasset.pe/errores/{codigo}",
            ["title"] = titulo,
            ["status"] = codigoEstado,
            ["detail"] = detalle,
            ["instance"] = contexto.Request.Path.Value,
            ["codigo"] = codigo,
            ["traceId"] = contexto.TraceIdentifier
        };

        if (errores is not null)
        {
            payload["errors"] = errores;
        }

        await contexto.Response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
