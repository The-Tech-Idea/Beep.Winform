using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// File and folder dialog methods for BeepDialogManager
    /// </summary>
    public partial class BeepDialogManager
    {
        #region Open File

        /// <summary>
        /// Shows an open file dialog (async)
        /// </summary>
        public async Task<string?> OpenFileAsync(string? filter = null, string? initialDir = null, string? title = null)
        {
            using var ofd = new OpenFileDialog();

            if (!string.IsNullOrEmpty(filter))
                ofd.Filter = filter;
            if (!string.IsNullOrEmpty(initialDir))
                ofd.InitialDirectory = initialDir;
            if (!string.IsNullOrEmpty(title))
                ofd.Title = title;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? ofd.ShowDialog(owner) : ofd.ShowDialog();

            return result == System.Windows.Forms.DialogResult.OK ? ofd.FileName : null;
        }

        /// <summary>
        /// Shows an open file dialog (sync)
        /// </summary>
        public string? OpenFile(string? filter = null, string? initialDir = null, string? title = null)
        {
            using var ofd = new OpenFileDialog();

            if (!string.IsNullOrEmpty(filter))
                ofd.Filter = filter;
            if (!string.IsNullOrEmpty(initialDir))
                ofd.InitialDirectory = initialDir;
            if (!string.IsNullOrEmpty(title))
                ofd.Title = title;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? ofd.ShowDialog(owner) : ofd.ShowDialog();

            return result == System.Windows.Forms.DialogResult.OK ? ofd.FileName : null;
        }

        /// <summary>
        /// Shows an open file dialog for multiple files (async)
        /// </summary>
        public async Task<List<string>> OpenFilesAsync(string? filter = null, string? initialDir = null, string? title = null)
        {
            using var ofd = new OpenFileDialog();

            ofd.Multiselect = true;
            if (!string.IsNullOrEmpty(filter))
                ofd.Filter = filter;
            if (!string.IsNullOrEmpty(initialDir))
                ofd.InitialDirectory = initialDir;
            if (!string.IsNullOrEmpty(title))
                ofd.Title = title;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? ofd.ShowDialog(owner) : ofd.ShowDialog();

            return result == System.Windows.Forms.DialogResult.OK ? ofd.FileNames.ToList() : new List<string>();
        }

        /// <summary>
        /// Shows an open file dialog for multiple files (sync)
        /// </summary>
        public List<string> OpenFiles(string? filter = null, string? initialDir = null, string? title = null)
        {
            using var ofd = new OpenFileDialog();

            ofd.Multiselect = true;
            if (!string.IsNullOrEmpty(filter))
                ofd.Filter = filter;
            if (!string.IsNullOrEmpty(initialDir))
                ofd.InitialDirectory = initialDir;
            if (!string.IsNullOrEmpty(title))
                ofd.Title = title;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? ofd.ShowDialog(owner) : ofd.ShowDialog();

            return result == System.Windows.Forms.DialogResult.OK ? ofd.FileNames.ToList() : new List<string>();
        }

        #endregion

        #region Save File

        /// <summary>
        /// Shows a save file dialog (async)
        /// </summary>
        public async Task<string?> SaveFileAsync(string? filter = null, string? initialDir = null, string? defaultFileName = null, string? title = null)
        {
            using var sfd = new SaveFileDialog();

            if (!string.IsNullOrEmpty(filter))
                sfd.Filter = filter;
            if (!string.IsNullOrEmpty(initialDir))
                sfd.InitialDirectory = initialDir;
            if (!string.IsNullOrEmpty(defaultFileName))
                sfd.FileName = defaultFileName;
            if (!string.IsNullOrEmpty(title))
                sfd.Title = title;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? sfd.ShowDialog(owner) : sfd.ShowDialog();

            return result == System.Windows.Forms.DialogResult.OK ? sfd.FileName : null;
        }

        /// <summary>
        /// Shows a save file dialog (sync)
        /// </summary>
        public string? SaveFile(string? filter = null, string? initialDir = null, string? defaultFileName = null, string? title = null)
        {
            using var sfd = new SaveFileDialog();

            if (!string.IsNullOrEmpty(filter))
                sfd.Filter = filter;
            if (!string.IsNullOrEmpty(initialDir))
                sfd.InitialDirectory = initialDir;
            if (!string.IsNullOrEmpty(defaultFileName))
                sfd.FileName = defaultFileName;
            if (!string.IsNullOrEmpty(title))
                sfd.Title = title;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? sfd.ShowDialog(owner) : sfd.ShowDialog();

            return result == System.Windows.Forms.DialogResult.OK ? sfd.FileName : null;
        }

        /// <summary>
        /// Shows a save file dialog with overwrite confirmation
        /// </summary>
        public string? SaveFileWithConfirm(string? filter = null, string? initialDir = null, string? defaultFileName = null, string? title = null)
        {
            var path = SaveFile(filter, initialDir, defaultFileName, title);

            if (path != null && File.Exists(path))
            {
                var confirm = ConfirmSync("Confirm Overwrite", $"The file '{Path.GetFileName(path)}' already exists. Do you want to overwrite it?");
                if (!confirm)
                    return null;
            }

            return path;
        }

        #endregion

        #region Select Folder

        /// <summary>
        /// Shows a folder browser dialog (async)
        /// </summary>
        public async Task<string?> SelectFolderAsync(string? title = null, string? initialDir = null, bool allowCreate = true)
        {
            using var fbd = new FolderBrowserDialog();

            if (!string.IsNullOrEmpty(title))
                fbd.Description = title;
            if (!string.IsNullOrEmpty(initialDir))
                fbd.SelectedPath = initialDir;
            fbd.ShowNewFolderButton = allowCreate;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? fbd.ShowDialog(owner) : fbd.ShowDialog();

            return result == System.Windows.Forms.DialogResult.OK ? fbd.SelectedPath : null;
        }

        /// <summary>
        /// Shows a folder browser dialog (sync)
        /// </summary>
        public string? SelectFolder(string? title = null, string? initialDir = null, bool allowCreate = true)
        {
            using var fbd = new FolderBrowserDialog();

            if (!string.IsNullOrEmpty(title))
                fbd.Description = title;
            if (!string.IsNullOrEmpty(initialDir))
                fbd.SelectedPath = initialDir;
            fbd.ShowNewFolderButton = allowCreate;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? fbd.ShowDialog(owner) : fbd.ShowDialog();

            return result == System.Windows.Forms.DialogResult.OK ? fbd.SelectedPath : null;
        }

        #endregion

        #region Special Directories

        /// <summary>
        /// Shows a dialog to select from special directories
        /// </summary>
        public SimpleItem? SelectSpecialDirectory(string? title = null)
        {
            var dirs = GetSpecialDirectories();
            return InputSelect(title ?? "Select Directory", "Choose a special directory:", dirs);
        }

        /// <summary>
        /// Gets a list of special directories as SimpleItems
        /// </summary>
        public List<SimpleItem> GetSpecialDirectories()
        {
            return new List<SimpleItem>
            {
                new SimpleItem { Text = "Desktop", Value = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) },
                new SimpleItem { Text = "My Documents", Value = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) },
                new SimpleItem { Text = "My Pictures", Value = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) },
                new SimpleItem { Text = "My Music", Value = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) },
                new SimpleItem { Text = "My Videos", Value = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) },
                new SimpleItem { Text = "Downloads", Value = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads") },
                new SimpleItem { Text = "Program Files", Value = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) },
                new SimpleItem { Text = "AppData", Value = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) },
                new SimpleItem { Text = "Local AppData", Value = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) },
                new SimpleItem { Text = "Temp", Value = Path.GetTempPath() },
                new SimpleItem { Text = "User Profile", Value = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) }
            };
        }

        #endregion

        #region IDialogManager Implementation (File Methods)

        DialogReturn IDialogManager.LoadFileDialog(string exts, string dir, string filter)
        {
            var path = OpenFile(filter, dir);
            return new DialogReturn
            {
                Result = path != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = path ?? string.Empty,
                Submit = path != null,
                Cancel = path == null
            };
        }

        DialogReturn IDialogManager.LoadFileDialog(string exts, string dir, string filter, string initialFileName)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = filter;
            ofd.InitialDirectory = dir;
            ofd.DefaultExt = exts;
            ofd.FileName = initialFileName;

            var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
            var result = owner != null ? ofd.ShowDialog(owner) : ofd.ShowDialog();

            return new DialogReturn
            {
                Result = result == System.Windows.Forms.DialogResult.OK ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = result == System.Windows.Forms.DialogResult.OK ? ofd.FileName : string.Empty,
                Submit = result == System.Windows.Forms.DialogResult.OK,
                Cancel = result != System.Windows.Forms.DialogResult.OK
            };
        }

        List<string> IDialogManager.LoadFilesDialog(string exts, string dir, string filter)
        {
            return OpenFiles(filter, dir);
        }

        DialogReturn IDialogManager.SaveFileDialog(string exts, string dir, string filter)
        {
            var path = SaveFile(filter, dir);
            return new DialogReturn
            {
                Result = path != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = path ?? string.Empty,
                Submit = path != null,
                Cancel = path == null
            };
        }

        DialogReturn IDialogManager.SaveFileDialog(string exts, string dir, string filter, string defaultFileName)
        {
            var path = SaveFile(filter, dir, defaultFileName);
            return new DialogReturn
            {
                Result = path != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = path ?? string.Empty,
                Submit = path != null,
                Cancel = path == null
            };
        }

        DialogReturn IDialogManager.SelectFolderDialog()
        {
            var path = SelectFolder();
            return new DialogReturn
            {
                Result = path != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = path ?? string.Empty,
                Submit = path != null,
                Cancel = path == null
            };
        }

        DialogReturn IDialogManager.SelectFolderDialog(string title, string initialDir, bool allowCreate)
        {
            var path = SelectFolder(title, initialDir, allowCreate);
            return new DialogReturn
            {
                Result = path != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = path ?? string.Empty,
                Submit = path != null,
                Cancel = path == null
            };
        }

        DialogReturn IDialogManager.SelectFile(List<SimpleItem> files, string filter)
        {
            var result = InputSelect("Select File", "Choose a file:", files);
            return new DialogReturn
            {
                Result = result != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = result?.Value?.ToString() ?? string.Empty,
                Tag = result,
                Submit = result != null,
                Cancel = result == null
            };
        }

        DialogReturn IDialogManager.ConfirmOverwrite(string filePath)
        {
            var confirm = ConfirmSync("Confirm Overwrite", $"The file '{Path.GetFileName(filePath)}' already exists. Do you want to overwrite it?");
            return new DialogReturn
            {
                Result = confirm ? BeepDialogResult.Yes : BeepDialogResult.No,
                Submit = confirm,
                Cancel = !confirm
            };
        }

        DialogReturn IDialogManager.SelectSpecialDirectoriesComboBox()
        {
            var result = SelectSpecialDirectory();
            return new DialogReturn
            {
                Result = result != null ? BeepDialogResult.OK : BeepDialogResult.Cancel,
                Value = result?.Value?.ToString() ?? string.Empty,
                Tag = result,
                Submit = result != null,
                Cancel = result == null
            };
        }

        DialogReturn IDialogManager.SelectSpecialDirectoriesListBox()
        {
            return ((IDialogManager)this).SelectSpecialDirectoriesComboBox();
        }

        DialogReturn IDialogManager.SelectSpecialDirectoriesRadioGroupBox()
        {
            return ((IDialogManager)this).SelectSpecialDirectoriesComboBox();
        }

        #endregion
    }
}

