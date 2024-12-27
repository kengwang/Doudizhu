<template>
    <div>
        <UserInfo :signature="logginedUser?.coin.toString() ?? ''" :name="logginedUser?.name ?? ''"
            :qq="logginedUser?.qq ?? ''" />
        <v-btn @click="createGame" block>创建牌局</v-btn>
        <div style="margin-top: 10px;" v-for="game in games" :key="game.id">
            <v-card>
                <v-card-text>
                    <div>玩家数: {{ game.users?.length }}</div>
                    <div>状态: {{ parseStatus(game.status ?? 0) }}</div>
                </v-card-text>
                <v-card-actions v-if="game.status === 0">
                    <v-btn @click="joinGame(game.id ?? '')" variant="elevated" block>进入</v-btn>
                </v-card-actions>
            </v-card>
        </div>
        <div>

        </div>

    </div>
</template>

<script lang="ts" setup>
const games = ref([] as DoudizhuApiModelsGameLogicGame[]);
definePageMeta({
    layout: 'card'
});

const api = useApi();
const router = useRouter();
const logginedUser = useLogginedUser().logginedUser;

function createGame() {
    api.games.createGameEndpoint().then((res) => {
        console.log(res);
        res.json().then((data) => {
            router.push('/games/' + data.id);
        });
    }).catch((err) => {
        alert("创建失败");
    });
}

function joinGame(gameId: string) {
    api.games.joinGameEndpoint(gameId).then((res) => {
        router.push('/games/' + gameId);
    }).catch((err) => {
        alert("加入失败");
    });
}

function parseStatus(status: DoudizhuApiModelsGameLogicGameStatus) {
    switch (status) {
        case DoudizhuApiModelsGameLogicGameStatus.Waiting:
            return '等待中';
        case DoudizhuApiModelsGameLogicGameStatus.Running:
        case DoudizhuApiModelsGameLogicGameStatus.Starting:
            return '游戏中';
        case DoudizhuApiModelsGameLogicGameStatus.Ended:
            return '已结束';
    }
}

onMounted(async () => {
    api.games.getGamesEndpoint().then((res) => {
        games.value = res.data ?? [];
    });
});
</script>


<style></style>