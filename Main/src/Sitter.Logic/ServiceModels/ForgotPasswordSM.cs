using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySitterHub.Logic.ServiceModels
{
    public class ChangeRequest_RequestSM
    {
        public string Mobile { get; set; }
    }
    public class ChangeRequest_ResponseSM
    {
        public Guid Hash { get; set; }
    }
    public class SendCode_RequestSM
    {
        public int Code { get; set; }
        public Guid Hash { get; set; }
    }
    public class SendCode_ResponseSM
    {
        public Guid Hash { get; set; }
    }

    public class ChangePassword_RequestSM
    {
        public Guid Hash { get; set; }
        public string Password { get; set; }
    }
}
