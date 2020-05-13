import Vue from 'vue';
import VueRouter, { RouteConfig } from 'vue-router';
import VThemes from '@/views/v-themes.vue';
import VThreads from '@/views/v-threads.vue';
import VPosts from '@/views/v-posts.vue';

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
  },
  {
    path: '/thread/:thread',
    name: 'v-posts',
    component: VPosts,
  },
  {
    path: '/forum.html/*',
    beforeEnter(to, from, next) {
      window.location.assign('http://hammerfest.fr' + to.fullPath);
    }
  }
];

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes,
});

export default router;
