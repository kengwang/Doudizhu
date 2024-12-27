<template>
    <div class="container">
        <slot />
    </div>
</template>

<style scoped>
.container {
    margin: 0px;
    background: url('/images/background.jpg') no-repeat;
    background-size: cover;
    background-position: center;
    background-attachment: fixed;
}
</style>