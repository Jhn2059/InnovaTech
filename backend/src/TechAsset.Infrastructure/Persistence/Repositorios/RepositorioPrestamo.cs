using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Application.Dtos;
using TechAsset.Domain.Entities;
using TechAsset.Domain.Enums;

namespace TechAsset.Infrastructure.Persistence.Repositorios;

/// <summary>
/// Repositorio de préstamos sobre EF Core. Los reportes agregados se resuelven
/// con SQL nativo sobre PostgreSQL para garantizar el rendimiento de las
/// consultas analíticas del dashboard (decisión de diseño documentada).
/// </summary>
public sealed class RepositorioPrestamo : IRepositorioPrestamo
{
    private readonly AppDbContext _contexto;

    public RepositorioPrestamo(AppDbContext contexto) => _contexto = contexto;

    public Task<Prestamo?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default) =>
        _contexto.Prestamos
            .Include(p => p.Usuario)
            .Include(p => p.Equipo)
            .Include(p => p.Responsable)
            .FirstOrDefaultAsync(p => p.Id == id, cancelacion);

    public Task<bool> ExistePrestamoActivoParaEquipoAsync(Guid equipoId, CancellationToken cancelacion = default) =>
        _contexto.Prestamos.AnyAsync(p => p.EquipoId == equipoId && p.Estado == EstadoPrestamo.Activo, cancelacion);

    public async Task<(IReadOnlyList<Prestamo> Items, int Total)> ListarAsync(ConsultaPrestamos consulta, CancellationToken cancelacion = default)
    {
        IQueryable<Prestamo> query = _contexto.Prestamos
            .AsNoTracking()
            .Include(p => p.Usuario)
            .Include(p => p.Equipo)
            .Include(p => p.Responsable);

        if (consulta.UsuarioId is not null)
        {
            query = query.Where(p => p.UsuarioId == consulta.UsuarioId);
        }

        if (consulta.EquipoId is not null)
        {
            query = query.Where(p => p.EquipoId == consulta.EquipoId);
        }

        if (!string.IsNullOrWhiteSpace(consulta.Area))
        {
            query = query.Where(p => p.Usuario!.Area == consulta.Area);
        }

        if (!string.IsNullOrWhiteSpace(consulta.Estado) &&
            Application.Common.Extensions.EnumExtensions.TryParseEstadoPrestamo(consulta.Estado, out var estado))
        {
            query = query.Where(p => p.Estado == estado);
        }

        if (consulta.Desde is not null)
        {
            var desde = consulta.Desde.Value.Date;
            query = query.Where(p => p.FechaPrestamo >= desde);
        }

        if (consulta.Hasta is not null)
        {
            var hasta = consulta.Hasta.Value.Date;
            query = query.Where(p => p.FechaPrestamo <= hasta);
        }

        var busqueda = consulta.BusquedaNormalizada;
        if (busqueda is not null)
        {
            var patron = $"%{busqueda}%";
            query = query.Where(p =>
                (p.Usuario != null && EF.Functions.ILike(p.Usuario.Nombres, patron)) ||
                (p.Usuario != null && EF.Functions.ILike(p.Usuario.Apellidos, patron)) ||
                (p.Equipo != null && EF.Functions.ILike(p.Equipo.Codigo, patron)) ||
                (p.Equipo != null && EF.Functions.ILike(p.Equipo.Nombre, patron)));
        }

        var total = await query.CountAsync(cancelacion);

        query = consulta.OrdenarPor switch
        {
            "fechaprestamo" => consulta.Descendente ? query.OrderByDescending(p => p.FechaPrestamo) : query.OrderBy(p => p.FechaPrestamo),
            "fechadevolucion" => consulta.Descendente ? query.OrderByDescending(p => p.FechaDevolucion) : query.OrderBy(p => p.FechaDevolucion),
            "usuario" => consulta.Descendente ? query.OrderByDescending(p => p.Usuario!.Nombres).ThenByDescending(p => p.Usuario!.Apellidos) : query.OrderBy(p => p.Usuario!.Nombres).ThenBy(p => p.Usuario!.Apellidos),
            "equipo" => consulta.Descendente ? query.OrderByDescending(p => p.Equipo!.Nombre) : query.OrderBy(p => p.Equipo!.Nombre),
            _ => consulta.Descendente ? query.OrderByDescending(p => p.FechaPrestamo) : query.OrderBy(p => p.FechaPrestamo)
        };

        var items = await query
            .Skip(consulta.Saltar)
            .Take(consulta.TamanoPagina)
            .ToListAsync(cancelacion);

        return (items, total);
    }

    public async Task<IReadOnlyList<Prestamo>> ListarActivosAsync(CancellationToken cancelacion = default) =>
        await _contexto.Prestamos
            .AsNoTracking()
            .Include(p => p.Usuario)
            .Include(p => p.Equipo)
            .Include(p => p.Responsable)
            .Where(p => p.Estado == EstadoPrestamo.Activo)
            .OrderBy(p => p.FechaPrestamo)
            .ToListAsync(cancelacion);

    public async Task<IReadOnlyList<PrestamosPorAreaDto>> ObtenerPrestamosPorAreaAsync(CancellationToken cancelacion = default)
    {
        const string sql = """
            SELECT u."Area"                          AS "Area",
                   COUNT(*)::int                    AS "TotalPrestamos",
                   COUNT(*) FILTER (WHERE p."Estado" = 1)::int AS "PrestamosActivos",
                   COUNT(*) FILTER (WHERE p."Estado" = 2)::int AS "PrestamosDevueltos",
                   COALESCE(AVG(COALESCE(p."FechaDevolucion", CURRENT_DATE) - p."FechaPrestamo"), 0)::float AS "PromedioDias"
            FROM "Prestamos" p
            INNER JOIN "Usuarios" u ON u."Id" = p."UsuarioId"
            GROUP BY u."Area"
            ORDER BY COUNT(*) DESC, u."Area";
            """;

        var resultados = new List<PrestamosPorAreaDto>();
        await foreach (var fila in EjecutarConsultaAsync(sql, cancelacion))
        {
            resultados.Add(new PrestamosPorAreaDto
            {
                Area = fila.GetString(0),
                TotalPrestamos = fila.GetInt32(1),
                PrestamosActivos = fila.GetInt32(2),
                PrestamosDevueltos = fila.GetInt32(3),
                PromedioDias = Math.Round(fila.GetDouble(4), 1)
            });
        }

        return resultados;
    }

    public async Task<IReadOnlyList<EquipoMasUtilizadoDto>> ObtenerEquiposMasUtilizadosAsync(int tope, CancellationToken cancelacion = default)
    {
        const string sql = """
            SELECT e."Id"                                AS "Id",
                   e."CodigoPatrimonial"                  AS "CodigoPatrimonial",
                   e."Nombre"                             AS "Nombre",
                   e."Categoria"                          AS "Categoria",
                   e."Marca"                              AS "Marca",
                   COUNT(p."Id")::int                     AS "TotalPrestamos",
                   COALESCE(AVG(COALESCE(p."FechaDevolucion", CURRENT_DATE) - p."FechaPrestamo"), 0)::float AS "DiasPromedio"
            FROM "Equipos" e
            INNER JOIN "Prestamos" p ON p."EquipoId" = e."Id"
            GROUP BY e."Id", e."CodigoPatrimonial", e."Nombre", e."Categoria", e."Marca"
            ORDER BY COUNT(p."Id") DESC, e."CodigoPatrimonial"
            LIMIT @tope;
            """;

        var resultados = new List<EquipoMasUtilizadoDto>();
        await foreach (var fila in EjecutarConsultaAsync(sql, cancelacion, ("@tope", NpgsqlDbType.Integer, tope)))
        {
            resultados.Add(new EquipoMasUtilizadoDto
            {
                Id = fila.GetGuid(0),
                CodigoPatrimonial = fila.GetString(1),
                Nombre = fila.GetString(2),
                Categoria = fila.GetString(3),
                Marca = fila.GetString(4),
                TotalPrestamos = fila.GetInt32(5),
                DiasPromedio = Math.Round(fila.GetDouble(6), 1)
            });
        }

        return resultados;
    }

    public async Task<IReadOnlyList<IndicadorDto>> ObtenerPrestamosPorMesAsync(int meses, CancellationToken cancelacion = default)
    {
        const string sql = """
            SELECT to_char(date_trunc('month', p."FechaPrestamo"), 'YYYY-MM') AS "Periodo",
                   COUNT(*)::int AS "Total"
            FROM "Prestamos" p
            GROUP BY date_trunc('month', p."FechaPrestamo")
            ORDER BY date_trunc('month', p."FechaPrestamo") DESC
            LIMIT @meses;
            """;

        var filas = new List<(string Periodo, int Total)>();
        await foreach (var fila in EjecutarConsultaAsync(sql, cancelacion, ("@meses", NpgsqlDbType.Integer, meses)))
        {
            filas.Add((fila.GetString(0), fila.GetInt32(1)));
        }

        var totalGeneral = filas.Sum(f => f.Total);

        return filas
            .OrderBy(f => f.Periodo)
            .Select(f => new IndicadorDto
            {
                Etiqueta = TraducirPeriodo(f.Periodo),
                Valor = f.Total,
                Porcentaje = totalGeneral == 0 ? 0 : Math.Round(f.Total * 100d / totalGeneral, 1)
            })
            .ToList();
    }

    private static string TraducirPeriodo(string periodo)
    {
        var partes = periodo.Split('-');
        if (partes.Length != 2 || !int.TryParse(partes[0], out var anio) || !int.TryParse(partes[1], out var mes))
        {
            return periodo;
        }

        var nombres = new[] { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
        return $"{nombres[Math.Clamp(mes - 1, 0, 11)]} {anio}";
    }

    /// <summary>Ejecuta una consulta SQL nativa y expone los resultados como filas indexadas.</summary>
    private async IAsyncEnumerable<DbDataReader> EjecutarConsultaAsync(
        string sql,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancelacion = default,
        params (string Nombre, NpgsqlTypes.NpgsqlDbType Tipo, object Valor)[] parametros)
    {
        var conexion = _contexto.Database.GetDbConnection();
        if (conexion.State != ConnectionState.Open)
        {
            await conexion.OpenAsync(cancelacion);
        }

        await using var comando = conexion.CreateCommand();
        comando.CommandText = sql;
        foreach (var (nombre, tipo, valor) in parametros)
        {
            var parametro = new NpgsqlParameter(nombre, tipo) { Value = valor };
            comando.Parameters.Add(parametro);
        }

        await using var lector = await comando.ExecuteReaderAsync(cancelacion);
        while (await lector.ReadAsync(cancelacion))
        {
            yield return lector;
        }
    }

    public void Agregar(Prestamo prestamo) => _contexto.Prestamos.Add(prestamo);

    public void Actualizar(Prestamo prestamo) => _contexto.Prestamos.Update(prestamo);
}
