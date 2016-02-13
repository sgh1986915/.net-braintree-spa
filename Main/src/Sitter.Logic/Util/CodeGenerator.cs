using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySitterHub.Logic.Util
{
    public class CodeGenerator
    {
        public static int Generate4DigitCode()
        {
            Random random = new Random();
            int value = random.Next(1000, 9999);
            return value;
        }
    }
}