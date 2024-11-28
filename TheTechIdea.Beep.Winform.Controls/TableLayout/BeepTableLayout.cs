using System;
using System.Data;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Addin;

namespace TheTechIdea.Beep.Winform.Controls.TableLayout
{

    [AddinAttribute(Caption = "Beep Table Layout", Name = "BeepTableLayout", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 46, RootNodeName = "Configuration", Order = 67, ID = 67, BranchText = "Beep Table Layout", BranchType = EnumPointType.Function, IconImageName = "fieldtypeconfig.png", BranchClass = "ADDIN", BranchDescription = "Data Sources Connection Drivers Setup Screen")]
    public partial class BeepTableLayout : uc_Addin, IAddinVisSchema
    {
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 3;
        public int ID { get; set; } = 3;
        public string BranchText { get; set; } = "Beep Table Layout";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 3;
        public string IconImageName { get; set; } = "connectiondrivers.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "Beep Table Layout Screen";
        public string BranchClass { get; set; } = "ADDIN";
        public Point CurrLoc { get; private set; }
        #endregion "IAddinVisSchema"
        private bool isResizing = false;
        private bool isControlDragging = false;
        private int resizeColumnIndex = -1;
        private int resizeRowIndex = -1;
        private int mouseDownX;
        private int mouseDownY;
        int minSize = 10; // Minimum size for rows and columns
        int maxSize = 1000; // Maximum size as an example
        private TableLayoutInfo layoutInfo = new TableLayoutInfo();
        ContextMenuStrip contextMenu = new ContextMenuStrip();
        public BeepTableLayout()
        {
            InitializeComponent();
            // Example setup with 3 rows and 3 columns
          
            this.DoubleBuffered = true;  // Reduce flickering
            this.DragEnter += Container_DragEnter;
            this.DragDrop += Container_DragDrop;
            this.DragLeave += Container_DragLeave;
            this.DragOver += Container_DragEnter;
            contextMenu = new ContextMenuStrip();
            ToolStripMenuItem addRowItem = new ToolStripMenuItem("Add Row", null, AddRow);
            ToolStripMenuItem removeRowItem = new ToolStripMenuItem("Remove Row", null, RemoveRow);
            ToolStripMenuItem addColumnItem = new ToolStripMenuItem("Add Column", null, AddColumn);
            ToolStripMenuItem removeColumnItem = new ToolStripMenuItem("Remove Column", null, RemoveColumn);
            ToolStripMenuItem AddTextBoxItem = new ToolStripMenuItem("Add TextBox", null, AddTextBox);
            ToolStripMenuItem AddInsideTableLayoutItem = new ToolStripMenuItem("Add Layout", null, AddInsideTableLayout);

            // Adding menu items to the context menu
            contextMenu.Items.AddRange(new ToolStripItem[] { AddInsideTableLayoutItem, addRowItem, removeRowItem, addColumnItem, removeColumnItem, AddTextBoxItem });
           
        }


        #region "Context Menu Events"
        private void AddInsideTableLayout(object? sender, EventArgs e)
        {
            Point point = CurrLoc;
            // get column and row of the control
            Point pt = GetRowAndColumnIndex(point);
            if(pt.X==0 && pt.Y == 0)
            {
                return;
            }
            if(layoutInfo.InLayout.ContainsKey(pt))
            {
                return;
            }
            TableLayoutInfo tableLayoutInfo = new TableLayoutInfo();
            int colwidth = layoutInfo.Columns[pt.X];
            int rowheight = layoutInfo.Rows[pt.Y];
            tableLayoutInfo.Columns.Add(colwidth);
            tableLayoutInfo.Rows.Add(rowheight/2);
            tableLayoutInfo.Rows.Add(rowheight / 2);
            layoutInfo.InLayout.Add(pt, tableLayoutInfo);
            Invalidate();
        }
        private void AddTextBox(object? sender, EventArgs e)
        {
            Point point = CurrLoc;
            var textBox = new TextBox();
           
            // get column and row of the control
            Point pt=GetRowAndColumnIndex(point);
            if(layoutInfo.Controls.Where(p => p.Column == pt.X && p.Row == pt.Y).Any())
            {
                return;
            }
            ControlInfo cf = new ControlInfo() { Column = pt.X, Row = pt.Y, Type = "System.Windows.Forms.TextBox" };
            textBox.Tag = cf.ID;
          
            Control control= GetChildAtPoint(point);
            if (control == null)
            {
                layoutInfo.Controls.Add(cf);
                Invalidate();
            }
        }

