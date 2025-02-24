using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;
using System.Drawing.Design;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using System.IO;
using TheTechIdea.Beep.Desktop.Common;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum TypeStyleFontSize
    {
        None,
        Small,
        Medium,
        Big,
        Banner,
        Large,
        ExtraLarge,
        ExtraExtraLarge,
        ExtraExtraExtraLarge
    }
    public enum CustomBorderStyle
    {
        None,
        Solid,
        Dashed,
        Dotted
    }
    public enum DisplayAnimationType
    {
        None,
        Popup,
        Slide,
        Fade,
        SlideAndFade
    }
    public enum EasingType
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut
    }
    public enum SlideDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Control")]
    [Description("A control that provides a base for all Beep UI components.")]
    public class BeepControl : ContainerControl, IBeepUIComponent,IDisposable
    {
        #region "protected Properties"
        Point originalLocation;
        protected bool _isControlinvalidated = false;
        protected bool tooltipShown = false; // Flag to track if tooltip is shown
        protected ImageScaleMode _scaleMode = ImageScaleMode.KeepAspectRatio;
        protected bool _staticnotmoving = false;
        protected ToolTip _toolTip;
        protected bool _showAllBorders = true;
        protected Color _focusIndicatorColor = Color.Blue;
        protected bool _showFocusIndicator = false;
        protected bool _showTopBorder;
        protected bool _showBottomBorder;
        protected bool _showLeftBorder;
        protected bool _showRightBorder;
        protected int _borderThickness = 1;
        protected BorderStyle _borderStyle = BorderStyle.FixedSingle;
        protected Color _borderColor = Color.Black;
        protected bool _isRounded = true;
        protected bool _useGradientBackground = false;
        protected LinearGradientMode _gradientDirection = LinearGradientMode.Horizontal;
        protected Color _gradientStartColor = Color.Gray;
        protected Color _gradientEndColor = Color.Gray;
        protected bool _showShadow = false;
        protected Color _shadowColor = Color.Black;
        protected float _shadowOpacity = 0.5f;
        protected Color _hoverBackColor = Color.Gray;
        protected Color _pressedBackColor = Color.Gray;
        protected Color _focusBackColor = Color.Gray;
        protected Color _inactiveBackColor = Color.Gray;
        protected Color _hoverBorderColor = Color.Gray;
        protected Color _pressedBorderColor = Color.Gray;
        protected Color _focusBorderColor = Color.Gray;
        protected Color _inactiveBorderColor = Color.Gray;
        protected Color _hoverForeColor = Color.Black;
        protected Color _pressedForeColor = Color.Black;
        protected Color _focusForeColor = Color.Black;
        protected Color _inactiveForeColor = Color.Black;

        protected Color _activeBackColor = Color.Gray;
        protected Color _disabledBackColor = Color.Gray;
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
        protected VScrollBar vScrollBar;
        protected HScrollBar hScrollBar;
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
        #endregion "protected Properties"
        #region "Public Properties"
        //    public IContainer Components => this.Components;
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
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        [Description("The text associated with the Beepbutton.")]
        public override string Text
        {
            get => _text;
            set
            {
                _text = value;
                _isControlinvalidated = true;
                Invalidate();  // Trigger repaint when the text changes
            }
        }
        [Browsable(true), Category("Appearance")]
        public TypeStyleFontSize OverrideFontSize
        {
            get { return _overridefontsize; }
            set { _overridefontsize = value; Invalidate(); }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsFramless { get { return _isframless; } set { _isframless = value; Invalidate(); } }

        [Browsable(true)]
        [Category("Appearance")]
        public Color HoveredBackcolor { get { return _hoveredBackcolor; } set { _hoveredBackcolor = value; Invalidate(); } }


        [Browsable(true)]
        [Category("Appearance")]
        public bool IsHovered { get { return _isHovered; } set { if (CanBeHovered) { _isHovered = value; Invalidate(); } } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsPressed { get { return _isPressed; } set { if (CanBePressed) { _isPressed = value; Invalidate(); } } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsFocused { get { return _isFocused; } set { if (CanBeFocused) { _isFocused = value; Invalidate(); } } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsDefault { get { return _isDefault; } set { _isDefault = value; Invalidate(); } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsAcceptButton { get { return _isAcceptButton; } set { _isAcceptButton = value; Invalidate(); } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsCancelButton { get { return _isCancelButton; } set { _isCancelButton = value; Invalidate(); } }


        public string SavedID { get; set; }
        public string SavedGuidID { get; set; }


        protected bool _isChild;
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
                ApplyTheme();
                Invalidate();
            }
        }
        // Border properties
        protected int _borderRadius = 3;

        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                _isControlinvalidated = true;
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

        public Color ActiveBackColor
        {
            get => _activeBackColor;
            set
            {
                _activeBackColor = value;
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
        public Color FocusBackColor
        {
            get => _focusBackColor;
            set
            {
                _focusBackColor = value;
                Invalidate();
            }
        }
        public Color InactiveBackColor
        {
            get => _inactiveBackColor;
            set
            {
                _inactiveBackColor = value;
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
        public Color PressedBorderColor
        {
            get => _pressedBorderColor;
            set
            {
                _pressedBorderColor = value;
                Invalidate();
            }
        }
        public Color FocusBorderColor
        {
            get => _focusBorderColor;
            set
            {
                _focusBorderColor = value;
                Invalidate();
            }
        }
        public Color InactiveBorderColor
        {
            get => _inactiveBorderColor;
            set
            {
                _inactiveBorderColor = value;
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
        public Color PressedForeColor
        {
            get => _pressedForeColor;
            set
            {
                _pressedForeColor = value;
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
        public Color InactiveForeColor
        {
            get => _inactiveForeColor;
            set
            {
                _inactiveForeColor = value;
                Invalidate();
            }
        }
        public Color DisabledForeColor
        {
            get => _disabledForeColor;
            set
            {
                _disabledForeColor = value;
                Invalidate();
            }
        }



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
        protected Color _disabledForeColor;
        private bool _isAnimating;



        public Rectangle DrawingRect { get; set; }
        public bool IsCustomeBorder { get; set; }

        #endregion "Public Properties"
        #region "Constructors"
        public BeepControl()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            InitializeTooltip();
            ShowAllBorders = true;
            //  BackColor = Color.Transparent;
            Padding = new Padding(0);
            UpdateDrawingRect();

            DataBindings.CollectionChanged += DataBindings_CollectionChanged;

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

        // Override property binding management when DataContext changes
        protected override void OnBindingContextChanged(EventArgs e)
        {
            base.OnBindingContextChanged(e);
            UpdateBindings();
        }

        // Clear and recreate bindings when DataContext changes
        protected virtual void UpdateBindings()
        {
            if (DataContext != null)
            {
                ReapplyBindings();
            }
            else
            {
                // Clear all bindings if DataContext is null
                DataBindings.Clear();
            }
        }
        // RefreshBindings method: Force an update to each binding in the control
        public void RefreshBinding()
        {
            if (DataContext != null && !string.IsNullOrEmpty(DataSourceProperty))
            {
                var dataSourceProperty = DataContext.GetType().GetProperty(DataSourceProperty);
                var controlProperty = GetType().GetProperty(BoundProperty);

                if (dataSourceProperty != null && controlProperty != null)
                {
                    var value = dataSourceProperty.GetValue(DataContext);
                    controlProperty.SetValue(this, value);
                }
            }
        }
        // Method to be called whenever DataContext changes
        protected virtual void OnDataContextChanged()
        {
            ReapplyBindings();
        }

        // Reapply bindings with updated DataContext
        private void ReapplyBindings()
        {
            // Cache original bindings to reapply with the updated DataContext
            var originalBindings = DataBindings.Cast<Binding>().ToList();

            // Clear existing bindings
            DataBindings.Clear();

            // Reapply each original binding with the updated DataContext
            foreach (var originalBinding in originalBindings)
            {
                var newBinding = new Binding(
                    originalBinding.PropertyName,
                    DataContext,
                    originalBinding.BindingMemberInfo.BindingField,
                    originalBinding.FormattingEnabled,
                    originalBinding.DataSourceUpdateMode,
                    originalBinding.NullValue
                )
                {
                    FormatString = originalBinding.FormatString,
                    NullValue = originalBinding.NullValue
                };

                // Add the new binding to the control's DataBindings collection
                DataBindings.Add(newBinding);
            }

            // Refresh the control to display updated values
            Invalidate();
        }

        // Event handler to manage data binding changes dynamically
        protected void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (DataContext != null)
            {
                UpdateBindings();
            }
        }

        #endregion
        #region "Theme"
        public virtual void ApplyTheme()
        {
            try
            {
                //ForeColor = _currentTheme.LatestForColor;
                //BackColor = _currentTheme.BackgroundColor;;
                BorderColor = _currentTheme.BorderColor;
                HoverBackColor = _currentTheme.ButtonHoverBackColor;
                PressedBackColor = _currentTheme.ButtonActiveBackColor;
                FocusBackColor = _currentTheme.ButtonActiveForeColor;
                HoverBorderColor = _currentTheme.HoverLinkColor;
                ActiveBackColor = _currentTheme.ButtonActiveBackColor;
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
        public virtual void ApplyTheme(EnumBeepThemes theme) => Theme = theme;
        public virtual void ApplyTheme(BeepTheme theme)
        {
            Theme = BeepThemesManager.GetThemeToEnum(theme);
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
        #region "Painting"
        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            UpdateDrawingRect();
            Invalidate(); // Trigger a redraw when padding changes
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDrawingRect();
            // Defer the region update to ensure layout is complete.
            //this.BeginInvoke((MethodInvoker)delegate {
            //    UpdateControlRegion();
            //});
            UpdateControlRegion();
            Invalidate(); // Redraw on resize to adjust title positioning
        }
        private void UpdateControlRegion()
        {
            if (this.Width <= 0 || this.Height <= 0)
                return;
            // For initial testing, use the full client rectangle
            Rectangle regionRect = new Rectangle(0, 0, this.Width, this.Height);
            if (IsRounded)
            {
                using (GraphicsPath path = GetRoundedRectanglePath(regionRect, BorderRadius))
                {
                    this.Region = new Region(path);
                }
            }
            else
            {
                this.Region = new Region(regionRect);
            }
        }

        // Override background painting for optimized repaint
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
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

            // Log or debug the dimensions if needed
            //  Debug.WriteLine($"DrawingRect: {DrawingRect}");
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
        protected override void OnPaint(PaintEventArgs e)
        {
            SuspendLayout();
            base.OnPaint(e);

            var g = e.Graphics;

            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.TextContrast = 12;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.None;
            // g.CompositingQuality = CompositingQuality.HighQuality;
            if (IsChild)
            {
                BackColor = parentbackcolor;
            }
            e.Graphics.Clear(BackColor);

            shadowOffset = ShowShadow ? 3 : 0;

            // Update the drawing rectangle to reflect shadow and border changes
            UpdateDrawingRect();

            Rectangle outerRectangle = new Rectangle(0, 0, Width, Height);
            Rectangle borderRectangle = new Rectangle(
                shadowOffset+ _borderThickness,
                shadowOffset+ _borderThickness,
                Width - (_borderThickness + shadowOffset),
                Height - (_borderThickness + shadowOffset)
            );
            
            if (UseGradientBackground)
            {
                using (var brush = new LinearGradientBrush(borderRectangle, GradientStartColor, GradientEndColor, GradientDirection))
                {
                    e.Graphics.FillRectangle(brush, outerRectangle);
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    e.Graphics.FillRectangle(brush, outerRectangle);
                }
            }
            if (!_isframless)
            {
                if (ShowShadow)
                {
                    DrawShadowUsingRectangle(g);
                }
                if (IsCustomeBorder)
                {
                    DrawCustomBorder(e);
                }
                else
                {
                    if (ShowAllBorders && IsRounded)
                    {
                          UpdateControlRegion();
                        //using (GraphicsPath path = GetRoundedRectanglePath(borderRectangle, BorderRadius))
                        //{
                        //    this.Region = new Region(path);
                        //}
                        using (GraphicsPath path = GetRoundedRectanglePath(borderRectangle, BorderRadius))
                        {
                            // Now draw the border
                            if (BorderThickness > 0)
                            {
                                using (Pen borderPen = new Pen(BorderColor, BorderThickness))
                                {
                                    borderPen.Alignment = PenAlignment.Inset;
                                    e.Graphics.DrawPath(borderPen, path);
                                }
                            }
                        }

                    }
                    else if (ShowAllBorders && BorderThickness > 0)
                    {
                        DrawBorder(g, borderRectangle);
                    }
                }
                
            }
            if (ShowFocusIndicator && Focused)
            {
                DrawFocusIndicator(g);
            }

            ResumeLayout();
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
        public virtual void DrawCustomBorder(PaintEventArgs e)
        {
            // Draw custom border based on the control's properties
            DrawBorder(e.Graphics, DrawingRect);
        }
        protected void DrawBackColor(PaintEventArgs e, Color color, Color hovercolor)
        {
            shadowOffset = ShowShadow ? 3 : 0;

            // Update the drawing rectangle to reflect shadow and border changes
            UpdateDrawingRect();

            if (IsChild)
            {
                if (IsHovered)
                {
                    // Draw background based on `IsRounded` and `UseGradientBackground`
                    if (IsRounded)
                    {
                        using (GraphicsPath path = GetRoundedRectPath(DrawingRect, BorderRadius))
                        {
                            if (UseGradientBackground)
                            {
                                using (var brush = new LinearGradientBrush(DrawingRect, GradientStartColor, GradientEndColor, GradientDirection))
                                {
                                    e.Graphics.FillPath(brush, path);
                                }
                            }
                            else
                            {
                                using (var brush = new SolidBrush(IsHovered ? hovercolor : color))
                                {
                                    e.Graphics.FillPath(brush, path);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (UseGradientBackground)
                        {
                            using (var brush = new LinearGradientBrush(DrawingRect, GradientStartColor, GradientEndColor, GradientDirection))
                            {
                                e.Graphics.FillRectangle(brush, DrawingRect);
                            }
                        }
                        else
                        {
                            using (var brush = new SolidBrush(IsHovered ? hovercolor : color))
                            {
                                e.Graphics.FillRectangle(brush, DrawingRect);
                            }
                        }
                    }
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(parentbackcolor))
                    {
                        e.Graphics.FillRectangle(brush, DrawingRect);
                    }
                }
            }
            else
            {
                // Draw background based on `IsRounded` and `UseGradientBackground`
                if (IsRounded)
                {
                    using (GraphicsPath path = GetRoundedRectPath(DrawingRect, BorderRadius))
                    {
                        if (UseGradientBackground)
                        {
                            using (var brush = new LinearGradientBrush(DrawingRect, GradientStartColor, GradientEndColor, GradientDirection))
                            {
                                e.Graphics.FillPath(brush, path);
                            }
                        }
                        else
                        {
                            using (var brush = new SolidBrush(IsHovered ? hovercolor : color))
                            {
                                e.Graphics.FillPath(brush, path);
                            }
                        }
                    }
                }
                else
                {
                    if (UseGradientBackground)
                    {
                        using (var brush = new LinearGradientBrush(DrawingRect, GradientStartColor, GradientEndColor, GradientDirection))
                        {
                            e.Graphics.FillRectangle(brush, DrawingRect);
                        }
                    }
                    else
                    {
                        using (var brush = new SolidBrush(IsHovered ? hovercolor : color))
                        {
                            e.Graphics.FillRectangle(brush, DrawingRect);
                        }
                    }
                }

            }
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
            using (var pen = new Pen(FocusIndicatorColor, 2))
            {
                graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
            }
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
        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            // If radius is 0 or less, return a normal rectangle.
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // Ensure the diameter does not exceed the bounds of the rectangle.
            if (diameter > rect.Width)
                diameter = rect.Width;
            if (diameter > rect.Height)
                diameter = rect.Height;

            // Define a rectangle for each corner arc.
            Rectangle topLeftArc = new Rectangle(rect.X, rect.Y, diameter, diameter);
            Rectangle topRightArc = new Rectangle(rect.Right - diameter, rect.Y, diameter, diameter);
            Rectangle bottomRightArc = new Rectangle(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter);
            Rectangle bottomLeftArc = new Rectangle(rect.X, rect.Bottom - diameter, diameter, diameter);

            // Add arcs for each corner.
            path.AddArc(topLeftArc, 180, 90);   // Top-left
            path.AddArc(topRightArc, 270, 90);  // Top-right
            path.AddArc(bottomRightArc, 0, 90); // Bottom-right
            path.AddArc(bottomLeftArc, 90, 90); // Bottom-left

            // Close the path to complete the rounded rectangle.
            path.CloseFigure();

            return path;
        }
        #endregion "Painting"

        #endregion "Drawing Methods"
        #region "Mouse events"
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            //   BorderColor = _currentTheme.HoverLinkColor;
         //   IsHovered = true;
            //  ShowToolTipIfExists();
            //Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
           
         
            base.OnMouseMove(e);

        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            //  BorderColor = _currentTheme.BorderColor;
           // IsPressed = false;
            IsFocused = false;
            IsHovered = false;
            // HideToolTip(); // Hide tooltip on mouse leave
            // Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            IsFocused = true;
            base.OnGotFocus(e);
        
        }
        protected override void OnLostFocus(EventArgs e)
        {
            IsFocused = false;
            IsHovered = false;
            base.OnLostFocus(e);
             
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            // BorderColor = _currentTheme.ActiveBorderColor;
            if (e.Button == MouseButtons.Left)
            {
                // Perform the hit test
                HitTest(e.Location);
                IsPressed = true;
                IsFocused = true;

            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            IsPressed = false;
            base.OnMouseUp(e);
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            IsHovered = true;
        }
        #endregion "Mouse events"
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
            //Size imageSize = beepImage != null && beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;

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
            base.OnParentChanged(e);

            // If BeepControl is removed from its parent, remove the floating badge.
            if (this.Parent == null && floatingBadgeForm != null && !floatingBadgeForm.IsDisposed)
            {
                // Remove from old parent if it still exists
                if (floatingBadgeForm.Parent != null)
                {
                    floatingBadgeForm.Parent.Controls.Remove(floatingBadgeForm);
                }
                floatingBadgeForm.Dispose();
                floatingBadgeForm = null;
            }
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

        protected EnumBeepThemes _themeEnum = EnumBeepThemes.DefaultTheme;
        protected BeepTheme _currentTheme = BeepThemesManager.DefaultTheme;
        public event EventHandler<BeepComponentEventArgs> OnSelected;
        public event EventHandler<BeepComponentEventArgs> OnValidate;
        public event EventHandler<BeepComponentEventArgs> OnValueChanged;
        public event EventHandler<BeepComponentEventArgs> OnLinkedValueChanged;
        public bool IsSelected { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsEditable { get; set; }
        public bool IsVisible { get; set; }
        public bool IsRequired { get; set; }
        public object Oldvalue { get; }
        [Browsable(true)]
        [TypeConverter(typeof(ThemeEnumConverter))]
        public EnumBeepThemes Theme
        {
            get => _themeEnum;
            set
            {
                _themeEnum = value;
                _currentTheme = BeepThemesManager.GetTheme(value);
                //      this.ApplyTheme();
                ApplyTheme();
            }
        }

        public string GuidID
        { get { return _guidID; } set { _guidID = value; } }
        public int Id { get { return _id; } set { _id = value; } }

        protected object _dataContext;
        [Browsable(true)]
        [Category("Data")]
        [Description("Gets or sets the data context for data binding.")]
        public new object DataContext
        {
            get => _dataContext;
            set
            {
                _dataContext = value;
                OnDataContextChanged();
            }
        }

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
        public virtual void SetValue(object value)
        {
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
            var controlProperty = GetType().GetProperty(BoundProperty);
            return controlProperty?.GetValue(this);
        }
        public virtual void ClearValue() => SetValue(null);
        public virtual bool HasFilterValue() => !string.IsNullOrEmpty(BoundProperty) && GetValue() != null;
        public AppFilter ToFilter()
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
        public event EventHandler<ControlHitTestArgs> OnControlHitTest;
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

        public void RemoveHitTest(ControlHitTest hitTest)
        {
            HitList.Remove(hitTest);
        }
        public void ClearHitList()
        {
            HitList.Clear();
        }
        public void UpdateHitTest(ControlHitTest hitTest)
        {
            var index = HitList.FindIndex(x => x.TargetRect == hitTest.TargetRect);
            if (index >= 0)
            {
                HitList[index] = hitTest;
            }

        }
        public void HitTest(Point location)
        {
            foreach (var hitTest in HitList)
            {
                hitTest.IsHit = hitTest.TargetRect.Contains(location);
                if (hitTest.IsHit)
                {
                    OnControlHitTest?.Invoke(this, new ControlHitTestArgs(hitTest));
                    if(hitTest.HitAction != null)
                    {
                        hitTest.HitAction.Invoke();
                    }
                    break;
                }
            }
        }

        #endregion "HitTest and HitList"
        #region Badge Feature
        private FloatingBadgeForm floatingBadgeForm;

        private string _badgeText = "";
        /// <summary>
        /// Gets or sets the text displayed inside the badge (for example, a counter).
        /// Set this to an empty string to hide the badge.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text displayed inside the badge (e.g. a counter). Leave empty to hide the badge.")]
        public string BadgeText
        {
            get => _badgeText;
            set { _badgeText = value; Invalidate(); }
        }

        private Color _badgeBackColor = Color.Red;
        /// <summary>
        /// Gets or sets the background color of the badge.
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
        /// Gets or sets the text color of the badge.
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
        protected void DrawBadge(Graphics g)
        {
            if (string.IsNullOrEmpty(BadgeText))
            {
                if (floatingBadgeForm != null && !floatingBadgeForm.IsDisposed)
                {
                    floatingBadgeForm.Hide();
                }
                return;
            }

            // Measure text size
            Size textSize = TextRenderer.MeasureText(BadgeText, BadgeFont);
            int padding = 4;
            int badgeWidth = textSize.Width + padding;
            int badgeHeight = textSize.Height + padding;

            // For single character, ensure circle proportions
            if (BadgeText.Length == 1)
            {
                badgeWidth = badgeHeight = Math.Max(badgeWidth, badgeHeight);
            }

            if (this.Parent != null)
            {
                // Calculate screen location
                Point controlScreenLocation = this.Parent.PointToScreen(this.Location);

                // Badge position (top-right)
                int badgeX = controlScreenLocation.X + this.Width - badgeWidth / 2;
                int badgeY = controlScreenLocation.Y - badgeHeight / 2;

                // Convert screen coordinates to parent's client area
                Point parentClientPoint = this.Parent.PointToClient(new Point(badgeX, badgeY));
                Rectangle badgeRect = new Rectangle(parentClientPoint, new Size(badgeWidth, badgeHeight));

                // **🔹 Recreate badge form if the shape has changed**
                if (floatingBadgeForm == null || floatingBadgeForm.IsDisposed || floatingBadgeForm.BadgeShape != this.BadgeShape)
                {
                    floatingBadgeForm?.Dispose(); // **Dispose old instance**
                    floatingBadgeForm = new FloatingBadgeForm
                    {
                        TopLevel = false,
                        FormBorderStyle = FormBorderStyle.None,
                        ShowInTaskbar = false,
                        BackColor = Color.Magenta,
                        TransparencyKey = Color.Magenta,
                        BadgeShape = this.BadgeShape // **Apply new shape**
                    };

                    this.Parent.Controls.Add(floatingBadgeForm);
                }

                // Update properties
                floatingBadgeForm.BadgeText = this.BadgeText;
                floatingBadgeForm.BadgeBackColor = this.BadgeBackColor;
                floatingBadgeForm.BadgeForeColor = this.BadgeForeColor;
                floatingBadgeForm.BadgeFont = this.BadgeFont;
                floatingBadgeForm.Size = new Size(badgeWidth, badgeHeight);
                floatingBadgeForm.Location = badgeRect.Location;

                // **Force redraw**
                floatingBadgeForm.Invalidate();
                floatingBadgeForm.BringToFront();
                if (!floatingBadgeForm.Visible)
                {
                    floatingBadgeForm.Show();
                }
            }
            else
            {
                // **Fallback: Draw inside the control**
                Rectangle badgeRect = new Rectangle(
                    DrawingRect.Right - badgeWidth,
                    DrawingRect.Y,
                    badgeWidth,
                    badgeHeight);
                int offsetX = 2;
                int offsetY = -2;
                badgeRect.Offset(offsetX, offsetY);

                using (GraphicsPath path = GetRoundedRectanglePath(badgeRect, badgeRect.Height / 2))
                {
                    using (SolidBrush brush = new SolidBrush(BadgeBackColor))
                    {
                        g.FillPath(brush, path);
                    }
                }

                TextRenderer.DrawText(g, BadgeText, BadgeFont, badgeRect, BadgeForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }




        #endregion
        #region "Dispose"
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // If the floating badge exists
                if (floatingBadgeForm != null && !floatingBadgeForm.IsDisposed)
                {
                    // Remove it from the parent’s Controls collection if it’s there
                    if (floatingBadgeForm.Parent != null)
                    {
                        floatingBadgeForm.Parent.Controls.Remove(floatingBadgeForm);
                    }

                    floatingBadgeForm.Dispose();
                    floatingBadgeForm = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion "Dispose"
    }
    public class ControlHitTest
    {
        public string Name { get; set; }
        public string GuidID { get; set; }= Guid.NewGuid().ToString();
        public Rectangle TargetRect { get; set; }
        public Action HitAction { get; set; }
        public string ActionName { get; set; }
        public bool IsHit
        {
            get; set;
        }
        public bool IsVisible { get; set; } = true;
        public bool IsEnabled { get; set; } = true;
        public bool IsSelected { get; set; }
        public bool IsPressed { get; set; }
        public bool IsHovered { get; set; }
        public bool IsFocused { get; set; }

        public ControlHitTest()
        {
        }
        public ControlHitTest(Rectangle rect)
        {
            TargetRect = rect;
        }
        public ControlHitTest(Rectangle rect, Point location)
        {
            TargetRect = rect;
            IsHit = TargetRect.Contains(location);
        }


    }
    public class ControlHitTestArgs : EventArgs
    {
        public ControlHitTest HitTest { get; set; }
        public ControlHitTestArgs(ControlHitTest hitTest)
        {
            HitTest = hitTest;
        }

    }
}

