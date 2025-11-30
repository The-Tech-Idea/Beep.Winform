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
    /// Dashed border style chips - "Add new" placeholder feel.
    /// Perfect for showing available options or add actions.
    /// </summary>
    internal class DashedChipGroupPainter : IChipGroupPainter
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
            // Plus icon for dashed style
            extraWidth += 20;
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
            var (bgColor, fgColor, borderColor) = GetColors(state, options);

            int cornerRadius = 12;
            using var path = CreateRoundedPath(bounds, cornerRadius);

            // Very light background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }

            // Dashed or solid border based on state
            using (var pen = new Pen(borderColor, 2f))
            {
                if (!state.IsSelected)
                {
                    pen.DashStyle = DashStyle.Dash;
                    pen.DashPattern = new float[] { 5, 3 };
                }
                g.DrawPath(pen, path);
            }

            var contentRect = Rectangle.Inflate(bounds, -10, -2);
            int leftOffset = 0;
            int rightOffset = 0;

            // Plus icon (or checkmark when selected)
            int iconSize = Math.Min(contentRect.Height - 6, 14);
            var iconRect = new Rectangle(
                contentRect.Left,
                contentRect.Top + (contentRect.Height - iconSize) / 2,
                iconSize, iconSize);

            if (state.IsSelected)
            {
                DrawCheckmark(g, iconRect, fgColor);
            }
            else
            {
                DrawPlusIcon(g, iconRect, fgColor);
            }
            leftOffset += iconSize + 8;

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

        private void DrawPlusIcon(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            int cx = rect.X + rect.Width / 2;
            int cy = rect.Y + rect.Height / 2;
            int half = rect.Width / 2 - 2;
            // Horizontal line
            g.DrawLine(pen, cx - half, cy, cx + half, cy);
            // Vertical line
            g.DrawLine(pen, cx, cy - half, cx, cy + half);
        }

        private void DrawCheckmark(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            var points = new Point[]
            {
                new Point(rect.Left + 2, rect.Top + rect.Height / 2),
                new Point(rect.Left + rect.Width / 3, rect.Bottom - 2),
                new Point(rect.Right - 2, rect.Top + 2)
            };
            g.DrawLines(pen, points);
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

        private (Color bg, Color fg, Color border) GetColors(ChipVisualState state, ChipRenderOptions options)
        {
            var theme = options.Theme ?? _theme;
            Color primary = theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            Color textColor = theme?.ForeColor ?? Color.FromArgb(117, 117, 117);
            Color borderColor = theme?.BorderColor ?? Color.FromArgb(189, 189, 189);

            if (state.IsSelected)
            {
                return (Color.FromArgb(30, primary), primary, primary);
            }
            else if (state.IsHovered)
            {
                return (Color.FromArgb(250, 250, 250), Color.FromArgb(97, 97, 97), Color.FromArgb(158, 158, 158));
            }
            else
            {
                return (Color.White, textColor, borderColor);
            }
        }

        #endregion
    }
}