        private void RemoveColumn(object? sender, EventArgs e)
        {
            Point pt = GetRowAndColumnIndex(CurrLoc);
            layoutInfo.Columns.RemoveAt(pt.X);
            List<ControlInfo> controls = layoutInfo.Controls.Where(p=>p.Column == pt.X ).ToList();
            foreach (var item in controls)
            {
                layoutInfo.Controls.Remove(item);
            }
            List<Point> ls = layoutInfo.InLayout.Where(p => p.Key.X == pt.X).ToList().Select(p => p.Key).ToList();
            foreach (var item in ls)
            {
                RemoveControlsFromInsideLayout(layoutInfo.InLayout[item]);
                layoutInfo.InLayout.Remove(item);
            }
            Invalidate();
        }

        private void AddColumn(object? sender, EventArgs e)
        {

            layoutInfo.Columns.Add(100);
            Invalidate();
        }

        private void RemoveRow(object? sender, EventArgs e)
        {
           
            Point pt = GetRowAndColumnIndex(CurrLoc);
            List<ControlInfo> controls = layoutInfo.Controls.Where(p => p.Row == pt.Y).ToList();
            foreach (var item in controls)
            {
                layoutInfo.Controls.Remove(item);
            }
            List<Point> ls = layoutInfo.InLayout.Where(p => p.Key.Y == pt.Y).ToList().Select(p => p.Key).ToList();
            foreach (var item in ls)
            {
                RemoveControlsFromInsideLayout(layoutInfo.InLayout[item]);
                layoutInfo.InLayout.Remove(item);
            }
            layoutInfo.Rows.RemoveAt(pt.Y);
            Invalidate();
        }
        private void RemoveControlsFromInsideLayout(TableLayoutInfo layout)
        {
            foreach (var item in layout.Controls)
            {
                layout.Controls.Remove(item);
            }

        }
        private void AddRow(object? sender, EventArgs e)
        {
            layoutInfo.Rows.Add(100);
            Invalidate();
        }
        #endregion
        #region "Mouse Events"
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            CurrLoc = e.Location;
            var indices = GetRowAndColumnIndex(CurrLoc);
            if (e.Button== MouseButtons.Right)
            {
               
                contextMenu.Show(this, e.Location);
                return;
            }
            if (layoutInfo.InLayout.ContainsKey(indices))
            {
                var nestedLayout = layoutInfo.InLayout[indices];
                var cellStart = GetCellStartingPoint(indices.X, indices.Y);

                if (IsMouseClickInsideNestedLayout(CurrLoc, cellStart, nestedLayout))
                {
                    // The mouse click is inside the nested layout
                    // Perform actions specific to clicking inside the nested layout
                    Console.WriteLine("Click inside nested layout detected");
                }
                else
                {
                    // The mouse click is outside the nested layout but inside the cell
                    Console.WriteLine("Click outside nested layout but inside the cell");
                }
            }
            else
            {
                // The mouse click is outside any nested layout
                Console.WriteLine("Click in main layout");
            }
            // Determine if near a border for resizing
            // Set isResizing, resizeColumnIndex, resizeRowIndex
            // Check if the cursor is near any column or row border
            resizeRowIndex = -1;
            resizeColumnIndex = -1;

