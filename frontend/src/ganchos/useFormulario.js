import { useCallback, useState } from 'react';

/**
 * Hook de formulario reutilizable: gestiona valores, validación por campo,
 * campos tocados, estado de envío y reinicio. Evita duplicar la lógica de
 * validación en cada pantalla (refactorización y reutilización).
 */
export function useFormulario(valoresIniciales = {}, reglas = {}) {
  const [valores, setValores] = useState(valoresIniciales);
  const [errores, setErrores] = useState({});
  const [tocados, setTocados] = useState({});
  const [enviando, setEnviando] = useState(false);
  const [enviado, setEnviado] = useState(false);

  const validarCampo = useCallback(
    (campo, valor) => {
      const regla = reglas[campo];
      if (!regla) return null;
      const error = regla(valor);
      setErrores((actuales) => {
        const copia = { ...actuales };
        if (error) copia[campo] = error;
        else delete copia[campo];
        return copia;
      });
      return error;
    },
    [reglas]
  );

  const cambiar = useCallback(
    (campo, valor) => {
      setValores((actuales) => ({ ...actuales, [campo]: valor }));
      setEnviado(false);
      if (tocados[campo]) validarCampo(campo, valor);
    },
    [tocados, validarCampo]
  );

  const marcarTocado = useCallback(
    (campo) => {
      setTocados((actuales) => ({ ...actuales, [campo]: true }));
      validarCampo(campo, valores[campo]);
    },
    [valores, validarCampo]
  );

  const validarTodo = useCallback(() => {
    const nuevosErrores = {};
    Object.keys(reglas).forEach((campo) => {
      const error = reglas[campo](valores[campo]);
      if (error) nuevosErrores[campo] = error;
    });
    setErrores(nuevosErrores);
    setTocados(Object.keys(reglas).reduce((acumulado, campo) => ({ ...acumulado, [campo]: true }), {}));
    return Object.keys(nuevosErrores).length === 0;
  }, [reglas, valores]);

  const establecerValores = useCallback((nuevos) => {
    setValores(nuevos);
    setErrores({});
    setTocados({});
    setEnviado(false);
  }, []);

  const reiniciar = useCallback(() => {
    setValores(valoresIniciales);
    setErrores({});
    setTocados({});
    setEnviado(false);
  }, [valoresIniciales]);

  /**
   * Ejecuta la acción de envío. Si la validación falla no se llama al
   * backend y se devuelven los errores por campo.
   */
  const enviar = useCallback(
    async (accion) => {
      setEnviando(true);
      setEnviado(false);
      const esValido = validarTodo();
      if (!esValido) {
        setEnviando(false);
        return { exito: false, errores };
      }

      try {
        const resultado = await accion(valores);
        setEnviado(true);
        return { exito: true, resultado };
      } finally {
        setEnviando(false);
      }
    },
    [validarTodo, valores, errores]
  );

  return {
    valores,
    errores,
    tocados,
    enviando,
    enviado,
    cambiar,
    marcarTocado,
    validarTodo,
    establecerValores,
    reiniciar,
    enviar
  };
}
