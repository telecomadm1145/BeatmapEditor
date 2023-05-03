using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatmapEditor
{
    public static class Extensions
    {
        public static TimeSpan Sec2TimeSpan(this int seconds)
        {
            return new TimeSpan(((long)(seconds * 10000000)));
        }
        public static TimeSpan Sec2TimeSpan(this long seconds)
        {
            return new TimeSpan(((long)(seconds * 10000000)));
        }
    }
}
