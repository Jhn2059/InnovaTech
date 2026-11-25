using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TechAsset.IntegrationTests.Infrastructure;
using Xunit;

namespace TechAsset.IntegrationTests;

/// <summary>
/// Verificación de las reglas de negocio (RN-01 … RN-10) y de los permisos por
/// rol sobre la API expuesta: la capa de aplicación y la base de datos deben
/// rechazar las operaciones no permitidas con el código HTTP correspondiente.
/// </summary>
public class ReglasDeNegocioTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _api;

    public ReglasDeNegocioTests(ApiFactory api)
    {
        _api = api;
        _api.ReiniciarDatosAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task RN01_UnEquipoConPrestamoActivo_NoPuedePrestarseDeNuevo()
    {
        var contexto = await PrepararAsync();
        await contexto.Administrador.PostAsJsonAsync("/api/prestamos", new { usuarioId = contexto.UsuarioId, equipoId = contexto.EquipoId });

        var respuesta = await contexto.Administrador.PostAsJsonAsync("/api/prestamos", new { usuarioId = contexto.UsuarioId, equipoId = contexto.EquipoId });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await ClienteApi.LeerCodigoAsync(respuesta)).Should().Be("RN-01");
    }

    [Fact]
    public async Task RN02_UnEquipoEnMantenimiento_NoPuedePrestarse()
    {
        var contexto = await PrepararAsync();
        await contexto.Administrador.PatchAsJsonAsync($"/api/equipos/{contexto.EquipoId}/estado", new
        {
            estado = 3,
            motivo = "Revisión técnica programada"
        });

        var respuesta = await contexto.Administrador.PostAsJsonAsync("/api/prestamos", new { usuarioId = contexto.UsuarioId, equipoId = contexto.EquipoId });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await ClienteApi.LeerCodigoAsync(respuesta)).Should().Be("RN-02");
    }

    [Fact]
    public async Task RN03_UnEquipoEnBaja_NoPuedePrestarse()
    {
        var contexto = await PrepararAsync();
        await contexto.Administrador.PatchAsJsonAsync($"/api/equipos/{contexto.EquipoId}/estado", new
        {
            estado = 4,
            motivo = "Obsolescencia del equipo"
        });

        var respuesta = await contexto.Administrador.PostAsJsonAsync("/api/prestamos", new { usuarioId = contexto.UsuarioId, equipoId = contexto.EquipoId });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await ClienteApi.LeerCodigoAsync(respuesta)).Should().Be("RN-03");
    }

    [Fact]
    public async Task RN04_LaDevolucionDebeSerPosteriorAlPrestamo()
    {
        var contexto = await PrepararAsync();
        var prestamo = await contexto.Administrador.PostAsJsonAsync("/api/prestamos", new
        {
            usuarioId = contexto.UsuarioId,
            equipoId = contexto.EquipoId,
            fechaPrestamo = DateTime.UtcNow.Date
        });
        var prestamoId = (await ClienteApi.LeerJsonAsync(prestamo)).GetProperty("id").GetString();

        var respuesta = await contexto.Administrador.PostAsJsonAsync($"/api/prestamos/{prestamoId}/devolucion", new
        {
            fechaDevolucion = DateTime.UtcNow.Date.AddDays(-5),
            observacion = "Intento de devolución con fecha anterior"
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await ClienteApi.LeerCodigoAsync(respuesta)).Should().Be("RN-04");
    }

    [Fact]
    public async Task RN05_TodoPrestamoRegistraElResponsable()
    {
        var contexto = await PrepararAsync();

        var prestamo = await contexto.Administrador.PostAsJsonAsync("/api/prestamos", new
        {
            usuarioId = contexto.UsuarioId,
            equipoId = contexto.EquipoId
        });

        var json = await ClienteApi.LeerJsonAsync(prestamo);
        json.GetProperty("responsableId").GetString().Should().NotBeNullOrWhiteSpace();
        json.GetProperty("responsableNombre").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task RN06_UnEquipoConHistorialNoPuedeEliminarse()
    {
        var contexto = await PrepararAsync();
        await contexto.Administrador.PostAsJsonAsync("/api/prestamos", new { usuarioId = contexto.UsuarioId, equipoId = contexto.EquipoId });

        var respuesta = await contexto.Administrador.DeleteAsync($"/api/equipos/{contexto.EquipoId}");

        respuesta.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await ClienteApi.LeerCodigoAsync(respuesta)).Should().Be("RN-06");
    }

    [Fact]
    public async Task RN06_UnEquipoSinHistorialPuedeEliminarse()
    {
        var contexto = await PrepararAsync();

        var respuesta = await contexto.Administrador.DeleteAsync($"/api/equipos/{contexto.EquipoId}");

        respuesta.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RN07_ElCodigoPatrimonialDebeSerUnico()
    {
        var contexto = await PrepararAsync();

        var respuesta = await contexto.Administrador.PostAsJsonAsync("/api/equipos", new
        {
            codigoPatrimonial = contexto.EquipoCodigo,
            nombre = "Equipo duplicado",
            categoria = "Laptop",
            marca = "Lenovo"
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await ClienteApi.LeerCodigoAsync(respuesta)).Should().Be("RN-07");
    }

    [Fact]
    public async Task RN08_ElCorreoDebeSerUnico()
    {
        var contexto = await PrepararAsync();

        var respuesta = await contexto.Administrador.PostAsJsonAsync("/api/usuarios", new
        {
            nombres = "Otro",
            apellidos = "Colaborador",
            correo = "carlos.mendoza@techasset.pe",
            area = "Ventas",
            rol = 3,
            contrasena = "Secreto1234",
            confirmarContrasena = "Secreto1234"
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await ClienteApi.LeerCodigoAsync(respuesta)).Should().Be("RN-08");
    }

    [Fact]
    public async Task RN09_SoloElAdministradorDeshabilitaUsuarios()
    {
        var contexto = await PrepararAsync();

        var tokenColaborador = await ClienteApi.ObtenerTokenAsync(contexto.Cliente, "carlos.mendoza@techasset.pe", "Secreto1234");
        var colaborador = _api.CrearClienteAutenticado(tokenColaborador);

        var respuesta = await colaborador.PatchAsJsonAsync($"/api/usuarios/{contexto.UsuarioId}/estado", new
        {
            habilitar = false,
            motivo = "Prueba de permisos"
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // El administrador sí puede deshabilitar (RN-09).
        var respuestaAdmin = await contexto.Administrador.PatchAsJsonAsync($"/api/usuarios/{contexto.UsuarioId}/estado", new
        {
            habilitar = false,
            motivo = "Cierre de cuenta"
        });

        respuestaAdmin.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ClienteApi.LeerJsonAsync(respuestaAdmin)).GetProperty("estado").GetBoolean().Should().BeFalse();
    }

    [Fact]
    public async Task RN10_TodaDevolucionRegistraObservaciones()
    {
        var contexto = await PrepararAsync();
        var prestamo = await contexto.Administrador.PostAsJsonAsync("/api/prestamos", new
        {
            usuarioId = contexto.UsuarioId,
            equipoId = contexto.EquipoId
        });
        var prestamoId = (await ClienteApi.LeerJsonAsync(prestamo)).GetProperty("id").GetString();

        var respuesta = await contexto.Administrador.PostAsJsonAsync($"/api/prestamos/{prestamoId}/devolucion", new
        {
            fechaDevolucion = DateTime.UtcNow,
            observacion = ""
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await ClienteApi.LeerCodigoAsync(respuesta)).Should().Be("VALIDACION");
    }

    [Fact]
    public async Task UnTokenInvalido_NoPermiteOperar()
    {
        var cliente = _api.CreateClient();
        var invalido = _api.CrearClienteAutenticado("token.invalido.firmado");

        var respuesta = await invalido.GetAsync("/api/reportes/dashboard");

        respuesta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ── Utilidades de preparación ──────────────────────────────────────────
    private sealed record Contexto(
        HttpClient Cliente,
        HttpClient Administrador,
        Guid UsuarioId,
        Guid EquipoId,
        string EquipoCodigo);

    private async Task<Contexto> PrepararAsync()
    {
        var cliente = _api.CreateClient();
        var token = await ClienteApi.ObtenerTokenAsync(cliente, ApiFactory.CorreoAdministrador, ApiFactory.ContrasenaAdministrador);
        var administrador = _api.CrearClienteAutenticado(token);

        await administrador.PostAsJsonAsync("/api/usuarios", new
        {
            nombres = "Carlos",
            apellidos = "Mendoza Ruiz",
            correo = "carlos.mendoza@techasset.pe",
            area = "Ventas",
            rol = 3,
            contrasena = "Secreto1234",
            confirmarContrasena = "Secreto1234"
        });

        const string codigoEquipo = "TECH-LAP-960";
        await administrador.PostAsJsonAsync("/api/equipos", new
        {
            codigoPatrimonial = codigoEquipo,
            nombre = "HP EliteBook 840",
            categoria = "Laptop",
            marca = "HP"
        });

        var equipos = await administrador.GetAsync($"/api/equipos?busqueda={codigoEquipo}");
        var equipoId = Guid.Parse((await ClienteApi.LeerJsonAsync(equipos)).GetProperty("items")[0].GetProperty("id").GetString()!);

        var usuarios = await administrador.GetAsync("/api/usuarios?busqueda=carlos.mendoza");
        var usuarioId = Guid.Parse((await ClienteApi.LeerJsonAsync(usuarios)).GetProperty("items")[0].GetProperty("id").GetString()!);

        return new Contexto(cliente, administrador, usuarioId, equipoId, codigoEquipo);
    }
}
