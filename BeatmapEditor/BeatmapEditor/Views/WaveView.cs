using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using BeatmapEditor.Math;
using BeatmapEditor.Performance;
using DynamicData.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatmapEditor.Views
{
    public class WaveView : Path
    {
        private LargeArray<WaveProcessing.MeanSample>? _data;
        public LargeArray<WaveProcessing.MeanSample>? AudioData
        {
            get
            {
                return _data;
            }
            set
            {
                SetAndRaise(AudioDataProperty, ref _data, value);
                UpdatePath();
            }
        }
        private double last_h = 0;
        private LargeArray<WaveProcessing.MeanSample>? last_dat = null;
        private void UpdatePath()
        {
            var ch = Bounds.Height;
            var wave = AudioData;
            if ((ch - last_h).IsNearPixel()) // 感知不强(
            {
                if (last_dat == wave)
                {
                    return; //不!更!新! 浪费cpu时间
                }
            }
            else
            {
                last_h = ch;
            }
            last_dat = wave;
            var points = linegeo.Points;
            var h = ch;
            var step = 5;
            points.Clear();
            if (wave == null)
                return;
            points.Add(new(0, h / 2));
            int i = 0;
            var wave2 = wave;
            foreach (var item in wave2)
            {
                points.Add(new Point(i, item.Positive * h / 2 + h / 2));
                i += step;
            }
            points.Add(new Point(i, h / 2));
            foreach (var item in wave2.Reverse())
            {
                points.Add(new Point(i, h / 2 - item.Negative * h / 2)); // 100 -
                i -= step;
            }
            points.Add(new(0, h / 2));
            linegeo.Points = points;
        }

        public static DirectProperty<WaveView, LargeArray<WaveProcessing.MeanSample>?> AudioDataProperty = AvaloniaProperty.RegisterDirect<WaveView, LargeArray<WaveProcessing.MeanSample>?>(nameof(AudioData), o => o.AudioData, (o, v) => o.AudioData = v);
        private PolylineGeometry linegeo;
        public WaveView()
        {
            linegeo = new PolylineGeometry();
            linegeo.IsFilled = true;
            linegeo.Points = new Points();
            Data = linegeo;
            SizeChanged += (s, e) =>
            {
                UpdatePath();
            };
        }
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
        }
    }
}
