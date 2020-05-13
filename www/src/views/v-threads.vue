<template>
  <section class="section">
    <nav class="level">
      <div class="level-left"></div>
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
    <thread-row v-for="thread in sortedThreads" :key="thread.thread" :thread="thread" />
  </section>
</template>

<script lang="ts">
import { Component, Prop, Watch, Vue } from 'vue-property-decorator';
import ThreadRow from '@/components/thread-row.vue';
import Realm from '@/dto/Realm';
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

  themeId: number|null = null;

  page = 0;

  private setThemeId() {
    const id = parseInt(this.$route.params['theme']);
    this.themeId = isNaN(id) ? null : id;
  }

  @Watch('page')
  private async fetchThreads() {
    if (this.themeId == null) {
      return;
    }
    const res = await fetch(`http://localhost:5000/forum/realms/${this.realm.toString()}/themes/${this.themeId}/threads?offset=${this.offset}&limit=10`);
    const json = await res.json();
    const threads = List.fromJSON(Thread, json).list;
    if (threads.length === 0) {
      return this.prevPage();
    }
    this.threads = threads;
  }

  beforeMount() {
    this.setThemeId();
    this.fetchThreads();
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
