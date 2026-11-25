/** Tarjeta de indicador con icono, valor, tendencia y apoyo contextual. */
export function TarjetaEstadistica({ titulo, valor, descripcion, icono, tono = 'marca', pie, progreso }) {
  const tonos = {
    marca: 'bg-brand-50 text-brand-600 dark:bg-brand-500/15 dark:text-brand-300',
    exito: 'bg-emerald-50 text-emerald-600 dark:bg-emerald-500/15 dark:text-emerald-300',
    aviso: 'bg-amber-50 text-amber-600 dark:bg-amber-500/15 dark:text-amber-300',
    peligro: 'bg-rose-50 text-rose-600 dark:bg-rose-500/15 dark:text-rose-300',
    info: 'bg-sky-50 text-sky-600 dark:bg-sky-500/15 dark:text-sky-300',
    neutro: 'bg-surface-3 text-muted'
  };

  return (
    <article className="tarjeta group relative overflow-hidden p-5 transition hover:shadow-pop">
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0">
          <p className="text-sm font-medium text-muted">{titulo}</p>
          <p className="mt-2 text-3xl font-extrabold tracking-tight text-ink">{valor}</p>
        </div>
        {icono && (
          <span className={`flex h-11 w-11 shrink-0 items-center justify-center rounded-xl ${tonos[tono]}`} aria-hidden="true">
            {icono}
          </span>
        )}
      </div>

      {descripcion && <p className="mt-2 text-sm text-muted">{descripcion}</p>}

      {typeof progreso === 'number' && (
        <div className="mt-4">
          <div className="h-2 w-full overflow-hidden rounded-full bg-surface-3" role="presentation">
            <div
              className={`h-full rounded-full transition-all duration-700 ${
                tono === 'exito' ? 'bg-emerald-500' : tono === 'aviso' ? 'bg-amber-500' : tono === 'peligro' ? 'bg-rose-500' : 'bg-brand-500'
              }`}
              style={{ width: `${Math.min(100, Math.max(0, progreso))}%` }}
            />
          </div>
          <p className="mt-1.5 text-xs font-medium text-muted">{pie}</p>
        </div>
      )}
    </article>
  );
}
