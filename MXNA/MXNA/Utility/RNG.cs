using System;
using System.Collections.Generic;

namespace MXNA
{
    public static class RNG
    {
        static System.Random r = new System.Random();

        public static int Next(int maxValue)
        {
            return r.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return minValue + r.Next(Math.Abs(maxValue - minValue));
        }
    }
}