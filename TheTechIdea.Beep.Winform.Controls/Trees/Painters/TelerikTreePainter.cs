using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Telerik tree painter.
    /// Features: Professional polish, subtle gradients, rich visuals, office-style appearance.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class TelerikTreePainter : BaseTreePainter
    {
        private const int CornerRadius = 2;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: gradient with glass effect
                using (var brush = new LinearGradientBrush(
                    nodeBounds,
                    ControlPaint.Light(_theme.TreeNodeSelectedBackColor, 0.1f),
                    _theme.TreeNodeSelectedBackColor,
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, nodeBounds);
                }

                // Top highlight
                using (var highlightPen = new Pen(Color.FromArgb(60, Color.White), 1f))
                {
                    g.DrawLine(highlightPen, nodeBounds.Left, nodeBounds.Top + 1, nodeBounds.Right, nodeBounds.Top + 1);
                }

                // Border
                using (var borderPen = new Pen(_theme.AccentColor, 1f))
                {
                    Rectangle borderRect = new Rectangle(
                        nodeBounds.X,
                        nodeBounds.Y,
                        nodeBounds.Width - 1,
                        nodeBounds.Height - 1);
                    g.DrawRectangle(borderPen, borderRect);
                }
            }
            else if (isHovered)
            {
                // Hover: subtle gradient
                using (var brush = new LinearGradientBrush(
                    nodeBounds,
                    ControlPaint.Light(_theme.TreeNodeHoverBackColor, 0.05f),
                    _theme.TreeNodeHoverBackColor,
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, nodeBounds);
                }

                // Subtle border
                using (var borderPen = new Pen(Color.FromArgb(40, _theme.BorderColor), 1f))
                {
                    Rectangle borderRect = new Rectangle(
                        nodeBounds.X,
                        nodeBounds.Y,
                        nodeBounds.Width - 1,
                        nodeBounds.Height - 1);
                    g.DrawRectangle(borderPen, borderRect);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Telerik triangle expand/collapse
            Color triangleColor = _theme.TreeForeColor;

            if (isHovered)
            {
                // Subtle hover background
                using (var hoverBrush = new SolidBrush(Color.FromArgb(30, _theme.AccentColor)))
                {
                    g.FillRectangle(hoverBrush, toggleRect);
                }
            }

            // Triangle
            using (var brush = new SolidBrush(triangleColor))
            {
                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                Point[] triangle;

                if (isExpanded)
                {
                    // Triangle down
                    triangle = new Point[]
                    {
                        new Point(centerX - size, centerY - size / 2),
                        new Point(centerX + size, centerY - size / 2),
                        new Point(centerX, centerY + size / 2)
                    };
                }
                else
                {
                    // Triangle right
                    triangle = new Point[]
                    {
                        new Point(centerX - size / 2, centerY - size),
                        new Point(centerX + size / 2, centerY),
                        new Point(centerX - size / 2, centerY + size)
                    };
                }

                g.FillPolygon(brush, triangle);
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

            // Default: Telerik professional icon
            PaintDefaultTelerikIcon(g, iconRect);
        }

        private void PaintDefaultTelerikIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            int padding = iconRect.Width / 5;
            Rectangle innerRect = new Rectangle(
                iconRect.X + padding,
                iconRect.Y + padding,
                iconRect.Width - padding * 2,
                iconRect.Height - padding * 2);

            // Folder with professional gradient
            using (var path = new GraphicsPath())
            {
                int tabWidth = innerRect.Width / 3;
                int tabHeight = innerRect.Height / 4;

                path.AddLine(innerRect.Left, innerRect.Top + tabHeight, innerRect.Left + tabWidth, innerRect.Top + tabHeight);
                path.AddLine(innerRect.Left + tabWidth, innerRect.Top + tabHeight, innerRect.Left + tabWidth + 2, innerRect.Top);
                path.AddLine(innerRect.Left + tabWidth + 2, innerRect.Top, innerRect.Right, innerRect.Top);
                path.AddLine(innerRect.Right, innerRect.Top, innerRect.Right, innerRect.Bottom);
                path.AddLine(innerRect.Right, innerRect.Bottom, innerRect.Left, innerRect.Bottom);
                path.CloseFigure();

                // Gradient
                using (var brush = new LinearGradientBrush(
                    innerRect,
                    ControlPaint.Light(iconColor, 0.2f),
                    ControlPaint.Dark(iconColor, 0.1f),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                // Glass highlight
                Rectangle glassRect = new Rectangle(innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height / 2);
                using (var glassBrush = new LinearGradientBrush(
                    glassRect,
                    Color.FromArgb(80, Color.White),
                    Color.FromArgb(0, Color.White),
                    LinearGradientMode.Vertical))
                {
                    using (var glassPath = new GraphicsPath())
                    {
                        glassPath.AddRectangle(glassRect);
                        g.FillPath(glassBrush, glassPath);
                    }
                }

                // Border
                using (var pen = new Pen(ControlPaint.Dark(iconColor, 0.4f), 1f))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Telerik uses Segoe UI
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
            // Telerik standard spacing
            return Math.Max(26, base.GetPreferredRowHeight(item, font));
        }
    }
}
