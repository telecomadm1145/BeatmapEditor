using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Avalonia.VisualTree;
using BeatmapEditor.Audio;
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
    public class MainViewModel : ViewModelBase
    {
        private string _audiofile = "";
        public string AudioFile
        {
            get
            {
                return _audiofile;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _audiofile, value);
            }
        }
        private string _backgroundimage = "";
        public string BackgroundImage
        {
            get
            {
                return _backgroundimage;
            }
        }
        public ICommand ExitCommand => ReactiveCommand.Create(() => {
            Environment.Exit(0);
        });
        public Points WavePoints { get; set; } = new Points();
    }
}