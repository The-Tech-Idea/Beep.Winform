using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    public partial class uc_NuggetsManage
    {
        private BeepTextBox? _txtInstalledFilter;
        private BeepButton? _btnInstalledRefresh;
        private BeepButton? _btnInstalledLoad;
        private BeepButton? _btnInstalledUnload;
        private BeepButton? _btnInstalledRemove;
        private BeepButton? _btnInstalledUpdate;
        private BeepGridPro? _gridInstalled;
        private BeepPanel? _pnlInstalledDetail;
        private BeepLabel? _lblInstalledDetailPackage;
        private BeepLabel? _lblInstalledDetailVersion;
        private BeepLabel? _lblInstalledDetailStatus;
        private BeepLabel? _lblInstalledDetailSource;
        private BeepLabel? _lblInstalledDetailPath;
        private BeepCheckBoxBool? _chkInstalledStartup;
        private BeepLabel? _lblInstalledStatus;
        private DataTable? _dtInstalled;

        private void BuildInstalledTab(BeepTabPage tab)
        {
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(6) };

            _txtInstalledFilter = new BeepTextBox { Location = new System.Drawing.Point(6, 5), Size = new System.Drawing.Size(200, 27), PlaceholderText = "Filter..." };
            _txtInstalledFilter.TextChanged += (_, _) => FilterInstalled();

            _btnInstalledRefresh = new BeepButton { Text = "Refresh", Location = new System.Drawing.Point(216, 5), Size = new System.Drawing.Size(80, 27) };
            _btnInstalledRefresh.Click += (_, _) => RefreshInstalled();

            _btnInstalledLoad = new BeepButton { Text = "Load", Location = new System.Drawing.Point(304, 5), Size = new System.Drawing.Size(80, 27) };
            _btnInstalledLoad.Click += (_, _) => LoadSelectedNugget();

            _btnInstalledUnload = new BeepButton { Text = "Unload", Location = new System.Drawing.Point(392, 5), Size = new System.Drawing.Size(80, 27) };
            _btnInstalledUnload.Click += (_, _) => UnloadSelectedNugget();

            _btnInstalledRemove = new BeepButton { Text = "Remove", Location = new System.Drawing.Point(480, 5), Size = new System.Drawing.Size(80, 27) };
            _btnInstalledRemove.Click += async (_, _) => 
            {
                try { await RemoveSelectedNuggetAsync(); }
                catch (Exception ex) { if (_lblInstalledStatus != null) _lblInstalledStatus.Text = $"Remove error: {ex.Message}"; }
            };

            _btnInstalledUpdate = new BeepButton { Text = "Update", Location = new System.Drawing.Point(568, 5), Size = new System.Drawing.Size(80, 27) };
            _btnInstalledUpdate.Click += async (_, _) => 
            {
                try { await UpdateSelectedNuggetAsync(); }
                catch (Exception ex) { if (_lblInstalledStatus != null) _lblInstalledStatus.Text = $"Update error: {ex.Message}"; }
            };

            topPanel.Controls.Add(_txtInstalledFilter);
            topPanel.Controls.Add(_btnInstalledRefresh);
            topPanel.Controls.Add(_btnInstalledLoad);
            topPanel.Controls.Add(_btnInstalledUnload);
            topPanel.Controls.Add(_btnInstalledRemove);
            topPanel.Controls.Add(_btnInstalledUpdate);

            var split = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 420 };

            _dtInstalled = new DataTable("InstalledNuggets");
            _dtInstalled.Columns.Add("PackageId", typeof(string));
            _dtInstalled.Columns.Add("Version", typeof(string));
            _dtInstalled.Columns.Add("Status", typeof(string));
            _dtInstalled.Columns.Add("Startup", typeof(string));
            _dtInstalled.Columns.Add("Source", typeof(string));
            _dtInstalled.Columns.Add("InstallPath", typeof(string));

            _gridInstalled = new BeepGridPro { Dock = DockStyle.Fill };
            _gridInstalled.Columns.Add(new BeepColumnConfig { ColumnName = "PackageId", ColumnCaption = "Package", Width = 180 });
            _gridInstalled.Columns.Add(new BeepColumnConfig { ColumnName = "Version", ColumnCaption = "Version", Width = 80 });
            _gridInstalled.Columns.Add(new BeepColumnConfig { ColumnName = "Status", ColumnCaption = "Status", Width = 80 });
            _gridInstalled.Columns.Add(new BeepColumnConfig { ColumnName = "Startup", ColumnCaption = "Startup", Width = 70 });
            _gridInstalled.Columns.Add(new BeepColumnConfig { ColumnName = "Source", ColumnCaption = "Source", Width = 150 });
            _gridInstalled.Columns.Add(new BeepColumnConfig { ColumnName = "InstallPath", ColumnCaption = "Path", Width = 200 });
            _gridInstalled.DataSource = _dtInstalled;
            _gridInstalled.SelectionChanged += (_, _) => OnInstalledSelected();

            _pnlInstalledDetail = new BeepPanel { Dock = DockStyle.Fill, ShowTitle = true, TitleText = "Nugget Details" };
            BuildInstalledDetailPanel();

            split.Panel1.Controls.Add(_gridInstalled);
            split.Panel2.Controls.Add(_pnlInstalledDetail);

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 30 };
            _lblInstalledStatus = new BeepLabel { Dock = DockStyle.Fill, Text = "Ready", TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            bottomPanel.Controls.Add(_lblInstalledStatus);

            tab.Controls.Add(split);
            tab.Controls.Add(topPanel);
            tab.Controls.Add(bottomPanel);

            tab.Enter += (_, _) => RefreshInstalled();
        }

        private void BuildInstalledDetailPanel()
        {
            if (_pnlInstalledDetail == null) return;

            int y = 30;
            _lblInstalledDetailPackage = new BeepLabel { Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(380, 23), Text = "Package: (none selected)" };
            y += 28;
            _lblInstalledDetailVersion = new BeepLabel { Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(380, 23), Text = "Version:" };
            y += 28;
            _lblInstalledDetailStatus = new BeepLabel { Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(380, 23), Text = "Status:" };
            y += 28;
            _lblInstalledDetailSource = new BeepLabel { Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(380, 23), Text = "Source:" };
            y += 28;
            _lblInstalledDetailPath = new BeepLabel { Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(380, 23), Text = "Path:" };
            y += 32;

            _chkInstalledStartup = new BeepCheckBoxBool { Text = "Enable at startup", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(150, 24) };
            _chkInstalledStartup.StateChanged += (_, _) => SetStartupFlag();

            _pnlInstalledDetail.Controls.Add(_lblInstalledDetailPackage);
            _pnlInstalledDetail.Controls.Add(_lblInstalledDetailVersion);
            _pnlInstalledDetail.Controls.Add(_lblInstalledDetailStatus);
            _pnlInstalledDetail.Controls.Add(_lblInstalledDetailSource);
            _pnlInstalledDetail.Controls.Add(_lblInstalledDetailPath);
            _pnlInstalledDetail.Controls.Add(_chkInstalledStartup);
        }

        private void RefreshInstalled()
        {
            if (_dtInstalled == null || _gridInstalled == null) return;

            _dtInstalled.Rows.Clear();
            try
            {
                var states = GetService().GetInstalledStates();
                foreach (var state in states)
                {
                    var isLoaded = GetService().IsAssemblyLoaded(state.PackageId);
                    var status = isLoaded ? "Loaded" : (string.IsNullOrWhiteSpace(state.InstallPath) || !System.IO.Directory.Exists(state.InstallPath) ? "Missing" : "Unloaded");
                    _dtInstalled.Rows.Add(
                        state.PackageId,
                        state.Version,
                        status,
                        state.IsEnabledAtStartup ? "Yes" : "No",
                        state.Source,
                        state.InstallPath);
                }

                if (_lblInstalledStatus != null) _lblInstalledStatus.Text = $"{states.Count} nuggets";
            }
            catch (Exception ex)
            {
                if (_lblInstalledStatus != null) _lblInstalledStatus.Text = $"Refresh failed: {ex.Message}";
            }
        }

        private void FilterInstalled()
        {
            if (_dtInstalled == null || _txtInstalledFilter == null) return;
            var filter = _txtInstalledFilter.Text?.ToLowerInvariant() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(filter))
            {
                RefreshInstalled();
                return;
            }

            _dtInstalled.Rows.Clear();
            try
            {
                var states = GetService().GetInstalledStates().Where(s => s.PackageId.ToLowerInvariant().Contains(filter)).ToList();
                foreach (var state in states)
                {
                    var isLoaded = GetService().IsAssemblyLoaded(state.PackageId);
                    var status = isLoaded ? "Loaded" : (string.IsNullOrWhiteSpace(state.InstallPath) || !System.IO.Directory.Exists(state.InstallPath) ? "Missing" : "Unloaded");
                    _dtInstalled.Rows.Add(state.PackageId, state.Version, status, state.IsEnabledAtStartup ? "Yes" : "No", state.Source, state.InstallPath);
                }
            }
            catch (Exception ex)
            {
                if (_lblInstalledStatus != null) _lblInstalledStatus.Text = $"Filter failed: {ex.Message}";
            }
        }

        private void OnInstalledSelected()
        {
            if (_gridInstalled?.CurrentRow == null)
            {
                // Clear detail panel when no selection
                if (_lblInstalledDetailPackage != null) _lblInstalledDetailPackage.Text = "Package: (none selected)";
                if (_lblInstalledDetailVersion != null) _lblInstalledDetailVersion.Text = "Version:";
                if (_lblInstalledDetailStatus != null) _lblInstalledDetailStatus.Text = "Status:";
                if (_lblInstalledDetailSource != null) _lblInstalledDetailSource.Text = "Source:";
                if (_lblInstalledDetailPath != null) _lblInstalledDetailPath.Text = "Path:";
                if (_chkInstalledStartup != null) _chkInstalledStartup.CurrentValue = false;
                return;
            }

            var packageId = _gridInstalled.CurrentRow.Cells["PackageId"]?.Value?.ToString() ?? string.Empty;
            var version = _gridInstalled.CurrentRow.Cells["Version"]?.Value?.ToString() ?? string.Empty;
            var status = _gridInstalled.CurrentRow.Cells["Status"]?.Value?.ToString() ?? string.Empty;
            var source = _gridInstalled.CurrentRow.Cells["Source"]?.Value?.ToString() ?? string.Empty;
            var path = _gridInstalled.CurrentRow.Cells["InstallPath"]?.Value?.ToString() ?? string.Empty;
            var startup = _gridInstalled.CurrentRow.Cells["Startup"]?.Value?.ToString() ?? "No";

            if (_lblInstalledDetailPackage != null) _lblInstalledDetailPackage.Text = $"Package: {packageId}";
            if (_lblInstalledDetailVersion != null) _lblInstalledDetailVersion.Text = $"Version: {version}";
            if (_lblInstalledDetailStatus != null) _lblInstalledDetailStatus.Text = $"Status: {status}";
            if (_lblInstalledDetailSource != null) _lblInstalledDetailSource.Text = $"Source: {source}";
            if (_lblInstalledDetailPath != null) _lblInstalledDetailPath.Text = $"Path: {path}";

            if (_chkInstalledStartup != null)
                _chkInstalledStartup.CurrentValue = string.Equals(startup, "Yes", StringComparison.OrdinalIgnoreCase);
        }

        private void LoadSelectedNugget()
        {
            var packageId = GetSelectedInstalledPackageId();
            if (string.IsNullOrWhiteSpace(packageId)) return;

            try
            {
                var result = GetService().LoadNugget(packageId);
                if (_lblInstalledStatus != null) _lblInstalledStatus.Text = result ? $"Loaded '{packageId}'" : $"Failed to load '{packageId}'";
                RefreshInstalled();
            }
            catch (Exception ex)
            {
                if (_lblInstalledStatus != null) _lblInstalledStatus.Text = $"Load error: {ex.Message}";
            }
        }

        private void UnloadSelectedNugget()
        {
            var packageId = GetSelectedInstalledPackageId();
            if (string.IsNullOrWhiteSpace(packageId)) return;

            try
            {
                var result = GetService().UnloadNugget(packageId);
                if (_lblInstalledStatus != null) _lblInstalledStatus.Text = result ? $"Unloaded '{packageId}'" : $"Failed to unload '{packageId}'";
                RefreshInstalled();
            }
            catch (Exception ex)
            {
                if (_lblInstalledStatus != null) _lblInstalledStatus.Text = $"Unload error: {ex.Message}";
            }
        }

        private async System.Threading.Tasks.Task RemoveSelectedNuggetAsync()
        {
            var packageId = GetSelectedInstalledPackageId();
            if (string.IsNullOrWhiteSpace(packageId)) return;

            var result = MessageBox.Show($"Remove '{packageId}'? This will unload and delete files.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            try
            {
                await GetService().RemoveAsync(packageId);
                if (_lblInstalledStatus != null) _lblInstalledStatus.Text = $"Removed '{packageId}'";
                RefreshInstalled();
            }
            catch (Exception ex)
            {
                if (_lblInstalledStatus != null) _lblInstalledStatus.Text = $"Remove error: {ex.Message}";
            }
        }

        private async System.Threading.Tasks.Task UpdateSelectedNuggetAsync()
        {
            var packageId = GetSelectedInstalledPackageId();
            if (string.IsNullOrWhiteSpace(packageId)) return;

            if (_lblInstalledStatus != null) _lblInstalledStatus.Text = $"Updating '{packageId}'...";
            try
            {
                var installResult = await GetService().UpdateAsync(packageId);
                if (_lblInstalledStatus != null) _lblInstalledStatus.Text = installResult.Message;
                RefreshInstalled();
            }
            catch (Exception ex)
            {
                if (_lblInstalledStatus != null) _lblInstalledStatus.Text = $"Update error: {ex.Message}";
            }
        }

        private void SetStartupFlag()
        {
            var packageId = GetSelectedInstalledPackageId();
            if (string.IsNullOrWhiteSpace(packageId) || _chkInstalledStartup == null) return;

            try
            {
                GetService().SetStartupEnabled(packageId, _chkInstalledStartup.CurrentValue);
                RefreshInstalled();
            }
            catch (Exception ex)
            {
                if (_lblInstalledStatus != null) _lblInstalledStatus.Text = $"Startup flag error: {ex.Message}";
            }
        }

        private string? GetSelectedInstalledPackageId()
        {
            return _gridInstalled?.CurrentRow?.Cells["PackageId"]?.Value?.ToString();
        }
    }
}
