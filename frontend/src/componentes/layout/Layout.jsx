import { useEffect, useState } from 'react';
import { Outlet } from 'react-router-dom';
import { BarraLateral } from './BarraLateral';
import { BarraSuperior } from './BarraSuperior';
import { Notificaciones } from '../ui/Notificaciones';
import { useTema } from '../../ganchos/useTema';

/**
 * Estructura general de la aplicación (shell): barra lateral, barra superior y
 * área de contenido responsiva. El diseño se adapta a móvil, tableta y escritorio.
 */
export function Layout() {
  const [menuAbierto, setMenuAbierto] = useState(false);
  const { tema, alternarTema } = useTema();

  useEffect(() => {
    const alRedimensionar = () => {
      if (window.innerWidth >= 1024) setMenuAbierto(false);
    };
    window.addEventListener('resize', alRedimensionar);
    return () => window.removeEventListener('resize', alRedimensionar);
  }, []);

  return (
    <div className="min-h-screen bg-surface-2">
      <a
        href="#contenido-principal"
        className="sr-only focus:not-sr-only focus:absolute focus:left-4 focus:top-4 focus:z-50 focus:rounded-lg focus:bg-brand-600 focus:px-4 focus:py-2 focus:text-sm focus:font-semibold focus:text-white"
      >
        Saltar al contenido principal
      </a>

      <BarraLateral abierta={menuAbierto} cerrar={() => setMenuAbierto(false)} />

      <div className="lg:pl-72">
        <BarraSuperior abrirMenu={() => setMenuAbierto(true)} alternarTema={alternarTema} tema={tema} />
        <main id="contenido-principal" className="px-4 py-6 sm:px-6 lg:px-8">
          <div className="mx-auto max-w-[90rem]">
            <Outlet />
          </div>
        </main>
      </div>

      <Notificaciones />
    </div>
  );
}
