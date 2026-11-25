import { useMemo } from 'react';
import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Legend,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis
} from 'recharts';
import { servicios } from '../api/servicios';
import { usePeticion } from '../ganchos/usePeticion';
import { EncabezadoPagina } from '../componentes/layout/EncabezadoPagina';
import { Tarjeta, TarjetaCuerpo, TarjetaEncabezado } from '../componentes/ui/Tarjeta';
import { TarjetaEstadistica } from '../componentes/ui/TarjetaEstadistica';
import { Cargador, EstadoVacio } from '../componentes/ui/Estados';
import { Icone } from '../componentes/ui/Icone';
import { Insignia } from '../componentes/ui/Insignia';
import { Boton } from '../componentes/ui/Boton';
import { useAuth } from '../contexto/AuthContexto';
import { formatearNumero, formatearPorcentaje, tiempoRelativo } from '../utilidades/formateadores';
import { COLORES_GRAFICO, ESTILO_LEYENDA_GRAFICO, ESTILO_TOOLTIP_GRAFICO } from '../utilidades/constantes';

/**
 * Panel general (RF-13): indicadores mínimos del caso práctico más el detalle
 * del estado del inventario, la demanda por área y los últimos movimientos.
 */
export function Panel() {
  const { usuario, puedeGestionar } = useAuth();
  const { datos, cargando, error, recargar } = usePeticion(() => servicios.reportes.dashboard(), []);

  const indicadores = useMemo(
    () => [
      {
        titulo: 'Total de equipos',
        valor: formatearNumero(datos?.totalEquipos),
        descripcion: `${formatearNumero(datos?.totalUsuarios)} colaboradores registrados`,
        icono: <Icone nombre="monitor" className="h-5 w-5" />,
        tono: 'marca'
      },
      {
        titulo: 'Equipos disponibles',
        valor: formatearNumero(datos?.equiposDisponibles),
        descripcion: 'Listos para ser prestados',
        icono: <Icone nombre="verificar" className="h-5 w-5" />,
        tono: 'exito'
      },
      {
        titulo: 'Equipos prestados',
        valor: formatearNumero(datos?.equiposPrestados),
        descripcion: `Tasa de ocupación: ${formatearPorcentaje(datos?.tasaOcupacion)}`,
        icono: <Icone nombre="prestamos" className="h-5 w-5" />,
        tono: 'marca',
        progreso: datos?.tasaOcupacion,
        pie: `${formatearPorcentaje(datos?.tasaOcupacion)} del inventario está asignado`
      },
      {
        titulo: 'En mantenimiento',
        valor: formatearNumero(datos?.equiposMantenimiento),
        descripcion: `${formatearNumero(datos?.equiposBaja)} equipos dados de baja`,
        icono: <Icone nombre="ajustes" className="h-5 w-5" />,
        tono: 'aviso'
      },
      {
        titulo: 'Total de préstamos',
        valor: formatearNumero(datos?.totalPrestamos),
        descripcion: `${formatearNumero(datos?.prestamosActivos)} activos · ${formatearNumero(datos?.prestamosDevueltos)} devueltos`,
        icono: <Icone nombre="reloj" className="h-5 w-5" />,
        tono: 'info'
      },
      {
        titulo: 'Área con mayor uso',
        valor: datos?.areaConMayorUso ?? '—',
        descripcion: `${formatearNumero(datos?.prestamosAreaMayorUso)} préstamos registrados`,
        icono: <Icone nombre="usuarios" className="h-5 w-5" />,
        tono: 'neutro'
      }
    ],
    [datos]
  );

  const datosEstado = useMemo(
    () => (datos?.equiposPorEstado ?? []).map((item) => ({ name: item.etiqueta, value: item.valor })),
    [datos]
  );

  const datosArea = useMemo(
    () => (datos?.prestamosPorArea ?? []).map((item) => ({ name: item.etiqueta, prestamos: item.valor })),
    [datos]
  );

  const datosMes = useMemo(
    () => (datos?.prestamosPorMes ?? []).map((item) => ({ name: item.etiqueta, prestamos: item.valor })),
    [datos]
  );

  if (cargando) return <Cargador texto="Cargando indicadores…" />;

  if (error) {
    return (
      <EstadoVacio
        titulo="No fue posible cargar el panel"
        descripcion={error.message}
        icono={<Icone nombre="alerta" className="h-7 w-7" />}
        accion={<Boton onClick={recargar}>Reintentar</Boton>}
      />
    );
  }

  return (
    <>
      <EncabezadoPagina
        titulo={`Hola, ${usuario?.nombres ?? ''}`}
        descripcion="Resumen del inventario tecnológico y del uso de equipos en Innovatech Solutions."
        acciones={
          <Boton variante="secundario" onClick={recargar}>
            <Icone nombre="reloj" className="h-4 w-4" />
            Actualizar
          </Boton>
        }
      />

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-6">
        {indicadores.map((indicador) => (
          <TarjetaEstadistica key={indicador.titulo} {...indicador} />
        ))}
      </div>

      <div className="mt-5 grid grid-cols-1 gap-5 lg:grid-cols-3">
        <Tarjeta className="lg:col-span-1">
          <TarjetaEncabezado titulo="Equipos por estado" descripcion="Distribución actual del inventario" />
          <TarjetaCuerpo>
            {datosEstado.length === 0 ? (
              <EstadoVacio titulo="Sin equipos registrados" descripcion="Registre equipos para visualizar su distribución." />
            ) : (
              <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie data={datosEstado} dataKey="value" nameKey="name" innerRadius={58} outerRadius={92} paddingAngle={3} stroke="none" isAnimationActive={false}>
                      {datosEstado.map((entrada, indice) => (
                        <Cell key={entrada.name} fill={COLORES_GRAFICO[indice % COLORES_GRAFICO.length]} />
                      ))}
                    </Pie>
                    <Tooltip
                      formatter={(valor) => [`${valor} equipos`, 'Total']}
                      contentStyle={ESTILO_TOOLTIP_GRAFICO}
                    />
                    <Legend verticalAlign="bottom" height={32} iconType="circle" wrapperStyle={ESTILO_LEYENDA_GRAFICO} />
                  </PieChart>
                </ResponsiveContainer>
              </div>
            )}
          </TarjetaCuerpo>
        </Tarjeta>

        <Tarjeta className="lg:col-span-2">
          <TarjetaEncabezado titulo="Préstamos por área" descripcion="Áreas con mayor demanda de equipos tecnológicos" />
          <TarjetaCuerpo>
            {datosArea.length === 0 ? (
              <EstadoVacio titulo="Sin préstamos registrados" descripcion="Los préstamos por área aparecerán aquí." />
            ) : (
              <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart data={datosArea} margin={{ top: 8, right: 8, left: -18, bottom: 4 }}>
                    <CartesianGrid strokeDasharray="3 3" stroke="rgb(var(--line-strong) / 0.45)" vertical={false} />
                    <XAxis dataKey="name" tick={{ fontSize: 12, fill: 'currentColor' }} axisLine={false} tickLine={false} />
                    <YAxis allowDecimals={false} tick={{ fontSize: 12, fill: 'currentColor' }} axisLine={false} tickLine={false} />
                    <Tooltip
                      cursor={{ fill: 'rgb(var(--chart-2) / 0.08)' }}
                      formatter={(valor) => [`${valor} préstamos`, 'Total']}
                      contentStyle={ESTILO_TOOLTIP_GRAFICO}
                    />
                    <Bar dataKey="prestamos" fill={COLORES_GRAFICO[1]} radius={[8, 8, 0, 0]} maxBarSize={46} isAnimationActive={false} />
                  </BarChart>
                </ResponsiveContainer>
              </div>
            )}
          </TarjetaCuerpo>
        </Tarjeta>
      </div>

      <div className="mt-5 grid grid-cols-1 gap-5 lg:grid-cols-3">
        <Tarjeta>
          <TarjetaEncabezado titulo="Préstamos por mes" descripcion="Tendencia de los últimos 6 meses" />
          <TarjetaCuerpo>
            {datosMes.length === 0 ? (
              <EstadoVacio titulo="Sin historial suficiente" />
            ) : (
              <div className="h-56">
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart data={datosMes} margin={{ top: 8, right: 8, left: -22, bottom: 4 }}>
                    <CartesianGrid strokeDasharray="3 3" stroke="rgb(var(--line-strong) / 0.45)" vertical={false} />
                    <XAxis dataKey="name" tick={{ fontSize: 11, fill: 'currentColor' }} axisLine={false} tickLine={false} />
                    <YAxis allowDecimals={false} tick={{ fontSize: 11, fill: 'currentColor' }} axisLine={false} tickLine={false} />
                    <Tooltip
                      cursor={{ fill: 'rgb(var(--chart-2) / 0.08)' }}
                      formatter={(valor) => [`${valor} préstamos`, 'Total']}
                      contentStyle={ESTILO_TOOLTIP_GRAFICO}
                    />
                    <Bar dataKey="prestamos" fill={COLORES_GRAFICO[1]} radius={[8, 8, 0, 0]} maxBarSize={38} isAnimationActive={false} />
                  </BarChart>
                </ResponsiveContainer>
              </div>
            )}
          </TarjetaCuerpo>
        </Tarjeta>

        <Tarjeta>
          <TarjetaEncabezado titulo="Equipos más utilizados" descripcion="Top 5 por número de préstamos" />
          <TarjetaCuerpo className="p-0">
            {(datos?.equiposMasUtilizados ?? []).length === 0 ? (
              <EstadoVacio titulo="Sin equipos prestados aún" />
            ) : (
              <ul className="divide-y divide-line">
                {datos.equiposMasUtilizados.map((equipo, indice) => (
                  <li key={equipo.id} className="flex items-center gap-3 px-5 py-3">
                    <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-surface-3 text-xs font-bold text-muted">
                      {indice + 1}
                    </span>
                    <div className="min-w-0 flex-1">
                      <p className="truncate text-sm font-semibold text-ink">{equipo.nombre}</p>
                      <p className="truncate text-xs text-muted">
                        {equipo.codigoPatrimonial} · {equipo.categoria}
                      </p>
                    </div>
                    <div className="text-right">
                      <p className="text-sm font-bold text-ink">{equipo.totalPrestamos}</p>
                      <p className="text-xs text-muted">{equipo.diasPromedio} días prom.</p>
                    </div>
                  </li>
                ))}
              </ul>
            )}
          </TarjetaCuerpo>
        </Tarjeta>

        <Tarjeta>
          <TarjetaEncabezado titulo="Últimos movimientos" descripcion="Actividad registrada en el sistema" />
          <TarjetaCuerpo className="p-0">
            {(datos?.ultimosMovimientos ?? []).length === 0 ? (
              <EstadoVacio titulo="Sin movimientos registrados" />
            ) : (
              <ol className="relative space-y-0 px-5 py-4">
                {datos.ultimosMovimientos.map((movimiento, indice) => (
                  <li key={movimiento.id} className="relative flex gap-3 pb-4 last:pb-0">
                    {indice < datos.ultimosMovimientos.length - 1 && (
                      <span className="absolute left-[7px] top-4 h-full w-px bg-line" aria-hidden="true" />
                    )}
                    <span className="relative mt-1 h-3.5 w-3.5 shrink-0 rounded-full border-2 border-brand-500 bg-surface" aria-hidden="true" />
                    <div className="min-w-0 flex-1">
                      <p className="text-sm font-medium leading-snug text-ink">{movimiento.descripcion}</p>
                      <p className="mt-0.5 text-xs text-muted">
                        {movimiento.tipoNombre} · {movimiento.usuarioNombre ?? 'Sistema'} · {tiempoRelativo(movimiento.fecha)}
                      </p>
                    </div>
                  </li>
                ))}
              </ol>
            )}
          </TarjetaCuerpo>
        </Tarjeta>
      </div>

      {puedeGestionar && (
        <div className="mt-5 flex flex-wrap items-center gap-3 rounded-2xl border border-dashed border-line bg-surface px-5 py-4 text-sm text-muted">
          <Icone nombre="informacion" className="h-5 w-5 shrink-0 text-brand-500" />
          <span>
            Como <strong className="text-ink">{usuario?.rol === 1 ? 'Administrador' : 'Responsable de Tecnología'}</strong> puede registrar equipos,
            aprobar préstamos y registrar devoluciones desde los módulos de inventario y préstamos.
          </span>
          <Insignia tono="marca" className="ml-auto">
            {formatearNumero(datos?.prestamosActivos)} préstamos activos
          </Insignia>
        </div>
      )}
    </>
  );
}
