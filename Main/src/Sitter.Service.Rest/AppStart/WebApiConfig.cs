using System.Net.Http.Formatting;
using System.Web.Http;
using MySitterHub.Service.Rest.Controllers;
using MySitterHub.Service.Rest.Filters;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace MySitterHub.Service.Rest.AppStart
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("ActionApi", "api/{controller}/{action}/{id}",
                new {action = RouteParameter.Optional, id = RouteParameter.Optional}
                );

            #region Media Formatters

            config.Formatters.Clear();

            // JSON media type formatter
            config.Formatters.Add(new JsonMediaTypeFormatter());
            JsonMediaTypeFormatter json = config.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());

            // File Upload
            //config.Formatters.Add(new FileUploadFormatter());

            #endregion Media Formatters

            #region Filters

            config.Filters.Add(new AuthenticationFilter());

            #endregion
        }

        public static string GetSiteUrl()
        {
            string hostName = "*";
            const string port = "";

            return string.Format("http://{0}{1}", hostName, port);
        }
    }
}