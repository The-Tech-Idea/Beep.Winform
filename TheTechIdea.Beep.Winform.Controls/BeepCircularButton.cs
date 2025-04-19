using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using Timer = System.Windows.Forms.Timer;

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
        private Size circlesize = Size.Empty;
        private Timer clickAnimationTimer;
        private float clickAnimationProgress = 1f;
        private const int clickAnimationDuration = 200;
        private DateTime clickAnimationStartTime;
        private bool isAnimatingClick = false;


        [Browsable(true)]
        [Category("Layout")]
        [Description("Size of the Circle  Default is empty")]
        public Size CircleSize
        {
            get => circlesize;
            set
            {
                circlesize = value;
               
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Layout")]
        [Description("Gets the X-offset of the circle's center relative to the control's left border.")]
        public int CircleMidXOffset
        {
            get
            {
                Rectangle textRect = GetTextRectangle();
                Rectangle circleBounds = GetCircleBounds(textRect);
                int circleCenterX = circleBounds.X + circleBounds.Width / 2;
                return circleCenterX - DrawingRect.Left;
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Gets the Y-offset of the circle's center relative to its default centered position.")]
        public int CircleMidYOffset
        {
            get
            {
                Rectangle textRect = GetTextRectangle();
                Rectangle circleBounds = GetCircleBounds(textRect);
                int circleCenterY = circleBounds.Y + (circleBounds.Height / 2);
                int borderSpace = _showBorder ? _borderThickness : 0;
                int margin = 2;
                int defaultCenterY = DrawingRect.Y + Padding.Top + borderSpace + margin +
                                    (DrawingRect.Height - Padding.Vertical - (2 * borderSpace) - (2 * margin)) / 2;
                return circleCenterY - defaultCenterY;
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Gets the offset of the circle's center from the control's top-left corner.")]
        public Point CircleCenterOffset
        {
            get
            {
                Rectangle textRect = GetTextRectangle();
                Rectangle circleBounds = GetCircleBounds(textRect);
                int circleCenterX = circleBounds.X + circleBounds.Width / 2;
                int circleCenterY = circleBounds.Y + circleBounds.Height / 2;
                return new Point(circleCenterX - DrawingRect.X, circleCenterY - DrawingRect.Y);
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
            IsSelectedOptionOn = false;
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
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            base.OnPaint(e);
        }
        protected override void DrawContent(Graphics g)
        {
          
            base.DrawContent(g);
            UpdateDrawingRect();
            Draw(g, DrawingRect);
        }
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            AdjustControlHeight();

            Rectangle textRect = GetTextRectangle();
            Rectangle circleBounds = GetCircleBounds(textRect);
            int diameter = Math.Min(circleBounds.Width, circleBounds.Height);

            // 🔁 Animation: scale the circle while animating
            float scale = isAnimatingClick ? 1f + 0.2f * (1 - clickAnimationProgress) : 1f;
            Rectangle animatedCircleBounds = Rectangle.Inflate(
                circleBounds,
                (int)(circleBounds.Width * (scale - 1f) / 2),
                (int)(circleBounds.Height * (scale - 1f) / 2)
            );

            // 🎨 Fill circle background
            using (Brush brush = new SolidBrush(IsHovered ? _currentTheme.ButtonHoverBackColor : _currentTheme.ButtonBackColor))
            {
                graphics.FillEllipse(brush, animatedCircleBounds);
            }

            // 🟠 Border
            if (_showBorder)
            {
                using (Pen pen = new Pen(_currentTheme.ShadowColor, _borderThickness))
                {
                    graphics.DrawEllipse(pen, animatedCircleBounds);
                }
            }

            // 🖼 Draw image inside circle
            if (!string.IsNullOrEmpty(beepImage.ImagePath))
            {
                beepImage.MaximumSize = GetInscribedSquareSize(diameter);
                beepImage.Size = beepImage.MaximumSize;

                beepImage.Location = new Point(
                    animatedCircleBounds.X + (animatedCircleBounds.Width - beepImage.Width) / 2,
                    animatedCircleBounds.Y + (animatedCircleBounds.Height - beepImage.Height) / 2
                );

                beepImage.DrawImage(graphics, new Rectangle(beepImage.Location, beepImage.Size));
            }

            // 📝 Draw text
            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

                // Horizontal alignment
                switch (_textAlign)
                {
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.BottomLeft:
                        flags |= TextFormatFlags.Left;
                        break;
                    case ContentAlignment.TopRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.BottomRight:
                        flags |= TextFormatFlags.Right;
                        break;
                    default:
                        flags |= TextFormatFlags.HorizontalCenter;
                        break;
                }

                // Vertical alignment
                switch (_textAlign)
                {
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.TopCenter:
                    case ContentAlignment.TopRight:
                        flags |= TextFormatFlags.Top;
                        break;
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.BottomRight:
                        flags |= TextFormatFlags.Bottom;
                        break;
                    default:
                        flags |= TextFormatFlags.VerticalCenter;
                        break;
                }

                TextRenderer.DrawText(graphics, Text, Font, textRect, _currentTheme.PrimaryTextColor, flags);
            }

            DrawBadge(graphics);
        }

        public Size GetInscribedSquareSize(int circleDiameter)
        {
            // The side length of the largest square that fits within a circle is the circle's diameter divided by √2.
            int squareSideLength = (int)(circleDiameter / Math.Sqrt(2));

            return new Size(squareSideLength, squareSideLength);
        }
        private Rectangle GetCircleBounds(Rectangle textRect)
        {
            int borderSpace = _showBorder ? _borderThickness : 0;
            int margin = 2;

            int diameter;
            if (!circlesize.IsEmpty)
            {
                // Use fixed CircleSize if set
                diameter = Math.Min(circlesize.Width, circlesize.Height); // Ensure circular shape
            }
            else
            {
                // Dynamic sizing based on control dimensions
                int maxDiameter = Math.Min(
                    DrawingRect.Width - Padding.Horizontal - (2 * borderSpace) - (2 * margin),
                    DrawingRect.Height - Padding.Vertical - (2 * borderSpace) - (2 * margin)
                );

                if (HideText || string.IsNullOrEmpty(Text))
                {
                    diameter = maxDiameter;
                }
                else
                {
                    diameter = (_textLocation == TextLocation.Above || _textLocation == TextLocation.Below)
                        ? Math.Max(0, maxDiameter - textRect.Height - TextPadding)
                        : maxDiameter;
                }
            }

            // Ensure diameter is non-negative
            diameter = Math.Max(0, diameter);

            // Center the circle within the available space
            int x = DrawingRect.X + Padding.Left + borderSpace + margin + (DrawingRect.Width - Padding.Horizontal - (2 * borderSpace) - (2 * margin) - diameter) / 2;
            int y = DrawingRect.Y + Padding.Top + borderSpace + margin + (DrawingRect.Height - Padding.Vertical - (2 * borderSpace) - (2 * margin) - diameter) / 2;

            if (!HideText && !string.IsNullOrEmpty(Text))
            {
                y += _textLocation switch
                {
                    TextLocation.Above => textRect.Height / 2 + TextPadding / 2,
                    TextLocation.Below => -textRect.Height / 2 - TextPadding / 2,
                    _ => 0
                };
            }

            return new Rectangle(x, y, diameter, diameter);
        }
        public Rectangle GetCircleBounds()
        {
            Rectangle textRect = GetTextRectangle();
            return GetCircleBounds(textRect);
        }
        private void AdjustControlHeight()
        {
            if (HideText || string.IsNullOrEmpty(Text)) return;

            // Calculate the required height based on text and circle
            int borderSpace = _showBorder ? _borderThickness : 0;
            int margin = 2;

            // Measure wrapped text size
            int maxTextWidth = Width - Padding.Horizontal - (2 * TextPadding);
            maxTextWidth = Math.Max(10, maxTextWidth);
            TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;
            Size textSize = TextRenderer.MeasureText(Text, Font, new Size(maxTextWidth, int.MaxValue), flags);

            // Calculate the minimum height needed
            int circleHeight = Math.Min(
                Width - Padding.Horizontal - (2 * borderSpace) - (2 * margin),
                Height - Padding.Vertical - (2 * borderSpace) - (2 * margin)
            );

            int requiredHeight = circleHeight + Padding.Vertical + (2 * borderSpace) + (2 * margin);
            if (_textLocation == TextLocation.Above || _textLocation == TextLocation.Below)
            {
                requiredHeight += textSize.Height + TextPadding;
            }

            // Adjust control height if needed
            if (Height < requiredHeight)
            {
                Height = requiredHeight;
                Invalidate();
            }
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
            if (HideText || string.IsNullOrEmpty(Text)) return Rectangle.Empty;

            // Define the maximum width for text wrapping and rendering
            int maxTextWidth = DrawingRect.Width - Padding.Horizontal - (2 * TextPadding);
            maxTextWidth = Math.Max(10, maxTextWidth);

            // Use TextRenderer to measure text with word wrap
            TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;
            switch (_textAlign)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    flags |= TextFormatFlags.Left;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Right;
                    break;
                default:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;
            }

            // Measure the text size with wrapping
            Size proposedSize = new Size(maxTextWidth, int.MaxValue);
            Size textSize = TextRenderer.MeasureText(Text, Font, proposedSize, flags);

            // Calculate the text rectangle based on TextLocation
            Rectangle textRect;
            switch (_textLocation)
            {
                case TextLocation.Above:
                    textRect = new Rectangle(
                        DrawingRect.Left + Padding.Left + TextPadding,
                        DrawingRect.Top + Padding.Top + TextPadding,
                        maxTextWidth, // Use full width for rendering
                        textSize.Height
                    );
                    break;
                case TextLocation.Below:
                    textRect = new Rectangle(
                        DrawingRect.Left + Padding.Left + TextPadding,
                        DrawingRect.Bottom - Padding.Bottom - textSize.Height - TextPadding,
                        maxTextWidth, // Use full width for rendering
                        textSize.Height
                    );
                    break;
                case TextLocation.Left:
                    textRect = new Rectangle(
                        DrawingRect.Left + Padding.Left + TextPadding,
                        DrawingRect.Top + Padding.Top + (DrawingRect.Height - Padding.Vertical - textSize.Height) / 2,
                        textSize.Width,
                        textSize.Height
                    );
                    break;
                case TextLocation.Right:
                    textRect = new Rectangle(
                        DrawingRect.Right - Padding.Right - textSize.Width - TextPadding,
                        DrawingRect.Top + Padding.Top + (DrawingRect.Height - Padding.Vertical - textSize.Height) / 2,
                        textSize.Width,
                        textSize.Height
                    );
                    break;
                case TextLocation.Inside:
                    textRect = new Rectangle(
                        DrawingRect.Left + Padding.Left + TextPadding,
                        DrawingRect.Top + Padding.Top + (DrawingRect.Height - Padding.Vertical - textSize.Height) / 2,
                        maxTextWidth, // Use full width for rendering
                        textSize.Height
                    );
                    break;
                default:
                    textRect = Rectangle.Empty;
                    break;
            }

            return textRect;
        }
        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            //TextFont=BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            if (IsChild && Parent != null)
            {
                BackColor = Parent.BackColor;
                ParentBackColor = Parent.BackColor;
            }
            BackColor = _currentTheme.ButtonBackColor;
            ForeColor = _currentTheme.ButtonForeColor;
            HoverBackColor = _currentTheme.ButtonHoverBackColor;
            HoverForeColor = _currentTheme.ButtonHoverForeColor;
            DisabledBackColor = _currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;
            FocusBackColor = _currentTheme.ButtonSelectedBackColor;
            FocusForeColor = _currentTheme.ButtonSelectedForeColor;


            PressedBackColor = _currentTheme.ButtonPressedBackColor;
            PressedForeColor = _currentTheme.ButtonPressedForeColor;
            beepLabel.Theme = Theme;
            //  if (_beepListBox != null)   _beepListBox.Theme = Theme;
            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
               
            }
            Font = _textFont;
            beepLabel.TextFont = _textFont;
            beepImage.ImageEmbededin = ImageEmbededin.Button;
            beepImage.Theme = Theme;
            
            if (ApplyThemeOnImage)
            {
                beepImage.ApplyThemeOnImage = true;
               
                
            }
            
           
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
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            StartClickAnimation();
        }
        private void StartClickAnimation()
        {
            clickAnimationProgress = 0f;
            clickAnimationStartTime = DateTime.Now;
            isAnimatingClick = true;

            if (clickAnimationTimer == null)
            {
                clickAnimationTimer = new Timer { Interval = 16 };
                clickAnimationTimer.Tick += (s, e) =>
                {
                    double elapsed = (DateTime.Now - clickAnimationStartTime).TotalMilliseconds;
                    clickAnimationProgress = (float)Math.Min(1, elapsed / clickAnimationDuration);
                    if (clickAnimationProgress >= 1f)
                    {
                        clickAnimationTimer.Stop();
                        isAnimatingClick = false;
                    }
                    Invalidate();
                };
            }

            clickAnimationTimer.Start();
        }

        private void BeepImage_Click(object? sender, EventArgs e)
        {
           // var ev = new BeepEventDataArgs("ImageClicked", this);
          //  ImageClicked?.Invoke(this, ev);
            base.OnClick(e);
        }
    }
}
