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
    /// Elevated chips with strong drop shadow effect.
    /// Floating, card-like appearance with depth.
    /// </summary>
    internal class ElevatedChipGroupPainter : IChipGroupPainter
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

        private const int SHADOW_OFFSET = 3;
        private const int SHADOW_BLUR = 6;

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

            // Add shadow space
            return new Size(textSize.Width + padding + extraWidth + SHADOW_OFFSET, height + SHADOW_OFFSET);
        }

        public void RenderChip(Graphics g, SimpleItem item, Rectangle bounds, ChipVisualState state, ChipRenderOptions options, out Rectangle closeRect)
        {
            closeRect = Rectangle.Empty;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var font = GetFont(options);
            var (bgColor, fgColor, shadowColor) = GetColors(state, options);

            // Adjust bounds to account for shadow
            var chipBounds = new Rectangle(
                bounds.X,
                bounds.Y,
                bounds.Width - SHADOW_OFFSET,
                bounds.Height - SHADOW_OFFSET);

            int cornerRadius = 12;

            // Draw shadow layers (soft shadow effect)
            DrawShadow(g, chipBounds, cornerRadius, shadowColor, state.IsHovered);

            // Main chip path
            using var path = CreateRoundedPath(chipBounds, cornerRadius);

            // Background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }

            // Subtle top highlight for 3D effect
            if (!state.IsSelected)
            {
                var highlightRect = new Rectangle(chipBounds.X + 2, chipBounds.Y + 1, chipBounds.Width - 4, chipBounds.Height / 3);
                using var highlightBrush = new LinearGradientBrush(
                    highlightRect,
                    Color.FromArgb(40, Color.White),
                    Color.FromArgb(0, Color.White),
                    LinearGradientMode.Vertical);
                using var highlightPath = CreateRoundedPath(highlightRect, cornerRadius - 2);
                g.FillPath(highlightBrush, highlightPath);
            }

            var contentRect = Rectangle.Inflate(chipBounds, -10, -2);
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

            // Text
            var textRect = new Rectangle(
                contentRect.Left + leftOffset,
                contentRect.Top,
                contentRect.Width - leftOffset - rightOffset,
                contentRect.Height);

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

        private void DrawShadow(Graphics g, Rectangle chipBounds, int cornerRadius, Color shadowColor, bool isHovered)
        {
            int layers = isHovered ? 4 : 3;
            int maxOffset = isHovered ? SHADOW_OFFSET + 2 : SHADOW_OFFSET;

            for (int i = layers; i >= 1; i--)
            {
                int offset = (maxOffset * i) / layers;
                int alpha = 15 + (i * 10);
                var shadowRect = new Rectangle(
                    chipBounds.X + offset,
                    chipBounds.Y + offset,
                    chipBounds.Width,
                    chipBounds.Height);

                using var shadowPath = CreateRoundedPath(shadowRect, cornerRadius);
                using var shadowBrush = new SolidBrush(Color.FromArgb(alpha, shadowColor));
                g.FillPath(shadowBrush, shadowPath);
            }
        }

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
            return new Font(options.Font?.FontFamily ?? FontFamily.GenericSansSerif, size, FontStyle.Regular);
        }

        private (Color bg, Color fg, Color shadow) GetColors(ChipVisualState state, ChipRenderOptions options)
        {
            var theme = options.Theme ?? _theme;
            Color primary = theme?.PrimaryColor ?? Color.FromArgb(63, 81, 181);
            Color surface = theme?.CardBackColor ?? Color.White;
            Color textColor = theme?.ForeColor ?? Color.FromArgb(33, 33, 33);

            primary = state.Color switch
            {
                ChipColor.Success => Color.FromArgb(76, 175, 80),
                ChipColor.Warning => Color.FromArgb(255, 152, 0),
                ChipColor.Error => Color.FromArgb(244, 67, 54),
                ChipColor.Info => Color.FromArgb(33, 150, 243),
                ChipColor.Secondary => Color.FromArgb(156, 39, 176),
                ChipColor.Dark => Color.FromArgb(55, 71, 79),
                _ => primary
            };

            Color shadowColor = Color.FromArgb(50, 50, 50);

            if (state.IsSelected)
            {
                return (primary, Color.White, primary);
            }
            else if (state.IsHovered)
            {
                return (surface, textColor, shadowColor);
            }
            else
            {
                return (surface, textColor, shadowColor);
            }
        }

        #endregion
    }
}

