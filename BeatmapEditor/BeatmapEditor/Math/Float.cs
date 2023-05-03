using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatmapEditor.Math
{
    public static class Float
    {
        public static float Abs(this float x)
        {
            return x >= 0 ? x : -x;
        }
        public static double Abs(this double x)
        {
            return x >= 0 ? x : -x;
        }
        public static bool IsNearZero(this float x)
        {
            return x.Abs() < 0.00001;
        }
        public static bool IsNearZero(this double x)
        {
            return x.Abs() < 0.00001;
        }
        public static bool IsNearPixel(this double x)
        {
            return x.Abs() < 1.5;
        }
        public static bool IsNearPixel(this float x)
        {
            return x.Abs() < 1.5;
        }
    }
}
