using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridRenderHelper
    {
        private readonly BeepGridPro _grid;
        private BeepCheckBoxBool _rowCheck;

        public GridRenderHelper(BeepGridPro grid)
        {
            _grid = grid;
        }

        internal IBeepTheme Theme => _grid.Theme != null ? BeepThemesManager.GetTheme(_grid.Theme) : BeepThemesManager.GetDefaultTheme();

        public void Draw(Graphics g)
        {
            // Validate graphics object and grid state
            if (g == null || _grid == null || _grid.Layout == null)
                return;

            var rowsRect = _grid.Layout.RowsRect;
            if (rowsRect.Width <= 0 || rowsRect.Height <= 0)
                return;

            // Draw background
            using (var brush = new SolidBrush(Theme?.GridBackColor ?? SystemColors.Window))
            {
                g.FillRectangle(brush, rowsRect);
            }

            // Draw column headers
            if (_grid.ShowColumnHeaders)
            {
                try
                {
                    DrawColumnHeaders(g);
                }
                catch (Exception)
                {
                    // Silently handle header drawing errors to prevent crashes
                }
            }

            // Draw data rows
            try
            {
                DrawRows(g);
            }
            catch (Exception)
            {
                // Silently handle row drawing errors to prevent crashes
            }

            // Draw navigator if enabled
            if (_grid.ShowNavigator)
            {
                try
                {
                    DrawNavigatorArea(g);
                }
                catch (Exception)
                {
                    // Silently handle navigator drawing errors
                }
            }
            
            // Draw selection indicators
            try
            {
                DrawSelectionIndicators(g);
            }
            catch (Exception)
            {
                // Silently handle selection drawing errors
            }
        }

        private void DrawColumnHeaders(Graphics g)
        {
            var headerRect = _grid.Layout.HeaderRect;
            if (headerRect.Height <= 0 || headerRect.Width <= 0) return;

            // Fill header background
            using (var brush = new SolidBrush(Theme?.GridHeaderBackColor ?? SystemColors.Control))
            {
                g.FillRectangle(brush, headerRect);
            }

            // Draw header border
            using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
            {
                g.DrawLine(pen, headerRect.Left, headerRect.Bottom, headerRect.Right, headerRect.Bottom);
            }

            // Draw column header cells
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                var column = _grid.Data.Columns[i];
                if (!column.Visible) continue;

                if (i < _grid.Layout.HeaderCellRects.Length)
                {
                    var cellRect = _grid.Layout.HeaderCellRects[i];
                    
                    // Validate the cell rectangle before drawing
                    if (cellRect.Width > 0 && cellRect.Height > 0 && 
                        cellRect.X >= 0 && cellRect.Y >= 0 &&
                        cellRect.Right <= headerRect.Right && cellRect.Bottom <= headerRect.Bottom)
                    {
                        DrawHeaderCell(g, column, cellRect);
                    }
                }
            }
        }

        private void DrawHeaderCell(Graphics g, BeepColumnConfig column, Rectangle cellRect)
        {
            // Validate inputs first
            if (g == null || column == null || cellRect.Width <= 0 || cellRect.Height <= 0)
                return;

            // Background
            using (var brush = new SolidBrush(Theme?.GridHeaderBackColor ?? SystemColors.Control))
            {
                g.FillRectangle(brush, cellRect);
            }

            // Text - with proper validation
            var textColor = Theme?.GridHeaderForeColor ?? SystemColors.ControlText;
            string text = column.ColumnCaption ?? column.ColumnName ?? "";
            
            // Only draw text if we have valid text and font
            if (!string.IsNullOrEmpty(text) && _grid.Font != null)
            {
                try
                {
                    using (var brush = new SolidBrush(textColor))
                    using (var format = new StringFormat { 
                        Alignment = StringAlignment.Center, 
                        LineAlignment = StringAlignment.Center,
                        FormatFlags = StringFormatFlags.NoWrap,
                        Trimming = StringTrimming.EllipsisCharacter
                    })
                    {
                        // Create a slightly smaller rectangle to ensure we don't draw outside bounds
                        var textRect = new Rectangle(
                            cellRect.X + 2, 
                            cellRect.Y + 2, 
                            Math.Max(1, cellRect.Width - 4), 
                            Math.Max(1, cellRect.Height - 4)
                        );
                        
                        g.DrawString(text, _grid.Font, brush, textRect, format);
                    }
                }
                catch (ArgumentException)
                {
                    // If drawing fails, fall back to TextRenderer
                    try
                    {
                        TextRenderer.DrawText(g, text, _grid.Font, cellRect, textColor,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | 
                            TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                    }
                    catch
                    {
                        // If all else fails, silently skip drawing the text
                    }
                }
            }

            // Border
            using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
            {
                g.DrawRectangle(pen, cellRect);
            }
        }

        private void DrawRows(Graphics g)
        {
            var rowsRect = _grid.Layout.RowsRect;
            using var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark);

            // Calculate sticky regions EXACTLY like BeepSimpleGrid.PaintRows()
            var selColumn = _grid.Data.Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (_grid.ShowCheckBox && selColumn != null)
            {
                selColumn.Visible = true;
            }
            else if (selColumn != null)
            {
                selColumn.Visible = false;
            }

            var stickyColumns = _grid.Data.Columns.Where(c => c.Sticked && c.Visible).ToList();
            int stickyWidth = stickyColumns.Sum(c => Math.Max(20, c.Width));
            stickyWidth = Math.Min(stickyWidth, rowsRect.Width); // Prevent overflow

            // ? FIX: Calculate Y offset properly like BeepSimpleGrid
            // Don't use VerticalOffset directly - use the actual first visible row's Y position
            int currentY = rowsRect.Top;
            int firstVisibleRowIndex = _grid.Scroll.FirstVisibleRowIndex;
            
            // Account for partial visibility of the first row due to pixel-level scrolling
            int totalRowsHeight = 0;
            for (int i = 0; i < firstVisibleRowIndex && i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                totalRowsHeight += row.Height > 0 ? row.Height : _grid.RowHeight;
            }
            
            // Adjust currentY by the difference between pixel offset and row-based offset
            int pixelOffset = _grid.Scroll.VerticalOffset;
            currentY = rowsRect.Top - (pixelOffset - totalRowsHeight);

            // Calculate which rows are actually visible
            int visibleRowStart = firstVisibleRowIndex;
            int visibleRowEnd = Math.Min(_grid.Data.Rows.Count - 1, 
                visibleRowStart + GetVisibleRowCount(_grid.Data.Rows, rowsRect.Height, visibleRowStart, pixelOffset));

            // Define sticky and scrolling regions EXACTLY like BeepSimpleGrid
            Rectangle stickyRegion = new Rectangle(rowsRect.Left, rowsRect.Top, stickyWidth, rowsRect.Height);
            Rectangle scrollingRegion = new Rectangle(rowsRect.Left + stickyWidth, rowsRect.Top, 
                                                     Math.Max(0, rowsRect.Width - stickyWidth), rowsRect.Height);

            // Draw scrolling columns first - with correct Y positioning
            var state1 = g.Save();
            g.SetClip(scrollingRegion);
            var scrollCols = _grid.Data.Columns.Select((c, idx) => new { Col = c, Index = idx })
                                               .Where(x => x.Col.Visible && !x.Col.Sticked)
                                               .ToList();
            
            // ? FIX: Draw rows with correct Y positioning
            int drawY = currentY;
            for (int r = visibleRowStart; r <= visibleRowEnd && r < _grid.Data.Rows.Count; r++)
            {
                var row = _grid.Data.Rows[r];
                int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                
                // Only draw if the row is within the visible area
                if (drawY + rowHeight > rowsRect.Top && drawY < rowsRect.Bottom)
                {
                    int x = scrollingRegion.Left - _grid.Scroll.HorizontalOffset;
                    
                    foreach (var sc in scrollCols)
                    {
                        int colW = Math.Max(20, sc.Col.Width);
                        if (x + colW > scrollingRegion.Left && x < scrollingRegion.Right)
                        {
                            if (sc.Index < row.Cells.Count)
                            {
                                var cell = row.Cells[sc.Index];
                                var rect = new Rectangle(x, drawY, colW, rowHeight);

                                bool isActiveRow = false; // TODO: implement selection
                                bool isSelectedRow = false; // TODO: implement selection
                                var useSelected = isActiveRow || isSelectedRow;

                                var back = useSelected
                                    ? (Theme?.GridRowSelectedBackColor == Color.Empty ? (Theme?.SelectedRowBackColor ?? SystemColors.Highlight) : Theme.GridRowSelectedBackColor)
                                    : (sc.Col.HasCustomBackColor && sc.Col.UseCustomColors ? sc.Col.ColumnBackColor : (Theme?.GridBackColor ?? SystemColors.Window));
                                var fore = sc.Col.HasCustomForeColor && sc.Col.UseCustomColors ? sc.Col.ColumnForeColor : (Theme?.GridForeColor ?? SystemColors.WindowText);

                                using (var bg = new SolidBrush(back)) g.FillRectangle(bg, rect);
                                DrawCellContent(g, sc.Col, cell, rect, fore, back);
                                g.DrawRectangle(pen, rect);
                            }
                        }
                        x += colW;
                        if (x > scrollingRegion.Right) break;
                    }
                }
                drawY += rowHeight;
            }
            g.Restore(state1);

            // Draw sticky columns last - with correct Y positioning
            var state2 = g.Save();
            g.SetClip(stickyRegion);
            var stickyCols = _grid.Data.Columns.Select((c, idx) => new { Col = c, Index = idx })
                                               .Where(x => x.Col.Visible && x.Col.Sticked)
                                               .ToList();
            int startX = rowsRect.Left;
            foreach (var st in stickyCols)
            {
                int colW = Math.Max(20, st.Col.Width);
                drawY = currentY; // Reset Y for each sticky column
                
                for (int r = visibleRowStart; r <= visibleRowEnd && r < _grid.Data.Rows.Count; r++)
                {
                    var row = _grid.Data.Rows[r];
                    int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                    
                    // Only draw if the row is within the visible area
                    if (drawY + rowHeight > rowsRect.Top && drawY < rowsRect.Bottom)
                    {
                        if (st.Index < row.Cells.Count)
                        {
                            var cell = row.Cells[st.Index];
                            var rect = new Rectangle(startX, drawY, colW, rowHeight);

                            bool isActiveRow = false; // TODO: implement selection
                            bool isSelectedRow = false; // TODO: implement selection
                            var useSelected = isActiveRow || isSelectedRow;

                            var back = useSelected
                                ? (Theme?.GridRowSelectedBackColor == Color.Empty ? (Theme?.SelectedRowBackColor ?? SystemColors.Highlight) : Theme.GridRowSelectedBackColor)
                                : (st.Col.HasCustomBackColor && st.Col.UseCustomColors ? st.Col.ColumnBackColor : (Theme?.GridBackColor ?? SystemColors.Window));
                            var fore = st.Col.HasCustomForeColor && st.Col.UseCustomColors ? st.Col.ColumnForeColor : (Theme?.GridForeColor ?? SystemColors.WindowText);

                            using (var bg = new SolidBrush(back)) g.FillRectangle(bg, rect);

                            // Handle checkbox drawing EXACTLY like BeepSimpleGrid
                            if (st.Col.IsSelectionCheckBox && _grid.ShowCheckBox)
                            {
                                _rowCheck ??= new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme };
                                _rowCheck.CurrentValue = (bool)(cell.CellValue ?? false);
                                _rowCheck.Draw(g, rect);
                            }
                            else if (st.Col.IsRowNumColumn)
                            {
                                // Draw row number exactly like BeepSimpleGrid
                                using var textBrush = new SolidBrush(fore);
                                using var font = _grid.Font;
                                TextRenderer.DrawText(g, cell.CellValue?.ToString() ?? "", font, rect, fore,
                                    TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis);
                            }
                            else
                            {
                                DrawCellContent(g, st.Col, cell, rect, fore, back);
                            }
                            
                            g.DrawRectangle(pen, rect);
                        }
                    }
                    drawY += rowHeight;
                }
                startX += colW;
            }
            g.Restore(state2);
        }

        private void DrawCellContent(Graphics g, BeepColumnConfig column, BeepCellConfig cell, Rectangle rect, Color foreColor, Color backColor)
        {
            // Validate inputs
            if (g == null || column == null || cell == null || rect.Width <= 0 || rect.Height <= 0)
                return;

            string text = cell.CellValue?.ToString() ?? "";
            
            if (column.IsSelectionCheckBox)
            {
                // Handle checkbox specially
                try
                {
                    bool isChecked = (bool)(cell.CellValue ?? false);
                    var checkRect = new Rectangle(
                        rect.X + Math.Max(0, (rect.Width - 16) / 2), 
                        rect.Y + Math.Max(0, (rect.Height - 16) / 2), 
                        Math.Min(16, rect.Width), 
                        Math.Min(16, rect.Height)
                    );
                    
                    if (checkRect.Width > 0 && checkRect.Height > 0)
                    {
                        ControlPaint.DrawCheckBox(g, checkRect, isChecked ? ButtonState.Checked : ButtonState.Normal);
                    }
                }
                catch
                {
                    // If checkbox drawing fails, fall back to text
                    bool isChecked = (bool)(cell.CellValue ?? false);
                    text = isChecked ? "?" : "?";
                }
            }
            
            // Draw text if we have any
            if (!string.IsNullOrEmpty(text) && _grid.Font != null)
            {
                try
                {
                    using (var brush = new SolidBrush(foreColor))
                    using (var format = new StringFormat { 
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Near,
                        FormatFlags = StringFormatFlags.NoWrap,
                        Trimming = StringTrimming.EllipsisCharacter
                    })
                    {
                        // Create a slightly smaller rectangle for text
                        var textRect = new Rectangle(
                            rect.X + 2, 
                            rect.Y + 1, 
                            Math.Max(1, rect.Width - 4), 
                            Math.Max(1, rect.Height - 2)
                        );
                        
                        g.DrawString(text, _grid.Font, brush, textRect, format);
                    }
                }
                catch (ArgumentException)
                {
                    // Fall back to TextRenderer if Graphics.DrawString fails
                    try
                    {
                        TextRenderer.DrawText(g, text, _grid.Font, rect, foreColor,
                            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | 
                            TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                    }
                    catch
                    {
                        // If all else fails, silently skip
                    }
                }
            }
        }

        /// <summary>
        /// Calculate how many rows can fit in the available height starting from a specific row
        /// </summary>
        private int GetVisibleRowCount(BindingList<BeepRowConfig> rows, int availableHeight, int startRow, int pixelOffset)
        {
            if (rows == null || rows.Count == 0) return 0;
            
            int visibleCount = 0;
            int usedHeight = 0;
            
            // Account for partial visibility of the first row
            if (startRow < rows.Count)
            {
                var firstRow = rows[startRow];
                int firstRowHeight = firstRow.Height > 0 ? firstRow.Height : _grid.RowHeight;
                
                // Calculate how much of the first row is visible
                int totalOffsetToFirstRow = 0;
                for (int i = 0; i < startRow; i++)
                {
                    var row = rows[i];
                    totalOffsetToFirstRow += row.Height > 0 ? row.Height : _grid.RowHeight;
                }
                
                int firstRowVisibleHeight = firstRowHeight - (pixelOffset - totalOffsetToFirstRow);
                if (firstRowVisibleHeight > 0)
                {
                    usedHeight += firstRowVisibleHeight;
                    visibleCount++;
                }
                
                // Add remaining rows
                for (int i = startRow + 1; i < rows.Count && usedHeight < availableHeight; i++)
                {
                    var row = rows[i];
                    int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                    
                    if (usedHeight + rowHeight > availableHeight)
                        break;
                        
                    usedHeight += rowHeight;
                    visibleCount++;
                }
            }
            
            return visibleCount;
        }

        private void DrawNavigatorArea(Graphics g)
        {
            var navRect = _grid.Layout.NavigatorRect;
            if (navRect.IsEmpty) return;

            // Fill navigator background
            using (var brush = new SolidBrush(Theme?.GridHeaderBackColor ?? SystemColors.Control))
            {
                g.FillRectangle(brush, navRect);
            }

            // Draw navigator border
            using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
            {
                g.DrawLine(pen, navRect.Left, navRect.Top, navRect.Right, navRect.Top);
            }
        }

        private void DrawSelectionIndicators(Graphics g)
        {
            // Draw selection indicators (can be enhanced later)
        }
    }
}

