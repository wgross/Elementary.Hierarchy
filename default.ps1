Import-Module psake
$nuget = (Get-Command nuget.exe).Path
$msbuild = (Get-Command msbuild.exe).Path
$hg = (Get-Command hg.exe).Path
$git = (Get-Command git.exe).Path

Task default -depends pack

Task build {

    & $msbuild (Resolve-path $PSScriptRoot\Elementary.Hierarchy.sln) /t:Rebuild /p:Configuration=Release

} -precondition { Test-Path $msbuild }

Task pack {

    & $nuget Pack (Resolve-path $PSScriptRoot\Elementary.Hierarchy\Elementary.Hierarchy.csproj) -Prop Configuration=Release -Build -Symbols
    
    Copy-Item $PSScriptRoot\Elementary.Hierarchy.*.nupkg C:\src\packages

} -precondition { Test-Path $nuget } -depends build 

Task commit {

    & $hg commit -m "Auto commit of changed files before push"
    & $git commit -m "Auto commit of changed files before push"

} -precondition { Test-Path $hg }

Task push {

    & $hg push bitbucket
    & $git push

} -precondition { Test-Path $hg } -depends commit

    