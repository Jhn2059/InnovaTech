# TechAsset · Sistema de Gestión de Inventario y Préstamo de Equipos Tecnológicos

**Innovatech Solutions S.A.C. — Gerencia de Tecnología**

Solución web integral para administrar el inventario tecnológico institucional y controlar el
préstamo de equipos a los colaboradores, developed en el marco del *Caso Práctico Integrador*
de la asignatura **Construcción de Software**.

---

## 1. Stack tecnológico

| Capa | Tecnología | Versión |
|------|-----------|---------|
| Backend | ASP.NET Core (Web API) | 8.0 |
| Backend | Entity Framework Core | 8.0 |
| Backend | PostgreSQL | 15+ |
| Backend | FluentValidation · BCrypt · Serilog · Swashbuckle (Swagger) | — |
| Frontend | React + Vite | 18 / 8 |
| Frontend | Tailwind CSS · React Router · Recharts · Axios | 3 / 6 / 2 |
| Pruebas | xUnit · FluentAssertions · WebApplicationFactory | — |

## 2. Arquitectura del backend (Clean Architecture por capas)

```
TechAsset/
├── backend/
│   ├── src/
│   │   ├── TechAsset.Domain/          ◄ Núcleo del negocio (sin dependencias)
│   │   │   ├── Entities/               Usuario, Equipo, Prestamo, Movimiento, EntidadBase
│   │   │   ├── Enums/                  RolUsuario, EstadoEquipo, EstadoPrestamo, TipoMovimiento
│   │   │   ├── ValueObjects/           Email, CodigoPatrimonial (inmutables, con invariantes)
│   │   │   ├── Exceptions/             Excepciones de dominio con código funcional (RN-xx)
│   │   │   └── Rules/                  ReglasNegocio: catálogo ejecutable RN-01 … RN-10
│   │   │
│   │   ├── TechAsset.Application/     ◄ Casos de uso (depende solo de Domain)
│   │   │   ├── Common/                 Result, PagedResult, abstracciones, mapeos, extensiones
│   │   │   ├── Dtos/                   Proyecciones de lectura (Usuario, Equipo, Prestamo, Dashboard)
│   │   │   └── Modulos/                Usuarios · Equipos · Préstamos · Reportes · Auth
│   │   │
│   │   ├── TechAsset.Infrastructure/  ◄ Persistencia y servicios técnicos (depende de Application)
│   │   │   ├── Persistence/            AppDbContext, configuraciones, repositorios, migraciones, semilla
│   │   │   └── Servicios/              BCrypt, JWT (IOptions), reloj del sistema
│   │   │
│   │   └── TechAsset.API/              ◄ Presentación (depende de Application + Infrastructure)
│   │       ├── Controllers/            Auth, Usuarios, Equipos, Préstamos, Reportes
│   │       ├── Middleware/             Manejador global de excepciones (ProblemDetails)
│   │       ├── Filtros/                Validación automática con FluentValidation
│   │       └── Seguridad/              Políticas por rol, contexto de usuario, configuración JWT
│   └── tests/
│       ├── TechAsset.UnitTests/        44 pruebas del dominio y los validadores
│       └── TechAsset.IntegrationTests/ 19 pruebas HTTP + PostgreSQL real
└── frontend/
    └── src/
        ├── api/                        Cliente HTTP (Axios) y servicios por módulo
        ├── componentes/ui/             Kit de componentes reutilizables
        ├── componentes/layout/         Shell, barra lateral, barra superior
        ├── contexto/                   Sesión (Auth) y notificaciones (Toast)
        ├── ganchos/                    useFormulario, usePeticion, usePaginacion, useDebounce
        ├── paginas/                    Ingreso, Panel, Inventario, Préstamos, Usuarios, Reportes, Auditoría
        └── utilidades/                 Formateadores, validaciones y catálogos compartidos
```

