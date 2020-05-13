<template>
  <div>
    <header>
      <nav class="navbar has-shadow is-spaced" role="navigation" aria-label="main navigation">
        <div class="container">
          <section class="navbar-brand">
            <a class="navbar-item" href="https://github.com/Aksamyt/hf-vault">
              <span class="title is-3 is-family-code">
                HF Vault
              </span>
            </a>
          </section>
          <section class="navbar-menu is-active">
            <section class="navbar-start">
              <div class="navbar-item">
                <nav-breadcrumb v-if="realm != null" :realm="realm" :theme="theme" />
              </div>
            </section>
            <section class="navbar-end">
              <div class="navbar-item">
                <realm-select @realm="setRealm" />
              </div>
            </section>
          </section>
        </div>
      </nav>
    </header>
    <main class="container">
      <router-view v-if="realm != null" :realm="realm" @theme="setTheme"/>
    </main>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import RealmSelect from '@/components/realm-select.vue';
import NavBreadcrumb from '@/components/nav-breadcrumb.vue';
import Realm from '@/dto/Realm';
import Theme from '@/dto/Theme';

@Component({
  components: {
    RealmSelect,
    NavBreadcrumb,
  },
})
export default class Forum extends Vue {
  realm: Realm|null = null;
  theme: Theme|null = null;

  setRealm(msg: Realm|null) {
    this.realm = msg;
  }

  setTheme(msg: Theme|null) {
    this.theme = msg;
  }
}
</script>
