import { useEffect, useMemo, useState } from 'react';
import { servicios } from '../api/servicios';
import { usePeticion, useDebounce } from '../ganchos/usePeticion';
import { usePaginacion } from '../ganchos/usePaginacion';
import { useFormulario } from '../ganchos/useFormulario';
import { useToast } from '../contexto/ToastContexto';
import { useAuth } from '../contexto/AuthContexto';
import { EncabezadoPagina } from '../componentes/layout/EncabezadoPagina';
import { Boton } from '../componentes/ui/Boton';
import { Campo, Entrada, AreaTexto, Seleccion } from '../componentes/ui/Campo';
import { Modal } from '../componentes/ui/Modal';
import { Tabla } from '../componentes/ui/Tabla';
import { Paginacion } from '../componentes/ui/Paginacion';
import { Tarjeta } from '../componentes/ui/Tarjeta';
import { Buscador, EstadoVacio } from '../componentes/ui/Estados';
import { Insignia } from '../componentes/ui/Insignia';
import { Icone } from '../componentes/ui/Icone';
import { formatearFecha, hoyISO } from '../utilidades/formateadores';
import { reglasDevolucion, reglasPrestamo } from '../utilidades/validaciones';
import { ESTILO_ESTADO_PRESTAMO, NOMBRES_ESTADO_PRESTAMO } from '../utilidades/constantes';

/**
 * Préstamos y devoluciones (RF-09 … RF-12): registro de entrega, devolución
 * con observaciones obligatorias (RN-10), consulta de activos e historial.
 */
