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
                UpdateSize(); // Update size when layout changes
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
                UpdateSize();
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
                UpdateSize(); // Adjust size when MaxImageSize changes
            }
        }

        public BeepLabel()
        {
            InitializeComponents();
            Padding = new Padding(0);
            Margin = new Padding(0);
            //  SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            AutoSize = false;
        }

        private void InitializeComponents()
        {
            beepImage = new BeepImage
            {
                Dock = DockStyle.None,
                Margin = new Padding(0),
                Size = _maxImageSize
            };
        }
        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    // Do not call base.OnPaintBackground(e);
        //    // Fill the background with the control's background color

        //    if (IsChild)
        //    {
        //        using (SolidBrush brush = new SolidBrush(parentbackcolor))
        //        {
        //            e.Graphics.FillRectangle(brush, DrawingRect);
        //        }
        //    }
        //    else
        //    {
        //        using (SolidBrush brush = new SolidBrush(BackColor))
        //        {
        //            e.Graphics.FillRectangle(brush, DrawingRect);
        //        }
        //    }
        //}
        protected override void OnPaint(PaintEventArgs e)
        {
           
            base.OnPaint(e);
           
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Font = BeepThemesManager.ToFont(_currentTheme?.BodySmall) ?? Font;
            Size textSize = TextRenderer.MeasureText(Text, Font);
            Size imageSize = beepImage?.HasImage == true ? beepImage.GetImageSize() : Size.Empty;

            // Scale image to fit within MaxImageSize if necessary
            if (imageSize.Width > _maxImageSize.Width || imageSize.Height > _maxImageSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)_maxImageSize.Width / imageSize.Width,
                    (float)_maxImageSize.Height / imageSize.Height);
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            // Calculate layout for text and image
            Rectangle textRect, imageRect;
            CalculateLayout(e.Graphics, textSize, imageSize, out textRect, out imageRect);
            // Ensure text fits within control bounds by applying clipping
           // textRect.Intersect(ClientRectangle);
            // Draw image if available
            if (beepImage != null && beepImage.HasImage)
            {
                beepImage.DrawImage(e.Graphics, imageRect);
            }

            // Draw text
            var textColor = _currentTheme?.LabelForeColor ?? ForeColor;
            ForeColor = textColor;
            TextRenderer.DrawText(e.Graphics, Text, Font, textRect, textColor); //          TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsClipping


        }
        public void DrawToGraphics(Graphics graphics, Rectangle bounds)
        {
            
            // Use the theme font or control font
            Font = BeepThemesManager.ToFont(_currentTheme?.BodySmall);
            Size textSize = TextRenderer.MeasureText(Text, Font);
            Size imageSize = beepImage?.HasImage == true ? beepImage.GetImageSize() : Size.Empty;

            // Scale image to fit within MaxImageSize if needed
            if (imageSize.Width > MaxImageSize.Width || imageSize.Height > MaxImageSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)MaxImageSize.Width / imageSize.Width,
                    (float)MaxImageSize.Height / imageSize.Height);
                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            // Calculate layout for text and image
            CalculateLayout(graphics, textSize, imageSize, out Rectangle textRect, out Rectangle imageRect);

            // Offset by the bounds
           // textRect.Offset(bounds.Location);
           // imageRect.Offset(bounds.Location);
            // Ensure text fits within control bounds by applying clipping
           // textRect.Intersect(ClientRectangle);
            // Draw image if available
            if (beepImage != null && beepImage.HasImage)
            {
                beepImage.DrawImage(graphics, imageRect);
            }

            // Draw text
            var textColor = _currentTheme?.LabelForeColor ?? ForeColor;
            ForeColor = textColor;
            TextRenderer.DrawText(graphics, Text, Font, textRect, textColor,
                          TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsClipping);
            // Show tooltip if text is truncated
            if (textSize.Width > textRect.Width || textSize.Height > textRect.Height)
            {
                ShowToolTip( Text);
            }
            else
            {
                ToolTipText = string.Empty;
            }
        }

        private void CalculateLayout(Graphics graphics, Size textSize, Size imageSize, out Rectangle textRect, out Rectangle imageRect)
        {
            int padding = 0;
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;

            Rectangle contentRect = DrawingRect; // Use DrawRect instead of ClientRectangle
            contentRect.Inflate(-1, -1);
            if (beepImage != null && beepImage.HasImage)
            {
                switch (TextImageRelation)
                {
                    case TextImageRelation.ImageBeforeText:
                        imageRect = new Rectangle(contentRect.Left + padding, (contentRect.Height - imageSize.Height) / 2, imageSize.Width, imageSize.Height);
                        textRect = AlignText(new Rectangle(imageRect.Right + padding, contentRect.Top, contentRect.Width - imageRect.Width - padding, contentRect.Height), textSize, TextAlign);
                        break;

                    case TextImageRelation.TextBeforeImage:
                        textRect = AlignText(new Rectangle(contentRect.Left + padding, contentRect.Top, contentRect.Width - imageSize.Width - padding * 2, contentRect.Height), textSize, TextAlign);
                        imageRect = new Rectangle(textRect.Right + padding, (contentRect.Height - imageSize.Height) / 2, imageSize.Width, imageSize.Height);
                        break;

                    case TextImageRelation.Overlay:
                        imageRect = AlignText(contentRect, imageSize, ImageAlign);
                        textRect = AlignText(contentRect, textSize, TextAlign);
                        break;
                }
            }
            else
            {
                // Align text within DrawRect when no image is present
                textRect = AlignText(contentRect, textSize, TextAlign);
            }
        }

        // Helper method to align text within the control
        // Updated helper method to align text within the control bounds based on alignment


        private Rectangle AlignText(Rectangle container, Size textSize, ContentAlignment alignment)
        {
            int x = container.Left;
            int y = container.Top;

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                    break;
                case ContentAlignment.TopCenter:
                    x = container.Left + (container.Width - textSize.Width) / 2;
                    break;
                case ContentAlignment.TopRight:
                    x = container.Right - textSize.Width;
                    break;
                case ContentAlignment.MiddleLeft:
                    y = container.Top + (container.Height - textSize.Height) / 2;
                    break;
                case ContentAlignment.MiddleCenter:
                    x = container.Left + (container.Width - textSize.Width) / 2;
                    y = container.Top + (container.Height - textSize.Height) / 2;
                    break;
                case ContentAlignment.MiddleRight:
                    x = container.Right - textSize.Width;
                    y = container.Top + (container.Height - textSize.Height) / 2;
                    break;
                case ContentAlignment.BottomLeft:
                    y = container.Bottom - textSize.Height;
                    break;
                case ContentAlignment.BottomCenter:
                    x = container.Left + (container.Width - textSize.Width) / 2;
                    y = container.Bottom - textSize.Height;
                    break;
                case ContentAlignment.BottomRight:
                    x = container.Right - textSize.Width;
                    y = container.Bottom - textSize.Height;
                    break;
            }

            return new Rectangle(new Point(x, y), textSize);
        }


        
        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            if (_currentTheme != null)
            {
                if (IsChild)
                {
                  //  Console.WriteLine("IsChild");
                   // Console.WriteLine("ParentBackColor: " + parentbackcolor);
                    BackColor = parentbackcolor;
                }
                else
                {
                   // Console.WriteLine("IsNotChild");
                    BackColor = _currentTheme.LabelBackColor;
                }

                ForeColor = _currentTheme.LabelForeColor;
                Font = BeepThemesManager.ToFont(_currentTheme.BodySmall);
                beepImage.Theme = Theme;
                Invalidate();
            }
        }

        // Dynamically calculate the preferred size based on text and image sizes
        public override Size GetPreferredSize(Size proposedSize)
        {
            if (AutoSize)
            {
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

                // Use CalculateLayout to get the correct positioning for text and image
                CalculateLayout(null, textSize, imageSize, out Rectangle textRect, out Rectangle imageRect);

                // Calculate the total width and height required for text and image with padding
                int width = Math.Max(textRect.Right, imageRect.Right) + Padding.Left + Padding.Right;
                int height = Math.Max(textRect.Bottom, imageRect.Bottom) + Padding.Top + Padding.Bottom;

                return new Size(width, height);
            }

            // Return the control's current size if AutoSize is disabled
            return base.Size;
        }



        private void UpdateSize()
        {
            if (AutoSize)
            {
                Size preferredSize = GetPreferredSize(Size.Empty);
                Size = preferredSize;
                Invalidate(); // Ensure repaint to reflect new size
            }
        }


    }
}
