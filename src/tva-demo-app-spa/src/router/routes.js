import LoginPage from 'pages/LoginPage.vue';
import IndexPage from 'pages/IndexPage.vue';
import PersonsPage from '../pages/PersonsPage.vue';
import PersonDetailsPage from '../pages/PersonDetailsPage.vue';
import AccountDetailsPage from 'src/pages/AccountDetailsPage.vue';
import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/authStore.js';

const routes = [
  {
      path: '/login', name: 'login', component: LoginPage, meta: { questOnly: true }
  },
  {
    path: '/home',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', name: 'home', component: IndexPage, meta: { requiresAuth: true } }
    ]
  },
  {
    path: '/persons',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', name: 'persons', component: PersonsPage }
    ]
  },
  {
    path: '/person/:personId?',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', name: 'person_details', component: PersonDetailsPage, props: true }
    ]
  },
  {
    path: '/account/:accountId?/:personId?',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', name: 'account_details', component: AccountDetailsPage, props: true }
    ]
  },
  {
    path: '/about',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', name: 'about', component: () => import('pages/AboutPage.vue') }
    ]
  },
  {
    path: '/contact',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', name: 'contact', component: () => import('pages/ContactPage.vue') }
    ]
  },
  { path: '/:catchAll(.*)*', redirect: '/login' },
  { path: '/', redirect: '/home' } 
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore(); // Get store instance inside guard

  // If trying to access a protected route and not logged in, redirect to login
  if (to.matched.some(record => record.meta.requiresAuth) && !authStore.isLoggedIn) {
    next({ name: 'Login', query: { redirect: to.fullPath } });
  }
  // If trying to access a guest-only route (like login/register) and already logged in, redirect to home
  else if (to.matched.some(record => record.meta.guestOnly) && authStore.isLoggedIn) {
    next({ name: 'Home' }); // Or your default authenticated route
  }
  // Otherwise, allow navigation
  else {
    next();
  }
});

export default routes;
