<template>
    <div>
        <div class="container">
            <slot />
        </div>
        <v-footer class="bg-grey-lighten-1">
            <v-row justify="center" no-gutters>
                <v-col class="text-center mt-4" cols="12">
                    {{ new Date().getFullYear() }} — <strong>CDUT</strong>
                </v-col>
            </v-row>
        </v-footer>
    </div>
</template>

<style scoped>
.container {
    margin: 0px;
    background: url('/images/intro.jpg') no-repeat;
    background-size: cover;
    background-position: center;
    background-attachment: fixed;
}
</style>