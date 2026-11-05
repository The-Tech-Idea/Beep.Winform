using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

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

            _grid.ConditionalFormatting.BeginFrame();
            _emptyStateClearRect = Rectangle.Empty;

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
                    // Sync host-driven properties into the paint-time state.
                    // These are READ at paint time; the grid properties are
                    // the source of truth, the state is the snapshot.
                    // SearchText is already up-to-date: the on-demand search
                    // editor pushes its in-progress value to ToolbarState on
                    // every keystroke (see FilterEditorHelper.OnSearchEditorTextChanged).
                    state.GridTitle = _grid.GridTitle;
                    state.ShowGridTitle = _grid.ShowGridTitle;
                    state.IsFilterActive = _grid.IsFiltered;
                    state.ActiveFilterCount = _grid.ActiveFilter?.Criteria.Count ?? 0;
                    state.DpiScale = _grid.DeviceDpi / 96f;
                    state.ShowFilterButton = _grid.ShowFilterButton;
                    // Have the painter build / refresh its brush/pen/font
                    // cache and copy the resolved fonts onto the state
                    // BEFORE CalculateLayout.  This makes the layout's
                    // text-width measurement use the same font instance
                    // the painter will draw with, avoiding a per-paint
                    // allocation of "new Font(...)" for the title.
                    _grid.ToolbarPainter.PrepareLayout(state);
                    state.CalculateLayout(_grid.Layout.ToolbarRect);
                    _grid.ToolbarPainter.Paint(g, _grid.Layout.ToolbarRect, state);
                }
                catch (Exception ex)
                {
                    // Absorbed so the rest of the grid still paints, but reported: a blank
                    // band with no explanation is what made these failures so expensive.
                    _grid.ReportRenderError("Toolbar", ex);
                }

                // The inline search editor is a real control, so it has to be moved to wherever
                // this pass just put the search box. It cannot be done from OnResize: the toolbar
                // is laid out here, during paint, so a resize hook runs while the state still
                // holds the *previous* geometry and positions the editor from that. Maximizing
                // moved the search box by thousands of pixels while the editor stayed where it
                // was, which is the floating box seen at the wrong location.
                //
                // Outside the try above deliberately - a failure to place a child control is not
                // a paint failure and must not be swallowed with one.
                _grid.SyncSearchEditorToToolbarLayout();
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
                    catch (Exception ex)
                    {
                        // Absorbed so the rest of the grid still paints, but reported: a blank
                        // band with no explanation is what made these failures so expensive.
                        _grid.ReportRenderError("TopFilterPanel", ex);
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
                catch (Exception ex)
                {
                    // Absorbed so the rest of the grid still paints, but reported: a blank
                    // band with no explanation is what made these failures so expensive.
                    _grid.ReportRenderError("ColumnHeaders", ex);
                }
            }

            // Draw the active-filter chips row (DevExpress-style removable
            // conditions). Its rect is empty unless a filter is active, so
            // drawing it is a no-op on an unfiltered grid.
            try
            {
                DrawActiveFilterBar(g);
            }
            catch (Exception ex)
            {
                _grid.ReportRenderError("ActiveFilterBar", ex);
            }

            // Draw data rows, or the empty state when there is nothing to draw
            try
            {
                if (_grid.Data?.Rows == null || _grid.Data.Rows.Count == 0)
                    DrawEmptyState(g, rowsRect);
                else
                    DrawRows(g);
            }
            catch (Exception ex)
            {
                // Absorbed so the rest of the grid still paints, but reported: a blank
                // band with no explanation is what made these failures so expensive.
                _grid.ReportRenderError("Rows", ex);
            }

            // Draw pinned aggregate footer if enabled
            if (_grid.Footer.Visible)
            {
                try
                {
                    DrawFooter(g);
                }
                catch (Exception ex)
                {
                    _grid.ReportRenderError("Footer", ex);
                }
            }

            // Draw navigator if enabled
            if (_grid.ShowNavigator)
            {
                try
                {
                    DrawNavigatorArea(g);
                }
                catch (Exception ex)
                {
                    // Absorbed so the rest of the grid still paints, but reported: a blank
                    // band with no explanation is what made these failures so expensive.
                    _grid.ReportRenderError("Navigator", ex);
                }
            }

            // Draw selection indicators
            try
            {
                DrawSelectionIndicators(g);
            }
            catch (Exception ex)
            {
                // Absorbed so the rest of the grid still paints, but reported: a blank
                // band with no explanation is what made these failures so expensive.
                _grid.ReportRenderError("Selection", ex);
            }

            // Draw column reorder drag feedback
            if (_grid.AllowColumnReorder && _grid.ColumnReorder.IsDragging)
            {
                try
                {
                    _grid.ColumnReorder.DrawDragFeedback(g);
                }
                catch (Exception ex)
                {
                    // Absorbed so the rest of the grid still paints, but reported: a blank
                    // band with no explanation is what made these failures so expensive.
                    _grid.ReportRenderError("DragFeedback", ex);
                }
            }

            // Draw focus indicator when the grid has keyboard focus
            try
            {
                _grid.FocusManager?.DrawFocusIndicator(g, _grid.DrawingRect);
            }
            catch (Exception ex)
            {
                // Absorbed so the rest of the grid still paints, but reported: a blank
                // band with no explanation is what made these failures so expensive.
                _grid.ReportRenderError("FocusIndicator", ex);
            }
        }

        /// <summary>
        /// Draws the grid body when there are no rows: empty row bands with the column grid lines,
        /// and a centred message.
        /// </summary>
        /// <remarks>
        /// A grid with no data used to paint nothing but background, so a null DataSource, an empty
        /// list, or a filter that matched nothing all looked identical to a broken control. Every
        /// mainstream grid keeps drawing its ruling and says why it is empty; this does the same,
        /// and it distinguishes "no columns defined" from "columns but no rows".
        /// </remarks>
        private void DrawEmptyState(Graphics g, Rectangle rowsRect)
        {
            var isDark = IsDarkTheme;
            var lineColor = Theme?.GridLineColor ?? (isDark ? Color.FromArgb(55, 65, 81) : Color.FromArgb(229, 231, 235));
            var textColor = Theme?.GridForeColor ?? (isDark ? Color.FromArgb(156, 163, 175) : Color.FromArgb(107, 114, 128));

            bool hasColumns = _grid.Data?.Columns?.Any(c => c != null && c.Visible && !c.IsSelectionCheckBox
                                                            && !c.IsRowNumColumn && !c.IsRowID) == true;

            // Empty row bands, so the body reads as a grid rather than a blank panel.
            int rowHeight = Math.Max(18, _grid.RowHeight);
            using (var pen = new Pen(lineColor))
            {
                for (int y = rowsRect.Top + rowHeight; y < rowsRect.Bottom; y += rowHeight)
                    g.DrawLine(pen, rowsRect.Left, y, rowsRect.Right, y);

                // Column separators follow the real header geometry when columns exist.
                if (hasColumns && _grid.Layout?.HeaderCellRects is { Length: > 0 } cells)
                {
                    foreach (var cell in cells)
                    {
                        if (cell.IsEmpty) continue;
                        int x = cell.Right;
                        if (x > rowsRect.Left && x < rowsRect.Right)
                            g.DrawLine(pen, x, rowsRect.Top, x, rowsRect.Bottom);
                    }
                }
            }

            bool filtered = _grid.IsFiltered ||
                            (_grid.SortFilter?.HasActiveFilters ?? false) ||
                            !string.IsNullOrWhiteSpace(_grid.ToolbarState?.SearchText);

            string message = !hasColumns
                ? "No columns defined."
                : filtered ? "No rows match the current search or filter." : "No data.";

            var font = GetSafeCellFont();
            var textRect = filtered
                ? new Rectangle(rowsRect.Left, rowsRect.Top, rowsRect.Width, Math.Max(0, rowsRect.Height - 52))
                : rowsRect;
            TextRenderer.DrawText(g, message, font, textRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            if (filtered)
            {
                const int buttonWidth = 190;
                const int buttonHeight = 30;
                var buttonRect = new Rectangle(
                    rowsRect.Left + (rowsRect.Width - buttonWidth) / 2,
                    rowsRect.Top + Math.Max(0, (rowsRect.Height / 2) + 16),
                    buttonWidth, buttonHeight);

                if (buttonRect.Bottom <= rowsRect.Bottom && buttonRect.Width > 0)
                {
                    var chrome = _grid.ChromeState;
                    using var path = CreateChipPath(buttonRect, 6);
                    using var fill = new SolidBrush(chrome.ActiveBackground);
                    using var border = new Pen(chrome.FocusRing);
                    g.FillPath(fill, path);
                    g.DrawPath(border, path);
                    TextRenderer.DrawText(g, "Clear search and filters", font, buttonRect, chrome.Foreground,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                    _emptyStateClearRect = buttonRect;
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
                _headerCheck ??= new BeepCheckBoxBool
                {
                    IsChild = true,
                    GridMode = true,
                    HideText = true,
                    Theme = _grid.Theme
                };
                _headerCheck.CurrentValue = allSelected;
                _headerCheck.Draw(g, _grid.Layout.SelectAllCheckRect);
            }

            var stickyColumns = _grid.Data.Columns.Where(c => c.Sticked && c.Visible).ToList();
            int stickyWidth = stickyColumns.Sum(c => Math.Max(BeepLayoutMetrics.GridColumnMinW, c.Width));
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

            // Header keyboard focus ring (F6 enters the header band)
            int focusedHeader = _grid.FocusedHeaderColumnIndex;
            if (focusedHeader >= 0 && focusedHeader < _grid.Layout.HeaderCellRects.Length)
            {
                var focusRect = _grid.Layout.HeaderCellRects[focusedHeader];
                if (!focusRect.IsEmpty)
                {
                    var metrics = Chrome.GridChromeMetrics.Resolve(_grid);
                    var chrome = _grid.ChromeState;
                    using var focusPen = new Pen(chrome.FocusRing, metrics.FocusRingWidth);
                    var ring = Rectangle.Inflate(focusRect, -2, -2);
                    g.DrawRectangle(focusPen, ring);
                }
            }

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
        /// Draws the active-filter chips row below the column headers.  Each
        /// chip is one filter condition — an <see cref="FilterConfiguration"/>
        /// criterion or a per-column SortFilter condition — with an "x" that
        /// removes just that condition; a trailing "Clear all" removes every
        /// condition at once. The row is only laid out (and therefore drawn)
        /// while at least one condition is active.
        /// </summary>
        private void DrawActiveFilterBar(Graphics g)
        {
            var barRect = _grid.Layout.ActiveFilterBarRect;
            _activeFilterChipClearRects.Clear();
            _sortFilterChipClearRects.Clear();
            _activeFilterChipRects.Clear();
            _sortFilterChipRects.Clear();
            _activeFilterClearAllRect = Rectangle.Empty;

            if (barRect.Width <= 0 || barRect.Height <= 0) return;

            var activeCriteria = _grid.ActiveFilter?.Criteria;
            var sortCriteria = _grid.SortFilter.GetActiveColumnCriteria();
            bool hasAny = (activeCriteria != null && activeCriteria.Count > 0)
                          || (sortCriteria != null && sortCriteria.Count > 0);
            if (!hasAny) return;

            var isDark = IsDarkTheme;
            float dpi = _grid.DeviceDpi / 96f;

            var barBack = Theme?.GridHeaderBackColor ?? (isDark ? Color.FromArgb(38, 48, 64) : Color.FromArgb(247, 248, 250));
            var chipBack = Theme?.GridBackColor ?? (isDark ? Color.FromArgb(31, 41, 55) : Color.White);
            var borderColor = Theme?.GridLineColor ?? (isDark ? Color.FromArgb(60, 70, 85) : Color.FromArgb(180, 180, 180));
            var textColor = Theme?.GridForeColor ?? (isDark ? Color.FromArgb(229, 231, 235) : Color.FromArgb(31, 41, 55));
            var accent = Theme?.AccentColor ?? Color.DodgerBlue;

            using (var backBrush = new SolidBrush(barBack))
            {
                g.FillRectangle(backBrush, barRect);
            }

            Font font = _grid.Font ?? SystemFonts.DefaultFont;

            int paddingX = (int)(8 * dpi);
            int chipHeight = Math.Max(20, (int)(22 * dpi));
            int chipGap = (int)(6 * dpi);
            int closeSize = (int)(16 * dpi);

            int x = barRect.Left + paddingX;
            int y = barRect.Top + (barRect.Height - chipHeight) / 2;

            var prefix = "Filter:";
            var prefixSize = TextRenderer.MeasureText(prefix, font);
            TextRenderer.DrawText(g, prefix, font,
                new Point(x, barRect.Top + (barRect.Height - prefixSize.Height) / 2), textColor);
            x += prefixSize.Width + (int)(8 * dpi);

            var chips = new List<(string Label, int ActiveIndex, string? Column)>();
            if (activeCriteria != null)
            {
                for (int i = 0; i < activeCriteria.Count; i++)
                {
                    var criterion = activeCriteria[i];
                    if (criterion == null) continue;
                    chips.Add((criterion.ToString(), i, null));
                }
            }

            if (sortCriteria != null)
            {
                foreach (var criterion in sortCriteria)
                    chips.Add((criterion.ToString(), -1, criterion.ColumnName));
            }

            const string clearAll = "Clear all";
            var clearSize = TextRenderer.MeasureText(clearAll, font);
            int clearAllWidth = clearSize.Width + (int)(12 * dpi);
            int rightLimit = barRect.Right - paddingX - clearAllWidth;

            int overflow = 0;
            var clipState = g.Save();
            g.SetClip(barRect);
            for (int i = 0; i < chips.Count; i++)
            {
                var chip = chips[i];
                int width = MeasureFilterChip(chip.Label, font, closeSize, dpi);
                if (x + width + chipGap > rightLimit)
                {
                    overflow = chips.Count - i;
                    break;
                }

                x = DrawFilterChip(g, chip.Label, font, chipBack, borderColor, textColor,
                    chipHeight, closeSize, dpi, x, y, chipGap, out var chipClearRect, out var chipBodyRect);
                if (chip.ActiveIndex >= 0)
                {
                    _activeFilterChipClearRects[chip.ActiveIndex] = chipClearRect;
                    _activeFilterChipRects[chip.ActiveIndex] = chipBodyRect;
                }
                else if (chip.Column != null)
                {
                    _sortFilterChipClearRects[chip.Column] = chipClearRect;
                    _sortFilterChipRects[chip.Column] = chipBodyRect;
                }
            }

            if (overflow > 0 && x + MeasureFilterChip($"+{overflow} more", font, closeSize, dpi) <= barRect.Right - paddingX)
            {
                x = DrawFilterChip(g, $"+{overflow} more", font, chipBack, borderColor, textColor,
                    chipHeight, closeSize, dpi, x, y, chipGap, out _, out _, showClose: false);
            }

            var clearRect = new Rectangle(x + (int)(4 * dpi), y, clearSize.Width + (int)(8 * dpi), chipHeight);
            if (clearRect.Right <= barRect.Right - paddingX)
            {
                _activeFilterClearAllRect = clearRect;
                using (var accentBrush = new SolidBrush(accent))
                {
                    TextRenderer.DrawText(g, clearAll, font, clearRect, accent,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                }
            }
            else
            {
                _activeFilterClearAllRect = Rectangle.Empty;
            }

            g.Restore(clipState);
        }

        private static int MeasureFilterChip(string label, Font font, int closeSize, float dpi)
        {
            var size = TextRenderer.MeasureText(label, font);
            return size.Width + closeSize + (int)(16 * dpi);
        }

        /// <summary>
        /// Draws one active-filter chip and returns the x position after the
        /// chip plus its gap. The "x" close-target rect is returned through
        /// <paramref name="clearRect"/> for hit-testing.
        /// </summary>
        private int DrawFilterChip(Graphics g, string label, Font font, Color chipBack, Color borderColor,
            Color textColor, int chipHeight, int closeSize, float dpi, int x, int y, int chipGap,
            out Rectangle clearRect, out Rectangle chipRect, bool showClose = true)
        {
            var textSize = TextRenderer.MeasureText(label, font);
            int chipWidth = textSize.Width + (showClose ? closeSize : 0) + (int)(16 * dpi);

            chipRect = new Rectangle(x, y, chipWidth, chipHeight);

            using (var path = CreateChipPath(chipRect, chipHeight / 2))
            using (var chipBrush = new SolidBrush(chipBack))
            using (var borderPen = new Pen(borderColor))
            {
                g.FillPath(chipBrush, path);
                g.DrawPath(borderPen, path);
            }

            var labelRect = new Rectangle(
                chipRect.X + (int)(7 * dpi), chipRect.Y,
                chipRect.Width - (showClose ? closeSize : 0) - (int)(10 * dpi), chipRect.Height);
            TextRenderer.DrawText(g, label, font, labelRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            if (showClose)
            {
                clearRect = new Rectangle(
                    chipRect.Right - closeSize - (int)(3 * dpi),
                    chipRect.Y + (chipRect.Height - closeSize) / 2,
                    closeSize, closeSize);
                DrawCloseGlyph(g, clearRect, textColor);
            }
            else
            {
                clearRect = Rectangle.Empty;
            }

            return chipRect.Right + chipGap;
        }

        private static GraphicsPath CreateChipPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int r = Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2));
            if (r <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            int d = r * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddLine(rect.X + r, rect.Y, rect.Right - r, rect.Y);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddLine(rect.Right, rect.Y + r, rect.Right, rect.Bottom - r);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddLine(rect.Right - r, rect.Bottom, rect.X + r, rect.Bottom);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.AddLine(rect.X, rect.Bottom - r, rect.X, rect.Y + r);
            path.CloseFigure();
            return path;
        }

        private static void DrawCloseGlyph(Graphics g, Rectangle rect, Color color)
        {
            int inset = Math.Max(2, rect.Width / 4);
            using var pen = new Pen(color, 1.5f);
            g.DrawLine(pen, rect.Left + inset, rect.Top + inset, rect.Right - inset, rect.Bottom - inset);
            g.DrawLine(pen, rect.Right - inset, rect.Top + inset, rect.Left + inset, rect.Bottom - inset);
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
        /// <summary>
        /// Draws the 1-based multi-sort position next to a sort arrow.
        /// </summary>
        private void DrawSortOrderBadge(Graphics g, Rectangle sortRect, int order)
        {
            if (sortRect.Width <= 0 || sortRect.Height <= 0) return;

            var color = Theme?.GridHeaderForeColor ?? (IsDarkTheme ? Color.FromArgb(229, 231, 235) : Color.FromArgb(31, 41, 55));
            float size = Math.Max(6f, sortRect.Height * 0.45f);

            var font = GetSortBadgeFont(size);
            var badgeRect = new Rectangle(sortRect.Right - 2, sortRect.Top, sortRect.Width, sortRect.Height);
            TextRenderer.DrawText(g, order.ToString(), font, badgeRect, color,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }

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
            var painter = _grid.HeaderPainter;

            // Per-style background: the painter owns the header chrome so each GridStyle renders
            // its own look. The uniform renderer remains only as the no-painter fallback.
            if (painter != null)
            {
                painter.PaintHeaderBackground(g, cellRect, isHovered, Theme);
            }
            else if (UseHeaderGradient)
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
            var font = UseBoldHeaderText ? GetBoldHeaderFont(baseFont) : baseFont;

            bool isSystemColumn = column.IsSelectionCheckBox || column.IsRowNumColumn || column.IsRowID;
            bool hasSortArea = ShowSortIndicators && SortIconVisibility != HeaderIconVisibility.Hidden && !isSystemColumn && column.AllowSort;
            bool hasMenuArea = !_grid.ShowTopFilterPanel && FilterIconVisibility != HeaderIconVisibility.Hidden && !isSystemColumn;

            // Geometry comes from the active style painter, so Material/AGGrid/Fluent/Telerik each
            // place the caption, indicator and menu their own way. The renderer only records where
            // the interactive parts landed, which is what keeps input handling style-agnostic.
            var layout = painter?.CalculateHeaderCellLayout(cellRect, column, _grid, _grid.DeviceDpi / 96f);

            Rectangle textRect, sortIconRect, menuButtonRect;
            if (layout != null)
            {
                textRect = layout.TextRect;
                sortIconRect = hasSortArea ? layout.SortIndicatorRect : Rectangle.Empty;
                menuButtonRect = hasMenuArea ? layout.MenuButtonRect : Rectangle.Empty;
            }
            else
            {
                textRect = new Rectangle(cellRect.X + HeaderCellPadding, cellRect.Y,
                    Math.Max(1, cellRect.Width - HeaderCellPadding * 2), cellRect.Height);
                sortIconRect = menuButtonRect = Rectangle.Empty;
            }

            if (!string.IsNullOrEmpty(text))
            {
                var flags = GetTextFormatFlagsForAlignment(column.HeaderTextAlignment, true);
                TextRenderer.DrawText(g, text, font, textRect, textColor, flags);
            }

            // Sort indicator: the slot is reserved whether or not the column is sorted (so the
            // caption never reflows on the sort click); the glyph itself appears when sorted, or on
            // hover as an affordance.
            if (!sortIconRect.IsEmpty)
            {
                var direction = (column.IsSorted && column.ShowSortIcon)
                    ? column.SortDirection
                    : TheTechIdea.Beep.Vis.Modules.SortDirection.None;

                bool drawGlyph = direction != TheTechIdea.Beep.Vis.Modules.SortDirection.None
                                 || SortIconVisibility == HeaderIconVisibility.Always
                                 || isHovered;
                if (drawGlyph)
                {
                    if (painter != null)
                        painter.PaintSortIndicator(g, sortIconRect, direction, Theme);
                    else
                        DrawSortIndicator(g, sortIconRect, direction);

                    // Multi-sort: show this column's position so a two-column sort reads 1 and 2
                    // rather than two identical arrows.
                    int sortedCount = _grid.Data.Columns.Count(c => c.IsSorted);
                    if (column.IsSorted && column.SortOrder > 0 && sortedCount > 1)
                        DrawSortOrderBadge(g, sortIconRect, column.SortOrder);
                }

                // Registered regardless of glyph visibility: the slot stays clickable so the user
                // does not have to hover-then-aim to sort.
                _headerSortIconRects[columnIndex] = sortIconRect;
            }
            else
            {
                _headerSortIconRects.Remove(columnIndex);
            }

            // Column menu button (sort + filter + clear in one place).
            bool showMenu = !menuButtonRect.IsEmpty &&
                (FilterIconVisibility == HeaderIconVisibility.Always || isHovered || column.IsFiltered);

            if (showMenu && painter != null)
            {
                painter.PaintColumnMenuButton(g, menuButtonRect, column.IsFiltered, isHovered, Theme);
            }

            if (!menuButtonRect.IsEmpty)
                _headerFilterIconRects[columnIndex] = menuButtonRect;
            else
                _headerFilterIconRects.Remove(columnIndex);

            // The caption itself sorts — the first thing users try. Recorded per column so the
            // input helper can route a header-text click without re-deriving geometry.
            if (hasSortArea && layout != null && !layout.SortHitRect.IsEmpty)
                _headerSortHitRects[columnIndex] = layout.SortHitRect;
            else
                _headerSortHitRects.Remove(columnIndex);

            // Border
            if (ShowGridLines)
            {
                using (var pen = new Pen(Theme?.GridLineColor ?? (isDark ? Color.FromArgb(60, 70, 85) : Color.FromArgb(180, 180, 180))))
                {
                    pen.DashStyle = GridLineStyle;
                    g.DrawRectangle(pen, cellRect);
                }
            }

            // Skill § perf: bold font is now cached in GridRenderHelper.GetBoldHeaderFont().
            // No per-draw allocation or dispose needed.
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
            int stickyWidth = stickyColumns.Sum(c => Math.Max(BeepLayoutMetrics.GridColumnMinW, c.Width));
            stickyWidth = Math.Min(stickyWidth, rowsRect.Width);

            int currentY = rowsRect.Top;
            int firstVisibleRowIndex = _grid.Scroll.FirstVisibleRowIndex;
            int totalRowsHeight;

            if (_grid.EnableVirtualization && _grid.VirtualDataSource != null)
            {
                totalRowsHeight = (int)(_grid.RowVirtualizer.WindowStart * _grid.RowHeight);
                for (int i = 0; i < firstVisibleRowIndex && i < _grid.Data.Rows.Count; i++)
                {
                    var row = _grid.Data.Rows[i];
                    if (!row.IsVisible) continue;
                    totalRowsHeight += row.Height > 0 ? row.Height : _grid.RowHeight;
                }
            }
            else
            {
                totalRowsHeight = _grid.Layout.RowMetrics.TopOfRow(firstVisibleRowIndex);
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
                    GetLastRenderableRowIndex(_grid.Data.Rows, rowsRect.Height, visibleRowStart, pixelOffset));
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
                            x += Math.Max(BeepLayoutMetrics.GridColumnMinW, scrollCols[si].Col.Width);
                    }

                    for (int si = firstScrollCol; si <= lastScrollCol && si < scrollCols.Count; si++)
                    {
                        var sc = scrollCols[si];
                        int colW = Math.Max(BeepLayoutMetrics.GridColumnMinW, sc.Col.Width);
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

                                var (cfBack, cfFore, cfFontStyle) = ApplyConditionalFormatting(row, r, sc.Col, cell, back, fore, isSelectedRow, isActiveRow);
                                back = cfBack;
                                fore = cfFore;

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
                                    DrawCellContent(g, sc.Col, cell, rect, fore, back, cfFontStyle);
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
                int colW = Math.Max(BeepLayoutMetrics.GridColumnMinW, st.Col.Width);
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

                            var (cfBack, cfFore, cfFontStyle) = ApplyConditionalFormatting(row, r, st.Col, cell, back, fore, isSelectedRow, isActiveRow);
                            back = cfBack;
                            fore = cfFore;

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
                                var font = GetCellFont(cfFontStyle);
                                TextRenderer.DrawText(g, cell.CellValue?.ToString() ?? string.Empty, font, rect, fore,
                                    TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                            }
                            else
                            {
                                DrawCellContent(g, st.Col, cell, rect, fore, back, cfFontStyle);
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
                int colW = Math.Max(BeepLayoutMetrics.GridColumnMinW, st.Width);
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
                    x += Math.Max(BeepLayoutMetrics.GridColumnMinW, scrollCols[si].Width);
            }

            for (int si = firstSumCol; si <= lastSumCol && si < scrollCols.Count; si++)
            {
                var sc = scrollCols[si];
                int colW = Math.Max(BeepLayoutMetrics.GridColumnMinW, sc.Width);
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
                int inset = (int)System.Math.Ceiling(FocusedCellBorderWidth / 2f);
                var borderRect = Rectangle.Inflate(focusRect, -inset, -inset);
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
