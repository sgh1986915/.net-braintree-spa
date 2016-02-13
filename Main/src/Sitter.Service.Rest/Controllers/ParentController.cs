using System.Collections.Generic;
using System.Web.Http;
using MySitterHub.Logic.Repository;
using MySitterHub.Logic.ServiceModels;
using MySitterHub.Model.Core;
using MySitterHub.Service.Rest.Util;

namespace MySitterHub.Service.Rest.Controllers
{
    public class ParentController : ApiController
    {
        private readonly JobRepository _jobRepo = new JobRepository();
        private readonly ParentRepository _parentRepo = new ParentRepository();

        [HttpGet]
        public IHttpActionResult MySitters()
        {
            int userId = HeaderHelper.GetAppUserId(this);
            return Ok(_parentRepo.GetSittersForParent(userId));
        }

        [HttpGet]
        public IHttpActionResult MyJobs()
        {
            int userId = HeaderHelper.GetAppUserId(this);
            return Ok(_jobRepo.GetParentMyJobs(userId));
        }

        [HttpPost]
        public IHttpActionResult PostJob([FromBody] PostJobSM value)
        {
            if (value == null)
            {
                return BadRequest("job is null");
            }
            value.ParentId = HeaderHelper.GetAppUserId(this);
            return Ok(_jobRepo.PostJob(value));
        }

        [HttpPost]
        public IHttpActionResult CancelJob([FromBody] CancelJobSM value)
        {
            value.UserId = value.UserId = HeaderHelper.GetAppUserId(this);
            return Ok(_jobRepo.CancelJob(value));
        }

        [HttpPost]
        public IHttpActionResult UpdateMySitters([FromBody] List<ParentMySitterSaveSM> sitters)
        {
            int userId = HeaderHelper.GetAppUserId(this);
            return Ok(_parentRepo.SaveSittersForParent(userId, sitters));
        }

        [HttpPost]
        public IHttpActionResult InviteSitter([FromBody] InviteToSignup data)
        {
            int userId = HeaderHelper.GetAppUserId(this);
            return Ok(_parentRepo.AddSitterInviteByMobile(userId, data));
        }

        [HttpPost]
        public IHttpActionResult CancelInviteSitter([FromBody] NewMySitterInviteVM data)
        {
            int userId = HeaderHelper.GetAppUserId(this);
            return Ok(_parentRepo.CancelInviteSitter(userId, data == null ? null : data.MobilePhone));
        }

        [HttpPost]
        public IHttpActionResult FinalizeJobPayment([FromBody] FinalizeJobPaymentSM data)
        {
            data.UserId = HeaderHelper.GetAppUserId(this);
            
            return Ok(_jobRepo.FinalizePayment(data));
        }
    }
}