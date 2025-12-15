using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using System.ComponentModel;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Painters
{
    /// <summary>
    /// Style 6: Classic data table with header row and alternating row colors.
    /// Corporate style with clean grid lines and subtle hover effects.
    /// </summary>
    public class VerticalTableStyle6Painter : IVerticalTablePainter
    {
        private int _rowHeaderWidth = 180;

        /// <inheritdoc/>
        public void CalculateLayout(BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, int headerHeight, int rowHeight, int columnWidth, int padding, bool showImage)
        {
            if (layout == null) return;
            var layoutColumns = new List<VerticalColumnLayout>();
            if (columns == null || columns.Count == 0)
            {
                layout.SetColumns(layoutColumns);
                return;
            }

            int x = padding + _rowHeaderWidth;

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

        /// <inheritdoc/>
        public void Paint(Graphics g, Rectangle bounds, BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, object owner)
        {
            if (g == null || layout == null || columns == null) return;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int padding = 8;
            int headerHeight = layout.Columns.Count > 0 ? layout.Columns[0].HeaderBounds.Height : 50;
            int rowHeight = layout.Columns.Count > 0 && layout.Columns[0].Cells.Count > 0
                ? layout.Columns[0].Cells[0].Bounds.Height : 44;

            // White background
            using (var backBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(backBrush, bounds);
            }

            // Calculate table bounds
            int tableWidth = _rowHeaderWidth + (layout.Columns.Count * (layout.Columns.Count > 0 ? layout.Columns[0].HeaderBounds.Width : 150));
            int maxRows = 0;
            foreach (var col in layout.Columns)
                maxRows = Math.Max(maxRows, col.Cells.Count);
            int tableHeight = headerHeight + (maxRows * rowHeight);

            var tableRect = new Rectangle(padding, padding, tableWidth, tableHeight);

            // Draw table border
            using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
            {
                g.DrawRectangle(pen, tableRect);
            }

            // Draw row header column
            DrawRowHeaderColumn(g, columns, padding, headerHeight, rowHeight, layout);

            // Draw column headers
            foreach (var col in layout.Columns)
            {
                var column = col.Column;
                if (column == null) continue;

                bool isColumnHovered = (layout.HoverColumnIndex == col.ColumnIndex);
                bool isColumnSelected = (layout.SelectedColumnIndex == col.ColumnIndex);

                DrawColumnHeader(g, col, column, isColumnHovered, isColumnSelected);

                // Draw cells
                foreach (var cell in col.Cells)
                {
                    bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                    bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                    bool isColHovered = layout.IsColumnHovered(col.ColumnIndex);
                    bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                    bool isRowSelected = layout.IsRowSelected(cell.RowIndex);
                    bool isColSelected = layout.IsColumnSelected(col.ColumnIndex);

                    DrawCell(g, cell, isCellHovered, isRowHovered, isColHovered, isCellSelected, isRowSelected, isColSelected, layout);
                }
            }
        }

        private void DrawRowHeaderColumn(Graphics g, BindingList<SimpleItem> columns, int padding, int headerHeight, int rowHeight, VerticalTableLayoutHelper layout)
        {
            var headerRect = new Rectangle(padding, padding, _rowHeaderWidth, headerHeight);

            // Corner header cell
            using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
            {
                g.FillRectangle(brush, headerRect);
            }
            using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
            {
                g.DrawRectangle(pen, headerRect);
            }

            // Feature rows
            if (columns.Count > 0 && columns[0].Children != null)
            {
                int y = padding + headerHeight;
                for (int i = 0; i < columns[0].Children.Count; i++)
                {
                    var item = columns[0].Children[i];
                    var rowRect = new Rectangle(padding, y, _rowHeaderWidth, rowHeight);

                    // Alternating row color
                    Color rowBg = (i % 2 == 0) ? Color.White : Color.FromArgb(249, 250, 251);
                    
                    bool isRowHovered = (layout.HoverRowIndex == i);
                    if (isRowHovered)
                        rowBg = Color.FromArgb(239, 246, 255);

                    using (var brush = new SolidBrush(rowBg))
                    {
                        g.FillRectangle(brush, rowRect);
                    }

                    // Border
                    using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    {
                        g.DrawRectangle(pen, rowRect);
                    }

                    // Text
                    var textRect = new Rectangle(rowRect.Left + 16, rowRect.Top, rowRect.Width - 32, rowRect.Height);
                    using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
                    using (var brush = new SolidBrush(Color.FromArgb(51, 65, 85)))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                        g.DrawString(item.Text ?? item.Name ?? $"Row {i + 1}", font, brush, textRect, sf);
                    }

                    y += rowHeight;
                }
            }
        }

        private void DrawColumnHeader(Graphics g, VerticalColumnLayout col, SimpleItem column, bool isHovered, bool isSelected)
        {
            var rect = col.HeaderBounds;

            // Header background
            Color bgColor = Color.FromArgb(248, 250, 252);
            if (isSelected) bgColor = Color.FromArgb(219, 234, 254);
            else if (isHovered) bgColor = Color.FromArgb(241, 245, 249);

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Border
            using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
            {
                g.DrawRectangle(pen, rect);
            }

            if (isSelected)
            {
                using (var pen = new Pen(Color.FromArgb(59, 130, 246), 2))
                {
                    g.DrawRectangle(pen, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
                }
            }

            int yOffset = rect.Top + 10;

            // Column title
            using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(30, 41, 59)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                g.DrawString(column.Text ?? column.Name ?? "", font, brush, new Rectangle(rect.Left, yOffset, rect.Width, 24), sf);
            }
            yOffset += 26;

            // Price/value
            string? priceValue = column.Value?.ToString();
            if (!string.IsNullOrEmpty(priceValue))
            {
                using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.FromArgb(16, 185, 129)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                    g.DrawString(priceValue, font, brush, new Rectangle(rect.Left, yOffset, rect.Width, 22), sf);
                }
            }
        }

        private void DrawCell(Graphics g, VerticalCellLayout cell, 
            bool isCellHovered, bool isRowHovered, bool isColHovered,
            bool isCellSelected, bool isRowSelected, bool isColSelected,
            VerticalTableLayoutHelper layout)
        {
            var rect = cell.Bounds;
            var item = cell.Item;

            // Background with row/column selection highlighting
            Color bgColor = (cell.RowIndex % 2 == 0) ? Color.White : Color.FromArgb(249, 250, 251);
            
            if (isCellSelected)
                bgColor = Color.FromArgb(199, 224, 254); // Strongest for exact cell
            else if (isRowSelected || isColSelected)
                bgColor = (isRowSelected && isColSelected) ? Color.FromArgb(209, 230, 254) : Color.FromArgb(219, 234, 254);
            else if (isCellHovered)
                bgColor = Color.FromArgb(229, 241, 255);
            else if (isRowHovered || isColHovered)
                bgColor = Color.FromArgb(239, 246, 255);

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Border
            using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
            {
                g.DrawRectangle(pen, rect);
            }

            // Selection border
            if (isCellSelected || (isRowSelected && isColSelected))
            {
                using (var pen = new Pen(Color.FromArgb(59, 130, 246), 2))
                {
                    g.DrawRectangle(pen, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
                }
            }
            else if (isRowSelected || isColSelected)
            {
                using (var pen = new Pen(Color.FromArgb(100, 59, 130, 246), 1))
                {
                    g.DrawRectangle(pen, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
                }
            }

            if (item == null) return;

            // Content
            string? text = item.Text ?? item.Name;
            bool hasText = !string.IsNullOrEmpty(text) && text != "-" && text.ToLower() != "x" && text.ToLower() != "false";

            if (hasText)
            {
                // Show text value
                using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
                using (var brush = new SolidBrush(Color.FromArgb(71, 85, 105)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(text, font, brush, rect, sf);
                }
            }
            else
            {
                // Show checkmark or dash
                int iconSize = 18;
                int iconX = rect.Left + (rect.Width - iconSize) / 2;
                int iconY = rect.Top + (rect.Height - iconSize) / 2;

                if (item.IsEnabled)
                {
                    // Green checkmark
                    using (var pen = new Pen(Color.FromArgb(16, 185, 129), 2.5f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        g.DrawLine(pen, iconX + 3, iconY + 9, iconX + 7, iconY + 13);
                        g.DrawLine(pen, iconX + 7, iconY + 13, iconX + 15, iconY + 5);
                    }
                }
                else
                {
                    // Gray dash
                    using (var pen = new Pen(Color.FromArgb(203, 213, 225), 2f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        g.DrawLine(pen, iconX + 4, iconY + iconSize / 2, iconX + iconSize - 4, iconY + iconSize / 2);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnCellSelected(VerticalTableLayoutHelper layout, SimpleItem? item, int columnIndex, int rowIndex) { }

        /// <inheritdoc/>
        public void OnCellHoverChanged(VerticalTableLayoutHelper layout, int columnIndex, int rowIndex) { }
    }
}
