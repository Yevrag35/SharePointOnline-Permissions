[CmdletBinding(PositionalBinding = $false)]
param
(
    [parameter(Mandatory=$true, Position = 0)]
	[string] $TenantName,

	[parameter(Mandatory=$false, Position = 1)]
	[string] $DestinationSite
)

$curDir = Split-Path -Parent $MyInvocation.MyCommand.Definition;
$psd1 = Get-ChildItem $curDir *.psd1 -File | Convert-Path;
Import-Module $psd1 -ea Stop;

Login-SharePoint -TenantName $TenantName -DestinationSite $DestinationSite -PromptBehavior Auto;
$pt = "Documents/PermTest";