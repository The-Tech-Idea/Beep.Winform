using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Reflection;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Editor.UOW;

namespace TheTechIdea.Beep.Winform.Default.Views.Template
{
    // Minimal, .NET 8-friendly template.
    public partial class TemplateUserControl : UserControl, IDM_Addin
    {
        // Common services and state expected by derived controls
        protected IBeepService? beepService;
        protected IDMEEditor? Editor { get; set; }
        protected IAppManager? appManager { get; set; }
        protected IBeepTheme _currentTheme = BeepThemesManager.GetDefaultTheme();
        protected IDataSource? ds;
        protected string? DataSourceName;
        protected string? EntityName;
        protected UnitOfWorkWrapper? uow;

        public TemplateUserControl()
        {
            InitializeComponent();
            Details = new AddinDetails { ObjectType = "UserControl" };
            Dependencies = new Dependencies();
        
        }

        public TemplateUserControl(IServiceProvider services) : this()
        {
            beepService = services.GetService<IBeepService>();
            if (beepService != null)
            {
                Dependencies.DMEEditor = beepService.DMEEditor;
                Dependencies.Logger = beepService.lg;
                Editor = beepService.DMEEditor;
                appManager = beepService.vis;

                BeepThemesManager.ThemeChanged += BeepThemesManager_ThemeChanged;
                Theme = BeepThemesManager.CurrentThemeName;
              //  MainTemplatePanel.EnableMaterialStyle = false;
                MainTemplatePanel.CanBeFocused = false;
                MainTemplatePanel.CanBeSelected = false;
                MainTemplatePanel.CanBeHovered = false;
                MainTemplatePanel.CanBePressed = false;
            }
        }

        private void BeepThemesManager_ThemeChanged(object? sender, ThemeChangeEventArgs e)
        {
            Theme = e.NewThemeName;
            if (Theme != BeepThemesManager.CurrentThemeName)
            {
                BeepThemesManager.SetCurrentTheme(Theme);
            }
            ApplyTheme();
        }

        private string _themeName = BeepThemesManager.CurrentThemeName;
        [Browsable(true)]
        [TypeConverter(typeof(ThemeEnumConverter))]
        public string Theme
        {
            get => _themeName;
            set
            {
                _themeName = value;
                _currentTheme = BeepThemesManager.GetTheme(value);
                ApplyTheme();
            }
        }

        public AddinDetails Details { get; set; }
        public Dependencies Dependencies { get; set; }
        public string GuidID { get; set; } = string.Empty;
        public bool IsConfigured { get ; set ; }=false;
        public bool IsRunning { get ; set ; }=false;
        public bool IsSuspended { get ; set ; }=false;
        public bool IsStarted { get ; set ; }=false;

        public event EventHandler? OnStart;
    public event EventHandler? OnStop;
    public event EventHandler<ErrorEventArgs>? OnError;

    // Note: All explicit scaling/DPI-blocking logic was removed to rely on default .NET 8 behavior.

        public virtual void Configure(Dictionary<string, object> settings)
        {
            if (Theme != BeepThemesManager.CurrentThemeName)
            {
                BeepThemesManager.SetCurrentTheme(Theme);
            }
            ApplyTheme();
        }

        public virtual void Initialize() { }
    public virtual void OnNavigatedTo(Dictionary<string, object> parameters) { }
        public virtual void Resume() { }
        public virtual void Run(IPassedArgs pPassedarg) { }
        public virtual void Run(params object[] args) { }
        public virtual void Suspend() { }
        public virtual void Dispose() { }

        public virtual string GetErrorDetails()
        {
            if (Editor?.ErrorObject?.Errors?.Count > 0)
            {
                return string.Join("\n", Editor.ErrorObject.Errors.Select(e => e.Message));
            }
            return string.Empty;
        }

        public virtual Task<IErrorsInfo> RunAsync(IPassedArgs pPassedarg)
        {
            try { }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod()?.Name ?? nameof(RunAsync);
                Editor?.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return Task.FromResult(Editor?.ErrorObject!);
        }

        public virtual Task<IErrorsInfo> RunAsync(params object[] args)
        {
            try { }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod()?.Name ?? nameof(RunAsync);
                Editor?.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return Task.FromResult(Editor?.ErrorObject!);
        }

        public virtual void SetError(string message) { }

        public void ApplyTheme()
        {
            foreach (Control item in Controls)
            {
                if (item is IBeepUIComponent ui)
                {
                    ui.Theme = Theme;
                }
            }
        }
    }
}
