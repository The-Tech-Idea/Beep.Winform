using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using CircularTextLocation = TheTechIdea.Beep.Winform.Controls.Buttons.TextLocation;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Circular layout mode for <see cref="BeepButton"/>. When <see cref="TextLocation"/>
    /// is set, the button renders a circle with the text placed around or inside it
    /// (the classic <c>BeepCircularButton</c> look) instead of the standard rendering.
    /// </summary>
    public partial class BeepButton
    {
        private const int CircularTextPadding = 5;

        private CircularTextLocation? _textLocation = null;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("When set, renders the button as a circle with text placed at the given location. Null uses the standard button rendering.")]
        [DefaultValue(null)]
        public CircularTextLocation? TextLocation
        {
            get => _textLocation;
            set
            {
                if (_textLocation != value)
                {
                    _textLocation = value;
                    if (value.HasValue)
                    {
                        UpdateCircleRegion();
                    }
                    else
                    {
                        BorderPath?.Dispose();
                        BorderPath = null;
                    }
                    Invalidate();
                }
            }
        }

        private void DrawCircularContent(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            UpdateDrawingRect();

            if (_currentTheme == null)
            {
                return;
            }

            CalculateCircularLayout(out Rectangle textRect, out Rectangle circleBounds);
            int diameter = Math.Min(circleBounds.Width, circleBounds.Height);
            if (diameter <= 0)
            {
                return;
            }

            Color bgColor = !Enabled ? DisabledBackColor
                : IsPressed ? PressedBackColor
                : IsHovered ? HoverBackColor
                : _currentTheme.ButtonBackColor;
            g.FillEllipse(PaintersFactory.GetSolidBrush(bgColor), circleBounds);

            if (BorderThickness > 0)
            {
                g.DrawEllipse(PaintersFactory.GetPen(BorderColor, BorderThickness), circleBounds);
            }

            if (Focused && _showFocusRing)
            {
                Color focusColor = _currentTheme.FocusIndicatorColor != Color.Empty
                    ? _currentTheme.FocusIndicatorColor
                    : ColorUtils.MapSystemColor(SystemColors.Highlight);
                var focusPen = PaintersFactory.GetPen(focusColor, 2f);
                focusPen.DashStyle = DashStyle.Dot;
                g.DrawEllipse(focusPen, Rectangle.Inflate(circleBounds, 4, 4));
            }

            if (!string.IsNullOrEmpty(ImagePath))
            {
                Size imageSize = GetInscribedSquareSize(diameter);
                int imageYOffset = _textLocation == CircularTextLocation.Inside && !HideText && !string.IsNullOrEmpty(Text)
                    ? -(int)(diameter * 0.1f)
                    : 0;
                Rectangle imageRect = new Rectangle(
                    circleBounds.X + (circleBounds.Width - imageSize.Width) / 2,
                    circleBounds.Y + (circleBounds.Height - imageSize.Height) / 2 + imageYOffset,
                    imageSize.Width,
                    imageSize.Height);

                try
                {
                    StyledImagePainter.Paint(g, imageRect, ImagePath);
                    AddHitArea("CircularImage", imageRect, null, () => OnClick(EventArgs.Empty));
                }
                catch
                {
                    // Image drawing is best-effort; continue without it.
                }
            }

            Color textColor = !Enabled ? DisabledForeColor
                : IsPressed ? PressedForeColor
                : IsHovered ? HoverForeColor
                : _currentTheme.ButtonForeColor;

            if (!string.IsNullOrEmpty(Text) && !HideText && !textRect.IsEmpty)
            {
                TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl |
                                        TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;

                switch (TextAlign)
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

                switch (TextAlign)
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

                Rectangle adjustedTextRect = textRect;
                if (_textLocation == CircularTextLocation.Inside && !string.IsNullOrEmpty(ImagePath))
                {
                    adjustedTextRect = new Rectangle(
                        textRect.X,
                        textRect.Y + (int)(diameter * 0.15f),
                        textRect.Width,
                        textRect.Height - (int)(diameter * 0.15f));
                }

                TextRenderer.DrawText(g, Text, _textFont ?? Font, adjustedTextRect, textColor, flags);
            }
        }

        private void CalculateCircularLayout(out Rectangle textRect, out Rectangle circleBounds)
        {
            int totalMargin = BorderThickness > 0 ? BorderThickness + 2 : 2;

            int contentX = DrawingRect.X + Padding.Left + totalMargin;
            int contentY = DrawingRect.Y + Padding.Top + totalMargin;
            int contentWidth = DrawingRect.Width - Padding.Horizontal - 2 * totalMargin;
            int contentHeight = DrawingRect.Height - Padding.Vertical - 2 * totalMargin;

            CircularTextLocation location = _textLocation ?? CircularTextLocation.Inside;

            if (HideText || string.IsNullOrEmpty(Text))
            {
                textRect = Rectangle.Empty;
                int diameter = Math.Max(0, Math.Min(contentWidth, contentHeight));
                int circleX = contentX + (contentWidth - diameter) / 2;
                int circleY = contentY + (contentHeight - diameter) / 2;
                circleBounds = new Rectangle(circleX, circleY, diameter, diameter);
                return;
            }

            TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding;
            Size textSize;
            int textMaxWidth;
            int diameter2;

            switch (location)
            {
                case CircularTextLocation.Above:
                case CircularTextLocation.Below:
                    textMaxWidth = Math.Max(10, contentWidth);
                    textSize = TextRenderer.MeasureText(Text, _textFont ?? Font, new Size(textMaxWidth, int.MaxValue), flags);

                    int remainingHeightV = contentHeight - textSize.Height - CircularTextPadding;
                    diameter2 = Math.Max(0, Math.Min(contentWidth, remainingHeightV));
                    int circleXV = contentX + (contentWidth - diameter2) / 2;

                    if (location == CircularTextLocation.Above)
                    {
                        textRect = new Rectangle(contentX, contentY, textMaxWidth, textSize.Height);
                        circleBounds = new Rectangle(circleXV, contentY + textSize.Height + CircularTextPadding, diameter2, diameter2);
                    }
                    else
                    {
                        circleBounds = new Rectangle(circleXV, contentY, diameter2, diameter2);
                        textRect = new Rectangle(contentX, contentY + diameter2 + CircularTextPadding, textMaxWidth, textSize.Height);
                    }
                    return;

                case CircularTextLocation.Left:
                case CircularTextLocation.Right:
                    diameter2 = Math.Max(10, Math.Min(contentHeight, contentWidth - CircularTextPadding - 20));
                    int remainingWidthH = Math.Max(10, contentWidth - diameter2 - CircularTextPadding);
                    textSize = TextRenderer.MeasureText(Text, _textFont ?? Font, new Size(remainingWidthH, int.MaxValue), flags);

                    int circleYH = contentY + (contentHeight - diameter2) / 2;
                    int textYH = contentY + (contentHeight - textSize.Height) / 2;

                    if (location == CircularTextLocation.Left)
                    {
                        textRect = new Rectangle(contentX, textYH, textSize.Width, textSize.Height);
                        circleBounds = new Rectangle(contentX + textSize.Width + CircularTextPadding, circleYH, diameter2, diameter2);
                    }
                    else
                    {
                        circleBounds = new Rectangle(contentX, circleYH, diameter2, diameter2);
                        textRect = new Rectangle(contentX + diameter2 + CircularTextPadding, textYH, textSize.Width, textSize.Height);
                    }
                    return;

                default: // Inside
                    diameter2 = Math.Max(0, Math.Min(contentWidth, contentHeight));
                    int circleXI = contentX + (contentWidth - diameter2) / 2;
                    int circleYI = contentY + (contentHeight - diameter2) / 2;
                    circleBounds = new Rectangle(circleXI, circleYI, diameter2, diameter2);

                    int textOffsetY = !string.IsNullOrEmpty(ImagePath) ? (int)(diameter2 * 0.15f) : 0;
                    int inscribedSize = Math.Max(10, (int)(diameter2 / Math.Sqrt(2)) - 2 * CircularTextPadding);
                    textSize = TextRenderer.MeasureText(Text, _textFont ?? Font, new Size(inscribedSize, int.MaxValue), flags);
                    int textXI = circleXI + (diameter2 - textSize.Width) / 2;
                    int textYI = circleYI + (diameter2 - textSize.Height) / 2 + textOffsetY;
                    textRect = new Rectangle(textXI, textYI, textSize.Width, textSize.Height);
                    return;
            }
        }

        private Size GetInscribedSquareSize(int circleDiameter)
        {
            int side = (int)(circleDiameter / Math.Sqrt(2));
            return new Size(side, side);
        }

        private void UpdateCircleRegion()
        {
            if (!_textLocation.HasValue)
            {
                return;
            }

            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            CalculateCircularLayout(out _, out Rectangle circleBounds);
            if (circleBounds.Width <= 0 || circleBounds.Height <= 0)
            {
                circleBounds = ClientRectangle;
            }

            var path = new GraphicsPath();
            if (_textLocation != CircularTextLocation.Inside && !HideText && !string.IsNullOrEmpty(Text))
            {
                path.AddRectangle(ClientRectangle);
            }
            else
            {
                path.AddEllipse(circleBounds);
            }

            BorderPath?.Dispose();
            BorderPath = path;
            UpdateRegionForBadge();
        }
    }
}
