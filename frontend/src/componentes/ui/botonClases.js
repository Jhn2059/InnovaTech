const VARIANTES = {
  primario: 'bg-brand-600 text-white shadow-sm hover:bg-brand-700 focus-visible:ring-brand-500 disabled:bg-brand-100 disabled:text-brand-700 dark:bg-brand-400 dark:text-brand-950 dark:hover:bg-brand-300 dark:focus-visible:ring-brand-300 dark:disabled:bg-brand-500/20 dark:disabled:text-brand-200',
  secundario: 'bg-surface text-ink ring-1 ring-line hover:bg-surface-3 disabled:text-muted',
  fantasma: 'text-muted hover:bg-surface-3 hover:text-ink disabled:opacity-50',
  peligro: 'bg-rose-600 text-white shadow-sm hover:bg-rose-700 focus-visible:ring-rose-500 disabled:bg-rose-100 disabled:text-rose-700 dark:disabled:bg-rose-500/20 dark:disabled:text-rose-200',
  exito: 'bg-emerald-600 text-white shadow-sm hover:bg-emerald-700 focus-visible:ring-emerald-500 disabled:bg-emerald-100 disabled:text-emerald-700 dark:disabled:bg-emerald-500/20 dark:disabled:text-emerald-200'
};

const TAMANOS = {
  pequeno: 'h-8 px-3 text-xs gap-1.5',
  medio: 'h-10 px-4 text-sm gap-2',
  grande: 'h-12 px-6 text-base gap-2.5'
};

export function clasesBoton({ variante = 'primario', tamano = 'medio', className = '' } = {}) {
  return `inline-flex items-center justify-center rounded-xl font-semibold transition
    focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-offset-2
    disabled:cursor-not-allowed ${VARIANTES[variante]} ${TAMANOS[tamano]} ${className}`;
}
