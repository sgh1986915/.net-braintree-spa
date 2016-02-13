using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Logic.Messaging;
using MySitterHub.Logic.Repository;
using MySitterHub.Model.Core;
using MySitterHub.Service.Rest.Util;

namespace MySitterHub.Service.Rest.Controllers
{
    public class SitterController : ApiController
    {
        private readonly SitterRepository _sitterRepo = new SitterRepository();
        private readonly JobRepository _jobRepo = new JobRepository();
        private readonly OutboundMessageManager _omm = new OutboundMessageManager();

        [Route("sitters/{id}")]
        public IHttpActionResult Get(int id)
        {
            return Ok(_sitterRepo.GetById(id));
        }

        [HttpGet]
        public IHttpActionResult MyJobs()
        {
            int userId = HeaderHelper.GetAppUserId(this);
            return Ok(_jobRepo.GetSitterMyJobs(userId));
        }

        [HttpGet]
        public IHttpActionResult MyClients()
        {
            int userid = HeaderHelper.GetAppUserId(this);
            return Ok(_sitterRepo.GetSitterMyClients(userid));
        }

        [HttpPost]
        public IHttpActionResult JobResponse([FromBody] SitterJobInviteResponseSM response)
        {
            int userid = HeaderHelper.GetAppUserId(this);
            response.SitterId = userid;
            return Ok(_jobRepo.ProcessSitterResponse(response, null));
        }

        [HttpPost]
        public IHttpActionResult RenegeAcceptedJob([FromBody] SitterCancelAcceptedJobSM renig)
        {
            int userid = HeaderHelper.GetAppUserId(this);
            renig.SitterId = userid;
            return Ok(_jobRepo.SitterRenegeAcceptedJob(renig));
        }

        [HttpPost]
        public IHttpActionResult RequestPayment([FromBody] SitterRequestPaymentSM requestPayment)
        {
            int userid = HeaderHelper.GetAppUserId(this);
            requestPayment.SitterId = userid;
            return Ok(_jobRepo.SitterRequestPayment(requestPayment));
        }

    }
}