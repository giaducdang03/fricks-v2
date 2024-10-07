using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Utils
{
    public class NumberUtils
    {
        public static int GenerateSixDigitNumber()
        {
            Random random = new Random();
            int number = random.Next(100000, 1000000);
            return number;
        }

        public static long GetRandomLong()
        {
            Random rand = new Random();
            return rand.NextInt64((long)Math.Pow(2, 53) - 1);
        }
    }
}
