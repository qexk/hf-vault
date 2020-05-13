<template>
  <p>
    {{ this.shortValue }}{{ this.prefix }}
  </p>
</template>

<script lang="ts">
import { Component, Prop, Watch, Vue } from 'vue-property-decorator';

interface Scale {
  prefix: string;
  m: number;
}

@Component
export default class ThemeRow extends Vue {
  @Prop() private value!: number;

  @Watch('value')
  private get scale(): Scale {
    if (this.value >= 1e15) {
      return { prefix: 'P', m: 1e15 };
    } else if (this.value >= 1e12) {
      return { prefix: 'T', m: 1e12 };
    } else if (this.value >= 1e9) {
      return { prefix: 'G', m: 1e9 };
    } else if (this.value >= 1e6) {
      return { prefix: 'M', m: 1e6 };
    } else if (this.value >= 1e3) {
      return { prefix: 'k', m: 1e3 };
    } else {
      return { prefix: '', m: 1 };
    }
  }

  get prefix() {
    return this.scale.prefix;
  }

  get shortValue() {
    if (this.scale.prefix === '') {
      return this.value;
    }
    const float = this.value / this.scale.m;
    if (float * 10 % 10 < 1) {
      return Math.trunc(float);
    } else {
      return (Math.trunc(float * 10)) / 10;
    }
  }
}
</script>
