export function IlustracionFlujo({ className = '' }) {
  return (
    <svg
      viewBox="0 0 640 360"
      role="img"
      aria-label="Flujo ilustrado de alta, préstamo y devolución de equipos tecnológicos"
      className={className}
    >
      <defs>
        <pattern id="flujo-puntos" width="20" height="20" patternUnits="userSpaceOnUse">
          <circle cx="1.5" cy="1.5" r="1.2" fill="rgb(var(--line-strong))" opacity="0.6" />
        </pattern>
        <filter id="flujo-sombra" x="-30%" y="-30%" width="160%" height="160%">
          <feDropShadow dx="0" dy="10" stdDeviation="10" floodColor="#0f172a" floodOpacity="0.12" />
        </filter>
        <marker
          id="flujo-flecha"
          viewBox="0 0 10 10"
          refX="8"
          refY="5"
          markerWidth="6"
          markerHeight="6"
          orient="auto-start-reverse"
        >
          <path d="M0 0 L10 5 L0 10 Z" fill="rgb(var(--chart-2))" />
        </marker>
      </defs>

      <rect width="640" height="360" fill="url(#flujo-puntos)" />
      <ellipse cx="320" cy="190" rx="282" ry="122" fill="none" stroke="rgb(var(--line-strong))" strokeDasharray="4 10" opacity="0.55" />

      <g stroke="rgb(var(--chart-2))" strokeWidth="2" strokeDasharray="6 8" fill="none">
        <path d="M212 198 C222 180 220 164 230 154" markerEnd="url(#flujo-flecha)" />
        <path d="M412 154 C422 164 420 180 430 190" markerEnd="url(#flujo-flecha)" />
      </g>

      <g transform="translate(30 112) rotate(-5 90 90)" filter="url(#flujo-sombra)">
        <rect width="180" height="180" rx="24" fill="rgb(var(--surface))" stroke="rgb(var(--line))" />
        <rect x="14" y="14" width="152" height="88" rx="16" fill="rgb(var(--chart-2))" opacity="0.08" />
        <g transform="translate(43 32)">
          <rect width="94" height="56" rx="6" fill="rgb(var(--chart-2))" opacity="0.16" stroke="rgb(var(--chart-2))" strokeWidth="2" />
          <rect x="8" y="8" width="78" height="40" rx="3" fill="rgb(var(--surface))" />
          <path d="M-8 58 H102 L94 68 H0 Z" fill="rgb(var(--chart-2))" opacity="0.35" />
          <circle cx="47" cy="63" r="2" fill="rgb(var(--chart-2))" />
        </g>
        <g transform="translate(16 112)">
          <rect width="86" height="26" rx="8" fill="rgb(var(--surface-2))" stroke="rgb(var(--line))" />
          <path d="M12 8 V18 M17 8 V18 M22 8 V18 M27 8 V18 M32 8 V18" stroke="rgb(var(--muted))" strokeWidth="2" strokeLinecap="round" />
          <text x="42" y="17" fontSize="8" fill="rgb(var(--muted))">TECH-001</text>
        </g>
        <g transform="translate(110 112)">
          <rect width="54" height="26" rx="13" fill="rgb(var(--chart-1))" opacity="0.14" />
          <text x="27" y="17" textAnchor="middle" fontSize="9" fontWeight="700" fill="rgb(var(--chart-1))">Alta</text>
        </g>
        <text x="16" y="158" fontSize="11" fontWeight="700" fill="rgb(var(--ink))">Inventario</text>
        <text x="16" y="171" fontSize="8" fill="rgb(var(--muted))">Código patrimonial y estado</text>
      </g>

      <g transform="translate(230 56)" filter="url(#flujo-sombra)">
        <rect width="180" height="200" rx="24" fill="rgb(var(--surface))" stroke="rgb(var(--line))" />
        <rect x="14" y="14" width="152" height="108" rx="16" fill="rgb(var(--chart-1))" opacity="0.08" />
        <g transform="translate(36 24)">
          <rect width="108" height="80" rx="12" fill="rgb(var(--surface-2))" stroke="rgb(var(--line))" />
          <path d="M14 22 H94 M14 38 H94 M14 54 H94" stroke="rgb(var(--line-strong))" strokeWidth="4" strokeLinecap="round" />
          <circle cx="88" cy="22" r="3" fill="rgb(var(--chart-1))" />
          <circle cx="88" cy="38" r="3" fill="rgb(var(--chart-3))" />
          <circle cx="88" cy="54" r="3" fill="rgb(var(--chart-2))" />
        </g>
        <g transform="translate(118 66)">
          <circle r="24" fill="rgb(var(--chart-1))" opacity="0.16" />
          <path
            d="M0 -18 L16 -10 V4 C16 16 8 22 0 26 C-8 22 -16 16 -16 4 V-10 Z"
            fill="rgb(var(--surface))"
            stroke="rgb(var(--chart-1))"
            strokeWidth="2"
          />
          <path d="M-7 2 L-1 8 L8 -5" fill="none" stroke="rgb(var(--chart-1))" strokeWidth="3" strokeLinecap="round" strokeLinejoin="round" />
        </g>
        <g transform="translate(16 134)">
          <rect width="148" height="26" rx="13" fill="rgb(var(--chart-1))" opacity="0.14" />
          <text x="74" y="17" textAnchor="middle" fontSize="9" fontWeight="700" fill="rgb(var(--chart-1))">Préstamo aprobado</text>
        </g>
        <text x="16" y="180" fontSize="11" fontWeight="700" fill="rgb(var(--ink))">Préstamos</text>
        <text x="16" y="193" fontSize="8" fill="rgb(var(--muted))">Entrega y devolución controladas</text>
      </g>

      <g transform="translate(430 116) rotate(5 90 90)" filter="url(#flujo-sombra)">
        <rect width="180" height="180" rx="24" fill="rgb(var(--surface))" stroke="rgb(var(--line))" />
        <rect x="14" y="14" width="152" height="88" rx="16" fill="rgb(var(--chart-2))" opacity="0.08" />
        <g transform="translate(46 32)">
          <path d="M0 18 L46 0 L92 18 L46 36 Z" fill="rgb(var(--chart-2))" opacity="0.18" stroke="rgb(var(--chart-2))" strokeWidth="2" />
          <path d="M0 18 V54 L46 72 V36 Z" fill="rgb(var(--chart-2))" opacity="0.1" stroke="rgb(var(--chart-2))" strokeWidth="2" />
          <path d="M92 18 V54 L46 72 V36 Z" fill="rgb(var(--chart-2))" opacity="0.14" stroke="rgb(var(--chart-2))" strokeWidth="2" />
          <path d="M46 36 V72" stroke="rgb(var(--chart-2))" strokeWidth="2" />
          <path d="M23 9 L46 0 L69 9" fill="none" stroke="rgb(var(--chart-2))" strokeWidth="2" />
        </g>
        <g transform="translate(122 60)">
          <path d="M16 0 A16 16 0 1 0 16 32" fill="none" stroke="rgb(var(--chart-1))" strokeWidth="4" strokeLinecap="round" />
          <path d="M8 6 L18 0 L8 -6" fill="none" stroke="rgb(var(--chart-1))" strokeWidth="4" strokeLinecap="round" strokeLinejoin="round" />
        </g>
        <g transform="translate(16 112)">
          <rect width="148" height="26" rx="13" fill="rgb(var(--chart-1))" opacity="0.14" />
          <text x="74" y="17" textAnchor="middle" fontSize="9" fontWeight="700" fill="rgb(var(--chart-1))">Equipo disponible</text>
        </g>
        <text x="16" y="158" fontSize="11" fontWeight="700" fill="rgb(var(--ink))">Devolución</text>
        <text x="16" y="171" fontSize="8" fill="rgb(var(--muted))">Estado actualizado y bitácora</text>
      </g>

      <g transform="translate(26 38)">
        <rect width="82" height="25" rx="12.5" fill="rgb(var(--surface))" stroke="rgb(var(--line))" />
        <circle cx="13" cy="12.5" r="3" fill="rgb(var(--chart-2))" />
        <text x="23" y="16" fontSize="9" fontWeight="700" fill="rgb(var(--ink))">RN-01</text>
      </g>

      <g transform="translate(512 38)">
        <rect width="96" height="25" rx="12.5" fill="rgb(var(--surface))" stroke="rgb(var(--line))" />
        <circle cx="13" cy="12.5" r="3" fill="rgb(var(--chart-4))" />
        <text x="23" y="16" fontSize="9" fontWeight="700" fill="rgb(var(--ink))">Auditoría</text>
      </g>

      <g transform="translate(518 314)">
        <rect width="98" height="25" rx="12.5" fill="rgb(var(--surface))" stroke="rgb(var(--line))" />
        <circle cx="13" cy="12.5" r="3" fill="rgb(var(--chart-1))" />
        <text x="23" y="16" fontSize="9" fontWeight="700" fill="rgb(var(--ink))">39 activos</text>
      </g>
    </svg>
  );
}
