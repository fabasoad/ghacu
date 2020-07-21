# GitHub Actions Check Updates
![CI (main)](https://github.com/fabasoad/ghacu/workflows/CI%20(main)/badge.svg) ![CI (latest)](https://github.com/fabasoad/ghacu/workflows/CI%20(latest)/badge.svg) ![YAML Lint](https://github.com/fabasoad/ghacu/workflows/YAML%20Lint/badge.svg) ![PowerShell Lint](https://github.com/fabasoad/ghacu/workflows/PowerShell%20Lint/badge.svg) [![Total alerts](https://img.shields.io/lgtm/alerts/g/fabasoad/ghacu.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/fabasoad/ghacu/alerts/) [![Maintainability](https://api.codeclimate.com/v1/badges/261a8a73037043dfde09/maintainability)](https://codeclimate.com/github/fabasoad/ghacu/maintainability) [![codecov](https://codecov.io/gh/fabasoad/ghacu/branch/main/graph/badge.svg)](https://codecov.io/gh/fabasoad/ghacu) ![License: MIT](https://img.shields.io/github/license/fabasoad/ghacu)
## Description
CLI tool that checks versions of GitHub Actions that used in a repository. Please read [documentation](https://github.com/fabasoad/ghacu/wiki) for more details.

## Commands
```bash
  --cache          (Default: Yes) Enable cache.

  --log-level      (Default: Error) Set log level. Possible values: Trace, Debug, Information, Warning, Error, Critical, None.

  --output-type    (Default: Color) Console output type. Possible values: Color, NoColor.

  --repository     Path to the root of a project.

  --token          GitHub token to work with actions repositories.

  --upgrade        Upgrade versions to the latest one.

  --help           Display this help screen.

  --version        Display version information.
```
All commands are optional and can be run by purpose.
### GitHub Token
There are 2 ways to pass GitHub token to _ghacu_:
1. Using `--token` parameter:
```bash
ghacu --token abc123 --repository "C:\Projects\business-card"
```
2. Defining `GHACU_GITHUB_TOKEN` environment variable:
```bash
export GHACU_GITHUB_TOKEN=abc123
ghacu --repository "C:\Projects\business-card"
```
> Program argument way takes precedence over the environment variable way. So the program looks at program argument first, if it's not present it looks at GHACU_GITHUB_TOKEN environment variable and if it's not present as well then ghacu will work as unauthenticated user.
## Example
```bash
PS C:\Projects\business-card> ghacu
> CI (.github\workflows\ci.yml)
actions/checkout                 v1  »  v2.1.0
bahmutov/npm-install             v1  »  v1.4.0
crazy-max/ghaction-github-pages  v1  »  v1.3.0

> Milestone Closure (.github\workflows\release-notes.yml)
actions/checkout                          master  »  v2.1.0
decathlon/release-notes-generator-action   2.0.0  »  v2.0.1

Run ghacu --upgrade to upgrade the actions.
```
## Dev section
### How to build an application
#### MacOS
```bash
brew cask install pwsh
pwsh ./build.ps1
```
#### Windows
```bash
.\build.ps1
```
### How to create MSI
#### MacOS
1. Open `inno/ghacu-win-{x64|x86}.iss` file in IDE.
2. Increase version.
3. Run the following command (bash):
```bash
docker run --rm -i -v "$PWD:/work" amake/innosetup inno/ghacu-win-{x64|x86}.iss
```
> Please take a look at [this](https://gist.github.com/amake/3e7194e5e61d0e1850bba144797fd797) page for more details.
#### Windows
1. Download and install [Inno Setup](https://jrsoftware.org/isinfo.php).
2. Open `inno\ghacu-win-{x64|x86}.iss` file with _Inno Setup_.
3. Increase version.
4. Run _Build_ -> _Compile_.
## Troubleshooting
### API rate limit exceeded for...
If you see such message it means that you [exceeded limit](https://developer.github.com/v3/#rate-limiting) of requests as unauthenticated user. For authenticated users rate limit is much bigger, so to solve this problem you need to pass GitHub token to _ghacu_. More information [here](#github-token).