import { useEffect, useMemo, useState } from 'react';
import { servicios } from '../api/servicios';
import { usePeticion, useDebounce } from '../ganchos/usePeticion';
import { usePaginacion } from '../ganchos/usePaginacion';
import { EncabezadoPagina } from '../componentes/layout/EncabezadoPagina';
import { Tabla } from '../componentes/ui/Tabla';
import { Paginacion } from '../componentes/ui/Paginacion';
import { Tarjeta } from '../componentes/ui/Tarjeta';
import { Buscador, EstadoVacio } from '../componentes/ui/Estados';
import { Seleccion } from '../componentes/ui/Campo';
import { Icone } from '../componentes/ui/Icone';
import { Insignia } from '../componentes/ui/Insignia';
import { formatearFechaHora, tiempoRelativo } from '../utilidades/formateadores';
import { TIPOS_MOVIMIENTO } from '../utilidades/constantes';

const COLOR_POR_TIPO = {
  AltaUsuario: 'exito',
  ActualizacionUsuario: 'info',
  CambioEstadoUsuario: 'aviso',
  AltaEquipo: 'exito',
  ActualizacionEquipo: 'info',
  CambioEstadoEquipo: 'aviso',
  Prestamo: 'marca',
  Devolucion: 'neutro'
};

/**
 * Bitácora de movimientos (RF-17): trazabilidad de altas, actualizaciones,
 * cambios de estado, préstamos y devoluciones.
 */
export function Auditoria() {
  const [busqueda, setBusqueda] = useState('');
  const busquedaDiferida = useDebounce(busqueda, 350);
  const { pagina, setPagina, tamanoPagina, setTamanoPagina, filtros, setFiltros } = usePaginacion();

  const parametros = useMemo(
    () => ({
      pagina,
      tamanoPagina,
      busqueda: busquedaDiferida || undefined,
      tipo: filtros.tipo || undefined,
      desde: filtros.desde ? new Date(filtros.desde).toISOString() : undefined,
      hasta: filtros.hasta ? new Date(`${filtros.hasta}T23:59:59`).toISOString() : undefined
    }),
    [pagina, tamanoPagina, busquedaDiferida, filtros]
  );

  const { datos, cargando, error } = usePeticion(() => servicios.reportes.movimientos(parametros), [parametros]);

  useEffect(() => {
    setPagina(1);
  }, [busquedaDiferida, filtros, setPagina]);

  const columnas = [
    {
      clave: 'fecha',
      titulo: 'Fecha',
      render: (movimiento) => (
        <div className="min-w-0">
          <p className="text-sm font-medium text-ink">{formatearFechaHora(movimiento.fecha)}</p>
          <p className="text-xs text-muted">{tiempoRelativo(movimiento.fecha)}</p>
        </div>
      )
    },
    {
      clave: 'tipo',
      titulo: 'Acción',
      render: (movimiento) => <Insignia tono={COLOR_POR_TIPO[movimiento.tipo] ?? 'neutro'}>{movimiento.tipoNombre}</Insignia>
    },
    { clave: 'descripcion', titulo: 'Detalle', render: (movimiento) => <span className="text-ink">{movimiento.descripcion}</span> },
    { clave: 'usuarioNombre', titulo: 'Registrado por', render: (movimiento) => <span className="text-muted">{movimiento.usuarioNombre ?? 'Sistema'}</span> }
  ];

  return (
    <>
      <EncabezadoPagina
        titulo="Bitácora de movimientos"
        descripcion="Historial auditable de las acciones realizadas en el sistema: altas, actualizaciones, préstamos y devoluciones."
      />

      <Tarjeta>
        <div className="flex flex-col gap-3 border-b border-line p-4 lg:flex-row lg:items-center">
          <Buscador valor={busqueda} onCambio={setBusqueda} placeholder="Buscar en el detalle de los movimientos…" className="w-full lg:max-w-sm" />

          <div className="grid flex-1 grid-cols-1 gap-3 sm:grid-cols-3">
            <Seleccion
              value={filtros.tipo ?? ''}
              onChange={(evento) => setFiltros((actuales) => ({ ...actuales, tipo: evento.target.value }))}
              placeholder="Todos los tipos"
              aria-label="Filtrar por tipo de movimiento"
            >
              {TIPOS_MOVIMIENTO.map((tipo) => (
                <option key={tipo.valor} value={tipo.valor}>
                  {tipo.etiqueta}
                </option>
              ))}
            </Seleccion>

            <input
              type="date"
              value={filtros.desde ?? ''}
              onChange={(evento) => setFiltros((actuales) => ({ ...actuales, desde: evento.target.value }))}
              className="campo-base"
              aria-label="Movimientos desde"
            />
            <input
              type="date"
              value={filtros.hasta ?? ''}
              onChange={(evento) => setFiltros((actuales) => ({ ...actuales, hasta: evento.target.value }))}
              className="campo-base"
              aria-label="Movimientos hasta"
            />
          </div>
        </div>

        <Tabla
          columnas={columnas}
          datos={datos?.items ?? []}
          cargando={cargando}
          error={error?.message}
          vacio={
            <EstadoVacio
              titulo="Sin movimientos registrados"
              descripcion="No hay actividades que coincidan con los filtros aplicados."
              icono={<Icone nombre="reloj" className="h-7 w-7" />}
            />
          }
        />

        {datos && (
          <Paginacion
            pagina={datos.pagina}
            totalPaginas={datos.totalPaginas}
            totalRegistros={datos.totalRegistros}
            tamanoPagina={datos.tamanoPagina}
            onCambioPagina={setPagina}
            onCambioTamano={(valor) => {
              setTamanoPagina(valor);
              setPagina(1);
            }}
            etiqueta="movimientos"
          />
        )}
      </Tarjeta>
    </>
  );
}
