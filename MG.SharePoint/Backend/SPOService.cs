using System;

namespace MG.SharePoint
{
    public class SPOService
    {
        // Methods
        public SPOService(CmdLetContext context) => 
            this.Context = context ?? throw new ArgumentNullException("context");

        // Properties
        private static SPOService curSrv;
        internal static SPOService CurrentService
        {
            get => curSrv;
            set => curSrv = value;
        }

        public string Url =>
            this.Context.Url;

        public CmdLetContext Context { get; set; }
    }
}
