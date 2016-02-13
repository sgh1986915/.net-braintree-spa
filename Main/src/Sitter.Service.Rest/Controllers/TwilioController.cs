using System.Web.Http;
using MySitterHub.Logic.Repository;

namespace MySitterHub.Service.Rest.Controllers
{
    public class TwilioController : ApiController
    {
        private readonly TwilioRepository _twilioRepository = new TwilioRepository();

        public IHttpActionResult Get()
        {
            string req = Request.RequestUri.ToString();
            bool ret = _twilioRepository.PersistIncommingTwillioMessage(req);
            return Ok(ret);
        }
    }
}