using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TechAsset.IntegrationTests.Infrastructure;

/// <summary>Utilidades compartidas por las pruebas de integración.</summary>
public static class ClienteApi
{
    public static readonly JsonSerializerOptions OpcionesJson = new(JsonSerializerDefaults.Web);

    /// <summary>Obtiene un token JWT para el usuario indicado.</summary>
    public static async Task<string> ObtenerTokenAsync(HttpClient cliente, string correo, string contrasena)
    {
        var respuesta = await cliente.PostAsJsonAsync("/api/auth/login", new { correo, contrasena }, OpcionesJson);
        respuesta.EnsureSuccessStatusCode();

        using var documento = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
        return documento.RootElement.GetProperty("token").GetString()!;
    }

    public static async Task<JsonElement> LeerJsonAsync(HttpResponseMessage respuesta)
    {
        var contenido = await respuesta.Content.ReadAsStringAsync();
        return JsonDocument.Parse(contenido).RootElement.Clone();
    }

    public static async Task<string> LeerCodigoAsync(HttpResponseMessage respuesta)
    {
        var json = await LeerJsonAsync(respuesta);
        return json.TryGetProperty("codigo", out var codigo) ? codigo.GetString() ?? string.Empty : string.Empty;
    }
}
