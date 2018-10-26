﻿using Microsoft.SharePoint.Client;
using System;

namespace MG.SharePoint
{
    public interface ISPObject
    {
        object Id { get; }
        string Name { get; }
        object ShowOriginal();
    }
}
