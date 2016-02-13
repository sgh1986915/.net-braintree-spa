using System.Collections.Generic;
using System.Linq;
using MySitterHub.DAL.DataAccess;
using MySitterHub.Model.Common.Authentication;

namespace MySitterHub.Logic.Repository
{
    public class AuthManager
    {
        public static AuthManager Instance = new AuthManager();

        /// <summary>
        ///     Returns token if username and password are valid
        /// </summary>
        public AuthenticatePassResult AuthenticateUser(AuthenticationRequest authRequest)
        {
            return new AuthDal().AuthenticatePassGetToken(authRequest.UserName, authRequest.Pass);
        }

        public AuthenteTokenResult AuthenticateToken(AuthenticationToken authToken)
        {
            var result = new AuthenteTokenResult();
            if (authToken == null || authToken.UserName == null || authToken.Token == null)
            {
                result.ErrorMessage = "Authentication failed, username or token not specified";
            }
            else
            {
                return new AuthDal().ValidateToken(authToken.UserName, authToken.Token);
            }

            return result;
        }

    }
}