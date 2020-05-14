<template>
  <router-link :to="`/thread/${this.thread.hfid}`">
    <article class="columns is-vcentered hf-thread">
      <div class="column">
        <h2 class="title is-4">
          <span v-html="thread.name"></span>
        </h2>
        <p class="subtitle is-6">
          {{ thread.createdAt.toFormat('ff') }} · <a :href="authorUrl">{{ thread.authorName }}</a>
        </p>
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
      <div class="column is-2 is-hidden-touch">
        <p class="has-text-grey has-text-right">
          {{ stats.lastUpdate.toFormat('ff') }}<br />
          <a :href="authorUrl">{{ stats.authorName }}</a>
        </p>
      </div>
    </article>
  </router-link>
</template>

<script lang="ts">
import { Component, Prop, Watch, Vue } from 'vue-property-decorator';
import MagnitudeNumber from '@/components/magnitude-number.vue';
import Thread from '@/dto/Thread';
import Realm from '@/dto/Realm';
import Theme from '@/dto/Theme';
import ThreadStats from '@/dto/ThreadStats';

@Component({
  components: {
    MagnitudeNumber
  },
})
export default class ThreadRow extends Vue {
  @Prop() private realm!: Realm;
  @Prop() private theme!: Theme;
  @Prop() private thread!: Thread;

  hfurl = `${this.thread.realm.host}/forum.html/thread/${this.thread.hfid}/`;
  authorUrl = `${this.thread.realm.host}/user.html/${this.thread.author}`;

  stats: ThreadStats = ThreadStats.empty;

  @Watch('thread')
  private async fetchStats() {
    const res = await fetch(`http://localhost:5000/forum/realms/${this.realm.toString()}/themes/${this.theme.hfid}/threads/${this.thread.hfid}/stats`);
    const json = await res.json();
    this.stats = ThreadStats.fromJSON(json) ?? ThreadStats.empty;
  }

  beforeMount() {
    this.fetchStats();
  }
}
</script>

<style>
.hf-thread .hf-button {
  display: none;
}

.hf-thread:hover .hf-button {
  display: block !important;
}
</style>
