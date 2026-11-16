import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // Calls to /api go to the C# Web API, so the React app and the API work together without CORS problems.
    // Start the API first: dotnet run --project src/RSC.Api
    proxy: {
      '/api': 'http://localhost:5024',
    },
  },
})
