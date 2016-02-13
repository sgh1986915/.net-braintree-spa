using Newtonsoft.Json;

namespace MySitterHub.Model.Misc
{
    public class JsonSerializerHelper
    {
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T DeSerialize<T>(string content, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(content, settings);
        }
    }
}