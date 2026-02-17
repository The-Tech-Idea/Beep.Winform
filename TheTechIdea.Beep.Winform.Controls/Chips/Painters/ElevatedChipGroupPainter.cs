using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Chips;

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

        private const int BASE_SHADOW_OFFSET = 3;
        private const int BASE_SHADOW_BLUR = 6;

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
            int shadowOffset = DpiScalingHelper.ScaleValue(BASE_SHADOW_OFFSET, scale);

            // Add shadow space
            return new Size(textSize.Width + padding + extraWidth + shadowOffset, height + shadowOffset);
        }

        public void RenderChip(Graphics g, SimpleItem item, Rectangle bounds, ChipVisualState state, ChipRenderOptions options, out Rectangle closeRect)
        {
            float scale = DpiScalingHelper.GetDpiScaleFactor(_owner);
            closeRect = Rectangle.Empty;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var font = GetFont(options, scale);
            var (bgColor, fgColor, shadowColor) = GetColors(state, options);
            int shadowOffset = DpiScalingHelper.ScaleValue(BASE_SHADOW_OFFSET, scale);

            // Adjust bounds to account for shadow
            var chipBounds = new Rectangle(
                bounds.X,
                bounds.Y,
                bounds.Width - shadowOffset,
                bounds.Height - shadowOffset);

            int cornerRadius = DpiScalingHelper.ScaleValue(12, scale);

            // Draw shadow layers (soft shadow effect)
            DrawShadow(g, chipBounds, cornerRadius, shadowColor, state.IsHovered, scale);

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

            var contentRect = Rectangle.Inflate(chipBounds, -DpiScalingHelper.ScaleValue(10, scale), -DpiScalingHelper.ScaleValue(2, scale));
            int leftOffset = 0;
            int rightOffset = 0;

            // Selection checkmark
            if (options.ShowSelectionCheck && state.IsSelected)
            {
                int checkSize = Math.Min(contentRect.Height - 4, DpiScalingHelper.ScaleValue(14, scale));
                var checkRect = new Rectangle(
                    contentRect.Left,
                    contentRect.Top + (contentRect.Height - checkSize) / 2,
                    checkSize, checkSize);

                DrawCheckmark(g, checkRect, fgColor, scale);
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
                    iconPath.AddRectangle(iconRect);
                    StyledImagePainter.PaintWithTint(g, iconPath, item?.ImagePath, fgColor, 1f);
                }
                catch
                {
                    _iconRenderer.ImagePath = item?.ImagePath;
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

        private void DrawShadow(Graphics g, Rectangle chipBounds, int cornerRadius, Color shadowColor, bool isHovered, float scale)
        {
            int layers = isHovered ? 4 : 3;
            float baseShadowOffset = DpiScalingHelper.ScaleValue(BASE_SHADOW_OFFSET, scale);
            float maxOffset = isHovered ? baseShadowOffset + DpiScalingHelper.ScaleValue(2, scale) : baseShadowOffset;

            for (int i = layers; i >= 1; i--)
            {
                int offset = (int)((maxOffset * i) / layers);
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

        private void DrawCheckmark(Graphics g, Rectangle rect, Color color, float scale)
        {
            using var pen = new Pen(color, DpiScalingHelper.ScaleValue(2f, scale)) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            var points = new Point[]
            {
                new Point(rect.Left + 2, rect.Top + rect.Height / 2),
                new Point(rect.Left + rect.Width / 3, rect.Bottom - 3),
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
                ChipSize.Small => 26,
                ChipSize.Medium => 34,
                ChipSize.Large => 42,
                _ => 34
            };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private int GetHorizontalPadding(ChipSize size, float scale)
        {
            int val = size switch
            {
                ChipSize.Small => 18,
                ChipSize.Medium => 24,
                ChipSize.Large => 30,
                _ => 24
            };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private Font GetFont(ChipRenderOptions options, float scale)
        {
            return ChipFontHelpers.GetChipFont(_owner.ControlStyle, options.Size, scale);
        }

        private (Color bg, Color fg, Color shadow) GetColors(ChipVisualState state, ChipRenderOptions options)
        {
            var theme = options.Theme ?? _theme;
            Color primary = theme?.ButtonBorderColor ?? Color.FromArgb(63, 81, 181);

            // Override for semantic colors
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
                Color bg = theme?.ButtonSelectedBackColor ?? primary;
                Color fg = theme?.ButtonSelectedForeColor ?? Color.White;
                return (bg, fg, bg);
            }
            else if (state.IsHovered)
            {
                Color bg = theme?.ButtonHoverBackColor ?? (theme?.CardBackColor ?? Color.White);
                Color fg = theme?.ButtonHoverForeColor ?? (theme?.ForeColor ?? Color.FromArgb(33, 33, 33));
                return (bg, fg, shadowColor);
            }
            else
            {
                Color bg = theme?.ButtonBackColor ?? (theme?.CardBackColor ?? Color.White);
                Color fg = theme?.ButtonForeColor ?? (theme?.ForeColor ?? Color.FromArgb(33, 33, 33));
                return (bg, fg, shadowColor);
            }
        }

        #endregion
    }
}
