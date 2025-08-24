using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Models;
using ContentAlignment = System.Drawing.ContentAlignment;
using TextImageRelation = System.Windows.Forms.TextImageRelation;
using Timer = System.Windows.Forms.Timer;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Button))]
    [Category("Controls")]
    [Description("A button control with an image and text.")]
    [DisplayName("Beep Button")]
    public class BeepButton : BeepControl
    {
        #region "Properties"
        // Add these new fields to your BeepButton class (in your region "Properties" or near other private fields):

        private Timer splashTimer;
        private float splashProgress; // 0.0f to 1.0f value representing animation progress
        private Point splashCenter;   // The point at which the click occurred (local coordinates)
        private bool splashActive=false;    // Whether the splash animation is currently active

        // Constants to control the animation speed and size:
        private const float SplashSpeed = 0.05f;     // Increase in progress per tick (adjust as needed)
        private const float MaxSplashRadius = 150f;  // Maximum radius for the splash effect

        private BeepImage beepImage;
        private int borderSize = 1;

        private Color borderColor = Color.Black;
        private Color selectedBorderColor = Color.Blue;
        private bool _isStillButton = false;

        private TextImageRelation textImageRelation = TextImageRelation.ImageBeforeText;
        private ContentAlignment imageAlign = ContentAlignment.MiddleLeft;
        private ContentAlignment textAlign = ContentAlignment.MiddleCenter;
        private bool _isSelected = false;
        private Size _maxImageSize = new Size(32, 32); // Default max image size
      //  private FlatStyle _flatStyle = FlatStyle.Standard;
        private bool _flatAppearanceEnabled = true;

        private bool _isSideMenuChild = false;
        // Private field to store the button's text

        private Rectangle contentRect;
      
        // create a public event for beepImage.onClick
        public EventHandler<BeepEventDataArgs> ImageClicked { get; set; }

        private ButtonType _buttonType = ButtonType.Normal;
        [Browsable(true)]
        [Category("Appearance")]
        public ButtonType ButtonType
        {
            get => _buttonType;
            set
            {
                _buttonType = value;
                Invalidate();  // Trigger repaint
            }
        }

        private Color tmpbackcolor;
        private Color tmpforcolor;

        private bool _isColorFromTheme = true;
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsColorFromTheme
        {
            get => _isColorFromTheme;
            set
            {
                _isColorFromTheme = value;
               
                Invalidate();  // Trigger repaint
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public Color SplashColor { get; set; } = Color.Gray;
        #region "Long press properties"
        private Timer longPressTimer;
        private bool isLongPressTriggered = false;
        private const int LongPressThreshold = 500; // milliseconds

        public event EventHandler LongPress;
        public event EventHandler DoubleClickAction;

        #endregion "Long press properties"

        #region "Popup List Properties"
        //  private BeepPopupForm _popupForm;
        // BeepPopupListForm menuDialog;
        private Color tmpfillcolor;
        private Color tmpstrokecolor;
        private bool _isPopupOpen;
        private bool _popupmode = false;
        private int _maxListHeight = 100;
        private int _maxListWidth = 100;
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool PopupMode
        {
            get => _popupmode;
            set
            {
                _popupmode = value;
            }
        }
        BeepPopupListForm menuDialog;
        // popup list items form
        [Browsable(false)]
        public BeepPopupListForm PopupListForm
        {
            get => menuDialog;
            set => menuDialog = value;
        }

        private BindingList<SimpleItem> _listItems = new BindingList<SimpleItem>();

        [Browsable(true)
        ]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => _listItems;
            set => _listItems = value;
        }
        // The item currently chosen by the user
        private SimpleItem _selectedItem;
        private int _selectedItemIndex;
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
             set
            {
                _selectedItem = value;
                _selectedItemIndex = _listItems.IndexOf(_selectedItem);
                OnSelectedItemChanged(_selectedItem); //
             

            }
        }
        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedItemIndex;
            set
            {
                if (value == null) return;
                if (value >= 0 && value < _listItems.Count)
                {
                    SelectedItem = _listItems[value];

                }
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set
            {
                _isPopupOpen = value;
                if (menuDialog != null)
                {
                    if (_isPopupOpen)
                    {
                        ShowPopup();
                    }
                    else
                    {
                        ClosePopup();
                    }
                }
      
            }
        }
        private BeepPopupFormPosition _beepPopupFormPosition= BeepPopupFormPosition.Bottom;
        [Browsable(true)]
        [Category("Appearance")]
      
        public BeepPopupFormPosition PopPosition
        {
            get { return _beepPopupFormPosition; }
            set { _beepPopupFormPosition = value; }

        }
        #endregion "Popup List Properties"
        private List<SimpleItem> _standardimages = new List<SimpleItem>();
        public List<SimpleItem> StandardImages
        {
            get => _standardimages;
            set
            {
                _standardimages = value;
                Invalidate();  // Trigger repaint
            }
        }
        private ImageEmbededin _imageEmbededin = ImageEmbededin.Button;
        [Category("Appearance")]
        [Description("Indicates where the image is embedded.")]
        public ImageEmbededin ImageEmbededin
        {
            get => _imageEmbededin;
            set
            {
                _imageEmbededin = value;
                beepImage.ImageEmbededin = value;
                Invalidate();
            }
        }
        private bool _useScaledfont = false;
        [Browsable(true)]
        [Category("Appearance")]
        public bool UseScaledFont
        {
            get => _useScaledfont;
            set
            {
                _useScaledfont = value;
                Invalidate();  // Trigger repaint
            }
        }
     
        [Browsable(true)]
        [Category("Appearance")]
        public int BorderSize
        {
            get => borderSize;
            set
            {
                borderSize = value;
                Invalidate();  // Trigger repaint
            }
        }
        bool _applyThemeOnImage = false;
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                beepImage.ApplyThemeOnImage = value;
                if (value)
                {

                    if (ApplyThemeOnImage)
                    {
                        tmpfillcolor = beepImage.FillColor;
                        tmpstrokecolor = beepImage.StrokeColor;
                        beepImage.Theme = Theme;
                    }
                }
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsSideMenuChild
        {
            get => _isSideMenuChild;
            set
            {
                _isSideMenuChild = value;
                Invalidate();  // Trigger repaint
            }

        }
        //[Browsable(true)]
        //[Category("Appearance")]
        //public Color BorderColor
        //{
        //    get => borderColor;
        //    set
        //    {
        //        borderColor = value;
        //        Invalidate();  // Trigger repaint
        //    }
        //}
        //[Browsable(true)]
        //[Category("Appearance")]
        //public Color SelectedBorderColor
        //{
        //    get => selectedBorderColor;
        //    set
        //    {
        //        selectedBorderColor = value;
        //        Invalidate();  // Trigger repaint
        //    }
        //}
        [Browsable(true)]
        [Category("Appearance")]
        public TextImageRelation TextImageRelation
        {
            get => textImageRelation;
            set
            {
                textImageRelation = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public ContentAlignment ImageAlign
        {
            get => imageAlign;
            set
            {
                imageAlign = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public ContentAlignment TextAlign
        {
            get => textAlign;
            set
            {
                textAlign = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        public string ImagePath
        {
            get => beepImage?.ImagePath;
            set
            {

                if (beepImage != null)
                {
                    beepImage.ImagePath = value;
                    beepImage.ScaleMode = ImageScaleMode.KeepAspectRatio;
                    if (ApplyThemeOnImage)
                    {
                        beepImage.Theme = Theme;
                        beepImage.ApplyThemeToSvg();
                        beepImage.ApplyTheme();
                    }
                    Invalidate(); // Repaint when the image changes
                                  // UpdateSize();
                }
            }
        }
        // New Properties
        [Browsable(true)]
        [Category("Appearance")]
        public Image Image
        {
            get => beepImage.Image;
            set
            {
                beepImage.Image = value;

                ApplyTheme();
                Invalidate();  // Trigger repaint
            }
        }
        //[Browsable(true)]
        //[Category("Behavior")]
        //public bool IsSelected
        //{
        //    get => _isSelected;
        //    set
        //    {
        //        _isSelected = value;
        //        if (_isSelected)
        //        {
                    
        //            BackColor = _currentTheme.ButtonSelectedBackColor;
        //            ForeColor = _currentTheme.ButtonPressedForeColor;
        //        }
        //        else
        //        {
        //            BackColor = _currentTheme.ButtonBackColor;
        //            ForeColor = _currentTheme.ButtonForeColor;
        //        }
        //        Invalidate(); // Repaint to reflect selection state
        //    }
        //}
        [Browsable(true)]
        [Category("Appearance")]
        public Size MaxImageSize
        {
            get => _maxImageSize;
            set
            {
                _maxImageSize = value;
                Invalidate(); // Repaint when the max image size changes
            }
        }
       
        // New Property for Hover Persistence using Theme
        [Browsable(true)]
        [Category("Behavior")]
        public bool IsStillButton
        {
            get => _isStillButton;
            set
            {
                _isStillButton = value;
                Invalidate(); // Trigger repaint when the state changes
            }
        }
        private bool _hideText = false;
        [Browsable(true)]
        [Category("Behavior")]
        public bool HideText
        {
            get => _hideText;
            set
            {
                _hideText = value;
                Invalidate(); // Trigger repaint when the state changes
            }
        }
        private bool _autoSize = false;
        [Browsable(true)]
        [Category("Behavior")]
        public bool AutoSize
        {
            get => _autoSize;
            set
            {
                _autoSize = value;
                Invalidate(); // Trigger repaint when the state changes
                PerformLayout();
            }
        }

           
        private Font _textFont = new Font("Arial", 10);
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new bool IsChild
        {
            get => _isChild;
            set
            {
                _isChild = value;
                base.IsChild = value;
                //Control parent = null;
                //if (this.Parent != null)
                //{
                //    parent = this.Parent;
                //}
                //if (parent != null)
                //{
                //    if (value)
                //    {
                //        parentbackcolor = parent.BackColor;
                //        _tempbackcolor = BackColor;
                //        BackColor = parentbackcolor;
                //        beepImage.BackColor = parentbackcolor;
                //    }
                //    else
                //    {
                       
                //        beepImage.BackColor = _tempbackcolor;
                //        BackColor = _tempbackcolor;
                //        ApplyTheme();
                //    }
                //}
               
                Invalidate();  // Trigger repaint
            }
        }
        private ControlHitTest beepImageHitTest;
        private Color _originalForColor;

        #endregion "Properties"
        #region "Constructor"
        // Constructor
        public BeepButton():base()
        {
            

            InitializeComponents();
            SetStyle(ControlStyles.StandardDoubleClick, true);

            longPressTimer = new Timer { Interval = LongPressThreshold };
            longPressTimer.Tick += (s, e) =>
            {
                longPressTimer.Stop();
                isLongPressTriggered = true;
                LongPress?.Invoke(this, EventArgs.Empty);
            };

            // Enable modern styling by default for professional appearance
            BorderRadius = 8;
            CanBeHovered = true;
            CanBePressed = true;
            CanBeFocused = true;
            
            // Enable modern gradient background for stylish look
            UseGradientBackground = false;
            ModernGradientType = ModernGradientType.Linear;
          //  ShowShadow = true;
            IsRounded = true;
            
            // CRITICAL: Set border thickness to 0 for modern flat button look
            BorderThickness = 0;
            ShowAllBorders = false;
            
            // DON'T set hardcoded gradient colors - let ApplyTheme() handle them from the theme
            
            #region "Popup List Initialization"
            IsChild= false;
            // Initialize the popup form and beepListBox
            // 1) Create beepListBox
           
            #endregion "Popup List Initialization"
            splashTimer = new Timer();
            splashTimer.Interval = 30; // Update every 30 ms (about 33 frames per second)
            splashTimer.Tick += SplashTimer_Tick;

        }
        private void InitializeComponents()
        {

            beepImage = new BeepImage
            {
                IsChild = true,
                Dock = DockStyle.None, // We'll manually position it
                Margin = new Padding(0),
                Location = new Point(0, 0), // Set initial position (will adjust in layout)
                Size = _maxImageSize // Set the size based on the max image size
            };
            beepImage.ImageEmbededin= ImageEmbededin.Button;
          //  beepImage.MouseHover += BeepImage_MouseHover;
         //   beepImage.MouseEnter += BeepImage_MouseEnter;
            //   beepImage.MouseLeave += BeepImage_MouseLeave;
            beepImage.MouseDown += BeepImage_MouseDown;
            Padding = new Padding(0);
            Margin = new Padding(0);
         
            ShowAllBorders = false;
            //  InitListbox();
            //  Controls.Add(beepImage);
        }
        protected override Size DefaultSize => new Size(100, 36);
        #endregion "Constructor"
        #region "Popup List Methods"


        private void TogglePopup()
        {
            if (_isPopupOpen)
                ClosePopup();
            else
                ShowPopup();
        }
        public void ShowPopup()
        {
            if (_isPopupOpen) return;
            if (ListItems.Count == 0)
            {
                return;
            }

            // Close any existing popup before showing a new one
            ClosePopup();
            Debug.WriteLine("1");
            menuDialog = new BeepPopupListForm(ListItems.ToList());
            Debug.WriteLine("1.1");
            menuDialog.Theme = Theme;
            menuDialog.SelectedItemChanged += MenuDialog_SelectedItemChanged;
            // Use a timer to call debug after form is fully rendered
            Debug.WriteLine("1.2");
            // Use the synchronous ShowPopup method
            SimpleItem x = menuDialog.ShowPopup(Text, this, _beepPopupFormPosition);
            _isPopupOpen = true;
            Invalidate();
        }
        public IErrorsInfo RunMethodFromGlobalFunctions(SimpleItem item, string MethodName)
        {
            ErrorsInfo errorsInfo = new ErrorsInfo();
            try
            {
                HandlersFactory.RunMethodFromObjectHandler(item, MethodName);

            }
            catch (Exception ex)
            {
                errorsInfo.Flag = Errors.Failed;
                errorsInfo.Message = ex.Message;
                errorsInfo.Ex = ex;
            }
            return errorsInfo;

        }
        private void MenuDialog_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
          SelectedItem = e.SelectedItem;
          if(SelectedItem.Children.Count==0)
            {
                ClosePopup();
            }
            else
            {
                if(SelectedItem.MethodName != null)
                {
                    RunMethodFromGlobalFunctions(SelectedItem, SelectedItem.MethodName);
                }
               
            }
             

        }
        public void ClosePopup()
        {

            if (!_isPopupOpen) return;

            if (menuDialog != null)
            {
                menuDialog.SelectedItemChanged -= MenuDialog_SelectedItemChanged;
                menuDialog.CloseCascade();
              //  menuDialog.Close();
                menuDialog.Dispose();
                menuDialog = null;
            }
            _isPopupOpen = false;
            Invalidate();
        }
        #endregion "Popup List Methods"
        #region "Theme"
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textFont = Font;
          // // Console.WriteLine("Font Changed");
            if (AutoSize)
            {
                Size textSize = TextRenderer.MeasureText(Text, _textFont);
                this.Size = new Size(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
            }
        }
        public override void ApplyTheme()
        {
            // CRITICAL: Call base.ApplyTheme() first to ensure proper DPI scaling handling
           // base.ApplyTheme();

            // Store whether colors should be from theme for later restoration
            _isColorFromTheme = true;

            // Handle parent background inheritance for child controls
            if (IsChild && Parent != null)
            {
                BackColor = Parent.BackColor;
                ParentBackColor = Parent.BackColor;
            }
            else
            {
                // Apply default button background color
                BackColor = _currentTheme.ButtonBackColor;
            }

            // Store original colors for state management
            _originalBackColor = BackColor;
            ForeColor = _currentTheme.ButtonForeColor;
            _originalForColor = ForeColor;

            // Apply all button state colors
            HoverBackColor = _currentTheme.ButtonHoverBackColor;
            HoverForeColor = _currentTheme.ButtonHoverForeColor;
            SelectedBackColor = _currentTheme.ButtonSelectedBackColor;
            SelectedForeColor = _currentTheme.ButtonSelectedForeColor;
            PressedBackColor = _currentTheme.ButtonPressedBackColor;
            PressedForeColor = _currentTheme.ButtonPressedForeColor;
            DisabledBackColor = _currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;
            FocusBackColor = _currentTheme.ButtonSelectedBackColor;
            FocusForeColor = _currentTheme.ButtonSelectedForeColor;

            // Apply border colors
            BorderColor = _currentTheme.ButtonBorderColor;

            // Apply gradient colors from theme if gradient is enabled
            if (UseGradientBackground)
            {
                // Use standard gradient colors from theme
                GradientStartColor = _currentTheme.GradientStartColor;
                GradientEndColor = _currentTheme.GradientEndColor;

                // Apply gradient direction if available
                if (_currentTheme.GradientDirection != LinearGradientMode.Horizontal)
                {
                    GradientDirection = _currentTheme.GradientDirection;
                }
            }

            // Apply font from theme if configured to use theme fonts
            if (UseThemeFont)
            {
                // Get font from button style or fall back to default style
                _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                    

            }
          //  SafeApplyFont(TextFont ?? _textFont);
            
            // Apply theme to child image control
            if (beepImage != null)
            {
                beepImage.BackColor = BackColor;
                beepImage.Theme = Theme;
                beepImage.IsChild = true;
                beepImage.ParentBackColor = BackColor;

                // Apply SVG theming if configured
                beepImage.ApplyThemeOnImage = ApplyThemeOnImage;
            }

            // Apply SVG theming if enabled
            ApplyThemeToSvg();

            // Handle popup list theming
            if (_popupmode && menuDialog != null && !menuDialog.IsDisposed)
            {
                menuDialog.Theme = Theme;
            }

            // Force redraw with new theme - base.ApplyTheme() already calls Invalidate()
            // so we don't need to call it again to avoid double invalidation
        }
        public void ApplyThemeToSvg()
        {

            if (beepImage != null) // Safely apply theme to beepImage
            {
                beepImage.ApplyThemeOnImage = ApplyThemeOnImage;
                if (ApplyThemeOnImage)
                {
                 if(ImageEmbededin== ImageEmbededin.Button)
                    {
                        beepImage.Theme = Theme;
                    }
                   
                    beepImage.ApplyThemeToSvg();
                }

            }


        }
        #endregion "Theme"
        #region "Paint"
        protected override void InitLayout()
        {
            base.InitLayout();
            // Initialize the layout of the control
           
        }
      
     
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Prevent default background painting.
            // Optionally, if you need a background color use:
            // pevent.Graphics.Clear(BackColor);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
          //  SuspendLayout();
            base.OnPaint(e);

         //   e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
         //   //    ResumeLayout();
        }
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            
           
            g.ResetTransform();
            // Let the base BeepControl handle modern styling (gradients, shadows, rounded corners)
            // This draws the beautiful gradient background, shadows, and rounded corners
           

        
            contentRect = DrawingRect;
            DrawImageAndText(g);
            // Draw button content (image and text) over the styled background
            //switch (ButtonType)
            //{
            //    case ButtonType.Normal:
            //        // Normal button with image and text
            //        DrawImageAndText(g);
            //        break;
            //    case ButtonType.AnimatedArrow:
            //        // Image-only button
            //        DrawButtonAndImage1(g);
            //        break;
            //    case ButtonType.ExpandingIcon:
            //        DrawButtonAndImage2(g);
            //        // Text-only button
            //        break;
            //        case ButtonType.SlidingArrow:
            //        DrawButtonAndImage3(g);
            //        break;
            //        case ButtonType.SlidingBackground:
            //        DrawButtonAndImage4(g);
            //        break;

            //}
            

            // Draw splash effect if active (Material Design ripple) - AFTER base styling
            if (splashActive)
            {
                DrawSplashEffect(g);
            }

            // Apply very subtle state overlays for better visual feedback (much more subtle)
            DrawStateOverlays(g);
            g.ResetClip();
        }

        private void DrawSplashEffect(Graphics g)
        {
            if (!splashActive) return;

            // Calculate the current radius based on the animation progress
            float currentRadius = splashProgress * MaxSplashRadius;
            // Compute an alpha value so that the ripple fades out
            int alpha = (int)((1f - splashProgress) * 150);
            alpha = Math.Max(0, Math.Min(255, alpha));

            using (SolidBrush rippleBrush = new SolidBrush(Color.FromArgb(alpha, SplashColor)))
            {
                // Calculate the rectangle for the splash circle centered on splashCenter
                Rectangle rippleRect = new Rectangle(
                    (int)(splashCenter.X - currentRadius),
                    (int)(splashCenter.Y - currentRadius),
                    (int)(2 * currentRadius),
                    (int)(2 * currentRadius));

                // Clip the drawing to the button bounds for clean edges
                if (IsRounded && BorderRadius > 0)
                {
                    using (GraphicsPath clipPath = GetRoundedRectPath(contentRect, BorderRadius))
                    {
                        g.SetClip(clipPath);
                        g.FillEllipse(rippleBrush, rippleRect);
                        g.ResetClip();
                    }
                }
                else
                {
                    g.SetClip(contentRect);
                    g.FillEllipse(rippleBrush, rippleRect);
                    g.ResetClip();
                }
            }
        }

        private void DrawStateOverlays(Graphics g)
        {
            if (!Enabled) return;

            Color overlayColor = Color.Empty;
            int alpha = 0;

            // Much more subtle overlays that don't interfere with gradients
            if (IsPressed)
            {
                // Very subtle pressed state overlay
                overlayColor = Color.Black;
                alpha = 15; // Reduced from 30 for more subtlety
            }
            else if (IsHovered)
            {
                // Very subtle hover state overlay
                overlayColor = Color.White;
                alpha = 8; // Reduced from 15 for more subtlety
            }

            // Only apply overlay if we have a valid state
            if (alpha > 0)
            using (SolidBrush overlayBrush = new SolidBrush(Color.FromArgb(alpha, overlayColor)))
            {
                if (IsRounded && BorderRadius > 0)
                {
                    using (GraphicsPath path = GetRoundedRectPath(contentRect, BorderRadius))
                    {
                        g.FillPath(overlayBrush, path);
                    }
                }
                else
                {
                    g.FillRectangle(overlayBrush, contentRect);
                }
            }
        }
    
        private void CalculateLayout(Rectangle contentRect, Size imageSize, Size textSize, out Rectangle imageRect, out Rectangle textRect)
        {
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;

            bool hasImage = imageSize != Size.Empty;
            bool hasText = !string.IsNullOrEmpty(Text) && !HideText; // Check if text is available and not hidden

            // Adjust contentRect for padding
            contentRect.Inflate(- 2, - 2);

            if (hasImage && !hasText)
            {
                // Center image in the button if there is no text
                imageRect = AlignRectangle(contentRect, imageSize, ContentAlignment.MiddleCenter);
            }
            else if (hasText && !hasImage)
            {
                // Only text is present, align text within the button
                textRect = AlignRectangle(contentRect, textSize, TextAlign);
            }
            else if (hasImage && hasText)
            {
                // Layout logic based on TextImageRelation when both text and image are present
                switch (this.TextImageRelation)
                {
                    case TextImageRelation.Overlay:
                        // ImagePath and text overlap
                        imageRect = AlignRectangle(contentRect, imageSize, ImageAlign);
                        textRect = AlignRectangle(contentRect, textSize, TextAlign);
                        break;

                    case TextImageRelation.ImageBeforeText:
                        imageRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, imageSize.Width, contentRect.Height), imageSize, ImageAlign);
                        textRect = AlignRectangle(new Rectangle(contentRect.Left + imageSize.Width + Padding.Horizontal, contentRect.Top, contentRect.Width - imageSize.Width - Padding.Horizontal, contentRect.Height), textSize, TextAlign);
                        break;

                    case TextImageRelation.TextBeforeImage:
                        textRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, textSize.Width, contentRect.Height), textSize, TextAlign);
                        imageRect = AlignRectangle(new Rectangle(contentRect.Left + textSize.Width + Padding.Horizontal, contentRect.Top, contentRect.Width - textSize.Width - Padding.Horizontal, contentRect.Height), imageSize, ImageAlign);
                        break;

                    case TextImageRelation.ImageAboveText:
                        imageRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, contentRect.Width, imageSize.Height), imageSize, ImageAlign);
                        textRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top + imageSize.Height + Padding.Vertical, contentRect.Width, contentRect.Height - imageSize.Height - Padding.Vertical), textSize, TextAlign);
                        break;

                    case TextImageRelation.TextAboveImage:
                        textRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top, contentRect.Width, textSize.Height), textSize, TextAlign);
                        imageRect = AlignRectangle(new Rectangle(contentRect.Left, contentRect.Top + textSize.Height + Padding.Vertical, contentRect.Width, contentRect.Height - textSize.Height - Padding.Vertical), imageSize, ImageAlign);
                        break;
                }
            }
        }
        private Rectangle AlignRectangle(Rectangle container, Size size, ContentAlignment alignment)
        {
            int x = 0;
            int y = 0;

            // Horizontal alignment
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    x = container.X;
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    x = container.X + (container.Width - size.Width) / 2;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    x = container.Right - size.Width;
                    break;
            }

            // Vertical alignment
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    y = container.Y;
                    break;
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    y = container.Y + (container.Height - size.Height) / 2;
                    break;
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    y = container.Bottom - size.Height;
                    break;
            }

            return new Rectangle(new Point(x, y), size);
        }
        private TextFormatFlags GetTextFormatFlags(ContentAlignment alignment)
        {
            TextFormatFlags flags = TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsClipping;

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                    flags |= TextFormatFlags.Left | TextFormatFlags.Top;
                    break;
                case ContentAlignment.TopCenter:
                    flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.Top;
                    break;
                case ContentAlignment.TopRight:
                    flags |= TextFormatFlags.Right | TextFormatFlags.Top;
                    break;
                case ContentAlignment.MiddleLeft:
                    flags |= TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
                    break;
                case ContentAlignment.MiddleCenter:
                    flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                    break;
                case ContentAlignment.MiddleRight:
                    flags |= TextFormatFlags.Right | TextFormatFlags.VerticalCenter;
                    break;
                case ContentAlignment.BottomLeft:
                    flags |= TextFormatFlags.Left | TextFormatFlags.Bottom;
                    break;
                case ContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom;
                    break;
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Right | TextFormatFlags.Bottom;
                    break;
            }

            return flags;
        }
        public override Size GetPreferredSize(Size proposedSize)
        {
            //if (!SetFont() && UseThemeFont)
            //{
            //    _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            //};
            // Measure and scale the font to fit within the control bounds

            Size textSize = TextRenderer.MeasureText(Text, _textFont);
            Size imageSize = beepImage?.HasImage == true ? beepImage.GetImageSize() : Size.Empty;

            // Scale the image to respect MaxImageSize if needed
            if (imageSize.Width > _maxImageSize.Width || imageSize.Height > _maxImageSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)_maxImageSize.Width / imageSize.Width,
                    (float)_maxImageSize.Height / imageSize.Height);
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }
            // Use a large enough virtual container for layout testing
            //  Rectangle virtualContent = new Rectangle(0, 0, 1000, 1000);
            Rectangle textRect, imageRect;
            CalculateLayout(DrawingRect, imageSize, textSize, out imageRect, out textRect);
            // Clip text rectangle to control bounds to prevent overflow
            //  textRect.Intersect(DrawingRect);
            // Calculate the total width and height required for text and image with padding
            // Calculate bounding size (not offset-based!)
            Rectangle bounds = Rectangle.Union(imageRect, textRect);
            int width = bounds.Width + Padding.Left + Padding.Right;
            int height = bounds.Height + Padding.Top + Padding.Bottom;

            return new Size(width, height);
            //  }

            // Return the control's current size if AutoSize is disabled
            //return base.Value;
        }
        #endregion "Paint"
        #region "Mouse and Click"
        //private void BeepImage_MouseHover(object? sender, EventArgs e)
        //{
        //   IsHovered = true;
        //    base.OnMouseHover(e);

        //}
        //private void BeepImage_MouseEnter(object? sender, EventArgs e)
        //{

        //    IsHovered = true;
        //    base.OnMouseEnter(e);
        //}
        private void BeepImage_MouseDown(object? sender, MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (IsSelectedOptionOn)
            {
                IsSelected = !IsSelected;
            }
            var ev = new BeepEventDataArgs("ImageClicked", this);

            ImageClicked?.Invoke(this, ev);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
           
            isLongPressTriggered = false;
            longPressTimer.Start();
            
            // Enable splash effect by default for modern Material Design feel
            splashActive = true;
            splashCenter = e.Location; // Use the mouse click location for the ripple's center
            splashProgress = 0f;
            splashTimer.Start();
           
            if (IsSelectedOptionOn)
            {
                IsHovered = false;
                IsPressed = false;
                IsSelected = !IsSelected;

            }
            else
            {
                IsSelected = false;
            }

            if (_popupmode)
            {
                TogglePopup();
            }
            else if (beepImageHitTest != null && beepImageHitTest.TargetRect.Contains(e.Location))
            {
                // Image clicked, handled by BeepImage_MouseDown
            }
           
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
          
            longPressTimer.Stop();
        }
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            DoubleClickAction?.Invoke(this, EventArgs.Empty);
        }
        #endregion "Mouse and Click"
        #region "Binding and Control Type"
        public DbFieldCategory Category { get; set; } = DbFieldCategory.Boolean;

        public void SetBinding(string controlProperty, string dataSourceProperty)
        {
            // Implementation for setting up data binding
            this.DataBindings.Add(controlProperty, DataContext, dataSourceProperty, true, DataSourceUpdateMode.OnPropertyChanged);
        }
        #endregion "Binding and Control Type"
        #region Splash Animation
        // Add the timer event handler method:
        private void SplashTimer_Tick(object sender, EventArgs e)
        {
            splashProgress += SplashSpeed; // Increase the progress
            if (splashProgress >= 1f)
            {
                splashTimer.Stop();
                splashActive = false;
                splashProgress = 0f;
            }
            Invalidate(); // Request the control be redrawn
        }
        #endregion Splash Animation
        #region "Badge"
        // after you change BadgeText, or on Resize, or on LocationChanged:
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
          
        }

        // if you have a BadgeText property override:
        public override string BadgeText
        {
            get => base.BadgeText;
            set
            {
                base.BadgeText = value;
                UpdateRegionForBadge();
                // tell parent to repaint
                //if (Parent is BeepControl bc) bc.Invalidate(this.Bounds);
            }
        }
        // 2) Whenever our handle is created or resized, recalc the hole
    

      

        private BeepControl _lastBeepParent;
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            // unregister from old parent
            if (_lastBeepParent != null)
                _lastBeepParent.ClearChildExternalDrawing(this);

            // register with new parent
            if (Parent is BeepControl newBeepParent)
            {
                newBeepParent.AddChildExternalDrawing(
                    this,
                    DrawBadgeExternally,
                    DrawingLayer.AfterAll
                );
            }

            _lastBeepParent = Parent as BeepControl;
        }


        #endregion "Badge"
        #region "Draw Image and text"

        public override void Draw(Graphics g, Rectangle rectangle)
        {
            contentRect = rectangle;
            DrawImageAndText(g);
        }
        private void DrawImageAndText(Graphics g)
        {
            Color textColor;

            // Determine text color based on current state and theme settings
            if (_isColorFromTheme)
            {
                textColor = _originalForColor;
            }
            else
            {
                textColor = ForeColor;
            }

            // Update text color based on button state for better visibility
            if (Enabled)
            {
                if (IsHovered)
                {
                    textColor = HoverForeColor;
                }
                else if (IsSelected)
                {
                    textColor = SelectedForeColor;
                }
                else if (IsPressed)
                {
                    textColor = PressedForeColor;
                }
            }
            else
            {
                textColor = DisabledForeColor;
            }

            // Use scaled font if configured
            Font scaledFont = _textFont;
            if (UseScaledFont)
            {
                scaledFont = GetScaledFont(g, Text, contentRect.Size, _textFont);
            }

            Size imageSize = beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;

            // Update image size to fit new size of control
            if (beepImage != null)
            {
                if (_maxImageSize.Height > this.Height)
                {
                    beepImage.Height = this.Height - 4;
                    _maxImageSize.Height = this.Height - 4;
                }
                if (_maxImageSize.Width > this.Width)
                {
                    beepImage.Width = this.Width - 4;
                    _maxImageSize.Width = this.Width - 4;
                }
            }

            // Limit image size to MaxImageSize
            if (imageSize.Width > _maxImageSize.Width || imageSize.Height > _maxImageSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)_maxImageSize.Width / imageSize.Width,
                    (float)_maxImageSize.Height / imageSize.Height);
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            Size textSize = TextRenderer.MeasureText(Text, scaledFont);

            // Calculate the layout of image and text
            Rectangle imageRect, textRect;
            CalculateLayout(contentRect, imageSize, textSize, out imageRect, out textRect);

            // Draw the image if available
            if (beepImage != null && beepImage.HasImage)
            {
                if (beepImage.Size.Width > this.Size.Width || beepImage.Size.Height > this.Size.Height)
                {
                    imageSize = this.Size;
                }
                beepImage.MaximumSize = imageSize;
                beepImage.Size = imageRect.Size;
                if (ApplyThemeOnImage)
                {
                    beepImage.Theme = Theme;
                    beepImage.ApplyTheme();
                }
                beepImage.DrawImage(g, imageRect);

                if (beepImageHitTest == null)
                {
                    beepImageHitTest = new ControlHitTest(imageRect, Point.Empty)
                    {
                        Name = "BeepImageRect",
                        ActionName = "ImageClicked",
                        HitAction = () =>
                        {
                            // Raise your ImageClicked event
                            var ev = new BeepEventDataArgs("ImageClicked", this);
                            ImageClicked?.Invoke(this, ev);
                        }
                    };
                }
                else
                {
                    beepImageHitTest.TargetRect = imageRect;
                }

                AddHitTest(beepImageHitTest);
            }

            // Draw text if available and not hidden
            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                TextFormatFlags flags = GetTextFormatFlags(TextAlign);

                // Use high-quality text rendering for professional appearance
                using (var textBrush = new SolidBrush(textColor))
                {
                    // For better text rendering on modern buttons
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    TextRenderer.DrawText(g, Text, scaledFont, textRect, textColor, flags);
                }
            }
        }
        #region "Draw Button From Html source"
        // all image and svg drawing is done in the BeepImage control
        // all functions will be similar to the orginal DrawImageAndText control but will have different style based on html .
      
        public void DrawButtonAndImage1(Graphics g)
        {
            Color textColor;

            // Determine text color based on current state and theme settings (same as original)
            if (_isColorFromTheme)
            {
                textColor = _originalForColor;
            }
            else
            {
                textColor = ForeColor;
            }

            // Update text color based on button state for better visibility (same as original)
            if (Enabled)
            {
                if (IsHovered)
                {
                    textColor = HoverForeColor;
                }
                else if (IsSelected)
                {
                    textColor = SelectedForeColor;
                }
                else if (IsPressed)
                {
                    textColor = PressedForeColor;
                }
            }
            else
            {
                textColor = DisabledForeColor;
            }

            // Use scaled font if configured (same as original)
            Font scaledFont = _textFont;
            if (UseScaledFont)
            {
                scaledFont = GetScaledFont(g, Text, contentRect.Size, _textFont);
            }

            // HTML-style font override for modern look
            Font htmlFont = new Font("Segoe UI", 12f, FontStyle.Regular);

            Size imageSize = beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;

            // Update image size to fit new size of control (same as original)
            if (beepImage != null)
            {
                if (_maxImageSize.Height > this.Height)
                {
                    beepImage.Height = this.Height - 4;
                    _maxImageSize.Height = this.Height - 4;
                }
                if (_maxImageSize.Width > this.Width)
                {
                    beepImage.Width = this.Width - 4;
                    _maxImageSize.Width = this.Width - 4;
                }
            }

            // Limit image size to MaxImageSize (same as original)
            if (imageSize.Width > _maxImageSize.Width || imageSize.Height > _maxImageSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)_maxImageSize.Width / imageSize.Width,
                    (float)_maxImageSize.Height / imageSize.Height);
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            Size textSize = TextRenderer.MeasureText(Text, htmlFont);

            // Use the existing CalculateLayout function - no need to duplicate!
            Rectangle imageRect, textRect;
            CalculateLayout(contentRect, imageSize, textSize, out imageRect, out textRect);

            // HTML-style hover animations and transformations
            float imageTranslateX = 0f;
            float imageRotation = 0f;
            float imageScale = 1f;
            float textTranslateX = 0f;

            if (IsHovered)
            {
                // HTML CSS-like hover effects
                imageTranslateX = 8f;
                imageRotation = 5f;
                imageScale = 1.1f;
                textTranslateX = 4f;
            }

            if (IsPressed)
            {
                // HTML CSS-like pressed effects
                imageScale = 0.95f;
                textTranslateX = 0f;
            }

            // Draw the image using BeepImage with HTML-style transformations (same structure as original)
            if (beepImage != null && beepImage.HasImage)
            {
                // Apply HTML-style transformations to BeepImage
                beepImage.ManualRotationAngle = imageRotation;
                beepImage.ScaleFactor = imageScale;

                // Calculate final image rectangle with transformations
                Rectangle finalImageRect = new Rectangle(
                    (int)(imageRect.X + imageTranslateX),
                    imageRect.Y,
                    imageRect.Width,
                    imageRect.Height
                );

                if (beepImage.Size.Width > this.Size.Width || beepImage.Size.Height > this.Size.Height)
                {
                    imageSize = this.Size;
                }
                beepImage.MaximumSize = imageSize;
                beepImage.Size = finalImageRect.Size;

                if (ApplyThemeOnImage)
                {
                    beepImage.Theme = Theme;
                    beepImage.ApplyTheme();
                }

                // Let BeepImage handle all the drawing (same as original)
                beepImage.DrawImage(g, finalImageRect);

                // Setup hit testing (same as original)
                if (beepImageHitTest == null)
                {
                    beepImageHitTest = new ControlHitTest(finalImageRect, Point.Empty)
                    {
                        Name = "BeepImageRect",
                        ActionName = "ImageClicked",
                        HitAction = () =>
                        {
                            var ev = new BeepEventDataArgs("ImageClicked", this);
                            ImageClicked?.Invoke(this, ev);
                        }
                    };
                }
                else
                {
                    beepImageHitTest.TargetRect = finalImageRect;
                }

                AddHitTest(beepImageHitTest);
            }

            // Draw text with HTML-style effects (same structure as original)
            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                // Apply text transformation for HTML-style animation
                Rectangle transformedTextRect = new Rectangle(
                    (int)(textRect.X + textTranslateX),
                    textRect.Y,
                    textRect.Width,
                    textRect.Height
                );

                TextFormatFlags flags = GetTextFormatFlags(TextAlign);

                // Use high-quality text rendering with HTML font
                using (var textBrush = new SolidBrush(textColor))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    TextRenderer.DrawText(g, Text, htmlFont, transformedTextRect, textColor, flags);
                }
            }
        }
        /// <summary>
        /// CSS Button Style 2: Purple button with expanding white icon container
        /// Based on .cssbuttons-io-button style with expanding icon animation
        /// </summary>
        public void DrawButtonAndImage2(Graphics g)
        {
            Color textColor;

            // Determine text color based on current state and theme settings (same as original)
            if (_isColorFromTheme)
            {
                textColor = _originalForColor;
            }
            else
            {
                textColor = ForeColor;
            }

            // Update text color based on button state for better visibility (same as original)
            if (Enabled)
            {
                if (IsHovered)
                {
                    textColor = HoverForeColor;
                }
                else if (IsSelected)
                {
                    textColor = SelectedForeColor;
                }
                else if (IsPressed)
                {
                    textColor = PressedForeColor;
                }
            }
            else
            {
                textColor = DisabledForeColor;
            }

            // Use scaled font if configured (same as original)
            Font scaledFont = _textFont;
            if (UseScaledFont)
            {
                scaledFont = GetScaledFont(g, Text, contentRect.Size, _textFont);
            }

            // CSS Style 2 font: inherit font-family, 17px, font-weight: 500
            Font cssFont = new Font("Segoe UI",11, FontStyle.Regular);

            Size imageSize = beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;

            // Update image size to fit new size of control (same as original)
            if (beepImage != null)
            {
                if (_maxImageSize.Height > this.Height)
                {
                    beepImage.Height = this.Height - 4;
                    _maxImageSize.Height = this.Height - 4;
                }
                if (_maxImageSize.Width > this.Width)
                {
                    beepImage.Width = this.Width - 4;
                    _maxImageSize.Width = this.Width - 4;
                }
            }

            // CSS Style 2: Icon size should be 2.2em height/width (about 24px at 17px font)
            int iconSize = Math.Min(24, Math.Min(contentRect.Height - 8, contentRect.Width - 8));
            if (imageSize.Width > iconSize || imageSize.Height > iconSize)
            {
                float scaleFactor = Math.Min(
                    (float)iconSize / imageSize.Width,
                    (float)iconSize / imageSize.Height);
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            Size textSize = TextRenderer.MeasureText(Text, cssFont);

            // CSS Style 2 Layout: Text on left, expanding icon container on right
            int iconContainerSize = Math.Min(contentRect.Height - 6, iconSize + 8); // 2.2em container
            int iconContainerPadding = 3; // 0.3em right padding

            // CSS Style 2 animations
            float iconContainerWidth = iconContainerSize; // Default width
            float iconTranslateX = 0f;
            float iconScale = 1f;

            if (IsHovered)
            {
                // CSS hover: .icon width: calc(100% - 0.6em) and translateX
                iconContainerWidth = contentRect.Width - 12; // calc(100% - 0.6em)
                iconTranslateX = 2f; // translateX(0.1em)
            }

            if (IsPressed)
            {
                // CSS active: transform: scale(0.95)
                iconScale = 0.95f;
            }

            // Calculate CSS Style 2 layout
            Rectangle textRect = new Rectangle(
                contentRect.Left + 18, // padding-left: 1.2em
                contentRect.Top,
                contentRect.Width - (int)iconContainerWidth - 30, // Leave space for icon
                contentRect.Height
            );

            Rectangle iconContainerRect = new Rectangle(
                (int)(contentRect.Right - iconContainerWidth - iconContainerPadding),
                contentRect.Top + (contentRect.Height - iconContainerSize) / 2,
                (int)iconContainerWidth,
                iconContainerSize
            );

            Rectangle iconRect = new Rectangle(
                (int)(iconContainerRect.X + iconTranslateX + (iconContainerRect.Width - imageSize.Width) / 2),
                iconContainerRect.Y + (iconContainerRect.Height - imageSize.Height) / 2,
                imageSize.Width,
                imageSize.Height
            );

            // Draw CSS Style 2: White icon container with shadow
            using (SolidBrush iconContainerBrush = new SolidBrush(Color.White))
            {
                using (GraphicsPath iconContainerPath = GetRoundedRectPath(iconContainerRect, iconContainerSize / 3))
                {
                    // Apply scale transformation for pressed state
                    if (IsPressed && iconScale != 1f)
                    {
                        Matrix scaleMatrix = new Matrix();
                        PointF center = new PointF(
                            iconContainerRect.X + iconContainerRect.Width / 2f,
                            iconContainerRect.Y + iconContainerRect.Height / 2f
                        );
                        scaleMatrix.Translate(center.X, center.Y);
                        scaleMatrix.Scale(iconScale, iconScale);
                        scaleMatrix.Translate(-center.X, -center.Y);
                        iconContainerPath.Transform(scaleMatrix);
                    }

                    // Draw shadow: box-shadow: 0.1em 0.1em 0.6em 0.2em #7b52b9
                    using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(40, 123, 82, 185)))
                    {
                        using (GraphicsPath shadowPath = (GraphicsPath)iconContainerPath.Clone())
                        {
                            Matrix shadowMatrix = new Matrix();
                            shadowMatrix.Translate(1.5f, 1.5f); // 0.1em offset
                            shadowPath.Transform(shadowMatrix);
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }

                    // Draw white icon container
                    g.FillPath(iconContainerBrush, iconContainerPath);
                }
            }

            // Draw the image using BeepImage with CSS Style 2 transformations (same structure as original)
            if (beepImage != null && beepImage.HasImage)
            {
                // CSS Style 2: Icon color should be #7b52b9 (purple)
                beepImage.ForeColor = Color.FromArgb(123, 82, 185);
                beepImage.ManualRotationAngle = 0; // No rotation in this style
                beepImage.ScaleFactor = 1f; // Scale handled by container

                beepImage.MaximumSize = imageSize;
                beepImage.Size = imageSize;

                if (ApplyThemeOnImage)
                {
                    beepImage.Theme = Theme;
                    beepImage.ApplyTheme();
                }

                // Let BeepImage handle all the drawing (same as original)
                beepImage.DrawImage(g, iconRect);

                // Setup hit testing (same as original)
                if (beepImageHitTest == null)
                {
                    beepImageHitTest = new ControlHitTest(iconRect, Point.Empty)
                    {
                        Name = "BeepImageRect",
                        ActionName = "ImageClicked",
                        HitAction = () =>
                        {
                            var ev = new BeepEventDataArgs("ImageClicked", this);
                            ImageClicked?.Invoke(this, ev);
                        }
                    };
                }
                else
                {
                    beepImageHitTest.TargetRect = iconRect;
                }

                AddHitTest(beepImageHitTest);
            }

            // Draw text with CSS Style 2 styling (same structure as original)
            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                TextFormatFlags flags = GetTextFormatFlags(TextAlign);

                // CSS Style 2: White text color, letter-spacing: 0.05em
                using (var textBrush = new SolidBrush(Color.White))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    TextRenderer.DrawText(g, Text, cssFont, textRect, Color.White, flags);
                }
            }
        }
        /// <summary>
        /// CSS Button Style 3: Lime green button with animated arrow SVG
        /// Based on lime button style with sliding arrow animation
        /// </summary>
        public void DrawButtonAndImage3(Graphics g)
        {
            Color textColor;

            // Determine text color based on current state and theme settings (same as original)
            if (_isColorFromTheme)
            {
                textColor = _originalForColor;
            }
            else
            {
                textColor = ForeColor;
            }

            // Update text color based on button state for better visibility (same as original)
            if (Enabled)
            {
                if (IsHovered)
                {
                    textColor = HoverForeColor;
                }
                else if (IsSelected)
                {
                    textColor = SelectedForeColor;
                }
                else if (IsPressed)
                {
                    textColor = PressedForeColor;
                }
            }
            else
            {
                textColor = DisabledForeColor;
            }

            // Use scaled font if configured (same as original)
            Font scaledFont = _textFont;
            if (UseScaledFont)
            {
                scaledFont = GetScaledFont(g, Text, contentRect.Size, _textFont);
            }

            // CSS Style 3 font: font-weight: 700, font-size: 15px
            Font cssFont = new Font("Segoe UI", 10f, FontStyle.Bold);

            Size imageSize = beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;

            // Update image size to fit new size of control (same as original)
            if (beepImage != null)
            {
                if (_maxImageSize.Height > this.Height)
                {
                    beepImage.Height = this.Height - 4;
                    _maxImageSize.Height = this.Height - 4;
                }
                if (_maxImageSize.Width > this.Width)
                {
                    beepImage.Width = this.Width - 4;
                    _maxImageSize.Width = this.Width - 4;
                }
            }

            // CSS Style 3: SVG size should be 34px
            int svgSize = Math.Min(28, Math.Min(contentRect.Height - 8, contentRect.Width - 8));
            if (imageSize.Width > svgSize || imageSize.Height > svgSize)
            {
                float scaleFactor = Math.Min(
                    (float)svgSize / imageSize.Width,
                    (float)svgSize / imageSize.Height);
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            Size textSize = TextRenderer.MeasureText(Text, cssFont);

            // CSS Style 3 Layout: Text on left, SVG on right with margin-left: 10px
            int svgMargin = 8; // margin-left: 10px equivalent

            // CSS Style 3 animations
            float svgTranslateX = 0f;
            float buttonScale = 1f;

            if (IsHovered)
            {
                // CSS hover: svg translateX(5px)
                svgTranslateX = 4f;
            }

            if (IsPressed)
            {
                // CSS active: transform: scale(0.95)
                buttonScale = 0.95f;
            }

            // Apply button scale transformation if pressed
            Rectangle scaledContentRect = contentRect;
            if (IsPressed && buttonScale != 1f)
            {
                int scaledWidth = (int)(contentRect.Width * buttonScale);
                int scaledHeight = (int)(contentRect.Height * buttonScale);
                int offsetX = (contentRect.Width - scaledWidth) / 2;
                int offsetY = (contentRect.Height - scaledHeight) / 2;
                scaledContentRect = new Rectangle(
                    contentRect.X + offsetX,
                    contentRect.Y + offsetY,
                    scaledWidth,
                    scaledHeight);
            }

            // Calculate CSS Style 3 layout with flex alignment
            Rectangle textRect = new Rectangle(
                scaledContentRect.Left + 16, // padding: 10px 20px equivalent
                scaledContentRect.Top,
                scaledContentRect.Width - imageSize.Width - svgMargin - 32, // Leave space for SVG and padding
                scaledContentRect.Height
            );

            Rectangle svgRect = new Rectangle(
                (int)(textRect.Right + svgMargin + svgTranslateX),
                scaledContentRect.Top + (scaledContentRect.Height - imageSize.Height) / 2,
                imageSize.Width,
                imageSize.Height
            );

            // Draw the image using BeepImage with CSS Style 3 transformations (same structure as original)
            if (beepImage != null && beepImage.HasImage)
            {
                // CSS Style 3: SVG should be black
                beepImage.ForeColor = Color.Black;
                beepImage.ManualRotationAngle = 0; // No rotation in this style
                beepImage.ScaleFactor = 1f; // No additional scaling

                beepImage.MaximumSize = imageSize;
                beepImage.Size = imageSize;

                if (ApplyThemeOnImage)
                {
                    beepImage.Theme = Theme;
                    beepImage.ApplyTheme();
                }

                // Let BeepImage handle all the drawing (same as original)
                beepImage.DrawImage(g, svgRect);

                // Setup hit testing (same as original)
                if (beepImageHitTest == null)
                {
                    beepImageHitTest = new ControlHitTest(svgRect, Point.Empty)
                    {
                        Name = "BeepImageRect",
                        ActionName = "ImageClicked",
                        HitAction = () =>
                        {
                            var ev = new BeepEventDataArgs("ImageClicked", this);
                            ImageClicked?.Invoke(this, ev);
                        }
                    };
                }
                else
                {
                    beepImageHitTest.TargetRect = svgRect;
                }

                AddHitTest(beepImageHitTest);
            }

            // Draw text with CSS Style 3 styling (same structure as original)
            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                TextFormatFlags flags = GetTextFormatFlags(TextAlign);

                // CSS Style 3: Black text color, bold weight
                using (var textBrush = new SolidBrush(Color.Black))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    TextRenderer.DrawText(g, Text, cssFont, textRect, Color.Black, flags);
                }
            }
        }
        /// <summary>
        /// CSS Button Style 4: Modern sliding background button with icon container
        /// Based on button with sliding background animation and icon area
        /// </summary>
        public void DrawButtonAndImage4(Graphics g)
        {
            Color textColor;

            // Determine text color based on current state and theme settings (same as original)
            if (_isColorFromTheme)
            {
                textColor = _originalForColor;
            }
            else
            {
                textColor = ForeColor;
            }

            // Update text color based on button state for better visibility (same as original)
            if (Enabled)
            {
                if (IsHovered)
                {
                    textColor = HoverForeColor;
                }
                else if (IsSelected)
                {
                    textColor = SelectedForeColor;
                }
                else if (IsPressed)
                {
                    textColor = PressedForeColor;
                }
            }
            else
            {
                textColor = DisabledForeColor;
            }

            // Use scaled font if configured (same as original)
            Font scaledFont = _textFont;
            if (UseScaledFont)
            {
                scaledFont = GetScaledFont(g, Text, contentRect.Size, _textFont);
            }

            // CSS Style 4 font: font-weight: 600, line-height: 1
            Font cssFont = new Font("Segoe UI", 9f, FontStyle.Bold);

            Size imageSize = beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;

            // Update image size to fit new size of control (same as original)
            if (beepImage != null)
            {
                if (_maxImageSize.Height > this.Height)
                {
                    beepImage.Height = this.Height - 4;
                    _maxImageSize.Height = this.Height - 4;
                }
                if (_maxImageSize.Width > this.Width)
                {
                    beepImage.Width = this.Width - 4;
                    _maxImageSize.Width = this.Width - 4;
                }
            }

            // CSS Style 4: Icon container size (48px width, 40px height from CSS)
            int iconContainerWidth = Math.Min(32, contentRect.Height - 4);
            int iconContainerHeight = contentRect.Height - 4;
            int iconSize = Math.Min(20, iconContainerHeight - 8);

            if (imageSize.Width > iconSize || imageSize.Height > iconSize)
            {
                float scaleFactor = Math.Min(
                    (float)iconSize / imageSize.Width,
                    (float)iconSize / imageSize.Height);
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            Size textSize = TextRenderer.MeasureText(Text, cssFont);

            // CSS Style 4 Layout: Icon container on left, text on right
            // CSS Style 4 animations
            float decorTranslateX = -contentRect.Width; // Start completely hidden (translateX(-100%))
            Color decorColor = Color.FromArgb(0, 173, 84); // --clr: #00ad54

            if (IsHovered)
            {
                // CSS hover: background slides in (transform: translate(0))
                decorTranslateX = 0f;
                textColor = Color.White; // Text becomes white on hover
            }

            // Calculate CSS Style 4 layout
            Rectangle iconContainerRect = new Rectangle(
                contentRect.Left + 2,
                contentRect.Top + 2,
                iconContainerWidth,
                iconContainerHeight
            );

            Rectangle textRect = new Rectangle(
                iconContainerRect.Right + 6, // padding-left: .75rem
                contentRect.Top,
                contentRect.Width - iconContainerWidth - 12, // Leave space for icon and padding
                contentRect.Height
            );

            Rectangle iconRect = new Rectangle(
                iconContainerRect.X + (iconContainerRect.Width - imageSize.Width) / 2,
                iconContainerRect.Y + (iconContainerRect.Height - imageSize.Height) / 2,
                imageSize.Width,
                imageSize.Height
            );

            // Draw CSS Style 4: Sliding background decoration (button-decor)
            Rectangle decorRect = new Rectangle(
                (int)(contentRect.X + decorTranslateX),
                contentRect.Y,
                contentRect.Width,
                contentRect.Height
            );

            if (IsHovered)
            {
                using (SolidBrush decorBrush = new SolidBrush(decorColor))
                {
                    if (IsRounded && BorderRadius > 0)
                    {
                        using (GraphicsPath decorPath = GetRoundedRectPath(decorRect, BorderRadius))
                        {
                            g.FillPath(decorBrush, decorPath);
                        }
                    }
                    else
                    {
                        g.FillRectangle(decorBrush, decorRect);
                    }
                }
            }

            // Draw CSS Style 4: Icon container background (always colored)
            using (SolidBrush iconContainerBrush = new SolidBrush(decorColor))
            {
                if (IsRounded && BorderRadius > 0)
                {
                    using (GraphicsPath iconContainerPath = GetRoundedRectPath(iconContainerRect, BorderRadius / 2))
                    {
                        g.FillPath(iconContainerBrush, iconContainerPath);
                    }
                }
                else
                {
                    g.FillRectangle(iconContainerBrush, iconContainerRect);
                }
            }

            // Draw the image using BeepImage with CSS Style 4 transformations (same structure as original)
            if (beepImage != null && beepImage.HasImage)
            {
                // CSS Style 4: Icon should be white
                beepImage.ForeColor = Color.White;
                beepImage.ManualRotationAngle = 0; // No rotation in this style
                beepImage.ScaleFactor = 1f; // No additional scaling

                beepImage.MaximumSize = imageSize;
                beepImage.Size = imageSize;

                if (ApplyThemeOnImage)
                {
                    beepImage.Theme = Theme;
                    beepImage.ApplyTheme();
                }

                // Let BeepImage handle all the drawing (same as original)
                beepImage.DrawImage(g, iconRect);

                // Setup hit testing (same as original)
                if (beepImageHitTest == null)
                {
                    beepImageHitTest = new ControlHitTest(iconRect, Point.Empty)
                    {
                        Name = "BeepImageRect",
                        ActionName = "ImageClicked",
                        HitAction = () =>
                        {
                            var ev = new BeepEventDataArgs("ImageClicked", this);
                            ImageClicked?.Invoke(this, ev);
                        }
                    };
                }
                else
                {
                    beepImageHitTest.TargetRect = iconRect;
                }

                AddHitTest(beepImageHitTest);
            }

            // Draw text with CSS Style 4 styling (same structure as original)
            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                // Constrain text width: max-width: 150px, text-overflow: ellipsis
                int maxTextWidth = Math.Min(120, textRect.Width);
                Rectangle constrainedTextRect = new Rectangle(
                    textRect.X,
                    textRect.Y,
                    maxTextWidth,
                    textRect.Height
                );

                TextFormatFlags flags = GetTextFormatFlags(TextAlign) | TextFormatFlags.EndEllipsis;

                // CSS Style 4: Dynamic text color (dark by default, white on hover)
                Color finalTextColor = IsHovered ? Color.White : Color.FromArgb(18, 18, 18); // #121212

                using (var textBrush = new SolidBrush(finalTextColor))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    TextRenderer.DrawText(g, Text, cssFont, constrainedTextRect, finalTextColor, flags);
                }
            }
        }
        #endregion "Draw Button From Html "
        #endregion "Draw Image and text"
    }
  

}
