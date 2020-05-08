<template>
  <div>
    <header>
      <nav class="navbar" role="navigation" aria-label="main navigation">
        <section class="navbar-menu is-active">
          <section class="navbar-end">
            <div class="navbar-item">
              <realm-select @realm="setRealm" />
            </div>
          </section>
        </section>
      </nav>
    </header>
    <main>
      <app-themes v-if="realm != null" :realm="realm"/>
    </main>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import RealmSelect from '@/components/realm-select.vue';
import AppThemes from '@/components/app-themes.vue';
import Realm from '@/dto/Realm';

@Component({
  components: {
    RealmSelect,
    AppThemes,
  },
})
export default class Forum extends Vue {
  realm: Realm|null = null;

  setRealm(msg: Realm|null) {
    this.realm = msg;
  }
}
</script>
