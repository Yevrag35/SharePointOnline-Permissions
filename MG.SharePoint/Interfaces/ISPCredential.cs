using System;
using System.Management.Automation;
using System.Security;

namespace MG.SharePoint.Security
{
    public interface ISPCredential
    {
        string UserName { get; set; }
        SecureString Password { get; set; }

        PSCredential AsPSCredential();
    }
}
