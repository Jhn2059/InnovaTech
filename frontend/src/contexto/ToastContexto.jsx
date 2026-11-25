import { createContext, useCallback, useContext, useMemo, useRef, useState } from 'react';

/**
 * Contexto de notificaciones (toasts). Permite informar al usuario del resultado
 * de cada acción: la visibilidad del estado del sistema es una heurística
 * clave de Nielsen y un principio de Norman (realimentación).
 */
const ContextoToast = createContext(null);

export function ProveedorToast({ children }) {
  const [notificaciones, setNotificaciones] = useState([]);
  const contador = useRef(0);

  const descartar = useCallback((id) => {
    setNotificaciones((actuales) => actuales.filter((n) => n.id !== id));
  }, []);

  const notificar = useCallback(
    (mensaje, tipo = 'exito', duracion = 4500) => {
      contador.current += 1;
      const id = contador.current;
      setNotificaciones((actuales) => [...actuales, { id, mensaje, tipo }]);
      if (duracion > 0) {
        window.setTimeout(() => descartar(id), duracion);
      }
      return id;
    },
    [descartar]
  );

  const valor = useMemo(
    () => ({
      notificaciones,
      descartar,
      exito: (mensaje) => notificar(mensaje, 'exito'),
      error: (mensaje) => notificar(mensaje, 'error', 6500),
      advertencia: (mensaje) => notificar(mensaje, 'advertencia', 5500),
      info: (mensaje) => notificar(mensaje, 'info')
    }),
    [notificaciones, descartar, notificar]
  );

  return <ContextoToast.Provider value={valor}>{children}</ContextoToast.Provider>;
}

export function useToast() {
  const contexto = useContext(ContextoToast);
  if (!contexto) {
    throw new Error('useToast debe utilizarse dentro de ProveedorToast.');
  }
  return contexto;
}
