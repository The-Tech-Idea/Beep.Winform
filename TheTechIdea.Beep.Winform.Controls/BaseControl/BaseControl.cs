using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
 
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters;


namespace TheTechIdea.Beep.Winform.Controls.Base
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Control Advanced")]
    [Description("Advanced Beep control with full feature parity to BeepControl but using helper architecture.")]
    public partial class BaseControl : ContainerControl, IBeepUIComponent, IDisposable
    {
        #region Private Fields
        private string _themeName;
        internal IBeepTheme _currentTheme; // defer initialization to runtime-safe paths
        private string _guidId = Guid.NewGuid().ToString();
        private string _blockId;
        private string _fieldId;
        private int _id = -1;
        private List<object> _items = new();
        private object _selectedValue;
        private object _oldValue;
        private string _boundProperty;
        private string _dataSourceProperty;
        private string _linkedProperty;
        private bool _isSelected;
        private bool _isDeleted;
        private bool _isNew;
        private bool _isDirty;
        private bool _isReadOnly;
        private bool _isEditable = true;
        private bool _isVisible = true;
        private bool _isFrameless = false;
        private bool _isRequired = false;
        private bool _staticNotMoving = false;
        private Point _originalLocation;
        private DbFieldCategory _category = DbFieldCategory.String;
        private Color _borderColor = Color.Black;
        protected string _text = string.Empty;
        private SimpleItem _info = new SimpleItem();
        private bool _isInitializing = true;
        protected ToolTip _toolTip;

        // Material Design size compensation fields
        private bool _materialAutoSizeCompensation = true;

        // State flags (exposed like base BeepControl)
        [Browsable(true)]
        public bool IsHovered { get; set; }

        [Browsable(true)]
        public bool IsPressed { get; set; }

        [Browsable(true)]
        public bool IsFocused
        {
            get => Focused;
            set
            {
                if (value && !_isInitializing && CanFocus())
                {
                    Focus();
                }
            }
        }

        public string ToolTipText { get ; set ; }

        private bool _canBeFocused= false;
        private bool CanFocus() => _canBeFocused;

        // Helpers - Make all consistently non-readonly to allow proper initialization
        internal ControlEffectHelper _effects;
        internal ControlHitTestHelper _hitTest;
        internal ControlInputHelper _input;
        internal ControlExternalDrawingHelper _externalDrawing;
        internal ControlDpiHelper _dpi;
        internal ControlDataBindingHelper _dataBinding;
     //   internal BaseControlMaterialHelper _materialHelper; // kept for binary compatibility; no longer constructed/used at runtime
        internal IBaseControlPainter _painter; // strategy-based painter (optional)

        // Track theme change subscription
        private bool _subscribedToThemeChanged = false;
        
    // Performance toggles
    [Category("Performance")]
    [Description("If true, uses an extra BufferedGraphics layer in OnPaint. When false, relies on built-in DoubleBuffered drawing.")]
    public bool UseExternalBufferedGraphics { get; set; } = false;

    [Category("Performance")]
    [Description("If true, sets high-quality smoothing/text rendering. Turn off to favor speed.")]
    public bool EnableHighQualityRendering { get; set; } = true;

    [Category("Performance")]
    [Description("Automatically draws components in HitList during OnPaint.")]
    public bool AutoDrawHitListComponents { get; set; } = false;

    [Category("Performance")]
    [Description("Optional cap for how many HitList components to draw per frame. 0 or negative disables capping.")]
    public int MaxHitListDrawPerFrame { get; set; } = 0;

    // Effects toggles
    [Category("Effects")]
    [Description("If true, controls inheriting from this base may show a splash (ink ripple) effect on click. Turn off to disable.")]
        private bool _enableSplashEffect = false;
        public bool EnableSplashEffect
        {
            get => _enableSplashEffect;
            set => _enableSplashEffect = value;
        }

        #endregion

        #region Constructor
        public BaseControl()
        {
            _isInitializing = true;
            
            // Critical: Check if we're in design mode to prevent designer issues
            bool isDesignMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
                               DesignMode ||
                               (this.Site != null && this.Site.DesignMode);

            // Initialize theme lazily and safely
            try
            {
                _currentTheme = isDesignMode ? BeepThemesManager.GetDefaultTheme() : (BeepThemesManager.GetTheme(BeepThemesManager.CurrentThemeName) ?? BeepThemesManager.GetDefaultTheme());
            }
            catch
            {
                _currentTheme = BeepThemesManager.GetDefaultTheme();
            }

            // Let WinForms (parent form) handle scaling; do not force a baseline here
            AutoScaleMode = AutoScaleMode.Inherit;
            DoubleBuffered = true;
            this.SetStyle(ControlStyles.ContainerControl, true);

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            // Consider adding for large datasets:
            SetStyle(ControlStyles.ResizeRedraw, false);  // Don't redraw on resize

            // Ensure _columns is only initialized once
            SetStyle(ControlStyles.Selectable | ControlStyles.UserMouse, true);
            this.UpdateStyles();

            // Initialize helpers in the correct order to avoid circular dependencies
            try
            {
                IsChild=true;
                // 1. Initialize core helpers first (no dependencies)
                // IMPORTANT: Do not create DPI helper by default; let framework scale.
                if (!DisableDpiAndScaling)
                    _dpi = new ControlDpiHelper(this);
                _dataBinding = new ControlDataBindingHelper(this);
                _externalDrawing = new ControlExternalDrawingHelper(this);
                
                // 2. Initialize helpers that depend on core helpers
                _effects = new ControlEffectHelper(this);
                _hitTest = new ControlHitTestHelper(this);
                
                // 3. Initialize helpers that depend on multiple other helpers
                _input = new ControlInputHelper(this, _effects, _hitTest);
                
                // 4. Legacy material helper is no longer constructed; painters own material rendering now
                // _materialHelper remains null for binary compatibility
                
                // 5. Initialize default painter based on PainterKind (defaults to Auto -> Classic)
                UpdatePainterFromKind();
                
            }
            catch (Exception ex)
            {
                // In design mode, log the error but don't let it crash the designer
                if (isDesignMode)
                {
                    System.Diagnostics.Debug.WriteLine($"BaseControl: Design-time initialization error: {ex.Message}");
                }
                else
                {
                    Console.WriteLine($"BaseControl: Helper initialization error: {ex.Message}");
                }
                
                // Create minimal safe helpers for design-time
                CreateSafeDesignTimeHelpers();
            }
           
            // Set defaults
            Padding = new Padding(0);
            ComponentName = "BaseControl";
            
            // Initialize tooltip with null check
            try
            {
                InitializeTooltip();
            }
            catch (Exception tooltipEx)
            {
                Console.WriteLine($"Tooltip initialization failed: {tooltipEx.Message}");
            }
            
            // Subscribe to global theme changes at runtime
            TrySubscribeThemeChanged(isDesignMode);
            
            _isInitializing = false;
        }

        /// <summary>
        /// Creates minimal, safe helpers for design-time when full initialization fails
        /// </summary>
        private void CreateSafeDesignTimeHelpers()
        {
            try
            {
                // Create only the most essential helpers
                if (_dpi == null && !DisableDpiAndScaling)
                {
                    Console.WriteLine("Creating minimal DPI helper for fallback");
                    _dpi = new ControlDpiHelper(this);
                }
                
                if (_externalDrawing == null)
                {
                    Console.WriteLine("Creating minimal external drawing helper for fallback");
                    _externalDrawing = new ControlExternalDrawingHelper(this);
                }
                
            }
            catch (Exception ex)
            {
                // If even safe helpers fail, we'll have to work without them
                System.Diagnostics.Debug.WriteLine($"BaseControl: Could not create safe design-time helpers: {ex.Message}");
                Console.WriteLine($"BaseControl: Could not create safe design-time helpers: {ex.Message}");
            }
        }
        #endregion

        #region ToolTip
        protected void InitializeTooltip()
        {
            _toolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 500,
                ReshowDelay = 500,
                ShowAlways = true // Always show the tooltip, even if the control is not active
            };
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose legacy tooltip if it exists
                if (GetType().GetField("_legacyToolTip", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) is System.Windows.Forms.ToolTip tip)
                {
                    tip.Dispose();
                }

                // Dispose tooltip
                _toolTip?.Dispose();

                // Clear external drawing from parent
                if (Parent is BaseControl parentBeepControl)
                {
                    parentBeepControl.ClearChildExternalDrawing(this);
                }

                // Unsubscribe from theme changes
                if (_subscribedToThemeChanged)
                {
                    try { BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged; } catch { }
                    _subscribedToThemeChanged = false;
                }

                // Dispose helpers
                _effects?.Dispose();
                _externalDrawing?.Dispose();
                _dataBinding?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void ClearChildExternalDrawing(BaseControl baseControl)
        {
           // Fix: only clear registrations for the specified child on this parent
           _externalDrawing.ClearChildExternalDrawing(baseControl);
        }
        #endregion

        #region Event Handlers
      
        #endregion

        #region Utility Methods
        public override string ToString()
        {
            return GetType().Name.Replace("Control", "").Replace("Beep", "Beep ");
        }

        public virtual void Print(Graphics graphics)
        {
            OnPrint(new PaintEventArgs(graphics, ClientRectangle));
        }

        // Allow derived controls to override custom border drawing
        public virtual void DrawCustomBorder(Graphics g)
        {
            // Default: do nothing, consumers can override
            DrawCustomBorder_Ext(g); // partial hook for extensions (e.g., Material)
        }

        // Partial extension hook implemented in BaseControl.Material.cs
        partial void DrawCustomBorder_Ext(Graphics g);

        public void ShowToolTip(string text)
        {
            ShowToolTip("Title", "Test");
        }
        #endregion
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                const int WS_CLIPCHILDREN = 0x02000000;
                const int WS_CLIPSIBLINGS = 0x04000000;

                // Prevent painting over child windows and sibling overlap artifacts
                cp.Style |= WS_CLIPCHILDREN | WS_CLIPSIBLINGS;

                return cp;
            }
        }
        protected override void InitLayout()
        {
            // Add design-time safety
            bool isDesignMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
                               DesignMode ||
                               (this.Site != null && this.Site.DesignMode);
            
            try
            {
                base.InitLayout();
                
                // Only perform complex layout operations if not in design mode
                if (!isDesignMode)
                {
                    EnsurePainter();
                    _painter?.UpdateLayout(this);
                }
            }
            catch (Exception ex)
            {
                if (isDesignMode)
                {
                    System.Diagnostics.Debug.WriteLine($"BaseControl.InitLayout: Design-time error: {ex.Message}");
                }
                else
                {
                    throw;
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            // Add design-time safety
            bool isDesignMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
                               DesignMode ||
                               (this.Site != null && this.Site.DesignMode);
            
            try
            {
                base.OnHandleCreated(e);
                
                // Ensure we are subscribed when a handle is created at runtime
                TrySubscribeThemeChanged(isDesignMode);
                
                // Only perform complex operations if not in design mode
                if (!isDesignMode)
                {
                    EnsurePainter();
                    _painter?.UpdateLayout(this);
                }
            }
            catch (Exception ex)
            {
                if (isDesignMode)
                {
                    System.Diagnostics.Debug.WriteLine($"BaseControl.OnHandleCreated: Design-time error: {ex.Message}");
                }
                else
                {
                    throw;
                }
            }
        }

        // Subscribe to BeepThemesManager to auto-apply theme changes
        private void TrySubscribeThemeChanged(bool isDesignMode)
        {
            if (_subscribedToThemeChanged || isDesignMode) return;
            try
            {
                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
                BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;
                _subscribedToThemeChanged = true;
            }
            catch { /* best-effort */ }
        }

        private void OnGlobalThemeChanged(object? sender, EventArgs e)
        {
            if (IsDisposed) return;
            try
            {
                var newThemeName = BeepThemesManager.CurrentThemeName;
                _themeName = newThemeName;
                _currentTheme = BeepThemesManager.GetTheme(newThemeName) ?? BeepThemesManager.GetDefaultTheme();
                ApplyTheme();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BaseControl: OnGlobalThemeChanged error: {ex.Message}");
            }
        }
    }
}
