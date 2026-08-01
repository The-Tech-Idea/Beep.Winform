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

        // The geometry that used to live here -- RecalculateLayout, RecalculateLayoutAsync,
        // CalculateNodeLayout, CalculateMultiColumnLayout and GetCellText -- has been removed.
        //
        // It was a second, divergent implementation of node layout: the control's own
        // BeepTree.RecalculateLayoutCache is what actually drives rendering, and SyncFromVisibleNodes
        // overwrote whatever this class computed. Measured against each other the two disagreed by
        // 4px on every node. The only live route into this code was RecalculateLayoutAsync, which
        // BeepTree fired automatically above 10,000 visible nodes -- so a tree could change its
        // indentation just by growing, and did so from a worker thread that mutated the cache paint
        // reads from.
        //
        // Multi-column cell rects (the only implementation anywhere, and previously never reached,
        // which is why BaseTreePainter always read Rectangle.Empty for every cell) were ported to
        // BeepTree.CalculateMultiColumnCells along with a fix for the first-column width.
        //
        // This class now owns the cache, viewport range, coordinate transforms and measurement --
        // not geometry.

        /// <summary>
        /// Rebuilds the layout by asking the control's single layout engine to run, then returns
        /// the resulting cache.
        /// <para>
        /// Kept as a method because callers legitimately need "recompute now and give me the
        /// layout" — but it no longer computes anything itself. It used to run this class's own
        /// divergent geometry, which measured 4px per node away from what the control produced,
        /// and callers that ran it straight after <c>RecalculateLayoutCache()</c> were silently
        /// replacing correct geometry with wrong geometry.
        /// </para>
        /// </summary>
        public List<NodeInfo> RecalculateLayout()
        {
            _owner.RecalculateLayoutCache();
            return _layoutCache;
        }

        /// <summary>
        /// Tracks the viewport range for scroll operations.
        /// <para>
        /// This used to re-run a second geometry implementation for any node whose RowHeight
        /// happened to equal the minimum, which would have shifted indentation mid-scroll had that
        /// equality ever held. It no longer computes geometry:
        /// <see cref="BeepTree.RecalculateLayoutCache"/> lays out every visible node eagerly, so
        /// there are no placeholders left to fill in.
        /// </para>
        /// </summary>
        public void UpdateViewportLayout()
        {
            if (_layoutCache.Count == 0)
                return;

            var (start, end) = GetVirtualizationRange(_layoutCache.Count);
            if (start == _lastViewportStart && end == _lastViewportEnd)
                return;

            _lastViewportStart = start;
            _lastViewportEnd = end;
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

            // Same flags the painter draws node labels with, so measurement and rendering agree.
            return TextRenderer.MeasureText(text, font, new Size(int.MaxValue, int.MaxValue),
                Painters.BaseTreePainter.NodeTextFlags);
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

        /// <summary>
        /// Computes the visible index range for the current scroll offset.
        /// <para>
        /// Takes a count rather than a materialised list: the caller used to build a fresh
        /// <c>List&lt;SimpleItem&gt;</c> of the entire tree on every scroll
        /// (<c>_layoutCache.Select(n => n.Item).ToList()</c>) purely to read <c>.Count</c>, while
        /// the row heights it actually needs were already in <c>_layoutCache</c>.
        /// </para>
        /// </summary>
        private (int start, int end) GetVirtualizationRange(int visibleItemCount)
        {
            int start = 0;
            int end = visibleItemCount - 1;
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
            end = Math.Min(visibleItemCount - 1, start + rowsInViewport + 2 * bufferRows);

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
