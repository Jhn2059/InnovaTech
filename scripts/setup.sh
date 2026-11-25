#!/usr/bin/env bash
# Preparación automatizada del entorno de desarrollo de TechAsset.
# Uso: bash scripts/setup.sh
set -euo pipefail

RAIZ="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
echo "TechAsset · preparación del entorno en ${RAIZ}"

# ── 1 · PostgreSQL ────────────────────────────────────────────────────────────
if ! command -v psql >/dev/null 2>&1; then
  echo "→ PostgreSQL no está instalado. En Debian/Ubuntu:"
  echo "    sudo apt-get update && sudo apt-get install -y postgresql postgresql-contrib"
  exit 1
fi

if ! su postgres -c "psql -tAc \"SELECT 1 FROM pg_roles WHERE rolname='techasset'\"" | grep -q 1; then
  echo "→ Creando el rol y las bases de datos de TechAsset…"
  su postgres -c "psql -c \"CREATE USER techasset WITH PASSWORD 'techasset' SUPERUSER;\""
  su postgres -c "psql -c \"CREATE DATABASE techasset OWNER techasset;\""
  su postgres -c "psql -c \"CREATE DATABASE techasset_test OWNER techasset;\""
else
  echo "→ El rol 'techasset' ya existe."
fi

# ── 2 · Backend ───────────────────────────────────────────────────────────────
echo "→ Restaurando y compilando la solución .NET…"
dotnet tool install --global dotnet-ef --version 8.0.11 >/dev/null 2>&1 || true
dotnet restore "${RAIZ}/backend/TechAsset.sln"
dotnet build "${RAIZ}/backend/TechAsset.sln" --nologo -v q

export PATH="$PATH:${HOME}/.dotnet/tools"
echo "→ Aplicando las migraciones de Entity Framework Core…"
dotnet ef database update \
  --project "${RAIZ}/backend/src/TechAsset.Infrastructure" \
  --startup-project "${RAIZ}/backend/src/TechAsset.API"

# ── 3 · Frontend ──────────────────────────────────────────────────────────────
echo "→ Instalando las dependencias del frontend…"
(cd "${RAIZ}/frontend" && npm install)
echo "→ Verificando la versión de Tailwind CSS…"
(cd "${RAIZ}/frontend" && node scripts/verificar-tailwind.mjs)

cat <<'EOF'

✔ Entorno listo.

  Backend : cd backend  && dotnet run --project src/TechAsset.API --urls "http://localhost:5080"
  Swagger : http://localhost:5080/swagger
  Frontend: cd frontend && npm run dev   →  http://localhost:5173

  Pruebas : cd backend  && dotnet test
EOF
