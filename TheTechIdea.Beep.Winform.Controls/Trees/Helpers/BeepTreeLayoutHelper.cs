using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Editors;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Helpers
{
    /// <summary>
    /// Helper class for tree layout calculation and caching.
    /// Handles text measurement, node positioning, and virtualization.
    /// Optimized to reduce allocations in hot paths.
    /// </summary>
    public class BeepTreeLayoutHelper
    {
        private readonly BeepTree _owner;
        private readonly BeepTreeHelper _treeHelper;
        private readonly List<NodeInfo> _layoutCache;

        // Background layout calculation for massive trees
        private System.Threading.CancellationTokenSource _layoutCts;
        private readonly object _layoutLock = new object();
        private bool _isLayoutCalculating;

        // Incremental update tracking
        private int _lastViewportStart = -1;
        private int _lastViewportEnd = -1;
        private int _lastTotalContentHeight = 0;

        public BeepTreeLayoutHelper(BeepTree owner, BeepTreeHelper treeHelper)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _treeHelper = treeHelper ?? throw new ArgumentNullException(nameof(treeHelper));
            _layoutCache = new List<NodeInfo>(512); // reserve capacity to avoid frequent resizes
        }

        #region Layout Calculation

        /// <summary>
        /// Rebuilds the layout cache for all visible nodes.
        /// </summary>
        public List<NodeInfo> RecalculateLayout()
        {
            _layoutCache.Clear();

            // Build list of visible nodes using yield-based traversal to avoid creating intermediate lists
            var visibleEnumerator = _treeHelper.TraverseVisible(_owner.Nodes).GetEnumerator();
            if (!visibleEnumerator.MoveNext())
                return _layoutCache;

            // Collect visible items into a local list only when needed for virtualization indexing
            List<SimpleItem> visibleItems = null;
            if (_owner.VirtualizeLayout)
            {
                visibleItems = new List<SimpleItem>();
                do
                {
                    visibleItems.Add(visibleEnumerator.Current);
                } while (visibleEnumerator.MoveNext());
            }

            // If not virtualizing, enumerate and calculate layout on the fly
            if (!_owner.VirtualizeLayout)
            {
                int y = 0;
                // process first element then continue enumerator
                var first = visibleEnumerator.Current;
                int levelFirst = _treeHelper.GetNodeLevel(first);
                var nodeInfoFirst = new NodeInfo { Item = first, Level = levelFirst, Y = y };
                CalculateNodeLayout(ref nodeInfoFirst);
                _layoutCache.Add(nodeInfoFirst);
                y += nodeInfoFirst.RowHeight;

                while (visibleEnumerator.MoveNext())
                {
                    var item = visibleEnumerator.Current;
                    int level = _treeHelper.GetNodeLevel(item);
                    var nodeInfo = new NodeInfo { Item = item, Level = level, Y = y };
                    CalculateNodeLayout(ref nodeInfo);
                    _layoutCache.Add(nodeInfo);
                    y += nodeInfo.RowHeight;
                }

                return _layoutCache;
            }

            // Virtualization path: use visibleItems list
            if (visibleItems == null || visibleItems.Count == 0)
                return _layoutCache;

            // Determine virtualization range
            var (start, end) = GetVirtualizationRange(visibleItems);

            int yPos = 0;
            for (int i = 0; i < visibleItems.Count; i++)
            {
                var item = visibleItems[i];
                int level = _treeHelper.GetNodeLevel(item);

                var nodeInfo = new NodeInfo { Item = item, Level = level, Y = yPos };

                // Apply conditional formatting
                if (_owner?.ConditionalFormats != null && _owner.ConditionalFormats.Count > 0)
                {
                    var (rowConfig, cellConfig) = _owner.ConditionalFormats.Evaluate(item);
                    if (rowConfig != null)
                    {
                        nodeInfo.RowConfig = rowConfig;
                    }
                    if (cellConfig != null)
                    {
                        nodeInfo.CellConfigs = new Dictionary<int, BeepTreeCellConfig> { { 0, cellConfig } };
                    }
                }

                if (i >= start && i <= end)
                {
                    CalculateNodeLayout(ref nodeInfo);
                }
                else
                {
                    nodeInfo.RowHeight = _owner.GetScaledMinRowHeight();
                }

                _layoutCache.Add(nodeInfo);
                yPos += nodeInfo.RowHeight;
            }

            // Update tracking
            _lastTotalContentHeight = yPos;

            return _layoutCache;
        }

        /// <summary>
        /// Performs layout calculation on a background thread for massive trees,
        /// then invokes the owner to invalidate on the UI thread.
        /// Use this when the tree has more than 10,000 visible nodes.
        /// </summary>
        public void RecalculateLayoutAsync()
        {
            // Cancel any previous calculation
            _layoutCts?.Cancel();
            _layoutCts = new System.Threading.CancellationTokenSource();
            var token = _layoutCts.Token;

            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    lock (_layoutLock)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        _isLayoutCalculating = true;
                        var result = RecalculateLayout();
                        _isLayoutCalculating = false;

                        if (!token.IsCancellationRequested && _owner != null && !_owner.IsDisposed)
                        {
                            _owner.BeginInvoke(new Action(() =>
                            {
                                if (!_owner.IsDisposed)
                                {
                                    _owner.UpdateScrollBars();
                                    _owner.Invalidate();
                                }
                            }));
                        }
                    }
                }
                catch (System.OperationCanceledException)
                {
                    // Expected when cancelled
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[BeepTreeLayoutHelper] Background layout error: {ex.Message}");
                }
                finally
                {
                    _isLayoutCalculating = false;
                }
            }, token);
        }

        /// <summary>
        /// Checks if layout is currently being calculated in the background.
        /// </summary>
        public bool IsLayoutCalculating => _isLayoutCalculating;

        /// <summary>
        /// Updates only the viewport portion of the layout cache without rebuilding everything.
        /// Useful for scroll operations where only the visible range changes.
        /// </summary>
        public void UpdateViewportLayout()
        {
            if (_layoutCache.Count == 0)
            {
                RecalculateLayout();
                return;
            }

            var (start, end) = GetVirtualizationRange(_layoutCache.Select(n => n.Item).ToList());
            
            if (start == _lastViewportStart && end == _lastViewportEnd)
                return; // Viewport hasn't changed

            _lastViewportStart = start;
            _lastViewportEnd = end;

            // Recalculate detailed layout only for nodes in the new viewport range
            for (int i = start; i <= end && i < _layoutCache.Count; i++)
            {
                var nodeInfo = _layoutCache[i];
                if (nodeInfo.RowHeight == _owner.GetScaledMinRowHeight())
                {
                    // This was a placeholder, calculate real layout
                    CalculateNodeLayout(ref nodeInfo);
                    _layoutCache[i] = nodeInfo;
                }
            }
        }

        /// <summary>
        /// Calculates detailed layout for a single node.
        /// </summary>
        private void CalculateNodeLayout(ref NodeInfo nodeInfo)
        {
            var item = nodeInfo.Item;
            string text = item?.Text ?? string.Empty;

            // Measure text using TextRenderer (no Graphics object needed)
            Size textSize = MeasureText(text, _owner.TextFont);

            // Calculate row height
            int rowHeight = CalculateRowHeight(textSize, nodeInfo);
            int baseIndent = CalculateIndent(nodeInfo.Level);

            // Toggle button
            int toggleX = baseIndent;
            int toggleY = nodeInfo.Y + (rowHeight - _owner.GetScaledBoxSize()) / 2;
            Rectangle toggleRect = new Rectangle(toggleX, toggleY, _owner.GetScaledBoxSize(), _owner.GetScaledBoxSize());

            // Checkbox (if enabled)
            int xAfterToggle = toggleRect.Right + 4;
            Rectangle checkRect = Rectangle.Empty;
            if (_owner.ShowCheckBox)
            {
                checkRect = new Rectangle(xAfterToggle, nodeInfo.Y + (rowHeight - _owner.GetScaledBoxSize()) / 2, _owner.GetScaledBoxSize(), _owner.GetScaledBoxSize());
                xAfterToggle = checkRect.Right + 4;
            }

            // Icon
            int iconX = xAfterToggle;
            int iconY = nodeInfo.Y + (rowHeight - _owner.GetScaledImageSize()) / 2;
            Rectangle iconRect = new Rectangle(iconX, iconY, _owner.GetScaledImageSize(), _owner.GetScaledImageSize());

            // Text
            int textX = iconRect.Right + 8;
            Rectangle textRect = new Rectangle(textX, nodeInfo.Y + _owner.GetScaledVerticalPadding(), textSize.Width + 10, textSize.Height);

            int minRowWidth = Math.Max(1, textX + textSize.Width + 10);
            int rowWidth = minRowWidth;
            Rectangle rowRect = new Rectangle(0, nodeInfo.Y, rowWidth, rowHeight);

            nodeInfo.TextSize = textSize;
            nodeInfo.RowHeight = rowHeight;
            nodeInfo.RowWidth = rowWidth;
            // Only assign a toggle rect for nodes that actually have children
            nodeInfo.ToggleRectContent = (nodeInfo.Item?.Children?.Count > 0) ? toggleRect : Rectangle.Empty;
            nodeInfo.CheckRectContent = checkRect;
            nodeInfo.IconRectContent = iconRect;
            nodeInfo.TextRectContent = textRect;
            nodeInfo.RowRectContent = rowRect;

            // Multi-column layout: calculate cell rectangles
            if (_owner.IsMultiColumn)
            {
                CalculateMultiColumnLayout(ref nodeInfo, rowHeight, baseIndent);
            }
        }

        /// <summary>
        /// Calculates cell rectangles for multi-column mode.
        /// </summary>
        private void CalculateMultiColumnLayout(ref NodeInfo nodeInfo, int rowHeight, int baseIndent)
        {
            var columns = _owner.Columns;
            if (columns == null || columns.Count == 0)
                return;

            int colIndex = 0;
            int x = 0;

            foreach (var column in columns.GetVisibleColumns())
            {
                var cellRect = new Rectangle(x, nodeInfo.Y, column.Width, rowHeight);

                if (colIndex == 0)
                {
                    // First column: tree structure (indent, toggle, checkbox, icon, text)
                    // Adjust for indent - the cell starts at indent position
                    cellRect = new Rectangle(baseIndent, nodeInfo.Y, column.Width - baseIndent, rowHeight);
                }

                nodeInfo.SetCellRect(colIndex, cellRect);

                // Measure cell text
                string cellText = GetCellText(nodeInfo.Item, column);
                Size cellTextSize = MeasureText(cellText, _owner.TextFont);
                nodeInfo.SetCellTextSize(colIndex, cellTextSize);

                x += column.Width;
                colIndex++;
            }

            // Update row width to include all columns
            nodeInfo.RowWidth = x;
        }

        /// <summary>
        /// Gets the display text for a cell based on the column's field binding.
        /// </summary>
        private string GetCellText(SimpleItem item, BeepTreeColumn column)
        {
            if (item == null || column == null)
                return string.Empty;

            // For now, use the item's Text for the first column, and try to get
            // additional values from the item's Data dictionary or properties
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

            // Fallback to item.Text for first column, empty for others
            return column.FieldName == item.Text ? item.Text : string.Empty;
        }

        #endregion

        #region Measurement

        /// <summary>
        /// Measures text size using TextRenderer (safe, no Graphics object required).
        /// </summary>
        public Size MeasureText(string text, Font font)
        {
            if (string.IsNullOrEmpty(text))
                return Size.Empty;

            return TextRenderer.MeasureText(text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
        }

        /// <summary>
        /// Calculates the row height for a node based on text size and custom row config.
        /// </summary>
        public int CalculateRowHeight(Size textSize, NodeInfo nodeInfo)
        {
            int minHeight = _owner.GetScaledMinRowHeight();
            int boxSize = _owner.GetScaledBoxSize();
            int imageSize = _owner.GetScaledImageSize();
            int vertPadding = _owner.GetScaledVerticalPadding();

            // Check for custom row height from config
            if (nodeInfo.RowConfig != null && nodeInfo.RowConfig.Height > 0)
            {
                int customHeight = nodeInfo.RowConfig.Height;
                if (nodeInfo.RowConfig.MinHeight > 0)
                {
                    customHeight = Math.Max(customHeight, nodeInfo.RowConfig.MinHeight);
                }
                return Math.Max(minHeight, customHeight);
            }

            int contentHeight = Math.Max(textSize.Height, Math.Max(boxSize, imageSize));
            return Math.Max(minHeight, contentHeight + 2 * vertPadding);
        }

        /// <summary>
        /// Legacy overload for backward compatibility.
        /// </summary>
        public int CalculateRowHeight(Size textSize)
        {
            return CalculateRowHeight(textSize, default);
        }

        /// <summary>
        /// Calculates the horizontal indent for a given level.
        /// </summary>
        public int CalculateIndent(int level)
        {
            return level * _owner.GetScaledIndentWidth();
        }

        #endregion

        #region Virtualization

        private (int start, int end) GetVirtualizationRange(List<SimpleItem> visibleItems)
        {
            int start = 0;
            int end = visibleItems.Count - 1;
            int yOffset = _owner.YOffset;
            int viewportHeight = _owner.GetClientArea().Height;
            int bufferRows = _owner.VirtualizationBufferRows;

            // Find start index
            int yAccum = 0;
            for (int i = 0; i < _layoutCache.Count; i++)
            {
                int estH = _layoutCache[i].RowHeight > 0 ? _layoutCache[i].RowHeight : _owner.GetScaledMinRowHeight();
                if (yAccum + estH >= yOffset)
                {
                    start = Math.Max(0, i - bufferRows);
                    break;
                }
                yAccum += estH;
            }

            int rowsInViewport = viewportHeight / Math.Max(1, _owner.GetScaledMinRowHeight());
            end = Math.Min(visibleItems.Count - 1, start + rowsInViewport + 2 * bufferRows);

            return (start, end);
        }

        public bool IsNodeInViewport(NodeInfo node)
        {
            int drawingHeight = _owner.GetClientArea().Height;
            if (drawingHeight <= 0)
            {
                return true;
            }

            int viewportTop = _owner.YOffset;
            int viewportBottom = _owner.YOffset + drawingHeight;

            int nodeBottom = node.Y + node.RowHeight;
            return nodeBottom >= viewportTop && node.Y <= viewportBottom;
        }

        #endregion

        #region Coordinate Transformation

        public Rectangle TransformToViewport(Rectangle contentRect)
        {
            Rectangle viewport = _owner.GetClientArea();
            return new Rectangle(viewport.Left + contentRect.X - _owner.XOffset,
                viewport.Top + contentRect.Y - _owner.YOffset,
                contentRect.Width, contentRect.Height);
        }

        public Point TransformToContent(Point viewportPoint)
        {
            Rectangle viewport = _owner.GetClientArea();
            return new Point(viewportPoint.X - viewport.Left + _owner.XOffset,
                viewportPoint.Y - viewport.Top + _owner.YOffset);
        }

        #endregion

        #region Cache Management

        public List<NodeInfo> GetCachedLayout()
        {
            return _layoutCache;
        }

        public NodeInfo? GetCachedLayoutForItem(SimpleItem item)
        {
            if (item == null) return null;
            return _layoutCache.FirstOrDefault(n => n.Item == item);
        }

        public void InvalidateCache()
        {
            _layoutCache.Clear();
        }

        /// <summary>
        /// Syncs the layout cache directly from pre-computed visible nodes,
        /// eliminating the need for a second tree traversal and O(depth) level lookups.
        /// </summary>
        public void SyncFromVisibleNodes(IReadOnlyList<NodeInfo> visibleNodes)
        {
            _layoutCache.Clear();
            if (visibleNodes == null || visibleNodes.Count == 0)
                return;
            // NodeInfo is a value type — each Add copies the struct
            for (int i = 0; i < visibleNodes.Count; i++)
                _layoutCache.Add(visibleNodes[i]);
        }

        public int CalculateTotalContentHeight()
        {
            int totalHeight = 0;
            foreach (var node in _layoutCache)
            {
                totalHeight += node.RowHeight > 0 ? node.RowHeight : _owner.GetScaledMinRowHeight();
            }
            return totalHeight;
        }

        public int CalculateTotalContentWidth()
        {
            int maxWidth = 0;
            foreach (var node in _layoutCache)
            {
                maxWidth = Math.Max(maxWidth, node.RowWidth);
            }
            return maxWidth;
        }

        #endregion
    }
}
