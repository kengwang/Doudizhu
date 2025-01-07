<template>
    <v-app class="container" fluid>
        <v-row class="topbar" align="start" justify="center" no-gutters>
            <Card :height="50" :number="card.number ?? 0" :color="card.color ?? 0"
                v-for="card in currentGame.game?.reservedCards" :key="card.number" />
        </v-row>
        <v-row align="start" no-gutters>
            <!-- left -->
            <v-col cols="2">
                <div v-if="currentGame.joinedUsers?.length ?? 0 > 0">
                    <UserInfo :signature="currentGame.joinedUsers?.[0]?.user?.coin?.toString() ?? ''"
                        :name="currentGame.joinedUsers?.[0]?.user?.name ?? ''"
                        :qq="currentGame.joinedUsers?.[0]?.user?.qq ?? ''" />
                    <div v-if="currentGame.landlordId == currentGame.joinedUsers?.[0]?.id" class="bg-yellow">地主</div>
                    <Clock v-if="currentGame.currentUserId == currentGame.joinedUsers?.[0]?.id" />
                </div>
                <div style="margin-top: 15px;" v-if="currentGame.joinedUsers?.length ?? 0 > 1">
                    <UserInfo :signature="currentGame.joinedUsers?.[1]?.user?.coin?.toString() ?? ''"
                        :name="currentGame.joinedUsers?.[1]?.user?.name ?? ''"
                        :qq="currentGame.joinedUsers?.[1]?.user?.qq ?? ''" />
                    <div v-if="currentGame.landlordId == currentGame.joinedUsers?.[1]?.id" class="bg-yellow">地主</div>
                    <Clock v-if="currentGame.currentUserId == currentGame.joinedUsers?.[1]?.id" />
                </div>
            </v-col>
            <!-- middle -->
            <v-col cols="8">
                <v-row align="start" justify="center" no-gutters>
                    <Card :height="150" :number="card.number ?? 0" :color="card.color ?? 0"
                        v-for="card in currentGame.lastCards" :key="`${card.color}-${card.number}`" />
                </v-row>
            </v-col>
        </v-row>
        <v-row align="end" style="max-height: 400px; margin-bottom: 8px;" no-gutters>
            <div class="display-grid">
                <div v-if="currentGame.shouldCallLandlord" class="card-action">
                    <v-btn @click="landlordCall(1)" color="red">一分</v-btn>
                    <v-btn @click="landlordCall(2)" color="green">两分</v-btn>
                    <v-btn @click="landlordCall(3)" color="green">三分</v-btn>
                </div>
            </div>

            <SelectableCards :selected-cards.sync="selectedCards" :cards="currentGame.cards" />
        </v-row>
        <v-row class="bg-red" align="end" style="width: 100vw; max-height: 70px;" no-gutters>
            <UserInfo :signature="logginedUser?.coin.toString() ?? ''" :name="logginedUser?.name ?? ''"
                :qq="logginedUser?.qq ?? ''" />
            <div v-if="currentGame.landlordId == logginedUser?.id" class="bg-yellow">地主</div>
        </v-row>
    </v-app>
</template>

<script setup lang="ts">

definePageMeta({
    layout: 'game'
})

const selectedCards = ref([] as DoudizhuApiModelsGameLogicCard[]);
const api = useApi();
const logginedUser = useLogginedUser().logginedUser;

const gameId = useRoute().params.id as string;



function landlordCall(points: number) {
    api.games.callLandlordPointEndpoint(gameId, { point: points, gameUser: logginedUser?.id}).then((res) => {
        currentGame.shouldCallLandlord = false;
    });
}

let currentGame = useCurrentGame();
onMounted(async () => {
    api.games.getGameEndpoint(gameId).then((res) => {
        currentGame.game = res.data;
        currentGame.joinedUsers = res.data.users ? res.data.users.filter((u) => u.user?.id !== logginedUser!.id) : [];
        console.log(currentGame.joinedUsers);
    });
    await connectGameHub();
});
</script>


<style scoped>
.container {
    background: none;
}

.card-action {
    justify-self: center;
}

.display-grid {
    display: grid;
    width: 100%;
}
</style>