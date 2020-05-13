<template>
  <section class="section">
    <nav class="level is-marginless">
      <div class="level-left">
        <div class="level-item">
          <h1 v-if="theme != null" class="title is-3">{{ theme.name }}</h1>
        </div>
      </div>
      <div class="level-right">
        <div class="field has-addons">
          <p class="control">
            <a href="#" class="button" :disabled="page === 0" @click="prevPage()">
              <ion-icon name="chevron-back-outline"></ion-icon>
              <span class="is-sr-only">Previous page</span>
            </a>
          </p>
          <p class="control">
            <span class="button">{{ page + 1 }}</span>
          </p>
          <p class="control">
            <a href="#" class="button" :disabled="threads.length === 0" @click="nextPage()">
              <ion-icon name="chevron-forward-outline"></ion-icon>
              <span class="is-sr-only">Next page</span>
            </a>
          </p>
        </div>
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
import Realm from '@/dto/Realm';
import Theme from '@/dto/Theme';
import Thread from '@/dto/Thread';
import List from '@/dto/List';

@Component({
  components: {
    ThreadRow,
  },
})
export default class VThreads extends Vue {
  @Prop() private realm!: Realm;

  threads: Thread[] = [];

  theme: Theme|null = null;

  page = 0;

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
  }

  @Watch('page')
  private async fetchThreads() {
    if (this.theme == null) {
      return;
    }
    const res = await fetch(`http://localhost:5000/forum/realms/${this.realm.toString()}/themes/${this.theme.hfid}/threads?offset=${this.offset}&limit=10`);
    const json = await res.json();
    const threads = List.fromJSON(Thread, json).list;
    if (threads.length === 0) {
      return this.prevPage();
    }
    this.threads = threads;
  }

  beforeMount() {
    this.setTheme().then(() => this.fetchThreads());
  }

  destroyed() {
    this.$emit('theme', null);
  }

  prevPage() {
    if (this.page > 0) {
      --this.page;
    }
  }

  nextPage() {
    if (this.threads.length > 0) {
      ++this.page;
    }
  }

  get sortedThreads() {
    return [...this.threads].sort((a, b) => a < b ? 1 : -1);
  }

  get offset() {
    return this.page * 10;
  }
}
</script>
