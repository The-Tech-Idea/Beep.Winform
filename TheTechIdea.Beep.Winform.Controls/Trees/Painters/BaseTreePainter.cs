using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Base abstract painter providing common functionality for all tree painters.
    /// </summary>
    public abstract class BaseTreePainter : ITreePainter
    {
        protected BeepTree _owner;
        protected IBeepTheme _theme;

        public virtual void Initialize(BeepTree owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));
        }

        /// <summary>
        /// Paint the entire tree - iterates through all visible nodes and calls PaintNode for each.
        /// Override this to customize tree-wide rendering (backgrounds, grid lines, etc.)
        /// </summary>
        public virtual void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] START - bounds: {bounds}");
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0)
            {
                System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] NULL/INVALID CHECK FAILED");
                return;
            }

            _owner = owner;

            // Get layout from helper
            var layoutHelper = owner.LayoutHelper;
            System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] LayoutHelper: {layoutHelper != null}, VisibleNodes: {owner.VisibleNodes?.Count ?? 0}");
            
            if (layoutHelper == null)
            {
                System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] FALLBACK: Using VisibleNodes directly");
                // Fallback: use _visibleNodes directly
                var visibleNodes = owner.VisibleNodes;
                if (visibleNodes == null || visibleNodes.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] FALLBACK FAILED: No visible nodes");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] FALLBACK: Painting {visibleNodes.Count} nodes");
                foreach (var nodeInfo in visibleNodes)
                {
                    var rowRect = nodeInfo.RowRectContent;
                    System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] Node: {nodeInfo.Item?.Text}, RowRect: {rowRect}");
                    bool isSelected = nodeInfo.Item.IsSelected;
                    bool isHovered = (owner.LastHoveredItem == nodeInfo.Item);

                    PaintNode(g, nodeInfo, rowRect, isHovered, isSelected);
                }
                System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] FALLBACK: Completed");
                return;
            }

            // Get cached layout
            var layout = layoutHelper.GetCachedLayout();
            System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] Cached layout count: {layout?.Count ?? 0}");
            
            if (layout == null || layout.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] Recalculating layout...");
                // Try to recalculate
                try
                {
                    layout = layoutHelper.RecalculateLayout();
                    System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] Recalculated layout count: {layout?.Count ?? 0}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] Recalculation FAILED: {ex.Message}");
                    return;
                }
            }

            if (layout == null || layout.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] No layout available, EXITING");
                return;
            }

            // Draw each visible node
            int drawnCount = 0;
            foreach (var nodeInfo in layout)
            {
                // Check if node is in viewport (for virtualization)
                if (owner.VirtualizeLayout && !layoutHelper.IsNodeInViewport(nodeInfo))
                    continue;

                // Transform to viewport coordinates
                Rectangle rowRect = layoutHelper.TransformToViewport(nodeInfo.RowRectContent);

                // Get node state
                bool isSelected = nodeInfo.Item.IsSelected;
                bool isHovered = (owner.LastHoveredItem == nodeInfo.Item);

                // Paint this node
                PaintNode(g, nodeInfo, rowRect, isHovered, isSelected);
                drawnCount++;
            }
            System.Diagnostics.Debug.WriteLine($"[BaseTreePainter.Paint] Drew {drawnCount} nodes, COMPLETE");
        }

        // Provide a default implementation so concrete painters are not forced to override
        public virtual void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null) return;

            // Background (selection/hover)
            PaintNodeBackground(g, nodeBounds, isHovered, isSelected);

            // Toggle (only if has children)
            bool hasChildren = node.Item?.Children != null && node.Item.Children.Count > 0;
            if (hasChildren && !node.ToggleRectContent.IsEmpty)
            {
                PaintToggle(g, node.ToggleRectContent, node.Item.IsExpanded, hasChildren, isHovered);
            }

            // Checkbox (if enabled on owner)
            if (_owner != null && _owner.ShowCheckBox && !node.CheckRectContent.IsEmpty)
            {
                PaintCheckbox(g, node.CheckRectContent, node.Item.IsChecked, isHovered);
            }

            // Icon
            if (!string.IsNullOrEmpty(node.Item?.ImagePath) && !node.IconRectContent.IsEmpty)
            {
                PaintIcon(g, node.IconRectContent, node.Item.ImagePath);
            }

            // Text
            if (!node.TextRectContent.IsEmpty)
            {
                var font = _owner?.UseThemeFont == true ? (BeepThemesManager.ToFont(_owner?._currentTheme?.LabelFont) ?? _owner.TextFont) : _owner?.TextFont;
                font ??= SystemFonts.DefaultFont;
                PaintText(g, node.TextRectContent, node.Item?.Text ?? string.Empty, font, isSelected, isHovered);
            }
        }

        public virtual void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0)
                return;

            // Default implementation: draw simple +/- or >/v
            var color = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
            using (var pen = new Pen(color, 2f))
            {
                var center = new Point(toggleRect.X + toggleRect.Width / 2, toggleRect.Y + toggleRect.Height / 2);
                var size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                if (isExpanded)
                {
                    // Draw down chevron
                    g.DrawLine(pen, center.X - size, center.Y - size / 2, center.X, center.Y + size / 2);
                    g.DrawLine(pen, center.X, center.Y + size / 2, center.X + size, center.Y - size / 2);
                }
                else
                {
                    // Draw right chevron
                    g.DrawLine(pen, center.X - size / 2, center.Y - size, center.X + size / 2, center.Y);
                    g.DrawLine(pen, center.X + size / 2, center.Y, center.X - size / 2, center.Y + size);
                }
            }
        }

        public virtual void PaintCheckbox(Graphics g, Rectangle checkRect, bool isChecked, bool isHovered)
        {
            if (checkRect.Width <= 0 || checkRect.Height <= 0)
                return;

            // Default implementation: simple checkbox
            var borderColor = isHovered ? _theme.AccentColor : _theme.BorderColor;
            var bgColor = isChecked ? _theme.AccentColor : _theme.TreeBackColor;

            using (var bgBrush = new SolidBrush(bgColor))
            using (var borderPen = new Pen(borderColor, 1f))
            {
                g.FillRectangle(bgBrush, checkRect);
                g.DrawRectangle(borderPen, checkRect);

                if (isChecked)
                {
                    // Draw checkmark
                    using (var checkPen = new Pen(Color.White, 2f))
                    {
                        var points = new Point[]
                        {
                            new Point(checkRect.X + checkRect.Width / 4, checkRect.Y + checkRect.Height / 2),
                            new Point(checkRect.X + checkRect.Width / 2, checkRect.Y + checkRect.Height * 3 / 4),
                            new Point(checkRect.X + checkRect.Width * 3 / 4, checkRect.Y + checkRect.Height / 4)
                        };
                        g.DrawLines(checkPen, points);
                    }
                }
            }
        }

        public virtual void PaintIcon(Graphics g, Rectangle iconRect, string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || iconRect.Width <= 0 || iconRect.Height <= 0)
                return;

            // Use StyledImagePainter for consistent image rendering with caching
            StyledImagePainter.Paint(g, iconRect, imagePath);
        }

        public virtual void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0)
                return;

            var textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;
            TextRenderer.DrawText(g, text, font, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        public virtual void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0)
                return;

            if (isSelected)
            {
                using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                {
                    g.FillRectangle(brush, nodeBounds);
                }
            }
            else if (isHovered)
            {
                using (var brush = new SolidBrush(_theme.TreeNodeHoverBackColor))
                {
                    g.FillRectangle(brush, nodeBounds);
                }
            }
        }

        public virtual int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // Default: measure text height + padding
            var textSize = TextRenderer.MeasureText(item.Text ?? "", font,
                new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            return Math.Max(textSize.Height + 8, 24); // Minimum 24px
        }

        /// <summary>
        /// Helper to get scaled value based on DPI.
        /// </summary>
        protected int Scale(int value)
        {
            // Use owner's scaling helper to avoid accessing protected DpiScaleFactor directly
            return _owner != null ? _owner.ScaleValue(value) : value;
        }

        /// <summary>
        /// Helper to create a lighter version of a color.
        /// </summary>
        protected Color Lighten(Color color, float factor)
        {
            return Color.FromArgb(color.A,
                Math.Min(255, (int)(color.R + (255 - color.R) * factor)),
                Math.Min(255, (int)(color.G + (255 - color.G) * factor)),
                Math.Min(255, (int)(color.B + (255 - color.B) * factor)));
        }

        /// <summary>
        /// Helper to create a darker version of a color.
        /// </summary>
        protected Color Darken(Color color, float factor)
        {
            return Color.FromArgb(color.A,
                (int)(color.R * (1 - factor)),
                (int)(color.G * (1 - factor)),
                (int)(color.B * (1 - factor)));
        }
    }
}
