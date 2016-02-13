using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySitterHub.Model.Core.User
{
    public class AppUserPass
    {
        public int Id { get; set; }

        /// <summary>
        ///     We are duplicating the Email and MobilePhone on this object because every web service call needs to call the
        ///     AuthManager.ValidateToken() which hopefully only needs to call database once.
        /// </summary>
        public string Email { get; set; }

        public string MobilePhone { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        [JsonIgnore]
        public string Token { get; set; }
    }
}