            for (int i = 0; i <= layoutInfo.Columns.Count; i++)
            {
                if (Math.Abs(e.X - layoutInfo.Columns.Take(i).Sum()) <= 15) // 5 pixels threshold for resizing
                {
                    isResizing = true;
                    if(i == 0)
                    {
                        resizeColumnIndex = 0;
                    }
                    else
                    {
                        resizeColumnIndex = i - 1;
                    }
                  
                    mouseDownX = e.X;
                    return;
                }
            }
            for (int j = 0; j <= layoutInfo.Rows.Count; j++)
            {
                if (Math.Abs(e.Y - layoutInfo.Rows.Take(j).Sum()) <= 15) // Same threshold for rows
                {
                    isResizing = true;
                    if(j==0)
                    {
                        resizeRowIndex = 0;
                    }
                    else
                    {
                        resizeRowIndex = j - 1;
                    }
                   
                    mouseDownY = e.Y;
                    return;
                }
            }
            draggedControl = GetChildAtPoint(e.Location);
            if(draggedControl != null)
            {
                draggables[draggedControl] = true;
                isControlDragging= true;
            }
            
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isResizing)
            {
                if (resizeColumnIndex != -1)
                {
                    int newWidth = layoutInfo.Columns[resizeColumnIndex] + (e.X - mouseDownX);
                    newWidth = Math.Max(minSize, Math.Min(maxSize, newWidth));
                    layoutInfo.Columns[resizeColumnIndex] = newWidth;
                    mouseDownX = e.X;
                    Invalidate();
                }
                else if (resizeRowIndex != -1)
                {
                    int newHeight = layoutInfo.Rows[resizeRowIndex] + (e.Y - mouseDownY);
                    newHeight = Math.Max(minSize, Math.Min(maxSize, newHeight));
                    layoutInfo.Rows[resizeRowIndex] = newHeight;
                    mouseDownY = e.Y;
                    Invalidate();
                }
            }
            if (isControlDragging)
            {
                if (draggedControl != null)
                {
                    draggedControl.Location = new Point(e.X - offset.X, e.Y - offset.Y);
                }
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isResizing = false;
            resizeColumnIndex = -1;
            resizeRowIndex = -1;
            if(isControlDragging)
            {
                isControlDragging = false;
                draggables[draggedControl] = false;
            }
            // Finalize resizing and apply new sizes
        }
        #endregion
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Draw grid lines and child controls based on current row/column sizes
            Graphics g = e.Graphics;
            int currentY = 0;

