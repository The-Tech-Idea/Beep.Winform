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
        internal void RebuildVisible()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"BeepTree.RebuildVisible: Starting with {_nodes.Count} root nodes");
#endif
            _visibleNodes.Clear();

            void Recurse(SimpleItem item, int level, SimpleItem parent = null)
            {
                // Skip nodes that are filtered out (IsVisible = false)
                if (item != null && !item.IsVisible)
                    return;

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
            
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"BeepTree.RebuildVisible: Created {_visibleNodes.Count} visible nodes");
#endif

            // Recalculate layout after rebuilding
            RecalculateLayoutCache();

            // Sync helper cache from already-computed _visibleNodes (avoids second traversal)
            try
            {
                _layoutHelper?.SyncFromVisibleNodes(_visibleNodes);
                // Recompute virtual size from helper cache for accurate scrollbars
                _virtualSize = new Size(
                    _layoutHelper?.CalculateTotalContentWidth() ?? 0,
                    _layoutHelper?.CalculateTotalContentHeight() ?? 0);
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"BeepTree.RebuildVisible: LayoutHelper sync failed: {ex.Message}");
#else
                _ = ex; // suppress unused warning in Release
#endif
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
        /// Assigns per-column cell rectangles for multi-column mode.
        /// <para>
        /// Ported from the previously unreachable <c>BeepTreeLayoutHelper.CalculateMultiColumnLayout</c>
        /// so that the one engine that drives rendering also produces column geometry.
        /// </para>
        /// </summary>
        private void CalculateMultiColumnCells(ref NodeInfo nodeInfo, int y)
        {
            var columns = Columns;
            if (columns == null) return;

            int colIndex = 0;
            int x = 0;

            foreach (var column in columns.GetVisibleColumns())
            {
                // Every column keeps its full declared width, including the first.
                //
                // The original implementation gave column 0 a rect of `column.Width - baseIndent`,
                // which shrank the column by the node's own indent — so a deep node produced a
                // narrower first column than a shallow one and the column edges did not line up
                // down the tree. Indentation belongs to the *content* inside the cell (the toggle,
                // icon and text rects already carry it), not to the cell itself.
                var cellRect = new Rectangle(x, y, column.Width, nodeInfo.RowHeight);
                nodeInfo.SetCellRect(colIndex, cellRect);

                string cellText = GetCellText(nodeInfo.Item, column);
                Font cellFont = _useThemeFont && _currentTheme != null
                    ? ThemeManagement.BeepThemesManager.ToFont(_currentTheme.LabelFont)
                    : TextFont;
                nodeInfo.SetCellTextSize(colIndex, TextRenderer.MeasureText(
                    cellText, cellFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding));

                x += column.Width;
                colIndex++;
            }

            nodeInfo.RowWidth = Math.Max(nodeInfo.RowWidth, x);
        }

        /// <summary>
        /// Resolves the display text for a cell from the column's field binding, falling back to
        /// the node's own text for the first column.
        /// </summary>
        private string GetCellText(SimpleItem item, Trees.Models.BeepTreeColumn column)
        {
            if (item == null || column == null) return string.Empty;
            if (string.IsNullOrEmpty(column.FieldName)) return item.Text ?? string.Empty;

            if (item.Data != null && item.Data.TryGetValue(column.FieldName, out var value) && value != null)
            {
                return string.IsNullOrEmpty(column.FormatString)
                    ? value.ToString()
                    : string.Format("{0:" + column.FormatString + "}", value);
            }

            return column.FieldName == item.Text ? item.Text : string.Empty;
        }

        /// <summary>
        /// The single font node labels are both measured and drawn with.
        /// <para>
        /// Layout used to resolve the theme font with <c>BeepThemesManager.ToFont</c> while
        /// <c>BaseTreePainter</c> drew with <c>ToFontForControl</c> — a DPI-scaled variant. The two
        /// produce different sizes, so every label was measured smaller than it rendered and got
        /// clipped mid-word at the right edge of a rectangle sized from the wrong font. Both sides
        /// now call this.
        /// </para>
        /// </summary>
        internal Font GetNodeFont()
        {
            if (_useThemeFont && _currentTheme?.LabelFont != null)
            {
                var themed = ThemeManagement.BeepThemesManager.ToFontForControl(_currentTheme.LabelFont, this);
                if (themed != null) return themed;
            }
            return _textFont ?? SystemFonts.DefaultFont;
        }

        /// <summary>
        /// True when at least one visible node carries an image, which is the condition
        /// <see cref="IconSlotMode.WhenAnyNodeHasIcon"/> reserves the icon column on.
        /// <para>
        /// O(n) over the visible list, which the layout pass already walks. Kept as a separate pass
        /// rather than folded into the geometry loop because the answer has to be known *before*
        /// the first row is positioned.
        /// </para>
        /// </summary>
        private bool AnyVisibleNodeHasIcon()
        {
            for (int i = 0; i < _visibleNodes.Count; i++)
            {
                if (!string.IsNullOrEmpty(_visibleNodes[i].Item?.ImagePath))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Recalculates the layout cache for all visible nodes.
        /// Measures text sizes and determines rectangles for all node elements.
        /// </summary>
        internal void RecalculateLayoutCache()
        {
            // CRITICAL: Ensure DrawingRect is up-to-date with current ControlStyle metrics
            // This ensures the painter has properly calculated border/padding/shadow
            UpdateDrawingRect();
            
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"BeepTree.RecalculateLayoutCache: Processing {_visibleNodes.Count} nodes");
            System.Diagnostics.Debug.WriteLine($"BeepTree.RecalculateLayoutCache: DrawingRect = {DrawingRect}, ControlStyle = {ControlStyle}, UseFormStylePaint = {UseFormStylePaint}");
#endif
            if (_visibleNodes.Count == 0)
            {
                return;
            }

            int y = 0;
            int maxWidth = 0;

            // Painter and font resolved once per pass rather than per node. Both are invariant
            // across the loop, and the font resolution allocated a new Font object *twice per node*
            // -- 20,000 undisposed Fonts on a 10,000-node tree.
            var layoutPainter = GetCurrentPainter();
            // Ask the *painter* which font it will draw labels with, rather than assuming the
            // tree's font. Styles like VercelClean (monospace) and FileBrowser (compact) render in
            // their own face; measuring with the tree font would size every rect for the wrong one.
            Font nodeFont = layoutPainter?.GetNodeFont(this) ?? GetNodeFont();
            int labelTrailingReserve = layoutPainter?.GetLabelTrailingReserve() ?? 0;

            // Decide once per pass whether rows reserve the icon column. Doing it per node is what
            // left labels ragged: a node with no ImagePath skipped the width entirely, so it sat
            // one icon-width left of its icon-bearing siblings on the same level.
            bool reserveIconSlot = _iconSlotMode switch
            {
                IconSlotMode.Always => true,
                IconSlotMode.Never => false,
                _ => AnyVisibleNodeHasIcon()
            };
            // Use index-based loop because NodeInfo is a struct (value type).
            // Modifying a foreach iteration variable for a struct is illegal and would not persist.
            for (int i = 0; i < _visibleNodes.Count; i++)
            {
                var nodeInfo = _visibleNodes[i];
                int preferredHeight = layoutPainter?.GetPreferredRowHeight(nodeInfo.Item, nodeFont) ?? GetScaledMinRowHeight();

                // Measured with the same font AND the same flags the painter draws with
                // (GetNodeFont / BaseTreePainter.NodeTextFlags). Measuring with one font and
                // drawing with another is what clipped labels mid-word.
                var measured = TextRenderer.MeasureText(nodeInfo.Item.Text ?? "", nodeFont,
                    new Size(int.MaxValue, int.MaxValue),
                    Trees.Painters.BaseTreePainter.NodeTextFlags);
                nodeInfo.TextSize = measured;

                // Calculate row height (minimum or text-based)
                nodeInfo.RowHeight = Math.Max(preferredHeight, nodeInfo.TextSize.Height + GetScaledVerticalPadding() * 2);

                // Calculate X positions for elements
                int currentX = nodeInfo.Level * GetScaledIndentWidth();
                int boxSize = GetScaledBoxSize();
                int imageSize = GetScaledImageSize();

                // Expander slot: reserved for EVERY node, drawn only for nodes with children.
                //
                // currentX used to advance past the toggle only when the node actually had
                // children, so a leaf started its icon/text one box-width further left than an
                // expandable sibling on the same level. With a 16px indent and a 14px box, a leaf
                // at level N landed within 2px of a parent at level N-1 -- children rendered at
                // their own parent's indent and the hierarchy read wrong. Reserving the slot
                // unconditionally is what standard trees (Explorer, VS Code) do.
                bool hasChildren = nodeInfo.Item.Children?.Count > 0;
                nodeInfo.ToggleRectContent = hasChildren
                    ? new Rectangle(currentX, y + (nodeInfo.RowHeight - boxSize) / 2, boxSize, boxSize)
                    : Rectangle.Empty;
                currentX += boxSize + 4;

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

                // Icon slot. Same rule as the expander: the rect is only produced for nodes that
                // actually have an image, but the width is reserved for every row so labels line up.
                // See IconSlotMode for why this is opt-out rather than unconditional.
                bool hasIcon = !string.IsNullOrEmpty(nodeInfo.Item.ImagePath);
                nodeInfo.IconRectContent = hasIcon
                    ? new Rectangle(currentX, y + (nodeInfo.RowHeight - imageSize) / 2, imageSize, imageSize)
                    : Rectangle.Empty;
                if (hasIcon || reserveIconSlot)
                {
                    currentX += imageSize + 4;
                }

                // Text. Width includes whatever trailing space the painter reserves for decoration
                // it draws inside this rect -- StripeDashboard appends a metric badge, and used to
                // make room by shrinking the rect it was handed, which squeezed the label down to
                // "Root ...". The painter declares the reserve; the layout provides it.
                int textWidth = nodeInfo.TextSize.Width + 10 + labelTrailingReserve;
                nodeInfo.TextRectContent = new Rectangle(
                    currentX,
                    y + (nodeInfo.RowHeight - nodeInfo.TextSize.Height) / 2,
                    textWidth,
                    nodeInfo.TextSize.Height
                );
                currentX += textWidth;

                // Row bounds should reflect actual content width only.
                // Forcing row width to viewport width causes virtual width inflation and
                // can incorrectly force a horizontal scrollbar when vertical is visible.
                int minRowWidth = Math.Max(1, currentX);
                int rowWidth = minRowWidth;
                
                nodeInfo.RowRectContent = new Rectangle(0, y, rowWidth, nodeInfo.RowHeight);
                nodeInfo.Y = y;
                nodeInfo.RowWidth = rowWidth;

                // Multi-column cell rectangles. BaseTreePainter and BeepTreeCellEditor both read
                // GetCellRect(colIndex); until this ran on the live path the only implementation
                // sat in BeepTreeLayoutHelper, which nothing called, so every cell rect was Empty
                // and multi-column mode rendered no columns at all.
                if (IsMultiColumn)
                {
                    CalculateMultiColumnCells(ref nodeInfo, y);
                    minRowWidth = nodeInfo.RowWidth;
                }

                // CRITICAL: Write the modified struct back to the list!
                _visibleNodes[i] = nodeInfo;

                // Track maximum width (use minRowWidth for actual content width)
                if (minRowWidth > maxWidth)
                    maxWidth = minRowWidth;

                y += nodeInfo.RowHeight;
            }

            // Update total content height (virtual size is owned by RebuildVisible via layout helper)
            _totalContentHeight = y;

            // Publish to the helper's cache, which is what hit-testing and the viewport transforms
            // read.
            //
            // This deliberately no longer kicks off BeepTreeLayoutHelper.RecalculateLayoutAsync()
            // above 10,000 nodes. That path ran a *second, different* geometry implementation --
            // measured 4px apart from this one on every node -- on a worker thread, and mutated
            // the very cache paint reads. A tree could therefore change its indentation purely by
            // growing past a node count, with a data race on top. There is now one engine.
            _layoutHelper?.SyncFromVisibleNodes(_visibleNodes);
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
