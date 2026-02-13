using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;



namespace TheTechIdea.Beep.Winform.Controls.Base
{
    public partial class BaseControl
    {
        #region Painter strategy
        private bool _istransparent = false;
        // Tracks whether a caller explicitly set IsTransparentBackground. When true, theme/application code should not override this property.
        private bool _explicitIsTransparent = false;
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsTransparentBackground
        {
            get => _istransparent;
            set
            {
                if (_istransparent == value) return;
                _istransparent = value;
                // Mark as an explicit developer action so theme application respects it
                _explicitIsTransparent = true;
                InvalidateParentBackgroundCache();
                Invalidate();
            }
        }

        /// <summary>
        /// Sets IsTransparentBackground as a result of a style/theme application. This will not overwrite an explicit developer-set value.
        /// </summary>
        /// <param name="value">Desired transparency value from theme or style.</param>
        internal void SetIsTransparentBackgroundFromTheme(bool value)
        {
            if (_explicitIsTransparent) return; // don't override explicit developer preference
            if (_istransparent == value) return;
            _istransparent = value;
            InvalidateParentBackgroundCache();
            Invalidate();
        }

        /// <summary>
        /// Clears explicit IsTransparentBackground flag so subsequent theme changes can control transparency.
        /// </summary>
        public void ClearExplicitIsTransparentFlag()
        {
            _explicitIsTransparent = false;
        }

        private BeepControlStyle _borderPainterStyle = BeepControlStyle.None;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Select the painter (renderer) used to draw the control border. Synchronized with ControlStyle.")]
        public BeepControlStyle BorderPainter
        {
            get => _borderPainterStyle;
            set
            {
                if (_borderPainterStyle == value) return;
                _borderPainterStyle = value;
                
                // CRITICAL: Also synchronize ControlStyle when BorderPainter changes directly
                if (_controlstyle != value)
                {
                    _controlstyle = value;
                }
                
                UpdateBorderPainter();
                Invalidate();
            }
        }
        private IBorderPainter _currentBorderPainter = null;
        private BaseControlPainterKind _painterKind = BaseControlPainterKind.Classic;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Select the painter (renderer) used to draw the control. Auto picks Material when Material ProgressBarStyle is enabled, otherwise Classic.")]
        public BaseControlPainterKind PainterKind
        {
            get => _painterKind;
            set
            {
                if (_painterKind == value) return;
                _painterKind = value;
                UpdatePainterFromKind();
                Invalidate();
            }
        }
        public bool EnableMaterialStyle { get;set; } = false;

        #endregion

        #region Text Property Override
        protected Font _textFont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {

                _textFont = value;

                SafeApplyFont(_textFont);
                UseThemeFont = false;
                Invalidate();


            }
        }
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
        public DrawingLayer ExternalDrawingLayer { get; set; } =  DrawingLayer.AfterAll;  
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
                
