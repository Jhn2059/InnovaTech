using TechAsset.Application.Common.Abstractions;

namespace TechAsset.Infrastructure.Servicios;

/// <summary>Reloj del sistema basado en UTC.</summary>
public sealed class FechaActual : IFechaActual
{
    public DateTime Ahora => DateTime.UtcNow;
}

/// <summary>Servicio de hashing de contraseñas con BCrypt.</summary>
public sealed class HashContrasena : IHashContrasena
{
    private const int Costo = 11;

    public string Hash(string contrasena) => BCrypt.Net.BCrypt.HashPassword(contrasena, Costo);

    public bool Verificar(string contrasena, string hash)
    {
        if (string.IsNullOrEmpty(hash)) return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(contrasena, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
