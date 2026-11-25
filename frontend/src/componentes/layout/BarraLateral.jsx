import { NavLink } from 'react-router-dom';
import { Icone } from '../ui/Icone';
import { IconoInnovatech } from '../marca/LogoInnovatech';
import { useAuth } from '../../contexto/AuthContexto';
import { iniciales } from '../../utilidades/formateadores';
import { NOMBRES_ROL } from '../../utilidades/constantes';

/**
 * Barra lateral de navegación. Muestra únicamente las opciones permitidas para
 * el rol del usuario (reconocimiento de la información y control de la
 * aplicación, heurísticas de Nielsen).
 */
export function BarraLateral({ abierta, cerrar }) {
  const { usuario, puedeGestionar, esAdministrador } = useAuth();

  const enlaces = [
    { a: '/panel', icono: 'panel', titulo: 'Panel general', visible: true },
    { a: '/equipos', icono: 'monitor', titulo: 'Inventario de equipos', visible: true },
    { a: '/prestamos', icono: 'prestamos', titulo: 'Préstamos y devoluciones', visible: true },
    { a: '/reportes', icono: 'graficos', titulo: 'Reportes e indicadores', visible: true },
    { a: '/usuarios', icono: 'usuarios', titulo: 'Usuarios', visible: puedeGestionar },
    { a: '/auditoria', icono: 'reloj', titulo: 'Bitácora de movimientos', visible: puedeGestionar }
  ].filter((enlace) => enlace.visible);

  return (
    <>
      {abierta && <div className="fixed inset-0 z-30 bg-slate-900/50 backdrop-blur-sm lg:hidden" onClick={cerrar} aria-hidden="true" />}

      <aside
        className={`fixed inset-y-0 left-0 z-40 flex w-72 flex-col border-r border-line bg-surface transition-transform duration-200 lg:translate-x-0 ${
          abierta ? 'translate-x-0' : '-translate-x-full'
        }`}
        aria-label="Navegación principal"
      >
        <div className="flex items-center gap-3 border-b border-line px-5 py-4">
          <IconoInnovatech className="h-9 w-9 !text-slate-900 dark:!text-white" />
          <div className="min-w-0">
            <p className="truncate text-base font-extrabold tracking-tight text-ink">TechAsset</p>
            <p className="truncate text-xs text-muted">Innovatech Solutions</p>
          </div>
          <button type="button" onClick={cerrar} className="ml-auto rounded-lg p-1.5 text-muted hover:bg-surface-3 lg:hidden" aria-label="Cerrar menú">
            <Icone nombre="cerrar" />
          </button>
        </div>

        <nav className="flex-1 space-y-1 overflow-y-auto p-3">
          <p className="px-3 pb-2 pt-2 text-[11px] font-bold uppercase tracking-wider text-muted/80">Navegación</p>
          {enlaces.map((enlace) => (
            <NavLink
              key={enlace.a}
              to={enlace.a}
              onClick={cerrar}
              className={({ isActive }) => `enlace-navegacion ${isActive ? 'enlace-navegacion-activo' : ''}`}
            >
              <Icone nombre={enlace.icono} className="h-5 w-5 shrink-0" />
              <span className="truncate">{enlace.titulo}</span>
            </NavLink>
          ))}
        </nav>

        <div className="border-t border-line p-3">
          <div className="flex items-center gap-3 rounded-xl bg-surface-2 px-3 py-2.5">
            <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-brand-100 text-xs font-bold text-brand-700 dark:bg-brand-500/20 dark:text-brand-200">
              {iniciales(usuario?.nombreCompleto)}
            </span>
            <div className="min-w-0">
              <p className="truncate text-sm font-semibold text-ink">{usuario?.nombreCompleto}</p>
              <p className="truncate text-xs text-muted">{NOMBRES_ROL[usuario?.rol] ?? 'Usuario'}</p>
            </div>
          </div>
        </div>
      </aside>
    </>
  );
}
