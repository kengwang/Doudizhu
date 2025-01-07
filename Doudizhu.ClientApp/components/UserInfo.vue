<template>
    <div class="user-info">
        <UserAvatar class="avatar" :qq="props.qq" />
        <span class="username">{{ props.name }}</span>
        <span class="signature">{{ props.signature }}</span>
    </div>
</template>

<script setup lang="ts">
const props = defineProps<{
    qq: String,
    name: String,
    signature: String
}>();
</script>

<style scoped>
.user-info {
    position: relative;
    -webkit-box-sizing: border-box;
    box-sizing: border-box;
    height: 72px;
    padding: 16px;
    background-color: #ffffff;
    color: black;
}

.avatar {
    float: left;
    width: 40px;
    height: 40px;
}

.username {
    display: block;
    margin-left: 52px;
    font-weight: 500;
    font-size: 16px;
    line-height: 20px;
    opacity: .87;
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis;
}

.signature {
    display: block;
    margin-left: 52px;
    font-weight: 400;
    font-size: 14px;
    line-height: 20px;
    opacity: .54;
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis;
}
</style>