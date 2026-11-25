using TechAsset.Domain.Entities;

namespace TechAsset.Application.Common.Abstractions;

/// <summary>Unidad de trabajo: confirma de forma atómica las operaciones del caso de uso.</summary>
public interface IUnitOfWork
{
    Task<int> GuardarCambiosAsync(CancellationToken cancelacion = default);
}

/// <summary>Abstracción del reloj del sistema para testabilidad determinista.</summary>
public interface IFechaActual
{
    DateTime Ahora { get; }
}

/// <summary>Servicio de hashing de contraseñas (implementado con BCrypt en Infrastructure).</summary>
public interface IHashContrasena
{
    string Hash(string contrasena);
    bool Verificar(string contrasena, string hash);
}

/// <summary>Emisor de tokens JWT (implementación en Infrastructure).</summary>
public interface IJwtTokenService
{
    (string Token, DateTime ExpiraEn) Generar(Usuario usuario);
}

/// <summary>Contexto del usuario autenticado en la petición actual.</summary>
public interface IUsuarioActual
{
    Guid Id { get; }
    string Correo { get; }
    string Nombre { get; }
    string Rol { get; }
    bool EsAdministrador { get; }
    bool EstaAutenticado { get; }
}
