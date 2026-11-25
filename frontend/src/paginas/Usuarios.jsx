import { useEffect, useMemo, useState } from 'react';
import { servicios } from '../api/servicios';
import { usePeticion, useDebounce } from '../ganchos/usePeticion';
import { usePaginacion } from '../ganchos/usePaginacion';
import { useFormulario } from '../ganchos/useFormulario';
import { useToast } from '../contexto/ToastContexto';
import { useAuth } from '../contexto/AuthContexto';
import { EncabezadoPagina } from '../componentes/layout/EncabezadoPagina';
import { Boton } from '../componentes/ui/Boton';
import { Campo, Entrada, Seleccion } from '../componentes/ui/Campo';
import { Modal, ConfirmarDialogo } from '../componentes/ui/Modal';
import { Tabla } from '../componentes/ui/Tabla';
import { Paginacion } from '../componentes/ui/Paginacion';
import { Tarjeta } from '../componentes/ui/Tarjeta';
import { Buscador, EstadoVacio } from '../componentes/ui/Estados';
import { Insignia } from '../componentes/ui/Insignia';
import { Icone } from '../componentes/ui/Icone';
import { formatearFechaHora } from '../utilidades/formateadores';
import { reglasUsuario } from '../utilidades/validaciones';
import { NOMBRES_ROL, ROLES } from '../utilidades/constantes';

const VALORES_INICIALES = {
  nombres: '',
  apellidos: '',
  correo: '',
  area: '',
  rol: ROLES.COLABORADOR,
  contrasena: '',
  confirmarContrasena: ''
};

/**
 * Gestión de usuarios (RF-01 … RF-04). El alta y la modificación son
 * exclusivas del Administrador (RN-09), igual que la habilitación/deshabilitación.
 */
