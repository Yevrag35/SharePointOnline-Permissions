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

## Clear out files
Get-ChildItem -Path $DebugDirectory -Include *.ps1xml -Recurse | Remove-Item -Force;

## Get Module Version
$assInfo = Get-Content $AssemblyInfo;
foreach ($line in $assInfo)
{
    if ($line -like "*AssemblyFileVersion(*")
    {
        $vers = $line -replace '^\s*\[assembly\:\sAssemblyFileVersion\(\"(.*?)\"\)\]$', '$1';
    }
}
$allFiles = Get-ChildItem $ModuleFileDirectory -Include * -Exclude *.old -Recurse;
$References = Join-Path "$ModuleFileDirectory\.." "Assemblies";

[string[]]$verbs = Get-Verb | Select-Object -ExpandProperty Verb;
$patFormat = '^({0})(\S{{1,}})\.cs';
$pattern = $patFormat -f ($verbs -join '|');
$cmdletFormat = "{0}-{1}";

$baseCmdletDir = Join-Path "$ModuleFileDirectory\.." "Cmdlets";
[string[]]$folders = [System.IO.Directory]::EnumerateDirectories($baseCmdletDir, "*", [System.IO.SearchOption]::TopDirectoryOnly) | Where-Object { -not $_.EndsWith('Bases') };

$aliasPat = '\[alias\(\"(.{1,})\"\)\]'
$csFiles = @(Get-ChildItem -Path $folders *.cs -File);
$Cmdlets = New-Object System.Collections.Generic.List[string] $csFiles.Count;
$Aliases = New-Object System.Collections.Generic.List[string];
foreach ($cs in $csFiles)
{
    $match = [regex]::Match($cs.Name, $pattern)
    $Cmdlets.Add(($cmdletFormat -f $match.Groups[1].Value, $match.Groups[2].Value));
    $content = Get-Content -Path $cs.PSPath -Raw;
    $aliasMatch = [regex]::Match($content, $aliasPat, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase);
    if ($aliasMatch.Success)
    {
        $Aliases.Add($aliasMatch.Groups[1].Value);
    }
}

[string[]]$allDlls = Get-ChildItem $References -Include *.dll -Exclude 'System.Management.Automation.dll' -Recurse | Select -ExpandProperty Name;
[string[]]$allFormats = $allFiles | ? { $_.Extension -eq ".ps1xml" } | Select -ExpandProperty Name;

$manifestFile = "MG.SharePoint.psd1";

$allFiles | Copy-Item -Destination $DebugDirectory -Force;
$modPath = Join-Path $DebugDirectory $manifestFile;

$manifest = @{
    Path                   = $modPath
    Guid                   = '7bee5c4e-bc82-4535-bc0d-dcc1f576704f'
    Description            = 'A extensible module specifically for modifying item-level permissions in SharePoint Online.'
    Author                 = 'Mike Garvey'
    CompanyName            = 'Yevrag35, LLC.'
    Copyright              = '(c) 2019 Yevrag35, LLC.  All rights reserved.'
    ModuleVersion          = $($vers.Trim() -split '\.' | Select-Object -First 3) -join '.'
    PowerShellVersion      = '5.1'
    DotNetFrameworkVersion = '4.7'
    RootModule             = $TargetFileName
    RequiredAssemblies     = $allDlls
    AliasesToExport        = $Aliases.ToArray()
    CmdletsToExport        = $Cmdlets.ToArray()
    DefaultCommandPrefix   = "SP"
    FormatsToProcess       = $allFormats
    ProjectUri             = 'https://github.com/Yevrag35/SharePointOnline-MFA'
    HelpInfoUri            = 'https://github.com/Yevrag35/SharePointOnline-MFA/issues'
    LicenseUri             = 'https://raw.githubusercontent.com/Yevrag35/SharePointOnline-Permissions/master/LICENSE'
};

New-ModuleManifest @manifest;
