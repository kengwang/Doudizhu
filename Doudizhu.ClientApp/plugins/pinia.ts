import { createPinia } from "pinia";

export default defineNuxtPlugin(({ vueApp }) => {
    const pinia = createPinia()
    vueApp.use(pinia);
})