import { createPortal } from 'react-dom';
import { useToast } from '../../contexto/ToastContexto';

/**
 * Contenedor de notificaciones. Se anuncia a lectores de pantalla mediante
 * `role="status"` / `role="alert"` (accesibilidad y realimentación inmediata).
 */
export function Notificaciones() {
  const { notificaciones, descartar } = useToast();

  if (!notificaciones.length) return null;

  return createPortal(
    <div className="pointer-events-none fixed bottom-4 right-4 z-[60] flex w-full max-w-sm flex-col gap-2" aria-live="polite" aria-atomic="false">
      {notificaciones.map((notificacion) => (
        <div
          key={notificacion.id}
          role={notificacion.tipo === 'error' ? 'alert' : 'status'}
          className={`pointer-events-auto flex items-start gap-3 rounded-xl border px-4 py-3 shadow-pop animate-slide-in ${
            ESTILOS[notificacion.tipo] ?? ESTILOS.info
          }`}
        >
          <span className="mt-0.5 shrink-0" aria-hidden="true">
            {ICONOS[notificacion.tipo] ?? ICONOS.info}
          </span>
          <p className="flex-1 text-sm font-medium leading-relaxed">{notificacion.mensaje}</p>
          <button
            type="button"
            onClick={() => descartar(notificacion.id)}
            className="shrink-0 rounded-md p-0.5 opacity-70 transition hover:opacity-100"
            aria-label="Cerrar notificación"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="h-4 w-4" aria-hidden="true">
              <path d="M6 6l12 12M18 6L6 18" strokeLinecap="round" />
            </svg>
          </button>
        </div>
      ))}
    </div>,
    document.body
  );
}

const ESTILOS = {
  exito: 'border-emerald-200 bg-emerald-50 text-emerald-800 dark:border-emerald-500/30 dark:bg-emerald-500/10 dark:text-emerald-200',
  error: 'border-rose-200 bg-rose-50 text-rose-800 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-200',
  advertencia: 'border-amber-200 bg-amber-50 text-amber-800 dark:border-amber-500/30 dark:bg-amber-500/10 dark:text-amber-200',
  info: 'border-sky-200 bg-sky-50 text-sky-800 dark:border-sky-500/30 dark:bg-sky-500/10 dark:text-sky-200'
};

const ICONOS = {
  exito: (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="h-5 w-5">
      <circle cx="12" cy="12" r="9" />
      <path d="m8.5 12.5 2.5 2.5 4.5-5" strokeLinecap="round" strokeLinejoin="round" />
    </svg>
  ),
  error: (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="h-5 w-5">
      <circle cx="12" cy="12" r="9" />
      <path d="M12 7.5v5M12 16h.01" strokeLinecap="round" />
    </svg>
  ),
  advertencia: (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="h-5 w-5">
      <path d="M10.3 3.9 1.8 18a2 2 0 0 0 1.7 3h16.9a2 2 0 0 0 1.7-3L13.7 3.9a2 2 0 0 0-3.4 0Z" strokeLinejoin="round" />
      <path d="M12 9v4M12 16h.01" strokeLinecap="round" />
    </svg>
  ),
  info: (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="h-5 w-5">
      <circle cx="12" cy="12" r="9" />
      <path d="M12 11v5M12 8h.01" strokeLinecap="round" />
    </svg>
  )
};
