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
    /// Style 13 — MinimalCompare: No column fill, thin borders only,
    /// clean sans-serif typography. Optimized for embedded / long-form content.
    /// Inspired by Primer variant="minimal" comparison tables.
    /// </summary>
    public class VerticalTableStyle13Painter : IVerticalTablePainter
    {
        private int _labelColumnWidth = 180;

        public void CalculateLayout(BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, int headerHeight, int rowHeight, int columnWidth, int padding, bool showImage)
        {
            if (layout == null) return;
            var layoutColumns = new List<VerticalColumnLayout>();
            if (columns == null || columns.Count == 0)
            {
                layout.SetColumns(layoutColumns);
                return;
            }

            int x = padding + _labelColumnWidth;

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
                x += columnWidth + padding;
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
                theme = table._currentTheme ?? (table.UseThemeColors ? BeepThemesManager.CurrentTheme : null);
                useThemeColors = table.UseThemeColors;
                highlightedCol = table.HighlightedColumnIndex;
            }

            int padding = 8;
            int headerHeight = layout.Columns.Count > 0 ? layout.Columns[0].HeaderBounds.Height : 50;
            int rowHeight = layout.Columns.Count > 0 && layout.Columns[0].Cells.Count > 0
                ? layout.Columns[0].Cells[0].Bounds.Height : 44;

            // White/transparent background
            Color bg = Color.White;
            if (useThemeColors && theme != null && theme.BackgroundColor != Color.Empty)
                bg = theme.BackgroundColor;
            using (var backBrush = new SolidBrush(bg))
                g.FillRectangle(backBrush, bounds);

            int maxRows = 0;
            foreach (var col in layout.Columns)
                maxRows = Math.Max(maxRows, col.Cells.Count);

            // Draw thin horizontal separator lines (no vertical lines — minimal)
            for (int row = 0; row <= maxRows; row++)
            {
                int lineY = padding + headerHeight + row * rowHeight - 1;
                Color lineColor = row == 0
                    ? Color.FromArgb(180, 190, 200)  // thickest separator under header
                    : Color.FromArgb(230, 235, 240); // thin between rows

                using (var pen = new Pen(lineColor, row == 0 ? 2 : 1))
                    g.DrawLine(pen, padding, lineY, bounds.Width - padding, lineY);
            }

            // Draw label column (feature names)
            DrawLabelColumn(g, columns, padding, headerHeight, rowHeight, maxRows, layout, theme, useThemeColors);

            // Draw data columns
            foreach (var col in layout.Columns)
            {
                var column = col.Column;
                if (column == null) continue;

                bool isColumnHovered = layout.HoverColumnIndex == col.ColumnIndex;
                bool isColumnSelected = layout.SelectedColumnIndex == col.ColumnIndex;
                bool isFeatured = col.ColumnIndex == highlightedCol;

                DrawMinimalHeader(g, col, column, isColumnHovered, isColumnSelected, isFeatured, theme, useThemeColors);

                foreach (var cell in col.Cells)
                {
                    bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                    bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                    bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                    bool isRowSelected = layout.IsRowSelected(cell.RowIndex);

                    DrawMinimalCell(g, cell, isCellHovered || isRowHovered,
                        isCellSelected || isRowSelected, isFeatured, theme, useThemeColors);
                }
            }
        }

        private void DrawLabelColumn(Graphics g, BindingList<SimpleItem> columns, int padding,
            int headerHeight, int rowHeight, int maxRows,
            VerticalTableLayoutHelper layout, IBeepTheme? theme, bool useThemeColors)
        {
            // Empty corner (no fill)
            var cornerRect = new Rectangle(padding, padding, _labelColumnWidth, headerHeight);

            // Feature labels
            if (columns.Count > 0 && columns[0].Children != null)
            {
                for (int i = 0; i < Math.Min(maxRows, columns[0].Children.Count); i++)
                {
                    var item = columns[0].Children[i];
                    if (item == null) continue;

                    int y = padding + headerHeight + i * rowHeight;
                    var labelRect = new Rectangle(padding, y, _labelColumnWidth, rowHeight);

                    bool isRowHovered = layout.HoverRowIndex == i;
                    bool isRowSelected = layout.IsRowSelected(i);

                    // Subtle background on hover
                    if (isRowHovered || isRowSelected)
                    {
                        Color hoverBg = isRowSelected
                            ? Color.FromArgb(240, 248, 255)
                            : Color.FromArgb(249, 250, 251);
                        using (var brush = new SolidBrush(hoverBg))
                            g.FillRectangle(brush, labelRect);
                    }

                    var textRect = new Rectangle(labelRect.Left + 16, labelRect.Top, labelRect.Width - 32, labelRect.Height);
                    using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
                    {
                        Color textColor = isRowSelected ? Color.FromArgb(29, 78, 216) : Color.FromArgb(75, 85, 99);
                        using (var brush = new SolidBrush(textColor))
                        {
                            var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                            g.DrawString(item.Text ?? item.Name ?? "", font, brush, textRect, sf);
                        }
                    }
                }
            }
        }

        private void DrawMinimalHeader(Graphics g, VerticalColumnLayout col, SimpleItem column,
            bool isHovered, bool isSelected, bool isFeatured,
            IBeepTheme? theme, bool useThemeColors)
        {
            var rect = col.HeaderBounds;

            // No background fill (minimal style)
            // Subtle highlight on hover
            if (isSelected || isHovered)
            {
                Color hoverBg = isSelected ? Color.FromArgb(245, 248, 255) : Color.FromArgb(251, 252, 253);
                using (var brush = new SolidBrush(hoverBg))
                    g.FillRectangle(brush, rect);
            }

            // Featured column: thin accent line at top of header
            if (isFeatured)
            {
                Color accent = theme != null && useThemeColors && theme.AccentColor != Color.Empty
                    ? theme.AccentColor : Color.FromArgb(99, 102, 241);
                using (var pen = new Pen(accent, 3))
                    g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
            }

            // Title
            Color textColor = isFeatured
                ? (theme != null && useThemeColors && theme.AccentColor != Color.Empty
                    ? theme.AccentColor : Color.FromArgb(99, 102, 241))
                : Color.FromArgb(30, 41, 59);

            using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
            {
                var titleRect = new Rectangle(rect.Left + 16, rect.Top + 8, rect.Width - 32, rect.Height - 16);
                using (var brush = new SolidBrush(textColor))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(column.Text ?? column.Name ?? "", font, brush, titleRect, sf);
                }
            }
        }

        private void DrawMinimalCell(Graphics g, VerticalCellLayout cell, bool isHovered, bool isSelected,
            bool isFeatured, IBeepTheme? theme, bool useThemeColors)
        {
            var rect = cell.Bounds;
            var item = cell.Item;
            if (item == null) return;

            // Subtle hover background
            if (isHovered || isSelected)
            {
                Color hoverBg = isSelected ? Color.FromArgb(240, 248, 255) : Color.FromArgb(251, 252, 253);
                using (var brush = new SolidBrush(hoverBg))
                    g.FillRectangle(brush, rect);
            }

            string? text = item.Text ?? item.Name;
            bool isNegative = !string.IsNullOrEmpty(text) &&
                (text?.ToLower() is "x" or "false" or "no" or "-" or "—");

            if (item.IsEnabled && string.IsNullOrEmpty(text))
            {
                // Checkmark
                int iconSize = 18;
                int iconX = rect.Left + (rect.Width - iconSize) / 2;
                int iconY = rect.Top + (rect.Height - iconSize) / 2;

                Color checkColor = isFeatured
                    ? (theme != null && useThemeColors && theme.AccentColor != Color.Empty
                        ? theme.AccentColor : Color.FromArgb(99, 102, 241))
                    : Color.FromArgb(16, 185, 129);

                using (var pen = new Pen(checkColor, 2f))
                {
                    pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, iconX + 3, iconY + 9, iconX + 7, iconY + 13);
                    g.DrawLine(pen, iconX + 7, iconY + 13, iconX + 15, iconY + 5);
                }
            }
            else if (isNegative)
            {
                // Dash
                int dashY = rect.Top + rect.Height / 2;
                using (var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f))
                    g.DrawLine(pen, rect.Left + rect.Width / 2 - 8, dashY, rect.Left + rect.Width / 2 + 8, dashY);
            }
            else if (!string.IsNullOrEmpty(text))
            {
                // Text — clean, minimal
                using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
                using (var brush = new SolidBrush(Color.FromArgb(75, 85, 99)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(text, font, brush, rect, sf);
                }
            }
        }

        public void OnCellSelected(VerticalTableLayoutHelper layout, SimpleItem? item, int columnIndex, int rowIndex) { }
        public void OnCellHoverChanged(VerticalTableLayoutHelper layout, int columnIndex, int rowIndex) { }
    }
}
