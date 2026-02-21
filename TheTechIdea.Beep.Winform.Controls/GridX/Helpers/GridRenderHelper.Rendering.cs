using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Rendering and drawing methods for grid visualization
    /// </summary>
    internal partial class GridRenderHelper
    {
        /// <summary>
        /// Main draw method - coordinates all drawing operations
        /// </summary>
        public void Draw(Graphics g)
        {
            if (g == null || _grid == null || _grid.Layout == null)
                return;

            var rowsRect = _grid.Layout.RowsRect;
            if (rowsRect.Width <= 0 || rowsRect.Height <= 0)
                return;

            // Draw background
            var gridBackColor = Theme?.GridBackColor ?? SystemColors.Window;
            using (var brush = new SolidBrush(gridBackColor))
            {
                g.FillRectangle(brush, rowsRect);
            }

            // Draw column headers
            if (_grid.ShowColumnHeaders)
            {
                if (_grid.ShowTopFilterPanel)
                {
                    try
                    {
                        DrawTopFilterPanel(g);
                    }
                    catch (Exception)
                    {
                        // Silently ignore top filter panel draw issues
                    }
                }
                else
                {
                    _topFilterCellRects.Clear();
                    _topFilterClearIconRects.Clear();
                }

                try
                {
                    DrawColumnHeaders(g);
                }
                catch (Exception)
                {
                    // Silently handle header drawing errors
                }
            }

            // Draw data rows
            try
            {
                DrawRows(g);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GridRenderHelper.DrawRows error: {ex.Message}");
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

            // Draw column reorder drag feedback
            if (_grid.AllowColumnReorder && _grid.ColumnReorder.IsDragging)
            {
                try
                {
                    _grid.ColumnReorder.DrawDragFeedback(g);
                }
                catch (Exception)
                {
                    // Silently handle drag feedback errors
                }
            }
        }

        /// <summary>
        /// Draws column headers with support for sticky columns
        /// </summary>
        private void DrawColumnHeaders(Graphics g)
        {
            var headerRect = _grid.Layout.HeaderRect;
            if (headerRect.Height <= 0 || headerRect.Width <= 0) return;

            _headerFilterIconRects.Clear();
            _headerSortIconRects.Clear();

            using (var brush = new SolidBrush(Theme?.GridHeaderBackColor ?? SystemColors.Control))
            {
                g.FillRectangle(brush, headerRect);
            }

            if (ShowGridLines)
            {
                using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
                {
                    pen.DashStyle = GridLineStyle;
                    g.DrawLine(pen, headerRect.Left, headerRect.Bottom, headerRect.Right, headerRect.Bottom);
                }
            }

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

            Rectangle stickyRegion = new Rectangle(headerRect.Left, headerRect.Top, stickyWidth, headerRect.Height);
            Rectangle scrollingRegion = new Rectangle(headerRect.Left + stickyWidth, headerRect.Top, 
                Math.Max(0, headerRect.Width - stickyWidth), headerRect.Height);

            // Draw scrolling headers
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
                        DrawHeaderCell(g, col, cellRect, i);
                }
            }
            g.Restore(state1);

            // Draw sticky headers
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
                        DrawHeaderCell(g, col, cellRect, i);
                }
            }
            g.Restore(state2);

            // Vertical separator after sticky section
            if (stickyWidth > 0 && ShowGridLines)
            {
                using var pen2 = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark);
                pen2.DashStyle = GridLineStyle;
                g.DrawLine(pen2, headerRect.Left + stickyWidth, headerRect.Top, headerRect.Left + stickyWidth, headerRect.Bottom);
            }
        }

        /// <summary>
        /// Draws the top filter panel
        /// </summary>
        private void DrawTopFilterPanel(Graphics g)
        {
            var panelRect = _grid.Layout.TopFilterRect;
            _topFilterCellRects.Clear();
            _topFilterClearIconRects.Clear();

            if (panelRect.Width <= 0 || panelRect.Height <= 0)
                return;

            var gridStyle = _grid.GridStyle;
            if (_filterPanelPainter == null || _filterPanelGridStyle != gridStyle)
            {
                _filterPanelPainter = FilterPanelPainterFactory.CreatePainterForGridStyle(gridStyle);
                _filterPanelGridStyle = gridStyle;
            }

            if (_filterPanelPainter == null)
                return;

            _filterPanelPainter.PaintFilterPanel(g, panelRect, _grid, Theme, _topFilterCellRects, _topFilterClearIconRects);
        }

        /// <summary>
        /// Draws a filter icon (funnel shape)
        /// </summary>
        private void DrawFilterIcon(Graphics g, Rectangle rect, bool active)
        {
            Color iconColor = active ? Color.DodgerBlue : (Theme?.GridHeaderForeColor ?? SystemColors.ControlText);
            
            using (Pen pen = new Pen(active ? iconColor : Color.FromArgb(180, iconColor), active ? 1.6f : 1.25f))
            using (Brush brush = new SolidBrush(active ? iconColor : Color.FromArgb(120, iconColor)))
            {
                int padding = 1;
                int funnelWidth = rect.Width - (padding * 2);
                int funnelHeight = rect.Height - (padding * 2);

                Point[] funnel = {
                    new Point(rect.X + padding, rect.Y + padding),
                    new Point(rect.Right - padding, rect.Y + padding),
                    new Point(rect.X + rect.Width / 2 + 2, rect.Y + funnelHeight / 2),
                    new Point(rect.X + rect.Width / 2 + 2, rect.Bottom - padding),
                    new Point(rect.X + rect.Width / 2 - 2, rect.Bottom - padding),
                    new Point(rect.X + rect.Width / 2 - 2, rect.Y + funnelHeight / 2)
                };

                if (active)
                {
                    g.FillPolygon(brush, funnel);
                }
                else
                {
                    g.FillPolygon(brush, funnel);
                    g.DrawPolygon(pen, funnel);
                }

                if (active)
                {
                    using (Brush dotBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
                    {
                        int dotSize = 2;
                        g.FillEllipse(dotBrush, rect.X + rect.Width / 2 - dotSize / 2,
                            rect.Y + rect.Height / 2 - dotSize / 2, dotSize, dotSize);
                    }
                }
            }
        }

        /// <summary>
        /// Draws sort indicator arrows
        /// </summary>
        private void DrawSortIndicator(Graphics g, Rectangle rect, TheTechIdea.Beep.Vis.Modules.SortDirection sortDirection)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            var color = Theme?.GridHeaderForeColor ?? SystemColors.ControlText;

            using (Pen pen = new Pen(color, 1.5f))
            using (Brush brush = new SolidBrush(color))
            {
                int centerX = rect.X + rect.Width / 2;
                int centerY = rect.Y + rect.Height / 2;
                int arrowSize = Math.Min(rect.Width, rect.Height) / 3;

                if (sortDirection == TheTechIdea.Beep.Vis.Modules.SortDirection.Ascending)
                {
                    Point[] upArrow = {
                        new Point(centerX, rect.Y + 2),
                        new Point(centerX - arrowSize, centerY + 1),
                        new Point(centerX + arrowSize, centerY + 1)
                    };
                    g.FillPolygon(brush, upArrow);
                }
                else if (sortDirection == TheTechIdea.Beep.Vis.Modules.SortDirection.Descending)
                {
                    Point[] downArrow = {
                        new Point(centerX, rect.Bottom - 2),
                        new Point(centerX - arrowSize, centerY - 1),
                        new Point(centerX + arrowSize, centerY - 1)
                    };
                    g.FillPolygon(brush, downArrow);
                }
                else
                {
                    using (Pen lightPen = new Pen(Color.FromArgb(100, color), 1))
                    {
                        Point[] upArrow = {
                            new Point(centerX, rect.Y + 2),
                            new Point(centerX - arrowSize + 1, centerY),
                            new Point(centerX + arrowSize - 1, centerY)
                        };
                        Point[] downArrow = {
                            new Point(centerX, rect.Bottom - 2),
                            new Point(centerX - arrowSize + 1, centerY),
                            new Point(centerX + arrowSize - 1, centerY)
                        };
                        g.DrawPolygon(lightPen, upArrow);
                        g.DrawPolygon(lightPen, downArrow);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a single header cell
        /// </summary>
        private void DrawHeaderCell(Graphics g, BeepColumnConfig column, Rectangle cellRect, int columnIndex)
        {
            if (g == null || column == null || cellRect.Width <= 0 || cellRect.Height <= 0)
                return;

            bool isHovered = UseHeaderHoverEffects && _grid.Layout.HoveredHeaderColumnIndex == columnIndex;

            // Background with gradient support
            if (UseHeaderGradient)
            {
                var baseColor = Theme?.GridHeaderBackColor ?? SystemColors.Control;
                var lightColor = Color.FromArgb(
                    Math.Min(255, baseColor.R + 20),
                    Math.Min(255, baseColor.G + 20),
                    Math.Min(255, baseColor.B + 20)
                );

                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(cellRect,
                    isHovered ? lightColor : baseColor, isHovered ? baseColor : lightColor,
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, cellRect);
                }
            }
            else
            {
                var backColor = Theme?.GridHeaderBackColor ?? SystemColors.Control;
                if (isHovered && UseHeaderHoverEffects)
                {
                    backColor = Color.FromArgb(
                        Math.Min(255, backColor.R + 15),
                        Math.Min(255, backColor.G + 15),
                        Math.Min(255, backColor.B + 15)
                    );
                }

                using (var brush = new SolidBrush(backColor))
                {
                    g.FillRectangle(brush, cellRect);
                }
            }

            // Elevation effect
            if (UseElevation)
            {
                using (var shadowPen = new Pen(Color.FromArgb(40, 0, 0, 0), 1))
                {
                    g.DrawLine(shadowPen, cellRect.Left + 1, cellRect.Bottom, cellRect.Right - 1, cellRect.Bottom);
                    g.DrawLine(shadowPen, cellRect.Right, cellRect.Top + 1, cellRect.Right, cellRect.Bottom - 1);
                }
            }

            // Text rendering
            var textColor = Theme?.GridHeaderForeColor ?? SystemColors.ControlText;
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;

            var baseFont = GetSafeHeaderFont();
            var font = UseBoldHeaderText ? new Font(baseFont.FontFamily, baseFont.Size, FontStyle.Bold) : baseFont;

            bool isSystemColumn = column.IsSelectionCheckBox || column.IsRowNumColumn || column.IsRowID;
            bool hasSortArea = ShowSortIndicators && SortIconVisibility != HeaderIconVisibility.Hidden && !isSystemColumn && column.AllowSort;
            bool hasFilterArea = !_grid.ShowTopFilterPanel && FilterIconVisibility != HeaderIconVisibility.Hidden && !isSystemColumn;

            int sortIconSize = hasSortArea ? Math.Min(cellRect.Height - HeaderCellPadding * 2, 14) : 0;
            int filterIconSize = hasFilterArea ? Math.Min(cellRect.Height - HeaderCellPadding * 2, 14) : 0;
            int rightX = cellRect.Right - HeaderCellPadding;

            Rectangle filterIconRect = Rectangle.Empty;
            if (hasFilterArea && filterIconSize > 0)
            {
                filterIconRect = new Rectangle(rightX - filterIconSize, cellRect.Top + HeaderCellPadding, filterIconSize, filterIconSize);
                rightX -= filterIconSize + HeaderCellPadding;
            }

            Rectangle sortIconRect = Rectangle.Empty;
            if (hasSortArea && sortIconSize > 0)
            {
                sortIconRect = new Rectangle(rightX - sortIconSize, cellRect.Top + HeaderCellPadding, sortIconSize, sortIconSize);
                rightX -= sortIconSize + HeaderCellPadding;
            }

            var textRect = new Rectangle(cellRect.X + HeaderCellPadding, cellRect.Y + HeaderCellPadding,
                Math.Max(1, rightX - cellRect.X - HeaderCellPadding),
                Math.Max(1, cellRect.Height - HeaderCellPadding * 2)
            );

            if (!string.IsNullOrEmpty(text))
            {
                var flags = GetTextFormatFlagsForAlignment(column.HeaderTextAlignment, true);
                TextRenderer.DrawText(g, text, font, textRect, textColor, flags);
            }

            // Draw sort icon
            bool showSortIcon = hasSortArea && !sortIconRect.IsEmpty &&
                (SortIconVisibility == HeaderIconVisibility.Always ||
                (SortIconVisibility == HeaderIconVisibility.HoverOnly && (isHovered || (column.IsSorted && column.ShowSortIcon))));

            if (showSortIcon)
            {
                var direction = (column.IsSorted && column.ShowSortIcon) ? column.SortDirection : TheTechIdea.Beep.Vis.Modules.SortDirection.None;
                DrawSortIndicator(g, sortIconRect, direction);
                _headerSortIconRects[columnIndex] = sortIconRect;
            }
            else
            {
                _headerSortIconRects.Remove(columnIndex);
            }

            // Draw filter icon
            bool showFilterIcon = hasFilterArea && !filterIconRect.IsEmpty &&
                (FilterIconVisibility == HeaderIconVisibility.Always ||
                (FilterIconVisibility == HeaderIconVisibility.HoverOnly && (isHovered || column.IsFiltered)));

            if (showFilterIcon)
            {
                DrawFilterIcon(g, filterIconRect, column.IsFiltered);
                _headerFilterIconRects[columnIndex] = filterIconRect;
            }
            else
            {
                _headerFilterIconRects.Remove(columnIndex);
            }

            // Border
            if (ShowGridLines)
            {
                using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
                {
                    pen.DashStyle = GridLineStyle;
                    g.DrawRectangle(pen, cellRect);
                }
            }

            if (UseBoldHeaderText)
                font.Dispose();
        }

        /// <summary>
        /// Draws all visible data rows with support for sticky columns and virtualization
        /// </summary>
        private void DrawRows(Graphics g)
        {
            var rowsRect = _grid.Layout.RowsRect;
            
            var theme = Theme;
            var gridBackColor = theme?.GridBackColor ?? SystemColors.Window;
            var gridForeColor = theme?.GridForeColor ?? SystemColors.WindowText;
            var gridLineColor = theme?.GridLineColor ?? SystemColors.ControlDark;
            var selectedBackColor = (theme?.GridRowSelectedBackColor == Color.Empty || theme?.GridRowSelectedBackColor == null) ? 
                (theme?.SelectedRowBackColor ?? SystemColors.Highlight) : (theme?.GridRowSelectedBackColor ?? SystemColors.Highlight);
            var hoverBackColor = (theme?.GridRowHoverBackColor == Color.Empty || theme?.GridRowHoverBackColor == null) ? 
                SystemColors.ControlLight : (theme?.GridRowHoverBackColor ?? SystemColors.ControlLight);
            var altRowBackColor = theme?.AltRowBackColor ?? Color.FromArgb(250, 250, 250);
            var focusColor = ResolveThemeFocusColor();
            var focusedRowBackColor = ResolveFocusedRowBackColor(hoverBackColor, focusColor);
         
            using var gridLinePen = new Pen(gridLineColor);
            gridLinePen.DashStyle = GridLineStyle;
            using var shadowPen = UseElevation ? new Pen(Color.FromArgb(30, 0, 0, 0), 1) : null;
            using var cardPen = CardStyle ? new Pen(Color.FromArgb(40, gridLineColor), 1) : null;

            var stickyColumns = _grid.Data.Columns.Where(c => c.Sticked && c.Visible).ToList();
            int stickyWidth = stickyColumns.Sum(c => Math.Max(20, c.Width));
            stickyWidth = Math.Min(stickyWidth, rowsRect.Width);

            int currentY = rowsRect.Top;
            int firstVisibleRowIndex = _grid.Scroll.FirstVisibleRowIndex;
            int totalRowsHeight = 0;
           
            for (int i = 0; i < firstVisibleRowIndex && i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                if (!row.IsVisible) continue;  // skip filtered-out rows
                totalRowsHeight += row.Height > 0 ? row.Height : _grid.RowHeight;
            }
            
            int pixelOffset = _grid.Scroll.VerticalOffset;
            currentY = rowsRect.Top - (pixelOffset - totalRowsHeight);

            int visibleRowStart = firstVisibleRowIndex;
            int visibleRowEnd = Math.Min(_grid.Data.Rows.Count - 1, 
                visibleRowStart + GetVisibleRowCount(_grid.Data.Rows, rowsRect.Height, visibleRowStart, pixelOffset));

            Rectangle stickyRegion = new Rectangle(rowsRect.Left, rowsRect.Top, stickyWidth, rowsRect.Height);
            Rectangle scrollingRegion = new Rectangle(rowsRect.Left + stickyWidth, rowsRect.Top, 
                Math.Max(0, rowsRect.Width - stickyWidth), rowsRect.Height);

            var scrollCols = _grid.Data.Columns.Select((c, idx) => new { Col = c, Index = idx })
                .Where(x => x.Col.Visible && !x.Col.Sticked)
                .OrderBy(x => x.Col.DisplayOrder)
                .ToList();
            
            var scrollState = g.Save();
            g.SetClip(scrollingRegion);
            
            int drawY = currentY;
            for (int r = visibleRowStart; r <= visibleRowEnd && r < _grid.Data.Rows.Count; r++)
            {
                var row = _grid.Data.Rows[r];
                if (!row.IsVisible) continue;  // skip filtered-out rows
                int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                
                if (drawY + rowHeight > rowsRect.Top && drawY < rowsRect.Bottom)
                {
                    bool isActiveRow = (_grid.Selection?.RowIndex ?? -1) == r;
                    bool isSelectedRow = row.IsSelected;
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
                                cell.Rect = rect;

                                Color back = isSelectedRow ? selectedBackColor : 
                                           isActiveRow ? (UseDedicatedFocusedRowStyle ? focusedRowBackColor : hoverBackColor) :
                                           ShowRowStripes && r % 2 == 1 ? altRowBackColor :
                                           sc.Col.HasCustomBackColor && sc.Col.UseCustomColors ? sc.Col.ColumnBackColor : gridBackColor;

                                Color fore = sc.Col.HasCustomForeColor && sc.Col.UseCustomColors ? sc.Col.ColumnForeColor : gridForeColor;

                                using (var bg = new SolidBrush(back)) 
                                    g.FillRectangle(bg, rect);

                                if (shadowPen != null && !isSelectedRow && !isActiveRow)
                                {
                                    g.DrawLine(shadowPen, rect.Left + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
                                    g.DrawLine(shadowPen, rect.Right, rect.Top + 1, rect.Right, rect.Bottom - 1);
                                }

                                if (cardPen != null && !isSelectedRow && !isActiveRow)
                                    g.DrawRectangle(cardPen, rect);

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

                                if (ShowGridLines)
                                    g.DrawRectangle(gridLinePen, rect);
                            }
                        }
                        x += colW;
                        if (x > scrollingRegion.Right) break;
                    }
                }
                drawY += rowHeight;
            }
            
            g.Restore(scrollState);

            var stickyCols = _grid.Data.Columns.Select((c, idx) => new { Col = c, Index = idx })
                .Where(x => x.Col.Visible && x.Col.Sticked)
                .OrderBy(x => x.Col.DisplayOrder)
                .ToList();
            
            var stickyState = g.Save();
            g.SetClip(stickyRegion);
            
            int startX = rowsRect.Left;
            foreach (var st in stickyCols)
            {
                int colW = Math.Max(20, st.Col.Width);
                drawY = currentY;
                
                for (int r = visibleRowStart; r <= visibleRowEnd && r < _grid.Data.Rows.Count; r++)
                {
                    var row = _grid.Data.Rows[r];
                    if (!row.IsVisible) continue;  // skip filtered-out rows
                    int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                    
                    if (drawY + rowHeight > rowsRect.Top && drawY < rowsRect.Bottom)
                    {
                        if (st.Index < row.Cells.Count)
                        {
                            bool isActiveRow = (_grid.Selection?.RowIndex ?? -1) == r;
                            bool isSelectedRow = row.IsSelected;

                            var cell = row.Cells[st.Index];
                            var rect = new Rectangle(startX, drawY, colW, rowHeight);
                            cell.Rect = rect;

                            Color back = isSelectedRow ? selectedBackColor : 
                                       isActiveRow ? (UseDedicatedFocusedRowStyle ? focusedRowBackColor : hoverBackColor) :
                                       ShowRowStripes && r % 2 == 1 ? altRowBackColor :
                                       st.Col.HasCustomBackColor && st.Col.UseCustomColors ? st.Col.ColumnBackColor : gridBackColor;

                            Color fore = st.Col.HasCustomForeColor && st.Col.UseCustomColors ? st.Col.ColumnForeColor : gridForeColor;

                            using (var bg = new SolidBrush(back)) 
                                g.FillRectangle(bg, rect);

                            if (shadowPen != null && !isSelectedRow && !isActiveRow)
                            {
                                g.DrawLine(shadowPen, rect.Left + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
                                g.DrawLine(shadowPen, rect.Right, rect.Top + 1, rect.Right, rect.Bottom - 1);
                            }

                            if (cardPen != null && !isSelectedRow && !isActiveRow)
                                g.DrawRectangle(cardPen, rect);

                            if (st.Col.IsSelectionCheckBox && _grid.ShowCheckBox)
                            {
                                _rowCheck ??= new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme };
                                _rowCheck.CurrentValue = row.IsSelected;
                                _rowCheck.Draw(g, rect);
                            }
                            else if (st.Col.IsRowNumColumn)
                            {
                                var font = GetSafeCellFont();
                                TextRenderer.DrawText(g, cell.CellValue?.ToString() ?? string.Empty, font, rect, fore,
                                    TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                            }
                            else
                            {
                                DrawCellContent(g, st.Col, cell, rect, fore, back);
                            }

                            if (ShowGridLines)
                                g.DrawRectangle(gridLinePen, rect);
                        }
                    }
                    drawY += rowHeight;
                }
                startX += colW;
            }
            
            g.Restore(stickyState);
        }

        /// <summary>
        /// Draws selection indicators and focus rectangle
        /// </summary>
        private void DrawSelectionIndicators(Graphics g)
        {
            if (g == null || _grid?.Selection == null || _grid.Data?.Rows == null)
                return;

            int rowIndex = _grid.Selection.RowIndex;
            int colIndex = _grid.Selection.ColumnIndex;
            if (rowIndex < 0 || rowIndex >= _grid.Data.Rows.Count)
                return;
            if (colIndex < 0 || colIndex >= _grid.Data.Columns.Count)
                return;

            var row = _grid.Data.Rows[rowIndex];
            if (colIndex >= row.Cells.Count)
                return;

            var cell = row.Cells[colIndex];
            if (cell.Rect.IsEmpty || cell.Rect.Width <= 0 || cell.Rect.Height <= 0)
                return;

            var rowsRect = _grid.Layout.RowsRect;
            var focusRect = Rectangle.Intersect(rowsRect, cell.Rect);
            if (focusRect.IsEmpty)
                return;

            var focusColor = ResolveThemeFocusColor();
            var state = g.Save();
            g.SetClip(rowsRect);

            if (ShowFocusedCellFill)
            {
                var fillBase = FocusedCellFillColor == Color.Empty ? focusColor : FocusedCellFillColor;
                int alpha = Math.Max(0, Math.Min(255, FocusedCellFillOpacity));
                using var fillBrush = new SolidBrush(Color.FromArgb(alpha, fillBase.R, fillBase.G, fillBase.B));
                g.FillRectangle(fillBrush, focusRect);
            }

            if (ShowFocusedCellBorder && FocusedCellBorderWidth > 0f)
            {
                var borderColor = FocusedCellBorderColor == Color.Empty ? focusColor : FocusedCellBorderColor;
                using var borderPen = new Pen(borderColor, FocusedCellBorderWidth)
                {
                    Alignment = System.Drawing.Drawing2D.PenAlignment.Inset
                };
                var borderRect = Rectangle.Inflate(focusRect, -1, -1);
                if (borderRect.Width > 0 && borderRect.Height > 0)
                    g.DrawRectangle(borderPen, borderRect);
            }

            g.Restore(state);
        }

        /// <summary>
        /// Draws navigator area
        /// </summary>
        private void DrawNavigatorArea(Graphics g)
        {
            _grid.NavigatorPainter.DrawNavigatorArea(g);
        }
    }
}
