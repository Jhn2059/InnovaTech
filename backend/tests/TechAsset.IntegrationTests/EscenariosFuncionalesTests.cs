using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TechAsset.IntegrationTests.Infrastructure;
using Xunit;

namespace TechAsset.IntegrationTests;

/// <summary>
/// Escenarios funcionales exigidos por el caso práctico (Actividad 6), ejecutados
/// contra la API real y PostgreSQL: registrar usuario, registrar equipo,
/// registrar préstamo, registrar devolución, consultar inventario, consultar
/// historial de préstamos y consultar el dashboard.
/// </summary>
public class EscenariosFuncionalesTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _api;

    public EscenariosFuncionalesTests(ApiFactory api)
    {
        _api = api;
        _api.ReiniciarDatosAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Escenario01_RegistrarUsuario()
    {
        var cliente = _api.CreateClient();
        var token = await ClienteApi.ObtenerTokenAsync(cliente, ApiFactory.CorreoAdministrador, ApiFactory.ContrasenaAdministrador);
        var autenticado = _api.CrearClienteAutenticado(token);

        var respuesta = await autenticado.PostAsJsonAsync("/api/usuarios", new
        {
            nombres = "María",
            apellidos = "Quispe Medina",
            correo = "maria.quispe@techasset.pe",
            area = "Contabilidad",
            rol = 3,
            contrasena = "Secreto1234",
            confirmarContrasena = "Secreto1234"
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await ClienteApi.LeerJsonAsync(respuesta);
        json.GetProperty("correo").GetString().Should().Be("maria.quispe@techasset.pe");
        json.GetProperty("rolNombre").GetString().Should().Be("Colaborador");
    }

    [Fact]
    public async Task Escenario02_RegistrarEquipo()
    {
        var cliente = _api.CreateClient();
        var token = await ClienteApi.ObtenerTokenAsync(cliente, ApiFactory.CorreoAdministrador, ApiFactory.ContrasenaAdministrador);
        var autenticado = _api.CrearClienteAutenticado(token);

        var respuesta = await autenticado.PostAsJsonAsync("/api/equipos", new
        {
            codigoPatrimonial = "TECH-LAP-900",
            nombre = "ThinkPad T14 Gen 4",
            categoria = "Laptop",
            marca = "Lenovo",
            numeroSerie = "SN900001",
            ubicacion = "Almacén TI - Estante 3",
            especificaciones = "16 GB RAM · 512 GB SSD"
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await ClienteApi.LeerJsonAsync(respuesta);
        json.GetProperty("codigoPatrimonial").GetString().Should().Be("TECH-LAP-900");
        json.GetProperty("estadoNombre").GetString().Should().Be("Disponible");
    }

    [Fact]
    public async Task Escenario03_RegistrarPrestamo()
    {
        var contexto = await PrepararEscenarioAsync();

        var respuesta = await contexto.Administrador.PostAsJsonAsync("/api/prestamos", new
        {
            usuarioId = contexto.UsuarioId,
            equipoId = contexto.EquipoId,
            observacion = "Préstamo para el proyecto de implementación"
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await ClienteApi.LeerJsonAsync(respuesta);
        json.GetProperty("estadoNombre").GetString().Should().Be("Activo");
        json.GetProperty("responsableNombre").GetString().Should().NotBeNullOrWhiteSpace();
        json.GetProperty("usuarioNombre").GetString().Should().Be("Carlos Mendoza Ruiz");
    }

    [Fact]
    public async Task Escenario04_RegistrarDevolucion()
    {
        var contexto = await PrepararEscenarioAsync();

        var prestamo = await contexto.Administrador.PostAsJsonAsync("/api/prestamos", new
        {
            usuarioId = contexto.UsuarioId,
            equipoId = contexto.EquipoId,
            fechaPrestamo = DateTime.UtcNow.Date.AddDays(-10)
        });
        var prestamoId = (await ClienteApi.LeerJsonAsync(prestamo)).GetProperty("id").GetString();

        var respuesta = await contexto.Administrador.PostAsJsonAsync($"/api/prestamos/{prestamoId}/devolucion", new
        {
            fechaDevolucion = DateTime.UtcNow,
            observacion = "Equipo devuelto en perfecto estado con todos sus accesorios"
        });

        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await ClienteApi.LeerJsonAsync(respuesta);
        json.GetProperty("estadoNombre").GetString().Should().Be("Devuelto");

        // El equipo vuelve a estar disponible (RN-01).
        var equipos = await contexto.Administrador.GetAsync($"/api/equipos?busqueda={contexto.EquipoCodigo}");
        var equipo = (await ClienteApi.LeerJsonAsync(equipos)).GetProperty("items")[0];
        equipo.GetProperty("estadoNombre").GetString().Should().Be("Disponible");
    }

    [Fact]
    public async Task Escenario05_ConsultarInventario()
    {
        var contexto = await PrepararEscenarioAsync();

        var respuesta = await contexto.Administrador.GetAsync("/api/equipos?pagina=1&tamanoPagina=10");

        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await ClienteApi.LeerJsonAsync(respuesta);
        json.GetProperty("totalRegistros").GetInt32().Should().BeGreaterThan(0);
        json.GetProperty("items").GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Escenario06_ConsultarHistorialDePrestamos()
    {
        var contexto = await PrepararEscenarioAsync();
        await contexto.Administrador.PostAsJsonAsync("/api/prestamos", new { usuarioId = contexto.UsuarioId, equipoId = contexto.EquipoId });

        var respuesta = await contexto.Administrador.GetAsync("/api/prestamos");

        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await ClienteApi.LeerJsonAsync(respuesta);
        json.GetProperty("totalRegistros").GetInt32().Should().BeGreaterThan(0);
        json.GetProperty("items")[0].GetProperty("equipoNombre").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Escenario07_ConsultarDashboard()
    {
        var contexto = await PrepararEscenarioAsync();
        await contexto.Administrador.PostAsJsonAsync("/api/prestamos", new { usuarioId = contexto.UsuarioId, equipoId = contexto.EquipoId });

        var respuesta = await contexto.Administrador.GetAsync("/api/reportes/dashboard");

        respuesta.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await ClienteApi.LeerJsonAsync(respuesta);

        // Indicadores mínimos exigidos por el caso práctico (sección 10).
        json.GetProperty("totalEquipos").GetInt32().Should().BeGreaterThan(0);
        json.GetProperty("equiposDisponibles").GetInt32().Should().BeGreaterThanOrEqualTo(0);
        json.GetProperty("equiposPrestados").GetInt32().Should().BeGreaterThan(0);
        json.GetProperty("equiposMantenimiento").GetInt32().Should().BeGreaterThanOrEqualTo(0);
        json.GetProperty("totalPrestamos").GetInt32().Should().BeGreaterThan(0);
        json.GetProperty("areaConMayorUso").GetString().Should().Be("Ventas");
    }

    // ── Utilidades de preparación ──────────────────────────────────────────
    private sealed record Contexto(
        HttpClient Cliente,
        HttpClient Administrador,
        Guid UsuarioId,
        Guid EquipoId,
        string EquipoCodigo);

    private async Task<Contexto> PrepararEscenarioAsync()
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

        const string codigoEquipo = "TECH-LAP-950";
        await administrador.PostAsJsonAsync("/api/equipos", new
        {
            codigoPatrimonial = codigoEquipo,
            nombre = "Dell Latitude 5440",
            categoria = "Laptop",
            marca = "Dell"
        });

        var equipos = await administrador.GetAsync($"/api/equipos?busqueda={codigoEquipo}");
        var equipoId = Guid.Parse((await ClienteApi.LeerJsonAsync(equipos)).GetProperty("items")[0].GetProperty("id").GetString()!);

        var usuarios = await administrador.GetAsync("/api/usuarios?busqueda=carlos.mendoza");
        var usuarioId = Guid.Parse((await ClienteApi.LeerJsonAsync(usuarios)).GetProperty("items")[0].GetProperty("id").GetString()!);

        return new Contexto(cliente, administrador, usuarioId, equipoId, codigoEquipo);
    }
}
