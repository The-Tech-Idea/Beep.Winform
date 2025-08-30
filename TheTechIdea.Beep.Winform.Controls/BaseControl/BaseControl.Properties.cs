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
        protected float DpiScaleFactor => _dpi.DpiScaleFactor;

        protected virtual void UpdateDpiScaling(Graphics g) => _dpi.UpdateDpiScaling(g);
        protected int ScaleValue(int value) => _dpi.ScaleValue(value);
        protected Size ScaleSize(Size size) => _dpi.ScaleSize(size);
        protected Font ScaleFont(Font font) => _dpi.ScaleFont(font);
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
        public bool IsValid
        {
            get => _paint.IsValid;
            private set { _paint.IsValid = value; }
        }
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

        #region Appearance Property Wrappers
        // Basic appearance
        [Browsable(true)] public bool ShowAllBorders { get => _paint.ShowAllBorders; set { _paint.ShowAllBorders = value; if (value && BorderThickness == 0) BorderThickness = 1; Invalidate(); } }
        [Browsable(true)] public bool ShowTopBorder { get => _paint.ShowTopBorder; set { _paint.ShowTopBorder = value; Invalidate(); } }
        [Browsable(true)] public bool ShowBottomBorder { get => _paint.ShowBottomBorder; set { _paint.ShowBottomBorder = value; Invalidate(); } }
        [Browsable(true)] public bool ShowLeftBorder { get => _paint.ShowLeftBorder; set { _paint.ShowLeftBorder = value; Invalidate(); } }
        [Browsable(true)] public bool ShowRightBorder { get => _paint.ShowRightBorder; set { _paint.ShowRightBorder = value; Invalidate(); } }
        [Browsable(true)] public int BorderThickness { get => _paint.BorderThickness; set { _paint.BorderThickness = value; Invalidate(); } }
        [Browsable(true)] public int BorderRadius { get => _paint.BorderRadius; set { _paint.BorderRadius = value; Invalidate();  } }
        [Browsable(true)] public bool IsRounded { get => _paint.IsRounded; set { _paint.IsRounded = value; Invalidate();  } }
        [Browsable(true)] public DashStyle BorderDashStyle { get => _paint.BorderDashStyle; set { _paint.BorderDashStyle = value; Invalidate(); } }
        [Browsable(true)] public Color InactiveBorderColor { get => _paint.InactiveBorderColor; set { _paint.InactiveBorderColor = value; Invalidate(); } }

        // Shadow
        [Browsable(true)] public bool ShowShadow { get => _paint.ShowShadow; set { _paint.ShowShadow = value; Invalidate(); } }
        [Browsable(true)] public Color ShadowColor { get => _paint.ShadowColor; set { _paint.ShadowColor = value; Invalidate(); } }
        [Browsable(true)] public float ShadowOpacity { get => _paint.ShadowOpacity; set { _paint.ShadowOpacity = value; Invalidate(); } }
        [Browsable(true)] public int ShadowOffset { get => _paint.ShadowOffset; set { _paint.ShadowOffset = value; Invalidate(); } }

        // Gradients
        [Browsable(true)] public bool UseGradientBackground { get => _paint.UseGradientBackground; set { _paint.UseGradientBackground = value; Invalidate(); } }
        [Browsable(true)] public LinearGradientMode GradientDirection { get => _paint.GradientDirection; set { _paint.GradientDirection = value; Invalidate(); } }
        [Browsable(true)] public Color GradientStartColor { get => _paint.GradientStartColor; set { _paint.GradientStartColor = value; Invalidate(); } }
        [Browsable(true)] public Color GradientEndColor { get => _paint.GradientEndColor; set { _paint.GradientEndColor = value; Invalidate(); } }

        // Modern gradients
        [Browsable(true)] public ModernGradientType ModernGradientType { get => _paint.ModernGradientType; set { _paint.ModernGradientType = value; Invalidate(); } }
        [Browsable(false)] public List<GradientStop> GradientStops { get => _paint.GradientStops; set { _paint.GradientStops = value; Invalidate(); } }
        [Browsable(true)] public PointF RadialCenter { get => _paint.RadialCenter; set { _paint.RadialCenter = value; Invalidate(); } }
        [Browsable(true)] public float GradientAngle { get => _paint.GradientAngle; set { _paint.GradientAngle = value; Invalidate(); } }
        [Browsable(true)] public bool UseGlassmorphism { get => _paint.UseGlassmorphism; set { _paint.UseGlassmorphism = value; Invalidate(); } }
        [Browsable(true)] public float GlassmorphismBlur { get => _paint.GlassmorphismBlur; set { _paint.GlassmorphismBlur = value; Invalidate(); } }
        [Browsable(true)] public float GlassmorphismOpacity { get => _paint.GlassmorphismOpacity; set { _paint.GlassmorphismOpacity = value; Invalidate(); } }

        // Material UI
        [Browsable(true)] public MaterialTextFieldVariant MaterialBorderVariant { get => _paint.MaterialBorderVariant; set { _paint.MaterialBorderVariant = value; Invalidate(); } }
        [Browsable(true)] public bool FloatingLabel { get => _paint.FloatingLabel; set { _paint.FloatingLabel = value; Invalidate(); } }
        [Browsable(true)] public string LabelText { get => _paint.LabelText; set { _paint.LabelText = value; Invalidate(); } }
        [Browsable(true)] public string HelperText { get => _paint.HelperText; set { _paint.HelperText = value; Invalidate(); } }
        [Browsable(true)] public Color FocusBorderColor { get => _paint.FocusBorderColor; set { _paint.FocusBorderColor = value; Invalidate(); } }
        [Browsable(true)] public Color FilledBackgroundColor { get => _paint.FilledBackgroundColor; set { _paint.FilledBackgroundColor = value; Invalidate(); } }

        // React UI
        [Browsable(true)] public ReactUIVariant UIVariant { get => _paint.UIVariant; set { _paint.UIVariant = value; Invalidate(); } }
        [Browsable(true)] public ReactUISize UISize { get => _paint.UISize; set { _paint.UISize = value; Invalidate(); } }
        [Browsable(true)] public ReactUIColor UIColor { get => _paint.UIColor; set { _paint.UIColor = value; Invalidate(); } }
        [Browsable(true)] public ReactUIDensity UIDensity { get => _paint.UIDensity; set { _paint.UIDensity = value; Invalidate(); } }
        [Browsable(true)] public ReactUIElevation UIElevation { get => _paint.UIElevation; set { _paint.UIElevation = value; Invalidate(); } }
        [Browsable(true)] public ReactUIShape UIShape { get => _paint.UIShape; set { _paint.UIShape = value; Invalidate(); } }
        [Browsable(true)] public ReactUIAnimation UIAnimation { get => _paint.UIAnimation; set { _paint.UIAnimation = value; Invalidate(); } }
        [Browsable(true)] public bool UIFullWidth { get => _paint.UIFullWidth; set { _paint.UIFullWidth = value; Invalidate(); } }
        [Browsable(true)] public int UICustomElevation { get => _paint.UICustomElevation; set { _paint.UICustomElevation = value; Invalidate(); } }
        [Browsable(true)] public bool UIDisabled 
        { 
            get => !Enabled; 
            set => Enabled = !value; 
        }

        // Badge
        [Browsable(true)]
        public virtual string BadgeText
        {
            get => _paint.BadgeText;
            set
            {
                if(value.Equals(_paint.BadgeText)) return;
                _paint.BadgeText = value;
                RegisterBadgeDrawer();
                UpdateRegionForBadge();
                Invalidate();
            }
        }
                //if (Parent is BaseControl parent) 
                //{ if (!string.IsNullOrEmpty(value)) 
                //        parent.AddChildExternalDrawing(this, DrawBadgeExternally, DrawingLayer.AfterAll); 
                //    else parent.ClearChildExternalDrawing(this); } } }
        [Browsable(true)] public Color BadgeBackColor { get => _paint.BadgeBackColor; set { _paint.BadgeBackColor = value; UpdateRegionForBadge(); Invalidate(); } }
        [Browsable(true)] public Color BadgeForeColor { get => _paint.BadgeForeColor; set { _paint.BadgeForeColor = value; UpdateRegionForBadge(); Invalidate(); } }
        [Browsable(true)] public Font BadgeFont { get => _paint.BadgeFont; set { _paint.BadgeFont = value; UpdateRegionForBadge(); Invalidate(); } }
        [Browsable(true)] public BadgeShape BadgeShape { get => _paint.BadgeShape; set { _paint.BadgeShape = value; UpdateRegionForBadge(); Invalidate(); } }

        // State colors
        [Browsable(true)] public Color HoverBackColor { get => _paint.HoverBackColor; set { _paint.HoverBackColor = value; Invalidate(); } }
        [Browsable(true)] public Color HoverBorderColor { get => _paint.HoverBorderColor; set { _paint.HoverBorderColor = value; Invalidate(); } }
        [Browsable(true)] public Color HoverForeColor { get => _paint.HoverForeColor; set { _paint.HoverForeColor = value; Invalidate(); } }
        [Browsable(true)] public Color PressedBackColor { get => _paint.PressedBackColor; set { _paint.PressedBackColor = value; Invalidate(); } }
        [Browsable(true)] public Color PressedBorderColor { get => _paint.PressedBorderColor; set { _paint.PressedBorderColor = value; Invalidate(); } }
        [Browsable(true)] public Color PressedForeColor { get => _paint.PressedForeColor; set { _paint.PressedForeColor = value; Invalidate(); } }
        [Browsable(true)] public Color FocusBackColor { get => _paint.FocusBackColor; set { _paint.FocusBackColor = value; Invalidate(); } }
        [Browsable(true)] public Color FocusForeColor { get => _paint.FocusForeColor; set { _paint.FocusForeColor = value; Invalidate(); } }
        [Browsable(true)] public Color DisabledBackColor { get => _paint.DisabledBackColor; set { _paint.DisabledBackColor = value; Invalidate(); } }
        [Browsable(true)] public Color DisabledBorderColor { get => _paint.DisabledBorderColor; set { _paint.DisabledBorderColor = value; Invalidate(); } }
        [Browsable(true)] public Color DisabledForeColor { get => _paint.DisabledForeColor; set { _paint.DisabledForeColor = value; Invalidate(); } }
        [Browsable(true)] public Color SelectedBackColor { get => _paint.SelectedBackColor; set { _paint.SelectedBackColor = value; Invalidate(); } }
        [Browsable(true)] public Color SelectedBorderColor { get => _paint.SelectedBorderColor; set { _paint.SelectedBorderColor = value; Invalidate(); } }
        [Browsable(true)] public Color SelectedForeColor { get => _paint.SelectedForeColor; set { _paint.SelectedForeColor = value; Invalidate(); } }

        // Effects
        [Browsable(true)] public bool ShowFocusIndicator { get => _effects.ShowFocusIndicator; set { _effects.ShowFocusIndicator = value; Invalidate(); } }
        [Browsable(true)] public Color FocusIndicatorColor { get => _effects.FocusIndicatorColor; set { _effects.FocusIndicatorColor = value; Invalidate(); } }
        [Browsable(true)] public bool EnableRippleEffect { get => _effects.EnableRippleEffect; set => _effects.EnableRippleEffect = value; }
        [Browsable(true)] public DisplayAnimationType AnimationType { get => _effects.AnimationType; set { _effects.AnimationType = value; Invalidate(); } }
        [Browsable(true)] public int AnimationDuration { get => _effects.AnimationDuration; set { _effects.AnimationDuration = value; Invalidate(); } }
        [Browsable(true)] public EasingType Easing { get => _effects.Easing; set { _effects.Easing = value; Invalidate(); } }
        [Browsable(true)] public SlideDirection SlideFrom { get => _effects.SlideFrom; set { _effects.SlideFrom = value; Invalidate(); } }

        // Additional BeepControl parity properties
        [Browsable(true)] public bool CanBeHovered { get => _paint.CanBeHovered; set => _paint.CanBeHovered = value; }
        [Browsable(true)] public bool CanBePressed { get => _paint.CanBePressed; set => _paint.CanBePressed = value; }
        [Browsable(true)] public bool CanBeFocused { get => _paint.CanBeFocused; set => _paint.CanBeFocused = value; }
        [Browsable(true)] public bool IsBorderAffectedByTheme { get => _paint.IsBorderAffectedByTheme; set => _paint.IsBorderAffectedByTheme = value; }
        [Browsable(true)] public bool IsShadowAffectedByTheme { get => _paint.IsShadowAffectedByTheme; set => _paint.IsShadowAffectedByTheme = value; }
        [Browsable(true)] public bool IsRoundedAffectedByTheme { get => _paint.IsRoundedAffectedByTheme; set => _paint.IsRoundedAffectedByTheme = value; }
        [Browsable(true)] public new BorderStyle BorderStyle { get => _paint.BorderStyle; set { _paint.BorderStyle = value; Invalidate(); } }

        // Drawing rect and offsets
        [Browsable(false)] public Rectangle DrawingRect  {get { return _paint.DrawingRect; }
            set { _paint.DrawingRect = value; _paint.UpdateRects(); Invalidate(); }
        }
        [Browsable(false)] public int LeftoffsetForDrawingRect { get => _paint.LeftoffsetForDrawingRect; set { _paint.LeftoffsetForDrawingRect = value; _paint.UpdateRects(); Invalidate(); } }
        [Browsable(false)] public int TopoffsetForDrawingRect { get => _paint.TopoffsetForDrawingRect; set { _paint.TopoffsetForDrawingRect = value; _paint.UpdateRects(); Invalidate(); } }
        [Browsable(false)] public int RightoffsetForDrawingRect { get => _paint.RightoffsetForDrawingRect; set { _paint.RightoffsetForDrawingRect = value; _paint.UpdateRects(); Invalidate(); } }
        [Browsable(false)] public int BottomoffsetForDrawingRect { get => _paint.BottomoffsetForDrawingRect; set { _paint.BottomoffsetForDrawingRect = value; _paint.UpdateRects(); Invalidate(); } }

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
        [Browsable(true)] public Color HoveredBackcolor { get => _paint.HoverBackColor; set { _paint.HoverBackColor = value; Invalidate(); } }
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
    }
}