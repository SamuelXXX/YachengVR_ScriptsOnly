using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Electrics.Utility
{
    public static class ReadingsFormatter
    {
        public static string Format(float reading, int width=4)
        {
            int w = Width(reading);
            if (w <= width)
            {
                int v = width - w;
                return reading.ToString("f" + v.ToString());
            }
            else
            {
                return "8888";
            }
        }

        public static int Width(float reading)
        {
            if (reading < 0)
                reading = -reading;
            int intPart = Mathf.FloorToInt(reading);

            int width = 0;

            while (intPart != 0)
            {
                intPart /= 10;
                width++;
            }

            if (width == 0)
                width = 1;

            return width;
        }
    }
}


