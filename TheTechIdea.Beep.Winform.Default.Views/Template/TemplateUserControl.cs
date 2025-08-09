using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Reflection;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Converters;


namespace TheTechIdea.Beep.Winform.Default.Views.Template
{
    public partial class TemplateUserControl: UserControl,IDM_Addin
    {
     
        public IAppManager appManager;
        public IDMEEditor Editor { get; }
        protected readonly IBeepService? beepService;
        
        // Store original positions to prevent DPI-related movement
        private readonly Dictionary<Control, Rectangle> _originalBounds = new Dictionary<Control, Rectangle>();
        private bool _isApplyingTheme = false;
        private bool _isDpiChanging = false;
        
        public TemplateUserControl()
        {
            InitializeComponent();
            Details = new AddinDetails();
            Dependencies = new Dependencies();
            Details.ObjectType = "UserControl";
        }
        public TemplateUserControl(IServiceProvider services) : base()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            Details = new AddinDetails();
            Dependencies = new Dependencies();
            Details.ObjectType = "UserControl";
            beepService = services.GetService<IBeepService>();
            Dependencies.DMEEditor = beepService.DMEEditor;
            Dependencies.Logger = beepService.lg;
            Editor = beepService.DMEEditor;
            appManager = beepService.vis;
          
            Dependencies.DMEEditor = beepService.DMEEditor;
            BeepThemesManager.ThemeChanged += BeepThemesManager_v2_ThemeChanged;
            Theme = BeepThemesManager.CurrentThemeName;
            
            // Store initial control bounds after initialization
            this.HandleCreated += (s, e) => StoreOriginalControlBounds();
        }

        private void BeepThemesManager_v2_ThemeChanged(object? sender, Controls.Models.ThemeChangeEventArgs e)
        {
            Theme = e.NewThemeName;

            if (Theme != BeepThemesManager.CurrentThemeName)
            {  BeepThemesManager.SetCurrentTheme(Theme); }
            ApplyTheme();
        }
        
        #region "DPI and Control Position Management"
        
        /// <summary>
        /// CRITICAL: Override OnDpiChangedAfterParent to prevent control movement during DPI/theme changes
        /// </summary>
        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            _isDpiChanging = true;
            
