using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace BeatmapEditor.Views
{
    [TemplatePart("PART_PathBox", typeof(TextBox))]
    [TemplatePart("PART_NotExistsTip", typeof(Control))]
    [TemplatePart("PART_OpenButton", typeof(Button))]
    public class FilePicker : TemplatedControl, IDisposable
    {
        private string _filename = "";
        public string FileName
        {
            get { return _filename; }
            set
            {
                SetAndRaise(FileNameProperty, ref _filename, value); 
                if (_pathBox != null)
                    _pathBox.Text = value;
            }
        }
        public static DirectProperty<FilePicker, string> FileNameProperty = AvaloniaProperty.RegisterDirect<FilePicker, string>(nameof(FileName), o => o.FileName, (o, v) => o.FileName = v, "");
        private TextBox? _pathBox;
        private Button? _openButton;
        private Control? _tip;
        private List<IDisposable> _eventhandlers = new();

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _eventhandlers.ForEach(x => x.Dispose());
            DragDrop.SetAllowDrop(this, true);
            _eventhandlers.Add(DragDrop.DropEvent.AddClassHandler<FilePicker>((s, e2) =>
            {
                string? filename = null;
                filename ??= e2.Data.GetFileNames()?.FirstOrDefault();
                var text = e2.Data.GetText();
                if (File.Exists(text))
                    filename ??= text;
                if (filename != null)
                {
                    if (s._pathBox != null)
                        s._pathBox.Text = filename;
                    s.FileName = filename;
                }
            }));
            _tip = e.NameScope.Find<Control>("PART_NotExistsTip");
            _pathBox = e.NameScope.Find<TextBox>("PART_PathBox");
            _openButton = e.NameScope.Find<Button>("PART_OpenButton");
            if (_pathBox != null && _tip != null)
            {
                _eventhandlers.Add(_pathBox.AddDisposableHandler(TextBox.LostFocusEvent, (s, e) =>
                {
                    string? text = ((TextBox?)s)?.Text;
                    if (!(_tip.IsVisible = !(File.Exists(text) || string.IsNullOrWhiteSpace(text))) && text != null)
                    {
                        FileName = text;
                    }
                }));
                _tip.IsVisible = false;
            }
            if (_openButton != null && _pathBox != null)
            {
                _eventhandlers.Add(_openButton.AddDisposableHandler(Button.ClickEvent, async (s, e) =>
                {
                    var root = (TopLevel?)this.GetVisualRoot();
                    if (root == null)
                    {
                        throw new PlatformNotSupportedException();
                    }
                    if (!root.StorageProvider.CanOpen)
                    {
                        throw new SecurityException();
                    }
                    FilePickerOpenOptions openOptions = new FilePickerOpenOptions();
                    openOptions.AllowMultiple = false;
                    openOptions.Title = "Select a file.";
                    openOptions.FileTypeFilter = new FilePickerFileType[] { new("All files") { Patterns = new string[] { "*.*" } } };
                    var res = await root.StorageProvider.OpenFilePickerAsync(openOptions);
                    if (res.Count > 0)
                    {
                        res[0].TryGetUri(out Uri? uri);
                        if (uri != null && uri.IsFile)
                        {
                            _pathBox.Text = uri.AbsolutePath;
                            FileName = uri.AbsolutePath;
                            if (_tip != null)
                                _tip.IsVisible = false;
                        }
                    }
                }));
            }
        }
        ~FilePicker()
        {
            Dispose();
        }
        public void Dispose()
        {
            _eventhandlers.ForEach(x => x.Dispose());
        }
    }
}
