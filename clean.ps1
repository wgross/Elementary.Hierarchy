[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Position=0)]
    $Path = $PWD,

    [Parameter(Position=1)]
    [ValidateSet("git_clean-xfd")]
    [string[]]$Method
)
process {
    if($Method -contains "git_clean-xfd") {
        if($PsCmdlet.ShouldProcess("git clean -xfd")) {
            "cleaning with git clean -xfd" | Write-Verbose
        } else {

            "cleaning with git clean -xfd -n" | Write-Verbose

            git clean -xfd -n
        }
        
        #git clean -xfd $dryrun
    } 
}