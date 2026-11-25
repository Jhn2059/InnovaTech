using Microsoft.EntityFrameworkCore;
using TechAsset.Application.Common.Abstractions;
using TechAsset.Application.Dtos;
using TechAsset.Domain.Entities;
using TechAsset.Domain.Enums;

namespace TechAsset.Infrastructure.Persistence.Repositorios;

/// <summary>Repositorio de usuarios sobre EF Core.</summary>
public sealed class RepositorioUsuario : IRepositorioUsuario
{
    private readonly AppDbContext _contexto;

    public RepositorioUsuario(AppDbContext contexto) => _contexto = contexto;

    public Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default) =>
        _contexto.Usuarios.FirstOrDefaultAsync(u => u.Id == id, cancelacion);

    public Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancelacion = default) =>
        _contexto.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo, cancelacion);

    public Task<bool> ExisteCorreoAsync(string correo, Guid? excluirId = null, CancellationToken cancelacion = default) =>
        _contexto.Usuarios.AnyAsync(u => u.Correo == correo && (excluirId == null || u.Id != excluirId), cancelacion);

    public Task<bool> CualquieraAsync(CancellationToken cancelacion = default) =>
        _contexto.Usuarios.AnyAsync(cancelacion);

    public async Task<(IReadOnlyList<Usuario> Items, int Total)> ListarAsync(ConsultaUsuarios consulta, CancellationToken cancelacion = default)
    {
        var query = _contexto.Usuarios.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(consulta.Area))
        {
            query = query.Where(u => u.Area == consulta.Area);
        }

        if (!string.IsNullOrWhiteSpace(consulta.Rol) &&
            Application.Common.Extensions.EnumExtensions.TryParseRol(consulta.Rol, out var rol))
        {
            query = query.Where(u => u.Rol == rol);
        }

        if (consulta.SoloActivos is true)
        {
            query = query.Where(u => u.Estado);
        }

        var busqueda = consulta.BusquedaNormalizada;
        if (busqueda is not null)
        {
            var patron = $"%{busqueda}%";
            query = query.Where(u =>
                EF.Functions.ILike(u.Nombres, patron) ||
                EF.Functions.ILike(u.Apellidos, patron) ||
                EF.Functions.ILike(u.Correo, patron) ||
                EF.Functions.ILike(u.Area, patron));
        }

        var total = await query.CountAsync(cancelacion);

        query = consulta.OrdenarPor switch
        {
            "nombres" => consulta.Descendente ? query.OrderByDescending(u => u.Nombres) : query.OrderBy(u => u.Nombres),
            "apellidos" => consulta.Descendente ? query.OrderByDescending(u => u.Apellidos) : query.OrderBy(u => u.Apellidos),
            "correo" => consulta.Descendente ? query.OrderByDescending(u => u.Correo) : query.OrderBy(u => u.Correo),
            "area" => consulta.Descendente ? query.OrderByDescending(u => u.Area) : query.OrderBy(u => u.Area),
            "rol" => consulta.Descendente ? query.OrderByDescending(u => u.Rol) : query.OrderBy(u => u.Rol),
            "estado" => consulta.Descendente ? query.OrderByDescending(u => u.Estado) : query.OrderBy(u => u.Estado),
            _ => consulta.Descendente ? query.OrderByDescending(u => u.FechaCreacion) : query.OrderBy(u => u.FechaCreacion)
        };

        var items = await query
            .Skip(consulta.Saltar)
            .Take(consulta.TamanoPagina)
            .ToListAsync(cancelacion);

        return (items, total);
    }

    public async Task<IReadOnlyList<string>> ObtenerAreasAsync(CancellationToken cancelacion = default) =>
        await _contexto.Usuarios
            .AsNoTracking()
            .Select(u => u.Area)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync(cancelacion);

    public async Task<(int Total, int Activos)> ObtenerTotalesAsync(CancellationToken cancelacion = default)
    {
        var total = await _contexto.Usuarios.CountAsync(cancelacion);
        var activos = await _contexto.Usuarios.CountAsync(u => u.Estado, cancelacion);
        return (total, activos);
    }

    public void Agregar(Usuario usuario) => _contexto.Usuarios.Add(usuario);

    public void Actualizar(Usuario usuario) => _contexto.Usuarios.Update(usuario);

    public void Remover(Usuario usuario) => _contexto.Usuarios.Remove(usuario);
}