            try
            {
              
                
                try
                {
                    // Call base implementation
                    base.OnDpiChangedAfterParent(e);
                    
                    // Apply DPI changes to child controls
                    UpdateChildControlsForDpi();
                }
                finally
                {
                    // Resume layout
                   
                }
                
              
                
                // Force redraw
                this.Invalidate();
            }
            finally
            {
                _isDpiChanging = false;
            }
        }
        
        /// <summary>
        /// Override OnFontChanged to prevent control movement during theme font changes
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            if (!_isApplyingTheme && !_isDpiChanging)
            {
                base.OnFontChanged(e);
                return;
            }
            
         
            
            try
            {
                base.OnFontChanged(e);
            }
            finally
            {
                // Resume layout and restore positions
               
            }
        }
        
        /// <summary>
        /// Update child controls for DPI changes without moving them
        /// </summary>
        private void UpdateChildControlsForDpi()
        {
            foreach (Control control in this.Controls)
            {
                UpdateControlForDpiSafe(control);
            }
        }
        
        /// <summary>
        /// Safely update a control for DPI changes
        /// </summary>
        private void UpdateControlForDpiSafe(Control control)
        {
            Rectangle originalBounds = control.Bounds;
            
            try
            {
                // If it's a BeepControl, let it handle its own DPI scaling
                if (control is BeepControl beepControl)
                {
                    beepControl.Invalidate();
                }
                else
                {
                    // For regular controls, just invalidate
                    control.Invalidate();
                }
                
                // Restore position if it changed
                if (control.Bounds != originalBounds)
                {
                    control.Bounds = originalBounds;
                }
                
                // Recursively update child controls
                if (control.HasChildren)
                {
                    foreach (Control child in control.Controls)
                    {
                        UpdateControlForDpiSafe(child);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but continue
                System.Diagnostics.Debug.WriteLine($"Error updating control {control.Name} for DPI: {ex.Message}");
                // Restore original bounds on error
                control.Bounds = originalBounds;
            }
        }
        
        /// <summary>
        /// Store original control bounds to prevent movement during theme changes
        /// </summary>
        private void StoreOriginalControlBounds()
        {
            if (!IsHandleCreated) return;
            
            _originalBounds.Clear();
            StoreControlBoundsRecursive(this);
        }
        
        /// <summary>
        /// Recursively store bounds for all controls
        /// </summary>
        private void StoreControlBoundsRecursive(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (!_originalBounds.ContainsKey(control))
                {
                    _originalBounds[control] = control.Bounds;
                }
                
                if (control.HasChildren)
                {
                    StoreControlBoundsRecursive(control);
                }
            }
        }
        
      
        
        /// <summary>
        
      
        
        #endregion "DPI and Control Position Management"
        
        #region "IDM_Addin Implementation"
     

        private string _theme;
        protected IBeepTheme _currentTheme = BeepThemesManager.GetDefaultTheme();
        [Browsable(true)]
        [TypeConverter(typeof(ThemeEnumConverter))]
        public string Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                _currentTheme = BeepThemesManager.GetTheme(value);
                ApplyTheme();
            }
        }
     

        public AddinDetails Details { get; set; }
        public Dependencies Dependencies { get; set; }
        public string GuidID { get; set; }

        public event EventHandler OnStart;
        public event EventHandler OnStop;
        public event EventHandler<ErrorEventArgs> OnError;

        protected IDataSource ds;
        string DataSourceName;
        protected string EntityName;
        protected UnitOfWorkWrapper uow;
        public virtual void Configure(Dictionary<string, object> settings)
        {
            if (Theme != BeepThemesManager.CurrentThemeName) 
            
            { BeepThemesManager.SetCurrentTheme(Theme);  }
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
           
            if (parameters != null)
            {
                if (parameters.ContainsKey("DatasourceName"))
                {
                    DataSourceName = (string)parameters["DatasourceName"];
                    if (DataSourceName != null)
                    {
                        if (parameters.ContainsKey("CurrentEntity"))
                        {
                            EntityName = (string)parameters["CurrentEntity"];
                            if (EntityName != null)
                            {
                                Details.ObjectName = EntityName;
                            }
                        }
                    }
                }
            }
            if (DataSourceName != null)
            {
                ds = beepService.DMEEditor.GetDataSource(DataSourceName);
                if (ds != null)
                {
                    if (EntityName != null)
                    {
                        EntityStructure entityStructure = ds.GetEntityStructure(EntityName, true);
                        if (entityStructure != null)
                        {
                            Type type = ds.GetEntityType(EntityName);
                            if (type != null)
                            {
                                var u = UnitOfWorkFactory.CreateUnitOfWork(type, beepService.DMEEditor, DataSourceName, EntityName);
                                uow = new UnitOfWorkWrapper(u);
                               
                            }

                        }

                    }
                }
            }

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
            if (_currentTheme == null) return;
            
            // CRITICAL FIX: Set flag to prevent DPI-related movement during theme application
            _isApplyingTheme = true;
            
            try
            {
                // Store current control positions before applying theme
             //   StoreOriginalControlBounds();
                
                // Suspend layout for the entire control hierarchy to prevent movement
           //     SuspendLayoutRecursive(this);
                
                try
                {
                    // Apply background color
                    BackColor = _currentTheme.BackColor;
                    
                    // Apply theme to child controls using safe font handling
                    ApplyThemeToChildControlsSafe();
                }
                finally
                {
                    // Always resume layout
               //     ResumeLayoutRecursive(this);
                }
                
                // Restore control positions after theme application
              //  RestoreControlPositions();
                
                // Invalidate for visual refresh only after positions are restored
                this.Invalidate();
            }
            finally
            {
                // Always clear the flag
                _isApplyingTheme = false;
            }
        }
        
        /// <summary>
        /// Apply theme to child controls with safe font handling to prevent position changes
        /// </summary>
        private void ApplyThemeToChildControlsSafe()
        {
            foreach (Control item in this.Controls)
            {
                ApplyThemeToControlSafe(item);
            }
        }
        
        /// <summary>
        /// Apply theme to a single control safely
        /// </summary>
        private void ApplyThemeToControlSafe(Control control)
        {
            try
            {
                // Store the control's original position
               // Rectangle originalBounds = control.Bounds;
                
                // Check if item is a Beep UI component
                if (control is IBeepUIComponent beepComponent)
                {
                    // For Beep components, apply theme through the component interface
                    // This will trigger proper theme application with DPI awareness
                    beepComponent.Theme = Theme;
                }
                else
                {
                    // For regular controls, apply basic theme properties
                    control.BackColor = _currentTheme.BackColor;
                    control.ForeColor = _currentTheme.ForeColor;
                }
                
                // Restore position if it changed during theme application (only for non-docked controls)
  
                  //  control.Bounds = originalBounds;
                
                
                // Recursively apply to child controls
                if (control.HasChildren)
                {
                    foreach (Control child in control.Controls)
                    {
                        ApplyThemeToControlSafe(child);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with other controls
                System.Diagnostics.Debug.WriteLine($"Error applying theme to control {control.Name}: {ex.Message}");
            }
        }
        
        #endregion "IDM_Addin Implementation"
        
    }
}
