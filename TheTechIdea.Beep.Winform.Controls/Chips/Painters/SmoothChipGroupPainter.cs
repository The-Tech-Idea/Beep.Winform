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
    /// Smooth chips with extra rounded corners and subtle inner glow.
    /// Polished, refined appearance with soft edges.
    /// </summary>
    internal class SmoothChipGroupPainter : IChipGroupPainter
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
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var font = GetFont(options);
            var (bgColor, fgColor, glowColor) = GetColors(state, options);

            int cornerRadius = 18; // Extra smooth corners
            using var path = CreateRoundedPath(bounds, cornerRadius);

            // Outer glow/shadow
            for (int i = 3; i >= 1; i--)
            {
                var glowRect = Rectangle.Inflate(bounds, i, i);
                using var glowPath = CreateRoundedPath(glowRect, cornerRadius + i);
                using var glowBrush = new SolidBrush(Color.FromArgb(8 * i, glowColor));
                g.FillPath(glowBrush, glowPath);
            }

            // Main background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }

            // Inner highlight (top edge glow)
            var highlightRect = new Rectangle(bounds.X + 3, bounds.Y + 2, bounds.Width - 6, bounds.Height / 3);
            using (var highlightBrush = new LinearGradientBrush(
                highlightRect,
                Color.FromArgb(50, Color.White),
                Color.FromArgb(0, Color.White),
                LinearGradientMode.Vertical))
            {
                using var highlightPath = CreateRoundedPath(highlightRect, cornerRadius - 3);
                g.FillPath(highlightBrush, highlightPath);
            }

            // Subtle border
            using (var borderPen = new Pen(Color.FromArgb(30, fgColor), 1f))
            {
                g.DrawPath(borderPen, path);
            }

            var contentRect = Rectangle.Inflate(bounds, -12, -2);
            int leftOffset = 0;
            int rightOffset = 0;

            // Smooth checkmark
            if (options.ShowSelectionCheck && state.IsSelected)
            {
                int checkSize = Math.Min(contentRect.Height - 4, 14);
                var checkRect = new Rectangle(
                    contentRect.Left,
                    contentRect.Top + (contentRect.Height - checkSize) / 2,
                    checkSize, checkSize);

                DrawSmoothCheckmark(g, checkRect, fgColor);
                leftOffset += checkSize + 6;
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
                    iconPath.AddEllipse(iconRect);
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

                DrawSmoothCloseButton(g, closeRect, fgColor);
                rightOffset += closeSize + 6;
            }

            // Text with anti-aliasing
            var textRect = new Rectangle(
                contentRect.Left + leftOffset,
                contentRect.Top,
                contentRect.Width - leftOffset - rightOffset,
                contentRect.Height);

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
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

        private void DrawSmoothCheckmark(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 2.5f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };
            var points = new PointF[]
            {
                new PointF(rect.Left + 2, rect.Top + rect.Height * 0.5f),
                new PointF(rect.Left + rect.Width * 0.35f, rect.Bottom - 2),
                new PointF(rect.Right - 2, rect.Top + 2)
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

        private void DrawSmoothCloseButton(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 2f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };
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
                ChipSize.Small => 8f,
                ChipSize.Medium => 9f,
                ChipSize.Large => 10f,
                _ => 9f
            };
            return new Font(options.Font?.FontFamily ?? FontFamily.GenericSansSerif, size, FontStyle.Regular);
        }

        private (Color bg, Color fg, Color glow) GetColors(ChipVisualState state, ChipRenderOptions options)
        {
            var theme = options.Theme ?? _theme;
            Color primary = theme?.PrimaryColor ?? Color.FromArgb(103, 58, 183);
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
                return (Color.FromArgb(250, 250, 250), textColor, Color.FromArgb(100, 100, 100));
            }
            else
            {
                return (surface, textColor, Color.FromArgb(150, 150, 150));
            }
        }

        #endregion
    }
}
