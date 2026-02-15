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
using TheTechIdea.Beep.Winform.Controls.Chips;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Painters
{
    /// <summary>
    /// Soft pastel colored chips with gentle, friendly appearance.
    /// Very rounded corners, subtle shadows, calming colors.
    /// </summary>
    internal class SoftChipGroupPainter : IChipGroupPainter
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

        // Pastel color palette
        private static readonly (Color Bg, Color Text)[] PastelColors = new[]
        {
            (Color.FromArgb(255, 205, 210), Color.FromArgb(183, 28, 28)),    // Pink
            (Color.FromArgb(187, 222, 251), Color.FromArgb(21, 101, 192)),   // Blue
            (Color.FromArgb(200, 230, 201), Color.FromArgb(27, 94, 32)),     // Green
            (Color.FromArgb(225, 190, 231), Color.FromArgb(106, 27, 154)),   // Purple
            (Color.FromArgb(255, 249, 196), Color.FromArgb(245, 127, 23)),   // Yellow
            (Color.FromArgb(255, 224, 178), Color.FromArgb(230, 81, 0)),     // Orange
            (Color.FromArgb(178, 235, 242), Color.FromArgb(0, 131, 143)),    // Cyan
            (Color.FromArgb(215, 204, 200), Color.FromArgb(78, 52, 46)),     // Brown
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
            float scale = DpiScalingHelper.GetDpiScaleFactor(_owner);
            string text = item?.Text ?? item?.Name ?? item?.DisplayField ?? string.Empty;
            var font = GetFont(options, scale);
            var textSize = TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);

            int extraWidth = 0;
            if (options.ShowIcon && !string.IsNullOrEmpty(item?.ImagePath))
                extraWidth += DpiScalingHelper.ScaleSize(options.IconMaxSize, scale).Width + DpiScalingHelper.ScaleValue(6, scale);
            if (options.ShowSelectionCheck)
                extraWidth += DpiScalingHelper.ScaleValue(18, scale);
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
            var (bgColor, textColor) = GetColors(item, state);

            int cornerRadius = DpiScalingHelper.ScaleValue(20, scale); // Very rounded for soft look
            using var path = CreateRoundedPath(bounds, cornerRadius);

            // Subtle shadow
            var shadowRect = bounds;
            shadowRect.Offset(2, 2);
            using (var shadowPath = CreateRoundedPath(shadowRect, cornerRadius))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(20, Color.Black)))
            {
                g.FillPath(shadowBrush, shadowPath);
            }

            // Background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }

            // Selection indicator - darker border
            if (state.IsSelected)
            {
                using var borderPen = new Pen(textColor, DpiScalingHelper.ScaleValue(2f, scale));
                g.DrawPath(borderPen, path);
            }

            // Hover effect - slight darkening
            if (state.IsHovered && !state.IsSelected)
            {
                using var hoverBrush = new SolidBrush(Color.FromArgb(15, Color.Black));
                g.FillPath(hoverBrush, path);
            }

            var contentRect = Rectangle.Inflate(bounds, -DpiScalingHelper.ScaleValue(12, scale), -DpiScalingHelper.ScaleValue(2, scale));
            int leftOffset = 0;
            int rightOffset = 0;

            // Soft checkmark
            if (options.ShowSelectionCheck && state.IsSelected)
            {
                int checkSize = Math.Min(contentRect.Height - 4, DpiScalingHelper.ScaleValue(14, scale));
                var checkRect = new Rectangle(
                    contentRect.Left,
                    contentRect.Top + (contentRect.Height - checkSize) / 2,
                    checkSize, checkSize);

                DrawSoftCheckmark(g, checkRect, textColor, scale);
                leftOffset += checkSize + DpiScalingHelper.ScaleValue(6, scale);
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
                    using var iconPath = new GraphicsPath();
                    iconPath.AddEllipse(iconRect);
                    StyledImagePainter.PaintWithTint(g, iconPath, item.ImagePath, textColor, 0.8f);
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

                DrawSoftCloseButton(g, closeRect, textColor, scale);
                rightOffset += closeSize + DpiScalingHelper.ScaleValue(6, scale);
            }

            // Text
            var textRect = new Rectangle(
                contentRect.Left + leftOffset,
                contentRect.Top,
                contentRect.Width - leftOffset - rightOffset,
                contentRect.Height);

            using (var textBrush = new SolidBrush(textColor))
            {
                g.DrawString(item?.Text ?? item?.DisplayField ?? string.Empty, font, textBrush, textRect, _centerFormat);
            }
        }

        public void RenderGroupBackground(Graphics g, Rectangle drawingRect, ChipRenderOptions options)
        {
            // No special group background
        }

        #region Private Helpers

        private (Color bg, Color text) GetColors(SimpleItem item, ChipVisualState state)
        {
            int colorIndex = Math.Abs((item?.GuidId ?? item?.Text ?? "").GetHashCode()) % PastelColors.Length;
            var (bg, text) = PastelColors[colorIndex];

            if (state.IsSelected)
            {
                // Slightly more saturated for selected
                bg = ControlPaint.Dark(bg, 0.05f);
            }

            return (bg, text);
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

        private void DrawSoftCheckmark(Graphics g, Rectangle rect, Color color, float scale)
        {
            using var pen = new Pen(color, DpiScalingHelper.ScaleValue(2.5f, scale)) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
            var points = new Point[]
            {
                new Point(rect.Left + 2, rect.Top + rect.Height / 2),
                new Point(rect.Left + rect.Width / 3, rect.Bottom - 2),
                new Point(rect.Right - 2, rect.Top + 2)
            };
            g.DrawLines(pen, points);
        }

        private void DrawSoftCloseButton(Graphics g, Rectangle rect, Color color, float scale)
        {
            using var pen = new Pen(color, DpiScalingHelper.ScaleValue(2f, scale)) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawLine(pen, rect.Left + 2, rect.Top + 2, rect.Right - 2, rect.Bottom - 2);
            g.DrawLine(pen, rect.Right - 2, rect.Top + 2, rect.Left + 2, rect.Bottom - 2);
        }

        private int GetChipHeight(ChipSize size, float scale) 
        {
            int val = size switch
            {
                ChipSize.Small => 28,
                ChipSize.Medium => 36,
                ChipSize.Large => 44,
                _ => 36
            };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private int GetHorizontalPadding(ChipSize size, float scale) 
        {
            int val = size switch
            {
                ChipSize.Small => 20,
                ChipSize.Medium => 26,
                ChipSize.Large => 32,
                _ => 26
            };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private Font GetFont(ChipRenderOptions options, float scale)
        {
            return ChipFontHelpers.GetChipFont(_owner.ControlStyle, options.Size, scale);
        }

        #endregion
    }
}
