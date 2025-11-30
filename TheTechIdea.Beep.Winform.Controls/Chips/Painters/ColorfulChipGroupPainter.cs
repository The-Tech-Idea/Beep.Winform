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
    /// Colorful chips with vibrant gradient backgrounds.
    /// Each chip gets a unique color from a rainbow palette.
    /// </summary>
    internal class ColorfulChipGroupPainter : IChipGroupPainter
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

        // Rainbow color palette
        private static readonly (Color Primary, Color Secondary)[] ColorPalette = new[]
        {
            (Color.FromArgb(239, 83, 80), Color.FromArgb(211, 47, 47)),      // Red
            (Color.FromArgb(255, 167, 38), Color.FromArgb(245, 124, 0)),     // Orange
            (Color.FromArgb(255, 238, 88), Color.FromArgb(253, 216, 53)),    // Yellow
            (Color.FromArgb(102, 187, 106), Color.FromArgb(67, 160, 71)),    // Green
            (Color.FromArgb(66, 165, 245), Color.FromArgb(30, 136, 229)),    // Blue
            (Color.FromArgb(171, 71, 188), Color.FromArgb(142, 36, 170)),    // Purple
            (Color.FromArgb(236, 64, 122), Color.FromArgb(216, 27, 96)),     // Pink
            (Color.FromArgb(38, 166, 154), Color.FromArgb(0, 137, 123)),     // Teal
        };

        private int _chipCounter = 0;

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

            // Get color based on item hash for consistent coloring
            int colorIndex = GetColorIndex(item);
            var (primaryColor, secondaryColor) = ColorPalette[colorIndex];

            int cornerRadius = 20;
            using var path = CreateRoundedPath(bounds, cornerRadius);

            // Gradient background
            using (var gradientBrush = new LinearGradientBrush(
                bounds,
                primaryColor,
                secondaryColor,
                LinearGradientMode.Horizontal))
            {
                g.FillPath(gradientBrush, path);
            }

            // Shine effect on top
            var shineRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 2);
            using (var shineBrush = new LinearGradientBrush(
                shineRect,
                Color.FromArgb(80, Color.White),
                Color.FromArgb(0, Color.White),
                LinearGradientMode.Vertical))
            {
                using var shinePath = CreateRoundedPath(shineRect, cornerRadius);
                g.FillPath(shineBrush, shinePath);
            }

            // Selection ring
            if (state.IsSelected)
            {
                using var ringPen = new Pen(Color.White, 3f);
                g.DrawPath(ringPen, path);
            }

            // Hover glow
            if (state.IsHovered && !state.IsSelected)
            {
                using var glowPen = new Pen(Color.FromArgb(100, Color.White), 2f);
                g.DrawPath(glowPen, path);
            }

            var contentRect = Rectangle.Inflate(bounds, -12, -2);
            int leftOffset = 0;
            int rightOffset = 0;

            // Selection checkmark (white for contrast)
            if (options.ShowSelectionCheck && state.IsSelected)
            {
                int checkSize = Math.Min(contentRect.Height - 4, 14);
                var checkRect = new Rectangle(
                    contentRect.Left,
                    contentRect.Top + (contentRect.Height - checkSize) / 2,
                    checkSize, checkSize);

                // White circle background
                using (var circleBrush = new SolidBrush(Color.FromArgb(180, Color.White)))
                {
                    g.FillEllipse(circleBrush, checkRect);
                }
                DrawCheckmark(g, checkRect, secondaryColor);
                leftOffset += checkSize + 6;
            }

            // Leading icon (white tinted)
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
                    StyledImagePainter.PaintWithTint(g, iconPath, item.ImagePath, Color.White, 1f);
                }
                catch
                {
                    _iconRenderer.ImagePath = item.ImagePath;
                    _iconRenderer.Draw(g, iconRect);
                }
                leftOffset += iconSize.Width + 6;
            }

            // Close button (white)
            if (options.ShowCloseOnSelected && state.IsSelected)
            {
                int closeSize = Math.Min(contentRect.Height - 6, 12);
                closeRect = new Rectangle(
                    contentRect.Right - closeSize,
                    contentRect.Top + (contentRect.Height - closeSize) / 2,
                    closeSize, closeSize);

                DrawCloseButton(g, closeRect, Color.White);
                rightOffset += closeSize + 6;
            }

            // Text (white with subtle shadow)
            var textRect = new Rectangle(
                contentRect.Left + leftOffset,
                contentRect.Top,
                contentRect.Width - leftOffset - rightOffset,
                contentRect.Height);

            // Text shadow
            var shadowRect = textRect;
            shadowRect.Offset(1, 1);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(80, Color.Black)))
            {
                g.DrawString(item?.Text ?? item?.DisplayField ?? string.Empty, font, shadowBrush, shadowRect, _centerFormat);
            }

            // Main text
            using (var textBrush = new SolidBrush(Color.White))
            {
                g.DrawString(item?.Text ?? item?.DisplayField ?? string.Empty, font, textBrush, textRect, _centerFormat);
            }

            _chipCounter++;
        }

        public void RenderGroupBackground(Graphics g, Rectangle drawingRect, ChipRenderOptions options)
        {
            _chipCounter = 0; // Reset counter for each render pass
        }

        #region Private Helpers

        private int GetColorIndex(SimpleItem item)
        {
            // Use item's GuidId or text hash for consistent color
            string key = item?.GuidId ?? item?.Text ?? _chipCounter.ToString();
            return Math.Abs(key.GetHashCode()) % ColorPalette.Length;
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
            ChipSize.Small => 28,
            ChipSize.Medium => 36,
            ChipSize.Large => 44,
            _ => 36
        };

        private int GetHorizontalPadding(ChipSize size) => size switch
        {
            ChipSize.Small => 20,
            ChipSize.Medium => 26,
            ChipSize.Large => 32,
            _ => 26
        };

        private Font GetFont(ChipRenderOptions options)
        {
            float size = options.Size switch
            {
                ChipSize.Small => 8.5f,
                ChipSize.Medium => 9.5f,
                ChipSize.Large => 10.5f,
                _ => 9.5f
            };
            return new Font(options.Font?.FontFamily ?? FontFamily.GenericSansSerif, size, FontStyle.Bold);
        }

        #endregion
    }
}

