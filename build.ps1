$runtimes = @(
  @("linux-x64", ""),
  @("osx-x64", ""),
  @("win-x64", ".exe"),
  @("win-x86", ".exe")
)
# $exclude = @(
#   'CommandLine.dll',
#   'ghacu.deps.json',
#   'ghacu.dll',
#   'ghacu.pdb',
#   'ghacu.runtimeconfig.json',
#   'LiteDB.dll',
#   'Octokit.dll',
#   'YamlDotNet.dll'
# )
$xml = [xml](Get-Content chocolatey\ghacu.nuspec)
$version = $xml.package.metadata.version
For ($i = 0; $i -lt $runtimes.Length; $i++) {
  & dotnet build -c Release -r $($runtimes[$i][0]) -p:Version=$version -o bin\$($runtimes[$i][0])
  # Get-ChildItem -Path $('bin\' + $runtimes[$i][0]) -Recurse -exclude $($exclude + $('ghacu' + $runtimes[$i][1])) |
  #   Select -ExpandProperty FullName |
  #   sort length -Descending |
  #   Remove-Item -force
}
