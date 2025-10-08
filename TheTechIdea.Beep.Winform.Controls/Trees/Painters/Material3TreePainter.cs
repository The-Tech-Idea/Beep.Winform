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

        /// <summary>
        /// Material3-specific node painting with elevation shadows, state layers, and rounded corners.
        /// Features: Shadow elevation on selection, ripple effects, material icon buttons.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for Material Design
            var oldSmoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            try
            {
                // STEP 1: Draw elevation shadow FIRST (if selected)
                if (isSelected)
                {
                    using (var shadowPath = CreateRoundedRectangle(
                        new Rectangle(
                            nodeBounds.X + ElevationOffset,
                            nodeBounds.Y + ElevationOffset,
                            nodeBounds.Width,
                            nodeBounds.Height),
                        CornerRadius))
                    {
                        using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                        {
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }
                }

                // STEP 2: Draw rounded background surface
                using (var surfacePath = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    Color surfaceColor = isSelected ? _theme.TreeNodeSelectedBackColor :
                                        isHovered ? _theme.TreeNodeHoverBackColor :
                                        _theme.TreeBackColor;

                    using (var surfaceBrush = new SolidBrush(surfaceColor))
                    {
                        g.FillPath(surfaceBrush, surfacePath);
                    }

                    // STEP 3: Draw state layer (Material Design overlay)
                    if (isSelected)
                    {
                        using (var stateBrush = new SolidBrush(Color.FromArgb(StateLayerAlpha * 2, _theme.AccentColor)))
                        {
                            g.FillPath(stateBrush, surfacePath);
                        }
                    }
                    else if (isHovered)
                    {
                        using (var stateBrush = new SolidBrush(Color.FromArgb(StateLayerAlpha, _theme.TreeForeColor)))
                        {
                            g.FillPath(stateBrush, surfacePath);
                        }
                    }
                }

                // STEP 4: Draw toggle with ripple effect
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color iconColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;

                    // Ripple effect background (circular)
                    if (isHovered)
                    {
                        using (var rippleBrush = new SolidBrush(Color.FromArgb(StateLayerAlpha * 2, iconColor)))
                        {
                            g.FillEllipse(rippleBrush, toggleRect);
                        }
                    }

                    // Material chevron icon
                    using (var pen = new Pen(iconColor, 2f))
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

                // STEP 5: Draw checkbox with Material Design styling
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
                    var borderColor = isHovered ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    // Rounded checkbox
                    using (var checkPath = CreateRoundedRectangle(checkRect, 2))
                    {
                        using (var bgBrush = new SolidBrush(bgColor))
                        {
                            g.FillPath(bgBrush, checkPath);
                        }

                        using (var borderPen = new Pen(borderColor, 2f))
                        {
                            g.DrawPath(borderPen, checkPath);
                        }
                    }

                    // Checkmark with Material animation feel
                    if (node.Item.IsChecked)
                    {
                        using (var checkPen = new Pen(Color.White, 2f))
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

                // STEP 6: Draw Material icon
                if (node.IconRectContent != Rectangle.Empty && !string.IsNullOrEmpty(node.Item.ImagePath))
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    PaintIcon(g, iconRect, node.Item.ImagePath);
                }

                // STEP 7: Draw text with Material typography (Roboto-like)
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    // Material typography: bold when selected
                    using (var renderFont = new Font("Segoe UI", _owner.TextFont.Size, 
                        isSelected ? FontStyle.Bold : FontStyle.Regular))
                    {
                        TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, renderFont, textRect, textColor,
                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                    }
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
            }
        }

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
