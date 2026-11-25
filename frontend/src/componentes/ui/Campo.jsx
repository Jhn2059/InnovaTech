import { useId } from 'react';

/**
 * Envoltorio de campo de formulario: etiqueta, ayuda, error y accessibility
 * (asociación label-control, aria-invalid y aria-describedby).
 */
export function Campo({ etiqueta, error, ayuda, requerido, children, className = '' }) {
  const id = useId();
  const idError = `${id}-error`;
  const idAyuda = `${id}-ayuda`;

  return (
    <div className={className}>
      {etiqueta && (
        <label htmlFor={id} className="etiqueta-campo">
          {etiqueta}
          {requerido && (
            <span className="ml-0.5 text-rose-500" aria-hidden="true">
              *
            </span>
          )}
        </label>
      )}
      {typeof children === 'function' ? children({ id, 'aria-invalid': Boolean(error), 'aria-describedby': error ? idError : ayuda ? idAyuda : undefined }) : children}
      {ayuda && !error && (
        <p id={idAyuda} className="mt-1.5 text-xs text-muted">
          {ayuda}
        </p>
      )}
      {error && (
        <p id={idError} role="alert" className="mt-1.5 flex items-center gap-1 text-xs font-medium text-rose-600 dark:text-rose-400">
          <svg viewBox="0 0 20 20" fill="currentColor" className="h-4 w-4 shrink-0" aria-hidden="true">
            <path
              fillRule="evenodd"
              d="M10 18a8 8 0 1 0 0-16 8 8 0 0 0 0 16Zm.75-11.25a.75.75 0 0 0-1.5 0v3.5a.75.75 0 0 0 1.5 0v-3.5ZM10 14a1 1 0 1 0 0 2 1 1 0 0 0 0-2Z"
              clipRule="evenodd"
            />
          </svg>
          {error}
        </p>
      )}
    </div>
  );
}

/** Campo de texto con estilos consistentes.</summary> */
export function Entrada({ className = '', error, ...props }) {
  return <input className={`campo-base ${error ? 'campo-error' : ''} ${className}`} aria-invalid={Boolean(error) || undefined} {...props} />;
}

/** Área de texto multilínea. */
export function AreaTexto({ className = '', error, rows = 3, ...props }) {
  return (
    <textarea
      rows={rows}
      className={`campo-base resize-y ${error ? 'campo-error' : ''} ${className}`}
      aria-invalid={Boolean(error) || undefined}
      {...props}
    />
  );
}

/** Lista desplegable con marcador de posición opcional. */
export function Seleccion({ className = '', error, children, placeholder, ...props }) {
  return (
    <div className="relative">
      <select className={`campo-base appearance-none pr-9 ${error ? 'campo-error' : ''} ${className}`} aria-invalid={Boolean(error) || undefined} {...props}>
        {placeholder && <option value="">{placeholder}</option>}
        {children}
      </select>
      <svg
        viewBox="0 0 20 20"
        fill="currentColor"
        className="pointer-events-none absolute right-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted"
        aria-hidden="true"
      >
        <path fillRule="evenodd" d="M5.23 7.21a.75.75 0 0 1 1.06.02L10 11.17l3.71-3.94a.75.75 0 1 1 1.08 1.04l-4.25 4.5a.75.75 0 0 1-1.08 0l-4.25-4.5a.75.75 0 0 1 .02-1.06Z" clipRule="evenodd" />
      </svg>
    </div>
  );
}
