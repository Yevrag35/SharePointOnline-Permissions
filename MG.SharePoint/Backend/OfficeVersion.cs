using System;
using System.Diagnostics;
using System.Reflection;

namespace MG.SharePoint
{
    internal class OfficeVersion
    {
        // Fields
        public const int MajorBuildVersion = 0x10;
        public const int PreviousMajorBuildVersion = 14;
        public const int MaxCompatibilityLevel = 15;
        public const string MajorVersion = "16";
        public const string PreviousVersion = "14";
        public const string MaxCompatibility = "15";
        public const string MajorVersionDotZero = "16.0";
        public const string PreviousVersionDotZero = "14.0";
        public const string AssemblyVersion = "16.0.0.0";
        public const string WssMajorVersion = "4";
        public const string WebServerExtensionsRegistryRoot = @"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions";
        public const string WebServerExtensionsVersionRegistryRoot = @"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\16.0";
        public const string WssRegistryRoot = @"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\16.0\WSS";
        public const string OfficeRegistryRoot = @"SOFTWARE\Microsoft\Office\16.0";
        public const string MossRegistryRoot = @"SOFTWARE\Microsoft\Office Server\16.0";
        public const string InstalledLanguagesPath = @"SOFTWARE\Microsoft\Office Server\16.0\InstalledLanguages";
        public const string SkuRegistrationPath = @"SOFTWARE\Microsoft\Office\16.0\Registration";
        public const string PublicKeyToken = "71e9bce111e9429c";
        public const string AssemblyFullyQualifiedName = ", Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";
        public static readonly string FullBuildVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        public static readonly string FullBuildBase = (new Version(FullBuildVersion).ToString(3) + ".0");

        // Methods
        private OfficeVersion()
        {
        }
    }
}
