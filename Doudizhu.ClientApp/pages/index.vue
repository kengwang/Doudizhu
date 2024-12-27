<template>
  <div class="container">
    
  </div>
</template>

<script setup lang="ts">

const userStore = useLogginedUser();
const router = useRouter();
const api = useApi();
if (userStore.logginedUser) {
    api.user.getUserStatusEndpoint()
        .then(() => {
            router.push('/games');
        }).catch(() => {
            router.push('/user/login');
        });

} else {
    router.push('/user/login');
}
</script>

<style scoped>
.container {
    margin: 0px;
    background: url('/images/intro.jpg') no-repeat;
    background-size: cover;
    background-position: center;
    background-attachment: fixed;
}
</style>