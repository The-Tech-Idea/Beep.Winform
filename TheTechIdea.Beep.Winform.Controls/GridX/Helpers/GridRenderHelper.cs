using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls; // BeepButton and other controls

using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Helpers; // BeepSvgPaths
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridRenderHelper
    {
        private readonly BeepGridPro _grid;
        private BeepCheckBoxBool _rowCheck;

        // Cache drawers per column (like BeepSimpleGrid)
        private readonly Dictionary<string, BeepControl> _columnDrawerCache = new();

        // Navigator buttons (owner-drawn)
        private BeepButton _btnFirst;
        private BeepButton _btnPrev;
        private BeepButton _btnNext;
        private BeepButton _btnLast;
        private BeepButton _btnInsert;
        private BeepButton _btnDelete;
        private BeepButton _btnSave;
        private BeepButton _btnCancel;
        private BeepButton _btnQuery;
        private BeepButton _btnFilter;
        private BeepButton _btnPrint;

        public GridRenderHelper(BeepGridPro grid)
        {
            _grid = grid;
        }

        internal IBeepTheme Theme => _grid.Theme != null ? BeepThemesManager.GetTheme(_grid.Theme) : BeepThemesManager.GetDefaultTheme();

        // Create or get cached drawer for a given column
        private BeepControl GetDrawerForColumn(BeepColumnConfig col)
        {
            if (col == null) return null;
            string key = col.ColumnName ?? col.ColumnCaption ?? col.GuidID ?? col.GetHashCode().ToString();
            if (_columnDrawerCache.TryGetValue(key, out var cached) && cached != null)
            {
                return cached;
            }

            BeepControl drawer = col.CellEditor switch
            {
                BeepColumnType.CheckBoxBool => new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true },
                BeepColumnType.CheckBoxChar => new BeepCheckBoxChar { IsChild = true, GridMode = true, HideText = true },
                BeepColumnType.CheckBoxString => new BeepCheckBoxString { IsChild = true, GridMode = true, HideText = true },
                BeepColumnType.ComboBox => new BeepComboBox { IsChild = true, GridMode = true },
                BeepColumnType.DateTime => new BeepDatePicker { IsChild = true, GridMode = true },
                BeepColumnType.Image => new BeepImage { IsChild = true },
                BeepColumnType.Button => new BeepButton { IsChild = true, IsFrameless = true },
                BeepColumnType.ProgressBar => new BeepProgressBar { IsChild = true },
                BeepColumnType.NumericUpDown => new BeepNumericUpDown { IsChild = true, GridMode = true },
                BeepColumnType.Radio => new BeepRadioButton { IsChild = true, GridMode = true },
                BeepColumnType.ListBox => new BeepListBox { IsChild = true, GridMode = true },
                BeepColumnType.ListOfValue => new BeepListofValuesBox { IsChild = true, GridMode = true },
                BeepColumnType.Text => new BeepTextBox { IsChild = true, GridMode = true, IsFrameless = true, ShowAllBorders = false },
                _ => new BeepTextBox { IsChild = true, GridMode = true, IsFrameless = true, ShowAllBorders = false }
            };

            drawer.Theme = _grid.Theme;
            _columnDrawerCache[key] = drawer;
            return drawer;
        }

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

            using (var brush = new SolidBrush(Theme?.GridHeaderBackColor ?? SystemColors.Control))
            {
                g.FillRectangle(brush, headerRect);
            }
            using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
            {
                g.DrawLine(pen, headerRect.Left, headerRect.Bottom, headerRect.Right, headerRect.Bottom);
            }

            // Optional: draw select-all checkbox visual in header if enabled
            var selColumn = _grid.Data.Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (_grid.ShowCheckBox && selColumn != null && _grid.Layout.SelectAllCheckRect != Rectangle.Empty)
            {
                bool allSelected = _grid.Rows.Count > 0 && _grid.Rows.All(r => r.IsSelected);
                var chk = new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme, CurrentValue = allSelected };
                chk.Draw(g, _grid.Layout.SelectAllCheckRect);
            }

            var stickyColumns = _grid.Data.Columns.Where(c => c.Sticked && c.Visible).ToList();
            int stickyWidth = stickyColumns.Sum(c => Math.Max(20, c.Width));
            stickyWidth = Math.Min(stickyWidth, headerRect.Width);

            // Define regions
            Rectangle stickyRegion = new Rectangle(headerRect.Left, headerRect.Top, stickyWidth, headerRect.Height);
            Rectangle scrollingRegion = new Rectangle(headerRect.Left + stickyWidth, headerRect.Top, Math.Max(0, headerRect.Width - stickyWidth), headerRect.Height);

            // Draw non-sticky (scrolling) headers in scrolling clip first
            var state1 = g.Save();
            g.SetClip(scrollingRegion);
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                var col = _grid.Data.Columns[i];
                if (!col.Visible || col.Sticked) continue;
                if (i < _grid.Layout.HeaderCellRects.Length)
                {
                    var cellRect = _grid.Layout.HeaderCellRects[i];
                    if (cellRect.Width > 0 && cellRect.Height > 0)
                        DrawHeaderCell(g, col, cellRect);
                }
            }
            g.Restore(state1);

            // Draw sticky headers on top within sticky clip
            var state2 = g.Save();
            g.SetClip(stickyRegion);
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                var col = _grid.Data.Columns[i];
                if (!col.Visible || !col.Sticked) continue;
                if (i < _grid.Layout.HeaderCellRects.Length)
                {
                    var cellRect = _grid.Layout.HeaderCellRects[i];
                    if (cellRect.Width > 0 && cellRect.Height > 0)
                        DrawHeaderCell(g, col, cellRect);
                }
            }
            g.Restore(state2);

            // Vertical separator after sticky section
            if (stickyWidth > 0)
            {
                using var pen2 = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark);
                g.DrawLine(pen2, headerRect.Left + stickyWidth, headerRect.Top, headerRect.Left + stickyWidth, headerRect.Bottom);
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

            // Text - with robust TextRenderer (avoids GDI+ DrawString issues)
            var textColor = Theme?.GridHeaderForeColor ?? SystemColors.ControlText;
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            var font = _grid?.Font ?? SystemFonts.DefaultFont;

            if (!string.IsNullOrEmpty(text))
            {
                var textRect = new Rectangle(
                    cellRect.X + 2,
                    cellRect.Y + 2,
                    Math.Max(1, cellRect.Width - 4),
                    Math.Max(1, cellRect.Height - 4)
                );
                TextRenderer.DrawText(
                    g,
                    text,
                    font,
                    textRect,
                    textColor,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis |
                    TextFormatFlags.NoPrefix
                );
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

            // calculate Y offset properly
            int currentY = rowsRect.Top;
            int firstVisibleRowIndex = _grid.Scroll.FirstVisibleRowIndex;
            
            int totalRowsHeight = 0;
            for (int i = 0; i < firstVisibleRowIndex && i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                totalRowsHeight += row.Height > 0 ? row.Height : _grid.RowHeight;
            }
            
            int pixelOffset = _grid.Scroll.VerticalOffset;
            currentY = rowsRect.Top - (pixelOffset - totalRowsHeight);

            // Calculate which rows are actually visible
            int visibleRowStart = firstVisibleRowIndex;
            int visibleRowEnd = Math.Min(_grid.Data.Rows.Count - 1, 
                visibleRowStart + GetVisibleRowCount(_grid.Data.Rows, rowsRect.Height, visibleRowStart, pixelOffset));

            // Define sticky and scrolling regions
            Rectangle stickyRegion = new Rectangle(rowsRect.Left, rowsRect.Top, stickyWidth, rowsRect.Height);
            Rectangle scrollingRegion = new Rectangle(rowsRect.Left + stickyWidth, rowsRect.Top, 
                                                     Math.Max(0, rowsRect.Width - stickyWidth), rowsRect.Height);

            // Draw scrolling columns first
            var state1 = g.Save();
            g.SetClip(scrollingRegion);
            var scrollCols = _grid.Data.Columns.Select((c, idx) => new { Col = c, Index = idx })
                                               .Where(x => x.Col.Visible && !x.Col.Sticked)
                                               .ToList();
            
            int drawY = currentY;
            for (int r = visibleRowStart; r <= visibleRowEnd && r < _grid.Data.Rows.Count; r++)
            {
                var row = _grid.Data.Rows[r];
                int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                
                if (drawY + rowHeight > rowsRect.Top && drawY < rowsRect.Bottom)
                {
                    bool isActiveRow = (_grid.Selection?.RowIndex ?? -1) == r; // highlight only
                    bool isSelectedRow = row.IsSelected; // selection only from checkbox

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

                                // Store rect for hit-testing and editor placement
                                cell.Rect = rect;

                                var back = isSelectedRow
                                    ? (Theme?.GridRowSelectedBackColor == Color.Empty ? (Theme?.SelectedRowBackColor ?? SystemColors.Highlight) : Theme.GridRowSelectedBackColor)
                                    : (isActiveRow ? (Theme?.GridRowHoverBackColor == Color.Empty ? SystemColors.ControlLight : Theme.GridRowHoverBackColor)
                                                   : (sc.Col.HasCustomBackColor && sc.Col.UseCustomColors ? sc.Col.ColumnBackColor : (Theme?.GridBackColor ?? SystemColors.Window)));
                                var fore = sc.Col.HasCustomForeColor && sc.Col.UseCustomColors ? sc.Col.ColumnForeColor : (Theme?.GridForeColor ?? SystemColors.WindowText);

                                using (var bg = new SolidBrush(back)) g.FillRectangle(bg, rect);

                                if (sc.Col.IsSelectionCheckBox && _grid.ShowCheckBox)
                                {
                                    _rowCheck ??= new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme };
                                    _rowCheck.CurrentValue = row.IsSelected;
                                    _rowCheck.Draw(g, rect);
                                }
                                else
                                {
                                    DrawCellContent(g, sc.Col, cell, rect, fore, back);
                                }

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

            // Draw sticky columns last
            var state2 = g.Save();
            g.SetClip(stickyRegion);
            var stickyCols = _grid.Data.Columns.Select((c, idx) => new { Col = c, Index = idx })
                                               .Where(x => x.Col.Visible && x.Col.Sticked)
                                               .ToList();
            int startX = rowsRect.Left;
            foreach (var st in stickyCols)
            {
                int colW = Math.Max(20, st.Col.Width);
                drawY = currentY;
                
                for (int r = visibleRowStart; r <= visibleRowEnd && r < _grid.Data.Rows.Count; r++)
                {
                    var row = _grid.Data.Rows[r];
                    int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                    
                    if (drawY + rowHeight > rowsRect.Top && drawY < rowsRect.Bottom)
                    {
                        if (st.Index < row.Cells.Count)
                        {
                            bool isActiveRow = (_grid.Selection?.RowIndex ?? -1) == r;
                            bool isSelectedRow = row.IsSelected;

                            var cell = row.Cells[st.Index];
                            var rect = new Rectangle(startX, drawY, colW, rowHeight);

                            // Store rect for hit-testing and editor placement
                            cell.Rect = rect;

                            var back = isSelectedRow
                                ? (Theme?.GridRowSelectedBackColor == Color.Empty ? (Theme?.SelectedRowBackColor ?? SystemColors.Highlight) : Theme.GridRowSelectedBackColor)
                                : (isActiveRow ? (Theme?.GridRowHoverBackColor == Color.Empty ? SystemColors.ControlLight : Theme.GridRowHoverBackColor)
                                               : (st.Col.HasCustomBackColor && st.Col.UseCustomColors ? st.Col.ColumnBackColor : (Theme?.GridBackColor ?? SystemColors.Window)));
                            var fore = st.Col.HasCustomForeColor && st.Col.UseCustomColors ? st.Col.ColumnForeColor : (Theme?.GridForeColor ?? SystemColors.WindowText);

                            using (var bg = new SolidBrush(back)) g.FillRectangle(bg, rect);

                            if (st.Col.IsSelectionCheckBox && _grid.ShowCheckBox)
                            {
                                _rowCheck ??= new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme };
                                _rowCheck.CurrentValue = row.IsSelected;
                                _rowCheck.Draw(g, rect);
                            }
                            else if (st.Col.IsRowNumColumn)
                            {
                                var font = _grid?.Font ?? SystemFonts.DefaultFont;
                                TextRenderer.DrawText(g, cell.CellValue?.ToString() ?? string.Empty, font, rect, fore,
                                    TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
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
            if (g == null || column == null || cell == null || rect.Width <= 0 || rect.Height <= 0)
                return;

            if (column.IsSelectionCheckBox)
            {
                _rowCheck ??= new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme };
                _rowCheck.CurrentValue = (bool)(cell.CellValue ?? false);
                _rowCheck.Draw(g, rect);
                return;
            }

            var drawer = GetDrawerForColumn(column);
            if (drawer == null)
            {
                string text = cell.CellValue?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(text))
                {
                    var font = BeepThemesManager.ToFont(BeepThemesManager.CurrentTheme.GridCellFont);
                    var textRect = new Rectangle(rect.X + 2, rect.Y + 1, Math.Max(1, rect.Width - 4), Math.Max(1, rect.Height - 2));
                    TextRenderer.DrawText(g, text, font, textRect, foreColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                }
                return;
            }

            drawer.Theme = _grid.Theme;
            drawer.BackColor = backColor;
            drawer.ForeColor = foreColor;
            drawer.Bounds = rect;

            // Populate list-based controls BEFORE setting value, so they can resolve SelectedItem
            if (drawer is BeepComboBox combo)
            {
                var items = GetFilteredItems(column, cell);
                combo.ListItems = new BindingList<SimpleItem>(items);
            }
            else if (drawer is BeepListBox listBox)
            {
                var items = GetFilteredItems(column, cell);
                listBox.ListItems = new BindingList<SimpleItem>(items);
            }
            else if (drawer is BeepListofValuesBox lov)
            {
                var items = GetFilteredItems(column, cell);
                lov.ListItems = new List<SimpleItem>(items);
            }

            if (drawer is IBeepUIComponent ic)
            {
                try { ic.SetValue(cell.CellValue); } catch { }
            }
            else if (drawer is BeepButton btn)
            {
                btn.Text = cell.CellValue?.ToString() ?? string.Empty;
            }

            // Draw via component to match BeepSimpleGrid look
            drawer.Draw(g, rect);
        }

        private List<SimpleItem> GetFilteredItems(BeepColumnConfig column, BeepCellConfig cell)
        {
            var baseItems = column?.Items ?? new List<SimpleItem>();
            if (baseItems == null || baseItems.Count == 0) return new List<SimpleItem>();

            // Simple parent filtering if configured
            if (!string.IsNullOrEmpty(column.ParentColumnName))
            {
                object parentValue = cell?.ParentCellValue;
                if (cell?.FilterdList != null && cell.FilterdList.Count > 0)
                {
                    return cell.FilterdList;
                }
                if (parentValue != null)
                {
                    return baseItems.Where(i => i.ParentValue?.ToString() == parentValue.ToString()).ToList();
                }
            }
            return baseItems.ToList();
        }

        /// <summary>
        /// Calculate how many rows can fit in the available height starting from a specific row
        /// </summary>
        private int GetVisibleRowCount(BindingList<BeepRowConfig> rows, int availableHeight, int startRow, int pixelOffset)
        {
            if (rows == null || rows.Count == 0) return 0;
            
            int visibleCount = 0;
            int usedHeight = 0;
            
            if (startRow < rows.Count)
            {
                var firstRow = rows[startRow];
                int firstRowHeight = firstRow.Height > 0 ? firstRow.Height : _grid.RowHeight;
                
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

        private void EnsureNavigatorButtons()
        {
            if (_btnFirst != null) return;

            // CRUD and actions
            _btnInsert = new BeepButton { ImagePath = BeepSvgPaths.NavPlus, Theme = _grid.Theme };
            _btnDelete = new BeepButton { ImagePath = BeepSvgPaths.NavMinus, Theme = _grid.Theme };
            _btnSave = new BeepButton { ImagePath = BeepSvgPaths.FloppyDisk, Theme = _grid.Theme };
            _btnCancel = new BeepButton { ImagePath = BeepSvgPaths.NavBackArrow, Theme = _grid.Theme };

            // Record navigation
            _btnFirst = new BeepButton { ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-double-small-left.svg", Theme = _grid.Theme };
            _btnPrev = new BeepButton { ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-left.svg", Theme = _grid.Theme };
            _btnNext = new BeepButton { ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-right.svg", Theme = _grid.Theme };
            _btnLast = new BeepButton { ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-double-small-right.svg", Theme = _grid.Theme };

            // Utilities
            _btnQuery = new BeepButton { ImagePath = BeepSvgPaths.NavSearch, Theme = _grid.Theme };
            _btnFilter = new BeepButton { ImagePath = BeepSvgPaths.NavWaving, Theme = _grid.Theme };
            _btnPrint = new BeepButton { ImagePath = BeepSvgPaths.NavPrinter, Theme = _grid.Theme };

            foreach (var btn in new[] { _btnInsert, _btnDelete, _btnSave, _btnCancel, _btnFirst, _btnPrev, _btnNext, _btnLast, _btnQuery, _btnFilter, _btnPrint })
                ConfigureIconButton(btn);
        }

        private void ConfigureIconButton(BeepButton btn)
        {
            if (btn == null) return;
            btn.HideText = true;
            btn.IsFrameless = true;
            btn.IsChild = true;
            btn.ApplyThemeOnImage = true;
            btn.UseThemeFont = true;
            btn.ShowAllBorders = false;
            btn.ShowShadow = false;
            btn.IsBorderAffectedByTheme = false;
            btn.IsShadowAffectedByTheme = false;
            btn.IsRoundedAffectedByTheme = false;
            btn.IsRounded = false;
            btn.ImageAlign = ContentAlignment.MiddleCenter;
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

            // Top border
            using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
            {
                g.DrawLine(pen, navRect.Left, navRect.Top, navRect.Right, navRect.Top);
            }

            // Ensure buttons exist and are themed
            EnsureNavigatorButtons();
            foreach (var btn in new[] { _btnFirst, _btnPrev, _btnNext, _btnLast, _btnInsert, _btnDelete, _btnSave, _btnCancel, _btnQuery, _btnFilter, _btnPrint })
            {
                if (btn == null) continue;
                btn.Theme = _grid.Theme;
                ConfigureIconButton(btn);
            }

            int buttonWidth = 24;
            int buttonHeight = 24;
            int padding = 6;
            int spacing = 4;
            int y = navRect.Top + (navRect.Height - buttonHeight) / 2;

            // Left CRUD
            int leftX = navRect.Left + padding;
            var insertRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var deleteRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var saveRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var cancelRect = new Rectangle(leftX, y, buttonWidth, buttonHeight);

            foreach (var tuple in new[] { (_btnInsert, insertRect), (_btnDelete, deleteRect), (_btnSave, saveRect), (_btnCancel, cancelRect) })
            {
                var b = tuple.Item1; var r = tuple.Item2;
                b.Size = r.Size; b.MaxImageSize = new Size(r.Width - 4, r.Height - 4);
                b.Draw(g, r);
            }

            // Center record counter and nav buttons
            string recordCounter = (_grid.Rows.Count > 0 && _grid.Selection?.RowIndex >= 0)
                ? $"{_grid.Selection.RowIndex + 1} - {_grid.Rows.Count}"
                : "0 - 0";
            var headerFont = _grid?.Font ?? SystemFonts.DefaultFont;
            Size textSize = TextRenderer.MeasureText(recordCounter, headerFont);
            float centerX = navRect.Left + (navRect.Width - textSize.Width) / 2f;

            var firstRect = new Rectangle((int)centerX - (buttonWidth * 2) - padding * 2, y, buttonWidth, buttonHeight);
            var prevRect = new Rectangle((int)centerX - buttonWidth - padding, y, buttonWidth, buttonHeight);
            var nextRect = new Rectangle((int)(centerX + textSize.Width + padding), y, buttonWidth, buttonHeight);
            var lastRect = new Rectangle(nextRect.Right + padding, y, buttonWidth, buttonHeight);

            foreach (var tuple in new[] { (_btnFirst, firstRect), (_btnPrev, prevRect), (_btnNext, nextRect), (_btnLast, lastRect) })
            {
                var b = tuple.Item1; var r = tuple.Item2;
                b.Size = r.Size; b.MaxImageSize = new Size(r.Width - 4, r.Height - 4);
                b.Draw(g, r);
            }

            TextRenderer.DrawText(g, recordCounter, headerFont, new Rectangle((int)centerX, y, textSize.Width, buttonHeight),
                Theme?.GridHeaderForeColor ?? SystemColors.ControlText,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            // Right utilities
            int rightX = navRect.Right - padding - buttonWidth;
            var printRect = new Rectangle(rightX, y, buttonWidth, buttonHeight); rightX -= buttonWidth + spacing;
            var filterRect = new Rectangle(rightX, y, buttonWidth, buttonHeight); rightX -= buttonWidth + spacing;
            var queryRect = new Rectangle(rightX, y, buttonWidth, buttonHeight);

            foreach (var tuple in new[] { (_btnQuery, queryRect), (_btnFilter, filterRect), (_btnPrint, printRect) })
            {
                var b = tuple.Item1; var r = tuple.Item2;
                b.Size = r.Size; b.MaxImageSize = new Size(r.Width - 4, r.Height - 4);
                b.Draw(g, r);
            }
        }

        private void DrawSelectionIndicators(Graphics g)
        {
            // Draw selection indicators (can be enhanced later)
        }
    }
}

