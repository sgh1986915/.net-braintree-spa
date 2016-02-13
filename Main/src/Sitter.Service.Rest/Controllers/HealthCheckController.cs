using System.ComponentModel;
using System.IO;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MySitterHub.Service.Rest.Controllers
{
    public class HealthCheckController : ApiController
    {

        public IHttpActionResult Get()
        {
            var result = new HealthCheckResult() {WebServiceOk = true};
            var driveInfo = new DriveInfo("C");

            double percentAvailable = ((double) driveInfo.TotalFreeSpace/(double) driveInfo.TotalSize);
            if (percentAvailable > .1)
                result.DiskSpaceCheckOk = true;

            return Ok(result);
        }

        public class HealthCheckResult
        {            
            public bool DiskSpaceCheckOk { get; set; }
            public bool WebServiceOk { get; set; }            
        }
    }
}