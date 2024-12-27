<template>
    <div>
        <div class="container">
            <v-row align="center">
                <v-col class="pt-md-16" cols="12" md="4" offset-md="4">
                    <v-card class="mx-auto pa-12 pb-8">
                        <slot></slot>
                    </v-card>
                </v-col>
            </v-row>
        </div>
        <v-row justify="center" no-gutters>
            <v-col class="text-center mt-4" cols="12">
                {{ new Date().getFullYear() }} — <strong>CDUT</strong>
            </v-col>
        </v-row>
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