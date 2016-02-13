using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.StaticFiles.ContentTypes;

namespace MySitterHub.Host.Console.Init.Misc
{
    public class AppTypeProvider : FileExtensionContentTypeProvider
    {
        public AppTypeProvider()
        {
            //Mappings.Add(".woff2", "application/font-woff");
            //font/ttf
        }
    }
}
