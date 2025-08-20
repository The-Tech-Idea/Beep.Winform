using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls; // Use BeepControls for rendering/editing
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Helpers; // ImageListHelper

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridRenderHelper
    {
        private readonly BeepGridPro _grid;
        private IBeepTheme Theme => BeepThemesManager.GetTheme(_grid.Theme);
        private readonly System.Collections.Generic.Dictionary<string, IBeepUIComponent> _columnDrawers = new();
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
        private BeepCheckBoxBool _headerCheck;
        private BeepCheckBoxBool _rowCheck;

        public GridRenderHelper(BeepGridPro grid) { _grid = grid; }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            DrawBackground(g);
            if (_grid.Layout.ShowColumnHeaders)
                DrawHeaders(g);
            DrawRows(g);
            // Ensure no clipping region is active before drawing navigator
            g.ResetClip();
            if (_grid.ShowNavigator)
                DrawNavigator(g);
            DrawSelection(g);
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

        private void DrawBackground(Graphics g)
        {
            using var b = new SolidBrush(Theme?.GridBackColor == Color.Empty ? _grid.BackColor : Theme.GridBackColor);
            g.FillRectangle(b, _grid.ClientRectangle);
        }

        private void DrawHeaders(Graphics g)
        {
            var rect = _grid.Layout.HeaderRect;
            if (rect == Rectangle.Empty) return;
            using var bg = new SolidBrush(Theme?.GridHeaderBackColor ?? SystemColors.Control);
            using var pen = new Pen(Theme?.GridHeaderBorderColor == Color.Empty ? (Theme?.GridLineColor ?? SystemColors.ControlDark) : Theme.GridHeaderBorderColor);
            g.FillRectangle(bg, rect);

            using var headerFont = BeepThemesManager.ToFont(Theme?.GridHeaderFont) ?? _grid.Font;

            // Draw select-all checkbox if enabled
            if (_grid.ShowCheckBox && _grid.Layout.SelectAllCheckRect != Rectangle.Empty)
            {
                _headerCheck ??= new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme };
                _headerCheck.Draw(g, _grid.Layout.SelectAllCheckRect);
            }

            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                var col = _grid.Data.Columns[i];
                var cellRect = _grid.Layout.HeaderCellRects.Length > i
                    ? _grid.Layout.HeaderCellRects[i]
                    : Rectangle.Empty;
                if (cellRect == Rectangle.Empty) continue;

                var headerBack = col.HasCustomHeaderBackColor && col.UseCustomColors ? col.ColumnHeaderBackColor : (Theme?.GridHeaderBackColor ?? SystemColors.Control);
                var headerFore = col.HasCustomHeaderForeColor && col.UseCustomColors ? col.ColumnHeaderForeColor : (Theme?.GridHeaderForeColor ?? SystemColors.ControlText);
                using var hbg = new SolidBrush(headerBack);
                using var hfg = new SolidBrush(headerFore);
                g.FillRectangle(hbg, cellRect);

                TextRenderer.DrawText(g, col.ColumnCaption ?? col.ColumnName, headerFont, cellRect, hfg.Color,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                g.DrawRectangle(pen, cellRect);
            }
        }

        private void DrawRows(Graphics g)
        {
            var rowsRect = _grid.Layout.RowsRect;
            using var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark);

            int extraLeft = _grid.ShowCheckBox ? _grid.Layout.CheckBoxColumnWidth : 0;

            int stickyWidth = _grid.Data.Columns.Where(c => c.Visible && c.Sticked).Sum(c => Math.Max(20, c.Width));
            stickyWidth = Math.Min(stickyWidth, Math.Max(0, rowsRect.Width - extraLeft));

            int visibleRows = Math.Max(1, rowsRect.Height / _grid.RowHeight);
            int startRow = Math.Max(0, _grid.Scroll.FirstVisibleRowIndex);
            int endRow = Math.Min(_grid.Data.Rows.Count - 1, startRow + visibleRows - 1);

            Rectangle scrollingRegion = new Rectangle(rowsRect.Left + extraLeft + stickyWidth, rowsRect.Top, Math.Max(0, rowsRect.Width - extraLeft - stickyWidth), rowsRect.Height);
            var state1 = g.Save();
            g.SetClip(scrollingRegion);
            var scrollCols = _grid.Data.Columns.Select((c, idx) => new { Col = c, Index = idx })
                                               .Where(x => x.Col.Visible && !x.Col.Sticked)
                                               .ToList();
            for (int r = startRow; r <= endRow; r++)
            {
                int y = rowsRect.Top + (r - startRow) * _grid.RowHeight;
                int x = rowsRect.Left + extraLeft + stickyWidth - _grid.Scroll.HorizontalOffset;

                if (_grid.ShowCheckBox)
                {
                    _rowCheck ??= new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme };
                    var cbRect = _grid.Data.Rows[r].RowCheckRect;
                    if (cbRect != Rectangle.Empty)
                        _rowCheck.Draw(g, cbRect);
                }

                foreach (var sc in scrollCols)
                {
                    var col = sc.Col;
                    var cell = _grid.Data.Rows[r].Cells[sc.Index];
                    var rect = new Rectangle(x, y, Math.Max(20, col.Width), _grid.RowHeight);

                    var back = r == _grid.Selection.RowIndex
                        ? (Theme?.GridRowSelectedBackColor == Color.Empty ? (Theme?.SelectedRowBackColor ?? SystemColors.Highlight) : Theme.GridRowSelectedBackColor)
                        : (col.HasCustomBackColor && col.UseCustomColors ? col.ColumnBackColor : (Theme?.GridBackColor ?? SystemColors.Window));
                    var fore = col.HasCustomForeColor && col.UseCustomColors ? col.ColumnForeColor : (Theme?.GridForeColor ?? SystemColors.WindowText);

                    using (var bg = new SolidBrush(back)) g.FillRectangle(bg, rect);
                    DrawCellWithBeepControl(g, col, cell, rect, fore, back);
                    g.DrawRectangle(pen, rect);

                    x += Math.Max(20, col.Width);
                    if (x > scrollingRegion.Right) break;
                }
            }
            g.Restore(state1);

            Rectangle stickyRegion = new Rectangle(rowsRect.Left + extraLeft, rowsRect.Top, stickyWidth, rowsRect.Height);
            var state2 = g.Save();
            g.SetClip(stickyRegion);
            var stickyCols = _grid.Data.Columns.Select((c, idx) => new { Col = c, Index = idx })
                                               .Where(x => x.Col.Visible && x.Col.Sticked)
                                               .ToList();
            int startX = rowsRect.Left + extraLeft;
            foreach (var st in stickyCols)
            {
                int colW = Math.Max(20, st.Col.Width);
                for (int r = startRow; r <= endRow; r++)
                {
                    int y = rowsRect.Top + (r - startRow) * _grid.RowHeight;
                    var cell = _grid.Data.Rows[r].Cells[st.Index];
                    var rect = new Rectangle(startX, y, colW, _grid.RowHeight);

                    var back = r == _grid.Selection.RowIndex
                        ? (Theme?.GridRowSelectedBackColor == Color.Empty ? (Theme?.SelectedRowBackColor ?? SystemColors.Highlight) : Theme.GridRowSelectedBackColor)
                        : (st.Col.HasCustomBackColor && st.Col.UseCustomColors ? st.Col.ColumnBackColor : (Theme?.GridBackColor ?? SystemColors.Window));
                    var fore = st.Col.HasCustomForeColor && st.Col.UseCustomColors ? st.Col.ColumnForeColor : (Theme?.GridForeColor ?? SystemColors.WindowText);

                    using (var bg = new SolidBrush(back)) g.FillRectangle(bg, rect);
                    DrawCellWithBeepControl(g, st.Col, cell, rect, fore, back);
                    g.DrawRectangle(pen, rect);
                }
                startX += colW;
                if (startX >= stickyRegion.Right) break;
            }
            g.Restore(state2);
        }

        private void EnsureNavigatorButtons()
        {
            if (_btnFirst != null) return;

            // CRUD and actions
            _btnInsert = new BeepButton { ImagePath =BeepSvgPaths.NavPlus, Theme = _grid.Theme };
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

        private void DrawNavigator(Graphics g)
        {
            var navRect = _grid.Layout is { } ? _grid.Layout.GetType().GetProperty("NavigatorRect")?.GetValue(_grid.Layout) as Rectangle? ?? Rectangle.Empty : Rectangle.Empty;
            if (navRect == Rectangle.Empty) return;

            using var bg = new SolidBrush(Theme?.GridHeaderBackColor ?? SystemColors.Control);
            using var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark);
            g.FillRectangle(bg, navRect);
            g.DrawLine(pen, navRect.Left, navRect.Top, navRect.Right, navRect.Top);

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

            using var headerFont = BeepThemesManager.ToFont(Theme?.GridHeaderFont) ?? _grid.Font;
            string recordCounter = (_grid.Data.Rows.Count > 0 && _grid.Selection.RowIndex >= 0)
                ? ($"{_grid.Selection.RowIndex + 1} - {_grid.Data.Rows.Count}")
                : "0 - 0";

            using var fg = new SolidBrush(Theme?.GridHeaderForeColor ?? SystemColors.ControlText);
            var textSize = TextRenderer.MeasureText(recordCounter, headerFont);
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

            TextRenderer.DrawText(g, recordCounter, headerFont, new Rectangle((int)centerX, y, (int)textSize.Width, buttonHeight), fg.Color,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

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

        private void DrawCellWithBeepControl(Graphics g, BeepColumnConfig col, BeepCellConfig cell, Rectangle rect, Color foreColor, Color backColor)
        {
            if (col == null) return;

            if (!_columnDrawers.TryGetValue(col.ColumnName ?? col.ColumnCaption ?? col.GuidID, out var drawer) || drawer == null)
            {
                drawer = CreateCellDrawer(col);
                _columnDrawers[col.ColumnName ?? col.ColumnCaption ?? col.GuidID] = drawer;
            }

            if (drawer is Control ctrl)
            {
                ctrl.ForeColor = foreColor;
                ctrl.BackColor = backColor;
            }

            UpdateDrawerFromCell(drawer, col, cell);

            if (drawer is BeepControl bc)
            {
                bc.Theme = _grid.Theme;
                bc.UseThemeFont = true;
                bc.IsFrameless = true;
                bc.Draw(g, rect);
            }
            else
            {
                using var fg = new SolidBrush(foreColor);
                using var cellFont = BeepThemesManager.ToFont(Theme?.GridCellFont) ?? _grid.Font;
                TextRenderer.DrawText(g, cell.CellValue?.ToString() ?? string.Empty, cellFont, rect, foreColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
        }

        private IBeepUIComponent CreateCellDrawer(BeepColumnConfig col)
        {
            IBeepUIComponent result;
            switch (col.CellEditor)
            {
                case BeepColumnType.CheckBoxBool:
                    result = new BeepCheckBoxBool { IsChild = true, HideText = true, GridMode = true };
                    break;
                case BeepColumnType.CheckBoxChar:
                    result = new BeepCheckBoxChar { IsChild = true, HideText = true, GridMode = true };
                    break;
                case BeepColumnType.CheckBoxString:
                    result = new BeepCheckBoxString { IsChild = true, HideText = true, GridMode = true };
                    break;
                case BeepColumnType.ComboBox:
                    result = new BeepComboBox { IsChild = true, GridMode = true };
                    break;
                case BeepColumnType.DateTime:
                    result = new BeepDatePicker { IsChild = true, GridMode = true };
                    break;
                case BeepColumnType.Image:
                    result = new BeepImage { IsChild = true, GridMode = true };
                    break;
                case BeepColumnType.Button:
                    result = new BeepButton { IsChild = true, GridMode = true };
                    break;
                case BeepColumnType.ProgressBar:
                    result = new BeepProgressBar { IsChild = true, GridMode = true };
                    break;
                case BeepColumnType.NumericUpDown:
                    result = new BeepNumericUpDown { IsChild = true, GridMode = true };
                    break;
                case BeepColumnType.Radio:
                    result = new BeepRadioButton { IsChild = true, GridMode = true };
                    break;
                case BeepColumnType.ListBox:
                    result = new BeepListBox { IsChild = true, GridMode = true };
                    break;
                case BeepColumnType.ListOfValue:
                    result = new BeepListofValuesBox { IsChild = true, GridMode = true };
                    break;
                default:
                    result = new BeepTextBox { IsChild = true, GridMode = true };
                    break;
            }
            return result;
        }

        private void UpdateDrawerFromCell(IBeepUIComponent drawer, BeepColumnConfig col, BeepCellConfig cell)
        {
            if (drawer == null) return;

            if (drawer is BeepComboBox combo)
            {
                if (col.Items != null)
                {
                    if (!string.IsNullOrEmpty(col.ParentColumnName) && cell.ParentCellValue != null)
                    {
                        var filtered = col.Items.Where(i => Equals(i.ParentValue, cell.ParentCellValue)).ToList();
                        combo.ListItems = new System.ComponentModel.BindingList<SimpleItem>(filtered);
                    }
                    else
                    {
                        combo.ListItems = new System.ComponentModel.BindingList<SimpleItem>(col.Items);
                    }
                }
                combo.SetValue(cell.CellValue);
            }
            else if (drawer is BeepListBox listBox)
            {
                if (col.Items != null)
                    listBox.ListItems = new System.ComponentModel.BindingList<SimpleItem>(col.Items);
                listBox.SetValue(cell.CellValue);
            }
            else if (drawer is BeepListofValuesBox lov)
            {
                lov.ListItems = col.Items?.ToList() ?? new System.Collections.Generic.List<SimpleItem>();
                lov.SetValue(cell.CellValue);
            }
            else if (drawer is BeepDatePicker dp)
            {
                if (cell.CellValue is DateTime dt)
                    dp.SelectedDate = dt.ToString(dp.GetCurrentFormat(), dp.Culture);
                else if (cell.CellValue != null)
                    dp.SelectedDate = cell.CellValue.ToString();
                else
                    dp.SelectedDate = null;
            }
            else if (drawer is BeepImage img)
            {
                img.ImagePath = ImageListHelper.GetImagePathFromName(cell.CellValue?.ToString());
            }
            else if (drawer is BeepProgressBar pb)
            {
                pb.Value = int.TryParse(cell.CellValue?.ToString(), out var v) ? v : 0;
            }
            else
            {
                // Generic SetValue support for other editors (text, checkbox, numeric, radio, etc.)
                drawer.SetValue(cell.CellValue);
            }
        }

        private void DrawSelection(Graphics g)
        {
            if (!_grid.Selection.HasSelection) return;
            var rect = _grid.Selection.SelectedCellRect;
            if (rect == Rectangle.Empty) return;

            var selBorder = Theme?.GridRowSelectedBorderColor;
            using var selPen = new Pen(selBorder == null || selBorder == Color.Empty ? Color.FromArgb(160, 30, 144, 255) : selBorder.Value, 2f);
            g.DrawRectangle(selPen, rect);
        }
    }
}
