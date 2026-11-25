import { clienteHttp } from './cliente';

/**
 * Servicios de la API: un único lugar donde se conoce el contrato HTTP.
 * Las páginas consumen funciones de negocio, nunca URLs (reutilización).
 */
export const servicios = {
  auth: {
    iniciarSesion: async (credenciales) => (await clienteHttp.post('/auth/login', credenciales)).data,
    perfil: async () => (await clienteHttp.get('/auth/perfil')).data
  },

  usuarios: {
    listar: async (params) => (await clienteHttp.get('/usuarios', { params })).data,
    obtener: async (id) => (await clienteHttp.get(`/usuarios/${id}`)).data,
    crear: async (datos) => (await clienteHttp.post('/usuarios', datos)).data,
    actualizar: async (id, datos) => (await clienteHttp.put(`/usuarios/${id}`, datos)).data,
    cambiarEstado: async (id, datos) => (await clienteHttp.patch(`/usuarios/${id}/estado`, datos)).data,
    areas: async () => (await clienteHttp.get('/usuarios/areas')).data
  },

  equipos: {
    listar: async (params) => (await clienteHttp.get('/equipos', { params })).data,
    obtener: async (id) => (await clienteHttp.get(`/equipos/${id}`)).data,
    crear: async (datos) => (await clienteHttp.post('/equipos', datos)).data,
    actualizar: async (id, datos) => (await clienteHttp.put(`/equipos/${id}`, datos)).data,
    cambiarEstado: async (id, datos) => (await clienteHttp.patch(`/equipos/${id}/estado`, datos)).data,
    eliminar: async (id) => (await clienteHttp.delete(`/equipos/${id}`)).data,
    categorias: async () => (await clienteHttp.get('/equipos/categorias')).data,
    marcas: async () => (await clienteHttp.get('/equipos/marcas')).data
  },

  prestamos: {
    listar: async (params) => (await clienteHttp.get('/prestamos', { params })).data,
    activos: async () => (await clienteHttp.get('/prestamos/activos')).data,
    obtener: async (id) => (await clienteHttp.get(`/prestamos/${id}`)).data,
    registrar: async (datos) => (await clienteHttp.post('/prestamos', datos)).data,
    registrarDevolucion: async (id, datos) => (await clienteHttp.post(`/prestamos/${id}/devolucion`, datos)).data
  },

  reportes: {
    dashboard: async () => (await clienteHttp.get('/reportes/dashboard')).data,
    equiposPorEstado: async () => (await clienteHttp.get('/reportes/equipos-por-estado')).data,
    equiposMasUtilizados: async (tope = 10) => (await clienteHttp.get('/reportes/equipos-mas-utilizados', { params: { tope } })).data,
    prestamosPorArea: async () => (await clienteHttp.get('/reportes/prestamos-por-area')).data,
    movimientos: async (params) => (await clienteHttp.get('/reportes/movimientos', { params })).data
  }
};
