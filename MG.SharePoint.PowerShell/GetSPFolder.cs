﻿using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SPFolder", DefaultParameterSetName = "ByRelativeUrl")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SPFolder))]
    public class GetSPFolder : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByRelativeUrl")]
        public string RelativeUrl { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByFolderId")]
        public Guid Id { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByFolderInput")]
        public Folder Folder { get; set; }

        private protected Folder inQuestion;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            switch (ParameterSetName)
            {
                case "ByFolderId":

                    inQuestion = CTX.SP1.Web.GetFolderById(Id);

                    break;
                case "ByFolderInput":

                    inQuestion = Folder;

                    break;
                default:

                    inQuestion = CTX.SP1.Web.GetFolderByServerRelativeUrl(RelativeUrl);

                    break;
            }

            SPFolder outFol = inQuestion;
            WriteObject(outFol);
        }
    }
}
