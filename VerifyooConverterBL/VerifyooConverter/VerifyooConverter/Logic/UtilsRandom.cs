using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyooConverter.Logic
{
    class UtilsRandom
    {
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random(); return random.Next(min, max);

        }
    }
}
