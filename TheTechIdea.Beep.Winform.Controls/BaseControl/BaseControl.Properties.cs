using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Vis.Modules.Managers;



namespace TheTechIdea.Beep.Winform.Controls.Base
{
    public partial class BaseControl
    {
        #region Text Property Override
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The text displayed by the control.")]
        public override string Text
        {
            get => _text;
            set
            {
                if (_isInitializing) return;
                
                if (value != null && value == "BaseControl")
                {
                    value = "";
                }

                _text = value;
                OnTextChanged(EventArgs.Empty);
                Invalidate();
            }
        }
        #endregion

        #region Info Property
        [Category("Appearance")]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SimpleItem Info
        {
            get => _info;
            set => _info = value;
        }
        #endregion

        #region Tooltips (Rich)
        private string _toolTipText = string.Empty;

        [Browsable(false)]
        public bool UseRichToolTip { get; set; } = true;
        #endregion

        #region Static/Location Control
        [Browsable(true)]
        [Category("Behavior")]
        public bool StaticNotMoving 
        { 
            get => _staticNotMoving; 
            set 
            { 
                _staticNotMoving = value; 
                _originalLocation = Location; 
            } 
        }
        #endregion

        #region DPI Scaling Support
        [Browsable(true)]
        [Category("DPI/Scaling")]
        [Description("Disable all DPI/scaling logic for this control (AutoScale, DPI-based size/font scaling).")]
    private bool _disableDpiAndScaling = false; // default opt-out of custom scaling
        public bool DisableDpiAndScaling
        {
            get => _disableDpiAndScaling;
            set
            {
                _disableDpiAndScaling = value;
                // Do not force AutoScale changes; let the framework/parent form handle scaling
                Invalidate();
            }
        }

        protected float DpiScaleFactor => DisableDpiAndScaling || _dpi == null ? 1.0f : _dpi.DpiScaleFactor;

        protected virtual void UpdateDpiScaling(Graphics g)
        {
            if (DisableDpiAndScaling || _dpi == null) return;
            _dpi.UpdateDpiScaling(g);
        }
        protected int ScaleValue(int value) => (DisableDpiAndScaling || _dpi == null) ? value : _dpi.ScaleValue(value);
        protected Size ScaleSize(Size size) => (DisableDpiAndScaling || _dpi == null) ? size : _dpi.ScaleSize(size);
        protected Font ScaleFont(Font font) => (DisableDpiAndScaling || _dpi == null) ? font : _dpi.ScaleFont(font);
        #endregion

        #region Theme Management
        [Browsable(true)]
        [Category("Appearance")]
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

        [Browsable(true)]
        [Category("Appearance")]
        public bool ApplyThemeToChilds { get; set; } = true;
        #endregion

        #region Font Management
        private bool _useThemeFont = true;
        private TypeStyleFontSize _overrideFontSize = TypeStyleFontSize.None;

