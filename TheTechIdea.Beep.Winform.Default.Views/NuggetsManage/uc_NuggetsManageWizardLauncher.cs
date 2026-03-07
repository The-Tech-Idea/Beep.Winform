using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Tools;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    [AddinAttribute(Caption = "Nugget Management Wizard", Name = "uc_NuggetsManageWizardLauncher",
        misc = "Config", menu = "Configuration", addinType = AddinType.Control,
        displayType = DisplayType.InControl, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 4, RootNodeName = "Configuration", Order = 4, ID = 4, BranchText = "Nugget Manager Wizard", BranchType = EnumPointType.Function, IconImageName = "drivers.svg", BranchClass = "ADDIN", BranchDescription = "NuGet package and nugget management wizard")]
    public partial class uc_NuggetsManageWizardLauncher : TemplateUserControl, IAddinVisSchema
    {
        private readonly IServiceProvider _services;
        private NuggetsWizardStateStore? _stateStore;
        private readonly List<(DateTime When, string Package, string Version, string Status)> _history = new();
        private DataTable? _historyDt;
        private bool _historyInitialized;
        private bool _isLoading;
        private bool _eventsConfigured;

        private NuggetsWizardStateStore GetStateStore() => _stateStore ??= new NuggetsWizardStateStore(Editor);

        public uc_NuggetsManageWizardLauncher(IServiceProvider services) : base(services)
        {
            _services = services;
            InitializeComponent();
            Details.AddinName = "Nugget Management Wizard";
        }

        #region IAddinVisSchema
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; } = string.Empty;
        public int Order { get; set; } = 4;
        public int ID { get; set; } = 4;
        public string BranchText { get; set; } = "Nugget Manager Wizard";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Function;
        public int BranchID { get; set; } = 4;
        public string IconImageName { get; set; } = "drivers.svg";
        public string BranchStatus { get; set; } = string.Empty;
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "NuGet package and nugget management wizard";
        public string BranchClass { get; set; } = "ADDIN";
        public string AddinName { get; set; } = "uc_NuggetsManageWizardLauncher";
        #endregion

        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            LoadSources();
            LoadPersistedDefaults();
            AppendLog("Ready. Configure defaults then launch the wizard.");
        }

        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
            InitializeHistoryGrid();

            if (_eventsConfigured)
            {
                return;
            }

            btnLaunchWizard.Click += (_, _) => LaunchWizard();
            btnClearLog.Click += (_, _) => txtLog.Clear();
            _eventsConfigured = true;
        }

        private void LoadSources()
        {
            if (Editor?.assemblyHandler == null) return;

            _isLoading = true;
            try
            {
                var sources = Editor.assemblyHandler.GetNuGetSources();
                var items = new BindingList<SimpleItem>(
                    sources.Where(s => s.IsEnabled)
                        .Select(s => new SimpleItem { Text = s.Name, Item = s.Url })
                        .ToList());
                cmbSource.ListItems = items;
                if (items.Count > 0)
                {
                    cmbSource.SelectedItem = items[0];
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void LoadPersistedDefaults()
        {
            if (Editor == null) return;

            var state = GetStateStore().Load();
            txtSearch.Text = state.LastSearchTerm ?? string.Empty;
            chkLoadNow.CurrentValue = state.LoadAfterInstall;
            chkSharedContext.CurrentValue = state.UseSingleSharedContext;
            if (!string.IsNullOrWhiteSpace(state.LastSourceUrl))
            {
                var item = cmbSource.ListItems?.FirstOrDefault(i =>
                    string.Equals(i.Item?.ToString(), state.LastSourceUrl, StringComparison.OrdinalIgnoreCase));
                if (item != null)
                {
                    cmbSource.SelectedItem = item;
                }
            }
        }

        private void LaunchWizard()
        {
            if (Editor?.assemblyHandler == null)
            {
                AppendLog("assemblyHandler not available.");
                return;
            }

            var selectStep = new uc_Nugget_Step_SelectSourceAndPackage(_services);
            var optionsStep = new uc_Nugget_Step_SelectVersionAndOptions(_services);
            var reviewStep = new uc_Nugget_Step_ReviewAndInstall(_services);
            var manageStep = new uc_Nugget_Step_InstalledManagement(_services);
            var wizardConfig = new WizardConfig
            {
                Key = $"NuggetsWizard_{Guid.NewGuid():N}",
                Title = "Nugget Management Wizard",
                Description = "Find, configure, install, and manage NuGet-based nuggets.",
                Style = WizardStyle.HorizontalStepper,
                ShowProgressBar = true,
                ShowStepList = true,
                AllowBack = true,
                AllowCancel = true,
                ShowInlineErrors = true,
                Steps = new List<WizardStep>
                {
                    new WizardStep { Key = "select", Title = "Select Package", Description = "Select source and package.", Content = selectStep },
                    new WizardStep { Key = "configure", Title = "Configure", Description = "Choose version and install options.", Content = optionsStep },
                    new WizardStep { Key = "review", Title = "Review", Description = "Review install request and finish behavior.", Content = reviewStep },
                    new WizardStep { Key = "manage", Title = "Manage Installed", Description = "Load/unload and maintain installed nuggets.", Content = manageStep }
                }
            };
            wizardConfig.OnProgress = (cur, total, title) => AppendLog($"[{cur}/{total}] {title}");
            wizardConfig.OnCancel = _ => AppendLog("Wizard cancelled.");
            wizardConfig.OnComplete = OnWizardComplete;
            var wizard = WizardManager.CreateWizard(wizardConfig);
            SeedContext(wizard.Context);
            var owner = FindForm();
            var result = owner == null ? wizard.ShowDialog() : wizard.ShowDialog(owner);
            AppendLog($"Wizard closed: {result}");
        }

        private void SeedContext(WizardContext context)
        {
            var sourceUrl = cmbSource.SelectedItem?.Item?.ToString() ?? string.Empty;
            var installRequest = new NuggetInstallRequest
            {
                Sources = string.IsNullOrWhiteSpace(sourceUrl) ? new List<string>() : new List<string> { sourceUrl },
                LoadAfterInstall = chkLoadNow.CurrentValue,
                UseSingleSharedContext = chkSharedContext.CurrentValue
            };

            context.SetValue(NuggetsWizardKeys.SelectedSourceUrl, sourceUrl);
            context.SetValue(NuggetsWizardKeys.SearchTerm, txtSearch.Text ?? string.Empty);
            context.SetValue(NuggetsWizardKeys.InstallRequest, installRequest);
            context.SetValue(NuggetsWizardKeys.RunInstallOnFinish, true);
        }

        private void OnWizardComplete(WizardContext context)
        {
            if (Editor?.assemblyHandler == null) return;

            var runOnFinish = context.GetValue(NuggetsWizardKeys.RunInstallOnFinish, true);
            if (!runOnFinish)
            {
                AppendLog("Run on finish disabled.");
                return;
            }

            var request = context.GetValue<NuggetInstallRequest?>(NuggetsWizardKeys.InstallRequest, null);
            if (request == null || string.IsNullOrWhiteSpace(request.PackageId))
            {
                AppendLog("No package request available from wizard context.");
                return;
            }

            AppendLog($"Installing '{request.PackageId}' '{request.Version}' ...");
            var assemblyHandler = Editor.assemblyHandler;
            _ = Task.Run(async () =>
            {
                var assemblies = await assemblyHandler.LoadNuggetFromNuGetAsync(
                    request.PackageId, request.Version, request.Sources,
                    request.UseSingleSharedContext, null, request.UseProcessHost).ConfigureAwait(false);
                var result = new NuggetInstallResult
                {
                    Success = assemblies?.Count > 0,
                    LoadedAssembliesCount = assemblies?.Count ?? 0,
                    Message = assemblies?.Count > 0
                        ? $"Installed '{request.PackageId}' \u2014 {assemblies.Count} assembly(s) loaded."
                        : $"Install of '{request.PackageId}' produced no assemblies."
                };
                if (IsHandleCreated)
                {
                    BeginInvoke(new Action(() =>
                    {
                        context.SetValue(NuggetsWizardKeys.InstallResult, result);
                        AppendLog(result.Message);
                        AddHistoryRow(request.PackageId, request.Version, result.Success ? "Success" : "Failed");
                        PersistLauncherDefaults(request);
                    }));
                }
            });
        }

        private void PersistLauncherDefaults(NuggetInstallRequest request)
        {
            if (Editor == null) return;

            var state = GetStateStore().Load();
            state.LastSearchTerm = txtSearch.Text ?? string.Empty;
            state.LastSourceUrl = request.Sources.FirstOrDefault() ?? string.Empty;
            state.LoadAfterInstall = chkLoadNow.CurrentValue;
            state.UseSingleSharedContext = chkSharedContext.CurrentValue;
            GetStateStore().Save(state);
        }

        private void InitializeHistoryGrid()
        {
            if (_historyInitialized) return;
            _historyInitialized = true;

            _historyDt = new DataTable("NuggetWizardHistory");
            _historyDt.Columns.Add("When", typeof(string));
            _historyDt.Columns.Add("Package", typeof(string));
            _historyDt.Columns.Add("Version", typeof(string));
            _historyDt.Columns.Add("Status", typeof(string));

            historyGrid.Columns.Clear();
            historyGrid.Columns.Add(new BeepColumnConfig { ColumnName = "When", ColumnCaption = "Time", Width = 90 });
            historyGrid.Columns.Add(new BeepColumnConfig { ColumnName = "Package", ColumnCaption = "Package", Width = 220 });
            historyGrid.Columns.Add(new BeepColumnConfig { ColumnName = "Version", ColumnCaption = "Version", Width = 90 });
            historyGrid.Columns.Add(new BeepColumnConfig { ColumnName = "Status", ColumnCaption = "Status", Width = 100 });
            historyGrid.DataSource = _historyDt;
        }

        private void AddHistoryRow(string package, string version, string status)
        {
            _history.Insert(0, (DateTime.Now, package, version, status));
            if (_history.Count > 50)
            {
                _history.RemoveAt(_history.Count - 1);
            }

            RefreshHistoryGrid();
        }

        private void RefreshHistoryGrid()
        {
            if (_historyDt == null)
            {
                return;
            }

            _historyDt.Rows.Clear();
            foreach (var item in _history.Take(10))
            {
                _historyDt.Rows.Add(item.When.ToString("HH:mm:ss"), item.Package, item.Version, item.Status);
            }
        }

        private void AppendLog(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            txtLog.Text = string.IsNullOrWhiteSpace(txtLog.Text)
                ? $"{DateTime.Now:HH:mm:ss}  {message}"
                : $"{txtLog.Text}{Environment.NewLine}{DateTime.Now:HH:mm:ss}  {message}";
        }
    }
}
