import { useCallback, useMemo, useState } from 'react';

/**
 * Hook de paginación con filtros: conserva el estado de la consulta y expone
 * los parámetros listos para enviar a la API.
 */
export function usePaginacion(inicial = {}) {
  const [pagina, setPagina] = useState(inicial.pagina ?? 1);
  const [tamanoPagina, setTamanoPagina] = useState(inicial.tamanoPagina ?? 10);
  const [filtros, setFiltros] = useState(inicial.filtros ?? {});

  const establecerFiltro = useCallback((clave, valor) => {
    setPagina(1);
    setFiltros((actuales) => ({ ...actuales, [clave]: valor }));
  }, []);

  const reiniciar = useCallback(() => {
    setPagina(1);
    setFiltros(inicial.filtros ?? {});
  }, [inicial.filtros]);

  const parametros = useMemo(
    () => ({
      pagina,
      tamanoPagina,
      ...Object.fromEntries(Object.entries(filtros).filter(([, valor]) => valor !== '' && valor !== null && valor !== undefined))
    }),
    [pagina, tamanoPagina, filtros]
  );

  return { pagina, setPagina, tamanoPagina, setTamanoPagina, filtros, setFiltros, establecerFiltro, reiniciar, parametros };
}
