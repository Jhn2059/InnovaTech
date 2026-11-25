import { Link } from 'react-router-dom';
import { useAuth } from '../contexto/AuthContexto';
import { useTema } from '../ganchos/useTema';
import { Icone } from '../componentes/ui/Icone';
import { Insignia } from '../componentes/ui/Insignia';
import { IlustracionFlujo } from '../componentes/ilustraciones/IlustracionFlujo';
import { IconoInnovatech, MarcaInnovatech } from '../componentes/marca/LogoInnovatech';
import imagenHero from '../assets/hero-innovatech.jpg';

const ESTADISTICAS = [
  { valor: '64', etiqueta: 'equipos registrados', icono: 'monitor' },
  { valor: '25', etiqueta: 'colaboradores activos', icono: 'usuarios' },
  { valor: '52', etiqueta: 'préstamos gestionados', icono: 'prestamos' },
  { valor: '10', etiqueta: 'reglas de negocio', icono: 'verificar' }
];

const MODULOS = [
  {
    icono: 'panel',
    titulo: 'Panel general',
    texto: 'Vista ejecutiva con tasa de ocupación, equipos por estado, demanda por área y tendencia mensual.'
  },
  {
    icono: 'monitor',
    titulo: 'Inventario de equipos',
    texto: 'Catálogo con código patrimonial, categoría, marca, modelo, ubicación y estado de cada activo.'
  },
  {
    icono: 'prestamos',
    titulo: 'Préstamos y devoluciones',
    texto: 'Registro de entregas, control de préstamos activos y devoluciones con validaciones automáticas.'
  },
  {
    icono: 'graficos',
    titulo: 'Reportes e indicadores',
    texto: 'Gráficos para decidir y exportación de resultados en CSV para análisis externo.'
  },
  {
    icono: 'usuarios',
    titulo: 'Usuarios y roles',
    texto: 'Permisos diferenciados para Administrador, Responsable de Tecnología y Colaborador.'
  },
  {
    icono: 'reportes',
    titulo: 'Bitácora de auditoría',
    texto: 'Historial trazable de cada movimiento con usuario, fecha y detalle para control interno.'
  }
];

const PASOS = [
  {
    titulo: 'Registre el equipo',
    texto: 'Alta con código patrimonial, categoría, ubicación y estado inicial dentro del inventario.'
  },
  {
    titulo: 'Gestione el préstamo',
    texto: 'Solicitud, aprobación y entrega con reglas de negocio que evitan conflictos de disponibilidad.'
  },
  {
    titulo: 'Devuelva y audite',
    texto: 'Cierre del préstamo, actualización del estado y registro automático en la bitácora de movimientos.'
  }
];

const REGLAS = [
  'Un equipo prestado no puede volver a prestarse.',
  'No se elimina un equipo con historial de movimientos.',
  'La devolución libera el equipo y registra la bitácora.',
  'Solo los administradores gestionan usuarios y permisos.'
];

const ROLES = [
  {
    nombre: 'Administrador',
    icono: 'ajustes',
    descripcion: 'Control completo del sistema y de la configuración de usuarios.',
    permisos: ['Gestiona usuarios y roles', 'Administra equipos y préstamos', 'Consulta reportes y auditoría'],
    cuenta: 'elena.quispe@techasset.pe',
    clave: 'Admin1234'
  },
  {
    nombre: 'Responsable de Tecnología',
    icono: 'equipos',
    descripcion: 'Operación diaria del inventario y de los préstamos institucionales.',
    permisos: ['Registra y actualiza equipos', 'Aprueba préstamos y devoluciones', 'Consulta reportes y bitácora'],
    cuenta: 'rodrigo.vasquez@techasset.pe',
    clave: 'Responsable1234'
  },
  {
    nombre: 'Colaborador',
    icono: 'usuarios',
    descripcion: 'Acceso al inventario disponible y a su propio historial de préstamos.',
    permisos: ['Consulta el inventario disponible', 'Solicita préstamos de equipos', 'Revisa su historial de préstamos'],
    cuenta: 'andrea.flores@techasset.pe',
    clave: 'Colaborador1234'
  }
];

