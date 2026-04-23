using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridScrollHelper
    {
        private readonly BeepGridPro _grid;
        private int _firstVisibleRowIndex = 0;
        
        public int HorizontalOffset { get; private set; }
        public int VerticalOffset { get; private set; }
        
        /// <summary>
        /// Index of the first visible row relative to the current row collection.
        /// In virtualization mode this is always 0 because Data.Rows contains only the visible window.
        /// </summary>
        public int FirstVisibleRowIndex
        {
            get
            {
                if (_grid.EnableVirtualization && _grid.VirtualDataSource != null)
                    return 0;
                return _firstVisibleRowIndex;
            }
        }

        public GridScrollHelper(BeepGridPro grid) 
        { 
            _grid = grid; 
        }

        public void HandleMouseWheel(MouseEventArgs e)
        {
            // Mouse wheel scrolling exactly like BeepTree
            int smallChange = 20; // Same as BeepTree
            int deltaOffset = e.Delta > 0 ? -smallChange : smallChange;
            SetVerticalOffset(VerticalOffset + deltaOffset);
        }

        public void ScrollHorizontal(int delta)
        {
            SetHorizontalOffset(HorizontalOffset + delta);
        }

        public void Reset()
        {
            HorizontalOffset = 0;
            VerticalOffset = 0;
            _firstVisibleRowIndex = 0;
        }

        public void SetVerticalIndex(int rowIndex)
        {
            if (_grid.Data?.Rows == null || _grid.Data.Rows.Count == 0 || rowIndex <= 0)
            {
                SetVerticalOffset(0);
                return;
            }

            int clampedIndex = System.Math.Max(0, System.Math.Min(rowIndex, _grid.Data.Rows.Count - 1));
            int px = 0;
            for (int i = 0; i < clampedIndex; i++)
            {
                var r = _grid.Data.Rows[i];
                if (!r.IsVisible) continue;
                px += r.Height > 0 ? r.Height : _grid.RowHeight;
            }
            // Account for group headers placed before the target row
            px += (_grid.GroupEngine?.GetHeaderCountBeforeRow(clampedIndex) ?? 0) * (_grid.GroupEngine?.GetHeaderHeight() ?? 0);
            SetVerticalOffset(px);
        }

        public void SetVerticalOffset(int offsetPx)
        {
            if (VerticalOffset == offsetPx) return;

            VerticalOffset = Math.Max(0, offsetPx);

            if (_grid.EnableVirtualization && _grid.VirtualDataSource != null)
            {
                _firstVisibleRowIndex = CalculateRowIndexFromPixelOffset(VerticalOffset);
                int viewportHeight = Math.Max(1, _grid.Layout?.RowsRect.Height ?? _grid.Height);
                _grid.RowVirtualizer.UpdateWindow(VerticalOffset, viewportHeight, _grid.RowHeight);
            }
            else
            {
                // === ALWAYS RECALCULATE FIRST VISIBLE ROW ===
                _firstVisibleRowIndex = CalculateRowIndexFromPixelOffset(VerticalOffset);
            }

            // Trigger layout update
            _grid.Layout.Recalculate();
            _grid.ScrollBars?.UpdateBars();
            _grid.SafeInvalidate();
        }

        public void SetHorizontalOffset(int offset)
        {
            if (HorizontalOffset == offset) return;

            HorizontalOffset = Math.Max(0, offset);
            // No row index for horizontal
            _grid.Layout.Recalculate();
            _grid.ScrollBars?.UpdateBars();
            _grid.SafeInvalidate();
        }

        /// <summary>
        /// Calculate which row index corresponds to a given pixel offset.
        /// In virtual mode, returns the absolute row index (into the full virtual dataset).
        /// </summary>
        private int CalculateRowIndexFromPixelOffset(int pixelOffset)
        {
            if (_grid.EnableVirtualization && _grid.VirtualDataSource != null)
            {
                if (pixelOffset <= 0) return 0;
                return pixelOffset / System.Math.Max(1, _grid.RowHeight);
            }

            if (_grid.Data?.Rows == null || pixelOffset <= 0)
                return 0;

            int currentOffset = 0;
            int previousHeaderCount = 0;
            int headerHeight = _grid.GroupEngine?.GetHeaderHeight() ?? 0;

            for (int i = 0; i < _grid.Data.Rows.Count; i++)
            {
                // Inject header height for any groups whose first row is at this index
                int currentHeaderCount = _grid.GroupEngine?.GetHeaderCountBeforeRow(i) ?? 0;
                int newHeaders = currentHeaderCount - previousHeaderCount;
                if (newHeaders > 0)
                {
                    currentOffset += newHeaders * headerHeight;
                    previousHeaderCount = currentHeaderCount;
                }

                var row = _grid.Data.Rows[i];
                if (!row.IsVisible) continue;
                int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                
                if (currentOffset + rowHeight > pixelOffset)
                    return i;
                
                currentOffset += rowHeight;
            }

            for (int i = _grid.Data.Rows.Count - 1; i >= 0; i--)
            {
                if (_grid.Data.Rows[i].IsVisible) return i;
            }
            return System.Math.Max(0, _grid.Data.Rows.Count - 1);
        }
    }
}
