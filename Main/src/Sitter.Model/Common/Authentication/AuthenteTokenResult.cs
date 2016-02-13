using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySitterHub.Model.Common.Authentication
{
    public class AuthenteTokenResult
    {
        public bool IsSuccess { get; set; }
        public int UserId { get; set; }
        public string ErrorMessage { get; set; }
    }
}
