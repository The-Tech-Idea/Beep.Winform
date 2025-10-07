using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Notion minimal tree painter.
    /// Features: Clean design, subtle hover states, emoji icons, minimal spacing.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class NotionMinimalTreePainter : BaseTreePainter
    {
        private const int MinimalPadding = 2;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            // Notion uses very subtle backgrounds
            if (isSelected)
            {
                // Selected: flat background, no borders
                using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                {
                    g.FillRectangle(brush, nodeBounds);
                }
            }
            else if (isHovered)
            {
                // Hover: extremely subtle
                using (var hoverBrush = new SolidBrush(_theme.TreeNodeHoverBackColor))
                {
                    g.FillRectangle(hoverBrush, nodeBounds);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Notion style: simple arrow
            Color arrowColor = _theme.TreeForeColor;

            using (var pen = new Pen(arrowColor, 1.5f))
            {
                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 4;

                if (isExpanded)
                {
                    // Arrow down
                    g.DrawLine(pen, centerX - size, centerY - size / 2, centerX, centerY + size / 2);
                    g.DrawLine(pen, centerX, centerY + size / 2, centerX + size, centerY - size / 2);
                }
                else
                {
                    // Arrow right
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.NotionMinimal);
                    return;
                }
                catch { }
            }

            // Default: emoji-style icon (simple square with character)
            PaintDefaultNotionIcon(g, iconRect);
        }

        private void PaintDefaultNotionIcon(Graphics g, Rectangle iconRect)
        {
            // Notion icons are typically emoji or simple shapes
            Color iconBg = Color.FromArgb(40, _theme.AccentColor);

            using (var brush = new SolidBrush(iconBg))
            {
                g.FillRectangle(brush, iconRect);
            }

            // Simple document icon
            Font iconFont = new Font("Segoe UI Emoji", iconRect.Height * 0.6f, FontStyle.Regular);
            string iconChar = "📄";

            using (var textBrush = new SolidBrush(_theme.TreeForeColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(iconChar, iconFont, textBrush, iconRect, sf);
            }

            iconFont.Dispose();
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Notion uses clean sans-serif
            Font renderFont = new Font("Segoe UI", font.Size, FontStyle.Regular);

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            renderFont.Dispose();
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

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // Notion uses compact spacing
            return Math.Max(26, base.GetPreferredRowHeight(item, font));
        }
    }
}
