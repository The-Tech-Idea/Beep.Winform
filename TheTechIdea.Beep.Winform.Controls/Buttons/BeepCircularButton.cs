using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Buttons
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
    [ToolboxBitmap(typeof(BeepCircularButton))]
    [Category("Beep Controls")]
    [DisplayName("Beep Circular Button")]
    [Description("A circular button control with an optional image and text.")]
    public class BeepCircularButton : BaseControl
    {
        private string _imagePath = "";
        private TextLocation _textLocation = TextLocation.Below;
        private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;
        private bool _showBorder = true;
        private bool _isForColorSet = false;
        private bool _hidetext = false;
        private const int TextPadding = 5;
        private Size circlesize = Size.Empty;
        private Timer clickAnimationTimer;
        private float clickAnimationProgress = 1f;
        private const int clickAnimationDuration = 200;
        private DateTime clickAnimationStartTime;
        private bool isAnimatingClick = false;
        private bool _applyThemeOnImage = false;
        
        // Layout rectangles for hit testing
        private Rectangle imageRect;

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

        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
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
                //UpdateCircleRegion();
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
             //   UpdateCircleRegion();
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
            get => _imagePath;
            set
            {
                _imagePath = value ?? "";
                Invalidate();
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

        public BeepCircularButton() : base()
        {
            if (Width <= 0 || Height <= 0)
            {
                Width = 100;
                Height = 100;
            }

            IsSelectedOptionOn = false;
            IsChild = true;
            IsShadowAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            IsCustomShape = true;
            ApplyTheme();
         //   UpdateCircleRegion();
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
           // UpdateCircleRegion();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
           // UpdateCircleRegion();
        }

        private void UpdateCircleRegion()
        {
            if (Width <= 0 || Height <= 0) return;

            // Get the calculated layout to know where circle and text are
            CalculateLayout(out Rectangle textRect, out Rectangle circleBounds);
            
            if (circleBounds.Width <= 0 || circleBounds.Height <= 0)
                circleBounds = ClientRectangle;

            var path = new GraphicsPath();
            
            // If text is outside the circle, include the full control bounds
            // so the text isn't clipped by the region
            if (_textLocation != TextLocation.Inside && !HideText && !string.IsNullOrEmpty(Text))
            {
                // Use full client rectangle to ensure text is visible
                path.AddRectangle(ClientRectangle);
            }
            else
            {
                // Text is inside or hidden - use circle only
                path.AddEllipse(circleBounds);
            }

            BorderPath?.Dispose();
            BorderPath = path;
            UpdateRegionForBadge();
        }

        protected override void DrawContent(Graphics g)
        {
             g.SmoothingMode = SmoothingMode.AntiAlias;
            UpdateDrawingRect();
            ClearHitList();
            Draw(g, DrawingRect);
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Calculate layout once - text has priority, circle gets remaining space
            CalculateLayout(out Rectangle textRect, out Rectangle circleBounds);
            
            int diameter = Math.Min(circleBounds.Width, circleBounds.Height);

            // Animation: scale the circle while animating
            float scale = isAnimatingClick ? 1f + 0.2f * (1 - clickAnimationProgress) : 1f;
            Rectangle animatedCircleBounds = Rectangle.Inflate(
                circleBounds,
                (int)(circleBounds.Width * (scale - 1f) / 2),
                (int)(circleBounds.Height * (scale - 1f) / 2)
            );

            // Fill circle background
            var fillBrush = PaintersFactory.GetSolidBrush(IsHovered ? _currentTheme.ButtonHoverBackColor : _currentTheme.ButtonBackColor);
            graphics.FillEllipse(fillBrush, animatedCircleBounds);

            // Border
            if (_showBorder)
            {
                var pen = PaintersFactory.GetPen(_currentTheme.ShadowColor, BorderThickness);
                graphics.DrawEllipse(pen, animatedCircleBounds);
            }

            // Draw image inside circle using StyledImagePainter
            if (!string.IsNullOrEmpty(_imagePath) && diameter > 0)
            {
                Size imageSize = GetInscribedSquareSize(diameter);
                imageRect = new Rectangle(
                    animatedCircleBounds.X + (animatedCircleBounds.Width - imageSize.Width) / 2,
                    animatedCircleBounds.Y + (animatedCircleBounds.Height - imageSize.Height) / 2,
                    imageSize.Width,
                    imageSize.Height
                );

                try
                {
                    Color tintColor = Color.Transparent;
                    if (ApplyThemeOnImage && _currentTheme != null)
                    {
                        tintColor = _currentTheme.ButtonForeColor;
                    }
                    StyledImagePainter.PaintWithTint(graphics, imageRect.ToGraphicsPath(), _imagePath, tintColor);
                    AddHitArea("Image", imageRect, null, () => OnImageClick());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error drawing image: {ex.Message}");
                }
            }

            // Draw text - use the pre-calculated textRect
            if (!string.IsNullOrEmpty(Text) && !HideText && !textRect.IsEmpty)
            {
                TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;

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

                var textColor = _currentTheme.PrimaryTextColor;
                TextRenderer.DrawText(graphics, Text, Font, textRect, textColor, flags);
            }
        }

        /// <summary>
        /// Unified layout calculation - text has priority, circle gets remaining space
        /// </summary>
        private void CalculateLayout(out Rectangle textRect, out Rectangle circleBounds)
        {
            int borderSpace = _showBorder ? BorderThickness : 0;
            int margin = 2;
            int totalMargin = borderSpace + margin;

            // Content area (inside padding and margins)
            int contentX = DrawingRect.X + Padding.Left + totalMargin;
            int contentY = DrawingRect.Y + Padding.Top + totalMargin;
            int contentWidth = DrawingRect.Width - Padding.Horizontal - 2 * totalMargin;
            int contentHeight = DrawingRect.Height - Padding.Vertical - 2 * totalMargin;

            // Handle no text case - circle takes all space
            if (HideText || string.IsNullOrEmpty(Text))
            {
                textRect = Rectangle.Empty;
                int diameter = Math.Min(contentWidth, contentHeight);
                diameter = Math.Max(0, diameter);
                int circleX = contentX + (contentWidth - diameter) / 2;
                int circleY = contentY + (contentHeight - diameter) / 2;
                circleBounds = new Rectangle(circleX, circleY, diameter, diameter);
                return;
            }

            // Measure text based on location
            TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding;
            Size textSize;
            int textMaxWidth;
            int diameter2;

            switch (_textLocation)
            {
                case TextLocation.Above:
                case TextLocation.Below:
                    // Text uses full width, measure height needed
                    textMaxWidth = Math.Max(10, contentWidth);
                    textSize = TextRenderer.MeasureText(Text, Font, new Size(textMaxWidth, int.MaxValue), flags);
                    
                    // Circle gets remaining height
                    int remainingHeightV = contentHeight - textSize.Height - TextPadding;
                    diameter2 = Math.Min(contentWidth, remainingHeightV);
                    diameter2 = Math.Max(0, diameter2);

                    int circleXV = contentX + (contentWidth - diameter2) / 2;
                    int textXV = contentX;

                    if (_textLocation == TextLocation.Above)
                    {
                        textRect = new Rectangle(textXV, contentY, textMaxWidth, textSize.Height);
                        int circleYV = contentY + textSize.Height + TextPadding;
                        circleBounds = new Rectangle(circleXV, circleYV, diameter2, diameter2);
                    }
                    else // Below
                    {
                        int circleYV = contentY;
                        circleBounds = new Rectangle(circleXV, circleYV, diameter2, diameter2);
                        int textYV = circleYV + diameter2 + TextPadding;
                        textRect = new Rectangle(textXV, textYV, textMaxWidth, textSize.Height);
                    }
                    return;

                case TextLocation.Left:
                case TextLocation.Right:
                    // For left/right, circle should be square fitting in height
                    // Text gets remaining width
                    diameter2 = Math.Min(contentHeight, contentWidth - TextPadding - 20); // Reserve at least 20px for text
                    diameter2 = Math.Max(10, diameter2); // Minimum circle size
                    
                    int remainingWidthH = contentWidth - diameter2 - TextPadding;
                    textMaxWidth = Math.Max(10, remainingWidthH);
                    textSize = TextRenderer.MeasureText(Text, Font, new Size(textMaxWidth, int.MaxValue), flags);

                    int circleYH = contentY + (contentHeight - diameter2) / 2;
                    int textYH = contentY + (contentHeight - textSize.Height) / 2;

                    if (_textLocation == TextLocation.Left)
                    {
                        textRect = new Rectangle(contentX, textYH, textSize.Width, textSize.Height);
                        int circleXH = contentX + textSize.Width + TextPadding;
                        circleBounds = new Rectangle(circleXH, circleYH, diameter2, diameter2);
                    }
                    else // Right
                    {
                        int circleXH = contentX;
                        circleBounds = new Rectangle(circleXH, circleYH, diameter2, diameter2);
                        int textXH = circleXH + diameter2 + TextPadding;
                        textRect = new Rectangle(textXH, textYH, textSize.Width, textSize.Height);
                    }
                    return;

                case TextLocation.Inside:
                default:
                    // Circle takes all space, text is centered inside inscribed square
                    diameter2 = Math.Min(contentWidth, contentHeight);
                    diameter2 = Math.Max(0, diameter2);
                    int circleXI = contentX + (contentWidth - diameter2) / 2;
                    int circleYI = contentY + (contentHeight - diameter2) / 2;
                    circleBounds = new Rectangle(circleXI, circleYI, diameter2, diameter2);

                    // Inscribed square for text
                    int inscribedSize = (int)(diameter2 / Math.Sqrt(2)) - 2 * TextPadding;
                    inscribedSize = Math.Max(10, inscribedSize);
                    textSize = TextRenderer.MeasureText(Text, Font, new Size(inscribedSize, int.MaxValue), flags);
                    
                    int textXI = circleXI + (diameter2 - textSize.Width) / 2;
                    int textYI = circleYI + (diameter2 - textSize.Height) / 2;
                    textRect = new Rectangle(textXI, textYI, textSize.Width, textSize.Height);
                    return;
            }
        }

        protected virtual void OnImageClick()
        {
            OnClick(EventArgs.Empty);
        }

        public Size GetInscribedSquareSize(int circleDiameter)
        {
            int squareSideLength = (int)(circleDiameter / Math.Sqrt(2));
            return new Size(squareSideLength, squareSideLength);
        }

        public Rectangle GetCircleBounds()
        {
            CalculateLayout(out _, out Rectangle circleBounds);
            return circleBounds;
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            if (_currentTheme == null) return;

            if (IsChild && Parent != null)
            {
                BackColor = Parent.BackColor;
                ParentBackColor = Parent.BackColor;
            }
            else
            {
                BackColor = _currentTheme.ButtonBackColor;
            }

            ForeColor = _currentTheme.ButtonForeColor;
            HoverBackColor = _currentTheme.ButtonHoverBackColor;
            HoverForeColor = _currentTheme.ButtonHoverForeColor;
            DisabledBackColor = _currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;
            FocusBackColor = _currentTheme.ButtonSelectedBackColor;
            FocusForeColor = _currentTheme.ButtonSelectedForeColor;
            SelectedBackColor = _currentTheme.ButtonSelectedBackColor;
            SelectedForeColor = _currentTheme.ButtonSelectedForeColor;
            PressedBackColor = _currentTheme.ButtonPressedBackColor;
            PressedForeColor = _currentTheme.ButtonPressedForeColor;

            if (_showBorder)
            {
                BorderColor = _currentTheme.ButtonBorderColor;
            }

            if (UseThemeFont)
            {
                if (_currentTheme.ButtonStyle != null)
                {
                    _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
                }
                else
                {
                    _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                }
            }

            Invalidate();
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (!string.IsNullOrEmpty(_imagePath) && imageRect.Contains(e.Location))
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Cursor = Cursors.Default;
        }

        #region "Badge"
        private BeepControl _lastBeepParent;

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (_lastBeepParent != null)
                _lastBeepParent.ClearChildExternalDrawing(this);

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

        private void DrawBadgeExternally(Graphics g, Rectangle childBounds)
        {
            if (string.IsNullOrEmpty(BadgeText))
                return;

            const int badgeSize = 22;
            int x = childBounds.Right - badgeSize / 2;
            int y = childBounds.Y - badgeSize / 2;
            var badgeRect = new Rectangle(x, y, badgeSize, badgeSize);

            // Draw badge directly since BaseControl doesn't have DrawBadgeImplementation
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Draw badge background
            Color badgeBackColor = BadgeBackColor != Color.Empty ? BadgeBackColor : Color.Red;
            using (var brush = new SolidBrush(badgeBackColor))
            {
                g.FillEllipse(brush, badgeRect);
            }

            // Draw badge text
            if (!string.IsNullOrEmpty(BadgeText))
            {
                Color badgeForeColor = BadgeForeColor != Color.Empty ? BadgeForeColor : Color.White;
                Font badgeFont = BadgeFont ?? new Font("Arial", 8, FontStyle.Bold);
                using (var textBrush = new SolidBrush(badgeForeColor))
                using (badgeFont)
                {
                    var fmt = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(BadgeText, badgeFont, textBrush, badgeRect, fmt);
                }
            }
        }
        #endregion "Badge"

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                clickAnimationTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
