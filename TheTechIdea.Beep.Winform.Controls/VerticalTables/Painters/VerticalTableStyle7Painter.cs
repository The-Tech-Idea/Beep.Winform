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
    /// Style 7: Side-by-side comparison cards with difference highlighting.
    /// Shows data records as cards with values displayed and differences marked.
    /// </summary>
    public class VerticalTableStyle7Painter : IVerticalTablePainter
    {
        private const int CornerRadius = 12;
        private const int CardGap = 20;
        private int _labelWidth = 160;

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

            int x = padding + _labelWidth + CardGap;

            for (int colIdx = 0; colIdx < columns.Count; colIdx++)
            {
                var column = columns[colIdx];
                if (column == null || !column.IsVisible) continue;

                int rowsHeight = (column.Children?.Count ?? 0) * rowHeight;
                int cardHeight = headerHeight + rowsHeight + 16;

                var colLayout = new VerticalColumnLayout
                {
                    Column = column,
                    ColumnIndex = colIdx,
                    HeaderBounds = new Rectangle(x, padding, columnWidth, headerHeight),
                    ColumnBounds = new Rectangle(x, padding, columnWidth, cardHeight)
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
                x += columnWidth + CardGap;
            }

            layout.SetColumns(layoutColumns);
        }

        /// <inheritdoc/>
        public void Paint(Graphics g, Rectangle bounds, BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, object owner)
        {
            if (g == null || layout == null || columns == null) return;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int padding = 12;
            int headerHeight = layout.Columns.Count > 0 ? layout.Columns[0].HeaderBounds.Height : 70;
            int rowHeight = layout.Columns.Count > 0 && layout.Columns[0].Cells.Count > 0
                ? layout.Columns[0].Cells[0].Bounds.Height : 48;

            // Soft gradient background
            using (var brush = new LinearGradientBrush(bounds, Color.FromArgb(250, 251, 253), Color.FromArgb(241, 245, 249), 90f))
            {
                g.FillRectangle(brush, bounds);
            }

            // Draw attribute labels column
            DrawAttributeLabels(g, columns, padding, headerHeight, rowHeight, layout);

            // Draw comparison cards
            for (int i = 0; i < layout.Columns.Count; i++)
            {
                var col = layout.Columns[i];
                var column = col.Column;
                if (column == null) continue;

                bool isColumnHovered = (layout.HoverColumnIndex == col.ColumnIndex);
                bool isColumnSelected = (layout.SelectedColumnIndex == col.ColumnIndex);

                DrawComparisonCard(g, col, column, isColumnHovered, isColumnSelected, layout, columns, i);
            }
        }

        private void DrawAttributeLabels(Graphics g, BindingList<SimpleItem> columns, int padding, int headerHeight, int rowHeight, VerticalTableLayoutHelper layout)
        {
            // Header area
            var headerRect = new Rectangle(padding, padding, _labelWidth, headerHeight);

            using (var font = new Font("Segoe UI", 13, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(71, 85, 105)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString("Compare", font, brush, new Rectangle(headerRect.Left + 8, headerRect.Top, headerRect.Width, headerRect.Height), sf);
            }

            // Attribute labels
            if (columns.Count > 0 && columns[0].Children != null)
            {
                int y = padding + headerHeight;
                for (int i = 0; i < columns[0].Children.Count; i++)
                {
                    var item = columns[0].Children[i];
                    var rowRect = new Rectangle(padding, y, _labelWidth, rowHeight);

                    bool isRowHovered = (layout.HoverRowIndex == i);
                    if (isRowHovered)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(30, 59, 130, 246)))
                        {
                            g.FillRectangle(brush, rowRect);
                        }
                    }

                    // Label text
                    using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
                    using (var brush = new SolidBrush(Color.FromArgb(100, 116, 139)))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                        g.DrawString(item.Text ?? item.Name ?? "", font, brush, new Rectangle(rowRect.Left + 8, rowRect.Top, rowRect.Width - 16, rowRect.Height), sf);
                    }

                    y += rowHeight;
                }
            }
        }

        private void DrawComparisonCard(Graphics g, VerticalColumnLayout col, SimpleItem column, bool isHovered, bool isSelected, VerticalTableLayoutHelper layout, BindingList<SimpleItem> allColumns, int colIndex)
        {
            var cardRect = col.ColumnBounds;

            // Shadow
            DrawShadow(g, cardRect, isHovered || isSelected ? 10 : 4);

            // Card background
            using (var path = CreateRoundedRectPath(cardRect, CornerRadius))
            {
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillPath(brush, path);
                }

                // Border on hover/select
                if (isSelected)
                {
                    using (var pen = new Pen(Color.FromArgb(59, 130, 246), 3))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    using (var pen = new Pen(Color.FromArgb(147, 197, 253), 2))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Header with item name/image
            DrawCardHeader(g, col, column);

            // Draw cell values with difference highlighting
            foreach (var cell in col.Cells)
            {
                bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                bool isColHovered = layout.IsColumnHovered(col.ColumnIndex);
                bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                bool isRowSelected = layout.IsRowSelected(cell.RowIndex);
                bool isColSelected = layout.IsColumnSelected(col.ColumnIndex);

                // Find best/different values for highlighting
                bool isBest = false;
                bool isDifferent = false;
                if (allColumns.Count > 1 && cell.Item != null)
                {
                    var values = new List<string>();
                    foreach (var c in allColumns)
                    {
                        if (c.Children != null && cell.RowIndex < c.Children.Count)
                        {
                            var val = c.Children[cell.RowIndex].Value?.ToString() ?? c.Children[cell.RowIndex].Text ?? "";
                            values.Add(val);
                        }
                    }
                    
                    string currentVal = cell.Item.Value?.ToString() ?? cell.Item.Text ?? "";
                    isDifferent = !values.TrueForAll(v => v == values[0]);
                    
                    // Check if this is highest numeric value
                    if (double.TryParse(currentVal, out double numVal))
                    {
                        isBest = true;
                        foreach (var v in values)
                        {
                            if (double.TryParse(v, out double other) && other > numVal)
                            {
                                isBest = false;
                                break;
                            }
                        }
                    }
                }

                DrawCell(g, cell, isCellHovered, isRowHovered, isColHovered, isCellSelected, isRowSelected, isColSelected, isBest, isDifferent);
            }
        }

        private void DrawShadow(Graphics g, Rectangle rect, int elevation)
        {
            for (int i = elevation; i > 0; i -= 2)
            {
                var shadowRect = new Rectangle(
                    rect.X + 1,
                    rect.Y + i / 2 + 1,
                    rect.Width,
                    rect.Height
                );

                int alpha = (int)(20 * ((float)i / elevation));
                using (var path = CreateRoundedRectPath(shadowRect, CornerRadius + 2))
                using (var brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private void DrawCardHeader(Graphics g, VerticalColumnLayout col, SimpleItem column)
        {
            var rect = col.HeaderBounds;
            int padding = 16;

            // Item image if available
            int imgSize = 40;
            int imgX = rect.Left + (rect.Width - imgSize) / 2;
            int imgY = rect.Top + 12;

            if (!string.IsNullOrEmpty(column.ImagePath))
            {
                try
                {
                    var imgRect = new Rectangle(imgX, imgY, imgSize, imgSize);
                    using (var path = CreateRoundedRectPath(imgRect, 8))
                    using (var brush = new SolidBrush(Color.FromArgb(241, 245, 249)))
                    {
                        g.FillPath(brush, path);
                    }
                    // Would draw actual image here with StyledImagePainter
                }
                catch { }
            }

            // Item name
            int textY = rect.Top + imgSize + 20;
            using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(30, 41, 59)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                g.DrawString(column.Text ?? column.Name ?? "", font, brush, new Rectangle(rect.Left, textY, rect.Width, 24), sf);
            }
        }

        private void DrawCell(Graphics g, VerticalCellLayout cell, 
            bool isCellHovered, bool isRowHovered, bool isColHovered,
            bool isCellSelected, bool isRowSelected, bool isColSelected, 
            bool isBest, bool isDifferent)
        {
            var rect = cell.Bounds;
            var item = cell.Item;
            if (item == null) return;

            int padding = 12;

            // Background with row/column selection highlighting
            if (isCellSelected)
            {
                using (var brush = new SolidBrush(Color.FromArgb(199, 224, 254)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isRowSelected || isColSelected)
            {
                int alpha = (isRowSelected && isColSelected) ? 45 : 30;
                using (var brush = new SolidBrush(Color.FromArgb(alpha, 59, 130, 246)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isCellHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(235, 242, 250)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isRowHovered || isColHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(241, 245, 249)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isBest)
            {
                using (var brush = new SolidBrush(Color.FromArgb(240, 253, 244)))
                {
                    g.FillRectangle(brush, rect);
                }
            }

            // Value text
            string text = item.Value?.ToString() ?? item.Text ?? item.Name ?? "-";
            Color textColor = Color.FromArgb(51, 65, 85);
            FontStyle fontStyle = FontStyle.Regular;

            if (isBest && isDifferent)
            {
                textColor = Color.FromArgb(22, 163, 74);
                fontStyle = FontStyle.Bold;
            }
            else if (isDifferent)
            {
                textColor = Color.FromArgb(59, 130, 246);
            }

            using (var font = new Font("Segoe UI", 11, fontStyle))
            using (var brush = new SolidBrush(textColor))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(text, font, brush, rect, sf);
            }

            // Best indicator
            if (isBest && isDifferent)
            {
                int badgeSize = 6;
                using (var brush = new SolidBrush(Color.FromArgb(22, 163, 74)))
                {
                    g.FillEllipse(brush, rect.Right - padding - badgeSize, rect.Top + (rect.Height - badgeSize) / 2, badgeSize, badgeSize);
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
