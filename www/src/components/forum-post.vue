<template>
  <article class="media">
    <a :href="authorUrl" class="media-left">
      <figure>
        <p class="image is-64x64">
          <img class="is-rounded" :src="avatarUrl" />
        </p>
      </figure>
      <p>
        <small><strong>{{ post.authorName }}</strong></small>
      </p>
    </a>
    <div class="media-content">
      <section class="message is-info">
        <div class="message-body">
          <small class="tag is-info is-light is-pulled-right">{{ post.createdAt.toFormat('ff') }}</small>
          <br />
          <div class="content">
            <span v-html="post.message"></span>
          </div>
        </div>
      </section>
    </div>
  </article>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import Realm from "@/dto/Realm";
import Post from "@/dto/Post";

const colors = [
  '00d1b2',
  '3273dc',
  '209cee',
  '48c774',
  'ffdd57',
  'ff3860',
]

function hashCode(s: string) {
    let hash = 0;
    if (s.length == 0) {
        return hash;
    }
    for (let i = 0; i < s.length; i++) {
        const char = s.charCodeAt(i);
        hash = ((hash << 5) - hash) + char;
        hash = hash & 0xFFFFFF;
    }
    return hash;
}

@Component({
  components: {}
})
export default class VThemes extends Vue {
  @Prop() private realm!: Realm;
  @Prop() private post!: Post;

  authorUrl = `${this.realm.host}/user.html/${this.post.author}`;

  get avatarUrl() {
    const color = colors[hashCode(this.post.authorName) % colors.length];
    return `https://eu.ui-avatars.com/api/?format=svg&background=${color}&color=FFF&name=${this.post.authorName}`;
  }
}
</script>

<style>
article.media .media-left {
  width: 6rem;
  display: flex;
  flex-direction: column;
  align-items: center;
}

article.media .media-content {
  position: relative;
}

article.media .media-content::before {
  content: "";
  display: inline-block;
  border-top: 10px solid transparent;
  border-bottom: 10px solid transparent;
  border-right: 10px solid #3298dc;
  position: absolute;
  left: -.5rem;
  top: 1rem;
}

.message-body {
  background-color: #d4eafc;
}

img[src^="/img/items"] {
  vertical-align: middle;
}

cite {
  display: block;
  background-color: #FFFFFF77;
  border-left: 5px solid #3298dc33;
  padding: 1.25em 1.5em;
  margin: 1.25em 0;
}
</style>
