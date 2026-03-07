using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Tools;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    // Internal wizard step — not a standalone view; no [AddinAttribute] to prevent DI auto-discovery.
    public class uc_Nugget_Step_SelectSourceAndPackage : TemplateUserControl, IWizardStepContent
    {
        private readonly BeepComboBox _cmbSources = new();
        private readonly BeepTextBox _txtSearch = new();
        private readonly BeepCheckBoxBool _chkPrerelease = new();
        private readonly BeepButton _btnSearch = new();
        private readonly BeepTextBox _txtSourceName = new();
        private readonly BeepTextBox _txtSourceUrl = new();
        private readonly BeepButton _btnAddSource = new();
        private readonly BeepButton _btnRemoveSource = new();
        private readonly BeepGridPro _resultsGrid = new();
        private readonly BeepLabel _status = new();
        private readonly DataTable _resultsTable = new("NuggetSearchResults");

        public uc_Nugget_Step_SelectSourceAndPackage(IServiceProvider services)
            : base(services)
        {
            BuildUi();
        }

        public event EventHandler<StepValidationEventArgs>? ValidationStateChanged;
        public bool IsComplete => !string.IsNullOrWhiteSpace(GetSelectedPackageId());
        public string NextButtonText => string.Empty;

        public void OnStepEnter(WizardContext context)
        {
            LoadSources();
            _txtSearch.Text = context.GetValue(NuggetsWizardKeys.SearchTerm, string.Empty);
            _chkPrerelease.CurrentValue = context.GetValue(NuggetsWizardKeys.IncludePrerelease, false);

            var selectedSource = context.GetValue(NuggetsWizardKeys.SelectedSourceUrl, string.Empty);
            if (!string.IsNullOrWhiteSpace(selectedSource))
            {
                var sourceItem = _cmbSources.ListItems?.FirstOrDefault(i =>
                    string.Equals(i.Item?.ToString(), selectedSource, StringComparison.OrdinalIgnoreCase));
                if (sourceItem != null)
                {
                    _cmbSources.SelectedItem = sourceItem;
                }
            }

            RaiseValidationState();
        }

        public void OnStepLeave(WizardContext context)
        {
            context.SetValue(NuggetsWizardKeys.SearchTerm, _txtSearch.Text ?? string.Empty);
            context.SetValue(NuggetsWizardKeys.IncludePrerelease, _chkPrerelease.CurrentValue);
            context.SetValue(NuggetsWizardKeys.SelectedSourceUrl, _cmbSources.SelectedItem?.Item?.ToString() ?? string.Empty);

            var packageId = GetSelectedPackageId();
            context.SetValue(NuggetsWizardKeys.SelectedPackageId, packageId);
            if (!string.IsNullOrWhiteSpace(packageId))
            {
                var request = context.GetValue<NuggetInstallRequest?>(NuggetsWizardKeys.InstallRequest, null) ?? new NuggetInstallRequest();
                request.PackageId = packageId;
                request.Sources = string.IsNullOrWhiteSpace(_cmbSources.SelectedItem?.Item?.ToString())
                    ? new List<string>()
                    : new List<string> { _cmbSources.SelectedItem?.Item?.ToString() ?? string.Empty };
                context.SetValue(NuggetsWizardKeys.InstallRequest, request);
            }
        }

        WizardValidationResult IWizardStepContent.Validate()
        {
            return IsComplete
                ? WizardValidationResult.Success()
                : WizardValidationResult.Error("Select a package from search results.");
        }

        public Task<WizardValidationResult> ValidateAsync()
        {
            return Task.FromResult(((IWizardStepContent)this).Validate());
        }

        private void BuildUi()
        {
            _resultsTable.Columns.Add("PackageId", typeof(string));
            _resultsTable.Columns.Add("Version", typeof(string));
            _resultsTable.Columns.Add("Downloads", typeof(string));
            _resultsTable.Columns.Add("Description", typeof(string));

            _resultsGrid.Columns.Add(new BeepColumnConfig { ColumnName = "PackageId", ColumnCaption = "Package", Width = 250 });
            _resultsGrid.Columns.Add(new BeepColumnConfig { ColumnName = "Version", ColumnCaption = "Latest", Width = 100 });
            _resultsGrid.Columns.Add(new BeepColumnConfig { ColumnName = "Downloads", ColumnCaption = "Downloads", Width = 100 });
            _resultsGrid.Columns.Add(new BeepColumnConfig { ColumnName = "Description", ColumnCaption = "Description", Width = 340 });
            _resultsGrid.DataSource = _resultsTable;
            _resultsGrid.Dock = DockStyle.Fill;
            _resultsGrid.SelectionChanged += (_, _) => RaiseValidationState();

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 66, Padding = new Padding(6, 6, 6, 0) };
            var sourceLabel = new BeepLabel { Text = "Source:", Location = new System.Drawing.Point(8, 10), Size = new System.Drawing.Size(58, 23) };
            _cmbSources.Location = new System.Drawing.Point(70, 7);
            _cmbSources.Size = new System.Drawing.Size(280, 27);
            var searchLabel = new BeepLabel { Text = "Search:", Location = new System.Drawing.Point(356, 10), Size = new System.Drawing.Size(52, 23) };
            _txtSearch.Location = new System.Drawing.Point(412, 7);
            _txtSearch.Size = new System.Drawing.Size(210, 27);
            _txtSearch.PlaceholderText = "search package";
            _chkPrerelease.Location = new System.Drawing.Point(70, 38);
            _chkPrerelease.Size = new System.Drawing.Size(140, 24);
            _chkPrerelease.Text = "Include Prerelease";
            _btnSearch.Location = new System.Drawing.Point(628, 7);
            _btnSearch.Size = new System.Drawing.Size(100, 27);
            _btnSearch.Text = "Search";
            _btnSearch.Click += async (_, _) => await SearchAsync();

            topPanel.Controls.Add(sourceLabel);
            topPanel.Controls.Add(_cmbSources);
            topPanel.Controls.Add(searchLabel);
            topPanel.Controls.Add(_txtSearch);
            topPanel.Controls.Add(_chkPrerelease);
            topPanel.Controls.Add(_btnSearch);

            var sourceManagePanel = new Panel { Dock = DockStyle.Top, Height = 36, Padding = new Padding(6, 4, 6, 0) };
            _txtSourceName.Location = new System.Drawing.Point(8, 4);
            _txtSourceName.Size = new System.Drawing.Size(160, 27);
            _txtSourceName.PlaceholderText = "source name";
            _txtSourceUrl.Location = new System.Drawing.Point(174, 4);
            _txtSourceUrl.Size = new System.Drawing.Size(370, 27);
            _txtSourceUrl.PlaceholderText = "source url";
            _btnAddSource.Location = new System.Drawing.Point(550, 4);
            _btnAddSource.Size = new System.Drawing.Size(85, 27);
            _btnAddSource.Text = "Add";
            _btnAddSource.Click += (_, _) => AddSource();
            _btnRemoveSource.Location = new System.Drawing.Point(641, 4);
            _btnRemoveSource.Size = new System.Drawing.Size(85, 27);
            _btnRemoveSource.Text = "Remove";
            _btnRemoveSource.Click += (_, _) => RemoveSource();
            sourceManagePanel.Controls.Add(_txtSourceName);
            sourceManagePanel.Controls.Add(_txtSourceUrl);
            sourceManagePanel.Controls.Add(_btnAddSource);
            sourceManagePanel.Controls.Add(_btnRemoveSource);

            _status.Dock = DockStyle.Bottom;
            _status.Height = 24;
            _status.Text = "Select source and search for a package.";
            _status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            Controls.Add(_resultsGrid);
            Controls.Add(sourceManagePanel);
            Controls.Add(topPanel);
            Controls.Add(_status);
        }

        private async Task SearchAsync()
        {
            _status.Text = "Searching...";
            _btnSearch.Enabled = false;
            try
            {
                var results = await Editor.assemblyHandler.SearchNuGetPackagesAsync(_txtSearch.Text ?? string.Empty, includePrerelease: _chkPrerelease.CurrentValue);
                _resultsTable.Rows.Clear();
                foreach (var item in results)
                {
                    _resultsTable.Rows.Add(
                        item.PackageId,
                        item.Version,
                        item.TotalDownloads.ToString("N0"),
                        Truncate(item.Description, 120));
                }

                _status.Text = $"Found {results.Count} packages.";
                RaiseValidationState();
            }
            catch (Exception ex)
            {
                _status.Text = $"Search failed: {ex.Message}";
            }
            finally
            {
                _btnSearch.Enabled = true;
            }
        }

        private void AddSource()
        {
            var name = _txtSourceName.Text?.Trim() ?? string.Empty;
            var url = _txtSourceUrl.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(url))
            {
                _status.Text = "Enter source name and URL.";
                return;
            }

            Editor.assemblyHandler.AddNuGetSource(name, url, true);
            LoadSources();
            _status.Text = $"Source '{name}' added.";
        }

        private void RemoveSource()
        {
            var name = _cmbSources.SelectedItem?.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                _status.Text = "Select a source to remove.";
                return;
            }

            Editor.assemblyHandler.RemoveNuGetSource(name);
            LoadSources();
            _status.Text = $"Source '{name}' removed.";
        }

        private void LoadSources()
        {
            var sources = Editor.assemblyHandler.GetNuGetSources();
            _cmbSources.ListItems = new BindingList<SimpleItem>(
                sources.Select(s => new SimpleItem { Text = s.Name, Item = s.Url }).ToList());
            if (_cmbSources.ListItems.Count > 0 && _cmbSources.SelectedItem == null)
            {
                _cmbSources.SelectedItem = _cmbSources.ListItems[0];
            }
        }

        private string GetSelectedPackageId()
        {
            return _resultsGrid.CurrentRow?.Cells["PackageId"]?.Value?.ToString() ?? string.Empty;
        }

        private static string Truncate(string? text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            return text.Length <= maxLength ? text : $"{text[..maxLength]}...";
        }

        private void RaiseValidationState()
        {
            var result = ((IWizardStepContent)this).Validate();
            ValidationStateChanged?.Invoke(this, new StepValidationEventArgs(result.IsValid, result.ErrorMessage));
        }
    }
}
