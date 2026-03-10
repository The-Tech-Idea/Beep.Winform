using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Linq;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public enum ImagePickerSourceType
    {
        EmbeddedResources,
        IconCatalog
    }

    public sealed class ImagePickerSelectionResult
    {
        public static ImagePickerSelectionResult Cancelled { get; } = new();
        public bool IsCancelled { get; init; } = true;
        public string SelectedPath { get; init; } = string.Empty;
        public bool IsEmbeddedResource { get; init; }
        public bool IsIconCatalog { get; init; }
        public string DisplayName { get; init; } = string.Empty;
    }

    internal sealed class PickerItem
    {
        public string DisplayName { get; init; } = string.Empty;
        public string ValuePath { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public ImagePickerSourceType Source { get; init; }
        public override string ToString() => DisplayName;
    }

    public partial class BeepImagePickerDialog : Form
    {
        private readonly Control? _ownerControl;
        private readonly bool _embed;
        private readonly IServiceProvider? _sp;
        private readonly Assembly? _resourceAssembly;
        private List<PickerItem> _allItems = new();
        private string _pendingFilePath = string.Empty;

        public string? SelectedFilePath { get; private set; }
        public string? SelectedResourcePath { get; private set; }
        public ImagePickerSelectionResult SelectionResult { get; private set; } = ImagePickerSelectionResult.Cancelled;

        public BeepImagePickerDialog(BeepImage? control, bool embed, IServiceProvider? sp, Assembly? resourceAssembly = null, string? currentPath = null)
            : this(control as Control, embed, sp, resourceAssembly, currentPath)
        {
        }

        public BeepImagePickerDialog(Control? ownerControl, bool embed, IServiceProvider? sp, Assembly? resourceAssembly = null, string? currentPath = null)
            : this()
        {
            _ownerControl = ownerControl;
            _embed = embed;
            _sp = sp;
            _resourceAssembly = resourceAssembly ?? ownerControl?.GetType().Assembly;
            Text = embed ? "Embed Image" : "Select Image";
            _txtPath.Text = currentPath ?? string.Empty;
            _pendingFilePath = currentPath ?? string.Empty;
            InitializeSources();
            WireEvents();
            PreselectCurrentPath(currentPath);
        }

        public BeepImagePickerDialog()
        {
            InitializeComponent();
            _btnOK.Enabled = false;
            _lblStatus.Text = "Select an image from sources or browse a local file.";
        }

        private void WireEvents()
        {
            _btnBrowse.Click += async (s, e) => await BrowseAsync();
            _btnEmbed.Click += async (s, e) => await ImportCurrentPathAsync();
            _cmbSource.SelectedIndexChanged += (s, e) => RefreshItems();
            _txtSearch.TextChanged += (s, e) => RefreshItems();
            _chkLimit.CheckedChanged += (s, e) => RefreshItems();
            _lstEmbedded.SelectedIndexChanged += (s, e) => UpdatePreviewForSelection();
            _lstEmbedded.DoubleClick += (s, e) => FinalizeSelection();
            _btnOK.Click += (s, e) => FinalizeSelection();

            _preview.AllowDrop = true;
            _preview.DragEnter += (s, e) => { if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy; };
            _preview.DragDrop += async (s, e) => await OnFilesDroppedAsync((string[])e.Data.GetData(DataFormats.FileDrop));

            AllowDrop = true;
            DragEnter += (s, e) => { if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy; };
            DragDrop += async (s, e) => await OnFilesDroppedAsync((string[])e.Data.GetData(DataFormats.FileDrop));
        }

        private void InitializeSources()
        {
            _cmbSource.Items.Clear();
            _cmbSource.Items.Add(ImagePickerSourceType.EmbeddedResources);
            _cmbSource.Items.Add(ImagePickerSourceType.IconCatalog);
            _cmbSource.SelectedIndex = 0;
            LoadAllItems();
            RefreshItems();
        }

        private void LoadAllItems()
        {
            _allItems = new List<PickerItem>();
            LoadEmbeddedItems();
            LoadIconCatalogItems();
        }

        private void LoadEmbeddedItems()
        {
            var asm = _resourceAssembly ?? _ownerControl?.GetType().Assembly;
            if (asm == null)
            {
                return;
            }

            IEnumerable<string> resources = asm.GetManifestResourceNames()
                .Where(IsSupportedImagePath)
                .OrderBy(r => r);

            foreach (string resource in resources)
            {
                _allItems.Add(new PickerItem
                {
                    DisplayName = resource,
                    ValuePath = resource,
                    Category = "Embedded",
                    Source = ImagePickerSourceType.EmbeddedResources
                });
            }
        }

        private void LoadIconCatalogItems()
        {
            foreach (var icon in IconCatalog.GetAllEntries())
            {
                if (!IsSupportedImagePath(icon.Path))
                {
                    continue;
                }

                _allItems.Add(new PickerItem
                {
                    DisplayName = $"[{icon.Source}/{icon.Category}] {icon.Name}",
                    ValuePath = icon.Path,
                    Category = icon.Category,
                    Source = ImagePickerSourceType.IconCatalog
                });
            }
        }

        private void RefreshItems()
        {
            if (_cmbSource.SelectedItem is not ImagePickerSourceType source)
            {
                return;
            }

            string term = _txtSearch.Text?.Trim() ?? string.Empty;
            bool svgOnly = _chkLimit.Checked;

            var items = _allItems.Where(i => i.Source == source);
            if (!string.IsNullOrWhiteSpace(term))
            {
                items = items.Where(i => i.DisplayName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0
                    || i.ValuePath.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (svgOnly)
            {
                items = items.Where(i => i.ValuePath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase));
            }

            _lstEmbedded.BeginUpdate();
            _lstEmbedded.Items.Clear();
            foreach (var item in items.OrderBy(i => i.DisplayName))
            {
                _lstEmbedded.Items.Add(item);
            }
            _lstEmbedded.EndUpdate();
            _preview.ClearImage();
            _btnOK.Enabled = false;
        }

        private async Task OnFilesDroppedAsync(string[] files)
        {
            if (files == null || files.Length == 0)
            {
                return;
            }

            string first = files.First();
            if (!IsSupportedImagePath(first))
            {
                ShowMessage("Unsupported file type.", "Image Picker");
                return;
            }

            _txtPath.Text = first;
            _pendingFilePath = first;
            SelectedFilePath = first;
            SelectedResourcePath = null;
            _btnOK.Enabled = true;
            _lblStatus.Text = "Local file selected.";
            LoadPreviewFromPath(first);
            await Task.CompletedTask;
        }

        private async Task BrowseAsync()
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Images|*.svg;*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.ico;*.webp|All Files|*.*"
            };

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            _txtPath.Text = ofd.FileName;
            _pendingFilePath = ofd.FileName;
            SelectedFilePath = ofd.FileName;
            SelectedResourcePath = null;
            _btnOK.Enabled = true;
            _lblStatus.Text = "Local file selected.";
            LoadPreviewFromPath(ofd.FileName);
            await Task.CompletedTask;
        }

        private void UpdatePreviewForSelection()
        {
            if (_lstEmbedded.SelectedItem is not PickerItem item)
            {
                _preview.ClearImage();
                _btnOK.Enabled = !string.IsNullOrWhiteSpace(_pendingFilePath);
                return;
            }

            SelectedFilePath = null;
            SelectedResourcePath = item.Source == ImagePickerSourceType.EmbeddedResources ? item.ValuePath : null;
            _pendingFilePath = item.Source == ImagePickerSourceType.IconCatalog ? item.ValuePath : _pendingFilePath;
            _lblStatus.Text = item.Source == ImagePickerSourceType.EmbeddedResources
                ? "Embedded resource selected."
                : "Icon catalog entry selected.";
            _btnOK.Enabled = true;
            LoadPreviewFromPath(item.ValuePath);
        }

        private void LoadPreviewFromPath(string path)
        {
            try
            {
                _preview.ImagePath = path;
            }
            catch
            {
                _preview.ClearImage();
            }
        }

        private void PreselectCurrentPath(string? currentPath)
        {
            if (string.IsNullOrWhiteSpace(currentPath))
            {
                return;
            }

            var item = _allItems.FirstOrDefault(i => string.Equals(i.ValuePath, currentPath, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                _cmbSource.SelectedItem = item.Source;
                for (int i = 0; i < _lstEmbedded.Items.Count; i++)
                {
                    if (_lstEmbedded.Items[i] is PickerItem pi &&
                        string.Equals(pi.ValuePath, currentPath, StringComparison.OrdinalIgnoreCase))
                    {
                        _lstEmbedded.SelectedIndex = i;
                        break;
                    }
                }
                return;
            }

            if (IsSupportedImagePath(currentPath))
            {
                _txtPath.Text = currentPath;
                _pendingFilePath = currentPath;
                SelectedFilePath = currentPath;
                _btnOK.Enabled = true;
                LoadPreviewFromPath(currentPath);
            }
        }

        private async Task ImportCurrentPathAsync()
        {
            string path = _txtPath.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                ShowMessage("Select a file first.", "Embed Image");
                return;
            }

            if (!IsSupportedImagePath(path))
            {
                ShowMessage("Unsupported file type.", "Embed Image");
                return;
            }

            await ImportFileAsync(path);
        }

        private async Task ImportFileAsync(string filePath)
        {
            try
            {
                string? projDir = FindProjectDirectory();
                if (string.IsNullOrWhiteSpace(projDir))
                {
                    ShowMessage("Could not locate project directory to embed into.", "Embed Image");
                    SelectedFilePath = filePath;
                    return;
                }

                var dict = new Dictionary<string, SimpleItem>();
                var result = await ProjectResourceEmbedder.CopyAndEmbedFileToProjectResourcesAsync(dict, filePath, projDir);
                if (!result.IsSuccess)
                {
                    string err = string.Join(Environment.NewLine, result.Errors ?? Enumerable.Empty<string>());
                    ShowMessage($"Embed failed: {err}", "Embed Image");
                    SelectedFilePath = filePath;
                    return;
                }

                string asmName = GetProjectAssemblyName(projDir);
                string rel = Path.GetRelativePath(projDir, result.FilePath).Replace("\\", "/");
                string manifest = asmName + "." + rel.Replace('/', '.');

                SelectedResourcePath = manifest;
                SelectedFilePath = null;
                _pendingFilePath = string.Empty;

                if (_allItems.All(i => !string.Equals(i.ValuePath, manifest, StringComparison.OrdinalIgnoreCase)))
                {
                    _allItems.Add(new PickerItem
                    {
                        DisplayName = manifest,
                        ValuePath = manifest,
                        Category = "Embedded",
                        Source = ImagePickerSourceType.EmbeddedResources
                    });
                }

                _cmbSource.SelectedItem = ImagePickerSourceType.EmbeddedResources;
                RefreshItems();
                SelectItemByPath(manifest);
                _lblStatus.Text = "Image embedded successfully.";
            }
            catch (Exception ex)
            {
                ShowMessage("Embed failed: " + ex.Message, "Embed Image");
                SelectedFilePath = filePath;
            }
        }

        private void SelectItemByPath(string path)
        {
            for (int i = 0; i < _lstEmbedded.Items.Count; i++)
            {
                if (_lstEmbedded.Items[i] is PickerItem item &&
                    string.Equals(item.ValuePath, path, StringComparison.OrdinalIgnoreCase))
                {
                    _lstEmbedded.SelectedIndex = i;
                    break;
                }
            }
        }

        private void FinalizeSelection()
        {
            if (_lstEmbedded.SelectedItem is PickerItem item)
            {
                SelectionResult = new ImagePickerSelectionResult
                {
                    IsCancelled = false,
                    SelectedPath = item.ValuePath,
                    IsEmbeddedResource = item.Source == ImagePickerSourceType.EmbeddedResources,
                    IsIconCatalog = item.Source == ImagePickerSourceType.IconCatalog,
                    DisplayName = item.DisplayName
                };

                if (SelectionResult.IsEmbeddedResource)
                {
                    SelectedResourcePath = item.ValuePath;
                    SelectedFilePath = null;
                }
                else
                {
                    SelectedFilePath = item.ValuePath;
                    SelectedResourcePath = null;
                }

                DialogResult = DialogResult.OK;
                return;
            }

            if (!string.IsNullOrWhiteSpace(_pendingFilePath))
            {
                SelectionResult = new ImagePickerSelectionResult
                {
                    IsCancelled = false,
                    SelectedPath = _pendingFilePath,
                    IsEmbeddedResource = false,
                    IsIconCatalog = false,
                    DisplayName = Path.GetFileName(_pendingFilePath)
                };
                SelectedFilePath = _pendingFilePath;
                SelectedResourcePath = null;
                DialogResult = DialogResult.OK;
                return;
            }

            SelectionResult = ImagePickerSelectionResult.Cancelled;
            DialogResult = DialogResult.Cancel;
        }

        private bool IsSupportedImagePath(string path)
        {
            string ext = Path.GetExtension(path ?? string.Empty).ToLowerInvariant();
            return ext is ".svg" or ".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif" or ".ico" or ".webp";
        }

        private string? FindProjectDirectory()
        {
            try
            {
                var designHost = _sp?.GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (designHost?.RootComponent is not Control)
                {
                    return null;
                }

                string probe = AppDomain.CurrentDomain.BaseDirectory;
                for (int i = 0; i < 8; i++)
                {
                    string? csproj = Directory.GetFiles(probe, "*.csproj").FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(csproj))
                    {
                        return Path.GetDirectoryName(csproj);
                    }

                    probe = Path.GetDirectoryName(probe);
                    if (string.IsNullOrWhiteSpace(probe))
                    {
                        break;
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        private string GetProjectAssemblyName(string projectDirectory)
        {
            try
            {
                string? csproj = Directory.GetFiles(projectDirectory, "*.csproj").FirstOrDefault();
                if (string.IsNullOrWhiteSpace(csproj))
                {
                    return new DirectoryInfo(projectDirectory).Name;
                }

                var xml = XDocument.Load(csproj);
                string? asmName = xml.Root?.Elements("PropertyGroup").Elements("AssemblyName").FirstOrDefault()?.Value;
                return !string.IsNullOrWhiteSpace(asmName)
                    ? asmName
                    : Path.GetFileNameWithoutExtension(csproj);
            }
            catch
            {
                return new DirectoryInfo(projectDirectory).Name;
            }
        }

        private void ShowMessage(string text, string caption)
        {
            if (_sp?.GetService(typeof(IUIService)) is IUIService uiService)
            {
                uiService.ShowMessage(text, caption, MessageBoxButtons.OK);
                return;
            }

            MessageBox.Show(this, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
