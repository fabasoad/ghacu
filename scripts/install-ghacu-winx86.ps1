$version = '2.0.2'
$os = 'win-x86'
Invoke-WebRequest -Uri "https://github.com/fabasoad/ghacu/releases/download/v$version/ghacu-$version-$os.tgz" -OutFile "~\ghacu-$version-$os.tgz"
cd ~ && tar -xf "ghacu-$version-$os.tgz"
$path = (Resolve-Path ~)
New-Item -ItemType SymbolicLink -Path C:\Windows\System32\ghacu.exe -Value $path\ghacu-$version-$os\ghacu.exe -Force
