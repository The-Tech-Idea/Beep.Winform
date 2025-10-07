using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Tailwind CSS card-based tree painter.
    /// Features: Card containers for nodes, utility-first design, shadow layers, clean spacing.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class TailwindCardTreePainter : BaseTreePainter
    {
        private const int CornerRadius = 8;
        private const int CardPadding = 6;
        private const int ShadowOffset = 2;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: card with shadow (shadow-lg)
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    // Shadow layer
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                    {
                        var shadowRect = new Rectangle(
                            nodeBounds.X,
                            nodeBounds.Y + ShadowOffset,
                            nodeBounds.Width,
                            nodeBounds.Height);

                        using (var shadowPath = CreateRoundedRectangle(shadowRect, CornerRadius))
                        {
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }

                    // Card
                    using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Ring (border)
                    using (var pen = new Pen(_theme.AccentColor, 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
            else if (isHovered)
            {
                // Hover: subtle card with shadow (shadow-md)
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    // Shadow
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
                    {
                        var shadowRect = new Rectangle(
                            nodeBounds.X,
                            nodeBounds.Y + 1,
                            nodeBounds.Width,
                            nodeBounds.Height);

                        using (var shadowPath = CreateRoundedRectangle(shadowRect, CornerRadius))
                        {
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }

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

            // Tailwind chevron
            Color chevronColor = _theme.TreeForeColor;

            if (isHovered)
            {
                // Hover background
                using (var hoverBrush = new SolidBrush(Color.FromArgb(20, _theme.AccentColor)))
                {
                    g.FillEllipse(hoverBrush, toggleRect);
                }
            }

            using (var pen = new Pen(chevronColor, 2f))
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.TailwindCard);
                    return;
                }
                catch { }
            }

            // Default: Tailwind-style icon
            PaintDefaultTailwindIcon(g, iconRect);
        }

        private void PaintDefaultTailwindIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            // Rounded square with gradient
            using (var path = CreateRoundedRectangle(iconRect, iconRect.Width / 4))
            {
                // Gradient background
                Color topColor = Color.FromArgb(120, iconColor);
                Color bottomColor = Color.FromArgb(80, iconColor);

                using (var brush = new LinearGradientBrush(iconRect, topColor, bottomColor, LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                // Icon symbol
                int padding = iconRect.Width / 4;
                Rectangle innerRect = new Rectangle(
                    iconRect.X + padding,
                    iconRect.Y + padding,
                    iconRect.Width - padding * 2,
                    iconRect.Height - padding * 2);

                using (var pen = new Pen(Color.FromArgb(200, iconColor), 1.5f))
                {
                    g.DrawRectangle(pen, innerRect);
                }
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Tailwind uses Inter/system fonts
            Font renderFont = new Font("Segoe UI", font.Size, isSelected ? FontStyle.Bold : FontStyle.Regular);

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

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            // Card padding
            rect = new Rectangle(rect.X + CardPadding, rect.Y + 2, rect.Width - CardPadding * 2, rect.Height - 4);

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
            // Tailwind comfortable spacing
            return Math.Max(36, base.GetPreferredRowHeight(item, font));
        }
    }
}
