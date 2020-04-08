$runtimes = @(
  @("win-x86", ".exe"),
  @("win-x64", ".exe"),
  @("osx-x64", ""),
  @("linux-x64", "")
)
$xml = [xml](Get-Content ghacu.nuspec)
$version = $xml.package.metadata.version
For ($i = 0; $i -lt $runtimes.Length; $i++) {
  & dotnet build -c Release -r $($runtimes[$i][0]) -p:Version=$version -o bin\$($runtimes[$i][0])
  Get-ChildItem -Path $('bin\' + $runtimes[$i][0]) -Recurse -exclude $('ghacu' + $runtimes[$i][1]) |
    Select -ExpandProperty FullName |
    sort length -Descending |
    Remove-Item -force
}
