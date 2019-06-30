using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint.PowerShell
{
    public static class Cache
    {
        public static SearchCollection SearchCache { get; set; }
        public static Guid CurrentWeb { get; set; }
    }
}