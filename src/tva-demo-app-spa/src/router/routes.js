import PersonsPage from '../pages/PersonsPage.vue';
import PersonDetailsPage from '../pages/PersonDetailsPage.vue';
import AccountDetailsPage from 'src/pages/AccountDetailsPage.vue';

const routes = [
  {
      path: '/', name: 'default', component: () => import('pages/LoginPage.vue')
  },
  {
    path: '/home',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', name: 'home', component: () => import('pages/IndexPage.vue') }
    ]
  },
  {
    path: '/dashboard',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', name: 'dashboard', component: () => import('pages/DashboardPage.vue') }
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
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue')
  }
];

export default routes;
