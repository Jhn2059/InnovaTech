/**
 * Validaciones reutilizables del frontend.
 * Devuelven un mensaje en español o `null` cuando el valor es válido, de modo que
 * el mismo conjunto de reglas se aplique en todos los formularios (Actividad 2).
 */

const correoValido = /^[^\s@]+@[^\s@]+\.[A-Za-z]{2,}$/;

export const requerido = (etiqueta = 'Este campo') => (valor) =>
  valor === null || valor === undefined || String(valor).trim() === ''
    ? `${etiqueta} es obligatorio.`
    : null;

export const longitudMinima = (minimo, etiqueta = 'Este campo') => (valor) =>
  valor && String(valor).trim().length < minimo
    ? `${etiqueta} debe tener al menos ${minimo} caracteres.`
    : null;

export const longitudMaxima = (maximo, etiqueta = 'Este campo') => (valor) =>
  valor && String(valor).length > maximo
    ? `${etiqueta} no puede superar los ${maximo} caracteres.`
    : null;

export const correo = (valor) =>
  valor && !correoValido.test(String(valor).trim())
    ? 'Ingrese un correo electrónico válido.'
    : null;

export const soloTexto = (etiqueta = 'Este campo') => (valor) =>
  valor && !/^[\p{L}\p{M}\s'.-]+$/u.test(String(valor).trim())
    ? `${etiqueta} solo admite letras y espacios.`
    : null;

export const codigoPatrimonial = (valor) => {
  if (!valor) return null;
  const texto = String(valor).trim();
  if (!/^[A-Za-z0-9-]+$/.test(texto)) {
    return 'El código patrimonial solo admite letras, números y guiones.';
  }
  return null;
};

export const numeroPositivo = (etiqueta = 'El valor') => (valor) =>
  valor !== null && valor !== undefined && valor !== '' && Number(valor) <= 0
    ? `${etiqueta} debe ser mayor que cero.`
    : null;

export const fechaNoFutura = (etiqueta = 'La fecha') => (valor) =>
  valor && new Date(valor) > new Date()
    ? `${etiqueta} no puede ser futura.`
    : null;

/**
 * Combina un conjunto de reglas en una única función de validación.
 * Devuelve el primer error encontrado o `null`.
 */
export const combinar = (...reglas) => (valor) => {
  for (const regla of reglas) {
    const error = regla(valor);
    if (error) return error;
  }
  return null;
};

/** Reglas reutilizables para el formulario de usuarios. */
export const reglasUsuario = {
  nombres: combinar(requerido('Los nombres'), longitudMaxima(100, 'Los nombres'), soloTexto('Los nombres')),
  apellidos: combinar(requerido('Los apellidos'), longitudMaxima(100, 'Los apellidos'), soloTexto('Los apellidos')),
  correo: combinar(requerido('El correo electrónico'), longitudMaxima(150, 'El correo'), correo),
  area: combinar(requerido('El área'), longitudMaxima(100, 'El área')),
  contrasena: combinar(
    requerido('La contraseña'),
    longitudMinima(8, 'La contraseña'),
    longitudMaxima(100, 'La contraseña'),
    (valor) => (valor && !/[A-Za-z]/.test(valor) ? 'La contraseña debe incluir al menos una letra.' : null),
    (valor) => (valor && !/[0-9]/.test(valor) ? 'La contraseña debe incluir al menos un número.' : null)
  )
};

/** Reglas reutilizables para el formulario de equipos. */
export const reglasEquipo = {
  codigoPatrimonial: combinar(requerido('El código patrimonial'), longitudMaxima(50, 'El código patrimonial'), codigoPatrimonial),
  nombre: combinar(requerido('El nombre'), longitudMaxima(100, 'El nombre')),
  categoria: combinar(requerido('La categoría'), longitudMaxima(50, 'La categoría')),
  marca: combinar(requerido('La marca'), longitudMaxima(50, 'La marca')),
  ubicacion: longitudMaxima(120, 'La ubicación'),
  numeroSerie: longitudMaxima(60, 'El número de serie'),
  especificaciones: longitudMaxima(500, 'Las especificaciones'),
  fechaAdquisicion: fechaNoFutura('La fecha de adquisición')
};

/** Reglas reutilizables para el formulario de préstamo. */
export const reglasPrestamo = {
  usuarioId: requerido('El colaborador'),
  equipoId: requerido('El equipo'),
  fechaPrestamo: fechaNoFutura('La fecha de préstamo'),
  observacion: longitudMaxima(500, 'La observación')
};

/** Reglas reutilizables para el formulario de devolución (RN-10). */
export const reglasDevolucion = {
  fechaDevolucion: fechaNoFutura('La fecha de devolución'),
  observacion: combinar(
    requerido('La observación de devolución'),
    longitudMinima(5, 'La observación de devolución'),
    longitudMaxima(500, 'La observación de devolución')
  )
};
