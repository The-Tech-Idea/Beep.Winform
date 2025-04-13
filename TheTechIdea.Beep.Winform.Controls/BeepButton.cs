using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.ConfigUtil;
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
        private bool splashActive;    // Whether the splash animation is currently active

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
        private Color tmpbackcolor;
        private Color tmpforcolor;

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

        [Browsable(true)]
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
        private bool isSelectedAuto = false;
        // Public properties
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsSelectedAuto
        {
            get => isSelectedAuto;
            set
            {
                isSelectedAuto = value;
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
                   
                  Font = _textFont;
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
                Control parent = null;
                if (this.Parent != null)
                {
                    parent = this.Parent;
                }
                if (parent != null)
                {
                    if (value)
                    {
                        parentbackcolor = parent.BackColor;
                        _tempbackcolor = BackColor;
                        BackColor = parentbackcolor;
                        beepImage.BackColor = parentbackcolor;
                    }
                    else
                    {
                       
                        beepImage.BackColor = _tempbackcolor;
                        BackColor = _tempbackcolor;
                        ApplyTheme();
                    }
                }
               
                Invalidate();  // Trigger repaint
            }
        }
        private ControlHitTest beepImageHitTest;

        #endregion "Properties"
        #region "Constructor"
        // Constructor
        public BeepButton()
        {
            

            InitializeComponents();
         
            CanBeHovered = true;
            CanBePressed = true;
            CanBeFocused = true;
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
            BorderRadius = 3;
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

            menuDialog = new BeepPopupListForm(ListItems.ToList());
            //menuDialog._beepListBox.IsRoundedAffectedByTheme = false;
            //menuDialog._beepListBox.IsRounded = false;
            //menuDialog._beepListBox.ShowTitle = false;
            //menuDialog._beepListBox.ShowTitleLine = false;
            //menuDialog._beepListBox.IsShadowAffectedByTheme=false;
            //menuDialog._beepListBox.ShowShadow = false;
            //menuDialog._beepListBox.IsBorderAffectedByTheme = false;
            //menuDialog._beepListBox.ShowAllBorders = false;
            //menuDialog._beepListBox.IsFrameless = true;
            menuDialog.Theme = Theme;
            menuDialog.SelectedItemChanged += MenuDialog_SelectedItemChanged;

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
                DynamicFunctionCallingManager.RunFunctionFromExtensions(item, MethodName);

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
         //   base.ApplyTheme();
            if (IsChild && Parent != null)
            {
                BackColor = Parent.BackColor;
                ParentBackColor = Parent.BackColor;
            }
            BackColor = _currentTheme.ButtonBackColor;
            ForeColor = _currentTheme.ButtonForeColor;
            HoverBackColor = _currentTheme.ButtonHoverBackColor;
            HoverForeColor = _currentTheme.ButtonHoverForeColor;
            DisabledBackColor=_currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;
            FocusBackColor = _currentTheme.ButtonSelectedBackColor;
            FocusForeColor = _currentTheme.ButtonSelectedForeColor;
            
          
            PressedBackColor = _currentTheme.ButtonPressedBackColor;
            PressedForeColor = _currentTheme.ButtonPressedForeColor;

            //  if (_beepListBox != null)   _beepListBox.Theme = Theme;
            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                 Font = _textFont;
            }
           
           
            ApplyThemeToSvg();
            Invalidate();  // Trigger repaint
        }
        public void ApplyThemeToSvg()
        {

            if (beepImage != null) // Safely apply theme to beepImage
            {
                beepImage.ApplyThemeOnImage = ApplyThemeOnImage;
                if (ApplyThemeOnImage)
                {
                   
         //           beepImage.ImageEmbededin = ImageEmbededin.Button;
           //         beepImage.Theme = Theme;
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
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (_autoSize)
            {
                // Compute what size we *should* be.
                Size preferred = GetPreferredSize(Size.Empty);

                // Update the actual Value if it differs
                if (Size != preferred)
                {
                    this.Size = preferred;
                }
            }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_autoSize)
            {
                // Compute what size we *should* be.
                Size preferred = GetPreferredSize(Size.Empty);

                // Update the actual Value if it differs
                if (Size != preferred)
                {
                    this.Size = preferred;
                }
            }
            UpdateDrawingRect();
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
            // First, call the base drawing code from BeepControl.
            base.DrawContent(g);

            // Add BeepAppBar-specific drawing here.
            // For instance, if you need to draw additional borders or background
            // elements that are unique to the app bar, add them below.

            // Example: Draw a line under the app bar.
            // Do not call base.OnPaint(e);
            
            UpdateDrawingRect();
            // Determine the fill color based on control state
            Color fillColor;
            // Draw the image and text
            contentRect = DrawingRect;



            // Now, if the splash effect is active, draw the ripple:
            if (splashActive)
            {
                // Calculate the current radius based on the animation progress.
                float currentRadius = splashProgress * MaxSplashRadius;
                // Compute an alpha value so that the ripple fades out (adjust the 150 value for intensity):
                int alpha = (int)((1f - splashProgress) * 150);
                if (alpha > 255) alpha = 255;
                if (alpha < 0) alpha = 0;

                // Create a brush with the calculated alpha (using a color of your choice, here we use Gray).
                using (SolidBrush rippleBrush = new SolidBrush(Color.FromArgb(alpha, Color.Gray)))
                {
                    // Calculate the rectangle for the splash circle centered on splashCenter.
                    Rectangle rippleRect = new Rectangle(
                        (int)(splashCenter.X - currentRadius),
                        (int)(splashCenter.Y - currentRadius),
                        (int)(2 * currentRadius),
                        (int)(2 * currentRadius));

                    // Optionally clip the drawing to the button bounds.
                    g.SetClip(DrawingRect);
                    // Draw the ripple using an ellipse.
                    g.FillEllipse(rippleBrush, rippleRect);
                    g.ResetClip();
                }
            }
            //else
            //{
            //    if (!Enabled)
            //    {
            //        fillColor = DisabledBackColor;
            //    }
            //    else if (IsSelected)
            //    {
            //        // Selected state: use BackColor (which is set in IsSelected setter)
            //        fillColor = SelectedBackColor;
            //    }
            //    else if (IsHovered)
            //    {
            //        fillColor = HoveredBackcolor;
            //    }
            //    else
            //    {
            //        fillColor = BackColor;
            //    }

            //    //  Rectangle outerRectangle = new Rectangle(0, 0, Width, Height);

            //    using (SolidBrush brush = new SolidBrush(fillColor))
            //    {
            //        g.FillRectangle(brush, DrawingRect);
            //    }
            //}

            DrawImageAndText(g);

            // If additional app bar elements (icons, text) require custom drawing,
            // they can be added here or left to be drawn by the child controls.
        }

        private void DrawImageAndText(Graphics g)
        {
           //// Console.WriteLine($"User ThemeFont is {UseThemeFont}");
            //if (!SetFont() && UseThemeFont)
            //{
            //    _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            //};
            // Measure and scale the font to fit within the control bounds
            Font scaledFont = _textFont;// GetScaledFont(g, Text, contentRect.Value, TextFont);
            if (UseScaledFont)
            {
                scaledFont = GetScaledFont(g, Text, contentRect.Size, _textFont);
            }
            Size imageSize = beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;

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
            //Console.WriteLine("Drawing ImagePath 1");
            // Draw the image if available
            if (beepImage != null && beepImage.HasImage)
            {
                if (beepImage.Size.Width > this.Size.Width || beepImage.Size.Height > this.Size.Height)
                {
                    imageSize = this.Size;
                }
                beepImage.MaximumSize = imageSize;
                beepImage.Size = imageRect.Size;
                beepImage.DrawImage(g, imageRect);
                if(beepImageHitTest == null)
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
            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                //if (IsPressed && !IsSelected)
                //{
                //    ForeColor = PressedForeColor;

                //}
                //else
                Color color = ForeColor;
                if (IsHovered)
                {
                    color = HoverForeColor;

                }
                else if (IsSelected)
                {
                    color = SelectedForeColor;
                }
                else if (!Enabled)
                {
                    color = DisabledForeColor;
                }
                else
                {
                    color = ForeColor;
                }

                TextFormatFlags flags = GetTextFormatFlags(TextAlign);
                TextRenderer.DrawText(g, Text, scaledFont, textRect, color, flags);
            }
            if(BadgeText != null)
            {
                DrawBadge(g);
            }
           
            //}
        }
        public override void Draw(Graphics g, Rectangle rectangle)
        {
            contentRect = rectangle;
            DrawImageAndText(g);
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

            var ev = new BeepEventDataArgs("ImageClicked", this);

            ImageClicked?.Invoke(this, ev);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
          //  IsSelected = false;
            // Start the splash effect: record the click location and reset progress
            splashCenter = e.Location; // Use the mouse click location for the ripple's center
            splashProgress = 0f;
            splashActive = true;
            splashTimer.Start();
            if (IsSelectedAuto)
            {
                IsSelected = !IsSelected;

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

    }


}