        [Browsable(true)]
        [Category("Appearance")]
        public bool UseThemeFont
        {
            get => _useThemeFont;
            set
            {
                _useThemeFont = value;
                if (value) SetFont();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public TypeStyleFontSize OverrideFontSize
        {
            get => _overrideFontSize;
            set
            {
                _overrideFontSize = value;
                SetFont();
                Invalidate();
            }
        }
        #endregion

        #region IBeepUIComponent Basic Properties
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

        public IBeepUIComponent Form { get; set; }
        public string GuidID { get => _guidId; set => _guidId = value; }
        public string BlockID { get => _blockId; set => _blockId = value; }
        public string FieldID { get => _fieldId; set => _fieldId = value; }
        public int Id { get => _id; set => _id = value; }
        public List<object> Items { get => _items; set => _items = value ?? new List<object>(); }
        
        public object SelectedValue 
        { 
            get => _selectedValue; 
            set 
            { 
                _selectedValue = value; 
                OnSelected?.Invoke(this, new BeepComponentEventArgs(this, BoundProperty, LinkedProperty, value)); 
            } 
        }

        public virtual string BoundProperty 
        { 
            get => _boundProperty; 
            set 
            { 
                _boundProperty = value;
                if (DataContext != null)
                {
                    _dataBinding.SetBinding(_boundProperty, DataSourceProperty);
                }
            } 
        }
        
        public string DataSourceProperty 
        { 
            get => _dataSourceProperty; 
            set 
            { 
                _dataSourceProperty = value;
                if (DataContext != null)
                {
                    _dataBinding.SetBinding(BoundProperty, _dataSourceProperty);
                }
            } 
        }
        
        public string LinkedProperty { get => _linkedProperty; set => _linkedProperty = value; }

        public object Oldvalue => _oldValue;
        
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }
        public bool IsRequired { get => _isRequired; set => _isRequired = value; }
        public bool IsSelected 
        { 
            get => _isSelected; 
            set 
            { 
                _isSelected = value; 
                Invalidate(); 
                if (value) OnSelected?.Invoke(this, new BeepComponentEventArgs(this, BoundProperty, LinkedProperty, GetValue())); 
            } 
        }
        
        public bool IsDeleted { get => _isDeleted; set => _isDeleted = value; }
        public bool IsNew { get => _isNew; set => _isNew = value; }
        public bool IsDirty { get => _isDirty; set => _isDirty = value; }
        public bool IsReadOnly { get => _isReadOnly; set => _isReadOnly = value; }
        public bool IsEditable { get => _isEditable; set => _isEditable = value; }
        public bool IsVisible { get => _isVisible; set { _isVisible = value; Visible = value; } }
        public bool IsFrameless { get => _isFrameless; set { _isFrameless = value; Invalidate(); } }
        public DbFieldCategory Category { get => _category; set => _category = value; }

        // Validation parity
        [Browsable(false)]
        public bool IsValid { get; set; } = true;
        #endregion

        #region BeepControl Parity: Parent/Child Theming
        protected bool _isChild = true;
        protected Color _parentBackColor = Color.Empty;
        protected Color _tempBackColor = Color.Empty;
        private Control _parentControl;

        [Browsable(true)]
        [Category("Appearance")]
        public Control ParentControl
        {
            get => _parentControl;
            set
            {
                _parentControl = value;
                if (value != null)
                {
                    _parentBackColor = value.BackColor;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color ParentBackColor
        {
            get => _parentBackColor;
            set
            {
                _parentBackColor = value;
                // Invalidate();  // Trigger repaint
            }
        }
      
        [Browsable(true)]
        [Category("Appearance")]
        public Color TempBackColor
        {
            get => _tempBackColor;
            set
            {
                _tempBackColor = value;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsChild
        {
            get => _isChild;
            set
            {
                _isChild = value;
                if (this.Parent != null)
                {
                    if (value)
                    {
                        _parentBackColor = this.Parent.BackColor;
                        _tempBackColor = BackColor;
                        BackColor = _parentBackColor;
                        
                    }
                    else
                    {
                        BackColor = _tempBackColor;
                    }
                }

                Invalidate();  // Trigger repaint
            }
        }

        #endregion

        #region Hit Testing Properties
        public List<ControlHitTest> HitList { get { return  _hitTest.HitList; } set {  _hitTest.HitList= value; } }
        public ControlHitTest HitTestControl { get { return _hitTest.HitTestControl; } set { _hitTest.HitTestControl = value; } }
      
        public bool HitAreaEventOn { get { return _hitTest.HitAreaEventOn; } set { _hitTest.HitAreaEventOn = value; } }
       
        #endregion

        #region External Drawing Properties
        public DrawingLayer ExternalDrawingLayer 
        { 
            get => _externalDrawing.ExternalDrawingLayer; 
            set => _externalDrawing.ExternalDrawingLayer = value; 
        }
        #endregion

        #region Direct Appearance Properties (Previously Delegated)
        /// <summary>
        /// Property to enable automatic size compensation for Material Design
        /// </summary>
        [Browsable(false)] // Hidden - use StylePreset instead
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Category("Material Design")]
        [Description("Automatically adjusts control size to accommodate Material Design spacing requirements")]
        [DefaultValue(true)]
        public bool MaterialAutoSizeCompensation
        {
            get => _materialAutoSizeCompensation;
            set
            {
                if (_materialAutoSizeCompensation != value)
                {
                    _materialAutoSizeCompensation = value;

                    // Trigger immediate compensation when enabled
                    if (value && EnableMaterialStyle && !_isInitializing)
                    {
                        ApplyMaterialSizeCompensation();
                    }

                    // Trigger property changed handler
                    OnMaterialPropertyChanged();

                    // Update layout if material helper exists
                    UpdateMaterialLayout();

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Alternative sizing mode for Material Design - preserves content area instead of following Material specs
        /// </summary>
        [Browsable(true)] // Hidden - use StylePreset instead
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Category("Material Design")]
        [Description("When enabled, preserves the original content area size instead of following Material Design size specifications")]
        [DefaultValue(false)]
        public bool MaterialPreserveContentArea { get; set; } = false;

        // Basic appearance
        [Browsable(true)] public bool ShowAllBorders { get; set; } = false;
        [Browsable(true)] public bool ShowTopBorder { get; set; } = false;
        [Browsable(true)] public bool ShowBottomBorder { get; set; } = false;
        [Browsable(true)] public bool ShowLeftBorder { get; set; } = false;
        [Browsable(true)] public bool ShowRightBorder { get; set; } = false;
        [Browsable(true)] public int BorderThickness { get; set; } = 1;
        [Browsable(true)] public int BorderRadius { get; set; } = 8;
        [Browsable(true)] public bool IsRounded { get; set; } = true;
        [Browsable(true)] public DashStyle BorderDashStyle { get; set; } = DashStyle.Solid;
        [Browsable(true)] public Color InactiveBorderColor { get; set; } = Color.Gray;

        // Shadow
        [Browsable(true)] public bool ShowShadow { get; set; } = false;
        [Browsable(true)] public Color ShadowColor { get; set; } = Color.Black;
        [Browsable(true)] public float ShadowOpacity { get; set; } = 0.25f;
        [Browsable(true)] public int ShadowOffset { get; set; } = 3;

        // Gradients
        [Browsable(true)] public bool UseGradientBackground { get; set; } = false;
        [Browsable(true)] public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Horizontal;
        [Browsable(true)] public Color GradientStartColor { get; set; } = Color.LightGray;
        [Browsable(true)] public Color GradientEndColor { get; set; } = Color.Gray;

        // Modern gradients
        [Browsable(true)] public ModernGradientType ModernGradientType { get; set; } = ModernGradientType.None;
        [Browsable(true)] public List<GradientStop> GradientStops { get; set; } = new List<GradientStop>();
        [Browsable(true)] public PointF RadialCenter { get; set; } = new PointF(0.5f, 0.5f);
        [Browsable(true)] public float GradientAngle { get; set; } = 0f;
        [Browsable(true)] public bool UseGlassmorphism { get; set; } = false;
        [Browsable(true)] public float GlassmorphismBlur { get; set; } = 10f;
        [Browsable(true)] public float GlassmorphismOpacity { get; set; } = 0.1f;

        // Material UI - Hide specific Material Design properties to avoid conflicts with StylePreset
        [Browsable(false)] // Hidden - use StylePreset instead 
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MaterialTextFieldVariant MaterialBorderVariant { get; set; } = MaterialTextFieldVariant.Standard;
        
        [Browsable(true)] public bool FloatingLabel { get; set; } = true;
        [Browsable(true)] public string LabelText { get; set; } = string.Empty;
        [Browsable(true)] public string HelperText { get; set; } = string.Empty;
        [Browsable(true)] public Color FocusBorderColor { get; set; } = Color.RoyalBlue;
        [Browsable(true)] public Color FilledBackgroundColor { get; set; } = Color.FromArgb(20, 0, 0, 0);

 

        // Badge
        private string _badgeText = "";
        [Browsable(true)]
        public virtual string BadgeText
        {
            get => _badgeText;
            set
            {
                if(value.Equals(_badgeText)) return;
                _badgeText = value;
                RegisterBadgeDrawer();
                UpdateRegionForBadge();
                Invalidate();
            }
        }
        [Browsable(true)] public Color BadgeBackColor { get; set; } = Color.Red;
        [Browsable(true)] public Color BadgeForeColor { get; set; } = Color.White;
        [Browsable(true)] public Font BadgeFont { get; set; } = new Font("Arial", 8, FontStyle.Bold);
        [Browsable(true)] public BadgeShape BadgeShape { get; set; } = BadgeShape.Circle;

        // State colors
        [Browsable(true)] public Color HoverBackColor { get; set; } = Color.LightBlue;
        [Browsable(true)] public Color HoverBorderColor { get; set; } = Color.Blue;
        [Browsable(true)] public Color HoverForeColor { get; set; } = Color.Black;
        [Browsable(true)] public Color PressedBackColor { get; set; } = Color.Gray;
        [Browsable(true)] public Color PressedBorderColor { get; set; } = Color.DarkGray;
        [Browsable(true)] public Color PressedForeColor { get; set; } = Color.White;
        [Browsable(true)] public Color FocusBackColor { get; set; } = Color.LightYellow;
        [Browsable(true)] public Color FocusForeColor { get; set; } = Color.Black;
        [Browsable(true)] public Color DisabledBackColor { get; set; } = Color.LightGray;
        [Browsable(true)] public Color DisabledBorderColor { get; set; } = Color.Gray;
        [Browsable(true)] public Color DisabledForeColor { get; set; } = Color.DarkGray;
        [Browsable(true)] public Color SelectedBackColor { get; set; } = Color.LightGreen;
        [Browsable(true)] public Color SelectedBorderColor { get; set; } = Color.Green;
        [Browsable(true)] public Color SelectedForeColor { get; set; } = Color.Black;

        // Effects
        [Browsable(true)] public bool ShowFocusIndicator { get => _effects.ShowFocusIndicator; set { _effects.ShowFocusIndicator = value; Invalidate(); } }
        [Browsable(true)] public Color FocusIndicatorColor { get => _effects.FocusIndicatorColor; set { _effects.FocusIndicatorColor = value; Invalidate(); } }
        [Browsable(true)] public bool EnableRippleEffect { get => _effects.EnableRippleEffect; set => _effects.EnableRippleEffect = value; }
        [Browsable(true)] public DisplayAnimationType AnimationType { get => _effects.AnimationType; set { _effects.AnimationType = value; Invalidate(); } }
        [Browsable(true)] public int AnimationDuration { get => _effects.AnimationDuration; set { _effects.AnimationDuration = value; Invalidate(); } }
        [Browsable(true)] public EasingType Easing { get => _effects.Easing; set { _effects.Easing = value; Invalidate(); } }
        [Browsable(true)] public SlideDirection SlideFrom { get => _effects.SlideFrom; set { _effects.SlideFrom = value; Invalidate(); } }

        #region Material Design Properties

        public enum MaterialOutsideBackgroundMode
        {
            ControlBackColor,
            ParentBackColor,
            Transparent
        }

        [Browsable(false)] // Hidden - use StylePreset instead
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Category("Material Design")]
        [Description("Determines how the area outside the material field is painted when Material Style is enabled.")]
        [DefaultValue(MaterialOutsideBackgroundMode.ParentBackColor)]
        public MaterialOutsideBackgroundMode MaterialOutsideBackground { get; set; } = MaterialOutsideBackgroundMode.ParentBackColor;

        // Additional BeepControl parity properties
        [Browsable(true)] public bool CanBeHovered { get; set; } = true;
        [Browsable(true)] public bool CanBePressed { get; set; } = true;
        [Browsable(true)] public bool CanBeFocused { get; set; } = true;
        [Browsable(true)] public bool IsBorderAffectedByTheme { get; set; } = true;
        [Browsable(true)] public bool IsShadowAffectedByTheme { get; set; } = true;
        [Browsable(true)] public bool IsRoundedAffectedByTheme { get; set; } = true;
        [Browsable(true)] public new BorderStyle BorderStyle { get; set; } = BorderStyle.FixedSingle;

        // Drawing rect and offsets
        [Browsable(false)] 
        public Rectangle DrawingRect
        {
            get { return _paint != null ? _paint.DrawingRect : Rectangle.Empty; }
            set 
            { 
                if (_paint != null)
                {
                    _paint.DrawingRect = value; 
                    _paint.UpdateRects();
                }
                Invalidate(); 
            }
        }
        private GraphicsPath _innerShape;
        // Drawing rect and offsets
        [Browsable(false)]
        public GraphicsPath InnerShape
        {
          get { return _innerShape; }
            set
            {
                _innerShape = value;
                //Invalidate();
            }
        }

        [Browsable(false)] public int LeftoffsetForDrawingRect { get; set; } = 0;
        [Browsable(false)] public int TopoffsetForDrawingRect { get; set; } = 0;
        [Browsable(false)] public int RightoffsetForDrawingRect { get; set; } = 0;
        [Browsable(false)] public int BottomoffsetForDrawingRect { get; set; } = 0;

        // Image scaling support
        [Browsable(true)] public ImageScaleMode ScaleMode { get; set; } = ImageScaleMode.KeepAspectRatio;

        // Utility properties for compatibility
        [Browsable(true)] public bool GridMode { get; set; } = false;
        [Browsable(true)] public bool IsSelectedOptionOn { get; set; } = false;
        
        // Additional state parity flags
        [Browsable(true)] public bool IsDefault { get; set; } = false;
        [Browsable(true)] public bool IsAcceptButton { get; set; } = false;
        [Browsable(true)] public bool IsCancelButton { get; set; } = false;

        // Custom border parity
        private bool _isCustomBorder = false;
        [Browsable(true)] public bool IsCustomeBorder { get => _isCustomBorder; set { _isCustomBorder = value; Invalidate(); } }
        #endregion

        #region Extra Parity Properties
        // Saved identifiers parity
        [Browsable(true)] public string SavedID { get; set; }
        [Browsable(true)] public string SavedGuidID { get; set; }

        // Alias to match BeepControl naming
        [Browsable(true)] public Color HoveredBackcolor { get => HoverBackColor; set { HoverBackColor = value; Invalidate(); } }
        #endregion

        #region Events
        public event EventHandler<BeepComponentEventArgs> PropertyChanged;
        public event EventHandler<BeepComponentEventArgs> PropertyValidate;
        public event EventHandler<BeepComponentEventArgs> OnSelected;
        public event EventHandler<BeepComponentEventArgs> OnValidate;
        public event EventHandler<BeepComponentEventArgs> OnValueChanged;
        public event EventHandler<BeepComponentEventArgs> OnLinkedValueChanged;
        public event EventHandler<BeepComponentEventArgs> SubmitChanges;

        public event EventHandler<ControlHitTestArgs> OnControlHitTest
        {
            add => _hitTest.OnControlHitTest += value;
            remove => _hitTest.OnControlHitTest -= value;
        }

        public event EventHandler<ControlHitTestArgs> HitDetected
        {
            add => _hitTest.HitDetected += value;
            remove => _hitTest.HitDetected -= value;
        }

        // Key events
        public event EventHandler TabKeyPressed
        {
            add => _input.TabKeyPressed += value;
            remove => _input.TabKeyPressed -= value;
        }

        public event EventHandler ShiftTabKeyPressed
        {
            add => _input.ShiftTabKeyPressed += value;
            remove => _input.ShiftTabKeyPressed -= value;
        }

        public event EventHandler EnterKeyPressed
        {
            add => _input.EnterKeyPressed += value;
            remove => _input.EnterKeyPressed -= value;
        }

        public event EventHandler EscapeKeyPressed
        {
            add => _input.EscapeKeyPressed += value;
            remove => _input.EscapeKeyPressed -= value;
        }

        public event EventHandler LeftArrowKeyPressed
        {
            add => _input.LeftArrowKeyPressed += value;
            remove => _input.LeftArrowKeyPressed -= value;
        }

        public event EventHandler RightArrowKeyPressed
        {
            add => _input.RightArrowKeyPressed += value;
            remove => _input.RightArrowKeyPressed -= value;
        }

        public event EventHandler UpArrowKeyPressed
        {
            add => _input.UpArrowKeyPressed += value;
            remove => _input.UpArrowKeyPressed -= value;
        }

        public event EventHandler DownArrowKeyPressed
        {
            add => _input.DownArrowKeyPressed += value;
            remove => _input.DownArrowKeyPressed -= value;
        }

        public event EventHandler PageUpKeyPressed
        {
            add => _input.PageUpKeyPressed += value;
            remove => _input.PageUpKeyPressed -= value;
        }

        public event EventHandler PageDownKeyPressed
        {
            add => _input.PageDownKeyPressed += value;
            remove => _input.PageDownKeyPressed -= value;
        }

        public event EventHandler HomeKeyPressed
        {
            add => _input.HomeKeyPressed += value;
            remove => _input.HomeKeyPressed -= value;
        }

        public event EventHandler EndKeyPressed
        {
            add => _input.EndKeyPressed += value;
            remove => _input.EndKeyPressed -= value;
        }

        public event EventHandler<KeyEventArgs> DialogKeyDetected
        {
            add => _input.DialogKeyDetected += value;
            remove => _input.DialogKeyDetected -= value;
        }
        #endregion
        #endregion
    }
}