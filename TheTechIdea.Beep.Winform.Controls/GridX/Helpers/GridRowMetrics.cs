using System;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Prefix-sum index of row offsets. Rebuilt only when the layout is
    /// invalidated or the row count changes, so scroll and paint paths do not
    /// scan from row zero on every operation.
    /// </summary>
    internal sealed class GridRowMetrics
    {
        private readonly BeepGridPro _grid;
        private int[] _tops = Array.Empty<int>();
        private int[] _heights = Array.Empty<int>();
        private int _count;
        private bool _dirty = true;

        public GridRowMetrics(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        public void Invalidate() => _dirty = true;

        public int TotalHeight
        {
            get
            {
                Ensure();
                return _count == 0 ? 0 : _tops[_count - 1] + _heights[_count - 1];
            }
        }

        /// <summary>Offset of <paramref name="rowIndex"/> from the content top.</summary>
        public int TopOfRow(int rowIndex)
        {
            Ensure();
            if (_count == 0 || rowIndex <= 0) return 0;
            if (rowIndex >= _count) return TotalHeight;
            return _tops[rowIndex];
        }

        /// <summary>
        /// Index of the first visible row whose bottom is below
        /// <paramref name="pixelOffset"/>; the last visible row when the offset
        /// is past the content.
        /// </summary>
        public int FirstVisibleRow(int pixelOffset)
        {
            Ensure();
            if (_count == 0) return 0;

            int lo = 0;
            int hi = _count - 1;
            int result = 0;
            while (lo <= hi)
            {
                int mid = lo + ((hi - lo) / 2);
                if (_tops[mid] <= pixelOffset)
                {
                    result = mid;
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }

            for (int i = result; i < _count; i++)
            {
                if (_heights[i] > 0) return i;
            }

            for (int i = _count - 1; i >= 0; i--)
            {
                if (_heights[i] > 0) return i;
            }

            return Math.Max(0, _count - 1);
        }

        private void Ensure()
        {
            int rowCount = _grid.Data?.Rows?.Count ?? 0;
            if (!_dirty && _count == rowCount) return;

            _count = rowCount;
            _tops = new int[rowCount];
            _heights = new int[rowCount];

            int currentOffset = 0;
            int previousHeaderCount = 0;
            int headerHeight = _grid.GroupEngine?.GetHeaderHeight() ?? 0;

            for (int i = 0; i < rowCount; i++)
            {
                int currentHeaderCount = _grid.GroupEngine?.GetHeaderCountBeforeRow(i) ?? 0;
                int newHeaders = currentHeaderCount - previousHeaderCount;
                if (newHeaders > 0)
                {
                    currentOffset += newHeaders * headerHeight;
                    previousHeaderCount = currentHeaderCount;
                }

                _tops[i] = currentOffset;

                var row = _grid.Data.Rows[i];
                if (!row.IsVisible)
                {
                    _heights[i] = 0;
                    continue;
                }

                int height = row.Height > 0 ? row.Height : _grid.RowHeight;
                _heights[i] = height;
                currentOffset += height;
            }

            _dirty = false;
        }
    }
}
