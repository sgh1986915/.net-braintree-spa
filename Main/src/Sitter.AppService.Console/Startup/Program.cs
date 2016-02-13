using System;
using MySitterHub.AppService.Sms;
using MySitterHub.Model.Misc;

namespace MySitterHub.AppService.Startup
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AppLogger log = new AppLogger();

            try
            {
                DisplayConfigInfo(log);
                new MessageWorker().DoWork();
            }
            catch (Exception ex)
            {
                log.Error(null, null, ex);
                Console.ReadLine();
            }
        }

        private static void DisplayConfigInfo(AppLogger log)
        {
            log.Info(null, "MySitterHub AppService started");
            log.Info(null, "ConfigType:" + AppConfigSvcValues.Instance.ConfigType);
            log.Info(null, "SmsSimulationMode:" + AppConfigSvcValues.Instance.SmsSimulationMode);
            log.Info(null, "WhiteList:" + AppConfigSvcValues.Instance.WhiteList);
            log.Info(null, "WhiteList#s:" + string.Join(",", MessagingConstants.Whitelist));
            log.Info(null, "SourcePhone:" + AppConfigSvcValues.Instance.SourcePhone);
        }
    }
}