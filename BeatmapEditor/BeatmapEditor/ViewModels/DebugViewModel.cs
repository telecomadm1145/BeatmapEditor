using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using BeatmapEditor.Audio;
using BeatmapEditor.Math;
using BeatmapEditor.Performance;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BeatmapEditor.ViewModels
{
    public class DebugViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
        private IAudioManager.IAudioStream? stream;
        public ICommand TestAudioCommand => ReactiveCommand.Create(() =>
        {
            if (am.DeviceOpened)
            {
                am.Close();
            }
            am.OpenDevice(SelectedDevice);
            var data = File.ReadAllBytes("audio.mp3");
            stream = am.Load(data);
            stream.Play();
        });
        public Vector OffsetView => maxx == 0 ? new() : new(maxx * Current / Duration, 0);
        private int maxx;
        public ICommand SaveWave => ReactiveCommand.Create(() =>
        {
            unsafe
            {
                using (var smp = am.LoadSample(File.ReadAllBytes("audio.mp3")))
                {
                    using (var data = smp.GetSamples())
                    {
                        var wave = WaveProcessing.CompressSample(data, 44100, 40);
                        AudioData = wave;
                        var h = 200;
                        var w = 800;
                        WavePoints.Clear();
                        WavePoints.Add(new(0, h / 2));
                        int i = 0;
                        var wave2 = wave;
                        var step = 5;
                        foreach (var item in wave2)
                        {
                            WavePoints.Add(new Point(i, item.Positive * h / 2 + h / 2));
                            i += step;
                        }
                        maxx = i;
                        WavePoints.Add(new Point(maxx + 800, h / 2));
                        foreach (var item in wave2.Reverse())
                        {
                            WavePoints.Add(new Point(i, h / 2 - item.Negative * h / 2)); // 100 -
                            i -= step;
                        }
                        WavePoints.Add(new(0, h / 2));
                        this.RaisePropertyChanged(nameof(AudioData));
                        //wave.Dispose();
                    }
                }
            }
        });
        private byte[] hitsample;
        public ICommand TestSampleCommand => ReactiveCommand.Create(() =>
        {
            PlayTestSample();
        });
        public ICommand PauseAudio => ReactiveCommand.Create(() =>
        {
            stream.Pause(!stream.Paused);
        });
        public ICommand StopAudio => ReactiveCommand.Create(() =>
        {
            stream.Stop();
        });
        public LargeArray<WaveProcessing.MeanSample>? AudioData { get; private set; }
        public Points WavePoints { get; set; } = new Points();
        public double Volume
        {
            get { return stream?.Volume ?? 1; }
            set
            {
                stream.Volume = value;
            }
        }
        public double PlaybackRate
        {
            get
            {
                return stream?.PlaybackRate ?? 1;
            }
            set
            {
                this.RaisePropertyChanged(nameof(PlaybackRate));
                stream.PlaybackRate = value;
            }
        }
        public double Duration => stream?.Duration.TotalSeconds ?? 0;
        public double Current
        {
            get
            {
                return stream?.Current.TotalSeconds ?? 0;
            }
            set
            {
                stream.Current = new System.TimeSpan(((long)value * 10000000));
            }
        }
        public double CurrentReadonly => stream?.Current.TotalSeconds ?? 0;
        public double AudioLoudness { get; set; } = 0;
        public void PlayTestSample()
        {
            am.Load(hitsample).Play();
        }

        private BassAudioManager am = new();

        public DebugViewModel()
        {
            SelectedDevice = am.GetDefaultDevice();
            if (File.Exists("hit.wav"))
                hitsample = File.ReadAllBytes("hit.wav");
            DispatcherTimer dt = new();
            dt.Interval = new System.TimeSpan(0, 0, 0, 0, 1, 0);
            dt.Tick += (s, e) =>
            {
                if (stream != null && stream.Playing)
                {
                    this.RaisePropertyChanged(nameof(Current));
                    this.RaisePropertyChanged(nameof(Duration));
                    this.RaisePropertyChanged(nameof(CurrentReadonly));
                    this.RaisePropertyChanged(nameof(OffsetView));
                    if (stream.GetAvaliable() > 10000)
                    {
                        var samples = stream.GetCurrentSamples();
                        AudioLoudness = samples.Select(x => x.Abs()).Sum() / samples.Length;
                        if (AudioLoudness > 0.8)
                        {
                            Debugger.Break();
                        }
                        this.RaisePropertyChanged(nameof(AudioLoudness));
                    }
                }
            };
            dt.Start();
        }

        public IAudioManager.IAudioDevice[] Devices => am.GetAudioDevices();
        public IAudioManager.IAudioDevice? SelectedDevice { get; set; }
    }
}