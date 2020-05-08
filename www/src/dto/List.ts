export default class List<Dto extends { toJSON(): any }> {
  constructor(
    public list: Dto[],
  ) {}

  static fromJSON<Dto>(dto: { new(...args: any[]): Dto }, o: any): List<Dto>|null {
    if (Array.isArray(o)) {
      const dtos = new Array(o.length);
      for (let i = 0; i < o.length; ++i) {
        dtos[i] = dto.fromJSON(o[i]);
        if (dtos[i] == null) {
          return null;
        }
      }
      return new List(dtos);
    }
    return null;
  }

  toJSON() {
    return this.list.map(o => o.toJSON());
  }
}
