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
            float scale = DpiScalingHelper.GetDpiScaleFactor(_owner);
            string text = item?.Text ?? item?.Name ?? item?.DisplayField ?? string.Empty;
            var font = GetFont(options, scale);
            var textSize = TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);

            int extraWidth = 0;
            // Checkbox/checkmark space
            extraWidth += DpiScalingHelper.ScaleValue(24, scale);
            // Optional icon
            if (options.ShowIcon && !string.IsNullOrEmpty(item?.ImagePath))
                extraWidth += DpiScalingHelper.ScaleSize(options.IconMaxSize, scale).Width + DpiScalingHelper.ScaleValue(6, scale);
            // Close button
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
            var (bgColor, fgColor, borderColor, checkColor) = GetColors(state);

            int cornerRadius = DpiScalingHelper.ScaleValue(8, scale);
            using var path = CreateRoundedPath(bounds, cornerRadius);

            // Background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }

            // Border (solid or dashed based on state)
            using (var pen = new Pen(borderColor, DpiScalingHelper.ScaleValue(1.5f, scale)))
            {
                if (!state.IsSelected)
                {
                    pen.DashStyle = DashStyle.Dash;
                    pen.DashPattern = new float[] { 4, 2 };
                }
                g.DrawPath(pen, path);
            }

            var contentRect = Rectangle.Inflate(bounds, -DpiScalingHelper.ScaleValue(8, scale), -DpiScalingHelper.ScaleValue(2, scale));
            int leftOffset = 0;
            int rightOffset = 0;

            // Checkbox/Checkmark circle
            int checkSize = Math.Min(contentRect.Height - 6, DpiScalingHelper.ScaleValue(18, scale));
            var checkRect = new Rectangle(
                contentRect.Left,
                contentRect.Top + (contentRect.Height - checkSize) / 2,
                checkSize, checkSize);

            DrawCheckCircle(g, checkRect, checkColor, state.IsSelected, scale);
            leftOffset += checkSize + DpiScalingHelper.ScaleValue(8, scale);

            // Leading icon (optional, like ingredient icon)
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

        private void DrawCheckCircle(Graphics g, Rectangle rect, Color color, bool isChecked, float scale)
        {
            // Draw circle outline
            using (var pen = new Pen(color, DpiScalingHelper.ScaleValue(2f, scale)))
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
                using (var checkPen = new Pen(Color.White, DpiScalingHelper.ScaleValue(2f, scale)) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    int inset = DpiScalingHelper.ScaleValue(4, scale);
                    var points = new Point[]
                    {
                        new Point(rect.Left + inset, rect.Top + rect.Height / 2),
                        new Point(rect.Left + rect.Width / 3 + 1, rect.Bottom - inset),
                        new Point(rect.Right - inset, rect.Top + inset + 1)
                    };
                    g.DrawLines(checkPen, points);
                }
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
                ChipSize.Small => 14,
                ChipSize.Medium => 18,
                ChipSize.Large => 22,
                _ => 18
            };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private Font GetFont(ChipRenderOptions options, float scale)
        {
            return ChipFontHelpers.GetChipFont(_owner.ControlStyle, options.Size, scale);
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
