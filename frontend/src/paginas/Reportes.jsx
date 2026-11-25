import { useMemo, useState } from 'react';
import { Bar, BarChart, CartesianGrid, Cell, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import { servicios } from '../api/servicios';
import { usePeticion } from '../ganchos/usePeticion';
import { EncabezadoPagina } from '../componentes/layout/EncabezadoPagina';
import { Tarjeta, TarjetaCuerpo, TarjetaEncabezado } from '../componentes/ui/Tarjeta';
import { Tabla } from '../componentes/ui/Tabla';
import { Boton } from '../componentes/ui/Boton';
import { Cargador, EstadoVacio } from '../componentes/ui/Estados';
import { Icone } from '../componentes/ui/Icone';
import { Insignia } from '../componentes/ui/Insignia';
import { formatearNumero } from '../utilidades/formateadores';
import { COLORES_GRAFICO, ESTILO_TOOLTIP_GRAFICO } from '../utilidades/constantes';

/**
 * Reportes e indicadores (RF-14 … RF-16): equipos por estado, equipos más
 * utilizados y préstamos por área, con exportación a CSV para la Gerencia.
 */
export function Reportes() {
  const { datos: porEstado, cargando: cargandoEstado } = usePeticion(() => servicios.reportes.equiposPorEstado(), []);
  const { datos: masUtilizados, cargando: cargandoUtilizados } = usePeticion(() => servicios.reportes.equiposMasUtilizados(10), []);
  const { datos: porArea, cargando: cargandoArea } = usePeticion(() => servicios.reportes.prestamosPorArea(), []);
  const [mensajeExport, setMensajeExport] = useState(null);

  const datosArea = useMemo(
    () => (porArea ?? []).map((item) => ({ ...item, nombre: item.area })),
    [porArea]
  );

  const exportarCsv = () => {
    const filas = [
      ['Área', 'Total de préstamos', 'Activos', 'Devueltos', 'Promedio de días'],
      ...(porArea ?? []).map((item) => [item.area, item.totalPrestamos, item.prestamosActivos, item.prestamosDevueltos, item.promedioDias])
    ];
    const csv = filas.map((fila) => fila.join(';')).join('\n');
    const blob = new Blob([`﻿${csv}`], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const enlace = document.createElement('a');
    enlace.href = url;
    enlace.download = `techasset-prestamos-por-area-${new Date().toISOString().slice(0, 10)}.csv`;
    enlace.click();
    URL.revokeObjectURL(url);
    setMensajeExport('Reporte exportado en formato CSV.');
    window.setTimeout(() => setMensajeExport(null), 4000);
  };

  const columnasEstado = [
    { clave: 'estadoNombre', titulo: 'Estado', render: (fila) => <span className="font-medium text-ink">{fila.estadoNombre}</span> },
    { clave: 'total', titulo: 'Equipos', render: (fila) => <span className="text-muted">{formatearNumero(fila.total)}</span> },
    {
      clave: 'porcentaje',
      titulo: 'Participación',
      render: (fila) => (
        <div className="flex items-center gap-2">
          <div className="h-2 w-28 overflow-hidden rounded-full bg-surface-3">
            <div className="h-full rounded-full bg-brand-500" style={{ width: `${fila.porcentaje}%` }} />
          </div>
          <span className="text-xs text-muted">{fila.porcentaje}%</span>
        </div>
      )
    }
  ];

  const columnasUtilizados = [
    {
      clave: 'codigoPatrimonial',
      titulo: 'Código',
      render: (fila) => <span className="font-mono text-xs font-semibold text-ink">{fila.codigoPatrimonial}</span>
    },
    {
      clave: 'nombre',
      titulo: 'Equipo',
      render: (fila) => (
        <div className="min-w-0">
          <p className="truncate font-semibold text-ink">{fila.nombre}</p>
          <p className="truncate text-xs text-muted">
            {fila.marca} · {fila.categoria}
          </p>
        </div>
      )
    },
    { clave: 'totalPrestamos', titulo: 'Préstamos', render: (fila) => <span className="font-bold text-ink">{fila.totalPrestamos}</span> },
    { clave: 'diasPromedio', titulo: 'Días promedio', render: (fila) => <span className="text-muted">{fila.diasPromedio}</span> }
  ];

  const columnasArea = [
    { clave: 'area', titulo: 'Área', render: (fila) => <span className="font-medium text-ink">{fila.area}</span> },
    { clave: 'totalPrestamos', titulo: 'Total', render: (fila) => <span className="text-muted">{fila.totalPrestamos}</span> },
    { clave: 'prestamosActivos', titulo: 'Activos', render: (fila) => <Insignia tono="marca">{fila.prestamosActivos}</Insignia> },
    { clave: 'prestamosDevueltos', titulo: 'Devueltos', render: (fila) => <span className="text-muted">{fila.prestamosDevueltos}</span> },
    { clave: 'promedioDias', titulo: 'Días prom.', render: (fila) => <span className="text-muted">{fila.promedioDias}</span> }
  ];

  if (cargandoEstado || cargandoUtilizados || cargandoArea) return <Cargador texto="Preparando reportes…" />;

  return (
    <>
      <EncabezadoPagina
        titulo="Reportes e indicadores"
        descripcion="Análisis del inventario y del uso de equipos para apoyar la toma de decisiones de la Gerencia de Tecnología."
        acciones={
          <Boton variante="secundario" onClick={exportarCsv}>
            <Icone nombre="descargar" className="h-4 w-4" />
            Exportar CSV
          </Boton>
        }
      />

      {mensajeExport && (
        <div role="status" className="mb-4 flex items-center gap-2 rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-2.5 text-sm font-medium text-emerald-700 dark:border-emerald-500/30 dark:bg-emerald-500/10 dark:text-emerald-200">
          <Icone nombre="verificar" className="h-4 w-4" />
          {mensajeExport}
        </div>
      )}

      <div className="grid grid-cols-1 gap-5 lg:grid-cols-2">
        <Tarjeta>
          <TarjetaEncabezado titulo="Equipos por estado" descripcion="Composición actual del inventario (RF-14)" />
          <TarjetaCuerpo>
            <div className="h-64">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={(porEstado ?? []).map((item) => ({ name: item.estadoNombre, total: item.total }))} margin={{ top: 8, right: 8, left: -22, bottom: 4 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke="rgb(var(--line-strong) / 0.45)" vertical={false} />
                  <XAxis dataKey="name" tick={{ fontSize: 12, fill: 'currentColor' }} axisLine={false} tickLine={false} />
                  <YAxis allowDecimals={false} tick={{ fontSize: 12, fill: 'currentColor' }} axisLine={false} tickLine={false} />
                  <Tooltip
                    cursor={{ fill: 'rgb(var(--chart-2) / 0.08)' }}
                    formatter={(valor) => [`${valor} equipos`, 'Total']}
                    contentStyle={ESTILO_TOOLTIP_GRAFICO}
                  />
                  <Bar dataKey="total" radius={[8, 8, 0, 0]} maxBarSize={54} isAnimationActive={false}>
                    {(porEstado ?? []).map((fila, indice) => (
                      <Cell key={fila.estado} fill={COLORES_GRAFICO[indice % COLORES_GRAFICO.length]} />
                    ))}
                  </Bar>
                </BarChart>
              </ResponsiveContainer>
            </div>
          </TarjetaCuerpo>
        </Tarjeta>

        <Tarjeta>
          <TarjetaEncabezado titulo="Préstamos por área" descripcion="Demanda de equipos por área (RF-16)" />
          <TarjetaCuerpo>
            {datosArea.length === 0 ? (
              <EstadoVacio titulo="Sin préstamos registrados" />
            ) : (
              <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart data={datosArea} layout="vertical" margin={{ top: 4, right: 16, left: 12, bottom: 4 }}>
                    <CartesianGrid strokeDasharray="3 3" stroke="rgb(var(--line-strong) / 0.45)" horizontal={false} />
                    <XAxis type="number" allowDecimals={false} tick={{ fontSize: 12, fill: 'currentColor' }} axisLine={false} tickLine={false} />
                    <YAxis type="category" dataKey="nombre" width={92} tick={{ fontSize: 12, fill: 'currentColor' }} axisLine={false} tickLine={false} />
                    <Tooltip
                      cursor={{ fill: 'rgb(var(--chart-2) / 0.08)' }}
                      formatter={(valor) => [`${valor} préstamos`, 'Total']}
                      contentStyle={ESTILO_TOOLTIP_GRAFICO}
                    />
                    <Bar dataKey="totalPrestamos" fill={COLORES_GRAFICO[1]} radius={[0, 8, 8, 0]} maxBarSize={26} isAnimationActive={false} />
                  </BarChart>
                </ResponsiveContainer>
              </div>
            )}
          </TarjetaCuerpo>
        </Tarjeta>
      </div>

      <div className="mt-5 grid grid-cols-1 gap-5 xl:grid-cols-2">
        <Tarjeta>
          <TarjetaEncabezado titulo="Equipos más utilizados" descripcion="Top 10 por número de préstamos (RF-15)" />
          <Tabla
            columnas={columnasUtilizados}
            datos={masUtilizados ?? []}
            cargando={cargandoUtilizados}
            vacio={<EstadoVacio titulo="Sin equipos prestados" descripcion="Todavía no se registran préstamos en el sistema." />}
          />
        </Tarjeta>

        <Tarjeta>
          <TarjetaEncabezado titulo="Detalle por área" descripcion="Totales, préstamos activos y días promedio de préstamo" />
          <Tabla
            columnas={columnasArea}
            datos={porArea ?? []}
            cargando={cargandoArea}
            vacio={<EstadoVacio titulo="Sin información por área" />}
          />
        </Tarjeta>
      </div>

      <Tarjeta className="mt-5">
        <TarjetaEncabezado titulo="Resumen del inventario por estado" descripcion="Detalle numérico de la distribución del inventario" />
        <Tabla columnas={columnasEstado} datos={porEstado ?? []} cargando={cargandoEstado} vacio={<EstadoVacio titulo="Sin equipos registrados" />} />
      </Tarjeta>
    </>
  );
}
