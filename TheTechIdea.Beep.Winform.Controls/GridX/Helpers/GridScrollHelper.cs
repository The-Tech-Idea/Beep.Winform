using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridScrollHelper
    {
        private readonly BeepGridPro _grid;
        public int HorizontalOffset { get; private set; }
        public int FirstVisibleRowIndex { get; private set; }

        public GridScrollHelper(BeepGridPro grid) { _grid = grid; }

        public void HandleMouseWheel(MouseEventArgs e)
        {
            // Vertical scroll by mouse wheel
            int deltaRows = e.Delta > 0 ? -1 : 1;
            FirstVisibleRowIndex = System.Math.Max(0, System.Math.Min(_grid.Data.Rows.Count - 1, FirstVisibleRowIndex + deltaRows));
            _grid.Layout.Recalculate();
            _grid.Invalidate();
        }

        public void ScrollHorizontal(int delta)
        {
            HorizontalOffset = System.Math.Max(0, HorizontalOffset + delta);
            _grid.Layout.Recalculate();
            _grid.Invalidate();
        }

        public void Reset()
        {
            HorizontalOffset = 0;
            FirstVisibleRowIndex = 0;
        }
    }
}
