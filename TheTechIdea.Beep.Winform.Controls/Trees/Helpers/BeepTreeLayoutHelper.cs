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
    /// </summary>
    public class BeepTreeLayoutHelper
    {
        private readonly BeepTree _owner;
        private readonly BeepTreeHelper _treeHelper;
        private List<NodeInfo> _layoutCache;

        public BeepTreeLayoutHelper(BeepTree owner, BeepTreeHelper treeHelper)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _treeHelper = treeHelper ?? throw new ArgumentNullException(nameof(treeHelper));
            _layoutCache = new List<NodeInfo>();
        }

        #region Layout Calculation

        /// <summary>
        /// Rebuilds the layout cache for all visible nodes.
        /// </summary>
        public List<NodeInfo> RecalculateLayout()
        {
            _layoutCache.Clear();

            // Build list of visible nodes
            var visibleItems = _treeHelper.TraverseVisible(_owner.Nodes).ToList();
            if (visibleItems.Count == 0)
                return _layoutCache;

            // Determine virtualization range
            int start = 0, end = visibleItems.Count - 1;
            if (_owner.VirtualizeLayout && _owner.DrawingRect.Height > 0)
            {
                (start, end) = GetVirtualizationRange(visibleItems);
            }

            int y = 0;
            for (int i = 0; i < visibleItems.Count; i++)
            {
                var item = visibleItems[i];
                int level = _treeHelper.GetNodeLevel(item);

                // Create node info
                var nodeInfo = new NodeInfo
                {
                    Item = item,
                    Level = level,
                    Y = y
                };

                // Calculate layout for visible nodes in viewport
                if (i >= start && i <= end)
                {
                    CalculateNodeLayout(ref nodeInfo);
                }
                else
                {
                    // Use estimated height for nodes outside viewport
                    nodeInfo.RowHeight = _owner.GetScaledMinRowHeight();
                }

                _layoutCache.Add(nodeInfo);
                y += nodeInfo.RowHeight;
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

            // Calculate rectangles in content space (no scroll offset)
            
            // Toggle button
            int toggleX = baseIndent;
            int toggleY = nodeInfo.Y + (rowHeight - _owner.GetScaledBoxSize()) / 2;
            Rectangle toggleRect = new Rectangle(toggleX, toggleY, _owner.GetScaledBoxSize(), _owner.GetScaledBoxSize());

            // Checkbox (if enabled)
            int xAfterToggle = toggleRect.Right + 4;
            Rectangle checkRect = Rectangle.Empty;
            if (_owner.ShowCheckBox)
            {
                checkRect = new Rectangle(
                    xAfterToggle,
                    nodeInfo.Y + (rowHeight - _owner.GetScaledBoxSize()) / 2,
                    _owner.GetScaledBoxSize(),
                    _owner.GetScaledBoxSize());
                xAfterToggle = checkRect.Right + 4;
            }

            // Icon
            int iconX = xAfterToggle;
            int iconY = nodeInfo.Y + (rowHeight - _owner.GetScaledImageSize()) / 2;
            Rectangle iconRect = new Rectangle(iconX, iconY, _owner.GetScaledImageSize(), _owner.GetScaledImageSize());

            // Text (add padding to text width for better spacing)
            int textX = iconRect.Right + 8;
            Rectangle textRect = new Rectangle(
                textX,
                nodeInfo.Y + _owner.GetScaledVerticalPadding(),
                textSize.Width + 10,  // Add 10px padding after text
                textSize.Height);

            // Row bounds - use DrawingRect width as default (nodes span full width)
            int minRowWidth = textX + textSize.Width + 10;  // Minimum needed for content
            int rowWidth = Math.Max(minRowWidth, _owner.DrawingRect.Width);  // At least DrawingRect width
            Rectangle rowRect = new Rectangle(0, nodeInfo.Y, rowWidth, rowHeight);

            // Update node info
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

            return TextRenderer.MeasureText(text, font,
                new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.NoPadding);
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

        /// <summary>
        /// Determines which nodes are in the viewport for virtualization.
        /// </summary>
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

            // Calculate end index
            int rowsInViewport = viewportHeight / Math.Max(1, _owner.GetScaledMinRowHeight());
            end = Math.Min(visibleItems.Count - 1, start + rowsInViewport + 2 * bufferRows);

            return (start, end);
        }

        /// <summary>
        /// Checks if a node is currently in the viewport.
        /// </summary>
        public bool IsNodeInViewport(NodeInfo node)
        {
            // If viewport height isn't established yet, don't filter anything out
            // This prevents an empty render when DrawingRect is zero-sized early in the paint pipeline
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

        /// <summary>
        /// Transforms a rectangle from content space to viewport space.
        /// </summary>
        public Rectangle TransformToViewport(Rectangle contentRect)
        {
            return new Rectangle(
                _owner.DrawingRect.Left + contentRect.X - _owner.XOffset,
                _owner.DrawingRect.Top + contentRect.Y - _owner.YOffset,
                contentRect.Width,
                contentRect.Height);
        }

        /// <summary>
        /// Transforms a point from viewport space to content space.
        /// </summary>
        public Point TransformToContent(Point viewportPoint)
        {
            return new Point(
                viewportPoint.X - _owner.DrawingRect.Left + _owner.XOffset,
                viewportPoint.Y - _owner.DrawingRect.Top + _owner.YOffset);
        }

        #endregion

        #region Cache Management

        /// <summary>
        /// Gets the cached layout for all visible nodes.
        /// </summary>
        public List<NodeInfo> GetCachedLayout()
        {
            return _layoutCache;
        }

        /// <summary>
        /// Finds cached layout for a specific item.
        /// </summary>
        public NodeInfo? GetCachedLayoutForItem(SimpleItem item)
        {
            if (item == null)
                return null;

            return _layoutCache.FirstOrDefault(n => n.Item == item);
        }

        /// <summary>
        /// Invalidates the entire layout cache (forces recalculation).
        /// </summary>
        public void InvalidateCache()
        {
            _layoutCache.Clear();
        }

        /// <summary>
        /// Calculates total content height from cached layouts.
        /// </summary>
        public int CalculateTotalContentHeight()
        {
            int totalHeight = 0;
            foreach (var node in _layoutCache)
            {
                totalHeight += node.RowHeight > 0 ? node.RowHeight : _owner.GetScaledMinRowHeight();
            }
            return totalHeight;
        }

        /// <summary>
        /// Calculates maximum content width from cached layouts.
        /// </summary>
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
