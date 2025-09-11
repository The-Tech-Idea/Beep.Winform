using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Svg;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public partial class BeepImagePickerDialog : Form
    {
        private readonly BeepImage _control;
        private readonly bool _embed;
        private readonly IServiceProvider _sp;
        private string[] _allResources = Array.Empty<string>();

        public string SelectedFilePath { get; private set; }
        public string SelectedResourcePath { get; private set; }

        public BeepImagePickerDialog(BeepImage control, bool embed, IServiceProvider sp) : this()
        {
            _control = control; _embed = embed; _sp = sp;
            Text = embed ? "Embed Image" : "Select Image";
            LoadEmbedded();
        }

        public BeepImagePickerDialog()
        {
            InitializeComponent();
            WireEvents();
        }

        private void WireEvents()
        {
            _btnBrowse.Click += (s, e) => Browse();
            _lstEmbedded.SelectedIndexChanged += (s, e) => UpdatePreviewForSelected();
            _lstEmbedded.DoubleClick += (s, e) => { if (_lstEmbedded.SelectedItem != null) { SelectedResourcePath = _lstEmbedded.SelectedItem.ToString(); DialogResult = DialogResult.OK; } };
            _chkLimit.CheckedChanged += (s, e) => ApplyFilter();
            _txtSearch.TextChanged += (s, e) => ApplyFilter();
            _btnOK.Click += (s, e) => { if (string.IsNullOrWhiteSpace(SelectedFilePath) && string.IsNullOrWhiteSpace(SelectedResourcePath)) DialogResult = DialogResult.Cancel; };
            // Drag & Drop support
            _preview.AllowDrop = true;
            _preview.DragEnter += (s, e) => { if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy; };
            _preview.DragDrop += (s, e) => OnFilesDropped((string[])e.Data.GetData(DataFormats.FileDrop));
            this.DragEnter += (s, e) => { if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy; };
            this.DragDrop += (s, e) => OnFilesDropped((string[])e.Data.GetData(DataFormats.FileDrop));
        }

        private void OnFilesDropped(string[] files)
        {
            if (files == null || files.Length == 0) return;
            var first = files.First();
            if (!IsSupported(first)) return;
            _txtPath.Text = first;
            if (_embed) TryEmbed(first); else SelectedFilePath = first;
            LoadPreviewFromFile(first);
        }

        private bool IsSupported(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            return new[] { ".svg", ".png", ".jpg", ".jpeg", ".bmp" }.Contains(ext);
        }

        private void Browse()
        {
            using var ofd = new OpenFileDialog { Filter = "Images|*.svg;*.png;*.jpg;*.jpeg;*.bmp|All Files|*.*" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _txtPath.Text = ofd.FileName;
                if (_embed)
                    TryEmbed(ofd.FileName);
                else
                    SelectedFilePath = ofd.FileName;
                LoadPreviewFromFile(ofd.FileName);
            }
        }

        private void LoadPreviewFromFile(string file)
        {
            try
            {
                // BeepImage can load directly by path assignment
                _preview.ImagePath = file;
            }
            catch { _preview.ClearImage(); }
        }

        private void UpdatePreviewForSelected()
        {
            if (_lstEmbedded.SelectedItem == null) { _preview.ClearImage(); return; }
            var resName = _lstEmbedded.SelectedItem.ToString();
            SelectedResourcePath = resName;
            try
            {
                // For embedded resources set ImagePath (control resolves embedded) or set EmbeddedImagePath
                _preview.ImagePath = resName; // assumes BeepImage handles embedded naming; otherwise fallback logic may be needed
            }
            catch { _preview.ClearImage(); }
        }

        private void TryEmbed(string filePath)
        {
            try
            {
                var projDir = FindProjectDirectory();
                if (projDir == null) { SelectedFilePath = filePath; return; }
                var resourcesDir = Path.Combine(projDir, "Resources");
                Directory.CreateDirectory(resourcesDir);
                var fileName = Path.GetFileName(filePath);
                var dest = Path.Combine(resourcesDir, fileName);
                File.Copy(filePath, dest, true);
                var asmName = _control?.GetType().Assembly.GetName().Name;
                SelectedResourcePath = $"{asmName}.Resources.{fileName}";
                _lstEmbedded.Items.Add(SelectedResourcePath);
                _lstEmbedded.SelectedItem = SelectedResourcePath;
                _preview.ImagePath = filePath; // show preview from file; ImagePath can later be switched to resource string if needed
            }
            catch (Exception ex)
            {
                MessageBox.Show("Embed failed: " + ex.Message, "Embed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SelectedFilePath = filePath;
            }
        }

        private string FindProjectDirectory()
        {
            try
            {
                var designHost = _sp?.GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (designHost?.RootComponent is Control)
                {
                    var dir = AppDomain.CurrentDomain.BaseDirectory;
                    var probe = dir;
                    for (int i = 0; i < 6; i++)
                    {
                        var csproj = Directory.GetFiles(probe, "*.csproj").FirstOrDefault();
                        if (csproj != null) return Path.GetDirectoryName(csproj);
                        probe = Path.GetDirectoryName(probe);
                        if (string.IsNullOrEmpty(probe)) break;
                    }
                }
            }
            catch { }
            return null;
        }

        private void LoadEmbedded()
        {
            _lstEmbedded.Items.Clear();
            try
            {
                var asm = _control?.GetType().Assembly;
                if (asm != null)
                {
                    _allResources = asm.GetManifestResourceNames()
                        .Where(res => res.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) || res.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || res.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || res.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || res.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                        .OrderBy(r => r)
                        .ToArray();
                }
                else _allResources = Array.Empty<string>();
                ApplyFilter();
            }
            catch { }
        }

        private void ApplyFilter()
        {
            if (_allResources == null) return;
            var term = _txtSearch.Text?.Trim() ?? string.Empty;
            var svgOnly = _chkLimit.Checked;
            var filtered = _allResources.Where(r => (!svgOnly || r.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)) && (term.Length == 0 || r.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0));
            _lstEmbedded.BeginUpdate();
            _lstEmbedded.Items.Clear();
            foreach (var r in filtered) _lstEmbedded.Items.Add(r);
            _lstEmbedded.EndUpdate();
            _preview.ClearImage();
        }
    }
}
