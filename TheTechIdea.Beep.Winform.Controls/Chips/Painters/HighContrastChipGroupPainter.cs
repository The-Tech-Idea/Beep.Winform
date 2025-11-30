using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Painters
{
    /// <summary>
    /// High contrast accessibility-focused chips.
    /// Bold borders, high visibility, WCAG compliant colors.
    /// </summary>
    internal class HighContrastChipGroupPainter : IChipGroupPainter
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
                extraWidth += options.IconMaxSize.Width + 8;
            if (options.ShowSelectionCheck)
                extraWidth += 22;
            if (options.ShowCloseOnSelected)
                extraWidth += 18;

            int height = GetChipHeight(options.Size);
            int padding = GetHorizontalPadding(options.Size);

            return new Size(textSize.Width + padding + extraWidth, height);
        }

        public void RenderChip(Graphics g, SimpleItem item, Rectangle bounds, ChipVisualState state, ChipRenderOptions options, out Rectangle closeRect)
        {
            closeRect = Rectangle.Empty;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var font = GetFont(options);
            var (bgColor, fgColor, borderColor) = GetColors(state);

            int cornerRadius = 10;
            using var path = CreateRoundedPath(bounds, cornerRadius);

            // Solid background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }

            // Bold border (3px for high visibility)
            using (var pen = new Pen(borderColor, 3f))
            {
                g.DrawPath(pen, path);
            }

            // Focus ring for keyboard navigation
            if (state.IsFocused)
            {
                var focusRect = Rectangle.Inflate(bounds, 3, 3);
                using var focusPath = CreateRoundedPath(focusRect, cornerRadius + 3);
                using var focusPen = new Pen(Color.FromArgb(0, 120, 215), 2f) { DashStyle = DashStyle.Dot };
                g.DrawPath(focusPen, focusPath);
            }

            var contentRect = Rectangle.Inflate(bounds, -12, -2);
            int leftOffset = 0;
            int rightOffset = 0;

            // Bold checkmark
            if (options.ShowSelectionCheck && state.IsSelected)
            {
                int checkSize = Math.Min(contentRect.Height - 4, 16);
                var checkRect = new Rectangle(
                    contentRect.Left,
                    contentRect.Top + (contentRect.Height - checkSize) / 2,
                    checkSize, checkSize);

                // High contrast checkbox
                using (var boxPen = new Pen(fgColor, 2f))
                {
                    g.DrawRectangle(boxPen, checkRect);
                }
                DrawBoldCheckmark(g, checkRect, fgColor);
                leftOffset += checkSize + 8;
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
                leftOffset += iconSize.Width + 8;
            }

            // Close button
            if (options.ShowCloseOnSelected && state.IsSelected)
            {
                int closeSize = Math.Min(contentRect.Height - 6, 14);
                closeRect = new Rectangle(
                    contentRect.Right - closeSize - 2,
                    contentRect.Top + (contentRect.Height - closeSize) / 2,
                    closeSize, closeSize);

                // High contrast close button
                using (var closePen = new Pen(fgColor, 2f))
                {
                    g.DrawRectangle(closePen, closeRect);
                }
                DrawBoldCloseButton(g, Rectangle.Inflate(closeRect, -2, -2), fgColor);
                rightOffset += closeSize + 8;
            }

            // Text (bold for readability)
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

        private void DrawBoldCheckmark(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 3f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            var points = new Point[]
            {
                new Point(rect.Left + 3, rect.Top + rect.Height / 2),
                new Point(rect.Left + rect.Width / 3 + 1, rect.Bottom - 3),
                new Point(rect.Right - 3, rect.Top + 4)
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

        private void DrawBoldCloseButton(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 2.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Bottom);
            g.DrawLine(pen, rect.Right, rect.Top, rect.Left, rect.Bottom);
        }

        private int GetChipHeight(ChipSize size) => size switch
        {
            ChipSize.Small => 30,
            ChipSize.Medium => 38,
            ChipSize.Large => 46,
            _ => 38
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
                ChipSize.Small => 9f,
                ChipSize.Medium => 10f,
                ChipSize.Large => 11f,
                _ => 10f
            };
            return new Font(options.Font?.FontFamily ?? FontFamily.GenericSansSerif, size, FontStyle.Bold);
        }

        private (Color bg, Color fg, Color border) GetColors(ChipVisualState state)
        {
            // High contrast: Black on White or White on Black
            if (state.IsSelected)
            {
                return (Color.Black, Color.White, Color.Black);
            }
            else if (state.IsHovered)
            {
                return (Color.FromArgb(240, 240, 240), Color.Black, Color.Black);
            }
            else
            {
                return (Color.White, Color.Black, Color.Black);
            }
        }

        #endregion
    }
}

