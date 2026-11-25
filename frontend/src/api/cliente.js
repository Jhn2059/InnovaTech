import axios from 'axios';

const TOKEN_KEY = 'techasset.token';

/** Cliente HTTP único de la aplicación: punto de control de la comunicación con la API. */
export const clienteHttp = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '/api',
  headers: { 'Content-Type': 'application/json' },
  timeout: 20000
});

export function guardarToken(token) {
  localStorage.setItem(TOKEN_KEY, token);
}

export function obtenerToken() {
  return localStorage.getItem(TOKEN_KEY);
}

export function limpiarToken() {
  localStorage.removeItem(TOKEN_KEY);
}

/* Interceptor de petición: adjunta el token JWT (heurística de Nielsen:
   el usuario no debe perceives el trabajo técnico de autenticación). */
clienteHttp.interceptors.request.use((configuracion) => {
  const token = obtenerToken();
  if (token) {
    configuracion.headers.Authorization = `Bearer ${token}`;
  }
  return configuracion;
});

/* Interceptor de respuesta: normaliza los errores de la API (ProblemDetails)
   a un formato uniforme que la interfaz pueda mostrar sin contexto adicional. */
clienteHttp.interceptors.response.use(
  (respuesta) => respuesta,
  (error) => {
    const datos = error.response?.data;
    const mensaje = datos?.detail || datos?.title || 'Ocurrió un error inesperado. Intente nuevamente.';
    const codigo = datos?.codigo || 'ERROR';

    if (error.response?.status === 401) {
      limpiarToken();
      if (!window.location.pathname.includes('/login')) {
        window.location.assign('/login?sesion=expirada');
      }
    }

    return Promise.reject(
      Object.assign(new Error(mensaje), {
        codigo,
        estado: error.response?.status ?? 0,
        errores: datos?.errors ?? null,
        detalle: datos?.detail ?? null,
        titulo: datos?.title ?? 'Error'
      })
    );
  }
);
