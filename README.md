# GitHub Actions Check Updates
![YAML Lint](https://github.com/fabasoad/ghacu/workflows/YAML%20Lint/badge.svg) [![Total alerts](https://img.shields.io/lgtm/alerts/g/fabasoad/ghacu.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/fabasoad/ghacu/alerts/) [![Maintainability](https://api.codeclimate.com/v1/badges/261a8a73037043dfde09/maintainability)](https://codeclimate.com/github/fabasoad/ghacu/maintainability) [![License: MIT](https://img.shields.io/badge/License-MIT-lightgrey.svg)](https://opensource.org/licenses/MIT)
## Description
CLI tool that checks versions of GitHub Actions that used in a repository.
## Commands
```bash
> ghacu --help
ghacu 1.0.0
Copyright (C) 2020 ghacu

  -r, --repository    Path to the root of a project.

  -u, --upgrade       Upgrade versions to the latest one.

  --help              Display this help screen.

  --version           Display version information.
```
## Example
```bash
PS C:\\Projects\\business-card> ghacu
> CI (.github\\workflows\\ci.yml)
actions/checkout                 v1  »  v2.1.0
bahmutov/npm-install             v1  »  v1.4.0
crazy-max/ghaction-github-pages  v1  »  v1.3.0

> Milestone Closure (.github\\workflows\\release-notes.yml)
actions/checkout                          master  »  v2.1.0
decathlon/release-notes-generator-action   2.0.0  »  v2.0.1

Run ghacu -u to upgrade actions.
```