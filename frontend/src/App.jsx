import { Navigate, Route, Routes, useLocation } from 'react-router-dom';
import { Layout } from './componentes/layout/Layout';
import { Cargador } from './componentes/ui/Estados';
import { useAuth } from './contexto/AuthContexto';
import { Panel } from './paginas/Panel';
import { Inventario } from './paginas/Inventario';
import { Prestamos } from './paginas/Prestamos';
import { Usuarios } from './paginas/Usuarios';
import { Reportes } from './paginas/Reportes';
import { Auditoria } from './paginas/Auditoria';
import { Ingreso } from './paginas/Ingreso';
import { Landing } from './paginas/Landing';
import { NoEncontrado } from './paginas/NoEncontrado';

/** Ruta que exige sesión activa. */
function RutaProtegida({ children }) {
  const { autenticado, cargando } = useAuth();
  const ubicacion = useLocation();

  if (cargando) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-surface-2">
        <Cargador texto="Preparando el sistema…" />
      </div>
    );
  }

  if (!autenticado) {
    return <Navigate to="/login" state={{ desde: ubicacion.pathname }} replace />;
  }

  return children;
}

/** Ruta restringida a determinados roles. */
function RutaPorRol({ roles, children }) {
  const { usuario } = useAuth();
  if (!roles.includes(Number(usuario?.rol))) {
    return <NoEncontrado />;
  }
  return children;
}

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Landing />} />
      <Route path="/login" element={<Ingreso />} />

      <Route
        element={
          <RutaProtegida>
            <Layout />
          </RutaProtegida>
        }
      >
        <Route path="/panel" element={<Panel />} />
        <Route path="/equipos" element={<Inventario />} />
        <Route path="/prestamos" element={<Prestamos />} />
        <Route path="/reportes" element={<Reportes />} />
        <Route
          path="/usuarios"
          element={
            <RutaPorRol roles={[1, 2]}>
              <Usuarios />
            </RutaPorRol>
          }
        />
        <Route
          path="/auditoria"
          element={
            <RutaPorRol roles={[1, 2]}>
              <Auditoria />
            </RutaPorRol>
          }
        />
      </Route>

      <Route path="*" element={<NoEncontrado />} />
    </Routes>
  );
}
