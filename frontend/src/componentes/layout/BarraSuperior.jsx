import { useEffect, useRef, useState } from 'react';
import { useLocation } from 'react-router-dom';
import { Icone } from '../ui/Icone';
import { useAuth } from '../../contexto/AuthContexto';
import { iniciales } from '../../utilidades/formateadores';
import { NOMBRES_ROL } from '../../utilidades/constantes';

const TITULOS = {
  '/panel': 'Panel general',
  '/equipos': 'Inventario de equipos',
  '/prestamos': 'Préstamos y devoluciones',
  '/usuarios': 'Gestión de usuarios',
  '/reportes': 'Reportes e indicadores',
  '/auditoria': 'Bitácora de movimientos'
};

/**
 * Barra superior: búsqueda contextual, conmutador de tema, menú de usuario y
 * acceso rápido a las acciones principales.
 */
export function BarraSuperior({ abrirMenu, alternarTema, tema }) {
  const { usuario, cerrarSesion } = useAuth();
  const [menuUsuario, setMenuUsuario] = useState(false);
  const referenciaMenu = useRef(null);
  const ubicacion = useLocation();

  useEffect(() => {
    const alPulsar = (evento) => {
      if (referenciaMenu.current && !referenciaMenu.current.contains(evento.target)) {
        setMenuUsuario(false);
      }
    };
    document.addEventListener('mousedown', alPulsar);
    return () => document.removeEventListener('mousedown', alPulsar);
  }, []);

  const titulo = TITULOS[ubicacion.pathname] ?? 'TechAsset';

  return (
    <header className="sticky top-0 z-20 flex h-16 items-center gap-3 border-b border-line bg-surface/90 px-4 backdrop-blur sm:px-6">
      <button
        type="button"
        onClick={abrirMenu}
        className="rounded-lg p-2 text-muted transition hover:bg-surface-3 hover:text-ink lg:hidden"
        aria-label="Abrir menú de navegación"
      >
        <Icone nombre="menu" />
      </button>

      <div className="min-w-0">
        <h1 className="truncate text-base font-bold text-ink sm:text-lg">{titulo}</h1>
      </div>

      <div className="ml-auto flex items-center gap-1.5">
        <button
          type="button"
          onClick={alternarTema}
          className="rounded-lg p-2 text-muted transition hover:bg-surface-3 hover:text-ink"
          aria-label={tema === 'oscuro' ? 'Cambiar a tema claro' : 'Cambiar a tema oscuro'}
          title={tema === 'oscuro' ? 'Tema claro' : 'Tema oscuro'}
        >
          <Icone nombre={tema === 'oscuro' ? 'sol' : 'luna'} />
        </button>

        <div className="relative" ref={referenciaMenu}>
          <button
            type="button"
            onClick={() => setMenuUsuario((abierto) => !abierto)}
            className="flex items-center gap-2 rounded-xl px-2 py-1.5 transition hover:bg-surface-3"
            aria-haspopup="menu"
            aria-expanded={menuUsuario}
          >
            <span className="flex h-8 w-8 items-center justify-center rounded-full bg-brand-100 text-xs font-bold text-brand-700 dark:bg-brand-500/20 dark:text-brand-200">
              {iniciales(usuario?.nombreCompleto)}
            </span>
            <span className="hidden text-left sm:block">
              <span className="block max-w-[9rem] truncate text-sm font-semibold text-ink">{usuario?.nombreCompleto}</span>
              <span className="block text-[11px] text-muted">{NOMBRES_ROL[usuario?.rol] ?? 'Usuario'}</span>
            </span>
            <Icone nombre="abajo" className="hidden h-4 w-4 text-muted sm:block" />
          </button>

          {menuUsuario && (
            <div
              role="menu"
              className="absolute right-0 mt-2 w-64 animate-fade-in-scale overflow-hidden rounded-xl border border-line bg-surface shadow-pop"
            >
              <div className="border-b border-line px-4 py-3">
                <p className="truncate text-sm font-semibold text-ink">{usuario?.nombreCompleto}</p>
                <p className="truncate text-xs text-muted">{usuario?.correo}</p>
                <p className="mt-1 text-xs text-muted">
                  Área: <span className="font-medium text-ink">{usuario?.area}</span>
                </p>
              </div>
              <button
                type="button"
                role="menuitem"
                onClick={cerrarSesion}
                className="flex w-full items-center gap-2 px-4 py-2.5 text-sm font-medium text-rose-600 transition hover:bg-surface-2 dark:text-rose-400"
              >
                <Icone nombre="salir" className="h-4 w-4" />
                Cerrar sesión
              </button>
            </div>
          )}
        </div>
      </div>
    </header>
  );
}