**Regla de dependencia:** `API → Infrastructure → Application → Domain`. El dominio no conoce
EF Core, HTTP ni PostgreSQL: se puede probar sin base de datos (44 pruebas unitarias lo demuestran).

## 3. Puesta en marcha

### 3.1 Requisitos

- .NET SDK 8.0
- Node.js 18 o superior
- PostgreSQL 14 o superior

### 3.2 Base de datos

```bash
sudo -u postgres psql -c "CREATE USER techasset WITH PASSWORD 'techasset' SUPERUSER;"
sudo -u postgres psql -c "CREATE DATABASE techasset OWNER techasset;"
sudo -u postgres psql -c "CREATE DATABASE techasset_test OWNER techasset;"
```

La cadena de conexión por defecto está en `backend/src/TechAsset.API/appsettings.json`
(`Host=localhost;Port=5432;Database=techasset;Username=techasset;Password=techasset`).

### 3.3 Backend

```bash
cd backend
dotnet restore
dotnet ef database update --project src/TechAsset.Infrastructure --startup-project src/TechAsset.API
dotnet run --project src/TechAsset.API --urls "http://localhost:5080"
```

- API: <http://localhost:5080>
- Swagger: <http://localhost:5080/swagger>
- Salud: <http://localhost:5080/api/health>

Al arrancar, la aplicación aplica las migraciones y, si la base está vacía y `SembrarDatos` es
`true`, carga el conjunto de datos de demostración (25 usuarios, 64 equipos, 52 préstamos y la
bitácora de movimientos).

### 3.4 Frontend

```bash
cd frontend
npm install
npm run dev
```

> **Importante — versión de Tailwind CSS.** TechAsset está configurado para **Tailwind CSS 3.4.19**
> (`tailwind.config.js` + directivas `@tailwind`), y la versión está fijada exactamente en
> `package.json` para que `npm install` no resuelva nunca la v4. Si instalaste Tailwind "a la
> última" (`npm install -D tailwindcss`), verás el error:
> *«It looks like you're trying to use `tailwindcss` directly as a PostCSS plugin…»*.
> Solución: `npm install -D tailwindcss@3.4.19` (y si persiste, `rm -rf node_modules package-lock.json && npm install`).
> El script `frontend/scripts/verificar-tailwind.mjs` se ejecuta antes de `dev` y `build`, de modo que
> detecta la versión incorrecta y muestra esta misma instrucción antes de que PostCSS falle.

- Aplicación: <http://localhost:5173>

El servidor de desarrollo de Vite redirige `/api` hacia `http://localhost:5080` (proxy definido en
`vite.config.js`), por lo que no hay problemas de CORS en desarrollo. Para producción, defina
`VITE_API_URL` con la URL pública de la API.

### 3.5 Cuentas de demostración

| Rol | Correo | Contraseña | Permisos |
|-----|--------|------------|----------|
| Administrador | `elena.quispe@techasset.pe` | `Admin1234` | Usuarios, equipos, préstamos, reportes y auditoría |
| Responsable de Tecnología | `rodrigo.vasquez@techasset.pe` | `Responsable1234` | Equipos, préstamos, devoluciones y auditoría |
| Colaborador | `andrea.flores@techasset.pe` | `Colaborador1234` | Inventario, préstamos propios y dashboard |

> Las 25 cuentas generadas por la semilla siguen el patrón `<nombre>.<apellido>@techasset.pe` con
> contraseña `Colaborador1234` (las tres cuentas destacadas usan su propia clave).

## 3.b Ejecución desde Visual Studio Code

El repositorio incluye la carpeta `.vscode` con **tareas y configuraciones de depuración ya
preparadas** (sin configuración manual).

### 3.b.1 Preparación (una sola vez)

1. **Abrir la carpeta raíz** del proyecto: `File ▸ Open Folder… ▸ TechAsset` (no `backend` ni
   `frontend` por separado: así los dos servidores se depuran en la misma ventana).
