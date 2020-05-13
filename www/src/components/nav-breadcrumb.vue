<template>
  <nav class="breadcrumb" aria-label="breadcrumbs">
    <ul>
      <li :class="{ 'is-active': theme == null}"><router-link to="/">{{ realm.flag }} Forum</router-link></li>
      <template v-if="theme != null">
        <li :class="{ 'is-active': thread == null}"><router-link :to="'/theme/' + theme.hfid">{{ theme.name }}</router-link></li>
      </template>
    </ul>
  </nav>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import Realm from '@/dto/Realm';
import Theme from '@/dto/Theme';
import Thread from '@/dto/Thread';

@Component
export default class NavBreadcrumb extends Vue {
  @Prop() realm!: Realm;
  @Prop() theme!: Theme|null;
  @Prop() thread!: Thread|null;
}
</script>
