using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySitterHub.Model.Common.Authentication
{
    public class AuthenticationRequest
    {
        public AuthenticationRequest(string userName, string pass)
        {
            UserName = userName;
            Pass = pass;
        }

        public string UserName { get; set; }
        public string Pass { get; set; }
    }
}
