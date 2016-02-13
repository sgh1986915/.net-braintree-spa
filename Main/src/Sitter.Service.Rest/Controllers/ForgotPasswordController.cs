using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySitterHub.Model;
using MySitterHub.Model.Misc;
using MySitterHub.Model.Core;
using MySitterHub.DAL.DataAccess;
using MySitterHub.DAL.General;
using MySitterHub.Logic.ServiceModels;
using MySitterHub.Logic.Util;
using MySitterHub.Logic.Messaging;

namespace MySitterHub.Service.Rest.Controllers
{
    public class ForgotPasswordController : ApiController
    {
        private AppUserDal _appUserDal = new AppUserDal();
        private ChangePasswordRequestDal _changePasswordRequestDal = new ChangePasswordRequestDal();
        private readonly OutboundMessageManager _omm = new OutboundMessageManager();
        private readonly UserPassDal _userPassDal = new UserPassDal();

        [HttpPost]
        public HttpResponseMessage ChangeRequest(ChangeRequest_RequestSM data)
        {
            // 1. Check does user with mobilePhone exist?
            AppUser user = _appUserDal.GetByMobile(data.Mobile);
            if (user != null)
            {
                // 2. If yes - create new entry in collection "ChangePasswordRequest".
                var changePasswordRequest = new ChangePasswordRequest {
                    Mobile = data.Mobile,
                    Created = DateTime.Now,
                    Code = CodeGenerator.Generate4DigitCode(),
                    Approved = false,
                    Hash = StringHasher.GenerateHash()
                };
                _changePasswordRequestDal.Insert(changePasswordRequest);
                // 3. Send SMS with code to user.
                _omm.SendPasswordToUserWhoForgotPassword(user, changePasswordRequest.Code.ToString());
                // 4. Response user with hash. With this hash user will send code back and change password then.
                var changeRequest_ResponseSM = new ChangeRequest_ResponseSM { 
                    Hash = changePasswordRequest.Hash
                };
                return Request.CreateResponse(HttpStatusCode.OK, changeRequest_ResponseSM);
            }
            else
            {
                // TODO: maybe return OK anyway to not let malefactors spam website to know all available mobilephone numbers?
                return Request.CreateResponse(HttpStatusCode.BadRequest, "User with such mobile phone doesn't exists.");
            }
        }

        [HttpPost]
        public HttpResponseMessage SendCode(SendCode_RequestSM data)
        {
            // 1. Check that hash+code are OK.
            ChangePasswordRequest changePasswordRequest = _changePasswordRequestDal.GetByHash(data.Hash);

            if (changePasswordRequest != null && changePasswordRequest.Code == data.Code)
            {
                // 2. Make this entry's 'approved' = true.
                changePasswordRequest.Approved = true;
                // TODO: btw in security reason we can change hash in changePasswordRequest and send new one.
                _changePasswordRequestDal.Update(changePasswordRequest);
                var sendCode_ResponseSM = new SendCode_ResponseSM {
                    Hash = data.Hash
                };
                return Request.CreateResponse(HttpStatusCode.OK, sendCode_ResponseSM);
            }

            else
            {
                // TODO: maybe we need to delete changePasswordRequest entry then?
                // Malefactors can spam codes until they succeed.
                // Or limit attempts...
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Code is invalid.");
            }
        }

        [HttpPost]
        public HttpResponseMessage ChangePassword(ChangePassword_RequestSM data)
        {
            // 1. Ensure that changePasswordRequest already approved.
            ChangePasswordRequest changePasswordRequest = _changePasswordRequestDal.GetByHash(data.Hash);
            if (changePasswordRequest != null && changePasswordRequest.Approved == true)
            {
                // 2. Change user password.
                AppUserPass appUserPass = _userPassDal.GetByMobileOrEmail(changePasswordRequest.Mobile);
                appUserPass.PasswordHash = StringHasher.GetHashString(data.Password);
                _userPassDal.Update(appUserPass);
                // 3. Remove ChangePasswrodRequest.
                // TODO: REMOVE IT ^ to never be used again e.g. by malefactors.
                return Request.CreateResponse(HttpStatusCode.OK, appUserPass);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong. Maybe hash is expired. Please try again.");
            }
        }
    }
}