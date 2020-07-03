# GitHub Actions Check Updates
![CI (master)](https://github.com/fabasoad/ghacu/workflows/CI%20(master)/badge.svg) ![CI (latest)](https://github.com/fabasoad/ghacu/workflows/CI%20(latest)/badge.svg) ![YAML Lint](https://github.com/fabasoad/ghacu/workflows/YAML%20Lint/badge.svg) ![PowerShell Lint](https://github.com/fabasoad/ghacu/workflows/PowerShell%20Lint/badge.svg) [![Total alerts](https://img.shields.io/lgtm/alerts/g/fabasoad/ghacu.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/fabasoad/ghacu/alerts/) [![Maintainability](https://api.codeclimate.com/v1/badges/261a8a73037043dfde09/maintainability)](https://codeclimate.com/github/fabasoad/ghacu/maintainability) [![codecov](https://codecov.io/gh/fabasoad/ghacu/branch/master/graph/badge.svg)](https://codecov.io/gh/fabasoad/ghacu) ![License: MIT](https://img.shields.io/github/license/fabasoad/ghacu)
## Description
CLI tool that checks versions of GitHub Actions that used in a repository.
## Installation
> .NET Core should be installed on your machine as a prerequisite.
### Windows
1. Install using:
    1. Installer:
    * _x86_: https://github.com/fabasoad/ghacu/releases/download/v1.1.4/ghacu-1.1.4-win-x86.exe
    * _x64_: https://github.com/fabasoad/ghacu/releases/download/v1.1.4/ghacu-1.1.4-win-x64.exe
    2. Compressed package:
    * _x86_: https://github.com/fabasoad/ghacu/releases/download/v1.1.4/ghacu-1.1.4-win-x86.tgz
    * _x64_: https://github.com/fabasoad/ghacu/releases/download/v1.1.4/ghacu-1.1.4-win-x64.tgz
2. [Add application path](https://stackoverflow.com/questions/44272416/how-to-add-a-folder-to-path-environment-variable-in-windows-10-with-screensho) to _PATH_ environment variable.
### Linux
```bash
cd ~ && wget https://github.com/fabasoad/ghacu/releases/download/v1.1.4/ghacu-1.1.4-linux-x64.tgz
tar -xvf ghacu-1.1.4-linux-x64.tgz
PATH=$PATH:~/ghacu-1.1.4-linux-x64
```
### MacOS
```bash
cd ~ && wget https://github.com/fabasoad/ghacu/releases/download/v1.1.4/ghacu-1.1.4-osx-x64.tgz
tar -xvf ghacu-1.1.4-osx-x64.tgz
PATH=$PATH:~/ghacu-1.1.4-osx-x64
```
> Examples above use version `1.1.4` but you can use any version from the [releases](https://github.com/fabasoad/ghacu/releases) page. Latest version is preferable.
## Commands
```bash
> ghacu --help
ghacu 1.1.4
Copyright (C) 2020 ghacu

  -r, --repository    Path to the root of a project.

  -u, --upgrade       Upgrade versions to the latest one.

  --help              Display this help screen.

  --version           Display version information.
```
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

Run ghacu -u to upgrade actions.
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
3. Run the following command:
```bash
docker run --rm -i -v "$PWD:/work" amake/innosetup inno/ghacu-win-{x64|x86}.iss
```
> Please take a look at [this](https://gist.github.com/amake/3e7194e5e61d0e1850bba144797fd797) page for more details.
#### Windows
1. Download and install [Inno Setup](https://jrsoftware.org/isinfo.php).
2. Open `inno\ghacu-win-{x64|x86}.iss` file with _Inno Setup_.
3. Increase version.
4. Run _Build_ -> _Compile_.