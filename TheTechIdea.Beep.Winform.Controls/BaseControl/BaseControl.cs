using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Vis.Modules.Managers;
using System.Drawing;
using System.Windows.Forms;


namespace TheTechIdea.Beep.Winform.Controls.Base
{
    [ToolboxItem(true)]
    [DesignerCategory("Control")]
    [Category("Beep Controls")]
    [DisplayName("Beep Control Advanced")]
    [Description("Advanced Beep control with full feature parity to BeepControl but using helper architecture.")]
    public partial class BaseControl : ContainerControl, IBeepUIComponent
    {
        #region Private Fields
        private string _themeName;
        protected IBeepTheme _currentTheme; // defer initialization to runtime-safe paths
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
        internal ControlPaintHelper _paint;
        internal ControlEffectHelper _effects;
        internal ControlHitTestHelper _hitTest;
        internal ControlInputHelper _input;
        internal ControlExternalDrawingHelper _externalDrawing;
        internal ControlDpiHelper _dpi;
        internal ControlDataBindingHelper _dataBinding;
        internal BaseControlMaterialHelper _materialHelper;

        // Internal access to paint helper for helpers within the same assembly
        internal ControlPaintHelper PaintHelper => _paint;
       
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

            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96f, 96f); // ensure design baseline
            DoubleBuffered = true;
            this.SetStyle(ControlStyles.ContainerControl, true);

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            // Consider adding for large datasets:
            SetStyle(ControlStyles.ResizeRedraw, false);  // Don't redraw on resize

            // Ensure _columns is only initialized once
            SetStyle(ControlStyles.Selectable | ControlStyles.UserMouse, true);
            this.UpdateStyles();

            // Initialize helpers in the correct order to avoid circular dependencies
            try
            {
                // 1. Initialize core helpers first (no dependencies)
                _dpi = new ControlDpiHelper(this);
                _paint = new ControlPaintHelper(this);
                _dataBinding = new ControlDataBindingHelper(this);
                _externalDrawing = new ControlExternalDrawingHelper(this);
                
                // 2. Initialize helpers that depend on core helpers
                _effects = new ControlEffectHelper(this);
                _hitTest = new ControlHitTestHelper(this);
                
                // 3. Initialize helpers that depend on multiple other helpers
                _input = new ControlInputHelper(this, _effects, _hitTest);
                
                // 4. Initialize Material helper last (depends on many others)
                _materialHelper = new BaseControlMaterialHelper(this);
                
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
            
            // Only update drawing rect if paint helper is working
            if (_paint != null)
            {
                try
                {
                    _paint.UpdateRects();
                }
                catch (Exception ex)
                {
                    if (isDesignMode)
                    {
                        System.Diagnostics.Debug.WriteLine($"BaseControl: UpdateDrawingRect error in design mode: {ex.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"BaseControl: UpdateDrawingRect error: {ex.Message}");
                    }
                }
            }
            
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
                if (_paint == null)
                {
                    Console.WriteLine("Creating minimal paint helper for fallback");
                    _paint = new ControlPaintHelper(this);
                }
                
                if (_dpi == null)
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

                // Dispose helpers
                _effects?.Dispose();
                _externalDrawing?.Dispose();
                _dataBinding?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void ClearChildExternalDrawing(BaseControl baseControl)
        {
           _externalDrawing.ClearAllChildExternalDrawing();
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
                    // Use the existing UpdateDrawingRect method from BaseControl.Methods.cs
                    if (_paint != null)
                    {
                        _paint.UpdateRects();
                    }
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
                
                // Only perform complex operations if not in design mode
                if (!isDesignMode)
                {
                    // Use the existing UpdateDrawingRect method from BaseControl.Methods.cs
                    if (_paint != null)
                    {
                        _paint.UpdateRects();
                    }
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
    }
}
