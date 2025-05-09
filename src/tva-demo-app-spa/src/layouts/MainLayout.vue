<template>
  <q-layout view="lHh LpR lFf">
    <q-header
      elevated
      reveal
      :class="$q.dark.isActive ? 'header_dark' : 'header_normal'"
    >
      <q-toolbar>
        <q-btn
          @click="toggleLeftDrawer"
          flat
          round
          dense
          icon="menu"
          class="q-mr-sm"
        />

        <q-toolbar-title>Person Account Manager</q-toolbar-title>

        <q-btn
          class="q-mr-xs"
          flat
          round
          @click="$q.dark.toggle()"
          :icon="$q.dark.isActive ? 'nights_stay' : 'wb_sunny'"
        />

        <q-btn
          flat
          round
          dense
          icon="logout"
          @click="logoutNotify"
          to="/"
        />
      </q-toolbar>

    </q-header>

    <q-drawer
      class="left-navigation text-white transition-width"
      :class="$q.dark.isActive ? 'drawer_dark' : 'drawer_normal'"
      show-if-above
      v-model="leftDrawerOpen"
      style="background-image: url(https://cdn.pixabay.com/photo/2020/03/26/10/58/norway-4970080_1280.jpg) !important;"
      side="left"
      elevated
      :width="drawerWidth"
      :breakpoint="600"
    >
      <div
        class="full-height"
        :class="$q.dark.isActive ? 'drawer_dark' : 'drawer_normal'"
      >
        <div style="height: calc(100% - 117px);padding:10px;">
          <q-toolbar>
            <q-avatar>
              <img alt="pam" src="https://cdn.quasar.dev/img/boy-avatar.png" />
            </q-avatar>

            <q-toolbar-title>P A M</q-toolbar-title>

          </q-toolbar>
          <hr />
          <q-scroll-area style="height:100%;">
            <q-list padding>
              <q-item
                active-class="tab-active"
                to="/home"
                exact
                class="q-ma-sm navigation-item"
                clickable
                v-ripple
              >
                <q-item-section avatar>
                  <q-icon name="home" />
                </q-item-section>

                <q-item-section v-show="!isDrawerMinimized">
                  Home
                </q-item-section>
              </q-item>

              <q-item
                :active="$route.path.startsWith('/person')"
                exact
                active-class="tab-active"
                to="/persons"
                class="q-ma-sm navigation-item"
                clickable
                v-ripple
              >
                <q-item-section avatar>
                  <q-icon name="people" />
                </q-item-section>

                <q-item-section v-show="!isDrawerMinimized">
                  Persons
                </q-item-section>
              </q-item>

              <q-item
                active-class="tab-active"
                to="/about"
                class="q-ma-sm navigation-item"
                clickable
                v-ripple
              >
                <q-item-section avatar>
                  <q-icon name="info" />
                </q-item-section>

                <q-item-section v-show="!isDrawerMinimized">
                  About
                </q-item-section>
              </q-item>

              <q-item
                active-class="tab-active"
                to="/contact"
                class="q-ma-sm navigation-item"
                clickable
                v-ripple
              >
                <q-item-section avatar>
                  <q-icon name="chat" />
                </q-item-section>

                <q-item-section v-show="!isDrawerMinimized">
                  Contact
                </q-item-section>
              </q-item>

            </q-list>
          </q-scroll-area>
        </div>
      </div>
    </q-drawer>

    <q-page-container>
      <q-page class="row no-wrap">
        <div class="col">
          <div class="full-height">
            <q-scroll-area class="col q-pr-sm full-height" visible>
              <router-view
                :key="$route.fullPath"
                :class="$q.dark.isActive ? 'router_view_dark' : 'router_view_normal'"
                v-slot="{ Component }">
                <keep-alive>
                  <component :is="Component" />
                </keep-alive>
              </router-view>
            </q-scroll-area>
          </div>
        </div>
      </q-page>
    </q-page-container>

  </q-layout>
</template>

<script setup>
import { ref } from "vue";
import { useQuasar } from "quasar";

const $q = useQuasar();

const leftDrawerOpen = ref(true);
const isDrawerMinimized = ref(false);
const normalDrawerWidth = 300;
const drawerWidth = ref(normalDrawerWidth);

function toggleLeftDrawer() {
  leftDrawerOpen.value = !leftDrawerOpen.value;
}

function logoutNotify() {
  $q.notify({
      message: "Logged out"
    });
}
</script>

<style>
.q-drawer {
  background-image: url("https://cdn.pixabay.com/photo/2020/03/26/10/58/norway-4970080_1280.jpg") !important;
  background-size: cover !important;
}

.drawer_normal {
  background-color: rgba(1, 1, 1, 0.75);
  opacity: 0.75;
}

.drawer_dark {
  background-color: #010101f2;
  opacity: 0.95;
}

.transition-width {
  transition: width 0.3s ease;
}

.navigation-item {
  border-radius: 5px;
}

.tab-active {
  background-color: green;
  color: #fff !important;
}

body {
  background: #f1f1f1 !important;
}

.header_normal {
  background-color: rgb(73, 92, 95);
  color: white;
}

.header_dark {

  background-color: rgb(31, 29, 28);
  color: white;
}

.router_view_dark{
  background-color: rgb(32, 33, 33);
  overflow: hidden;
}

.router_view_normal {
  background-color: #f3f3f3e4;
  overflow: hidden;
}
</style>
