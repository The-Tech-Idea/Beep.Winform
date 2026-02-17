using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Chips;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Helpers; // For DpiScalingHelper

namespace TheTechIdea.Beep.Winform.Controls.Chips.Painters
{
    /// <summary>
    /// Pill/Stadium shaped chips - full capsule shape with height/2 corner radius.
    /// Perfect rounded ends like a medicine pill or stadium.
    /// </summary>
    internal class PillChipGroupPainter : IChipGroupPainter
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
            float scale = DpiScalingHelper.GetDpiScaleFactor(_owner);
            string text = item?.Text ?? item?.Name ?? item?.DisplayField ?? string.Empty;
            var font = GetFont(options, scale);
            var textSize = TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);

            int extraWidth = 0;
            // Icon space
            if (options.ShowIcon && !string.IsNullOrEmpty(item?.ImagePath))
                extraWidth += DpiScalingHelper.ScaleSize(options.IconMaxSize, scale).Width + DpiScalingHelper.ScaleValue(8, scale);
            // Selection check space
            if (options.ShowSelectionCheck)
                extraWidth += DpiScalingHelper.ScaleValue(18, scale);
            // Close button space
            if (options.ShowCloseOnSelected)
                extraWidth += DpiScalingHelper.ScaleValue(16, scale);

            int height = GetChipHeight(options.Size, scale);
            int horizontalPadding = height; // Pill shape needs more horizontal padding
            
            return new Size(textSize.Width + horizontalPadding + extraWidth, height);
        }

        public void RenderChip(Graphics g, SimpleItem item, Rectangle bounds, ChipVisualState state, ChipRenderOptions options, out Rectangle closeRect)
        {
            float scale = DpiScalingHelper.GetDpiScaleFactor(_owner);
            closeRect = Rectangle.Empty;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var font = GetFont(options, scale);
            var (bgColor, fgColor, borderColor) = GetColors(state, options);

            // Pill shape: corner radius = height / 2
            int pillRadius = bounds.Height / 2;
            using var path = CreatePillPath(bounds, pillRadius);

            // Background fill
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }

            // Optional border
            if (options.ShowBorders && options.BorderWidth > 0)
            {
                using var pen = new Pen(borderColor, Math.Max(1, DpiScalingHelper.ScaleValue(options.BorderWidth, scale)));
                g.DrawPath(pen, path);
            }

            // Content layout
            var contentRect = Rectangle.Inflate(bounds, -pillRadius / 2, -DpiScalingHelper.ScaleValue(2, scale));
            int leftOffset = 0;
            int rightOffset = 0;

            // Selection checkmark (left side)
            if (options.ShowSelectionCheck && state.IsSelected)
            {
                int checkSize = Math.Min(contentRect.Height - 4, DpiScalingHelper.ScaleValue(14, scale));
                var checkRect = new Rectangle(
                    contentRect.Left + 2,
                    contentRect.Top + (contentRect.Height - checkSize) / 2,
                    checkSize, checkSize);

                DrawCheckmark(g, checkRect, fgColor);
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
                    iconPath.AddEllipse(iconRect); // Circular clip for pill style
                    StyledImagePainter.PaintWithTint(g, iconPath, item.ImagePath, fgColor, 1f);
                }
                catch
                {
                    _iconRenderer.ImagePath = item.ImagePath;
                    _iconRenderer.Draw(g, iconRect);
                }
                leftOffset += iconSize.Width + DpiScalingHelper.ScaleValue(6, scale);
            }

            // Close button (right side)
            if (options.ShowCloseOnSelected && state.IsSelected)
            {
                int closeSize = Math.Min(contentRect.Height - DpiScalingHelper.ScaleValue(6, scale), DpiScalingHelper.ScaleValue(12, scale));
                closeRect = new Rectangle(
                    contentRect.Right - closeSize - 2,
                    contentRect.Top + (contentRect.Height - closeSize) / 2,
                    closeSize, closeSize);

                DrawCloseButton(g, closeRect, fgColor);
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
            // No special group background for pill style
        }

        #region Private Helpers

        private GraphicsPath CreatePillPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int r = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
            if (r <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int d = r * 2;
            // Left semicircle
            path.AddArc(rect.Left, rect.Top, d, rect.Height, 90, 180);
            // Top line
            path.AddLine(rect.Left + r, rect.Top, rect.Right - r, rect.Top);
            // Right semicircle
            path.AddArc(rect.Right - d, rect.Top, d, rect.Height, 270, 180);
            // Bottom line
            path.AddLine(rect.Right - r, rect.Bottom, rect.Left + r, rect.Bottom);
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

        private int GetChipHeight(ChipSize size, float scale) 
        {
            int val = size switch
            {
                ChipSize.Small => 26,
                ChipSize.Medium => 34,
                ChipSize.Large => 42,
                _ => 34
            };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private Font GetFont(ChipRenderOptions options, float scale = 1.0f)
        {
            return ChipFontHelpers.GetChipFont(_owner.ControlStyle, options.Size, scale);
        }

        private (Color bg, Color fg, Color border) GetColors(ChipVisualState state, ChipRenderOptions options)
        {
            var theme = options.Theme ?? _theme;
            Color primary = theme?.ButtonBorderColor ?? Color.FromArgb(33, 150, 243);

            // Apply ChipColor semantic overrides
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

            Color bg, fg, border;
            if (state.IsSelected)
            {
                bg = theme?.ButtonSelectedBackColor ?? primary;
                fg = theme?.ButtonSelectedForeColor ?? Color.White;
                border = bg;
            }
            else if (state.IsHovered)
            {
                bg = theme?.ButtonHoverBackColor ?? Color.FromArgb(30, primary);
                fg = theme?.ButtonHoverForeColor ?? (theme?.ForeColor ?? Color.Black);
                border = theme?.ButtonHoverBorderColor ?? Color.FromArgb(100, primary);
            }
            else
            {
                bg = theme?.ButtonBackColor ?? (theme?.CardBackColor ?? Color.White);
                fg = theme?.ButtonForeColor ?? (theme?.ForeColor ?? Color.Black);
                border = theme?.ButtonBorderColor ?? (theme?.BorderColor ?? Color.LightGray);
            }

            return (bg, fg, border);
        }

        #endregion
    }
}
