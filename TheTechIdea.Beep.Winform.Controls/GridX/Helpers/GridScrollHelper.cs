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
        
        // Backward-compatible row index (read-only)
        public int FirstVisibleRowIndex => _firstVisibleRowIndex;

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
            int px = System.Math.Max(0, rowIndex) * _grid.RowHeight;
            SetVerticalOffset(px);
        }

        public void SetVerticalOffset(int offsetPx)
        {
            VerticalOffset = Math.Max(0, offsetPx);
            // ? FIX: Recalculate FirstVisibleRowIndex from pixel offset
            _firstVisibleRowIndex = CalculateRowIndexFromPixelOffset(offsetPx);
        }

        public void SetHorizontalOffset(int offset)
        {
            HorizontalOffset = System.Math.Max(0, offset);
            // Don't call layout recalculate here - let the scrollbar helper handle it
        }

        /// <summary>
        /// Calculate which row index corresponds to a given pixel offset
        /// </summary>
        private int CalculateRowIndexFromPixelOffset(int pixelOffset)
        {
            if (_grid.Data.Rows == null || pixelOffset <= 0)
                return 0;

            int currentOffset = 0;
            for (int i = 0; i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                
                if (currentOffset + rowHeight > pixelOffset)
                    return i;
                
                currentOffset += rowHeight;
            }

            return System.Math.Max(0, _grid.Data.Rows.Count - 1);
        }
    }
}
