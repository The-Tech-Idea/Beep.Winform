using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Description("Beep Multi Splitter - A resizable TableLayoutPanel with design-time support")]
    [ToolboxBitmap(typeof(BeepMultiSplitter), "BeepMultiSplitter.bmp")]
    [DisplayName("Beep Multi Splitter")]
    [Designer(typeof(BeepMultiSplitterDesigner))]
    [DefaultProperty("TableLayoutPanel")]
    public partial class BeepMultiSplitter : BaseControl
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
        private TableLayoutPanel _tableLayoutPanel = new TableLayoutPanel();

        /// <summary>
        /// Gets or sets the underlying TableLayoutPanel. Use this to add controls and configure layout at design time.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The TableLayoutPanel that manages the layout. Add controls to this panel at design time.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableLayoutPanel TableLayoutPanel
        {
            get => _tableLayoutPanel;
            set
            {
                if (_tableLayoutPanel != value)
                {
                    if (_tableLayoutPanel != null)
                    {
                        // Remove old event handlers
                        _tableLayoutPanel.MouseDown -= TableLayoutPanel_MouseDown;
                        _tableLayoutPanel.MouseMove -= TableLayoutPanel_MouseMove;
                        _tableLayoutPanel.MouseUp -= TableLayoutPanel_MouseUp;
                        _tableLayoutPanel.DragEnter -= TableLayoutPanel_DragEnter;
                        _tableLayoutPanel.DragOver -= TableLayoutPanel_DragOver;
                        Controls.Remove(_tableLayoutPanel);
                    }

                    _tableLayoutPanel = value;

                    if (_tableLayoutPanel != null)
                    {
                        // Add new event handlers
                        _tableLayoutPanel.MouseDown += TableLayoutPanel_MouseDown;
                        _tableLayoutPanel.MouseMove += TableLayoutPanel_MouseMove;
                        _tableLayoutPanel.MouseUp += TableLayoutPanel_MouseUp;
                        _tableLayoutPanel.DragEnter += TableLayoutPanel_DragEnter;
                        _tableLayoutPanel.DragOver += TableLayoutPanel_DragOver;
                        _tableLayoutPanel.ContextMenuStrip = contextMenu;
                        _tableLayoutPanel.Dock = DockStyle.Fill;
                        Controls.Add(_tableLayoutPanel);
                    }
                }
            }
        }

        // Context menu for dynamic manipulation
        private ContextMenuStrip contextMenu = new ContextMenuStrip();
        public object tableLayoutPanel;

        public BeepMultiSplitter()
        {
            // Basic control config
            DoubleBuffered = true;          // Reduce flicker
            
            // Initialize TableLayoutPanel
            _tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            _tableLayoutPanel.AllowDrop = true;
            _tableLayoutPanel.DragEnter += TableLayoutPanel_DragEnter;
            _tableLayoutPanel.DragOver += TableLayoutPanel_DragOver;

            // Mouse event handlers for resizing (only at runtime)
            _tableLayoutPanel.MouseDown += TableLayoutPanel_MouseDown;
            _tableLayoutPanel.MouseMove += TableLayoutPanel_MouseMove;
            _tableLayoutPanel.MouseUp += TableLayoutPanel_MouseUp;
            
            // Default layout
            _tableLayoutPanel.ColumnCount = 1;
            _tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _tableLayoutPanel.Location = new Point(0, 0);
            _tableLayoutPanel.Name = "tableLayoutPanel";
            _tableLayoutPanel.RowCount = 1;
            _tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.28877F));
            _tableLayoutPanel.Size = new Size(1102, 748);
            _tableLayoutPanel.TabIndex = 0;

            // set control size and anchor using DrawingRect to fit table layout panel to fill the control
            Size = new Size(1102, 748);
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UpdateDrawingRect();
            _tableLayoutPanel.Left = DrawingRect.Left;
            _tableLayoutPanel.Top = DrawingRect.Top;
            _tableLayoutPanel.Size = new Size(DrawingRect.Width, DrawingRect.Height);
            

            // Initialize context menu with row/column management
            InitializeContextMenu();

            
            // Associate context menu with the TableLayoutPanel (only at runtime)
            if (!DesignMode)
            {
                _tableLayoutPanel.ContextMenuStrip = contextMenu;
            }

            // Add the TableLayoutPanel as a child of this BeepControl
            Controls.Add(_tableLayoutPanel);
            _tableLayoutPanel.Dock = DockStyle.Fill;
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
            _tableLayoutPanel.RowCount++;
            _tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize, 100f));

            // Populate the new row with placeholder controls (optional)
            for (int column = 0; column < _tableLayoutPanel.ColumnCount; column++)
            {
                var newControl = new Label()
                {
                    Text = $"Row {_tableLayoutPanel.RowCount}, Col {column + 1}",
                    TextAlign = ContentAlignment.MiddleCenter
                };
                _tableLayoutPanel.Controls.Add(newControl, column, _tableLayoutPanel.RowCount - 1);
            }
        }

        private void RemoveRow()
        {
            if (_tableLayoutPanel.RowCount > 1)
            {
                int rowIndex = _tableLayoutPanel.RowCount - 1; // Last row index

                // Remove all controls in the last row
                for (int column = 0; column < _tableLayoutPanel.ColumnCount; column++)
                {
                    Control control = _tableLayoutPanel.GetControlFromPosition(column, rowIndex);
                    if (control != null)
                    {
                        _tableLayoutPanel.Controls.Remove(control);
                        control.Dispose();
                    }
                }

                // Remove the row Style and decrement the row count
                _tableLayoutPanel.RowStyles.RemoveAt(rowIndex);
                _tableLayoutPanel.RowCount--;
            }
        }

        // Example of an animated approach
        private async void AddRowAnimated()
        {
            int rowIndex = _tableLayoutPanel.RowCount;
            _tableLayoutPanel.RowCount++;
            _tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));

            // Add placeholder controls
            for (int i = 0; i < _tableLayoutPanel.ColumnCount; i++)
            {
                var label = new Label()
                {
                    Text = $"New Cell {rowIndex + 1},{i + 1}",
                    TextAlign = ContentAlignment.MiddleCenter
                };
                _tableLayoutPanel.Controls.Add(label, i, rowIndex);
            }

            // Animation: Gradually increase row height from 0 to 30
            for (int height = 0; height <= 30; height += 5)
            {
                _tableLayoutPanel.RowStyles[rowIndex].Height = height;
                await Task.Delay(50); // short delay for each step
            }
        }

        #endregion

        #region "Column Routines"

        // Adds a new column with default properties
        private void AddColumn()
        {
            int newColumnIndex = _tableLayoutPanel.ColumnCount;
            _tableLayoutPanel.ColumnCount++;
            _tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 100f));

            // Populate the new column with placeholder controls (optional)
            for (int row = 0; row < _tableLayoutPanel.RowCount; row++)
            {
                var newControl = new Label()
                {
                    Text = $"Row {row + 1}, Col {newColumnIndex + 1}",
                    TextAlign = ContentAlignment.MiddleCenter
                };
                _tableLayoutPanel.Controls.Add(newControl, newColumnIndex, row);
            }
        }

        private void RemoveColumn()
        {
            if (_tableLayoutPanel.ColumnCount > 1)
            {
                int columnIndex = _tableLayoutPanel.ColumnCount - 1; // Last column index

                // Remove all controls in the last column
                for (int row = 0; row < _tableLayoutPanel.RowCount; row++)
                {
                    Control control = _tableLayoutPanel.GetControlFromPosition(columnIndex, row);
                    if (control != null)
                    {
                        _tableLayoutPanel.Controls.Remove(control);
                        control.Dispose();
                    }
                }

                // Remove the column Style and decrement the column count
                _tableLayoutPanel.ColumnStyles.RemoveAt(columnIndex);
                _tableLayoutPanel.ColumnCount--;
            }
        }

        // Moves a column from sourceIndex to destinationIndex (example usage)
        private void MoveColumn(int sourceIndex, int destinationIndex)
        {
            foreach (Control control in _tableLayoutPanel.Controls)
            {
                int columnIndex = _tableLayoutPanel.GetColumn(control);
                if (columnIndex == sourceIndex)
                {
                    _tableLayoutPanel.SetColumn(control, destinationIndex);
                }
            }
            // Optionally, refresh the layout or animate
        }

        #endregion

        #region "TableLayoutPanel Resizing Logic"

        private void TableLayoutPanel_MouseDown(object sender, MouseEventArgs e)
        {
            // Don't handle resizing at design time
            if (DesignMode) return;

            // Convert to local coordinates relative to the TableLayoutPanel
            Point localPoint = _tableLayoutPanel.PointToClient(e.Location);

            // Identify if user is near a row or column border
            rowOrColumnIndex = GetRowOrColumnIndexNearMouse(localPoint, RESIZE_TOLERANCE, out isColumnResize);

            if (rowOrColumnIndex != -1)
            {
                isResizing = true;
                mouseStartX = e.X;
                mouseStartY = e.Y;
                _tableLayoutPanel.Capture = true; // ensures we get mouse events even if it moves out
            }
        }

        private void TableLayoutPanel_MouseMove(object sender, MouseEventArgs e)
        {
            // Don't handle resizing at design time
            if (DesignMode) return;

            if (isResizing && rowOrColumnIndex != -1)
            {
                // We scale the delta by 0.5f to reduce sensitivity
                if (isColumnResize)
                {
                    float deltaX = (e.X - mouseStartX) * 0.5f;
                    float newWidth = _tableLayoutPanel.ColumnStyles[rowOrColumnIndex].Width + deltaX;
                    _tableLayoutPanel.ColumnStyles[rowOrColumnIndex].Width = Math.Max(10, newWidth);
                    mouseStartX = e.X;
                }
                else
                {
                    float deltaY = (e.Y - mouseStartY) * 0.5f;
                    float newHeight = _tableLayoutPanel.RowStyles[rowOrColumnIndex].Height + deltaY;
                    _tableLayoutPanel.RowStyles[rowOrColumnIndex].Height = Math.Max(10, newHeight);
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
                _tableLayoutPanel.Capture = false;
            }
        }

        private int GetRowOrColumnIndexNearMouse(Point location, int tolerance, out bool isColumn)
        {
            // columns first
            isColumn = true;
            int accumulatedWidth = 0;
            var widths = _tableLayoutPanel.GetColumnWidths();
            for (int col = 0; col < _tableLayoutPanel.ColumnCount; col++)
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
            var heights = _tableLayoutPanel.GetRowHeights();
            for (int row = 0; row < _tableLayoutPanel.RowCount; row++)
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
            if (DesignMode) return;

            Point tablePanelPt = _tableLayoutPanel.PointToClient(new Point(e.X, e.Y));
            TableLayoutPanelCellPosition cellPos = GetCellFromPoint(tablePanelPt);
            HighlightCell(cellPos);
        }

        /// <summary>
        /// Highlights the cell at the given position, restoring others to default.
        /// </summary>
        private void HighlightCell(TableLayoutPanelCellPosition cellPosition)
        {
            // Loop through controls, highlighting the one at cellPosition
            foreach (Control control in _tableLayoutPanel.Controls)
            {
                TableLayoutPanelCellPosition pos = _tableLayoutPanel.GetPositionFromControl(control);
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
            for (int i = 0; i < _tableLayoutPanel.RowCount; i++)
            {
                cumulativeHeight += _tableLayoutPanel.GetRowHeights()[i];
                if (point.Y <= cumulativeHeight)
                {
                    row = i;
                    break;
                }
            }

            int cumulativeWidth = 0;
            for (int i = 0; i < _tableLayoutPanel.ColumnCount; i++)
            {
                cumulativeWidth += _tableLayoutPanel.GetColumnWidths()[i];
                if (point.X <= cumulativeWidth)
                {
                    col = i;
                    break;
                }
            }
            return new TableLayoutPanelCellPosition(col, row);
        }

        #endregion
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDrawingRect();
            _tableLayoutPanel.Left = DrawingRect.Left;
            _tableLayoutPanel.Top = DrawingRect.Top;
            _tableLayoutPanel.Size = new Size(DrawingRect.Width, DrawingRect.Height);
        }

        /// <summary>
        /// Gets or sets the number of columns in the table layout.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The number of columns in the table layout.")]
        [DefaultValue(1)]
        public int ColumnCount
        {
            get => _tableLayoutPanel.ColumnCount;
            set => _tableLayoutPanel.ColumnCount = value;
        }

        /// <summary>
        /// Gets or sets the number of rows in the table layout.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The number of rows in the table layout.")]
        [DefaultValue(1)]
        public int RowCount
        {
            get => _tableLayoutPanel.RowCount;
            set => _tableLayoutPanel.RowCount = value;
        }

        /// <summary>
        /// Gets the collection of column styles for the table layout.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableLayoutColumnStyleCollection ColumnStyles => _tableLayoutPanel.ColumnStyles;

        /// <summary>
        /// Gets the collection of row styles for the table layout.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableLayoutRowStyleCollection RowStyles => _tableLayoutPanel.RowStyles;

        /// <summary>
        /// Gets or sets the cell border style.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The style of the cell borders.")]
        [DefaultValue(TableLayoutPanelCellBorderStyle.Single)]
        public TableLayoutPanelCellBorderStyle CellBorderStyle
        {
            get => _tableLayoutPanel.CellBorderStyle;
            set => _tableLayoutPanel.CellBorderStyle = value;
        }
    }
}
