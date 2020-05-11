<template>
  <fieldset :disabled="this.state == 0">
    <div class="field is-horizontal">
      <div class="field-body">
        <div class="field is-narrow has-addons">
          <div class="control">
            <div
                class="select is-fullwidth"
                :class="{
                  'is-loading': this.state == 0,
                }"
                >
              <select>
                <option
                    v-for="option in options"
                    :key="option.text"
                    :value="option.value"
                    >
                  {{ option.text }}
                </option>
              </select>
            </div>
          </div>
          <div class="control">
            <a class="button is-info" @click="fetchRealms()">
              Refresh realms
            </a>
          </div>
        </div>
      </div>
    </div>
  </fieldset>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import Realm from '@/dto/Realm';
import List from '@/dto/List';

interface Option {
  text: string;
  value: Realm;
}

enum State {
  Loading,
  Ready,
  FirstTime,
}

@Component
export default class RealmSelect extends Vue {
  options: Option[] = [];
  state: State = State.FirstTime;

  private emitEvent(dto: List<Realm>) {
    const realms = dto.list;
    if (realms.length > 0) {
      this.$emit('realm', realms[0]);
    } else {
      this.$emit('realm', null);
    }
  }

  private async fetchRealms() {
    if (this.state == State.Loading) {
      return;
    }
    this.state = State.Loading;
    const res = await fetch('http://localhost:5000/forum/realms');
    const json = await res.json();
    const realms = List.fromJSON(Realm, json);
    this.options = realms.list.map(r => {
      return {
        text: r.flag,
        value: r,
      }
    });
    this.emitEvent(realms);
    this.state = State.Ready;
  }

  beforeMount() {
    this.fetchRealms();
  }
}
</script>
