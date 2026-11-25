namespace TechAsset.Domain.Exceptions;

/// <summary>
/// Excepción base del dominio. Las capas superiores la traducen a respuestas HTTP
/// con un código de negocio rastreable (precisión y analizabilidad, ISO/IEC 25010).
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>Código funcional del error (p. ej. "RN-02").</summary>
    public string Codigo { get; }

    protected DomainException(string codigo, string mensaje) : base(mensaje)
    {
        Codigo = codigo;
    }
}

/// <summary>Se dispara cuando una regla de negocio (RN-xx) impide la operación.</summary>
public sealed class ReglaNegocioException : DomainException
{
    public ReglaNegocioException(string codigo, string mensaje) : base(codigo, mensaje) { }
}

/// <summary>Se dispara cuando la entidad solicitada no existe.</summary>
public sealed class EntidadNoEncontradaException : DomainException
{
    public EntidadNoEncontradaException(string entidad, object clave)
        : base("NOT_FOUND", $"No se encontró el registro solicitado: {entidad} ({clave}).") { }
}

/// <summary>Se dispara ante duplicados o conflictos de unicidad (RN-07, RN-08).</summary>
public sealed class ConflictoException : DomainException
{
    public ConflictoException(string codigo, string mensaje) : base(codigo, mensaje) { }
}

/// <summary>Se dispara cuando el usuario autenticado no tiene permisos (RN-09).</summary>
public sealed class OperacionNoPermitidaException : DomainException
{
    public OperacionNoPermitidaException(string mensaje) : base("FORBIDDEN", mensaje) { }
}
