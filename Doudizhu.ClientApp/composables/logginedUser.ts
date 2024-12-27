import { defineStore } from "pinia"
import type { User } from "./models"

export const useLogginedUser = defineStore("logginedUser", () => {
    const logginedUser = ref<User | null>(null)
    
    return {
        logginedUser
    }
}, {
    persist: {
        key: "logginedUser"
    }    
})