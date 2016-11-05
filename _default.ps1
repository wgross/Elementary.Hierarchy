Import-Module psake

#region Commands

# Retrieve the pathes of the files from path

$dotnet = (Get-Command dotnet.exe).Path
$msbuild = (Get-Command msbuild.exe).Path
$git = (Get-Command git.exe).Path

# NUnit cnosole runner is retrieved from the packages directory
$nunit = (Get-Command $PSScriptRoot\packages\NUnit.ConsoleRunner.3.2.0\tools\nunit3-console.exe).Path

#endregion 

#region Files

$localPackageSource = (Resolve-Path "C:\src\packages")


#endregion 

#region Nuget dependencies

Task query_dependencies {

    $script:nuget = (Get-Command nuget.exe).Path
}

Task restore_dependencies {

    # restore all nuget packages referenced by the current solution
    & $script:nuget  restore $solutionFileItems

} -depends query_dependencies

Task clean_dependencies {

    Remove-Item $PSScriptRoot\packages -Recurse -ErrorAction SilentlyContinue

}

#endregion 

#region Visual Studio Solutions

Task query_solution {

    $script:solutionFileItems = (Get-ChildItem -File $PSScriptRoot -Include *.sln)

}

Task clean_solution {

    & $msbuild $script:solutionFileItems /t:Clean /p:Configuration=Release
    & $msbuild $script:solutionFileItems /t:Clean /p:Configuration=Debug

} -depends query_solution

Task build_solution {

    & $msbuild $script:solutionFileItems /t:Build /p:Configuration=Debug

} -depends query_solution

#endregion

#region Nuget packages

Task query_packages {

    $script:packageDefinitionFileItems = (Get-ChildItem -File $PSScriptRoot -Include *.nuspec -Recurse)
    $script:nuget = (Get-Command nuget.exe).Path
}

Task clean_packages {
 
    # Deletion of package files is done in the project root only.     
    # It is expected that all packages are created from 'buid_package' which is called from the project root.
    
    Remove-Item $PSScriptRoot\*.nupkg -ErrorAction SilentlyContinue
}

Task build_packages {

    $script:packageDefinitionFileItems | ForEach-Object {

        # The package is built by nuget in a Release configuration using the nuget package proerties extracted
        # from the project file and following the package definitions of the nuspec file lying next to the project file.
        # As a result there should be nuget package containing the ElementaryHierarchy assembly and its generated XML documentation

        & $script:nuget pack (Split-Path $_ -Parent) -Build -Prop "Configuration=Release" -Symbols -MSbuildVersion 14
    }

} -depends query_packages

Task rebuild_packages -depends clean_packages,build_packages

#endregion

Task build -depends build_solution

Task test {

    & $nunit (Resolve-Path $PSScriptRoot/Elementary.Hierarchy.Test/Elementary.Hierarchy.Test.csproj)

} -precondition { Test-Path $nunit } -depends build,restore

Task publish_local {
    
    # deploy to local package repo
    Copy-Item $PSScriptRoot\Elementary.Hierarchy.*.nupkg $localPackageSource

}  -depends clean_packages,build_packages

Task restore -depends restore_dependencies

# To clean the whole project tree, the nuget dependencies all compiles projects and all 
# locally created packages hav to be removed.

Task clean -depends clean_dependencies,clean_solution,clean_packages

# The full build process included cleaning of the project tree, restoring the dependencies and
# building and testing all the resilting artifacts
# Building packages is not included. This may happen after the testresults are evaluated

Task default -depends clean,restore,build,test