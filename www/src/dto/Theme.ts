import Realm from './Realm';

export default class Theme {
  constructor(
    public readonly name: string,
    public readonly hfid: number,
    public readonly realm: Realm,
  ) { }

  static fromJSON(o: any): Theme|null {
    const realm = o.realm === void 0 ? null : Realm.fromJSON(o.realm);
    if (o.name !== void 0 && typeof o.name === 'string') {
      if (o.hfid !== void 0 && typeof o.hfid === 'number') {
        if (realm != null) {
          return new Theme(o.name, o.hfid, realm);
        }
      }
    }
    return null;
  }

  toJSON() {
    return {
      name: this.name,
      hfid: this.hfid,
      realm: this.realm.toJSON(),
    };
  }
}
