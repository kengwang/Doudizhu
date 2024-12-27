<template>
    <div>
        <v-card-title>注册</v-card-title>
        <v-card-text>
            <v-form>
                <v-text-field v-model="username" label="用户名" required></v-text-field>
                <v-text-field v-model="qq" label="QQ" required></v-text-field>
                <v-btn @click="register" variant="tonal" block color="indigo-darken-3">注册</v-btn>
            </v-form>
        </v-card-text>
        <v-snackbar v-model="snackbar">
            {{ msg }}
        </v-snackbar>
    </div>
</template>

<script setup lang="ts">

definePageMeta({
    layout: 'card'
})

const qq = ref('');
const username = ref('');
const api = useApi();
const snackbar = ref(false);
const msg = ref('');

const router = useRouter();

function register() {
    api.user.userRegisterEndpoint({
        qq: qq.value,
        userName: username.value
    }).then((res) => {
        router.push('/user/login');
    }).catch((err) => {
        console.log(err);
        msg.value = "注册失败";
        snackbar.value = true;
    });
}
</script>

<style></style>