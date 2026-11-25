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
import { Modal, ConfirmarDialogo } from '../componentes/ui/Modal';
import { Tabla } from '../componentes/ui/Tabla';
import { Paginacion } from '../componentes/ui/Paginacion';
import { Tarjeta } from '../componentes/ui/Tarjeta';
import { Buscador, EstadoVacio } from '../componentes/ui/Estados';
import { Insignia } from '../componentes/ui/Insignia';
import { Icone } from '../componentes/ui/Icone';
import { formatearFecha, hoyISO } from '../utilidades/formateadores';
import { reglasEquipo } from '../utilidades/validaciones';
import { CATEGORIAS_EQUIPO, ESTADOS_EQUIPO, ESTILO_ESTADO_EQUIPO, NOMBRES_ESTADO_EQUIPO } from '../utilidades/constantes';

const VALORES_INICIALES = {
  codigoPatrimonial: '',
  nombre: '',
  categoria: 'Laptop',
  marca: '',
  numeroSerie: '',
  ubicacion: '',
  fechaAdquisicion: '',
  especificaciones: ''
};

/**
 * Inventario de equipos (RF-05 … RF-08): alta, edición, cambio de estado,
 * consulta con filtros y eliminación controlada (RN-06, RN-07).
 */
export function Inventario() {
  const toast = useToast();
  const { puedeGestionar, esAdministrador } = useAuth();

  const [busqueda, setBusqueda] = useState('');
  const busquedaDiferida = useDebounce(busqueda, 350);
  const { pagina, setPagina, tamanoPagina, setTamanoPagina, filtros, setFiltros } = usePaginacion();

  const parametros = useMemo(
    () => ({
      pagina,
      tamanoPagina,
      busqueda: busquedaDiferida || undefined,
      estado: filtros.estado || undefined,
      categoria: filtros.categoria || undefined,
      marca: filtros.marca || undefined
    }),
    [pagina, tamanoPagina, busquedaDiferida, filtros]
  );

  const { datos, cargando, error, recargar } = usePeticion(() => servicios.equipos.listar(parametros), [parametros]);
  const { datos: categorias } = usePeticion(() => servicios.equipos.categorias(), []);
  const { datos: marcas } = usePeticion(() => servicios.equipos.marcas(), []);

  const [modalAbierto, setModalAbierto] = useState(false);
  const [equipoEditando, setEquipoEditando] = useState(null);
  const [equipoEstado, setEquipoEstado] = useState(null);
  const [equipoEliminar, setEquipoEliminar] = useState(null);
  const [procesando, setProcesando] = useState(false);

  const formulario = useFormulario(VALORES_INICIALES, reglasEquipo);

  useEffect(() => {
    setPagina(1);
  }, [busquedaDiferida, filtros, setPagina]);

  const abrirNuevo = () => {
    setEquipoEditando(null);
    formulario.establecerValores(VALORES_INICIALES);
    setModalAbierto(true);
  };

  const abrirEdicion = (equipo) => {
    setEquipoEditando(equipo);
    formulario.establecerValores({
      codigoPatrimonial: equipo.codigoPatrimonial,
      nombre: equipo.nombre,
      categoria: equipo.categoria,
      marca: equipo.marca,
      numeroSerie: equipo.numeroSerie ?? '',
      ubicacion: equipo.ubicacion ?? '',
      fechaAdquisicion: equipo.fechaAdquisicion ? equipo.fechaAdquisicion.slice(0, 10) : '',
      especificaciones: equipo.especificaciones ?? ''
    });
    setModalAbierto(true);
  };

  const guardar = async (evento) => {
    evento.preventDefault();

    try {
      const { exito } = await formulario.enviar(async (valores) => {
        const datos = {
          ...valores,
          fechaAdquisicion: valores.fechaAdquisicion ? new Date(valores.fechaAdquisicion).toISOString() : null,
          numeroSerie: valores.numeroSerie || null,
          ubicacion: valores.ubicacion || null,
          especificaciones: valores.especificaciones || null
        };

        if (equipoEditando) {
          return servicios.equipos.actualizar(equipoEditando.id, datos);
        }
        return servicios.equipos.crear(datos);
      });

      if (!exito) {
        toast.error('Revise los campos marcados: hay datos por corregir.');
        return;
      }

      setModalAbierto(false);
      toast.exito(equipoEditando ? 'Equipo actualizado correctamente.' : 'Equipo registrado correctamente.');
      recargar();
    } catch (fallo) {
      toast.error(fallo.message);
    }
  };

  const guardarEstado = async () => {
    setProcesando(true);
    try {
      await servicios.equipos.cambiarEstado(equipoEstado.equipo.id, {
        estado: equipoEstado.estado,
        motivo: equipoEstado.motivo || null
      });
      toast.exito(`Estado del equipo ${equipoEstado.equipo.codigoPatrimonial} actualizado.`);
      setEquipoEstado(null);
      recargar();
    } catch (fallo) {
      toast.error(fallo.message);
    } finally {
      setProcesando(false);
    }
  };

  const eliminar = async () => {
    setProcesando(true);
    try {
      await servicios.equipos.eliminar(equipoEliminar.id);
      toast.exito(`Equipo ${equipoEliminar.codigoPatrimonial} eliminado.`);
      setEquipoEliminar(null);
      recargar();
    } catch (fallo) {
      toast.error(fallo.message);
    } finally {
      setProcesando(false);
    }
  };

  const columnas = [
    {
      clave: 'codigoPatrimonial',
      titulo: 'Código',
      ordenable: true,
      render: (equipo) => <span className="font-mono text-xs font-semibold text-ink">{equipo.codigoPatrimonial}</span>
    },
    {
      clave: 'nombre',
      titulo: 'Equipo',
      ordenable: true,
      render: (equipo) => (
        <div className="min-w-0">
          <p className="truncate font-semibold text-ink">{equipo.nombre}</p>
          <p className="truncate text-xs text-muted">
            {equipo.marca}
            {equipo.numeroSerie ? ` · ${equipo.numeroSerie}` : ''}
          </p>
        </div>
      )
    },
    { clave: 'categoria', titulo: 'Categoría', ordenable: true, render: (equipo) => <span className="text-muted">{equipo.categoria}</span> },
    {
      clave: 'ubicacion',
      titulo: 'Ubicación',
      render: (equipo) => (
        <span className="inline-flex items-center gap-1.5 text-muted">
          <Icone nombre="ubicacion" className="h-4 w-4 opacity-60" />
          {equipo.ubicacion ?? '—'}
        </span>
      )
    },
    {
      clave: 'estado',
      titulo: 'Estado',
      ordenable: true,
      render: (equipo) => (
        <Insignia estilo={ESTILO_ESTADO_EQUIPO[equipo.estado]}>
          {equipo.estadoNombre}
        </Insignia>
      )
    },
    {
      clave: 'prestamo',
      titulo: 'Asignación actual',
      render: (equipo) =>
        equipo.tienePrestamoActivo ? (
          <div className="min-w-0">
            <p className="truncate text-sm font-medium text-ink">{equipo.usuarioActual}</p>
            <p className="text-xs text-muted">Desde {formatearFecha(equipo.fechaPrestamoActual)}</p>
          </div>
        ) : (
          <span className="text-xs text-muted">Sin asignación</span>
        )
    },
    {
      clave: 'acciones',
      titulo: 'Acciones',
      claseCelda: 'text-right',
      render: (equipo) => (
        <div className="flex items-center justify-end gap-1">
          {puedeGestionar && (
            <button
              type="button"
              onClick={() => setEquipoEstado({ equipo, estado: ESTADOS_EQUIPO.MANTENIMIENTO, motivo: '' })}
              className="rounded-lg p-1.5 text-muted transition hover:bg-surface-3 hover:text-ink"
              title="Cambiar estado"
              aria-label={`Cambiar estado de ${equipo.codigoPatrimonial}`}
            >
              <Icone nombre="ajustes" className="h-4 w-4" />
            </button>
          )}
          {puedeGestionar && (
            <button
              type="button"
              onClick={() => abrirEdicion(equipo)}
              className="rounded-lg p-1.5 text-muted transition hover:bg-surface-3 hover:text-ink"
              title="Editar equipo"
              aria-label={`Editar ${equipo.codigoPatrimonial}`}
            >
              <Icone nombre="editar" className="h-4 w-4" />
            </button>
          )}
          {esAdministrador && (
            <button
              type="button"
              onClick={() => setEquipoEliminar(equipo)}
              disabled={!equipo.puedeEliminar}
              className="rounded-lg p-1.5 text-muted transition hover:bg-rose-50 hover:text-rose-600 disabled:cursor-not-allowed disabled:opacity-30 disabled:hover:bg-transparent disabled:hover:text-muted dark:hover:bg-rose-500/10"
              title={equipo.puedeEliminar ? 'Eliminar equipo' : 'El equipo tiene historial de préstamos (RN-06)'}
              aria-label={`Eliminar ${equipo.codigoPatrimonial}`}
            >
              <Icone nombre="basura" className="h-4 w-4" />
            </button>
          )}
        </div>
      )
    }
  ];

  return (
    <>
      <EncabezadoPagina
        titulo="Inventario de equipos"
        descripcion="Consulte, registre y actualice los equipos tecnológicos del inventario institucional."
        acciones={
          puedeGestionar && (
            <Boton onClick={abrirNuevo}>
              <Icone nombre="mas" className="h-4 w-4" />
              Registrar equipo
            </Boton>
          )
        }
      />

      <Tarjeta>
        <div className="flex flex-col gap-3 border-b border-line p-4 lg:flex-row lg:items-center">
          <Buscador valor={busqueda} onCambio={setBusqueda} placeholder="Buscar por código, nombre, marca o ubicación…" className="w-full lg:max-w-sm" />

          <div className="grid flex-1 grid-cols-1 gap-3 sm:grid-cols-3">
            <Seleccion
              value={filtros.estado ?? ''}
              onChange={(evento) => setFiltros((actuales) => ({ ...actuales, estado: evento.target.value }))}
              placeholder="Todos los estados"
              aria-label="Filtrar por estado"
            >
              {Object.entries(NOMBRES_ESTADO_EQUIPO).map(([valor, etiqueta]) => (
                <option key={valor} value={valor}>
                  {etiqueta}
                </option>
              ))}
            </Seleccion>

            <Seleccion
              value={filtros.categoria ?? ''}
              onChange={(evento) => setFiltros((actuales) => ({ ...actuales, categoria: evento.target.value }))}
              placeholder="Todas las categorías"
              aria-label="Filtrar por categoría"
            >
              {(categorias ?? CATEGORIAS_EQUIPO).map((categoria) => (
                <option key={categoria} value={categoria}>
                  {categoria}
                </option>
              ))}
            </Seleccion>

            <Seleccion
              value={filtros.marca ?? ''}
              onChange={(evento) => setFiltros((actuales) => ({ ...actuales, marca: evento.target.value }))}
              placeholder="Todas las marcas"
              aria-label="Filtrar por marca"
            >
              {(marcas ?? []).map((marca) => (
                <option key={marca} value={marca}>
                  {marca}
                </option>
              ))}
            </Seleccion>
          </div>

          {(busqueda || filtros.estado || filtros.categoria || filtros.marca) && (
            <Boton
              variante="fantasma"
              onClick={() => {
                setBusqueda('');
                setFiltros({});
              }}
            >
              <Icone nombre="cerrar" className="h-4 w-4" />
              Limpiar
            </Boton>
          )}
        </div>

        <Tabla
          columnas={columnas}
          datos={datos?.items ?? []}
          cargando={cargando}
          error={error?.message}
          vacio={
            <EstadoVacio
              titulo="No se encontraron equipos"
              descripcion="Ajuste los filtros o registre un nuevo equipo en el inventario."
              accion={
                puedeGestionar && (
                  <Boton onClick={abrirNuevo}>
                    <Icone nombre="mas" className="h-4 w-4" />
                    Registrar equipo
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
            etiqueta="equipos"
          />
        )}
      </Tarjeta>

      {/* ── Formulario de alta / edición ───────────────────────────────────── */}
      <Modal
        abierto={modalAbierto}
        cerrar={() => setModalAbierto(false)}
        titulo={equipoEditando ? `Editar equipo ${equipoEditando.codigoPatrimonial}` : 'Registrar equipo'}
        descripcion={
          equipoEditando
            ? 'Actualice los datos descriptivos del equipo (RN-07: el código patrimonial no se modifica).'
            : 'Complete los datos del nuevo equipo tecnológico. Los campos marcados son obligatorios.'
        }
        ancho="mediano"
        pie={
          <>
            <Boton variante="secundario" onClick={() => setModalAbierto(false)}>
              Cancelar
            </Boton>
            <Boton onClick={guardar} cargando={formulario.enviando}>
              {equipoEditando ? 'Guardar cambios' : 'Registrar equipo'}
            </Boton>
          </>
        }
      >
        <form onSubmit={guardar} className="grid grid-cols-1 gap-4 sm:grid-cols-2" noValidate>
          <Campo etiqueta="Código patrimonial" requerido error={formulario.errores.codigoPatrimonial} ayuda="Identificador único del equipo (RN-07).">
            {(atributos) => (
              <Entrada
                {...atributos}
                value={formulario.valores.codigoPatrimonial}
                onChange={(evento) => formulario.cambiar('codigoPatrimonial', evento.target.value)}
                onBlur={() => formulario.marcarTocado('codigoPatrimonial')}
                placeholder="TECH-LAP-001"
                disabled={Boolean(equipoEditando)}
                className="font-mono"
              />
            )}
          </Campo>

          <Campo etiqueta="Nombre del equipo" requerido error={formulario.errores.nombre}>
            {(atributos) => (
              <Entrada
                {...atributos}
                value={formulario.valores.nombre}
                onChange={(evento) => formulario.cambiar('nombre', evento.target.value)}
                onBlur={() => formulario.marcarTocado('nombre')}
                placeholder="ThinkPad T14 Gen 3"
              />
            )}
          </Campo>

          <Campo etiqueta="Categoría" requerido error={formulario.errores.categoria}>
            {(atributos) => (
              <Seleccion
                {...atributos}
                value={formulario.valores.categoria}
                onChange={(evento) => formulario.cambiar('categoria', evento.target.value)}
              >
                {CATEGORIAS_EQUIPO.map((categoria) => (
                  <option key={categoria} value={categoria}>
                    {categoria}
                  </option>
                ))}
              </Seleccion>
            )}
          </Campo>

          <Campo etiqueta="Marca" requerido error={formulario.errores.marca}>
            {(atributos) => (
              <Entrada
                {...atributos}
                value={formulario.valores.marca}
                onChange={(evento) => formulario.cambiar('marca', evento.target.value)}
                onBlur={() => formulario.marcarTocado('marca')}
                placeholder="Lenovo"
              />
            )}
          </Campo>

          <Campo etiqueta="Número de serie" error={formulario.errores.numeroSerie}>
            {(atributos) => (
              <Entrada
                {...atributos}
                value={formulario.valores.numeroSerie}
                onChange={(evento) => formulario.cambiar('numeroSerie', evento.target.value)}
                placeholder="SN123456"
              />
            )}
          </Campo>

          <Campo etiqueta="Ubicación" error={formulario.errores.ubicacion} ayuda="Almacén, sala o puesto asignado.">
            {(atributos) => (
              <Entrada
                {...atributos}
                value={formulario.valores.ubicacion}
                onChange={(evento) => formulario.cambiar('ubicacion', evento.target.value)}
                placeholder="Almacén TI - Estante 1"
              />
            )}
          </Campo>

          <Campo etiqueta="Fecha de adquisición" error={formulario.errores.fechaAdquisicion}>
            {(atributos) => (
              <Entrada
                {...atributos}
                type="date"
                max={hoyISO()}
                value={formulario.valores.fechaAdquisicion}
                onChange={(evento) => formulario.cambiar('fechaAdquisicion', evento.target.value)}
              />
            )}
          </Campo>

          <div className="sm:col-span-2">
            <Campo etiqueta="Especificaciones" error={formulario.errores.especificaciones}>
              {(atributos) => (
                <AreaTexto
                  {...atributos}
                  value={formulario.valores.especificaciones}
                  onChange={(evento) => formulario.cambiar('especificaciones', evento.target.value)}
                  placeholder="Procesador, memoria, almacenamiento, accesorios entregados…"
                />
              )}
            </Campo>
          </div>
        </form>
      </Modal>

      {/* ── Cambio de estado ──────────────────────────────────────────────── */}
      <Modal
        abierto={Boolean(equipoEstado)}
        cerrar={() => setEquipoEstado(null)}
        titulo="Actualizar estado del equipo"
        descripcion={equipoEstado ? `${equipoEstado.equipo.codigoPatrimonial} · ${equipoEstado.equipo.nombre}` : ''}
        ancho="pequeno"
        pie={
          <>
            <Boton variante="secundario" onClick={() => setEquipoEstado(null)}>
              Cancelar
            </Boton>
            <Boton onClick={guardarEstado} cargando={procesando}>
              Actualizar estado
            </Boton>
          </>
        }
      >
        {equipoEstado && (
          <div className="space-y-4">
            <Campo etiqueta="Nuevo estado" requerido>
              {(atributos) => (
                <Seleccion
                  {...atributos}
                  value={equipoEstado.estado}
                  onChange={(evento) => setEquipoEstado((actual) => ({ ...actual, estado: Number(evento.target.value) }))}
                >
                  <option value={ESTADOS_EQUIPO.DISPONIBLE}>Disponible</option>
                  <option value={ESTADOS_EQUIPO.MANTENIMIENTO}>Mantenimiento</option>
                  <option value={ESTADOS_EQUIPO.BAJA}>Baja</option>
                </Seleccion>
              )}
            </Campo>

            <Campo
              etiqueta="Motivo del cambio"
              requerido={equipoEstado.estado !== ESTADOS_EQUIPO.DISPONIBLE}
              ayuda="Se registra en la bitácora de movimientos del sistema."
            >
              {(atributos) => (
                <AreaTexto
                  {...atributos}
                  rows={3}
                  value={equipoEstado.motivo}
                  onChange={(evento) => setEquipoEstado((actual) => ({ ...actual, motivo: evento.target.value }))}
                  placeholder="Ej.: revisión técnica programada por el área de soporte."
                />
              )}
            </Campo>
          </div>
        )}
      </Modal>

      {/* ── Confirmación de eliminación ───────────────────────────────────── */}
      <ConfirmarDialogo
        abierto={Boolean(equipoEliminar)}
        cerrar={() => setEquipoEliminar(null)}
        confirmar={eliminar}
        cargando={procesando}
        peligro
        titulo="Eliminar equipo"
        textoConfirmar="Eliminar equipo"
        mensaje={
          equipoEliminar
            ? `¿Confirma la eliminación del equipo ${equipoEliminar.codigoPatrimonial} · ${equipoEliminar.nombre}? Esta acción no se puede deshacer.`
            : ''
        }
        descripcion="Los equipos con historial de préstamos no pueden eliminarse (RN-06); deben pasar a estado «Baja»."
      />
    </>
  );
}
