<template>
  <section class="section">
    <nav v-if="thread != null" class="level is-marginless">
      <div class="level-left">
        <div class="level-item">
          <h1 class="title is-3">{{ thread.name }}</h1>
        </div>
      </div>
      <div class="level-right">
        <nav-page v-model="page" min="1" :max="maxPage" />
      </div>
    </nav>
    <article class="section">
      <forum-post v-for="post in sortedPosts" :key="post.id" :realm="realm" :post="post" />
    </article>
    <nav v-if="thread != null" class="level is-marginless">
      <div class="level-left">
      </div>
      <div class="level-right">
        <nav-page v-model="page" min="1" :max="maxPage" />
      </div>
    </nav>
  </section>
</template>

<script lang="ts">
import { Component, Prop, Watch, Vue } from 'vue-property-decorator';
import ForumPost from '@/components/forum-post.vue';
import NavPage from '@/components/nav-page.vue';
import Realm from '@/dto/Realm';
import Theme from '@/dto/Theme';
import Thread from '@/dto/Thread';
import Post from '@/dto/Post';
import List from '@/dto/List';

const OFFSET = 10;

@Component({
  components: {
    ForumPost,
    NavPage,
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
    this.posts = posts;
  }

  beforeMount() {
    this.setThread().then(() => this.setTheme()).then(() => this.fetchPosts());
  }

  get sortedPosts() {
    return [...this.posts].sort((a, b) => a.createdAt < b.createdAt ? -1 : 1);
  }

get offset() {
    return (this.page - 1) * 10;
  }

  get maxPage() {
    return Math.ceil(this.thread!.posts / OFFSET);
  }
}
</script>
