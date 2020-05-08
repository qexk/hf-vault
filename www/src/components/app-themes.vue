<template>
  <article>
    {{ test }}
  </article>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import Realm from '@/dto/Realm';

@Component
export default class AppThemes extends Vue {
  @Prop() private realm!: Realm;

  test = 'vide';

  private async fetchThemes() {
    const res = await fetch(`http://localhost:5000/forum/realms/${this.realm.toString()}/themes`);
    const json = await res.json();
    this.test = json;
  }

  beforeMount() {
    this.fetchThemes();
  }
}
</script>
