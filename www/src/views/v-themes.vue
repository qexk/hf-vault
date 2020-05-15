<template>
  <section class="section">
    <theme-row v-for="theme in sortedThemes" :key="theme.hfid" :theme="theme" />
  </section>
</template>

<script lang="ts">
import { Component, Prop, Watch, Vue } from 'vue-property-decorator';
import ThemeRow from '@/components/theme-row.vue';
import Realm from '@/dto/Realm';
import Theme from '@/dto/Theme';
import List from '@/dto/List';

@Component({
  components: {
    ThemeRow,
  },
})
export default class VThemes extends Vue {
  @Prop() private realm!: Realm;

  themes: Theme[] = [];

  @Watch('realm')
  private async fetchThemes() {
    const res = await fetch(`http://localhost:5000/forum/realms/${this.realm.toString()}/themes`);
    const json = await res.json();
    this.themes = List.fromJSON(Theme, json).list;
  }

  get sortedThemes() {
    return [...this.themes].sort((a, b) => a.hfid - b.hfid);
  }

  beforeMount() {
    this.fetchThemes();
    this.$emit('theme', null);
    this.$emit('thread', null);
  }
}
</script>
