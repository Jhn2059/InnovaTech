using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TechAsset.API.Filtros;

/// <summary>
/// Filtro global que ejecuta los validadores de FluentValidation asociados al
/// modelo de la petición. Garantiza que ninguna operación de entrada llegue al
/// dominio sin haber sido validada (Actividad 2 del caso práctico).
/// </summary>
public sealed class ValidacionSolicitudFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _proveedor;

    public ValidacionSolicitudFilter(IServiceProvider proveedor) => _proveedor = proveedor;

    public async Task OnActionExecutionAsync(ActionExecutingContext contexto, ActionExecutionDelegate siguiente)
    {
        foreach (var argumento in contexto.ActionArguments.Values)
        {
            if (argumento is null) continue;

            var tipoValidador = typeof(IValidator<>).MakeGenericType(argumento.GetType());
            var validadores = _proveedor.GetServices(tipoValidador).Where(v => v is not null).ToList();
            if (validadores.Count == 0) continue;

            var contextoValidacion = new ValidationContext<object>(argumento);
            var resultados = await Task.WhenAll(validadores
                .Cast<IValidator>()
                .Select(v => v.ValidateAsync(contextoValidacion)));

            var errores = resultados
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .GroupBy(f => f.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());

            if (errores.Count > 0)
            {
                var modelState = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
                foreach (var error in errores)
                {
                    foreach (var mensaje in error.Value)
                    {
                        modelState.AddModelError(error.Key, mensaje);
                    }
                }

                contexto.Result = new BadRequestObjectResult(new ValidationProblemDetails(modelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Los datos enviados no son válidos",
                    Detail = "Revise los campos marcados e intente nuevamente.",
                    Instance = contexto.HttpContext.Request.Path,
                    Extensions =
                    {
                        ["codigo"] = "VALIDACION",
                        ["traceId"] = contexto.HttpContext.TraceIdentifier
                    }
                });

                return;
            }
        }

        await siguiente();
    }
}
