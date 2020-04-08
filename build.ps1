$runtimes = @(
  @("win-x86", ".exe"),
  @("win-x64", ".exe"),
  @("osx-x64", ""),
  @("linux-x64", "")
)
For ($i = 0; $i -lt $runtimes.Length; $i++) {
  & dotnet build -c Release -r $($runtimes[$i][0]) -p:Version=1.1.0 -o bin\Release\$($runtimes[$i][0])
  Get-Children -Path 'bin\Release\'$runtimes[$i][0] -Recurse -exclude 'ghacu'$runtimes[$i][1] |
    Select -ExpandProperty FullName |
    sort length -Descending |
    Remove-Item -force
}