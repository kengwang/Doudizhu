<template>
    <div>
        <v-avatar :image="`https://q.qlogo.cn/headimg_dl?dst_uin=${props.qq}&spec=640&img_type=jpg`" alt="avatar" />
    </div>
</template>

<script lang="ts" setup>

// define props
const props = defineProps<{
    qq: String
}>();

</script>

<style>

</style>