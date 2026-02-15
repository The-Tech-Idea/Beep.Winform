using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Chips;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Painters
{
    /// <summary>
    /// Square chips with minimal corner rounding (4px).
    /// Sharp, modern, clean aesthetic.
    /// </summary>
    internal class SquareChipGroupPainter : IChipGroupPainter
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

        private const int BASE_CORNER_RADIUS = 4; // Minimal rounding

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
            float scale = DpiScalingHelper.GetDpiScaleFactor(_owner);
            string text = item?.Text ?? item?.Name ?? item?.DisplayField ?? string.Empty;
            var font = GetFont(options, scale);
            var textSize = TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);

            int extraWidth = 0;
            if (options.ShowIcon && !string.IsNullOrEmpty(item?.ImagePath))
                extraWidth += DpiScalingHelper.ScaleSize(options.IconMaxSize, scale).Width + DpiScalingHelper.ScaleValue(6, scale);
            if (options.ShowSelectionCheck)
                extraWidth += DpiScalingHelper.ScaleValue(20, scale); // Square checkbox
            if (options.ShowCloseOnSelected)
                extraWidth += DpiScalingHelper.ScaleValue(16, scale);

            int height = GetChipHeight(options.Size, scale);
            int padding = GetHorizontalPadding(options.Size, scale);

            return new Size(textSize.Width + padding + extraWidth, height);
        }

        public void RenderChip(Graphics g, SimpleItem item, Rectangle bounds, ChipVisualState state, ChipRenderOptions options, out Rectangle closeRect)
        {
            float scale = DpiScalingHelper.GetDpiScaleFactor(_owner);
            closeRect = Rectangle.Empty;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var font = GetFont(options, scale);
            var (bgColor, fgColor, borderColor) = GetColors(state, options);

            int cornerRadius = DpiScalingHelper.ScaleValue(BASE_CORNER_RADIUS, scale);
            using var path = CreateSquarePath(bounds, cornerRadius);

            // Background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }

            // Border (always show for square style)
            using (var pen = new Pen(borderColor, Math.Max(1, DpiScalingHelper.ScaleValue(options.BorderWidth, scale))))
            {
                g.DrawPath(pen, path);
            }

            var contentRect = Rectangle.Inflate(bounds, -DpiScalingHelper.ScaleValue(8, scale), -DpiScalingHelper.ScaleValue(2, scale));
            int leftOffset = 0;
            int rightOffset = 0;

            // Square checkbox style selection mark
            if (options.ShowSelectionCheck)
            {
                int boxSize = Math.Min(contentRect.Height - 6, DpiScalingHelper.ScaleValue(14, scale));
                var boxRect = new Rectangle(
                    contentRect.Left,
                    contentRect.Top + (contentRect.Height - boxSize) / 2,
                    boxSize, boxSize);

                // Draw checkbox outline
                using (var boxPen = new Pen(state.IsSelected ? fgColor : borderColor, DpiScalingHelper.ScaleValue(1.5f, scale)))
                {
                    g.DrawRectangle(boxPen, boxRect);
                }

                // Draw checkmark if selected
                if (state.IsSelected)
                {
                    DrawCheckmark(g, boxRect, fgColor, scale);
                }

                leftOffset += boxSize + DpiScalingHelper.ScaleValue(8, scale);
            }

            // Leading icon
            if (options.ShowIcon && !string.IsNullOrEmpty(item?.ImagePath))
            {
                var iconSize = DpiScalingHelper.ScaleSize(options.IconMaxSize, scale);
                var iconRect = new Rectangle(
                    contentRect.Left + leftOffset,
                    contentRect.Top + (contentRect.Height - iconSize.Height) / 2,
                    iconSize.Width, iconSize.Height);

                try
                {
                    // Square clip for square style
                    using var iconPath = CreateSquarePath(iconRect, 2); // 2 is small enough to generally ignore scaling for tiny radii or scale checks
                    StyledImagePainter.PaintWithTint(g, iconPath, item.ImagePath, fgColor, 1f);
                }
                catch
                {
                    _iconRenderer.ImagePath = item.ImagePath;
                    _iconRenderer.Draw(g, iconRect);
                }
                leftOffset += iconSize.Width + DpiScalingHelper.ScaleValue(6, scale);
            }

            // Close button
            if (options.ShowCloseOnSelected && state.IsSelected)
            {
                int closeSize = Math.Min(contentRect.Height - 6, DpiScalingHelper.ScaleValue(12, scale));
                closeRect = new Rectangle(
                    contentRect.Right - closeSize,
                    contentRect.Top + (contentRect.Height - closeSize) / 2,
                    closeSize, closeSize);

                DrawCloseButton(g, closeRect, fgColor, scale);
                rightOffset += closeSize + DpiScalingHelper.ScaleValue(6, scale);
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

        private GraphicsPath CreateSquarePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int r = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
            if (r <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int d = r * 2;
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void DrawCheckmark(Graphics g, Rectangle rect, Color color, float scale)
        {
            using var pen = new Pen(color, DpiScalingHelper.ScaleValue(2f, scale)) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            var points = new Point[]
            {
                new Point(rect.Left + 3, rect.Top + rect.Height / 2),
                new Point(rect.Left + rect.Width / 3 + 1, rect.Bottom - 3),
                new Point(rect.Right - 2, rect.Top + 3)
            };
            g.DrawLines(pen, points);
        }

        private void DrawCloseButton(Graphics g, Rectangle rect, Color color, float scale)
        {
            using var pen = new Pen(color, DpiScalingHelper.ScaleValue(1.5f, scale)) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawLine(pen, rect.Left + 2, rect.Top + 2, rect.Right - 2, rect.Bottom - 2);
            g.DrawLine(pen, rect.Right - 2, rect.Top + 2, rect.Left + 2, rect.Bottom - 2);
        }

        private int GetChipHeight(ChipSize size, float scale)
        {
            int val = size switch
            {
                ChipSize.Small => 24,
                ChipSize.Medium => 32,
                ChipSize.Large => 40,
                _ => 32
            };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private int GetHorizontalPadding(ChipSize size, float scale)
        {
            int val = size switch
            {
                ChipSize.Small => 16,
                ChipSize.Medium => 20,
                ChipSize.Large => 24,
                _ => 20
            };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private Font GetFont(ChipRenderOptions options, float scale)
        {
            return ChipFontHelpers.GetChipFont(_owner.ControlStyle, options.Size, scale);
        }

        private (Color bg, Color fg, Color border) GetColors(ChipVisualState state, ChipRenderOptions options)
        {
            var theme = options.Theme ?? _theme;
            Color primary = theme?.PrimaryColor ?? Color.FromArgb(63, 81, 181);
            Color surface = theme?.CardBackColor ?? Color.White;
            Color textColor = theme?.ForeColor ?? Color.FromArgb(33, 33, 33);
            Color borderColor = theme?.BorderColor ?? Color.FromArgb(189, 189, 189);

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

            Color bg, fg, border;
            if (state.IsSelected)
            {
                bg = primary;
                fg = Color.White;
                border = primary;
            }
            else if (state.IsHovered)
            {
                bg = Color.FromArgb(20, primary);
                fg = textColor;
                border = Color.FromArgb(150, primary);
            }
            else
            {
                bg = surface;
                fg = textColor;
                border = borderColor;
            }

            return (bg, fg, border);
        }

        #endregion
    }
}
