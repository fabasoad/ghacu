param([string] $os = "")

function Build-Output {
  param (
    [string] $argVersion,
    [string] $argOS
  )
  $('ghacu-' + $argVersion + '-' + $argOS)
}

function Build-Package {
  param (
    [string] $argVersion,
    [string] $argOS
  )
  $Output = Build-Output $argVersion $argOS
  dotnet build -c Release -r $argOS -p:Version=$argVersion -o bin/$Output -f netcoreapp5.0
}

function Compress-Package {
  param (
    [string] $argVersion,
    [string] $argOS
  )
  Set-Location bin/
  $Output = Build-Output $argVersion $argOS
  tar -czf $($Output + '.tgz') $Output
  Set-Location ..
}

$xml = [xml](Get-Content src/Ghacu.Runner/Ghacu.Runner.csproj)
$version = $xml.Project.PropertyGroup.PackageVersion

switch($os) {
  {($_ -eq "macos-latest") -Or ($_ -eq "")} {
    Build-Package $version "osx-x64"
    Compress-Package $version "osx-x64"
  }
  {($_ -eq "ubuntu-latest") -Or ($_ -eq "")} {
    Build-Package $version "linux-x64"
    Compress-Package $version "linux-x64"
  }
  {($_ -eq "windows-latest") -Or ($_ -eq "")} {
    Build-Package $version "win-x64"
    Compress-Package $version "win-x64"
  }
  {($_ -eq "win-x86") -Or ($_ -eq "")} {
    Build-Package $version "win-x86"
    Compress-Package $version "win-x86"
  }
}
