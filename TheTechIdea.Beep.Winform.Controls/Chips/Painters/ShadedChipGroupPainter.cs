using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Painters
{
    /// <summary>
    /// Shaded chips with gradient backgrounds (top to bottom).
    /// 3D shaded effect with depth perception.
    /// </summary>
    internal class ShadedChipGroupPainter : IChipGroupPainter
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private readonly BeepImage _iconRenderer = new BeepImage();
        private readonly StringFormat _centerFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };

        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
        }

        public void UpdateTheme(IBeepTheme theme)
        {
            _theme = theme;
        }

        public Size MeasureChip(SimpleItem item, Graphics g, ChipRenderOptions options)
        {
            string text = item?.Text ?? item?.Name ?? item?.DisplayField ?? string.Empty;
            var font = GetFont(options);
            var textSize = TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);

            int extraWidth = 0;
            if (options.ShowIcon && !string.IsNullOrEmpty(item?.ImagePath))
                extraWidth += options.IconMaxSize.Width + 6;
            if (options.ShowSelectionCheck)
                extraWidth += 18;
            if (options.ShowCloseOnSelected)
                extraWidth += 16;

            int height = GetChipHeight(options.Size);
            int padding = GetHorizontalPadding(options.Size);

            return new Size(textSize.Width + padding + extraWidth, height);
        }

        public void RenderChip(Graphics g, SimpleItem item, Rectangle bounds, ChipVisualState state, ChipRenderOptions options, out Rectangle closeRect)
        {
            closeRect = Rectangle.Empty;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var font = GetFont(options);
            var (lightColor, darkColor, fgColor) = GetColors(state, options);

            int cornerRadius = 14;
            using var path = CreateRoundedPath(bounds, cornerRadius);

            // Gradient background (light to dark, top to bottom)
            using (var gradientBrush = new LinearGradientBrush(
                bounds,
                lightColor,
                darkColor,
                LinearGradientMode.Vertical))
            {
                g.FillPath(gradientBrush, path);
            }

            // Inner highlight at top for 3D effect
            var highlightRect = new Rectangle(bounds.X + 2, bounds.Y + 1, bounds.Width - 4, bounds.Height / 3);
            using (var highlightBrush = new LinearGradientBrush(
                highlightRect,
                Color.FromArgb(60, Color.White),
                Color.FromArgb(0, Color.White),
                LinearGradientMode.Vertical))
            {
                using var highlightPath = CreateRoundedPath(highlightRect, cornerRadius - 2);
                g.FillPath(highlightBrush, highlightPath);
            }

            // Bottom shadow line for depth
            var shadowRect = new Rectangle(bounds.X + 2, bounds.Bottom - 3, bounds.Width - 4, 2);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(30, Color.Black)))
            {
                g.FillRectangle(shadowBrush, shadowRect);
            }

            var contentRect = Rectangle.Inflate(bounds, -10, -2);
            int leftOffset = 0;
            int rightOffset = 0;

            // Selection checkmark
            if (options.ShowSelectionCheck && state.IsSelected)
            {
                int checkSize = Math.Min(contentRect.Height - 4, 14);
                var checkRect = new Rectangle(
                    contentRect.Left,
                    contentRect.Top + (contentRect.Height - checkSize) / 2,
                    checkSize, checkSize);

                // Circular background for check
                using (var checkBgBrush = new SolidBrush(Color.FromArgb(60, Color.White)))
                {
                    g.FillEllipse(checkBgBrush, checkRect);
                }
                DrawCheckmark(g, checkRect, fgColor);
                leftOffset += checkSize + 6;
            }

            // Leading icon
            if (options.ShowIcon && !string.IsNullOrEmpty(item?.ImagePath))
            {
                var iconSize = options.IconMaxSize;
                var iconRect = new Rectangle(
                    contentRect.Left + leftOffset,
                    contentRect.Top + (contentRect.Height - iconSize.Height) / 2,
                    iconSize.Width, iconSize.Height);

                try
                {
                    using var iconPath = new GraphicsPath();
                    iconPath.AddRectangle(iconRect);
                    StyledImagePainter.PaintWithTint(g, iconPath, item.ImagePath, fgColor, 1f);
                }
                catch
                {
                    _iconRenderer.ImagePath = item.ImagePath;
                    _iconRenderer.Draw(g, iconRect);
                }
                leftOffset += iconSize.Width + 6;
            }

            // Close button
            if (options.ShowCloseOnSelected && state.IsSelected)
            {
                int closeSize = Math.Min(contentRect.Height - 6, 12);
                closeRect = new Rectangle(
                    contentRect.Right - closeSize,
                    contentRect.Top + (contentRect.Height - closeSize) / 2,
                    closeSize, closeSize);

                DrawCloseButton(g, closeRect, fgColor);
                rightOffset += closeSize + 6;
            }

            // Text with subtle shadow
            var textRect = new Rectangle(
                contentRect.Left + leftOffset,
                contentRect.Top,
                contentRect.Width - leftOffset - rightOffset,
                contentRect.Height);

            // Text shadow
            var shadowTextRect = textRect;
            shadowTextRect.Offset(1, 1);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(40, Color.Black)))
            {
                g.DrawString(item?.Text ?? item?.DisplayField ?? string.Empty, font, shadowBrush, shadowTextRect, _centerFormat);
            }

            // Main text
            using (var textBrush = new SolidBrush(fgColor))
            {
                g.DrawString(item?.Text ?? item?.DisplayField ?? string.Empty, font, textBrush, textRect, _centerFormat);
            }
        }

        public void RenderGroupBackground(Graphics g, Rectangle drawingRect, ChipRenderOptions options)
        {
            // No special group background
        }

        #region Private Helpers

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int r = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
            if (r <= 0) { path.AddRectangle(rect); return path; }
            int d = r * 2;
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void DrawCheckmark(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            var points = new Point[]
            {
                new Point(rect.Left + 2, rect.Top + rect.Height / 2),
                new Point(rect.Left + rect.Width / 3, rect.Bottom - 3),
                new Point(rect.Right - 2, rect.Top + 3)
            };
            g.DrawLines(pen, points);
        }

        private void DrawCloseButton(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawLine(pen, rect.Left + 2, rect.Top + 2, rect.Right - 2, rect.Bottom - 2);
            g.DrawLine(pen, rect.Right - 2, rect.Top + 2, rect.Left + 2, rect.Bottom - 2);
        }

        private int GetChipHeight(ChipSize size) => size switch
        {
            ChipSize.Small => 26,
            ChipSize.Medium => 34,
            ChipSize.Large => 42,
            _ => 34
        };

        private int GetHorizontalPadding(ChipSize size) => size switch
        {
            ChipSize.Small => 18,
            ChipSize.Medium => 24,
            ChipSize.Large => 30,
            _ => 24
        };

        private Font GetFont(ChipRenderOptions options)
        {
            float size = options.Size switch
            {
                ChipSize.Small => 8f,
                ChipSize.Medium => 9f,
                ChipSize.Large => 10f,
                _ => 9f
            };
            return new Font(options.Font?.FontFamily ?? FontFamily.GenericSansSerif, size, FontStyle.Bold);
        }

        private (Color light, Color dark, Color fg) GetColors(ChipVisualState state, ChipRenderOptions options)
        {
            var theme = options.Theme ?? _theme;
            Color primary = theme?.PrimaryColor ?? Color.FromArgb(63, 81, 181);

            primary = state.Color switch
            {
                ChipColor.Success => Color.FromArgb(76, 175, 80),
                ChipColor.Warning => Color.FromArgb(255, 152, 0),
                ChipColor.Error => Color.FromArgb(244, 67, 54),
                ChipColor.Info => Color.FromArgb(33, 150, 243),
                ChipColor.Secondary => Color.FromArgb(156, 39, 176),
                ChipColor.Dark => Color.FromArgb(66, 66, 66),
                _ => primary
            };

            if (state.IsSelected)
            {
                // Darker gradient for selected
                Color light = ControlPaint.Light(primary, 0.3f);
                Color dark = ControlPaint.Dark(primary, 0.2f);
                return (light, dark, Color.White);
            }
            else if (state.IsHovered)
            {
                // Medium gradient for hover
                Color light = Color.FromArgb(245, 245, 245);
                Color dark = Color.FromArgb(210, 210, 210);
                return (light, dark, Color.FromArgb(66, 66, 66));
            }
            else
            {
                // Light gradient for normal
                Color light = Color.FromArgb(250, 250, 250);
                Color dark = Color.FromArgb(225, 225, 225);
                return (light, dark, Color.FromArgb(97, 97, 97));
            }
        }

        #endregion
    }
}

