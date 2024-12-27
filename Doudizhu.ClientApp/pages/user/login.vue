<template>
    <div>
        <v-card-title>登录</v-card-title>
        <v-card-text>
            <v-form>
                <v-text-field v-model="qq" label="QQ" required></v-text-field>
                <v-btn @click="login" variant="flat" block color="blue-darken-4">登录</v-btn>
                <br />
                <v-btn @click="register" variant="tonal" block color="blue-darken-4">注册</v-btn>
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
});

const qq = ref('');
const api = useApi();
const snackbar = ref(false);
const msg = ref('');
const router = useRouter();
const logginUser = useLogginedUser();

function register() {
    router.push('/user/register');
}

function login() {
    api.user.userLoginEndpoint({
        qq: qq.value as string
    }).then((res) => {
        logginUser.logginedUser = {
            id: res.data.id ?? '',
            name: res.data.name ?? '',
            coin: res.data.coin ?? 0,
            qq: res.data.qq ?? '',
        };
        router.push('/games');
    }).catch((err) => {
        msg.value = "登录失败";
        snackbar.value = true;
    });
}
</script>

<style>

</style>