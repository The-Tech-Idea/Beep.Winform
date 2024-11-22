using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Template;
using System.Drawing;
using Microsoft.VisualBasic.Logging;
using Svg;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum TypeStyleFontSize
    {
        None,
        Small,
        Medium,
        Big,
        Banner
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
    public class BeepControl : ContainerControl,  IBeepUIComponent
    {
        #region "protected Properties"
        Point originalLocation;
        protected bool _isControlinvalidated = false;
        protected bool tooltipShown = false; // Flag to track if tooltip is shown
        protected ImageScaleMode _scaleMode = ImageScaleMode.KeepAspectRatio;
        protected bool _staticnotmoving = false;
        protected ToolTip _toolTip;
        protected bool _showAllBorders = true;
        protected Color _focusIndicatorColor  = Color.Blue;
        protected bool _showFocusIndicator=false;
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
        protected bool _showShadow = true;
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
        protected string [] _items = new string[0];
        protected bool _isHovered = false;
        protected bool _isPressed = false;
        protected bool _isFocused = false;
        protected bool _isDefault = false;
        protected bool _isAcceptButton = false;
        protected bool _isCancelButton = false;
       protected bool _isframless = false;
        protected Color _hoveredBackcolor = Color.Wheat;
        protected TypeStyleFontSize  _overridefontsize= TypeStyleFontSize.None;
        protected string _text = string.Empty;
        protected bool _isborderaffectedbytheme = true;
        protected bool _isshadowaffectedbytheme = true;
        #endregion "protected Properties"
        #region "Public Properties"
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
        [Description("The text associated with the BeepButton.")]
        public override string Text
        {
            get => _text;
            set
            {
                _text = value;
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
        public bool IsFramless { get { return _isframless; } set { _isframless = value;Invalidate(); } }

        [Browsable(true)]
        [Category("Appearance")]
        public Color HoveredBackcolor { get { return _hoveredBackcolor; } set { _hoveredBackcolor = value; Invalidate(); } }


        [Browsable(true)]
        [Category("Appearance")]
        public bool IsHovered { get { return _isHovered; } set { _isHovered = value; Invalidate(); } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsPressed { get { return _isPressed; } set { _isPressed = value; Invalidate(); } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsFocused { get { return _isFocused; } set { _isFocused = value; Invalidate(); } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsDefault { get { return _isDefault; } set { _isDefault = value; Invalidate(); } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsAcceptButton { get { return _isAcceptButton; } set { _isAcceptButton = value; Invalidate(); } }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsCancelButton { get { return _isCancelButton; } set { _isCancelButton = value; Invalidate(); } }
 
        public string FieldID { get; set; }
        public string BlockID { get; set; }
        public string SavedID { get; set; }
        public string SavedGuidID { get; set; }
        public string [] Items { get { return _items; } set { _items = value; } }
        public string GuidID
            { get { return _guidID; } }
        public int Id {  get { return _id; }set { _id = value; } }

        protected bool _isChild;
        protected Color parentbackcolor;
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


        [Browsable(true)]
        [Category("Appearance")]
        public Color ParentBackColor
        {
            get => parentbackcolor;
            set
            {
                parentbackcolor = value;
                Invalidate();  // Trigger repaint
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsChild
        {
            get => _isChild;
            set
            {
                _isChild = value;
                if (this.Parent != null)
                {
                    parentbackcolor = this.Parent.BackColor;

                }
                BackColor = parentbackcolor;
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
        // Theme Properties
        protected EnumBeepThemes _themeEnum = EnumBeepThemes.DefaultTheme;
        protected BeepTheme _currentTheme = BeepThemesManager.DefaultTheme;
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
        [Description("Show the top border.")]
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
        [Description("Show the bottom border.")]
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
        [Description("Show the left border.")]
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
        [Description("Show the right border.")]
        public bool ShowRightBorder
        {
            get => _showRightBorder;
            set
            {
                _showRightBorder = value;
                Invalidate(); // Redraw when this property changes
            }
        }
        public new virtual bool Enabled { get; set; } = true;
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
                if (BorderThickness == 0) { 
                    _borderThickness = 1;
                }
                Invalidate(); // Redraw when all borders are set at once
            }
        }


        // Border properties
        protected int _borderRadius = 5;

        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                Invalidate();
            }
        }

        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
                Invalidate();
            }
        }

        public new BorderStyle BorderStyle
        {
            get => _borderStyle;
            set
            {
                _borderStyle = value;
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
                if(_showShadow)
                {
                    shadowOffset = _tmpShadowOffset;
                }
                else
                {
                    shadowOffset = 0;
                }
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

        [Browsable(true)]
        [TypeConverter(typeof(ThemeConverter))]
        public EnumBeepThemes Theme
        {
            get => _themeEnum;
            set
            {
                _themeEnum = value;
                _currentTheme = BeepThemesManager.GetTheme(value);
                this.ApplyTheme();
                ApplyTheme();
            }
        }
        protected object _dataContext;

        // Track Format and Parse event handlers for reattachment
        protected Dictionary<Binding, EventHandler<ConvertEventArgs>> formatHandlers = new();
        protected Dictionary<Binding, EventHandler<ConvertEventArgs>> parseHandlers = new();
        protected List<Binding> _originalBindings = new List<Binding>();
        protected Color _disabledForeColor;
        private bool _isAnimating;

        [Browsable(true)]
        [Category("Data")]
        [Description("Gets or sets the data context for data binding.")]
        public object DataContext
        {
            get => _dataContext;
            set
            {
                _dataContext = value;
                OnDataContextChanged();
            }
        }
        public IBeepUIComponent Form { get; set; }
        public Rectangle DrawingRect { get; set; }
        
        #endregion "Public Properties"

        public BeepControl()
        {
            DoubleBuffered = true;
            SetStyle( ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true); // Ensure we handle transparent backcolors
            InitializeTooltip();
            ShowAllBorders = true;
            //  BackColor = Color.Transparent;
            Padding = new Padding(0);
            UpdateDrawingRect();
            DataBindings.CollectionChanged += DataBindings_CollectionChanged;

        }
        #region "Data Binding"
        // Override property binding management when DataContext changes
        protected override void OnBindingContextChanged(EventArgs e)
        {
            base.OnBindingContextChanged(e);
            UpdateBindings();
        }
        // Clear and recreate bindings when DataContext changes
        protected void UpdateBindings()
        {
            OnDataContextChanged();
        }
        // RefreshBindings method: Force an update to each binding in the control
        protected void RefreshBindings()
        {
            foreach (Binding binding in DataBindings)
            {
                // Refresh each binding to re-evaluate the property on the new DataContext
                binding.ReadValue();
            }

            // Optionally trigger a repaint to reflect changes in the UI
            Invalidate();
        }
        // Method to be called whenever DataContext changes
        protected virtual void OnDataContextChanged()
        {
            // Clear and refresh the original bindings to avoid stale references
            _originalBindings.Clear();
            _originalBindings.AddRange(DataBindings.Cast<Binding>());
            // Clear existing bindings
            DataBindings.Clear();

            // Reapply each original binding with the new DataContext
            foreach (var originalBinding in _originalBindings)
            {
                // Create a new binding with the updated DataContext
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
        // Cache event handlers for Format and Parse events
       
        // Event handler to manage data binding changes, if needed
        protected void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            // Adjust bindings as needed (e.g., add/remove bindings dynamically)
            if (DataContext != null)
            {
                UpdateBindings();
            }
        }
        public void SetBinding(string controlProperty, string dataSourceProperty)
        {
            if (DataContext == null)
                throw new InvalidOperationException("DataContext is not set.");

            var binding = new Binding(controlProperty, DataContext, dataSourceProperty);
            this.DataBindings.Add(binding);
        }
    
    #endregion "Data Binding"
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
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
      
        }
        #region "Theme"
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
            foreach (Control ctrl in Controls)
            {
                if (ctrl is BeepControl)
                {
                    ApplyThemeToControl(ctrl);
                }
            }
        }
        #endregion "Theme"

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDrawingRect();
            Invalidate(); // Redraw on resize to adjust title positioning
        }


        public void UpdateDrawingRect()
        {

            DrawingRect = new Rectangle(
                BorderThickness + shadowOffset - scrollOffsetX,
                BorderThickness + shadowOffset - scrollOffsetY,
                Width - 2 * (BorderThickness + shadowOffset),
                Height - 2 * (BorderThickness + shadowOffset)
            );

        }

        // Override background painting for optimized repaint
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            SuspendLayout();
            base.OnPaint(e);
            e.Graphics.Clear(BackColor);
            shadowOffset = ShowShadow ? 3 : 0;
            // Define the padded drawing rectangle to leave room for the shadow
            UpdateDrawingRect();

            Rectangle rectangle = new Rectangle(0, 0, Width, Height);
            if (IsChild)
            {
                if (this.Parent != null)
                {
                    parentbackcolor = this.Parent.BackColor;
                    BackColor = parentbackcolor;

                }
            }
           
            if (!_isframless)
            {
                if (ShowShadow && IsShadowAffectedByTheme)
                {
                    DrawShadowUsingRectangle(e.Graphics);
                }

            }

            if (IsChild)
            {
                
                using (SolidBrush brush = new SolidBrush(parentbackcolor))
                {
                    e.Graphics.FillRectangle(brush, DrawingRect);
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
                            using (var brush = new SolidBrush(BackColor))
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
                        using (var brush = new SolidBrush(BackColor))
                        {
                            e.Graphics.FillRectangle(brush, DrawingRect);
                        }
                    }
                }

            }
            if (!_isframless)
            {
                if(ShowAllBorders && BorderThickness == 0)
                {
                    _borderThickness = 1;
                }
                if ((BorderThickness > 0) &&  ShowAllBorders && _isborderaffectedbytheme)
                {
                    DrawBorder(e.Graphics, DrawingRect);
                }


            }

            if (ShowFocusIndicator && Focused)
            {
                DrawFocusIndicator(e.Graphics);
            }
           ResumeLayout();
        }
        protected Font GetScaledFont(Graphics graphics, string text, Size maxSize, Font originalFont)
        {
            Font currentFont = originalFont;
            Size textSize = TextRenderer.MeasureText(graphics, text, currentFont);

            // Set a minimum font size to prevent text from becoming unreadable
            float minFontSize = 6.0f;

            // Reduce font size until text fits within maxSize or reaches minimum font size
            while ((textSize.Width > maxSize.Width || textSize.Height > maxSize.Height) && currentFont.Size > minFontSize)
            {
                currentFont = new Font(currentFont.FontFamily, currentFont.Size - 0.5f, currentFont.Style);
                textSize = TextRenderer.MeasureText(graphics, text, currentFont);
            }

            return currentFont;
        }
        protected void DrawBackColor(PaintEventArgs e,Color color,Color hovercolor)
        {
            if (IsChild)
            {
                //if (this.Parent != null)
                //{
                //    parentbackcolor = this.Parent.BackColor;
                //    BackColor = parentbackcolor;

                //}
                using (SolidBrush brush = new SolidBrush( parentbackcolor))
                {
                    e.Graphics.FillRectangle(brush, DrawingRect);
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
                        using (var brush = new SolidBrush(IsHovered? hovercolor : color))
                        {
                            e.Graphics.FillRectangle(brush, DrawingRect);
                        }
                    }
                }

            }
        }
        protected void DrawBorder(Graphics graphics, Rectangle drawingRect)
        {
            int brder= BorderThickness;
            Color color = BorderColor;
            if (IsHovered)
            {
                color = _currentTheme.HoverLinkColor;
                brder = BorderThickness + 1;
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

                // Draw rounded or regular borders
                if (IsRounded &&  ShowAllBorders)
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
                        graphics.DrawLine(pen, drawingRect.Left, drawingRect.Bottom , drawingRect.Right, drawingRect.Bottom );
                    if (ShowLeftBorder)
                        graphics.DrawLine(pen, drawingRect.Left, drawingRect.Top, drawingRect.Left, drawingRect.Bottom);
                    if (ShowRightBorder)
                        graphics.DrawLine(pen, drawingRect.Right, drawingRect.Top, drawingRect.Right , drawingRect.Bottom);
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
        protected virtual void DrawBorder(Graphics graphics)
        {
            using (var pen = new Pen(BorderColor, BorderThickness))
            {
                pen.DashStyle = _borderDashStyle;
                // Apply border style to the pen
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
                
                // Draw a rounded or standard rectangle border
                if (IsRounded)
                {
                    // Draw a fully rounded border if `IsRounded` is true
                    var rect = new Rectangle(BorderThickness, BorderThickness, Width - 2 * BorderThickness, Height - 2 * BorderThickness);
                    using (GraphicsPath path = GetRoundedRectPath(rect, BorderRadius))
                    {
                        graphics.DrawPath(pen, path);
                    }
                }
                else
                {
                    // For standard borders, draw individual sides based on the border settings
                    if (ShowAllBorders || ShowTopBorder)
                    {
                        graphics.DrawLine(pen, 0, 0, Width, 0); // Top border
                    }
                    if (ShowAllBorders || ShowBottomBorder)
                    {
                        graphics.DrawLine(pen, 0, Height - BorderThickness, Width, Height - BorderThickness); // Bottom border
                    }
                    if (ShowAllBorders || ShowLeftBorder)
                    {
                        graphics.DrawLine(pen, 0, 0, 0, Height); // Left border
                    }
                    if (ShowAllBorders || ShowRightBorder)
                    {
                        graphics.DrawLine(pen, Width - BorderThickness, 0, Width - BorderThickness, Height); // Right border
                    }
                }
            }
        }
        protected virtual void DrawBackground(Graphics graphics)
        {
            if (UseGradientBackground)
            {
                using (var brush = new LinearGradientBrush(ClientRectangle, GradientStartColor, GradientEndColor, GradientDirection))
                {
                    graphics.FillRectangle(brush, ClientRectangle);
                }
            }
            else
            {
                using (var brush = new SolidBrush(BackColor))
                {
                    graphics.FillRectangle(brush, ClientRectangle);
                }
            }
        }
        protected void DrawShadowUsingRectangle(Graphics graphics)
        {
            // Ensure shadow is drawn only if it's enabled and the control is not transparent
            if (ShowShadow ) // Ensure no transparency conflicts
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb((int)(255 * ShadowOpacity), ShadowColor)))
                {
                    // Calculate an offset shadow rectangle slightly larger than the main control
                    //Rectangle shadowRect = new Rectangle(
                    //    DrawingRect.Left+BorderThickness +shadowOffset,
                    //    DrawingRect.Top + BorderThickness + shadowOffset,
                    //    DrawingRect.Width + BorderThickness+shadowOffset,
                    //    DrawingRect.Height + BorderThickness+shadowOffset
                    //);
                    Rectangle shadowRect = new Rectangle(
                       DrawingRect.Left + BorderThickness + shadowOffset,
                       DrawingRect.Top + BorderThickness + shadowOffset,
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
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            BorderColor = _currentTheme.HoverLinkColor;
            IsHovered = true;
            ShowToolTipIfExists();
            //Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            BorderColor = _currentTheme.BorderColor;
            IsPressed = false;
            IsFocused = false;
            IsHovered=false;
            HideToolTip(); // Hide tooltip on mouse leave
           // Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
           // Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
           // Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            IsPressed = true;
            base.OnClick(e);
            IsPressed=false;
          //  Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            BorderColor = _currentTheme.ActiveBorderColor;
            if (e.Button == MouseButtons.Left)
            {
                IsPressed = true;
                IsFocused = true;

            }
            IsPressed = false;
           // Invalidate();
        }
        #region "Animation"
        // Default Animation Properties
      
        /// <summary>
        /// Shows the control with the specified animation.
        /// </summary>
        /// 

        public void ShowWithAnimation( DisplayAnimationType animationType,Control parentControl = null)
        {
            AnimationType = animationType;
            if (AnimationType == DisplayAnimationType.None)
            {
                Visible = true;
                return;
            }
            if (Visible==false)
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
                progress = ApplyEasing( progress);

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


        public Size GetSize()

        {
            return new Size(Width, Height);

        }
      

        public virtual void Print(Graphics graphics)
        {
            // Draw the control on the provided graphics object
            OnPrint(new PaintEventArgs(graphics, ClientRectangle));
        }

        public void ShowToolTip(string text)
        {
            ToolTipText = text;
            if (!string.IsNullOrEmpty(ToolTipText))
            {
                _toolTip.Show(ToolTipText, this, PointToClient(MousePosition), 3000); // Show tooltip for 3 seconds
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

        #region "Util"
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
        public Size GetSuitableSizeForTextandImage(Size imageSize,Size MaxImageSize, TextImageRelation TextImageRelation)
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
        #endregion "Util"
    }

}

