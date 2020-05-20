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
              <select @input="selectRealm">
                <option
                    v-for="(option, ix) in options"
                    :key="option.text"
                    :value="ix"
                    >
                  {{ option.text }}
                </option>
              </select>
            </div>
          </div>
          <div class="control">
            <a class="button is-info" @click="fetchRealms">
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
    if (realms.list.length > 0) {
      this.$emit('realm', realms.list[0]);
    } else {
      this.$emit('realm', null);
    }
    this.state = State.Ready;
  }

  beforeMount() {
    this.fetchRealms();
  }

  selectRealm(ev: Event) {
    const ix = +(ev.target as HTMLSelectElement).value;
    const realm = this.options[ix].value;
    this.$emit('realm', realm || null);
  }
}
</script>
