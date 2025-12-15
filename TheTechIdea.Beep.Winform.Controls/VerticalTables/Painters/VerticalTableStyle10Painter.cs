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
    /// Style 10: Specification comparison table with category grouping.
    /// Tech specs style with attribute labels on left and values in columns.
    /// </summary>
    public class VerticalTableStyle10Painter : IVerticalTablePainter
    {
        private const int CornerRadius = 8;
        private int _labelWidth = 200;

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

            int x = padding + _labelWidth;

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
            int headerHeight = layout.Columns.Count > 0 ? layout.Columns[0].HeaderBounds.Height : 80;
            int rowHeight = layout.Columns.Count > 0 && layout.Columns[0].Cells.Count > 0
                ? layout.Columns[0].Cells[0].Bounds.Height : 44;

            // Background
            using (var brush = new SolidBrush(Color.FromArgb(250, 251, 252)))
            {
                g.FillRectangle(brush, bounds);
            }

            // Draw main table container with rounded corners
            int tableWidth = _labelWidth + (layout.Columns.Count * (layout.Columns.Count > 0 ? layout.Columns[0].HeaderBounds.Width : 180));
            int maxRows = 0;
            foreach (var col in layout.Columns)
                maxRows = Math.Max(maxRows, col.Cells.Count);
            int tableHeight = headerHeight + (maxRows * rowHeight);

            var tableRect = new Rectangle(padding, padding, tableWidth, tableHeight);
            using (var path = CreateRoundedRectPath(tableRect, CornerRadius))
            {
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Draw specification labels
            DrawSpecLabels(g, columns, padding, headerHeight, rowHeight, layout);

            // Draw column headers and cells
            for (int i = 0; i < layout.Columns.Count; i++)
            {
                var col = layout.Columns[i];
                var column = col.Column;
                if (column == null) continue;

                bool isColumnHovered = (layout.HoverColumnIndex == col.ColumnIndex);
                bool isColumnSelected = (layout.SelectedColumnIndex == col.ColumnIndex);
                bool isHighlighted = column.IsSelected;

                DrawColumnHeader(g, col, column, isColumnHovered, isColumnSelected, isHighlighted, i == layout.Columns.Count - 1);

                foreach (var cell in col.Cells)
                {
                    bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                    bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                    bool isColHovered = layout.IsColumnHovered(col.ColumnIndex);
                    bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                    bool isRowSelected = layout.IsRowSelected(cell.RowIndex);
                    bool isColSelected = layout.IsColumnSelected(col.ColumnIndex);

                    DrawSpecCell(g, cell, isCellHovered, isRowHovered, isColHovered, isCellSelected, isRowSelected, isColSelected, isHighlighted, layout, columns, i == layout.Columns.Count - 1);
                }
            }
        }

        private void DrawSpecLabels(Graphics g, BindingList<SimpleItem> columns, int padding, int headerHeight, int rowHeight, VerticalTableLayoutHelper layout)
        {
            // Header corner
            var headerRect = new Rectangle(padding, padding, _labelWidth, headerHeight);

            using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
            {
                g.FillRectangle(brush, headerRect);
            }

            using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(71, 85, 105)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString("Specifications", font, brush, new Rectangle(headerRect.Left + 16, headerRect.Top, headerRect.Width - 32, headerRect.Height), sf);
            }

            // Borders
            using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
            {
                g.DrawLine(pen, headerRect.Right, headerRect.Top, headerRect.Right, headerRect.Bottom);
                g.DrawLine(pen, headerRect.Left, headerRect.Bottom, headerRect.Right, headerRect.Bottom);
            }

            // Spec labels
            if (columns.Count > 0 && columns[0].Children != null)
            {
                int y = padding + headerHeight;
                for (int i = 0; i < columns[0].Children.Count; i++)
                {
                    var item = columns[0].Children[i];
                    var rowRect = new Rectangle(padding, y, _labelWidth, rowHeight);

                    bool isRowHovered = (layout.HoverRowIndex == i);

                    // Alternating background
                    Color bgColor = (i % 2 == 0) ? Color.White : Color.FromArgb(249, 250, 251);
                    if (isRowHovered) bgColor = Color.FromArgb(239, 246, 255);

                    using (var brush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(brush, rowRect);
                    }

                    // Border
                    using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    {
                        g.DrawLine(pen, rowRect.Right, rowRect.Top, rowRect.Right, rowRect.Bottom);
                        g.DrawLine(pen, rowRect.Left, rowRect.Bottom, rowRect.Right, rowRect.Bottom);
                    }

                    // Spec name
                    using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
                    using (var brush = new SolidBrush(Color.FromArgb(71, 85, 105)))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                        g.DrawString(item.Text ?? item.Name ?? $"Spec {i + 1}", font, brush, new Rectangle(rowRect.Left + 16, rowRect.Top, rowRect.Width - 32, rowRect.Height), sf);
                    }

                    y += rowHeight;
                }
            }
        }

        private void DrawColumnHeader(Graphics g, VerticalColumnLayout col, SimpleItem column, bool isHovered, bool isSelected, bool isHighlighted, bool isLast)
        {
            var rect = col.HeaderBounds;

            // Background
            Color bgColor = isHighlighted ? Color.FromArgb(239, 246, 255) :
                            isSelected ? Color.FromArgb(219, 234, 254) :
                            isHovered ? Color.FromArgb(248, 250, 252) :
                            Color.FromArgb(248, 250, 252);

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Borders
            using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
            {
                if (!isLast) g.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);
                g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
            }

            // Highlight indicator
            if (isHighlighted)
            {
                using (var pen = new Pen(Color.FromArgb(59, 130, 246), 3))
                {
                    g.DrawLine(pen, rect.Left, rect.Top + 2, rect.Right, rect.Top + 2);
                }
            }

            int yOffset = rect.Top + 12;

            // Product/Item image placeholder
            int imgSize = 36;
            int imgX = rect.Left + (rect.Width - imgSize) / 2;
            using (var brush = new SolidBrush(Color.FromArgb(226, 232, 240)))
            {
                g.FillEllipse(brush, imgX, yOffset, imgSize, imgSize);
            }

            // Name
            yOffset += imgSize + 8;
            using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(30, 41, 59)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                g.DrawString(column.Text ?? column.Name ?? "", font, brush, new Rectangle(rect.Left, yOffset, rect.Width, 22), sf);
            }

            // SubText (model, version, etc.)
            if (!string.IsNullOrEmpty(column.SubText))
            {
                yOffset += 22;
                using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
                using (var brush = new SolidBrush(Color.FromArgb(100, 116, 139)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                    g.DrawString(column.SubText, font, brush, new Rectangle(rect.Left, yOffset, rect.Width, 18), sf);
                }
            }
        }

        private void DrawSpecCell(Graphics g, VerticalCellLayout cell, 
            bool isCellHovered, bool isRowHovered, bool isColHovered,
            bool isCellSelected, bool isRowSelected, bool isColSelected,
            bool isColumnHighlighted, VerticalTableLayoutHelper layout, BindingList<SimpleItem> allColumns, bool isLast)
        {
            var rect = cell.Bounds;
            var item = cell.Item;

            // Background with row/column selection highlighting
            Color bgColor = (cell.RowIndex % 2 == 0) ? Color.White : Color.FromArgb(249, 250, 251);
            if (isColumnHighlighted) bgColor = (cell.RowIndex % 2 == 0) ? Color.FromArgb(239, 246, 255) : Color.FromArgb(224, 237, 255);
            
            if (isCellSelected)
                bgColor = Color.FromArgb(199, 224, 254);
            else if (isRowSelected || isColSelected)
                bgColor = (isRowSelected && isColSelected) ? Color.FromArgb(209, 230, 254) : Color.FromArgb(219, 234, 254);
            else if (isCellHovered)
                bgColor = Color.FromArgb(235, 242, 250);
            else if (isRowHovered || isColHovered)
                bgColor = Color.FromArgb(241, 245, 249);

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Borders
            using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
            {
                if (!isLast) g.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);
                g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
            }

            if (item == null) return;

            // Check if this value is the best in the row
            bool isBest = false;
            if (allColumns.Count > 1 && item.Value != null)
            {
                string currentVal = item.Value.ToString() ?? "";
                if (double.TryParse(currentVal, out double numVal))
                {
                    isBest = true;
                    foreach (var c in allColumns)
                    {
                        if (c.Children != null && cell.RowIndex < c.Children.Count)
                        {
                            var otherVal = c.Children[cell.RowIndex].Value?.ToString() ?? "";
                            if (double.TryParse(otherVal, out double other) && other > numVal)
                            {
                                isBest = false;
                                break;
                            }
                        }
                    }
                }
            }

            // Value
            string valueText = item.Value?.ToString() ?? item.Text ?? "-";
            Color textColor = isBest ? Color.FromArgb(22, 163, 74) : Color.FromArgb(51, 65, 85);
            FontStyle fontStyle = isBest ? FontStyle.Bold : FontStyle.Regular;

            using (var font = new Font("Segoe UI", 10, fontStyle))
            using (var brush = new SolidBrush(textColor))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(valueText, font, brush, rect, sf);
            }

            // Best indicator
            if (isBest)
            {
                int dotSize = 6;
                using (var brush = new SolidBrush(Color.FromArgb(22, 163, 74)))
                {
                    g.FillEllipse(brush, rect.Right - 14, rect.Top + (rect.Height - dotSize) / 2, dotSize, dotSize);
                }
            }
        }

        private GraphicsPath CreateRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <inheritdoc/>
        public void OnCellSelected(VerticalTableLayoutHelper layout, SimpleItem? item, int columnIndex, int rowIndex) { }

        /// <inheritdoc/>
        public void OnCellHoverChanged(VerticalTableLayoutHelper layout, int columnIndex, int rowIndex) { }
    }
}
