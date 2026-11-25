/** Indicador de carga con etiqueta opcional (visibilidad del estado del sistema). */
export function Cargador({ texto = 'Cargando…', className = '' }) {
  return (
    <div className={`flex flex-col items-center justify-center gap-3 py-12 text-muted ${className}`} role="status" aria-live="polite">
      <span className="h-8 w-8 animate-spin rounded-full border-[3px] border-line border-t-brand-600" aria-hidden="true" />
      <span className="text-sm font-medium">{texto}</span>
    </div>
  );
}

/** Esqueleto de carga para tarjetas y bloques de contenido. */
export function Esqueleto({ className = '' }) {
  return <div className={`animate-pulse rounded-lg bg-surface-3 ${className}`} aria-hidden="true" />;
}

/** Estado vacío con acción opcional (evita páginas en blanco sin explicación). */
export function EstadoVacio({ titulo, descripcion, icono, accion }) {
  return (
    <div className="flex flex-col items-center justify-center gap-3 px-6 py-14 text-center">
      <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-brand-50 text-brand-600 dark:bg-brand-500/10 dark:text-brand-300" aria-hidden="true">
        {icono ?? (
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.6" className="h-7 w-7">
            <path d="M20 13V6a2 2 0 0 0-2-2H6a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h7M9 9h6m-6 4h3M15 19l3 3 4-4" strokeLinecap="round" strokeLinejoin="round" />
          </svg>
        )}
      </div>
      <div>
        <p className="text-base font-semibold text-ink">{titulo}</p>
        {descripcion && <p className="mt-1 max-w-sm text-sm text-muted">{descripcion}</p>}
      </div>
      {accion}
    </div>
  );
}

/** Buscador con etiqueta accesible y limpieza rápida. */
export function Buscador({ valor, onCambio, placeholder = 'Buscar…', className = '' }) {
  return (
    <div className={`relative ${className}`}>
      <svg
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        strokeWidth="2"
        className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted"
        aria-hidden="true"
      >
        <circle cx="11" cy="11" r="7" />
        <path d="m20 20-3.5-3.5" strokeLinecap="round" />
      </svg>
      <input
        type="search"
        value={valor}
        onChange={(evento) => onCambio(evento.target.value)}
        placeholder={placeholder}
        aria-label={placeholder}
        className="campo-base pl-9 pr-9"
      />
      {valor && (
        <button
          type="button"
          onClick={() => onCambio('')}
          className="absolute right-2 top-1/2 -translate-y-1/2 rounded-md p-1 text-muted transition hover:bg-surface-3 hover:text-ink"
          aria-label="Limpiar búsqueda"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="h-4 w-4" aria-hidden="true">
            <path d="M6 6l12 12M18 6L6 18" strokeLinecap="round" />
          </svg>
        </button>
      )}
    </div>
  );
}
