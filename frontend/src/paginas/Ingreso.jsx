import { useEffect, useState } from 'react';
import { Link, Navigate, useLocation, useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../contexto/AuthContexto';
import { useToast } from '../contexto/ToastContexto';
import { Boton } from '../componentes/ui/Boton';
import { Campo, Entrada } from '../componentes/ui/Campo';
import { Icone } from '../componentes/ui/Icone';
import { Notificaciones } from '../componentes/ui/Notificaciones';
import { IconoInnovatech, LogotipoInnovatech } from '../componentes/marca/LogoInnovatech';
import imagenLogin from '../assets/login-innovatech.jpg';

const CUENTAS_DEMO = [
  { rol: 'Administrador', correo: 'elena.quispe@techasset.pe', contrasena: 'Admin1234', descripcion: 'Gestiona usuarios, equipos, préstamos y reportes' },
  { rol: 'Responsable de Tecnología', correo: 'rodrigo.vasquez@techasset.pe', contrasena: 'Responsable1234', descripcion: 'Registra equipos, aprueba préstamos y devoluciones' },
  { rol: 'Colaborador', correo: 'andrea.flores@techasset.pe', contrasena: 'Colaborador1234', descripcion: 'Solicita equipos y consulta su historial' }
];

/**
 * Pantalla de acceso. Presenta el producto, los actores del sistema y permite
 * iniciar sesión con cuentas de demostración (reconocimiento en lugar de
 * memorización, heurística de Nielsen nº 6).
 */
export function Ingreso() {
  const { autenticado, cargando, iniciarSesion } = useAuth();
  const toast = useToast();
  const navegar = useNavigate();
  const ubicacion = useLocation();
  const [parametros] = useSearchParams();

  const [correo, setCorreo] = useState('');
  const [contrasena, setContrasena] = useState('');
  const [verContrasena, setVerContrasena] = useState(false);
  const [error, setError] = useState(null);
  const [enviando, setEnviando] = useState(false);

  useEffect(() => {
    if (parametros.get('sesion') === 'expirada') {
      toast.advertencia('Su sesión expiró. Ingrese nuevamente con sus credenciales.');
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (!cargando && autenticado) {
    return <Navigate to={ubicacion.state?.desde ?? '/panel'} replace />;
  }

  const enviar = async (evento) => {
    evento.preventDefault();
    setError(null);

    if (!correo.trim() || !contrasena) {
      setError('Ingrese su correo y contraseña para continuar.');
      return;
    }

    setEnviando(true);
    try {
      const usuario = await iniciarSesion({ correo: correo.trim(), contrasena });
      toast.exito(`Bienvenido(a), ${usuario.nombres}. Sesión iniciada correctamente.`);
      navegar(ubicacion.state?.desde ?? '/panel', { replace: true });
    } catch (fallo) {
      setError(fallo.message || 'No fue posible iniciar sesión.');
    } finally {
      setEnviando(false);
    }
  };

  const accesoRapido = async (cuenta) => {
    setCorreo(cuenta.correo);
    setContrasena(cuenta.contrasena);
    setError(null);
    setEnviando(true);
    try {
      const usuario = await iniciarSesion(cuenta);
      toast.exito(`Sesión iniciada como ${usuario.nombreCompleto}.`);
      navegar('/panel', { replace: true });
    } catch (fallo) {
      setError(fallo.message || 'No fue posible iniciar sesión.');
    } finally {
      setEnviando(false);
    }
  };

  return (
    <div className="min-h-screen bg-surface-2 lg:grid lg:grid-cols-[minmax(0,3fr)_minmax(0,2fr)]">
      {/* Panel de marca */}
      <section className="relative hidden overflow-hidden bg-[#020b18] px-10 py-14 text-white lg:flex lg:flex-col lg:items-center lg:justify-center">
        <img
          src={imagenLogin}
          alt=""
          aria-hidden="true"
          className="absolute inset-0 h-full w-full object-cover object-[38%_center] opacity-95"
        />
        <div className="absolute inset-0 bg-[#020b18]/45" aria-hidden="true" />
        <div
          className="absolute inset-0 bg-gradient-to-br from-[#020b18]/85 via-[#041a38]/55 to-[#062a54]/20"
          aria-hidden="true"
        />
        <div
          className="pointer-events-none absolute inset-0 opacity-[0.07]"
          style={{
            backgroundImage:
              'linear-gradient(rgba(34, 211, 238, 0.7) 1px, transparent 1px), linear-gradient(90deg, rgba(34, 211, 238, 0.7) 1px, transparent 1px)',
            backgroundSize: '56px 56px'
          }}
          aria-hidden="true"
        />
        <div
          className="pointer-events-none absolute left-1/2 top-1/2 h-[28rem] w-[28rem] -translate-x-1/2 -translate-y-1/2 rounded-full bg-cyan-400/15 blur-3xl"
          aria-hidden="true"
        />
        <div
          className="pointer-events-none absolute -right-24 -top-24 h-96 w-96 rounded-full bg-cyan-300/10 blur-2xl"
          aria-hidden="true"
        />

        <div className="relative flex flex-col items-center text-center">
          <LogotipoInnovatech />
          <p className="mt-8 max-w-xs text-sm leading-relaxed text-slate-300">
            Sistema de gestión de inventario y préstamos de equipos tecnológicos.
          </p>
        </div>

        <p className="absolute bottom-8 text-xs text-slate-400">
          Caso práctico integrador · Construcción de Software · API ASP.NET Core 8 + React + PostgreSQL
        </p>
      </section>

      {/* Formulario de acceso */}
      <section className="flex min-h-screen items-center justify-center px-5 py-10 sm:px-8">
        <div className="w-full max-w-md">
          <div className="mb-8 flex items-center justify-between gap-3">
            <div className="flex items-center gap-3 lg:hidden">
              <IconoInnovatech className="h-9 w-9 !text-slate-900 dark:!text-white" />
              <div>
                <p className="text-lg font-black leading-none tracking-[-0.035em] text-ink">Innova<span className="text-cyan-600 dark:text-cyan-400">TECH</span></p>
                <p className="text-[10px] font-bold uppercase tracking-[0.38em] text-slate-500 dark:text-slate-400">
                  Solutions
                </p>
              </div>
            </div>

            <Link
              to="/"
              className="group inline-flex shrink-0 items-center gap-1.5 text-sm font-medium text-muted transition hover:text-brand-600 dark:hover:text-brand-300"
            >
              <Icone nombre="devolucion" className="h-4 w-4 transition group-hover:-translate-x-0.5" />
              Volver al inicio
            </Link>
          </div>

          <h2 className="text-2xl font-bold tracking-tight text-ink">Iniciar sesión</h2>
          <p className="mt-1.5 text-sm text-muted">
            Acceda con las credenciales asignadas por la Gerencia de Tecnología.
          </p>

          <form onSubmit={enviar} className="mt-7 space-y-4" noValidate>
            <Campo etiqueta="Correo electrónico" requerido>
              {(atributos) => (
                <Entrada
                  {...atributos}
                  type="email"
                  name="correo"
                  autoComplete="username"
                  placeholder="usuario@techasset.pe"
                  value={correo}
                  onChange={(evento) => setCorreo(evento.target.value)}
                />
              )}
            </Campo>

            <Campo etiqueta="Contraseña" requerido>
              {(atributos) => (
                <div className="relative">
                  <Entrada
                    {...atributos}
                    type={verContrasena ? 'text' : 'password'}
                    name="contrasena"
                    autoComplete="current-password"
                    placeholder="••••••••"
                    className="pr-11"
                    value={contrasena}
                    onChange={(evento) => setContrasena(evento.target.value)}
                  />
                  <button
                    type="button"
                    onClick={() => setVerContrasena((visible) => !visible)}
                    className="absolute right-2 top-1/2 -translate-y-1/2 rounded-md p-1.5 text-muted transition hover:bg-surface-3 hover:text-ink"
                    aria-label={verContrasena ? 'Ocultar contraseña' : 'Mostrar contraseña'}
                  >
                    <Icone nombre={verContrasena ? 'ojoTachado' : 'ojo'} className="h-4 w-4" />
                  </button>
                </div>
              )}
            </Campo>

            {error && (
              <div role="alert" className="flex items-start gap-2.5 rounded-xl border border-rose-200 bg-rose-50 px-3.5 py-3 text-sm text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-200">
                <Icone nombre="alerta" className="mt-0.5 h-4 w-4 shrink-0" />
                <span>{error}</span>
              </div>
            )}

            <Boton type="submit" cargando={enviando} className="w-full" tamano="grande">
              Ingresar al sistema
            </Boton>
          </form>

          <div className="mt-8">
            <div className="flex items-center gap-3">
              <span className="h-px flex-1 bg-line" />
              <span className="text-xs font-semibold uppercase tracking-wider text-muted">Accesos de demostración</span>
              <span className="h-px flex-1 bg-line" />
            </div>

            <div className="mt-4 space-y-2">
              {CUENTAS_DEMO.map((cuenta) => (
                <button
                  key={cuenta.correo}
                  type="button"
                  onClick={() => accesoRapido(cuenta)}
                  disabled={enviando}
                  className="group flex w-full items-center gap-3 rounded-xl border border-line bg-surface px-3.5 py-3 text-left transition hover:border-brand-400 hover:bg-brand-50/60 disabled:opacity-60 dark:hover:bg-brand-500/10"
                >
                  <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-brand-50 text-brand-600 transition group-hover:bg-white dark:bg-brand-500/15 dark:text-brand-300 dark:group-hover:bg-brand-500/25">
                    <Icone nombre="usuarios" className="h-4 w-4" />
                  </span>
                  <span className="min-w-0 flex-1">
                    <span className="block text-sm font-semibold text-ink">{cuenta.rol}</span>
                    <span className="block truncate text-xs text-muted">{cuenta.descripcion}</span>
                  </span>
                  <Icone nombre="chevron" className="h-4 w-4 shrink-0 text-muted transition group-hover:translate-x-0.5 group-hover:text-brand-600 dark:group-hover:text-brand-300" />
                </button>
              ))}
            </div>
          </div>
        </div>
      </section>

      <Notificaciones />
    </div>
  );
}
