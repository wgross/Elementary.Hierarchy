[CmdletBinding(DefaultParameterSetName="local")]
param(
    [Parameter()]
    [array]$Projects = (((Get-Content -Path (Join-Path -Path $PSScriptRoot -ChildPath scripts.config)) | ConvertFrom-Json).pack.projects),

    [Parameter(ParameterSetName="local")]
    [switch]$Local,

    [Parameter(ParameterSetName="global")]
    [switch]$Global
)
begin {

    function Get-NugetConfigItem {
        param(
            [Parameter(Mandatory)]
            [ValidateScript({Test-Path -Path $_})]
            $Path
        )
            
        # search upwards for first nuget.config

        $currentDirectory = $Path
        while(-not(Test-Path -Path $currentDirectory\nuget.config)) {
            $currentDirectory = Split-Path -Path $currentDirectory -Parent
            
            if([string]::IsNullOrEmpty($currentDirectory)) { 
                throw "cant't find nuget.config above $PWD" 
            }
        }
        
        # if the directory doesn't contains nuget.config and exception had been thrown already
        # just return what must be there:
        
        Get-Item -Path (Join-Path -Path $currentDirectory -ChildPath nuget.config)
    }

    function packLocal {
        [CmdletBinding()]
        param(
            [Parameter(ValueFromPipeline)]
            $Project,

            [Parameter(Mandatory)]
            [ValidateScript({Test-Path -Path $_})]
            $LocalPackageDirectory
        )
            
        $projectFullName = (Join-Path -Path $PSScriptRoot -ChildPath $Project)

        "Calling dotnet pack $projectFullName -c Release -o $LocalPackageDirectory" | Write-Verbose
            
        dotnet pack $projectFullName -c Release -o $LocalPackageDirectory
    }
}
process {   
    switch($PSCmdlet.ParameterSetName) {

        "local" {

            # From the local direcory search upwards to the Nuget.Config           
            
            $nugetConfigItem = Get-NugetConfigItem -Path $PWD

            "Found nuget.config at $nugetConfigItem" | Write-Verbose
            
            # From the Nuget.Config get the path with key 'local'. 
            # This points to tehdirectory for the local packages 

            $localPackageDirectory = ((Select-Xml -Path ($NugetConfigItem.FullName) -XPath "//packageSources/add[@key = 'local']").Node.Value)
            $localPackageDirectory = Resolve-Path -Path (Join-Path -Path ($NugetConfigItem.Directory) -ChildPath $localPackageDirectory)

            "Found local package directory: $localPackageDirectory" | Write-Verbose

            # Now pipe all proect pathes in the packLocal cmdlet

            $Projects | packLocal -LocalPackageDirectory $localPackageDirectory
        }

        "global" {
            
            # What will happen here.. some kind of publishing.. maybe to nuget.org?

            throw "global package creation isn't supported yet"
        }
    }
}