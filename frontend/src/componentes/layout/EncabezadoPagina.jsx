import { Icone } from '../ui/Icone';

/** Encabezado de página con título, descripción y acciones (contexto permanente). */
export function EncabezadoPagina({ titulo, descripcion, acciones, migas }) {
  return (
    <div className="mb-5 flex flex-wrap items-end justify-between gap-4">
      <div className="min-w-0">
        {migas && (
          <nav className="mb-1.5 flex items-center gap-1 text-xs text-muted" aria-label="Ruta de navegación">
            {migas.map((miga, indice) => (
              <span key={miga.etiqueta} className="flex items-center gap-1">
                {indice > 0 && <Icone nombre="chevron" className="h-3 w-3 opacity-50" />}
                <span className={indice === migas.length - 1 ? 'font-medium text-ink' : ''}>{miga.etiqueta}</span>
              </span>
            ))}
          </nav>
        )}
        <h2 className="text-xl font-bold tracking-tight text-ink sm:text-2xl">{titulo}</h2>
        {descripcion && <p className="mt-1 max-w-2xl text-sm text-muted">{descripcion}</p>}
      </div>
      {acciones && <div className="flex flex-wrap items-center gap-2">{acciones}</div>}
    </div>
  );
}
