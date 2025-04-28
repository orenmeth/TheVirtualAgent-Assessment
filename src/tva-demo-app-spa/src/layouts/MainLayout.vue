<template>
  <q-layout view="lHh LpR lFf">
    <q-header
      reveal
      :class="$q.dark.isActive ? 'header_dark' : 'header_normal'"
    >
      <q-toolbar>
        <q-btn
          @click="leftDrawerOpen = !leftDrawerOpen"
          flat
          round
          dense
          icon="menu"
          class="q-mr-sm"
        />

        <q-toolbar-title>PAM - Person Account Management</q-toolbar-title>
        <q-btn
          class="q-mr-xs"
          flat
          round
          @click="$q.dark.toggle()"
          :icon="$q.dark.isActive ? 'nights_stay' : 'wb_sunny'"
        />

        <q-btn flat round dense icon="search" class="q-mr-xs" />
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
      class="left-navigation text-white"
      :class="$q.dark.isActive ? 'drawer_dark' : 'drawer_normal'"
      show-if-above
      v-model="leftDrawerOpen"
      style="background-image: url(https://cdn.pixabay.com/photo/2020/03/26/10/58/norway-4970080_1280.jpg) !important;"
      side="left"
      elevated
    >
      <div
        class="full-height"
        :class="$q.dark.isActive ? 'drawer_dark' : 'drawer_normal'"
      >
        <div style="height: calc(100% - 117px);padding:10px;">
          <q-toolbar>
            <q-avatar>
              <img alt="boy" src="https://cdn.quasar.dev/img/boy-avatar.png" />
            </q-avatar>

            <q-toolbar-title></q-toolbar-title>
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
                  <q-icon name="dashboard" />
                </q-item-section>

                <q-item-section>
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

                <q-item-section>
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

                <q-item-section>
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

                <q-item-section>
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

<script>
import { ref } from "vue";

export default {
  setup() {
    const leftDrawerOpen = ref(false);

    return {
      leftDrawerOpen
    }
  },
  methods: {
    logoutNotify() {
      this.$q.notify({
        message: "Logged out"
      });
    }
  }
};
</script>

<style>
.q-drawer {
  background-image: url("https://cdn.pixabay.com/photo/2020/03/26/10/58/norway-4970080_1280.jpg") !important;
  background-size: cover !important;
}

.drawer_normal {
  background-color: rgba(1, 1, 1, 0.75);
}

.drawer_dark {
  background-color: #010101f2;
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
  background: linear-gradient(
    145deg,
    rgb(32, 106, 80) 15%,
    rgb(21, 57, 102) 70%
  );
}

.header_dark {
  background: linear-gradient(145deg, rgb(61, 14, 42) 15%, rgb(14, 43, 78) 70%);
}

.router_view_dark{
  background-color: #313131e4;
}

.router_view_normal {
  background-color: #f3f3f3e4;
}
</style>
