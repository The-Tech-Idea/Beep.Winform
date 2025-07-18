﻿using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;
using System.Drawing.Design;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using System.IO;
using TheTechIdea.Beep.Winform.Controls.Models;
using LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode;




namespace TheTechIdea.Beep.Winform.Controls
{
   
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Control")]
    [Description("A control that provides a base for all Beep UI components.")]
    public class BeepControl : ContainerControl, IBeepUIComponent,IDisposable
    {
        #region "Delegates"
       
        #endregion "Delegates"
        #region "protected Properties"
        Point originalLocation;

        protected bool _isControlinvalidated = false;
        protected bool tooltipShown = false; // Flag to track if tooltip is shown
        protected ImageScaleMode _scaleMode = ImageScaleMode.KeepAspectRatio;
        protected bool _staticnotmoving = false;
        protected ToolTip _toolTip;
        // Border properties

        protected bool _showAllBorders = false; // Keep as is
        protected Color _focusIndicatorColor = Color.Blue; // Keep as is
        protected bool _showFocusIndicator = false; // Keep as is
        protected bool _showTopBorder; // Keep as is
        protected bool _showBottomBorder; // Keep as is
        protected bool _showLeftBorder; // Keep as is
        protected bool _showRightBorder; // Keep as is
        protected int _borderThickness = 1; // Keep as is
        protected int _borderRadius = 8; // Increase from 3 to 8
        protected BorderStyle _borderStyle = BorderStyle.FixedSingle;
        protected Color _borderColor = Color.Black;
        //-----------------------------------------------
        
        /// </summary>
        protected bool _isRounded = true;
        protected bool _useGradientBackground = false;
        protected LinearGradientMode _gradientDirection = LinearGradientMode.Horizontal;
        protected Color _gradientStartColor = Color.Gray;
        protected Color _gradientEndColor = Color.Gray;
        protected bool _showShadow = false;
        protected Color _shadowColor = Color.Black;
        protected float _shadowOpacity = 0.5f;

        protected Color _hoverBackColor = Color.White;
        protected Color _pressedBackColor = Color.White;
        protected Color _focusBackColor = Color.White;
        protected Color _disabledBackColor = Color.White;
        protected Color _selectedBackColor = Color.White;



        protected Color _hoverBorderColor = Color.Gray;
        protected Color _pressedBorderColor = Color.Gray;
        protected Color _focusBorderColor = Color.Gray;
        protected Color _disabledBorderColor = Color.Gray;
        protected Color _selectedBorderColor = Color.Gray;


        protected Color _hoverForeColor = Color.Black;
        protected Color _pressedForeColor = Color.Black;
        protected Color _focusForeColor = Color.Black;
        protected Color _disabledForeColor = Color.Black;
        protected Color _selectedForeColor = Color.Black;


        protected string _tooltipText = string.Empty;
        protected Color _originalBackColor;
        protected System.Windows.Forms.Timer _animationTimer;
        protected float _opacity = 0f; // Initial opacity for fade animations
        private int _animationElapsedTime;
        private Rectangle _slideStartRect;
        private Rectangle _slideEndRect;
        protected int scrollOffsetX = 0;
        protected int scrollOffsetY = 0;
        protected Size virtualSize = new Size(0, 0);
      
        protected bool _autoScroll = false;
        protected int shadowOffset = 5;
        protected int _tmpShadowOffset = 5;
        protected DashStyle _borderDashStyle = DashStyle.Solid;
        protected SlideDirection _slideFrom = SlideDirection.Left;
        protected DisplayAnimationType _animationType = DisplayAnimationType.None;
        protected int _animationDuration = 500;
        protected string _guidID = Guid.NewGuid().ToString();
        protected int _id = -1;
        protected List<object> _items = new List<object>();
        protected bool _isHovered = false;
        protected bool _isPressed = false;
        protected bool _isFocused = false;
        protected bool _isDefault = false;
        protected bool _isAcceptButton = false;
        protected bool _isCancelButton = false;
        protected bool _isframless = false;
        protected Color _hoveredBackcolor = Color.Wheat;
        protected TypeStyleFontSize _overridefontsize = TypeStyleFontSize.None;
        protected string _text = string.Empty;
        protected bool _isborderaffectedbytheme = true;
        protected bool _isshadowaffectedbytheme = true;
        private bool _isroundedffectedbytheme = true;
        private bool _applythemetochilds = false;
        private int _topoffsetForDrawingRect = 0;
        private int _leftoffsetForDrawingRect = 0;
        private int _bottomoffsetForDrawingRect = 0;
        private int _rightoffsetForDrawingRect = 0;
        private bool _canbehovered = false;
        private bool _canbepressed = true;
        private bool _canbefocused = true;
        private bool _canbedefault = false;
        private Rectangle borderRectangle;

        #endregion "protected Properties"
        #region "Diagramming Properties"

        #endregion "Diagramming Properties"
        #region "React-Style UI Properties"
        // Add these properties to BeepControl
        [Category("React UI")]
        [Description("Determines the visual variant style of the control similar to React components")]
        public ReactUIVariant UIVariant { get; set; } = ReactUIVariant.Default;

        [Category("React UI")]
        [Description("Sets the size of the control similar to React component sizing")]
        public ReactUISize UISize { get; set; } = ReactUISize.Medium;

        [Category("React UI")]
        [Description("Sets the color theme of the control similar to React component theme colors")]
        public ReactUIColor UIColor { get; set; } = ReactUIColor.Primary;

        [Category("React UI")]
        [Description("Controls the density of the component layout similar to React components")]
        public ReactUIDensity UIDensity { get; set; } = ReactUIDensity.Standard;

        [Category("React UI")]
        [Description("Sets the elevation/shadow level for the control")]
        public ReactUIElevation UIElevation { get; set; } = ReactUIElevation.None;

        [Category("React UI")]
        [Description("Controls the corner shape of the UI element")]
        public ReactUIShape UIShape { get; set; } = ReactUIShape.Rounded;

        [Category("React UI")]
        [Description("Sets the animation style for user interactions")]
        public ReactUIAnimation UIAnimation { get; set; } = ReactUIAnimation.None;

        [Category("React UI")]
        [Description("Determines if the control should take full width of its container")]
        public bool UIFullWidth { get; set; } = false;

        [Category("React UI")]
        [Description("Determines whether the component is disabled")]
        public bool UIDisabled
        {
            get => !Enabled;
            set => Enabled = !value;
        }

        [Category("React UI")]
        [Description("Custom shadow/elevation level when UIElevation is set to Custom")]
        public int UICustomElevation { get; set; } = 0;
        // Add these properties to BeepControl
        [Category("Material UI")]
        [Description("Material UI TextField border style variant")]
        public MaterialTextFieldVariant MaterialBorderVariant { get; set; } = MaterialTextFieldVariant.Standard;

        [Category("Material UI")]
        [Description("Whether the label should float when the control is focused or has content")]
        public bool FloatingLabel { get; set; } = true;

        [Category("Material UI")]
        [Description("Label text displayed with the control")]
        public string LabelText { get; set; } = string.Empty;

        [Category("Material UI")]
        [Description("Helper text displayed below the control")]
        public string HelperText { get; set; } = string.Empty;

        [Category("Material UI")]
        [Description("Color of the focused border/underline")]
        public Color FocusBorderColor { get; set; } = Color.RoyalBlue;

        // For Filled variant
        [Category("Material UI")]
        [Description("Background color when using the Filled variant")]
        public Color FilledBackgroundColor { get; set; } = Color.FromArgb(20, 0, 0, 0);

        [Category("Material UI")]
        [Description("Whether to show a ripple effect on click (Material Design style)")]
        public bool EnableRippleEffect { get; set; } = false;
        // Ripple effect properties
        private Point _rippleCenter;
        private float _rippleSize = 0;
        private bool _showRipple = false;
        private float _rippleOpacity = 1.0f;
        private System.Windows.Forms.Timer _rippleTimer;
        private bool _showMaterialRipple = false;
        private Point _rippleOrigin = Point.Empty;
        private float _rippleRadius = 0;

        #endregion
        #region "Public Properties"

