[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Position=0)]
    $Path = $PWD,

    [Parameter(Position=1)]
    [ValidateSet("git_clean-xfd")]
    [string[]]$Method,

    [Parameter()]
    [switch]$Force
)
begin {
    Import-Module -Name dev-tools
}
process {
   
    if($Method -contains "git_clean-xfd") {

        if($Force.IsPresent) {
            $gitForce = "-f"
        }

        $Path | Get-DotNetProjectItem -Process {

            if($PsCmdlet.ShouldProcess("git clean -xfd")) {
                
                "cleaning with git clean -xd $gitForce $($_.Directory)" | Write-Verbose
                    
                git clean -xd $gitForce $_.Directory

            } else {

                "cleaning with git clean -xd -n $gitForce $($_.Directory)" | Write-Verbose

                git clean -xd -n $gitForce $_.Directory
            }
        }
    } 
}
