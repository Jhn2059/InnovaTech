/** @type {import('tailwindcss').Config} */
export default {
  darkMode: 'class',
  content: ['./index.html', './src/**/*.{js,jsx}'],
  theme: {
    extend: {
      colors: {
        brand: {
          50: '#eff6ff',
          100: '#dbeafe',
          200: '#bfdbfe',
          300: '#93c5fd',
          400: '#60a5fa',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8',
          800: '#1e40af',
          900: '#1e3a8a',
          950: '#172554'
        },
        surface: 'rgb(var(--surface) / <alpha-value>)',
        'surface-2': 'rgb(var(--surface-2) / <alpha-value>)',
        'surface-3': 'rgb(var(--surface-3) / <alpha-value>)',
        ink: 'rgb(var(--ink) / <alpha-value>)',
        muted: 'rgb(var(--muted) / <alpha-value>)',
        line: 'rgb(var(--line) / <alpha-value>)',
        'line-strong': 'rgb(var(--line-strong) / <alpha-value>)',
        control: 'rgb(var(--control) / <alpha-value>)'
      },
      fontFamily: {
        sans: ['Inter', 'ui-sans-serif', 'system-ui', '-apple-system', 'Segoe UI', 'Roboto', 'Helvetica Neue', 'Arial', 'sans-serif'],
        mono: ['ui-monospace', 'SFMono-Regular', 'Menlo', 'monospace']
      },
      boxShadow: {
        card: '0 1px 2px rgba(15, 23, 42, 0.06), 0 12px 32px -18px rgba(15, 23, 42, 0.35)',
        pop: '0 24px 60px -20px rgba(15, 23, 42, 0.45)'
      },
      keyframes: {
        'fade-in': { '0%': { opacity: '0', transform: 'translateY(6px)' }, '100%': { opacity: '1', transform: 'translateY(0)' } },
        'fade-in-scale': { '0%': { opacity: '0', transform: 'scale(.97)' }, '100%': { opacity: '1', transform: 'scale(1)' } },
        'slide-in': { '0%': { opacity: '0', transform: 'translateX(16px)' }, '100%': { opacity: '1', transform: 'translateX(0)' } }
      },
      animation: {
        'fade-in': 'fade-in .28s ease-out both',
        'fade-in-scale': 'fade-in-scale .2s ease-out both',
        'slide-in': 'slide-in .25s ease-out both'
      }
    }
  },
  plugins: []
}
