import { defineStore } from "pinia"

export const useCurrentGame = defineStore("currentGame", () => {

    const game = ref<DoudizhuApiModelsGameLogicGame | null>(null)
    const cards = ref<DoudizhuApiModelsGameLogicCard[]>([])
    const shouldCallLandlord = ref<boolean>(false)
    const lastCards = ref<DoudizhuApiModelsGameLogicCard[]>([])
    const canPlayCard = ref<boolean>(false)
    const joinedUsers = ref<DoudizhuApiModelsGameLogicGameUser[]>([])
    const landlordId = ref<string | null>(null)
    const currentUserId = ref<string | null>(null)
    return {
        landlordId,
        joinedUsers,
        game,
        shouldCallLandlord,
        cards,
        lastCards,
        canPlayCard,
        currentUserId
    }
})