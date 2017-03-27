<#
.SYNOPSIS
    Builds and packs the the given dotnet core projects and deplys the packge to a location specifed by the first nuget.config it finds
    by searching upwards. 
#>
[CmdletBinding(DefaultParameterSetName="local")]
param(
    [Parameter()]
    [array]$Projects = $null,

    [Parameter(ParameterSetName="local")]
    [switch]$Local,

    [Parameter(ParameterSetName="global")]
    [switch]$Global
)
begin {

    function Get-NugetConfigItem {
        <#
        .SYNOPSIS
            From the given path ascend in the file system and return any nuget.config file found.
        #>
        [CmdletBinding()]
        param(
            [Parameter(ValueFromPipeline)]
            [ValidateScript({Test-Path -Path $_})]
            $Path = $PWD
        )
        process {
            $currentDirectory = (Get-Item -Path $Path).FullName

            while(-not([string]::IsNullOrEmpty($currentDirectory))) {
            
                "Probing directory: $currentDirectory" | Write-Verbose

                Get-Item -Path (Join-Path -Path $currentDirectory -ChildPath nuget.config) -ErrorAction SilentlyContinue

                $currentDirectory =  Split-Path -Path $currentDirectory -Parent
            }
        }
    }

    function Get-PackageSourceValue {
        <#
        .SYNOPSIS
            From a nuget.config file get the package source with the given key.
            Because the path is expected to be relative it is resolved relative to the directory of the nuget.config
        #>
        param(
            [Parameter(ValueFromPipeline )]
            [ValidateScript({Test-Path -Path $_})]
            [string]$Path,

            [Parameter()]
            [string]$Key  = "local"
        )
        process {
            $packageSoureValue = ((Select-Xml -Path $Path -XPath "//packageSources/add[@key = '$key']").Node.Value)
            # path should be relative...
            Resolve-Path -Path (Join-Path -Path (Split-Path -Path $Path -Parent) -ChildPath $packageSoureValue)
        }
    }

    function Invoke-DotNetPack {
        <#
        .SYNOPSIS
            For and given project find out find the nearest nuget.config and pack into the local package directory
        #>
        [CmdletBinding()]
        param(
            [Parameter(ValueFromPipeline)]
            $Project
        )
        process {
            "Got project to pack: $Project" | Write-Verbose

            # take the fist local package source found
            $localPackageDirectory = $Project | Get-NugetConfigItem | Get-PackageSourceValue -Key "local" | Select-Object -First 1

            try {
                
                Push-Location -Path $Project 

                "Calling: dotnet pack $Project -c Release -o $localPackageDirectory" | Write-Verbose
            
                dotnet pack $projectFullName -c Release -o $localPackageDirectory
             
             } finally {
                Pop-Location
             }
        }
    }
}
process {   

    if(-not($Projects)) {
        # read from config file
        $Projects = (((Get-Content -Path (Join-Path -Path $PSScriptRoot -ChildPath scripts.config)) | ConvertFrom-Json).pack.projects)
    }

    switch($PSCmdlet.ParameterSetName) {

        "local" {
            $Projects | Invoke-DotNetPack
        }

        "global" {
            # What will happen here.. some kind of publishing.. maybe to nuget.org?
            throw "global package creation isn't supported yet"
        }
    }
}