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
            IsCustomeBorder = true;
            ApplyTheme();
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
            ClearHitList();
            Draw(g, DrawingRect);
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            AdjustControlHeight();

            Rectangle textRect = GetTextRectangle();
            Rectangle circleBounds = GetCircleBounds(textRect);
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
            if (!string.IsNullOrEmpty(_imagePath))
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

            // Draw text - measure to prevent clipping
            if (!string.IsNullOrEmpty(Text) && !HideText)
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

                // Measure text to ensure it fits
                var textSize = TextRenderer.MeasureText(graphics, Text, Font, new Size(textRect.Width, int.MaxValue), flags);
                textRect.Width = Math.Min(textSize.Width, textRect.Width);
                textRect.Height = Math.Min(textSize.Height, textRect.Height);

                var textColor = _currentTheme.PrimaryTextColor;
                TextRenderer.DrawText(graphics, Text, Font, textRect, textColor, flags);
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

        private Rectangle GetCircleBounds(Rectangle textRect)
        {
            int borderSpace = _showBorder ? BorderThickness : 0;
            int margin = 2;

            int diameter;
            if (!circlesize.IsEmpty)
            {
                diameter = Math.Min(circlesize.Width, circlesize.Height);
            }
            else
            {
                int maxDiameter = Math.Min(
                    DrawingRect.Width - Padding.Horizontal - 2 * borderSpace - 2 * margin,
                    DrawingRect.Height - Padding.Vertical - 2 * borderSpace - 2 * margin
                );

                if (HideText || string.IsNullOrEmpty(Text))
                {
                    diameter = maxDiameter;
                }
                else
                {
                    diameter = _textLocation == TextLocation.Above || _textLocation == TextLocation.Below
                        ? Math.Max(0, maxDiameter - textRect.Height - TextPadding)
                        : maxDiameter;
                }
            }

            diameter = Math.Max(0, diameter);

            int x = DrawingRect.X + Padding.Left + borderSpace + margin + (DrawingRect.Width - Padding.Horizontal - 2 * borderSpace - 2 * margin - diameter) / 2;
            int y = DrawingRect.Y + Padding.Top + borderSpace + margin + (DrawingRect.Height - Padding.Vertical - 2 * borderSpace - 2 * margin - diameter) / 2;

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

            int borderSpace = _showBorder ? BorderThickness : 0;
            int margin = 2;

            int maxTextWidth = Width - Padding.Horizontal - 2 * TextPadding;
            maxTextWidth = Math.Max(10, maxTextWidth);
            TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding;
            Size textSize = TextRenderer.MeasureText(Text, Font, new Size(maxTextWidth, int.MaxValue), flags);

            int circleHeight = Math.Min(
                Width - Padding.Horizontal - 2 * borderSpace - 2 * margin,
                Height - Padding.Vertical - 2 * borderSpace - 2 * margin
            );

            int requiredHeight = circleHeight + Padding.Vertical + 2 * borderSpace + 2 * margin;
            if (_textLocation == TextLocation.Above || _textLocation == TextLocation.Below)
            {
                requiredHeight += textSize.Height + TextPadding;
            }

            if (Height < requiredHeight)
            {
                Height = requiredHeight;
                Invalidate();
            }
        }

        private Rectangle GetTextRectangle()
        {
            if (HideText || string.IsNullOrEmpty(Text)) return Rectangle.Empty;

            int maxTextWidth = DrawingRect.Width - Padding.Horizontal - 2 * TextPadding;
            maxTextWidth = Math.Max(10, maxTextWidth);

            TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding;
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

            Size proposedSize = new Size(maxTextWidth, int.MaxValue);
            Size textSize = TextRenderer.MeasureText(Text, Font, proposedSize, flags);

            Rectangle textRect;
            switch (_textLocation)
            {
                case TextLocation.Above:
                    textRect = new Rectangle(
                        DrawingRect.Left + Padding.Left + TextPadding,
                        DrawingRect.Top + Padding.Top + TextPadding,
                        maxTextWidth,
                        textSize.Height
                    );
                    break;
                case TextLocation.Below:
                    textRect = new Rectangle(
                        DrawingRect.Left + Padding.Left + TextPadding,
                        DrawingRect.Bottom - Padding.Bottom - textSize.Height - TextPadding,
                        maxTextWidth,
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
                        maxTextWidth,
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