export function Prestamos() {
  const toast = useToast();
  const { puedeGestionar, usuario } = useAuth();

  const [pestana, setPestana] = useState('todos');
  const [busqueda, setBusqueda] = useState('');
  const busquedaDiferida = useDebounce(busqueda, 350);
  const { pagina, setPagina, tamanoPagina, setTamanoPagina, filtros, setFiltros } = usePaginacion();

  const parametros = useMemo(
    () => ({
      pagina,
      tamanoPagina,
      busqueda: busquedaDiferida || undefined,
      estado: pestana === 'todos' ? undefined : pestana === 'activos' ? 'Activo' : 'Devuelto',
      area: filtros.area || undefined,
      desde: filtros.desde || undefined,
      hasta: filtros.hasta || undefined
    }),
    [pagina, tamanoPagina, busquedaDiferida, pestana, filtros]
  );

  const { datos, cargando, error, recargar } = usePeticion(() => servicios.prestamos.listar(parametros), [parametros]);
  const { datos: usuarios, recargar: recargarUsuarios } = usePeticion(
    () => (puedeGestionar ? servicios.usuarios.listar({ pagina: 1, tamanoPagina: 100, soloActivos: true }) : Promise.resolve(null)),
    [puedeGestionar]
  );
  const { datos: equipos } = usePeticion(
    () => (puedeGestionar ? servicios.equipos.listar({ pagina: 1, tamanoPagina: 100, estado: 'Disponible' }) : Promise.resolve(null)),
    [puedeGestionar]
  );

  const [modalPrestamo, setModalPrestamo] = useState(false);
  const [modalDevolucion, setModalDevolucion] = useState(false);
  const [prestamoSeleccionado, setPrestamoSeleccionado] = useState(null);
  const [procesando, setProcesando] = useState(false);

  const formularioPrestamo = useFormulario(
    { usuarioId: '', equipoId: '', fechaPrestamo: hoyISO(), observacion: '' },
    reglasPrestamo
  );
  const formularioDevolucion = useFormulario({ fechaDevolucion: hoyISO(), observacion: '' }, reglasDevolucion);

  useEffect(() => {
    setPagina(1);
  }, [busquedaDiferida, pestana, filtros, setPagina]);

  const registrarPrestamo = async (evento) => {
    evento.preventDefault();
    try {
      const { exito } = await formularioPrestamo.enviar(async (valores) =>
        servicios.prestamos.registrar({
          usuarioId: valores.usuarioId,
          equipoId: valores.equipoId,
          fechaPrestamo: valores.fechaPrestamo ? new Date(valores.fechaPrestamo).toISOString() : null,
          observacion: valores.observacion || null
        })
      );

      if (!exito) {
        toast.error('Revise los campos marcados: hay datos por corregir.');
        return;
      }

      setModalPrestamo(false);
      toast.exito('Préstamo registrado. El equipo quedó asignado y en estado «Prestado».');
      recargar();
      recargarUsuarios();
    } catch (fallo) {
      toast.error(fallo.message);
    }
  };

  const registrarDevolucion = async (evento) => {
    evento.preventDefault();
    try {
      const { exito } = await formularioDevolucion.enviar(async (valores) =>
        servicios.prestamos.registrarDevolucion(prestamoSeleccionado.id, {
          fechaDevolucion: valores.fechaDevolucion ? new Date(valores.fechaDevolucion).toISOString() : null,
          observacion: valores.observacion
        })
      );

      if (!exito) {
        toast.error('La devolución requiere observaciones válidas (RN-10).');
        return;
      }

      setModalDevolucion(false);
      toast.exito('Devolución registrada. El equipo volvió a estar disponible.');
      recargar();
    } catch (fallo) {
      toast.error(fallo.message);
    }
  };

  const columnas = [
    {
      clave: 'equipo',
      titulo: 'Equipo',
      render: (prestamo) => (
        <div className="min-w-0">
          <p className="truncate font-semibold text-ink">{prestamo.equipoNombre}</p>
          <p className="truncate font-mono text-xs text-muted">{prestamo.codigoPatrimonial}</p>
        </div>
      )
    },
    {
      clave: 'usuario',
      titulo: 'Colaborador',
      render: (prestamo) => (
        <div className="min-w-0">
          <p className="truncate font-medium text-ink">{prestamo.usuarioNombre}</p>
          <p className="truncate text-xs text-muted">{prestamo.usuarioArea}</p>
        </div>
      )
    },
    { clave: 'fechaPrestamo', titulo: 'Entrega', render: (prestamo) => <span className="text-muted">{formatearFecha(prestamo.fechaPrestamo)}</span> },
    {
      clave: 'fechaDevolucion',
      titulo: 'Devolución',
      render: (prestamo) => <span className="text-muted">{prestamo.fechaDevolucion ? formatearFecha(prestamo.fechaDevolucion) : '—'}</span>
    },
    {
      clave: 'duracion',
      titulo: 'Días',
      render: (prestamo) => (
        <span className="inline-flex items-center gap-1 text-muted">
          <Icone nombre="reloj" className="h-4 w-4 opacity-60" />
          {prestamo.diasDePrestamo}
        </span>
      )
    },
    {
      clave: 'responsable',
      titulo: 'Responsable',
      render: (prestamo) => <span className="text-xs text-muted">{prestamo.responsableNombre}</span>
    },
    {
      clave: 'estado',
      titulo: 'Estado',
      render: (prestamo) => (
        <Insignia estilo={ESTILO_ESTADO_PRESTAMO[prestamo.estado]}>
          {prestamo.estadoNombre}
        </Insignia>
      )
    },
    {
      clave: 'acciones',
      titulo: 'Acciones',
      claseCelda: 'text-right',
      render: (prestamo) =>
        prestamo.estado === 1 && puedeGestionar ? (
          <Boton
            variante="secundario"
            tamano="pequeno"
            className="whitespace-nowrap"
            onClick={() => {
              setPrestamoSeleccionado(prestamo);
              formularioDevolucion.establecerValores({ fechaDevolucion: hoyISO(), observacion: '' });
              setModalDevolucion(true);
            }}
          >
            <Icone nombre="devolucion" className="h-4 w-4" />
            Registrar devolución
          </Boton>
        ) : (
          <span className="text-xs text-muted">—</span>
        )
    }
  ];

  const pestanas = [
    { clave: 'todos', titulo: 'Todos' },
    { clave: 'activos', titulo: 'Activos' },
    { clave: 'devueltos', titulo: 'Devueltos' }
  ];

  return (
    <>
      <EncabezadoPagina
        titulo="Préstamos y devoluciones"
        descripcion="Controle las entregas de equipos, registre devoluciones y consulte el historial completo."
        acciones={
          puedeGestionar && (
            <Boton
              onClick={() => {
                formularioPrestamo.establecerValores({ usuarioId: '', equipoId: '', fechaPrestamo: hoyISO(), observacion: '' });
                setModalPrestamo(true);
              }}
            >
              <Icone nombre="mas" className="h-4 w-4" />
              Registrar préstamo
            </Boton>
          )
        }
      />

      <Tarjeta>
        <div className="flex flex-col gap-3 border-b border-line p-4">
          <div className="flex flex-col gap-3 lg:flex-row lg:items-center">
            <div className="flex rounded-xl bg-surface-3 p-1" role="tablist" aria-label="Filtrar préstamos por estado">
              {pestanas.map((opcion) => (
                <button
                  key={opcion.clave}
                  type="button"
                  role="tab"
                  aria-selected={pestana === opcion.clave}
                  onClick={() => setPestana(opcion.clave)}
                  className={`rounded-lg px-3.5 py-1.5 text-sm font-semibold transition ${
                    pestana === opcion.clave ? 'bg-surface text-ink shadow-sm' : 'text-muted hover:text-ink'
                  }`}
                >
                  {opcion.titulo}
                </button>
              ))}
            </div>

            <Buscador valor={busqueda} onCambio={setBusqueda} placeholder="Buscar por equipo o colaborador…" className="w-full lg:max-w-xs" />

          </div>

          <div className="grid flex-1 grid-cols-1 gap-3 sm:grid-cols-3">
              <Seleccion
                value={filtros.area ?? ''}
                onChange={(evento) => setFiltros((actuales) => ({ ...actuales, area: evento.target.value }))}
                placeholder="Todas las áreas"
                aria-label="Filtrar por área"
              >
                {[...new Set((usuarios?.items ?? []).map((item) => item.area))].sort().map((area) => (
                  <option key={area} value={area}>
                    {area}
                  </option>
                ))}
              </Seleccion>

              <Campo etiqueta="Desde">
                {(atributos) => (
                  <Entrada
                    {...atributos}
                    type="date"
                    value={filtros.desde ?? ''}
                    onChange={(evento) => setFiltros((actuales) => ({ ...actuales, desde: evento.target.value }))}
                  />
                )}
              </Campo>

              <Campo etiqueta="Hasta">
                {(atributos) => (
                  <Entrada
                    {...atributos}
                    type="date"
                    value={filtros.hasta ?? ''}
                    onChange={(evento) => setFiltros((actuales) => ({ ...actuales, hasta: evento.target.value }))}
                  />
                )}
              </Campo>
          </div>
        </div>

        <Tabla
          columnas={columnas}
          datos={datos?.items ?? []}
          cargando={cargando}
          error={error?.message}
          vacio={
            <EstadoVacio
              titulo="No hay préstamos para mostrar"
              descripcion={
                pestana === 'activos'
                  ? 'Actualmente no existen préstamos activos en el sistema.'
                  : 'Ajuste los filtros o registre un nuevo préstamo.'
              }
              accion={
                puedeGestionar && (
                  <Boton
                    onClick={() => {
                      formularioPrestamo.establecerValores({ usuarioId: '', equipoId: '', fechaPrestamo: hoyISO(), observacion: '' });
                      setModalPrestamo(true);
                    }}
                  >
                    <Icone nombre="mas" className="h-4 w-4" />
                    Registrar préstamo
                  </Boton>
                )
              }
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
            etiqueta="préstamos"
          />
        )}
      </Tarjeta>

      {/* ── Registro de préstamo ──────────────────────────────────────────── */}
      <Modal
        abierto={modalPrestamo}
        cerrar={() => setModalPrestamo(false)}
        titulo="Registrar préstamo de equipo"
        descripcion="La entrega se registra a nombre de un colaborador y queda asociada al responsable de la operación (RN-05)."
        ancho="mediano"
        pie={
          <>
            <Boton variante="secundario" onClick={() => setModalPrestamo(false)}>
              Cancelar
            </Boton>
            <Boton onClick={registrarPrestamo} cargando={formularioPrestamo.enviando}>
              Registrar préstamo
            </Boton>
          </>
        }
      >
        <form onSubmit={registrarPrestamo} className="grid grid-cols-1 gap-4 sm:grid-cols-2" noValidate>
          <Campo etiqueta="Colaborador que recibe el equipo" requerido error={formularioPrestamo.errores.usuarioId}>
            {(atributos) => (
              <Seleccion
                {...atributos}
                value={formularioPrestamo.valores.usuarioId}
                onChange={(evento) => formularioPrestamo.cambiar('usuarioId', evento.target.value)}
              >
                <option value="">Seleccione un colaborador…</option>
                {(usuarios?.items ?? []).map((item) => (
                  <option key={item.id} value={item.id}>
                    {item.nombreCompleto} — {item.area}
                  </option>
                ))}
              </Seleccion>
            )}
          </Campo>

          <Campo
            etiqueta="Equipo a prestar"
            requerido
            error={formularioPrestamo.errores.equipoId}
            ayuda="Solo se listan los equipos disponibles (RN-01, RN-02 y RN-03)."
          >
            {(atributos) => (
              <Seleccion
                {...atributos}
                value={formularioPrestamo.valores.equipoId}
                onChange={(evento) => formularioPrestamo.cambiar('equipoId', evento.target.value)}
              >
                <option value="">Seleccione un equipo…</option>
                {(equipos?.items ?? []).map((equipo) => (
                  <option key={equipo.id} value={equipo.id}>
                    {equipo.codigoPatrimonial} · {equipo.nombre} ({equipo.marca})
                  </option>
                ))}
              </Seleccion>
            )}
          </Campo>

          <Campo etiqueta="Fecha de préstamo" requerido error={formularioPrestamo.errores.fechaPrestamo}>
            {(atributos) => (
              <Entrada
                {...atributos}
                type="date"
                max={hoyISO()}
                value={formularioPrestamo.valores.fechaPrestamo}
                onChange={(evento) => formularioPrestamo.cambiar('fechaPrestamo', evento.target.value)}
              />
            )}
          </Campo>

          <div className="sm:col-span-2">
            <Campo etiqueta="Observaciones de la entrega" error={formularioPrestamo.errores.observacion}>
              {(atributos) => (
                <AreaTexto
                  {...atributos}
                  value={formularioPrestamo.valores.observacion}
                  onChange={(evento) => formularioPrestamo.cambiar('observacion', evento.target.value)}
                  placeholder="Accesorios entregados, purpose del equipo, agreements…"
                />
              )}
            </Campo>
          </div>
        </form>
      </Modal>

      {/* ── Registro de devolución ────────────────────────────────────────── */}
      <Modal
        abierto={modalDevolucion}
        cerrar={() => setModalDevolucion(false)}
        titulo="Registrar devolución"
        descripcion={
          prestamoSeleccionado
            ? `${prestamoSeleccionado.codigoPatrimonial} · ${prestamoSeleccionado.equipoNombre} — ${prestamoSeleccionado.usuarioNombre}`
            : ''
        }
        ancho="mediano"
        pie={
          <>
            <Boton variante="secundario" onClick={() => setModalDevolucion(false)}>
              Cancelar
            </Boton>
            <Boton onClick={registrarDevolucion} cargando={formularioDevolucion.enviando}>
              Registrar devolución
            </Boton>
          </>
        }
      >
        <form onSubmit={registrarDevolucion} className="space-y-4" noValidate>
          <div className="rounded-xl border border-line bg-surface-2 px-4 py-3 text-sm text-muted">
            <p>
              <span className="font-medium text-ink">Regla RN-10:</span> toda devolución debe registrar observaciones (estado del
              equipo, incidencias y accesorios devueltos).
            </p>
          </div>

          <Campo etiqueta="Fecha de devolución" requerido error={formularioDevolucion.errores.fechaDevolucion}>
            {(atributos) => (
              <Entrada
                {...atributos}
                type="date"
                max={hoyISO()}
                value={formularioDevolucion.valores.fechaDevolucion}
                onChange={(evento) => formularioDevolucion.cambiar('fechaDevolucion', evento.target.value)}
              />
            )}
          </Campo>

          <Campo etiqueta="Observaciones de la devolución" requerido error={formularioDevolucion.errores.observacion}>
            {(atributos) => (
              <AreaTexto
                {...atributos}
                rows={4}
                value={formularioDevolucion.valores.observacion}
                onChange={(evento) => formularioDevolucion.cambiar('observacion', evento.target.value)}
                onBlur={() => formularioDevolucion.marcarTocado('observacion')}
                placeholder="Ej.: equipo devuelto en perfecto estado, con cargador y mouse."
              />
            )}
          </Campo>
        </form>
      </Modal>
    </>
  );
}
