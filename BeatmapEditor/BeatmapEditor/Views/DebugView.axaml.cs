using Avalonia.Controls;
using BeatmapEditor.ViewModels;

namespace BeatmapEditor.Views
{
    public partial class DebugView : UserControl
    {
        public DebugView()
        {
#if DEBUG
            DataContext = new DebugViewModel(); // ¶ÀÁ¢µÄbug
            InitializeComponent();
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Avalonia.Input.Key.Z || e.Key == Avalonia.Input.Key.X)
                    ((DebugViewModel)DataContext).PlayTestSample();
            };
#else
            Content = "Use debug build for debug page.";
#endif
        }
    }
}
