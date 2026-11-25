using Microsoft.EntityFrameworkCore;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Domain.Entities;
using TechAsset.Domain.Enums;

namespace TechAsset.Infrastructure.Persistence.Sembrado;

/// <summary>
/// Genera un conjunto de datos de demostración coherente y determinista:
/// usuarios por área, inventario tecnológico variedoy loans con historial
/// realista. Permite evaluar el sistema con información significativa desde el
/// primer arranque (criterio de usabilidad de la ISO/IEC 9241-210).
/// </summary>
public sealed class SembradorDatos
{
    private readonly AppDbContext _contexto;
    private readonly IHashContrasena _hash;

    public SembradorDatos(AppDbContext contexto, IHashContrasena hash)
    {
        _contexto = contexto;
        _hash = hash;
    }

    public static async Task SembrarAsync(AppDbContext contexto, IHashContrasena hash, CancellationToken cancelacion = default)
    {
        var sembrador = new SembradorDatos(contexto, hash);
        await sembrador.EjecutarAsync(cancelacion);
    }

    private async Task EjecutarAsync(CancellationToken cancelacion)
    {
        if (await _contexto.Usuarios.AnyAsync(cancelacion))
        {
            return; // La base ya contiene información: no se sobrescribe.
        }

        var aleatorio = new Random(42);
        var ahora = DateTime.UtcNow;

        var (usuarios, movimientos) = CrearUsuarios(ahora, aleatorio);
        var (equipos, equiposMovimiento) = CrearEquipos(ahora, aleatorio);
        var prestamos = CrearPrestamos(usuarios, equipos, ahora, aleatorio);

        _contexto.Usuarios.AddRange(usuarios);
        _contexto.Equipos.AddRange(equipos);
        _contexto.Prestamos.AddRange(prestamos);
        _contexto.Movimientos.AddRange(movimientos);
        _contexto.Movimientos.AddRange(equiposMovimiento);
        _contexto.Movimientos.AddRange(prestamos.Select(p => new Movimiento(
            TipoMovimiento.Prestamo,
            $"Préstamo registrado: {p.Equipo?.Codigo} → {p.Usuario?.NombreCompleto} ({p.Usuario?.Area}).",
            nameof(Prestamo),
            p.Id,
            p.ResponsableId,
            p.Responsable?.NombreCompleto,
            p.FechaPrestamo)));

        await _contexto.SaveChangesAsync(cancelacion);
    }

