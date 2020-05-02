CREATE TYPE "realm" AS ENUM (
  'FR',
  'EN',
  'ES'
);

CREATE TABLE "theme" (
  "id" serial NOT NULL,
  "name" varchar NOT NULL,
  PRIMARY KEY ("id")
);

CREATE TABLE "hf_theme" (
  "hfid" int NOT NULL,
  "realm" realm NOT NULL,
  "theme" int NOT NULL,
  PRIMARY KEY ("hfid", "realm")
);

CREATE TABLE "thread" (
  "id" serial NOT NULL,
  "author" int NOT NULL,
  "created_at" timestamp NOT NULL,
  "updated_at" timestamp NOT NULL,
  "theme" int NOT NULL,
  "name" varchar NOT NULL,
  "open" boolean NOT NULL,
  "sticky" boolean NOT NULL,
  PRIMARY KEY ("id")
);

CREATE TABLE "hf_thread" (
  "hfid" int NOT NULL,
  "realm" realm NOT NULL,
  "thread" int NOT NULL,
  PRIMARY KEY ("hfid", "realm")
);

CREATE TABLE "post" (
  "id" serial NOT NULL,
  "author" int NOT NULL,
  "created_at" timestamp NOT NULL,
  "message" text NOT NULL,
  "thread" int NOT NULL,
  PRIMARY KEY ("id")
);

CREATE TABLE "hf_post" (
  "realm" realm NOT NULL,
  "post" int NOT NULL,
  PRIMARY KEY ("realm", "post")
);

CREATE TABLE "author" (
  "id" serial NOT NULL,
  "name" varchar NOT NULL,
  PRIMARY KEY ("id")
);

CREATE TABLE "hf_user" (
  "hfid" int NOT NULL,
  "realm" realm NOT NULL,
  "author" int NOT NULL,
  PRIMARY KEY ("hfid", "realm")
);

CREATE UNIQUE INDEX ON "hf_theme" ("theme");
CREATE UNIQUE INDEX ON "hf_thread" ("thread");
CREATE UNIQUE INDEX ON "hf_post" ("post");
CREATE UNIQUE INDEX ON "hf_user" ("author");

ALTER TABLE "hf_theme" ADD FOREIGN KEY ("theme") REFERENCES "theme" ("id");
ALTER TABLE "thread" ADD FOREIGN KEY ("theme") REFERENCES "theme" ("id");
ALTER TABLE "hf_thread" ADD FOREIGN KEY ("thread") REFERENCES "thread" ("id");
ALTER TABLE "post" ADD FOREIGN KEY ("thread") REFERENCES "thread" ("id");
ALTER TABLE "post" ADD FOREIGN KEY ("author") REFERENCES "author" ("id");
ALTER TABLE "hf_post" ADD FOREIGN KEY ("post") REFERENCES "post" ("id");
ALTER TABLE "hf_user" ADD FOREIGN KEY ("author") REFERENCES "author" ("id");
