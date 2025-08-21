using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Advanced.Helpers;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Vis.Modules.Managers;

namespace TheTechIdea.Beep.Winform.Controls.Advanced
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Control Advanced")]
    [Description("Advanced Beep control with full feature parity to BeepControl but using helper architecture.")]
    public partial class BeepControlAdvanced : ContainerControl, IBeepUIComponent
    {
        #region Private Fields
        private readonly ToolTip _toolTip;
        private string _themeName;
        protected IBeepTheme _currentTheme;
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
        private string _text = string.Empty;
        private SimpleItem _info = new SimpleItem();
        private bool _isInitializing = true;

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

        private bool CanFocus() => _paint?.CanBeFocused ?? true;

        // Helpers
        private readonly ControlPaintHelper _paint;
        private readonly ControlEffectHelper _effects;
        private readonly ControlHitTestHelper _hitTest;
        private readonly ControlInputHelper _input;
        private readonly ControlExternalDrawingHelper _externalDrawing;
        private readonly ControlDpiHelper _dpi;
        private readonly ControlDataBindingHelper _dataBinding;
        #endregion

        #region Constructor
        public BeepControlAdvanced()
        {
            _isInitializing = true;
            
            // Configure control styles
            AutoScaleMode = AutoScaleMode.Dpi;
            DoubleBuffered = true;
            SetStyle(ControlStyles.ContainerControl | ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.Selectable | ControlStyles.UserMouse, true);
            SetStyle(ControlStyles.ResizeRedraw, false);
            UpdateStyles();

            // Initialize tooltip
            _toolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 500,
                ReshowDelay = 500,
                ShowAlways = true
            };

            // Initialize helpers
            _paint = new ControlPaintHelper(this);
            _effects = new ControlEffectHelper(this);
            _hitTest = new ControlHitTestHelper(this);
            _dpi = new ControlDpiHelper(this);
            _dataBinding = new ControlDataBindingHelper(this);
            _externalDrawing = new ControlExternalDrawingHelper(this);
            _input = new ControlInputHelper(this, _effects, _hitTest);

            // Let helper call back for custom border drawing
            _paint.CustomBorderDrawer = g => DrawCustomBorder(g);

            // Set defaults
            Padding = new Padding(0);
            ComponentName = "BeepControlAdvanced";
            
            _isInitializing = false;
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clear external drawing from parent
                if (Parent is BeepControlAdvanced parentBeepControl)
                {
                    parentBeepControl.ClearChildExternalDrawing(this);
                }

                // Dispose helpers
                _effects?.Dispose();
                _externalDrawing?.Dispose();
                _dataBinding?.Dispose();

                // Dispose tooltip
                _toolTip?.Dispose();
            }

            base.Dispose(disposing);
        }
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
        }
        #endregion

        #region Event Helper Methods for Data Binding
        internal void InvokePropertyValidate(BeepComponentEventArgs args)
        {
            PropertyValidate?.Invoke(this, args);
        }

        internal void InvokeOnValueChanged(BeepComponentEventArgs args)
        {
            OnValueChanged?.Invoke(this, args);
        }

        internal void InvokeOnLinkedValueChanged(BeepComponentEventArgs args)
        {
            OnLinkedValueChanged?.Invoke(this, args);
        }

        internal void InvokePropertyChanged(BeepComponentEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        internal void InvokeSubmitChanges(BeepComponentEventArgs args)
        {
            SubmitChanges?.Invoke(this, args);
        }
        #endregion
    }
}