    private (List<Usuario> Usuarios, List<Movimiento> Movimientos) CrearUsuarios(DateTime ahora, Random aleatorio)
    {
        var usuarios = new List<Usuario>();
        var movimientos = new List<Movimiento>();

        var catalogo = new (string Nombres, string Apellidos, string Area, RolUsuario Rol, string Contrasena)[]
        {
            ("Elena", "Quispe Medina", "TI", RolUsuario.Administrador, "Admin1234"),
            ("Rodrigo", "Vásquez Léon", "TI", RolUsuario.ResponsableTecnologia, "Responsable1234"),
            ("Andrea", "Flores Roja", "TI", RolUsuario.Colaborador, "Colaborador1234"),
            ("Carlos", "Mendoza Ruiz", "Contabilidad", RolUsuario.Colaborador, "Colaborador1234"),
            ("Lucía", "Torres Alvarez", "Ventas", RolUsuario.Colaborador, "Colaborador1234"),
            ("Miguel", "Soto Paredes", "Marketing", RolUsuario.Colaborador, "Colaborador1234"),
            ("Sofía", "Herrera Canto", "Operaciones", RolUsuario.Colaborador, "Colaborador1234"),
            ("Jorge", "Palomino Vera", "Recursos Humanos", RolUsuario.Colaborador, "Colaborador1234"),
            ("Patricia", "Ramos Leiva", "Legal", RolUsuario.Colaborador, "Colaborador1234"),
            ("Diego", "Castillo Núñez", "Soporte", RolUsuario.Colaborador, "Colaborador1234"),
            ("Claudia", "Benavides Ortiz", "Ventas", RolUsuario.Colaborador, "Colaborador1234"),
            ("Fernando", "Salazar Ruiz", "Marketing", RolUsuario.Colaborador, "Colaborador1234"),
            ("Rosa", "Guzmán Peña", "Contabilidad", RolUsuario.Colaborador, "Colaborador1234"),
            ("Álvaro", "Ponce Muñoz", "Operaciones", RolUsuario.Colaborador, "Colaborador1234"),
            ("Gabriela", "Reyes Cabrera", "TI", RolUsuario.Colaborador, "Colaborador1234"),
            ("Ricardo", "Alvarado Soto", "Soporte", RolUsuario.Colaborador, "Colaborador1234"),
            ("Silvia", "Chávez Ríos", "Recursos Humanos", RolUsuario.Colaborador, "Colaborador1234"),
            ("Héctor", "Ibáñez Vega", "Legal", RolUsuario.Colaborador, "Colaborador1234"),
            ("Teresa", "Morales Luna", "Operaciones", RolUsuario.Colaborador, "Colaborador1234"),
            ("Julio", "Cárdenas Paz", "Ventas", RolUsuario.Colaborador, "Colaborador1234"),
            ("Karina", "Espinoza Rojas", "Marketing", RolUsuario.Colaborador, "Colaborador1234"),
            ("Óscar", "Valdez Miranda", "Contabilidad", RolUsuario.Colaborador, "Colaborador1234"),
            ("Melissa", "Aguirre Farfán", "TI", RolUsuario.Colaborador, "Colaborador1234"),
            ("Iván", "Loayza Campos", "Soporte", RolUsuario.Colaborador, "Colaborador1234"),
            ("Noelia", "Quispe Arana", "Operaciones", RolUsuario.Colaborador, "Colaborador1234")
        };

        foreach (var item in catalogo)
        {
            var correo = NormalizarCorreo(item.Nombres, item.Apellidos);
            var usuario = new Usuario(
                item.Nombres,
                item.Apellidos,
                correo,
                item.Area,
                item.Rol,
                _hash.Hash(item.Contrasena),
                ahora.AddDays(-aleatorio.Next(120, 400)));

            usuarios.Add(usuario);
            movimientos.Add(new Movimiento(
                TipoMovimiento.AltaUsuario,
                $"Se registró al usuario {usuario.NombreCompleto} ({usuario.Correo}) en el área {usuario.Area}.",
                nameof(Usuario),
                usuario.Id,
                null,
                "Sistema",
                usuario.FechaCreacion));
        }

        return (usuarios, movimientos);
    }

    private (List<Equipo> Equipos, List<Movimiento> Movimientos) CrearEquipos(DateTime ahora, Random aleatorio)
    {
        var equipos = new List<Equipo>();
        var movimientos = new List<Movimiento>();

        var catalogo = new (string Categoria, string[] Modelos, string[] Marcas)[]
        {
            ("Laptop", new[] { "ThinkPad T14", "Latitude 5440", "EliteBook 840", "VivoBook 15", "MacBook Air M2" }, new[] { "Lenovo", "Dell", "HP", "Asus", "Apple" }),
            ("Desktop", new[] { "OptiPlex 7010", "ProDesk 400", "OptiPlex 3000" }, new[] { "Dell", "HP" }),
            ("Monitor", new[] { "ThinkVision 24", "P2422H", "UltraSharp 24", "ViewSonic VA24" }, new[] { "Lenovo", "Dell", "HP", "ViewSonic" }),
            ("Impresora", new[] { "LaserJet M428", "EcoTank L3250", "ImageCLASS MF445dw" }, new[] { "HP", "Epson", "Canon" }),
            ("Tablet", new[] { "Tab M10", "Galaxy Tab A9", "iPad 10ma" }, new[] { "Lenovo", "Samsung", "Apple" }),
            ("Proyector", new[] { "EB-X52", "PowerLite L520U", "Home Cinema EH-TW6250" }, new[] { "Epson", "Epson", "Epson" }),
            ("Equipo de Red", new[] { "Aruba 2530 Switch", "EdgeRouter 4", "Catalyst 1000", "Access Point U6" }, new[] { "HPE", "Ubiquiti", "Cisco", "Ubiquiti" })
        };

        var ubicaciones = new[] { "Sala 3-A", "Sala 3-B", "Almacén TI - Estante 1", "Almacén TI - Estante 2", "Puesto 12", "Puesto 45", "Recepción", "Sala de reuniones", "Data Center - Rack 2" };
        var correlativo = 1;

        foreach (var item in catalogo)
        {
            var cantidad = item.Categoria switch
            {
                "Laptop" => 18,
                "Desktop" => 8,
                "Monitor" => 14,
                "Impresora" => 6,
                "Tablet" => 6,
                "Proyector" => 4,
                _ => 8
            };

            for (var i = 0; i < cantidad; i++)
            {
                var modelo = item.Modelos[aleatorio.Next(item.Modelos.Length)];
                var marca = item.Marcas[aleatorio.Next(item.Marcas.Length)];
                var codigo = $"TECH-{item.Categoria[..3].ToUpperInvariant()}-{correlativo.ToString("000")}";
                correlativo++;

                var equipo = new Equipo(
                    codigo,
                    modelo,
                    item.Categoria,
                    marca,
                    $"SN{aleatorio.Next(100000, 999999)}",
                    ubicaciones[aleatorio.Next(ubicaciones.Length)],
                    ahora.AddDays(-aleatorio.Next(200, 1400)).Date,
                    "Equipo institucional con garantía de 24 meses y soporte de por vida.",
                    ahora.AddDays(-aleatorio.Next(30, 360)));

                // Algunos equipos quedan fuera de servicio (indicador de mantenimiento).
                if (aleatorio.Next(100) < 9)
                {
                    equipo.CambiarEstado(EstadoEquipo.Mantenimiento, "Revisión técnica programada", ahora);
                }
                else if (aleatorio.Next(100) < 3)
                {
                    equipo.CambiarEstado(EstadoEquipo.Baja, "Obsolescencia tecnológica", ahora);
                }

                equipos.Add(equipo);
                movimientos.Add(new Movimiento(
                    TipoMovimiento.AltaEquipo,
                    $"Se registró el equipo {equipo.Codigo} · {equipo.Nombre} ({equipo.Marca}).",
                    nameof(Equipo),
                    equipo.Id,
                    null,
                    "Sistema",
                    equipo.FechaCreacion));
            }
        }

        return (equipos, movimientos);
    }

