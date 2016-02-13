using System.Security.Authentication;
using System.Web.Http;

namespace MySitterHub.Service.Rest.Util
{
    public class HeaderHelper
    {
        /// <summary>
        /// Get AppUserId from Security Context
        /// </summary>
        public static int GetAppUserId(ApiController controller)
        {
            if (controller.User == null || controller.User.Identity == null)
                throw new AuthenticationException("API call not authenticated");

            int userIdint;
            if (int.TryParse(controller.User.Identity.Name, out userIdint))
            {
                return userIdint;
            }
            else
            {
                throw new AuthenticationException("API call not authenticated, invalid principal id.");
            }
        }
    }
}