/**
 * Insignia de estado con estilo semántico (color + texto: nunca solo color).
 * `estilo` permite aplicar una paleta específica (por ejemplo, la del estado
 * del equipo) sin acumular clases que compitan entre sí.
 */
export function Insignia({ children, tono = 'neutro', estilo, className = '' }) {
  const tonos = {
    neutro: 'bg-surface-3 text-muted ring-line-strong/60',
    marca: 'bg-brand-50 text-brand-700 ring-brand-600/20 dark:bg-brand-500/10 dark:text-brand-300',
    exito: 'bg-emerald-50 text-emerald-700 ring-emerald-600/20 dark:bg-emerald-500/10 dark:text-emerald-300',
    aviso: 'bg-amber-50 text-amber-700 ring-amber-600/20 dark:bg-amber-500/10 dark:text-amber-300',
    peligro: 'bg-rose-50 text-rose-700 ring-rose-600/20 dark:bg-rose-500/10 dark:text-rose-300',
    info: 'bg-sky-50 text-sky-700 ring-sky-600/20 dark:bg-sky-500/10 dark:text-sky-300'
  };

  return (
    <span
      className={`inline-flex items-center gap-1.5 whitespace-nowrap rounded-full px-2.5 py-0.5 text-xs font-semibold ring-1 ring-inset ${
        estilo ?? tonos[tono]
      } ${className}`}
    >
      {children}
    </span>
  );
}

/** Punto de color con texto que acompaña al estado del equipo. */
export function InsigniaEstado({ estado, nombre, tono = 'neutro' }) {
  return (
    <Insignia tono={tono} className="gap-2">
      <span className="h-1.5 w-1.5 rounded-full bg-current opacity-80" aria-hidden="true" />
      {nombre}
    </Insignia>
  );
}
