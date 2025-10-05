using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Painters
{
    /// <summary>
    /// Standard tooltip painter with clean, modern look
    /// </summary>
    public class StandardToolTipPainter : IToolTipPainter
    {
        private const int ArrowSize = 6;
        private const int CornerRadius = 8;
        private const int ShadowSize = 8;
        private const int Padding = 12;

        public void Paint(Graphics g, Rectangle bounds, ToolTipConfig config, ToolTipPlacement placement, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            if (config.ShowShadow)
            {
                PaintShadow(g, bounds, config);
            }

            PaintBackground(g, bounds, config, theme);
            PaintBorder(g, bounds, config, theme);

            if (config.ShowArrow)
            {
                var arrowPos = ToolTipHelpers.CalculateArrowPosition(bounds, placement, ArrowSize);
                PaintArrow(g, arrowPos, placement, config, theme);
            }

            PaintContent(g, bounds, config, theme);
        }

        public Size CalculateSize(Graphics g, ToolTipConfig config)
        {
            var font = config.Font ?? SystemFonts.DefaultFont;
            return ToolTipHelpers.MeasureContentSize(g, config, font, 300);
        }

        public void PaintBackground(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var (backColor, _, _) = ToolTipHelpers.GetThemeColors(theme, config.Theme);
            
            using (var path = CreateRoundedRectangle(bounds, CornerRadius))
            using (var brush = new SolidBrush(backColor))
            {
                g.FillPath(brush, path);
            }
        }

        public void PaintBorder(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var (_, _, borderColor) = ToolTipHelpers.GetThemeColors(theme, config.Theme);

            using (var path = CreateRoundedRectangle(bounds, CornerRadius))
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawPath(pen, path);
            }
        }

        public void PaintArrow(Graphics g, Point arrowPosition, ToolTipPlacement placement, ToolTipConfig config, IBeepTheme theme)
        {
            var (backColor, _, borderColor) = ToolTipHelpers.GetThemeColors(theme, config.Theme);

            using (var path = ToolTipHelpers.CreateArrowPath(arrowPosition, placement, ArrowSize))
            {
                using (var brush = new SolidBrush(backColor))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(borderColor, 1))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        public void PaintContent(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var (_, foreColor, _) = ToolTipHelpers.GetThemeColors(theme, config.Theme);
            var font = config.Font ?? SystemFonts.DefaultFont;

            var contentRect = new Rectangle(
                bounds.X + Padding,
                bounds.Y + Padding,
                bounds.Width - Padding * 2,
                bounds.Height - Padding * 2
            );

            var currentY = contentRect.Top;

            // Draw image using ImagePath (like BeepButton)
            if (!string.IsNullOrEmpty(config.ImagePath) || config.Icon != null)
            {
                var iconRect = new Rectangle(contentRect.Left, currentY, 20, 20);
                DrawImageFromPath(g, iconRect, config, theme);
                contentRect = new Rectangle(contentRect.Left + 28, contentRect.Top, contentRect.Width - 28, contentRect.Height);
            }

            // Draw title
            if (!string.IsNullOrEmpty(config.Title))
            {
                using (var titleFont = new Font(font.FontFamily, font.Size + 2, FontStyle.Bold))
                using (var brush = new SolidBrush(foreColor))
                {
                    var titleSize = TextRenderer.MeasureText(g, config.Title, titleFont);
                    TextRenderer.DrawText(g, config.Title, titleFont, 
                        new Rectangle(contentRect.Left, currentY, contentRect.Width, titleSize.Height),
                        foreColor, TextFormatFlags.Left | TextFormatFlags.Top);
                    currentY += titleSize.Height + 6;
                }
            }

            // Draw text
            if (!string.IsNullOrEmpty(config.Text))
            {
                using (var brush = new SolidBrush(foreColor))
                {
                    TextRenderer.DrawText(g, config.Text, font,
                        new Rectangle(contentRect.Left, currentY, contentRect.Width, contentRect.Bottom - currentY),
                        foreColor, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
                }
            }

            // Draw close button if closable
            if (config.Closable)
            {
                DrawCloseButton(g, new Rectangle(bounds.Right - 24, bounds.Top + 8, 16, 16), foreColor);
            }
        }

        public void PaintShadow(Graphics g, Rectangle bounds, ToolTipConfig config)
        {
            var shadowBounds = new Rectangle(
                bounds.X + 2,
                bounds.Y + 2,
                bounds.Width,
                bounds.Height
            );

            using (var path = CreateRoundedRectangle(shadowBounds, CornerRadius))
            using (var brush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
            {
                g.FillPath(brush, path);
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;

            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void DrawImageFromPath(Graphics g, Rectangle iconRect, ToolTipConfig config, IBeepTheme theme)
        {
            // Use ImagePainter to load and draw SVG/images with theme support
            string imagePath = config.ImagePath ?? config.IconPath;
            
            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    using (var painter = new ImagePainter(imagePath, theme))
                    {
                        painter.ApplyThemeOnImage = config.ApplyThemeOnImage;
                        painter.ScaleMode = ImageScaleMode.KeepAspectRatio;
                        painter.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
                        
                        if (config.ApplyThemeOnImage && theme != null)
                        {
                            painter.ApplyThemeToSvg();
                        }
                        
                        // Draw the image using ImagePainter
                        painter.DrawImage(g, iconRect);
                    }
                }
                catch { /* Image loading failed, skip */ }
            }
            else if (config.Icon != null)
            {
                g.DrawImage(config.Icon, iconRect);
            }
        }

        private void DrawCloseButton(Graphics g, Rectangle rect, Color color)
        {
            using (var pen = new Pen(color, 2))
            {
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Bottom);
                g.DrawLine(pen, rect.Right, rect.Top, rect.Left, rect.Bottom);
            }
        }
    }
}
