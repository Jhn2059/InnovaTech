import { useCallback, useEffect, useState } from 'react';

const CLAVE_TEMA = 'techasset.tema';

export function useTema() {
  const [tema, setTema] = useState(() => {
    try {
      return localStorage.getItem(CLAVE_TEMA) || 'claro';
    } catch {
      return 'claro';
    }
  });

  useEffect(() => {
    document.documentElement.classList.toggle('dark', tema === 'oscuro');
    try {
      localStorage.setItem(CLAVE_TEMA, tema);
    } catch {}
  }, [tema]);

  const alternarTema = useCallback(() => {
    setTema((actual) => (actual === 'claro' ? 'oscuro' : 'claro'));
  }, []);

  return { tema, alternarTema };
}
