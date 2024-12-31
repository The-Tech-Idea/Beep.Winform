using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Desktop.Controls.Common;

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
        private FlatStyle _flatStyle = FlatStyle.Standard;
        private bool _flatAppearanceEnabled = true;

        private bool _isSideMenuChild = false;
        // Private field to store the button's text

        private Rectangle contentRect;

        // create a public event for beepImage.onClick
        public EventHandler<BeepEventDataArgs> ImageClicked { get; set; }
        private Color tmpbackcolor;
        private Color tmpforcolor;

        #region "Popup List Properties"
        private BeepPopupForm _popupForm;
        private BeepListBox _beepListBox;
        private bool _isPopupOpen;
        private bool _popupmode = false;
        private int _maxListHeight = 100;
        private int _maxListWidth = 100;
        public event EventHandler SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(EventArgs e)
            => SelectedItemChanged?.Invoke(this, e);

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
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => _beepListBox.ListItems;
            set => _beepListBox.ListItems = value;
        }
        // The item currently chosen by the user
        private SimpleItem _selectedItem;
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            private set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnSelectedItemChanged(EventArgs.Empty);
                }
            }
        }
        [Browsable(false)]
        public int SelectedIndex
        {
            get => _beepListBox.SelectedIndex;
            set
            {
                if (value >= 0 && value < _beepListBox.ListItems.Count)
                {
                    _beepListBox.SelectedIndex = value;
                    SelectedItem = _beepListBox.ListItems[value];
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
                if (_popupForm != null)
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
        #endregion "Popup List Properties"
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
        private bool isSelectedAuto = true;
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
                        beepImage.Theme = Theme;

                        beepImage.ApplyThemeToSvg();

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
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                Invalidate();  // Trigger repaint
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public Color SelectedBorderColor
        {
            get => selectedBorderColor;
            set
            {
                selectedBorderColor = value;
                Invalidate();  // Trigger repaint
            }
        }
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
        [Browsable(true)]
        [Category("Behavior")]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                if (_isSelected)
                {
                    tmpbackcolor = BackColor;
                    tmpforcolor = ForeColor;
                    BackColor = _currentTheme.ButtonActiveBackColor;
                    ForeColor = _currentTheme.ButtonActiveForeColor;
                }
                else
                {
                    BackColor = tmpbackcolor;
                    ForeColor = tmpforcolor;
                }
                Invalidate(); // Repaint to reflect selection state
            }
        }
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
        [Browsable(true)]
        [Category("Appearance")]
        public FlatStyle FlatStyle
        {
            get => _flatStyle;
            set
            {
                _flatStyle = value;
                Invalidate(); // Trigger repaint based on the FlatStyle
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool FlatAppearance
        {
            get => _flatAppearanceEnabled;
            set
            {
                _flatAppearanceEnabled = value;
                Invalidate(); // Trigger repaint based on the flat appearance
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
        #endregion "Properties"
        #region "Constructor"
        // Constructor
        public BeepButton()
        {
            InitializeComponents();
            ApplyTheme();
            CanBeHovered = true;
            CanBePressed = true;
            CanBeFocused = true;
            #region "Popup List Initialization"
            IsChild= false;
            // Initialize the popup form and beepListBox
            // 1) Create beepListBox
        
            #endregion "Popup List Initialization"
        }
        private void InitializeComponents()
        {

            beepImage = new BeepImage
            {
                Dock = DockStyle.None, // We'll manually position it
                Margin = new Padding(0),
                Location = new Point(0, 0), // Set initial position (will adjust in layout)
                Size = _maxImageSize // Set the size based on the max image size
            };
            beepImage.MouseHover += BeepImage_MouseHover;
            beepImage.MouseEnter += BeepImage_MouseEnter;
            //   beepImage.MouseLeave += BeepImage_MouseLeave;
            IsChild = false;
            beepImage.Click += BeepImage_Click;
            Padding = new Padding(0);
            Margin = new Padding(0);
            Size = new Size(120, 40);  // Default size
            InitListbox();
                                       //  Controls.Add(beepImage);
        }
        private void InitListbox()
        {
            // Rebuild beepListBox's layout
            _beepListBox = new BeepListBox
            {
                TitleText = "Select an item",
                ShowTitle = false,
                ShowTitleLine = false,
                Width = _maxListWidth,
                Height = _maxListHeight,
                ShowAllBorders = false,
                IsBorderAffectedByTheme = false,
                IsRoundedAffectedByTheme = false,
                IsShadowAffectedByTheme = false,

            };
            _beepListBox.ItemClicked += (sender, item) =>
            {
                SelectedItem = item;
                ClosePopup();
            };
        }
        #endregion "Constructor"
        #region "Popup List Methods"
        private void TogglePopup()
        {
            if (_isPopupOpen)
                ClosePopup();
            else
                ShowPopup();
        }
        private void ShowPopup()
        {
            if (_isPopupOpen) return;
            // Always create a new instance from scratch
            _popupForm = new BeepPopupForm();
            _popupForm.OnLeave += (sender, e) =>
            {
                _isPopupOpen = false;
                ClosePopup();
            };
            _isPopupOpen = true;
            int _maxListHeight=Width;
            int _maxListWidth=100;

            //    InitListbox();
            // 2) Create a borderless popup form
            //  _popupForm = new BeepPopupForm();
            _popupForm.BorderThickness = 1;
            _popupForm.Controls.Add(_beepListBox);
            _beepListBox.ShowHilightBox = false;
            _beepListBox.Dock = DockStyle.None;
            _beepListBox.MenuItemHeight = 15;
            _beepListBox.InitializeMenu();

            int neededHeight = _beepListBox.GetMaxHeight() ;
            int finalHeight = Math.Min(neededHeight, _maxListHeight);
            // possibly also compute width
            int finalWidth = Math.Max(Width, _maxListWidth);

            // The popup form is sized to fit beepListBox
            _popupForm.Size = new Size(finalWidth, neededHeight+5);
            // Position popup just below the main control
            var screenPoint = this.PointToScreen(new Point(0, Height+5));
            _popupForm.Location = screenPoint;
            _beepListBox.Theme = Theme;
            _beepListBox.ShowAllBorders = false;
            //_popupForm.BackColor = _currentTheme.BackColor;
            _popupForm.Theme = Theme;
            _beepListBox.Dock = DockStyle.Fill; // Manually size and position
        

            _popupForm.Show();
            _popupForm.BringToFront();
            _popupForm.Invalidate();
        }
        private void ClosePopup()
        {
            if (_isPopupOpen) return;
            _isPopupOpen = false;
            _popupForm.Hide();
        }
        #endregion "Popup List Methods"
        #region "Theme"
        public override void ApplyTheme()
        {
            BackColor = _currentTheme.ButtonBackColor;
            ForeColor = _currentTheme.ButtonForeColor;
            HoverBackColor = _currentTheme.ButtonHoverBackColor;
            HoverForeColor = _currentTheme.ButtonHoverForeColor;
            try
            {
                Font = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            }
            catch (Exception ex)
            {
                Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, _currentTheme.FontSize, FontWeight.Normal, FontStyle.Regular);
            }

            ApplyThemeToSvg();
            Invalidate();  // Trigger repaint
        }
        public void ApplyThemeToSvg()
        {

            if (beepImage != null) // Safely apply theme to beepImage
            {
                if (ApplyThemeOnImage)
                {
                    beepImage.Theme = Theme;
                    beepImage.BackColor = _currentTheme.BackColor;
                    beepImage.ForeColor = ForeColor;
                    beepImage.ApplyThemeToSvg();
                }

            }


        }
        #endregion "Theme"
        #region "Paint"
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Do not call base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            UpdateDrawingRect();

            // Draw the image and text
            contentRect = DrawingRect;
           // if (!SetFont())
           // {
            //    Font = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
          //  };
            //   DrawBackColor(e, BackColor, _currentTheme.ButtonHoverBackColor);
            DrawImageAndText(e.Graphics);
        }
        private void DrawImageAndText(Graphics g)
        {
            if (!SetFont())
            {
                Font = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            };
            // Measure and scale the font to fit within the control bounds
            Font scaledFont = Font;// GetScaledFont(g, Text, contentRect.Size, Font);
            if (UseScaledFont)
            {
                scaledFont = GetScaledFont(g, Text, contentRect.Size, Font);
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

            }
            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                TextFormatFlags flags = GetTextFormatFlags(TextAlign);
                TextRenderer.DrawText(g, Text, scaledFont, textRect, ForeColor, flags);
            }

            //}
        }
        public void DrawToGraphics(Graphics g, Rectangle rectangle)
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
            contentRect.Inflate(-Padding.Horizontal / 2, -Padding.Vertical / 2);

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
            //if (AutoSize)
            //{
            if (!SetFont())
            {
                Font = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            };

            Size textSize = TextRenderer.MeasureText(Text, Font);
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

            Rectangle textRect, imageRect;
            CalculateLayout(DrawingRect, imageSize, textSize, out imageRect, out textRect);
            // Clip text rectangle to control bounds to prevent overflow
            //  textRect.Intersect(DrawingRect);
            // Calculate the total width and height required for text and image with padding
            int width = Math.Max(textRect.Right, imageRect.Right) + Padding.Left + Padding.Right;
            int height = Math.Max(textRect.Bottom, imageRect.Bottom) + Padding.Top + Padding.Bottom;

            return new Size(width, height);
            //  }

            // Return the control's current size if AutoSize is disabled
            //return base.Size;
        }
        #endregion "Paint"
        #region "Mouse and Click"
        private void BeepImage_MouseHover(object? sender, EventArgs e)
        {
         
            //  BackColor = _currentTheme.ButtonHoverBackColor;
            base.OnMouseHover(e);

        }
       
        private void BeepImage_MouseEnter(object? sender, EventArgs e)
        {
         
            base.OnMouseEnter(e);
        }
        private void BeepImage_Click(object? sender, EventArgs e)
        {
            var ev = new BeepEventDataArgs("ImageClicked", this);

            ImageClicked?.Invoke(this, ev);
            base.OnClick(e);
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (_popupmode)
            {
                TogglePopup();
            }
            else
            {
                if (isSelectedAuto)
                {
                    IsSelected = !IsSelected;
                }
            }
           

        }
        #endregion "Mouse and Click"
    }


}
