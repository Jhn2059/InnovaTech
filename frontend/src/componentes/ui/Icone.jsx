/**
 * Catálogo de iconos SVG reutilizables. Centraliza los gráficos para mantener
 * coherencia visual y evitar duplicación de marcado.
 */
const TRAZADOS = {
  panel: 'M3 13h8V3H3v10Zm0 8h8v-6H3v6Zm10 0h8V11h-8v10Zm0-18v6h8V3h-8Z',
  usuarios:
    'M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2M9 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8Zm13 10v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75',
  monitor: 'M4 5a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V5Zm4 16h8m-4-4v4',
  equipos:
    'M12 18V6m0 0 3 3m-3-3-3 3M4 14v4a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-4M4 10V6a2 2 0 0 1 2-2h3',
  prestamos: 'M7 3h10a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2Zm5 5v6m-3-3h6',
  graficos: 'M4 20V10m6 10V4m6 16v-7m4 7H2',
  reportes:
    'M9 3h6a1 1 0 0 1 1 1v1h3a1 1 0 0 1 1 1v14a1 1 0 0 1-1 1H5a1 1 0 0 1-1-1V6a1 1 0 0 1 1-1h3V4a1 1 0 0 1 1-1Zm0 6h6M9 13h6M9 17h4',
  salir: 'M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4m7 14 5-5-5-5m5 5H9',
  sol: 'M12 17a5 5 0 1 0 0-10 5 5 0 0 0 0 10Zm0-14v2m0 18v-2M3 12h2m14 0h2M5.6 5.6 7 7m10 10 1.4 1.4M18.4 5.6 17 7M7 17l-1.4 1.4',
  luna: 'M21 12.8A9 9 0 1 1 11.2 3a7 7 0 0 0 9.8 9.8Z',
  menu: 'M4 6h16M4 12h16M4 18h16',
  cerrar: 'M6 6l12 12M18 6 6 18',
  editar: 'M4 20h4l10-10a2.83 2.83 0 0 0-4-4L4 16v4Zm9-13 4 4',
  basura: 'M4 7h16M10 11v6m4-6v6M5 7l1 13h12l1-13M9 7V4h6v3',
  mas: 'M12 5v14M5 12h14',
  buscar: 'M11 19a8 8 0 1 0 0-16 8 8 0 0 0 0 16Zm10 2-4.35-4.35',
  filtro: 'M3 5h18l-7 8v6l-4 2v-8L3 5Z',
  descargar: 'M12 3v12m0 0 4-4m-4 4-4-4M4 17v2a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-2',
  chevron: 'm9 6 6 6-6 6',
  abajo: 'm6 9 6 6 6-6',
  verificar: 'M12 22a10 10 0 1 0 0-20 10 10 0 0 0 0 20Zm-4-10 3 3 5-6',
  alerta: 'M12 8v5m0 3h.01M10.3 3.9 1.8 18a2 2 0 0 0 1.7 3h16.9a2 2 0 0 0 1.7-3L13.7 3.9a2 2 0 0 0-3.4 0Z',
  reloj: 'M12 21a9 9 0 1 0 0-18 9 9 0 0 0 0 18Zm0-14v5l3 2',
  ubicacion: 'M12 21s7-5.5 7-11a7 7 0 1 0-14 0c0 5.5 7 11 7 11Zm0-8.5a2.5 2.5 0 1 0 0-5 2.5 2.5 0 0 0 0 5Z',
  etiqueta: 'M20.6 13.4 12 22l-9-9V3h10l7.6 7.6a2 2 0 0 1 0 2.8ZM7.5 7.5h.01',
  calendario: 'M7 3v4m10-4v4M3 10h18M5 5h14a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V7a2 2 0 0 1 2-2Z',
  devolucion: 'M9 14 4 9l5-5M4 9h10a6 6 0 0 1 6 6v5',
  cohete: 'M4.5 16.5c-1.5 1.5-1 4 0 5 1 1 3.5 1.5 5 0l2-2-5-5-2 2Zm4-4 5 5 5-5c2-2 3-5 2-8-3-1-6 0-8 2l-4 4Zm8-4h.01',
  ajustes: 'M12 15a3 3 0 1 0 0-6 3 3 0 0 0 0 6Zm7.4-3a7.4 7.4 0 0 0-.1-1l2-1.6-2-3.4-2.4 1a7.5 7.5 0 0 0-1.8-1L14.7 3h-4l-.4 2.5a7.5 7.5 0 0 0-1.8 1l-2.4-1-2 3.4L6 11a7.4 7.4 0 0 0 0 2l-2 1.6 2 3.4 2.4-1a7.5 7.5 0 0 0 1.8 1l.4 2.5h4l.4-2.5a7.5 7.5 0 0 0 1.8-1l2.4 1 2-3.4-2-1.6c.1-.3.1-.7.1-1Z',
  informacion: 'M12 21a9 9 0 1 0 0-18 9 9 0 0 0 0 18Zm0-13h.01M11 12h1v5h1',
  ojo: 'M2.5 12S6 5.5 12 5.5 21.5 12 21.5 12 18 18.5 12 18.5 2.5 12 2.5 12Zm9.5 3a3 3 0 1 0 0-6 3 3 0 0 0 0 6Z',
  ojoTachado: 'M3 3l18 18M10.6 6.1A9.9 9.9 0 0 1 12 6c6 0 9.5 6 9.5 6a17 17 0 0 1-3.2 3.8M6.5 7.8A17 17 0 0 0 2.5 12S6 18 12 18a9.6 9.6 0 0 0 3.5-.6M9.9 9.9a3 3 0 0 0 4.2 4.2'
};

export function Icone({ nombre, className = 'h-5 w-5', relleno = false }) {
  const trazado = TRAZADOS[nombre] ?? TRAZADOS.informacion;
  return (
    <svg
      viewBox="0 0 24 24"
      fill={relleno ? 'currentColor' : 'none'}
      stroke={relleno ? 'none' : 'currentColor'}
      strokeWidth="1.8"
      strokeLinecap="round"
      strokeLinejoin="round"
      className={className}
      aria-hidden="true"
    >
      <path d={trazado} />
    </svg>
  );
}
