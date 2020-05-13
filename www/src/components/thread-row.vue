<template>
  <router-link :to="`/thread/${this.thread.thread}`">
    <article class="columns is-vcentered hf-thread">
      <div class="column">
        <h2 class="title is-4">
          {{ thread.name }}
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
    </article>
  </router-link>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import MagnitudeNumber from '@/components/magnitude-number.vue';
import Thread from '@/dto/Thread';

@Component({
  components: {
    MagnitudeNumber
  },
})
export default class ThreadRow extends Vue {
  @Prop() private thread!: Thread;

  hfurl = `${this.thread.realm.host}/forum.html/thread/${this.thread.hfid}/`;
  authorUrl = `${this.thread.realm.host}/user.html/${this.thread.author}`;
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
