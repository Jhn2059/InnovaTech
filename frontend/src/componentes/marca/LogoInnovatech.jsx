import { useId } from 'react';

export function IconoInnovatech({ className = 'h-10 w-10' }) {
  const uid = useId().replace(/:/g, '');
  const gradId = `innovatech-grad-${uid}`;
  const glowId = `innovatech-glow-${uid}`;

  return (
    <svg viewBox="0 0 64 64" role="img" aria-label="Innovatech Solutions" className={`${className} text-white`}>
      <defs>
        <linearGradient id={gradId} x1="8" y1="8" x2="56" y2="56" gradientUnits="userSpaceOnUse">
          <stop stopColor="#67e8f9" />
          <stop offset="1" stopColor="#0ea5e9" />
        </linearGradient>
        <filter id={glowId} x="-50%" y="-50%" width="200%" height="200%">
          <feGaussianBlur stdDeviation="1.8" result="blur" />
          <feMerge>
            <feMergeNode in="blur" />
            <feMergeNode in="SourceGraphic" />
          </feMerge>
        </filter>
      </defs>

      <ellipse
        cx="32"
        cy="35"
        rx="26"
        ry="14"
        transform="rotate(-22 32 35)"
        fill="none"
        stroke={`url(#${gradId})`}
        strokeWidth="4"
        strokeLinecap="round"
        filter={`url(#${glowId})`}
      />
      <text
        x="32"
        y="45"
        textAnchor="middle"
        fontFamily="Arial Black, Arial, Helvetica, sans-serif"
        fontSize="30"
        fontStyle="italic"
        fontWeight="900"
        fill="currentColor"
        letterSpacing="-3"
      >
        IS
      </text>
      <circle cx="7" cy="40" r="4.2" fill={`url(#${gradId})`} />
      <rect x="48" y="14" width="4" height="4" rx="0.6" fill="#67e8f9" />
      <rect x="54" y="20" width="4" height="4" rx="0.6" fill="#22d3ee" />
      <rect x="48" y="26" width="4" height="4" rx="0.6" fill="#0ea5e9" opacity="0.8" />
    </svg>
  );
}

export function MarcaInnovatech({ className = '', compacto = false, claro = true }) {
  return (
    <span className={`flex items-center gap-3 ${className}`}>
      <IconoInnovatech className={`${compacto ? 'h-10 w-10 shrink-0' : 'h-12 w-12 shrink-0'} ${claro ? '!text-white' : '!text-slate-950'}`} />
      <span className="leading-none">
        <span className={`block text-base font-black tracking-[-0.035em] ${claro ? 'text-white' : 'text-slate-950'}`}>
          Innova<span className={claro ? 'text-cyan-400' : 'text-cyan-600'}>TECH</span>
        </span>
        <span className={`block text-[10px] font-bold uppercase tracking-[0.38em] ${claro ? 'text-slate-300' : 'text-slate-500'}`}>
          Solutions
        </span>
        {!compacto && (
          <span className={`mt-1 block text-[9px] font-semibold uppercase tracking-[0.24em] ${claro ? 'text-slate-400' : 'text-slate-500'}`}>
            Gestiona · Controla · Innova
          </span>
        )}
      </span>
    </span>
  );
}

export function LogotipoInnovatech({ className = '' }) {
  return (
    <div className={`flex flex-col items-center text-center ${className}`}>
      <IconoInnovatech className="h-28 w-28 !text-white drop-shadow-[0_0_32px_rgba(34,211,238,0.35)]" />
      <p className="mt-5 text-4xl font-black leading-none tracking-[-0.045em] text-white sm:text-5xl">
        Innova<span className="bg-gradient-to-r from-cyan-300 to-cyan-500 bg-clip-text text-transparent">TECH</span>
      </p>
      <p className="mt-2 text-lg font-bold uppercase leading-none tracking-[0.42em] text-slate-300 sm:text-xl">
        Solutions
      </p>
      <p className="mt-5 text-xs font-semibold uppercase tracking-[0.3em] text-slate-300 sm:text-sm">
        Gestiona · Controla · Innova
      </p>
    </div>
  );
}
