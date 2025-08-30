using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Vis.Modules.Managers;


namespace TheTechIdea.Beep.Winform.Controls.Base
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Control Advanced")]
    [Description("Advanced Beep control with full feature parity to BeepControl but using helper architecture.")]
    public partial class BaseControl : ContainerControl, IBeepUIComponent
    {
        #region Private Fields
        private string _themeName;
        protected IBeepTheme _currentTheme = BeepThemesManager.GetDefaultTheme();
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

        private bool CanFocus() => _paint?.CanBeFocused ?? true;

        // Helpers
        internal readonly ControlPaintHelper _paint;
        internal readonly ControlEffectHelper _effects;
        internal readonly ControlHitTestHelper _hitTest;
        internal readonly ControlInputHelper _input;
        internal readonly ControlExternalDrawingHelper _externalDrawing;
        internal readonly ControlDpiHelper _dpi;
        internal readonly ControlDataBindingHelper _dataBinding;
        internal BaseControlMaterialHelper _materialHelper;

        // Internal access to paint helper for helpers within the same assembly
        internal ControlPaintHelper PaintHelper => _paint;
       
        #endregion

        #region Constructor
        public BaseControl()
        {
            _isInitializing = true;
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

            // Initialize helpers
            _paint = new ControlPaintHelper(this);
            _effects = new ControlEffectHelper(this);
            _hitTest = new ControlHitTestHelper(this);
            _dpi = new ControlDpiHelper(this);
            _dataBinding = new ControlDataBindingHelper(this);
            _externalDrawing = new ControlExternalDrawingHelper(this);
            _input = new ControlInputHelper(this, _effects, _hitTest);
            _materialHelper = new BaseControlMaterialHelper(this);
            // Let helper call back for custom border drawing
            // _paint.CustomBorderDrawer = g => DrawCustomBorder(g);

            // Set defaults
            Padding = new Padding(0);
            ComponentName = "BaseControl";
            InitializeTooltip();
            UpdateDrawingRect();
            
            _isInitializing = false;
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

    }
}
