using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Microsoft Fluent Design 2 tree painter.
    /// Features: Acrylic blur effects, subtle reveals, smooth animations, modern spacing.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class Fluent2TreePainter : BaseTreePainter
    {
        private const int CornerRadius = 4;
        private const int RevealBorderWidth = 1;
        private const float AcrylicTintOpacity = 0.15f;

        /// <summary>
        /// Fluent2-specific node painting with acrylic backgrounds and reveal borders.
        /// Features: Acrylic tint overlays, reveal borders on selection/hover, subtle mica texture.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for Fluent Design
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw acrylic background with rounded corners
                using (var acrylicPath = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    // Base surface color
                    Color surfaceColor = isSelected ? _theme.TreeNodeSelectedBackColor :
                                        isHovered ? _theme.TreeNodeHoverBackColor :
                                        Color.Transparent;

                    if (isSelected || isHovered)
                    {
                        using (var surfaceBrush = new SolidBrush(surfaceColor))
                        {
                            g.FillPath(surfaceBrush, acrylicPath);
                        }

                        // STEP 2: Acrylic tint overlay (only on selection)
                        if (isSelected)
                        {
                            using (var tintBrush = new SolidBrush(
                                Color.FromArgb((int)(255 * AcrylicTintOpacity), _theme.AccentColor)))
                            {
                                g.FillPath(tintBrush, acrylicPath);
                            }
                        }

                        // STEP 3: Reveal border
                        Color borderColor = isSelected ? _theme.AccentColor :
                                          Color.FromArgb(60, _theme.TreeForeColor);
                        
                        using (var revealPen = new Pen(borderColor, RevealBorderWidth))
                        {
                            g.DrawPath(revealPen, acrylicPath);
                        }
                    }
                }

                // STEP 4: Draw Fluent chevron toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color chevronColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;

                    // Subtle circular hover background
                    if (isHovered)
                    {
                        using (var hoverBrush = new SolidBrush(Color.FromArgb(20, chevronColor)))
                        {
                            g.FillEllipse(hoverBrush, toggleRect);
                        }
                    }

                    // Draw Fluent-style chevron
                    using (var pen = new Pen(chevronColor, 1.5f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;

                        int centerX = toggleRect.Left + toggleRect.Width / 2;
                        int centerY = toggleRect.Top + toggleRect.Height / 2;
                        int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                        if (node.Item.IsExpanded)
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

                // STEP 5: Draw Fluent checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
                    var borderColor = isHovered ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    // Fluent rounded checkbox
                    using (var checkPath = CreateRoundedRectangle(checkRect, 2))
                    {
                        using (var bgBrush = new SolidBrush(bgColor))
                        {
                            g.FillPath(bgBrush, checkPath);
                        }

                        using (var borderPen = new Pen(borderColor, 1.5f))
                        {
                            g.DrawPath(borderPen, checkPath);
                        }
                    }

                    // Fluent checkmark
                    if (node.Item.IsChecked)
                    {
                        using (var checkPen = new Pen(Color.White, 1.5f))
                        {
                            checkPen.StartCap = LineCap.Round;
                            checkPen.EndCap = LineCap.Round;

                            var points = new Point[]
                            {
                                new Point(checkRect.X + checkRect.Width / 4, checkRect.Y + checkRect.Height / 2),
                                new Point(checkRect.X + checkRect.Width / 2 - 1, checkRect.Y + checkRect.Height * 3 / 4),
                                new Point(checkRect.X + checkRect.Width * 3 / 4, checkRect.Y + checkRect.Height / 4)
                            };
                            g.DrawLines(checkPen, points);
                        }
                    }
                }

                // STEP 6: Draw Fluent icon
                if (!string.IsNullOrEmpty(node.Item.ImagePath) && node.IconRectContent != Rectangle.Empty)
                {
                    PaintIcon(g, node.IconRectContent, node.Item.ImagePath);
                }

                // STEP 7: Draw text with Fluent typography
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    // Fluent uses Segoe UI Variable
                    using (var renderFont = new Font("Segoe UI", _owner.TextFont.Size, FontStyle.Regular))
                    {
                        TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, renderFont, textRect, textColor,
                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                    }
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
                g.TextRenderingHint = oldTextRendering;
            }
        }

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: acrylic surface with subtle gradient
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    // Acrylic base
                    using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Tint overlay
                    using (var tintBrush = new SolidBrush(Color.FromArgb((int)(255 * AcrylicTintOpacity), _theme.AccentColor)))
                    {
                        g.FillPath(tintBrush, path);
                    }

                    // Reveal border (accent)
                    using (var pen = new Pen(_theme.AccentColor, RevealBorderWidth))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
            else if (isHovered)
            {
                // Hover: subtle reveal effect
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    using (var hoverBrush = new SolidBrush(_theme.TreeNodeHoverBackColor))
                    {
                        g.FillPath(hoverBrush, path);
                    }

                    // Reveal border (subtle)
                    using (var pen = new Pen(Color.FromArgb(60, _theme.TreeForeColor), RevealBorderWidth))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Fluent chevron
            Color chevronColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;

            if (isHovered)
            {
                // Subtle hover background
                using (var hoverBrush = new SolidBrush(Color.FromArgb(20, chevronColor)))
                {
                    g.FillEllipse(hoverBrush, toggleRect);
                }
            }

            // Chevron icon
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.Fluent2);
                    return;
                }
                catch { }
            }

            // Default Fluent icon (segoe fluent icons style)
            PaintDefaultFluentIcon(g, iconRect);
        }

        private void PaintDefaultFluentIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            // Fluent icon style: outlined with subtle fill
            using (var path = new GraphicsPath())
            {
                int padding = iconRect.Width / 5;
                Rectangle innerRect = new Rectangle(
                    iconRect.X + padding,
                    iconRect.Y + padding,
                    iconRect.Width - padding * 2,
                    iconRect.Height - padding * 2);

                // Folder shape
                path.AddLine(innerRect.Left, innerRect.Top + innerRect.Height / 4, 
                             innerRect.Left + innerRect.Width / 3, innerRect.Top + innerRect.Height / 4);
                path.AddLine(innerRect.Left + innerRect.Width / 3, innerRect.Top + innerRect.Height / 4,
                             innerRect.Left + innerRect.Width / 3 + 3, innerRect.Top);
                path.AddLine(innerRect.Left + innerRect.Width / 3 + 3, innerRect.Top,
                             innerRect.Right, innerRect.Top);
                path.AddLine(innerRect.Right, innerRect.Top, innerRect.Right, innerRect.Bottom);
                path.AddLine(innerRect.Right, innerRect.Bottom, innerRect.Left, innerRect.Bottom);
                path.CloseFigure();

                // Subtle fill
                using (var fillBrush = new SolidBrush(Color.FromArgb(30, iconColor)))
                {
                    g.FillPath(fillBrush, path);
                }

                // Outline
                using (var pen = new Pen(iconColor, 1.5f))
                {
                    pen.LineJoin = LineJoin.Round;
                    g.DrawPath(pen, path);
                }
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Fluent typography (Segoe UI Variable)
            Font renderFont = new Font("Segoe UI", font.Size, isSelected ? FontStyle.Regular : FontStyle.Regular);

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            renderFont.Dispose();
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Background with subtle mica effect
            using (var brush = new SolidBrush(_theme.TreeBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Subtle noise texture (mica simulation)
            PaintMicaTexture(g, bounds);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.Paint(g, owner, bounds);
        }

        private void PaintMicaTexture(Graphics g, Rectangle bounds)
        {
            // Very subtle noise for mica effect
            Random rand = new Random(bounds.GetHashCode());
            using (var pen = new Pen(Color.FromArgb(3, _theme.TreeForeColor)))
            {
                for (int i = 0; i < bounds.Width * bounds.Height / 2000; i++)
                {
                    int x = rand.Next(bounds.Left, bounds.Right);
                    int y = rand.Next(bounds.Top, bounds.Bottom);
                    g.DrawRectangle(pen, x, y, 1, 1);
                }
            }
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
            // Fluent comfortable spacing
            return Math.Max(32, base.GetPreferredRowHeight(item, font));
        }
    }
}
