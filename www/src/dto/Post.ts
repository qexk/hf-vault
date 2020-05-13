import { DateTime } from 'luxon';

export default class Post {
  constructor(
    public readonly id: number,
    public readonly author: number,
    public readonly authorName: string,
    public readonly createdAt: DateTime,
    public readonly message: string,
  ) { }

  static fromJSON(o: any): Post|null {
    const createdAt =
      o.createdAt === void 0
      ? null
      : DateTime.fromISO(o.createdAt, { zone: 'local' });
    if (
      createdAt != null
      && o.post !== void 0 && typeof o.post === 'number'
      && o.author !== void 0 && typeof o.author === 'number'
      && o.authorName !== void 0 && typeof o.authorName === 'string'
      && o.message !== void 0 && typeof o.message === 'string'
    ) {
      return new Post(
        o.post,
        o.author,
        o.authorName,
        createdAt,
        o.message,
      );
    }
    return null;
  }

  toJSON() {
    return {
      post: this.id,
      author: this.author,
      authorName: this.authorName,
      createdAt: this.createdAt,
      message: this.message,
    };
  }
}
