using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers;
using System.ComponentModel;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Painters
{
    /// <summary>
    /// Style 12 — FeatureMatrix: Horizontal-scroll grid with sticky headers,
    /// zebra stripes, and check/cross icon indicators.
    /// Inspired by Sky UI and SaaS dashboard comparison tables.
    /// </summary>
    public class VerticalTableStyle12Painter : IVerticalTablePainter
    {
        private int _featureLabelWidth = 200;

        public void CalculateLayout(BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, int headerHeight, int rowHeight, int columnWidth, int padding, bool showImage)
        {
            if (layout == null) return;
            var layoutColumns = new List<VerticalColumnLayout>();
            if (columns == null || columns.Count == 0)
            {
                layout.SetColumns(layoutColumns);
                return;
            }

            int x = padding + _featureLabelWidth;

            for (int colIdx = 0; colIdx < columns.Count; colIdx++)
            {
                var column = columns[colIdx];
                if (column == null || !column.IsVisible) continue;

                var colLayout = new VerticalColumnLayout
                {
                    Column = column,
                    ColumnIndex = colIdx,
                    HeaderBounds = new Rectangle(x, padding, columnWidth, headerHeight),
                    ColumnBounds = new Rectangle(x, padding, columnWidth, layout.Owner.ClientSize.Height - padding * 2)
                };

                int y = padding + headerHeight;
                if (column.Children != null)
                {
                    for (int rowIdx = 0; rowIdx < column.Children.Count; rowIdx++)
                    {
                        var rowItem = column.Children[rowIdx];
                        if (rowItem == null || !rowItem.IsVisible) continue;

                        var cellRect = new Rectangle(x, y, columnWidth, rowHeight);
                        colLayout.Cells.Add(new VerticalCellLayout
                        {
                            Item = rowItem,
                            Bounds = cellRect,
                            RowIndex = rowIdx,
                            ColumnIndex = colIdx
                        });
                        y += rowHeight;
                    }
                }

                layoutColumns.Add(colLayout);
                x += columnWidth;
            }

            layout.SetColumns(layoutColumns);
        }

        public void Paint(Graphics g, Rectangle bounds, BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, object owner)
        {
            if (g == null || layout == null || columns == null) return;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            IBeepTheme? theme = null;
            bool useThemeColors = false;
            int highlightedCol = -1;

            if (owner is BeepVerticalTable table)
            {
                theme = table._currentTheme;
                useThemeColors = table.UseThemeColors;
                highlightedCol = table.HighlightedColumnIndex;
            }

            int padding = 8;
            int headerHeight = layout.Columns.Count > 0 ? layout.Columns[0].HeaderBounds.Height : 50;
            int rowHeight = layout.Columns.Count > 0 && layout.Columns[0].Cells.Count > 0
                ? layout.Columns[0].Cells[0].Bounds.Height : 44;

            // Background
            using (var backBrush = new SolidBrush(VerticalTableThemeHelpers.Cur.GridHeaderBackColor))
                g.FillRectangle(backBrush, bounds);

            int maxRows = 0;
            foreach (var col in layout.Columns)
                maxRows = Math.Max(maxRows, col.Cells.Count);
            int tableHeight = headerHeight + maxRows * rowHeight;
            int tableWidth = _featureLabelWidth;
            foreach (var col in layout.Columns)
                tableWidth += col.HeaderBounds.Width;

            // Table border
            var tableRect = new Rectangle(padding, padding, tableWidth, tableHeight);
            using (var pen = new Pen(VerticalTableThemeHelpers.Cur.GridLineColor, 1))
                g.DrawRectangle(pen, tableRect);

            // Draw feature label column (sticky first column)
            DrawFeatureLabelColumn(g, columns, padding, headerHeight, rowHeight, maxRows, layout, theme, useThemeColors);

            // Draw data columns
            foreach (var col in layout.Columns)
            {
                var column = col.Column;
                if (column == null) continue;

                bool isColumnHovered = layout.HoverColumnIndex == col.ColumnIndex;
                bool isColumnSelected = layout.SelectedColumnIndex == col.ColumnIndex;
                bool isFeatured = col.ColumnIndex == highlightedCol;

                DrawMatrixHeader(g, col, column, isColumnHovered, isColumnSelected, isFeatured, theme, useThemeColors);

                foreach (var cell in col.Cells)
                {
                    bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                    bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                    bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                    bool isRowSelected = layout.IsRowSelected(cell.RowIndex);

                    DrawMatrixCell(g, cell, isCellHovered || isRowHovered,
                        isCellSelected || isRowSelected, isFeatured, theme, useThemeColors);
                }
            }

            // Draw horizontal grid lines
            for (int row = 0; row <= maxRows; row++)
            {
                int lineY = padding + headerHeight + row * rowHeight;
                using (var pen = new Pen(VerticalTableThemeHelpers.Cur.GridLineColor, 1))
                    g.DrawLine(pen, padding, lineY, padding + tableWidth, lineY);
            }
        }

        private void DrawFeatureLabelColumn(Graphics g, BindingList<SimpleItem> columns, int padding,
            int headerHeight, int rowHeight, int maxRows,
            VerticalTableLayoutHelper layout, IBeepTheme? theme, bool useThemeColors)
        {
            // Corner header cell
            var cornerRect = new Rectangle(padding, padding, _featureLabelWidth, headerHeight);
            using (var brush = new SolidBrush(VerticalTableThemeHelpers.Cur.GridHeaderBackColor))
                g.FillRectangle(brush, cornerRect);
            using (var pen = new Pen(VerticalTableThemeHelpers.Cur.GridLineColor, 1))
                g.DrawRectangle(pen, cornerRect);

            // Feature row labels
            if (columns.Count > 0 && columns[0].Children != null)
            {
                for (int i = 0; i < Math.Min(maxRows, columns[0].Children.Count); i++)
                {
                    var item = columns[0].Children[i];
                    if (item == null) continue;

                    int y = padding + headerHeight + i * rowHeight;
                    var labelRect = new Rectangle(padding, y, _featureLabelWidth, rowHeight);

                    bool isRowHovered = layout.HoverRowIndex == i;
                    bool isRowSelected = layout.IsRowSelected(i);

                    Color bg = (i % 2 == 0) ? VerticalTableThemeHelpers.Cur.GridBackColor : VerticalTableThemeHelpers.Cur.AltRowBackColor;
                    if (isRowSelected) bg = VerticalTableThemeHelpers.Cur.GridRowSelectedBackColor;
                    else if (isRowHovered) bg = VerticalTableThemeHelpers.Cur.GridRowHoverBackColor;

                    using (var brush = new SolidBrush(bg))
                        g.FillRectangle(brush, labelRect);
                    using (var pen = new Pen(VerticalTableThemeHelpers.Cur.GridLineColor, 1))
                        g.DrawRectangle(pen, labelRect);

                    var textRect = new Rectangle(labelRect.Left + 16, labelRect.Top, labelRect.Width - 32, labelRect.Height);
                    using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
                    {
                        Color textColor = isRowSelected
                            ? VerticalTableThemeHelpers.Cur.PrimaryColor
                            : VerticalTableThemeHelpers.Cur.GridForeColor;
                        using (var brush = new SolidBrush(textColor))
                        {
                            var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                            g.DrawString(item.Text ?? item.Name ?? "", font, brush, textRect, sf);
                        }
                    }
                }
            }
        }

        private void DrawMatrixHeader(Graphics g, VerticalColumnLayout col, SimpleItem column,
            bool isHovered, bool isSelected, bool isFeatured,
            IBeepTheme? theme, bool useThemeColors)
        {
            var rect = col.HeaderBounds;

            Color bgColor = VerticalTableThemeHelpers.Cur.GridHeaderBackColor;
            if (isFeatured)
            {
                Color accent = theme != null && useThemeColors && theme.AccentColor != Color.Empty
                    ? theme.AccentColor : VerticalTableThemeHelpers.Cur.AccentColor;
                using (var brush = new LinearGradientBrush(rect, accent, Color.FromArgb(200, accent), 90f))
                    g.FillRectangle(brush, rect);
            }
            else if (isSelected)
            {
                using (var brush = new SolidBrush(VerticalTableThemeHelpers.Cur.GridRowSelectedBackColor))
                    g.FillRectangle(brush, rect);
            }
            else if (isHovered)
            {
                using (var brush = new SolidBrush(VerticalTableThemeHelpers.Cur.GridLineColor))
                    g.FillRectangle(brush, rect);
            }
            else
            {
                using (var brush = new SolidBrush(bgColor))
                    g.FillRectangle(brush, rect);
            }

            // Border
            using (var pen = new Pen(VerticalTableThemeHelpers.Cur.GridLineColor, 1))
                g.DrawRectangle(pen, rect);

            // Title
            Color textColor = isFeatured ? VerticalTableThemeHelpers.Cur.GridHeaderSelectedForeColor : VerticalTableThemeHelpers.Cur.GridHeaderForeColor;
            using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using (var brush = new SolidBrush(textColor))
                    g.DrawString(column.Text ?? column.Name ?? "", font, brush, rect, sf);
            }
        }

        private void DrawMatrixCell(Graphics g, VerticalCellLayout cell, bool isHovered, bool isSelected,
            bool isFeatured, IBeepTheme? theme, bool useThemeColors)
        {
            var rect = cell.Bounds;
            var item = cell.Item;
            if (item == null) return;

            // Background — alternating rows (zebra stripes)
            Color bgColor = (cell.RowIndex % 2 == 0) ? VerticalTableThemeHelpers.Cur.GridBackColor : VerticalTableThemeHelpers.Cur.AltRowBackColor;
            if (isSelected) bgColor = VerticalTableThemeHelpers.Cur.GridRowSelectedBackColor;
            else if (isHovered) bgColor = VerticalTableThemeHelpers.Cur.GridRowHoverBackColor;

            using (var brush = new SolidBrush(bgColor))
                g.FillRectangle(brush, rect);

            using (var pen = new Pen(VerticalTableThemeHelpers.Cur.GridLineColor, 1))
                g.DrawRectangle(pen, rect);

            string? text = item.Text ?? item.Name;
            bool isPositive = item.IsEnabled;
            bool isNegative = !string.IsNullOrEmpty(text) &&
                (text?.ToLower() is "x" or "false" or "no");

            int iconSize = 18;
            int iconX = rect.Left + (rect.Width - iconSize) / 2;
            int iconY = rect.Top + (rect.Height - iconSize) / 2;

            if (isNegative)
            {
                // Red cross
                using (var pen = new Pen(VerticalTableThemeHelpers.Cur.ErrorColor, 2f))
                {
                    pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, iconX + 3, iconY + 3, iconX + iconSize - 3, iconY + iconSize - 3);
                    g.DrawLine(pen, iconX + iconSize - 3, iconY + 3, iconX + 3, iconY + iconSize - 3);
                }
            }
            else if (isPositive && string.IsNullOrEmpty(text))
            {
                // Green checkmark
                Color checkColor = isFeatured
                    ? (theme != null && useThemeColors && theme.AccentColor != Color.Empty
                        ? theme.AccentColor : VerticalTableThemeHelpers.Cur.AccentColor)
                    : VerticalTableThemeHelpers.Cur.SuccessColor;

                using (var pen = new Pen(checkColor, 2.5f))
                {
                    pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, iconX + 2, iconY + 9, iconX + 7, iconY + 14);
                    g.DrawLine(pen, iconX + 7, iconY + 14, iconX + 16, iconY + 4);
                }
            }
            else if (!string.IsNullOrEmpty(text))
            {
                // Text cell
                using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
                using (var brush = new SolidBrush(VerticalTableThemeHelpers.Cur.GridForeColor))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(text, font, brush, rect, sf);
                }
            }
            else
            {
                // Dash for N/A
                using (var pen = new Pen(VerticalTableThemeHelpers.Cur.GridLineColor, 1.5f))
                    g.DrawLine(pen, iconX + 3, iconY + iconSize / 2, iconX + iconSize - 3, iconY + iconSize / 2);
            }
        }

        public void OnCellSelected(VerticalTableLayoutHelper layout, SimpleItem? item, int columnIndex, int rowIndex) { }
        public void OnCellHoverChanged(VerticalTableLayoutHelper layout, int columnIndex, int rowIndex) { }
    }
}
