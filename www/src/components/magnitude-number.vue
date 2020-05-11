<template>
  <p>
    {{ this.shortValue }}{{ this.prefix }}
  </p>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

  interface Scale {
    prefix: string;
    m: number;
  }

@Component
export default class ThemeRow extends Vue {
  @Prop() private value!: number;

  private getScale(): Scale {
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

  private scale: Scale = this.getScale();

  get prefix() {
    return this.scale.prefix;
  }

  get shortValue() {
    if (this.scale.prefix === '') {
      return this.value;
    }
    const float = this.value / this.scale.m;
    const fraction = float * 10 % 10 < 1 ? 0 : 1;
    return (this.value / this.scale.m).toFixed(fraction);
  }
}
</script>
