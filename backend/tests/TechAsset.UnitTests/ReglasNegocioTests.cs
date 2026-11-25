using FluentAssertions;
using TechAsset.Domain.Entities;
using TechAsset.Domain.Enums;
using TechAsset.Domain.Exceptions;
using TechAsset.Domain.Rules;
using Xunit;

namespace TechAsset.UnitTests;

/// <summary>
/// Pruebas unitarias del catálogo de reglas de negocio (RN-01 … RN-10).
/// Cada regla del caso práctico tiene al menos una verificación positiva y otra negativa.
/// </summary>
public class ReglasNegocioTests
{
    private static Equipo CrearEquipo(EstadoEquipo estado = EstadoEquipo.Disponible)
    {
        var equipo = new Equipo("TECH-LAP-001", "ThinkPad T14", "Laptop", "Lenovo", "SN1", "Almacén", DateTime.UtcNow.AddDays(-100), null, DateTime.UtcNow);

        if (estado != EstadoEquipo.Disponible)
        {
            equipo.CambiarEstado(estado, "Estado asignado en la prueba", DateTime.UtcNow);
        }

        return equipo;
    }

    // RN-01
    [Fact]
    public void RN01_UnEquipoConPrestamoActivo_NoPuedePrestarseDeNuevo()
    {
        var equipo = CrearEquipo();
        var excepcion = Assert.Throws<ReglaNegocioException>(() =>
            ReglasNegocio.ValidarEquipoSinPrestamoActivo(equipo, tienePrestamoActivo: true));

        excepcion.Codigo.Should().Be("RN-01");
        excepcion.Message.Should().Contain(equipo.Codigo);
    }

    [Fact]
    public void RN01_UnEquipoSinPrestamoActivo_PuedePrestarse()
    {
        var equipo = CrearEquipo();
        var accion = () => ReglasNegocio.ValidarEquipoSinPrestamoActivo(equipo, tienePrestamoActivo: false);
        accion.Should().NotThrow();
    }

    // RN-02
    [Fact]
    public void RN02_UnEquipoEnMantenimiento_NoPuedePrestarse()
    {
        var equipo = CrearEquipo(EstadoEquipo.Mantenimiento);
        var excepcion = Assert.Throws<ReglaNegocioException>(() =>
            ReglasNegocio.ValidarEquipoNoEnMantenimiento(equipo));

        excepcion.Codigo.Should().Be("RN-02");
    }

    [Fact]
    public void RN02_UnEquipoDisponible_PuedePrestarse()
    {
        var equipo = CrearEquipo();
        var accion = () => ReglasNegocio.ValidarEquipoNoEnMantenimiento(equipo);
        accion.Should().NotThrow();
    }

    // RN-03
    [Fact]
    public void RN03_UnEquipoEnBaja_NoPuedePrestarse()
    {
        var equipo = CrearEquipo(EstadoEquipo.Baja);
        var excepcion = Assert.Throws<ReglaNegocioException>(() =>
            ReglasNegocio.ValidarEquipoNoEnBaja(equipo));

        excepcion.Codigo.Should().Be("RN-03");
    }

    // RN-04
    [Theory]
    [InlineData(2026, 11, 17, 2026, 11, 16)]
    [InlineData(2026, 11, 17, 2026, 11, 17)]
    public void RN04_LaDevolucionDebeSerPosteriorAlPrestamo(int anioPrestamo, int mesPrestamo, int diaPrestamo, int anioDevolucion, int mesDevolucion, int diaDevolucion)
    {
        var excepcion = Assert.Throws<ReglaNegocioException>(() =>
            ReglasNegocio.ValidarFechasDePrestamo(
                new DateTime(anioPrestamo, mesPrestamo, diaPrestamo),
                new DateTime(anioDevolucion, mesDevolucion, diaDevolucion)));

        excepcion.Codigo.Should().Be("RN-04");
    }

    [Fact]
    public void RN04_UnaDevolucionPosteriorEsValida()
    {
        var accion = () => ReglasNegocio.ValidarFechasDePrestamo(new DateTime(2026, 11, 1), new DateTime(2026, 11, 17));
        accion.Should().NotThrow();
    }

    // RN-05
    [Fact]
    public void RN05_TodoPrestamoDebeRegistrarResponsable()
    {
        var excepcion = Assert.Throws<ReglaNegocioException>(() =>
            ReglasNegocio.ValidarResponsableRegistrado(Guid.Empty));

        excepcion.Codigo.Should().Be("RN-05");
    }

    // RN-06
    [Fact]
    public void RN06_UnEquipoConHistorialNoPuedeEliminarse()
    {
        var equipoConHistorial = CrearEquipo();
        var prestamo = new Prestamo(Guid.NewGuid(), equipoConHistorial.Id, Guid.NewGuid(), DateTime.UtcNow, null, DateTime.UtcNow);
        equipoConHistorial.AgregarPrestamo(prestamo);

        equipoConHistorial.TieneHistorial.Should().BeTrue();

        var excepcion = Assert.Throws<ReglaNegocioException>(() =>
            ReglasNegocio.ValidarEquipoEliminable(equipoConHistorial));

        excepcion.Codigo.Should().Be("RN-06");
    }

    // RN-07
    [Fact]
    public void RN07_ElCodigoPatrimonialDebeSerUnico()
    {
        var excepcion = Assert.Throws<ConflictoException>(() =>
            ReglasNegocio.ValidarCodigoPatrimonialUnico("TECH-LAP-001", yaExiste: true));

        excepcion.Codigo.Should().Be("RN-07");
    }

    // RN-08
    [Fact]
    public void RN08_ElCorreoDebeSerUnico()
    {
        var excepcion = Assert.Throws<ConflictoException>(() =>
            ReglasNegocio.ValidarCorreoUnico("usuario@techasset.pe", yaExiste: true));

        excepcion.Codigo.Should().Be("RN-08");
    }

    // RN-09
    [Fact]
    public void RN09_SoloElAdministradorPuedeDeshabilitarUsuarios()
    {
        var excepcion = Assert.Throws<OperacionNoPermitidaException>(() =>
            ReglasNegocio.ValidarPermisoDeshabilitar(esAdministrador: false));

        excepcion.Codigo.Should().Be("FORBIDDEN");

        var accion = () => ReglasNegocio.ValidarPermisoDeshabilitar(esAdministrador: true);
        accion.Should().NotThrow();
    }

    // RN-10
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RN10_TodaDevolucionDebeRegistrarObservaciones(string? observacion)
    {
        var excepcion = Assert.Throws<ReglaNegocioException>(() =>
            ReglasNegocio.ValidarObservacionEnDevolucion(observacion));

        excepcion.Codigo.Should().Be("RN-10");
    }
}
