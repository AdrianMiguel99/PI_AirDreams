import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5276', // Reemplaza con el puerto de tu API
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
