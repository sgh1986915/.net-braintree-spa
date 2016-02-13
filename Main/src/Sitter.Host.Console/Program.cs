using System;
using Microsoft.Owin.Hosting;
using MySitterHub.Host.Console.Init;
using MySitterHub.Model.Misc;
using MySitterHub.Service.Rest.AppStart;

namespace MySitterHub.Host.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = new AppLogger();

            try
            {
                string url = WebApiConfig.GetSiteUrl();
                using (WebApp.Start<Startup>(url))
                {
                    logger.Info(null, "Started Sitter on {0}", url);
                    logger.Info(null, "ConfigType:" + AppConfigWebValues.Instance.ConfigType);
                    logger.Info(null, "S3BucketName: {0} ", AppConfigWebValues.Instance.S3BucketName);
                    logger.Info(null, "AwsKeyId: {0} ", AppConfigWebValues.Instance.AwsKeyId);


                    System.Console.ReadLine();
                    System.Console.ReadLine();

                }
            }
            catch (Exception ex)
            {
                logger.Error(null, "Program.Main", ex);
                System.Console.ReadLine();
            }
        }
    }
}