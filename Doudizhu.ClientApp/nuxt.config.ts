// https://nuxt.com/docs/api/configuration/nuxt-config
import vuetify, { transformAssetUrls } from 'vite-plugin-vuetify'
export default defineNuxtConfig({
    app: {
        pageTransition: { name: 'page', mode: 'out-in' }
    },
    build: {
        transpile: ['vuetify'],
    },
    compatibilityDate: '2024-11-01',
    devtools: { enabled: true },
    modules: [
        (_options, nuxt) => {
            nuxt.hooks.hook('vite:extendConfig', (config) => {
                // @ts-expect-error
                config.plugins.push(vuetify({ autoImport: true }))
            })
        },
        '@nuxtjs/i18n',
        '@nuxt/image',
        '@vueuse/nuxt',
        'pinia-plugin-persistedstate/nuxt'
    ],
    vite: {
        vue: {
            template: {
                transformAssetUrls,
            },
        },
    },
    nitro: {
        devProxy: {
            '/api': 'http://localhost:44460'
        }
    },
    i18n: {
        // Module Options
    }
})