const NAVEGACION = [
  { texto: 'Inicio', href: '#inicio' },
  { texto: 'Inventario', href: '#modulos' },
  { texto: 'Préstamos', href: '#como-funciona' },
  { texto: 'Reportes', href: '#indicadores' },
  { texto: 'Acerca de', href: '#roles' }
];

const ICONOS_ESCENA = [
  { icono: 'monitor', texto: 'Inventario' },
  { icono: 'calendario', texto: 'Préstamos' },
  { icono: 'graficos', texto: 'Reportes' }
];

function VisualHero() {
  return (
    <div className="relative mx-auto w-full max-w-xl lg:max-w-none" aria-hidden="true">
      <div className="pointer-events-none absolute -inset-10 rounded-full bg-blue-8000/10 blur-2xl dark:bg-cyan-500/10" />

      <div className="relative overflow-hidden rounded-[2rem] border border-blue-800/20 bg-slate-100 dark:bg-[#041326] p-2 shadow-2xl shadow-blue-950/60 dark:border-cyan-400/20 dark:shadow-cyan-950/60">
        <div className="relative overflow-hidden rounded-[1.5rem]">
          <img
            src={imagenHero}
            alt=""
            className="h-[420px] w-full object-cover object-center"
          />
          <div className="absolute inset-0 bg-gradient-to-tr from-[#020b18]/95 via-[#031a36]/70 to-blue-800/20 dark:to-cyan-400/20" />
          <div className="absolute inset-0 bg-[radial-gradient(circle_at_58%_45%,rgba(34,211,238,0.3),transparent_45%)]" />

          <div className="absolute left-1/2 top-1/2 h-72 w-72 -translate-x-1/2 -translate-y-1/2 rounded-full border border-blue-800/20 dark:border-cyan-300/20" />
          <div className="absolute left-1/2 top-1/2 h-96 w-96 -translate-x-1/2 -translate-y-1/2 rounded-full border border-dashed border-blue-800/15 dark:border-cyan-300/15" />
          <div className="absolute left-1/2 top-1/2 h-[27rem] w-[27rem] -translate-x-1/2 -translate-y-1/2 rounded-full border border-blue-800/10 dark:border-cyan-300/10" />

          <div className="absolute right-4 top-5 hidden flex-col gap-2.5 sm:flex">
            {ICONOS_ESCENA.map((elemento) => (
              <div
                key={elemento.texto}
                className="flex items-center gap-2.5 rounded-xl border border-blue-800/25 bg-slate-50/70 dark:bg-[#031226]/70 px-3 py-2 text-xs font-semibold text-slate-950 dark:text-white backdrop-blur-md dark:border-cyan-300/25"
              >
                <span className="flex h-7 w-7 items-center justify-center rounded-lg bg-blue-800/15 text-blue-800 dark:text-cyan-300 dark:bg-cyan-400/15 dark:text-cyan-600">
                  <Icone nombre={elemento.icono} className="h-4 w-4" />
                </span>
                {elemento.texto}
              </div>
            ))}
          </div>

          <div className="absolute bottom-5 left-5 flex items-center gap-2.5 rounded-2xl border border-blue-800/25 bg-slate-50/80 dark:bg-[#031226]/80 px-3 py-2.5 backdrop-blur-md dark:border-cyan-300/25">
            <IconoInnovatech className="h-9 w-9" />
            <div>
              <p className="text-xs font-extrabold leading-none text-slate-950 dark:text-white">Innovatech</p>
              <p className="mt-1 text-[9px] font-bold uppercase tracking-[0.22em] text-blue-800 dark:text-cyan-300 dark:text-cyan-600">Solutions</p>
            </div>
          </div>

          <div className="absolute bottom-5 right-5 hidden items-center gap-1.5 rounded-full border border-emerald-300/25 bg-slate-50/80 dark:bg-[#031226]/80 px-3 py-1.5 text-[10px] font-semibold text-emerald-300 backdrop-blur-md sm:flex">
            <span className="h-1.5 w-1.5 rounded-full bg-emerald-400" />
            Sistema en línea
          </div>
        </div>
      </div>
    </div>
  );
}

