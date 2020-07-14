Invoke-WebRequest -Uri https://github.com/fabasoad/ghacu/releases/download/v2.0.1/ghacu-2.0.1-win-x64.tgz -OutFile ~\ghacu-2.0.1-win-x64.tgz
cd ~ && tar -xf ghacu-2.0.1-win-x64.tgz
Rename-Item .\ghacu-2.0.1-win-x64 .\ghacu
$PathCurrentUser = (Resolve-Path ~)
$PathEnvOld = (Get-ItemProperty -Path 'Registry::HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment' -Name PATH).path
$PathEnvNew = “$PathEnvOld;$PathCurrentUser\ghacu”
Set-ItemProperty -Path 'Registry::HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment' -Name PATH -Value $PathEnvNew
