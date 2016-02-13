using System.IO;
using MySitterHub.Model.Misc;

namespace MySitterHub.AppService.Startup
{
    public class AppConfigSvcValues
    {
        static AppConfigSvcValues()
        {
            string content = File.ReadAllText(@"appconfigsvc.json");
            Instance = JsonSerializerHelper.DeSerialize<AppConfigSvcValues>(content);
        }

        public static AppConfigSvcValues Instance { get; private set; }

        public bool SmsSimulationMode { get;  set; }

        public WhiteListActive WhiteList { get;  set; }
        public string TwilioAccountSid { get; set; }
        public string TwilioAuthToken { get; set; }
        public string SourcePhone { get; set; }
        public ConfigType ConfigType { get; set; }
    }

    public enum WhiteListActive
    {
        Unspecified,
        On,
        Off
    }
}