using System;
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
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    // Internal wizard step — not a standalone view; no [AddinAttribute] to prevent DI auto-discovery.
    public class uc_Nugget_Step_InstalledManagement : TemplateUserControl, IWizardStepContent
    {
        private readonly BeepGridPro _grid = new();
        private readonly BeepButton _btnRefresh = new();
        private readonly BeepButton _btnLoad = new();
        private readonly BeepButton _btnUnload = new();
        private readonly BeepButton _btnRemoveState = new();
        private readonly BeepCheckBoxBool _chkStartup = new();
        private readonly BeepLabel _status = new();
        private readonly DataTable _table = new("InstalledNuggets");

        public uc_Nugget_Step_InstalledManagement(IServiceProvider services)
            : base(services)
        {
            BuildUi();
        }

        public event EventHandler<StepValidationEventArgs>? ValidationStateChanged;
        public bool IsComplete => true;
        public string NextButtonText => "Finish";

        public void OnStepEnter(WizardContext context)
        {
            RefreshData();
            RaiseValidationState();
        }

        public void OnStepLeave(WizardContext context)
        {
        }

        WizardValidationResult IWizardStepContent.Validate()
        {
            return WizardValidationResult.Success();
        }

        public Task<WizardValidationResult> ValidateAsync()
        {
            return Task.FromResult(WizardValidationResult.Success());
        }

        private void BuildUi()
        {
            _table.Columns.Add("PackageId", typeof(string));
            _table.Columns.Add("Version", typeof(string));
            _table.Columns.Add("Source", typeof(string));
            _table.Columns.Add("Loaded", typeof(string));
            _table.Columns.Add("Startup", typeof(string));
            _table.Columns.Add("InstallPath", typeof(string));

            _grid.Columns.Add(new BeepColumnConfig { ColumnName = "PackageId", ColumnCaption = "Package", Width = 220 });
            _grid.Columns.Add(new BeepColumnConfig { ColumnName = "Version", ColumnCaption = "Version", Width = 90 });
            _grid.Columns.Add(new BeepColumnConfig { ColumnName = "Source", ColumnCaption = "Source", Width = 180 });
            _grid.Columns.Add(new BeepColumnConfig { ColumnName = "Loaded", ColumnCaption = "Loaded", Width = 80 });
            _grid.Columns.Add(new BeepColumnConfig { ColumnName = "Startup", ColumnCaption = "Startup", Width = 80 });
            _grid.Columns.Add(new BeepColumnConfig { ColumnName = "InstallPath", ColumnCaption = "Install Path", Width = 280 });
            _grid.DataSource = _table;
            _grid.Dock = DockStyle.Fill;
            _grid.SelectionChanged += (_, _) => SyncStartupCheckbox();

            var actions = new Panel { Dock = DockStyle.Top, Height = 36, Padding = new Padding(6, 4, 6, 0) };
            _btnRefresh.Location = new System.Drawing.Point(8, 4);
            _btnRefresh.Size = new System.Drawing.Size(90, 27);
            _btnRefresh.Text = "Refresh";
            _btnRefresh.Click += (_, _) => RefreshData();

            _btnLoad.Location = new System.Drawing.Point(104, 4);
            _btnLoad.Size = new System.Drawing.Size(90, 27);
            _btnLoad.Text = "Load";
            _btnLoad.Click += (_, _) => LoadSelected();

            _btnUnload.Location = new System.Drawing.Point(200, 4);
            _btnUnload.Size = new System.Drawing.Size(90, 27);
            _btnUnload.Text = "Unload";
            _btnUnload.Click += (_, _) => UnloadSelected();

            _btnRemoveState.Location = new System.Drawing.Point(296, 4);
            _btnRemoveState.Size = new System.Drawing.Size(110, 27);
            _btnRemoveState.Text = "Remove State";
            _btnRemoveState.Click += (_, _) => RemoveSelectedState();

            _chkStartup.Location = new System.Drawing.Point(420, 6);
            _chkStartup.Size = new System.Drawing.Size(170, 24);
            _chkStartup.Text = "Enable At Startup";
            _chkStartup.StateChanged += (_, _) => SetStartupFlag();

            actions.Controls.Add(_btnRefresh);
            actions.Controls.Add(_btnLoad);
            actions.Controls.Add(_btnUnload);
            actions.Controls.Add(_btnRemoveState);
            actions.Controls.Add(_chkStartup);

            _status.Dock = DockStyle.Bottom;
            _status.Height = 24;
            _status.Text = "Manage installed nuggets.";

            Controls.Add(_grid);
            Controls.Add(actions);
            Controls.Add(_status);
        }

        private void RefreshData()
        {
            var items = Editor.assemblyHandler.GetAllNuggets();
            _table.Rows.Clear();
            foreach (var item in items)
            {
                _table.Rows.Add(
                    item.Name ?? item.Id,
                    item.Version ?? string.Empty,
                    item.SourcePath ?? string.Empty,
                    item.IsActive ? "Yes" : "No",
                    "N/A",
                    item.SourcePath ?? string.Empty);
            }

            _status.Text = $"Loaded nuggets: {items.Count}";
        }

        private void LoadSelected()
        {
            var package = GetSelectedPackageId();
            if (string.IsNullOrWhiteSpace(package))
            {
                _status.Text = "Select a package first.";
                return;
            }

            var installPath = _grid.CurrentRow?.Cells["InstallPath"]?.Value?.ToString() ?? string.Empty;
            var loaded = Editor.assemblyHandler.LoadNugget(string.IsNullOrWhiteSpace(installPath) ? package : installPath);
            _status.Text = loaded ? $"Loaded '{package}'." : $"Failed to load '{package}'.";
            RefreshData();
        }

        private void UnloadSelected()
        {
            var package = GetSelectedPackageId();
            if (string.IsNullOrWhiteSpace(package))
            {
                _status.Text = "Select a package first.";
                return;
            }

            var unloaded = Editor.assemblyHandler.UnloadNugget(package);
            _status.Text = unloaded ? $"Unloaded '{package}'." : $"Failed to unload '{package}'.";  
            RefreshData();
        }

        private void RemoveSelectedState()
        {
            var package = GetSelectedPackageId();
            if (string.IsNullOrWhiteSpace(package))
            {
                _status.Text = "Select a package first.";
                return;
            }

            Editor.assemblyHandler.UnloadNugget(package);
            _status.Text = $"Unloaded '{package}'."
;
            RefreshData();
        }

        private void SyncStartupCheckbox()
        {
            var startup = _grid.CurrentRow?.Cells["Startup"]?.Value?.ToString() ?? "No";
            _chkStartup.CurrentValue = string.Equals(startup, "Yes", StringComparison.OrdinalIgnoreCase);
        }

        private void SetStartupFlag()
        {
            _status.Text = "Startup flag persistence requires state store.";
        }

        private string GetSelectedPackageId()
        {
            return _grid.CurrentRow?.Cells["PackageId"]?.Value?.ToString() ?? string.Empty;
        }

        private void RaiseValidationState()
        {
            ValidationStateChanged?.Invoke(this, new StepValidationEventArgs(true));
        }
    }
}