            // Draw rows
            for (int row = 0; row < layoutInfo.Rows.Count; row++)
            {
                int currentX = 0;
                int rowHeight = layoutInfo.Rows[row];

                // Draw columns within each row
                for (int col = 0; col < layoutInfo.Columns.Count; col++)
                {
                    int colWidth = layoutInfo.Columns[col];

                    // Draw cell border (rectangle for each cell)
                    g.DrawRectangle(Pens.White, currentX, currentY, colWidth, rowHeight);

                    // Example: Fill the cell with a light color (optional)
                    //g.FillRectangle(Brushes.LightGray, currentX + 1, currentY + 1, colWidth - 1, rowHeight - 1);

                    // Increment to next column position
                    currentX += colWidth;
                    if (layoutInfo.InLayout.Count > 0)
                    {
                        if (layoutInfo.InLayout.Where(c => c.Key.X == col && c.Key.Y == row).Any())
                        {
                            TableLayoutInfo tableLayoutInfo = layoutInfo.InLayout.Where(c => c.Key.X == col && c.Key.Y == row).FirstOrDefault().Value;
                            if (tableLayoutInfo != null)
                            {
                                DrawTableLayout( g,tableLayoutInfo, col, row);
                            }
                        }
                    }
                    if (layoutInfo.Controls != null)
                    {
                        if(layoutInfo.Controls.Where(c => c.Column == col && c.Row == row).Any())
                        {
                            string typestring = layoutInfo.Controls.Where(c => c.Column == col && c.Row == row).FirstOrDefault().Type;
                            if (typestring != null)
                            {
                                // Create control based on type (e.g., TextBox, Button, etc.)
                                // Example: Control ctl = new TextBox(
                                Type type = DMEEditor.assemblyHandler.GetType(typestring);
                                if (type != null)
                                {
                                    Control ctl = (Control)Activator.CreateInstance(type);
                                    if (ctl != null)
                                    {
                                        ctl.Location = GetCellStartingPoint(col,row);
                                        ctl.Size = new Size(colWidth, rowHeight);
                                        Controls.Add(ctl);
                                    }
                                }
                                
                            }
                        }
                        // Check if there is a control at this cell
                    }
                 
                }

                // Increment to next row position
                currentY += rowHeight;
            }
        }

        private void DrawTableLayout(Graphics g, TableLayoutInfo tableLayoutInfo, int pcol, int prow)
        {
          
            // Draw rows
            Point point=GetCellStartingPoint(pcol, prow);
            int currentY = point.Y;
            for (int row = 0; row < tableLayoutInfo.Rows.Count; row++)
            {
                int currentX = point.X;
                int rowHeight = tableLayoutInfo.Rows[row];

                // Draw columns within each row
                for (int col = 0; col < tableLayoutInfo.Columns.Count; col++)
                {
                    int colWidth = tableLayoutInfo.Columns[col];

                    // Draw cell border (rectangle for each cell)
                    g.DrawRectangle(Pens.White, currentX, currentY, colWidth, rowHeight);
                }
            }
        }
         
        #region "Control Management"
        //public void AddControl(Control control, int row, int column)
        //{
        //    control.Tag = new Point(column, row); // Storing column and row as a Point
        //    Controls.Add(control);
        //    PositionControl(control, row, column);
        //}
        //private void PositionControl(Control control, int row, int column)
        //{
        //    control.Location = GetCellStartingPoint(column,row);
        //    control.Size = new Size(layoutInfo.GridViewColumns[column], layoutInfo.Rows[row]);
        //}
        //private void ExtendTable(int rowsNeeded, int columnsNeeded)
        //{
        //    while (layoutInfo.Rows.Count <= rowsNeeded)
        //        layoutInfo.Rows.Add(100); // Default size
        //    while (layoutInfo.GridViewColumns.Count <= columnsNeeded)
        //        layoutInfo.GridViewColumns.Add(100); // Default size
        //}
        //protected override void OnControlAdded(ControlEventArgs e)
        //{
        //    base.OnControlAdded(e);
        //    PositionControls();
        //}
        //protected override void OnControlRemoved(ControlEventArgs e)
        //{
        //    base.OnControlRemoved(e);
        //    //PositionControls();
        //}
        //private void PositionControls()
        //{
        //    foreach (Control control in Controls)
        //    {
        //        int rowIndex = ...; // Determine row index based on control's initial position or metadata
        //        int columnIndex = ...; // Determine column index
        //        PositionControl(control, rowIndex, columnIndex);
        //    }
        //}
        //private void PositionControlsb()
        //{
        //    int rowIndex = 0;
        //    foreach (Control control in Controls)
        //    {
        //        int columnIndex = Controls.IndexOf(control) % columnWidths.Count;  // Example calculation
        //        rowIndex = Controls.IndexOf(control) / columnWidths.Count;  // Example calculation

        //        if (columnIndex < columnWidths.Count && rowIndex < rowHeights.Count)
        //        {
        //            control.Location = new Point(columnWidths.Take(columnIndex).Sum(), rowHeights.Take(rowIndex).Sum());
        //            control.Size = new Size(columnWidths[columnIndex], rowHeights[rowIndex]);
        //        }
        //    }
        //}
        #endregion
        #region "Drag and Drop"
        private Control draggedControl = null;
        static Dictionary<Control, bool> draggables = new Dictionary<Control, bool>();
        private Point offset;
        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    base.OnMouseDown(e);
        //    draggedControl = GetChildAtPoint(e.Location);
        //    draggables[draggedControl] = true;
        //    if (draggedControl != null)
        //    {
        //        offset = new Point(e.X - draggedControl.Left, e.Y - draggedControl.Top);
        //    }
        //}

      

        private void Container_DragDrop(object sender, DragEventArgs e)
        {
            string controlname = e.Data.GetData(DataFormats.Text).ToString();
            Point screenCoords = new Point(e.X, e.Y);
            Point controlRelatedCoords = this.PointToClient(screenCoords);
            try
            {
             
            }
            catch (Exception ex)
            {
                //DMEEditor.AddLogMessage("Beep", $"Error Dragedd item {ex.Message}", DateTime.Now, 0, null, TheTechIdea.Util.Errors.Failed);
            }
            //DMEEditor.AddLogMessage("Beep", $"Dragedd item", DateTime.Now, 0, null, TheTechIdea.Util.Errors.Ok);
        }
        private void Container_DragLeave(object sender, EventArgs e)
        {
            DoDragDrop("MOVE", DragDropEffects.Move);
        }
        private void Container_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string actiontype = e.Data.GetData(DataFormats.Text).ToString();
                switch (actiontype)
                {
                    case "NEW":
                        e.Effect = DragDropEffects.Copy;
                        break;
                    case "MOVE":
                        e.Effect = DragDropEffects.Move;
                        break;
                    default:
                        e.Effect = DragDropEffects.None;
                        break;
                }
            }


        }
        #endregion
        #region "Utility Methods"
        private Point GetRowAndColumnIndex(Point mouseLocation)
        {
            Point indices = new Point();
            int sum = 0;

            // Calculate column index
            for (int i = 0; i < layoutInfo.Columns.Count; i++)
            {
                sum += layoutInfo.Columns[i];
                if (mouseLocation.X < sum)
                {
                    indices.X = i;
                    break;
                }
            }

            // Reset sum for row calculation
            sum = 0;

            // Calculate row index
            for (int j = 0; j < layoutInfo.Rows.Count; j++)
            {
                sum += layoutInfo.Rows[j];
                if (mouseLocation.Y < sum)
                {
                    indices.Y = j;
                    break;
                }
            }

            return indices;
        }
        private Point GetCellStartingPoint(int column, int row)
        {
            Point startPoint = new Point();

            // Sum up all column widths up to the specified column to get the X coordinate
            startPoint.X = layoutInfo.Columns.Take(column).Sum();

            // Sum up all row heights up to the specified row to get the Y coordinate
            startPoint.Y = layoutInfo.Rows.Take(row).Sum();

            return startPoint;
        }
        private int GetRowHitTest(Point point)
        {
            int sum = 0;
            for (int i = 0; i < layoutInfo.Rows.Count; i++)
            {
                sum += layoutInfo.Rows[i];
                if (point.Y < sum)
                {
                    return i;
                }
            }
            return -1;
        }
        private int GetColumnHitTest(Point point)
        {
            int sum = 0;
            for (int i = 0; i < layoutInfo.Columns.Count; i++)
            {
                sum += layoutInfo.Columns[i];
                if (point.X < sum)
                {
                    return i;
                }
            }
            return -1;
        }
        private int GetRowHeight(int row)
        {
            // Return the height of the specified row
            return layoutInfo.Rows[row];
        }
        private int GetColumnWidth(int col)
        {
            // Return the height of the specified row
            return layoutInfo.Columns[col];
        }
        private Point GetNestedLayoutOffset(Point indices)
        {
            // This method calculates the starting position of the cell that contains the nested layout
            return GetCellStartingPoint(indices.X, indices.Y);
        }
        // This method would determine if the click is on the main layout or a nested one
        private TableLayoutInfo GetActiveLayout(Point indices, Point location)
        {
            if (layoutInfo.InLayout.ContainsKey(indices))
            {
                // You need to calculate the offset to adjust the mouse coordinates to the nested layout's coordinate system
                Point nestedOffset = GetNestedLayoutOffset(indices);
                Point adjustedLocation = new Point(location.X - nestedOffset.X, location.Y - nestedOffset.Y);
                return layoutInfo.InLayout[indices];
            }
            return layoutInfo;
        }
        private bool IsNearNestedBorder(Point mouseLocation, Point cellStart, TableLayoutInfo nestedLayout)
        {
            // Check proximity to left, right, top, and bottom borders of the nested layout
            int left = cellStart.X;
            int right = left + nestedLayout.Columns.Sum();
            int top = cellStart.Y;
            int bottom = top + nestedLayout.Rows.Sum();

            // Define a sensitivity margin for resizing (e.g., 5 pixels)
            int margin = 5;

            return (Math.Abs(mouseLocation.X - left) <= margin ||
                    Math.Abs(mouseLocation.X - right) <= margin ||
                    Math.Abs(mouseLocation.Y - top) <= margin ||
                    Math.Abs(mouseLocation.Y - bottom) <= margin);
        }
        private bool IsMouseClickInsideNestedLayout(Point mouseLocation, Point cellStart, TableLayoutInfo nestedLayout)
        {
            // Calculate the bounds of the nested layout
            int nestedLeft = cellStart.X;
            int nestedTop = cellStart.Y;
            int nestedRight = nestedLeft + nestedLayout.Columns.Sum();
            int nestedBottom = nestedTop + nestedLayout.Rows.Sum();

            // Check if the mouse location is within these bounds
            return mouseLocation.X >= nestedLeft && mouseLocation.X <= nestedRight &&
                   mouseLocation.Y >= nestedTop && mouseLocation.Y <= nestedBottom;
        }

        private void AdjustForNestedLayout(Point cellStart)
        {
            // Adjust global variables used in resizing
            mouseDownX -= cellStart.X;
            mouseDownY -= cellStart.Y;
        }
        #endregion
    }
}
