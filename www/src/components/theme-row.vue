<template>
  <router-link :to="'/theme/' + this.theme.hfid">
    <article class="columns is-vcentered hf-theme">
      <div class="column is-narrow">
        <figure class="image is-32x32">
          <img v-if="theme.hfid == 1" src="/image/icon_key.svg" />
          <img v-else-if="theme.hfid == 2" src="/image/icon_ankhel.svg" />
          <img v-else-if="theme.hfid == 3" src="/image/icon_secret.svg" />
          <img v-else-if="theme.hfid == 4" src="/image/icon_tomato.svg" />
          <img v-else src="/image/icon_paper.svg" />
        </figure>
      </div>
      <div class="column">
        <h2 class="title is-3">
          {{ theme.name }}
        </h2>
      </div>
      <div class="column is-narrow hf-button">
        <a class="button is-small is-rounded is-link is-outlined" :href="hfurl">
          <span>Original</span>
          <span class="icon">
            <ion-icon name="open-outline"></ion-icon>
          </span>
        </a>
      </div>
      <div class="column is-1 has-text-centered is-hidden-touch">
        <div class="level-item">
          <div>
            <magnitude-number class="title is-4" :value="stats.posts" />
            <p class="heading has-text-grey">Messages</p>
          </div>
        </div>
      </div>
      <div class="column is-1 has-text-centered is-hidden-touch">
        <div class="level-item">
          <div>
            <magnitude-number class="title is-4" :value="stats.threads" />
            <p class="heading has-text-grey">Threads</p>
          </div>
        </div>
      </div>
      <div class="column is-3 is-hidden-touch">
        <p>
          <router-link :to="threadUrl">{{ stats.threadName }}</router-link><br />
          <span class="has-text-grey">{{ stats.lastUpdate.toFormat('ff') }}</span> · <a :href="authorUrl">{{ stats.authorName }}</a>
        </p>
      </div>
    </article>
  </router-link>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import MagnitudeNumber from '@/components/magnitude-number.vue';
import Theme from '@/dto/Theme';
import ThemeStats from '@/dto/ThemeStats';

@Component({
  components: {
    MagnitudeNumber
  },
})
export default class ThemeRow extends Vue {
  @Prop() private theme!: Theme;

  hfurl = `${this.theme.realm.host}/forum.html/theme/${this.theme.hfid}/`;
  stats: ThemeStats = ThemeStats.empty;

  get authorUrl() {
    return `${this.theme.realm.host}/user.html/${this.stats.author}`;
  }

  get threadUrl() {
    return `/theme/${this.theme.hfid}/thread/${this.stats.thread}`;
  }

  private async fetchStats() {
    const res = await fetch(`http://localhost:5000/forum/realms/${this.theme.realm.toString()}/themes/${this.theme.hfid}/stats`);
    const json = await res.json();
    this.stats = ThemeStats.fromJSON(json) ?? ThemeStats.empty;
  }

  beforeMount() {
    this.fetchStats();
  }
}
</script>

<style>
.hf-theme .hf-button {
  display: none;
}

.hf-theme:hover .hf-button {
  display: block !important;
}
</style>
