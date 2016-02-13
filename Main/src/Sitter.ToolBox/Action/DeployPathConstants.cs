using System.IO;

namespace Sitter.ToolBox.Controls
{
    public static class DeployPathConstants
    {
        public const string TaasAppFolder = @"C:\TaasApp";
        public const string TaasAppDeployFolder = @"C:\TaasAppDeploy";

        public static string DestConsoleHost
        {
            get
            {
                return Path.Combine(TaasAppDeployFolder, @"Taas.Host.Console\bin\x64\Debug");
            }
        }
        public static string DestTaasClient
        {
            get
            {
                return Path.Combine(TaasAppDeployFolder, @"Taas.Client");
            }
        }
        public static string DestTaasToolbox
        {
            get
            {
                return Path.Combine(TaasAppDeployFolder, @"Taas.ToolBox\bin\x64\Debug");
            }
        }

        public static string DestTaasRealTime
        {
            get
            {
                return Path.Combine(TaasAppDeployFolder, @"TaaS.Service.RealTime\bin\x64\Debug");
            }
        }

        public const string SourceConsoleHostBin = @"..\..\..\..\TaaS.Host.Console\bin\x64\Debug";
        public const string SourceTaasClient = @"..\..\..\..\TaaS.Client";

        public const string SourceTaasToolBox = @"..\..\..\..\TaaS.ToolBox\bin\x64\Debug";

        public const string SourceTaasRealTime = @"..\..\..\..\TaaS.Service.RealTime\bin\x64\Debug";


    }
}
