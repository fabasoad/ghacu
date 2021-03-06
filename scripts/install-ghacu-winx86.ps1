$os = "win-x86"
$release = Invoke-WebRequest -Uri "https://api.github.com/repos/fabasoad/ghacu/releases/latest" | ConvertFrom-Json
$asset = $null
For ($i = 0; $i -lt $release.assets.Length; $i++)
{
    If ($release.assets[$i].name -like "*$os.tgz")
    {
        $asset = $release.assets[$i]
        break
    }
}
$asset_name = $asset.name
Invoke-WebRequest -Uri $asset.browser_download_url -OutFile "~\$asset_name"
cd ~ && tar -xf $asset_name
$folder = [io.path]::GetFileNameWithoutExtension($asset_name)
$path = (Resolve-Path ~)
$env:Path += ";$path\$folder"
New-Item -ItemType SymbolicLink -Path C:\Windows\System32\ghacu.exe -Value $path\$folder\ghacu.exe -Force
