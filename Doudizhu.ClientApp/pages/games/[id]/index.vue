<template>
    <v-app class="container" fluid>
        <v-row class="topbar" align="start" justify="center" no-gutters>
            <Card :height="50" :number="card.number ?? 0" :color="card.color ?? 0" v-for="card in game.reservedCards"
                :key="card.number" />
        </v-row>
        <v-row class="bg-yellow" align="start" no-gutters>
            333
        </v-row>
        <v-row class="bg-green" align="end" style="max-height: 400px;" no-gutters>
            111111
        </v-row>
        <v-row class="bg-red" align="end" style="max-height: 70px;" no-gutters>
            <UserInfo :signature="logginedUser?.coin.toString() ?? ''" :name="logginedUser?.name ?? ''"
                :qq="logginedUser?.qq ?? ''" />
        </v-row>
    </v-app>
</template>

<script setup lang="ts">
definePageMeta({
    layout: 'game'
})


const game = ref({} as DoudizhuApiModelsGameLogicGame);

const api = useApi();
const logginedUser = useLogginedUser().logginedUser;

const gameId = useRoute().params.id as string;
onMounted(() => {
    api.games.getGameEndpoint(gameId).then((res) => {
        game.value = res.data ?? {};
    });
});
</script>


<style scoped>
.container
{
    background: none;
}</style>