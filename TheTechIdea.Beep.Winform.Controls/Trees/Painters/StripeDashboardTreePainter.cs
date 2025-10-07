using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Stripe Dashboard tree painter.
    /// Features: Clean fintech design, data hierarchy, metric badges, professional spacing.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class StripeDashboardTreePainter : BaseTreePainter
    {
        private const int CornerRadius = 6;
        private const int MetricBadgeWidth = 40;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: clean rounded background
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Subtle border
                    using (var pen = new Pen(_theme.AccentColor, 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
            else if (isHovered)
            {
                // Hover: very subtle
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    using (var hoverBrush = new SolidBrush(_theme.TreeNodeHoverBackColor))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Stripe-style chevron
            Color chevronColor = _theme.TreeForeColor;

            using (var pen = new Pen(chevronColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                if (isExpanded)
                {
                    // Chevron down
                    g.DrawLine(pen, centerX - size, centerY - size / 2, centerX, centerY + size / 2);
                    g.DrawLine(pen, centerX, centerY + size / 2, centerX + size, centerY - size / 2);
                }
                else
                {
                    // Chevron right
                    g.DrawLine(pen, centerX - size / 2, centerY - size, centerX + size / 2, centerY);
                    g.DrawLine(pen, centerX + size / 2, centerY, centerX - size / 2, centerY + size);
                }
            }
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, string imagePath)
        {
            if (iconRect.Width <= 0 || iconRect.Height <= 0) return;

            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.StripeDashboard);
                    return;
                }
                catch { }
            }

            // Default: Stripe-style icon
            PaintDefaultStripeIcon(g, iconRect);
        }

        private void PaintDefaultStripeIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            // Clean rounded icon
            using (var path = CreateRoundedRectangle(iconRect, iconRect.Width / 4))
            {
                using (var brush = new SolidBrush(Color.FromArgb(80, iconColor)))
                {
                    g.FillPath(brush, path);
                }

                // Simple icon mark
                int padding = iconRect.Width / 3;
                Rectangle innerRect = new Rectangle(
                    iconRect.X + padding,
                    iconRect.Y + padding,
                    iconRect.Width - padding * 2,
                    iconRect.Height - padding * 2);

                using (var pen = new Pen(iconColor, 2f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;

                    // Stripe-like lines
                    int lineSpacing = innerRect.Height / 3;
                    g.DrawLine(pen, innerRect.Left, innerRect.Top, innerRect.Right, innerRect.Top);
                    g.DrawLine(pen, innerRect.Left, innerRect.Top + lineSpacing, innerRect.Right, innerRect.Top + lineSpacing);
                }
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Stripe uses clean sans-serif
            Font renderFont = new Font("Segoe UI", font.Size, isSelected ? FontStyle.Bold : FontStyle.Regular);

            // Reserve space for metric badge
            Rectangle adjustedTextRect = new Rectangle(
                textRect.X,
                textRect.Y,
                textRect.Width - MetricBadgeWidth,
                textRect.Height);

            TextRenderer.DrawText(g, text, renderFont, adjustedTextRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            // Paint metric badge (simulated)
            PaintMetricBadge(g, textRect, isSelected);

            renderFont.Dispose();
        }

        private void PaintMetricBadge(Graphics g, Rectangle textRect, bool isSelected)
        {
            // Metric badge on right side
            Rectangle badgeRect = new Rectangle(
                textRect.Right - MetricBadgeWidth,
                textRect.Y + textRect.Height / 4,
                MetricBadgeWidth - 4,
                textRect.Height / 2);

            if (badgeRect.Width > 0 && badgeRect.Height > 0)
            {
                Color badgeColor = isSelected ? _theme.AccentColor : Color.FromArgb(100, _theme.TreeForeColor);

                using (var path = CreateRoundedRectangle(badgeRect, 3))
                {
                    using (var brush = new SolidBrush(Color.FromArgb(40, badgeColor)))
                    {
                        g.FillPath(brush, path);
                    }
                }

                // Metric text
                Font metricFont = new Font("Segoe UI", 7f, FontStyle.Regular);
                using (var textBrush = new SolidBrush(badgeColor))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString("99+", metricFont, textBrush, badgeRect, sf);
                }

                metricFont.Dispose();
            }
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Clean background
            using (var brush = new SolidBrush(_theme.TreeBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.Paint(g, owner, bounds);
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            // Padding
            rect = new Rectangle(rect.X + 4, rect.Y + 2, rect.Width - 8, rect.Height - 4);

            if (rect.Width < diameter || rect.Height < diameter)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // Stripe comfortable spacing
            return Math.Max(32, base.GetPreferredRowHeight(item, font));
        }
    }
}
