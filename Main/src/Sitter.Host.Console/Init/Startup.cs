using System;
using System.IO;
using System.Web.Http;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using MySitterHub.Model.Misc;
using MySitterHub.Service.Rest;
using MySitterHub.Service.Rest.AppStart;
using Owin;

namespace MySitterHub.Host.Console.Init
{

    public class Startup
    {


        private AppLogger _logger = new AppLogger();

        public void Configuration(IAppBuilder app)
        {
            // STEP Map Folder Paths
            const string clientRootFolder = @"..\..\..\";
            DirectoryInfo webDirectory = null;

            webDirectory = new DirectoryInfo(clientRootFolder + @"Sitter.Client\src\build");

            ValidateDirectory(webDirectory);

            // Path to index
            var fsOptions = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                FileSystem = new PhysicalFileSystem(webDirectory.FullName)
            };

            app.UseFileServer(fsOptions);

            // Self-host the WebApi
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            IoCConfig.Register(config);
            app.UseWebApi(config);
        }

        private void ValidateDirectory(DirectoryInfo webDirectory)
        {
            if (webDirectory == null || !webDirectory.Exists)
            {
                string currentDIr = Directory.GetCurrentDirectory();
                throw new Exception("Path not found: " + (webDirectory == null ? "" : webDirectory.FullName) + " current dir:" + currentDIr);
            }

            _logger.Info(null, "Directory : {0}", webDirectory.FullName);
            _logger.Info(null, "MachineName : {0}", Environment.MachineName);
        }
    }
}