using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

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

            return _layoutCache;
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
            int rowHeight = CalculateRowHeight(textSize);
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

            int minRowWidth = textX + textSize.Width + 10;
            int rowWidth = Math.Max(minRowWidth, _owner.DrawingRect.Width);
            Rectangle rowRect = new Rectangle(0, nodeInfo.Y, rowWidth, rowHeight);

            nodeInfo.TextSize = textSize;
            nodeInfo.RowHeight = rowHeight;
            nodeInfo.RowWidth = rowWidth;
            nodeInfo.ToggleRectContent = toggleRect;
            nodeInfo.CheckRectContent = checkRect;
            nodeInfo.IconRectContent = iconRect;
            nodeInfo.TextRectContent = textRect;
            nodeInfo.RowRectContent = rowRect;
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
        /// Calculates the row height for a node based on text size.
        /// </summary>
        public int CalculateRowHeight(Size textSize)
        {
            int minHeight = _owner.GetScaledMinRowHeight();
            int boxSize = _owner.GetScaledBoxSize();
            int imageSize = _owner.GetScaledImageSize();
            int vertPadding = _owner.GetScaledVerticalPadding();

            int contentHeight = Math.Max(textSize.Height, Math.Max(boxSize, imageSize));
            return Math.Max(minHeight, contentHeight + 2 * vertPadding);
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
            int viewportHeight = _owner.DrawingRect.Height;
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
            int drawingHeight = _owner.DrawingRect.Height;
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
            return new Rectangle(_owner.DrawingRect.Left + contentRect.X - _owner.XOffset,
                _owner.DrawingRect.Top + contentRect.Y - _owner.YOffset,
                contentRect.Width, contentRect.Height);
        }

        public Point TransformToContent(Point viewportPoint)
        {
            return new Point(viewportPoint.X - _owner.DrawingRect.Left + _owner.XOffset,
                viewportPoint.Y - _owner.DrawingRect.Top + _owner.YOffset);
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
