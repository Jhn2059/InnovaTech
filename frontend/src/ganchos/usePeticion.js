import { useCallback, useEffect, useRef, useState } from 'react';

/**
 * Hook para ejecutar peticiones a la API con estados de carga y error.
 * Permite recargar los datos tras una mutación y evita estados inconsistentes.
 */
export function usePeticion(cargador, dependencias = [], opciones = {}) {
  const { activo = true, datosIniciales = null } = opciones;

  const [datos, setDatos] = useState(datosIniciales);
  const [cargando, setCargando] = useState(activo);
  const [error, setError] = useState(null);
  const [contador, setContador] = useState(0);
  const montado = useRef(true);
  const referenciaCargador = useRef(cargador);

  referenciaCargador.current = cargador;

  useEffect(() => {
    montado.current = true;
    return () => {
      montado.current = false;
    };
  }, []);

  useEffect(() => {
    if (!activo) {
      setCargando(false);
      return;
    }

    let cancelado = false;
    setCargando(true);
    setError(null);

    Promise.resolve()
      .then(() => referenciaCargador.current())
      .then((respuesta) => {
        if (!cancelado && montado.current) setDatos(respuesta);
      })
      .catch((fallo) => {
        if (!cancelado && montado.current) setError(fallo);
      })
      .finally(() => {
        if (!cancelado && montado.current) setCargando(false);
      });

    return () => {
      cancelado = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [...dependencias, contador, activo]);

  const recargar = useCallback(() => setContador((valor) => valor + 1), []);

  return { datos, cargando, error, recargar, setDatos };
}

/** Retrasa la propagación de un valor: evita peticiones por cada pulsación. */
export function useDebounce(valor, retardo = 350) {
  const [valorRetrasado, setValorRetrasado] = useState(valor);

  useEffect(() => {
    const temporizador = window.setTimeout(() => setValorRetrasado(valor), retardo);
    return () => window.clearTimeout(temporizador);
  }, [valor, retardo]);

  return valorRetrasado;
}
