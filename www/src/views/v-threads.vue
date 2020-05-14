<template>
  <section class="section">
    <nav v-if="theme != null" class="level is-marginless">
      <div class="level-left">
        <div class="level-item">
          <h1 class="title is-3">{{ theme.name }}</h1>
        </div>
      </div>
      <div class="level-right">
        <nav-page v-model="page" min="1" :max="maxPage" />
      </div>
    </nav>
    <article class="section">
      <thread-row v-for="thread in sortedThreads" :key="thread.thread" :realm="realm" :theme="theme" :thread="thread" />
    </article>
  </section>
</template>

<script lang="ts">
import { Component, Prop, Watch, Vue } from 'vue-property-decorator';
import ThreadRow from '@/components/thread-row.vue';
import NavPage from '@/components/nav-page.vue';
import Realm from '@/dto/Realm';
import Theme from '@/dto/Theme';
import Thread from '@/dto/Thread';
import List from '@/dto/List';

const OFFSET = 10;

@Component({
  components: {
    ThreadRow,
    NavPage,
  },
})
export default class VThreads extends Vue {
  @Prop() private realm!: Realm;

  threads: Thread[] = [];

  theme: Theme|null = null;

  page = 1;

  private async setTheme() {
    const id = parseInt(this.$route.params['theme']);
    if (isNaN(id)) {
      this.theme = null;
    } else {
      try {
        const res = await fetch(`http://localhost:5000/forum/realms/${this.realm.toString()}/themes/${id}`);
        const json = await res.json();
        this.theme = Theme.fromJSON(json);
      } catch {
        this.theme = null;
      }
    }
    this.$emit('theme', this.theme);
    this.$emit('thread', null);
  }

  @Watch('page')
  private async fetchThreads() {
    if (this.theme == null) {
      return;
    }
    const res = await fetch(`http://localhost:5000/forum/realms/${this.realm.toString()}/themes/${this.theme.hfid}/threads?offset=${this.offset}&limit=10`);
    const json = await res.json();
    const threads = List.fromJSON(Thread, json).list;
    this.threads = threads;
  }

  beforeMount() {
    this.setTheme().then(() => this.fetchThreads());
  }

  get sortedThreads() {
    return [...this.threads].sort((a, b) => a < b ? 1 : -1);
  }

  get offset() {
    return (this.page - 1) * OFFSET;
  }

  get maxPage() {
    return Math.ceil(this.theme!.threads / OFFSET);
  }
}
</script>
