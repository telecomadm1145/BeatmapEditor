using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using BeatmapEditor.Performance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BeatmapEditor.Math
{
    public class WaveProcessing
    {
        public struct MeanSample
        {
            public float Positive; public float Negative;
        }
        // 压缩波形到正负数组 compressrate越大数据越详细
        public static LargeArray<MeanSample> CompressSample(LargeArray<float> data, int samplerate, double compressrate)
        {
            var totaltime = (double)data.Length / samplerate;
            double width = totaltime * compressrate;
            LargeArray<MeanSample> res = new((int)width);
            for (int x = 0; x < (int)width; x++)
            {
                var bsi = (int)(x / width * totaltime * samplerate);
                var esi = (int)((x + 1) / width * totaltime * samplerate);
                var avgsamplep = 0.0;
                var avgsamplen = 0.0;
                var totalpsampled = 0;
                var totalnsampled = 0;
                for (int si = bsi; si <= esi; si++)
                {
                    if (si >= data.Length)
                        break;
                    var sample = data[si];
                    if (sample > 0)
                    {
                        avgsamplep += sample;
                        totalpsampled++;
                    }
                    else if (sample < 0)
                    {
                        avgsamplen += -sample;
                        totalnsampled++;
                    }
                }
                if (totalnsampled != 0) avgsamplen /= totalnsampled;
                if (totalpsampled != 0) avgsamplep /= totalpsampled;
                MeanSample dat;
                dat.Positive = (float)avgsamplep;
                dat.Negative = (float)avgsamplen;
                res[x] = dat;
            }
            return res;
        }
        //// scalex:1s==1px
        //public unsafe static Bitmap Convert(LargeArray<float> data, int samplerate, int height, double scalex, Color fill, double opacity)
        //{
        //    var totaltime = (double)data.Length / samplerate;
        //    int width = (int)(totaltime * scalex + 0.5);
        //    byte* pixels = (byte*)Marshal.AllocHGlobal(width * height * 4);
        //    for (int x = 0; x < width; x++)
        //    {
        //        var bsi = (int)((double)x / width * totaltime * samplerate);
        //        var esi = (int)((double)(x + 1) / width * totaltime * samplerate);
        //        var avgsamplep = 0.0;
        //        var avgsamplen = 0.0;
        //        var totalpsampled = 0;
        //        var totalnsampled = 0;
        //        for (int si = bsi; si <= esi; si++)
        //        {
        //            if (si >= data.Length)
        //                break;
        //            var sample = data[si];
        //            if (sample > 0)
        //            {
        //                avgsamplep += sample;
        //                totalpsampled++;
        //            }
        //            else if (sample < 0)
        //            {
        //                avgsamplen += -sample;
        //                totalnsampled++;
        //            }
        //        }
        //        if (totalnsampled != 0) avgsamplen /= totalnsampled;
        //        if (totalpsampled != 0) avgsamplep /= totalpsampled;
        //        for (int y = 0; y < height; y++)
        //        {
        //            if (
        //                ((y - (height / 2)) < avgsamplen * (height / 2)) && //
        //                ((y - (height / 2)) > -avgsamplep * (height / 2)) ||
        //                (y == height / 2)
        //                )
        //            {
        //                var bi = (y * width + x) * 4;
        //                pixels[bi] = fill.R;
        //                pixels[bi + 1] = fill.G;
        //                pixels[bi + 2] = fill.B;
        //                pixels[bi + 3] = (byte)(fill.A * opacity);
        //            }
        //        }
        //    }
        //    var bmp = new Bitmap(Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul, (nint)pixels, new(width, height), new(96, 96), width * 4);
        //    Marshal.FreeHGlobal((nint)pixels);
        //    return bmp;
        //}
    }
}
