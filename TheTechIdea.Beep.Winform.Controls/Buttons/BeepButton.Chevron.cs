using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Chevron-shaped rendering mode for <see cref="BeepButton"/>. When
    /// <see cref="Direction"/> is set, the button renders as a directional
    /// chevron (the classic <c>BeepChevronButton</c> look) instead of the
    /// standard or advanced-painter rendering.
    /// </summary>
    public partial class BeepButton
    {
        private const int ChevronPaddingToken = 6;
        private const int ChevronImageInsetToken = 5;
        private const int ChevronDefaultImageSizeToken = 16;
        private const int ChevronImageTextGapToken = 8;

        private ChevronDirection? _chevronDirection = null;
        private ChevronButtonVariant _chevronButtonVariant = ChevronButtonVariant.Filled;
        private bool _isCheckable = true;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("When set, renders the button as a chevron pointing in the given direction. Null uses the standard button rendering.")]
        [DefaultValue(null)]
        public ChevronDirection? Direction
        {
            get => _chevronDirection;
            set
            {
                if (_chevronDirection != value)
                {
                    _chevronDirection = value;
                    if (value.HasValue)
                    {
                        UpdateChevronRegion();
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

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Chevron fill style (Filled, Outlined, Tonal, Text).")]
        [DefaultValue(ChevronButtonVariant.Filled)]
        public ChevronButtonVariant Variant
        {
            get => _chevronButtonVariant;
            set
            {
                if (_chevronButtonVariant != value)
                {
                    _chevronButtonVariant = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true and Direction is set, clicking toggles the checked state (IsToggled).")]
        [DefaultValue(true)]
        public bool IsCheckable
        {
            get => _isCheckable;
            set => _isCheckable = value;
        }

        private void DrawChevronContent(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            UpdateDrawingRect();
            Rectangle rect = DrawingRect;

            using (GraphicsPath path = CreateChevronPath(rect))
            {
                ResolveChevronColors(out Color fillColor, out Color borderColor, out Color textColor);

                g.FillPath(PaintersFactory.GetSolidBrush(fillColor), path);
                float borderWidth = GetChevronBorderWidth();
                if (borderWidth > 0f)
                {
                    g.DrawPath(PaintersFactory.GetPen(borderColor, borderWidth), path);
                }

                DrawChevronStateOverlay(g, path);
                DrawChevronFocusRing(g, path);
            }

            Rectangle contentRectBase = GetChevronContentRect(rect);

            if (!string.IsNullOrEmpty(ImagePath))
            {
                int imageInset = DpiScalingHelper.ScaleValue(ChevronImageInsetToken, this);
                int imageSize = Math.Min(
                    rect.Height - imageInset * 2,
                    DpiScalingHelper.ScaleValue(ChevronDefaultImageSizeToken, this));
                Rectangle imageRect = new Rectangle(
                    rect.Left + imageInset,
                    rect.Top + (rect.Height - imageSize) / 2,
                    imageSize,
                    imageSize);

                try
                {
                    StyledImagePainter.Paint(g, imageRect, ImagePath);
                    AddHitArea("ChevronImage", imageRect, null, () => OnClick(EventArgs.Empty));
                }
                catch
                {
                    // Image drawing is best-effort; continue without it.
                }

                int gap = DpiScalingHelper.ScaleValue(ChevronImageTextGapToken, this);
                contentRectBase = Rectangle.FromLTRB(
                    Math.Min(contentRectBase.Right, imageRect.Right + gap),
                    contentRectBase.Top,
                    contentRectBase.Right,
                    contentRectBase.Bottom);
            }

            if (!string.IsNullOrEmpty(Text) && !HideText)
            {
                ResolveChevronColors(out _, out _, out Color textColor);
                Font font = _textFont ?? Font;
                Size textSize = TextRenderer.MeasureText(
                    g, Text, font, new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);

                Rectangle textRect = new Rectangle(
                    contentRectBase.Left,
                    contentRectBase.Top,
                    Math.Min(textSize.Width, contentRectBase.Width),
                    Math.Min(textSize.Height, contentRectBase.Height));
                textRect.X = contentRectBase.Left + (contentRectBase.Width - textRect.Width) / 2;
                textRect.Y = contentRectBase.Top + (contentRectBase.Height - textRect.Height) / 2;

                TextRenderer.DrawText(g, Text, font, textRect, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }
        }

        private GraphicsPath CreateChevronPath(Rectangle bounds)
        {
            ChevronDirection direction = _chevronDirection ?? ChevronDirection.Forward;
            int arrowSize = direction is ChevronDirection.Up or ChevronDirection.Down
                ? Math.Max(4, bounds.Width / 2)
                : Math.Max(4, bounds.Height / 2);

            Point[] points;
            switch (direction)
            {
                case ChevronDirection.Backward:
                    points = new[]
                    {
                        new Point(bounds.Right, bounds.Top),
                        new Point(bounds.Left + arrowSize, bounds.Top),
                        new Point(bounds.Left, bounds.Top + bounds.Height / 2),
                        new Point(bounds.Left + arrowSize, bounds.Bottom),
                        new Point(bounds.Right, bounds.Bottom)
                    };
                    break;
                case ChevronDirection.Down:
                    points = new[]
                    {
                        new Point(bounds.Left, bounds.Top),
                        new Point(bounds.Left, bounds.Bottom - arrowSize),
                        new Point(bounds.Left + bounds.Width / 2, bounds.Bottom),
                        new Point(bounds.Right, bounds.Bottom - arrowSize),
                        new Point(bounds.Right, bounds.Top)
                    };
                    break;
                case ChevronDirection.Up:
                    points = new[]
                    {
                        new Point(bounds.Left, bounds.Bottom),
                        new Point(bounds.Left, bounds.Top + arrowSize),
                        new Point(bounds.Left + bounds.Width / 2, bounds.Top),
                        new Point(bounds.Right, bounds.Top + arrowSize),
                        new Point(bounds.Right, bounds.Bottom)
                    };
                    break;
                default:
                    points = new[]
                    {
                        new Point(bounds.Left, bounds.Top),
                        new Point(bounds.Right - arrowSize, bounds.Top),
                        new Point(bounds.Right, bounds.Top + bounds.Height / 2),
                        new Point(bounds.Right - arrowSize, bounds.Bottom),
                        new Point(bounds.Left, bounds.Bottom)
                    };
                    break;
            }

            var path = new GraphicsPath();
            path.AddPolygon(points);
            return path;
        }

        private Rectangle GetChevronContentRect(Rectangle bounds)
        {
            int padding = DpiScalingHelper.ScaleValue(ChevronPaddingToken, this);
            Rectangle content = Rectangle.Inflate(bounds, -padding, -padding);
            return content.Width <= 0 || content.Height <= 0 ? bounds : content;
        }

        private void DrawChevronStateOverlay(Graphics graphics, GraphicsPath path)
        {
            if (!Enabled)
            {
                return;
            }

            int alpha = IsPressed ? 28 : Focused ? 24 : IsHovered ? 20 : 0;
            if (alpha <= 0)
            {
                return;
            }

            Color overlayColor = Color.FromArgb(alpha, _currentTheme?.ButtonForeColor ?? ForeColor);
            graphics.FillPath(PaintersFactory.GetSolidBrush(overlayColor), path);
        }

        private void DrawChevronFocusRing(Graphics graphics, GraphicsPath path)
        {
            if (!Focused || !_showFocusRing)
            {
                return;
            }

            using GraphicsPath ringPath = path.CreateInsetPath(2f);
            if (ringPath == null || ringPath.PointCount <= 2)
            {
                return;
            }

            Color focusColor = _currentTheme?.FocusIndicatorColor ?? ColorUtils.MapSystemColor(SystemColors.Highlight);
            graphics.DrawPath(PaintersFactory.GetPen(Color.FromArgb(200, focusColor), 2f), ringPath);
        }

        private float GetChevronBorderWidth()
        {
            float baseWidth = Math.Max(1f, BorderThickness);
            return _chevronButtonVariant switch
            {
                ChevronButtonVariant.Text => 0f,
                ChevronButtonVariant.Outlined => Math.Max(1.5f, baseWidth),
                _ => baseWidth
            };
        }

        private void ResolveChevronColors(out Color fillColor, out Color borderColor, out Color textColor)
        {
            Color baseBack = _currentTheme?.ButtonBackColor ?? BackColor;
            Color hoverBack = _currentTheme?.ButtonHoverBackColor ?? HoverBackColor;
            Color pressedBack = _currentTheme?.ButtonPressedBackColor ?? PressedBackColor;
            Color disabledBack = _currentTheme?.DisabledBackColor ?? DisabledBackColor;
            Color baseText = _currentTheme?.ButtonForeColor ?? ForeColor;
            Color disabledText = _currentTheme?.DisabledForeColor ?? DisabledForeColor;
            Color baseBorder = _currentTheme?.ShadowColor ?? baseText;

            if (!Enabled)
            {
                fillColor = _chevronButtonVariant == ChevronButtonVariant.Text ? Color.Transparent : disabledBack;
                borderColor = Color.FromArgb(100, disabledText);
                textColor = disabledText;
                return;
            }

            fillColor = IsToggled ? pressedBack : IsHovered ? hoverBack : baseBack;
            textColor = baseText;
            borderColor = baseBorder;

            switch (_chevronButtonVariant)
            {
                case ChevronButtonVariant.Outlined:
                    fillColor = Color.Transparent;
                    borderColor = IsPressed ? pressedBack : IsHovered ? hoverBack : baseText;
                    break;

                case ChevronButtonVariant.Tonal:
                    fillColor = GraphicsExtensions.BlendColors(baseBack, baseText, 0.14f);
                    borderColor = GraphicsExtensions.BlendColors(baseText, baseBack, 0.45f);
                    if (IsPressed || IsToggled)
                    {
                        fillColor = GraphicsExtensions.BlendColors(fillColor, baseText, 0.15f);
                    }
                    else if (IsHovered)
                    {
                        fillColor = GraphicsExtensions.BlendColors(fillColor, baseText, 0.08f);
                    }
                    break;

                case ChevronButtonVariant.Text:
                    fillColor = Color.Transparent;
                    borderColor = Color.Transparent;
                    textColor = IsPressed ? pressedBack : IsHovered ? hoverBack : baseText;
                    break;
            }
        }

        private void UpdateChevronRegion()
        {
            if (!_chevronDirection.HasValue)
            {
                return;
            }

            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            Rectangle regionRect = ClientRectangle;
            if (regionRect.Width <= 0 || regionRect.Height <= 0)
            {
                return;
            }

            BorderPath?.Dispose();
            BorderPath = CreateChevronPath(regionRect);
            UpdateRegionForBadge();
        }
    }
}
