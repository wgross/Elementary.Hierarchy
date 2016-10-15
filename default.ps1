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

Task restore_dependencies {

    # restore all nuget packages referenced by the current solution
    & $nuget restore $solutionFileNames

} -precondition { Test-Path $nuget }

Task clean_dependencies {

    Remove-Item $PSScriptRoot\packages -Recurse -ErrorAction SilentlyContinue

} -precondition { Test-Path $nuget }

#endregion 

#region .Net Assemblies 

Task clean_assemblies {

    & $msbuild $solutionFileNames /t:Clean /p:Configuration=Release
    & $msbuild $solutionFileNames /t:Clean /p:Configuration=Debug
}

#endregion

#region Nuget packages

Task clean_packages {
     
    Remove-Item $PSScriptRoot\*.nupkg -ErrorAction SilentlyContinue
}

Task build_packages {

    # The package is built automatically by nuget in a Release configuration using the nuget package proerties extracted
    # from the project file and following the package defineions of teh nuspec file lying next to the project file.
    # As a result there should be nuget package containing the ElementaryHierarchy assembly and its generated XML documentation

    & $nuget Pack (Resolve-path $PSScriptRoot\Elementary.Hierarchy\Elementary.Hierarchy.csproj) -Build -Prop "Configuration=Release" -Symbols -MSbuildVersion 14
}

Task rebuild_packages -depends clean_packages,build_packages

#endregion

Task build {

    & $msbuild $solutionFileNames /t:Build /p:Configuration=Debug

} -precondition { Test-Path $msbuild } -depends restore

Task test {

    & $nunit (Resolve-Path $PSScriptRoot/Elementary.Hierarchy.Test/Elementary.Hierarchy.Test.csproj)

} -precondition { Test-Path $nunit } -depends build,restore

Task publish_local {

    
    # deploy to local package repo
    Copy-Item $PSScriptRoot\Elementary.Hierarchy.*.nupkg $localPackageSource

} -precondition { Test-Path $nuget } -depends test,clean_packages

Task restore -depends restore_dependencies
Task clean -depends clean_dependencies,clean_assemblies
Task default -depends clean,build,test