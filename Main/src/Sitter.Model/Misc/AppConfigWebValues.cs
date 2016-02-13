using System.IO;

namespace MySitterHub.Model.Misc
{
    public class AppConfigWebValues
    {
        static AppConfigWebValues()
        {
            string content = File.ReadAllText(@"appconfigweb.json");
            Instance = JsonSerializerHelper.DeSerialize<AppConfigWebValues>(content);
        }

        public static AppConfigWebValues Instance { get; private set; }

        public string S3BucketName { get; set; } // Profile picture bucket
        public string AwsKeyId { get; set; }
        public string AwsKeySecret { get; set; }
        public ConfigType ConfigType { get; set; }
    }

    public enum ConfigType
    {
        Unspecified,
        Dev,
        Prod
    }
}