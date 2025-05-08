<template>
  <q-layout>
    <q-page-container>
      <q-page class="flex flex-center login-page">
        <div class="gradient" :class="$q.dark.isActive ? 'dark' : 'normal'"></div>
        <q-btn
          color="white"
          class="absolute-top-right"
          flat
          round
          @click="$q.dark.toggle()"
          :icon="$q.dark.isActive ? 'nights_stay' : 'wb_sunny'"
        />
        <q-card :style="cardStyle">
          <q-img src="https://cdn.pixabay.com/photo/2018/05/04/15/49/cyber-security-3374252_1280.jpg"></q-img>
          <q-card-section>
            <q-avatar
              size="74px"
              class="absolute"
              style="top: 0; right: 25px; transform: translateY(-50%)"
            >
            </q-avatar>
          </q-card-section>

          <q-card-section>
            <q-form class="q-gutter-md">
              <q-input filled v-model="username" label="Username" lazy-rules />

              <q-input type="password" filled v-model="password" label="Password" lazy-rules />

              <div>
                <q-btn
                  label="Login"
                  to="/home"
                  type="button"
                  color="primary"
                  @click="loginNotify"
                />
              </div>
            </q-form>
          </q-card-section>
        </q-card>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useQuasar } from 'quasar'

const $q = useQuasar()
const username = ref('admin')
const password = ref('Admin@CRM')

const cardStyle = computed(() => {
  return $q.platform.is.mobile
    ? { width: '90%', margin: '0 auto' }
    : { width: '30%', minWidth: '200px' }
})

function loginNotify() {
  $q.notify({
    message: 'Login Successful',
  })
}
</script>

<style scoped>
.login-page {
  position: relative;
  width: 100%;
  height: 100%;
  overflow: hidden;
  display: flex;
  justify-content: center;
  align-items: center;
}

.gradient {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-size: 400% 400%;
  animation: gradient 15s ease infinite;

  &.dark {
    background-image: linear-gradient(45deg, #1B5E20, #0D47A1, #4A148C, #E65100);
  }

  &.normal {
    background-image: linear-gradient(45deg, #8BC34A, #1E88E5, #AB47BC, #FFB300);
  }
}

@keyframes gradient {
    0% {
      background-position: 0% 50%;
    }
    50% {
      background-position: 100% 50%;
    }
    100% {
      background-position: 0% 50%;
    }
  }

.login-container {
  background-color: rgba(255, 255, 255, 0.8);
  padding: 30px;
  border-radius: 8px;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.2);
  text-align: center;
  z-index: 1;
}

.login-container-dark {
  background-color: rgba(255, 255, 255, 0.8);
  padding: 30px;
  border-radius: 8px;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.2);
  text-align: center;
  z-index: 1;
}

h1 {
  margin-bottom: 20px;
  color: #333;
}
</style>
