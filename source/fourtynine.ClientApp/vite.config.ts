import { defineConfig } from "vite";
import { fileURLToPath, URL } from "url";
import typescript from '@rollup/plugin-typescript';
import ttypescript from 'ttypescript'

const vitePort = 5173;

export default defineConfig({
    build: {
        manifest: true,
        rollupOptions: {
            input: [
                "src/main.ts", 
                "src/postingForm.ts",
            ],
        },
    },
    resolve: {
        alias: {
            "@": fileURLToPath(new URL("./src", import.meta.url)),
        }
    },
    server: {
        port: vitePort,
        strictPort: true,
        https: {
            cert: "../../fourtynine.pem",
            key: "../../fourtynine.key",
        },
        hmr: {
            protocol: "wss",
            
            // Here, we have to specify something that doesn't get mapped to an ASP.NET Core controller.
            // Otherwise, all websocket connection attempts get processed by APS.NET Core, without forwarding
            // them to the vite server, which makes the client fail to connect to the vite server.
            path: "vite-ws",
        },
    },
    plugins: [
        typescript({
            typescript: ttypescript,
        }),
    ],
})

