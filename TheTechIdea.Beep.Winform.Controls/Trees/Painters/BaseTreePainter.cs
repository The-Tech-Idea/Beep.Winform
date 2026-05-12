using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Base abstract painter providing common functionality for all tree painters.
    /// </summary>
    public abstract class BaseTreePainter : ITreePainter
    {
        protected BeepTree _owner;
        protected IBeepTheme _theme;
        private BeepTreeRowConfig _currentNodeRowConfig; // Temporary storage to avoid O(n) search

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
            if (g == null || owner == null || bounds.Width <=0 || bounds.Height <=0)
                return;

            _owner = owner;

            // Multi-column: paint column headers first
            Rectangle nodeBounds = bounds;
            if (owner.IsMultiColumn && owner.ShowColumnHeaders)
            {
                int headerHeight = owner.ColumnHeaderHeight;
                var headerBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, headerHeight);
                PaintColumnHeaders(g, headerBounds, owner.Columns);
                nodeBounds = new Rectangle(bounds.X, bounds.Y + headerHeight, bounds.Width, bounds.Height - headerHeight);
            }

            // Get layout from helper
            var layoutHelper = owner.LayoutHelper;

            if (layoutHelper == null)
            {
                // Fallback: use _visibleNodes directly
                var visibleNodes = owner.VisibleNodes;
                if (visibleNodes == null || visibleNodes.Count ==0)
                    return;

                foreach (var nodeInfo in visibleNodes)
                {
                    var rowRect = nodeInfo.RowRectContent;
                    bool isSelected = nodeInfo.Item.IsSelected;
                    bool isHovered = (owner.LastHoveredItem == nodeInfo.Item);

                    PaintNode(g, nodeInfo, rowRect, isHovered, isSelected);
                }

                // Multi-column: paint grid lines after nodes
                if (owner.IsMultiColumn && owner.ShowGridLines)
                {
                    PaintGridLines(g, nodeBounds, owner.Columns, visibleNodes?.Count ?? 0, owner.GetScaledMinRowHeight());
                }
                return;
            }

            // Get cached layout
            var layout = layoutHelper.GetCachedLayout();
            
            if (layout == null || layout.Count ==0)
            {
                // Try to recalculate
                try
                {
                    layout = layoutHelper.RecalculateLayout();
                }
                catch
                {
                    return;
                }
            }

            if (layout == null || layout.Count ==0)
                return;

            // Draw each visible node
            foreach (var nodeInfo in layout)
            {
                // Check if node is in viewport (for virtualization)
                if (owner.VirtualizeLayout && !layoutHelper.IsNodeInViewport(nodeInfo))
                    continue;

                // Get node state
                bool isSelected = nodeInfo.Item.IsSelected;
                bool isHovered = (owner.LastHoveredItem == nodeInfo.Item);

                // Transform to viewport coordinates
                Rectangle rowRect = layoutHelper.TransformToViewport(nodeInfo.RowRectContent);

                // For FullRowSelect, extend the background to the full width of the tree
                if (owner.FullRowSelect && isSelected)
                {
                    var clientArea = owner.GetClientArea();
                    rowRect = new Rectangle(clientArea.X, rowRect.Y, clientArea.Width, rowRect.Height);
                }

                // Paint this node
                PaintNode(g, nodeInfo, rowRect, isHovered, isSelected);
            }

            // Multi-column: paint grid lines after nodes
            if (owner.IsMultiColumn && owner.ShowGridLines)
            {
                PaintGridLines(g, nodeBounds, owner.Columns, layout.Count, owner.GetScaledMinRowHeight());
            }
        }

        // Provide a default implementation so concrete painters are not forced to override
        public virtual void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null) return;

            // Fire CustomDrawNode event
            if (_owner?.OnCustomDrawNode(g, nodeBounds, node.Item, node, isSelected, isHovered, () =>
            {
                DefaultPaintNode(g, node, nodeBounds, isHovered, isSelected);
            }) == true)
            {
                return;
            }

            DefaultPaintNode(g, node, nodeBounds, isHovered, isSelected);
        }

        private void DefaultPaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            // Background (selection/hover)
            // Store row config temporarily to avoid O(n) search in DefaultPaintNodeBackground
            _currentNodeRowConfig = node.RowConfig;
            PaintNodeBackground(g, nodeBounds, isHovered, isSelected);
            _currentNodeRowConfig = null;

            // Multi-column mode: paint cells
            if (_owner?.IsMultiColumn == true)
            {
                PaintNodeCells(g, node, isHovered, isSelected);
                return;
            }

            // Single-column mode: traditional tree rendering
            // Toggle (only if has children)
            bool hasChildren = node.Item?.Children != null && node.Item.Children.Count > 0;
            if (hasChildren && !node.ToggleRectContent.IsEmpty && _owner?.LayoutHelper != null)
            {
                // Transform to viewport coordinates before drawing
                var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                PaintToggle(g, toggleRect, node.Item.IsExpanded, hasChildren, isHovered);
            }

            // Checkbox (if enabled on owner)
            if (_owner != null && _owner.ShowCheckBox && !node.CheckRectContent.IsEmpty && _owner.LayoutHelper != null)
            {
                var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                PaintCheckbox(g, checkRect, node.Item.IsChecked, node.Item.IsIndeterminate, isHovered);
            }

            // Icon
            if (!string.IsNullOrEmpty(node.Item?.ImagePath) && !node.IconRectContent.IsEmpty && _owner?.LayoutHelper != null)
            {
                var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                PaintIcon(g, iconRect, node.Item.ImagePath);
            }

            // Text
            if (!node.TextRectContent.IsEmpty && _owner?.LayoutHelper != null)
            {
                var font = _owner?.UseThemeFont == true
                    ? (BeepThemesManager.ToFontForControl(_owner?._currentTheme?.LabelFont, _owner) ?? _owner?.TextFont)
                    : _owner?.TextFont;
                font ??= SystemFonts.DefaultFont;
                var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                PaintText(g, textRect, node.Item?.Text ?? string.Empty, font, isSelected, isHovered);
            }
        }

        /// <summary>
        /// Paints a node in multi-column mode, rendering each cell in its column.
        /// </summary>
        protected virtual void PaintNodeCells(Graphics g, NodeInfo node, bool isHovered, bool isSelected)
        {
            if (_owner?.LayoutHelper == null || _owner.Columns == null)
                return;

            var font = _owner?.UseThemeFont == true
                ? (BeepThemesManager.ToFontForControl(_owner?._currentTheme?.LabelFont, _owner) ?? _owner?.TextFont)
                : _owner?.TextFont;
            font ??= SystemFonts.DefaultFont;

            int colIndex = 0;
            foreach (var column in _owner.Columns.GetVisibleColumns())
            {
                var cellRectContent = node.GetCellRect(colIndex);
                if (cellRectContent.IsEmpty)
                {
                    colIndex++;
                    continue;
                }

                var cellRect = _owner.LayoutHelper.TransformToViewport(cellRectContent);

                // First column: tree structure (toggle, checkbox, icon, text)
                if (colIndex == 0)
                {
                    PaintFirstColumnCell(g, node, cellRect, isHovered, isSelected, font);
                }
                else
                {
                    // Other columns: just the cell value
                    string cellText = GetCellText(node.Item, column);
                    PaintCell(g, cellRect, cellText, font, column, isSelected, isHovered);
                }

                colIndex++;
            }
        }

        /// <summary>
        /// Paints the first column cell which contains the tree structure.
        /// </summary>
        protected virtual void PaintFirstColumnCell(Graphics g, NodeInfo node, Rectangle cellRect, bool isHovered, bool isSelected, Font font)
        {
            // Toggle (only if has children)
            bool hasChildren = node.Item?.Children != null && node.Item.Children.Count > 0;
            if (hasChildren && !node.ToggleRectContent.IsEmpty && _owner?.LayoutHelper != null)
            {
                var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                // Ensure toggle is within cell bounds
                if (cellRect.Contains(toggleRect) || cellRect.IntersectsWith(toggleRect))
                    PaintToggle(g, toggleRect, node.Item.IsExpanded, hasChildren, isHovered);
            }

            // Checkbox (if enabled on owner)
            if (_owner != null && _owner.ShowCheckBox && !node.CheckRectContent.IsEmpty && _owner.LayoutHelper != null)
            {
                var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                if (cellRect.Contains(checkRect) || cellRect.IntersectsWith(checkRect))
                    PaintCheckbox(g, checkRect, node.Item.IsChecked, node.Item.IsIndeterminate, isHovered);
            }

            // Icon
            if (!string.IsNullOrEmpty(node.Item?.ImagePath) && !node.IconRectContent.IsEmpty && _owner?.LayoutHelper != null)
            {
                var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                if (cellRect.Contains(iconRect) || cellRect.IntersectsWith(iconRect))
                    PaintIcon(g, iconRect, node.Item.ImagePath);
            }

            // Text - use the cell rect for text bounds, but offset for tree elements
            if (!node.TextRectContent.IsEmpty && _owner?.LayoutHelper != null)
            {
                var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                // Clip text to cell bounds
                textRect.Intersect(cellRect);
                if (textRect.Width > 0 && textRect.Height > 0)
                    PaintText(g, textRect, node.Item?.Text ?? string.Empty, font, isSelected, isHovered);
            }
        }

        /// <summary>
        /// Gets the display text for a cell based on the column's field binding.
        /// </summary>
        protected virtual string GetCellText(SimpleItem item, BeepTreeColumn column)
        {
            if (item == null || column == null)
                return string.Empty;

            if (string.IsNullOrEmpty(column.FieldName))
                return item.Text ?? string.Empty;

            // Try to get value from item.Data dictionary
            if (item.Data != null && item.Data.ContainsKey(column.FieldName))
            {
                var value = item.Data[column.FieldName];
                if (value != null)
                {
                    if (!string.IsNullOrEmpty(column.FormatString))
                        return string.Format("{0:" + column.FormatString + "}", value);
                    return value.ToString();
                }
            }

            // Fallback
            return item.Text ?? string.Empty;
        }

        public virtual void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <=0 || toggleRect.Height <=0)
                return;

            // Default implementation: draw simple +/- or >/v
            var color = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
            var pen = PaintersFactory.GetPen(color,2f);

            var center = new Point(toggleRect.X + toggleRect.Width /2, toggleRect.Y + toggleRect.Height /2);
            var size = Math.Min(toggleRect.Width, toggleRect.Height) /3;

            if (isExpanded)
            {
                // Draw down chevron
                g.DrawLine(pen, center.X - size, center.Y - size /2, center.X, center.Y + size /2);
                g.DrawLine(pen, center.X, center.Y + size /2, center.X + size, center.Y - size /2);
            }
            else
            {
                // Draw right chevron
                g.DrawLine(pen, center.X - size /2, center.Y - size, center.X + size /2, center.Y);
                g.DrawLine(pen, center.X + size /2, center.Y, center.X - size /2, center.Y + size);
            }
        }

        public virtual void PaintCheckbox(Graphics g, Rectangle checkRect, bool isChecked, bool isIndeterminate, bool isHovered)
        {
            if (checkRect.Width <=0 || checkRect.Height <=0)
                return;

            // Default implementation: simple checkbox with three-state support
            var borderColor = isHovered ? _theme.AccentColor : _theme.BorderColor;
            var bgColor = isChecked || isIndeterminate ? _theme.AccentColor : _theme.TreeBackColor;

            var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
            var borderPen = PaintersFactory.GetPen(borderColor,1f);

            g.FillRectangle(bgBrush, checkRect);
            g.DrawRectangle(borderPen, checkRect);

            if (isChecked)
            {
                // Draw checkmark
                var checkPen = PaintersFactory.GetPen(Color.White,2f);
                var points = new Point[]
                {
                    new Point(checkRect.X + checkRect.Width /4, checkRect.Y + checkRect.Height /2),
                    new Point(checkRect.X + checkRect.Width /2, checkRect.Y + checkRect.Height *3 /4),
                    new Point(checkRect.X + checkRect.Width *3 /4, checkRect.Y + checkRect.Height /4)
                };
                g.DrawLines(checkPen, points);
            }
            else if (isIndeterminate)
            {
                // Draw indeterminate dash
                var dashPen = PaintersFactory.GetPen(Color.White, 2f);
                int dashY = checkRect.Y + checkRect.Height / 2;
                int dashPadding = checkRect.Width / 4;
                g.DrawLine(dashPen, checkRect.X + dashPadding, dashY, checkRect.Right - dashPadding, dashY);
            }
        }

        public virtual void PaintIcon(Graphics g, Rectangle iconRect, string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || iconRect.Width <=0 || iconRect.Height <=0)
                return;

            // Check if async loading is enabled and this is a URL
            if (_owner?.EnableAsyncImageLoading == true &&
                (imagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                 imagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            {
                var loader = _owner.AsyncImageLoader;
                if (loader != null)
                {
                    var image = loader.GetImage(imagePath, out bool isLoading);
                    if (image != null)
                    {
                        // Draw the loaded image
                        g.DrawImage(image, iconRect);
                        return;
                    }
                    else if (isLoading)
                    {
                        // Draw loading placeholder (light gray background with dots)
                        var placeholderBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(240, 240, 240));
                        g.FillRectangle(placeholderBrush, iconRect);
                        var dotBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(180, 180, 180));
                        int dotSize = 3;
                        int dotSpacing = 5;
                        int totalWidth = 3 * dotSize + 2 * dotSpacing;
                        int startX = iconRect.X + (iconRect.Width - totalWidth) / 2;
                        int centerY = iconRect.Y + iconRect.Height / 2;
                        for (int i = 0; i < 3; i++)
                        {
                            int dotX = startX + i * (dotSize + dotSpacing);
                            g.FillEllipse(dotBrush, dotX, centerY - dotSize / 2, dotSize, dotSize);
                        }
                        return;
                    }
                }
            }

            // Use StyledImagePainter for consistent image rendering with caching (local paths)
            StyledImagePainter.Paint(g, iconRect, imagePath);
        }

        #region Theme Override Helpers

        /// <summary>
        /// Gets the effective background color for selected nodes, respecting theme overrides.
        /// </summary>
        protected Color GetSelectedBackColor()
        {
            if (_owner?.SelectedNodeBackColor != Color.Empty)
                return _owner.SelectedNodeBackColor;
            return _theme?.TreeNodeSelectedBackColor ?? Color.LightBlue;
        }

        /// <summary>
        /// Gets the effective foreground color for selected nodes, respecting theme overrides.
        /// </summary>
        protected Color GetSelectedForeColor()
        {
            if (_owner?.SelectedNodeForeColor != Color.Empty)
                return _owner.SelectedNodeForeColor;
            return _theme?.TreeNodeSelectedForeColor ?? Color.White;
        }

        /// <summary>
        /// Gets the effective background color for hovered nodes, respecting theme overrides.
        /// </summary>
        protected Color GetHoverBackColor()
        {
            if (_owner?.HoverNodeBackColor != Color.Empty)
                return _owner.HoverNodeBackColor;
            return _theme?.TreeNodeHoverBackColor ?? Color.LightGray;
        }

        /// <summary>
        /// Gets the effective foreground color for hovered nodes, respecting theme overrides.
        /// </summary>
        protected Color GetHoverForeColor()
        {
            if (_owner?.HoverNodeForeColor != Color.Empty)
                return _owner.HoverNodeForeColor;
            return _theme?.TreeNodeHoverForeColor ?? Color.Black;
        }

        /// <summary>
        /// Gets the effective color for column header text, respecting theme overrides.
        /// </summary>
        protected Color GetColumnHeaderForeColor()
        {
            if (_owner?.ColumnHeaderForeColor != Color.Empty)
                return _owner.ColumnHeaderForeColor;
            return _theme?.TreeForeColor ?? Color.Black;
        }

        /// <summary>
        /// Gets the effective background color for column headers, respecting theme overrides.
        /// </summary>
        protected Color GetColumnHeaderBackColor()
        {
            if (_owner?.ColumnHeaderBackColor != Color.Empty)
                return _owner.ColumnHeaderBackColor;
            return _theme?.TreeBackColor ?? Color.LightGray;
        }

        /// <summary>
        /// Gets the effective color for grid lines, respecting theme overrides.
        /// </summary>
        protected Color GetGridLineColor()
        {
            if (_owner?.GridLineColor != Color.Empty)
                return _owner.GridLineColor;
            return Color.FromArgb(40, _theme?.BorderColor ?? Color.Gray);
        }

        /// <summary>
        /// Gets the effective color for sort indicators, respecting theme overrides.
        /// </summary>
        protected Color GetSortIndicatorColor()
        {
            if (_owner?.SortIndicatorColor != Color.Empty)
                return _owner.SortIndicatorColor;
            return _theme?.AccentColor ?? Color.Blue;
        }

        /// <summary>
        /// Gets the effective color for filter indicators, respecting theme overrides.
        /// </summary>
        protected Color GetFilterIndicatorColor(bool isFiltered)
        {
            if (isFiltered)
            {
                if (_owner?.FilterIndicatorColor != Color.Empty)
                    return _owner.FilterIndicatorColor;
                return _theme?.AccentColor ?? Color.Blue;
            }
            return _theme?.BorderColor ?? Color.Gray;
        }

        /// <summary>
        /// Gets the effective focus indicator color, respecting theme overrides.
        /// </summary>
        protected Color GetFocusIndicatorColor()
        {
            if (_owner?.FocusIndicatorColor != Color.Empty)
                return _owner.FocusIndicatorColor;
            return _theme?.AccentColor ?? Color.Blue;
        }

        #endregion

        public virtual void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            PaintText(g, textRect, text, font, isSelected, isHovered, null);
        }

        public virtual void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered, BeepTreeRowConfig rowConfig)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0)
                return;

            // Determine text color
            Color textColor = Color.Empty;
            if (rowConfig != null && rowConfig.UseCustomStyle)
            {
                if (isSelected && rowConfig.SelectedForeColor != Color.Empty)
                    textColor = rowConfig.SelectedForeColor;
                else if (isHovered && rowConfig.HoverForeColor != Color.Empty)
                    textColor = rowConfig.HoverForeColor;
                else if (rowConfig.ForeColor != Color.Empty)
                    textColor = rowConfig.ForeColor;
            }

            if (textColor == Color.Empty)
            {
                textColor = isSelected ? GetSelectedForeColor() : (isHovered ? GetHoverForeColor() : _theme.TreeForeColor);
            }

            // Determine font
            Font drawFont = font;
            if (rowConfig?.Font != null)
            {
                drawFont = rowConfig.Font;
            }
            else if (rowConfig?.FontStyle != null && font != null)
            {
                drawFont = new Font(font, rowConfig.FontStyle.Value);
            }

            // Respect TextAlignment property
            TextFormatFlags alignmentFlags = TextFormatFlags.Left;
            if (_owner != null)
            {
                switch (_owner.TextAlignment)
                {
                    case TextAlignment.Right:
                        alignmentFlags = TextFormatFlags.Right;
                        break;
                    case TextAlignment.Left:
                        alignmentFlags = TextFormatFlags.Left;
                        break;
                    default:
                        alignmentFlags = TextFormatFlags.Left;
                        break;
                }
            }

            TextRenderer.DrawText(g, text, drawFont, textRect, textColor,
                alignmentFlags | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        public virtual void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0)
                return;

            // Fire CustomDrawNodeBackground event
            if (_owner?.OnCustomDrawNodeBackground(g, nodeBounds, _owner?.SelectedNode, isSelected, isHovered, () =>
            {
                DefaultPaintNodeBackground(g, nodeBounds, isHovered, isSelected);
            }) == true)
            {
                return;
            }

            DefaultPaintNodeBackground(g, nodeBounds, isHovered, isSelected);
        }

        private void DefaultPaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0)
                return;

            // Use the pre-set row config to avoid O(n) search through visible nodes
            var rowConfig = _currentNodeRowConfig;

            Color backColor = Color.Empty;

            if (rowConfig != null && rowConfig.UseCustomStyle)
            {
                if (isSelected && rowConfig.SelectedBackColor != Color.Empty)
                    backColor = rowConfig.SelectedBackColor;
                else if (isHovered && rowConfig.HoverBackColor != Color.Empty)
                    backColor = rowConfig.HoverBackColor;
                else if (rowConfig.BackColor != Color.Empty)
                    backColor = rowConfig.BackColor;

                // Handle gradient
                if (rowConfig.GradientStartColor != Color.Empty && rowConfig.GradientEndColor != Color.Empty)
                {
                    using (var gradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        nodeBounds, rowConfig.GradientStartColor, rowConfig.GradientEndColor, rowConfig.GradientDirection))
                    {
                        g.FillRectangle(gradientBrush, nodeBounds);
                        return;
                    }
                }
            }

            if (backColor == Color.Empty)
            {
                if (isSelected)
                    backColor = GetSelectedBackColor();
                else if (isHovered)
                    backColor = GetHoverBackColor();
            }

            if (backColor != Color.Empty)
            {
                var brush = PaintersFactory.GetSolidBrush(backColor);
                g.FillRectangle(brush, nodeBounds);
            }
        }

        public virtual int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // Default: measure text height + padding
            var textSize = TextRenderer.MeasureText(item.Text ?? string.Empty, font,
                new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            return Math.Max(textSize.Height +8,24); // Minimum24px
        }

        public virtual void PaintColumnHeaders(Graphics g, Rectangle headersBounds, BeepTreeColumnCollection columns)
        {
            if (g == null || columns == null || headersBounds.Width <= 0 || headersBounds.Height <= 0)
                return;

            // Default header background
            var headerBrush = PaintersFactory.GetSolidBrush(GetColumnHeaderBackColor());
            g.FillRectangle(headerBrush, headersBounds);

            // Header border bottom
            var borderPen = PaintersFactory.GetPen(_theme?.BorderColor ?? Color.Gray, 1f);
            g.DrawLine(borderPen, headersBounds.Left, headersBounds.Bottom - 1, headersBounds.Right, headersBounds.Bottom - 1);

            // Draw each column header
            int x = headersBounds.Left;
            int colIndex = 0;
            foreach (var column in columns.GetVisibleColumns())
            {
                var colRect = new Rectangle(x, headersBounds.Top, column.Width, headersBounds.Height);

                // Fire CustomDrawColumnHeader event
                bool handled = false;
                if (_owner != null)
                {
                    int currentColIndex = colIndex;
                    handled = _owner.OnCustomDrawColumnHeader(g, colRect, column, currentColIndex, () =>
                    {
                        DefaultPaintColumnHeader(g, colRect, column, currentColIndex, headersBounds, borderPen);
                    });
                }

                if (!handled)
                {
                    DefaultPaintColumnHeader(g, colRect, column, colIndex, headersBounds, borderPen);
                }

                x += column.Width;
                colIndex++;
            }
        }

        private void DefaultPaintColumnHeader(Graphics g, Rectangle colRect, BeepTreeColumn column, int colIndex, Rectangle headersBounds, Pen borderPen)
        {
            // Header text
            var headerFont = _owner?.UseThemeFont == true
                ? (BeepThemesManager.ToFontForControl(_owner?._currentTheme?.LabelFont, _owner) ?? _owner?.TextFont)
                : _owner?.TextFont;
            headerFont ??= SystemFonts.DefaultFont;

            // Calculate text rect (leave room for filter button)
            int filterButtonWidth = column.Filterable ? 16 : 0;
            var textRect = new Rectangle(colRect.X + 4, colRect.Y, colRect.Width - filterButtonWidth - 8, colRect.Height);

            var textColor = GetColumnHeaderForeColor();
            TextRenderer.DrawText(g, column.DisplayText, headerFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            // Draw sort indicator if column is sorted
            if (column.SortDirection != TheTechIdea.Beep.Winform.Controls.Trees.Models.BeepTreeSortDirection.None)
            {
                int sortIndicatorWidth = 12;
                int sortIndicatorX = column.Filterable ? colRect.Right - 32 : colRect.Right - 16;
                var sortRect = new Rectangle(sortIndicatorX, colRect.Top + (colRect.Height - 12) / 2, sortIndicatorWidth, 12);
                DrawSortIndicator(g, sortRect, column.SortDirection);
            }

            // Draw filter button if filterable
            if (column.Filterable)
            {
                var filterRect = new Rectangle(colRect.Right - 18, colRect.Top + (colRect.Height - 16) / 2, 16, 16);
                DrawFilterButton(g, filterRect, column.IsFiltered);
            }

            // Column separator
            if (_owner?.Columns != null && colIndex < _owner.Columns.Count - 1)
            {
                g.DrawLine(borderPen, colRect.Right - 1, colRect.Top + 4, colRect.Right - 1, colRect.Bottom - 4);
            }
        }

        /// <summary>
        /// Draws a filter button in the column header.
        /// </summary>
        protected virtual void DrawFilterButton(Graphics g, Rectangle rect, bool isFiltered)
        {
            if (g == null || rect.Width <= 0 || rect.Height <= 0)
                return;

            var color = GetFilterIndicatorColor(isFiltered);
            using (var brush = new SolidBrush(color))
            {
                // Draw a simple funnel/filter icon
                var points = new Point[]
                {
                    new Point(rect.Left + 2, rect.Top + 3),
                    new Point(rect.Right - 2, rect.Top + 3),
                    new Point(rect.Left + rect.Width / 2, rect.Bottom - 3)
                };
                g.FillPolygon(brush, points);
            }
        }

        /// <summary>
        /// Draws a sort indicator (up/down arrow) in the column header.
        /// </summary>
        protected virtual void DrawSortIndicator(Graphics g, Rectangle rect, TheTechIdea.Beep.Winform.Controls.Trees.Models.BeepTreeSortDirection sortDirection)
        {
            if (g == null || rect.Width <= 0 || rect.Height <= 0)
                return;

            var color = GetSortIndicatorColor();
            var pen = PaintersFactory.GetPen(color, 1.5f);

            int centerX = rect.Left + rect.Width / 2;
            int centerY = rect.Top + rect.Height / 2;
            int size = Math.Min(rect.Width, rect.Height) / 3;

            if (sortDirection == TheTechIdea.Beep.Winform.Controls.Trees.Models.BeepTreeSortDirection.Ascending)
            {
                // Up arrow
                g.DrawLine(pen, centerX, centerY - size, centerX - size, centerY + size / 2);
                g.DrawLine(pen, centerX, centerY - size, centerX + size, centerY + size / 2);
            }
            else if (sortDirection == TheTechIdea.Beep.Winform.Controls.Trees.Models.BeepTreeSortDirection.Descending)
            {
                // Down arrow
                g.DrawLine(pen, centerX - size, centerY - size / 2, centerX, centerY + size);
                g.DrawLine(pen, centerX + size, centerY - size / 2, centerX, centerY + size);
            }
        }

        public virtual void PaintCell(Graphics g, Rectangle cellRect, string text, Font font, BeepTreeColumn column, bool isSelected, bool isHovered)
        {
            if (g == null || cellRect.Width <= 0 || cellRect.Height <= 0)
                return;

            // Fire CustomDrawCell event
            if (_owner?.OnCustomDrawCell(g, cellRect, _owner?.SelectedNode, column, _owner?.Columns?.IndexOf(column) ?? 0, text, isSelected, () =>
            {
                DefaultPaintCell(g, cellRect, text, font, column, isSelected);
            }) == true)
            {
                return;
            }

            DefaultPaintCell(g, cellRect, text, font, column, isSelected);
        }

        private void DefaultPaintCell(Graphics g, Rectangle cellRect, string text, Font font, BeepTreeColumn column, bool isSelected)
        {
            if (g == null || cellRect.Width <= 0 || cellRect.Height <= 0)
                return;

            var textColor = isSelected ? GetSelectedForeColor() : _theme?.TreeForeColor ?? Color.Black;

            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis;
            switch (column?.Alignment)
            {
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopRight:
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Right;
                    break;
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.TopCenter:
                case ContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;
                default:
                    flags |= TextFormatFlags.Left;
                    break;
            }

            TextRenderer.DrawText(g, text ?? string.Empty, font, cellRect, textColor, flags);
        }

        public virtual void PaintGridLines(Graphics g, Rectangle bounds, BeepTreeColumnCollection columns, int rowCount, int rowHeight)
        {
            if (g == null || columns == null || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            var gridPen = PaintersFactory.GetPen(GetGridLineColor(), 1f);

            // Vertical lines (column separators)
            int x = bounds.Left;
            foreach (var column in columns.GetVisibleColumns())
            {
                x += column.Width;
                g.DrawLine(gridPen, x - 1, bounds.Top, x - 1, bounds.Bottom);
            }

            // Horizontal lines (row separators)
            for (int i = 0; i <= rowCount; i++)
            {
                int y = bounds.Top + i * rowHeight;
                g.DrawLine(gridPen, bounds.Left, y, bounds.Right, y);
            }
        }

        /// <summary>
        /// Helper to get scaled value using the owner control's DPI scale factor.
        /// </summary>
        protected int Scale(int value)
        {
            return _owner != null ? DpiScalingHelper.ScaleValue(value, _owner) : value;
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

        #region Safe Drawing Helpers (prevent GDI corruption of cached pens)

        /// <summary>
        /// Draws a chevron toggle button using a cloned pen to avoid corrupting the shared cache.
        /// </summary>
        protected void DrawChevron(Graphics g, Rectangle rect, Color color, float width, bool isExpanded)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            var basePen = PaintersFactory.GetPen(color, width);
            using (var pen = (Pen)basePen.Clone())
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = rect.Left + rect.Width / 2;
                int centerY = rect.Top + rect.Height / 2;
                int size = Math.Min(rect.Width, rect.Height) / 3;

                if (isExpanded)
                {
                    // Down chevron
                    g.DrawLine(pen, centerX - size, centerY - size / 2, centerX, centerY + size / 2);
                    g.DrawLine(pen, centerX, centerY + size / 2, centerX + size, centerY - size / 2);
                }
                else
                {
                    // Right chevron
                    g.DrawLine(pen, centerX - size / 2, centerY - size, centerX + size / 2, centerY);
                    g.DrawLine(pen, centerX + size / 2, centerY, centerX - size / 2, centerY + size);
                }
            }
        }

        /// <summary>
        /// Draws a plus/minus toggle button using a cloned pen.
        /// </summary>
        protected void DrawPlusMinus(Graphics g, Rectangle rect, Color color, float width, bool isExpanded)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            var basePen = PaintersFactory.GetPen(color, width);
            using (var pen = (Pen)basePen.Clone())
            {
                int centerX = rect.Left + rect.Width / 2;
                int centerY = rect.Top + rect.Height / 2;
                int halfSize = Math.Min(rect.Width, rect.Height) / 3;

                // Horizontal line (always drawn)
                g.DrawLine(pen, centerX - halfSize, centerY, centerX + halfSize, centerY);

                // Vertical line (only when collapsed = plus sign)
                if (!isExpanded)
                {
                    g.DrawLine(pen, centerX, centerY - halfSize, centerX, centerY + halfSize);
                }
            }
        }

        /// <summary>
        /// Draws a checkmark using a cloned pen.
        /// </summary>
        protected void DrawCheckmark(Graphics g, Rectangle rect, Color color, float width)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            var basePen = PaintersFactory.GetPen(color, width);
            using (var pen = (Pen)basePen.Clone())
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                var points = new Point[]
                {
                    new Point(rect.X + rect.Width / 4, rect.Y + rect.Height / 2),
                    new Point(rect.X + rect.Width / 2 - 1, rect.Y + rect.Height * 3 / 4),
                    new Point(rect.X + rect.Width * 3 / 4, rect.Y + rect.Height / 4)
                };
                g.DrawLines(pen, points);
            }
        }

        #endregion
    }
}
