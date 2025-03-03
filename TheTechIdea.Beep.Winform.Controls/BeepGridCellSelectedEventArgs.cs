using TheTechIdea.Beep.Winform.Controls.Grid;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepGridCellSelectedEventArgs
    {
        public BeepGridCellSelectedEventArgs(int row, int col, BeepGridCell cell)
        {
            Row = row;
            Col = col;
            Cell = cell;
        }
        public BeepGridCellSelectedEventArgs(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public int Row { get; }
        public int Col { get; }
        public BeepGridCell Cell { get; set; }
    }
    public class BeepGridRowSelectedEventArgs
    {
        public BeepGridRowSelectedEventArgs(int row, BeepGridRow rowdata)
        {
            RowIndex = row;
            Row = rowdata;
        }
        public BeepGridRowSelectedEventArgs(int row)
        {
            RowIndex = row;
        }
        public BeepGridRow Row { get; }
        public int RowIndex { get; set; }

    }
}