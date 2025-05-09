<template>
  <router-view />
</template>

<script setup>
import { useAuthStore } from './stores/authStore';
const authStore = useAuthStore();
authStore.initializeAuth();
</script>
