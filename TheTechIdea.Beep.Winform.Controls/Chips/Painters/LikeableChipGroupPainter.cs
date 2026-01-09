using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Painters
{
    /// <summary>
    /// Likeable/Favorite style chips with heart icons and pink/red theme.
    /// Perfect for favorites, likes, wishlists.
    /// </summary>
    internal class LikeableChipGroupPainter : IChipGroupPainter
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

        // Likeable color palette
        private static readonly Color PinkPrimary = Color.FromArgb(255, 107, 157);      // #FF6B9D
        private static readonly Color PinkSelected = Color.FromArgb(233, 30, 99);       // #E91E63
        private static readonly Color PinkHover = Color.FromArgb(255, 143, 177);        // #FF8FB1
        private static readonly Color PinkLight = Color.FromArgb(252, 228, 236);        // #FCE4EC
        private static readonly Color HeartRed = Color.FromArgb(229, 57, 53);           // #E53935

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
            // Heart icon always shown
            extraWidth += 22; // Heart icon space
            // Close button
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
            var (bgColor, fgColor, heartColor) = GetColors(state);

            int cornerRadius = 16;
            using var path = CreateRoundedPath(bounds, cornerRadius);

            // Gradient background for selected state
            if (state.IsSelected)
            {
                using var gradientBrush = new LinearGradientBrush(
                    bounds,
                    PinkPrimary,
                    PinkSelected,
                    LinearGradientMode.Horizontal);
                g.FillPath(gradientBrush, path);
            }
            else
            {
                using var bgBrush = new SolidBrush(bgColor);
                g.FillPath(bgBrush, path);
            }

            // Subtle border for unselected
            if (!state.IsSelected)
            {
                using var pen = new Pen(Color.FromArgb(80, PinkPrimary), 1f);
                g.DrawPath(pen, path);
            }

            var contentRect = Rectangle.Inflate(bounds, -10, -2);
            int leftOffset = 0;
            int rightOffset = 0;

            // Heart icon (always on left)
            int heartSize = Math.Min(contentRect.Height - 4, 18);
            var heartRect = new Rectangle(
                contentRect.Left,
                contentRect.Top + (contentRect.Height - heartSize) / 2,
                heartSize, heartSize);

            DrawHeart(g, heartRect, heartColor, state.IsSelected);
            leftOffset += heartSize + 8;

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
            // Optional: subtle pink tint for the group area
        }

        #region Private Helpers

        private void DrawHeart(Graphics g, Rectangle rect, Color color, bool filled)
        {
            // Try to use SVG icon first
            try
            {
                using var heartPath = CreateHeartPath(rect);
                if (filled)
                {
                    using var brush = new SolidBrush(color);
                    g.FillPath(brush, heartPath);
                }
                else
                {
                    using var pen = new Pen(color, 1.5f);
                    g.DrawPath(pen, heartPath);
                }
            }
            catch
            {
                // Fallback: draw simple heart shape
                using var heartPath = CreateHeartPath(rect);
                if (filled)
                {
                    using var brush = new SolidBrush(color);
                    g.FillPath(brush, heartPath);
                }
                else
                {
                    using var pen = new Pen(color, 1.5f);
                    g.DrawPath(pen, heartPath);
                }
            }
        }

        private GraphicsPath CreateHeartPath(Rectangle rect)
        {
            var path = new GraphicsPath();
            float cx = rect.X + rect.Width / 2f;
            float cy = rect.Y + rect.Height / 2f;
            float size = Math.Min(rect.Width, rect.Height);

            // Heart shape using bezier curves
            float w = size * 0.5f;
            float h = size * 0.5f;

            path.AddBezier(
                cx, cy + h * 0.7f,           // Bottom point
                cx - w, cy + h * 0.1f,       // Left control
                cx - w, cy - h * 0.5f,       // Left top
                cx, cy - h * 0.2f);          // Top center

            path.AddBezier(
                cx, cy - h * 0.2f,           // Top center
                cx + w, cy - h * 0.5f,       // Right top
                cx + w, cy + h * 0.1f,       // Right control
                cx, cy + h * 0.7f);          // Bottom point

            path.CloseFigure();
            return path;
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
            ChipSize.Small => 16,
            ChipSize.Medium => 22,
            ChipSize.Large => 28,
            _ => 22
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

        private (Color bg, Color fg, Color heart) GetColors(ChipVisualState state)
        {
            if (state.IsSelected)
            {
                return (PinkSelected, Color.White, Color.White);
            }
            else if (state.IsHovered)
            {
                return (PinkHover, Color.FromArgb(136, 14, 79), HeartRed);
            }
            else
            {
                return (PinkLight, Color.FromArgb(136, 14, 79), PinkPrimary);
            }
        }

        #endregion
    }
}

