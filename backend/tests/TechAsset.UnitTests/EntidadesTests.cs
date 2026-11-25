using FluentAssertions;
using TechAsset.Domain.Entities;
using TechAsset.Domain.Enums;
using TechAsset.Domain.Exceptions;
using TechAsset.Domain.ValueObjects;
using Xunit;

namespace TechAsset.UnitTests;

/// <summary>Pruebas del comportamiento de las entidades del dominio.</summary>
public class EntidadesTests
{
    private static Usuario CrearUsuario(RolUsuario rol = RolUsuario.Colaborador) =>
        new("María", "Quispe Medina", "maria.quispe@techasset.pe", "Contabilidad", rol, "hash", DateTime.UtcNow);

    private static Equipo CrearEquipo() =>
        new("TECH-LAP-001", "ThinkPad T14", "Laptop", "Lenovo", "SN1", "Almacén TI", DateTime.UtcNow.AddDays(-100), null, DateTime.UtcNow);

    [Fact]
    public void Usuario_NormalizaElCorreoAMinusculas()
    {
        var usuario = new Usuario("María", "Quispe", "Maria.Quispe@TechAsset.PE", "TI", RolUsuario.Colaborador, "hash", DateTime.UtcNow);

        usuario.Correo.Should().Be("maria.quispe@techasset.pe");
    }

    [Fact]
    public void Usuario_Deshabilitar_RegistraMotivoYFecha()
    {
        var usuario = CrearUsuario();
        var momento = DateTime.UtcNow;

        usuario.Deshabilitar("Cierre de cuenta", momento);

        usuario.Estado.Should().BeFalse();
        usuario.MotivoDeshabilitacion.Should().Be("Cierre de cuenta");
        usuario.FechaDeshabilitacion.Should().Be(momento);
    }

    [Fact]
    public void Usuario_Deshabilitar_DosVeces_GeneraErrorDeRegla()
    {
        var usuario = CrearUsuario();
        usuario.Deshabilitar("Primera vez", DateTime.UtcNow);

        var excepcion = Assert.Throws<ReglaNegocioException>(() => usuario.Deshabilitar("Segunda vez", DateTime.UtcNow));
        excepcion.Codigo.Should().Be("RN-09");
    }

    [Fact]
    public void Usuario_Habilitar_RestauraElAcceso()
    {
        var usuario = CrearUsuario();
        usuario.Deshabilitar("Motivo", DateTime.UtcNow);

        usuario.Habilitar(DateTime.UtcNow);

        usuario.Estado.Should().BeTrue();
        usuario.MotivoDeshabilitacion.Should().BeNull();
    }

    [Fact]
    public void Usuario_Permisos_SegunElRol()
    {
        CrearUsuario(RolUsuario.Administrador).EsAdministrador.Should().BeTrue();
        CrearUsuario(RolUsuario.Administrador).PuedeGestionarInventario.Should().BeTrue();
        CrearUsuario(RolUsuario.ResponsableTecnologia).PuedeGestionarInventario.Should().BeTrue();
        CrearUsuario(RolUsuario.ResponsableTecnologia).EsAdministrador.Should().BeFalse();
        CrearUsuario(RolUsuario.Colaborador).PuedeGestionarInventario.Should().BeFalse();
    }

    [Fact]
    public void Equipo_CambiarEstado_AhPrestado_NoEsPermitidoManualmente()
    {
        var equipo = CrearEquipo();

        var excepcion = Assert.Throws<ReglaNegocioException>(() =>
            equipo.CambiarEstado(EstadoEquipo.Prestado, "Asignación manual", DateTime.UtcNow));

        excepcion.Codigo.Should().Be("RN-01");
    }

    [Fact]
    public void Equipo_MarcarComoPrestado_ActualizaElEstado()
    {
        var equipo = CrearEquipo();

        equipo.MarcarComoPrestado(DateTime.UtcNow);

        equipo.Estado.Should().Be(EstadoEquipo.Prestado);
        equipo.EstaDisponible.Should().BeFalse();
    }

