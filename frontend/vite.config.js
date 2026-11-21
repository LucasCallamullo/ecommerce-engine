import path from "path"
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import { defineConfig } from 'vite'


export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
  ],
  resolve: {
    alias: {
      '@': path.resolve(import.meta.dirname, './src'),
      '@products': path.resolve(import.meta.dirname, './src/features/products'),
      '@auth': path.resolve(import.meta.dirname, './src/features/auth'),
      '@home': path.resolve(import.meta.dirname, './src/features/home'),
      '@shared': path.resolve(import.meta.dirname, './src/shared'),
      '@features': path.resolve(import.meta.dirname, './src/features'),
    },
  },
})