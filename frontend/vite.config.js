import { defineConfig } from 'vite';
import solidPlugin from 'vite-plugin-solid';

export default defineConfig({
  plugins: [solidPlugin()],
  server: {
    port: 3000,
    allowedHosts: ["vps.edc.lol", "localhost", "127.0.0.1"],
  },
  build: {
    target: 'esnext',
  },
});
