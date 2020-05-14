import Realm from './Realm';
import { DateTime } from 'luxon';

export default class Thread {
  constructor(
    public readonly hfid: number,
    public readonly realm: Realm,
    public readonly theme: number,
    public readonly posts: number,
    public readonly author: number,
    public readonly authorName: string,
    public readonly createdAt: DateTime,
    public readonly updatedAt: DateTime,
    public readonly name: string,
    public readonly open: boolean,
    public readonly sticky: boolean,
  ) { }

  static fromJSON(o: any): Thread|null {
    const realm = o.realm === void 0 ? null : Realm.fromJSON(o.realm);
    const createdAt =
      o.createdAt === void 0
      ? null
      : DateTime.fromISO(o.createdAt, { zone: 'local' });
    const updatedAt =
      o.updatedAt === void 0
      ? null
      : DateTime.fromISO(o.updatedAt, { zone: 'local' });
    if (
      createdAt != null
      && updatedAt != null
      && realm != null
      && o.thread !== void 0 && typeof o.thread === 'number'
      && o.theme !== void 0 && typeof o.theme === 'number'
      && o.posts !== void 0 && typeof o.posts === 'number'
      && o.author !== void 0 && typeof o.author === 'number'
      && o.authorName !== void 0 && typeof o.authorName === 'string'
      && o.name !== void 0 && typeof o.name === 'string'
      && o.open !== void 0 && typeof o.open === 'boolean'
      && o.sticky !== void 0 && typeof o.sticky === 'boolean'
    ) {
      return new Thread(
        o.thread,
        realm,
        o.theme,
        o.posts,
        o.author,
        o.authorName,
        createdAt,
        updatedAt,
        o.name,
        o.open,
        o.sticky);
    }
    return null;
  }

  toJSON() {
    return {
      thread: this.hfid,
      realm: this.realm,
      theme: this.theme,
      posts: this.posts,
      author: this.author,
      authorName: this.authorName,
      createdAt: this.createdAt,
      name: this.name,
      open: this.open,
      sticky: this.sticky,
    };
  }
}
