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
 
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Models;
using ContentAlignment = System.Drawing.ContentAlignment;
using TextImageRelation = System.Windows.Forms.TextImageRelation;
 


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Button))]
    [Category("Controls")]
    [Description("A button control with an image and text.")]
    [DisplayName("Beep Button")]
    [Designer("TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.BeepButtonDesigner, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    public class BeepButton : BaseControl
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

        // REMOVED: DisableDpiAndScaling property - .NET 8/9+ handles DPI automatically via framework
        // DPI scaling is managed by the framework using AutoScaleMode.Inherit

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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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
        private Size _buttonminsize = new Size(32, 32);
        [Browsable(true)]
        [Category("Appearance")]
        public Size ButtonMinSize
        {
            get => _buttonminsize;
            set
            {
                _buttonminsize = value;
                UpdateMinSizeForMode();
                Invalidate();
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
        [Browsable(true)]
        [Category("Appearance")]
        [Editor("TheTechIdea.Beep.Winform.Controls.Design.Server.Editors.BeepImagePathEditor, TheTechIdea.Beep.Winform.Controls.Design.Server", typeof(System.Drawing.Design.UITypeEditor))]
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
        // Expose alignment and relation as properties (needed by designer and drawing)
        [Browsable(true)]
        [Category("Appearance")]
        public TextImageRelation TextImageRelation
        {
            get => textImageRelation;
            set { textImageRelation = value; Invalidate(); }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public ContentAlignment ImageAlign
        {
            get => imageAlign;
            set { imageAlign = value; Invalidate(); }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public ContentAlignment TextAlign
        {
            get => textAlign;
            set { textAlign = value; Invalidate(); }
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
        private bool IsImageOnly =>
    (string.IsNullOrEmpty(Text) || _hideText) &&
     !string.IsNullOrEmpty(beepImage?.ImagePath) ;

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
       
        // New Property for Hover Persistence using MenuStyle
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
                UpdateMinSizeForMode();
                Invalidate();
            }
        }
        private bool _autoSize = false;
        // Recommended: rename to avoid surprises
        [Browsable(true)]
        [Category("Behavior")]
        public bool AutoSizeContent
        {
            get => _autoSize;
            set { _autoSize = value; Invalidate(); PerformLayout(); }
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
               
                Invalidate();  // Trigger repaint
            }
        }
        private ControlHitTest beepImageHitTest;
        private Color _originalForColor;

     

        // Material Design convenience properties
        [Browsable(true)]
        [Category("Material Design")]
        [Description("The floating label text that appears above the button.")]
        public string ButtonLabel
        {
            get => LabelText;
            set => LabelText = value;
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Helper text that appears below the button.")]
        public string ButtonHelperText
        {
            get => HelperText;
            set => HelperText = value;
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Error message to display when validation fails.")]
        public string ButtonErrorText
        {
            get => ErrorText;
            set => ErrorText = value;
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Whether the button is in an error state.")]
        public bool ButtonHasError
        {
            get => HasError;
            set => HasError = value;
        }

        [Browsable(true)]
        [Category("Material Design")]
        [Description("Automatically adjust size when Material Design styling is enabled.")]
        [DefaultValue(false)]
        public bool ButtonAutoSizeForMaterial { get; set; } = false;

        [Browsable(true)]
        [Category("Layout")]
        [Description("Prevents automatic width/height expansion for Material Design. Default is true.")]
        [DefaultValue(true)]
        public bool ButtonPreventAutoExpansion { get; set; } = true;

        /// <summary>
        /// Override to provide button specific minimum dimensions
        /// </summary>
        /// <returns>Minimum height for Material Design button</returns>
        protected override int GetMaterialMinimumHeight()
        {
            // If this is an image-only button (no text), allow much smaller height
            if ((string.IsNullOrEmpty(Text) || HideText) && beepImage?.HasImage == true)
            {
                Size imageSize = beepImage.GetImageSize();
                if (imageSize.Width > _maxImageSize.Width || imageSize.Height > _maxImageSize.Height)
                {
                    float scaleFactor = Math.Min(
                        (float)_maxImageSize.Width / imageSize.Width,
                        (float)_maxImageSize.Height / imageSize.Height);
                    imageSize = new Size(
                        (int)(imageSize.Width * scaleFactor),
                        (int)(imageSize.Height * scaleFactor));
                }
                return Math.Max(ButtonMinSize.Height, imageSize.Height + 8);
            }

            // Keep buttons compact; base spec sizes are large for touch. Use compact heights.
            return Math.Max(ButtonMinSize.Height, 28);
        }

        /// <summary>
        /// Override to provide button specific minimum width
        /// </summary>
        /// <returns>Minimum width for Material Design button</returns>
        protected override int GetMaterialMinimumWidth()
        {
            // Image-only compact width
            if ((string.IsNullOrEmpty(Text) || HideText) && beepImage?.HasImage == true)
            {
                Size imageSize = beepImage.GetImageSize();
                if (imageSize.Width > _maxImageSize.Width || imageSize.Height > _maxImageSize.Height)
                {
                    float scaleFactor = Math.Min(
                        (float)_maxImageSize.Width / imageSize.Width,
                        (float)_maxImageSize.Height / imageSize.Height);
                    imageSize = new Size(
                        (int)(imageSize.Width * scaleFactor),
                        (int)(imageSize.Height * scaleFactor));
                }
                return Math.Max(ButtonMinSize.Width, imageSize.Width + 8);
            }

            // Compact width for text buttons
            int baseMinWidth = 48; // compact baseline
            var iconSpace = GetMaterialIconSpace();
            baseMinWidth += iconSpace.Width;
            return Math.Max(ButtonMinSize.Width, baseMinWidth);
        }
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

            // Enable material Style for modern button appearance, but keep it compact
            //EnableMaterialStyle = true;
          
            ButtonAutoSizeForMaterial = false; // Default to false to prevent large buttons
            MaterialPreserveContentArea = true; // Preserve content area instead of expanding

            // Apply size compensation when handle is created if explicitly enabled
            HandleCreated += (s, e) => {
                if (PainterKind == BaseControlPainterKind.Material && ButtonAutoSizeForMaterial && !ButtonPreventAutoExpansion)
                {
                    ApplyMaterialSizeCompensation();
                }
            };
            // honor BaseControl scaling setting (do not force disable here)

            // DON'T set hardcoded gradient colors - let ApplyTheme() handle them from the theme

            #region "Popup List Initialization"
            IsChild = false;
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
            // Mouse events for image
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
                SimpleItemFactory.RunMethodFromObjectHandler(item, MethodName);

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
     //       base.OnFontChanged(e);
            _textFont = Font;
            
            // Apply Material Design size compensation if enabled
            if (PainterKind== BaseControlPainterKind.Material && ButtonAutoSizeForMaterial)
            {
                ApplyMaterialSizeCompensation();
            }
            
          // // Console.WriteLine("Font Changed");
            if (AutoSize)
            {
                Size textSize = TextRenderer.MeasureText(Text, _textFont);
                Size = new Size(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
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
                // Get font from button Style or fall back to default Style
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
            Invalidate();
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
      
    

      
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            UpdateDrawingRect();
            contentRect = DrawingRect;

            DrawStateOverlays(g);   // <— subtle hover/press glaze
            DrawImageAndText(g);
            DrawSplashEffect(g);    // ripple on top

        }

        private void DrawSplashEffect(Graphics g)
        {
            if (!EnableSplashEffect || !splashActive) return;

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
                    using (GraphicsPath clipPath = GraphicsExtensions.GetRoundedRectPath(contentRect, BorderRadius))
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
                overlayColor = PressedBackColor;
                alpha = 15; // Reduced from 30 for more subtlety
            }
            else if (IsHovered)
            {
                // Very subtle hover state overlay
                overlayColor =HoverBackColor;
                alpha = 8; // Reduced from 15 for more subtlety
            }

            // Only apply overlay if we have a valid state
            if (alpha > 0)
            using (SolidBrush overlayBrush = new SolidBrush(Color.FromArgb(alpha, overlayColor)))
            {
                if (IsRounded && BorderRadius > 0)
                {
                    using (GraphicsPath path = GraphicsExtensions.GetRoundedRectPath(contentRect, BorderRadius))
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
                switch (TextImageRelation)
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
            // Start with base preferred to let BaseControl add any baseline effects
            var pref = base.GetPreferredSize(proposedSize);

            // Measure content (text and/or image)
            Size textSize = Size.Empty;
            Size imageSize = Size.Empty;

            // Only measure text if we have text and it's not hidden
            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                textSize = TextRenderer.MeasureText(Text, _textFont ?? Font);
            }

            // Get image size if available
            if (beepImage?.HasImage == true)
            {
                imageSize = beepImage.GetImageSize();

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
            }

            // If no content, return a minimal default size
            if (textSize.IsEmpty && imageSize.IsEmpty)
            {
                // Minimal button size
                var minimal = new Size(32, 28);
                return PainterKind == BaseControlPainterKind.Material
                    ? GetEffectiveMaterialMinimum(new Size(32, 16))
                    : minimal;
            }

            // Compute base content size (without Material paddings)
            Size baseContentSize;

            if (textSize.IsEmpty && !imageSize.IsEmpty)
            {
                // Image only - minimal content area
                baseContentSize = new Size(imageSize.Width, imageSize.Height);
            }
            else if (!textSize.IsEmpty && imageSize.IsEmpty)
            {
                // Text only - use measured text
                baseContentSize = textSize;
            }
            else
            {
                // Both text and image: compute union per current relation using a virtual layout
                Rectangle virtualContentRect = new Rectangle(0, 0, 1000, 1000);
                CalculateLayout(virtualContentRect, imageSize, textSize, out var imageRect, out var textRect);
                Rectangle totalBounds = Rectangle.Union(imageRect, textRect);
                baseContentSize = totalBounds.Size;
            }

            // If Material Style is enabled, expand by Material paddings/effects properly
            if (PainterKind ==BaseControlPainterKind.Material)
            {
                // Ask BaseControl to compute effective minimum including Material paddings, icons, effects, DPI, etc.
                var materialMin = GetEffectiveMaterialMinimum(baseContentSize);

                // Respect button-specific minimums
                materialMin.Width = Math.Max(materialMin.Width, ButtonMinSize.Width);
                materialMin.Height = Math.Max(materialMin.Height, ButtonMinSize.Height);

                return materialMin;
            }

            // Non-material: add simple padding around content
            int finalWidth;
            int finalHeight;

            if (!textSize.IsEmpty && imageSize.IsEmpty)
            {
                finalWidth = textSize.Width + Padding.Left + Padding.Right + 16;
                finalHeight = textSize.Height + Padding.Top + Padding.Bottom + 12;
            }
            else if (textSize.IsEmpty && !imageSize.IsEmpty)
            {
                finalWidth = imageSize.Width + 8;  // small padding for image-only
                finalHeight = imageSize.Height + 8;
            }
            else
            {
                Rectangle virtualContentRect = new Rectangle(0, 0, 1000, 1000);
                CalculateLayout(virtualContentRect, imageSize, textSize, out var imageRect, out var textRect);
                Rectangle totalBounds = Rectangle.Union(imageRect, textRect);
                finalWidth = totalBounds.Width + Padding.Left + Padding.Right + 16;
                finalHeight = totalBounds.Height + Padding.Top + Padding.Bottom + 12;
            }

            finalWidth = Math.Max(finalWidth, ButtonMinSize.Width);
            finalHeight = Math.Max(finalHeight, ButtonMinSize.Height);

            return new Size(finalWidth, finalHeight);
        }
        #endregion "Paint"
        #region "Mouse and Click"
        /// <summary>
        /// Programmatically triggers the Click event on this button.
        /// Simulates a press state and starts the splash ripple (if enabled).
        /// </summary>
        public void PerformClick()
        {
            if (!Enabled || !Visible)
                return;

            // Simulate press state
            IsPressed = true;
            Invalidate();

            try
            {
                // Optional ripple from center
                if (EnableSplashEffect && splashTimer != null)
                {
                    splashActive = true;
                    splashCenter = new Point(Width / 2, Height / 2);
                    splashProgress = 0f;
                    splashTimer.Start();
                }

                // Raise Click
                OnClick(EventArgs.Empty);
            }
            finally
            {
                // Release press state
                IsPressed = false;
                Invalidate();
            }
        }

        /// <summary>
        /// Programmatically triggers the Click event and shows splash ripple at a custom center.
        /// </summary>
        /// <param name="rippleCenter">Ripple origin; ignored if splash effect is disabled.</param>
        public void PerformClick(Point rippleCenter)
        {
            if (!Enabled || !Visible)
                return;

            IsPressed = true;
            Invalidate();

            try
            {
                if (EnableSplashEffect && splashTimer != null)
                {
                    splashActive = true;
                    splashCenter = rippleCenter;
                    splashProgress = 0f;
                    splashTimer.Start();
                }

                OnClick(EventArgs.Empty);
            }
            finally
            {
                IsPressed = false;
                Invalidate();
            }
        }
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
            
            // Enable splash effect when allowed
            if (EnableSplashEffect)
            {
                splashActive = true;
                splashCenter = e.Location; // Use the mouse click location for the ripple's center
                splashProgress = 0f;
                splashTimer.Start();
            }
           
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

        public void SetBinding(String controlProperty, String dataSourceProperty)
        {
            // Implementation for setting up data binding
            DataBindings.Add(controlProperty, DataContext, dataSourceProperty, true, DataSourceUpdateMode.OnPropertyChanged);
        }
        #endregion "Binding and Control Type"
        #region Splash Animation
        // Add the timer event handler method:
        private void SplashTimer_Tick(object sender, EventArgs e)
        {
            if (!EnableSplashEffect)
            {
                splashTimer.Stop();
                splashActive = false;
                splashProgress = 0f;
                return;
            }
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
        private Color _originalBackColor;

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            // unregister from old parent
            if (_lastBeepParent != null)
                _lastBeepParent.ClearChildExternalDrawing(this);

            // register with new parent
            if (Parent is BeepControl newBeepParent)
            {
                // inherit splash effect setting from parent BeepControl
                EnableSplashEffect = newBeepParent.EnableSplashEffect;
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
        private void UpdateMinSizeForMode()
        {
            if (PainterKind != BaseControlPainterKind.Material)
            {
                // Image-only? keep the floor at ButtonMinSize; otherwise no clamp.
                bool imageOnly = (string.IsNullOrEmpty(Text) || HideText) && beepImage?.HasImage == true;
                MinimumSize = imageOnly ? ButtonMinSize : Size.Empty;
            }
            if (IsImageOnly)
                MinimumSize = _buttonminsize;     // allow small image-only buttons
            else
                MinimumSize = Size.Empty;         // let Material/Base logic handle text buttons
        }

        public override void Draw(Graphics g, Rectangle rectangle)
        {
            contentRect = rectangle;
            DrawImageAndText(g);
        }
        private void DrawImageAndText(Graphics g)
        {
            Color textColor;

           

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
                else
                {
                    textColor = ForeColor;
                }
            }
            else
            {
                textColor = DisabledForeColor;
            }

            // Framework handles DPI scaling automatically - use font directly
            Font scaledFont = _textFont;

            Size imageSize = beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;

            // Update image size to fit new size of control
            if (beepImage != null)
            {
                if (_maxImageSize.Height > Height)
                {
                    beepImage.Height = Height - 4;
                    _maxImageSize.Height = Height - 4;
                }
                if (_maxImageSize.Width > Width)
                {
                    beepImage.Width = Width - 4;
                    _maxImageSize.Width = Width - 4;
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
                if (beepImage.Size.Width > Size.Width || beepImage.Size.Height > Size.Height)
                {
                    imageSize = Size;
                }
                beepImage.MaximumSize = imageSize;
                beepImage.Size = imageRect.Size;
                if ( ApplyThemeOnImage)
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
        #endregion // End Draw Button From Html source
        #region "Material Design Support"

        /// <summary>
        /// Manually triggers Material Design size compensation for testing/debugging
        /// </summary>
        public void ForceMaterialSizeCompensation()
        {
            Console.WriteLine($"BeepButton: Force compensation called. EnableMaterialStyle: {PainterKind == BaseControlPainterKind.Material}, AutoSize: {ButtonAutoSizeForMaterial}, PreventExpansion: {ButtonPreventAutoExpansion}");
            
            // Temporarily enable auto size and disable expansion prevention if needed
            bool originalAutoSize = ButtonAutoSizeForMaterial;
            bool originalPreventExpansion = ButtonPreventAutoExpansion;
            
            ButtonAutoSizeForMaterial = true;
            ButtonPreventAutoExpansion = false;
            
            ApplyMaterialSizeCompensation();
            
            // Restore original settings
            ButtonAutoSizeForMaterial = originalAutoSize;
            ButtonPreventAutoExpansion = originalPreventExpansion;
            
          
            Invalidate();
        }
        protected override void OnClick(EventArgs e)
        {
            if (isLongPressTriggered)
            {
                // swallow the click that follows a long press
                isLongPressTriggered = false;
                return;
            }
            base.OnClick(e);
        }

        /// <summary>
        /// Override the base Material size compensation to handle Button-specific logic
        /// </summary>
        public override void ApplyMaterialSizeCompensation()
        {
            if (PainterKind != BaseControlPainterKind.Material || !ButtonAutoSizeForMaterial || ButtonPreventAutoExpansion)
                return;

            // Use fixed base size to prevent unwanted expansion for image-only buttons
            Size baseContentSize;
            
            if ((string.IsNullOrEmpty(Text) || HideText) && beepImage?.HasImage == true)
            {
                // For image-only buttons, use actual image size
                Size imageSize = beepImage.GetImageSize();
                if (imageSize.Width > _maxImageSize.Width || imageSize.Height > _maxImageSize.Height)
                {
                    float scaleFactor = Math.Min(
                        (float)_maxImageSize.Width / imageSize.Width,
                        (float)_maxImageSize.Height / imageSize.Height);
                    imageSize = new Size(
                        (int)(imageSize.Width * scaleFactor),
                        (int)(imageSize.Height * scaleFactor));
                }
                baseContentSize = new Size(imageSize.Width + 8, imageSize.Height + 8);
            }
            else if (!string.IsNullOrEmpty(Text))
            {
                var measuredSize = TextUtils.MeasureText(Text, _textFont ?? Font);
                baseContentSize = new Size((int)Math.Ceiling(measuredSize.Width), (int)Math.Ceiling(measuredSize.Height));
            }
            else
            {
                // Default minimum size for empty buttons
                baseContentSize = new Size(48, 24);
            }

            // Apply Material size compensation using base method
            AdjustSizeForMaterial(baseContentSize, true);
        }

        /// <summary>
        /// Gets current Material Design size information for debugging
        /// </summary>
        public string GetMaterialSizeInfo()
        {
            if (PainterKind != BaseControlPainterKind.Material)
                return "Material Design is disabled";
                
            var padding = GetMaterialStylePadding();
            var effects = GetMaterialEffectsSpace();
            var icons = GetMaterialIconSpace();
            var minSize = CalculateMinimumSizeForMaterial(new Size(100, 32));
            
            return "Material Info:\n" +
                   $"Current Size: {Width}x{Height}\n" +
                   $"Variant: {MaterialVariant}\n" +
                   $"Padding: {padding}\n" +
                   $"Effects Space: {effects}\n" +
                   $"Icon Space: {icons}\n" +
                   $"Calculated Min Size: {minSize}\n" +
                   $"Auto Size Enabled: {ButtonAutoSizeForMaterial}\n" +
                   $"Prevent Auto Expansion: {ButtonPreventAutoExpansion}\n" +
                   $"Has Image: {beepImage?.HasImage}\n" +
                   $"TextImageRelation: {TextImageRelation}\n" +
                   $"ButtonType: {ButtonType}";
        }
        
        #endregion "Material Design Support"
    }
}
