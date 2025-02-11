using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum TextLocation
    {
        Above,
        Right,
        Left,
        Below,
        Inside
    }

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepCircularButton))] //, "BeepCircularButton.bmp"
    [Category("Beep Controls")]
    [DisplayName("Beep Circular Button")]
    [Description("A circular button control with an optional image and text.")]
    public class BeepCircularButton : BeepControl
    {
        private BeepImage beepImage;
        private BeepLabel beepLabel;
        private TextLocation _textLocation = TextLocation.Below;
        private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;
        private bool _showBorder = true;
        private bool _isForColorSet = false;
        private bool _hidetext = false;
        private const int TextPadding = 5; // Padding to prevent overlap
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
        public bool HideText
        {
            get => _hidetext;
            set
            {
                _hidetext = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        public bool IsForColorSet
        {
            get => _isForColorSet;
            set
            {
                _isForColorSet = value;
                Invalidate();
            }
        }
      


        [Browsable(true)]
        [Category("Appearance")]
        public TextLocation TextLocation
        {
            get => _textLocation;
            set
            {
                _textLocation = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public ContentAlignment TextAlign
        {
            get => _textAlign;
            set
            {
                _textAlign = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Category("Appearance")]
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
        public bool ShowBorder
        {
            get => _showBorder;
            set
            {
                _showBorder = value;
                Invalidate();
            }
        }

        public BeepCircularButton():base()
        {

            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 100;
                Height = 100;
            }
            beepImage = new BeepImage
            {
                Dock = DockStyle.None,
                Margin = new Padding(0),
            };
            beepLabel = new BeepLabel
            {
                Dock = DockStyle.None,
                Margin = new Padding(0),
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.None,
                ShowAllBorders = false,
                ShowShadow = false,
            };
            IsChild= true;
            IsShadowAffectedByTheme = false;
           // IsBorderAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            IsCustomeBorder=true;
            ApplyTheme();
            beepImage.MouseHover += BeepImage_MouseHover;
            beepImage.MouseLeave += BeepImage_MouseLeave;
            beepImage.Click += BeepImage_Click;
            beepLabel.MouseClick += BeepImage_Click;
            beepLabel.MouseHover += BeepImage_MouseHover;
            beepLabel.MouseLeave += BeepImage_MouseLeave;
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            _isframless = true;

            base.OnPaint(pevent);
             UpdateDrawingRect();
            // Calculate text rectangle first to adjust the circle bounds accordingly
            Rectangle textRect = GetTextRectangle();
            Rectangle circleBounds = GetCircleBounds(textRect);
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Calculate the circle bounds based on control size
            int diameter = Math.Min(circleBounds.Width, circleBounds.Height);

            using (Brush brush = new SolidBrush(IsHovered?_currentTheme.ButtonHoverBackColor: _currentTheme.ButtonBackColor ))
            {
                pevent.Graphics.FillEllipse(brush, circleBounds);
            }
            if (_showBorder)
            {
                using (Pen pen = new Pen(_currentTheme.ShadowColor, _borderThickness))
                {
                    circleBounds.Inflate(_borderThickness / 2, _borderThickness / 2);
                    pevent.Graphics.DrawEllipse(pen, circleBounds);
                }
            }
           
            if (IsPressed)
            {
                using (Pen pen = new Pen(_currentTheme.ButtonActiveBackColor, _borderThickness))
                {
                    circleBounds.Inflate(-_borderThickness, -_borderThickness);
                    pevent.Graphics.DrawEllipse(pen, circleBounds);
                }
            }
            // Position and set the maximum size for beepImage to fit inside the circle
            if (!string.IsNullOrEmpty(beepImage.ImagePath))
            {
                beepImage.MaximumSize = GetInscribedSquareSize( diameter );  // Constrain to circle diameter
                beepImage.Size = beepImage.MaximumSize;  // Apply the size directly to beepImage
                beepImage.Location = new Point(
                    circleBounds.X + (circleBounds.Width - beepImage.Width) / 2,
                    circleBounds.Y + (circleBounds.Height - beepImage.Height) / 2
                );

                // Render the image within the circular area
                beepImage.DrawImage(pevent.Graphics, new Rectangle(beepImage.Location, beepImage.Size));
            }
            // Draw the text inside the button if enabled
            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                TextRenderer.DrawText(pevent.Graphics, Text, Font, textRect, _currentTheme.PrimaryTextColor);
            }
            DrawBadge(pevent.Graphics);
        }
        public Size GetInscribedSquareSize(int circleDiameter)
        {
            // The side length of the largest square that fits within a circle is the circle's diameter divided by √2.
            int squareSideLength = (int)(circleDiameter / Math.Sqrt(2));

            return new Size(squareSideLength, squareSideLength);
        }
        private Rectangle GetCircleBounds(Rectangle textRect)
        {
            int maxDiameter = Math.Min(
                DrawingRect.Width - Padding.Horizontal,
                DrawingRect.Height - Padding.Vertical
            );

            int diameter = (!HideText && (_textLocation == TextLocation.Above || _textLocation == TextLocation.Below))
                ? Math.Max(0, maxDiameter - textRect.Height - TextPadding) // Include padding here
                : maxDiameter;

            int x = DrawingRect.X + Padding.Left + (DrawingRect.Width - diameter) / 2;
            int y = DrawingRect.Y + Padding.Top + (DrawingRect.Height - diameter) / 2;

            if (!HideText)
            {
                y += _textLocation switch
                {
                    TextLocation.Below => -textRect.Height / 2 - TextPadding / 2, // Adjust up for below
                    TextLocation.Above => textRect.Height / 2 + TextPadding / 2,  // Adjust down for above
                    _ => 0 // No adjustment for other locations
                };
            }

            return new Rectangle(x, y, Math.Max(0, diameter), Math.Max(0, diameter));
        }

        private Rectangle GetImageRectangle(Rectangle circleBounds)
        {
            // Define the maximum size as the circle's diameter
            int diameter = Math.Min(circleBounds.Width, circleBounds.Height);
            beepImage.MaximumSize = new Size(diameter, diameter);

            // Get the original image size
            Size originalSize = beepImage.GetImageSize();
            if (originalSize.IsEmpty) return Rectangle.Empty;

            // Calculate the scale factor to fit within MaximumSize, keeping aspect ratio
            float scale = Math.Min((float)beepImage.MaximumSize.Width / originalSize.Width,
                                   (float)beepImage.MaximumSize.Height / originalSize.Height);

            // Apply the scale factor to the image dimensions
            int scaledWidth = (int)(originalSize.Width * scale);
            int scaledHeight = (int)(originalSize.Height * scale);

            // Center the image within the circle bounds
            int x = circleBounds.X + (circleBounds.Width - scaledWidth) / 2;
            int y = circleBounds.Y + (circleBounds.Height - scaledHeight) / 2;

            return new Rectangle(x, y, scaledWidth, scaledHeight);
        }
        private Rectangle GetTextRectangle()
        {
            if (HideText) return Rectangle.Empty; // Return an empty rectangle if text is hidden

            Size textSize = TextRenderer.MeasureText(Text, Font);
            Rectangle textRect = new Rectangle();

            switch (_textLocation)
            {
                case TextLocation.Above:
                    textRect = new Rectangle(
                        (DrawingRect.Width - textSize.Width) / 2,
                        DrawingRect.Top + TextPadding, // Add padding from the top
                        textSize.Width,
                        textSize.Height
                    );
                    break;
                case TextLocation.Below:
                    textRect = new Rectangle(
                        (DrawingRect.Width - textSize.Width) / 2,
                        DrawingRect.Bottom - textSize.Height - TextPadding, // Add padding from the bottom
                        textSize.Width,
                        textSize.Height
                    );
                    break;
                case TextLocation.Left:
                    textRect = new Rectangle(
                        TextPadding, // Add padding from the left
                        (DrawingRect.Height - textSize.Height) / 2,
                        textSize.Width,
                        textSize.Height
                    );
                    break;
                case TextLocation.Right:
                    textRect = new Rectangle(
                        DrawingRect.Width - textSize.Width - TextPadding, // Add padding from the right
                        (DrawingRect.Height - textSize.Height) / 2,
                        textSize.Width,
                        textSize.Height
                    );
                    break;
                case TextLocation.Inside:
                    textRect = new Rectangle(
                        (DrawingRect.Width - textSize.Width) / 2,
                        (DrawingRect.Height - textSize.Height) / 2,
                        textSize.Width,
                        textSize.Height
                    );
                    break;
            }
            return textRect;
        }
        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            //TextFont=BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            if(UseThemeFont)    Font = _currentTheme.GetCaptionFont();
            
            ForeColor = _currentTheme.ButtonForeColor;
            BackColor = _currentTheme.ButtonBackColor;
            beepLabel.Theme = Theme;
            if (ApplyThemeOnImage)
            {
                beepImage.ApplyThemeOnImage = true;
               
                
            }
            beepImage.Theme = Theme;
            beepImage.ForeColor = _currentTheme.ButtonForeColor;
            Invalidate();
        }
        private void BeepImage_MouseLeave(object? sender, EventArgs e)
        {
            IsHovered = false;
            BackColor = _currentTheme.ButtonBackColor;
            base.OnMouseLeave(e);

        }
        private void BeepImage_MouseHover(object? sender, EventArgs e)
        {
            IsHovered = true;
            BackColor = _currentTheme.ButtonHoverBackColor;
            base.OnMouseHover(e);

        }
        private void BeepImage_Click(object? sender, EventArgs e)
        {
           // var ev = new BeepEventDataArgs("ImageClicked", this);
          //  ImageClicked?.Invoke(this, ev);
            base.OnClick(e);
        }
    }
}
