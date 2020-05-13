import { DateTime } from 'luxon';

export default class ThreadStats {
  constructor(
    public readonly posts: number,
    public readonly lastUpdate: DateTime,
    public readonly author: number,
    public readonly authorName: string,
  ) { }

  static fromJSON(o: any): ThreadStats|null {
    const lastUpdate =
      o.lastUpdate === void 0
      ? null
      : DateTime.fromISO(o.lastUpdate, { zone: 'local' });
    const posts = parseInt(o.posts);
    if (
      lastUpdate != null
      && !isNaN(posts)
      && o.author !== void 0 && typeof o.author == 'number'
      && o.authorName !== void 0 && typeof o.authorName == 'string'
    ) {
      return new ThreadStats(
        posts,
        lastUpdate,
        o.author,
        o.authorName,
      );
    }
    return null;
  }

  toJSON() {
    return {
      posts: this.posts,
      lastUpdate: this.lastUpdate,
      author: this.author,
      authorName: this.authorName,
    };
  }

  static empty: Readonly<ThreadStats> = Object.freeze(
    new ThreadStats(0, DateTime.fromMillis(0), 0, 'loading...')
  );
}
