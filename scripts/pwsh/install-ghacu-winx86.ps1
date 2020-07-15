$os = 'win-x86'
$version = '2.0.1'
Invoke-WebRequest -Uri "https://github.com/fabasoad/ghacu/releases/download/v$version/ghacu-$version-$os.tgz" -OutFile "~\ghacu-$version-$os.tgz"
cd ~ && tar -xf "ghacu-$version-$os.tgz"
$path = (Resolve-Path ~)
$env:Path += ";$path\ghacu-$version-$os"
