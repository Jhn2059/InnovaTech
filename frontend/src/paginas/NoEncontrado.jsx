import { Link } from 'react-router-dom';
import { Boton } from '../componentes/ui/Boton';
import { Icone } from '../componentes/ui/Icone';

/** Página de error 404: orienta al usuario y ofrece una salida clara. */
export function NoEncontrado() {
  return (
    <div className="flex min-h-[60vh] flex-col items-center justify-center gap-4 text-center">
      <div className="flex h-16 w-16 items-center justify-center rounded-2xl bg-brand-50 text-brand-600 dark:bg-brand-500/15 dark:text-brand-300">
        <Icone nombre="alerta" className="h-8 w-8" />
      </div>
      <div>
        <p className="text-sm font-semibold uppercase tracking-wider text-muted">Error 404</p>
        <h1 className="mt-1 text-2xl font-bold text-ink">No encontramos la página solicitada</h1>
        <p className="mx-auto mt-2 max-w-md text-sm text-muted">
          Es posible que el enlace esté desactualizado o que no tenga permisos para ver este módulo.
        </p>
      </div>
      <Link to="/panel">
        <Boton>
          <Icone nombre="panel" className="h-4 w-4" />
          Volver al panel general
        </Boton>
      </Link>
    </div>
  );
}
