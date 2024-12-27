<template>
    <div class="clock">
        <svg viewBox="0 0 36 36">
            <path
                class="circle-bg"
                d="M18 2.0845
                     a 15.9155 15.9155 0 0 1 0 31.831
                     a 15.9155 15.9155 0 0 1 0 -31.831"
            />
            <path
                class="circle"
                :stroke-dasharray="strokeDashArray"
                d="M18 2.0845
                     a 15.9155 15.9155 0 0 1 0 31.831
                     a 15.9155 15.9155 0 0 1 0 -31.831"
            />
            <text x="18" y="20.35" class="percentage">{{ secondsLeft }}</text>
        </svg>
    </div>
</template>

<script>
export default {
    data() {
        return {
            secondsLeft: 30,
            intervalId: null,
        };
    },
    computed: {
        strokeDashArray() {
            return `${(this.secondsLeft / 30) * 100}, 100`;
        },
    },
    mounted() {
        this.startCountdown();
    },
    beforeDestroy() {
        clearInterval(this.intervalId);
    },
    methods: {
        startCountdown() {
            this.intervalId = setInterval(() => {
                if (this.secondsLeft > 0) {
                    this.secondsLeft -= 1;
                } else {
                    clearInterval(this.intervalId);
                }
            }, 1000);
        },
    },
};
</script>

<style scoped>
.clock {
    width: 100px;
    height: 100px;
}
svg {
    width: 100%;
    height: 100%;
}
.circle-bg {
    fill: none;
    stroke: #eee;
    stroke-width: 3.8;
}
.circle {
    fill: none;
    stroke: #00acc1;
    stroke-width: 3.8;
    stroke-linecap: round;
    transition: stroke-dasharray 0.3s;
}
.percentage {
    fill: #00acc1;
    font-family: Arial, sans-serif;
    font-size: 0.5em;
    text-anchor: middle;
}
</style>