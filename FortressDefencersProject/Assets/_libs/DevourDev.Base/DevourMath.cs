using System;

namespace DevourDev.Base
{
    public static class DevourMath
    {
        public static double Lerp(double start, double end, double value)
        {
            if (start > end)
                return 1 - Lerp(end, start, value);

            if (value >= end)
                return 1;
            if (value <= start)
                return 0;

            double ratio = (value - start) / (end - start);
            return ratio;

        }
        public static int LerpInt(int start, int end, int value, int multiplier = 100)
        {
            if (start > end)
                return 1 - LerpInt(end, start, value);

            if (value >= end)
                return multiplier;
            if (value <= start)
                return 0;

            double ratio = (double)(value - start) / (end - start);
            return (int)(ratio * multiplier);
        }


    }
}
