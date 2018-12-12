param
(
    [parameter(Mandatory=$true, Position=0)]
    [string] $DebugDirectory,

    [parameter(Mandatory=$true, Position=1)]
    [string] $ModuleFileDirectory,

    [parameter(Mandatory=$true, Position=2)]
    [string] $AssemblyInfo,

    [parameter(Mandatory=$true, Position=3)]
    [string] $TargetFileName
)

## Get Module Version
$assInfo = Get-Content $AssemblyInfo;
foreach ($line in $assInfo)
{
    if ($line -like "*AssemblyFileVersion(*")
    {
        $vers = $line -replace '^\s*\[assembly\:\sAssemblyFileVersion\(\"(.*?)\"\)\]$', '$1';
    }
}
$allFiles = Get-ChildItem $ModuleFileDirectory * -File;
$References = Join-Path "$ModuleFileDirectory\.." "Assemblies";

[string[]]$allDlls = Get-ChildItem $References -Include *.dll -Exclude 'System.Management.Automation.dll' -Recurse | Select -ExpandProperty Name;
# Import-Module $(Join-Path $DebugDirectory $TargetFileName);
# $moduleInfo = Get-Command -Module $($TargetFileName.Replace('.dll', ''));
# [string[]]$allCmd = $moduleInfo | ? { $_.CommandType -eq "Cmdlet" } | Select -ExpandProperty Name;
# [string[]]$allAlias = $moduleInfo | ? { $_.CommandType -eq "Alias" } | Select -ExpandProperty Name;
[string[]]$allFormats = $allFiles | ? { $_.Extension -eq ".ps1xml" } | Select -ExpandProperty Name;

$manifestFile = $TargetFileName.Replace('.dll', '.psd1');
# $allNames = @($($allFiles | Select -ExpandProperty Name), $manifestFile);

$allFiles | Copy-Item -Destination $DebugDirectory -Force;

$manifest = @{
    Path               = $(Join-Path $DebugDirectory $manifestFile)
    Guid               = '7bee5c4e-bc82-4535-bc0d-dcc1f576704f'
    Description        = 'A extensible module specifically for modifying item-level permissions in SharePoint Online.'
    Author             = 'Mike Garvey'
    CompanyName        = 'Yevrag35, LLC.'
    Copyright          = '(c) 2018 Yevrag35, LLC.  All rights reserved.'
    ModuleVersion      = $vers.Trim()
    PowerShellVersion  = '5.1'
    RootModule         = $TargetFileName
    RequiredAssemblies = $allDlls
    AliasesToExport    = ''
    CmdletsToExport    = '*'
    FunctionsToExport  = @()
    VariablesToExport  = ''
    FormatsToProcess   = $allFormats
    ProjectUri	       = 'https://github.com/Yevrag35/SharePointOnline-MFA'
};

New-ModuleManifest @manifest;
