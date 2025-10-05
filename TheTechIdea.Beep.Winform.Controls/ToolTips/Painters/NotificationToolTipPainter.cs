using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Painters
{
    /// <summary>
    /// Notification-style tooltip painter with large icon, action buttons, and timestamp
    /// Inspired by modern OS notification panels
    /// </summary>
    public class NotificationToolTipPainter : IToolTipPainter
    {
        public void Paint(Graphics g, Rectangle bounds, ToolTipConfig config, ToolTipPlacement actualPlacement, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (config.ShowShadow)
                PaintShadow(g, bounds, config, theme);

            PaintBackground(g, bounds, config, theme);
            PaintBorder(g, bounds, config, theme);
            PaintContent(g, bounds, config, theme);

            if (config.ShowArrow)
                PaintArrow(g, bounds, config, actualPlacement, theme);
        }

        public void PaintBackground(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var (backColor, _, _) = ToolTipHelpers.GetThemeColors(theme, config.Theme);

            using (var path = CreateRoundedRectangle(bounds, 12))
            {
                // Slight gradient for depth
                using (var brush = new LinearGradientBrush(
                    bounds,
                    backColor,
                    ControlPaint.Light(backColor, 0.05f),
                    90f))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        public void PaintBorder(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var (_, _, borderColor) = ToolTipHelpers.GetThemeColors(theme, config.Theme);

            using (var path = CreateRoundedRectangle(bounds, 12))
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawPath(pen, path);
            }
        }

        public void PaintArrow(Graphics g, Rectangle bounds, ToolTipConfig config, ToolTipPlacement actualPlacement, IBeepTheme theme)
        {
            var (backColor, _, borderColor) = ToolTipHelpers.GetThemeColors(theme, config.Theme);
            var arrowPoints = ToolTipHelpers.CalculateArrowPosition(bounds, actualPlacement);

            if (arrowPoints.Length > 0)
            {
                using (var arrowPath = ToolTipHelpers.CreateArrowPath(arrowPoints))
                using (var brush = new SolidBrush(backColor))
                using (var pen = new Pen(borderColor, 1))
                {
                    g.FillPath(brush, arrowPath);
                    g.DrawPath(pen, arrowPath);
                }
            }
        }

        public void PaintContent(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var (backColor, foreColor, _) = ToolTipHelpers.GetThemeColors(theme, config.Theme);
            var contentRect = new Rectangle(bounds.X + 16, bounds.Y + 16, bounds.Width - 32, bounds.Height - 32);

            // Large icon on the left (48x48)
            int iconSize = 48;
            var iconRect = new Rectangle(contentRect.X, contentRect.Y, iconSize, iconSize);

            if (!string.IsNullOrEmpty(config.ImagePath) || !string.IsNullOrEmpty(config.IconPath) || config.Icon != null)
            {
                DrawImageFromPath(g, iconRect, config, theme);
            }
            else
            {
                // Default notification bell icon
                DrawNotificationIcon(g, iconRect, foreColor);
            }

            // Content area (right of icon)
            int textX = iconRect.Right + 12;
            int textWidth = contentRect.Right - textX;

            // Title (bold, larger)
            if (!string.IsNullOrEmpty(config.Title))
            {
                using (var titleFont = new Font(config.Font?.FontFamily ?? FontFamily.GenericSansSerif, 11f, FontStyle.Bold))
                using (var brush = new SolidBrush(foreColor))
                {
                    var titleRect = new Rectangle(textX, contentRect.Y, textWidth, 20);
                    g.DrawString(config.Title, titleFont, brush, titleRect, new StringFormat
                    {
                        LineAlignment = StringAlignment.Near,
                        Alignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter
                    });
                }
            }

            // Timestamp (small, muted)
            string timestamp = DateTime.Now.ToString("HH:mm");
            using (var timeFont = new Font(config.Font?.FontFamily ?? FontFamily.GenericSansSerif, 7.5f, FontStyle.Regular))
            using (var timeBrush = new SolidBrush(ColorUtils.ChangeColorBrightness(foreColor, -0.3f)))
            {
                var timeSize = g.MeasureString(timestamp, timeFont);
                var timeRect = new Rectangle(
                    contentRect.Right - (int)timeSize.Width,
                    contentRect.Y + 2,
                    (int)timeSize.Width,
                    (int)timeSize.Height);
                g.DrawString(timestamp, timeFont, timeBrush, timeRect);
            }

            // Message text (below title)
            if (!string.IsNullOrEmpty(config.Text))
            {
                int titleHeight = !string.IsNullOrEmpty(config.Title) ? 24 : 0;
                using (var textFont = config.Font ?? new Font(FontFamily.GenericSansSerif, 9f))
                using (var brush = new SolidBrush(ColorUtils.ChangeColorBrightness(foreColor, -0.2f)))
                {
                    var textRect = new Rectangle(
                        textX,
                        contentRect.Y + titleHeight,
                        textWidth,
                        iconSize - titleHeight - 8);

                    g.DrawString(config.Text, textFont, brush, textRect, new StringFormat
                    {
                        LineAlignment = StringAlignment.Near,
                        Alignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter,
                        FormatFlags = StringFormatFlags.LineLimit
                    });
                }
            }

            // Action buttons at bottom (if closable or has actions)
            if (config.Closable)
            {
                int buttonY = iconRect.Bottom + 12;
                DrawActionButtons(g, contentRect, buttonY, foreColor, theme);
            }

            // Close button (X) in top-right
            if (config.Closable)
            {
                var closeRect = new Rectangle(bounds.Right - 32, bounds.Top + 8, 24, 24);
                DrawCloseButton(g, closeRect, foreColor);
            }
        }

        public void PaintShadow(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
        {
            var shadowBounds = new Rectangle(bounds.X + 4, bounds.Y + 4, bounds.Width, bounds.Height);

            using (var path = CreateRoundedRectangle(shadowBounds, 12))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
            {
                g.FillPath(shadowBrush, path);
            }
        }

        public Size CalculateSize(ToolTipConfig config, IBeepTheme theme)
        {
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                // Notification tooltips are larger and wider
                int iconSize = 48;
                int padding = 16;
                int iconTextGap = 12;
                int buttonHeight = config.Closable ? 32 : 0;

                // Measure title
                int titleHeight = 0;
                if (!string.IsNullOrEmpty(config.Title))
                {
                    using (var titleFont = new Font(config.Font?.FontFamily ?? FontFamily.GenericSansSerif, 11f, FontStyle.Bold))
                    {
                        titleHeight = 20;
                    }
                }

                // Measure text
                int textHeight = 0;
                int textWidth = 0;
                if (!string.IsNullOrEmpty(config.Text))
                {
                    using (var textFont = config.Font ?? new Font(FontFamily.GenericSansSerif, 9f))
                    {
                        var maxTextWidth = 320; // Max notification width
                        var textSize = g.MeasureString(config.Text, textFont, maxTextWidth);
                        textHeight = (int)Math.Ceiling(textSize.Height);
                        textWidth = (int)Math.Ceiling(textSize.Width);
                    }
                }

                // Calculate dimensions
                int contentHeight = Math.Max(iconSize, titleHeight + textHeight + 8);
                int totalWidth = padding * 2 + iconSize + iconTextGap + Math.Max(textWidth, 280);
                int totalHeight = padding * 2 + contentHeight + buttonHeight + (buttonHeight > 0 ? 12 : 0);

                // Clamp to max size
                if (config.MaxSize.Width > 0)
                    totalWidth = Math.Min(totalWidth, config.MaxSize.Width);
                if (config.MaxSize.Height > 0)
                    totalHeight = Math.Min(totalHeight, config.MaxSize.Height);

                // Notification tooltips minimum size
                totalWidth = Math.Max(totalWidth, 360);
                totalHeight = Math.Max(totalHeight, 100);

                return new Size(totalWidth, totalHeight);
            }
        }

        #region Helper Methods

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void DrawImageFromPath(Graphics g, Rectangle iconRect, ToolTipConfig config, IBeepTheme theme)
        {
            string imagePath = config.ImagePath ?? config.IconPath;

            if (!string.IsNullOrEmpty(imagePath))
            {
                using (var painter = new ImagePainter(imagePath, theme))
                {
                    if (config.ApplyThemeOnImage)
                        painter.ApplyThemeToSvg();

                    painter.DrawImage(g, iconRect);
                }
            }
            else if (config.Icon != null)
            {
                g.DrawImage(config.Icon, iconRect);
            }
        }

        private void DrawNotificationIcon(Graphics g, Rectangle rect, Color color)
        {
            // Draw a simple bell icon
            using (var pen = new Pen(color, 2))
            using (var brush = new SolidBrush(Color.FromArgb(40, color)))
            {
                // Bell body
                var bellRect = new Rectangle(
                    rect.X + rect.Width / 4,
                    rect.Y + rect.Height / 6,
                    rect.Width / 2,
                    rect.Height / 2);

                g.FillEllipse(brush, bellRect);
                g.DrawArc(pen, bellRect, 180, 180);

                // Bell bottom
                int bellBottom = bellRect.Bottom;
                g.DrawLine(pen,
                    bellRect.X - 4, bellBottom,
                    bellRect.Right + 4, bellBottom);

                // Bell clapper
                var clapperRect = new Rectangle(
                    rect.X + rect.Width / 2 - 3,
                    bellBottom,
                    6, 8);
                g.FillEllipse(brush, clapperRect);
            }
        }

        private void DrawActionButtons(Graphics g, Rectangle contentRect, int buttonY, Color foreColor, IBeepTheme theme)
        {
            int buttonWidth = 80;
            int buttonHeight = 28;
            int buttonSpacing = 8;

            // "Dismiss" button
            var dismissRect = new Rectangle(
                contentRect.Right - buttonWidth,
                buttonY,
                buttonWidth,
                buttonHeight);

            DrawButton(g, dismissRect, "Dismiss", foreColor, theme, false);

            // "View" button (primary)
            var viewRect = new Rectangle(
                dismissRect.X - buttonWidth - buttonSpacing,
                buttonY,
                buttonWidth,
                buttonHeight);

            DrawButton(g, viewRect, "View", foreColor, theme, true);
        }

        private void DrawButton(Graphics g, Rectangle rect, string text, Color foreColor, IBeepTheme theme, bool isPrimary)
        {
            using (var path = CreateRoundedRectangle(rect, 4))
            {
                if (isPrimary)
                {
                    using (var brush = new SolidBrush(theme.AccentColor))
                    {
                        g.FillPath(brush, path);
                    }

                    using (var font = new Font(FontFamily.GenericSansSerif, 8.5f, FontStyle.Regular))
                    using (var brush = new SolidBrush(Color.White))
                    {
                        var sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        g.DrawString(text, font, brush, rect, sf);
                    }
                }
                else
                {
                    using (var pen = new Pen(ColorUtils.ChangeColorBrightness(foreColor, -0.3f), 1))
                    {
                        g.DrawPath(pen, path);
                    }

                    using (var font = new Font(FontFamily.GenericSansSerif, 8.5f, FontStyle.Regular))
                    using (var brush = new SolidBrush(ColorUtils.ChangeColorBrightness(foreColor, -0.2f)))
                    {
                        var sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        g.DrawString(text, font, brush, rect, sf);
                    }
                }
            }
        }

        private void DrawCloseButton(Graphics g, Rectangle rect, Color foreColor)
        {
            using (var pen = new Pen(ColorUtils.ChangeColorBrightness(foreColor, -0.3f), 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int inset = 6;
                g.DrawLine(pen,
                    rect.X + inset, rect.Y + inset,
                    rect.Right - inset, rect.Bottom - inset);
                g.DrawLine(pen,
                    rect.Right - inset, rect.Y + inset,
                    rect.X + inset, rect.Bottom - inset);
            }
        }

        #endregion
    }
}
