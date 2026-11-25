using System.Text.RegularExpressions;

namespace TechAsset.Domain.ValueObjects;

/// <summary>
/// Objeto valor inmutable con semántica de igualdad por componentes.
/// Encapsula invariantes que no dependen de la identidad de la entidad.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other)
    {
        if (other is null || other.GetType() != GetType()) return false;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj) => obj is ValueObject other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var componente in GetEqualityComponents())
        {
            hash.Add(componente);
        }
        return hash.ToHashCode();
    }

    public static bool operator ==(ValueObject? a, ValueObject? b) =>
        a is null ? b is null : a.Equals(b);

    public static bool operator !=(ValueObject? a, ValueObject? b) => !(a == b);
}

/// <summary>
/// Objeto valor para el correo electrónico del usuario.
/// Normaliza a minúsculas (garantiza RN-08) y valida el formato antes de persistir.
/// </summary>
public sealed class Email : ValueObject
{
    private static readonly Regex FormatoValido = new(
        @"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Valor { get; private set; }

    private Email(string valor) => Valor = valor;

    /// <summary>Crea el objeto valor validando el formato del correo.</summary>
    public static Email Crear(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException("El correo electrónico es obligatorio.", nameof(valor));
        }

        var normalizado = valor.Trim().ToLowerInvariant();
        if (!FormatoValido.IsMatch(normalizado))
        {
            throw new ArgumentException("El correo electrónico no tiene un formato válido.", nameof(valor));
        }

        return new Email(normalizado);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
    }

    public override string ToString() => Valor;
}

/// <summary>
/// Objeto valor para el código patrimonial del equipo (RN-07).
/// Normaliza a mayúsculas y unifica el formato para evitar duplicados por mayúsculas/espacios.
/// </summary>
public sealed class CodigoPatrimonial : ValueObject
{
    public string Valor { get; private set; }

    private CodigoPatrimonial(string valor) => Valor = valor;

    public static CodigoPatrimonial Crear(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException("El código patrimonial es obligatorio.", nameof(valor));
        }

        var normalizado = valor.Trim().ToUpperInvariant();
        if (normalizado.Length > 50)
        {
            throw new ArgumentException("El código patrimonial no puede superar los 50 caracteres.", nameof(valor));
        }

        return new CodigoPatrimonial(normalizado);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
    }

    public override string ToString() => Valor;
}
