$runtimes = @(
  @("linux-x64", ""),
  @("osx-x64", ""),
  @("win-x64", ".exe"),
  @("win-x86", ".exe")
)
$xml = [xml](Get-Content chocolatey\ghacu.nuspec)
$version = $xml.package.metadata.version
For ($i = 0; $i -lt $runtimes.Length; $i++) {
  & dotnet build -c Release -r $($runtimes[$i][0]) -p:Version=$version -o bin\$($runtimes[$i][0])
}
