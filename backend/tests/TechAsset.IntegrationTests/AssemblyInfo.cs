using Xunit;

// Las pruebas de integración comparten una base de datos PostgreSQL real:
// se ejecutan de forma secuencial para garantizar el aislamiento de los datos.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
