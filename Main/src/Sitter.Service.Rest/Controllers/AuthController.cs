using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Model;
using MySitterHub.Model.Common.Authentication;
using MySitterHub.Model.Core;
using MySitterHub.Logic.Repository;
using MySitterHub.Logic.ServiceModels;

namespace MySitterHub.Service.Rest.Controllers
{
    public class AuthController : ApiController
    {
        private readonly AuthManager _authManager;

        public AuthController()
        {
            _authManager = new AuthManager();
        } 

        public IHttpActionResult Post([FromBody] AuthenticationRequest value)
        {
            if (value == null)
            {
                return BadRequest("AuthendicationRequest is invalid");
            }

            AuthenticatePassResult result = _authManager.AuthenticateUser(value);
            if (!result.Success) 
            {
                HttpResponseMessage error = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                error.Content = new StringContent("Username or password is invalid.");
                return ResponseMessage(error);
            }

            return Ok(result);

        }

    }
}