export function Landing() {
  const { autenticado } = useAuth();
  const { tema, alternarTema } = useTema();
  const esOscuro = tema === 'oscuro';
  const destinoPrincipal = autenticado ? '/panel' : '/login';
  const textoPrincipal = autenticado ? 'Ir al panel' : 'Iniciar sesión';

  return (
    <div className="min-h-screen bg-white text-slate-900 dark:bg-[#020b18] dark:text-slate-100">
      <a
        href="#contenido-principal"
        className="sr-only focus:not-sr-only focus:absolute focus:left-4 focus:top-4 focus:z-50 focus:rounded-lg focus:bg-blue-800 focus:px-4 focus:py-2 focus:text-sm focus:font-semibold focus:text-slate-950 dark:focus:bg-cyan-400"
      >
        Saltar al contenido principal
      </a>

      <header className="sticky top-0 z-40 border-b border-slate-200 dark:border-white/10 bg-white dark:bg-[#020b18]/85">
        <div className="mx-auto flex h-20 max-w-7xl items-center justify-between gap-4 px-4 sm:px-6 lg:px-8">
          <Link to="/" className="shrink-0" aria-label="Innovatech Solutions, ir al inicio">
            <MarcaInnovatech compacto claro={esOscuro} />
          </Link>

          <nav className="hidden items-center gap-1 lg:flex" aria-label="Navegación principal">
            {NAVEGACION.map((enlace, indice) => (
              <a
                key={enlace.texto}
                href={enlace.href}
                className={`relative rounded-lg px-4 py-2 text-sm font-medium transition ${
                  indice === 0
                    ? 'text-blue-600 dark:text-cyan-300 after:absolute after:inset-x-4 after:-bottom-1 after:h-0.5 after:rounded-full after:bg-blue-400 dark:after:bg-cyan-400'
                    : 'text-slate-600 dark:text-slate-300 hover:bg-slate-100 dark:bg-white/5 hover:text-slate-950 dark:text-white'
                }`}
              >
                {enlace.texto}
              </a>
            ))}
          </nav>

          <div className="flex items-center gap-2">
            <button
              type="button"
              onClick={alternarTema}
              className="rounded-lg p-2 text-slate-500 transition hover:bg-slate-100 hover:text-slate-900 dark:text-slate-300 dark:hover:bg-white/5 dark:hover:text-white"
              aria-label={esOscuro ? 'Cambiar a tema claro' : 'Cambiar a tema oscuro'}
              title={esOscuro ? 'Tema claro' : 'Tema oscuro'}
            >
              <Icone nombre={esOscuro ? 'sol' : 'luna'} />
            </button>
            <Link
              to={destinoPrincipal}
              className="inline-flex h-10 items-center gap-2 rounded-full border border-blue-800/50 px-4 text-sm font-semibold text-blue-800 transition hover:border-blue-8000 hover:bg-blue-800 hover:text-blue-800 dark:text-cyan-300 dark:hover:border-cyan-300 dark:hover:bg-cyan-400/10 dark:hover:text-cyan-200 dark:border-cyan-400/50 dark:text-cyan-600 dark:hover:border-cyan-500 dark:hover:bg-cyan-50 dark:hover:text-cyan-700"
            >
              <Icone nombre="usuarios" className="h-4 w-4" />
              <span className="hidden sm:inline">{textoPrincipal}</span>
              <span className="sm:hidden">Ingresar</span>
            </Link>
          </div>
        </div>
      </header>

      <main id="contenido-principal">
        <section id="inicio" className="relative scroll-mt-24 overflow-hidden bg-white dark:bg-transparent">
          <div
            className="pointer-events-none absolute inset-0"
            style={{
              backgroundImage:
                'radial-gradient(circle at 15% 10%, rgba(34, 211, 238, 0.16), transparent 34%), radial-gradient(circle at 88% 18%, rgba(37, 99, 235, 0.24), transparent 38%), radial-gradient(circle at 50% 100%, rgba(14, 165, 233, 0.12), transparent 42%)'
            }}
          />
          <div
            className="pointer-events-none absolute inset-0 opacity-[0.035]"
            style={{
              backgroundImage:
                'linear-gradient(rgba(103, 232, 249, 0.8) 1px, transparent 1px), linear-gradient(90deg, rgba(103, 232, 249, 0.8) 1px, transparent 1px)',
              backgroundSize: '64px 64px'
            }}
          />

          <div className="relative mx-auto grid max-w-7xl gap-14 px-4 pb-20 pt-16 sm:px-6 lg:grid-cols-[1.02fr_0.98fr] lg:items-center lg:gap-10 lg:px-8 lg:pb-28 lg:pt-24">
            <div>
              <p className="flex items-center gap-3 text-xs font-semibold uppercase tracking-[0.32em] text-slate-600 dark:text-slate-300">
                Sistema de gestión
                <span className="h-px w-14 bg-blue-800/60 dark:bg-cyan-400/60" />
              </p>

              <h1 className="mt-6 text-5xl font-black leading-[0.9] tracking-[-0.045em] text-slate-950 dark:text-white sm:text-6xl lg:text-7xl">
                Innova
                <span className="text-blue-800 dark:bg-gradient-to-r dark:from-cyan-300 dark:to-cyan-500 dark:bg-clip-text dark:text-transparent">TECH</span>
                <span className="mt-2 block text-xl font-bold uppercase tracking-[0.42em] text-slate-400 dark:text-slate-500 dark:text-slate-400 sm:text-2xl">
                  Solutions
                </span>
              </h1>

              <p className="mt-6 max-w-xl text-base leading-relaxed text-slate-600 dark:text-slate-300 sm:text-lg">
                Gestiona tu inventario y controla el préstamo de equipos tecnológicos de manera eficiente, segura y sencilla.
              </p>

              <div className="mt-8 flex flex-col gap-3 sm:flex-row">
                <Link
                  to={destinoPrincipal}
                  className="inline-flex h-12 items-center justify-center gap-2 rounded-full bg-blue-800 px-6 text-base font-bold text-slate-950 shadow-lg shadow-blue-8000/20 transition hover:bg-blue-800 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-800 focus-visible:ring-offset-2 focus-visible:ring-offset-[#020b18] dark:bg-cyan-400 dark:shadow-cyan-500/20 dark:hover:bg-cyan-300 dark:focus-visible:ring-cyan-300"
                >
                  <Icone nombre="cohete" className="h-4 w-4" />
                  Comenzar ahora
                </Link>
                <a
                  href="#modulos"
                  className="inline-flex h-12 items-center justify-center gap-2 rounded-full border border-blue-800/35 px-6 text-base font-semibold text-blue-800 dark:text-cyan-200 transition hover:border-blue-800 hover:bg-blue-800/10 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-800 focus-visible:ring-offset-2 focus-visible:ring-offset-[#020b18] dark:border-cyan-400/35 dark:text-cyan-700 dark:hover:border-cyan-300 dark:hover:bg-cyan-400/10 dark:focus-visible:ring-cyan-300"
                >
                  <Icone nombre="informacion" className="h-4 w-4" />
                  Más información
                </a>
              </div>

              <div className="mt-8 flex flex-wrap items-center gap-x-5 gap-y-2 text-xs text-slate-400 dark:text-slate-500 dark:text-slate-400">
                <span className="inline-flex items-center gap-2">
                  <span className="h-1.5 w-1.5 rounded-full bg-emerald-400" />
                  Datos de demostración incluidos
                </span>
                <span>TechAsset · Plataforma de gestión de activos TI</span>
              </div>
            </div>

            <VisualHero />
          </div>
        </section>

        <section id="indicadores" className="scroll-mt-24 border-y border-slate-200 dark:border-white/10 bg-slate-100 dark:bg-white/[0.03]">
          <div className="mx-auto grid max-w-7xl grid-cols-2 gap-px bg-slate-200 dark:bg-white/10 sm:grid-cols-4">
            {ESTADISTICAS.map((estadistica) => (
              <div key={estadistica.etiqueta} className="flex flex-col items-center bg-white dark:bg-[#020b18] px-4 py-7 text-center shadow-sm dark:shadow-none">
                <span className="flex h-9 w-9 items-center justify-center rounded-xl border border-blue-800/20 bg-blue-800/10 text-blue-800 dark:text-cyan-300 dark:border-cyan-400/20 dark:bg-cyan-400/10 dark:text-cyan-600">
                  <Icone nombre={estadistica.icono} className="h-4 w-4" />
                </span>
                <p className="mt-3 text-3xl font-extrabold tracking-tight text-slate-950 dark:text-white">{estadistica.valor}</p>
                <p className="mt-1 text-xs font-medium text-slate-400 dark:text-slate-500 dark:text-slate-400">{estadistica.etiqueta}</p>
              </div>
            ))}
          </div>
        </section>

        <section id="modulos" className="mx-auto max-w-7xl scroll-mt-24 px-4 py-20 sm:px-6 lg:px-8 lg:py-24">
          <div className="max-w-2xl">
            <p className="text-xs font-semibold uppercase tracking-[0.3em] text-blue-800 dark:text-cyan-400 dark:text-cyan-600">Módulos</p>
            <h2 className="mt-3 text-3xl font-extrabold tracking-tight text-slate-950 dark:text-white sm:text-4xl">
              Un sistema, todo el ciclo del activo.
            </h2>
            <p className="mt-4 text-base leading-relaxed text-slate-400 dark:text-slate-500 dark:text-slate-400">
              Desde el alta del equipo hasta la devolución y la auditoría, cada paso queda registrado y disponible para consulta.
            </p>
          </div>

          <div className="mt-12 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {MODULOS.map((modulo) => (
              <article
                key={modulo.titulo}
                className="group rounded-2xl border border-slate-200 dark:border-white/10 bg-white dark:bg-white/[0.04] p-6 shadow-sm transition hover:-translate-y-0.5 hover:border-blue-800/40 hover:bg-slate-50 hover:shadow-md dark:hover:bg-cyan-400/[0.06] dark:shadow-none dark:hover:border-cyan-400/40 dark:hover:bg-cyan-50"
              >
                <span className="flex h-11 w-11 items-center justify-center rounded-xl border border-blue-800/20 bg-blue-800/10 text-blue-800 dark:text-cyan-300 transition group-hover:border-blue-800/40 group-hover:bg-blue-800/15 dark:border-cyan-400/20 dark:bg-cyan-400/10 dark:text-cyan-600 dark:group-hover:border-cyan-300/40 dark:group-hover:bg-cyan-400/15">
                  <Icone nombre={modulo.icono} className="h-5 w-5" />
                </span>
                <h3 className="mt-4 text-base font-bold text-slate-950 dark:text-white">{modulo.titulo}</h3>
                <p className="mt-2 text-sm leading-relaxed text-slate-400 dark:text-slate-500 dark:text-slate-400">{modulo.texto}</p>
              </article>
            ))}
          </div>
        </section>

        <section id="como-funciona" className="scroll-mt-24 border-y border-slate-200 dark:border-white/10 bg-slate-50 dark:bg-[#031226]">
          <div className="mx-auto grid max-w-7xl gap-12 px-4 py-20 sm:px-6 lg:grid-cols-2 lg:items-center lg:gap-16 lg:px-8 lg:py-24">
            <div>
              <p className="text-xs font-semibold uppercase tracking-[0.3em] text-blue-800 dark:text-cyan-400 dark:text-cyan-600">Cómo funciona</p>
              <h2 className="mt-3 text-3xl font-extrabold tracking-tight text-slate-950 dark:text-white sm:text-4xl">
                Del alta del equipo a la devolución, sin perder trazabilidad.
              </h2>
              <p className="mt-4 text-base leading-relaxed text-slate-400 dark:text-slate-500 dark:text-slate-400">
                El flujo está diseñado para que cada rol haga lo que le corresponde y el sistema aplique las reglas de negocio automáticamente.
              </p>

              <ol className="mt-8 space-y-5">
                {PASOS.map((paso, indice) => (
                  <li key={paso.titulo} className="flex gap-4">
                    <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-blue-800 text-sm font-bold text-slate-950 dark:bg-cyan-400">
                      {String(indice + 1).padStart(2, '0')}
                    </span>
                    <div>
                      <p className="font-semibold text-slate-950 dark:text-white">{paso.titulo}</p>
                      <p className="mt-1 text-sm leading-relaxed text-slate-400 dark:text-slate-500 dark:text-slate-400">{paso.texto}</p>
                    </div>
                  </li>
                ))}
              </ol>
            </div>

            <div className="space-y-5">
              <div className="relative overflow-hidden rounded-3xl border border-slate-200 dark:border-white/10 bg-white dark:bg-white/[0.04] p-3 shadow-sm dark:shadow-none">
                <div className="pointer-events-none absolute -right-10 -top-10 h-40 w-40 rounded-full bg-blue-8000/10 blur-2xl dark:bg-cyan-500/10" />
                <IlustracionFlujo className="relative w-full" />
              </div>

              <div className="rounded-3xl border border-slate-200 dark:border-white/10 bg-white dark:bg-white/[0.04] p-6 sm:p-7 shadow-sm dark:shadow-none">
                <div className="flex items-start gap-3">
                  <span className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl border border-emerald-400/20 bg-emerald-400/10 text-emerald-300">
                    <Icone nombre="verificar" className="h-5 w-5" />
                  </span>
                  <div>
                    <p className="font-bold text-slate-950 dark:text-white">Reglas de negocio activas</p>
                    <p className="mt-1 text-xs text-slate-400 dark:text-slate-500 dark:text-slate-400">Validaciones automáticas en cada operación</p>
                  </div>
                </div>

                <ul className="mt-5 space-y-2.5">
                  {REGLAS.map((regla) => (
                    <li key={regla} className="flex items-start gap-2.5 text-sm text-slate-600 dark:text-slate-300">
                      <Icone nombre="verificar" className="mt-0.5 h-4 w-4 shrink-0 text-emerald-400" />
                      {regla}
                    </li>
                  ))}
                </ul>

                <div className="mt-5 rounded-xl border border-blue-800/20 bg-blue-800/10 p-3.5 dark:border-cyan-400/20 dark:bg-cyan-400/10">
                  <p className="text-sm font-semibold text-blue-800 dark:text-cyan-200 dark:text-cyan-700">Trazabilidad completa</p>
                  <p className="mt-1 text-xs leading-relaxed text-blue-900 dark:text-cyan-100/80 dark:text-cyan-900/80">
                    Cada movimiento queda en la bitácora con usuario, fecha, equipo y detalle de la operación.
                  </p>
                </div>
              </div>
            </div>
          </div>
        </section>

        <section id="roles" className="mx-auto max-w-7xl scroll-mt-24 px-4 py-20 sm:px-6 lg:px-8 lg:py-24">
          <div className="max-w-2xl">
            <p className="text-xs font-semibold uppercase tracking-[0.3em] text-blue-800 dark:text-cyan-400 dark:text-cyan-600">Roles</p>
            <h2 className="mt-3 text-3xl font-extrabold tracking-tight text-slate-950 dark:text-white sm:text-4xl">
              Un acceso para cada responsabilidad.
            </h2>
            <p className="mt-4 text-base leading-relaxed text-slate-400 dark:text-slate-500 dark:text-slate-400">
              El sistema aplica permisos por rol y muestra solo las acciones que corresponden a cada usuario.
            </p>
          </div>

          <div className="mt-12 grid gap-4 lg:grid-cols-3">
            {ROLES.map((rol) => (
              <article key={rol.nombre} className="flex flex-col rounded-2xl border border-slate-200 dark:border-white/10 bg-white dark:bg-white/[0.04] p-6 shadow-sm dark:shadow-none">
                <div className="flex items-start justify-between gap-3">
                  <span className="flex h-11 w-11 items-center justify-center rounded-xl border border-blue-800/20 bg-blue-800/10 text-blue-800 dark:text-cyan-300 dark:border-cyan-400/20 dark:bg-cyan-400/10 dark:text-cyan-600">
                    <Icone nombre={rol.icono} className="h-5 w-5" />
                  </span>
                  <Insignia tono="marca">Demo</Insignia>
                </div>

                <h3 className="mt-4 text-lg font-bold text-slate-950 dark:text-white">{rol.nombre}</h3>
                <p className="mt-2 text-sm leading-relaxed text-slate-400 dark:text-slate-500 dark:text-slate-400">{rol.descripcion}</p>

                <ul className="mt-4 space-y-2 text-sm text-slate-600 dark:text-slate-300">
                  {rol.permisos.map((permiso) => (
                    <li key={permiso} className="flex items-start gap-2.5">
                      <Icone nombre="verificar" className="mt-0.5 h-4 w-4 shrink-0 text-emerald-400" />
                      {permiso}
                    </li>
                  ))}
                </ul>

                <div className="mt-auto pt-5">
                  <div className="rounded-xl border border-slate-200 dark:border-white/10 bg-white/60 dark:bg-[#020b18]/60 p-3">
                    <p className="text-[10px] font-semibold uppercase tracking-wider text-slate-400 dark:text-slate-500">Cuenta demo</p>
                    <p className="mt-1 break-all font-mono text-[11px] text-slate-800 dark:text-slate-200">{rol.cuenta}</p>
                    <p className="font-mono text-[11px] text-slate-400 dark:text-slate-500 dark:text-slate-400">{rol.clave}</p>
                  </div>
                </div>
              </article>
            ))}
          </div>

          <p className="mt-6 text-center text-sm text-slate-400 dark:text-slate-500 dark:text-slate-400">
            Las tres cuentas están disponibles en la pantalla de acceso para probar el sistema.
          </p>
        </section>

        <section className="mx-auto max-w-7xl px-4 pb-20 sm:px-6 lg:px-8 lg:pb-24">
          <div className="relative overflow-hidden rounded-[2rem] border border-blue-800 bg-gradient-to-br from-blue-800 via-white to-blue-800 px-6 py-14 text-center shadow-sm sm:px-12 sm:py-16 dark:border-cyan-400/20 dark:bg-gradient-to-br dark:from-[#062a54] dark:via-[#031a36] dark:to-[#020b18] dark:shadow-2xl dark:shadow-cyan-950/40">
            <div className="pointer-events-none absolute -right-16 -top-16 h-64 w-64 rounded-full bg-blue-800/20 blur-3xl dark:bg-cyan-400/20" />
            <div className="pointer-events-none absolute -bottom-24 -left-10 h-64 w-64 rounded-full bg-blue-8000/20 blur-3xl dark:bg-cyan-500/20" />

            <div className="relative">
              <p className="text-xs font-semibold uppercase tracking-[0.3em] text-blue-800 dark:text-cyan-300">Empiece hoy</p>
              <h2 className="mx-auto mt-3 max-w-2xl text-3xl font-extrabold tracking-tight text-slate-950 sm:text-4xl dark:text-white">
                Tome el control de sus activos tecnológicos.
              </h2>
              <p className="mx-auto mt-3 max-w-2xl text-slate-600 dark:text-slate-300">
                Ingrese con una cuenta de demostración y recorra el panel, el inventario, los préstamos y los reportes.
              </p>

              <div className="mt-8 flex flex-col justify-center gap-3 sm:flex-row">
                <Link
                  to={destinoPrincipal}
                  className="inline-flex h-12 items-center justify-center gap-2 rounded-full bg-blue-800 px-6 text-base font-bold text-white shadow-lg shadow-blue-800/20 transition hover:bg-blue-800 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-8000 focus-visible:ring-offset-2 focus-visible:ring-offset-white dark:bg-cyan-400 dark:text-slate-950 dark:shadow-cyan-500/20 dark:hover:bg-cyan-300 dark:focus-visible:ring-cyan-300 dark:focus-visible:ring-offset-[#031a36]"
                >
                  <Icone nombre="panel" className="h-4 w-4" />
                  {autenticado ? 'Ir al panel general' : 'Iniciar sesión'}
                </Link>
                <a
                  href="#modulos"
                  className="inline-flex h-12 items-center justify-center gap-2 rounded-full border border-blue-800 px-6 text-base font-semibold text-blue-800 transition hover:bg-blue-800 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-8000 focus-visible:ring-offset-2 focus-visible:ring-offset-white dark:border-cyan-300/30 dark:text-cyan-200 dark:hover:bg-cyan-400/10 dark:focus-visible:ring-cyan-300 dark:focus-visible:ring-offset-[#031a36]"
                >
                  Explorar módulos
                  <Icone nombre="abajo" className="h-4 w-4" />
                </a>
              </div>
            </div>
          </div>
        </section>
      </main>

      <footer className="border-t border-slate-200 dark:border-white/10 bg-slate-100 dark:bg-[#01060f]">
        <div className="mx-auto grid max-w-7xl gap-8 px-4 py-10 sm:px-6 lg:grid-cols-[1.5fr_1fr_1fr] lg:px-8">
          <div>
            <MarcaInnovatech claro={esOscuro} />
            <p className="mt-4 max-w-sm text-sm leading-relaxed text-slate-400 dark:text-slate-500 dark:text-slate-400">
              Sistema de gestión de inventario y préstamo de equipos tecnológicos de Innovatech Solutions S.A.C.
            </p>
          </div>

          <div>
            <p className="text-sm font-semibold text-slate-950 dark:text-white">Navegación</p>
            <ul className="mt-3 space-y-2 text-sm text-slate-400 dark:text-slate-500 dark:text-slate-400">
              {NAVEGACION.map((enlace) => (
                <li key={enlace.texto}>
                  <a href={enlace.href} className="transition hover:text-blue-800 dark:text-cyan-300 dark:hover:text-cyan-600">
                    {enlace.texto}
                  </a>
                </li>
              ))}
              <li>
                <Link to="/login" className="transition hover:text-blue-800 dark:text-cyan-300 dark:hover:text-cyan-600">Iniciar sesión</Link>
              </li>
            </ul>
          </div>

          <div>
            <p className="text-sm font-semibold text-slate-950 dark:text-white">Tecnología</p>
            <p className="mt-3 text-sm text-slate-400 dark:text-slate-500 dark:text-slate-400">ASP.NET Core 8 · React · PostgreSQL</p>
            <p className="mt-1 text-xs text-slate-400 dark:text-slate-500">Caso Práctico Integrador · Construcción de Software</p>
          </div>
        </div>

        <div className="border-t border-slate-200 dark:border-white/10">
          <div className="mx-auto flex max-w-7xl flex-col gap-2 px-4 py-5 text-xs text-slate-400 dark:text-slate-500 sm:flex-row sm:items-center sm:justify-between sm:px-6 lg:px-8">
            <p>© 2026 Innovatech Solutions S.A.C. Todos los derechos reservados.</p>
            <p>Hecho para la Gerencia de Tecnología.</p>
          </div>
        </div>
      </footer>
    </div>
  );
}
