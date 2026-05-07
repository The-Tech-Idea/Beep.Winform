using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.NuGet;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    public partial class uc_NuggetsManage
    {
        private BeepGridPro? _gridSources;
        private BeepButton? _btnSourceAdd;
        private BeepButton? _btnSourceEdit;
        private BeepButton? _btnSourceRemove;
        private BeepButton? _btnSourceTest;
        private BeepTextBox? _txtSourceName;
        private BeepTextBox? _txtSourceUrl;
        private BeepCheckBoxBool? _chkSourceEnabled;
        private BeepButton? _btnSourceSave;
        private BeepButton? _btnSourceCancel;
        private BeepLabel? _lblSourceStatus;
        private DataTable? _dtSources;
        private NuGetSourceConfig? _editingSource;

        private void BuildSourcesTab(TabPage tab)
        {
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(6) };

            _btnSourceAdd = new BeepButton { Text = "Add", Location = new System.Drawing.Point(6, 5), Size = new System.Drawing.Size(80, 27) };
            _btnSourceAdd.Click += (_, _) => BeginAddSource();

            _btnSourceEdit = new BeepButton { Text = "Edit", Location = new System.Drawing.Point(94, 5), Size = new System.Drawing.Size(80, 27) };
            _btnSourceEdit.Click += (_, _) => BeginEditSource();

            _btnSourceRemove = new BeepButton { Text = "Remove", Location = new System.Drawing.Point(182, 5), Size = new System.Drawing.Size(80, 27) };
            _btnSourceRemove.Click += (_, _) => RemoveSource();

            _btnSourceTest = new BeepButton { Text = "Test", Location = new System.Drawing.Point(270, 5), Size = new System.Drawing.Size(80, 27) };
            _btnSourceTest.Click += async (_, _) => 
            {
                try { await TestSelectedSource(); }
                catch (Exception ex) { if (_lblSourceStatus != null) _lblSourceStatus.Text = $"Test error: {ex.Message}"; }
            };

            topPanel.Controls.Add(_btnSourceAdd);
            topPanel.Controls.Add(_btnSourceEdit);
            topPanel.Controls.Add(_btnSourceRemove);
            topPanel.Controls.Add(_btnSourceTest);

            _dtSources = new DataTable("Sources");
            _dtSources.Columns.Add("Name", typeof(string));
            _dtSources.Columns.Add("Url", typeof(string));
            _dtSources.Columns.Add("Enabled", typeof(string));
            _dtSources.Columns.Add("Type", typeof(string));

            _gridSources = new BeepGridPro { Dock = DockStyle.Fill };
            _gridSources.Columns.Add(new BeepColumnConfig { ColumnName = "Name", ColumnCaption = "Name", Width = 150 });
            _gridSources.Columns.Add(new BeepColumnConfig { ColumnName = "Url", ColumnCaption = "URL / Path", Width = 300 });
            _gridSources.Columns.Add(new BeepColumnConfig { ColumnName = "Enabled", ColumnCaption = "Enabled", Width = 70 });
            _gridSources.Columns.Add(new BeepColumnConfig { ColumnName = "Type", ColumnCaption = "Type", Width = 80 });
            _gridSources.DataSource = _dtSources;

            var editPanel = new BeepPanel { Dock = DockStyle.Bottom, Height = 120, ShowTitle = true, TitleText = "Source Details", Padding = new Padding(6) };

            _txtSourceName = new BeepTextBox { Location = new System.Drawing.Point(10, 30), Size = new System.Drawing.Size(150, 27), PlaceholderText = "Name" };
            _txtSourceUrl = new BeepTextBox { Location = new System.Drawing.Point(170, 30), Size = new System.Drawing.Size(300, 27), PlaceholderText = "URL or path" };
            _chkSourceEnabled = new BeepCheckBoxBool { Text = "Enabled", Location = new System.Drawing.Point(480, 30), Size = new System.Drawing.Size(80, 24), CurrentValue = true };

            _btnSourceSave = new BeepButton { Text = "Save", Location = new System.Drawing.Point(10, 62), Size = new System.Drawing.Size(80, 27) };
            _btnSourceSave.Click += (_, _) => SaveSource();
            _btnSourceCancel = new BeepButton { Text = "Cancel", Location = new System.Drawing.Point(98, 62), Size = new System.Drawing.Size(80, 27) };
            _btnSourceCancel.Click += (_, _) => CancelSourceEdit();

            editPanel.Controls.Add(_txtSourceName);
            editPanel.Controls.Add(_txtSourceUrl);
            editPanel.Controls.Add(_chkSourceEnabled);
            editPanel.Controls.Add(_btnSourceSave);
            editPanel.Controls.Add(_btnSourceCancel);

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 30 };
            _lblSourceStatus = new BeepLabel { Dock = DockStyle.Fill, Text = "Ready", TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            bottomPanel.Controls.Add(_lblSourceStatus);

            tab.Controls.Add(_gridSources);
            tab.Controls.Add(topPanel);
            tab.Controls.Add(editPanel);
            tab.Controls.Add(bottomPanel);

            tab.Enter += (_, _) => RefreshSources();
        }

        private void RefreshSources()
        {
            if (_dtSources == null) return;
            _dtSources.Rows.Clear();
            try
            {
                var sources = GetService().GetAllSources();
                foreach (var source in sources)
                {
                    _dtSources.Rows.Add(source.Name, source.Url, source.IsEnabled ? "Yes" : "No", source.IsLocal ? "Local" : "NuGet");
                }
                if (_lblSourceStatus != null) _lblSourceStatus.Text = $"{sources.Count} sources";
            }
            catch (Exception ex)
            {
                if (_lblSourceStatus != null) _lblSourceStatus.Text = $"Refresh failed: {ex.Message}";
            }
        }

        private void BeginAddSource()
        {
            _editingSource = null;
            if (_txtSourceName != null) _txtSourceName.Text = string.Empty;
            if (_txtSourceUrl != null) _txtSourceUrl.Text = string.Empty;
            if (_chkSourceEnabled != null) _chkSourceEnabled.CurrentValue = true;
            _txtSourceName?.Focus();
        }

        private void BeginEditSource()
        {
            if (_gridSources?.CurrentRow == null) return;
            var name = _gridSources.CurrentRow.Cells["Name"]?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(name)) return;

            try
            {
                var source = GetService().GetAllSources().FirstOrDefault(s => s.Name == name);
                if (source == null) return;

                _editingSource = source;
                if (_txtSourceName != null) _txtSourceName.Text = source.Name;
                if (_txtSourceUrl != null) _txtSourceUrl.Text = source.Url;
                if (_chkSourceEnabled != null) _chkSourceEnabled.CurrentValue = source.IsEnabled;
            }
            catch (Exception ex)
            {
                if (_lblSourceStatus != null) _lblSourceStatus.Text = $"Edit failed: {ex.Message}";
            }
        }

        private void SaveSource()
        {
            var name = _txtSourceName?.Text?.Trim() ?? string.Empty;
            var url = _txtSourceUrl?.Text?.Trim() ?? string.Empty;
            var enabled = _chkSourceEnabled?.CurrentValue ?? true;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(url))
            {
                if (_lblSourceStatus != null) _lblSourceStatus.Text = "Name and URL are required.";
                return;
            }

            try
            {
                GetService().AddSource(name, url, enabled);
                if (_lblSourceStatus != null) _lblSourceStatus.Text = $"Source '{name}' saved.";
                RefreshSources();
                _editingSource = null;
                if (_txtSourceName != null) _txtSourceName.Text = string.Empty;
                if (_txtSourceUrl != null) _txtSourceUrl.Text = string.Empty;
            }
            catch (Exception ex)
            {
                if (_lblSourceStatus != null) _lblSourceStatus.Text = $"Save failed: {ex.Message}";
            }
        }

        private void CancelSourceEdit()
        {
            _editingSource = null;
            if (_txtSourceName != null) _txtSourceName.Text = string.Empty;
            if (_txtSourceUrl != null) _txtSourceUrl.Text = string.Empty;
        }

        private void RemoveSource()
        {
            if (_gridSources?.CurrentRow == null) return;
            var name = _gridSources.CurrentRow.Cells["Name"]?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(name)) return;

            try
            {
                GetService().RemoveSource(name);
                if (_lblSourceStatus != null) _lblSourceStatus.Text = $"Source '{name}' removed.";
                RefreshSources();
            }
            catch (Exception ex)
            {
                if (_lblSourceStatus != null) _lblSourceStatus.Text = $"Remove failed: {ex.Message}";
            }
        }

        private async System.Threading.Tasks.Task TestSelectedSource()
        {
            if (_gridSources?.CurrentRow == null) return;
            var name = _gridSources.CurrentRow.Cells["Name"]?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(name)) return;

            try
            {
                var source = GetService().GetAllSources().FirstOrDefault(s => s.Name == name);
                if (source == null) return;

                if (_lblSourceStatus != null) _lblSourceStatus.Text = $"Testing '{name}'...";
                var healthy = await GetService().TestSourceAsync(source);
                if (_lblSourceStatus != null) _lblSourceStatus.Text = $"Source '{name}' is {(healthy ? "healthy" : "unreachable")}.";
            }
            catch (Exception ex)
            {
                if (_lblSourceStatus != null) _lblSourceStatus.Text = $"Test failed: {ex.Message}";
            }
        }
    }
}
