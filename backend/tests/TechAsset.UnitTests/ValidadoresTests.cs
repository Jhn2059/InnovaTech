using FluentAssertions;
using TechAsset.Application.Modulos.Equipos;
using TechAsset.Application.Modulos.Prestamos;
using TechAsset.Application.Modulos.Usuarios;
using TechAsset.Domain.Enums;
using Xunit;

namespace TechAsset.UnitTests;

/// <summary>
/// Pruebas de los validadores de entrada (validación de backend, Actividad 2).
/// Se verifica que cada dato obligatorio o inconsistente se rechace con un
/// mensaje comprensible en español.
/// </summary>
public class ValidadoresTests
{
    private readonly CrearUsuarioValidador _validadorUsuario = new();
    private readonly CrearEquipoValidador _validadorEquipo = new();
    private readonly RegistrarPrestamoValidador _validadorPrestamo = new();
    private readonly RegistrarDevolucionValidador _validadorDevolucion = new();

    [Fact]
    public void Usuario_ExigeTodosLosCamposObligatorios()
    {
        var resultado = _validadorUsuario.Validate(new CrearUsuarioSolicitud());

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Nombres");
        resultado.Errors.Should().Contain(e => e.PropertyName == "Apellidos");
        resultado.Errors.Should().Contain(e => e.PropertyName == "Correo");
        resultado.Errors.Should().Contain(e => e.PropertyName == "Area");
        resultado.Errors.Should().Contain(e => e.PropertyName == "Contrasena");
    }

    [Fact]
    public void Usuario_RechazaCorreoInvalido()
    {
        var resultado = _validadorUsuario.Validate(new CrearUsuarioSolicitud
        {
            Nombres = "María",
            Apellidos = "Quispe",
            Correo = "correo-invalido",
            Area = "TI",
            Rol = RolUsuario.Colaborador,
            Contrasena = "Secreto1234",
            ConfirmarContrasena = "Secreto1234"
        });

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Correo");
    }

    [Fact]
    public void Usuario_ExigeConfirmacionCoincidente()
    {
        var resultado = _validadorUsuario.Validate(new CrearUsuarioSolicitud
        {
            Nombres = "María",
            Apellidos = "Quispe",
            Correo = "maria@techasset.pe",
            Area = "TI",
            Rol = RolUsuario.Colaborador,
            Contrasena = "Secreto1234",
            ConfirmarContrasena = "OtraClave1234"
        });

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "ConfirmarContrasena");
    }

    [Fact]
    public void Usuario_AceptaUnRegistroValido()
    {
        var resultado = _validadorUsuario.Validate(new CrearUsuarioSolicitud
        {
            Nombres = "María",
            Apellidos = "Quispe Medina",
            Correo = "maria.quispe@techasset.pe",
            Area = "Contabilidad",
            Rol = RolUsuario.Colaborador,
            Contrasena = "Secreto1234",
            ConfirmarContrasena = "Secreto1234"
        });

        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Equipo_ExigeCodigoNombreCategoriaYMarca()
    {
        var resultado = _validadorEquipo.Validate(new CrearEquipoSolicitud());

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Select(e => e.PropertyName).Should().Contain(new[] { "CodigoPatrimonial", "Nombre", "Categoria", "Marca" });
    }

    [Fact]
    public void Equipo_RechazaCodigoConCaracteresNoPermitidos()
    {
        var resultado = _validadorEquipo.Validate(new CrearEquipoSolicitud
        {
            CodigoPatrimonial = "TECH LAP 001",
            Nombre = "ThinkPad",
            Categoria = "Laptop",
            Marca = "Lenovo"
        });

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "CodigoPatrimonial");
    }

    [Fact]
    public void Equipo_RechazaFechaDeAdquisicionFutura()
    {
        var resultado = _validadorEquipo.Validate(new CrearEquipoSolicitud
        {
            CodigoPatrimonial = "TECH-LAP-001",
            Nombre = "ThinkPad",
            Categoria = "Laptop",
            Marca = "Lenovo",
            FechaAdquisicion = DateTime.Now.AddDays(5)
        });

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "FechaAdquisicion");
    }

    [Fact]
    public void Prestamo_ExigeColaboradorEquipoYFechaValida()
    {
        var resultado = _validadorPrestamo.Validate(new RegistrarPrestamoSolicitud
        {
            FechaPrestamo = DateTime.Now.AddDays(3)
        });

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "UsuarioId");
        resultado.Errors.Should().Contain(e => e.PropertyName == "EquipoId");
        resultado.Errors.Should().Contain(e => e.PropertyName == "FechaPrestamo");
    }

    [Fact]
    public void Devolucion_ExigeObservacionesMinimas_RN10()
    {
        var resultado = _validadorDevolucion.Validate(new RegistrarDevolucionSolicitud
        {
            FechaDevolucion = DateTime.Now,
            Observacion = "ok"
        });

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Observacion");
    }
}
