import Vue from 'vue';
import VueRouter, { RouteConfig } from 'vue-router';
import VThemes from '@/views/v-themes.vue';
import VThreads from '@/views/v-threads.vue';

Vue.use(VueRouter);
Vue.config.ignoredElements = [/^ion-/];

const routes: Array<RouteConfig> = [
  {
    path: '/',
    name: 'v-themes',
    component: VThemes,
  },
  {
    path: '/theme/:theme',
    name: 'v-threads',
    component: VThreads,
  }
];

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes,
});

export default router;
