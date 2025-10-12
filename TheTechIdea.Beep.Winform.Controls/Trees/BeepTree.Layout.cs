using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepTree - Layout partial class.
    /// Handles visible node rebuilding, layout calculations, and caching.
    /// </summary>
    public partial class BeepTree
    {
        #region Rebuild Visible Nodes

        /// <summary>
        /// Rebuilds the list of visible nodes by recursively traversing the tree.
        /// Only includes nodes that are actually visible (parent is expanded).
        /// </summary>
        private void RebuildVisible()
        {
            System.Diagnostics.Debug.WriteLine($"BeepTree.RebuildVisible: Starting with {_nodes.Count} root nodes");
            _visibleNodes.Clear();

            void Recurse(SimpleItem item, int level, SimpleItem parent = null)
            {
                // Ensure ParentItem linkage is correct for helpers relying on it
                if (item != null && parent != null && item.ParentItem != parent)
                {
                    item.ParentItem = parent;
                }
                _visibleNodes.Add(new NodeInfo { Item = item, Level = level });
                if (item.IsExpanded && item.Children?.Count > 0)
                {
                    foreach (var child in item.Children)
                    {
                        Recurse(child, level + 1, item);
                    }
                }
            }

            foreach (var root in _nodes)
            {
                Recurse(root, 0, null);
            }
            
            System.Diagnostics.Debug.WriteLine($"BeepTree.RebuildVisible: Created {_visibleNodes.Count} visible nodes");

            // Recalculate layout after rebuilding
            RecalculateLayoutCache();

            // Keep the layout helper's cache in sync so DrawVisibleNodes() has data
            try
            {
                _layoutHelper?.InvalidateCache();
                var cached = _layoutHelper?.RecalculateLayout();
                if (cached != null)
                {
                    // Recompute virtual size from helper cache for accurate scrollbars
                    _virtualSize = new Size(
                        _layoutHelper.CalculateTotalContentWidth(),
                        _layoutHelper.CalculateTotalContentHeight());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BeepTree.RebuildVisible: LayoutHelper sync failed: {ex.Message}");
            }

            // Update scrollbars after layout changes
            if (!DesignMode && IsHandleCreated)
            {
                UpdateScrollBars();
                // Update hit areas because layout changed
                try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
            }
        }

        #endregion

        #region Layout Cache Calculation

        /// <summary>
        /// Recalculates the layout cache for all visible nodes.
        /// Measures text sizes and determines rectangles for all node elements.
        /// </summary>
        internal void RecalculateLayoutCache()
        {
            System.Diagnostics.Debug.WriteLine($"BeepTree.RecalculateLayoutCache: Processing {_visibleNodes.Count} nodes");
            if (_visibleNodes.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("BeepTree.RecalculateLayoutCache: No visible nodes to process");
                return;
            }

            int y = 0;
            int maxWidth = 0;
            // Use index-based loop because NodeInfo is a struct (value type).
            // Modifying a foreach iteration variable for a struct is illegal and would not persist.
            for (int i = 0; i < _visibleNodes.Count; i++)
            {
                var nodeInfo = _visibleNodes[i];
                // Use painter to get preferred row height
                var painter = GetCurrentPainter();
                Font font = UseThemeFont && _currentTheme != null
                    ? ThemeManagement.BeepThemesManager.ToFont(_currentTheme.LabelFont)
                    : TextFont;
                int preferredHeight = painter?.GetPreferredRowHeight(nodeInfo.Item, font) ?? GetScaledMinRowHeight();

                // Measure text size via TextRenderer
                font = UseThemeFont && _currentTheme != null
                    ? ThemeManagement.BeepThemesManager.ToFont(_currentTheme.LabelFont)
                    : TextFont;
                var measured = TextRenderer.MeasureText(nodeInfo.Item.Text ?? "", font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                nodeInfo.TextSize = measured;

                // Calculate row height (minimum or text-based)
                nodeInfo.RowHeight = Math.Max(preferredHeight, nodeInfo.TextSize.Height + GetScaledVerticalPadding() * 2);

                // Calculate X positions for elements
                int currentX = nodeInfo.Level * GetScaledIndentWidth();
                int boxSize = GetScaledBoxSize();
                int imageSize = GetScaledImageSize();

                // Toggle button (if has children)
                if (nodeInfo.Item.Children?.Count > 0)
                {
                    nodeInfo.ToggleRectContent = new Rectangle(currentX, y + (nodeInfo.RowHeight - boxSize) / 2, boxSize, boxSize);
                    currentX += boxSize + 4;
                }
                else
                {
                    nodeInfo.ToggleRectContent = Rectangle.Empty;
                }

                // Checkbox (if ShowCheckBox is true)
                if (ShowCheckBox)
                {
                    nodeInfo.CheckRectContent = new Rectangle(currentX, y + (nodeInfo.RowHeight - boxSize) / 2, boxSize, boxSize);
                    currentX += boxSize + 4;
                }
                else
                {
                    nodeInfo.CheckRectContent = Rectangle.Empty;
                }

                // Icon (if has image)
                if (!string.IsNullOrEmpty(nodeInfo.Item.ImagePath))
                {
                    nodeInfo.IconRectContent = new Rectangle(currentX, y + (nodeInfo.RowHeight - imageSize) / 2, imageSize, imageSize);
                    currentX += imageSize + 4;
                }
                else
                {
                    nodeInfo.IconRectContent = Rectangle.Empty;
                }

                // Text
                nodeInfo.TextRectContent = new Rectangle(
                    currentX,
                    y + (nodeInfo.RowHeight - nodeInfo.TextSize.Height) / 2,
                    nodeInfo.TextSize.Width + 10,
                    nodeInfo.TextSize.Height
                );
                currentX += nodeInfo.TextSize.Width + 10;

                // Row bounds - use DrawingRect width as default (nodes span full width)
                int minRowWidth = currentX;  // Minimum needed for content
                int rowWidth = Math.Max(minRowWidth, DrawingRect.Width);  // At least DrawingRect width
                nodeInfo.RowRectContent = new Rectangle(0, y, rowWidth, nodeInfo.RowHeight);
                nodeInfo.Y = y;
                nodeInfo.RowWidth = rowWidth;

                // CRITICAL: Write the modified struct back to the list!
                _visibleNodes[i] = nodeInfo;

                // Track maximum width (use minRowWidth for actual content width)
                if (minRowWidth > maxWidth)
                    maxWidth = minRowWidth;

                y += nodeInfo.RowHeight;

                // Write updated struct back to the list
                _visibleNodes[i] = nodeInfo;
            }

            // Update virtual size
            _totalContentHeight = y;
            _virtualSize = new Size(maxWidth, _totalContentHeight);
        }

        #endregion

        #region Node Finding Methods

        /// <summary>
        /// Finds a node by its text.
        /// </summary>
        public SimpleItem FindNode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            SimpleItem FindRecursive(IList<SimpleItem> items)
            {
                if (items == null) return null;

                foreach (var item in items)
                {
                    if (item.Text?.Equals(text, StringComparison.OrdinalIgnoreCase) == true)
                        return item;

                    var found = FindRecursive(item.Children);
                    if (found != null)
                        return found;
                }
                return null;
            }

            return FindRecursive(_nodes);
        }

        /// <summary>
        /// Gets a node by its GUID.
        /// </summary>
        public SimpleItem GetNodeByGuid(string guidid)
        {
            if (string.IsNullOrEmpty(guidid))
                return null;

            SimpleItem FindRecursive(IList<SimpleItem> items)
            {
                if (items == null) return null;

                foreach (var item in items)
                {
                    if (item.GuidId?.Equals(guidid, StringComparison.OrdinalIgnoreCase) == true)
                        return item;

                    var found = FindRecursive(item.Children);
                    if (found != null)
                        return found;
                }
                return null;
            }

            return FindRecursive(_nodes);
        }

        /// <summary>
        /// Gets a node by its name.
        /// </summary>
        public SimpleItem GetNode(string nodeName)
        {
            return FindNode(nodeName);
        }

        #endregion

        #region Node Manipulation Methods

        /// <summary>
        /// Expands all nodes in the tree.
        /// </summary>
        public void ExpandAll()
        {
            void ExpandRecursive(IList<SimpleItem> items)
            {
                if (items == null) return;
                foreach (var item in items)
                {
                    item.IsExpanded = true;
                    ExpandRecursive(item.Children);
                }
            }

            ExpandRecursive(_nodes);
            RebuildVisible();
            Invalidate();
        }

        /// <summary>
        /// Collapses all nodes in the tree.
        /// </summary>
        public void CollapseAll()
        {
            void CollapseRecursive(IList<SimpleItem> items)
            {
                if (items == null) return;
                foreach (var item in items)
                {
                    item.IsExpanded = false;
                    CollapseRecursive(item.Children);
                }
            }

            CollapseRecursive(_nodes);
            RebuildVisible();
            Invalidate();
        }

        /// <summary>
        /// Selects all nodes in the tree (if multi-select is enabled).
        /// </summary>
        public void SelectAllNodes()
        {
            if (!AllowMultiSelect) return;

            void SelectRecursive(IList<SimpleItem> items)
            {
                if (items == null) return;
                foreach (var item in items)
                {
                    item.IsSelected = true;
                    SelectedNodes.Add(item);
                    SelectRecursive(item.Children);
                }
            }

            SelectedNodes.Clear();
            SelectRecursive(_nodes);
            Invalidate();
        }

        /// <summary>
        /// Deselects all nodes in the tree.
        /// </summary>
        public void DeselectAllNodes()
        {
            void DeselectRecursive(IList<SimpleItem> items)
            {
                if (items == null) return;
                foreach (var item in items)
                {
                    item.IsSelected = false;
                    DeselectRecursive(item.Children);
                }
            }

            DeselectRecursive(_nodes);
            SelectedNodes.Clear();
            Invalidate();
        }

        /// <summary>
        /// Clears all nodes from the tree.
        /// </summary>
        public void ClearNodes()
        {
            _nodes.Clear();
            _visibleNodes.Clear();
            SelectedNodes.Clear();
            _lastSelectedNode = null;
            RefreshTree();
        }

        /// <summary>
        /// Filters nodes based on a predicate.
        /// </summary>
        public void FilterNodes(Func<SimpleItem, bool> predicate)
        {
            if (predicate == null)
                return;

            void FilterRecursive(IList<SimpleItem> items)
            {
                if (items == null) return;
                foreach (var item in items)
                {
                    item.IsVisible = predicate(item);
                    FilterRecursive(item.Children);
                }
            }

            FilterRecursive(_nodes);
            RebuildVisible();
            Invalidate();
        }

        #endregion
    }
}
