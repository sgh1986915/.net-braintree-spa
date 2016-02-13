using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySitterHub.Model.Misc
{
    // data = {mobilePhone: "+xyyyyyyyyyy"}
    // Create new entry in collection "ChangePasswordRequests". 
    // This collection consists of objects like {mobilePhone: <>, code: <>, created: <>, hash: <>, approved: <bool>}
    // and entries should be removed each 10mins('created' key needed for that).
    // Malefactors can try to spam 4 digits code on every mobilePhone so we need to add 'hash' that we will check in next actions.
    public class ChangePasswordRequest
    {
        public int Id { get; set; }
        public string Mobile { get; set; }
        public Guid Hash { get; set; }
        public DateTime Created { get; set; }
        public bool Approved { get; set; }
        public int Code { get; set; }
    }
}
