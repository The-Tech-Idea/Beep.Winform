﻿using TheTechIdea.Beep.Vis.Modules;
using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Controls")]
    public class BeepLabel : BeepControl
    {
        #region "Properties"
        private BeepImage beepImage;
        private TextImageRelation textImageRelation = TextImageRelation.ImageBeforeText;
        private ContentAlignment imageAlign = ContentAlignment.MiddleLeft;
        private Size _maxImageSize = new Size(16, 16); // Default max image size
        private bool _hideText = false;
        int offset = 3;


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
        [Description("ImagePath alignment relative to text (Left or Right).")]
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
                if (beepImage == null)
                {
                    beepImage = new BeepImage();

                }
                if (beepImage != null)
                {
                    beepImage.ImagePath = value;
                    if (ApplyThemeOnImage)
                    {
                        beepImage.Theme = Theme;
                        beepImage.ApplyThemeOnImage = true;
                        beepImage.ApplyThemeToSvg();
                        beepImage.ApplyTheme();
                    }
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
        private int padding;
        private int spacing;
        #endregion "Properties"

        public BeepLabel()
        {
            InitializeComponents();
            ApplyTheme();
           
            //  SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            AutoSize = false;
            BoundProperty = "Text";
            //  UpdateSize();

        }
        protected override Size DefaultSize => new Size(100,GetSingleLineHeight());
        private int GetSingleLineHeight()
        {
            // Ensure DrawingRect is updated
            UpdateDrawingRect();
            int textBoxHeight;
            padding = BorderThickness + offset;
            spacing = 5;
            using (Label tempTextBox = new Label())
            {
                tempTextBox.BorderStyle = BorderStyle.None;
                tempTextBox.Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall);

                textBoxHeight = tempTextBox.PreferredHeight + (padding * 2);

                // Calculate the total height, including borders and padding
            }


            return textBoxHeight;
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
    
                // Update DrawingRect to get accurate measurements
                UpdateDrawingRect();

                int singleLineHeight = GetSingleLineHeight();

                // Set Minimum and Maximum height to enforce fixed height
                this.MinimumSize = new Size(0, singleLineHeight);
                this.MaximumSize = new Size(0, singleLineHeight);

                height = singleLineHeight;
                specified &= ~BoundsSpecified.Height; // Remove the Height flag to prevent external changes
            
          

            base.SetBoundsCore(x, y, width, height, specified);
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
          //  beepImage.MouseHover += BeepImage_MouseHover;
            //   beepImage.MouseLeave += BeepImage_MouseLeave;
          
          //  beepImage.Click += BeepImage_Click;
            Padding = new Padding(0);
            Margin = new Padding(0);
            IsChild = true;
         
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            UpdateDrawingRect();
            // Draw the image and text
            contentRect = DrawingRect;
           // contentRect.Inflate(-Padding.Left - Padding.Right, -Padding.Top - Padding.Bottom);
          //  DrawBackColor(e, _currentTheme.BackColor, _currentTheme.ButtonHoverBackColor);
            DrawToGraphics(e.Graphics);
        }
        private void DrawToGraphics(Graphics g)
        {
            DrawToGraphics(g, contentRect);
        }
        public void DrawToGraphics(Graphics g,Rectangle drawrect)
        {
            //drawrect.Inflate(-Padding.Left - Padding.Right, -Padding.Top - Padding.Bottom);
           
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

            Size textSize = TextRenderer.MeasureText(Text, Font);

            // Calculate the layout of image and text
            Rectangle imageRect, textRect;
            CalculateLayout(drawrect, imageSize, textSize, out imageRect, out textRect);
            //Console.WriteLine("Drawing ImagePath 1");
            // Draw the image if available
            if (beepImage != null && beepImage.HasImage)
            {
                beepImage.MaximumSize = imageSize;
                beepImage.Size = imageRect.Size;
               // Console.WriteLine("Label show ImagePath");
                beepImage.DrawImage(g, imageRect);
                // place beepimage in the same place imagerect is
                //beepImage.Location = imageRect.Location;
            }


          // Draw the text
            if (!string.IsNullOrEmpty(Text))
            {
                Color textColor = IsHovered? _currentTheme.HoverLinkColor: _currentTheme.LabelForeColor;
                TextFormatFlags flags = GetTextFormatFlags(TextAlign);
                TextRenderer.DrawText(g, Text, Font, textRect, textColor, flags);
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
        public override void ApplyTheme()
        {
          //  base.ApplyTheme();
            if (_currentTheme != null)
            {
                BackColor = _currentTheme.BackgroundColor;
                ForeColor = _currentTheme.LabelForeColor;
                HoverBackColor = _currentTheme.ButtonHoverBackColor;
                HoverForeColor = _currentTheme.ButtonHoverForeColor;
                if (IsChild)
                {
                    ForeColor = _currentTheme.LabelForeColor;

                }

                try
                {
                    Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                }
                catch (Exception ex)
                {
                    Font = BeepThemesManager.ToFont(_currentTheme.FontFamily, _currentTheme.FontSize, FontWeight.Normal, FontStyle.Regular);
                }

                ApplyThemeToSvg();
                Invalidate();
            }
        }
        public void ApplyThemeToSvg()
        {

            if (beepImage != null) // Safely apply theme to beepImage
            {

               

                if (ApplyThemeOnImage)
                {
                  
                    beepImage.Theme = Theme;
                    beepImage.BackColor = BackColor;
                    if (IsChild)
                    {
                        beepImage.ForeColor = _currentTheme.LabelForeColor;
                    }
                    else
                    {
                        beepImage.ForeColor = _currentTheme.LabelForeColor;
                    }
                    beepImage.ApplyThemeToSvg();
                }

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

        protected override void OnMouseHover(EventArgs e)
        {
            IsHovered = true;
            
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            IsHovered = false;
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            IsHovered = true;
        }




    }
}