    private List<Prestamo> CrearPrestamos(List<Usuario> usuarios, List<Equipo> equipos, DateTime ahora, Random aleatorio)
    {
        var prestamos = new List<Prestamo>();
        var colaboradores = usuarios.Where(u => u.Rol == RolUsuario.Colaborador).ToList();
        var responsables = usuarios.Where(u => u.Rol != RolUsuario.Colaborador).ToList();
        var prestables = equipos.Where(e => e.Estado == EstadoEquipo.Disponible).ToList();

        for (var i = 0; i < 52 && i < prestables.Count; i++)
        {
            var equipo = prestables[i];
            var usuario = colaboradores[aleatorio.Next(colaboradores.Count)];
            var responsable = responsables[aleatorio.Next(responsables.Count)];
            var activo = i % 4 != 0; // 75 % devueltos, 25 % activos

            var diasAtras = activo ? aleatorio.Next(2, 45) : aleatorio.Next(50, 330);
            var fechaPrestamo = ahora.Date.AddDays(-diasAtras);

            var prestamo = new Prestamo(
                usuario.Id,
                equipo.Id,
                responsable.Id,
                fechaPrestamo,
                "Entrega de equipo para el desarrollo de funciones del proyecto institucional.",
                ahora);

            if (!activo)
            {
                var diasPrestamo = aleatorio.Next(3, 30);
                var fechaDevolucion = fechaPrestamo.AddDays(diasPrestamo);
                if (fechaDevolucion > ahora.Date) fechaDevolucion = ahora.Date.AddDays(-1);
                if (fechaDevolucion <= fechaPrestamo) fechaDevolucion = fechaPrestamo.AddDays(1);

                prestamo.RegistrarDevolucion(
                    fechaDevolucion,
                    "Equipo devuelto en perfecto estado, sin observaciones de daño.",
                    ahora);
                equipo.CambiarEstado(EstadoEquipo.Disponible, "Equipo devuelto", ahora);
            }
            else
            {
                equipo.MarcarComoPrestado(ahora);
            }

            prestamos.Add(prestamo);
        }

        return prestamos;
    }

    private static string NormalizarCorreo(string nombres, string apellidos)
    {
        var nombre = nombres.Split(' ')[0].ToLowerInvariant();
        var apellido = apellidos.Split(' ')[0].ToLowerInvariant();
        var correo = $"{nombre}.{apellido}@techasset.pe";
        return System.Text.RegularExpressions.Regex.Replace(correo, "[^a-z0-9.@]", string.Empty);
    }
}
