using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Grid;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepGridRowPainter
    {
        #region "Constructors"
        public BeepGridRowPainter()
        {
        }
        public BeepGridRowPainter(BeepSimpleGrid grid)
        {
            Grid = grid;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid, Rectangle drawingRect)
        {
            Grid = grid;
            DrawingRect = drawingRect;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid, Rectangle drawingRect, int rowHeight)
        {
            Grid = grid;
            DrawingRect = drawingRect;
            RowHeight = rowHeight;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid, Rectangle drawingRect, int rowHeight, int gridHeight)
        {
            Grid = grid;
            DrawingRect = drawingRect;
            RowHeight = rowHeight;
            GridHeight = gridHeight;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid, Rectangle drawingRect, int rowHeight, int gridHeight, int gridWidth)
        {
            Grid = grid;
            DrawingRect = drawingRect;
            RowHeight = rowHeight;
            GridHeight = gridHeight;
            GridWidth = gridWidth;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid, Rectangle drawingRect, int rowHeight, int gridHeight, int gridWidth, int xOffset)
        {
            Grid = grid;
            DrawingRect = drawingRect;
            RowHeight = rowHeight;
            GridHeight = gridHeight;
            GridWidth = gridWidth;
            XOffset = xOffset;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid, Rectangle drawingRect, int rowHeight, int gridHeight, int gridWidth, int xOffset, int yOffset)
        {
            Grid = grid;
            DrawingRect = drawingRect;
            RowHeight = rowHeight;
            GridHeight = gridHeight;
            GridWidth = gridWidth;
            XOffset = xOffset;
            YOffset = yOffset;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid, Rectangle drawingRect, int rowHeight, int gridHeight, int gridWidth, int xOffset, int yOffset, BindingList<BeepGridRow> rows)
        {
            Grid = grid;
            DrawingRect = drawingRect;
            RowHeight = rowHeight;
            GridHeight = gridHeight;
            GridWidth = gridWidth;
            XOffset = xOffset;
            YOffset = yOffset;
            Rows = rows;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid, Rectangle drawingRect, int rowHeight, int gridHeight, int gridWidth, int xOffset, int yOffset, BindingList<BeepGridRow> rows, Object dataSource)
        {
            Grid = grid;
            DrawingRect = drawingRect;
            RowHeight = rowHeight;
            GridHeight = gridHeight;
            GridWidth = gridWidth;
            XOffset = xOffset;
            YOffset = yOffset;
            Rows = rows;
            DataSource = dataSource;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid, Rectangle drawingRect, int rowHeight, int gridHeight, int gridWidth, int xOffset, int yOffset, BindingList<BeepGridRow> rows, Object dataSource, List<BeepGridColumnConfig> columns)
        {
            Grid = grid;
            DrawingRect = drawingRect;
            RowHeight = rowHeight;
            GridHeight = gridHeight;
            GridWidth = gridWidth;
            XOffset = xOffset;
            YOffset = yOffset;
            Rows = rows;
            DataSource = dataSource;
         
        }
        public BeepGridRowPainter(BeepSimpleGrid grid , object dataSource)
        {
            Grid = grid;
         
            DataSource = dataSource;

        }
        public BeepGridRowPainter(BeepSimpleGrid grid,  object dataSource, Rectangle drawingRect)
        {
            Grid = grid;
        
            DataSource = dataSource;
            DrawingRect = drawingRect;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid,  object dataSource, Rectangle drawingRect, int rowHeight)
        {
            Grid = grid;
         
            DataSource = dataSource;
            DrawingRect = drawingRect;
            RowHeight = rowHeight;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid, object dataSource, Rectangle drawingRect, int rowHeight, int gridHeight)
        {
            Grid = grid;
          
            DataSource = dataSource;
            DrawingRect = drawingRect;
            RowHeight = rowHeight;
            GridHeight = gridHeight;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid,  object dataSource, Rectangle drawingRect, int rowHeight, int gridHeight, int gridWidth)
        {
            Grid = grid;
          
            DataSource = dataSource;
            DrawingRect = drawingRect;
            RowHeight = rowHeight;
            GridHeight = gridHeight;
            GridWidth = gridWidth;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid,  object dataSource, Rectangle drawingRect, int rowHeight, int gridHeight, int gridWidth, int xOffset)
        {
            Grid = grid;
         
            DataSource = dataSource;
            DrawingRect = drawingRect;
            RowHeight = rowHeight;
            GridHeight = gridHeight;
            GridWidth = gridWidth;
            XOffset = xOffset;
        }
        public BeepGridRowPainter(BeepSimpleGrid grid,  object dataSource, Rectangle drawingRect, int rowHeight, int gridHeight, int gridWidth, int xOffset, int yOffset)
        {
            Grid = grid;
           
            DataSource = dataSource;
            DrawingRect = drawingRect;
            RowHeight = rowHeight;
            GridHeight = gridHeight;
            GridWidth = gridWidth;
            XOffset = xOffset;
            YOffset = yOffset;
        }
        #endregion "Constructors"
        #region "Propoerties needed for the grid"
        public Object DataSource { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
        public int RowHeight { get; set; } = 30;
        public int GridHeight { get; set; }
        public int GridWidth { get; set; }
        public BeepSimpleGrid Grid { get; }
        public Rectangle DrawingRect { get; set; }
        #endregion "Propoerties needed for the grid"
        public List<BeepGridColumnConfig> Columns
        {
            get => Grid.Columns;
            
        }
        private Dictionary<string, Control> controlPool = new(); // Pool of controls by Cell ID
        public BindingList<BeepGridRow> Rows { get; set; } = new BindingList<BeepGridRow>();
        public BeepGridRow CurrentRow { get; set; }
        public BeepGridCell CurrentCell { get; set; }
        public BeepGridCell CurrentCellInEdit { get; set; }
        public BeepGridRow CurrentRowInEdit { get; set; }
        #region "Events Delegates"
        // Row Events
        public event EventHandler<BeepGridRowEventArgs> OnRowSelected;
        public event EventHandler<BeepGridRowEventArgs> OnRowValidate;
        public event EventHandler<BeepGridRowEventArgs> OnRowDelete;
        public event EventHandler<BeepGridRowEventArgs> OnRowAdded;
        public event EventHandler<BeepGridRowEventArgs> OnRowUpdate;
        //Cell Events
        public event EventHandler<BeepGridCellEventArgs> OnCellSelected;
        public event EventHandler<BeepGridCellEventArgs> OnCellValidate;
        #endregion "Events Delegates"
        #region "Virtualization"
        private int _verticalScrollOffset = 0; // Track vertical scroll
        private int _horizontalScrollOffset = 0; // Track horizontal scroll
        private List<BeepGridColumnConfig> _columns;
        private (int firstRowIndex, int lastRowIndex) GetVisibleRows()
        {
            int firstRowIndex = Math.Max(0, _verticalScrollOffset / RowHeight);
            int visibleRowCount = (int)Math.Ceiling((double)GridHeight / RowHeight);
            int lastRowIndex = Math.Min(firstRowIndex + visibleRowCount, Rows.Count - 1);
            return (firstRowIndex, lastRowIndex);
        }
        private (int firstColumnIndex, int lastColumnIndex) GetVisibleColumns()
        {
            int xOffset = _horizontalScrollOffset;
            int firstColumnIndex = 0;
            int visibleWidth = DrawingRect.Width;

            for (int i = 0; i < Columns.Count; i++)
            {
                if (xOffset < Columns[i].Width)
                {
                    firstColumnIndex = i;
                    break;
                }
                xOffset -= Columns[i].Width;
            }

            int lastColumnIndex = firstColumnIndex;
            int usedWidth = xOffset;
            for (int i = firstColumnIndex; i < Columns.Count; i++)
            {
                usedWidth += Columns[i].Width;
                if (usedWidth >= visibleWidth)
                {
                    lastColumnIndex = i;
                    break;
                }
            }

            return (firstColumnIndex, lastColumnIndex);
        }

        #endregion "Virtualization"
        #region "Painting"
        public void Paint(Graphics g)
        {
            if (Rows.Count == 0)
            {
                return;
            }
            // Get the visible columns
            var visibleColumns = GetVisibleColumns();
            // Draw the rows
            int yOffset = 0;
            for (int i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];
                if (yOffset + row.Height < DrawingRect.Top)
                {
                    yOffset += row.Height;
                    continue;
                }
                if (yOffset > DrawingRect.Bottom)
                {
                    break;
                }
                PaintRow(g, row, yOffset, visibleColumns.firstColumnIndex, visibleColumns.lastColumnIndex);
                yOffset += row.Height;
            }
        }
        private void PaintRow(Graphics g, BeepGridRow row, int yOffset, int firstColumnIndex, int lastColumnIndex)
        {
            int xOffset = 0;
            for (int i = firstColumnIndex; i <= lastColumnIndex; i++)
            {
                var cell = row.Cells[i];
                if (xOffset + cell.Width < DrawingRect.Left)
                {
                    xOffset += cell.Width;
                    continue;
                }
                if (xOffset > DrawingRect.Right)
                {
                    break;
                }
                PaintCell(g, cell, xOffset, yOffset);
                xOffset += cell.Width;
            }
        }
        private void PaintCell(Graphics g, BeepGridCell cell, int xOffset, int yOffset)
        {
            // Draw the cell background
            g.FillRectangle(Brushes.White, new Rectangle(xOffset, yOffset, cell.Width, cell.RowIdx));
            // Draw the cell border
            g.DrawRectangle(Pens.Black, new Rectangle(xOffset, yOffset, cell.Width, cell.RowIdx));
            // Draw the cell text
            g.DrawString(cell.UIComponent.Text, SystemFonts.DefaultFont, Brushes.Black, new PointF(xOffset + 2, yOffset + 2));
        }
        public void Paint(FlowLayoutPanel flyout)
        {
            if (Rows.Count == 0)
            {
                return;
            }
            // Get the visible columns
            var visibleColumns = GetVisibleColumns();
            // Draw the rows
            int yOffset = 0;
            for (int i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];
                if (yOffset + row.Height < DrawingRect.Top)
                {
                    yOffset += row.Height;
                    continue;
                }
                if (yOffset > DrawingRect.Bottom)
                {
                    break;
                }
                PaintRow(flyout, row, yOffset, visibleColumns.firstColumnIndex, visibleColumns.lastColumnIndex);
                yOffset += row.Height;
            }
        }
        public void PaintRow(FlowLayoutPanel flyout, BeepGridRow row, int yOffset, int firstColumnIndex, int lastColumnIndex)
        {
            int xOffset = 0;
            for (int i = firstColumnIndex; i <= lastColumnIndex; i++)
            {
                var cell = row.Cells[i];
                if (xOffset + cell.Width < DrawingRect.Left)
                {
                    xOffset += cell.Width;
                    continue;
                }
                if (xOffset > DrawingRect.Right)
                {
                    break;
                }
                PaintCell(flyout, cell, xOffset, yOffset);
                xOffset += cell.Width;
            }
        }
        public void PaintCell(FlowLayoutPanel flyout, BeepGridCell cell, int xOffset, int yOffset)
        {
            // Draw the cell background
            // Draw the cell border
            // Draw the cell text
            Label lbl = new Label();
            lbl.Text = cell.UIComponent.Text;
            lbl.Width = cell.Width;
            lbl.Height = cell.Height;
            lbl.Location = new Point(xOffset, yOffset);
            flyout.Controls.Add(lbl);

        }
        public void Paint(TableLayoutPanel tableLayout)
        {
            if (Rows.Count == 0)
            {
                return;
            }
            // Get the visible columns
            var visibleColumns = GetVisibleColumns();
            // Draw the rows
            int yOffset = 0;
            for (int i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];
                if (yOffset + row.Height < DrawingRect.Top)
                {
                    yOffset += row.Height;
                    continue;
                }
                if (yOffset > DrawingRect.Bottom)
                {
                    break;
                }
                PaintRow(tableLayout, row, yOffset, visibleColumns.firstColumnIndex, visibleColumns.lastColumnIndex);
                yOffset += row.Height;
            }
        }
        public void PaintRow(TableLayoutPanel tableLayout, BeepGridRow row, int yOffset, int firstColumnIndex, int lastColumnIndex)
        {
            int xOffset = 0;
            for (int i = firstColumnIndex; i <= lastColumnIndex; i++)
            {
                var cell = row.Cells[i];
                if (xOffset + cell.Width < DrawingRect.Left)
                {
                    xOffset += cell.Width;
                    continue;
                }
                if (xOffset > DrawingRect.Right)
                {
                    break;
                }
                PaintCell(tableLayout, cell, xOffset, yOffset);
                xOffset += cell.Width;
            }
        }
        public void PaintCell(TableLayoutPanel tableLayout, BeepGridCell cell, int xOffset, int yOffset)
        {
            // Draw the cell background
            // Draw the cell border
            // Draw the cell text
            Label lbl = new Label();
            lbl.Text = cell.UIComponent.Text;
            lbl.Width = cell.Width;
            lbl.Height = cell.Height;
            lbl.Location = new Point(xOffset, yOffset);
            tableLayout.Controls.Add(lbl);
        }
        #endregion "Painting"
        #region "Events Handling"
        public void OnMouseDown(MouseEventArgs e)
        {
            if (Rows.Count == 0)
            {
                return;
            }
            int yOffset = 0;
            for (int i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];
                if (yOffset + row.Height < DrawingRect.Top)
                {
                    yOffset += row.Height;
                    continue;
                }
                if (yOffset > DrawingRect.Bottom)
                {
                    break;
                }
                if (e.Y >= yOffset && e.Y <= yOffset + row.Height)
                {
                    OnRowSelected?.Invoke(this, new BeepGridRowEventArgs(row));
                    break;
                }
                yOffset += row.Height;
            }
        }
        public void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (Rows.Count == 0)
            {
                return;
            }
            int yOffset = 0;
            for (int i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];
                if (yOffset + row.Height < DrawingRect.Top)
                {
                    yOffset += row.Height;
                    continue;
                }
                if (yOffset > DrawingRect.Bottom)
                {
                    break;
                }
                if (e.Y >= yOffset && e.Y <= yOffset + row.Height)
                {
                    OnRowSelected?.Invoke(this, new BeepGridRowEventArgs(row));
                    break;
                }
                yOffset += row.Height;
            }
        }
        public void OnMouseUp(MouseEventArgs e)
        {
            if (Rows.Count == 0)
            {
                return;
            }
            int yOffset = 0;
            for (int i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];
                if (yOffset + row.Height < DrawingRect.Top)
                {
                    yOffset += row.Height;
                    continue;
                }
                if (yOffset > DrawingRect.Bottom)
                {
                    break;
                }
                if (e.Y >= yOffset && e.Y <= yOffset + row.Height)
                {
                    OnRowSelected?.Invoke(this, new BeepGridRowEventArgs(row));
                    break;
                }
                yOffset += row.Height;
            }
        }
        public void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                _verticalScrollOffset -= 10;
            }
            else
            {
                _verticalScrollOffset += 10;
            }
        }
        public void OnKeyDown(KeyEventArgs e)
        {
            if (Rows.Count == 0)
            {
                return;
            }
            if (CurrentRow == null)
            {
                CurrentRow = Rows[0];
            }
            else
            {
                int index = Rows.IndexOf(CurrentRow);
                if (e.KeyCode == Keys.Down)
                {
                    if (index < Rows.Count - 1)
                    {
                        CurrentRow = Rows[index + 1];
                    }
                }
                else if (e.KeyCode == Keys.Up)
                {
                    if (index > 0)
                    {
                        CurrentRow = Rows[index - 1];
                    }
                }
            }
        }

        #endregion "Events Handling"
        #region "Data Binding and Handling"
        public void BindData()
        {
            if (DataSource == null)
            {
                return;
            }
            Rows.Clear();
            var data = DataSource as IEnumerable<object>;
            if (data == null)
            {
                return;
            }
            foreach (var item in data)
            {
                var row = new BeepGridRow { RowData = item };
                foreach (var column in Columns)
                {
                    var cell = new BeepGridCell
                    {
                        UIComponent = new BeepLabel { Text = item.GetType().GetProperty(column.ColumnCaption)?.GetValue(item)?.ToString() }
                    };
                    row.Cells.Add(cell);
                }
                Rows.Add(row);
            }
        }

        #endregion "Data Binding and Handling"
        #region "Dynamic Control Pooling"


        private Control GetControlForCell(BeepGridCell cell)
        {
            if (!controlPool.TryGetValue(cell.Id, out var control))
            {
                // Create a new control if not already in the pool
                control = CreateControlForCell(cell);
                controlPool[cell.Id] = control;
               
                // Controls.Add(control);
            }

            control.Visible = true;
            return control;
        }

        private Control CreateControlForCell(BeepGridCell cell)
        {
            Control control = cell.UIComponent switch
            {
                BeepLabel => new BeepLabel(),
                BeepTextBox => new BeepTextBox(),
                BeepComboBox => new BeepComboBox(),
                _ => new Label() // Fallback control
            };

            control.Tag = cell; // Associate the control with its cell
            return control;
        }
        private void RenderVisibleCells()
        {
            var (firstRowIndex, lastRowIndex) = GetVisibleRows();
            var (firstColumnIndex, lastColumnIndex) = GetVisibleColumns();

            // Hide all controls initially
            foreach (var control in controlPool.Values)
            {
                control.Visible = false;
            }

            // Render visible cells
            int yOffset = YOffset;
            for (int rowIndex = firstRowIndex; rowIndex <= lastRowIndex; rowIndex++)
            {
                var row = Rows[rowIndex];
                int xOffset = XOffset;

                for (int colIndex = firstColumnIndex; colIndex <= lastColumnIndex; colIndex++)
                {
                    var cell = row.Cells[colIndex];
                    var cellControl = GetControlForCell(cell);
                    cellControl.SetBounds(
    xOffset - _horizontalScrollOffset, // Account for horizontal scroll
    yOffset - _verticalScrollOffset,   // Account for vertical scroll
    Columns[colIndex].Width,
    RowHeight
);
                    // Position the control
                    //   cellControl.SetBounds(xOffset, yOffset, Columns[colIndex].Width, _rowHeight);

                    // Update content if necessary
                    UpdateControlContent(cellControl, cell);

                    xOffset += Columns[colIndex].Width;
                }

                yOffset += RowHeight;
            }
        }
        private void SetEventsforControl(Control control)
        {
            IBeepUIComponent beepUIComponent = (IBeepUIComponent)control;
            if (beepUIComponent != null) {
                //set events
                beepUIComponent.PropertyChanged += (s, e) =>
                {
                    if (s is Control c && c.Tag is BeepGridCell cell)
                    {
                        CurrentCell = cell;
                        OnCellSelected?.Invoke(this, new BeepGridCellEventArgs(cell));
                    }
                };


            }
            control.MouseDown += (s, e) =>
            {
                if (s is Control c && c.Tag is BeepGridCell cell)
                {
                    CurrentCell = cell;
                    OnCellSelected?.Invoke(this, new BeepGridCellEventArgs(cell));
                }
            };
            control.MouseDoubleClick += (s, e) =>
            {
                if (s is Control c && c.Tag is BeepGridCell cell)
                {
                    CurrentCell = cell;
                    OnCellSelected?.Invoke(this, new BeepGridCellEventArgs(cell));
                }
            };
            control.MouseUp += (s, e) =>
            {
                if (s is Control c && c.Tag is BeepGridCell cell)
                {
                    CurrentCell = cell;
                    OnCellSelected?.Invoke(this, new BeepGridCellEventArgs(cell));
                }
            };
            control.KeyDown += (s, e) =>
            {
                if (s is Control c && c.Tag is BeepGridCell cell)
                {
                    CurrentCell = cell;
                    OnCellSelected?.Invoke(this, new BeepGridCellEventArgs(cell));
                }
            };
        }

        private void UpdateControlContent(Control control, BeepGridCell cell)
        {
            if (control is BeepLabel label && cell.UIComponent is BeepLabel)
            {
                label.Text = ((BeepLabel)cell.UIComponent).Text;
                label.ApplyTheme();
            }
            else if (control is BeepTextBox textBox && cell.UIComponent is BeepTextBox)
            {
                textBox.Text = ((BeepTextBox)cell.UIComponent).Text;
                textBox.ApplyTheme();
            }
            else if (control is BeepComboBox comboBox && cell.UIComponent is BeepComboBox)
            {
                comboBox.Text = ((BeepComboBox)cell.UIComponent).Text;
                comboBox.ApplyTheme();
            }
            else if (control is BeepDatePicker datePicker)
            {
                datePicker.Text = cell.UIComponent.ToString();
            }
            else if (control is BeepCheckBox checkBox)
            {
                // get property value from cell
                PropertyInfo propertyInfo = cell.UIComponent.GetType().GetProperty(cell.UIComponent.BoundProperty);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(cell.UIComponent);
                    if (value != null)
                    {
                        if (value is bool)
                        {
                            checkBox.CheckedValue = (bool)value;
                        }
                        else
                        {
                            checkBox.Text = value.ToString();
                        }
                    }

                }
            }
            else
            {
                control.Text = cell.UIComponent.ToString();
            }
            // Add other UIComponent updates as needed
        }

        #endregion "Dynamic Control Pooling"
    }
    #region "Beep Grid Data Classes"
    // Represents a single row in the grid
    public class BeepGridRow
    {
        public BeepGridRow()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public int Index { get; set; }
        public int DisplayIndex { get; set; }
        public BindingList<BeepGridCell> Cells { get; set; } = new BindingList<BeepGridCell>();
        public object RowData { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsEditable { get; set; }
        public bool IsVisible { get; set; }

        public int Width { get; set; } = 100; // Default cell width
        public int Height { get; set; } = 30; // Default cell height

        // Row Events
        public event EventHandler<BeepGridRowEventArgs> OnRowSelected;
        public event EventHandler<BeepGridRowEventArgs> OnRowValidate;
        public event EventHandler<BeepGridRowEventArgs> OnRowDelete;
        public event EventHandler<BeepGridRowEventArgs> OnRowAdded;
        public event EventHandler<BeepGridRowEventArgs> OnRowUpdate;
        //Cell Events
        public event EventHandler<BeepGridCellEventArgs> OnCellSelected;
        public event EventHandler<BeepGridCellEventArgs> OnCellValidate;


        public void ApplyTheme(BeepTheme theme)
        {
            foreach (var cell in Cells)
            {
                cell.ApplyTheme(theme);
            }
        }
    }

    // Represents a single cell in the grid
    public class BeepGridCell
    {
        public BeepGridCell()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public int Index { get; set; }
        public int DisplayIndex { get; set; }
        private int Colidx { get; set; } // used for to store the display header of column
        private int _rowIdx; // used for to store the display header  of row
        public int RowIdx
        {
            get { return _rowIdx; }
            set { _rowIdx = value; }
        }
        public int Width { get; set; } = 100; // Default cell width
        public int Height { get; set; } = 30; // Default cell height
        public bool IsSelected { get; set; }
        public bool IsDirty { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsEditable { get; set; }
        public bool IsVisible { get; set; }
        public event EventHandler<BeepGridCellEventArgs> OnCellSelected;
        public event EventHandler<BeepGridCellEventArgs> OnCellValidate;

        public IBeepUIComponent UIComponent { get; set; }

        public void ApplyTheme(BeepTheme theme)
        {
            if (UIComponent != null)
            {
                UIComponent.ApplyTheme(theme);
            }
        }
    }

    // Custom event args for row events
    public class BeepGridRowEventArgs : EventArgs
    {
        public BeepGridRow Row { get; }
        public BeepGridRowEventArgs(BeepGridRow row) => Row = row;
    }

    // Custom event args for cell events
    public class BeepGridCellEventArgs : EventArgs
    {
        public BeepGridCell Cell { get; }

        public BeepGridCellEventArgs(BeepGridCell cell) => Cell = cell;
    }
    #endregion "Beep Grid Data Classes"

}
