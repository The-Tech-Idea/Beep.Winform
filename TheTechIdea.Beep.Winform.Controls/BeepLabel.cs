using TheTechIdea.Beep.Vis.Modules;
using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Controls")]
    public class BeepLabel : BeepControl
    {
        private BeepImage beepImage;
        private TextImageRelation textImageRelation = TextImageRelation.ImageBeforeText;
        private ContentAlignment imageAlign = ContentAlignment.MiddleLeft;
        private Size _maxImageSize = new Size(16, 16); // Default max image size
       
        // Add TextAlign property in BeepControl
        private ContentAlignment _textAlign = ContentAlignment.MiddleLeft;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Sets the alignment of the text within the control bounds.")]
        public ContentAlignment TextAlign
        {
            get => _textAlign;
            set
            {
                _textAlign = value;
                Invalidate(); // Redraw control to apply new alignment
            }
        }

       
        // Properties for customization
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Image alignment relative to text (Left or Right).")]
        public TextImageRelation TextImageRelation
        {
            get => textImageRelation;
            set
            {
                textImageRelation = value;
                Invalidate(); // Repaint on change
                //UpdateSize(); // Update size when layout changes
            }
        }
        bool _applyThemeOnImage = false;
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                if (value)
                {
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
        [Description("Align image within the BeepLabel (Left, Center, Right).")]
        public ContentAlignment ImageAlign
        {
            get => imageAlign;
            set
            {
                imageAlign = value;
                Invalidate(); // Repaint on change
               // UpdateSize();
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

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Maximum size for the image.")]
        public Size MaxImageSize
        {
            get => _maxImageSize;
            set
            {
                _maxImageSize = value;
                Invalidate(); // Repaint on change
                              //  UpdateSize(); // Adjust size when MaxImageSize changes
            }
        }
        Rectangle contentRect;
        public BeepLabel()
        {
            InitializeComponents();
            ApplyTheme();

            //  SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            AutoSize = false;
            IsChild = true;
            IsBorderAffectedByTheme = false;
            IsShadowAffectedByTheme = false;
            ShowAllBorders = false;
            ShowShadow = false;
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
          //  beepImage.MouseHover += BeepImage_MouseHover;
            //   beepImage.MouseLeave += BeepImage_MouseLeave;
            IsChild = false;
          //  beepImage.Click += BeepImage_Click;
            Padding = new Padding(2);
            Margin = new Padding(0);
            Size = new Size(120, 40);  // Default size
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
            DrawBackColor(e, _currentTheme.LabelBackColor, _currentTheme.ButtonHoverBackColor);
            DrawToGraphics(e.Graphics);
        }
        private void DrawToGraphics(Graphics g)
        {
            DrawToGraphics(g, contentRect);
        }
        public void DrawToGraphics(Graphics g,Rectangle drawrect)
        {
            drawrect.Inflate(-Padding.Left - Padding.Right, -Padding.Top - Padding.Bottom);
            Font = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            // Measure and scale the font to fit within the control bounds
            Font scaledFont = GetScaledFont(g, Text, drawrect.Size, Font);
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
            CalculateLayout(drawrect, imageSize, textSize, out imageRect, out textRect);
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
            if (!string.IsNullOrEmpty(Text))
            {
                TextFormatFlags flags = GetTextFormatFlags(TextAlign);
                TextRenderer.DrawText(g, Text, scaledFont, textRect, _currentTheme.LabelForeColor, flags);
            }

            //}
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

        private void CalculateLayout(Rectangle contentRect, Size imageSize, Size textSize, out Rectangle imageRect, out Rectangle textRect)
        {
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;

            // Handle the case where there is no image
            bool hasImage = imageSize != Size.Empty;
            bool hasText = !string.IsNullOrEmpty(Text) ; // Check if text is available and not hidden

            // Adjust drawrect for padding
            //   drawrect.Inflate(-Padding.Left - Padding.Right, -Padding.Top - Padding.Bottom);

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

 
        
        public override void ApplyTheme()
        {
          //  base.ApplyTheme();
            if (_currentTheme != null)
            {
                //if (IsChild)
                //{
                //  //  Console.WriteLine("IsChild");
                //   // Console.WriteLine("ParentBackColor: " + parentbackcolor);
                //    BackColor = parentbackcolor;
                //}
                //else
                //{
                //   // Console.WriteLine("IsNotChild");
                //    BackColor = _currentTheme.LabelBackColor;
                //}

                BackColor = _currentTheme.BackgroundColor;
                ForeColor = _currentTheme.LabelForeColor;
               // Font = BeepThemesManager.ToFont(_currentTheme.BodySmall);
                beepImage.Theme = Theme;
               // Invalidate();
            }
        }

        // Dynamically calculate the preferred size based on text and image sizes
        public override Size GetPreferredSize(Size proposedSize)
        {
            //if (AutoSize)
            //{
               Font = BeepThemesManager.ToFont(_currentTheme?.BodySmall) ?? Font;
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



      


    }
}
