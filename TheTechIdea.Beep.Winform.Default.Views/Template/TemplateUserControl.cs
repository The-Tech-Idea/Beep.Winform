﻿
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using System.Reflection;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Container.Services;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Converters;


namespace TheTechIdea.Beep.Winform.Default.Views.Template
{
    public partial class TemplateUserControl: UserControl,IDM_Addin
    {
     
        public IAppManager appManager;
        public IDMEEditor Editor { get; }
        public TemplateUserControl()
        {
            InitializeComponent();
            Details = new AddinDetails();
            Dependencies = new Dependencies();
            Details.ObjectType = "UserControl";
        }
        public TemplateUserControl(IBeepService service) : base()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            Details = new AddinDetails();
            Dependencies = new Dependencies();
            Details.ObjectType = "UserControl";
            Dependencies.DMEEditor = service.DMEEditor;
            Dependencies.Logger = service.lg;
            Editor = service.DMEEditor;
            appManager = service.vis;
            beepService = service;
            Dependencies.DMEEditor = beepService.DMEEditor;
            BeepThemesManager.ThemeChanged += BeepThemesManager_ThemeChanged;
            Theme = BeepThemesManager.CurrentTheme;
        }
        #region "IDM_Addin Implementation"
        protected  IBeepService? beepService;

        protected EnumBeepThemes _themeEnum = EnumBeepThemes.DefaultTheme;
        protected BeepTheme _currentTheme = BeepThemesManager.DefaultTheme;
        [Browsable(true)]
        [TypeConverter(typeof(ThemeEnumConverter))]
        public EnumBeepThemes Theme
        {
            get => _themeEnum;
            set
            {
                _themeEnum = value;
                _currentTheme = BeepThemesManager.GetTheme(value);
                //      this.ApplyTheme();
                ApplyTheme();
            }
        }
        private void BeepThemesManager_ThemeChanged(object? sender, ThemeChangeEventsArgs e)
        {
            Theme = e.NewTheme;

            if (Theme != BeepThemesManager.CurrentTheme) { Theme = BeepThemesManager.CurrentTheme;  }
            ApplyTheme();
        }

        public AddinDetails Details { get; set; }
        public Dependencies Dependencies { get; set; }
        public string GuidID { get; set; }

        public event EventHandler OnStart;
        public event EventHandler OnStop;
        public event EventHandler<ErrorEventArgs> OnError;


        public virtual void Configure(Dictionary<string, object> settings)
        {
            if (Theme != BeepThemesManager.CurrentTheme) { Theme = BeepThemesManager.CurrentTheme;  }
            ApplyTheme();
        }

        public virtual void Dispose()
        {

        }

        public virtual string GetErrorDetails()
        {
            // if error occured return the error details
            // create error messege sring 
            string errormessage = "";
            if (Editor.ErrorObject != null)
            {
                if (Editor.ErrorObject.Errors.Count > 0)
                {
                    foreach (var item in Editor.ErrorObject.Errors)
                    {
                        errormessage += item.Message + "\n";
                    }
                }
            }

            return errormessage;
        }

        public virtual void Initialize()
        {

        }

        public virtual void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            
        }

        public virtual void Resume()
        {

        }

        public virtual void Run(IPassedArgs pPassedarg)
        {

        }

        public virtual void Run(params object[] args)
        {

        }

        public virtual Task<IErrorsInfo> RunAsync(IPassedArgs pPassedarg)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                Editor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return Task.FromResult(Editor.ErrorObject);
        }

        public virtual Task<IErrorsInfo> RunAsync(params object[] args)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name; // Retrieves "PrintGrid"
                Editor.AddLogMessage("Beep", $"in {methodName} Error : {ex.Message}", DateTime.Now, -1, null, Errors.Failed);
            }
            return Task.FromResult(Editor.ErrorObject);
        }

        public virtual void SetError(string message)
        {

        }

        public virtual void Suspend()
        {

        }
        public void ApplyTheme()
        {
            foreach (Control item in this.Controls)
            {
                // check if item is a usercontrol
                if (item is IBeepUIComponent)
                {
                    // apply theme to usercontrol
                    ((IBeepUIComponent)item).Theme = Theme;
                    // ((IBeepUIComponent)item).ApplyTheme();

                }
            }

        }
        #endregion "IDM_Addin Implementation"
        public virtual void ResumeFormLayout()
        {
            return;
          this.ResumeLayout(false);
            this.PerformLayout(); // Ensure layout is recalculated
            foreach (Control ctrl in this.Controls)
            {
                ctrl.ResumeLayout(false);
                ctrl.PerformLayout();
                if (ctrl is IBeepUIComponent bp)
                {
                    bp.ResumeFormLayout();
                }
                if (ctrl.Dock == DockStyle.Fill)
                {
                    ctrl.Size = this.Size;
                }
            }
            this.Invalidate();
        }
        public virtual void SuspendFormLayout()
        {
            return;
       this.SuspendLayout();
            foreach (Control ctrl in this.Controls)
            {
                ctrl.SuspendLayout();
                if (ctrl is IBeepUIComponent bp)
                {
                    bp.SuspendFormLayout();
                }
            }
        }
    }
}
