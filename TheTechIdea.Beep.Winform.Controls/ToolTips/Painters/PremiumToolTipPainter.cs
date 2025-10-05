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
    /// Premium tooltip painter with gradient background and badge (inspired by image 1)
    /// </summary>
    public class PremiumToolTipPainter : IToolTipPainter
    {
        private const int ArrowSize = 6;
        private const int CornerRadius = 12;
        private const int Padding = 16;

        public void Paint(Graphics g, Rectangle bounds, ToolTipConfig config, ToolTipPlacement placement, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

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
            var size = ToolTipHelpers.MeasureContentSize(g, config, font, 350);
            return new Size(size.Width + 10, size.Height + 10); // Extra space for premium look
        }

        public void PaintBackground(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var (baseColor, _, _) = ToolTipHelpers.GetThemeColors(theme, config.Theme);
            
            // Create gradient from base color
            var color1 = baseColor;
            var color2 = ControlPaint.Dark(baseColor, 0.05f);

            using (var path = CreateRoundedRectangle(bounds, CornerRadius))
            using (var brush = new LinearGradientBrush(bounds, color1, color2, 135f))
            {
                // Add color blend for smooth gradient
                var colorBlend = new ColorBlend(3)
                {
                    Colors = new[] { color1, ControlPaint.Light(baseColor, 0.02f), color2 },
                    Positions = new[] { 0f, 0.5f, 1f }
                };
                brush.InterpolationColors = colorBlend;
                
                g.FillPath(brush, path);
            }

            // Add subtle inner glow
            using (var path = CreateRoundedRectangle(bounds, CornerRadius))
            using (var glowBrush = new LinearGradientBrush(
                new Point(bounds.Left, bounds.Top),
                new Point(bounds.Left, bounds.Top + 40),
                Color.FromArgb(30, 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255)))
            {
                g.FillPath(glowBrush, path);
            }
        }

        public void PaintBorder(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var (_, _, borderColor) = ToolTipHelpers.GetThemeColors(theme, config.Theme);
            
            // Enhanced border with gradient
            var lightBorder = ControlPaint.Light(borderColor, 0.2f);

            using (var path = CreateRoundedRectangle(bounds, CornerRadius))
            using (var pen = new Pen(lightBorder, 1.5f))
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

                using (var pen = new Pen(ControlPaint.Light(borderColor, 0.2f), 1.5f))
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

            // Draw premium badge in top-right corner
            DrawPremiumBadge(g, new Rectangle(bounds.Right - 70, bounds.Top + 10, 60, 24));

            // Draw image using ImagePath (like BeepButton)
            if (!string.IsNullOrEmpty(config.ImagePath) || config.Icon != null)
            {
                var iconRect = new Rectangle(contentRect.Left, currentY, 24, 24);
                DrawImageFromPath(g, iconRect, config, theme);
                contentRect = new Rectangle(contentRect.Left + 32, contentRect.Top, contentRect.Width - 32, contentRect.Height);
            }

            // Draw title with premium styling
            if (!string.IsNullOrEmpty(config.Title))
            {
                using (var titleFont = new Font(font.FontFamily, font.Size + 3, FontStyle.Bold))
                using (var brush = new SolidBrush(foreColor))
                {
                    var titleSize = TextRenderer.MeasureText(g, config.Title, titleFont);
                    
                    // Add subtle text shadow for depth
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                    {
                        g.DrawString(config.Title, titleFont, shadowBrush, 
                            contentRect.Left + 1, currentY + 1);
                    }
                    
                    g.DrawString(config.Title, titleFont, brush, contentRect.Left, currentY);
                    currentY += titleSize.Height + 8;
                }
            }

            // Draw text with enhanced readability
            if (!string.IsNullOrEmpty(config.Text))
            {
                using (var brush = new SolidBrush(foreColor))
                {
                    TextRenderer.DrawText(g, config.Text, font,
                        new Rectangle(contentRect.Left, currentY, contentRect.Width - 70, contentRect.Bottom - currentY),
                        foreColor, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
                }
            }

            // Draw close button if closable
            if (config.Closable)
            {
                DrawCloseButton(g, new Rectangle(bounds.Right - 28, bounds.Top + 10, 18, 18), foreColor);
            }
        }

        public void PaintShadow(Graphics g, Rectangle bounds, ToolTipConfig config)
        {
            // Enhanced shadow for premium look
            for (int i = 6; i > 0; i--)
            {
                var alpha = (int)(20 * (1 - i / 6.0));
                var shadowBounds = new Rectangle(
                    bounds.X + i / 2,
                    bounds.Y + i / 2,
                    bounds.Width,
                    bounds.Height
                );

                using (var path = CreateRoundedRectangle(shadowBounds, CornerRadius))
                using (var brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                {
                    g.FillPath(brush, path);
                }
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

        private void DrawPremiumBadge(Graphics g, Rectangle badgeRect)
        {
            // Gradient badge background
            using (var path = CreateRoundedRectangle(badgeRect, 4))
            using (var brush = new LinearGradientBrush(badgeRect, 
                Color.FromArgb(255, 215, 0), // Gold
                Color.FromArgb(218, 165, 32), // Darker gold
                90f))
            {
                g.FillPath(brush, path);
            }

            // Badge border
            using (var path = CreateRoundedRectangle(badgeRect, 4))
            using (var pen = new Pen(Color.FromArgb(184, 134, 11), 1))
            {
                g.DrawPath(pen, path);
            }

            // "PRO" text
            using (var font = new Font("Segoe UI", 8, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(64, 64, 64)))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString("PREMIUM", font, brush, badgeRect, sf);
            }
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
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Bottom);
                g.DrawLine(pen, rect.Right, rect.Top, rect.Left, rect.Bottom);
            }
        }
    }
}
