/** Tarjeta reutilizable con encabezado, contenido y pie opcionales. */
export function Tarjeta({ children, className = '', ...props }) {
  return (
    <section className={`tarjeta overflow-hidden ${className}`} {...props}>
      {children}
    </section>
  );
}

export function TarjetaEncabezado({ titulo, descripcion, acciones, className = '' }) {
  return (
    <header className={`flex flex-wrap items-start justify-between gap-3 border-b border-line px-5 py-4 ${className}`}>
      <div className="min-w-0">
        <h2 className="truncate text-base font-bold text-ink">{titulo}</h2>
        {descripcion && <p className="mt-0.5 text-sm text-muted">{descripcion}</p>}
      </div>
      {acciones && <div className="flex shrink-0 flex-wrap items-center gap-2">{acciones}</div>}
    </header>
  );
}

export function TarjetaCuerpo({ children, className = '' }) {
  return <div className={`px-5 py-4 ${className}`}>{children}</div>;
}

export function TarjetaPie({ children, className = '' }) {
  return <footer className={`border-t border-line bg-surface-2 px-5 py-3 ${className}`}>{children}</footer>;
}
