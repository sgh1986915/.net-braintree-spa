using MySitterHub.Model.Common.Authentication;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;

namespace MySitterHub.DAL.DataAccess
{
    public class AuthDal
    {
        private readonly AppUserDal _appUserDal = new AppUserDal();
        private readonly UserPassDal _userPassDal = new UserPassDal();

        /// <summary>
        ///     returns user if password is valid
        /// </summary>
        public AuthenticatePassResult AuthenticatePassGetToken(string userName, string pass)
        {
            var result = new AuthenticatePassResult();
            // In case it could be parsed as phone let's try to make it valid.
            userName = PhoneUtil.CleanAndEnsureCountryCode(userName);
            // Error message.
            const string userOrPassInvalid = "User name or password is invalid";
            // Retrieve password by name.
            if (userName != null)
                userName = userName.ToLower();
            AppUserPass appUserPass = _userPassDal.GetByMobileOrEmail(userName);
            // Seems like such user doesn't exist.
            if (appUserPass == null)
            {
                result.ErrorMessage = userOrPassInvalid;
                return result;
            }
            // User exists. Let's check password hash.
            string hash = StringHasher.GetHashString(pass);
            if (hash != appUserPass.PasswordHash)
            {
                result.ErrorMessage = userOrPassInvalid;
                return result;
            }
            // We can authenticate user now and return token with other data.
            return AuthenticateUserByName(userName);
        }

        public AuthenticatePassResult AuthenticateUserByName(string userName)
        {
            var result = new AuthenticatePassResult();
            AppUserPass appUserPass = _userPassDal.GetByMobileOrEmail(userName);
            AppUser appUser = _appUserDal.GetById(appUserPass.Id);
            if (appUserPass != null && appUser != null)
            {
                result.Token = appUserPass.Token;
                result.Success = true;
                result.UserDisplayName = appUser.FirstName;
                result.UserId = appUserPass.Id;
                result.UserRole = appUser.UserRole;
            }
            return result;
        }

        public AuthenteTokenResult ValidateToken(string userId, string token)
        {
            var result = new AuthenteTokenResult();
            int uid;
            int.TryParse(userId, out uid);
            AppUserPass userPass = _userPassDal.GetById(uid);

            if (userPass == null || userPass.Token != token)
            {
                result.ErrorMessage = "Authentication failed, invalid email or token";
                return result;
            }

            result.IsSuccess = true;
            result.UserId = userPass.Id;
            return result;
        }
    }
}