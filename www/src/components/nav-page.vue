<template>
  <nav class="pagination is-centered" role="navigation" aria-label="pagination">
    <a @click="prevPage" :disabled="page <= min" :aria-disabled="page <= min" class="pagination-previous">Previous</a>
    <a @click="nextPage" :disabled="page >= max" :aria-disabled="page >= max" class="pagination-next">Next</a>
    <ul class="pagination-list">
      <li>
        <a
            class="pagination-link"
            :class="{ 'is-current': page <= min }"
            :disabled="page <= min"
            :aria-disabled="page <= min"
            :aria-label="'Goto page ' + min"
            @click="goto(min|0)"
            >
          {{ min }}
        </a>
      </li>
      <li>
        <span class="pagination-ellipsis">&hellip;</span>
      </li>
      <li :class="{ 'is-invisible': (page - 1) < min }">
        <a
            class="pagination-link"
            :aria-disabled="(page - 1) < min"
            :aria-label="'Goto page ' + (page - 1)"
            @click="goto(page - 1)"
            >
          {{ page - 1 }}
        </a>
      </li>
      <li>
        <p class="pagination-link is-current control">
          <input
              type="number"
              step="1"
              class="input"
              :value="page"
              @input="inputPage"
              >
        </p>
      </li>
      <li :class="{ 'is-invisible': (page + 1) > max }">
        <a
            class="pagination-link"
            :aria-disabled="(page + 1) > max"
            :aria-label="'Goto page ' + (page + 1)"
            @click="goto(page + 1)"
            >
          {{ page + 1 }}
        </a>
      </li>
      <li>
        <span class="pagination-ellipsis">&hellip;</span>
      </li>
      <li>
        <a
            class="pagination-link"
            :class="{ 'is-current': page >= max }"
            :disabled="page >= max"
            :aria-disabled="page >= max"
            :aria-label="'Goto page ' + max"
            @click="goto(max|0)"
            >
          {{ max }}
        </a>
      </li>
    </ul>
  </nav>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component
export default class NavPage extends Vue {
  @Prop() private readonly value!: number;
  @Prop() private readonly min!: number;
  @Prop() private readonly max!: number;

  private page = this.value;

  private prevPage() {
    if (this.page > this.min) {
      this.goto(--this.page);
    }
  }

  private nextPage() {
    if (this.page < this.max) {
      this.goto(++this.page);
    }
  }

  private goto(page: number) {
    this.$emit('input', (this.page = page|0));
  }

  private inputPage(ev: Event) {
    const page = parseInt((ev.target as HTMLInputElement).value);
    if (isNaN(page) || page < this.min) {
      this.goto(this.min);
    } else if (page > this.max) {
      this.goto(this.max);
    } else {
      this.goto(page);
    }
  }
}
</script>

<style scoped>
input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

input[type=number] {
  -moz-appearance: textfield;
  width: 3em;
  text-align: center;
}
</style>
