import { useEffect, useRef } from 'react';
import { createPortal } from 'react-dom';
import { Boton } from './Boton';

const ANCHOS = {
  pequeno: 'max-w-md',
  mediano: 'max-w-2xl',
  grande: 'max-w-4xl'
};

/**
 * Modal accesible: bloqueo del scroll, cierre con Escape, foco inicial y
 * devolución del foco al elemento que lo abrió (principio de Norman:
 * el usuario mantiene el control y el contexto).
 */
export function Modal({ abierto, cerrar, titulo, descripcion, ancho = 'mediano', children, pie, cerrarAlPulsarFondo = true }) {
  const contenedor = useRef(null);
  const botonCerrar = useRef(null);

  useEffect(() => {
    if (!abierto) return undefined;

    const elementoAnterior = document.activeElement;
    const overflowAnterior = document.body.style.overflow;
    document.body.style.overflow = 'hidden';

    const alPulsar = (evento) => {
      if (evento.key === 'Escape') cerrar?.();
    };
    document.addEventListener('keydown', alPulsar);

    window.setTimeout(() => botonCerrar.current?.focus(), 30);

    return () => {
      document.removeEventListener('keydown', alPulsar);
      document.body.style.overflow = overflowAnterior;
      if (elementoAnterior instanceof HTMLElement) elementoAnterior.focus();
    };
  }, [abierto, cerrar]);

  if (!abierto) return null;

  return createPortal(
    <div className="fixed inset-0 z-50 flex items-end justify-center overflow-y-auto p-0 sm:items-center sm:p-6">
      <div
        className="fixed inset-0 bg-slate-900/50 backdrop-blur-sm animate-fade-in"
        onClick={cerrarAlPulsarFondo ? cerrar : undefined}
        aria-hidden="true"
      />
      <div
        ref={contenedor}
        role="dialog"
        aria-modal="true"
        aria-label={titulo}
        className={`relative z-10 w-full ${ANCHOS[ancho]} animate-fade-in-scale overflow-hidden rounded-t-2xl bg-surface shadow-pop sm:rounded-2xl`}
      >
        <header className="flex items-start justify-between gap-4 border-b border-line px-5 py-4">
          <div>
            <h2 className="text-lg font-bold text-ink">{titulo}</h2>
            {descripcion && <p className="mt-1 text-sm text-muted">{descripcion}</p>}
          </div>
          <button
            ref={botonCerrar}
            type="button"
            onClick={cerrar}
            className="rounded-lg p-1.5 text-muted transition hover:bg-surface-3 hover:text-ink"
            aria-label="Cerrar ventana"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="h-5 w-5" aria-hidden="true">
              <path d="M6 6l12 12M18 6L6 18" strokeLinecap="round" />
            </svg>
          </button>
        </header>

        <div className="max-h-[70vh] overflow-y-auto px-5 py-4">{children}</div>

        {pie && <footer className="flex flex-wrap items-center justify-end gap-2 border-t border-line bg-surface-2 px-5 py-3">{pie}</footer>}
      </div>
    </div>,
    document.body
  );
}

/** Diálogo de confirmación para acciones destructivas o irreversibles. */
export function ConfirmarDialogo({
  abierto,
  cerrar,
  confirmar,
  titulo = '¿Confirmar la acción?',
  descripcion,
  mensaje,
  textoConfirmar = 'Confirmar',
  textoCancelar = 'Cancelar',
  peligro = false,
  cargando = false
}) {
  return (
    <Modal
      abierto={abierto}
      cerrar={cerrar}
      titulo={titulo}
      descripcion={descripcion}
      ancho="pequeno"
      pie={
        <>
          <Boton variante="secundario" onClick={cerrar} disabled={cargando}>
            {textoCancelar}
          </Boton>
          <Boton variante={peligro ? 'peligro' : 'primario'} onClick={confirmar} cargando={cargando}>
            {textoConfirmar}
          </Boton>
        </>
      }
    >
      {mensaje && <p className="text-sm leading-relaxed text-ink">{mensaje}</p>}
    </Modal>
  );
}
