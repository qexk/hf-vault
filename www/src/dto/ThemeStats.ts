import { DateTime } from 'luxon';

export default class ThemeStats {
  constructor(
    public readonly posts: number,
    public readonly threads: number,
    public readonly lastUpdate: DateTime,
    public readonly thread: number,
    public readonly threadName: string,
    public readonly author: number,
    public readonly authorName: string,
  ) { }

  static fromJSON(o: any): ThemeStats|null {
    const lastUpdate =
      o.lastUpdate === void 0
      ? null
      : DateTime.fromISO(o.lastUpdate, { zone: 'local' });
    const posts = parseInt(o.posts);
    const threads = parseInt(o.threads);
    if (
      lastUpdate != null
      && !isNaN(posts)
      && !isNaN(threads)
      && o.thread !== void 0 && typeof o.thread == 'number'
      && o.threadName !== void 0 && typeof o.threadName == 'string'
      && o.author !== void 0 && typeof o.author == 'number'
      && o.authorName !== void 0 && typeof o.authorName == 'string'
    ) {
      return new ThemeStats(
        posts,
        threads,
        lastUpdate,
        o.thread,
        o.threadName,
        o.author,
        o.authorName,
      );
    }
    return null;
  }

  toJSON() {
    return {
      posts: this.posts,
      threads: this.threads,
      lastUpdate: this.lastUpdate,
      thread: this.thread,
      threadName: this.threadName,
      author: this.author,
      authorName: this.authorName,
    };
  }

  static empty: Readonly<ThemeStats> = Object.freeze(
    new ThemeStats(0, 0, DateTime.fromMillis(0), 0, 'loading...', 0, 'loading...')
  );
}
