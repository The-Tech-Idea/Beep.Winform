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
    /// Alert tooltip painter with left accent bar and status icons (inspired by image 3)
    /// </summary>
    public class AlertToolTipPainter : IToolTipPainter
    {
        private const int ArrowSize = 6;
        private const int CornerRadius = 6;
        private const int Padding = 12;
        private const int AccentBarWidth = 4;

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
            PaintAccentBar(g, bounds, config, theme);
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
            var size = ToolTipHelpers.MeasureContentSize(g, config, font, 320);
            return new Size(size.Width + AccentBarWidth, size.Height);
        }

        public void PaintBackground(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            // Use light/dark background based on theme
            var backColor = config.Theme == ToolTipTheme.Light 
                ? Color.White 
                : Color.FromArgb(45, 45, 48);

            using (var path = CreateRoundedRectangle(bounds, CornerRadius))
            using (var brush = new SolidBrush(backColor))
            {
                g.FillPath(brush, path);
            }
        }

        public void PaintBorder(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var borderColor = config.Theme == ToolTipTheme.Light 
                ? Color.FromArgb(217, 217, 217) 
                : Color.FromArgb(60, 60, 60);

            using (var path = CreateRoundedRectangle(bounds, CornerRadius))
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawPath(pen, path);
            }
        }

        public void PaintArrow(Graphics g, Point arrowPosition, ToolTipPlacement placement, ToolTipConfig config, IBeepTheme theme)
        {
            var backColor = config.Theme == ToolTipTheme.Light 
                ? Color.White 
                : Color.FromArgb(45, 45, 48);

            var borderColor = config.Theme == ToolTipTheme.Light 
                ? Color.FromArgb(217, 217, 217) 
                : Color.FromArgb(60, 60, 60);

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
            var foreColor = config.Theme == ToolTipTheme.Light 
                ? Color.FromArgb(33, 33, 33) 
                : Color.FromArgb(241, 241, 241);

            var font = config.Font ?? SystemFonts.DefaultFont;

            // Account for accent bar
            var contentRect = new Rectangle(
                bounds.X + Padding + AccentBarWidth + 4,
                bounds.Y + Padding,
                bounds.Width - Padding * 2 - AccentBarWidth - 4,
                bounds.Height - Padding * 2
            );

            var currentY = contentRect.Top;

            // Draw image from ImagePath if provided, otherwise draw alert icon
            var iconRect = new Rectangle(contentRect.Left, currentY, 20, 20);
            if (!string.IsNullOrEmpty(config.ImagePath) || !string.IsNullOrEmpty(config.IconPath) || config.Icon != null)
            {
                DrawImageFromPath(g, iconRect, config, theme);
            }
            else
            {
                DrawAlertIcon(g, iconRect, config.Theme, theme);
            }
            contentRect = new Rectangle(contentRect.Left + 28, contentRect.Top, contentRect.Width - 28, contentRect.Height);

            // Draw title with alert emphasis
            if (!string.IsNullOrEmpty(config.Title))
            {
                using (var titleFont = new Font(font.FontFamily, font.Size + 1, FontStyle.Bold))
                using (var brush = new SolidBrush(foreColor))
                {
                    var titleSize = TextRenderer.MeasureText(g, config.Title, titleFont);
                    TextRenderer.DrawText(g, config.Title, titleFont,
                        new Rectangle(contentRect.Left, currentY, contentRect.Width, titleSize.Height),
                        foreColor, TextFormatFlags.Left | TextFormatFlags.Top);
                    currentY += titleSize.Height + 4;
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

        private void PaintAccentBar(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var accentColor = GetAccentColor(config.Theme, theme);

            var barRect = new Rectangle(bounds.Left, bounds.Top, AccentBarWidth, bounds.Height);

            // Create rounded path for left edge only
            using (var path = new GraphicsPath())
            {
                var radius = CornerRadius;
                path.AddArc(bounds.Left, bounds.Top, radius * 2, radius * 2, 180, 90);
                path.AddLine(bounds.Left + AccentBarWidth, bounds.Top, bounds.Left + AccentBarWidth, bounds.Bottom);
                path.AddArc(bounds.Left, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                path.CloseFigure();

                using (var brush = new SolidBrush(accentColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private Color GetAccentColor(ToolTipTheme theme, IBeepTheme beepTheme)
        {
            if (beepTheme != null)
            {
                return theme switch
                {
                    ToolTipTheme.Primary => beepTheme.AccentColor,
                    ToolTipTheme.Success => beepTheme.SuccessColor,
                    ToolTipTheme.Warning => beepTheme.WarningColor,
                    ToolTipTheme.Error => beepTheme.ErrorColor,
                    ToolTipTheme.Info => beepTheme.InfoColor,
                    _ => beepTheme.AccentColor
                };
            }
            
            return theme switch
            {
                ToolTipTheme.Primary => Color.FromArgb(64, 158, 255),
                ToolTipTheme.Success => Color.FromArgb(82, 196, 26),
                ToolTipTheme.Warning => Color.FromArgb(250, 173, 20),
                ToolTipTheme.Error => Color.FromArgb(245, 63, 63),
                ToolTipTheme.Info => Color.FromArgb(24, 144, 255),
                _ => Color.FromArgb(64, 158, 255)
            };
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

        private void DrawAlertIcon(Graphics g, Rectangle iconRect, ToolTipTheme tooltipTheme, IBeepTheme theme)
        {
            var iconColor = GetAccentColor(tooltipTheme, theme);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;

            switch (tooltipTheme)
            {
                case ToolTipTheme.Success:
                    // Checkmark
                    using (var pen = new Pen(iconColor, 2))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        var points = new[]
                        {
                            new Point(iconRect.Left + 4, iconRect.Top + 10),
                            new Point(iconRect.Left + 8, iconRect.Bottom - 4),
                            new Point(iconRect.Right - 2, iconRect.Top + 4)
                        };
                        g.DrawLines(pen, points);
                    }
                    break;

                case ToolTipTheme.Warning:
                    // Exclamation mark
                    using (var pen = new Pen(iconColor, 2))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        g.DrawLine(pen, 
                            iconRect.Left + iconRect.Width / 2, iconRect.Top + 3,
                            iconRect.Left + iconRect.Width / 2, iconRect.Top + 12);
                    }
                    using (var brush = new SolidBrush(iconColor))
                    {
                        g.FillEllipse(brush, 
                            iconRect.Left + iconRect.Width / 2 - 1, iconRect.Bottom - 5, 3, 3);
                    }
                    break;

                case ToolTipTheme.Error:
                    // X mark
                    using (var pen = new Pen(iconColor, 2))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        g.DrawLine(pen, 
                            iconRect.Left + 4, iconRect.Top + 4,
                            iconRect.Right - 4, iconRect.Bottom - 4);
                        g.DrawLine(pen, 
                            iconRect.Right - 4, iconRect.Top + 4,
                            iconRect.Left + 4, iconRect.Bottom - 4);
                    }
                    break;

                case ToolTipTheme.Info:
                case ToolTipTheme.Primary:
                default:
                    // Info "i"
                    using (var brush = new SolidBrush(iconColor))
                    {
                        g.FillEllipse(brush, 
                            iconRect.Left + iconRect.Width / 2 - 1, iconRect.Top + 3, 3, 3);
                    }
                    using (var pen = new Pen(iconColor, 2))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        g.DrawLine(pen, 
                            iconRect.Left + iconRect.Width / 2, iconRect.Top + 8,
                            iconRect.Left + iconRect.Width / 2, iconRect.Bottom - 3);
                    }
                    break;
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
