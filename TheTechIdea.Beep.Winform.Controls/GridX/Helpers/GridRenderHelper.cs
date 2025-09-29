using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
 
using TheTechIdea.Beep.Winform.Controls.Helpers; // Svgs
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridRenderHelper
    {
        private readonly BeepGridPro _grid;
        private BeepCheckBoxBool _rowCheck;

        // Cache drawers per column (like BeepSimpleGrid)
        private readonly Dictionary<string, IBeepUIComponent> _columnDrawerCache = new();

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

        // Enhanced navigation controls for professional paging
        private BeepButton _btnPageFirst;
        private BeepButton _btnPagePrev;
        private BeepButton _btnPageNext;
        private BeepButton _btnPageLast;
        private BeepComboBox _cmbPageSize;
        private BeepTextBox _txtPageJump;
        private BeepButton _btnGoToPage;
        private BeepLabel _lblPageInfo;
        // Public property to access page info label for hit testing
        public BeepLabel PageInfoLabel => _lblPageInfo;

        // Store filter icon rectangles for hit-testing
        private readonly Dictionary<int, Rectangle> _headerFilterIconRects = new();
        public Dictionary<int, Rectangle> HeaderFilterIconRects => _headerFilterIconRects;

        // Store sort icon rectangles for hit-testing
        private readonly Dictionary<int, Rectangle> _headerSortIconRects = new();
        public Dictionary<int, Rectangle> HeaderSortIconRects => _headerSortIconRects;

        public GridRenderHelper(BeepGridPro grid)
        {
            _grid = grid;
        }

        // Grid style properties
        public bool ShowGridLines { get; set; } = true;
        public bool ShowRowStripes { get; set; } = false;
        public System.Drawing.Drawing2D.DashStyle GridLineStyle { get; set; } = System.Drawing.Drawing2D.DashStyle.Solid;
        public bool UseElevation { get; set; } = false;
        public bool CardStyle { get; set; } = false;

        // Advanced header styling properties
        public bool UseHeaderGradient { get; set; } = false;
        public bool ShowSortIndicators { get; set; } = true;
        public bool UseHeaderHoverEffects { get; set; } = true;
        public bool UseBoldHeaderText { get; set; } = false;
        public int HeaderCellPadding { get; set; } = 2;

        internal IBeepTheme Theme => _grid.Theme != null ? BeepThemesManager.GetTheme(_grid.Theme) : BeepThemesManager.GetDefaultTheme();

        // Create or get cached drawer for a given column
        private IBeepUIComponent GetDrawerForColumn(BeepColumnConfig col)
        {
            if (col == null) return null;
            string key = col.ColumnName ?? col.ColumnCaption ?? col.GuidID ?? col.GetHashCode().ToString();
            if (_columnDrawerCache.TryGetValue(key, out var cached) && cached != null)
            {
                return cached;
            }

            IBeepUIComponent drawer = col.CellEditor switch
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
                BeepColumnType.Radio => new BeepRadioGroup { IsChild = true, GridMode = true },
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
            Console.WriteLine("GridRenderHelper.Draw called.");
            // Validate graphics object and grid state
            if (g == null || _grid == null || _grid.Layout == null)
            {
                Console.WriteLine("Draw skipped: Invalid graphics or grid state.");

                return;
            }
              
            Console.WriteLine("Drawing grid...");
            var rowsRect = _grid.Layout.RowsRect;
            if (rowsRect.Width <= 0 || rowsRect.Height <= 0)
            {
                Console.WriteLine("Draw skipped: Invalid rows rectangle.");
                return;
            }
               
            Console.WriteLine($"RowsRect: {rowsRect}");
            // Draw background
            using (var brush = new SolidBrush(Theme?.GridBackColor ?? SystemColors.Window))
            {
                g.FillRectangle(brush, rowsRect);
            }
            Console.WriteLine("Background drawn.");
            // Draw column headers
            if (_grid.ShowColumnHeaders)
            {
                try
                {
                    Console.WriteLine("Drawing column headers...");
                    DrawColumnHeaders(g);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error drawing column headers.");
                    // Silently handle header drawing errors to prevent crashes
                }
            }

            // Draw data rows
            try
            {
                Console.WriteLine("Drawing rows...");
                DrawRows(g);
            }
            catch (Exception)
            {
                Console.WriteLine("Error drawing rows.");
                // Silently handle row drawing errors to prevent crashes
            }

            // Draw navigator if enabled
            if (_grid.ShowNavigator)
            {
                try
                {
                    Console.WriteLine("Drawing navigator...");
                    DrawNavigatorArea(g);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error drawing navigator.");
                    // Silently handle navigator drawing errors
                }
            }
            
            // Draw selection indicators
            try
            {
                Console.WriteLine("Drawing selection indicators...");
                DrawSelectionIndicators(g);
            }
            catch (Exception)
            {
                Console.WriteLine("Error drawing selection indicators.");
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

            // Draw bottom border line if grid lines are enabled
            if (ShowGridLines)
            {
                using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
                {
                    pen.DashStyle = GridLineStyle;
                    g.DrawLine(pen, headerRect.Left, headerRect.Bottom, headerRect.Right, headerRect.Bottom);
                }
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
                        DrawHeaderCell(g, col, cellRect, i);
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

        // Draws a simple filter icon (funnel shape)
        private void DrawFilterIcon(Graphics g, Rectangle rect, bool active)
        {
            Color iconColor = active ? Color.DodgerBlue : (Theme?.GridHeaderForeColor ?? SystemColors.ControlText);
            using (var pen = new Pen(iconColor, 2))
            {
                Point[] funnel = new[]
                {
                    new Point(rect.Left + rect.Width / 8, rect.Top + rect.Height / 4),
                    new Point(rect.Right - rect.Width / 8, rect.Top + rect.Height / 4),
                    new Point(rect.Left + rect.Width / 2, rect.Bottom - rect.Height / 8)
                };
                g.DrawLines(pen, funnel);
                // Draw handle
                g.DrawLine(pen,
                    rect.Left + rect.Width / 2,
                    rect.Bottom - rect.Height / 8,
                    rect.Left + rect.Width / 2,
                    rect.Bottom - rect.Height / 4);
            }
        }

        // Draws sort indicator arrows (up/down)
        private void DrawSortIndicator(Graphics g, Rectangle rect, TheTechIdea.Beep.Vis.Modules.SortDirection sortDirection)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            var color = Theme?.GridHeaderForeColor ?? SystemColors.ControlText;

            using (var pen = new Pen(color, 2f))
            {
                int centerX = rect.Left + rect.Width / 2;
                int centerY = rect.Top + rect.Height / 2;

                if (sortDirection == TheTechIdea.Beep.Vis.Modules.SortDirection.Ascending)
                {
                    // Draw up arrow
                    var points = new Point[]
                    {
                        new Point(centerX, rect.Top + 2),
                        new Point(centerX - 4, rect.Bottom - 2),
                        new Point(centerX + 4, rect.Bottom - 2)
                    };
                    g.DrawPolygon(pen, points);
                }
                else if (sortDirection == TheTechIdea.Beep.Vis.Modules.SortDirection.Descending)
                {
                    // Draw down arrow
                    var points = new Point[]
                    {
                        new Point(centerX, rect.Bottom - 2),
                        new Point(centerX - 4, rect.Top + 2),
                        new Point(centerX + 4, rect.Top + 2)
                    };
                    g.DrawPolygon(pen, points);
                }
            }
        }

        private void DrawHeaderCell(Graphics g, BeepColumnConfig column, Rectangle cellRect, int columnIndex)
        {
            // Validate inputs first
            if (g == null || column == null || cellRect.Width <= 0 || cellRect.Height <= 0)
                return;

            // Determine if this header cell is being hovered
            bool isHovered = UseHeaderHoverEffects && _grid.Layout.HoveredHeaderColumnIndex == columnIndex;

            // Background - with gradient support
            if (UseHeaderGradient)
            {
                // Create subtle gradient from theme color to slightly lighter
                var baseColor = Theme?.GridHeaderBackColor ?? SystemColors.Control;
                var lightColor = Color.FromArgb(
                    Math.Min(255, baseColor.R + 20),
                    Math.Min(255, baseColor.G + 20),
                    Math.Min(255, baseColor.B + 20)
                );

                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    cellRect,
                    isHovered ? lightColor : baseColor,
                    isHovered ? baseColor : lightColor,
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, cellRect);
                }
            }
            else
            {
                // Solid background with hover effect
                var backColor = Theme?.GridHeaderBackColor ?? SystemColors.Control;
                if (isHovered && UseHeaderHoverEffects)
                {
                    // Slightly lighter color for hover
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

            // Add elevation effect to header if enabled
            if (UseElevation)
            {
                using (var shadowPen = new Pen(Color.FromArgb(40, 0, 0, 0), 1))
                {
                    // Draw subtle shadow at bottom and right
                    g.DrawLine(shadowPen, cellRect.Left + 1, cellRect.Bottom, cellRect.Right - 1, cellRect.Bottom);
                    g.DrawLine(shadowPen, cellRect.Right, cellRect.Top + 1, cellRect.Right, cellRect.Bottom - 1);
                }
            }

            // Text - with bold support and custom padding
            var textColor = Theme?.GridHeaderForeColor ?? SystemColors.ControlText;
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;

            // Create font with bold support
            var baseFont = BeepThemesManager.ToFont(_grid._currentTheme.GridHeaderFont) ?? SystemFonts.DefaultFont;
            var font = UseBoldHeaderText ?
                new Font(baseFont.FontFamily, baseFont.Size, FontStyle.Bold) :
                baseFont;

            // Calculate areas with custom padding
            int sortIconSize = ShowSortIndicators ? Math.Min(cellRect.Height - HeaderCellPadding * 2, 16) : 0;
            int filterIconSize = Math.Min(cellRect.Height - HeaderCellPadding * 2, 18);

            // Calculate positions from right to left
            int rightX = cellRect.Right - HeaderCellPadding;

            // Filter icon area (if shown)
            Rectangle filterIconRect = new Rectangle(
                rightX - filterIconSize,
                cellRect.Top + HeaderCellPadding,
                filterIconSize,
                filterIconSize);
            rightX -= filterIconSize + HeaderCellPadding;

            // Sort icon area (if shown and column is sorted)
            Rectangle sortIconRect = Rectangle.Empty;
            if (ShowSortIndicators && column.IsSorted && column.ShowSortIcon)
            {
                sortIconRect = new Rectangle(
                    rightX - sortIconSize,
                    cellRect.Top + HeaderCellPadding,
                    sortIconSize,
                    sortIconSize);
                rightX -= sortIconSize + HeaderCellPadding;
            }

            // Text area
            var textRect = new Rectangle(
                cellRect.X + HeaderCellPadding,
                cellRect.Y + HeaderCellPadding,
                Math.Max(1, rightX - cellRect.X - HeaderCellPadding),
                Math.Max(1, cellRect.Height - HeaderCellPadding * 2)
            );

            // Draw text honoring column header alignment
            if (!string.IsNullOrEmpty(text))
            {
                var headerAlign = column.HeaderTextAlignment;
                var flags = GetTextFormatFlagsForAlignment(headerAlign, true);
                TextRenderer.DrawText(g, text, font, textRect, textColor, flags);
            }

            // Draw sort indicator if enabled and column is sorted
            if (ShowSortIndicators && column.IsSorted && column.ShowSortIcon)
            {
                DrawSortIndicator(g, sortIconRect, column.SortDirection);
                _headerSortIconRects[columnIndex] = sortIconRect;
            }
            else
            {
                _headerSortIconRects.Remove(columnIndex);
            }

            // Draw filter icon if hovered
            bool showFilterIcon = column.ShowFilterIcon && _grid.Layout.HoveredHeaderColumnIndex == columnIndex;
            if (showFilterIcon)
            {
                DrawFilterIcon(g, filterIconRect, column.IsFiltered);
                _headerFilterIconRects[columnIndex] = filterIconRect;
            }
            else
            {
                _headerFilterIconRects.Remove(columnIndex);
            }

            // Border - only draw if grid lines are enabled
            if (ShowGridLines)
            {
                using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
                {
                    pen.DashStyle = GridLineStyle;
                    g.DrawRectangle(pen, cellRect);
                }
            }

            // Clean up font if we created a bold version
            if (UseBoldHeaderText)
            {
                font.Dispose();
            }
        }

        private void DrawRows(Graphics g)
        {
            var rowsRect = _grid.Layout.RowsRect;
            using var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark);
            pen.DashStyle = GridLineStyle; // Apply the grid line style

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

                                // Determine background color based on row stripes and selection
                                Color back;
                                if (isSelectedRow)
                                {
                                    back = Theme?.GridRowSelectedBackColor == Color.Empty ? (Theme?.SelectedRowBackColor ?? SystemColors.Highlight) : Theme.GridRowSelectedBackColor;
                                }
                                else if (isActiveRow)
                                {
                                    back = Theme?.GridRowHoverBackColor == Color.Empty ? SystemColors.ControlLight : Theme.GridRowHoverBackColor;
                                }
                                else if (ShowRowStripes && r % 2 == 1)
                                {
                                    // Alternate row color for stripes
                                    back = Theme?.AltRowBackColor ?? Color.FromArgb(250, 250, 250);
                                }
                                else
                                {
                                    back = sc.Col.HasCustomBackColor && sc.Col.UseCustomColors ? sc.Col.ColumnBackColor : (Theme?.GridBackColor ?? SystemColors.Window);
                                }

                                var fore = sc.Col.HasCustomForeColor && sc.Col.UseCustomColors ? sc.Col.ColumnForeColor : (Theme?.GridForeColor ?? SystemColors.WindowText);

                                using (var bg = new SolidBrush(back)) g.FillRectangle(bg, rect);

                                // Add elevation effect if enabled
                                if (UseElevation && !isSelectedRow && !isActiveRow)
                                {
                                    using (var shadowPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                                    {
                                        // Draw subtle shadow at bottom
                                        g.DrawLine(shadowPen, rect.Left + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
                                        g.DrawLine(shadowPen, rect.Right, rect.Top + 1, rect.Right, rect.Bottom - 1);
                                    }
                                }

                                // Add card-style effect if enabled
                                if (CardStyle && !isSelectedRow && !isActiveRow)
                                {
                                    using (var cardPen = new Pen(Color.FromArgb(40, Theme?.GridLineColor ?? SystemColors.ControlDark), 1))
                                    {
                                        g.DrawRectangle(cardPen, rect);
                                    }
                                }

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

                                // Draw grid lines only if enabled
                                if (ShowGridLines)
                                {
                                    g.DrawRectangle(pen, rect);
                                }
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

                            // Determine background color based on row stripes and selection
                            Color back;
                            if (isSelectedRow)
                            {
                                back = Theme?.GridRowSelectedBackColor == Color.Empty ? (Theme?.SelectedRowBackColor ?? SystemColors.Highlight) : Theme.GridRowSelectedBackColor;
                            }
                            else if (isActiveRow)
                            {
                                back = Theme?.GridRowHoverBackColor == Color.Empty ? SystemColors.ControlLight : Theme.GridRowHoverBackColor;
                            }
                            else if (ShowRowStripes && r % 2 == 1)
                            {
                                // Alternate row color for stripes
                                back = Theme?.AltRowBackColor ?? Color.FromArgb(250, 250, 250);
                            }
                            else
                            {
                                back = st.Col.HasCustomBackColor && st.Col.UseCustomColors ? st.Col.ColumnBackColor : (Theme?.GridBackColor ?? SystemColors.Window);
                            }

                            var fore = st.Col.HasCustomForeColor && st.Col.UseCustomColors ? st.Col.ColumnForeColor : (Theme?.GridForeColor ?? SystemColors.WindowText);

                            using (var bg = new SolidBrush(back)) g.FillRectangle(bg, rect);

                            // Add elevation effect if enabled
                            if (UseElevation && !isSelectedRow && !isActiveRow)
                            {
                                using (var shadowPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                                {
                                    // Draw subtle shadow at bottom
                                    g.DrawLine(shadowPen, rect.Left + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
                                    g.DrawLine(shadowPen, rect.Right, rect.Top + 1, rect.Right, rect.Bottom - 1);
                                }
                            }

                            // Add card-style effect if enabled
                            if (CardStyle && !isSelectedRow && !isActiveRow)
                            {
                                using (var cardPen = new Pen(Color.FromArgb(40, Theme?.GridLineColor ?? SystemColors.ControlDark), 1))
                                {
                                    g.DrawRectangle(cardPen, rect);
                                }
                            }

                            if (st.Col.IsSelectionCheckBox && _grid.ShowCheckBox)
                            {
                                _rowCheck ??= new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme };
                                _rowCheck.CurrentValue = row.IsSelected;
                                _rowCheck.Draw(g, rect);
                            }
                            else if (st.Col.IsRowNumColumn)
                            {
                                var font = BeepThemesManager.ToFont(_grid._currentTheme.GridCellFont) ?? SystemFonts.DefaultFont;
                                TextRenderer.DrawText(g, cell.CellValue?.ToString() ?? string.Empty, font, rect, fore,
                                    TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                            }
                            else
                            {
                                DrawCellContent(g, st.Col, cell, rect, fore, back);
                            }

                            // Draw grid lines only if enabled
                            if (ShowGridLines)
                            {
                                g.DrawRectangle(pen, rect);
                            }
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
                    var flags = GetTextFormatFlagsForAlignment(column.CellTextAlignment, true);
                    TextRenderer.DrawText(g, text, font, textRect, foreColor, flags);
                }
                return;
            }

            drawer.Theme = _grid.Theme;
            Control control= drawer as Control;
            control.BackColor = backColor;
            control.ForeColor = foreColor;
            control.Bounds = rect;

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
            else if (drawer is IBeepUIComponent btn)
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
            _btnInsert = new BeepButton { ImagePath = Svgs.NavPlus, Theme = _grid.Theme };
            _btnDelete = new BeepButton { ImagePath = Svgs.NavMinus, Theme = _grid.Theme };
            _btnSave = new BeepButton { ImagePath = Svgs.FloppyDisk, Theme = _grid.Theme };
            _btnCancel = new BeepButton { ImagePath = Svgs.NavBackArrow, Theme = _grid.Theme };

            // Record navigation
            _btnFirst = new BeepButton { ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-double-small-left.svg", Theme = _grid.Theme };
            _btnPrev = new BeepButton { ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-left.svg", Theme = _grid.Theme };
            _btnNext = new BeepButton { ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-right.svg", Theme = _grid.Theme };
            _btnLast = new BeepButton { ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-double-small-right.svg", Theme = _grid.Theme };

            // Page info label (professional paging — keep only this)
            _lblPageInfo = new BeepLabel { Theme = _grid.Theme, Text = "Page 1 of 1 — 0 records" };

            // Utilities
            _btnQuery = new BeepButton { ImagePath = Svgs.NavSearch, Theme = _grid.Theme };
            _btnFilter = new BeepButton { ImagePath = Svgs.NavWaving, Theme = _grid.Theme };
            _btnPrint = new BeepButton { ImagePath = Svgs.NavPrinter, Theme = _grid.Theme };

            foreach (var btn in new[] { _btnInsert, _btnDelete, _btnSave, _btnCancel, _btnFirst, _btnPrev, _btnNext, _btnLast,
                                       _btnQuery, _btnFilter, _btnPrint })
                ConfigureIconButton(btn);

            // Configure non-icon controls
            ConfigurePagingControls();
        }

        // Configure icon button properties
        private void ConfigureIconButton(BeepButton btn)
        {
            if (btn != null)
            {
                // Icon buttons don't need text, just set size and max image size
                btn.Text = "";
                btn.UseThemeFont = true;
                btn.AutoSize = false;
            }
        }

        private void ConfigurePagingControls()
        {
            // Only page info label remains
            if (_lblPageInfo != null)
            {
                _lblPageInfo.IsChild = true;
                _lblPageInfo.UseThemeFont = true;
                _lblPageInfo.AutoSize = true;
            }

            // _btnGoToPage removed; no per-control config needed
        }

        private void DrawNavigatorArea(Graphics g)
        {
            var navRect = _grid.Layout.NavigatorRect;
            if (navRect.IsEmpty) return;

            // Clear existing navigator hit tests
            _grid.ClearHitList();

            // Fill navigator background
            using (var brush = new SolidBrush(Theme?.GridHeaderBackColor ?? SystemColors.Control))
            {
                g.FillRectangle(brush, navRect);
            }

            // Top border - only draw if grid lines are enabled
            if (ShowGridLines)
            {
                using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
                {
                    pen.DashStyle = GridLineStyle;
                    g.DrawLine(pen, navRect.Left, navRect.Top, navRect.Right, navRect.Top);
                }
            }

            // Ensure buttons exist and are themed
            EnsureNavigatorButtons();
            foreach (var btn in new[] { _btnFirst, _btnPrev, _btnNext, _btnLast, _btnInsert, _btnDelete, _btnSave, _btnCancel,
                                       _btnQuery, _btnFilter, _btnPrint })
            {
                if (btn == null) continue;
                btn.Theme = _grid.Theme;
                ConfigureIconButton(btn);
            }

            // Configure paging controls (only page info remains)
            if (_lblPageInfo != null) _lblPageInfo.Theme = _grid.Theme;

            // Improved layout constants for better alignment
            const int buttonWidth = 28;
            const int buttonHeight = 24;
            const int padding = 8;
            const int spacing = 6;
            const int sectionSpacing = 16;

            int y = navRect.Top + (navRect.Height - buttonHeight) / 2;

            // Check if navigator is wide enough for all controls
            int minRequiredWidth = (buttonWidth * 10) + (spacing * 9) + (padding * 2) + (sectionSpacing * 2) + 200; // Extra for text/page controls
            bool compactMode = navRect.Width < minRequiredWidth;

            // Let GridStyle presets influence navigator layout and visibility
            bool forceCompact = false;
            bool hideUtilities = false;
            bool hidePageInfo = false;
            switch (_grid.GridStyle)
            {
                case BeepGridStyle.Compact:
                    forceCompact = true;
                    break;
                case BeepGridStyle.Minimal:
                case BeepGridStyle.Borderless:
                    // Minimal and borderless favor very compact navigator with minimal chrome
                    forceCompact = true;
                    hideUtilities = true;
                    hidePageInfo = true;
                    break;
                case BeepGridStyle.Corporate:
                    // Corporate keeps full professional layout
                    break;
                default:
                    // Other styles keep default behavior
                    break;
            }

            compactMode = forceCompact || compactMode;

            if (compactMode)
            {
                // Compact mode: Show only essential controls
                DrawCompactNavigator(g, navRect, buttonWidth, buttonHeight, padding, spacing, y, !hideUtilities);
            }
            else
            {
                // Full mode: Show all controls with proper spacing
                DrawFullNavigator(g, navRect, buttonWidth, buttonHeight, padding, spacing, sectionSpacing, y, !hideUtilities, !hidePageInfo);
            }
        }

    private void DrawCompactNavigator(Graphics g, Rectangle navRect, int buttonWidth, int buttonHeight, int padding, int spacing, int y, bool showUtilities)
        {
            // Left section: Essential CRUD buttons
            int leftX = navRect.Left + padding;
            var insertRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var deleteRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var saveRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var cancelRect = new Rectangle(leftX, y, buttonWidth, buttonHeight);

            // Register CRUD button hit tests
            _grid.AddHitArea("Insert", insertRect, _btnInsert, () => _grid.InsertNew());
            _grid.AddHitArea("Delete", deleteRect, _btnDelete, () => _grid.DeleteCurrent());
            _grid.AddHitArea("Save", saveRect, _btnSave, () => _grid.Save());
            _grid.AddHitArea("Cancel", cancelRect, _btnCancel, () => _grid.Cancel());

            foreach (var tuple in new[] { (_btnInsert, insertRect), (_btnDelete, deleteRect), (_btnSave, saveRect), (_btnCancel, cancelRect) })
            {
                var b = tuple.Item1; var r = tuple.Item2;
                b.Size = r.Size; b.MaxImageSize = new Size(r.Width - 4, r.Height - 4);
                b.Draw(g, r);
            }

            // Center section: Navigation buttons and counter
            string recordCounter = (_grid.Rows.Count > 0 && _grid.Selection?.RowIndex >= 0)
                ? $"{_grid.Selection.RowIndex + 1} - {_grid.Rows.Count}"
                : "0 - 0";
            var headerFont = BeepThemesManager.ToFont(_grid._currentTheme.GridCellFont) ?? SystemFonts.DefaultFont;
            Size textSize = TextRenderer.MeasureText(recordCounter, headerFont);

            // Calculate center position. There are 4 nav buttons and 4 internal spacings between items.
            int compactCenterTotal = buttonWidth * 4 + textSize.Width + spacing * 4;
            int centerStart = (navRect.Left + navRect.Right) / 2 - compactCenterTotal / 2;

            // Clamp centerStart so the center group won't overlap left CRUD or right utilities in compact mode.
            int leftOccupied = cancelRect.Right + spacing;
            int rightX = navRect.Right - padding;
            int rightOccupiedStart = rightX - (buttonWidth * 2 + spacing); // filter+print estimate
            int minCenter = leftOccupied + spacing;
            int maxCenter = Math.Max(minCenter, rightOccupiedStart - compactCenterTotal - spacing);
            centerStart = Math.Min(Math.Max(centerStart, minCenter), maxCenter);
            var firstRect = new Rectangle(centerStart, y, buttonWidth, buttonHeight);
            var prevRect = new Rectangle(firstRect.Right + spacing, y, buttonWidth, buttonHeight);
            var counterRect = new Rectangle(prevRect.Right + spacing, y, textSize.Width, buttonHeight);
            var nextRect = new Rectangle(counterRect.Right + spacing, y, buttonWidth, buttonHeight);
            var lastRect = new Rectangle(nextRect.Right + spacing, y, buttonWidth, buttonHeight);

            // Register navigation button hit tests
            _grid.AddHitArea("First", firstRect, _btnFirst, () => _grid.MoveFirst());
            _grid.AddHitArea("Prev", prevRect, _btnPrev, () => _grid.MovePrevious());
            _grid.AddHitArea("Next", nextRect, _btnNext, () => _grid.MoveNext());
            _grid.AddHitArea("Last", lastRect, _btnLast, () => _grid.MoveLast());

            foreach (var tuple in new[] { (_btnFirst, firstRect), (_btnPrev, prevRect), (_btnNext, nextRect), (_btnLast, lastRect) })
            {
                var b = tuple.Item1; var r = tuple.Item2;
                b.Size = r.Size; b.MaxImageSize = new Size(r.Width - 4, r.Height - 4);
                b.Draw(g, r);
            }

            // Draw record counter
            TextRenderer.DrawText(g, recordCounter, headerFont, counterRect,
                Theme?.GridHeaderForeColor ?? SystemColors.ControlText,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            // Right section: Essential utility buttons (optional per style)
            if (showUtilities)
            {
                rightX = navRect.Right - padding;
                var filterRect = new Rectangle(rightX - buttonWidth, y, buttonWidth, buttonHeight); rightX = filterRect.Left - spacing;
                var printRect = new Rectangle(rightX - buttonWidth, y, buttonWidth, buttonHeight);

                // Register utility button hit tests
                _grid.AddHitArea("Filter", filterRect, _btnFilter);
                _grid.AddHitArea("Print", printRect, _btnPrint);

                foreach (var tuple in new[] { (_btnFilter, filterRect), (_btnPrint, printRect) })
                {
                    var b = tuple.Item1; var r = tuple.Item2;
                    b.Size = r.Size; b.MaxImageSize = new Size(r.Width - 4, r.Height - 4);
                    b.Draw(g, r);
                }
            }
        }

    private void DrawFullNavigator(Graphics g, Rectangle navRect, int buttonWidth, int buttonHeight, int padding, int spacing, int sectionSpacing, int y, bool showUtilities, bool showPageInfo)
        {
            // Left section: CRUD buttons
            int leftX = navRect.Left + padding;
            var insertRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var deleteRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var saveRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var cancelRect = new Rectangle(leftX, y, buttonWidth, buttonHeight);

            // Register CRUD button hit tests
            _grid.AddHitArea("Insert", insertRect, _btnInsert, () => _grid.InsertNew());
            _grid.AddHitArea("Delete", deleteRect, _btnDelete, () => _grid.DeleteCurrent());
            _grid.AddHitArea("Save", saveRect, _btnSave, () => _grid.Save());
            _grid.AddHitArea("Cancel", cancelRect, _btnCancel, () => _grid.Cancel());

            foreach (var tuple in new[] { (_btnInsert, insertRect), (_btnDelete, deleteRect), (_btnSave, saveRect), (_btnCancel, cancelRect) })
            {
                var b = tuple.Item1; var r = tuple.Item2;
                b.Size = r.Size; b.MaxImageSize = new Size(r.Width - 4, r.Height - 4);
                b.Draw(g, r);
            }

            // Center section: Navigation buttons and counter
            string recordCounter = (_grid.Rows.Count > 0 && _grid.Selection?.RowIndex >= 0)
                ? $"{_grid.Selection.RowIndex + 1} - {_grid.Rows.Count}"
                : "0 - 0";
            var headerFont = BeepThemesManager.ToFont(_grid._currentTheme.GridCellFont) ?? SystemFonts.DefaultFont;
            Size textSize = TextRenderer.MeasureText(recordCounter, headerFont);

            // Reserve space used by left CRUD (we already placed them) and compute their right edge
            int leftOccupied = cancelRect.Right + spacing;

            // Simulate right section consumption to find leftmost X of right area
            int rightTemp = navRect.Right - padding;
            if (showPageInfo && _lblPageInfo != null)
            {
                Size pageInfoSize = TextRenderer.MeasureText(_lblPageInfo.Text, headerFont);
                rightTemp -= pageInfoSize.Width + sectionSpacing;
            }
            // page-jump textbox and small page buttons removed — no reserved space
            // page size selector removed — don't reserve space
            // utilities (query, filter, print)
            rightTemp -= (buttonWidth * 3 + spacing * 2);

            int rightOccupiedStart = rightTemp;

            // Total width needed by center controls (four nav buttons + counter + internal spacings)
            // There are 4 spacings between the 5 elements (btn,btn,counter,btn,btn)
            int totalCenterWidth = buttonWidth * 4 + textSize.Width + spacing * 4;

            int availableWidth = Math.Max(0, rightOccupiedStart - leftOccupied - sectionSpacing * 2);
            int centerStart;
            if (availableWidth >= totalCenterWidth)
            {
                centerStart = leftOccupied + sectionSpacing + (availableWidth - totalCenterWidth) / 2;
            }
            else
            {
                // fallback to using full navRect center if not enough space
                centerStart = (int)(navRect.Left + (navRect.Width - totalCenterWidth) / 2.0);
            }

            // Clamp centerStart so the center group never overlaps the left or right occupied areas.
            // This keeps placement deterministic even when the simulated right-side width estimate
            // is slightly off due to varying text/button sizes.
            int minCenter = leftOccupied + sectionSpacing;
            int maxCenter = Math.Max(minCenter, rightOccupiedStart - totalCenterWidth - sectionSpacing);
            centerStart = Math.Min(Math.Max(centerStart, minCenter), maxCenter);

            var firstRect = new Rectangle(centerStart, y, buttonWidth, buttonHeight);
            var prevRect = new Rectangle(firstRect.Right + spacing, y, buttonWidth, buttonHeight);
            var counterRect = new Rectangle(prevRect.Right + spacing, y, textSize.Width, buttonHeight);
            var nextRect = new Rectangle(counterRect.Right + spacing, y, buttonWidth, buttonHeight);
            var lastRect = new Rectangle(nextRect.Right + spacing, y, buttonWidth, buttonHeight);

            // Register navigation button hit tests
            _grid.AddHitArea("First", firstRect, _btnFirst, () => _grid.MoveFirst());
            _grid.AddHitArea("Prev", prevRect, _btnPrev, () => _grid.MovePrevious());
            _grid.AddHitArea("Next", nextRect, _btnNext, () => _grid.MoveNext());
            _grid.AddHitArea("Last", lastRect, _btnLast, () => _grid.MoveLast());

            foreach (var tuple in new[] { (_btnFirst, firstRect), (_btnPrev, prevRect), (_btnNext, nextRect), (_btnLast, lastRect) })
            {
                var b = tuple.Item1; var r = tuple.Item2;
                b.Size = r.Size; b.MaxImageSize = new Size(r.Width - 4, r.Height - 4);
                b.Draw(g, r);
            }

            // Draw record counter
            TextRenderer.DrawText(g, recordCounter, headerFont, counterRect,
                Theme?.GridHeaderForeColor ?? SystemColors.ControlText,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            // Right section: Page controls and utilities
            int rightX = navRect.Right - padding;

            // Page info label (rightmost)
            if (showPageInfo && _lblPageInfo != null)
            {
                Size pageInfoSize = TextRenderer.MeasureText(_lblPageInfo.Text, headerFont);
                var pageInfoRect = new Rectangle(rightX - pageInfoSize.Width, y, pageInfoSize.Width, buttonHeight);
                _grid.AddHitArea("PageInfo", pageInfoRect, _lblPageInfo);
                _lblPageInfo.Draw(g, pageInfoRect);
                rightX = pageInfoRect.Left - sectionSpacing;
            }

            // Secondary page controls removed — rely on main navigation buttons and page info only

            // Page size selector removed by request

            // Utility buttons (left of paging controls)
            // Utility buttons (left of paging controls) - optional per style
            if (showUtilities)
            {
                var queryRect = new Rectangle(rightX - buttonWidth, y, buttonWidth, buttonHeight); rightX -= buttonWidth + spacing;
                var filterRect = new Rectangle(rightX - buttonWidth, y, buttonWidth, buttonHeight); rightX -= buttonWidth + spacing;
                var printRect = new Rectangle(rightX - buttonWidth, y, buttonWidth, buttonHeight);

                // Register utility button hit tests
                _grid.AddHitArea("Query", queryRect, _btnQuery);
                _grid.AddHitArea("Filter", filterRect, _btnFilter);
                _grid.AddHitArea("Print", printRect, _btnPrint);

                foreach (var tuple in new[] { (_btnQuery, queryRect), (_btnFilter, filterRect), (_btnPrint, printRect) })
                {
                    var b = tuple.Item1; var r = tuple.Item2;
                    b.Size = r.Size; b.MaxImageSize = new Size(r.Width - 4, r.Height - 4);
                    b.Draw(g, r);
                }
            }
        }

        private void DrawSelectionIndicators(Graphics g)
        {
            // Draw selection indicators (can be enhanced later)
        }

        // Professional paging methods
        public void UpdatePageInfo(int currentPage, int totalPages, int totalRecords)
        {
            if (_lblPageInfo != null)
            {
                if (totalRecords <= 0)
                    _lblPageInfo.Text = "No records";
                else
                    _lblPageInfo.Text = $"Page {currentPage} of {totalPages} — {totalRecords} records";
            }
        }

        public void SetCurrentPage(int pageNumber)
        {
            // Page jump textbox removed; nothing to set
        }

        public int GetCurrentPageSize()
        {
            // Page size combobox removed — return default
            return 10;
        }

        public int GetJumpToPage()
        {
            // No page jump control; default to 1
            return 1;
        }

        public void EnablePagingControls(bool enable)
        {
            // Only main navigation buttons exist; enable/disable them
            if (_btnFirst != null) _btnFirst.Enabled = enable;
            if (_btnPrev != null) _btnPrev.Enabled = enable;
            if (_btnNext != null) _btnNext.Enabled = enable;
            if (_btnLast != null) _btnLast.Enabled = enable;
        }

        // Public methods for hit testing - connected to actual navigation
        public void TriggerPageFirst() => _grid.Navigator?.MoveFirst();
        public void TriggerPagePrev() => _grid.Navigator?.MovePrevious();
        public void TriggerPageNext() => _grid.Navigator?.MoveNext();
        public void TriggerPageLast() => _grid.Navigator?.MoveLast();
        public void TriggerGoToPage()
        {
            int page = GetJumpToPage();
            // For now, just move to first/last based on page number
            // TODO: Implement actual page-based navigation when paging is added
            if (page == 1)
                _grid.Navigator?.MoveFirst();
            else
                _grid.Navigator?.MoveLast();
        }
    public void FocusPageJump() { /* no-op: page jump removed */ }
    public void FocusPageSize() { /* no-op: page size selector removed */ }

        public void SetupPagingEventHandlers(Action<int> onPageSizeChanged, Action<int> onPageJump, Action onFirstPage, Action onPrevPage, Action onNextPage, Action onLastPage)
        {
            // page size selector removed; nothing to wire here
            // Only wire the main navigation callbacks to the grid navigator
            // The main nav buttons are handled via hit test -> _grid.Navigator actions
            // Keep this method for API compatibility but avoid wiring removed controls.
        }

        private static TextFormatFlags GetTextFormatFlagsForAlignment(ContentAlignment alignment, bool endEllipsis)
        {
            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.PreserveGraphicsClipping;
            if (endEllipsis) flags |= TextFormatFlags.EndEllipsis;

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    flags |= TextFormatFlags.Left;
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Right;
                    break;
            }
            return flags;
        }
    }
}

