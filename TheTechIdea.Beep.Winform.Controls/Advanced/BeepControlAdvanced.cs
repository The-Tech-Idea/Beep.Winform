using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules; // IBeepUIComponent, IBeepTheme, DbFieldCategory, HitTestEventArgs
using TheTechIdea.Beep.Report; // AppFilter
using TheTechIdea.Beep.Winform.Controls.Models; // MouseEventType
using TheTechIdea.Beep.Winform.Controls.Advanced.Helpers;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls.Advanced
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Control Advanced")]
    [Description("Lightweight Beep base control for building other controls (future replacement for BeepControl).")]
    public class BeepControlAdvanced : ContainerControl, IBeepUIComponent
    {
        // keep fields minimal
        private readonly ToolTip _toolTip;
        private string _themeName;
        private IBeepTheme _currentTheme; // optional; used only if provided
        private string _guidId = Guid.NewGuid().ToString();
        private string _blockId;
        private string _fieldId;
        private int _id;
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
        private DbFieldCategory _category = DbFieldCategory.String;
        private Color _borderColor = Color.Black;

        // simple state used by effects
        internal bool IsHovered { get; private set; }
        internal bool IsPressed { get; private set; }

        // Helpers
        private readonly ControlPaintHelper _paint;
        private readonly ControlEffectHelper _effects;
        private readonly ControlHitTestHelper _hitTest;
        private readonly ControlInputHelper _input;

        public BeepControlAdvanced()
        {
            // Let Windows handle DPI
            AutoScaleMode = AutoScaleMode.Dpi;

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            _toolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 500,
                ReshowDelay = 500,
                ShowAlways = true
            };

            _paint = new ControlPaintHelper(this);
            _effects = new ControlEffectHelper(this);
            _hitTest = new ControlHitTestHelper(this);
            _input = new ControlInputHelper(this, _effects, _hitTest);
        }

        #region Theme
        [Browsable(true)]
        [Category("Appearance")]
        public string Theme
        {
            get => _themeName;
            set
            {
                _themeName = value;
                ApplyTheme();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool ApplyThemeToChilds { get; set; } = true;

        public virtual void ApplyTheme()
        {
            if (_currentTheme != null)
            {
                ForeColor = _currentTheme.ForeColor;
                BackColor = _currentTheme.BackColor;
                BorderColor = _currentTheme.BorderColor;
            }

            if (ApplyThemeToChilds)
            {
                foreach (Control c in Controls)
                {
                    var themeProp = TypeDescriptor.GetProperties(c)["Theme"];
                    themeProp?.SetValue(c, Theme);
                }
            }

            Invalidate();
        }

        public virtual void ApplyTheme(string theme) => Theme = theme;
        public virtual void ApplyTheme(IBeepTheme theme) { _currentTheme = theme; ApplyTheme(); }
        #endregion

        #region IBeepUIComponent implementation
        [Browsable(true)]
        [Category("Data")]
        public string ComponentName
        {
            get => Name;
            set
            {
                Name = value;
                PropertyChanged?.Invoke(this, new BeepComponentEventArgs(this, nameof(ComponentName), LinkedProperty, value));
            }
        }

        public Size GetSize() => new Size(Width, Height);

        public new string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public void ShowToolTip(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            _toolTip.Show(text, this, PointToClient(MousePosition), 3000);
        }

        public void HideToolTip() => _toolTip.Hide(this);

        public IBeepUIComponent Form { get; set; }

        public string GuidID { get => _guidId; set => _guidId = value; }
        public string BlockID { get => _blockId; set => _blockId = value; }
        public string FieldID { get => _fieldId; set => _fieldId = value; }
        public int Id { get => _id; set => _id = value; }
        public List<object> Items { get => _items; set => _items = value ?? new List<object>(); }
        public object SelectedValue { get => _selectedValue; set { _selectedValue = value; OnSelected?.Invoke(this, new BeepComponentEventArgs(this, BoundProperty, LinkedProperty, value)); } }

        public bool ValidateData(out string messege)
        {
            var args = new BeepComponentEventArgs(this, BoundProperty, LinkedProperty, GetValue());
            PropertyValidate?.Invoke(this, args);
            messege = args.Message;
            return !args.Cancel;
        }

        public string BoundProperty { get => _boundProperty; set => _boundProperty = value; }
        public string DataSourceProperty { get => _dataSourceProperty; set => _dataSourceProperty = value; }
        public string LinkedProperty { get => _linkedProperty; set => _linkedProperty = value; }

        [Browsable(true)]
        [Category("Appearance")]
        public string ToolTipText { get; set; } = string.Empty;

        public void SetValue(object value)
        {
            _oldValue = GetValue();
            if (!string.IsNullOrEmpty(BoundProperty))
            {
                var prop = GetType().GetProperty(BoundProperty);
                prop?.SetValue(this, value);
            }
            else
            {
                base.Text = value?.ToString() ?? string.Empty;
            }
            IsDirty = true;
            OnValueChanged?.Invoke(this, new BeepComponentEventArgs(this, BoundProperty, LinkedProperty, value));
        }

        public object GetValue()
        {
            if (!string.IsNullOrEmpty(BoundProperty))
            {
                var prop = GetType().GetProperty(BoundProperty);
                return prop?.GetValue(this);
            }
            return base.Text;
        }

        public object Oldvalue => _oldValue;
        public void ClearValue() => SetValue(null);
        public bool HasFilterValue() => !string.IsNullOrEmpty(BoundProperty) && GetValue() != null;

        public AppFilter ToFilter()
        {
            return new AppFilter
            {
                FieldName = BoundProperty,
                FilterValue = GetValue()?.ToString(),
                Operator = "=",
                valueType = "string"
            };
        }

        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }
        public bool IsRequired { get => _isRequired; set => _isRequired = value; }
        public bool IsSelected { get => _isSelected; set { _isSelected = value; Invalidate(); if (value) OnSelected?.Invoke(this, new BeepComponentEventArgs(this, BoundProperty, LinkedProperty, GetValue())); } }
        public bool IsDeleted { get => _isDeleted; set => _isDeleted = value; }
        public bool IsNew { get => _isNew; set => _isNew = value; }
        public bool IsDirty { get => _isDirty; set => _isDirty = value; }
        public bool IsReadOnly { get => _isReadOnly; set => _isReadOnly = value; }
        public bool IsEditable { get => _isEditable; set => _isEditable = value; }
        public bool IsVisible { get => _isVisible; set { _isVisible = value; Visible = value; } }
        public bool IsFrameless { get => _isFrameless; set { _isFrameless = value; Invalidate(); } }
        public DbFieldCategory Category { get => _category; set => _category = value; }

        public void Draw(Graphics graphics, Rectangle rectangle) { /* extension point */ }
        public void SuspendFormLayout() => SuspendLayout();
        public void ResumeFormLayout() { ResumeLayout(false); PerformLayout(); }

        public void ReceiveMouseEvent(HitTestEventArgs eventArgs) => _input.ReceiveMouseEvent(eventArgs);
        public virtual void SendMouseEvent(IBeepUIComponent targetControl, MouseEventType eventType, Point screenLocation) => _hitTest.SendMouseEvent(targetControl, eventType, screenLocation);

        public void SetBinding(string controlProperty, string dataSourceProperty)
        {
            BoundProperty = controlProperty;
            DataSourceProperty = dataSourceProperty;

            var existing = DataBindings[controlProperty];
            if (existing != null) DataBindings.Remove(existing);

            var binding = new Binding(controlProperty, this, dataSourceProperty)
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            };
            DataBindings.Add(binding);
        }

        public event EventHandler<BeepComponentEventArgs> PropertyChanged;
        public event EventHandler<BeepComponentEventArgs> PropertyValidate;
        public event EventHandler<BeepComponentEventArgs> OnSelected;
        public event EventHandler<BeepComponentEventArgs> OnValidate;
        public event EventHandler<BeepComponentEventArgs> OnValueChanged;
        public event EventHandler<BeepComponentEventArgs> OnLinkedValueChanged;
        public event EventHandler<BeepComponentEventArgs> SubmitChanges;

        public void RaiseSubmitChanges() => SubmitChanges?.Invoke(this, new BeepComponentEventArgs(this, BoundProperty, LinkedProperty, GetValue()));
        #endregion

        #region Paint pipeline (delegated)
        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            _paint.UpdateRects();
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _paint.UpdateRects();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _paint.Draw(e.Graphics);
            _effects.DrawOverlays(e.Graphics);
        }
        #endregion

        #region Mouse routing to helpers
        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); IsHovered = true; _input.OnMouseEnter(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); IsHovered = false; _input.OnMouseLeave(); }
        protected override void OnMouseMove(MouseEventArgs e) { base.OnMouseMove(e); _input.OnMouseMove(e.Location); }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); if (e.Button==MouseButtons.Left) IsPressed = true; _input.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); if (e.Button==MouseButtons.Left) IsPressed = false; _input.OnMouseUp(e); }
        protected override void OnMouseHover(EventArgs e) { base.OnMouseHover(e); _input.OnMouseHover(); }
        protected override void OnClick(EventArgs e) { base.OnClick(e); _input.OnClick(); }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _toolTip?.Dispose();
            }
            base.Dispose(disposing);
        }

        // Appearance wrappers delegating to paint/effects helpers
        #region Appearance wrappers
        [Browsable(true)] public bool ShowAllBorders { get => _paint.ShowAllBorders; set { _paint.ShowAllBorders = value; Invalidate(); } }
        [Browsable(true)] public int BorderThickness { get => _paint.BorderThickness; set { _paint.BorderThickness = value; Invalidate(); } }
        [Browsable(true)] public int BorderRadius { get => _paint.BorderRadius; set { _paint.BorderRadius = value; Invalidate(); } }
        [Browsable(true)] public bool IsRounded { get => _paint.IsRounded; set { _paint.IsRounded = value; Invalidate(); } }
        [Browsable(true)] public bool ShowShadow { get => _paint.ShowShadow; set { _paint.ShowShadow = value; Invalidate(); } }
        [Browsable(true)] public Color ShadowColor { get => _paint.ShadowColor; set { _paint.ShadowColor = value; Invalidate(); } }
        [Browsable(true)] public float ShadowOpacity { get => _paint.ShadowOpacity; set { _paint.ShadowOpacity = value; Invalidate(); } }
        [Browsable(true)] public int ShadowOffset { get => _paint.ShadowOffset; set { _paint.ShadowOffset = value; Invalidate(); } }
        [Browsable(false)] public Rectangle DrawingRect => _paint.DrawingRect;

        [Browsable(true)] public bool ShowFocusIndicator { get => _effects.ShowFocusIndicator; set { _effects.ShowFocusIndicator = value; Invalidate(); } }
        [Browsable(true)] public Color FocusIndicatorColor { get => _effects.FocusIndicatorColor; set { _effects.FocusIndicatorColor = value; Invalidate(); } }
        [Browsable(true)] public bool EnableRippleEffect { get => _effects.EnableRippleEffect; set => _effects.EnableRippleEffect = value; }
        #endregion
    }
}
