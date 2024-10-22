﻿using System;
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
            long min = (long)Math.Pow(10, 13); // 10^13, minimum 14-digit number
            long max = (long)Math.Pow(10, 14) - 1; // 10^14 - 1, maximum 14-digit number
            return min + (long)(rand.NextDouble() * (max - min));
        }
    }
}
