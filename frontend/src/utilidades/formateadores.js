/** Formateadores reutilizables de presentación (fechas, números e iniciales). */

const MESES = ['enero', 'febrero', 'marzo', 'abril', 'mayo', 'junio', 'julio', 'agosto', 'septiembre', 'octubre', 'noviembre', 'diciembre'];

const sinDesplazamiento = (valor) => {
  if (!valor) return null;
  const fecha = new Date(valor);
  return Number.isNaN(fecha.getTime()) ? null : fecha;
};

/** Formatea una fecha de negocio (sin zona horaria) en dd/mm/aaaa. */
export function formatearFecha(valor) {
  if (!valor) return '—';
  const fecha = new Date(valor);
  if (Number.isNaN(fecha.getTime())) return '—';
  const dia = String(fecha.getDate()).padStart(2, '0');
  const mes = String(fecha.getMonth() + 1).padStart(2, '0');
  return `${dia}/${mes}/${fecha.getFullYear()}`;
}

/** Formatea fecha y hora en dd/mm/aaaa hh:mm. */
export function formatearFechaHora(valor) {
  if (!valor) return '—';
  const fecha = sinDesplazamiento(valor);
  if (!fecha) return '—';
  const hora = String(fecha.getHours()).padStart(2, '0');
  const minuto = String(fecha.getMinutes()).padStart(2, '0');
  return `${formatearFecha(valor)} ${hora}:${minuto}`;
}

/** Formatea una fecha en texto largo: "17 de noviembre de 2026". */
export function formatearFechaLarga(valor) {
  if (!valor) return '—';
  const fecha = new Date(valor);
  if (Number.isNaN(fecha.getTime())) return '—';
  return `${fecha.getDate()} de ${MESES[fecha.getMonth()]} de ${fecha.getFullYear()}`;
}

/** Tiempo transcurrido en lenguaje natural ("hace 3 días"). */
export function tiempoRelativo(valor) {
  const fecha = sinDesplazamiento(valor);
  if (!fecha) return '—';

  const segundos = Math.floor((Date.now() - fecha.getTime()) / 1000);
  if (segundos < 60) return 'hace un momento';
  if (segundos < 3600) return `hace ${Math.floor(segundos / 60)} min`;
  if (segundos < 86400) return `hace ${Math.floor(segundos / 3600)} h`;
  const dias = Math.floor(segundos / 86400);
  if (dias === 1) return 'ayer';
  if (dias < 30) return `hace ${dias} días`;
  const meses = Math.floor(dias / 30);
  if (meses < 12) return `hace ${meses} ${meses === 1 ? 'mes' : 'meses'}`;
  const anios = Math.floor(meses / 12);
  return `hace ${anios} ${anios === 1 ? 'año' : 'años'}`;
}

/** Número con separador de miles. */
export function formatearNumero(valor) {
  if (valor === null || valor === undefined || Number.isNaN(valor)) return '0';
  return new Intl.NumberFormat('es-PE').format(valor);
}

/** Porcentaje con un decimal. */
export function formatearPorcentaje(valor) {
  if (valor === null || valor === undefined || Number.isNaN(valor)) return '0%';
  return `${Number(valor).toFixed(1).replace('.', ',')}%`;
}

/** Iniciales de un nombre completo (para el avatar). */
export function iniciales(nombre = '') {
  const partes = nombre.trim().split(/\s+/).filter(Boolean);
  if (partes.length === 0) return '?';
  if (partes.length === 1) return partes[0].slice(0, 2).toUpperCase();
  return `${partes[0][0]}${partes[partes.length - 1][0]}`.toUpperCase();
}

/** Fecha de hoy en formato yyyy-MM-dd (para los campos de tipo date). */
export function hoyISO() {
  const fecha = new Date();
  const mes = String(fecha.getMonth() + 1).padStart(2, '0');
  const dia = String(fecha.getDate()).padStart(2, '0');
  return `${fecha.getFullYear()}-${mes}-${dia}`;
}

/** Convierte un valor a yyyy-MM-dd (para inputs de fecha). */
export function aISO(valor) {
  if (!valor) return '';
  const fecha = new Date(valor);
  if (Number.isNaN(fecha.getTime())) return '';
  const mes = String(fecha.getMonth() + 1).padStart(2, '0');
  const dia = String(fecha.getDate()).padStart(2, '0');
  return `${fecha.getFullYear()}-${mes}-${dia}`;
}
