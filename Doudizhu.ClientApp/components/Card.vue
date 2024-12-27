<template>
    <div>
        <img :height="props.height" class="card" :src="`/images/cards/${parseColor(props.color)}${parseNumber(props.number)}.png`" alt="intro" />
    </div>
</template>

<script setup lang="ts">
const props = defineProps<{
    number: DoudizhuApiModelsGameLogicCardNumber
    color: DoudizhuApiModelsGameLogicCardColor,
    height: number
}>()

function parseColor(color: DoudizhuApiModelsGameLogicCardColor) {
    switch (color) {
        case DoudizhuApiModelsGameLogicCardColor.Heitao:
            return 'Spade'
        case DoudizhuApiModelsGameLogicCardColor.Hongtao:
            return 'Heart'
        case DoudizhuApiModelsGameLogicCardColor.Meihua:
            return 'Club'
        case DoudizhuApiModelsGameLogicCardColor.Fangkuai:
            return 'Diamond'
    }
    return ''
}

function parseNumber(number: DoudizhuApiModelsGameLogicCardNumber) {
    switch (number) {
        case DoudizhuApiModelsGameLogicCardNumber.A:
            return 'A'
        case DoudizhuApiModelsGameLogicCardNumber.J:
            return 'J'
        case DoudizhuApiModelsGameLogicCardNumber.Q:
            return 'Q'
        case DoudizhuApiModelsGameLogicCardNumber.K:
            return 'K'
        case DoudizhuApiModelsGameLogicCardNumber.BigJoker:
            return 'JOKER-A'
        case DoudizhuApiModelsGameLogicCardNumber.SmallJoker:
            return 'JOKER-B'
        case DoudizhuApiModelsGameLogicCardNumber.Two:
            return '2'
        case DoudizhuApiModelsGameLogicCardNumber.Three:
            return '3'
        case DoudizhuApiModelsGameLogicCardNumber.Four:
            return '4'
        case DoudizhuApiModelsGameLogicCardNumber.Five:
            return '5'
        case DoudizhuApiModelsGameLogicCardNumber.Six:
            return '6'
        case DoudizhuApiModelsGameLogicCardNumber.Seven:
            return '7'
        case DoudizhuApiModelsGameLogicCardNumber.Eight:
            return '8'
        case DoudizhuApiModelsGameLogicCardNumber.Nine:
            return '9'
        case DoudizhuApiModelsGameLogicCardNumber.Ten:
            return '10'
    }
    return ''
}
</script>

<style scoped>
.card{
    user-select: none;
    -webkit-user-drag: none;
}
</style>