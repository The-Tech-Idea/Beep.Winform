using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using Svg;

using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Button))]
    [Category("Controls")]
    public class BeepButton : BeepControl
    {
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


        // Public properties

       
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
                if (value) {
                    beepImage.Theme = Theme;
                    if (ApplyThemeOnImage)
                    {
                        beepImage.ApplyThemeOnImage = true;
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
                    beepImage.ApplyThemeToSvg();
                    beepImage.ApplyTheme();
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


        // Updated Property to use theme for hover color


        // Constructor
        public BeepButton()
        {
            InitializeComponents();
            ApplyTheme();
            
           
           
        }


        private void InitializeComponents()
        {

            beepImage = new BeepImage
            {
                Dock = DockStyle.None, // We'll manually position it
                Margin = new Padding(0),
                Location = new Point(5, 5), // Set initial position (will adjust in layout)
                Size = _maxImageSize // Set the size based on the max image size
            };
            beepImage.MouseHover += BeepImage_MouseHover;
         //   beepImage.MouseLeave += BeepImage_MouseLeave;
            IsChild = false;
            beepImage.Click += BeepImage_Click;
            Padding = new Padding(2);
            Margin = new Padding(0);
            Size = new Size(120, 40);  // Default size
        }

    

        public override void ApplyTheme()
        {
            BackColor = _currentTheme.BackgroundColor;
            ForeColor = _currentTheme.ButtonForeColor;

        

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
        public  void ApplyThemeToSvg()
        {

            if (beepImage != null) // Safely apply theme to beepImage
            {
                beepImage.ApplyThemeOnImage = true;
                beepImage.Theme = Theme;
                if (ApplyThemeOnImage)
                {
                    beepImage.ApplyThemeToSvg();
                }

            }
           

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Do not call base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            UpdateDrawingRect();
            // Draw the image and text
            contentRect = DrawingRect;
            contentRect.Inflate(-Padding.Left - Padding.Right, -Padding.Top - Padding.Bottom);
            DrawBackColor(e, _currentTheme.ButtonBackColor,_currentTheme.ButtonHoverBackColor);
            DrawImageAndText(e.Graphics);
        }
        private void DrawImageAndText(Graphics g)
        { 
            Font = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            // Measure and scale the font to fit within the control bounds
            Font scaledFont = GetScaledFont(g, Text, contentRect.Size, Font);
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
            //Console.WriteLine("Drawing Image 1");
            // Draw the image if available
            if (beepImage != null && beepImage.HasImage)
            {
                //Console.WriteLine("Loading Image 2 333");
                beepImage.DrawImage(g, imageRect);
                // place beepimage in the same place imagerect is
                beepImage.Location = imageRect.Location;
            }
           

            //Console.WriteLine("Font changed  3");
            // Draw the text
            // Draw the text
            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                TextFormatFlags flags = GetTextFormatFlags(TextAlign);
                TextRenderer.DrawText(g, Text, scaledFont, textRect, _currentTheme.ButtonForeColor, flags);
            }

            //}
        }
        public void DrawToGraphics(Graphics g, Rectangle rectangle) {
            contentRect = rectangle;
            DrawImageAndText(g);
        }
        private void CalculateLayout(Rectangle contentRect, Size imageSize, Size textSize, out Rectangle imageRect, out Rectangle textRect)
        {
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;

            // Handle the case where there is no image
            bool hasImage = imageSize != Size.Empty;
            bool hasText = !string.IsNullOrEmpty(Text) && !HideText; // Check if text is available and not hidden

            // Adjust contentRect for padding
         //   contentRect.Inflate(-Padding.Left - Padding.Right, -Padding.Top - Padding.Bottom);

            if (!hasText && hasImage)
            {
                // If there's no text but an image, center the image
                imageRect = AlignRectangle(contentRect, imageSize, ContentAlignment.MiddleCenter);
            }
            else
            {
                // Layout logic based on TextImageRelation when both text and image are present
                switch (this.TextImageRelation)
                {
                    case TextImageRelation.Overlay:
                        // Image and text overlap
                        if (hasImage)
                        {
                            imageRect = AlignRectangle(contentRect, imageSize, this.ImageAlign);
                        }
                        if (hasText)
                        {
                            textRect = AlignRectangle(contentRect, textSize, this.TextAlign);
                        }
                        break;

                    case TextImageRelation.ImageBeforeText:
                        if (hasImage)
                        {
                            Rectangle imageArea = new Rectangle(contentRect.Location, new Size(imageSize.Width, contentRect.Height));
                            imageRect = AlignRectangle(imageArea, imageSize, this.ImageAlign);
                        }
                        if (hasText)
                        {
                            Rectangle textArea = new Rectangle(
                                contentRect.X + imageSize.Width,
                                contentRect.Y,
                                contentRect.Width - imageSize.Width,
                                contentRect.Height);
                            textRect = AlignRectangle(textArea, textSize, this.TextAlign);
                        }
                        break;

                    case TextImageRelation.TextBeforeImage:
                        if (hasText)
                        {
                            Rectangle textArea = new Rectangle(contentRect.Location, new Size(textSize.Width, contentRect.Height));
                            textRect = AlignRectangle(textArea, textSize, this.TextAlign);
                        }
                        if (hasImage)
                        {
                            Rectangle imageArea = new Rectangle(
                                contentRect.X + textSize.Width,
                                contentRect.Y,
                                contentRect.Width - textSize.Width,
                                contentRect.Height);
                            imageRect = AlignRectangle(imageArea, imageSize, this.ImageAlign);
                        }
                        break;

                    case TextImageRelation.ImageAboveText:
                        if (hasImage)
                        {
                            Rectangle imageArea = new Rectangle(contentRect.Location, new Size(contentRect.Width, imageSize.Height));
                            imageRect = AlignRectangle(imageArea, imageSize, this.ImageAlign);
                        }
                        if (hasText)
                        {
                            Rectangle textArea = new Rectangle(
                                contentRect.X,
                                contentRect.Y + imageSize.Height,
                                contentRect.Width,
                                contentRect.Height - imageSize.Height);
                            textRect = AlignRectangle(textArea, textSize, this.TextAlign);
                        }
                        break;

                    case TextImageRelation.TextAboveImage:
                        if (hasText)
                        {
                            Rectangle textArea = new Rectangle(contentRect.Location, new Size(contentRect.Width, textSize.Height));
                            textRect = AlignRectangle(textArea, textSize, this.TextAlign);
                        }
                        if (hasImage)
                        {
                            Rectangle imageArea = new Rectangle(
                                contentRect.X,
                                contentRect.Y + textSize.Height,
                                contentRect.Width,
                                contentRect.Height - textSize.Height);
                            imageRect = AlignRectangle(imageArea, imageSize, this.ImageAlign);
                        }
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
        public Size GetSuitableSize()
        {
            // Measure the text size based on the current font
            Size textSize = TextRenderer.MeasureText(Text, Font);

            // Get the original image size and limit it by the MaxImageSize property if necessary
            Size imageSize = beepImage != null && beepImage.HasImage ? beepImage.GetImageSize() : Size.Empty;

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

     
      

        private void BeepImage_MouseHover(object? sender, EventArgs e)
        {
            IsHovered = true;
          //  BackColor = _currentTheme.ButtonHoverBackColor;
          //  base.OnMouseHover(e);

        }
        private void BeepImage_Click(object? sender, EventArgs e)
        {
            var ev = new BeepEventDataArgs("ImageClicked", this);
            ImageClicked?.Invoke(this, ev);
            base.OnClick(e);
        }

    }


}
