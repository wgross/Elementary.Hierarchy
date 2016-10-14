Import-Module psake

#region Commands

# Retrieve the pathes of the files from path
$nuget = (Get-Command nuget.exe).Path
$msbuild = (Get-Command msbuild.exe).Path
$git = (Get-Command git.exe).Path

# NUnit cnosole runner is retrieved from the packages directory
$nunit = (Get-Command $PSScriptRoot\packages\NUnit.ConsoleRunner.3.2.0\tools\nunit3-console.exe).Path

#endregion 

#region Files

$localPackageSource = (Resolve-Path "C:\src\packages")
$solutionFileNames = (Get-ChildItem -File $PSScriptRoot -Include *.sln)

#endregion 

#region Nuget dependencies

Task update_packages {
    & $nuget update 
} -precondition { Test-Path $nuget }

Task restore_packages {

    # restore all nuget packages referenced by the current solution
    & $nuget restore $solutionFileNames

} -precondition { Test-Path $nuget }

Task clean_packages {

    Remove-Item $PSScriptRoot\packages -Recurse -ErrorAction SilentlyContinue

} -precondition { Test-Path $nuget }

#endregion 

Task clean {

    & $msbuild $solutionFileNames /t:Clean /p:Configuration=Release
    & $msbuild $solutionFileNames /t:Clean /p:Configuration=Debug
    
    Remove-Item $PSScriptRoot\*.nupkg -ErrorAction SilentlyContinue

} -depends clean_packages

Task build {

    & $msbuild $solutionFileNames /t:Build /p:Configuration=Debug

} -precondition { Test-Path $msbuild } -depends restore

Task test {

    & $nunit (Resolve-Path $PSScriptRoot/Elementary.Hierarchy.Test/Elementary.Hierarchy.Test.csproj)

} -precondition { Test-Path $nunit } -depends build,restore

Task publish_local {

    & $nuget Pack (Resolve-path $PSScriptRoot\Elementary.Hierarchy\Elementary.Hierarchy.csproj) -Build -Prop "Configuration=Release" -Symbols -MSbuildVersion 14
    
    # deploy to local package repo
    Copy-Item $PSScriptRoot\Elementary.Hierarchy.*.nupkg $localPackageSource

} -precondition { Test-Path $nuget } -depends clean,test

Task restore -depends restore_packages
Task default -depends clean,build,test