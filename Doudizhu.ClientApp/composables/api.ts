import { Api } from "~/utils/Api"

export const ApiCore = new Api({
    baseUrl: '/api'
})
export const useApi = () => {
    return ApiCore
}