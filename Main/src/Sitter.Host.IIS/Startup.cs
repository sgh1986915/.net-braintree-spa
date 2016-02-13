using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.StaticFiles.ContentTypes;
using Owin;
using Avocado.Sitter.Service.Rest;
using Avocado.Sitter.Service.Rest.AppStart;

[assembly: OwinStartup(typeof(Avocado.Sitter.Host.IIS.Startup))]
namespace Avocado.Sitter.Host.IIS
{
    public enum ConfigurationType
    {
        Dev,
        Prod
    }

    public class Startup
    {
        public static ConfigurationType BuildConfiguration = ConfigurationType.Prod;
        public void Configuration(IAppBuilder app)
        {

            //if (BuildConfiguration == ConfigurationType.Dev)
            //{
            //    webDirectory = new DirectoryInfo(clientRootFolder + @"Sitter.Client\src");
            //}
            //else if (BuildConfiguration == ConfigurationType.Prod)
            //{
            //webDirectory = new DirectoryInfo(clientRootFolder + @"Sitter.Client\build");
                //webDirectory = new DirectoryInfo(@"C:\_git\avocado\Main\src\Sitter.Client\src");
            
            //}

            var config = new HttpConfiguration();
            //config.Filters.Add(new AuthenticationFilter());
            WebApiConfig.Register(config);
            IoCConfig.Register(config);
            app.UseWebApi(config);
            app.UseCors(CorsOptions.AllowAll);

            // STEP Map Folder Paths
            const string clientRootFolder = @"C:\_github\sitter\Main\src\";
            DirectoryInfo webDirectory = null;
            BuildConfiguration = ConfigurationType.Dev;

            if (BuildConfiguration == ConfigurationType.Dev)
            {
                webDirectory = new DirectoryInfo(clientRootFolder + @"Sitter.Client\src");
            }
            else if (BuildConfiguration == ConfigurationType.Prod)
            {
                webDirectory = new DirectoryInfo(clientRootFolder + @"Sitter.Client\build");
                if (!IsValidateDirectory(webDirectory))
                {
                    // TODO JFK: Standardize hosting paths
                    webDirectory = new DirectoryInfo(@"C:\SitterAppDeploy\appclient");
                }
            }


            var isValid = IsValidateDirectory(webDirectory);
            if (!isValid)
                return;
            // Path to index
            var fsOptions = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                FileSystem = new PhysicalFileSystem(webDirectory.FullName)
            };
            fsOptions.StaticFileOptions.ContentTypeProvider = new TcsTypeProvider();
            app.UseFileServer(fsOptions);
        }

        private bool IsValidateDirectory(DirectoryInfo webDirectory)
        {
            if (webDirectory == null || !webDirectory.Exists)
            {
                string currentDIr = Directory.GetCurrentDirectory();
                return false;
                //throw new Exception("Path not found: " + (webDirectory == null ? "" : webDirectory.FullName) + " current dir:" + currentDIr);

            }
            //System.Console.WriteLine("Directory : {0}\n", webDirectory.FullName);
            return true;
        }

    }
    public class TcsTypeProvider : FileExtensionContentTypeProvider
    {
        public TcsTypeProvider()
        {
            Mappings.Add(".woff2", "application/font-woff");
            //font/ttf
        }
    }

}