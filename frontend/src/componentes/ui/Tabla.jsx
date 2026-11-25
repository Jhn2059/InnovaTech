/**
 * Tabla de datos reutilizable con soporte para estados de carga, vacío y error.
 * Incluye encabezados ordenables opcionales y renderizado por columna.
 */
export function Tabla({ columnas, datos, claveFila = clavePorDefecto, cargando = false, vacio, error, alOrdenar, columnaOrden, descendente = true }) {
  return (
    <div className="overflow-x-auto">
      <table className="w-full min-w-[46rem] border-collapse text-left text-sm">
        <thead>
          <tr className="border-b border-line bg-surface-2/60">
            {columnas.map((columna) => {
              const activa = columnaOrden === columna.clave;
              return (
                <th
                  key={columna.clave}
                  scope="col"
                  className="px-4 py-3 text-xs font-semibold uppercase tracking-wide text-muted"
                  aria-sort={activa ? (descendente ? 'descending' : 'ascending') : 'none'}
                >
                  {columna.ordenable && alOrdenar ? (
                    <button
                      type="button"
                      onClick={() => alOrdenar(columna.clave)}
                      className="inline-flex items-center gap-1 rounded transition hover:text-ink"
                    >
                      {columna.titulo}
                      <svg viewBox="0 0 20 20" fill="currentColor" className={`h-3.5 w-3.5 ${activa ? 'opacity-100' : 'opacity-30'}`} aria-hidden="true">
                        <path fillRule="evenodd" d="M5.22 8.22a.75.75 0 0 1 1.06 0L10 11.94l3.72-3.72a.75.75 0 1 1 1.06 1.06l-4.25 4.25a.75.75 0 0 1-1.06 0L5.22 9.28a.75.75 0 0 1 0-1.06Z" clipRule="evenodd" />
                      </svg>
                    </button>
                  ) : (
                    columna.titulo
                  )}
                </th>
              );
            })}
          </tr>
        </thead>
        <tbody className="divide-y divide-line">
          {cargando
            ? Array.from({ length: 5 }).map((_, indice) => (
                <tr key={`esqueleto-${indice}`}>
                  {columnas.map((columna) => (
                    <td key={columna.clave} className="px-4 py-3.5">
                      <div className="h-4 w-full animate-pulse rounded bg-surface-3" style={{ maxWidth: `${60 + (indice % 3) * 15}%` }} />
                    </td>
                  ))}
                </tr>
              ))
            : datos?.map((fila) => (
                <tr key={claveFila(fila)} className="transition hover:bg-surface-2/70">
                  {columnas.map((columna) => (
                    <td key={columna.clave} className={`px-4 py-3.5 align-middle ${columna.claseCelda ?? 'text-ink'}`}>
                      {columna.render ? columna.render(fila) : fila[columna.clave]}
                    </td>
                  ))}
                </tr>
              ))}
        </tbody>
      </table>

      {!cargando && error && <MensajeTabla titulo="No fue posible cargar la información" detalle={error} tipo="error" />}
      {!cargando && !error && (!datos || datos.length === 0) && (vacio ?? <MensajeTabla titulo="Sin registros" detalle="No hay información que coincida con los filtros aplicados." />)}
    </div>
  );
}

/** Clave estable por fila para los distintos proyecciones que usa el sistema. */
function clavePorDefecto(fila) {
  return (
    fila.id ??
    fila.codigoPatrimonial ??
    fila.entidadId ??
    fila.equipoId ??
    fila.area ??
    fila.nombre ??
    fila.etiqueta ??
    JSON.stringify(fila)
  );
}

function MensajeTabla({ titulo, detalle, tipo = 'vacio' }) {  return (
    <div className="flex flex-col items-center justify-center gap-1 px-6 py-12 text-center">
      <div
        className={`mb-2 flex h-11 w-11 items-center justify-center rounded-full ${
          tipo === 'error' ? 'bg-rose-50 text-rose-500 dark:bg-rose-500/10' : 'bg-surface-3 text-muted'
        }`}
        aria-hidden="true"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" className="h-6 w-6">
          {tipo === 'error' ? (
            <path d="M12 9v4m0 4h.01M10.3 3.86 1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.7 3.86a2 2 0 0 0-3.4 0Z" strokeLinecap="round" strokeLinejoin="round" />
          ) : (
            <path d="M20 13V6a2 2 0 0 0-2-2H6a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h7M9 9h6m-6 4h3M15 19l3 3 4-4" strokeLinecap="round" strokeLinejoin="round" />
          )}
        </svg>
      </div>
      <p className="text-sm font-semibold text-ink">{titulo}</p>
      {detalle && <p className="max-w-sm text-sm text-muted">{detalle}</p>}
    </div>
  );
}
