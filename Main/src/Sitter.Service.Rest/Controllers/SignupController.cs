using System.Web.Http;
using MySitterHub.Logic.Repository;
using MySitterHub.Model.Core;

namespace MySitterHub.Service.Rest.Controllers
{
    public class SignupController : ApiController
    {
        private readonly SignupRepository _signupRepository = new SignupRepository();
        
        [HttpPost]
        public IHttpActionResult Signup([FromBody] SignupInfo signupInfo)
        {
            SignupResult ret = _signupRepository.SaveNewSignup(signupInfo);
            if (!ret.IsSuccess)
                return BadRequest(ret.Error);
            return Ok(ret);
        }
    }
}