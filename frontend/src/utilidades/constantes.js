/** Etiquetas y catálogos compartidos por toda la interfaz (consistencia de contenido). */

export const ROLES = {
  ADMINISTRADOR: 1,
  RESPONSABLE_TECNOLOGIA: 2,
  COLABORADOR: 3
};

export const NOMBRES_ROL = {
  1: 'Administrador',
  2: 'Responsable de Tecnología',
  3: 'Colaborador'
};

export const ESTADOS_EQUIPO = {
  DISPONIBLE: 1,
  PRESTADO: 2,
  MANTENIMIENTO: 3,
  BAJA: 4
};

export const NOMBRES_ESTADO_EQUIPO = {
  1: 'Disponible',
  2: 'Prestado',
  3: 'Mantenimiento',
  4: 'Baja'
};

/** Estilo visual asociado a cada estado del equipo (insignias consistentes). */
export const ESTILO_ESTADO_EQUIPO = {
  1: 'bg-emerald-50 text-emerald-700 ring-emerald-600/20 dark:bg-emerald-500/10 dark:text-emerald-300',
  2: 'bg-brand-50 text-brand-700 ring-brand-600/20 dark:bg-brand-500/10 dark:text-brand-300',
  3: 'bg-amber-50 text-amber-700 ring-amber-600/20 dark:bg-amber-500/10 dark:text-amber-300',
  4: 'bg-rose-50 text-rose-700 ring-rose-600/20 dark:bg-rose-500/10 dark:text-rose-300'
};

export const ESTADOS_PRESTAMO = {
  ACTIVO: 1,
  DEVUELTO: 2
};

export const NOMBRES_ESTADO_PRESTAMO = {
  1: 'Activo',
  2: 'Devuelto'
};

export const ESTILO_ESTADO_PRESTAMO = {
  1: 'bg-brand-50 text-brand-700 ring-brand-600/20 dark:bg-brand-500/10 dark:text-brand-300',
  2: 'bg-slate-100 text-slate-700 ring-slate-500/20 dark:bg-white/5 dark:text-slate-300'
};

export const CATEGORIAS_EQUIPO = [
  'Laptop',
  'Desktop',
  'Monitor',
  'Impresora',
  'Tablet',
  'Proyector',
  'Equipo de Red'
];

export const TIPOS_MOVIMIENTO = [
  { valor: 'AltaUsuario', etiqueta: 'Alta de usuario' },
  { valor: 'ActualizacionUsuario', etiqueta: 'Actualización de usuario' },
  { valor: 'CambioEstadoUsuario', etiqueta: 'Cambio de estado de usuario' },
  { valor: 'AltaEquipo', etiqueta: 'Alta de equipo' },
  { valor: 'ActualizacionEquipo', etiqueta: 'Actualización de equipo' },
  { valor: 'CambioEstadoEquipo', etiqueta: 'Cambio de estado de equipo' },
  { valor: 'Prestamo', etiqueta: 'Préstamo' },
  { valor: 'Devolucion', etiqueta: 'Devolución' }
];

/** Roles con permisos de gestión del inventario y de los préstamos. */
export const ROLES_GESTION = [ROLES.ADMINISTRADOR, ROLES.RESPONSABLE_TECNOLOGIA];

export const esGestion = (rol) => ROLES_GESTION.includes(Number(rol));
export const esAdministrador = (rol) => Number(rol) === ROLES.ADMINISTRADOR;

/**
 * Paleta de gráficos por tema (variables CSS) y estilos compartidos de Recharts.
 * Los tonos 600 se usan en tema claro y los 400 en oscuro para mantener 3:1.
 */
export const COLORES_GRAFICO = [
  'rgb(var(--chart-1))',
  'rgb(var(--chart-2))',
  'rgb(var(--chart-3))',
  'rgb(var(--chart-4))'
];

export const ESTILO_TOOLTIP_GRAFICO = {
  borderRadius: 12,
  border: '1px solid rgb(var(--line-strong))',
  backgroundColor: 'rgb(var(--surface))',
  color: 'rgb(var(--ink))',
  fontSize: 13
};

export const ESTILO_LEYENDA_GRAFICO = {
  fontSize: 12,
  color: 'rgb(var(--ink))'
};
