import { Boton } from './Boton';

/** Control de paginación con información de rango y navegación accesible. */
export function Paginacion({ pagina, totalPaginas, totalRegistros, tamanoPagina, onCambioPagina, onCambioTamano, etiqueta = 'registros' }) {
  if (totalRegistros === 0) return null;

  const desde = (pagina - 1) * tamanoPagina + 1;
  const hasta = Math.min(pagina * tamanoPagina, totalRegistros);

  const paginas = construirPaginas(pagina, totalPaginas);

  return (
    <div className="flex flex-col items-center justify-between gap-3 border-t border-line px-4 py-3 sm:flex-row">
      <div className="flex items-center gap-3 text-sm text-muted">
        <span>
          Mostrando <span className="font-semibold text-ink">{desde}</span>–<span className="font-semibold text-ink">{hasta}</span> de{' '}
          <span className="font-semibold text-ink">{totalRegistros}</span> {etiqueta}
        </span>
        {onCambioTamano && (
          <select
            value={tamanoPagina}
            onChange={(evento) => onCambioTamano(Number(evento.target.value))}
            className="rounded-lg border border-line bg-surface px-2 py-1 text-xs font-medium text-ink focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-500/25"
            aria-label="Registros por página"
          >
            {[10, 25, 50, 100].map((opcion) => (
              <option key={opcion} value={opcion}>
                {opcion} por página
              </option>
            ))}
          </select>
        )}
      </div>

      <nav className="flex items-center gap-1" aria-label="Paginación">
        <Boton variante="secundario" tamano="pequeno" onClick={() => onCambioPagina(pagina - 1)} disabled={pagina <= 1} aria-label="Página anterior">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="h-4 w-4" aria-hidden="true">
            <path d="m15 18-6-6 6-6" strokeLinecap="round" strokeLinejoin="round" />
          </svg>
        </Boton>

        {paginas.map((item, indice) =>
          item === '...' ? (
            <span key={`puntos-${indice}`} className="px-1.5 text-sm text-muted" aria-hidden="true">
              …
            </span>
          ) : (
            <button
              key={item}
              type="button"
              onClick={() => onCambioPagina(item)}
              aria-current={item === pagina ? 'page' : undefined}
              className={`h-8 min-w-8 rounded-lg px-2 text-sm font-semibold transition ${
                item === pagina ? 'bg-brand-600 text-white' : 'text-muted hover:bg-surface-3 hover:text-ink'
              }`}
            >
              {item}
            </button>
          )
        )}

        <Boton variante="secundario" tamano="pequeno" onClick={() => onCambioPagina(pagina + 1)} disabled={pagina >= totalPaginas} aria-label="Página siguiente">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="h-4 w-4" aria-hidden="true">
            <path d="m9 18 6-6-6-6" strokeLinecap="round" strokeLinejoin="round" />
          </svg>
        </Boton>
      </nav>
    </div>
  );
}

/** Genera el conjunto de páginas visibles con elipsis. */
function construirPaginas(pagina, totalPaginas) {
  if (totalPaginas <= 7) return Array.from({ length: totalPaginas }, (_, indice) => indice + 1);

  const paginas = [1];
  const inicio = Math.max(2, pagina - 1);
  const fin = Math.min(totalPaginas - 1, pagina + 1);

  if (inicio > 2) paginas.push('...');
  for (let indice = inicio; indice <= fin; indice += 1) paginas.push(indice);
  if (fin < totalPaginas - 1) paginas.push('...');
  paginas.push(totalPaginas);

  return paginas;
}
