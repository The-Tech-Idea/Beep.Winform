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
            Row = row;
            RowData = rowdata;
        }
        public BeepGridRowSelectedEventArgs(int row)
        {
            Row = row;
        }
        public int Row { get; }
        public BeepGridRow RowData { get; set; }

    }
}