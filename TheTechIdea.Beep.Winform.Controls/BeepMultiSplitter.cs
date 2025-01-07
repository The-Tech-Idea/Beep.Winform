using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Description("Beep Multi Splitter")]
    [ToolboxBitmap(typeof(BeepMultiSplitter), "BeepMultiSplitter.bmp")]
    [DisplayName("Beep Multi Splitter")]
    public partial class BeepMultiSplitter : BeepControl
    {
        // Constants for user interactions
        private const int RESIZE_TOLERANCE = 10; // Pixel threshold near edges for resizing

        // Fields for resizing logic
        private bool isResizing = false;
        private int rowOrColumnIndex = -1;
        private bool isColumnResize = true;  // True if resizing columns, false if resizing rows
        private float mouseStartX;
        private float mouseStartY;

        // The TableLayoutPanel that we manage
        public TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();

        // Context menu for dynamic manipulation
        private ContextMenuStrip contextMenu = new ContextMenuStrip();

        public BeepMultiSplitter()
        {
            // Basic control config
            DoubleBuffered = true;          // Reduce flicker
            tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel.AllowDrop = true;
            tableLayoutPanel.DragEnter += TableLayoutPanel_DragEnter;
            tableLayoutPanel.DragOver += TableLayoutPanel_DragOver;

            // Mouse event handlers for resizing
            tableLayoutPanel.MouseDown += TableLayoutPanel_MouseDown;
            tableLayoutPanel.MouseMove += TableLayoutPanel_MouseMove;
            tableLayoutPanel.MouseUp += TableLayoutPanel_MouseUp;
            // 
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.28877F));
            tableLayoutPanel.Size = new Size(1102, 748);
            tableLayoutPanel.TabIndex = 0;

            // set control size and anchor using DrawingRect to fit table layout panel to fill the control
            Size = new Size(1102, 748);
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UpdateDrawingRect();
            tableLayoutPanel.Left = DrawingRect.Left;
            tableLayoutPanel.Top = DrawingRect.Top;
            tableLayoutPanel.Size = new Size(DrawingRect.Width, DrawingRect.Height);
            

            // Initialize context menu with row/column management
            InitializeContextMenu();

            
            // Associate context menu with the TableLayoutPanel
            tableLayoutPanel.ContextMenuStrip = contextMenu;

            // Add the TableLayoutPanel as a child of this BeepControl
            Controls.Add(tableLayoutPanel);
        }

        #region "Context Menu Setup & Handlers"

        private void InitializeContextMenu()
        {
            ToolStripMenuItem addRowItem = new ToolStripMenuItem("Add Row", null, AddRowContextItem_Click);
            ToolStripMenuItem removeRowItem = new ToolStripMenuItem("Remove Row", null, RemoveRowContextItem_Click);
            ToolStripMenuItem addColumnItem = new ToolStripMenuItem("Add Column", null, AddColumnContextItem_Click);
            ToolStripMenuItem removeColumnItem = new ToolStripMenuItem("Remove Column", null, RemoveColumnContextItem_Click);

            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                addRowItem,
                removeRowItem,
                addColumnItem,
                removeColumnItem
            });
        }

        private void AddRowContextItem_Click(object sender, EventArgs e)
            => AddRow();

        private void RemoveRowContextItem_Click(object sender, EventArgs e)
            => RemoveRow();

        private void AddColumnContextItem_Click(object sender, EventArgs e)
            => AddColumn();

        private void RemoveColumnContextItem_Click(object sender, EventArgs e)
            => RemoveColumn();

        #endregion

        #region "Row Routines"

        // Adds a new row with default properties
        private void AddRow()
        {
            tableLayoutPanel.RowCount++;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize, 100f));

            // Populate the new row with placeholder controls (optional)
            for (int column = 0; column < tableLayoutPanel.ColumnCount; column++)
            {
                var newControl = new Label()
                {
                    Text = $"Row {tableLayoutPanel.RowCount}, Col {column + 1}",
                    TextAlign = ContentAlignment.MiddleCenter
                };
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
                    if (control != null)
                    {
                        tableLayoutPanel.Controls.Remove(control);
                        control.Dispose();
                    }
                }

                // Remove the row style and decrement the row count
                tableLayoutPanel.RowStyles.RemoveAt(rowIndex);
                tableLayoutPanel.RowCount--;
            }
        }

        // Example of an animated approach
        private async void AddRowAnimated()
        {
            int rowIndex = tableLayoutPanel.RowCount;
            tableLayoutPanel.RowCount++;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));

            // Add placeholder controls
            for (int i = 0; i < tableLayoutPanel.ColumnCount; i++)
            {
                var label = new Label()
                {
                    Text = $"New Cell {rowIndex + 1},{i + 1}",
                    TextAlign = ContentAlignment.MiddleCenter
                };
                tableLayoutPanel.Controls.Add(label, i, rowIndex);
            }

            // Animation: Gradually increase row height from 0 to 30
            for (int height = 0; height <= 30; height += 5)
            {
                tableLayoutPanel.RowStyles[rowIndex].Height = height;
                await Task.Delay(50); // short delay for each step
            }
        }

        #endregion

        #region "Column Routines"

        // Adds a new column with default properties
        private void AddColumn()
        {
            int newColumnIndex = tableLayoutPanel.ColumnCount;
            tableLayoutPanel.ColumnCount++;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 100f));

            // Populate the new column with placeholder controls (optional)
            for (int row = 0; row < tableLayoutPanel.RowCount; row++)
            {
                var newControl = new Label()
                {
                    Text = $"Row {row + 1}, Col {newColumnIndex + 1}",
                    TextAlign = ContentAlignment.MiddleCenter
                };
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
                    if (control != null)
                    {
                        tableLayoutPanel.Controls.Remove(control);
                        control.Dispose();
                    }
                }

                // Remove the column style and decrement the column count
                tableLayoutPanel.ColumnStyles.RemoveAt(columnIndex);
                tableLayoutPanel.ColumnCount--;
            }
        }

        // Moves a column from sourceIndex to destinationIndex (example usage)
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
            // Optionally, refresh the layout or animate
        }

        #endregion

        #region "TableLayoutPanel Resizing Logic"

        private void TableLayoutPanel_MouseDown(object sender, MouseEventArgs e)
        {
            // Convert to local coordinates relative to the TableLayoutPanel
            Point localPoint = tableLayoutPanel.PointToClient(e.Location);

            // Identify if user is near a row or column border
            rowOrColumnIndex = GetRowOrColumnIndexNearMouse(localPoint, RESIZE_TOLERANCE, out isColumnResize);

            if (rowOrColumnIndex != -1)
            {
                isResizing = true;
                mouseStartX = e.X;
                mouseStartY = e.Y;
                tableLayoutPanel.Capture = true; // ensures we get mouse events even if it moves out
            }
        }

        private void TableLayoutPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing && rowOrColumnIndex != -1)
            {
                // We scale the delta by 0.5f to reduce sensitivity
                if (isColumnResize)
                {
                    float deltaX = (e.X - mouseStartX) * 0.5f;
                    float newWidth = tableLayoutPanel.ColumnStyles[rowOrColumnIndex].Width + deltaX;
                    tableLayoutPanel.ColumnStyles[rowOrColumnIndex].Width = Math.Max(10, newWidth);
                    mouseStartX = e.X;
                }
                else
                {
                    float deltaY = (e.Y - mouseStartY) * 0.5f;
                    float newHeight = tableLayoutPanel.RowStyles[rowOrColumnIndex].Height + deltaY;
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
                tableLayoutPanel.Capture = false;
            }
        }

        private int GetRowOrColumnIndexNearMouse(Point location, int tolerance, out bool isColumn)
        {
            // columns first
            isColumn = true;
            int accumulatedWidth = 0;
            var widths = tableLayoutPanel.GetColumnWidths();
            for (int col = 0; col < tableLayoutPanel.ColumnCount; col++)
            {
                accumulatedWidth += widths[col];
                if (Math.Abs(accumulatedWidth - location.X) <= tolerance)
                {
                    return col;
                }
            }

            // if not near any column border, check row borders
            isColumn = false;
            int accumulatedHeight = 0;
            var heights = tableLayoutPanel.GetRowHeights();
            for (int row = 0; row < tableLayoutPanel.RowCount; row++)
            {
                accumulatedHeight += heights[row];
                if (Math.Abs(accumulatedHeight - location.Y) <= tolerance)
                {
                    return row;
                }
            }

            return -1;
        }

        #endregion

        #region "Drag & Drop"

        private void TableLayoutPanel_DragEnter(object sender, DragEventArgs e)
        {
            // Example: always show move effect
            e.Effect = DragDropEffects.Move;
        }

        private void TableLayoutPanel_DragOver(object sender, DragEventArgs e)
        {
            Point tablePanelPt = tableLayoutPanel.PointToClient(new Point(e.X, e.Y));
            TableLayoutPanelCellPosition cellPos = GetCellFromPoint(tablePanelPt);
            HighlightCell(cellPos);
        }

        /// <summary>
        /// Highlights the cell at the given position, restoring others to default.
        /// </summary>
        private void HighlightCell(TableLayoutPanelCellPosition cellPosition)
        {
            // Loop through controls, highlighting the one at cellPosition
            foreach (Control control in tableLayoutPanel.Controls)
            {
                TableLayoutPanelCellPosition pos = tableLayoutPanel.GetPositionFromControl(control);
                if (pos.Equals(cellPosition))
                {
                    control.BackColor = Color.CadetBlue;
                }
                else
                {
                    control.BackColor = Color.White;
                }
            }
        }

        #endregion

        #region "Utility Methods"

        /// <summary>
        /// Given a point in local coordinates, identifies which cell is under the pointer.
        /// </summary>
        private TableLayoutPanelCellPosition GetCellFromPoint(Point point)
        {
            int row = -1;
            int col = -1;

            int cumulativeHeight = 0;
            for (int i = 0; i < tableLayoutPanel.RowCount; i++)
            {
                cumulativeHeight += tableLayoutPanel.GetRowHeights()[i];
                if (point.Y <= cumulativeHeight)
                {
                    row = i;
                    break;
                }
            }

            int cumulativeWidth = 0;
            for (int i = 0; i < tableLayoutPanel.ColumnCount; i++)
            {
                cumulativeWidth += tableLayoutPanel.GetColumnWidths()[i];
                if (point.X <= cumulativeWidth)
                {
                    col = i;
                    break;
                }
            }
            return new TableLayoutPanelCellPosition(col, row);
        }

        #endregion
    }
}
