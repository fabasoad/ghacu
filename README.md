<p align="center"><a href="https://github.com/fabasoad/ghacu"><img src="https://raw.githubusercontent.com/fabasoad/ghacu/feature/41/resources/ghacu-logo-png-img.png" alt="ghacu logo" height="120"/></a></p>
<h1 align="center">ghacu</h1>
<p align="center">Keep your GitHub Actions up-to-date.</p>

<p align="center">
	<a href="https://github.com/fabasoad/ghacu/actions?query=workflow%3A%22CI+%28main%29%22"><img src="https://github.com/fabasoad/ghacu/workflows/CI%20(main)/badge.svg" /></a>
	<a href="https://github.com/fabasoad/ghacu/actions?query=workflow%3A%22CI+%28latest%29%22"><img src="https://github.com/fabasoad/ghacu/workflows/CI%20(latest)/badge.svg" /></a>
	<a href="https://github.com/fabasoad/ghacu/actions?query=workflow%3A%22YAML+Lint%22"><img src="https://github.com/fabasoad/ghacu/workflows/YAML%20Lint/badge.svg" /></a>
	<a href="https://github.com/fabasoad/ghacu/actions?query=workflow%3A%22PowerShell+Lint%22"><img src="https://github.com/fabasoad/ghacu/workflows/PowerShell%20Lint/badge.svg" /></a>
	<a href="https://lgtm.com/projects/g/fabasoad/ghacu/alerts/"><img src="https://img.shields.io/lgtm/alerts/g/fabasoad/ghacu.svg?logo=lgtm&logoWidth=18" /></a>
	<a href="https://codeclimate.com/github/fabasoad/ghacu/maintainability"><img src="https://api.codeclimate.com/v1/badges/261a8a73037043dfde09/maintainability" /></a>
	<a href="https://codecov.io/gh/fabasoad/ghacu"><img src="https://codecov.io/gh/fabasoad/ghacu/branch/main/graph/badge.svg" /></a>
	<a href="https://choosealicense.com/licenses/mit/"><img src="https://img.shields.io/github/license/fabasoad/ghacu" /></a>
    <a href="https://twitter.com/intent/tweet?text=Ditch%20all%20that%20convoluted%20%22Math.random()%22%20code.%20Rando.js%20makes%20picking%20a%20number%201-100%20as%20simple%20as%20%22rando(1,%20100)%22,%20and%20it%20can%20do%20way%20more%20just%20as%20easily-%20at%20a%20cryptographically%20strong%20level.&url=https://github.com/nastyox/Rando.js&hashtags=javascript,opensource,js,webdev,developers"><img src="http://randojs.com/images/tweetShield.svg" alt="Tweet" height="20"/></a>
</p><br/><br/>

<p align="center"><a href="https://github.com/fabasoad/ghacu"><img src="https://raw.githubusercontent.com/fabasoad/ghacu/feature/41/resources/ghacu-demo.gif" width="60%"/></a></p><br/>

## :newspaper: Description
CLI tool that checks versions of GitHub Actions that used in a repository. Please read [documentation](https://github.com/fabasoad/ghacu/wiki) for more details.

## :shipit: Commands
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

### :key: GitHub Token
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

## :crystal_ball: Examples  
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

<p align="center"><a href="https://github.com/fabasoad/ghacu#">:top:</a></p>