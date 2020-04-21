$Output = $(Invoke-ScriptAnalyzer -Path *.ps1)
Write-Output $Output
&{If($Output.Length -ne 0) {exit 1} else {Write-Output 'Linting passed successfully.'}}
