import Realm from './Realm';

export default class Theme {
  constructor(
    public readonly name: string,
    public readonly hfid: number,
    public readonly realm: Realm,
    public readonly threads: number,
  ) { }

  static fromJSON(o: any): Theme|null {
    const realm = o.realm === void 0 ? null : Realm.fromJSON(o.realm);
    if (
      realm != null
      && o.name !== void 0 && typeof o.name === 'string'
      && o.hfid !== void 0 && typeof o.hfid === 'number'
      && o.threads !== void 0 && typeof o.threads === 'number'
    ) {
      return new Theme(o.name, o.hfid, realm, o.threads);
    }
    return null;
  }

  toJSON() {
    return {
      name: this.name,
      hfid: this.hfid,
      realm: this.realm.toJSON(),
      threads: this.threads,
    };
  }
}
