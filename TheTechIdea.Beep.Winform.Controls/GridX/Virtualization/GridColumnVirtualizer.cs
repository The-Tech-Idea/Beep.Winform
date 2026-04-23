using System;
using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Virtualization
{
    /// <summary>
    /// Manages on-demand column visibility for wide grids with many columns.
    /// Tracks a horizontal window of scrolling (non-sticky) columns so that
    /// rendering, hit-testing, and layout can skip entirely off-screen columns.
    /// </summary>
    public sealed class GridColumnVirtualizer
    {
        private readonly BeepGridPro _grid;
        private int _firstScrollingVisibleIndex;
        private int _lastScrollingVisibleIndex;
        private bool _isActive = true;
        private int _overscan = 2;

        public GridColumnVirtualizer(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        /// <summary>
        /// Whether column virtualization is enabled. When false, all columns are treated as visible.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        /// <summary>
        /// Number of extra scrolling columns to include on each side of the visible viewport.
        /// Higher values reduce pop-in during fast horizontal scrolling at the cost of more cell draws.
        /// Default: 2.
        /// </summary>
        public int Overscan
        {
            get => _overscan;
            set => _overscan = Math.Max(0, value);
        }

        /// <summary>
        /// Index (within the ordered scrolling-columns list) of the first visible column.
        /// </summary>
        public int FirstScrollingVisibleIndex => _firstScrollingVisibleIndex;

        /// <summary>
        /// Index (within the ordered scrolling-columns list) of the last visible column.
        /// </summary>
        public int LastScrollingVisibleIndex => _lastScrollingVisibleIndex;

        /// <summary>
        /// Updates the visible column window based on horizontal scroll offset and viewport width.
        /// Call this after horizontal scroll changes or during layout recalculation.
        /// </summary>
        /// <param name="horizontalOffset">Current horizontal scroll offset in pixels.</param>
        /// <param name="viewportWidth">Width of the scrolling viewport in pixels.</param>
        public void UpdateWindow(int horizontalOffset, int viewportWidth)
        {
            if (!_isActive || _grid.Data?.Columns == null || _grid.Data.Columns.Count == 0)
            {
                _firstScrollingVisibleIndex = 0;
                _lastScrollingVisibleIndex = int.MaxValue;
                return;
            }

            // Get scrolling columns in display order
            var scrollCols = _grid.Data.Columns
                .Select((c, idx) => new { Col = c, Index = idx })
                .Where(x => x.Col.Visible && !x.Col.Sticked)
                .OrderBy(x => x.Col.DisplayOrder)
                .ToList();

            if (scrollCols.Count == 0)
            {
                _firstScrollingVisibleIndex = 0;
                _lastScrollingVisibleIndex = int.MaxValue;
                return;
            }

            if (viewportWidth <= 0)
            {
                _firstScrollingVisibleIndex = 0;
                _lastScrollingVisibleIndex = int.MaxValue;
                return;
            }

            int x = -horizontalOffset;
            int first = 0;
            int last = scrollCols.Count - 1;

            // Find first column whose right edge is past the left viewport edge
            for (int i = 0; i < scrollCols.Count; i++)
            {
                int colW = Math.Max(20, scrollCols[i].Col.Width);
                if (x + colW > 0)
                {
                    first = i;
                    break;
                }
                x += colW;
            }

            // Find last column whose left edge is before the right viewport edge
            x = -horizontalOffset;
            for (int i = 0; i < scrollCols.Count; i++)
            {
                int colW = Math.Max(20, scrollCols[i].Col.Width);
                if (x >= viewportWidth)
                {
                    last = i - 1;
                    break;
                }
                x += colW;
            }

            // Apply overscan
            first = Math.Max(0, first - _overscan);
            last = Math.Min(scrollCols.Count - 1, last + _overscan);

            _firstScrollingVisibleIndex = first;
            _lastScrollingVisibleIndex = Math.Max(first, last);
        }

        /// <summary>
        /// Returns whether a given ordered scrolling-column index is inside the visible window.
        /// </summary>
        public bool IsScrollingColumnVisible(int orderedIndex)
        {
            if (!_isActive)
                return true;
            return orderedIndex >= _firstScrollingVisibleIndex && orderedIndex <= _lastScrollingVisibleIndex;
        }

        /// <summary>
        /// Invalidates the current window so the next <see cref="UpdateWindow"/> call recomputes everything.
        /// </summary>
        public void Refresh()
        {
            _firstScrollingVisibleIndex = 0;
            _lastScrollingVisibleIndex = int.MaxValue;
        }
    }
}
