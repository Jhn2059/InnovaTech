import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { limpiarToken, obtenerToken, guardarToken } from '../api/cliente';
import { servicios } from '../api/servicios';

/**
 * Contexto de autenticación: mantiene la sesión, el perfil del usuario y los
 * permisos derivados del rol. Expone además un guardia de rutas.
 */
const ContextoAuth = createContext(null);

export function ProveedorAuth({ children }) {
  const [usuario, setUsuario] = useState(null);
  const [cargando, setCargando] = useState(true);

  const cerrarSesion = useCallback(() => {
    limpiarToken();
    setUsuario(null);
  }, []);

  // Restaura la sesión al recargar la página (evita que el usuario vuelva a autenticarse).
  useEffect(() => {
    const token = obtenerToken();
    if (!token) {
      setCargando(false);
      return;
    }

    servicios.auth
      .perfil()
      .then(setUsuario)
      .catch(() => limpiarToken())
      .finally(() => setCargando(false));
  }, []);

  const iniciarSesion = useCallback(async (credenciales) => {
    const respuesta = await servicios.auth.iniciarSesion(credenciales);
    guardarToken(respuesta.token);
    setUsuario(respuesta.usuario);
    return respuesta.usuario;
  }, []);

  const valor = useMemo(
    () => ({
      usuario,
      cargando,
      autenticado: Boolean(usuario),
      iniciarSesion,
      cerrarSesion,
      esAdministrador: usuario?.rol === 1,
      puedeGestionar: usuario?.rol === 1 || usuario?.rol === 2
    }),
    [usuario, cargando, iniciarSesion, cerrarSesion]
  );

  return <ContextoAuth.Provider value={valor}>{children}</ContextoAuth.Provider>;
}

export function useAuth() {
  const contexto = useContext(ContextoAuth);
  if (!contexto) {
    throw new Error('useAuth debe utilizarse dentro de ProveedorAuth.');
  }
  return contexto;
}
