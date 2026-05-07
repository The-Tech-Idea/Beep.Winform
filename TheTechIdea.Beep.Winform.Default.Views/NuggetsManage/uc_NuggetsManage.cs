using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    [AddinAttribute(Caption = "Nugget Manager", Name = "uc_NuggetsManage",
        misc = "Config", menu = "Configuration", addinType = AddinType.Control,
        displayType = DisplayType.InControl, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 4, RootNodeName = "Configuration", Order = 4, ID = 4,
        BranchText = "Nugget Manager", BranchType = EnumPointType.Function,
        IconImageName = "drivers.svg", BranchClass = "ADDIN",
        BranchDescription = "NuGet package search, install, and management")]
    public partial class uc_NuggetsManage : TemplateUserControl, IAddinVisSchema, IDisposable
    {
        private readonly IServiceProvider _services;
        private NuggetsManageService? _service;
        private BeepTabs? _tabs;
        private bool _disposed;

        private NuggetsManageService GetService()
        {
            if (_service == null)
            {
                if (Editor == null)
                    throw new InvalidOperationException("Editor is not available.");
                _service = new NuggetsManageService(Editor);
            }
            return _service;
        }

        public uc_NuggetsManage(IServiceProvider services) : base(services)
        {
            _services = services;
            InitializeComponent();
            Details.AddinName = "Nugget Manager";
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _service?.Dispose();
                    _searchCts?.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        #region IAddinVisSchema
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; } = string.Empty;
        public int Order { get; set; } = 4;
        public int ID { get; set; } = 4;
        public string BranchText { get; set; } = "Nugget Manager";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Function;
        public int BranchID { get; set; } = 4;
        public string IconImageName { get; set; } = "drivers.svg";
        public string BranchStatus { get; set; } = string.Empty;
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "NuGet package search, install, and management";
        public string BranchClass { get; set; } = "ADDIN";
        public string AddinName { get; set; } = "uc_NuggetsManage";
        #endregion

        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            if (Editor == null)
            {
                MessageBox.Show("Editor is not available. Cannot initialize Nugget Manager.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            RestoreLastTab();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            
            _tabs = new BeepTabs
            {
                Dock = DockStyle.Fill,
                ShowCloseButtons = false
            };

            var tabSearch = new TabPage { Text = "Search & Install", Name = "tabSearch" };
            var tabInstalled = new TabPage { Text = "Installed", Name = "tabInstalled" };
            var tabSources = new TabPage { Text = "Sources", Name = "tabSources" };
            var tabActivity = new TabPage { Text = "Activity", Name = "tabActivity" };

            BuildSearchTab(tabSearch);
            BuildInstalledTab(tabInstalled);
            BuildSourcesTab(tabSources);
            BuildActivityTab(tabActivity);

            _tabs.TabPages.Add(tabSearch);
            _tabs.TabPages.Add(tabInstalled);
            _tabs.TabPages.Add(tabSources);
            _tabs.TabPages.Add(tabActivity);

            _tabs.SelectedIndexChanged += (_, _) =>
            {
                try
                {
                    var state = GetService().LoadState();
                    state.LastActiveTabIndex = _tabs.SelectedIndex;
                    GetService().SaveState(state);
                }
                catch { /* ignore save errors */ }
            };

            Controls.Add(_tabs);
            
            ResumeLayout(true);
        }

        private void RestoreLastTab()
        {
            if (_tabs == null) return;
            try
            {
                var state = GetService().LoadState();
                if (state.LastActiveTabIndex >= 0 && state.LastActiveTabIndex < _tabs.TabPages.Count)
                {
                    _tabs.SelectedIndex = state.LastActiveTabIndex;
                }
            }
            catch { /* ignore restore errors */ }
        }
    }
}
