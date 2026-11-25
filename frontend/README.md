# TechAsset · Frontend

Aplicación web del caso práctico TechAsset (Innovatech Solutions S.A.C.), construida con
**React 18 + Vite + Tailwind CSS**.

## Puesta en marcha

```bash
npm install
npm run dev        # http://localhost:5173
```

El servidor de desarrollo redirige `/api` hacia el backend (por defecto
`http://localhost:5080`, configurable con `VITE_API_PROXY` en `.env`). Para producción:

```bash
npm run build      # genera dist/
npm run preview
```

Defina `VITE_API_URL` con la URL pública de la API si el frontend se sirve en otro dominio
(vea `.env.example`).

## ⚠️ Versión de Tailwind CSS

Este proyecto usa **Tailwind CSS 3.4.19** (configuración en `tailwind.config.js` y directivas
`@tailwind` en `src/index.css`). La versión 4 de Tailwind movió el plugin de PostCSS al paquete
`@tailwindcss/postcss` y dejó de admitir esas directivas, por lo que produce el error:

```
[plugin:vite:css] [postcss] It looks like you're trying to use `tailwindcss` directly as a
PostCSS plugin. The PostCSS plugin has moved to a separate package…
```

**Corrección** (en la carpeta `frontend`):

```bash
npm install -D tailwindcss@3.4.19
# si la instalación anterior dejó el árbol inconsistente:
rm -rf node_modules package-lock.json && npm install
```

El script `scripts/verificar-tailwind.mjs` se ejecuta automáticamente antes de `npm run dev` y
`npm run build` (`predev` / `prebuild`) y detiene el proceso con este mismo mensaje si detecta una
versión incompatible, para que el error aparezca con la solución en pantalla.

## Estructura

```
src/
├── api/                cliente HTTP (Axios con interceptores) y servicios por módulo
├── componentes/
│   ├── ui/             kit de componentes reutilizables (botón, campo, modal, tabla, toasts…)
│   └── layout/         shell de la aplicación (barra lateral, barra superior, encabezados)
├── contexto/           sesión (AuthContexto) y notificaciones (ToastContexto)
├── ganchos/            useFormulario · usePeticion · usePaginacion · useDebounce
├── paginas/            Ingreso · Panel · Inventario · Préstamos · Usuarios · Reportes · Auditoría
└── utilidades/         formateadores, validaciones y catálogos compartidos
```

## Cuentas de demostración

| Rol | Correo | Contraseña |
|---|---|---|
| Administrador | `elena.quispe@techasset.pe` | `Admin1234` |
| Responsable de Tecnología | `rodrigo.vasquez@techasset.pe` | `Responsable1234` |
| Colaborador | `andrea.flores@techasset.pe` | `Colaborador1234` |

La documentación completa (arquitectura, reglas de negocio, ISO, Nielsen y Norman) está en
[`../docs/INFORME_TECNICO.md`](../docs/INFORME_TECNICO.md).
