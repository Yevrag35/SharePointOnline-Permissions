# SharePoint-MFA (MG.SharePoint)

We live in a world where MFA is becoming ever more popular to implement.  In the spirit of supporting that, here's a framework to support script/app building for managing your SharePoint Online instance with complete MFA/Modern authentication support.

## How is this different than the SPO PowerShell Module?

Currently, the SPO module requires that you be a SharePoint admin in order to login.  Building a PowerShell script using this framework will allow even non-admins to manage aspects of SPO.

### Well then... why wouldn't I just start working with CSOM?

CSOM makes it fairly painless to accomodate non-admin logins, and you absolutely should continue to develop in CSOM.  This framework just streamlines the authentication process and OAuth token handling.

## Features

1. Non-SPO Admin logins supported.
1. Fully-integrated with Modern authentication (etc. - MFA).
1. Have the ability to create a context in any site collection within your SPO instance.

---

## Getting Started

Depending on what you're building, here are two ways you can utilize this framework:

1. Through PowerShell:
<code>Import-Module .\MG.SharePoint.dll</code>

1. Through C#:
Simply add "MG.SharePoint.dll" and the dependencies to your projects' references.  I don't know if I'll make a NuGet package for this yet.

---

## Logging In

To log in to "https://myspo.sharepoint.com" through PowerShell on a __Non-AzureAD joined__ machine:

<code>$spoWeb = [MG.SharePoint.CTX]::Login("myspo", $null, "Always")</code>

To log in to "https://myspo.sharepoint.com/sites/docCenter", do:

<code>$spoWeb = [MG.SharePoint.CTX]::Login("myspo", "sites/docCenter", "Always")</code>

To log in to "https://myspo.sharepoint.com" on an __AzureAD joined or Hybrid Domain-joined__ machine, you can utilize the 'auto' login feature:

<code>$spoWeb = [MG.SharePoint.CTX]::Login("myspo", $null, "Auto")</code>

To do it the __old fashioned way__ (with a UserName and Password combo), do:

<code>$creds = Get-Credential "spouser@myspo.com"
$spoWeb = [MG.SharePoint.CTX]::Login("myspo", $null, $creds)</code>

---