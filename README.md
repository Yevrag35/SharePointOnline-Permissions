# SharePoint-MFA (MG.SharePoint)

We live in a world where MFA is becoming ever more popular to implement.  In the spirit of supporting that, here's a framework to support script/app building for managing your SharePoint Online instance with complete MFA/Modern authentication support.

## How is this different than the SPO PowerShell Module?

Currently, the SPO module requires that you be a SharePoint admin in order to login.  Building a PowerShell script using this framework will allow even non-admins to manage aspects of SPO.

### Well then... why wouldn't I just start working with CSOM?

Not everything from CSOM is being tweaked in this framework.

## Features

1. Non-SPO Admin logins supported.
1. Fully-integrated with Modern authentication (etc. - MFA).
1. Have the ability to create a context in any site collection within your SPO instance.

In addition to the better login methods, new objects have been created to make working with CSOM in PowerShell less painful.  All new wrapper objects, have an method to __Load Properties__ by just specifying the property names.

    [MG.SharePoint.SPFolder]$folder = "documents/test folder";
    $folder.LoadProperty("Files", "Folders");

1. __SPWeb__
    * A wrapper for [Microsoft.SharePoint.Client.Web].
    * <code>$web = New-Object MG.SharePoint.SPWeb</code>
    * <code>[MG.SharePoint.SPWeb]$web = "/sites/TeamSite";</code>
2. __SPWebCollection__
    * A wrapper for [Microsoft.SharePoint.Client.WebCollection].
3. __SPList__
    * A wrapper for [Microsoft.SharePoint.Client.List].  By default, it looks under the current Web.
    * <code>$list = New-Object MG.SharePoint.SPList("Access Requests");</code>
    * <code>[MG.SharePoint.SPList]$list = "Access Requests";</code>
4. __SPListCollection__
    * A wrapper for [Microsoft.SharePoint.Client.ListCollection].
5. __SPListItemCollection__
    * A wrapper for [Microsoft.SharePoint.Client.ListItemCollection].
6. __SPListItem__
    * A wrapper for [Microsoft.SharePoint.Client.ListItem].
    * <code>$list = $web.GetLists()["ListOfStuff"];<br>
            $list.GetItems();  $list.Items["Title of Item"];  $list.Items[14];</code>
7. __SPFolder__
    * A wrapper for [Microsoft.SharePoint.Client.Folder].
    * <code>$folder = New-Object MG.SharePoint.SPFolder("Documents/Test Folder");</code>
    * <code>$folder.UploadFile("C:\Users\Mike\Desktop\MyWordDoc.docx", $true, $true);</code>
8. __SPFile__
    * A wrapper for [Microsoft.SharePoint.Client.File].
9. __SPFolderCollection__
    * A wrapper for [Microsoft.SharePoint.Client.FolderCollection].
10. __SPFileCollection__
    * A wrapper for [Microsoft.SharePoint.Client.FileCollection].

I've made __NEW__ objects to help consolidate certains CSOM objects (e.g. - Permissions).

1. __SPPermission__
    * _An object containing a Principal and a RoleAssignment; used for 'viewing' permissions._
2. __SPBinding__
   * _An object containing a Principal and a RoleDefinition; used for 'applying' permissions._
3. __SPPermissionCollection__
4. __SPBindingCollection__

---

## Getting Started

Depending on what you're building, here are two ways you can utilize this framework:

Through PowerShell:

    Import-Module .\MG.SharePoint.dll;

Through C#?
    Simply add "MG.SharePoint.dll" and the dependencies to your projects' references.  I don't know if I'll make a NuGet package for this yet.

---

## Logging In

To log in to "https://myspo.sharepoint.com" through PowerShell on a __Non-AzureAD joined__ machine:

    $spoWeb = [MG.SharePoint.CTX]::Login("myspo", $null, "Always")

To log in to "https://myspo.sharepoint.com/sites/docCenter", do:

    $spoWeb = [MG.SharePoint.CTX]::Login("myspo", "sites/docCenter", "Always")

To log in to "https://myspo.sharepoint.com" on an __AzureAD joined or Hybrid Domain-joined__ machine, you can utilize the 'auto' login feature:

    $spoWeb = [MG.SharePoint.CTX]::Login("myspo", $null, "Auto")

To do it the __old fashioned way__ (with a UserName and Password combo), do:

    $creds = Get-Credential "spouser@myspo.com";
    $spoWeb = [MG.SharePoint.CTX]::Login("myspo", $null, $creds)

---

## Permissions

I've taken a quite a bit of time making getting/setting web/list/folder/item permissions a ton easier to work with in PowerShell.  On any object you retrieve from SharePoint, you can invoke the "GetPermissions()" method.

A nice compact table of the object's permissions are presented (and stored) for viewing.

    $web = New-Object MG.SharePoint.Web;
    $web.GetPermissions();

To add permissions, it's a simple as specifying the user and what role to give them:

    $folder = New-Object MG.SharePoint.SPFolder("Documents/Test Folder/WayDownHere");
    $folder.AddPermission("mary@contoso.com", "Contribute", $true);

To do more complex sets of permissions; I accommodate you.  Just make your permissions in the form of a Hashtable like so:

    $newPermissions = @{
        Reader = "The Whole Company", "someexternalguy@whatever.org"
        "Custom Role"  = "janetwhothinksshespecial@italktoomuch.biz"
        Contribute = "mary@contoso.com"
        "Full Control" = "Bob the Boss"
    }

    $folder.AddPermissions($newPermissions, $true);

## What I'm planning

1. Making wrapper objects for Users/Principals and Groups.
2. Implementing a "RemovePermission()" method.
3. Starting to translate the functionality over to 'full' PowerShell cmdlets.