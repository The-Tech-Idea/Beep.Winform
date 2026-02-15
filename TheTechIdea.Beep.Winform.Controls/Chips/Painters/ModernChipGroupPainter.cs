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
    /// Modern Material Design inspired chips.
    /// Subtle elevation, smooth transitions, filled dot indicator.
    /// </summary>
    internal class ModernChipGroupPainter : IChipGroupPainter
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
            if (options.ShowIcon && !string.IsNullOrEmpty(item?.ImagePath))
                extraWidth += DpiScalingHelper.ScaleSize(options.IconMaxSize, scale).Width + DpiScalingHelper.ScaleValue(8, scale);
            if (options.ShowSelectionCheck)
                extraWidth += DpiScalingHelper.ScaleValue(16, scale); // Dot indicator
            if (options.ShowCloseOnSelected)
                extraWidth += DpiScalingHelper.ScaleValue(18, scale);

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
            var (bgColor, fgColor, accentColor) = GetColors(state, options);

            int cornerRadius = DpiScalingHelper.ScaleValue(16, scale);
            using var path = CreateRoundedPath(bounds, cornerRadius);

            // Subtle shadow for elevation
            if (!state.IsSelected)
            {
                var shadowRect = bounds;
                shadowRect.Offset(1, 2);
                using var shadowPath = CreateRoundedPath(shadowRect, cornerRadius);
                using var shadowBrush = new SolidBrush(Color.FromArgb(15, Color.Black));
                g.FillPath(shadowBrush, shadowPath);
            }

            // Background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }

            // Ripple effect hint on hover (color pulse)
            if (state.IsHovered && !state.IsSelected)
            {
                using var rippleBrush = new SolidBrush(Color.FromArgb(8, accentColor));
                g.FillPath(rippleBrush, path);
            }

            var contentRect = Rectangle.Inflate(bounds, -DpiScalingHelper.ScaleValue(10, scale), -DpiScalingHelper.ScaleValue(2, scale));
            int leftOffset = 0;
            int rightOffset = 0;

            // Filled dot indicator for selection
            if (options.ShowSelectionCheck && state.IsSelected)
            {
                int dotSize = Math.Min(contentRect.Height - 10, DpiScalingHelper.ScaleValue(10, scale));
                var dotRect = new Rectangle(
                    contentRect.Left + 2,
                    contentRect.Top + (contentRect.Height - dotSize) / 2,
                    dotSize, dotSize);

                using (var dotBrush = new SolidBrush(fgColor))
                {
                    g.FillEllipse(dotBrush, dotRect);
                }
                leftOffset += dotSize + DpiScalingHelper.ScaleValue(8, scale);
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
                    StyledImagePainter.PaintWithTint(g, iconPath, item.ImagePath, fgColor, 1f);
                }
                catch
                {
                    _iconRenderer.ImagePath = item.ImagePath;
                    _iconRenderer.Draw(g, iconRect);
                }
                leftOffset += iconSize.Width + DpiScalingHelper.ScaleValue(8, scale);
            }

            // Close button
            if (options.ShowCloseOnSelected && state.IsSelected)
            {
                int closeSize = Math.Min(contentRect.Height - 6, DpiScalingHelper.ScaleValue(14, scale));
                closeRect = new Rectangle(
                    contentRect.Right - closeSize - 2,
                    contentRect.Top + (contentRect.Height - closeSize) / 2,
                    closeSize, closeSize);

                // Circle background for close
                using (var closeBgBrush = new SolidBrush(Color.FromArgb(30, fgColor)))
                {
                    g.FillEllipse(closeBgBrush, closeRect);
                }
                DrawCloseButton(g, Rectangle.Inflate(closeRect, -2, -2), fgColor, scale);
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

        private void DrawCloseButton(Graphics g, Rectangle rect, Color color, float scale)
        {
            using var pen = new Pen(color, DpiScalingHelper.ScaleValue(1.5f, scale)) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawLine(pen, rect.Left + 1, rect.Top + 1, rect.Right - 1, rect.Bottom - 1);
            g.DrawLine(pen, rect.Right - 1, rect.Top + 1, rect.Left + 1, rect.Bottom - 1);
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

        private (Color bg, Color fg, Color accent) GetColors(ChipVisualState state, ChipRenderOptions options)
        {
            var theme = options.Theme ?? _theme;
            Color primary = theme?.PrimaryColor ?? Color.FromArgb(98, 0, 238); // Material purple
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

            if (state.IsSelected)
            {
                return (primary, Color.White, primary);
            }
            else if (state.IsHovered)
            {
                return (Color.FromArgb(245, 245, 245), textColor, primary);
            }
            else
            {
                return (surface, textColor, primary);
            }
        }

        #endregion
    }
}
