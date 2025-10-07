using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// DevExpress tree painter.
    /// Features: Professional gradients, icons with badges, focus indicators, polished appearance.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class DevExpressTreePainter : BaseTreePainter
    {
        private const int CornerRadius = 2;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: gradient fill
                using (var brush = new LinearGradientBrush(
                    nodeBounds,
                    _theme.TreeNodeSelectedBackColor,
                    ControlPaint.Dark(_theme.TreeNodeSelectedBackColor, 0.05f),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, nodeBounds);
                }

                // Focus border
                using (var pen = new Pen(_theme.AccentColor, 1f))
                {
                    Rectangle focusRect = new Rectangle(
                        nodeBounds.X + 1,
                        nodeBounds.Y,
                        nodeBounds.Width - 2,
                        nodeBounds.Height - 1);
                    g.DrawRectangle(pen, focusRect);
                }
            }
            else if (isHovered)
            {
                // Hover: subtle gradient
                using (var brush = new LinearGradientBrush(
                    nodeBounds,
                    _theme.TreeNodeHoverBackColor,
                    ControlPaint.Light(_theme.TreeNodeHoverBackColor, 0.02f),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, nodeBounds);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // DevExpress plus/minus box
            Color boxColor = _theme.BorderColor;
            Color signColor = _theme.TreeForeColor;

            // Box background
            using (var boxBrush = new SolidBrush(_theme.TreeBackColor))
            {
                g.FillRectangle(boxBrush, toggleRect);
            }

            // Box border
            using (var pen = new Pen(boxColor, 1f))
            {
                g.DrawRectangle(pen, toggleRect);
            }

            // Plus/minus sign
            using (var signPen = new Pen(signColor, 1.5f))
            {
                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                // Horizontal line (always present)
                g.DrawLine(signPen, centerX - size, centerY, centerX + size, centerY);

                if (!isExpanded)
                {
                    // Vertical line (only for collapsed)
                    g.DrawLine(signPen, centerX, centerY - size, centerX, centerY + size);
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath);
                    return;
                }
                catch { }
            }

            // Default: DevExpress-style icon with gloss
            PaintDefaultDevExpressIcon(g, iconRect);
        }

        private void PaintDefaultDevExpressIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            // Folder with gradient
            int padding = iconRect.Width / 5;
            Rectangle innerRect = new Rectangle(
                iconRect.X + padding,
                iconRect.Y + padding,
                iconRect.Width - padding * 2,
                iconRect.Height - padding * 2);

            using (var path = new GraphicsPath())
            {
                // Folder shape
                int tabWidth = innerRect.Width / 3;
                int tabHeight = innerRect.Height / 4;

                path.AddLine(innerRect.Left, innerRect.Top + tabHeight, innerRect.Left + tabWidth, innerRect.Top + tabHeight);
                path.AddLine(innerRect.Left + tabWidth, innerRect.Top + tabHeight, innerRect.Left + tabWidth + 2, innerRect.Top);
                path.AddLine(innerRect.Left + tabWidth + 2, innerRect.Top, innerRect.Right, innerRect.Top);
                path.AddLine(innerRect.Right, innerRect.Top, innerRect.Right, innerRect.Bottom);
                path.AddLine(innerRect.Right, innerRect.Bottom, innerRect.Left, innerRect.Bottom);
                path.CloseFigure();

                // Gradient fill
                using (var brush = new LinearGradientBrush(
                    innerRect,
                    iconColor,
                    ControlPaint.Dark(iconColor, 0.2f),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                // Gloss effect on top
                Rectangle glossRect = new Rectangle(innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height / 2);
                using (var glossBrush = new SolidBrush(Color.FromArgb(40, Color.White)))
                {
                    using (var glossPath = new GraphicsPath())
                    {
                        glossPath.AddRectangle(glossRect);
                        g.FillPath(glossBrush, glossPath);
                    }
                }

                // Border
                using (var pen = new Pen(ControlPaint.Dark(iconColor, 0.3f), 1f))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // DevExpress uses Tahoma/Segoe UI
            Font renderFont = new Font("Segoe UI", font.Size, FontStyle.Regular);

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            renderFont.Dispose();
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Background
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
            // DevExpress standard spacing
            return Math.Max(24, base.GetPreferredRowHeight(item, font));
        }
    }
}
