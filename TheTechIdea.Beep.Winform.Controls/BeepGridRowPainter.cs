﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Models;
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
        public BeepGridRowPainter(BeepSimpleGrid grid, Rectangle drawingRect, int rowHeight, int gridHeight, int gridWidth, int xOffset, int yOffset, BindingList<BeepRowConfig> rows)
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
        public BeepGridRowPainter(BeepSimpleGrid grid, Rectangle drawingRect, int rowHeight, int gridHeight, int gridWidth, int xOffset, int yOffset, BindingList<BeepRowConfig> rows, Object dataSource)
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
        public BeepGridRowPainter(BeepSimpleGrid grid, Rectangle drawingRect, int rowHeight, int gridHeight, int gridWidth, int xOffset, int yOffset, BindingList<BeepRowConfig> rows, Object dataSource, List<BeepColumnConfig> columns)
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
        private BeepTheme _currentTheme => BeepThemesManager.GetTheme(Grid!=null? Grid.Theme : EnumBeepThemes.DefaultTheme );
        public Rectangle DrawingRect { get; set; }
        #endregion "Propoerties needed for the grid"
        public BindingList<BeepRowConfig> Rows { get; set; } = new BindingList<BeepRowConfig>();
        public BeepRowConfig CurrentRow { get; set; }
        public BeepCellConfig CurrentCell { get; set; }
        public BeepCellConfig CurrentCellInEdit { get; set; }
        public BeepRowConfig CurrentRowInEdit { get; set; }
        public List<BeepColumnConfig> Columns
        {
            get => Grid.Columns;
            
        }
        private Dictionary<string, Control> controlPool = new(); // Pool of controls by Cell ID
       
        #region "Events Delegates"
        // Row Events
        public event EventHandler<BeepRowEventArgs> OnRowSelected;
        public event EventHandler<BeepRowEventArgs> OnRowValidate;
        public event EventHandler<BeepRowEventArgs> OnRowDelete;
        public event EventHandler<BeepRowEventArgs> OnRowAdded;
        public event EventHandler<BeepRowEventArgs> OnRowUpdate;
        //Cell Events
        public event EventHandler<BeepCellEventArgs> OnCellSelected;
        public event EventHandler<BeepCellEventArgs> OnCellValidate;
        #endregion "Events Delegates"
        #region "Virtualization"
        private int _verticalScrollOffset = 0; // Track vertical scroll
        private int _horizontalScrollOffset = 0; // Track horizontal scroll
        private List<BeepColumnConfig> _columns;
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
                if (Grid.ShowHorizontalGridLines)
                {
                    Pen pen = new Pen(_currentTheme.GridLineColor);
                    g.DrawLine(pen, DrawingRect.Left, yOffset + row.Height, DrawingRect.Right, yOffset + row.Height);
                }
                yOffset += row.Height;
            }
        }
        private void PaintRow(Graphics g, BeepRowConfig row, int yOffset, int firstColumnIndex, int lastColumnIndex)
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
        private void PaintCell(Graphics g, BeepCellConfig cell, int xOffset, int yOffset)
        {
            // Draw the cell background
            g.FillRectangle(Brushes.White, new Rectangle(xOffset, yOffset, cell.Width, cell.RowIndex));
            // Draw the cell border
            g.DrawRectangle(Pens.Black, new Rectangle(xOffset, yOffset, cell.Width, cell.RowIndex));
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
        public void PaintRow(FlowLayoutPanel flyout, BeepRowConfig row, int yOffset, int firstColumnIndex, int lastColumnIndex)
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
        public void PaintCell(FlowLayoutPanel flyout, BeepCellConfig cell, int xOffset, int yOffset)
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
        public void PaintRow(TableLayoutPanel tableLayout, BeepRowConfig row, int yOffset, int firstColumnIndex, int lastColumnIndex)
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
        public void PaintCell(TableLayoutPanel tableLayout, BeepCellConfig cell, int xOffset, int yOffset)
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
                    OnRowSelected?.Invoke(this, new BeepRowEventArgs(row));
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
                    OnRowSelected?.Invoke(this, new BeepRowEventArgs(row));
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
                    OnRowSelected?.Invoke(this, new BeepRowEventArgs(row));
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
                var row = new BeepRowConfig { RowData = item };
                foreach (var column in Columns)
                {
                    var cell = new BeepCellConfig
                    {
                        UIComponent = new BeepLabel { Text = item.GetType().GetProperty(column.ColumnCaption)?.GetValue(item)?.ToString() }
                    };
                    row.Cells.Add(cell);
                }
                Rows.Add(row);
            }
        }
        public void AddRow(params object[] values)
        {
            // Make sure the number of incoming values matches the number of columns
            if (values.Length != Columns.Count)
                throw new ArgumentException("Number of values does not match number of columns.");

            // Create a new BeepRowConfig
            var row = new BeepRowConfig();

            for (int i = 0; i < Columns.Count; i++)
            {
                // Create a cell
                var cell = new BeepCellConfig
                {
                    ColumnIndex = i,
                    // For simplicity, let's just stick a BeepLabel in each cell
                    UIComponent = new BeepLabel
                    {
                        Text = values[i]?.ToString() ?? string.Empty
                    }
                };
                // Add the cell to the row
                row.Cells.Add(cell);
            }

            // Finally, add the newly created row to our grid's Rows collection
            Rows.Add(row);

            // Optionally force a redraw
            Grid.Invalidate();
        }
        public void AddRow(Dictionary<string, object> dataByColumnName)
        {
            // Create a new row
            var row = new BeepRowConfig();

            foreach (var columnConfig in Columns)
            {
                // For each configured column, check if dictionary has a matching key
                object cellValue = null;

                // You might check either the ColumnName or ColumnCaption,
                // depending on how you prefer to identify the column in the dictionary.
                // Let's assume dictionary keys match the ColumnCaption for now.
                if (dataByColumnName.ContainsKey(columnConfig.ColumnCaption))
                    cellValue = dataByColumnName[columnConfig.ColumnCaption];

                // Create the cell
                var cell = new BeepCellConfig
                {
                    UIComponent = new BeepLabel { Text = cellValue?.ToString() ?? string.Empty }
                };

                // Add the cell
                row.Cells.Add(cell);
            }

            // Add to grid
            Rows.Add(row);
            Grid.Invalidate();
        }
        public void AddRowFromObject<T>(T item)
        {
            var row = new BeepRowConfig();

            // For each BeepColumnConfig in the grid
            foreach (var columnConfig in Columns)
            {
                // If the column’s ColumnCaption matches a property in T,
                // or if you have a different convention, handle it here
                var property = typeof(T).GetProperty(columnConfig.ColumnCaption);
                object value = property != null ? property.GetValue(item) : null;

                // Create the cell
                var cell = new BeepCellConfig
                {
                    UIComponent = new BeepLabel { Text = value?.ToString() ?? string.Empty }
                };

                // Add the cell
                row.Cells.Add(cell);
            }

            Rows.Add(row);
            Grid.Invalidate();
        }


        #endregion "Data Binding and Handling"
        #region "Dynamic Control Pooling"


        private Control GetControlForCell(BeepCellConfig cell)
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

        private Control CreateControlForCell(BeepCellConfig cell)
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
                    if (s is Control c && c.Tag is BeepCellConfig cell)
                    {
                        CurrentCell = cell;
                        OnCellSelected?.Invoke(this, new BeepCellEventArgs(cell));
                    }
                };


            }
            control.MouseDown += (s, e) =>
            {
                if (s is Control c && c.Tag is BeepCellConfig cell)
                {
                    CurrentCell = cell;
                    OnCellSelected?.Invoke(this, new BeepCellEventArgs(cell));
                }
            };
            control.MouseDoubleClick += (s, e) =>
            {
                if (s is Control c && c.Tag is BeepCellConfig cell)
                {
                    CurrentCell = cell;
                    OnCellSelected?.Invoke(this, new BeepCellEventArgs(cell));
                }
            };
            control.MouseUp += (s, e) =>
            {
                if (s is Control c && c.Tag is BeepCellConfig cell)
                {
                    CurrentCell = cell;
                    OnCellSelected?.Invoke(this, new BeepCellEventArgs(cell));
                }
            };
            control.KeyDown += (s, e) =>
            {
                if (s is Control c && c.Tag is BeepCellConfig cell)
                {
                    CurrentCell = cell;
                    OnCellSelected?.Invoke(this, new BeepCellEventArgs(cell));
                }
            };
        }
        private void UpdateControlContent(Control control, BeepCellConfig cell)
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
            else if (control is BeepCheckBoxBool checkBox)
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
        #region "Draw Borders"
        private void DrawColumnBorders(Graphics g, Rectangle drawingBounds)
        {

            int xOffset = drawingBounds.Left + XOffset;
            int yOffset = drawingBounds.Top + YOffset;
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                for (int i = 0; i < Columns.Count - 1; i++)
                {
                    xOffset += Columns[i].Width;
                    g.DrawLine(pen, xOffset, yOffset, xOffset, drawingBounds.Bottom);
                }
            }
        }
        private void DrawRowsBorders(Graphics g, Rectangle drawingBounds)
        {
            int yOffset = drawingBounds.Top + YOffset;
            using (var pen = new Pen(_currentTheme.BorderColor))
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    yOffset += Grid.RowHeight;
                    g.DrawLine(pen, drawingBounds.Left, yOffset, drawingBounds.Right, yOffset);
                }
            }
        }
        #endregion "Draw Borders"
        #region "Theme Management"
        public void ApplyTheme(BeepTheme theme)
        {
            foreach (var row in Rows)
            {
                row.ApplyTheme(theme);
            }
        }
        #endregion "Theme Management"


    }
  

}
