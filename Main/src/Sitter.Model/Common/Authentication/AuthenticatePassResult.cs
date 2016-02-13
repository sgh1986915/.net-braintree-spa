using MySitterHub.Model.Core;

namespace MySitterHub.Model.Common.Authentication
{


    public class AuthenticatePassResult
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string UserDisplayName { get; set; }
        public UserRole UserRole { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

    }


}
