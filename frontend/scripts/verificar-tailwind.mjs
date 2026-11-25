/**
 * Verificador de la versión de Tailwind CSS.
 *
 * TechAsset usa Tailwind CSS 3.x (tailwind.config.js + directives @tailwind).
 * La versión 4 movió el plugin de PostCSS a un paquete aparte (@tailwindcss/postcss)
 * y dejó de admitir @tailwind, por lo que el error que se vería es:
 *
 *   [postcss] It looks like you're trying to use `tailwindcss` directly as a
 *   PostCSS plugin. The PostCSS plugin has moved to a separate package…
 *
 * Este guardián detecta esa situación ANTES de iniciar Vite o compilar y
 * muestra la orden exacta que la corrige, en lugar de dejar un error de PostCSS.
 */
import { createRequire } from 'node:module';

const require = createRequire(import.meta.url);
const silencioso = process.argv.includes('--silencioso');

let version = null;
try {
  version = require('tailwindcss/package.json').version;
} catch {
  version = null;
}

const mayor = version ? Number.parseInt(version.split('.')[0], 10) : null;

if (mayor === 3) {
  if (!silencioso) {
    console.log(`\n  ✔ Tailwind CSS ${version} detectado (configuración v3 compatible).\n`);
  }
  process.exit(0);
}

if (mayor === 4) {
  if (silencioso) {
    // Durante `npm install` no se bloquea la instalación: solo se avisa.
    console.warn(
      '\n  ⚠ Tailwind CSS 4 detectado. TechAsset requiere 3.4.19 (npm install -D tailwindcss@3.4.19).\n'
    );
    process.exit(0);
  }

  console.error(
    [
      '',
      '  ✖ Se detectó Tailwind CSS 4 (' + version + '), pero TechAsset está configurado para Tailwind 3.',
      '',
      '    En la versión 4 el plugin de PostCSS se movió al paquete "@tailwindcss/postcss"',
      '    y desaparecieron las directivas "@tailwind"; por eso PostCSS se detiene con:',
      '    "It looks like you\'re trying to use `tailwindcss` directly as a PostCSS plugin".',
      '',
      '    Solución (ejecuta en la carpeta frontend):',
      '      npm install -D tailwindcss@3.4.19',
      '      rm -rf node_modules package-lock.json',
      '      npm install',
      '',
      '    Alternativa: si prefieres Tailwind 4, hay que migrar tailwind.config.js a @theme',
      '    en CSS y cambiar postcss.config.js a { plugins: { "@tailwindcss/postcss": {} } }.',
      ''
    ].join('\n')
  );
  process.exit(1);
}

if (silencioso) {
  console.warn('\n  ⚠ No se encontró "tailwindcss"; ejecuta npm install -D tailwindcss@3.4.19\n');
  process.exit(0);
}

if (!silencioso) {
  console.error(
    [
      '',
      '  ✖ No se pudo localizar el paquete "tailwindcss".',
      '',
      '    Ejecuta en la carpeta frontend:',
      '      npm install -D tailwindcss@3.4.19 postcss autoprefixer',
      ''
    ].join('\n')
  );
}
process.exit(1);