export function Usuarios() {
  const toast = useToast();
  const { esAdministrador } = useAuth();

  const [busqueda, setBusqueda] = useState('');
  const busquedaDiferida = useDebounce(busqueda, 350);
  const { pagina, setPagina, tamanoPagina, setTamanoPagina, filtros, setFiltros } = usePaginacion();

  const parametros = useMemo(
    () => ({
      pagina,
      tamanoPagina,
      busqueda: busquedaDiferida || undefined,
      area: filtros.area || undefined,
      rol: filtros.rol || undefined,
      soloActivos: filtros.estado === 'activos' ? true : filtros.estado === 'inactivos' ? false : undefined
    }),
    [pagina, tamanoPagina, busquedaDiferida, filtros]
  );

  const { datos, cargando, error, recargar } = usePeticion(() => servicios.usuarios.listar(parametros), [parametros]);
  const { datos: areas } = usePeticion(() => servicios.usuarios.areas(), []);

  const [modalAbierto, setModalAbierto] = useState(false);
  const [usuarioEditando, setUsuarioEditando] = useState(null);
  const [usuarioEstado, setUsuarioEstado] = useState(null);
  const [procesando, setProcesando] = useState(false);

  const formulario = useFormulario(VALORES_INICIALES, {
    ...reglasUsuario,
    confirmarContrasena: (valor, ) => {
      if (!valor) return 'Debe confirmar la contraseña.';
      if (valor !== formulario.valores.contrasena) return 'Las contraseñas no coinciden.';
      return null;
    }
  });

  useEffect(() => {
    setPagina(1);
  }, [busquedaDiferida, filtros, setPagina]);

  const abrirNuevo = () => {
    setUsuarioEditando(null);
    formulario.establecerValores(VALORES_INICIALES);
    setModalAbierto(true);
  };

  const abrirEdicion = (usuario) => {
    setUsuarioEditando(usuario);
    formulario.establecerValores({
      nombres: usuario.nombres,
      apellidos: usuario.apellidos,
      correo: usuario.correo,
      area: usuario.area,
      rol: usuario.rol,
      contrasena: '',
      confirmarContrasena: ''
    });
    setModalAbierto(true);
  };

  const guardar = async (evento) => {
    evento.preventDefault();
    try {
      const { exito } = await formulario.enviar(async (valores) => {
        if (usuarioEditando) {
          return servicios.usuarios.actualizar(usuarioEditando.id, {
            nombres: valores.nombres,
            apellidos: valores.apellidos,
            correo: valores.correo,
            area: valores.area,
            rol: Number(valores.rol)
          });
        }

        return servicios.usuarios.crear({
          nombres: valores.nombres,
          apellidos: valores.apellidos,
          correo: valores.correo,
          area: valores.area,
          rol: Number(valores.rol),
          contrasena: valores.contrasena,
          confirmarContrasena: valores.confirmarContrasena
        });
      });

      if (!exito) {
        toast.error('Revise los campos marcados: hay datos por corregir.');
        return;
      }

      setModalAbierto(false);
      toast.exito(usuarioEditando ? 'Usuario actualizado correctamente.' : 'Usuario registrado correctamente.');
      recargar();
    } catch (fallo) {
      toast.error(fallo.message);
    }
  };

  const cambiarEstado = async () => {
    setProcesando(true);
    try {
      const habilitar = usuarioEstado.estado;
      await servicios.usuarios.cambiarEstado(usuarioEstado.usuario.id, {
        habilitar,
        motivo: habilitar ? null : usuarioEstado.motivo
      });
      toast.exito(
        habilitar
          ? `Usuario ${usuarioEstado.usuario.nombreCompleto} reactivado.`
          : `Usuario ${usuarioEstado.usuario.nombreCompleto} deshabilitado.`
      );
      setUsuarioEstado(null);
      recargar();
    } catch (fallo) {
      toast.error(fallo.message);
    } finally {
      setProcesando(false);
    }
  };

  const columnas = [
    {
      clave: 'nombreCompleto',
      titulo: 'Colaborador',
      render: (usuario) => (
        <div className="flex min-w-0 items-center gap-3">
          <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-brand-50 text-xs font-bold text-brand-700 dark:bg-brand-500/15 dark:text-brand-300">
            {usuario.nombres.charAt(0)}
            {usuario.apellidos.charAt(0)}
          </span>
          <div className="min-w-0">
            <p className="truncate font-semibold text-ink">{usuario.nombreCompleto}</p>
            <p className="truncate text-xs text-muted">{usuario.correo}</p>
          </div>
        </div>
      )
    },
    { clave: 'area', titulo: 'Área', render: (usuario) => <span className="text-muted">{usuario.area}</span> },
    {
      clave: 'rol',
      titulo: 'Rol',
      render: (usuario) => (
        <Insignia tono={usuario.rol === ROLES.ADMINISTRADOR ? 'marca' : usuario.rol === ROLES.RESPONSABLE_TECNOLOGIA ? 'info' : 'neutro'}>
          {usuario.rolNombre}
        </Insignia>
      )
    },
    {
      clave: 'estado',
      titulo: 'Estado',
      render: (usuario) =>
        usuario.estado ? (
          <Insignia tono="exito">Activo</Insignia>
        ) : (
          <Insignia tono="peligro" >
            Deshabilitado
          </Insignia>
        )
    },
    { clave: 'totalPrestamos', titulo: 'Préstamos', render: (usuario) => <span className="text-muted">{usuario.totalPrestamos}</span> },
    {
      clave: 'ultimoAcceso',
      titulo: 'Último acceso',
      render: (usuario) => <span className="text-xs text-muted">{usuario.ultimoAcceso ? formatearFechaHora(usuario.ultimoAcceso) : 'Nunca'}</span>
    },
    {
      clave: 'acciones',
      titulo: 'Acciones',
      claseCelda: 'text-right',
      render: (usuario) => (
        <div className="flex items-center justify-end gap-1">
          {esAdministrador && (
            <button
              type="button"
              onClick={() => abrirEdicion(usuario)}
              className="rounded-lg p-1.5 text-muted transition hover:bg-surface-3 hover:text-ink"
              title="Editar usuario"
              aria-label={`Editar ${usuario.nombreCompleto}`}
            >
              <Icone nombre="editar" className="h-4 w-4" />
            </button>
          )}
          {esAdministrador && (
            <button
              type="button"
              onClick={() => setUsuarioEstado({ usuario, estado: !usuario.estado, motivo: '' })}
              className={`rounded-lg p-1.5 transition ${
                usuario.estado
                  ? 'text-muted hover:bg-rose-50 hover:text-rose-600 dark:hover:bg-rose-500/10'
                  : 'text-emerald-600 hover:bg-emerald-50 dark:hover:bg-emerald-500/10'
              }`}
              title={usuario.estado ? 'Deshabilitar usuario' : 'Reactivar usuario'}
              aria-label={`${usuario.estado ? 'Deshabilitar' : 'Reactivar'} ${usuario.nombreCompleto}`}
            >
              <Icone nombre={usuario.estado ? 'basura' : 'verificar'} className="h-4 w-4" />
            </button>
          )}
        </div>
      )
    }
  ];

  return (
    <>
      <EncabezadoPagina
        titulo="Gestión de usuarios"
        descripcion="Registre, actualice y controle el acceso de los colaboradores de Innovatech Solutions."
        acciones={
          esAdministrador && (
            <Boton onClick={abrirNuevo}>
              <Icone nombre="mas" className="h-4 w-4" />
              Registrar usuario
            </Boton>
          )
        }
      />

      <Tarjeta>
        <div className="flex flex-col gap-3 border-b border-line p-4 lg:flex-row lg:items-center">
          <Buscador valor={busqueda} onCambio={setBusqueda} placeholder="Buscar por nombre, correo o área…" className="w-full lg:max-w-sm" />

          <div className="grid flex-1 grid-cols-1 gap-3 sm:grid-cols-3">
            <Seleccion
              value={filtros.area ?? ''}
              onChange={(evento) => setFiltros((actuales) => ({ ...actuales, area: evento.target.value }))}
              placeholder="Todas las áreas"
              aria-label="Filtrar por área"
            >
              {(areas ?? []).map((area) => (
                <option key={area} value={area}>
                  {area}
                </option>
              ))}
            </Seleccion>

            <Seleccion
              value={filtros.rol ?? ''}
              onChange={(evento) => setFiltros((actuales) => ({ ...actuales, rol: evento.target.value }))}
              placeholder="Todos los roles"
              aria-label="Filtrar por rol"
            >
              {Object.entries(NOMBRES_ROL).map(([valor, etiqueta]) => (
                <option key={valor} value={valor}>
                  {etiqueta}
                </option>
              ))}
            </Seleccion>

            <Seleccion
              value={filtros.estado ?? ''}
              onChange={(evento) => setFiltros((actuales) => ({ ...actuales, estado: evento.target.value }))}
              placeholder="Todos los estados"
              aria-label="Filtrar por estado"
            >
              <option value="activos">Solo activos</option>
              <option value="inactivos">Solo deshabilitados</option>
            </Seleccion>
          </div>
        </div>

        <Tabla
          columnas={columnas}
          datos={datos?.items ?? []}
          cargando={cargando}
          error={error?.message}
          vacio={<EstadoVacio titulo="No se encontraron usuarios" descripcion="Ajuste los filtros o registre un nuevo colaborador." />}
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
            etiqueta="usuarios"
          />
        )}
      </Tarjeta>

      {/* ── Formulario de usuario ─────────────────────────────────────────── */}
      <Modal
        abierto={modalAbierto}
        cerrar={() => setModalAbierto(false)}
        titulo={usuarioEditando ? `Editar ${usuarioEditando.nombreCompleto}` : 'Registrar usuario'}
        descripcion={
          usuarioEditando
            ? 'Actualice los datos del colaborador y su rol en el sistema.'
            : 'Complete los datos del nuevo colaborador. El correo electrónico debe ser único (RN-08).'
        }
        ancho="mediano"
        pie={
          <>
            <Boton variante="secundario" onClick={() => setModalAbierto(false)}>
              Cancelar
            </Boton>
            <Boton onClick={guardar} cargando={formulario.enviando}>
              {usuarioEditando ? 'Guardar cambios' : 'Registrar usuario'}
            </Boton>
          </>
        }
      >
        <form onSubmit={guardar} className="grid grid-cols-1 gap-4 sm:grid-cols-2" noValidate>
          <Campo etiqueta="Nombres" requerido error={formulario.errores.nombres}>
            {(atributos) => (
              <Entrada
                {...atributos}
                value={formulario.valores.nombres}
                onChange={(evento) => formulario.cambiar('nombres', evento.target.value)}
                onBlur={() => formulario.marcarTocado('nombres')}
                placeholder="María"
              />
            )}
          </Campo>

          <Campo etiqueta="Apellidos" requerido error={formulario.errores.apellidos}>
            {(atributos) => (
              <Entrada
                {...atributos}
                value={formulario.valores.apellidos}
                onChange={(evento) => formulario.cambiar('apellidos', evento.target.value)}
                onBlur={() => formulario.marcarTocado('apellidos')}
                placeholder="Quispe Medina"
              />
            )}
          </Campo>

          <Campo etiqueta="Correo electrónico" requerido error={formulario.errores.correo}>
            {(atributos) => (
              <Entrada
                {...atributos}
                type="email"
                value={formulario.valores.correo}
                onChange={(evento) => formulario.cambiar('correo', evento.target.value)}
                onBlur={() => formulario.marcarTocado('correo')}
                placeholder="usuario@techasset.pe"
              />
            )}
          </Campo>

          <Campo etiqueta="Área" requerido error={formulario.errores.area}>
            {(atributos) => (
              <Entrada
                {...atributos}
                list="areas-registradas"
                value={formulario.valores.area}
                onChange={(evento) => formulario.cambiar('area', evento.target.value)}
                onBlur={() => formulario.marcarTocado('area')}
                placeholder="Contabilidad"
              />
            )}
          </Campo>
          <datalist id="areas-registradas">
            {(areas ?? []).map((area) => (
              <option key={area} value={area} />
            ))}
          </datalist>

          <Campo etiqueta="Rol" requerido error={formulario.errores.rol}>
            {(atributos) => (
              <Seleccion
                {...atributos}
                value={formulario.valores.rol}
                onChange={(evento) => formulario.cambiar('rol', Number(evento.target.value))}
              >
                {Object.entries(NOMBRES_ROL).map(([valor, etiqueta]) => (
                  <option key={valor} value={valor}>
                    {etiqueta}
                  </option>
                ))}
              </Seleccion>
            )}
          </Campo>

          {!usuarioEditando && (
            <>
              <Campo etiqueta="Contraseña" requerido error={formulario.errores.contrasena} ayuda="Mínimo 8 caracteres, con letras y números.">
                {(atributos) => (
                  <Entrada
                    {...atributos}
                    type="password"
                    autoComplete="new-password"
                    value={formulario.valores.contrasena}
                    onChange={(evento) => formulario.cambiar('contrasena', evento.target.value)}
                    onBlur={() => formulario.marcarTocado('contrasena')}
                  />
                )}
              </Campo>

              <div className="sm:col-span-2">
                <Campo etiqueta="Confirmar contraseña" requerido error={formulario.errores.confirmarContrasena}>
                  {(atributos) => (
                    <Entrada
                      {...atributos}
                      type="password"
                      autoComplete="new-password"
                      value={formulario.valores.confirmarContrasena}
                      onChange={(evento) => formulario.cambiar('confirmarContrasena', evento.target.value)}
                      onBlur={() => formulario.marcarTocado('confirmarContrasena')}
                    />
                  )}
                </Campo>
              </div>
            </>
          )}
        </form>
      </Modal>

      {/* ── Cambio de estado ──────────────────────────────────────────────── */}
      <ConfirmarDialogo
        abierto={Boolean(usuarioEstado)}
        cerrar={() => setUsuarioEstado(null)}
        confirmar={cambiarEstado}
        cargando={procesando}
        peligro={usuarioEstado?.estado === false}
        titulo={usuarioEstado?.estado ? 'Deshabilitar usuario' : 'Reactivar usuario'}
        textoConfirmar={usuarioEstado?.estado ? 'Deshabilitar' : 'Reactivar'}
        descripcion="Solo el Administrador puede habilitar o deshabilitar usuarios (RN-09)."
        mensaje={
          usuarioEstado
            ? usuarioEstado.estado
              ? `¿Confirma la reactivación de ${usuarioEstado.usuario.nombreCompleto}? El usuario podrá volver a iniciar sesión.`
              : `¿Confirma la deshabilitación de ${usuarioEstado.usuario.nombreCompleto}? No podrá recibir equipos ni iniciar sesión.`
            : ''
        }
      />
    </>
  );
}
