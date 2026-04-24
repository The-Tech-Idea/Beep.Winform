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
            var isDark = IsDarkTheme;
            var gridBackColor = Theme?.GridBackColor ?? (isDark ? Color.FromArgb(31, 41, 55) : Color.White);
            using (var brush = new SolidBrush(gridBackColor))
            {
                g.FillRectangle(brush, rowsRect);
            }

            // Draw toolbar (unified actions + search + filter + export)
            if (_grid.ShowToolbar && !_grid.Layout.ToolbarRect.IsEmpty)
            {
                try
                {
                    var state = _grid.ToolbarState;
                    state.IsFilterActive = _grid.IsFiltered;
                    state.ActiveFilterCount = _grid.ActiveFilter?.Criteria.Count ?? 0;
                    state.DpiScale = _grid.DeviceDpi / 96f;
                    state.CalculateLayout(_grid.Layout.ToolbarRect);
                    _grid.ToolbarPainter.Paint(g, _grid.Layout.ToolbarRect, state);
                }
                catch (Exception)
                {
                }
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

            // Draw focus indicator when the grid has keyboard focus
            try
            {
                _grid.FocusManager?.DrawFocusIndicator(g, _grid.DrawingRect);
            }
            catch (Exception)
            {
                // Silently handle focus indicator errors
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

            var isDark = IsDarkTheme;
            using (var brush = new SolidBrush(Theme?.GridHeaderBackColor ?? (isDark ? Color.FromArgb(45, 55, 72) : Color.FromArgb(240, 240, 240))))
            {
                g.FillRectangle(brush, headerRect);
            }

            if (ShowGridLines)
            {
                using (var pen = new Pen(Theme?.GridLineColor ?? (isDark ? Color.FromArgb(60, 70, 85) : Color.FromArgb(180, 180, 180))))
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
                using var pen2 = new Pen(Theme?.GridLineColor ?? (isDark ? Color.FromArgb(60, 70, 85) : Color.FromArgb(180, 180, 180)));
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
            var isDark = IsDarkTheme;
            Color iconColor = active ? Color.DodgerBlue : (Theme?.GridHeaderForeColor ?? (isDark ? Color.FromArgb(229, 231, 235) : Color.FromArgb(31, 41, 55)));
            
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

            var isDark = IsDarkTheme;
            var color = Theme?.GridHeaderForeColor ?? (isDark ? Color.FromArgb(229, 231, 235) : Color.FromArgb(31, 41, 55));

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
            var isDark = IsDarkTheme;

            // Background with gradient support
            if (UseHeaderGradient)
            {
                var baseColor = Theme?.GridHeaderBackColor ?? (isDark ? Color.FromArgb(45, 55, 72) : Color.FromArgb(240, 240, 240));
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
                var backColor = Theme?.GridHeaderBackColor ?? (isDark ? Color.FromArgb(45, 55, 72) : Color.FromArgb(240, 240, 240));
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
            var textColor = Theme?.GridHeaderForeColor ?? (isDark ? Color.FromArgb(229, 231, 235) : Color.FromArgb(31, 41, 55));
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
                using (var pen = new Pen(Theme?.GridLineColor ?? (isDark ? Color.FromArgb(60, 70, 85) : Color.FromArgb(180, 180, 180))))
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
            _groupHeaderRects.Clear();
            var rowsRect = _grid.Layout.RowsRect;
            
            var theme = Theme;
            var isDark = (theme?.GridBackColor != null && theme.GridBackColor != Color.Empty ? theme.GridBackColor : Color.White).GetBrightness() < 0.5;
            var gridBackColor = theme?.GridBackColor ?? (isDark ? Color.FromArgb(31, 41, 55) : Color.White);
            var gridForeColor = theme?.GridForeColor ?? (isDark ? Color.FromArgb(229, 231, 235) : Color.FromArgb(31, 41, 55));
            var gridLineColor = theme?.GridLineColor ?? (isDark ? Color.FromArgb(60, 70, 85) : Color.FromArgb(180, 180, 180));
            var selectedBackColor = (theme?.GridRowSelectedBackColor == Color.Empty || theme?.GridRowSelectedBackColor == null) ? 
                (theme?.PrimaryColor != Color.Empty && theme?.PrimaryColor != null ? theme!.PrimaryColor : Color.FromArgb(0, 120, 212)) : (theme?.GridRowSelectedBackColor ?? (theme?.PrimaryColor != Color.Empty && theme?.PrimaryColor != null ? theme!.PrimaryColor : Color.FromArgb(0, 120, 212)));
            var hoverBackColor = (theme?.GridRowHoverBackColor == Color.Empty || theme?.GridRowHoverBackColor == null) ? 
                (isDark ? Color.FromArgb(55, 65, 81) : Color.FromArgb(229, 229, 229)) : (theme?.GridRowHoverBackColor ?? (isDark ? Color.FromArgb(55, 65, 81) : Color.FromArgb(229, 229, 229)));
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
            if (_grid.EnableVirtualization && _grid.VirtualDataSource != null)
            {
                totalRowsHeight += (int)(_grid.RowVirtualizer.WindowStart * _grid.RowHeight);
            }
            else
            {
                // Account for group headers before the first visible row
                totalRowsHeight += (_grid.GroupEngine?.GetHeaderCountBeforeRow(firstVisibleRowIndex) ?? 0) * (_grid.GroupEngine?.GetHeaderHeight() ?? 0);
            }
            
            int pixelOffset = _grid.Scroll.VerticalOffset;
            currentY = rowsRect.Top - (pixelOffset - totalRowsHeight);

            var groupRenderer = new Grouping.GridGroupHeaderRenderer(_grid);
            int groupHeaderHeight = _grid.GroupEngine?.GetHeaderHeight() ?? 0;

            int visibleRowStart = firstVisibleRowIndex;
            int visibleRowEnd;
            if (_grid.EnableVirtualization && _grid.VirtualDataSource != null)
            {
                // In virtual mode Data.Rows is the visible window; draw all of it
                visibleRowEnd = _grid.Data.Rows.Count - 1;
            }
            else
            {
                visibleRowEnd = Math.Min(_grid.Data.Rows.Count - 1,
                    visibleRowStart + GetVisibleRowCount(_grid.Data.Rows, rowsRect.Height, visibleRowStart, pixelOffset));
            }

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
                if (!(_grid.EnableVirtualization && _grid.VirtualDataSource != null))
                    drawY += DrawGroupHeadersAtRow(g, r, drawY, rowsRect, groupRenderer, groupHeaderHeight);

                var row = _grid.Data.Rows[r];
                if (!row.IsVisible) continue;  // skip filtered-out rows
                int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                
                if (drawY + rowHeight > rowsRect.Top && drawY < rowsRect.Bottom)
                {
                    bool isActiveRow = (_grid.Selection?.RowIndex ?? -1) == r;
                    bool isSelectedRow = row.IsSelected;
                    int x = scrollingRegion.Left - _grid.Scroll.HorizontalOffset;

                    // If column virtualization is active, skip off-screen columns entirely
                    int firstScrollCol = 0;
                    int lastScrollCol = scrollCols.Count - 1;
                    if (_grid.EnableColumnVirtualization)
                    {
                        firstScrollCol = _grid.ColumnVirtualizer.FirstScrollingVisibleIndex;
                        lastScrollCol = _grid.ColumnVirtualizer.LastScrollingVisibleIndex;
                        // Advance x past skipped columns so coordinates are correct
                        for (int si = 0; si < firstScrollCol && si < scrollCols.Count; si++)
                            x += Math.Max(20, scrollCols[si].Col.Width);
                    }

                    for (int si = firstScrollCol; si <= lastScrollCol && si < scrollCols.Count; si++)
                    {
                        var sc = scrollCols[si];
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
                    if (!(_grid.EnableVirtualization && _grid.VirtualDataSource != null))
                        drawY += DrawGroupHeadersAtRow(g, r, drawY, rowsRect, groupRenderer, groupHeaderHeight);

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

            // ---- Summary row pass (after sticky + scrolling) ----
            if (!(_grid.EnableVirtualization && _grid.VirtualDataSource != null))
            {
                DrawSummaryRows(g, rowsRect, currentY, visibleRowStart, visibleRowEnd,
                    gridBackColor, gridForeColor, gridLinePen, stickyWidth, scrollingRegion);
            }
        }

        /// <summary>
        /// Draws group summary rows after their respective groups.
        /// Called once per <see cref="DrawRows"/> after both sticky and scrolling passes.
        /// </summary>
        private void DrawSummaryRows(Graphics g, Rectangle rowsRect, int currentY,
            int visibleRowStart, int visibleRowEnd,
            Color gridBackColor, Color gridForeColor,
            Pen gridLinePen, int stickyWidth, Rectangle scrollingRegion)
        {
            if (_grid.GroupEngine?.IsGrouped != true) return;

            int groupHeaderHeight = _grid.GroupEngine.GetHeaderHeight();
            var groupRenderer = new Grouping.GridGroupHeaderRenderer(_grid);

            int drawY = currentY;
            for (int r = visibleRowStart; r <= visibleRowEnd && r < _grid.Data.Rows.Count; r++)
            {
                drawY += DrawGroupHeadersAtRow(g, r, drawY, rowsRect, groupRenderer, groupHeaderHeight);

                var row = _grid.Data.Rows[r];
                if (!row.IsVisible)
                {
                    continue;
                }
                int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                drawY += rowHeight;

                // Check if this row is the last row of any group
                foreach (var group in _grid.GroupEngine.Groups)
                {
                    if (group.IsCollapsed || group.SummaryRow == null) continue;
                    if (group.RowIndices.Count == 0) continue;
                    int lastRow = group.RowIndices.Max();
                    if (lastRow != r) continue;

                    int summaryHeight = group.SummaryRow.Height;
                    if (drawY + summaryHeight > rowsRect.Top && drawY < rowsRect.Bottom)
                    {
                        DrawSummaryRowContent(g, group, drawY, rowsRect,
                            gridBackColor, gridForeColor, gridLinePen, stickyWidth, scrollingRegion);
                    }
                    drawY += summaryHeight;
                }
            }
        }

        /// <summary>
        /// Draws a single group summary row across all visible columns.
        /// </summary>
        private void DrawSummaryRowContent(Graphics g, Models.GridGroup group, int y, Rectangle rowsRect,
            Color gridBackColor, Color gridForeColor,
            Pen gridLinePen, int stickyWidth, Rectangle scrollingRegion)
        {
            int summaryHeight = group.SummaryRow?.Height ?? 22;
            var summaryBack = Color.FromArgb(245, 245, 250);
            var summaryFore = Color.DarkSlateGray;
            var font = GetSafeCellFont();

            var stickyCols = _grid.Data.Columns.Where(c => c.Sticked && c.Visible).OrderBy(c => c.DisplayOrder).ToList();
            var scrollCols = _grid.Data.Columns.Where(c => c.Visible && !c.Sticked).OrderBy(c => c.DisplayOrder).ToList();

            // Draw sticky columns portion
            int x = rowsRect.Left;
            foreach (var st in stickyCols)
            {
                int colW = Math.Max(20, st.Width);
                var rect = new Rectangle(x, y, colW, summaryHeight);
                using (var bg = new SolidBrush(summaryBack)) g.FillRectangle(bg, rect);

                object? aggValue = null;
                group.SummaryRow?.Values.TryGetValue(st.ColumnName, out aggValue);
                if (aggValue != null)
                {
                    string text = FormatAggregateValue(aggValue, st);
                    TextRenderer.DrawText(g, text, font, rect, summaryFore,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                }

                if (ShowGridLines) g.DrawRectangle(gridLinePen, rect);
                x += colW;
            }

            // Draw scrolling columns portion
            x = scrollingRegion.Left - _grid.Scroll.HorizontalOffset;

            int firstSumCol = 0;
            int lastSumCol = scrollCols.Count - 1;
            if (_grid.EnableColumnVirtualization)
            {
                firstSumCol = _grid.ColumnVirtualizer.FirstScrollingVisibleIndex;
                lastSumCol = _grid.ColumnVirtualizer.LastScrollingVisibleIndex;
                for (int si = 0; si < firstSumCol && si < scrollCols.Count; si++)
                    x += Math.Max(20, scrollCols[si].Width);
            }

            for (int si = firstSumCol; si <= lastSumCol && si < scrollCols.Count; si++)
            {
                var sc = scrollCols[si];
                int colW = Math.Max(20, sc.Width);
                if (x + colW > scrollingRegion.Left && x < scrollingRegion.Right)
                {
                    var rect = new Rectangle(x, y, colW, summaryHeight);
                    using (var bg = new SolidBrush(summaryBack)) g.FillRectangle(bg, rect);

                    object? aggValue = null;
                    group.SummaryRow?.Values.TryGetValue(sc.ColumnName, out aggValue);
                    if (aggValue != null)
                    {
                        string text = FormatAggregateValue(aggValue, sc);
                        TextRenderer.DrawText(g, text, font, rect, summaryFore,
                            TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                    }

                    if (ShowGridLines) g.DrawRectangle(gridLinePen, rect);
                }
                x += colW;
                if (x > scrollingRegion.Right) break;
            }
        }

        /// <summary>
        /// Formats an aggregate value for display in a summary row cell.
        /// </summary>
        private string FormatAggregateValue(object? value, BeepColumnConfig column)
        {
            if (value == null) return string.Empty;
            if (!string.IsNullOrEmpty(column.Format) && value is IFormattable fmt)
                return fmt.ToString(column.Format, System.Globalization.CultureInfo.CurrentCulture);
            return value.ToString() ?? string.Empty;
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

        /// <summary>
        /// Draws any group headers whose first row is <paramref name="rowIndex"/>
        /// and advances the Y position. Returns total header height drawn.
        /// </summary>
        private int DrawGroupHeadersAtRow(Graphics g, int rowIndex, int y, Rectangle bounds,
            Grouping.GridGroupHeaderRenderer renderer, int headerHeight)
        {
            int totalHeight = 0;
            if (_grid.GroupEngine?.IsGrouped != true) return totalHeight;

            foreach (var group in _grid.GroupEngine.Groups)
            {
                if (group.FirstRowIndex == rowIndex && group.RowIndices.Count > 0)
                {
                    if (y + totalHeight + headerHeight > bounds.Top && y + totalHeight < bounds.Bottom)
                    {
                        var headerBounds = new Rectangle(bounds.Left, y + totalHeight, bounds.Width, headerHeight);
                        renderer.PaintHeader(g, headerBounds, group, _hoveredGroupHeaderKey == group.Key, false);
                        _groupHeaderRects[group.Key] = headerBounds;
                    }
                    totalHeight += headerHeight;
                }
                else if (group.FirstRowIndex > rowIndex)
                {
                    break; // groups are sorted by FirstRowIndex
                }
            }
            return totalHeight;
        }

        /// <summary>
        /// Calculate the Y pixel position of a row index within the viewport,
        /// accounting for group header bands that consume vertical space.
        /// </summary>
        internal int CalculateRowY(int rowIndex, int rowsRectTop)
        {
            int pixelOffset = _grid.Scroll.VerticalOffset;
            int totalHeightBefore = 0;
            for (int i = 0; i < rowIndex && i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                if (!row.IsVisible) continue;
                totalHeightBefore += row.Height > 0 ? row.Height : _grid.RowHeight;
            }
            // Add group header heights placed before this row
            totalHeightBefore += (_grid.GroupEngine?.GetHeaderCountBeforeRow(rowIndex) ?? 0) * (_grid.GroupEngine?.GetHeaderHeight() ?? 0);
            // Add summary row heights placed before this row
            totalHeightBefore += _grid.GroupEngine?.GetSummaryRowHeightBeforeRow(rowIndex) ?? 0;
            return rowsRectTop - (pixelOffset - totalHeightBefore);
        }
    }
}
