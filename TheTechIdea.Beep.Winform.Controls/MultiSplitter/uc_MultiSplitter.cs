using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;


namespace TheTechIdea.Beep.Winform.Controls.MultiSplitter
{
   
    [AddinAttribute(Caption = "Multi splitter", Name = "uc_MultiSplitter", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup, ObjectType = "Beep")]
    [AddinVisSchema(BranchID =45, RootNodeName = "Configuration", Order = 66, ID = 66, BranchText = "Multi splitter", BranchType = EnumPointType.Function, IconImageName = "fieldtypeconfig.png", BranchClass = "ADDIN", BranchDescription = "Data Sources Connection Drivers Setup Screen")]
    public partial class uc_MultiSplitter : uc_Addin,IAddinVisSchema
    {
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 3;
        public int ID { get; set; } = 3;
        public string BranchText { get; set; } = "Multi splitter";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 3;
        public string IconImageName { get; set; } = "connectiondrivers.ico";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "Multi splitter Screen";
        public string BranchClass { get; set; } = "ADDIN";
        #endregion "IAddinVisSchema"
        public uc_MultiSplitter()
        {
            InitializeComponent();
            tableLayoutPanel.MouseDown += TableLayoutPanel_MouseDown;
            tableLayoutPanel.MouseMove += TableLayoutPanel_MouseMove;
            tableLayoutPanel.MouseUp += TableLayoutPanel_MouseUp;
            tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel.DragEnter += TableLayoutPanel_DragEnter;
            tableLayoutPanel.DragOver += TableLayoutPanel_DragOver;
            tableLayoutPanel.AllowDrop = true;
            contextMenu = new ContextMenuStrip();
            ToolStripMenuItem addRowItem = new ToolStripMenuItem("Add Row", null, AddRowContextItem_Click);
            ToolStripMenuItem removeRowItem = new ToolStripMenuItem("Remove Row", null, RemoveRowContextItem_Click);
            ToolStripMenuItem addColumnItem = new ToolStripMenuItem("Add Column", null, AddColumnContextItem_Click);
            ToolStripMenuItem removeColumnItem = new ToolStripMenuItem("Remove Column", null, RemoveColumnContextItem_Click);

            // Adding menu items to the context menu
            contextMenu.Items.AddRange(new ToolStripItem[] { addRowItem, removeRowItem, addColumnItem, removeColumnItem });

            // Associating the context menu with the TableLayoutPanel
            tableLayoutPanel.ContextMenuStrip = contextMenu;
        }
        public override void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pDMEEditor, plogger, putil, args, e, per);

        }
        ContextMenuStrip contextMenu = new ContextMenuStrip();
        private bool isResizing = false;
        private int rowOrColumnIndex = -1;
        private bool isColumnResize = true;  // True if resizing columns, false if resizing rows
        private float mouseStartY;
        private float mouseStartX;

        #region "Column Routines"
        private void MoveColumn(int sourceIndex, int destinationIndex)
        {
            foreach (Control control in tableLayoutPanel.Controls)
            {
                int columnIndex = tableLayoutPanel.GetColumn(control);
                if (columnIndex == sourceIndex)
                {
                    tableLayoutPanel.SetColumn(control, destinationIndex);
                }
            }
            // Optionally, animate the move or refresh the layout visibly here
        }
        private async void AddRowAnimated()
        {
            int rowIndex = tableLayoutPanel.RowCount;
            tableLayoutPanel.RowCount++;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
            for (int i = 0; i < tableLayoutPanel.ColumnCount; i++)
            {
                var label = new Label() { Text = $"New Cell {rowIndex + 1},{i + 1}", TextAlign = ContentAlignment.MiddleCenter };
                tableLayoutPanel.Controls.Add(label, i, rowIndex);
            }

            // Animation: Gradually increase row height
            for (int height = 0; height <= 30; height += 5)
            {
                tableLayoutPanel.RowStyles[rowIndex].Height = height;
                await Task.Delay(50);  // Delay to simulate animation
            }
        }
        private void AddColumnContextItem_Click(object sender, EventArgs e)
        {
            AddColumn();
        }
        private void RemoveColumnContextItem_Click(object sender, EventArgs e)
        {
            RemoveColumn();
        }
        // Example implementations of AddColumn and RemoveColumn
        private void AddColumn()
        {
            int newColumnIndex = tableLayoutPanel.ColumnCount;
            tableLayoutPanel.ColumnCount++;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 100f ));

            for (int row = 0; row < tableLayoutPanel.RowCount; row++)
            {
                var newControl = new Label() { Text = $"Row {row + 1}, Col {newColumnIndex + 1}" };
                tableLayoutPanel.Controls.Add(newControl, newColumnIndex, row);
            }
        }
        private void RemoveColumn()
        {
            if (tableLayoutPanel.ColumnCount > 1)
            {
                int columnIndex = tableLayoutPanel.ColumnCount - 1; // Last column index

                // Remove all controls in the last column
                for (int row = 0; row < tableLayoutPanel.RowCount; row++)
                {
                    Control control = tableLayoutPanel.GetControlFromPosition(columnIndex, row);
                    tableLayoutPanel.Controls.Remove(control);
                    control.Dispose();
                }

                // Remove the column style and decrement the column count
                tableLayoutPanel.ColumnStyles.RemoveAt(columnIndex);
                tableLayoutPanel.ColumnCount--;
            }
        }
        #endregion
        #region "Row Routines"
        private void AddRowContextItem_Click(object sender, EventArgs e)
        {
            AddRow();
        }
        private void RemoveRowContextItem_Click(object sender, EventArgs e)
        {
            RemoveRow();
        }
        // Example implementations of AddRow and RemoveRow
        private void AddRow()
        {
            tableLayoutPanel.RowCount++;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize, 100f ));
            for (int column = 0; column < tableLayoutPanel.ColumnCount; column++)
            {
                var newControl = new Label() { Text = $"Row {tableLayoutPanel.RowCount}, Col {column + 1}" };
                tableLayoutPanel.Controls.Add(newControl, column, tableLayoutPanel.RowCount - 1);
            }
        }

        private void RemoveRow()
        {
            if (tableLayoutPanel.RowCount > 1)
            {
                int rowIndex = tableLayoutPanel.RowCount - 1; // Last row index

                // Remove all controls in the last row
                for (int column = 0; column < tableLayoutPanel.ColumnCount; column++)
                {
                    Control control = tableLayoutPanel.GetControlFromPosition(column, rowIndex);
                    tableLayoutPanel.Controls.Remove(control);
                    control.Dispose();
                }

                // Remove the row style and decrement the row count
                tableLayoutPanel.RowStyles.RemoveAt(rowIndex);
                tableLayoutPanel.RowCount--;
            }
        }

        #endregion
        #region "Cell Routines"
        #endregion
        #region "TableLayoutPanel Routines"
        private const int ResizeTolerance = 15;  // Pixel tolerance for resizing area
        private void TableLayoutPanel_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = tableLayoutPanel.PointToClient(new Point(e.X, e.Y));
            int tolerance = 10;
            rowOrColumnIndex = GetRowOrColumnIndexNearMouse(e.Location, tolerance, out isColumnResize);

            if (rowOrColumnIndex != -1)
            {
                isResizing = true;
                mouseStartY = e.Y;
                mouseStartX = e.X;
                tableLayoutPanel.Capture = true;  // Ensure the control captures all mouse events
            }
        }
        private int GetRowOrColumnIndexNearMouse(Point location, int tolerance, out bool isColumn)
        {
            isColumn = true;
            int sumWidth = 0;
            for (int col = 0; col < tableLayoutPanel.ColumnCount; col++)
            {
                sumWidth += tableLayoutPanel.GetColumnWidths()[col];
                if (Math.Abs(sumWidth - location.X) <= tolerance)
                {
                    return col; // Correctly identifies the column
                }
            }

            int sumHeight = 0;
            isColumn = false;
            for (int row = 0; row < tableLayoutPanel.RowCount; row++)
            {
                sumHeight += tableLayoutPanel.GetRowHeights()[row];
                if (Math.Abs(sumHeight - location.Y) <= tolerance)
                {
                    return row; // Correctly identifies the row
                }
            }

            return -1; // Not near any boundary
        }

        private void TableLayoutPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing)
            {
                if (isColumnResize)
                {
                    float delta = (e.X - mouseStartX) * 0.5f;  // Scale the delta to reduce sensitivity
                    float newWidth = tableLayoutPanel.ColumnStyles[rowOrColumnIndex].Width + delta;
                    tableLayoutPanel.ColumnStyles[rowOrColumnIndex].Width = Math.Max(10, newWidth);
                    mouseStartX = e.X;
                }
                else
                {
                    float delta = (e.Y - mouseStartY) * 0.5f;  // Scale the delta to reduce sensitivity
                    float newHeight = tableLayoutPanel.RowStyles[rowOrColumnIndex].Height + delta;
                    tableLayoutPanel.RowStyles[rowOrColumnIndex].Height = Math.Max(10, newHeight);
                    mouseStartY = e.Y;
                }
            }
        }

        private void TableLayoutPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (isResizing)
            {
                isResizing = false;
                rowOrColumnIndex = -1;
               
            }
        }
        #endregion
        #region "Drag and Drop"
        private void TableLayoutPanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void TableLayoutPanel_DragOver(object sender, DragEventArgs e)
        {
            Point point = tableLayoutPanel.PointToClient(new Point(e.X, e.Y));
            var hoveredCell = GetCellFromPoint(point);
            HighlightCell(hoveredCell);
        }
        #endregion
        private void HighlightCell(TableLayoutPanelCellPosition cellPosition)
        {
            foreach (Control control in tableLayoutPanel.Controls)
            {
                var pos = tableLayoutPanel.GetPositionFromControl(control);
                if (pos.Equals(cellPosition))
                {
                    control.BackColor = Color.CadetBlue; // Highlight color
                }
                else
                {
                    control.BackColor = Color.White; // Normal color
                }
            }
        }
        private TableLayoutPanelCellPosition GetCellFromPoint(Point point)
        {
            int row = -1;
            int column = -1;
            int cumulativeHeight = 0;
            int cumulativeWidth = 0;

            // Calculate which row the point is in
            for (int i = 0; i < tableLayoutPanel.RowCount; i++)
            {
                cumulativeHeight += tableLayoutPanel.GetRowHeights()[i];
                if (point.Y <= cumulativeHeight)
                {
                    row = i;
                    break;
                }
            }

            // Calculate which column the point is in
            for (int i = 0; i < tableLayoutPanel.ColumnCount; i++)
            {
                cumulativeWidth += tableLayoutPanel.GetColumnWidths()[i];
                if (point.X <= cumulativeWidth)
                {
                    column = i;
                    break;
                }
            }

            return new TableLayoutPanelCellPosition(column, row);
        }
        #region "Load and Save"
    
        #endregion


    }
}
