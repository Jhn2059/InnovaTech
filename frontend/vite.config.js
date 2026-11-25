import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    host: true,
    // Proxy de desarrollo: el frontend consume rutas relativas "/api" y las
    // reenvía al backend, evitando problemas de CORS en desarrollo.
    proxy: {
      '/api': {
        target: process.env.VITE_API_PROXY || 'http://localhost:5080',
        changeOrigin: true
      }
    }
  },
  preview: {
    port: 4173,
    host: true
  },
  build: {
    outDir: 'dist',
    sourcemap: false,
    chunkSizeWarningLimit: 900
  }
})
