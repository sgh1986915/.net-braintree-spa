using System;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;
using MySitterHub.Model.Common.Authentication;
using System.Net.Mail;

namespace MySitterHub.Logic.Repository
{
    public class SignupRepository
    {
        private readonly AppUserDal _appUserDal = new AppUserDal();
        private readonly AuthDal _authDal = new AuthDal();
        private readonly ParentDal _parentDal = new ParentDal();
        private readonly SitterDal _sitterDal = new SitterDal();
        private readonly UserPassDal _userPassDal = new UserPassDal();

        public SignupResult SaveNewSignup(SignupInfo signupInfo,bool requireValidEmail = true, bool requireSitterParentPhone = false)
        {
            var result = new SignupResult() ;
            ValidateSignup(signupInfo, result, requireValidEmail, requireSitterParentPhone);
            if (result.Error != null)
                return result;

            signupInfo.User.Email = signupInfo.User.Email.ToLower();

            _appUserDal.InsertUser(signupInfo.User);
            var userPass = new AppUserPass
            {
                Id = signupInfo.User.Id,
                PasswordHash = StringHasher.GetHashString(signupInfo.Pass),
                Token = Guid.NewGuid().ToString(),
                Email = signupInfo.User.Email.ToLower(),
                MobilePhone =  signupInfo.User.MobilePhone
            };

            _userPassDal.InsertUserPass(userPass);

            if (signupInfo.User.UserRole == UserRole.Parent)
            {
                var parent = new Parent {Id = signupInfo.User.Id};
                _parentDal.Insert(parent);
            }
            else if (signupInfo.User.UserRole == UserRole.Sitter)
            {
                var sitter = new Sitter
                {
                    Id = signupInfo.User.Id,
                    ParentEmail = signupInfo.SitterSignupInfo.ParentEmail,
                    ParentMobile = signupInfo.SitterSignupInfo.ParentMobile,
                    Age = signupInfo.SitterSignupInfo.Age
                };
                _sitterDal.Insert(sitter);
                result.NewId = sitter.Id;
            }
            else if (signupInfo.User.UserRole == UserRole.Admin)
            {                
            }
            else
            {
                throw new AppException(string.Format("userRole '{0}' not valid", signupInfo.User.UserRole));
            }

            string userName = signupInfo.User.Email.ToLower() != null ? signupInfo.User.Email.ToLower() : signupInfo.User.MobilePhone;
            result.newUserData =_authDal.AuthenticateUserByName(userName);
            result.IsSuccess = true;
            return result;
        }

        private void ValidateSignup(SignupInfo signupInfo, SignupResult result, bool requireValidEmail,bool requireSitterParentPhone)
        {
            if (signupInfo == null)
            {
                result.Error = "signupInfo is null";
                return;
            }
            
            if (signupInfo.User == null)
            {
                result.Error = "user is null";
                return;
            }

            if (string.IsNullOrWhiteSpace(signupInfo.User.FirstName ))
            {
                result.Error = "First Name is required";
                return;
            }

            if (requireValidEmail)
            {
                try
                {
                    MailAddress email = new MailAddress(signupInfo.User.Email); //FutureDev: use Regex so that 1) we don't have to reference System.Net assembly, and 2) don't have to catch an exception which is expensive.
                }
                catch (FormatException)
                {
                    result.Error = "Invalid email";
                    return;
                }
            }
            if (string.IsNullOrWhiteSpace(signupInfo.Pass))
            {
                result.Error = "Password is required";
                return;
            }

            if (!PhoneUtil.IsValidPhoneNumber(signupInfo.User.MobilePhone))
            {
                result.Error = "Mobile Phone is invalid";
                return;
            }

            signupInfo.User.MobilePhone = PhoneUtil.CleanAndEnsureCountryCode(signupInfo.User.MobilePhone);
            if (_appUserDal.GetByMobile(signupInfo.User.MobilePhone) != null)
            {
                result.Error = "Mobile Phone is already associated with an account.";
                return;
            }

            if (requireSitterParentPhone)
            {
                if (signupInfo.User.UserRole == UserRole.Sitter)
                {
                    if (signupInfo.SitterSignupInfo == null)
                    {
                        result.Error = "Signup is sitter, but SitterSignupInfo is null";
                        return;
                    }
                    if (signupInfo.SitterSignupInfo.Age < 18 && string.IsNullOrEmpty(signupInfo.SitterSignupInfo.ParentMobile))
                    {
                        result.Error = "Signup is sitter and is less than Age 18, parent mobile is required.";
                        return;
                    }
                }
            }
        }
    }

    public class SignupResult
    {
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
        public int NewId { get; set; }
        public AuthenticatePassResult newUserData { get; set; }
    }
}