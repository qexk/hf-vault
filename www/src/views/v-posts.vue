<template>
  <section class="section">
    <nav class="level is-marginless">
      <div class="level-left">
        <div class="level-item">
          <h1 v-if="thread != null" class="title is-3">{{ thread.name }}</h1>
        </div>
      </div>
      <div class="level-right">
        <div class="field is-horizontal has-addons">
          <div class="field-label is-normal">
            <label class="label">Page</label>
          </div>
          <div class="field-body">
            <p class="control">
              <a href="#" class="button" :disabled="page === 1" @click="prevPage()">
                <ion-icon name="chevron-back-outline"></ion-icon>
                <span class="is-sr-only">Previous page</span>
              </a>
            </p>
            <p class="control">
              <input style="width: 5em" type="number" class="input" v-model="page">
            </p>
            <p class="control">
              <a href="#" class="button" :disabled="posts.length === 0" @click="nextPage()">
                <ion-icon name="chevron-forward-outline"></ion-icon>
                <span class="is-sr-only">Next page</span>
              </a>
            </p>
          </div>
        </div>
      </div>
    </nav>
    <article class="section">
      <forum-post v-for="post in sortedPosts" :key="post.id" :realm="realm" :post="post" />
    </article>
    <nav class="level is-marginless">
      <div class="level-left">
      </div>
      <div class="level-right">
        <div class="field is-horizontal has-addons">
          <div class="field-label is-normal">
            <label class="label">Page</label>
          </div>
          <div class="field-body">
            <p class="control">
              <a href="#" class="button" :disabled="page === 1" @click="prevPage()">
                <ion-icon name="chevron-back-outline"></ion-icon>
                <span class="is-sr-only">Previous page</span>
              </a>
            </p>
            <p class="control">
              <input style="width: 5em" type="number" class="input" v-model="page">
            </p>
            <p class="control">
              <a href="#" class="button" :disabled="posts.length === 0" @click="nextPage()">
                <ion-icon name="chevron-forward-outline"></ion-icon>
                <span class="is-sr-only">Next page</span>
              </a>
            </p>
          </div>
        </div>
      </div>
    </nav>
  </section>
</template>

<script lang="ts">
import { Component, Prop, Watch, Vue } from 'vue-property-decorator';
import ForumPost from '@/components/forum-post.vue';
import Realm from '@/dto/Realm';
import Theme from '@/dto/Theme';
import Thread from '@/dto/Thread';
import Post from '@/dto/Post';
import List from '@/dto/List';

@Component({
  components: {
    ForumPost,
  },
})
export default class VPosts extends Vue {
  @Prop() private realm!: Realm;

  theme: Theme|null = null;
  thread: Thread|null = null;

  posts: Post[] = [];

  page = 1;

  private async setTheme() {
    if (this.thread == null) {
      return;
    }
    try {
      const res = await fetch(`http://localhost:5000/forum/realms/${this.realm.toString()}/themes/${this.thread!.theme}`);
      const json = await res.json();
      this.theme = Theme.fromJSON(json);
    } catch {
      this.theme = null;
    }
    this.$emit('theme', this.theme);
  }

  private async setThread() {
    const id = parseInt(this.$route.params['thread']);
    if (isNaN(id)) {
      this.thread = null;
    } else {
      try {
        const res = await fetch(`http://localhost:5000/forum/realms/${this.realm.toString()}/threads/${id}`);
        const json = await res.json();
        this.thread = Thread.fromJSON(json);
      } catch {
        this.thread = null;
      }
    }
    this.$emit('thread', this.thread);
  }

  @Watch('page')
  private async fetchPosts() {
    if (this.theme == null || this.thread == null) {
      return;
    }
    const res = await fetch(`http://localhost:5000/forum/realms/${this.realm.toString()}/themes/${this.theme.hfid}/threads/${this.thread.hfid}/posts?offset=${this.offset}&limit=10`);
    const json = await res.json();
    const posts = List.fromJSON(Post, json).list;
    if (posts.length === 0) {
      return this.prevPage();
    }
    this.posts = posts;
  }

  beforeMount() {
    this.setThread().then(() => this.setTheme()).then(() => this.fetchPosts());
  }

  get offset() {
    return (this.page - 1) * 10;
  }

  prevPage() {
    if (this.page > 1) {
      --this.page;
    }
  }

  nextPage() {
    if (this.posts.length > 0) {
      ++this.page;
    }
  }

  get sortedPosts() {
    return [...this.posts].sort((a, b) => a.createdAt < b.createdAt ? -1 : 1);
  }
}
</script>