    [Fact]
    public void Prestamo_RegistrarDevolucion_ActualizaEstadoYObservacion()
    {
        var prestamo = new Prestamo(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 11, 1), "Entrega", DateTime.UtcNow);

        prestamo.RegistrarDevolucion(new DateTime(2026, 11, 10), "Equipo devuelto sin daños", DateTime.UtcNow);

        prestamo.Estado.Should().Be(EstadoPrestamo.Devuelto);
        prestamo.EstaActivo.Should().BeFalse();
        prestamo.ObservacionDevolucion.Should().Be("Equipo devuelto sin daños");
        prestamo.DiasDePrestamo(DateTime.UtcNow).Should().Be(9);
    }

    [Fact]
    public void Prestamo_RegistrarDevolucionSinObservaciones_EsRechazada()
    {
        var prestamo = new Prestamo(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 11, 1), "Entrega", DateTime.UtcNow);

        var excepcion = Assert.Throws<ReglaNegocioException>(() =>
            prestamo.RegistrarDevolucion(new DateTime(2026, 11, 10), "   ", DateTime.UtcNow));

        excepcion.Codigo.Should().Be("RN-10");
    }

    [Fact]
    public void Prestamo_RegistrarDevolucionConFechaInvalida_EsRechazada()
    {
        var prestamo = new Prestamo(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 11, 10), "Entrega", DateTime.UtcNow);

        var excepcion = Assert.Throws<ReglaNegocioException>(() =>
            prestamo.RegistrarDevolucion(new DateTime(2026, 11, 1), "Equipo devuelto", DateTime.UtcNow));

        excepcion.Codigo.Should().Be("RN-04");
    }

    [Fact]
    public void Prestamo_DevolucionRepetida_EsRechazada()
    {
        var prestamo = new Prestamo(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 11, 1), "Entrega", DateTime.UtcNow);
        prestamo.RegistrarDevolucion(new DateTime(2026, 11, 5), "Devuelto", DateTime.UtcNow);

        var excepcion = Assert.Throws<ReglaNegocioException>(() =>
            prestamo.RegistrarDevolucion(new DateTime(2026, 11, 8), "Devuelto otra vez", DateTime.UtcNow));

        excepcion.Codigo.Should().Be("RN-10");
    }

    [Fact]
    public void Movimiento_RegistraLaTrazaDeLaAccion()
    {
        var movimiento = new Movimiento(TipoMovimiento.Prestamo, "Préstamo registrado", "Prestamo", Guid.NewGuid(), Guid.NewGuid(), "Administrador", DateTime.UtcNow);

        movimiento.Tipo.Should().Be(TipoMovimiento.Prestamo);
        movimiento.Descripcion.Should().Be("Préstamo registrado");
        movimiento.UsuarioNombre.Should().Be("Administrador");
    }
}

/// <summary>Pruebas de los objetos valor (inmutabilidad, normalización y validación).</summary>
public class ObjetosValorTests
{
    [Fact]
    public void Email_NormalizaYValidaElFormato()
    {
        var correo = Email.Crear("  Usuario.TEC@TechAsset.PE ");

        correo.Valor.Should().Be("usuario.tec@techasset.pe");
    }

    [Theory]
    [InlineData("correo-sin-arroba")]
    [InlineData("correo@dominio")]
    [InlineData("@techasset.pe")]
    public void Email_RechazaFormatosInvalidos(string valor)
    {
        var accion = () => Email.Crear(valor);
        accion.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CodigoPatrimonial_NormalizaAMayusculas()
    {
        var codigo = CodigoPatrimonial.Crear(" tech-lap-001 ");

        codigo.Valor.Should().Be("TECH-LAP-001");
    }

    [Fact]
    public void CodigoPatrimonial_RechazaValoresDemasiadoLargos()
    {
        var accion = () => CodigoPatrimonial.Crear(new string('A', 51));
        accion.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ObjetosValor_SonIgualesPorValor()
    {
        Email.Crear("a@b.com").Should().Be(Email.Crear("A@B.com"));
        Email.Crear("a@b.com").Should().NotBe(Email.Crear("c@d.com"));
    }
}
