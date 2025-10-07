using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Google Material Design 3 tree painter.
    /// Features: Elevation shadows, rounded corners, material colors, state layers.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class Material3TreePainter : BaseTreePainter
    {
        private const int ElevationOffset = 2;
        private const int CornerRadius = 8;
        private const int StateLayerAlpha = 12;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: elevated surface with shadow
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    // Elevation shadow
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                    {
                        var shadowRect = new Rectangle(
                            nodeBounds.X + ElevationOffset,
                            nodeBounds.Y + ElevationOffset,
                            nodeBounds.Width,
                            nodeBounds.Height);

                        using (var shadowPath = CreateRoundedRectangle(shadowRect, CornerRadius))
                        {
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }

                    // Surface
                    using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // State layer
                    using (var stateBrush = new SolidBrush(Color.FromArgb(StateLayerAlpha * 2, _theme.AccentColor)))
                    {
                        g.FillPath(stateBrush, path);
                    }
                }
            }
            else if (isHovered)
            {
                // Hover: state layer over surface
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    using (var hoverBrush = new SolidBrush(_theme.TreeNodeHoverBackColor))
                    {
                        g.FillPath(hoverBrush, path);
                    }

                    // State layer
                    using (var stateBrush = new SolidBrush(Color.FromArgb(StateLayerAlpha, _theme.TreeForeColor)))
                    {
                        g.FillPath(stateBrush, path);
                    }
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Material icon button
            Color iconColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;

            if (isHovered)
            {
                // Icon button ripple effect (circular background)
                using (var rippleBrush = new SolidBrush(Color.FromArgb(StateLayerAlpha, iconColor)))
                {
                    g.FillEllipse(rippleBrush, toggleRect);
                }
            }

            // Icon
            using (var pen = new Pen(iconColor, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.Material3);
                    return;
                }
                catch { }
            }

            // Default Material icon
            PaintDefaultMaterialIcon(g, iconRect);
        }

        private void PaintDefaultMaterialIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            // Filled rounded square (Material icon container)
            using (var path = CreateRoundedRectangle(iconRect, 4))
            {
                using (var brush = new SolidBrush(Color.FromArgb(100, iconColor)))
                {
                    g.FillPath(brush, path);
                }
            }

            // Icon symbol (folder)
            int padding = iconRect.Width / 4;
            Rectangle symbolRect = new Rectangle(
                iconRect.X + padding,
                iconRect.Y + padding,
                iconRect.Width - padding * 2,
                iconRect.Height - padding * 2);

            using (var pen = new Pen(iconColor, 1.5f))
            {
                pen.LineJoin = LineJoin.Round;
                g.DrawRectangle(pen, symbolRect);
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Material typography (Roboto-style)
            Font renderFont = new Font("Segoe UI", font.Size, isSelected ? FontStyle.Bold : FontStyle.Regular);

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            renderFont.Dispose();
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Background surface
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

            // Shrink for padding
            rect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4);

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
            // Material Design needs comfortable touch targets (48dp minimum)
            return Math.Max(32, base.GetPreferredRowHeight(item, font));
        }
    }
}
