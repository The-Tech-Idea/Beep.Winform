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
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;


namespace TheTechIdea.Beep.Winform.Controls.Base
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Control Advanced")]
    [Description("Advanced Beep control with full feature parity to BeepControl but using helper architecture.")]
    public partial class BaseControl : ContainerControl, IBeepUIComponent, IExternalDrawingProvider, IDisposable
    {
        #region Private Fields
        // Add these safety utilities at the class level in BaseControl
        protected bool IsReadyForDrawing => !IsDisposed && IsHandleCreated && Width > 0 && Height > 0 && !_isInitializing;

        protected static bool IsValidRectangle(Rectangle rect) => rect.Width > 0 && rect.Height > 0;

        /// <summary>
        /// When true, BaseControl will clear the graphics context before drawing.
        /// Override to false in derived controls that handle their own complete rendering (e.g., grids)
        /// </summary>
        protected virtual bool AllowBaseControlClear => true;

        /// <summary>
        /// When true, BaseControl will exclude child control bounds from painting to prevent flickering.
        /// Override to true in container controls like BeepPanel.
        /// </summary>
        protected virtual bool IsContainerControl => false;

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

        // Cached parent background for transparent controls (avoid BitBlt feedback loop)
        private Bitmap? _cachedParentBackground = null;
        private bool _parentBackgroundCacheValid = false;

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
        // REMOVED: ControlDpiHelper _dpi - .NET 8/9+ handles DPI automatically via framework
        internal ControlDataBindingHelper _dataBinding;
     //   internal BaseControlMaterialHelper _materialHelper; // kept for binary compatibility; no longer constructed/used at runtime
        internal IBaseControlPainter _painter; // strategy-based painter (optional)
        protected float _dpiScaleX = 1f;
        protected float _dpiScaleY = 1f;

        // Track theme change subscription
        private bool _subscribedToThemeChanged = false;
        
    // Performance toggles
    [Category("Performance")]
    [Description("If true, uses an extra BufferedGraphics layer in OnPaint. When false, relies on built-in DoubleBuffered drawing.")]
    public bool UseExternalBufferedGraphics { get; set; } = true;

    [Category("Performance")]
    [Description("If true, sets high-quality smoothing/text rendering. Turn off to favor speed.")]
    public bool EnableHighQualityRendering { get; set; } = true;

    [Category("Performance")]
    [Description("Automatically draws components in HitList during OnPaint.")]
    public bool AutoDrawHitListComponents { get; set; } = true;

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
            this.AutoScaleMode = AutoScaleMode.Inherit; // or Font
            //this.AutoScaleDimensions = new SizeF(96f, 96f); // ensure design baseline
                                                            // Avoid manual scaling of sizes/locations/fonts.
            DoubleBuffered = true;
            this.SetStyle(ControlStyles.ContainerControl, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer , true);

            // Consider adding for large datasets:
            SetStyle(ControlStyles.ResizeRedraw, false);  // Don't redraw on resize

            // Ensure _columns is only initialized once
            SetStyle(ControlStyles.Selectable | ControlStyles.UserMouse, true);
            this.UpdateStyles();
            
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

            // Let WinForms handle scaling via parent container's AutoScaleMode
            // Per Microsoft docs: "Each container control is responsible for scaling its children 
            // using its own scaling factors and not the one from its parent container"
            //// We inherit the parent's mode and let it handle our scaling
            //AutoScaleMode = AutoScaleMode.Inherit;
            //DoubleBuffered = true;
            //BackColor = Color.Transparent;

            // Initialize helpers in the correct order to avoid circular dependencies
            try
            {
                IsChild=true;
                
                // .NET 8/9+ High-DPI Support:
                // Modern .NET WinForms handles DPI automatically via AutoScaleMode.Inherit.
                // No manual DPI helper needed - the framework uses DeviceDpi property internally.
                // Reference: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms
                
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
                
                // 6. Initialize border painter if ControlStyle/BorderPainter is already set
                // This ensures border style, color, and width are applied on construction
                if (_borderPainterStyle != BeepControlStyle.None)
                {
                    UpdateBorderPainter();
                }
                
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
                // REMOVED: DPI helper creation - .NET 8/9+ handles DPI automatically
                
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
                // Cleanup ToolTipManager registration
                CleanupTooltip();

                // Dispose legacy tooltip if it exists
                if (GetType().GetField("_legacyToolTip", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) is System.Windows.Forms.ToolTip tip)
                {
                    tip.Dispose();
                }

                // Dispose tooltip
                _toolTip?.Dispose();

                // Clear external drawing from parent
                if (Parent is IExternalDrawingProvider externalDrawingProvider)
                {
                    externalDrawingProvider.ClearChildExternalDrawing(this);
                }

                // Unsubscribe from theme changes
                if (_subscribedToThemeChanged)
                {
                    try { BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged; } catch { }
                    try { BeepThemesManager.FormStyleChanged -= OnGlobalFormStyleChanged; } catch { }
                    _subscribedToThemeChanged = false;
                }

                // Dispose helpers
                _effects?.Dispose();
                _externalDrawing?.Dispose();
                _dataBinding?.Dispose();
                
                // Dispose cached parent background
                _cachedParentBackground?.Dispose();
                _cachedParentBackground = null;
                _parentBackgroundCacheValid = false;
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
                const int WS_EX_TRANSPARENT = 0x20;

                // Prevent painting over child windows and sibling overlap artifacts
                cp.Style |= WS_CLIPCHILDREN | WS_CLIPSIBLINGS;
                
                // CRITICAL: Only apply WS_EX_TRANSPARENT at runtime, NOT in design mode
                // In design mode, WS_EX_TRANSPARENT causes the form to go blank when controls are selected
                // because the parent form needs to repaint but doesn't get proper invalidation notifications
                bool isDesignMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
                                   DesignMode ||
                                   (this.Site != null && this.Site.DesignMode);
                
                if (!isDesignMode)
                {
                    cp.ExStyle |= WS_EX_TRANSPARENT; // Runtime only: lets parent paint first; reduces flicker
                }
                
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
        protected static Rectangle EnsureValidRectangle(Rectangle rect, int minSize = 1)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return new Rectangle(rect.X, rect.Y,
                    Math.Max(minSize, rect.Width),
                    Math.Max(minSize, rect.Height));
            }
            return rect;
        }

        protected static Font GetSafeFont(Font preferredFont)
        {
            if (preferredFont == null)
                return SystemFonts.DefaultFont;
            return preferredFont;
        }

        // Add cached font metrics to avoid repeated measurements
        private int _cachedFontHeight = -1;
        private Font _lastMeasuredFont;

        /// <summary>
        /// Gets cached font height to avoid repeated TextRenderer.MeasureText calls.
        /// Cache is automatically invalidated on DPI changes.
        /// </summary>
        protected int GetCachedFontHeight(Font font)
        {
            font = GetSafeFont(font);
            if (_cachedFontHeight < 0 || _lastMeasuredFont != font)
            {
                _lastMeasuredFont = font;
                _cachedFontHeight = TextRenderer.MeasureText("Aj", font).Height;
            }
            return _cachedFontHeight;
        }

        /// <summary>
        /// Invalidates cached font metrics. Called automatically on DPI changes.
        /// Derived controls should call this if they change Font programmatically.
        /// </summary>
        protected void InvalidateFontCache()
        {
            _cachedFontHeight = -1;
            _lastMeasuredFont = null;
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

                // Refresh local DPI scales as soon as the handle exists.
                DpiScalingHelper.RefreshScaleFactors(this, ref _dpiScaleX, ref _dpiScaleY);
                
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
                BeepThemesManager.FormStyleChanged -= OnGlobalFormStyleChanged;
                BeepThemesManager.FormStyleChanged += OnGlobalFormStyleChanged;
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
                Theme = newThemeName;// BeepThemesManager.GetTheme(newThemeName) ?? BeepThemesManager.GetDefaultTheme();
                ApplyTheme();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BaseControl: OnGlobalThemeChanged error: {ex.Message}");
            }
        }
        private void OnGlobalFormStyleChanged(object? sender, StyleChangeEventArgs e)
        {
            if (IsDisposed) return;
            try
            {
                var newFormStyle = e.NewStyle;
                ControlStyle= BeepStyling.GetControlStyle(newFormStyle);
                Theme= BeepThemesManager.CurrentThemeName;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BaseControl: OnGlobalFormStyleChanged error: {ex.Message}");
            }
        }

        #region DPI Awareness Support (Microsoft High-DPI Best Practices)

        /// <summary>
        /// Handle DPI changes for Per-Monitor V2 DPI awareness
        /// Per Microsoft docs: Use DpiChanged events for dynamic DPI scenarios
        /// Per Microsoft docs: "DpiChangedAfterParent is raised when the parent's DPI changes"
        /// Reference: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms
        /// 
        /// NOTE: Base implementation sets scaleFont=false to avoid double-scaling.
        /// Derived controls that explicitly set Font should override OnDpiScaleChanged() and call ScaleFontForDpi().
        /// </summary>
        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            var oldScaleX = _dpiScaleX;
            var oldScaleY = _dpiScaleY;

            base.OnDpiChangedAfterParent(e);
            
            try
            {
                if (IsHandleCreated && !IsDisposed)
                {
                    DpiScalingHelper.RefreshScaleFactors(this, ref _dpiScaleX, ref _dpiScaleY);

                    if (!DpiScalingHelper.AreScaleFactorsEqual(oldScaleX, _dpiScaleX) ||
                        !DpiScalingHelper.AreScaleFactorsEqual(oldScaleY, _dpiScaleY))
                    {
                        // Preserve Anchor/Dock behavior: only non-auto controls are scaled manually.
                        DpiScalingHelper.ScaleControlTreeForDpiChange(
                            this,
                            oldScaleX,
                            oldScaleY,
                            _dpiScaleX,
                            _dpiScaleY,
                            scaleFont: false);
                        OnDpiScaleChanged(oldScaleX, oldScaleY, _dpiScaleX, _dpiScaleY);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BaseControl.OnDpiChangedAfterParent error: {ex.Message}");
            }
        }

        /// <summary>
        /// Called after this control observes a DPI scale transition.
        /// Derived controls can override to recompute custom drawing metrics.
        /// Per Microsoft: Invalidate cached font metrics and notify painters.
        /// </summary>
        protected virtual void OnDpiScaleChanged(float oldScaleX, float oldScaleY, float newScaleX, float newScaleY)
        {
            // Invalidate font cache (height measurements change with DPI)
            InvalidateFontCache();
            
            UpdateDrawingRect();
            _painter?.UpdateLayout(this);
            PerformLayout();
            Invalidate(true);
        }

        /// <summary>
        /// Helper for derived controls to scale their font during DPI changes.
        /// Uses BeepFontManager.TryScaleControlFont() per Microsoft DPI guidance.
        /// Only call this if the derived control explicitly sets Font (not inherited).
        /// </summary>
        /// <example>
        /// protected override void OnDpiScaleChanged(float oldScaleX, float oldScaleY, float newScaleX, float newScaleY)
        /// {
        ///     int oldDpi = (int)(oldScaleX * 96);
        ///     int newDpi = (int)(newScaleX * 96);
        ///     ScaleFontForDpi(oldDpi, newDpi);
        ///     base.OnDpiScaleChanged(oldScaleX, oldScaleY, newScaleX, newScaleY);
        /// }
        /// </example>
        protected bool ScaleFontForDpi(int oldDpi, int newDpi)
        {
            // Only scale if font was explicitly set (not inherited from parent)
            if (DpiScalingHelper.IsFontInherited(this, this.Font))
                return false;

            return BeepFontManager.TryScaleControlFont(this, oldDpi, newDpi);
        }

        /// <summary>
        /// Override ScaleControl to customize scaling behavior
        /// Per Microsoft docs: "The ScaleControl method can be overridden to change the scaling logic"
        /// https://learn.microsoft.com/en-us/dotnet/desktop/winforms/forms/autoscale
        /// </summary>
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            // Let base handle standard scaling
            base.ScaleControl(factor, specified);
            
            //// If we have painter, notify it of scale change
            //try
            //{
            //    if (_painter != null && IsHandleCreated && !IsDisposed)
            //    {
            //        _painter.UpdateLayout(this);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine($"BaseControl.ScaleControl painter update error: {ex.Message}");
            //}
        }
        
        #endregion
    }
}
