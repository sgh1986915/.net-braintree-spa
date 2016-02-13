using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySitterHub.Model.Misc
{
    public class AppException : Exception
    {
        public AppException(string msg)
            : base(msg)
        {
        }
    }
}
