import { forwardRef } from 'react';
import { clasesBoton } from './botonClases';

/**
 * Botón reutilizable con variantes y tamaños consistentes
 * (heurística de Nielsen: consistencia y estándares).
 */
export const Boton = forwardRef(function Boton(
  { variante = 'primario', tamano = 'medio', className = '', tipo = 'button', cargando = false, disabled, children, ...props },
  referencia
) {
  return (
    <button
      ref={referencia}
      type={tipo}
      disabled={disabled || cargando}
      aria-busy={cargando || undefined}
      className={clasesBoton({ variante, tamano, className })}
      {...props}
    >
      {cargando && (
        <span
          className="h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent"
          aria-hidden="true"
        />
      )}
      {children}
    </button>
  );
});
