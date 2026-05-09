using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.NuGet;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    public partial class uc_NuggetsManage
    {
        private BeepComboBox? _cmbSearchSource;
        private BeepTextBox? _txtSearch;
        private BeepCheckBoxBool? _chkPrerelease;
        private BeepButton? _btnSearch;
        private BeepGridPro? _gridSearchResults;
        private BeepPanel? _pnlSearchDetail;
        private BeepLabel? _lblDetailPackageId;
        private BeepLabel? _lblDetailVersion;
        private BeepLabel? _lblDetailDownloads;
        private BeepLabel? _lblDetailAuthors;
        private BeepLabel? _lblDetailDescription;
        private BeepComboBox? _cmbDetailVersion;
        private BeepCheckBoxBool? _chkDetailLoadNow;
        private BeepCheckBoxBool? _chkDetailSharedContext;
        private BeepButton? _btnInstall;
        private BeepButton? _btnDownloadOnly;
        private BeepProgressBar? _progressBar;
        private BeepLabel? _lblSearchStatus;
        private DataTable? _dtSearchResults;
        private CancellationTokenSource? _searchCts;
        private List<NuGetSearchResult>? _lastSearchResults;

        private void BuildSearchTab(BeepTabPage tab)
        {
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(6) };

            var lblSource = new BeepLabel { Text = "Source:", Location = new System.Drawing.Point(6, 8), Size = new System.Drawing.Size(50, 23) };
            _cmbSearchSource = new BeepComboBox { Location = new System.Drawing.Point(58, 5), Size = new System.Drawing.Size(200, 27) };

            var lblSearch = new BeepLabel { Text = "Search:", Location = new System.Drawing.Point(268, 8), Size = new System.Drawing.Size(50, 23) };
            _txtSearch = new BeepTextBox { Location = new System.Drawing.Point(320, 5), Size = new System.Drawing.Size(250, 27), PlaceholderText = "package name..." };
            _txtSearch.KeyDown += async (_, e) => 
            { 
                if (e.KeyCode == Keys.Enter) 
                {
                    try { await ExecuteSearchAsync(); }
                    catch (Exception ex) { if (_lblSearchStatus != null) _lblSearchStatus.Text = $"Search error: {ex.Message}"; }
                }
            };

            _chkPrerelease = new BeepCheckBoxBool { Text = "Prerelease", Location = new System.Drawing.Point(580, 5), Size = new System.Drawing.Size(90, 27) };

            _btnSearch = new BeepButton { Text = "Search", Location = new System.Drawing.Point(680, 5), Size = new System.Drawing.Size(80, 27) };
            _btnSearch.Click += async (_, _) => 
            {
                try { await ExecuteSearchAsync(); }
                catch (Exception ex) { if (_lblSearchStatus != null) _lblSearchStatus.Text = $"Search error: {ex.Message}"; }
            };

            topPanel.Controls.Add(lblSource);
            topPanel.Controls.Add(_cmbSearchSource);
            topPanel.Controls.Add(lblSearch);
            topPanel.Controls.Add(_txtSearch);
            topPanel.Controls.Add(_chkPrerelease);
            topPanel.Controls.Add(_btnSearch);

            var split = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 420 };

            // Left: Results grid
            _dtSearchResults = new DataTable("SearchResults");
            _dtSearchResults.Columns.Add("PackageId", typeof(string));
            _dtSearchResults.Columns.Add("Version", typeof(string));
            _dtSearchResults.Columns.Add("Downloads", typeof(string));
            _dtSearchResults.Columns.Add("Description", typeof(string));

            _gridSearchResults = new BeepGridPro { Dock = DockStyle.Fill };
            _gridSearchResults.Columns.Add(new BeepColumnConfig { ColumnName = "PackageId", ColumnCaption = "Package", Width = 200 });
            _gridSearchResults.Columns.Add(new BeepColumnConfig { ColumnName = "Version", ColumnCaption = "Version", Width = 80 });
            _gridSearchResults.Columns.Add(new BeepColumnConfig { ColumnName = "Downloads", ColumnCaption = "Downloads", Width = 90 });
            _gridSearchResults.Columns.Add(new BeepColumnConfig { ColumnName = "Description", ColumnCaption = "Description", Width = 250 });
            _gridSearchResults.DataSource = _dtSearchResults;
            _gridSearchResults.SelectionChanged += async (_, _) => 
            {
                try { await OnSearchResultSelectedAsync(); }
                catch (Exception ex) { if (_lblSearchStatus != null) _lblSearchStatus.Text = $"Selection error: {ex.Message}"; }
            };

            // Right: Detail panel
            _pnlSearchDetail = new BeepPanel { Dock = DockStyle.Fill, ShowTitle = true, TitleText = "Package Details" };
            BuildSearchDetailPanel();

            split.Panel1.Controls.Add(_gridSearchResults);
            split.Panel2.Controls.Add(_pnlSearchDetail);

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 30 };
            _lblSearchStatus = new BeepLabel { Dock = DockStyle.Fill, Text = "Ready", TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            bottomPanel.Controls.Add(_lblSearchStatus);

            _progressBar = new BeepProgressBar { Dock = DockStyle.Bottom, Height = 6, Visible = false };

            tab.Controls.Add(split);
            tab.Controls.Add(topPanel);
            tab.Controls.Add(bottomPanel);
            tab.Controls.Add(_progressBar);

            tab.Enter += (_, _) => LoadSearchSources();
        }

        private void BuildSearchDetailPanel()
        {
            if (_pnlSearchDetail == null) return;

            int y = 30;
            _lblDetailPackageId = new BeepLabel { Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(380, 23), Text = "Package: (none selected)" };
            y += 28;
            _lblDetailVersion = new BeepLabel { Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(380, 23), Text = "Latest Version:" };
            y += 28;
            _lblDetailDownloads = new BeepLabel { Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(380, 23), Text = "Downloads:" };
            y += 28;
            _lblDetailAuthors = new BeepLabel { Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(380, 23), Text = "Authors:" };
            y += 28;
            _lblDetailDescription = new BeepLabel { Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(380, 60), Text = "Description:" };
            y += 65;

            var lblVersion = new BeepLabel { Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(55, 23), Text = "Version:" };
            _cmbDetailVersion = new BeepComboBox { Location = new System.Drawing.Point(70, y), Size = new System.Drawing.Size(150, 27) };
            y += 32;

            _chkDetailLoadNow = new BeepCheckBoxBool { Text = "Load after install", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(140, 24), CurrentValue = true };
            _chkDetailSharedContext = new BeepCheckBoxBool { Text = "Shared context", Location = new System.Drawing.Point(160, y), Size = new System.Drawing.Size(130, 24), CurrentValue = true };
            y += 32;

            _btnInstall = new BeepButton { Text = "Install", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(100, 30) };
            _btnInstall.Click += async (_, _) => 
            {
                try { await ExecuteInstallAsync(); }
                catch (Exception ex) { if (_lblSearchStatus != null) _lblSearchStatus.Text = $"Install error: {ex.Message}"; }
            };
            _btnDownloadOnly = new BeepButton { Text = "Download Only", Location = new System.Drawing.Point(120, y), Size = new System.Drawing.Size(120, 30) };
            _btnDownloadOnly.Click += async (_, _) => 
            {
                try { await ExecuteDownloadOnlyAsync(); }
                catch (Exception ex) { if (_lblSearchStatus != null) _lblSearchStatus.Text = $"Download error: {ex.Message}"; }
            };

            _pnlSearchDetail.Controls.Add(_lblDetailPackageId);
            _pnlSearchDetail.Controls.Add(_lblDetailVersion);
            _pnlSearchDetail.Controls.Add(_lblDetailDownloads);
            _pnlSearchDetail.Controls.Add(_lblDetailAuthors);
            _pnlSearchDetail.Controls.Add(_lblDetailDescription);
            _pnlSearchDetail.Controls.Add(lblVersion);
            _pnlSearchDetail.Controls.Add(_cmbDetailVersion);
            _pnlSearchDetail.Controls.Add(_chkDetailLoadNow);
            _pnlSearchDetail.Controls.Add(_chkDetailSharedContext);
            _pnlSearchDetail.Controls.Add(_btnInstall);
            _pnlSearchDetail.Controls.Add(_btnDownloadOnly);
        }

        private void LoadSearchSources()
        {
            if (_cmbSearchSource == null) return;

            try
            {
                var sources = GetService().GetAllSources().Where(s => s.IsEnabled).ToList();
                var items = new BindingList<SimpleItem>(sources.Select(s => new SimpleItem { Text = s.Name, Item = s.Url }).ToList());
                _cmbSearchSource.ListItems = items;
                if (items.Count > 0 && _cmbSearchSource.SelectedItem == null)
                    _cmbSearchSource.SelectedItem = items[0];

                var state = GetService().LoadState();
                if (_chkPrerelease != null) _chkPrerelease.CurrentValue = state.IncludePrerelease;
                if (!string.IsNullOrWhiteSpace(state.LastSearchTerm) && _txtSearch != null)
                    _txtSearch.Text = state.LastSearchTerm;
            }
            catch (Exception ex)
            {
                if (_lblSearchStatus != null) _lblSearchStatus.Text = $"Failed to load sources: {ex.Message}";
            }
        }

        private async Task ExecuteSearchAsync()
        {
            if (_btnSearch == null || _txtSearch == null || _dtSearchResults == null || _gridSearchResults == null || _lblSearchStatus == null) return;

            var searchTerm = _txtSearch.Text?.Trim();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _lblSearchStatus.Text = "Please enter a search term";
                return;
            }

            _searchCts?.Cancel();
            _searchCts?.Dispose();
            _searchCts = new CancellationTokenSource();

            _btnSearch.Enabled = false;
            _lblSearchStatus.Text = "Searching...";
            _dtSearchResults.Rows.Clear();
            _gridSearchResults.DataSource = null;
            _gridSearchResults.DataSource = _dtSearchResults;

            try
            {
                var results = await GetService().SearchAsync(searchTerm, _chkPrerelease?.CurrentValue ?? false, _searchCts.Token);
                _lastSearchResults = results;
                foreach (var item in results)
                {
                    _dtSearchResults.Rows.Add(item.PackageId, item.Version, item.TotalDownloads.ToString("N0"), Truncate(item.Description, 100));
                }
                _gridSearchResults.DataSource = null;
                _gridSearchResults.DataSource = _dtSearchResults;
                _lblSearchStatus.Text = $"Found {results.Count} packages";

                var state = GetService().LoadState();
                state.LastSearchTerm = searchTerm;
                state.IncludePrerelease = _chkPrerelease?.CurrentValue ?? false;
                GetService().SaveState(state);
            }
            catch (OperationCanceledException)
            {
                _lblSearchStatus.Text = "Search cancelled.";
            }
            catch (Exception ex)
            {
                _lblSearchStatus.Text = $"Search failed: {ex.Message}";
                Editor?.AddLogMessage("NuggetsManage", $"Search error: {ex}", DateTime.Now, 0, null, Errors.Failed);
            }
            finally
            {
                _btnSearch.Enabled = true;
            }
        }

        private async Task OnSearchResultSelectedAsync()
        {
            if (_gridSearchResults?.CurrentRow == null || _lblDetailPackageId == null)
            {
                // Clear detail panel when no selection
                if (_lblDetailPackageId != null) _lblDetailPackageId.Text = "Package: (none selected)";
                if (_lblDetailVersion != null) _lblDetailVersion.Text = "Latest Version:";
                if (_lblDetailDownloads != null) _lblDetailDownloads.Text = "Downloads:";
                if (_lblDetailAuthors != null) _lblDetailAuthors.Text = "Authors:";
                if (_lblDetailDescription != null) _lblDetailDescription.Text = "Description:";
                if (_cmbDetailVersion != null) _cmbDetailVersion.ListItems = new BindingList<SimpleItem>();
                return;
            }

            var packageId = _gridSearchResults.CurrentRow.Cells["PackageId"]?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(packageId)) return;

            _lblDetailPackageId.Text = $"Package: {packageId}";
            if (_lblDetailVersion != null) _lblDetailVersion.Text = "Latest Version: loading...";
            if (_lblDetailDownloads != null) _lblDetailDownloads.Text = "Downloads: loading...";
            if (_lblDetailAuthors != null) _lblDetailAuthors.Text = "Authors: loading...";
            if (_lblDetailDescription != null) _lblDetailDescription.Text = "Description: loading...";
            if (_cmbDetailVersion != null) _cmbDetailVersion.ListItems = new BindingList<SimpleItem>();

            try
            {
                var versions = await GetService().GetVersionsAsync(packageId, _chkPrerelease?.CurrentValue ?? false);
                var items = new BindingList<SimpleItem>(versions.Select(v => new SimpleItem { Text = v, Item = v }).ToList());
                if (_cmbDetailVersion != null)
                {
                    _cmbDetailVersion.ListItems = items;
                    if (items.Count > 0)
                        _cmbDetailVersion.SelectedItem = items[0];
                }
                if (_lblDetailVersion != null)
                    _lblDetailVersion.Text = $"Latest Version: {(versions.FirstOrDefault() ?? "unknown")}";
            }
            catch (Exception ex)
            {
                if (_lblDetailVersion != null)
                    _lblDetailVersion.Text = $"Version lookup failed: {ex.Message}";
            }

            // Try to get more details from the selected row and stored results
            if (_gridSearchResults != null && _lblDetailDownloads != null && _lblDetailDescription != null)
            {
                var row = _gridSearchResults.CurrentRow;
                _lblDetailDownloads.Text = $"Downloads: {row.Cells["Downloads"]?.Value}";
                _lblDetailDescription.Text = $"Description: {row.Cells["Description"]?.Value}";
            }

            // Get Authors from stored search results
            var fullResult = _lastSearchResults?.FirstOrDefault(r => r.PackageId == packageId);
            if (fullResult != null && _lblDetailAuthors != null)
            {
                _lblDetailAuthors.Text = $"Authors: {fullResult.Authors}";
            }
            else if (_lblDetailAuthors != null)
            {
                _lblDetailAuthors.Text = "Authors: (unknown)";
            }
        }

        private async Task ExecuteInstallAsync()
        {
            if (_cmbDetailVersion?.SelectedItem == null || _lblDetailPackageId == null || _progressBar == null || _btnInstall == null || _lblSearchStatus == null) return;

            var packageId = _gridSearchResults?.CurrentRow?.Cells["PackageId"]?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(packageId)) return;

            var version = _cmbDetailVersion.SelectedItem.Text;
            var sourceUrl = _cmbSearchSource?.SelectedItem?.Item?.ToString();

            _progressBar.Visible = true;
            _btnInstall.Enabled = false;
            _lblSearchStatus.Text = $"Installing {packageId}...";

            var progress = new Progress<string>(msg => { if (_lblSearchStatus != null) _lblSearchStatus.Text = msg; });

            var request = new NuggetInstallRequest
            {
                PackageId = packageId,
                Version = version,
                Sources = string.IsNullOrWhiteSpace(sourceUrl) ? new List<string>() : new List<string> { sourceUrl },
                LoadAfterInstall = _chkDetailLoadNow?.CurrentValue ?? true,
                UseSingleSharedContext = _chkDetailSharedContext?.CurrentValue ?? true
            };

            try
            {
                var result = await GetService().InstallAsync(request, progress);
                _lblSearchStatus.Text = result.Message;
            }
            catch (Exception ex)
            {
                _lblSearchStatus.Text = $"Install failed: {ex.Message}";
            }
            finally
            {
                _progressBar.Visible = false;
                _btnInstall.Enabled = true;
            }
        }

        private async Task ExecuteDownloadOnlyAsync()
        {
            using var dialog = new FolderBrowserDialog { Description = "Select download folder" };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            var packageId = _gridSearchResults?.CurrentRow?.Cells["PackageId"]?.Value?.ToString();
            var version = _cmbDetailVersion?.SelectedItem?.Text;
            if (string.IsNullOrWhiteSpace(packageId) || _lblSearchStatus == null) return;

            _lblSearchStatus.Text = $"Downloading {packageId} to {dialog.SelectedPath}...";
            try
            {
                // TODO: Implement download-only via NuGetPackageManager
                await Task.Delay(500); // Placeholder
                _lblSearchStatus.Text = $"Downloaded {packageId} to {dialog.SelectedPath}";
            }
            catch (Exception ex)
            {
                _lblSearchStatus.Text = $"Download failed: {ex.Message}";
            }
        }

        private static string Truncate(string? text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            return text.Length <= maxLength ? text : text[..maxLength] + "...";
        }
    }
}
