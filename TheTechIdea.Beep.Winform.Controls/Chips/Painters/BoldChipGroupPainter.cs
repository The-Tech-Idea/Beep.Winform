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
    /// Bold text chips with 30% opacity background tint.
    /// Strong visual emphasis with semi-transparent backgrounds.
    /// </summary>
    internal class BoldChipGroupPainter : IChipGroupPainter
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
            var font = GetFont(options); // Bold font
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
            var (bgColor, fgColor, accentColor) = GetColors(state, options);

            int cornerRadius = 10;
            using var path = CreateRoundedPath(bounds, cornerRadius);

            // 30% opacity background
            using (var bgBrush = new SolidBrush(Color.FromArgb(77, accentColor))) // ~30% of 255
            {
                g.FillPath(bgBrush, path);
            }

            // Stronger background on selection
            if (state.IsSelected)
            {
                using var selectedBrush = new SolidBrush(Color.FromArgb(180, accentColor));
                g.FillPath(selectedBrush, path);
            }

            // Hover effect
            if (state.IsHovered && !state.IsSelected)
            {
                using var hoverBrush = new SolidBrush(Color.FromArgb(40, accentColor));
                g.FillPath(hoverBrush, path);
            }

            var contentRect = Rectangle.Inflate(bounds, -10, -2);
            int leftOffset = 0;
            int rightOffset = 0;

            // Bold checkmark
            if (options.ShowSelectionCheck && state.IsSelected)
            {
                int checkSize = Math.Min(contentRect.Height - 4, 14);
                var checkRect = new Rectangle(
                    contentRect.Left,
                    contentRect.Top + (contentRect.Height - checkSize) / 2,
                    checkSize, checkSize);

                DrawBoldCheckmark(g, checkRect, fgColor);
                leftOffset += checkSize + 6;
            }

            // Leading icon (bold/thick style)
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

                DrawBoldCloseButton(g, closeRect, fgColor);
                rightOffset += closeSize + 6;
            }

            // Bold text
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
            using var pen = new Pen(color, 3f) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
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

        private void DrawBoldCloseButton(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 2.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
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
                ChipSize.Small => 8.5f,
                ChipSize.Medium => 9.5f,
                ChipSize.Large => 10.5f,
                _ => 9.5f
            };
            return new Font(options.Font?.FontFamily ?? FontFamily.GenericSansSerif, size, FontStyle.Bold);
        }

        private (Color bg, Color fg, Color accent) GetColors(ChipVisualState state, ChipRenderOptions options)
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
                return (primary, Color.White, primary);
            }
            else
            {
                return (primary, primary, primary);
            }
        }

        #endregion
    }
}

