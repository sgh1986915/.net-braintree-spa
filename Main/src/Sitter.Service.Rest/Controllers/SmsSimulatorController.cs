using System.Web.Http;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Logic.Repository;
using MySitterHub.Model.Core;

namespace MySitterHub.Service.Rest.Controllers
{
    public class SmsSimulationController : ApiController
    {
        private readonly AppUserRepository _appUserRepo = new AppUserRepository();
        private readonly SmsSimulatorRepository _smsSimulatorRepository = new SmsSimulatorRepository();
        private readonly TxtMsgAwaitingResponseDal _txtMsgAwaitingResponseDal = new TxtMsgAwaitingResponseDal();

        [HttpGet]
        public IHttpActionResult ConfigInfo()
        {
            return Ok("");
        }

        [HttpGet]
        public IHttpActionResult AllUsers()
        {
            return Ok(_appUserRepo.GetAllAppUsers());
        }

        [HttpGet]
        public IHttpActionResult AllMessages()
        {
            return Ok(_smsSimulatorRepository.GetAllInboundAndOutbound());
        }

        [HttpGet]
        public IHttpActionResult AllTxtMsgAwaitingResponse()
        {
            return Ok(_txtMsgAwaitingResponseDal.GetAll());
        }

        [HttpPost]
        public IHttpActionResult SendSimulatedInboundMessage([FromBody] SimulatedInboundTxt simMsg)
        {
            var txt = new TxtMsgInbound
            {
                MobilePhone = simMsg.FromMobilePhone,
                Message = simMsg.Message
            };

            _smsSimulatorRepository.InsertInbound(txt);
            return Ok();
        }

        public class SimulatedInboundTxt
        {
            public string FromMobilePhone { get; set; }
            public string Message { get; set; }
        }
    }
}