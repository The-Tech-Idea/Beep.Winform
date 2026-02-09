using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.TextFields;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing cell and row rendering functionality for BeepSimpleGrid
    /// Handles drawing of grid cells, rows, and cell borders
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Rows Painting

        /// <summary>
        /// Paints all visible rows
        /// </summary>
        private void PaintRows(Graphics g, Rectangle bounds)
        {
            int yOffset = bounds.Top;
            var selColumn = Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (_showCheckboxes)
            {
                selColumn.Visible = true;
            }
            else
                selColumn.Visible = false;
            var stickyColumns = Columns.Where(c => c.Sticked && c.Visible).ToList();

            // Ensure _stickyWidth is calculated and capped
            _stickyWidth = stickyColumns.Sum(c => c.Width);
            _stickyWidth = Math.Min(_stickyWidth, bounds.Width); // Prevent overflow

            // Define sticky and scrolling regions
            Rectangle stickyRegion = new Rectangle(bounds.Left, bounds.Top, _stickyWidth, bounds.Height);
            Rectangle scrollingRegion = new Rectangle(bounds.Left + _stickyWidth, bounds.Top, bounds.Width - _stickyWidth, bounds.Height);

            // Draw scrolling columns first
            using (Region clipRegion = new Region(scrollingRegion))
            {
                g.Clip = clipRegion;
                for (int i = 0; i < Rows.Count; i++)
                {
                    var row = Rows[i];
                    int displayY = yOffset;
                    // Stop if yOffset exceeds bounds for non-aggregation rows
                    if (yOffset + row.Height > bounds.Bottom)
                        break;
                    var rowRect = new Rectangle(scrollingRegion.Left, displayY, scrollingRegion.Width, row.Height);

                    PaintScrollingRow(g, row, rowRect);
                    if (!row.IsAggregation) yOffset += row.Height; // Only increment yOffset for non-aggregation rows

                }
            }

            // Reset yOffset for sticky columns
            yOffset = bounds.Top;

            // Draw sticky columns last (on top)
            using (Region clipRegion = new Region(stickyRegion))
            {
                g.Clip = clipRegion;
                foreach (var stickyCol in stickyColumns)
                {
                    int stickyX = bounds.Left + stickyColumns.TakeWhile(c => c != stickyCol).Sum(c => c.Width);
                    for (int i = 0; i < Rows.Count; i++)
                    {
                        var row = Rows[i];
                        int displayY = yOffset;
                        // Stop if yOffset exceeds bounds for non-aggregation rows
                        if (yOffset + row.Height > bounds.Bottom)
                            break;

                        var cell = row.Cells[Columns.IndexOf(stickyCol)];
                        var cellRect = new Rectangle(stickyX, displayY, stickyCol.Width, row.Height);
                        Color backcolor = cell.RowIndex == _currentRowIndex ? _currentTheme.SelectedRowBackColor : _currentTheme.GridBackColor;
                        PaintCell(g, cell, cellRect, backcolor);
                        // set cell coordinates and size in cell
                        cell.X = cellRect.X;
                        cell.Y = cellRect.Y;
                        cell.Width = cellRect.Width;
                        cell.Height = cellRect.Height;
                        if (!row.IsAggregation) yOffset += row.Height; // Only increment yOffset for non-aggregation rows
                    }
                    yOffset = bounds.Top;
                }
            }

            g.ResetClip();
        }

        private void PaintScrollingRow(Graphics g, BeepRowConfig row, Rectangle rowRect)
        {

            // Calculate effective right boundary by subtracting the vertical scrollbar width if it's visible.
            // Compute boundaries for the scrolling area:
            // Assume gridRect represents the entire grid, and sticky columns occupy the left part.
            int scrollingLeft = gridRect.Left + _stickyWidth;
            int scrollingRight = gridRect.Right;
            // If the vertical scrollbar is visible, subtract its width from the right boundary.
            if (_verticalScrollBar != null && _verticalScrollBar.Visible)
            {
                scrollingRight -= _verticalScrollBar.Width;
            }

            // We use rowRect for initial positioning, but clamp our effective boundaries:
            int effectiveLeft = Math.Max(rowRect.Left, scrollingLeft);
            int effectiveRight = Math.Min(rowRect.Right, scrollingRight);

            int accumulatedWidth = 0;
            foreach (var column in Columns.Where(c => !c.Sticked && c.Visible))
            {
                // Calculate the X coordinate for the cell using the accumulated width and horizontal scroll.
                int cellX = rowRect.Left + accumulatedWidth - _xOffset;
                int cellWidth = column.Width;

                // If the cell starts before the effective left boundary, adjust it.
                if (cellX < effectiveLeft)
                {
                    int overflow = effectiveLeft - cellX;
                    cellX = effectiveLeft;
                    cellWidth = Math.Max(0, cellWidth - overflow);
                }

                // If the cell would extend beyond the effective right boundary, truncate its width.
                if (cellX + cellWidth > effectiveRight)
                {
                    cellWidth = Math.Max(0, effectiveRight - cellX);
                }

                // Skip the cell if there's no visible width.
                if (cellWidth <= 0)
                {
                    accumulatedWidth += column.Width;
                    continue;
                }

                // Define the cell rectangle and paint the cell.
                Rectangle cellRect = new Rectangle(cellX, rowRect.Top, cellWidth, rowRect.Height);
                Color backcolor = row.Cells[Columns.IndexOf(column)].RowIndex == _currentRowIndex
                                  ? _currentTheme.SelectedRowBackColor
                                  : _currentTheme.GridBackColor;
                PaintCell(g, row.Cells[Columns.IndexOf(column)], cellRect, backcolor);

                accumulatedWidth += column.Width;
                // If we've reached the effective right boundary, exit the loop.
                if (rowRect.Left + accumulatedWidth - _xOffset >= effectiveRight)
                    break;
            }
        }

        #endregion

        #region Cell Painting

        /// <summary>
        /// Paints a single cell
        /// </summary>
        private void PaintCell(Graphics g, BeepCellConfig cell, Rectangle cellRect, Color backcolor)
        {
            // If this cell is being edited, skip drawing so that
            // the editor control remains visible.
            Rectangle TargetRect = cellRect;
            BeepColumnConfig column;
            cell.Rect = TargetRect;

            if (cell.IsAggregation)
            {
                column = Columns[cell.ColumnIndex];
            }
            else
            {
                column = Columns[cell.ColumnIndex];
            }

            // Determine colors to use - check for custom column colors first
            Color cellBackColor = backcolor;
            Color cellForeColor = _currentTheme.GridForeColor;
            Color cellBorderColor = _currentTheme.GridLineColor;

            // Apply custom column colors if enabled
            if (column.UseCustomColors)
            {
                if (column.HasCustomBackColor)
                {
                    cellBackColor = column.ColumnBackColor;
                }
                if (column.HasCustomForeColor)
                {
                    cellForeColor = column.ColumnForeColor;
                }
                if (column.HasCustomBorderColor)
                {
                    cellBorderColor = column.ColumnBorderColor;
                }
            }

            // Use cached brushes with custom colors
            using (var brush = new SolidBrush(cellBackColor))
            {
                g.FillRectangle(brush, cellRect);
            }

            // Draw selection border if this is the selected cell
            if (_selectedCell == cell)
            {
                using (var pen = new Pen(_currentTheme.PrimaryTextColor, 2))
                {
                    g.DrawRectangle(pen, cellRect);
                }
            }
            else
            {
                // Draw normal border with custom color if specified
                using (var pen = new Pen(cellBorderColor, 1))
                {
                    g.DrawRectangle(pen, cellRect);
                }
            }

            // Get the column editor if available
            if (!_columnDrawers.TryGetValue(Columns[cell.ColumnIndex].ColumnName, out IBeepUIComponent columnDrawer))
            {
                // Create a new control if it doesn't exist (failsafe)
                columnDrawer = CreateCellControlForDrawing(cell);
                _columnDrawers[Columns[cell.ColumnIndex].ColumnName] = columnDrawer;
            }

            if (columnDrawer != null)
            {
                var editor = (Control)columnDrawer;
                editor.Bounds = new Rectangle(TargetRect.X, TargetRect.Y, TargetRect.Width, TargetRect.Height);

                var checkValueupdate = new BeepCellEventArgs(cell);
                CellPreUpdateCellValue?.Invoke(this, checkValueupdate);

                if (!checkValueupdate.Cancel)
                {
                    UpdateCellControl(columnDrawer, Columns[cell.ColumnIndex], cell, cell.CellValue);
                }

                // Force BeepTextBox for aggregation cells
                if (cell.IsAggregation)
                {
                    BeepTextBox textBox = columnDrawer as BeepTextBox ?? new BeepTextBox
                    {
                        Theme = Theme,
                        IsReadOnly = true,
                        Text = cell.CellValue?.ToString() ?? ""
                    };
                    textBox.ForeColor = cellForeColor;
                    textBox.BackColor = cellBackColor;
                    textBox.Draw(g, TargetRect);
                }
                else
                {
                    var checkCustomDraw = new BeepCellEventArgs(cell);
                    checkCustomDraw.Graphics = g;
                    CellCustomCellDraw?.Invoke(this, checkCustomDraw);

                    if (checkCustomDraw.Cancel)
                    {

                        return;
                    }

                    columnDrawer.IsFrameless = true;

                    // Apply custom colors to the editor controls
                    if (columnDrawer is Control editorControl)
                    {
                        editorControl.ForeColor = cellForeColor;
                        editorControl.BackColor = cellBackColor;
                    }

                    // Draw the editor based on column type for non-aggregation cells
                    switch (columnDrawer)
                    {
                        case BeepTextBox textBox:
                            textBox.ForeColor = cellForeColor;
                            textBox.BackColor = cellBackColor;
                            textBox.Draw(g, TargetRect);
                            break;
                        case BeepCheckBoxBool checkBox1:
                            checkBox1.ForeColor = cellForeColor;
                            checkBox1.BackColor = cellBackColor;
                            checkBox1.Draw(g, TargetRect);
                            break;
                        case BeepCheckBoxChar checkBox2:
                            checkBox2.ForeColor = cellForeColor;
                            checkBox2.BackColor = cellBackColor;
                            checkBox2.Draw(g, TargetRect);
                            break;
                        case BeepCheckBoxString checkBox3:
                            checkBox3.ForeColor = cellForeColor;
                            checkBox3.BackColor = cellBackColor;
                            checkBox3.Draw(g, TargetRect);
                            break;
                        case BeepComboBox comboBox:
                            comboBox.ForeColor = cellForeColor;
                            comboBox.BackColor = cellBackColor;
                            comboBox.Draw(g, TargetRect);
                            break;
                        case BeepDatePicker datePicker:
                            datePicker.ForeColor = cellForeColor;
                            datePicker.BackColor = cellBackColor;
                            datePicker.Draw(g, TargetRect);
                            break;
                        case BeepImage image:
                            image.Size = new Size(column.MaxImageWidth, column.MaxImageHeight);
                            image.DrawImage(g, TargetRect);
                            break;
                        case BeepButton button:
                            button.ForeColor = cellForeColor;
                            button.BackColor = cellBackColor;
                            button.Draw(g, TargetRect);
                            break;
                        case BeepProgressBar progressBar:
                            progressBar.ForeColor = cellForeColor;
                            progressBar.BackColor = cellBackColor;
                            progressBar.Draw(g, TargetRect);
                            break;
                        case BeepStarRating starRating:
                            starRating.ForeColor = cellForeColor;
                            starRating.BackColor = cellBackColor;
                            starRating.Draw(g, TargetRect);
                            break;
                        case BeepNumericUpDown numericUpDown:
                            numericUpDown.ForeColor = cellForeColor;
                            numericUpDown.BackColor = cellBackColor;
                            numericUpDown.Draw(g, TargetRect);
                            break;
                        case BeepSwitch switchControl:
                            switchControl.ForeColor = cellForeColor;
                            switchControl.BackColor = cellBackColor;
                            switchControl.Draw(g, TargetRect);
                            break;
                        case BeepListofValuesBox listBox:
                            listBox.ForeColor = cellForeColor;
                            listBox.BackColor = cellBackColor;
                            listBox.Draw(g, TargetRect);
                            break;
                        case BeepLabel label:
                            label.ForeColor = cellForeColor;
                            label.BackColor = cellBackColor;
                            label.Draw(g, TargetRect);
                            break;
                        default:
                            using (var textBrush = new SolidBrush(cellForeColor))
                            {
                                g.DrawString(cell.CellValue?.ToString() ?? "", Font, textBrush, TargetRect,
                                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region Grid Lines Drawing

        /// <summary>
        /// Draws row borders
        /// </summary>
        private void DrawRowsBorders(Graphics g, Rectangle bounds)
        {
            int yOffset = bounds.Top;
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    var row = Rows[i];
                    yOffset += row.Height;
                    if (yOffset < bounds.Bottom)
                        g.DrawLine(pen, bounds.Left, yOffset, bounds.Right, yOffset);
                }
            }
        }

        /// <summary>
        /// Draws column borders
        /// </summary>
        private void DrawColumnBorders(Graphics g, Rectangle bounds)
        {
            int xOffset = bounds.Left;
            int stickyWidth = _stickyWidth;
            var stickyColumns = Columns.Where(c => c.Sticked && c.Visible).ToList();

            // Draw sticky column borders
            using (Region clipRegion = new Region(new Rectangle(bounds.Left, bounds.Top, stickyWidth, bounds.Height)))
            {
                g.Clip = clipRegion;
                foreach (var col in stickyColumns)
                {
                    xOffset += col.Width;
                    if (xOffset < bounds.Left + stickyWidth) // Internal sticky borders
                    {
                        using (Pen borderPen = new Pen(_currentTheme.GridLineColor))
                        {
                            g.DrawLine(borderPen, xOffset, bounds.Top, xOffset, bounds.Bottom);
                        }
                    }
                }
            }

            // Draw scrolling column borders
            using (Region clipRegion = new Region(new Rectangle(bounds.Left + stickyWidth, bounds.Top, bounds.Width - stickyWidth, bounds.Height)))
            {
                g.Clip = clipRegion;
                xOffset = bounds.Left + stickyWidth - _xOffset; // Start after sticky, shift with _xOffset
                foreach (var col in Columns.Where(c => !c.Sticked && c.Visible))
                {
                    xOffset += col.Width;
                    using (Pen borderPen = new Pen(_currentTheme.GridLineColor))
                    {
                        g.DrawLine(borderPen, xOffset, bounds.Top, xOffset, bounds.Bottom);
                    }
                }
            }

            // Separator after sticky columns
            if (stickyWidth > 0)
            {
                g.ResetClip();
                using (Pen borderPen = new Pen(_currentTheme.GridLineColor))
                {
                    g.DrawLine(borderPen, bounds.Left + stickyWidth, bounds.Top, bounds.Left + stickyWidth, bounds.Bottom);
                }
            }
        }

        #endregion

        // Cell events (CellPreUpdateCellValue, CellCustomCellDraw) are declared in BeepSimpleGrid.Events.cs
    }
}
