import { defineConfig } from 'vite';
import { fileURLToPath, URL } from 'url';
import * as path from 'path';
import basicSsl from '@vitejs/plugin-basic-ssl'

let aspnetProxy = {
    target: 'https://localhost:7186',
    secure: false
};

// https://vitejs.dev/config/
export default defineConfig({
    build: {
        manifest: true,  
    },
    resolve: {
        alias: {
            '@': fileURLToPath(new URL("./src", import.meta.url)),
        }
    },
    server: {
        hmr: {
            protocol: 'ws'
        },
    },
    plugins: [
        basicSsl()
    ],
})

