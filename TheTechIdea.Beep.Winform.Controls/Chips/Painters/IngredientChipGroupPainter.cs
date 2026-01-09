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
    /// Ingredient/Tag style chips with checkmarks and green theme.
    /// Perfect for recipes, filters, tags, categories.
    /// </summary>
    internal class IngredientChipGroupPainter : IChipGroupPainter
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

        // Ingredient/Green color palette
        private static readonly Color GreenPrimary = Color.FromArgb(76, 175, 80);       // #4CAF50
        private static readonly Color GreenSelected = Color.FromArgb(46, 125, 50);      // #2E7D32
        private static readonly Color GreenLight = Color.FromArgb(232, 245, 233);       // #E8F5E9
        private static readonly Color GreenHover = Color.FromArgb(200, 230, 201);       // #C8E6C9
        private static readonly Color GreenBorder = Color.FromArgb(129, 199, 132);      // #81C784
        private static readonly Color TextDark = Color.FromArgb(27, 94, 32);            // #1B5E20

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
            // Checkbox/checkmark space
            extraWidth += 24;
            // Optional icon
            if (options.ShowIcon && !string.IsNullOrEmpty(item?.ImagePath))
                extraWidth += options.IconMaxSize.Width + 6;
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
            var (bgColor, fgColor, borderColor, checkColor) = GetColors(state);

            int cornerRadius = 8;
            using var path = CreateRoundedPath(bounds, cornerRadius);

            // Background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }

            // Border (solid or dashed based on state)
            using (var pen = new Pen(borderColor, 1.5f))
            {
                if (!state.IsSelected)
                {
                    pen.DashStyle = DashStyle.Dash;
                    pen.DashPattern = new float[] { 4, 2 };
                }
                g.DrawPath(pen, path);
            }

            var contentRect = Rectangle.Inflate(bounds, -8, -2);
            int leftOffset = 0;
            int rightOffset = 0;

            // Checkbox/Checkmark circle
            int checkSize = Math.Min(contentRect.Height - 6, 18);
            var checkRect = new Rectangle(
                contentRect.Left,
                contentRect.Top + (contentRect.Height - checkSize) / 2,
                checkSize, checkSize);

            DrawCheckCircle(g, checkRect, checkColor, state.IsSelected);
            leftOffset += checkSize + 8;

            // Leading icon (optional, like ingredient icon)
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

        private void DrawCheckCircle(Graphics g, Rectangle rect, Color color, bool isChecked)
        {
            // Draw circle outline
            using (var pen = new Pen(color, 2f))
            {
                g.DrawEllipse(pen, rect);
            }

            // Draw checkmark if selected
            if (isChecked)
            {
                // Fill circle
                using (var brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, rect);
                }

                // Draw white checkmark
                using var checkPen = new Pen(Color.White, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                int inset = 4;
                var points = new Point[]
                {
                    new Point(rect.Left + inset, rect.Top + rect.Height / 2),
                    new Point(rect.Left + rect.Width / 3 + 1, rect.Bottom - inset),
                    new Point(rect.Right - inset, rect.Top + inset + 1)
                };
                g.DrawLines(checkPen, points);
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
            ChipSize.Small => 14,
            ChipSize.Medium => 18,
            ChipSize.Large => 22,
            _ => 18
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

        private (Color bg, Color fg, Color border, Color check) GetColors(ChipVisualState state)
        {
            if (state.IsSelected)
            {
                return (GreenHover, TextDark, GreenSelected, GreenSelected);
            }
            else if (state.IsHovered)
            {
                return (GreenLight, TextDark, GreenPrimary, GreenPrimary);
            }
            else
            {
                return (Color.White, Color.FromArgb(97, 97, 97), GreenBorder, GreenBorder);
            }
        }

        #endregion
    }
}

