Invoke-WebRequest -Uri https://github.com/fabasoad/ghacu/releases/download/v2.0.1/ghacu-2.0.1-win-x64.tgz -OutFile ~\ghacu-2.0.1-win-x64.tgz
tar -xf ~\ghacu-2.0.1-win-x64.tgz
New-Item -ItemType SymbolicLink -Path "~\ghacu-2.0.1-win-x64\ghacu.exe" -Target "C:\Windows\System32\ghacu.exe"
