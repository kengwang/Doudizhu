<template>
    <div style="width: 100%">
        <div v-if="currentGame.canPlayCard" class="card-action">
            <v-btn @click="onPass" color="red">不出</v-btn>
            <v-btn @click="onPlay" color="green">出牌</v-btn>
            <Clock />
        </div>
    <div class="selectable-cards">
        <div class="container">
            <transition name="bounce" v-for="card in cards" :key="`${card.number}-${card.color}`">
                <Card :isSelected="selectedCards.includes(card)"
                    @click="onSelect(card)" :id="`${card.number}-${card.color}`" :height="200"
                    :color="card.color ?? 0" :number="card.number ?? 0" class="card"
                    :class="{ 'selected': selectedCards.includes(card) }" />
            </transition>
        </div>
    </div>
</div>
</template>

<script setup lang="ts">

const currentGame = useCurrentGame();
const api = useApi();
const props = defineProps<{
    cards: DoudizhuApiModelsGameLogicCard[];
}>();

function onPlay() {
    api.games.playCardEndpoint(currentGame.game?.id ?? '', { cards: selectedCards.value }).then((res) => {
        currentGame.cards = currentGame.cards.filter((c: DoudizhuApiModelsGameLogicCard) => !selectedCards.value.includes(c));
        selectedCards.value = [];
        currentGame.canPlayCard = false;
    }).catch((err) => {
        console.error(err);
        selectedCards.value = [];
        alert("你出的牌不符合规则");
    });
}

function onPass() {
    selectedCards.value = [];
    api.games.playCardEndpoint(currentGame.game?.id ?? '', { cards: [] }).then((res) => {
        currentGame.canPlayCard = false;
    });
}

const selectedCards = ref<DoudizhuApiModelsGameLogicCard[]>([]);

function onSelect(cardId: DoudizhuApiModelsGameLogicCard) {
  if(selectedCards.value.includes(cardId)) {
    selectedCards.value=selectedCards.value.filter((card: DoudizhuApiModelsGameLogicCard) => card!==cardId);
  } else {
    selectedCards.value.push(cardId);
    }
}
</script>

<style scoped>
/* 选中状态样式 */
.selected {
    position: relative;
    top: -20px;
    transition: top 0.3s ease, z-index 0.3s ease;
    z-index: 10;
}

.card {
    position: relative;
    z-index: 1;
    height: 200px;
    margin-right: -90px;
    user-select: none;
    transition: top 0.3s ease, z-index 0.3s ease;
}

.card:last-child {
    margin-right: 0;
}

.bounce-enter-active,
.bounce-leave-active {
    transition: transform 0.3s ease;
}

.bounce-enter,
.bounce-leave-to {
    transform: translateY(20px);
}

.selection-area {
    position: absolute;
    background-color: rgba(0, 0, 255, 0.2);
    border: 1px dashed blue;
    pointer-events: none;
}

.selectable-cards {
    width: 100%;
    display: flex;
    justify-content: center;
}

.container {
    display: flex;
}
</style>
