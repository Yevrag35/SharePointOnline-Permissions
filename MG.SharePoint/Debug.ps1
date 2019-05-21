$curDir = Split-Path -Parent $MyInvocation.MyCommand.Definition;
Import-Module "$curDir\MG.SharePoint.dll" -ea Stop;
Import-Module "$curDir\MethodsProperties.psm1" -ea Stop;

#if ([MG.SharePoint.CTX]::Login("dgrsystems", "sites/clients", "Auto"))
if ([MG.SharePoint.CTX]::Login("yevrag35", $null, "Auto"))
{
	[MG.SharePoint.SPWeb]$web = '';
}
