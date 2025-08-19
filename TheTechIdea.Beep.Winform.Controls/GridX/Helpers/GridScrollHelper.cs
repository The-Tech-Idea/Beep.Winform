using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridScrollHelper
    {
        private readonly BeepGridPro _grid;
        public GridScrollHelper(BeepGridPro grid) { _grid = grid; }

        public void HandleMouseWheel(MouseEventArgs e)
        {
            _grid.Invalidate();
        }
    }
}