2. VS Code detectará el archivo `.vscode/extensions.json` y propondrá instalar las extensiones
   recomendadas: pulse **Install All** (C# Dev Kit, C# Extension, MSSQL, Tailwind CSS IntelliSense,
   Debugger for Chrome, ESLint, Prettier).
3. **Base de datos** (Linux/macOS):
   - Con Docker o PostgreSQL ya instalado, cree el rol y las bases:

     ```bash
     sudo -u postgres psql -c "CREATE USER techasset WITH PASSWORD 'techasset' SUPERUSER;"
     sudo -u postgres psql -c "CREATE DATABASE techasset OWNER techasset;"
     sudo -u postgres psql -c "CREATE DATABASE techasset_test OWNER techasset;"
     ```

   - O ejecute la tarea **`PostgreSQL · iniciar servicio`** (Tasks ▸ Run Task) en Linux.
4. **Instalar dependencias** (Terminal integrada `Ctrl+` `):
   ```bash
   bash scripts/setup.sh
   ```
   El script restaura y compila el backend, aplica las migraciones de EF Core y ejecuta
   `npm install` en el frontend.

### 3.b.2 Ejecutar el sistema completo con F5

1. Pulse **F5** (o *Run ▸ Start Debugging*).
2. En la lista de configuración elija **`▶ TechAsset completo (API + Frontend)`**.
3. VS Code compila la API, aplica las migraciones, levanta:
   - la API en `http://localhost:5080` (Swagger en `/swagger`), con el depurador de .NET attached, y
   - el frontend con Vite en `http://localhost:5173`, abriendo Chrome con el depurador de JavaScript.
4. Inicie sesión con `elena.quispe@techasset.pe` / `Admin1234`.

> El compound `▶ TechAsset completo` detiene ambos procesos al terminar la depuración.

### 3.b.3 Configuraciones y tareas disponibles

| Configuración de depuración (F5) | Qué hace |
|---|---|
| `▶ TechAsset completo (API + Frontend)` | Comp compound: API + frontend + Chrome, con depuradores activos |
| `API · ASP.NET Core (depurar)` | Solo la API, con puntos de depuración y Swagger abierto |
| `API · ejecutar sin depurador` | Solo la API, más rápida |
| `Frontend · React (depurar en Chrome)` | Solo el frontend, con *source maps* y breakpoints en React |

| Tarea (Tasks ▸ Run Task… o `Ctrl+Shift+B`) | Qué hace |
|---|---|
| `4 · Preparar base de datos` | Restaura, aplica migraciones y carga la semilla |
| `3 · Aplicar migraciones (dotnet ef database update)` | Sincroniza el esquema con PostgreSQL |
| `5 · Crear migración (dotnet ef migrations add)` | Pide el nombre y genera la migración |
| `▶ Ejecutar API (dotnet run)` | API en segundo plano, con panel dedicado |
| `▶ Ejecutar frontend (npm run dev)` | Vite en segundo plano, con panel dedicado |
| `▶ Ejecutar pruebas (dotnet test)` | Ejecuta las 63 pruebas (unitarias + integración) |
| `PostgreSQL · iniciar/detener servicio` | Control del servidor de base de datos (Linux) |

Atajos útiles: `Ctrl+Shift+B` compila el backend · `Ctrl+Shift+P ▸ Tasks: Run Task` abre las
tareas · `F5` depura · `Shift+F5` detiene.

### 3.b.4 Depuración de la aplicación

- **Backend:** ponga un *breakpoint* en `PrestamoServicio.RegistrarAsync` o en cualquier
  controlador y pulse F5; el panel *Variables* muestra el `Result`, los DTOs y las entidades.
- **Frontend:** en `src/paginas/Inventario.jsx` puede depurar la carga de datos, los filtros y el
  envío del formulario; el mapa de fuentes ya está configurado (`isAnimationActive` deshabilitado
  en los gráficos para que los breakpoints no se pierdan durante la animación).
- **SQL:** con la extensión *MSSQL* puede conectarse a `techasset` y consultar
  `"Prestamos"`, `"Equipos"` y `"Movimientos"` mientras la aplicación está en ejecución.

### 3.b.5 Problemas frecuentes en VS Code

| Síntoma | Causa | Solución |
|---|---|---|
| *«No se encuentra la base techasset»* | La base no existe o el servicio está detenido | Tarea *PostgreSQL · iniciar servicio* y las sentencias `CREATE DATABASE` del apartado 3.b.1 |
| *«dotnet: no se reconoce como comando»* | Falta el SDK de .NET | Instalar el .NET SDK 8.0 y reiniciar VS Code |
| El frontend carga pero da *Failed to fetch* | La API no está levantada en el puerto 5080 | Ejecutar `▶ Ejecutar API (dotnet run)` o el compound F5 |
| `401 Unauthorized` al iniciar sesión | La clave JWT de `appsettings.json` fue modificada | Restaurar la clave de 32+ caracteres o alinear `Jwt:ClaveSecreta` con la del emisor |
| *«dotnet-ef no se encuentra»* | Falta la herramienta de migraciones | Tarea `0 · Instalar EF Core (dotnet-ef)` |
| El puerto 5080 está ocupado | Otra instancia de la API | Cambiar `--urls` en la tarea o detener el proceso anterior |


## 4. Módulos yEndpoints

| Módulo | Requerimientos | Endpoints principales |
|--------|----------------|-----------------------|
| Usuarios | RF-01 … RF-04 | `GET/POST /api/usuarios`, `PUT/PATCH /api/usuarios/{id}` |
| Equipos | RF-05 … RF-08 | `GET/POST /api/equipos`, `PUT/PATCH /api/equipos/{id}`, `DELETE /api/equipos/{id}` |
| Préstamos | RF-09 … RF-12 | `GET /api/prestamos`, `GET /api/prestamos/activos`, `POST /api/prestamos`, `POST /api/prestamos/{id}/devolucion` |
| Reportes | RF-13 … RF-17 | `GET /api/reportes/dashboard`, `/equipos-por-estado`, `/equipos-mas-utilizados`, `/prestamos-por-area`, `/movimientos` |
| Autenticación | — | `POST /api/auth/login`, `GET /api/auth/perfil` |

Todas las respuestas de error siguen **RFC 9457 (ProblemDetails)** e incluyen el código funcional
(`RN-01`, `RN-07`, `VALIDACION`…) y el `traceId` de la petición.

## 5. Pruebas

```bash
cd backend
dotnet test                                   # 63 pruebas (44 unitarias + 19 de integración)
dotnet test tests/TechAsset.UnitTests
dotnet test tests/TechAsset.IntegrationTests  # requiere PostgreSQL con la base techasset_test
```

- **Unitarias:** las 10 reglas de negocio, el comportamiento de las entidades, los objetos valor y
  los validadores de entrada.
- **De integración:** los 7 escenarios mínimos del caso práctico y la verificación de cada RN sobre
  la API real (código HTTP esperado, código de negocio y estado final de los datos).

## 6. Documentación

- [`docs/INFORME_TECNICO.md`](docs/INFORME_TECNICO.md) — informe técnico completo: análisis,
  trazabilidad RF/RN, arquitectura, estándares, ISO, Nielsen/Norman, evidencias y reflexión.
- [`docs/MANUAL_USUARIO.md`](docs/MANUAL_USUARIO.md) — guía de uso por rol.
- `evidencias/` — capturas de la ejecución real del sistema (panel, inventario, préstamos,
  usuarios, reportes, bitácora, tema oscuro y vista móvil).

## 7. Licencia y autoría

Material académico developed para el Caso Práctico Integrador de Construcción de Software.
