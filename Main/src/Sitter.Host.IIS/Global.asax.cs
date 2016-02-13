using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Avocado.Sitter.Host.IIS
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //GlobalConfiguration.Configure(Avocado.Sitter.Service.Rest.WebApiConfig.Register);
        }
    }
}
