$runtimes = @(
  @("win-x86", ".exe"),
  @("win-x64", ".exe"),
  @("osx-x64", ""),
  @("linux-x64", "")
)
$xml = [xml](Get-Content chocolatey\ghacu.nuspec)
$version = $xml.package.metadata.version
For ($i = 0; $i -lt $runtimes.Length; $i++) {
  & dotnet build -c Release -r $($runtimes[$i][0]) -p:Version=$version -o bin\$($runtimes[$i][0])
  Move-Item -Path $('bin\' + $runtimes[$i][0] + '\ghacu' + $runtimes[$i][1]) -Destination $('bin\ghacu-' + $version + '-' + $runtimes[$i][0] + $runtimes[$i][1])
  Remove-Item $('bin\' + $runtimes[$i][0]) -Recurse
}
