<p align="center">
  <a href="http://hammerfest.fr">
    <img src="https://upload.wikimedia.org/wikipedia/fr/d/d9/Les_Cavernes_de_Hammerfest_Logo.png" alt="Logo hammerfest" width="400" />
  </a>
  <a href="https://eternal-twin.net">
    <img src="https://eternal-twin.net/assets/banner.png" alt="Eternal Twin project"/>
  </a>
  <h3 align="center"><strong>HF Vault</strong></h3>
  <p align="center">A crawler for the <a href="http://hammerfest.fr/forum.html">Hammerfest</a> forum.</p>
</p>

## Table of Contents

* [About](#about)
* [Getting started](#getting-started)
  * [Prerequisites](#prerequisites)
  * [Usage](#usage)
* [Compiling](#compiling)
  * [Prerequisites](#prerequisites-1)
  * [Building](#building)
* [Roadmap](#roadmap)
* [Contributing](#contributing)
  * [Overview](#overview)
  * [Libraries used](#libraries-used)
* [Acknowledgements](#acknowledgements)
* [License](#license)

## About

On the 27th of March 2020, [Motion Twin] published a [heartbreaking message] on their Twinoid platform. As Flash is nearing its end of support, they will gradually close and remove their older games from the web.

Moments after the official announcement, hundreds of players from multiple countries united to save these games and memories. Thus was born [Eternal-Twin], a project dedicated to save the communities and memories of this part of the video game history, led by [Demurgos] and MrPapy.

HF Vault is a crawler for archiving the [Hammerfest] forum. It scrapes each and every page and stores the data in a PostgreSQL database, while computing the right year for each forum post.

This project is for documentation purpose only. The forum has already been archived. Please don’t abuse the poor servers from 2006.

## Getting started

### Prerequisites

- [.NET Core 3.1 runtime](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- PostgreSQL ⩾ 11 database initialised with the latest [migration](./migration)
- There must be a `config.json` file in your CWD configured as:
  ```json
  {
    "ConnectionStrings": {
      "hf-vault": <fill in your connection string>
    }
  }
  ```

### Usage

#### Synopsis
```
dotnet <path-to-hf-vault.dll> --help
dotnet <path-to-hf-vault.dll> [-r REALM]
```
Or if running the SDK in the project’s root:
```
dotnet run -- --help
dotnet run -- [-r REALM]
```

#### Options

Mandatory arguments to long options are mandatory for short options too.

##### `-r, --realm=REALM`
Tell the scraper which host to use.

| Value| Host                       |
|------|----------------------------|
| `FR` | "http://www.hammerfest.fr" |
| `EN` | "http://www.hfest.net"     |
| `ES` | "http://www.hammerfest.es" |

##### `-h, --help`
Display the usage and exit.

## Compiling

### Prerequisites

- [.NET Core 3.1 runtime](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- PostgreSQL ⩾ 11 database initialised with the latest [migration](./migration)
- Add the connection string to [`./src/development.settings.json`](./src/development.settings.json)
- Copy [`./src/development.settings.json`](./src/development.settings.json) to the project root (next to the README) as `config.json`.

Your project directory should look like this:
```
.
├── AUTHORS.md
├── ChangeLog.md
├── config.json
├── COPYING
├── hf-vault.fsproj
├── migration
│   └── ...
├── NEWS.md
├── README.md
└── src
    ├── development.settings.json
    ├── Dto
    │   └── ...
    ├── Forum
    │   └── ...
    └── ...
```

### Building

To build the project (by default to `./bin/Debug/netcoreapp3.0/hf-vault.dll`) run:
```
dotnet build
```
Or just type
```
dotnet run -- --help
```
To build and run the project. If it shows the usage and some log lines, it’s all good!

> ```diff
> ! Warning !
> The build will fail if the database hasn’t been properly initialised.
> ```

## Roadmap

See the [open issues] for a list of proposed features (and known issues).

## Contributing

Pull requests are welcome! Tests are not mandatory, but a good typed representation of the business logic is always better.

1. Fork the project
2. Switch to the develop branch: `git checkout develop`
3. Create your feature/hotfix branch: `git checkout -b feature/be-faster` or `git checkout -b hotfix/stop-choking-on-nulls`
4. Commit your changes following these [commit guidelines]
5. Push your branch
6. Open a Pull Request

### Overview

- [`./src`](./src): all the source files.
- [`./src/HfVault.fs`](./src/HfVault.fs): entry point and crawler implementation.
- [`./src/Forum`](./src/Forum): these modules represent a hierarchical layer of the forum (root > theme > thread > post), every module provides a type and a `load` function.
- [`./src/Dto`](./src/Dto): these modules represent the logic using types and help validate the data before inserting it into the database.

### Libraries used

- [HtmlAgilityPack]\: parsing HTML
- [Aether]\: optics
- [FSharp.Data.Npgsql]\: PostgreSQL type provider
- [Logary]\: logging
- [Hopac]\: mainly used for logging, the rest is classic F# Async

## Acknowledgements

- [Motion Twin] whose awesome games have created great moments in all of France’s middle schools
- [Eternalfest] for keeping this game alive
- [Eternal-Twin] for keeping the memories alive

## License

Distributed under the [GNU General Public License v3].

[Motion Twin]: https://motion-twin.com
[heartbreaking message]: https://twinoid.com/fr/article-fr/6437/twinoid-et-les-jeux-web-de-motion-twin-evoluent
[Eternal-Twin]: https://eternal-twin.net
[Demurgos]: https://github.com/demurgos
[Hammerfest]: http://hfest.net/
[open issues]: https://github.com/Aksamyt/hf-vault/issues
[commit guidelines]: https://chris.beams.io/posts/git-commit/
[HtmlAgilityPack]: https://html-agility-pack.net/documentation
[Aether]: https://xyncro.tech/aether/
[FSharp.Data.Npgsql]: https://github.com/demetrixbio/FSharp.Data.Npgsql
[Logary]: https://logary.tech
[Hopac]: https://github.com/Hopac/Hopac/blob/master/Docs/Programming.md
[Eternalfest]: https://eternalfest.net
[GNU General Public License v3]: https://www.gnu.org/licenses/gpl-3.0.html
