using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Vercel clean tree painter.
    /// Features: Ultra-minimal, monospace text, subtle borders, clean spacing.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class VercelCleanTreePainter : BaseTreePainter
    {
        private const int BorderWidth = 1;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: flat with left accent border
                using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                {
                    g.FillRectangle(brush, nodeBounds);
                }

                // Left accent border
                using (var pen = new Pen(_theme.AccentColor, 2f))
                {
                    g.DrawLine(pen, nodeBounds.Left, nodeBounds.Top, nodeBounds.Left, nodeBounds.Bottom);
                }
            }
            else if (isHovered)
            {
                // Hover: very subtle border
                using (var hoverBrush = new SolidBrush(_theme.TreeNodeHoverBackColor))
                {
                    g.FillRectangle(hoverBrush, nodeBounds);
                }

                using (var pen = new Pen(Color.FromArgb(30, _theme.BorderColor), BorderWidth))
                {
                    g.DrawRectangle(pen, nodeBounds);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Vercel style: minimalist plus/minus
            Color iconColor = _theme.TreeForeColor;

            using (var pen = new Pen(iconColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 4;

                if (isExpanded)
                {
                    // Minus (horizontal line)
                    g.DrawLine(pen, centerX - size, centerY, centerX + size, centerY);
                }
                else
                {
                    // Plus (horizontal + vertical)
                    g.DrawLine(pen, centerX - size, centerY, centerX + size, centerY);
                    g.DrawLine(pen, centerX, centerY - size, centerX, centerY + size);
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.VercelClean);
                    return;
                }
                catch { }
            }

            // Default: minimal outlined icon
            PaintDefaultVercelIcon(g, iconRect);
        }

        private void PaintDefaultVercelIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.TreeForeColor;

            // Simple outlined square
            int padding = iconRect.Width / 4;
            Rectangle innerRect = new Rectangle(
                iconRect.X + padding,
                iconRect.Y + padding,
                iconRect.Width - padding * 2,
                iconRect.Height - padding * 2);

            using (var pen = new Pen(iconColor, 1f))
            {
                g.DrawRectangle(pen, innerRect);

                // Diagonal line for "folder"
                g.DrawLine(pen, innerRect.Left, innerRect.Top, innerRect.Right, innerRect.Bottom);
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Vercel uses monospace fonts
            Font renderFont = new Font("Consolas", font.Size, FontStyle.Regular);

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            renderFont.Dispose();
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Ultra-clean background
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
            // Vercel compact spacing
            return Math.Max(28, base.GetPreferredRowHeight(item, font));
        }
    }
}