                // Invalidate cached parent background when theme changes
                InvalidateParentBackgroundCache();
                ApplyTheme();
            }
        }
     
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

        // Data binding context
        public object DataContext { get; set; }

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

        #region Direct Appearance Properties (Previously Delegated)
        /// <summary>
        /// Alternative sizing mode for Material Design - preserves content area instead of following Material specs
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Material Design")]
        [Description("When enabled, preserves the original content area size instead of following Material Design size specifications")]
        [DefaultValue(false)]
        public bool MaterialPreserveContentArea
        {
            get => _materialPreserveContentArea;
            set
            {
                if (_materialPreserveContentArea == value) return;
                _materialPreserveContentArea = value;
                OnMaterialPropertyChanged();
                UpdateMaterialLayout();
                Invalidate();
            }
        }
        private bool _materialPreserveContentArea = false;

        // Basic appearance
        [Browsable(true)]
        public bool ShowAllBorders
        {
            get => _showAllBorders;
            set { if (_showAllBorders == value) return; _showAllBorders = value; Invalidate(); }
        }
        private bool _showAllBorders = false;

        [Browsable(true)]
        public bool ShowTopBorder
        {
            get => _showTopBorder;
            set { if (_showTopBorder == value) return; _showTopBorder = value; Invalidate(); }
        }
        private bool _showTopBorder = false;

        [Browsable(true)]
        public bool ShowBottomBorder
        {
            get => _showBottomBorder;
            set { if (_showBottomBorder == value) return; _showBottomBorder = value; Invalidate(); }
        }
        private bool _showBottomBorder = false;

        [Browsable(true)]
        public bool ShowLeftBorder
        {
            get => _showLeftBorder;
            set { if (_showLeftBorder == value) return; _showLeftBorder = value; Invalidate(); }
        }
        private bool _showLeftBorder = false;

        [Browsable(true)]
        public bool ShowRightBorder
        {
            get => _showRightBorder;
            set { if (_showRightBorder == value) return; _showRightBorder = value; Invalidate(); }
        }
        private bool _showRightBorder = false;

        [Browsable(true)]
        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                if (_borderThickness == value) return;
                _borderThickness = Math.Max(0, value);
                UpdateControlRegion();
                Invalidate();
            }
        }
        private int _borderThickness = 1;

        /// <summary>
        /// Custom padding added on top of style padding for additional spacing control.
        /// Allows asymmetric padding (e.g., only top padding for title labels).
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Additional custom padding to add on top of style padding (supports asymmetric values)")]
        public Padding CustomPadding
        {
            get => _customPadding;
            set
            {
                if (_customPadding == value) return;
                _customPadding = value;
                UpdateControlRegion();
                Invalidate();
            }
        }
        private Padding _customPadding = Padding.Empty;

        [Browsable(true)]
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                if (_borderRadius == value) return;
                _borderRadius = Math.Max(0, value);
                UpdateControlRegion();
                Invalidate();
            }
        }
        private int _borderRadius = 8;

        [Browsable(true)]
        public bool IsRounded
        {
            get => _isRounded;
            set
            {
                if (_isRounded == value) return;
                _isRounded = value;
                UpdateControlRegion();
                Invalidate();
            }
        }
        private bool _isRounded = true;

        [Browsable(true)]
        public DashStyle BorderDashStyle
        {
            get => _borderDashStyle;
            set { if (_borderDashStyle == value) return; _borderDashStyle = value; Invalidate(); }
        }
        private DashStyle _borderDashStyle = DashStyle.Solid;

        [Browsable(true)]
        public Color InactiveBorderColor
        {
            get => _inactiveBorderColor;
            set { if (_inactiveBorderColor == value) return; _inactiveBorderColor = value; Invalidate(); }
        }
        private Color _inactiveBorderColor = Color.Gray;

        // Shadow
        [Browsable(true)]
        public bool ShowShadow
        {
            get => _showShadow;
            set { if (_showShadow == value) return; _showShadow = value; Invalidate(); }
        }
        private bool _showShadow = false;

        [Browsable(true)]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { if (_shadowColor == value) return; _shadowColor = value; Invalidate(); }
        }
        private Color _shadowColor = Color.Black;

        [Browsable(true)]
        public float ShadowOpacity
        {
            get => _shadowOpacity;
            set { if (Math.Abs(_shadowOpacity - value) < float.Epsilon) return; _shadowOpacity = Math.Max(0f, Math.Min(1f, value)); Invalidate(); }
        }
        private float _shadowOpacity = 0.25f;

        [Browsable(true)]
        public int ShadowOffset
        {
            get => _shadowOffset;
            set { if (_shadowOffset == value) return; _shadowOffset = Math.Max(0, value); Invalidate(); }
        }
        private int _shadowOffset = 3;

        // Gradients
        [Browsable(true)]
        public bool UseGradientBackground
        {
            get => _useGradientBackground;
            set { if (_useGradientBackground == value) return; _useGradientBackground = value; Invalidate(); }
        }
        private bool _useGradientBackground = false;

        [Browsable(true)]
        public LinearGradientMode GradientDirection
        {
            get => _gradientDirection;
            set { if (_gradientDirection == value) return; _gradientDirection = value; Invalidate(); }
        }
        private LinearGradientMode _gradientDirection = LinearGradientMode.Horizontal;

        [Browsable(true)]
        public Color GradientStartColor
        {
            get => _gradientStartColor;
            set { if (_gradientStartColor == value) return; _gradientStartColor = value; Invalidate(); }
        }
        private Color _gradientStartColor = Color.LightGray;

        [Browsable(true)]
        public Color GradientEndColor
        {
            get => _gradientEndColor;
            set { if (_gradientEndColor == value) return; _gradientEndColor = value; Invalidate(); }
        }
        private Color _gradientEndColor = Color.Gray;

        // Modern gradients
        [Browsable(true)]
        public ModernGradientType ModernGradientType
        {
            get => _modernGradientType;
            set { if (_modernGradientType == value) return; _modernGradientType = value; Invalidate(); }
        }
        private ModernGradientType _modernGradientType = ModernGradientType.None;

        [Browsable(true)]
        public List<GradientStop> GradientStops
        {
            get => _gradientStops;
            set { _gradientStops = value ?? new List<GradientStop>(); Invalidate(); }
        }
        private List<GradientStop> _gradientStops = new List<GradientStop>();

        [Browsable(true)]
        public PointF RadialCenter
        {
            get => _radialCenter;
            set { if (_radialCenter == value) return; _radialCenter = value; Invalidate(); }
        }
        private PointF _radialCenter = new PointF(0.5f, 0.5f);

        [Browsable(true)]
        public float GradientAngle
        {
            get => _gradientAngle;
            set { if (Math.Abs(_gradientAngle - value) < float.Epsilon) return; _gradientAngle = value; Invalidate(); }
        }
        private float _gradientAngle = 0f;

        [Browsable(true)]
        public bool UseGlassmorphism
        {
            get => _useGlassmorphism;
            set { if (_useGlassmorphism == value) return; _useGlassmorphism = value; Invalidate(); }
        }
        private bool _useGlassmorphism = false;

        [Browsable(true)]
        public float GlassmorphismBlur
        {
            get => _glassmorphismBlur;
            set { if (Math.Abs(_glassmorphismBlur - value) < float.Epsilon) return; _glassmorphismBlur = Math.Max(0f, value); Invalidate(); }
        }
        private float _glassmorphismBlur = 10f;

        [Browsable(true)]
        public float GlassmorphismOpacity
        {
            get => _glassmorphismOpacity;
            set { if (Math.Abs(_glassmorphismOpacity - value) < float.Epsilon) return; _glassmorphismOpacity = Math.Max(0f, Math.Min(1f, value)); Invalidate(); }
        }
        private float _glassmorphismOpacity = 0.1f;

        // Material UI - kept for compatibility (material painter removed)
        [Browsable(false)] // Hidden - use StylePreset instead 
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MaterialTextFieldVariant MaterialBorderVariant
        {
            get => _materialBorderVariant;
            set
            {
                if (_materialBorderVariant == value) return;
                _materialBorderVariant = value;
                OnMaterialPropertyChanged();
                UpdateMaterialLayout();
                Invalidate();
            }
        }
        private MaterialTextFieldVariant _materialBorderVariant = MaterialTextFieldVariant.Standard;

        [Browsable(true)]
        public bool FloatingLabel
        {
            get => _floatingLabel;
            set { if (_floatingLabel == value) return; _floatingLabel = value; OnMaterialPropertyChanged(); UpdateMaterialLayout(); Invalidate(); }
        }
        private bool _floatingLabel = true;

        private bool _showLabelText = false;
        public bool LabelTextOn
        {
            get => _showLabelText;
            set { if (_showLabelText == value) return; _showLabelText = value; OnMaterialPropertyChanged(); UpdateMaterialLayout(); Invalidate(); }
        }


        [Browsable(true)]
        public string LabelText
        {
            get => _labelText;
            set { if (string.Equals(_labelText, value, StringComparison.Ordinal)) return; _labelText = value ?? string.Empty; OnMaterialPropertyChanged(); UpdateMaterialLayout(); UpdateExternalLabelHelperDrawing(); Invalidate(); }
        }
        private string _labelText = string.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show label above the border (floating on border) or above the control")]
        public bool ShowLabelAboveBorder
        {
            get => _showLabelAboveBorder;
            set { if (_showLabelAboveBorder == value) return; _showLabelAboveBorder = value; UpdateMaterialLayout(); Invalidate(); }
        }
        private bool _showLabelAboveBorder = false;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Position of the label text (Left, Center, Right)")]
        public LabelPosition LabelPosition
        {
            get => _labelPosition;
            set { if (_labelPosition == value) return; _labelPosition = value; Invalidate(); }
        }
        private LabelPosition _labelPosition = LabelPosition.Left;

        private bool _showHelperText = false;
        public bool HelperTextOn
        {
            get => _showHelperText;
            set { if (_showHelperText == value) return; _showHelperText = value; OnMaterialPropertyChanged(); UpdateMaterialLayout(); Invalidate(); }
        }

        [Browsable(true)]
        public string HelperText
        {
            get => _helperText;
            set { if (string.Equals(_helperText, value, StringComparison.Ordinal)) return; _helperText = value ?? string.Empty; OnMaterialPropertyChanged(); UpdateMaterialLayout(); UpdateExternalLabelHelperDrawing(); Invalidate(); }
        }
        private string _helperText = string.Empty;

        [Browsable(true)]
        public Color FocusBorderColor
        {
            get => _focusBorderColor;
            set { if (_focusBorderColor == value) return; _focusBorderColor = value; Invalidate(); }
        }
        private Color _focusBorderColor = Color.RoyalBlue;

        [Browsable(true)]
        public Color FilledBackgroundColor
        {
            get => _filledBackgroundColor;
            set { if (_filledBackgroundColor == value) return; _filledBackgroundColor = value; Invalidate(); }
        }
        private Color _filledBackgroundColor = Color.FromArgb(20, Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ShadowColor ?? Color.Black);
        // Badge
        private string _badgeText = "";
        [Browsable(true)] public virtual string BadgeText
        {
            get => _badgeText;
            set
            {
                if(value.Equals(_badgeText)) return;
                _badgeText = value;
             //   UpdateRegionForBadge();
                Invalidate();
            }
        }
        [Browsable(true)] public Color BadgeBackColor { get; set; } = Color.Red;
        [Browsable(true)] public Color BadgeForeColor { get; set; } = Color.White;
        [Browsable(true)] public Font BadgeFont { get; set; } = new Font("Segoe UI", 7, FontStyle.Bold);
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
        [Browsable(true)] public Color TempBackColor { get; set; }= Color.LightGray;
        // Effects
        [Browsable(true)] public bool ShowFocusIndicator { get => _effects.ShowFocusIndicator; set { _effects.ShowFocusIndicator = value; Invalidate(); } }
        [Browsable(true)] public Color FocusIndicatorColor { get => _effects.FocusIndicatorColor; set { _effects.FocusIndicatorColor = value; Invalidate(); } }
        [Browsable(true)] public bool EnableRippleEffect { get => _effects.EnableRippleEffect; set => _effects.EnableRippleEffect = value; }
        [Browsable(true)] public DisplayAnimationType AnimationType { get => _effects.AnimationType; set { _effects.AnimationType = value; Invalidate(); } }
        [Browsable(true)] public int AnimationDuration { get => _effects.AnimationDuration; set { _effects.AnimationDuration = value; Invalidate(); } }
        [Browsable(true)] public EasingType Easing { get => _effects.Easing; set { _effects.Easing = value; Invalidate(); } }
        [Browsable(true)] public SlideDirection SlideFrom { get => _effects.SlideFrom; set { _effects.SlideFrom = value; Invalidate(); } }

        #region Material Design Properties
        // Additional BeepControl parity properties
        private bool _canBeSelected = true;
        [Browsable(true)]
        public bool CanBeSelected
        {
            get => _canBeSelected   ;
            set
            {
                if (_canBeSelected == value) return;
                _canBeSelected = value;
                if (!value && IsSelected) IsSelected = false;
                Invalidate();
            }
        }
        private bool _canBeHovered = true;
        [Browsable(true)]
        public bool CanBeHovered
        {
            get => _canBeHovered;
            set
            {
                if (_canBeHovered == value) return;
                _canBeHovered = value;
                if (!value && IsHovered) IsHovered = false;
                Invalidate();
            }
        }

        private bool _canBePressed = true;
        [Browsable(true)]
        public bool CanBePressed
        {
            get => _canBePressed;
            set
            {
                if (_canBePressed == value) return;
                _canBePressed = value;
                if (!value && IsPressed) IsPressed = false;
                Invalidate();
            }
        }

        // Note: BaseControl.cs has a private field `_canBeFocused` and CanFocus() uses it.
        // Keep them in sync here.
        [Browsable(true)]
        public bool CanBeFocused
        {
            get => _canBeFocused;
            set
            {
                if (_canBeFocused == value) return;
                _canBeFocused = value;
                try
                {
                    // Allow keyboard focus/tab navigation when true
                    this.SetStyle(ControlStyles.Selectable, value);
                    this.TabStop = value;
                }
                catch { /* design-time safe */ }
                Invalidate();
            }
        }

        private bool _isBorderAffectedByTheme = true;
        [Browsable(true)]
        public bool IsBorderAffectedByTheme
        {
            get => _isBorderAffectedByTheme;
            set
            {
                if (_isBorderAffectedByTheme == value) return;
                _isBorderAffectedByTheme = value;
                Invalidate();
            }
        }

        private bool _isShadowAffectedByTheme = true;
        [Browsable(true)]
        public bool IsShadowAffectedByTheme
        {
            get => _isShadowAffectedByTheme;
            set
            {
                if (_isShadowAffectedByTheme == value) return;
                _isShadowAffectedByTheme = value;
                Invalidate();
            }
        }

        private bool _isRoundedAffectedByTheme = true;
        [Browsable(true)]
        public bool IsRoundedAffectedByTheme
        {
            get => _isRoundedAffectedByTheme;
            set
            {
                if (_isRoundedAffectedByTheme == value) return;
                _isRoundedAffectedByTheme = value;
                UpdateControlRegion();
                Invalidate();
            }
        }

        private BorderStyle _borderStyle = BorderStyle.FixedSingle;
        [Browsable(true)]
        public new BorderStyle BorderStyle
        {
            get => _borderStyle;
            set
            {
                if (_borderStyle == value) return;
                _borderStyle = value;
                Invalidate();
            }
        }

      
     
        private GraphicsPath _borderpath;

        [Browsable(false)]
        public GraphicsPath BorderPath
        {
            get { return _borderpath; }
            set
            {
                _borderpath = value;
                //Invalidate();
            }
        }
        private GraphicsPath _outerShape;
        [Browsable(false)]
        public GraphicsPath OuterShape
        {
            get { return _outerShape; }
            set
            {
                _outerShape = value;
                //Invalidate();
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
        private GraphicsPath _contentShape;
        [Browsable(false)]
        public GraphicsPath ContentShape
        {
            get { return _contentShape; }
            set
            {
                _contentShape = value;
                //Invalidate();
            }
        }
        //used for child controls drawing area inside _innerShape
        private Rectangle _drawingRect;
        [Browsable(false)]
        public Rectangle DrawingRect
        {
            get { return _drawingRect; }
            set
            {
                if (_drawingRect == value) return;
                _drawingRect = value;
                // Do not Invalidate here; drawing pipeline sets this every frame
            }
        }
        
        private Rectangle _borderRect;
        [Browsable(false)]
        public Rectangle BorderRect
        {
            get { return _borderRect; }
            set { _borderRect = value; }
        }

        private Rectangle _contentRect;
        [Browsable(false)]
        public Rectangle ContentRect
        {
            get { return _contentRect; }
            set { _contentRect = value; }
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

        /// <summary>
        /// Updates painter layout (replaces material helper usage)
        /// </summary>
        private void UpdateMaterialLayout()
        {
            EnsurePainter();
            _painter?.UpdateLayout(this);
        }
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

        #region BeepContextMenu
        private ContextMenus.BeepContextMenu _beepContextMenu;

        [Browsable(true)]
        [Category("Beep")]
        [Description("Context menu for this control. All inherited controls can access this property.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ContextMenus.BeepContextMenu BeepContextMenu
        {
            get => _beepContextMenu;
            set
            {
                _beepContextMenu = value;
                Invalidate();
            }
        }
        #endregion

        #region Painting and Layout
        private bool _useFormStylePaint = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use the form's style/painter to render this control.")]
        [DefaultValue(false)]
        public bool UseFormStylePaint
        {
            get => _useFormStylePaint;
            set
            {
                if (_useFormStylePaint != value)
                {
                    _useFormStylePaint = value;
                    Invalidate();
                }
            }
        }
        private BeepControlStyle _controlstyle = BeepControlStyle.None;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual style/painter to use for rendering the sidebar.")]
        [DefaultValue(BeepControlStyle.None)]
        public BeepControlStyle ControlStyle
        {
            get => _controlstyle;
            set
            {
                if (_controlstyle != value)
                {
                    var oldStyle = _controlstyle; // capture old style for delta-based resize
                    _controlstyle = value;

                    // CRITICAL: Synchronize BorderPainter with ControlStyle
                    // This ensures border style, color, and width update when ControlStyle changes
                    if (_borderPainterStyle != value)
                    {
                        _borderPainterStyle = value;
                        UpdateBorderPainter();
                    }
                    // Get corresponding FormStyle from ControlStyle

                    Theme = BeepStyling.GetThemeStyle(_controlstyle);
                    // Update tooltip if it uses ControlStyle
                    UpdateTooltipTheme();
                    
                    // Auto-resize to compensate chrome when style-based painting is used
                    try
                    {
                        if (UseFormStylePaint && !IsDisposed)
                        {
                            AdjustSizeForControlStyle(oldStyle, value);
                        }
                    }
                    catch { /* design-time safe */ }
                    
                    // Invalidate cached parent background when style changes
                    InvalidateParentBackgroundCache();
                    Invalidate();
                }
            }
        }
        private BeepButtonShapeType _shapeType = BeepButtonShapeType.Default;
        /// <summary>
        /// Overrides the geometric shape of the control path, independent of
        /// <see cref="ControlStyle"/>. When set to <see cref="BeepButtonShapeType.Default"/>,
        /// the shape is determined by the current <see cref="ControlStyle"/>.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Override the control's geometric shape (Pill, Stadium, Elevated, etc.). Default = shape from ControlStyle.")]
        [DefaultValue(BeepButtonShapeType.Default)]
        public BeepButtonShapeType ShapeType
        {
            get => _shapeType;
            set
            {
                if (_shapeType != value)
                {
                    _shapeType = value;
                    Invalidate();
                }
            }
        }

        private bool _useThemeColors = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use theme colors instead of custom accent color.")]
        [DefaultValue(true)]
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set
            {
                _useThemeColors = value;
                Invalidate();
            }
        }
        #endregion
    }
}

namespace TheTechIdea.Beep.Winform.Controls.Base
{
    public partial class BaseControl
    {
        /// <summary>
        /// Delta-based resize: computes the chrome (shadow + border) for old and new styles,
        /// then grows or shrinks the control by the difference so the content area stays the same.
        /// </summary>
        private void AdjustSizeForControlStyle(BeepControlStyle oldStyle, BeepControlStyle newStyle)
        {
            // Do nothing if style-based painting is not used
            if (!UseFormStylePaint)
                return;

            int oldChrome = CalculateStyleChrome(oldStyle);
            int newChrome = CalculateStyleChrome(newStyle);
            int delta = newChrome - oldChrome;

            if (delta == 0) return;

            int newW = Math.Max(16, Width + delta);
            int newH = Math.Max(16, Height + delta);

            // Only set when different to avoid layout churn
            if (Width != newW || Height != newH)
            {
                Size = new Size(newW, newH);
            }
        }

        /// <summary>
        /// Calculates the total chrome (non-content overhead) for a given control style.
        /// Chrome = (shadow margin + border thickness) on each side, doubled for both sides.
        /// </summary>
        private static int CalculateStyleChrome(BeepControlStyle style)
        {
            int border = (int)Math.Ceiling(StyleBorders.GetBorderWidth(style));

            int shadow = 0;
            if (StyleShadows.HasShadow(style))
            {
                int blur = StyleShadows.GetShadowBlur(style);
                int offX = Math.Abs(StyleShadows.GetShadowOffsetX(style));
                int offY = Math.Abs(StyleShadows.GetShadowOffsetY(style));
                shadow = Math.Max(blur, Math.Max(offX, offY));
            }

            // Total chrome = (shadow + border) per side, doubled for both sides
            return (shadow + border) * 2;
        }
    }
}
