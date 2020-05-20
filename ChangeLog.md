# Changelog

All notable changes to this project will be documented in this file. See [NEWS.md](./NEWS.md) for a less technical changelog.

The format is based on [Keep a Changelog], and this project adheres to [Semantic Versioning].

### [1.1.0] – 2020-05-20

#### Added

- Two subprojects used for testing the data:
  + `./api/`: sample F# REST API serving the scraped forum data.
  + `./www/`: sample Vue.js webapp.

#### Changed

- `v1.theme-description.sql`: New database migration.
- The theme’s descriptions are now scraped and stored.

### [1.0.0] – 2020-05-05

#### Added

- The README!
- The project is now stable.

### [0.1.0] – 2020-05-04

#### Added

- Logging.
- The data is now stored in the database.

#### Fixed

- The detection of the last page of a theme was utterly broken, now it stops at the last page and does not continue indefinitely.

### [0.0.0] – 2020-04-15

#### Added

- Basic scraper. Does not store anything.

[Keep a Changelog]: <https://keepachangelog.com/en/1.0.0/>
[Semantic Versioning]: <https://semver.org/spec/v2.0.0.html>
[0.0.0]: <https://github.com/Aksamyt/hf-vault/releases/tag/v0.0.0>
[0.1.0]: <https://github.com/Aksamyt/hf-vault/compare/v0.0.0...v0.1.0>
[1.0.0]: <https://github.com/Aksamyt/hf-vault/compare/v0.0.0...v1.0.0>
