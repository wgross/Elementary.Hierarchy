Import-Module psake

$nuget = (Get-Command nuget.exe).Path
$msbuild = (Get-Command msbuild.exe).Path
$hg = (Get-Command hg.exe).Path
$git = (Get-Command git.exe).Path
$nunit = (Get-Command $PSScriptRoot\packages\NUnit.ConsoleRunner.3.2.0\tools\nunit3-console.exe).Path
$localPackageSource = (Resolve-Path "C:\src\packages")

Task default -depends build

Task package_restore {

    & $nuget restore

} -precondition { Test-Path $nuget }

Task clean {

    & $msbuild (Resolve-path $PSScriptRoot\Elementary.Hierarchy.sln) /t:Clean /p:Configuration=Release
    & $msbuild (Resolve-path $PSScriptRoot\Elementary.Hierarchy.sln) /t:Clean /p:Configuration=Debug
    
    Remove-Item $PSScriptRoot\*.nupkg -ErrorAction SilentlyContinue
}

Task build {

    & $msbuild (Resolve-path $PSScriptRoot\Elementary.Hierarchy.sln) /t:Build /p:Configuration=Debug

} -precondition { Test-Path $msbuild } -depends package_restore

Task test {
    
    & $nunit (Resolve-Path $PSScriptRoot/Elementary.Hierarchy.Test/Elementary.Hierarchy.Test.csproj)

} -precondition { Test-Path $nunit } -depends build,package_restore

Task pack {

    & $nuget Pack (Resolve-path $PSScriptRoot\Elementary.Hierarchy\Elementary.Hierarchy.csproj) -Build -Prop "Configuration=Release" -Symbols -MSbuildVersion 14
    
    # deploy to local package repo
    Copy-Item $PSScriptRoot\Elementary.Hierarchy.*.nupkg $localPackageSource

} -precondition { Test-Path $nuget } -depends clean,test


Task commit {

    & $hg commit -m "Auto commit of changed files before push"
    & $git commit -m "Auto commit of changed files before push"

} -precondition { Test-Path $hg }

Task push {

    & $hg push bitbucket
    & $git push

} -precondition { Test-Path $hg } -depends commit


    