        private bool _isselectedoptionon = false;
        public bool IsSelectedOptionOn
        {
            get => _isselectedoptionon;
            set
            {
                _isselectedoptionon = value;
                Invalidate();
            }
        }
        private bool _hitareaeventon = false;
        public bool HitAreaEventOn
        {
            get => _hitareaeventon;
            set
            {
                _hitareaeventon = value;
               
            }
        }
        private ControlHitTest _hitTestControl;
        public ControlHitTest HitTestControl
        {
            get => _hitTestControl;
            set
            {
                _hitTestControl = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Data")]
        [Description("The Text  represent for the control.")]
        public override string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnTextChanged(EventArgs.Empty);
                _isControlinvalidated = true;
                Invalidate();  // Trigger repaint when the text changes
            }
        }
        private SimpleItem _info = new SimpleItem();
        [Browsable(true)]
        [Category("Appearance")]
        public SimpleItem Info
        {
            get => _info;
            set => _info = value;
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool CanBeHovered { get { return _canbehovered; } set { _canbehovered = value; } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool CanBePressed { get { return _canbepressed; } set { _canbepressed = value; } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool CanBeFocused { get { return _canbefocused; } set { _canbefocused = value; } }
        [Browsable(false)]
        [Category("Appearance")]
        public int RightoffsetForDrawingRect { get { return _rightoffsetForDrawingRect; } set { _rightoffsetForDrawingRect = value; Invalidate(); } }
        [Browsable(false)]
        [Category("Appearance")]
        public int BottomoffsetForDrawingRect { get { return _bottomoffsetForDrawingRect; } set { _bottomoffsetForDrawingRect = value; Invalidate(); } }
        [Browsable(false)]
        [Category("Appearance")]
        public int TopoffsetForDrawingRect { get { return _topoffsetForDrawingRect; } set { _topoffsetForDrawingRect = value; Invalidate(); } }
        [Browsable(false)]
        [Category("Appearance")]
        public int LeftoffsetForDrawingRect { get { return _leftoffsetForDrawingRect; } set { _leftoffsetForDrawingRect = value; Invalidate(); } }
        //IsRoundedAffectedByTheme
        [Browsable(true)]
        [Category("Appearance")]
        public bool ApplyThemeToChilds { get { return _applythemetochilds; } set { _applythemetochilds = value; } }

        [Browsable(true)]
        [Category("Appearance")]
        public bool IsRoundedAffectedByTheme { get { return _isroundedffectedbytheme; } set { _isroundedffectedbytheme = value; } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsBorderAffectedByTheme { get { return _isborderaffectedbytheme; } set { _isborderaffectedbytheme = value; } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsShadowAffectedByTheme { get { return _isshadowaffectedbytheme; } set { _isshadowaffectedbytheme = value; } }
        // Make the Text property visible in the designer

        
        //[Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        //[Category("Appearance")]
        //[Description("The text associated with the Beepbutton.")]
        //public  override  string Text
        //{
        //    get => _text;
        //    set
        //    {
        //        _text = value;
        //        _isControlinvalidated = true;
        //        Invalidate();  // Trigger repaint when the text changes
        //    }
        //}
        [Browsable(true), Category("Appearance")]
        public TypeStyleFontSize OverrideFontSize
        {
            get { return _overridefontsize; }
            set { _overridefontsize = value; Invalidate(); }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsFrameless { get { return _isframless; } set { _isframless = value; Invalidate(); } }

        [Browsable(true)]
        [Category("Appearance")]
        public Color HoveredBackcolor { get { return _hoveredBackcolor; } set { _hoveredBackcolor = value;  } }


        [Browsable(true)]
        [Category("Appearance")]
        public bool IsHovered { get { return _isHovered; } set { if (CanBeHovered) { _isHovered = value;  } } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsPressed { get { return _isPressed; } set { if (CanBePressed) { _isPressed = value;  } } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsFocused { get { return _isFocused; } set { if (CanBeFocused) { _isFocused = value;  } } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsDefault { get { return _isDefault; } set { _isDefault = value; Invalidate(); } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsAcceptButton { get { return _isAcceptButton; } set { _isAcceptButton = value; }  }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsCancelButton { get { return _isCancelButton; } set { _isCancelButton = value; } }


        public string SavedID { get; set; }
        public string SavedGuidID { get; set; }


        protected bool _isChild=false;
        protected Color parentbackcolor;
        protected Color _tempbackcolor;

      
       

        [Browsable(true)]
        [Category("Appearance")]
        public int ShadowOffset
        {
            get => shadowOffset;
            set
            {
                shadowOffset = value;
                _tmpShadowOffset = value;
                Invalidate(); // Redraw if shadow offset changes
            }
        }
        //ParentControl
        private Control parentcontrol;
        [Browsable(true)]
        [Category("Appearance")]
        public Control ParentControl
        {
            get => parentcontrol;
            set
            {
                parentcontrol = value;
                if (value != null)
                {
                    parentbackcolor = value.BackColor;
                }
                Invalidate();  // Trigger repaint
            }
        }
            //TempBackColor
            [Browsable(true)]
        [Category("Appearance")]
        public Color TempBackColor
        {
            get => _tempbackcolor;
            set
            {
                _tempbackcolor = value;
               // Invalidate();  // Trigger repaint
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public Color ParentBackColor
        {
            get => parentbackcolor;
            set
            {
                parentbackcolor = value;
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
                        parentbackcolor = this.Parent.BackColor;
                        _tempbackcolor = BackColor;
                        BackColor = parentbackcolor;
                    }
                    else
                    {
                        BackColor = _tempbackcolor;
                    }
                }

                Invalidate();  // Trigger repaint
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool StaticNotMoving { get { return _staticnotmoving; } set { _staticnotmoving = value; originalLocation = this.Location; } }
        [Browsable(true)]
        public bool ShowFocusIndicator
        {
            get => _showFocusIndicator;
            set
            {
                _showFocusIndicator = value;
                Invalidate(); // Redraw if animation changes should reflect immediately
                              // InitializeAnimation(); // Reinitialize the animation with the new type
            }
        }
        public Color FocusIndicatorColor
        {
            get => _focusIndicatorColor;
            set
            {
                _focusIndicatorColor = value;
                Invalidate(); // Redraw if animation changes should reflect immediately
                              // InitializeAnimation(); // Reinitialize the animation with the new type
            }
        }
        public SlideDirection SlideFrom
        {
            get => _slideFrom;
            set
            {
                _slideFrom = value;
                Invalidate(); // Redraw to reflect animation start direction if needed
                //InitializeAnimation(); // Reinitialize the animation with the new direction
            }
        }
        public DisplayAnimationType AnimationType
        {
            get => _animationType;
            set
            {
                _animationType = value;
                Invalidate(); // Redraw if animation changes should reflect immediately
                              // InitializeAnimation(); // Reinitialize the animation with the new type
            }
        }
        public int AnimationDuration
        {
            get => _animationDuration;
            set
            {
                _animationDuration = value;
                Invalidate(); // Redraw or reinitialize based on duration changes
                              //   InitializeAnimation(); // Reinitialize the animation with the new duration
            }
        }
        protected EasingType _easing = EasingType.Linear;
        public EasingType Easing
        {
            get => _easing;
            set
            {
                _easing = value;
                Invalidate(); // Trigger redraw if the easing change affects appearance

            }
        }
        // Border control properties
        // Custom DashStyle for the border
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The dash style of the border.")]
        public DashStyle BorderDashStyle
        {
            get => _borderDashStyle;
            set
            {
                _borderDashStyle = value;
                Invalidate(); // Redraw the control when the dash style changes
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Config the top border.")]
        public bool ShowTopBorder
        {
            get => _showTopBorder;
            set
            {
                _showTopBorder = value;
                Invalidate(); // Redraw when this property changes
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Config the bottom border.")]
        public bool ShowBottomBorder
        {
            get => _showBottomBorder;
            set
            {
                _showBottomBorder = value;
                Invalidate(); // Redraw when this property changes
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Config the left border.")]
        public bool ShowLeftBorder
        {
            get => _showLeftBorder;
            set
            {
                _showLeftBorder = value;
                Invalidate(); // Redraw when this property changes
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Config the right border.")]
        public bool ShowRightBorder
        {
            get => _showRightBorder;
            set
            {
                _showRightBorder = value;
                Invalidate(); // Redraw when this property changes
            }
        }
        public new virtual bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                Invalidate(); // Redraw when the enabled state changes
            }
        }
            // ShowAllBorders Property
            [Browsable(true)]
        [Category("Appearance")]
        [Description("Set all Borders.")]
        public virtual bool ShowAllBorders
        {
            get => ShowTopBorder && ShowBottomBorder && ShowLeftBorder && ShowRightBorder;
            set
            {
                _showAllBorders = value;
                ShowTopBorder = value;
                ShowBottomBorder = value;
                ShowLeftBorder = value;
                ShowRightBorder = value;
                if (BorderThickness == 0)
                {
                    _borderThickness = 1;
                }
                _isControlinvalidated = true;
                Invalidate(); // Redraw when all borders are set at once

            }
        }
        private bool _useThemeFont = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("If true, the label's font is always set to the theme font.")]
        public bool UseThemeFont
        {
            get => _useThemeFont;
            set
            {
                _useThemeFont = value;
               // ApplyTheme();
                Invalidate();
            }
        }
        // Border properties
      

        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                _isControlinvalidated = true;
                UpdateControlRegion();
                Invalidate();
            }
        }

        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
                _isControlinvalidated = true;
                Invalidate();
            }
        }

        public new BorderStyle BorderStyle
        {
            get => _borderStyle;
            set
            {
                _borderStyle = value;
                _isControlinvalidated = true;
                Invalidate();
            }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        public bool IsRounded
        {
            get => _isRounded;
            set
            {
                _isRounded = value;
                _isControlinvalidated = true;
                UpdateControlRegion();
                Invalidate();

            }
        }

        // Background properties
        public bool UseGradientBackground
        {
            get => _useGradientBackground;
            set
            {
                _useGradientBackground = value;
                _isControlinvalidated = true;
                Invalidate();
            }
        }

        public LinearGradientMode GradientDirection
        {
            get => _gradientDirection;
            set
            {
                _gradientDirection = value;
                Invalidate();
            }
        }

        public Color GradientStartColor
        {
            get => _gradientStartColor;
            set
            {
                _gradientStartColor = value;
                Invalidate();
            }
        }
     
        // HoverBackColor
        public Color GradientEndColor
        {
            get => _gradientEndColor;
            set
            {
                _gradientEndColor = value;
                Invalidate();
            }
        }

        // Shadow properties
        public bool ShowShadow
        {
            get => _showShadow;
            set
            {
                _showShadow = value;
                if (_showShadow)
                {
                    shadowOffset = _tmpShadowOffset;
                }
                else
                {
                    shadowOffset = 0;
                }
                _isControlinvalidated = true;
                Invalidate();
            }
        }

        public Color ShadowColor
        {
            get => _shadowColor;
            set
            {
                _shadowColor = value;
                Invalidate();
            }
        }

        public float ShadowOpacity
        {
            get => _shadowOpacity;
            set
            {
                _shadowOpacity = value;
                _isControlinvalidated = true;
                Invalidate();
            }
        }

        // State-based colors
        public Color HoverBackColor
        {
            get => _hoverBackColor;
            set
            {
                _hoverBackColor = value;
                Invalidate();
            }
        }
        public Color HoverBorderColor
        {
            get => _hoverBorderColor;
            set
            {
                _hoverBorderColor = value;
                Invalidate();
            }
        }
        public Color HoverForeColor
        {
            get => _hoverForeColor;
            set
            {
                _hoverForeColor = value;
                Invalidate();
            }
        }
        public Color SelectedForeColor
        {
            get => _selectedForeColor;
            set
            {
                _selectedForeColor = value;
                Invalidate();
            }
        }
        public Color SelectedBackColor
        {
            get => _selectedBackColor;
            set
            {
                _selectedBackColor = value;
                Invalidate();
            }
        }
        public Color PressedBackColor
        {
            get => _pressedBackColor;
            set
            {
                _pressedBackColor = value;
                Invalidate();
            }
        }
        public Color PressedForeColor
        {
            get => _pressedBorderColor;
            set
            {
                _pressedBorderColor = value;
                Invalidate();
            }
        }
        public Color PressedBorderColor
        {
            get => _pressedBorderColor;
            set
            {
                _pressedBorderColor = value;
                Invalidate();
            }
        }
        public Color FocusBackColor
        {
            get => _focusBackColor;
            set
            {
                _focusBackColor = value;
                Invalidate();
            }
        }
        public Color FocusForeColor
        {
            get => _focusForeColor;
            set
            {
                _focusForeColor = value;
                Invalidate();
            }
        }
      
        public Color InactiveBorderColor
        {
            get => _selectedBorderColor;
            set
            {
                _selectedBorderColor = value;
                Invalidate();
            }
        }
        
        //public Color InactiveForeColor
        //{
        //    get => _inactiveForeColor;
        //    set
        //    {
        //        _inactiveForeColor = value;
        //        Invalidate();
        //    }
        //}
        public Color DisabledForeColor
        {
            get => _disabledForeColor;
            set
            {
                _disabledForeColor = value;
                Invalidate();
            }
        }

        public Color SelectedBorderColor { get; set; }
        public Color DisabledBorderColor { get;  set; }

        public Color DisabledBackColor
        {
            get => _disabledBackColor;
            set
            {
                _disabledBackColor = value;
                Invalidate();
            }
        }

        // Tooltip property
        public string ToolTipText
        {
            get => _tooltipText;
            set
            {
                _tooltipText = value;
                Invalidate();
            }
        }
        // Theme Properties



        // Track Format and Parse event handlers for reattachment
        protected Dictionary<Binding, EventHandler<ConvertEventArgs>> formatHandlers = new();
        protected Dictionary<Binding, EventHandler<ConvertEventArgs>> parseHandlers = new();
        protected List<Binding> _originalBindings = new List<Binding>();
   
        private bool _isAnimating;



        public Rectangle DrawingRect { get; set; }
        public bool IsCustomeBorder { get; set; }

        #endregion "Public Properties"
        #region "Constructors"
        public BeepControl()
        {
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
            //  base.ProcessTabKey(true);

            InitializeTooltip();
           // ShowAllBorders = true;
            //  BackColor = Color.Transparent;
            Padding = new Padding(0);
            UpdateDrawingRect();
            ComponentName = "BeepControl";


        }
    

        #endregion "Constructors"
        #region "Feature Management"
        protected void BeepControl_LocationChanged(object? sender, EventArgs e)
        {
            if (StaticNotMoving)
            {
                this.Location = originalLocation;
            }
        }
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            // UpdateScrollBars();
        }
        // Method to initialize tooltip with default settings
        #endregion "Feature Management"
        #region "Data Binding"

     

        #endregion
        #region "Theme"
        public virtual void  ApplyTheme(IBeepTheme theme)
        {
            _currentTheme=theme;
            ApplyTheme();
        }
        public virtual void ApplyTheme()
        {
            try
            {
                if (_currentTheme == null) return;
                //ForeColor = _currentTheme.LatestForColor;
                //BackColor = _currentTheme.BackgroundColor;;
                BorderColor = _currentTheme.BorderColor;
     
                ShadowColor = _currentTheme.ShadowColor;
                GradientStartColor = _currentTheme.GradientStartColor;
                GradientEndColor = _currentTheme.GradientEndColor;
                if (IsChild)
                {
                    BackColor = ParentBackColor;
                }else
                {
                    BackColor = _currentTheme.BackColor;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        public virtual void ApplyThemeToControl(Control control)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["Theme"];
            if (themeProperty != null)
            {
                themeProperty.SetValue(control, Theme); // Set the theme on the control
            }
        }
       
        public virtual void ApplyTheme(string theme)
        {
            Theme=theme;
          //  _currentTheme = BeepThemesManager.GetTheme(theme);
            // set theme to contained controls
            //foreach (Control ctrl in Controls)
            //{
            //    if (ctrl is BeepControl)
            //    {
            //        ApplyThemeToControl(ctrl);
            //    }
            //}
        }
        #endregion "Theme"
        #region "React-Style UI Implementation"
        /// <summary>
        /// Applies React-style UI properties to the control's visual appearance
        /// </summary>
        public virtual void ApplyReactUIStyles(Graphics g)
        {
            // Apply shape based on UIShape
            switch (UIShape)
            {
                case ReactUIShape.Square:
                    IsRounded = false;
                    BorderRadius = 0;
                    break;
                case ReactUIShape.Rounded:
                    IsRounded = true;
                    BorderRadius = GetSizeBasedValue(8, 4, 8, 12, 16);
                    break;
                case ReactUIShape.Circular:
                    IsRounded = true;
                    BorderRadius = Math.Min(Width, Height) / 2;
                    break;
                case ReactUIShape.Pill:
                    IsRounded = true;
                    BorderRadius = Height / 2;
                    break;
            }

            // Apply elevation/shadows based on UIElevation
            switch (UIElevation)
            {
                case ReactUIElevation.None:
                    ShowShadow = false;
                    break;
                case ReactUIElevation.Low:
                    ShowShadow = true;
                    ShadowOpacity = 0.2f;
                    ShadowOffset = 2;
                    break;
                case ReactUIElevation.Medium:
                    ShowShadow = true;
                    ShadowOpacity = 0.3f;
                    ShadowOffset = 4;
                    break;
                case ReactUIElevation.High:
                    ShowShadow = true;
                    ShadowOpacity = 0.4f;
                    ShadowOffset = 6;
                    break;
                case ReactUIElevation.Custom:
                    ShowShadow = true;
                    ShadowOpacity = 0.3f;
                    ShadowOffset = UICustomElevation;
                    break;
            }

            // Apply sizes based on UISize
            if (UISize != ReactUISize.Medium)
            {
                // We'll implement padding and font size adjustments based on UISize
                UpdateSizeBasedDimensions();
            }

            // Apply color scheme based on UIColor
            ApplyColorScheme();

            // Apply variant-specific styling
            ApplyVariantStyling(g);

            // Draw ripple effect if active
            DrawRippleEffect(g);
        }

        /// <summary>
        /// Returns a numeric value based on the current UISize
        /// </summary>
        private int GetSizeBasedValue(int xs, int sm, int md, int lg, int xl)
        {
            return UISize switch
            {
                ReactUISize.ExtraSmall => xs,
                ReactUISize.Small => sm,
                ReactUISize.Medium => md,
                ReactUISize.Large => lg,
                ReactUISize.ExtraLarge => xl,
                _ => md
            };
        }

        /// <summary>
        /// Updates the control dimensions based on UISize
        /// </summary>
        private void UpdateSizeBasedDimensions()
        {
            // These would be your default padding values per size
            Padding newPadding = UISize switch
            {
                ReactUISize.ExtraSmall => new Padding(4),
                ReactUISize.Small => new Padding(6),
                ReactUISize.Medium => new Padding(8),
                ReactUISize.Large => new Padding(12),
                ReactUISize.ExtraLarge => new Padding(16),
                _ => new Padding(8)
            };

            // Only update padding if not specifically set by user
            if (Padding == new Padding(0))
            {
                Padding = newPadding;
            }

            // Update font size if using theme font
            if (UseThemeFont)
            {
                // Map UISize to TypeStyleFontSize
                OverrideFontSize = UISize switch
                {
                    ReactUISize.ExtraSmall => TypeStyleFontSize.Small,
                    ReactUISize.Small => TypeStyleFontSize.Medium,
                    ReactUISize.Medium => TypeStyleFontSize.Large,
                    ReactUISize.Large => TypeStyleFontSize.ExtraLarge,
                    ReactUISize.ExtraLarge => TypeStyleFontSize.ExtraExtraLarge,
                    _ => TypeStyleFontSize.Medium
                };
            }

            // Apply UIDensity adjustments
            if (UIDensity == ReactUIDensity.Compact)
            {
                Padding = new Padding(Padding.Left / 2, Padding.Top / 2, Padding.Right / 2, Padding.Bottom / 2);
            }
            else if (UIDensity == ReactUIDensity.Comfortable)
            {
                Padding = new Padding(Padding.Left * 2, Padding.Top * 2, Padding.Right * 2, Padding.Bottom * 2);
            }
        }

        /// <summary>
        /// Applies color scheme based on the current UIColor setting
        /// </summary>
        private void ApplyColorScheme()
        {
            // Define color schemes similar to React UI libraries
            Color primaryColor, secondaryColor, backgroundColor, textColor, borderColor;

            switch (UIColor)
            {
                case ReactUIColor.Primary:
                    primaryColor = _currentTheme?.ButtonBackColor ?? Color.FromArgb(25, 118, 210);
                    secondaryColor = _currentTheme?.ButtonHoverBackColor ?? Color.FromArgb(66, 165, 245);
                    borderColor = _currentTheme?.ButtonBorderColor ?? Color.FromArgb(25, 118, 210);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = _currentTheme?.ButtonForeColor ?? Color.White;
                    break;

                case ReactUIColor.Secondary:
                    primaryColor = Color.FromArgb(156, 39, 176);
                    secondaryColor = Color.FromArgb(186, 104, 200);
                    borderColor = Color.FromArgb(156, 39, 176);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = Color.White;
                    break;

                case ReactUIColor.Success:
                    primaryColor = Color.FromArgb(46, 125, 50);
                    secondaryColor = Color.FromArgb(76, 175, 80);
                    borderColor = Color.FromArgb(46, 125, 50);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = Color.White;
                    break;

                case ReactUIColor.Error:
                    primaryColor = Color.FromArgb(211, 47, 47);
                    secondaryColor = Color.FromArgb(239, 83, 80);
                    borderColor = Color.FromArgb(211, 47, 47);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = Color.White;
                    break;

                case ReactUIColor.Warning:
                    primaryColor = Color.FromArgb(237, 108, 2);
                    secondaryColor = Color.FromArgb(255, 152, 0);
                    borderColor = Color.FromArgb(237, 108, 2);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = Color.White;
                    break;

                case ReactUIColor.Info:
                    primaryColor = Color.FromArgb(2, 136, 209);
                    secondaryColor = Color.FromArgb(3, 169, 244);
                    borderColor = Color.FromArgb(2, 136, 209);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = Color.White;
                    break;

                default: // Default
                    primaryColor = Color.FromArgb(158, 158, 158);
                    secondaryColor = Color.FromArgb(189, 189, 189);
                    borderColor = Color.FromArgb(158, 158, 158);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = Color.Black;
                    break;
            }

            // Apply colors based on the variant
            if (UIVariant == ReactUIVariant.Outlined || UIVariant == ReactUIVariant.Text)
            {
                // For outlined and text variants, we use the color as forecolor and transparent background
                ForeColor = primaryColor;
                BackColor = backgroundColor;
                BorderColor = primaryColor;

                // State colors
                HoverForeColor = secondaryColor;
                HoverBackColor = Color.FromArgb(10, primaryColor);
                HoverBorderColor = secondaryColor;

                PressedForeColor = primaryColor;
                PressedBackColor = Color.FromArgb(20, primaryColor);
                PressedBorderColor = primaryColor;
            }
            else
            {
                // For filled/contained variants
                ForeColor = textColor;
                BackColor = primaryColor;
                BorderColor = primaryColor;

                // State colors
                HoverForeColor = textColor;
                HoverBackColor = secondaryColor;
                HoverBorderColor = secondaryColor;

                PressedForeColor = textColor;
                PressedBackColor = Color.FromArgb(
                    Math.Max(0, primaryColor.R - 20),
                    Math.Max(0, primaryColor.G - 20),
                    Math.Max(0, primaryColor.B - 20));
                PressedBorderColor = PressedBackColor;
            }

            // Disabled state
            DisabledBackColor = Color.FromArgb(230, 230, 230);
            DisabledForeColor = Color.FromArgb(150, 150, 150);
            DisabledBorderColor = Color.FromArgb(200, 200, 200);
        }

        /// <summary>
        /// Applies styling specific to the selected UIVariant
        /// </summary>
        private void ApplyVariantStyling(Graphics g)
        {
            switch (UIVariant)
            {
                case ReactUIVariant.Outlined:
                    ShowAllBorders = true;
                    BorderThickness = 1;
                    break;

                case ReactUIVariant.Text:
                    ShowAllBorders = false;
                    break;

                case ReactUIVariant.Contained:
                case ReactUIVariant.Filled:
                    ShowAllBorders = false;
                    // Additional visual elements could be added here
                    break;

                case ReactUIVariant.Ghost:
                    BackColor = Color.Transparent;
                    ShowAllBorders = false;
                    break;

                default: // Default
                    ShowAllBorders = true;
                    BorderThickness = 1;
                    break;
            }
        }

        /// <summary>
        /// Initiates a ripple effect animation from the specified point
        /// </summary>
        public void StartRippleEffect(Point center)
        {
            if (UIAnimation != ReactUIAnimation.Ripple)
                return;

            _rippleCenter = center;
            _rippleSize = 0;
            _showRipple = true;
            _rippleOpacity = 1.0f;

            if (_rippleTimer == null)
            {
                _rippleTimer = new System.Windows.Forms.Timer();
                _rippleTimer.Interval = 20; // 50fps
                _rippleTimer.Tick += (s, e) => {
                    // Increase ripple size
                    _rippleSize += Width / 20.0f;
                    // Decrease opacity
                    _rippleOpacity -= 0.05f;

                    // Stop when ripple is big enough or fully transparent
                    if (_rippleOpacity <= 0 || _rippleSize > Math.Max(Width, Height) * 2)
                    {
                        _showRipple = false;
                        _rippleTimer.Stop();
                    }

                    Invalidate();
                };
            }

            _rippleTimer.Start();
        }

        /// <summary>
        /// Draws the ripple effect if it's active
        /// </summary>
        private void DrawRippleEffect(Graphics g)
        {
            if (!_showRipple || UIAnimation != ReactUIAnimation.Ripple)
                return;

            // Draw a circular ripple effect
            using (var brush = new SolidBrush(Color.FromArgb(
                (int)(_rippleOpacity * 64), // 25% opacity
                ForeColor)))
            {
                float diameter = _rippleSize * 2;
                g.FillEllipse(
                    brush,
                    _rippleCenter.X - _rippleSize,
                    _rippleCenter.Y - _rippleSize,
                    diameter,
                    diameter);
            }
        }
        #endregion "React-Style UI Implementation"
        #region "Painting"
        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            UpdateDrawingRect();
            Invalidate(); // Trigger a redraw when padding changes
        }
        protected override void OnResize(EventArgs e)
        {
            // Suspend layout to prevent flickering
            this.SuspendLayout();

            base.OnResize(e);
            UpdateDrawingRect();

            // Check if the handle is created before using BeginInvoke
            if (IsHandleCreated)
            {
                // Defer the region update to prevent multiple repaints
                this.BeginInvoke((MethodInvoker)delegate {
                    UpdateControlRegion(); // This now handles the badge too
                });
            }
            else
            {
                // If handle is not created yet, call UpdateControlRegion directly
                UpdateControlRegion(); // This now handles the badge too
            }

            // Resume layout
            this.ResumeLayout();
        }
        // Next, update the UpdateControlRegion method to properly include the badge area
        private void UpdateControlRegion()
        {
            if (Width <= 0 || Height <= 0)
                return;

            // First update the region for the control shape (rounded or square)
            int border = BorderThickness;
            int shadow = ShowShadow ? shadowOffset : 0;

            Rectangle regionRect = new Rectangle(
                0,
                0,
                this.Width,
                this.Height
            );

            // Create the basic region for the control
            Region controlRegion;

            if (IsRounded)
            {
                using (GraphicsPath path = GetRoundedRectPath(regionRect, BorderRadius))
                {
                    controlRegion = new Region(path);
                }
            }
            else
            {
                controlRegion = new Region(regionRect);
            }

            // Now, if we have a badge, expand the region to include it
            if (!string.IsNullOrEmpty(BadgeText))
            {
                const int badgeSize = 22; // Same as in DrawBadgeExternally

                // Badge position: top-right, slightly overlapping
                int badgeX = Width - badgeSize / 2;
                int badgeY = -badgeSize / 2;
                var badgeRect = new Rectangle(badgeX, badgeY, badgeSize, badgeSize);

                using (var badgePath = new GraphicsPath())
                {
                    // Add the appropriate shape for the badge
                    switch (BadgeShape)
                    {
                        case BadgeShape.Circle:
                            badgePath.AddEllipse(badgeRect);
                            break;
                        case BadgeShape.Rectangle:
                            badgePath.AddRectangle(badgeRect);
                            break;
                        case BadgeShape.RoundedRectangle:
                            badgePath.AddPath(GetRoundedRectPath(badgeRect, badgeRect.Height / 4), false);
                            break;
                    }

                    // Union the badge shape with the control region
                    controlRegion.Union(badgePath);
                }
            }

            // Apply the combined region
            this.Region = controlRegion;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
          //  base.OnPaintBackground(e);
        }
        public override Size GetPreferredSize(Size proposedSize)
        {
            UpdateDrawingRect();
            // Adjust size based on border thickness
            int adjustedWidth = DrawingRect.Width;
            int adjustedHeight = DrawingRect.Height;

            return new Size(adjustedWidth, adjustedHeight);
        }
        public virtual void UpdateDrawingRect()
        {
            // Initialize offsets for shadow and border
            int shadowOffsetValue = ShowShadow ? shadowOffset : 0;
            int borderOffsetValue = ShowAllBorders ? BorderThickness : 0;

            // Account for padding and offsets
            int leftPadding = Padding.Left + LeftoffsetForDrawingRect;
            int topPadding = Padding.Top + TopoffsetForDrawingRect;
            int rightPadding = Padding.Right + RightoffsetForDrawingRect;
            int bottomPadding = Padding.Bottom + BottomoffsetForDrawingRect;

            // Calculate the DrawingRect as the inner rectangle avoiding borders, shadows, and padding
            int calculatedWidth = Width - (shadowOffsetValue * 2 + borderOffsetValue * 2 + leftPadding + rightPadding);
            int calculatedHeight = Height - (shadowOffsetValue * 2 + borderOffsetValue * 2 + topPadding + bottomPadding);

            DrawingRect = new Rectangle(
                shadowOffsetValue + borderOffsetValue + leftPadding,
                shadowOffsetValue + borderOffsetValue + topPadding,
                Math.Max(0, calculatedWidth),  // Prevent negative dimensions
                Math.Max(0, calculatedHeight) // Prevent negative dimensions
            );
            UpdateBorderRectangle();
            // Log or debug the dimensions if needed
            //  //Debug.WriteLine($"DrawingRect: {DrawingRect}");
        }
        private void UpdateBorderRectangle()
        {
            // If you have shadows:
            int shadowOffsetValue = ShowShadow ? shadowOffset : 0;

            // If you want a 10px border, halfPen is 5
            // so the stroke sits fully inside the control
            int halfPen = (int)Math.Ceiling(BorderThickness / 2f);

            // Start from the top-left corner + shadow + halfPen
            // Then subtract them again on right/bottom
            borderRectangle = new Rectangle(
                shadowOffsetValue + halfPen,
                shadowOffsetValue + halfPen,
                Width - (shadowOffsetValue + halfPen) * 2,
                Height - (shadowOffsetValue + halfPen) * 2
            );
        }
        public virtual bool SetFont()
        {
            bool retval = true;
            switch (OverrideFontSize)
            {
                case TypeStyleFontSize.None:
                    retval = false;
                    break;
                case TypeStyleFontSize.Small:
                    Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, 8, FontWeight.Normal, FontStyle.Regular);
                    break;
                case TypeStyleFontSize.Medium:
                    Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, 10, FontWeight.Normal, FontStyle.Regular);
                    break;
                case TypeStyleFontSize.Large:
                    Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, 12, FontWeight.Normal, FontStyle.Regular);
                    break;
                case TypeStyleFontSize.ExtraLarge:
                    Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, 14, FontWeight.Normal, FontStyle.Regular);
                    break;
                case TypeStyleFontSize.ExtraExtraLarge:
                    Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, 16, FontWeight.Normal, FontStyle.Regular);
                    break;
                case TypeStyleFontSize.ExtraExtraExtraLarge:
                    Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, 18, FontWeight.Normal, FontStyle.Regular);
                    break;
            }
            return retval;
        }
        /// <summary>
        /// Draws the border according to Material-UI TextField styles
        /// </summary>
        /// <param name="g">Graphics object to draw on</param>
        /// <param name="rect">Rectangle to draw within</param>
        protected virtual void DrawMaterialBorder(Graphics g, Rectangle rect)
        {
            if (DesignMode)
                return;

            // Determine colors based on state
            Color borderColor = this.BorderColor;
            Color backgroundColor = this.BackColor;
            int labelHeight = string.IsNullOrEmpty(LabelText) ? 0 : 16;

            if (!Enabled)
            {
                borderColor = DisabledBorderColor;
            }
            else if (IsFocused)
            {
                borderColor = FocusBorderColor;
            }
            else if (IsHovered)
            {
                borderColor = HoverBorderColor;
            }

            Rectangle borderRect = rect;

            // Draw based on variant
            switch (MaterialBorderVariant)
            {
                case MaterialTextFieldVariant.Standard:
                    // Standard has only a bottom line
                    using (Pen underlinePen = new Pen(borderColor, 1))
                    {
                        // Draw underline at bottom
                        g.DrawLine(underlinePen,
                            borderRect.Left, borderRect.Bottom - 1,
                            borderRect.Right, borderRect.Bottom - 1);

                        // Draw thicker line when focused
                        if (IsFocused)
                        {
                            using (Pen focusPen = new Pen(FocusBorderColor, 2))
                            {
                                g.DrawLine(focusPen,
                                    borderRect.Left, borderRect.Bottom,
                                    borderRect.Right, borderRect.Bottom);
                            }
                        }
                    }
                    break;

                case MaterialTextFieldVariant.Outlined:
                    // Outlined variant has a border all around
                    using (Pen borderPen = new Pen(borderColor, 1))
                    {
                        // If the control is rounded and we want rounded borders
                        if (IsRounded)
                        {
                            using (GraphicsPath path = GetRoundedRectPath(borderRect, BorderRadius))
                            {
                                g.DrawPath(borderPen, path);

                                // If there's a label and we want it to appear as a notch in the border
                                if (FloatingLabel && !string.IsNullOrEmpty(LabelText))
                                {
                                    // Measure the label to create a gap in the outline
                                    Font labelFont = new Font(Font.FontFamily, Font.Size * 0.8f);
                                    Size labelSize = TextRenderer.MeasureText(LabelText, labelFont);

                                    // Create a small rect to "erase" part of the top border for the label
                                    int labelX = borderRect.X + 10; // Position the label with some padding
                                    Rectangle labelGapRect = new Rectangle(
                                        labelX - 2, // Slightly wider than the text
                                        borderRect.Y - labelSize.Height / 2, // Center on the border
                                        labelSize.Width + 4, // Add some padding
                                        labelSize.Height
                                    );

                                    using (SolidBrush backBrush = new SolidBrush(BackColor))
                                    {
                                        g.FillRectangle(backBrush, labelGapRect);
                                    }

                                    // Draw the floating label
                                    using (SolidBrush brushLabel = new SolidBrush(IsFocused ? FocusBorderColor : borderColor))
                                    {
                                        g.DrawString(LabelText, labelFont, brushLabel,
                                            labelX, borderRect.Y - labelSize.Height / 2);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Draw a regular rectangle border
                            g.DrawRectangle(borderPen, borderRect);

                            // Draw the label for non-rounded borders if needed
                            if (FloatingLabel && !string.IsNullOrEmpty(LabelText))
                            {
                                Font labelFont = new Font(Font.FontFamily, Font.Size * 0.8f);
                                Size labelSize = TextRenderer.MeasureText(LabelText, labelFont);

                                int labelX = borderRect.X + 10;
                                Rectangle labelRect = new Rectangle(
                                    labelX - 2, borderRect.Y - labelSize.Height / 2,
                                    labelSize.Width + 4, labelSize.Height);

                                using (SolidBrush backBrush = new SolidBrush(BackColor))
                                {
                                    g.FillRectangle(backBrush, labelRect);
                                }

                                using (SolidBrush brushLabel = new SolidBrush(IsFocused ? FocusBorderColor : borderColor))
                                {
                                    g.DrawString(LabelText, labelFont, brushLabel, labelX, borderRect.Y - labelSize.Height / 2);
                                }
                            }
                        }
                    }
                    break;

                case MaterialTextFieldVariant.Filled:
                    // Filled variant has a colored background and bottom border
                    using (SolidBrush fillBrush = new SolidBrush(FilledBackgroundColor))
                    {
                        // Create a filled background with a top-only rounded rectangle if rounded is on
                        if (IsRounded)
                        {
                            using (GraphicsPath path = new GraphicsPath())
                            {
                                // Create a path with only the top corners rounded
                                int radius = BorderRadius;
                                int diameter = radius * 2;

                                // Top-left corner
                                path.AddArc(borderRect.X, borderRect.Y, diameter, diameter, 180, 90);
                                // Top-right corner
                                path.AddArc(borderRect.Right - diameter, borderRect.Y, diameter, diameter, 270, 90);
                                // Bottom-right corner (no rounding)
                                path.AddLine(borderRect.Right, borderRect.Bottom, borderRect.Left, borderRect.Bottom);
                                // Close the path
                                path.CloseFigure();

                                g.FillPath(fillBrush, path);

                                // Draw bottom border
                                using (Pen underlinePen = new Pen(borderColor, 1))
                                {
                                    g.DrawLine(underlinePen,
                                        borderRect.Left, borderRect.Bottom - 1,
                                        borderRect.Right, borderRect.Bottom - 1);

                                    if (IsFocused)
                                    {
                                        using (Pen focusPen = new Pen(FocusBorderColor, 2))
                                        {
                                            g.DrawLine(focusPen,
                                                borderRect.Left, borderRect.Bottom,
                                                borderRect.Right, borderRect.Bottom);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Fill the entire background for non-rounded style
                            g.FillRectangle(fillBrush, borderRect);

                            // Draw bottom border
                            using (Pen underlinePen = new Pen(borderColor, 1))
                            {
                                g.DrawLine(underlinePen,
                                    borderRect.Left, borderRect.Bottom - 1,
                                    borderRect.Right, borderRect.Bottom - 1);

                                if (IsFocused)
                                {
                                    using (Pen focusPen = new Pen(FocusBorderColor, 2))
                                    {
                                        g.DrawLine(focusPen,
                                            borderRect.Left, borderRect.Bottom,
                                            borderRect.Right, borderRect.Bottom);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }

            // Draw helper text if provided
            if (!string.IsNullOrEmpty(HelperText))
            {
                Font helperFont = new Font(Font.FontFamily, Font.Size * 0.8f);
                Color helperColor = IsValid ? Color.Gray : Color.Red;

                Rectangle helperRect = new Rectangle(
                    rect.X, rect.Bottom + 2,
                    rect.Width, 20);

                TextRenderer.DrawText(g, HelperText, helperFont, helperRect,
                    helperColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }
        /// <summary>
        /// Starts a Material Design style ripple effect from the specified point
        /// </summary>
        protected void StartMaterialRipple(Point clickPosition)
        {
            if (!EnableRippleEffect)
                return;

            _showMaterialRipple = true;
            _rippleOrigin = clickPosition;
            _rippleRadius = 0;
            _rippleOpacity = 0.5f;

            // Create or reset ripple animation timer
            if (_rippleTimer == null)
            {
                _rippleTimer = new Timer();
                _rippleTimer.Interval = 20; // 50fps
                _rippleTimer.Tick += (s, e) =>
                {
                    // Expand ripple and fade it out
                    _rippleRadius += Math.Max(Width, Height) / 10f;
                    _rippleOpacity -= 0.05f;

                    if (_rippleOpacity <= 0)
                    {
                        _rippleTimer.Stop();
                        _showMaterialRipple = false;
                    }

                    Invalidate();
                };
            }

            _rippleTimer.Start();
        }

        /// <summary>
        /// Draws the Material Design ripple effect if active
        /// </summary>
        protected void DrawMaterialRipple(Graphics g)
        {
            if (!_showMaterialRipple || !EnableRippleEffect)
                return;

            using (SolidBrush rippleBrush = new SolidBrush(Color.FromArgb(
                (int)(_rippleOpacity * 64), // 25% opacity
                IsFocused ? FocusBorderColor : ForeColor)))
            {
                float diameter = _rippleRadius * 2;
                g.FillEllipse(
                    rippleBrush,
                    _rippleOrigin.X - _rippleRadius,
                    _rippleOrigin.Y - _rippleRadius,
                    diameter,
                    diameter);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            // Use the current BufferedGraphicsContext to allocate a buffer
            BufferedGraphicsContext context = BufferedGraphicsManager.Current;
            using (BufferedGraphics buffer = context.Allocate(e.Graphics, this.ClientRectangle))
            {
                // Use the buffered Graphics object for all drawing
                Graphics g = buffer.Graphics;

                // Set graphic quality options as before
              //  g.SmoothingMode = SmoothingMode.AntiAlias;
             //   g.TextContrast = 12;
                if (IsChild)
                {
                    BackColor = parentbackcolor;
                }

                // Clear the entire buffer with the control's BackColor
                g.Clear(BackColor);
                // EXTERNAL DRAWING - BEFORE CONTENT LAYER
    
                    PerformExternalDrawing(g, DrawingLayer.BeforeContent);
           

                // Draw the main content
                DrawContent(g);

                // 3) AFTER CONTENT
                PerformExternalDrawing(g, DrawingLayer.AfterContent);
             
                // 5) AFTER ALL
                PerformExternalDrawing(g, DrawingLayer.AfterAll);
              
                // Finally, render the entire off-screen buffer to the screen
                buffer.Render(e.Graphics);
            }
        }
        protected virtual void DrawContent(Graphics g)
        {
            // Update drawing bounds as necessary
            UpdateDrawingRect();

            // Determine shadow offset based on whether shadows should be drawn
            shadowOffset = ShowShadow ? 3 : 0;

            // Use an outer rectangle that covers the whole control
            Rectangle outerRectangle = new Rectangle(0, 0, Width, Height);

            // Apply React UI styles if enabled
            if (UIVariant != ReactUIVariant.Default)
            {
                ApplyReactUIStyles(g);
            }

            // Draw background: either with a gradient or a solid fill based on state
            if (UseGradientBackground)
            {
                using (var brush = new LinearGradientBrush(borderRectangle, GradientStartColor, GradientEndColor, GradientDirection))
                {
                    g.FillRectangle(brush, outerRectangle);
                }
            }
            else
            {
                Color backcolor = IsHovered ? HoveredBackcolor : BackColor;

                // Determine fill backcolor based on state
                if (Enabled)
                {
                    if (IsHovered)
                    {
                        backcolor = HoverBackColor;
                    }
                    else if (IsSelected && IsSelectedOptionOn)
                    {
                        backcolor = SelectedBackColor;
                    }

                    // For Filled variant in Material UI, use the FilledBackgroundColor
                    if (MaterialBorderVariant == MaterialTextFieldVariant.Filled)
                    {
                        backcolor = FilledBackgroundColor;
                    }
                }
                else
                {
                    backcolor = DisabledBackColor;
                }

                using (SolidBrush brush = new SolidBrush(backcolor))
                {
                    g.FillRectangle(brush, DrawingRect);
                }
            }

            //// Update rounded region if needed
            //if (IsRounded)
            //{
            //    UpdateControlRegion();
            //}

            // Draw shadow if applicable
            if (ShowShadow)
            {
                DrawShadowUsingRectangle(g);
            }

            // Draw borders - with priority logic to prevent conflicts
            if (!_isframless)
            {
                // Choose the appropriate border drawing method:
                // 1. If Material UI is active, use DrawMaterialBorder
                // 2. Otherwise use regular border drawing logic

                if (MaterialBorderVariant != MaterialTextFieldVariant.Standard)
                {
                    // Use Material UI border styles
                    DrawMaterialBorder(g, borderRectangle);
                }
                else if (IsCustomeBorder)
                {
                    // Use custom border
                    DrawCustomBorder(g);
                }
                else if (ShowAllBorders && BorderThickness > 0)
                {
                    // Use standard borders
                    if (IsRounded)
                    {
                        using (GraphicsPath path = GetRoundedRectPath(borderRectangle, BorderRadius))
                        {
                            using (Pen borderPen = new Pen(BorderColor, BorderThickness))
                            {
                                borderPen.Alignment = PenAlignment.Inset;
                                g.DrawPath(borderPen, path);
                            }
                        }
                    }
                    else
                    {
                        DrawBorder(g, borderRectangle);
                    }
                }
            }

            // Draw focus indicator if needed
            if (ShowFocusIndicator && Focused)
            {
                DrawFocusIndicator(g);
            }

            // Draw hit area components
            if (HitList != null)
            {
                foreach (var hitTest in HitList)
                {
                    if (hitTest.IsVisible && hitTest.uIComponent != null)
                    {
                        hitTest.uIComponent.Draw(g, hitTest.TargetRect);
                    }
                }
            }

            // Draw Material ripple effect if active
            DrawMaterialRipple(g);
        }
        protected Font GetScaledFont(Graphics g, string text, Size maxSize, Font originalFont)
        {
            if(originalFont==null)
            {
                originalFont= Font;
            }

            // Quickly check if the original font already fits.
            if (Fits(g, text, originalFont, maxSize))
                return originalFont;

            float minSize = 6.0f;
            float lower = minSize;
            float upper = originalFont.Size;
            float finalSize = lower; // fallback

            while ((upper - lower) > 0.5f) // 0.5f = desired precision
            {
                float mid = (lower + upper) / 2f;
                using (var testFont = new Font(originalFont.FontFamily, mid, originalFont.Style))
                {
                    if (Fits(g, text, testFont, maxSize))
                    {
                        finalSize = mid;   // It fits, so remember this as a candidate
                        upper = mid;       // Keep searching for a possibly smaller upper
                    }
                    else
                    {
                        lower = mid;       // It's too big, so move the lower bound
                    }
                }
            }

            // Return a final font at the best size found
            return new Font(originalFont.FontFamily, finalSize, originalFont.Style);
        }
        private bool Fits(Graphics g, string text, Font font, Size maxSize)
        {
            var measured = TextRenderer.MeasureText(g, text, font);
            return (measured.Width <= maxSize.Width && measured.Height <= maxSize.Height);
        }
        #region "Drawing Methods"
        public virtual void DrawCustomBorder(Graphics g)
        {
            // Draw custom border based on the control's properties
            DrawBorder(g, DrawingRect);
        }
        protected void DrawBorder(Graphics graphics, Rectangle drawingRect)
        {
            int brder = BorderThickness;
            Color color = BorderColor;
            if (IsHovered)
            {
                color = _currentTheme.HoverLinkColor;
                // brder = _borderThickness + 1;
            }

            using (var pen = new Pen(color, brder))
            {
                pen.DashStyle = _borderDashStyle;
                // Set the border style
                // set pen to draw based on border style
                switch (BorderDashStyle)
                {
                    case DashStyle.Dash:
                        pen.DashStyle = DashStyle.Dash;
                        break;
                    case DashStyle.Dot:
                        pen.DashStyle = DashStyle.Dot;
                        break;
                    case DashStyle.Solid:
                        pen.DashStyle = DashStyle.Solid;
                        break;
                    case DashStyle.DashDotDot:
                        return; // No border to draw
                }
                pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                // Draw rounded or regular borders
                if (IsRounded && ShowAllBorders)
                {

                    using (GraphicsPath path = GetRoundedRectPath(drawingRect, BorderRadius))
                    {
                        graphics.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Draw individual borders based on settings
                    if (ShowTopBorder)
                        graphics.DrawLine(pen, drawingRect.Left, drawingRect.Top, drawingRect.Right, drawingRect.Top);
                    if (ShowBottomBorder)
                        graphics.DrawLine(pen, drawingRect.Left, drawingRect.Bottom, drawingRect.Right, drawingRect.Bottom);
                    if (ShowLeftBorder)
                        graphics.DrawLine(pen, drawingRect.Left, drawingRect.Top, drawingRect.Left, drawingRect.Bottom);
                    if (ShowRightBorder)
                        graphics.DrawLine(pen, drawingRect.Right, drawingRect.Top, drawingRect.Right, drawingRect.Bottom);
                }
            }
        }
        protected void DrawShadow(Graphics graphics, Rectangle drawingRect)
        {
            if (ShadowOpacity <= 0 || ShadowOpacity > 1) return;

            int shadowThickness = 5;  // Thickness of the shadow effect
            int shadowSpacing = 2;    // Spacing between shadow lines
            Color shadowColor = Color.FromArgb((int)(255 * ShadowOpacity), ShadowColor);

            // Draw multiple outlines along the control's edge for a soft shadow effect
            for (int i = 1; i <= shadowThickness; i++)
            {
                using (Pen shadowPen = new Pen(Color.FromArgb((int)(255 * ShadowOpacity / i), shadowColor), 1))
                {
                    // Adjust the drawing rect to slightly expand outward for each shadow line
                    Rectangle outerRect = new Rectangle(
                        drawingRect.X - (i * shadowSpacing),
                        drawingRect.Y - (i * shadowSpacing),
                        drawingRect.Width + (i * shadowSpacing * 2),
                        drawingRect.Height + (i * shadowSpacing * 2));

                    // Draw the shadow as an outline around the control's border, based on shape
                    if (IsRounded)
                    {
                        using (GraphicsPath shadowPath = GetRoundedRectPath(outerRect, BorderRadius + (i * shadowSpacing)))
                        {
                            graphics.DrawPath(shadowPen, shadowPath);
                        }
                    }
                    else
                    {
                        using (GraphicsPath shadowPath = GetControlShapePath(outerRect))
                        {
                            graphics.DrawPath(shadowPen, shadowPath);
                        }
                    }
                }
            }
        }
        // Helper method for custom control shape outline; adjust shape as needed (e.g., ellipse for rounded)
        protected GraphicsPath GetControlShapePath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            // Example shape, add an ellipse or any other shape if needed:
            path.AddEllipse(rect);
            return path;
        }
        protected void DrawShadowUsingRectangle(Graphics graphics)
        {
            // Ensure shadow is drawn only if it's enabled
            if (ShowShadow)
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb((int)(255 * ShadowOpacity), ShadowColor)))
                {
                    // Calculate an offset shadow rectangle larger than the DrawingRect and extending outside the border
                    Rectangle shadowRect = new Rectangle(
                        DrawingRect.Left - shadowOffset - BorderThickness,
                        DrawingRect.Top - shadowOffset - BorderThickness,
                        DrawingRect.Width + (2 * shadowOffset) + (2 * BorderThickness),
                        DrawingRect.Height + (2 * shadowOffset) + (2 * BorderThickness)
                    );

                    if (IsRounded)
                    {
                        // If the control is rounded, draw a rounded shadow
                        using (GraphicsPath shadowPath = GetRoundedRectPath(shadowRect, BorderRadius + shadowOffset))
                        {
                            graphics.FillPath(shadowBrush, shadowPath);
                        }
                    }
                    else
                    {
                        // Draw a rectangular shadow for non-rounded controls
                        graphics.FillRectangle(shadowBrush, shadowRect);
                    }
                }
            }
        }
        protected void DrawShadow(Graphics graphics)
        {
            // Ensure shadow is drawn only if it's enabled and the control is not transparent
            if (ShowShadow) // Ensure no transparency conflicts
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb((int)(255 * ShadowOpacity), ShadowColor)))
                {
                    // Calculate an offset shadow rectangle slightly larger than the main control
                    Rectangle shadowRect = new Rectangle(
                        Left + BorderThickness + shadowOffset,
                        Top + BorderThickness + shadowOffset,
                        Width + BorderThickness + shadowOffset,
                        Height + BorderThickness + shadowOffset
                    );

                    if (IsRounded)
                    {
                        using (GraphicsPath shadowPath = GetRoundedRectPath(shadowRect, BorderRadius))
                        {
                            graphics.FillPath(shadowBrush, shadowPath);
                        }
                    }
                    else
                    {
                        graphics.FillRectangle(shadowBrush, shadowRect);
                    }
                }
            }
        }
        protected virtual void DrawFocusIndicator(Graphics graphics)
        {
            Rectangle glowRect = new Rectangle(-3, -3, Width + 6, Height + 6);
            using (GraphicsPath path = GetRoundedRectPath(glowRect, BorderRadius + 2))
            using (var glowBrush = new SolidBrush(Color.FromArgb(128, FocusIndicatorColor)))
            {
                graphics.FillPath(glowBrush, path);
            }
            // or Color Overlay
            //using (var overlayBrush = new SolidBrush(Color.FromArgb(50, FocusIndicatorColor)))
            //{
            //    graphics.FillRectangle(overlayBrush, ClientRectangle);
            //}
        }

        protected GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            // Ensure the radius is valid relative to the rectangle's dimensions
            int diameter = Math.Min(Math.Min(radius * 2, rect.Width), rect.Height);

            if (diameter > 0)
            {
                // Add arcs and lines for rounded rectangle
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
            }
            else
            {
                // Fallback to a regular rectangle if diameter is zero
                path.AddRectangle(rect);
            }

            return path;
         //   return GetEllipticalRoundedRectPath(rect, radius, radius);
        }
        /// <summary>
        /// Creates a GraphicsPath for a rectangle with elliptical corners
        /// allowing different horizontal and vertical radii. This can be used
        /// for more “modern” or “material” style corners.
        /// </summary>
        /// <param name="rect">The overall bounding rectangle.</param>
        /// <param name="radiusX">Horizontal radius for corners.</param>
        /// <param name="radiusY">Vertical radius for corners.</param>
        /// <returns>A GraphicsPath defining the elliptical-corner rectangle.</returns>
        protected GraphicsPath GetEllipticalRoundedRectPath(Rectangle rect, int radiusX, int radiusY)
        {
            GraphicsPath path = new GraphicsPath();

            // Cap the radii so we don't exceed rect dimensions
            int diameterX = Math.Min(radiusX * 2, rect.Width);
            int diameterY = Math.Min(radiusY * 2, rect.Height);

            // If either diameter is 0, fallback to a plain rectangle
            if (diameterX <= 0 || diameterY <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // top-left corner
            path.AddArc(rect.X, rect.Y, diameterX, diameterY, 180, 90);

            // top-right corner
            path.AddArc(rect.Right - diameterX, rect.Y, diameterX, diameterY, 270, 90);

            // bottom-right corner
            path.AddArc(rect.Right - diameterX, rect.Bottom - diameterY, diameterX, diameterY, 0, 90);

            // bottom-left corner
            path.AddArc(rect.X, rect.Bottom - diameterY, diameterX, diameterY, 90, 90);

            path.CloseFigure();
            return path;
        }
        #endregion "Drawing Methods"
        #endregion "Painting"
        #region "Mouse events"
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (DesignMode)
                return;
            IsHovered = true;

            Point location = PointToClient(Cursor.Position);
            if (HitTest(location) && HitTestControl != null && HitTestControl.uIComponent != null)
            {
                HitTestControl.IsHovered = true;
                SendMouseEvent(HitTestControl.uIComponent, MouseEventType.MouseEnter, PointToScreen(location));
                if (HitTestControl.HitAction != null)
                    HitTestControl.HitAction.Invoke();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (DesignMode)
                return;
            IsHovered = true;

            if (HitTest(e.Location))
            {
                if (HitTestControl != null && HitTestControl.uIComponent != null)
                {
                    HitTestControl.IsHovered = true;
                    SendMouseEvent(HitTestControl.uIComponent, MouseEventType.MouseMove, PointToScreen(e.Location));
                    // Don't invoke HitAction on hover - actions should only trigger on clicks
                    // if (HitTestControl.HitAction != null)
                    //     HitTestControl.HitAction.Invoke();
                }
            }
            else
            {
                foreach (var hitTest in HitList)
                {
                    hitTest.IsHovered = false;
                }
            }
        }


        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (DesignMode)
                return;
            IsHovered = false;
            HitAreaEventOn = false;

            if (HitTestControl != null && HitTestControl.uIComponent != null)
            {
                SendMouseEvent(HitTestControl.uIComponent, MouseEventType.MouseLeave, PointToScreen(Point.Empty));
            }
            HitTestControl = null;

            if (HitList != null)
            {
                foreach (var hitTest in HitList)
                {
                    hitTest.IsHit = false;
                    hitTest.IsHovered = false;
                    hitTest.IsPressed = false;
                }
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (DesignMode)
                return;
            IsFocused = true;

            if (HitTestWithMouse() && HitTestControl != null && HitTestControl.uIComponent != null)
            {
                HitTestControl.IsFocused = true;
                // No SendMouseEvent here; focus isn't a mouse event
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (DesignMode)
                return;
            IsFocused = false;

            IsHovered = false;
            if (HitList != null)
            {
                foreach (var hitTest in HitList)
                {
                    hitTest.IsFocused = false;
                    hitTest.IsHovered = hitTest.IsHit;
                }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            // Early exit if component is being disposed or is already disposed
            if (IsDisposed)
                return;
            if(DesignMode)
                return;
            try
            {
                Point location = PointToClient(Cursor.Position);

                // Make sure HitTest doesn't throw and the results are still valid
                if (!HitTest(location))
                    return;

                // Verify all required objects are still valid
                if (HitTestControl == null || HitTestControl.uIComponent == null)
                    return;

                // Check if the component is a Control that's been disposed
                if (HitTestControl.uIComponent is Control control && control.IsDisposed)
                    return;

                // Now we can safely send the mouse event
                SendMouseEvent(HitTestControl.uIComponent, MouseEventType.Click, PointToScreen(location));

                // Safely invoke the action if it exists
                HitTestControl.HitAction?.Invoke();
            }
            catch (ObjectDisposedException)
            {
                // Object was disposed between our check and usage - just silently handle it
                HitTestControl = null;
            }
            catch (Exception ex)
            {
                // Optional: Log the exception if needed
                // You might want to add your logging code here
               // System.Diagnostics.Debug.WriteLine($"Error in BeepControl.OnClick: {ex.Message}");
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (EnableRippleEffect && e.Button == MouseButtons.Left)
            {
                StartMaterialRipple(e.Location);
            }
            base.OnMouseDown(e);
            if (DesignMode)
                return;
            if (e.Button == MouseButtons.Left)
            {
                if (HitTest(e.Location) && HitTestControl != null && HitTestControl.uIComponent != null)
                {
                    HitTestControl.IsPressed = true;
                    SendMouseEvent(HitTestControl.uIComponent, MouseEventType.MouseDown, PointToScreen(e.Location));
                    if (HitTestControl.HitAction != null)
                        HitTestControl.HitAction.Invoke();
                }
                IsPressed = true;
                IsSelected = !_isSelected;
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (DesignMode)
                return;
            IsPressed = false;

            if (HitTest(e.Location) && HitTestControl != null && HitTestControl.uIComponent != null)
            {
                SendMouseEvent(HitTestControl.uIComponent, MouseEventType.MouseUp, PointToScreen(e.Location));
                if (HitTestControl.HitAction != null)
                    HitTestControl.HitAction.Invoke();
            }

            if (HitList != null)
            {
                foreach (var hitTest in HitList)
                {
                    hitTest.IsPressed = false;
                }
            }

            if (HitTestControl != null)
            {
                HitTestControl.IsHovered = true;
            }
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            if (DesignMode)
                return;
            IsHovered = true;

            Point location = PointToClient(Cursor.Position);
            if (HitTest(location) && HitTestControl != null && HitTestControl.uIComponent != null)
            {
                HitTestControl.IsHovered = true;
                SendMouseEvent(HitTestControl.uIComponent, MouseEventType.MouseHover, PointToScreen(location));
                if (HitTestControl.HitAction != null)
                    HitTestControl.HitAction.Invoke();
            }
        }
        #endregion "Mouse events"
        #region "Key events"
        public event EventHandler TabKeyPressed;
        public event EventHandler ShiftTabKeyPressed;
        public event EventHandler EnterKeyPressed;
        public event EventHandler EscapeKeyPressed;

        public event EventHandler LeftArrowKeyPressed;
        public event EventHandler RightArrowKeyPressed;
        public event EventHandler UpArrowKeyPressed;
        public event EventHandler DownArrowKeyPressed;
        public event EventHandler PageUpKeyPressed;
        public event EventHandler PageDownKeyPressed;
        public event EventHandler HomeKeyPressed;
        public event EventHandler EndKeyPressed;

        /// <summary>
        /// This event is raised *first* for any “dialog” key so you get 
        /// a chance to see which key was pressed *before* specialized events.
        /// </summary>
        public event EventHandler<KeyEventArgs> DialogKeyDetected;

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Example: extract only the base key, ignoring Ctrl, Alt, etc. 
            // (But you can do more advanced checks if you want.)
            var keyCode = (keyData & Keys.KeyCode);

            // Raise a “catch-all” event for anything that goes through ProcessDialogKey.
            DialogKeyDetected?.Invoke(this, new KeyEventArgs(keyData));

            // Check for SHIFT pressed
            bool shiftPressed = (keyData & Keys.Shift) == Keys.Shift;

            switch (keyCode)
            {
                case Keys.Tab:
                    // You can separately raise events for Tab vs. Shift+Tab
                    if (shiftPressed)
                    {
                        ShiftTabKeyPressed?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        TabKeyPressed?.Invoke(this, EventArgs.Empty);
                    }

                    // Return `true` if you want to STOP normal focus behavior
                    // Return `false` if you want normal Tab navigation to continue
                    return false;

                case Keys.Enter:
                    EnterKeyPressed?.Invoke(this, EventArgs.Empty);
                    // Return `true` if you have fully handled Enter 
                    // and do NOT want the default “AcceptButton” behavior:
                    return true;

                case Keys.Escape:
                    EscapeKeyPressed?.Invoke(this, EventArgs.Empty);
                    // Return `true` if you have fully handled Escape 
                    // and do NOT want default “CancelButton” behavior:
                    return true;

                case Keys.Left:
                    LeftArrowKeyPressed?.Invoke(this, EventArgs.Empty);
                    return false; // Let normal arrow navigation proceed if desired

                case Keys.Right:
                    RightArrowKeyPressed?.Invoke(this, EventArgs.Empty);
                    return false;

                case Keys.Up:
                    UpArrowKeyPressed?.Invoke(this, EventArgs.Empty);
                    return false;

                case Keys.Down:
                    DownArrowKeyPressed?.Invoke(this, EventArgs.Empty);
                    return false;

                case Keys.PageUp:
                    PageUpKeyPressed?.Invoke(this, EventArgs.Empty);
                    return false;

                case Keys.PageDown:
                    PageDownKeyPressed?.Invoke(this, EventArgs.Empty);
                    return false;

                case Keys.Home:
                    HomeKeyPressed?.Invoke(this, EventArgs.Empty);
                    return false;

                case Keys.End:
                    EndKeyPressed?.Invoke(this, EventArgs.Empty);
                    return false;

                default:
                    // If it's a key you don't care about, let the base class handle it.
                    return base.ProcessDialogKey(keyData);
            }
        }

        #endregion "Key events"
        #region "Animation"
        // Default Animation Properties

        /// <summary>
        /// Shows the control with the specified animation.
        /// </summary>
        /// 

        public void ShowWithAnimation(DisplayAnimationType animationType, Control parentControl = null)
        {
            AnimationType = animationType;
            if (AnimationType == DisplayAnimationType.None)
            {
                Visible = true;
                return;
            }
            if (Visible == false)
            {
                Visible = true;
            }
            InitializeAnimation(parentControl);
        }
        /// <summary>
        /// Initialize animation properties and start animation.
        /// </summary>
        private void InitializeAnimation(Control parentControl)
        {
            _animationTimer?.Stop();

            // Ensure menu is visible
            Visible = true;

            // Reset animation state
            _opacity = 0f;
            _animationElapsedTime = 0;

            // Define initial and final positions
            _slideStartRect = GetSlideStartRect(parentControl);
            _slideEndRect = Bounds;

            // Start animation timer
            _animationTimer = new Timer { Interval = 15 }; // ~60 FPS
            _animationTimer.Tick += OnAnimationTick;
            _isAnimating = true;

            // Set initial state for fade or slide
            if (AnimationType == DisplayAnimationType.Fade || AnimationType == DisplayAnimationType.SlideAndFade)
            {
                BackColor = Color.FromArgb((int)(_opacity * 255), BackColor);
            }

            if (AnimationType == DisplayAnimationType.Slide || AnimationType == DisplayAnimationType.SlideAndFade)
            {
                Bounds = _slideStartRect;
            }

            // Start the timer
            _animationTimer.Start();
        }
        /// <summary>
        /// Handles each animation frame.
        /// </summary>
        private void OnAnimationTick(object sender, EventArgs e)
        {
            _animationElapsedTime += _animationTimer.Interval;
            float progress = Math.Min(1.0f, (float)_animationElapsedTime / AnimationDuration);
            progress = ApplyEasing(progress); // Smooth the animation with easing

            // Fade effect
            if (AnimationType == DisplayAnimationType.Fade || AnimationType == DisplayAnimationType.SlideAndFade)
            {
                _opacity = progress;
                BackColor = Color.FromArgb((int)(_opacity * 255), BackColor);
            }

            // Slide effect
            if (AnimationType == DisplayAnimationType.Slide || AnimationType == DisplayAnimationType.SlideAndFade)
            {
                int x = (int)(_slideStartRect.X + (_slideEndRect.X - _slideStartRect.X) * progress);
                int y = (int)(_slideStartRect.Y + (_slideEndRect.Y - _slideStartRect.Y) * progress);
                int width = (int)(_slideStartRect.Width + (_slideEndRect.Width - _slideStartRect.Width) * progress);
                int height = (int)(_slideStartRect.Height + (_slideEndRect.Height - _slideStartRect.Height) * progress);
                Bounds = new Rectangle(x, y, width, height);
            }

            // Stop animation when done
            if (progress >= 1.0f)
            {
                _animationTimer.Stop();
                _isAnimating = false;
            }
        }

        /// <summary>
        /// Calculates the start rectangle for slide animation.
        /// </summary>
        private Rectangle GetSlideStartRect(Control parentControl)
        {
            Rectangle startRect = Bounds;

            if (parentControl != null)
            {
                Point parentLocation = parentControl.PointToScreen(Point.Empty);

                switch (SlideFrom)
                {
                    case SlideDirection.Bottom:
                        startRect = new Rectangle(parentLocation.X, parentLocation.Y + parentControl.Height, Width, Height);
                        break;
                    case SlideDirection.Top:
                        startRect = new Rectangle(parentLocation.X, parentLocation.Y - Height, Width, Height);
                        break;
                    case SlideDirection.Left:
                        startRect = new Rectangle(parentLocation.X - Width, parentLocation.Y, Width, Height);
                        break;
                    case SlideDirection.Right:
                        startRect = new Rectangle(parentLocation.X + parentControl.Width, parentLocation.Y, Width, Height);
                        break;
                }
            }

            return startRect;
        }
        public void ShowWithDropdownAnimation(Control parentControl = null)
        {
            if (AnimationType == DisplayAnimationType.None)
            {
                this.Visible = true;
                return;
            }

            _animationTimer = new System.Windows.Forms.Timer { Interval = 15 };
            int elapsedTime = 0;
            float startOpacity = 0f;
            float endOpacity = 1f;

            // Determine the starting and ending positions based on SlideDirection
            int startX = this.Location.X, startY = this.Location.Y;
            int finalX = this.Location.X, finalY = this.Location.Y;

            if (parentControl != null)
            {
                switch (SlideFrom)
                {
                    case SlideDirection.Bottom:
                        // Start aligned to the bottom of parent and slide downward
                        startX = parentControl.Left;
                        startY = parentControl.Bottom; // Start at bottom of parent
                        finalY = startY + this.Height; // Slide down
                        break;

                    case SlideDirection.Top:
                        // Start aligned to the top of parent and slide upward
                        startX = parentControl.Left;
                        startY = parentControl.Top - this.Height; // Start at top and slide upwards
                        finalY = parentControl.Top - this.Height;
                        break;

                    case SlideDirection.Left:
                        // Start aligned to the left of the parent and slide leftward
                        startX = parentControl.Left - this.Width;
                        finalX = parentControl.Left - this.Width;
                        startY = parentControl.Top; // Align vertically with parent
                        finalY = startY;
                        break;

                    case SlideDirection.Right:
                        // Start aligned to the right of the parent and slide rightward
                        startX = parentControl.Right; // Start at the right edge of parent
                        finalX = startX + this.Width; // Slide right
                        startY = parentControl.Top; // Align vertically with parent
                        finalY = startY;
                        break;
                }
            }

            // Set the initial position and make the control visible before animation
            this.Location = new Point(startX, startY);
            this.Visible = true;

            _animationTimer.Tick += (sender, args) =>
            {
                elapsedTime += _animationTimer.Interval;
                float progress = Math.Min(1.0f, (float)elapsedTime / AnimationDuration);
                progress = ApplyEasing(progress);

                // Handle fade and slide animations
                if (AnimationType == DisplayAnimationType.Fade || AnimationType == DisplayAnimationType.SlideAndFade)
                {
                    _opacity = startOpacity + (endOpacity - startOpacity) * progress;
                    this.BackColor = Color.FromArgb((int)(_opacity * 255), _originalBackColor);
                }

                if (AnimationType == DisplayAnimationType.Slide || AnimationType == DisplayAnimationType.SlideAndFade)
                {
                    // Interpolate position for the slide animation
                    this.Location = new Point(
                        (int)(startX + (finalX - startX) * progress),
                        (int)(startY + (finalY - startY) * progress)
                    );
                }

                // Stop animation once fully displayed
                if (progress >= 1.0f)
                {
                    _animationTimer.Stop();
                    _animationTimer.Dispose();
                }
            };

            _animationTimer.Start();
        }


        /// <summary>
        /// Applies easing to the animation progress.
        /// </summary>
        private float ApplyEasing(float progress)
        {
            return Easing switch
            {
                EasingType.Linear => progress,
                EasingType.EaseIn => progress * progress,
                EasingType.EaseOut => 1 - (1 - progress) * (1 - progress),
                EasingType.EaseInOut => progress < 0.5f ? 2 * progress * progress : 1 - 2 * (1 - progress) * (1 - progress),
                _ => progress
            };
        }

        /// <summary>
        /// Stops the animation immediately.
        /// </summary>
        public void StopAnimation()
        {
            _animationTimer?.Stop();
            _isAnimating = false;
        }

        #endregion "Animation"
        #region "Util"
        public override string ToString()
        {
            return GetType().Name.Replace("Control", "").Replace("Beep", "Beep ");
        }
        public Size GetSize()

        {
            return new Size(Width, Height);

        }
        public virtual void Print(Graphics graphics)
        {
            // Draw the control on the provided graphics object
            OnPrint(new PaintEventArgs(graphics, ClientRectangle));
        }
        public float GetScaleFactor(SizeF imageSize, Size targetSize)
        {
            float scaleX = targetSize.Width / imageSize.Width;
            float scaleY = targetSize.Height / imageSize.Height;

            return _scaleMode switch
            {
                ImageScaleMode.Stretch => 1.0f,
                ImageScaleMode.KeepAspectRatioByWidth => scaleX,
                ImageScaleMode.KeepAspectRatioByHeight => scaleY,
                _ => Math.Min(scaleX, scaleY) // Default to KeepAspectRatio
            };
        }
        public RectangleF GetScaledBounds(SizeF imageSize, Rectangle targetRect)
        {
            float scale = GetScaleFactor(imageSize, targetRect.Size);

            if (scale <= 0)
                return RectangleF.Empty;

            float newWidth = imageSize.Width * scale;
            float newHeight = imageSize.Height * scale;

            float xOffset = targetRect.X + (targetRect.Width - newWidth) / 2;  // Center the image horizontally
            float yOffset = targetRect.Y + (targetRect.Height - newHeight) / 2; // Center the image vertically

            return new RectangleF(xOffset, yOffset, newWidth, newHeight);
        }
        public float GetScaleFactor(SizeF imageSize)
        {
            float availableWidth = ClientSize.Width;
            float availableHeight = ClientSize.Height;

            switch (_scaleMode)
            {
                case ImageScaleMode.Stretch:
                    return 1.0f; // No scaling, just stretch
                case ImageScaleMode.KeepAspectRatioByWidth:
                    return availableWidth / imageSize.Width;
                case ImageScaleMode.KeepAspectRatioByHeight:
                    return availableHeight / imageSize.Height;
                case ImageScaleMode.KeepAspectRatio:
                default:
                    float scaleX = availableWidth / imageSize.Width;
                    float scaleY = availableHeight / imageSize.Height;
                    return Math.Min(scaleX, scaleY); // Maintain aspect ratio
            }
        }
        public RectangleF GetScaledBounds(SizeF imageSize)
        {
            float controlWidth = ClientSize.Width;
            float controlHeight = ClientSize.Height;

            float scaleX = controlWidth / imageSize.Width;
            float scaleY = controlHeight / imageSize.Height;
            float scale = Math.Min(scaleX, scaleY);

            float newWidth = imageSize.Width * scale;
            float newHeight = imageSize.Height * scale;

            float xOffset = (controlWidth - newWidth) / 2;
            float yOffset = (controlHeight - newHeight) / 2;

            return new RectangleF(xOffset, yOffset, newWidth, newHeight);
        }
        public Size GetSuitableSizeForTextandImage(Size imageSize, Size MaxImageSize, TextImageRelation TextImageRelation)
        {
            // Measure the text size based on the current font
            Size textSize = TextRenderer.MeasureText(Text, Font);

            // Get the original image size and limit it by the MaxImageSize property if necessary
            //Value imageSize = beepImage != null && beepImage.HasImage ? beepImage.GetImageSize() : Value.Empty;

            // Restrict the image size using the MaxImageSize property
            if (imageSize.Width > MaxImageSize.Width || imageSize.Height > MaxImageSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)MaxImageSize.Width / imageSize.Width,
                    (float)MaxImageSize.Height / imageSize.Height);

                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            // Calculate the required width and height based on TextImageRelation
            int width = 0;
            int height = 0;

            switch (TextImageRelation)
            {
                case TextImageRelation.ImageBeforeText:
                case TextImageRelation.TextBeforeImage:
                    width = imageSize.Width + textSize.Width + Padding.Left + Padding.Right;
                    height = Math.Max(imageSize.Height, textSize.Height) + Padding.Top + Padding.Bottom;
                    break;

                case TextImageRelation.ImageAboveText:
                case TextImageRelation.TextAboveImage:
                    width = Math.Max(imageSize.Width, textSize.Width) + Padding.Left + Padding.Right;
                    height = imageSize.Height + textSize.Height + Padding.Top + Padding.Bottom;
                    break;

                case TextImageRelation.Overlay:
                    width = Math.Max(imageSize.Width, textSize.Width) + Padding.Left + Padding.Right;
                    height = Math.Max(imageSize.Height, textSize.Height) + Padding.Top + Padding.Bottom;
                    break;
            }

            // Return the calculated size
            return new Size(width, height);
        }
        protected override void OnParentChanged(EventArgs e)
        {
            // Remember the previous parent
            var oldParent = Tag as Control;

            // Update Tag to the new parent
            Tag = Parent;

            // If the old parent was a BeepControl, clear just this child’s drawing handler
            if (oldParent is BeepControl oldBeepParent)
            {
                oldBeepParent.ClearChildExternalDrawing(this);
            }
            // register with new parent
          
            // If we’re completely removed (no new Parent), also sweep any remaining BeepControl on the Form
            if (Parent == null)
            {
                var form = FindForm();
                if (form != null)
                {
                    foreach (Control c in form.Controls)
                    {
                        if (c is BeepControl beepParent)
                        {
                            beepParent.ClearChildExternalDrawing(this);
                        }
                    }
                }
            }

            base.OnParentChanged(e);
        }


        #endregion "Util"
        #region "IBeepUIComoponent"
        #region "ToolTip"
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
        // Apply theme properties to the control and children
        public void ShowToolTip(string text)
        {
            ToolTipText = text;
            if (!string.IsNullOrEmpty(ToolTipText))
            {
                _toolTip.Show(ToolTipText, this, PointToClient(MousePosition), 3000); // Config tooltip for 3 seconds
            }

        }

        public void HideToolTip()
        {
            _toolTip.Hide(this);

        }
        protected void ShowToolTipIfExists()
        {
            if (!string.IsNullOrEmpty(ToolTipText))
            {
                ShowToolTip(ToolTipText);
            }
        }
        #endregion "ToolTip"
        public event EventHandler<BeepComponentEventArgs> PropertyChanged; // Event to notify that a property has changed
        public List<object> Items { get { return _items; } set { _items = value; } }

        //protected EnumBeepThemes _themeEnum = EnumBeepThemes.DefaultTheme;
        protected IBeepTheme _currentTheme = BeepThemesManager.GetDefaultTheme();
        public event EventHandler<BeepComponentEventArgs> OnSelected;
        public event EventHandler<BeepComponentEventArgs> OnValidate;
        public event EventHandler<BeepComponentEventArgs> OnValueChanged;
        public event EventHandler<BeepComponentEventArgs> OnLinkedValueChanged;
        private object _selectedValue;
        public object SelectedValue
        {
            get => _selectedValue;
            set
            {
                _selectedValue = value;
                if (value != null)
                {
                    OnSelected?.Invoke(this, new BeepComponentEventArgs(this, BoundProperty, _linkedproperty, value));
                }
            }
        }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                if (value)
                {
                    OnSelected?.Invoke(this, new BeepComponentEventArgs(this, BoundProperty, _linkedproperty, GetValue()));
                }
                Invalidate();
            }
        }
        public bool IsDeleted { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsEditable { get; set; }
        public bool IsVisible { get; set; }
        public bool IsRequired { get; set; }
        public object Oldvalue { get; }

        private string _theme;
        [Browsable(true)]
        [TypeConverter(typeof(ThemeEnumConverter))]
        public string Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                _currentTheme = BeepThemesManager.GetTheme(value);
                //      this.ApplyTheme();
                ApplyTheme();
            }
        }

        public string GuidID
        { get { return _guidID; } set { _guidID = value; } }
        public int Id { get { return _id; } set { _id = value; } }

       
        public virtual IBeepUIComponent Form { get; set; }
        [Browsable(true)]
        [Category("Data")]
        public string ComponentName
        {
            get => base.Name;
            set
            {
                base.Name = value;
                PropertyChanged?.Invoke(this, new BeepComponentEventArgs(this, "Name", string.Empty, value));
            }

        }
        public event EventHandler<BeepComponentEventArgs> PropertyValidate; // Event to notify that a property is being validated
        [Browsable(true)]
        [Category("Data")]
        public virtual string FieldID { get; set; }
        [Browsable(true)]
        [Category("Data")]
        public virtual string BlockID { get; set; }
        private DbFieldCategory _category = DbFieldCategory.String;
        [Browsable(true)]
        [Category("Data")]
        public DbFieldCategory Category
        {
            get => _category;
            set
            {
                _category = value;
            }
        }
        private string _boundProperty;
        [Browsable(true)]
        [Category("Data")]
        public virtual string BoundProperty
        {
            get => _boundProperty;
            set
            {
                _boundProperty = value;
                if (DataContext != null)
                {
                    SetBinding(_boundProperty, DataSourceProperty);
                }
            }
        }
        private string _linkedproperty;  // this is the property that store the name of another property in the Record data linked to this control 
        [Browsable(true)]
        [Category("Data")]
        public virtual string LinkedProperty
        {
            get => _linkedproperty;
            set
            {
                _linkedproperty = value;

            }
        }
        private string _datasourceproperty;
        [Browsable(true)]
        [Category("Data")]
        public virtual string DataSourceProperty
        {
            get => _datasourceproperty;
            set
            {
                _datasourceproperty = value;
                if (DataContext != null)
                {
                    SetBinding(BoundProperty, _datasourceproperty);
                }
            }


        } // The property of the data source
        #region "IBeepUIComoponent Distinct Control Implementation"
        public virtual void SuspendFormLayout()
        {
            if (Parent != null)
            {
              //  Parent.SuspendLayout();
                this.SuspendLayout();
            }
        }
        public virtual void ResumeFormLayout()
        {
            if (Parent != null)
            {
              //  Parent.ResumeLayout();
                this.ResumeLayout();
                this.PerformLayout();
            }
        }
        public virtual void SetValue(object value)
        {
            if (string.IsNullOrEmpty(BoundProperty)) return;
            var controlProperty = GetType().GetProperty(BoundProperty);
            controlProperty?.SetValue(this, value);

            if (DataContext != null && !string.IsNullOrEmpty(DataSourceProperty))
            {
                var dataSourceProperty = DataContext.GetType().GetProperty(DataSourceProperty);
                dataSourceProperty?.SetValue(DataContext, value);
            }
        }
        public virtual object GetValue()
        {
            if (string.IsNullOrEmpty(BoundProperty))
            {
                return null;
            }
            var controlProperty = GetType().GetProperty(BoundProperty);
            return controlProperty?.GetValue(this);
        }
        public virtual void ClearValue() => SetValue(null);
        public virtual bool HasFilterValue() => !string.IsNullOrEmpty(BoundProperty) && GetValue() != null;
        public virtual AppFilter ToFilter()
        {
            return new AppFilter
            {
                FieldName = BoundProperty,
                FilterValue = GetValue().ToString(),
                Operator = "=",
                valueType = "string"
            };
        }
        // Set binding for a specific control property to a data source property
        public virtual void SetBinding(string controlProperty, string dataSourceProperty)
        {
            if (DataContext == null)
                throw new InvalidOperationException("DataContext is not set.");

            // Check if a binding already exists for the property
            var existingBinding = DataBindings[controlProperty];
            if (existingBinding != null)
            {
                DataBindings.Remove(existingBinding);
            }

            // Add a new binding
            var binding = new Binding(controlProperty, DataContext, dataSourceProperty)
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            };

            DataBindings.Add(binding);

            // Track bound properties for later reference
            BoundProperty = controlProperty;
            DataSourceProperty = dataSourceProperty;
        }

        // Method to validate data, with a default implementation
        public virtual bool ValidateData(out string message)
        {
            var x = new BeepComponentEventArgs(this, BoundProperty, _linkedproperty, GetValue());
            PropertyValidate?.Invoke(this, x);
            if (x.Cancel)
            {
                message = x.Message;
                return false;
            }
            else
            {
                message = string.Empty; return true;
            }

        }

        public virtual void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Draw the control on the provided graphics object


        }
        #endregion "IBeepUIComoponent Distinct Control Implementation"
        #endregion "IBeepUIComoponent"
        #region "HitTest and HitList"
        // Add this method to the BeepControl class
        // Add to the BeepControl class
        // New: ReceiveMouseEvent to emulate mouse events
        // ReceiveMouseEvent pipes the event to native handlers
        public virtual void ReceiveMouseEvent(HitTestEventArgs eventArgs)
        {
            Point location = eventArgs.Location;
            switch (eventArgs.MouseEvent)
            {
                case MouseEventType.Click:
                    OnClick(new EventArgs());
                    break;
                case MouseEventType.DoubleClick:
                    OnDoubleClick(new EventArgs());
                    break;
                case MouseEventType.MouseDown:
                    OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, location.X, location.Y, 0));
                    break;
                case MouseEventType.MouseUp:
                    OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, location.X, location.Y, 0));
                    break;
                case MouseEventType.MouseMove:
                    OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, location.X, location.Y, 0));
                    break;
                case MouseEventType.MouseEnter:
                    OnMouseEnter(new EventArgs());
                    break;
                case MouseEventType.MouseLeave:
                    OnMouseLeave(new EventArgs());
                    break;
                case MouseEventType.MouseHover:
                    OnMouseHover(new EventArgs());
                    break;
                case MouseEventType.MouseWheel:
                    OnMouseWheel(new MouseEventArgs(MouseButtons.None, 0, location.X, location.Y, 120)); // Basic delta
                    break;
                case MouseEventType.None:
                default:
                    break;
            }
        }

        // Helper method to emulate mouse events
        private void EmulateMouseEvent(BeepControl targetControl, MouseEventType eventType, Point location)
        {
            switch (eventType)
            {
                case MouseEventType.Click:
                    targetControl.OnClick(new EventArgs());
                    break;
                case MouseEventType.DoubleClick:
                    targetControl.OnDoubleClick(new EventArgs());
                    break;
                case MouseEventType.MouseDown:
                    targetControl.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, location.X, location.Y, 0));
                    break;
                case MouseEventType.MouseUp:
                    targetControl.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, location.X, location.Y, 0));
                    break;
                case MouseEventType.MouseMove:
                    targetControl.OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, location.X, location.Y, 0));
                    break;
                case MouseEventType.MouseEnter:
                    targetControl.OnMouseEnter(new EventArgs());
                    break;
                case MouseEventType.MouseLeave:
                    targetControl.OnMouseLeave(new EventArgs());
                    break;
                case MouseEventType.MouseHover:
                    targetControl.OnMouseHover(new EventArgs());
                    break;
                case MouseEventType.MouseWheel:
                    // Note: MouseWheel requires a delta value; this is a basic emulation
                    targetControl.OnMouseWheel(new MouseEventArgs(MouseButtons.None, 0, location.X, location.Y, 120));
                    break;
                case MouseEventType.None:
                default:
                    break;
            }
        }

        // New: SendMouseEvent (static)
        // SendMouseEvent for IBeepUIComponent compatibility
        public virtual void SendMouseEvent(IBeepUIComponent targetControl, MouseEventType eventType, Point screenLocation)
        {
            if (targetControl != null)
            {
                // Assume screenLocation is relative to the control's parent; adjust if needed
                Point clientPoint = screenLocation; // If screenLocation is already client coords, use directly
                if (targetControl is Control control)
                {
                    clientPoint = control.PointToClient(screenLocation);
                }
                HitTestEventArgs hitTestEventArgs = new HitTestEventArgs(eventType, clientPoint);
                //MiscFunctions.SendLog($"Sending {eventType} to {targetControl.GetType().Name} at client coordinates: {clientPoint}");
                targetControl.ReceiveMouseEvent(hitTestEventArgs);
            }
        }
        public event EventHandler<ControlHitTestArgs> OnControlHitTest;
        public event EventHandler<ControlHitTestArgs> HitDetected;
        public List<ControlHitTest> HitList { get; set; } = new List<ControlHitTest>();
        public void AddHitTest(ControlHitTest hitTest)
        {
            // Find if there's a match by comparing TargetRect or any unique property you prefer
            var index = HitList.FindIndex(x => x.TargetRect == hitTest.TargetRect);
            if (index >= 0)
            {
                // Update the existing entry with new data
                HitList[index] = hitTest;
            }
            else
            {
                // Otherwise, add as a new entry
                HitList.Add(hitTest);
            }
        }
        // New AddHitArea calculating TargetRect from component
        // New AddHitArea calculating TargetRect and handling coordinate conversion
        // New AddHitArea calculating TargetRect
        public virtual void AddHitArea(string name, IBeepUIComponent component = null, Action hitAction = null)
        {
            Rectangle targetRect = Rectangle.Empty;
            bool isVisible = true;
            bool isEnabled = true;

            if (component is Control control && control.Visible)
            {
                // Default: Use control's Location and Size relative to this BeepControl
                targetRect = new Rectangle(control.Location, control.Size);
                isVisible = control.Visible;
                isEnabled = control.Enabled;

                // Wrap hitAction with coordinate conversion using TargetRect
                Action wrappedHitAction = hitAction != null ? () =>
                {
                    if (component != null)
                    {
                        // Use TargetRect.Location, which is already in client coordinates
                        component.SendMouseEvent(component, MouseEventType.Click, PointToScreen(targetRect.Location));
                    }
                    hitAction.Invoke();
                }
                : null;

                var hitTest = new ControlHitTest
                {
                    Name = name,
                    GuidID = Guid.NewGuid().ToString(),
                    TargetRect = targetRect,
                    uIComponent = component,
                    HitAction = wrappedHitAction,
                    IsVisible = isVisible,
                    IsEnabled = isEnabled
                };

                var index = HitList.FindIndex(x => x.Name == name);
                if (index >= 0)
                {
                    HitList[index] = hitTest;
                }
                else
                {
                    HitList.Add(hitTest);
                }
            }
        }
        public virtual void AddHitArea(string name, Rectangle rect, IBeepUIComponent component = null, Action hitAction = null)
        {
            var hitTest = new ControlHitTest
            {
                Name = name,
                GuidID = Guid.NewGuid().ToString(),
                TargetRect = rect,
                uIComponent = component,
                HitAction = hitAction,
                IsVisible = true,
                IsEnabled = true
            };

            var index = HitList.FindIndex(x => x.Name == name);
            if (index >= 0)
            {
                HitList[index] = hitTest;
            }
            else
            {
                HitList.Add(hitTest);
            }
        }
        public virtual void AddHitTest(Control childControl)
        {
            if (childControl == null)
                throw new ArgumentNullException(nameof(childControl));

            // Ensure the control is a child of this parent
            if (!Controls.Contains(childControl))
                throw new ArgumentException("The specified control is not a child of this control.", nameof(childControl));

            // Create a new ControlHitTest based on the child control
            var hitTest = new ControlHitTest
            {
                Name = childControl.Name, // Use the control's Name property
                GuidID = Guid.NewGuid().ToString(), // Generate a unique ID
                TargetRect = childControl.Bounds, // Set to control's bounds relative to parent
                IsVisible = childControl.Visible, // Sync with control's visibility
                IsEnabled = childControl.Enabled, // Sync with control's enabled state
                                                  // Other properties like HitAction, uIComponent, or ActionName can be set if needed
            };

            // Find if there's an existing entry for this control (based on Name or GuidID)
            var index = HitList.FindIndex(x => x.Name == childControl.Name);
            if (index >= 0)
            {
                // Update the existingfernacht: Update existing entry
                HitList[index] = hitTest;
            }
            else
            {
                // Add as a new entry
                HitList.Add(hitTest);
            }
        }
        public virtual void RemoveHitTest(ControlHitTest hitTest)
        {
            HitList.Remove(hitTest);
        }
        public virtual void ClearHitList()
        {
            HitList.Clear();
        }
        public virtual void UpdateHitTest(ControlHitTest hitTest)
        {
            var index = HitList.FindIndex(x => x.TargetRect == hitTest.TargetRect);
            if (index >= 0)
            {
                HitList[index] = hitTest;
            }

        }
        public virtual bool HitTest(Point location)
        {
            if (HitList == null || !HitList.Any())
            {
                HitAreaEventOn = false;
                HitTestControl = null;
                return false;
            }

            bool hitDetected = false;
            foreach (var hitTest in HitList)
            {
                hitTest.IsHit = false; // Reset IsHit
                if (hitTest?.TargetRect != null && hitTest.IsVisible && hitTest.IsEnabled && hitTest.TargetRect.Contains(location))
                {
                    hitTest.IsHit = true;
                    hitDetected = true;
                    HitAreaEventOn = true; // Flag hit event
                    HitTestControl = hitTest; // Set current hit
                    OnControlHitTest?.Invoke(this, new ControlHitTestArgs(hitTest));
                    break; // First hit only, per your original
                }
            }

            if (!hitDetected)
            {
                HitAreaEventOn = false;
                HitTestControl = null;
            }

            return hitDetected;
        }
        public virtual bool HitTest(Point location, out ControlHitTest hitTest)
        {
            hitTest = null;
            foreach (var test in HitList)
            {
                if (test.TargetRect.Contains(location))
                {
                    hitTest = test;
                    return true;
                }
            }
            return false;
        }
        public virtual bool HitTest(Rectangle rectangle, out ControlHitTest hitTest)
        {
            hitTest = null;
            foreach (var test in HitList)
            {
                if (test.TargetRect.IntersectsWith(rectangle))
                {
                    hitTest = test;
                    return true;
                }
            }
            return false;
        }
        public virtual bool HitTestWithMouse()
        {
            if (!Visible || HitList == null || !HitList.Any())
            {
                HitAreaEventOn = false;
                HitTestControl = null;
                return false;
            }

            Point location = PointToClient(Cursor.Position);
            return HitTest(location);
        }

        #endregion "HitTest and HitList"
        #region Badge Feature
       // private FloatingBadgeForm floatingBadgeForm;

        private string _badgeText = "";
        /// <summary>
        /// Gets or sets the text displayed inside the badge (for example, a counter).
        /// Set this to an empty string to hide the badge.
        /// </summary>
        // First, update the BadgeText property to call UpdateRegionForBadge
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text displayed inside the badge (e.g. a counter). Leave empty to hide the badge.")]
        public virtual string BadgeText
        {
            get => _badgeText;
            set
            {
                if (_badgeText != value)
                {
                    _badgeText = value;
                    UpdateRegionForBadge(); // Add this call to update the region
                    if (Parent is BeepControl bc)
                        bc.Invalidate(Bounds);
                    else
                        Invalidate();
                }
            }
        }

        private Color _badgeBackColor = Color.Red;
        /// <summary>
        /// Gets or sets the background backcolor of the badge.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Background color of the badge.")]
        public Color BadgeBackColor
        {
            get => _badgeBackColor;
            set { _badgeBackColor = value; Invalidate(); }
        }

        private Color _badgeForeColor = Color.White;
        /// <summary>
        /// Gets or sets the text backcolor of the badge.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text color of the badge.")]
        public Color BadgeForeColor
        {
            get => _badgeForeColor;
            set { _badgeForeColor = value; Invalidate(); }
        }

        private Font _badgeFont = new Font("Arial", 8, FontStyle.Bold);
        /// <summary>
        /// Gets or sets the font used for the badge text.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for the badge text.")]
        public Font BadgeFont
        {
            get => _badgeFont;
            set { _badgeFont = value; Invalidate(); }
        }
        BadgeShape _badgeshape = BadgeShape.Circle;
        private bool _isSelected=false;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("shape of Badge.")]
        public BadgeShape BadgeShape
        {
            get => _badgeshape;
            set { _badgeshape = value; Invalidate(); }
        }
        /// <summary>
        /// Draws a badge in the top–right corner of the control’s DrawingRect.
        /// The badge is drawn as a rounded rectangle (or circle for a single character)
        /// and displays the value of BadgeText.
        /// </summary>
        /// <param name="g">The Graphics object to draw on.</param>
        // Replace the existing DrawBadge method
     
        public void DrawBadgeExternally(Graphics g, Rectangle childBounds)
        {
            Console.WriteLine(this.ComponentName);
            // only draw if there's text
            if (string.IsNullOrEmpty(BadgeText))
                return;
      
            const int badgeSize = 22;
            // place it top-right, slightly overlapping
            int x = childBounds.Right - badgeSize / 2;
            int y = childBounds.Y - badgeSize / 2;
            var badgeRect = new Rectangle(x, y, badgeSize, badgeSize);
          
            // now call your existing routine
            DrawBadgeImplementation(g, badgeRect);
        }

        // Next, update the UpdateControlRegion method to properly include the badge area
      
        public void DrawBadgeImplementation(Graphics g, Rectangle badgeRect)
        {
            // High-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Background
            using (var brush = new SolidBrush(BadgeBackColor))
            {
                switch (BadgeShape)
                {
                    case BadgeShape.Circle:
                        g.FillEllipse(brush, badgeRect);
                        break;

                    case BadgeShape.RoundedRectangle:
                        using (var path = GetRoundedRectPath(badgeRect, badgeRect.Height / 4))
                            g.FillPath(brush, path);
                        break;

                    case BadgeShape.Rectangle:
                        g.FillRectangle(brush, badgeRect);
                        break;

                    default:
                        // fall back to circle if someone adds a new enum later
                        g.FillEllipse(brush, badgeRect);
                        break;
                }
            }

            // Text
            using (var textBrush = new SolidBrush(BadgeForeColor))
            using (var scaledFont = GetScaledBadgeFont(
                       g,
                       BadgeText,
                       new Size(badgeRect.Width - 4, badgeRect.Height - 4),
                       BadgeFont ?? Font))
            {
                var fmt = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(BadgeText, scaledFont, textBrush, badgeRect, fmt);
            }
        }

        // Next, update the UpdateControlRegion method to properly include the badge area
        // Finally, make sure UpdateRegionForBadge calls UpdateControlRegion
        // or just replace it entirely (simpler approach):
        public void UpdateRegionForBadge()
        {
            // Simply call the comprehensive method that now handles both control and badge
            UpdateControlRegion();
        }

        // Add this helper method to properly scale badge text
        // Replace the GetScaledBadgeFont method with this safer implementation
        private Font GetScaledBadgeFont(Graphics g, string text, Size maxSize, Font originalFont)
        {
            // Always have a fallback option - use the control's font if anything fails
            Font fallbackFont = Font;

            // If any parameters are null or invalid, use fallback
            if (string.IsNullOrEmpty(text) || originalFont == null || g == null)
                return fallbackFont;

            try
            {
                // For single characters like "1", use a simple fixed size approach
                if (text.Length == 1)
                {
                    float fontSize = Math.Min(maxSize.Height * 0.7f, 12); // 70% of height, max 12pt
                    return new Font(fallbackFont.FontFamily, fontSize, FontStyle.Regular);
                }

                // For longer text, try to fit within bounds
                SizeF textSize;
                try
                {
                    textSize = g.MeasureString(text, originalFont);
                }
                catch
                {
                    return fallbackFont;
                }

                // If original font fits, use it
                if (textSize.Width <= maxSize.Width && textSize.Height <= maxSize.Height)
                    return originalFont;

                // Calculate a scaling factor to fit text
                float scale = Math.Min(
                    maxSize.Width / textSize.Width,
                    maxSize.Height / textSize.Height);

                // Apply scale to font size with minimum limit
                float newSize = Math.Max(originalFont.Size * scale, 7);

                // Create new font
                return new Font(fallbackFont.FontFamily, newSize, FontStyle.Regular);
            }
            catch
            {
                // Any exception, use fallback
                return fallbackFont;
            }
        }

        #endregion
        #region "External Drawing Support"
        // Delegate: child drawing gets parent Graphics plus its own Bounds
        public delegate void DrawExternalHandler(Graphics parentGraphics, Rectangle childBounds);
        // With this new dictionary that uses ExternalDrawingFunction
        private readonly Dictionary<Control, ExternalDrawingFunction> _childExternalDrawers
            = new Dictionary<Control, ExternalDrawingFunction>();
        // When the drawing should occur
        public enum DrawingLayer
        {
            BeforeContent,
            AfterContent,
            AfterAll
        }
        public class ExternalDrawingFunction
        {
            public Control ChildControl { get; set; }
            public DrawExternalHandler Handler { get; set; }
            public DrawingLayer Layer { get; set; }
            public ExternalDrawingFunction(DrawExternalHandler handler, DrawingLayer layer)
            {
                Handler = handler;
                Layer = layer;
            }
            public bool IsValid => Handler != null;
            public void Invoke(Graphics g, Rectangle childBounds)
            {
                Handler?.Invoke(g, childBounds);
            }
            public void Clear()
            {
                Handler = null;
            }
            public void Dispose()
            {
                Handler = null;
            }
            private bool _redraw= false;
            public bool Redraw
            {
                get => _redraw;
                set
                {
                    _redraw = value;
                   
                }
            }
            public void Dispose(bool disposing)
            {
                if (disposing)
                {
                    Handler = null;
                }
            }
        }
        // Global layer selector
        private DrawingLayer _externalDrawingLayer = DrawingLayer.AfterAll;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Which layer to invoke child-registered drawing handlers at")]
        public DrawingLayer ExternalDrawingLayer
        {
            get => _externalDrawingLayer;
            set { _externalDrawingLayer = value; Invalidate(); }
        }

        public bool IsValid { get; private set; }


        /// <summary>
        /// Child calls this to hook its own drawing into the parent.
        /// </summary>
        /// <summary>
        /// Child calls this to hook its own drawing into the parent.
        /// </summary>
        public void AddChildExternalDrawing(Control child, DrawExternalHandler handler, DrawingLayer layer = DrawingLayer.AfterAll)
        {
            if (child == null) throw new ArgumentNullException(nameof(child));

            var drawingFunction = new ExternalDrawingFunction(handler, layer)
            {
                ChildControl = child,
                Redraw = true // Set to true by default so existing code continues to work
            };

            if (_childExternalDrawers.ContainsKey(child))
            {
                _childExternalDrawers[child] = drawingFunction;
                _childExternalDrawers[child].Redraw = true;
            }
            else
            {
                _childExternalDrawers.Add(child, drawingFunction);
            }
        }

        /// <summary>
        /// Sets the redraw state for a child's external drawing function
        /// </summary>
        public void SetChildExternalDrawingRedraw(Control child, bool redraw)
        {
            if (child != null && _childExternalDrawers.TryGetValue(child, out var function))
            {
                function.Redraw = redraw;
            }
        }

        /// <summary>
        /// Function to Tell control that external function status updated
        /// </summary>

        /// <summary>
        /// Remove a previously-registered handler for that child.
        /// </summary>
        /// <summary>
        /// Remove a previously-registered handler for that child.
        /// </summary>
        public void ClearChildExternalDrawing(Control child)
        {
            if (child == null) return;

            if (_childExternalDrawers.TryGetValue(child, out var function))
            {
                function.Dispose();
                _childExternalDrawers.Remove(child);
                Invalidate();
            }
        }

        /// <summary>
        /// Remove *all* child-registered handlers.
        /// </summary>
        public void ClearAllChildExternalDrawing()
        {
            foreach (var function in _childExternalDrawers.Values)
            {
                function.Dispose();
            }

            _childExternalDrawers.Clear();
            Invalidate();
        }

        /// <summary>
        /// Invoke exactly those child-registered handlers whose Layer matches.
        /// </summary>
        /// <summary>
        /// Invoke exactly those child-registered handlers whose Layer matches and Redraw is true.
        /// </summary>
        private void PerformExternalDrawing(Graphics g, DrawingLayer layer)
        {
            foreach (var kvp in _childExternalDrawers)
            {
                var child = kvp.Key;
                var drawingFunction = kvp.Value;

                // Only execute if:
                // 1. Child is visible
                // 2. The layer matches
                // 3. Redraw is true
                // 4. The function is valid (has a handler)
                if (!child.Visible || drawingFunction.Layer != layer || !drawingFunction.Redraw || !drawingFunction.IsValid)
                    continue;
           //     drawingFunction.Redraw = false; // Reset redraw state
                // Pass the child's bounds so the handler knows where to draw
                drawingFunction.Invoke(g, child.Bounds);
            }
        }

        #endregion

        #region "Dispose"
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 1) If we'd registered external drawing with our parent, clear it
                if (Parent is BeepControl parentBeepControl)
                {
                    parentBeepControl.ClearChildExternalDrawing(this);
                }

                // 2) Clean up all drawing functions
                foreach (var function in _childExternalDrawers.Values)
                {
                    function.Dispose(true);
                }
                _childExternalDrawers.Clear();

                // 3) Stop & dispose any running animation timer
                if (_animationTimer != null)
                {
                    _animationTimer.Stop();
                    _animationTimer.Dispose();
                    _animationTimer = null;
                }

                // 4) Dispose of the tooltip
                _toolTip?.Dispose();
                _toolTip = null;
            }

            base.Dispose(disposing);
        }


        #endregion "Dispose"
    }

}

