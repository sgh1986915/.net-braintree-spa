using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySitterHub.Model.Misc
{
  public  class TimeUtil
    {
      public static DateTime GetCurrentUtcTime()
      {
          return DateTime.Now.ToUniversalTime();
      }
    }
}
