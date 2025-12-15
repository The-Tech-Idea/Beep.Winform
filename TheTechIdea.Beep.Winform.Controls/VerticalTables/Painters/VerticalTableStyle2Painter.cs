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
    /// Style 2: Grid-style comparison table with feature labels on left, checkmarks/X marks in cells.
    /// Similar to feature comparison matrix with colored column headers.
    /// </summary>
    public class VerticalTableStyle2Painter : IVerticalTablePainter
    {
        private const int CornerRadius = 8;
        private int _featureLabelWidth = 200;

        // Column header colors (gradient pairs)
        private readonly Color[] _headerColors = new Color[]
        {
            Color.FromArgb(79, 70, 229),   // Indigo
            Color.FromArgb(16, 185, 129),  // Emerald
            Color.FromArgb(245, 158, 11),  // Amber
            Color.FromArgb(239, 68, 68),   // Red
            Color.FromArgb(139, 92, 246),  // Violet
            Color.FromArgb(6, 182, 212),   // Cyan
        };

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

            // Start after the feature label column
            int x = padding + _featureLabelWidth + padding;

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

                // Layout rows (children) within this column
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
                ? layout.Columns[0].Cells[0].Bounds.Height : 40;

            // Background
            using (var backBrush = new SolidBrush(Color.FromArgb(245, 247, 250)))
            {
                g.FillRectangle(backBrush, bounds);
            }

            // Draw feature labels column (first column with row names)
            DrawFeatureLabelColumn(g, columns, padding, headerHeight, rowHeight, layout);

            // Draw each column
            for (int i = 0; i < layout.Columns.Count; i++)
            {
                var col = layout.Columns[i];
                var column = col.Column;
                if (column == null) continue;

                bool isColumnHovered = (layout.HoverColumnIndex == col.ColumnIndex);
                bool isColumnSelected = (layout.SelectedColumnIndex == col.ColumnIndex);
                Color headerColor = _headerColors[i % _headerColors.Length];

                DrawColumnHeader(g, col, column, headerColor, isColumnHovered, isColumnSelected);

                // Draw cells
                foreach (var cell in col.Cells)
                {
                    bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                    bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                    bool isColHovered = layout.IsColumnHovered(col.ColumnIndex);
                    bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                    bool isRowSelected = layout.IsRowSelected(cell.RowIndex);
                    bool isColSelected = layout.IsColumnSelected(col.ColumnIndex);

                    DrawCell(g, cell, isCellHovered, isRowHovered, isColHovered, isCellSelected, isRowSelected, isColSelected, headerColor);
                }
            }
        }

        private void DrawFeatureLabelColumn(Graphics g, BindingList<SimpleItem> columns, int padding, int headerHeight, int rowHeight, VerticalTableLayoutHelper layout)
        {
            var labelRect = new Rectangle(padding, padding, _featureLabelWidth, headerHeight);

            // Header for feature column
            using (var path = CreateRoundedRectPath(labelRect, CornerRadius))
            using (var brush = new SolidBrush(Color.FromArgb(30, 41, 59)))
            {
                g.FillPath(brush, path);
            }

            using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("FEATURES", font, brush, labelRect, sf);
            }

            // Draw feature labels from first column's children names
            if (columns.Count > 0 && columns[0].Children != null)
            {
                int y = padding + headerHeight;
                for (int i = 0; i < columns[0].Children.Count; i++)
                {
                    var item = columns[0].Children[i];
                    var rowRect = new Rectangle(padding, y, _featureLabelWidth, rowHeight);

                    // Alternating row background
                    Color rowBg = (i % 2 == 0) ? Color.White : Color.FromArgb(248, 250, 252);

                    // Check if this row is hovered in any column
                    bool isRowHovered = (layout.HoverRowIndex == i);
                    if (isRowHovered)
                        rowBg = Color.FromArgb(240, 245, 255);

                    using (var brush = new SolidBrush(rowBg))
                    {
                        g.FillRectangle(brush, rowRect);
                    }

                    // Draw feature text
                    using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
                    using (var brush = new SolidBrush(Color.FromArgb(71, 85, 105)))
                    {
                        var textRect = new Rectangle(rowRect.Left + 12, rowRect.Top, rowRect.Width - 24, rowRect.Height);
                        var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                        g.DrawString(item.Text ?? item.Name ?? $"Feature {i + 1}", font, brush, textRect, sf);
                    }

                    y += rowHeight;
                }
            }
        }

        private void DrawColumnHeader(Graphics g, VerticalColumnLayout col, SimpleItem column, Color headerColor, bool isHovered, bool isSelected)
        {
            var rect = col.HeaderBounds;

            // Hover/selection effects
            if (isSelected)
            {
                rect.Inflate(2, 2);
            }
            else if (isHovered)
            {
                rect.Inflate(1, 1);
            }

            // Gradient header
            using (var path = CreateTopRoundedRectPath(rect, CornerRadius))
            {
                Color gradientEnd = Color.FromArgb(
                    Math.Min(255, headerColor.R + 30),
                    Math.Min(255, headerColor.G + 30),
                    Math.Min(255, headerColor.B + 30));

                using (var brush = new LinearGradientBrush(rect, headerColor, gradientEnd, 45f))
                {
                    g.FillPath(brush, path);
                }

                if (isSelected)
                {
                    using (var pen = new Pen(Color.White, 3))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            int yOffset = rect.Top + 15;

            // Plan name
            using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                g.DrawString(column.Text ?? column.Name ?? "", font, brush, new Rectangle(rect.Left, yOffset, rect.Width, 24), sf);
            }
            yOffset += 28;

            // Price
            string? priceValue = column.Value?.ToString();
            if (!string.IsNullOrEmpty(priceValue))
            {
                using (var font = new Font("Segoe UI", 22, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                    g.DrawString(priceValue, font, brush, new Rectangle(rect.Left, yOffset, rect.Width, 32), sf);
                }
                yOffset += 30;
            }

            // Period
            if (!string.IsNullOrEmpty(column.Description))
            {
                using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
                using (var brush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                    g.DrawString(column.Description, font, brush, new Rectangle(rect.Left, yOffset, rect.Width, 18), sf);
                }
            }
        }

        private void DrawCell(Graphics g, VerticalCellLayout cell, 
            bool isCellHovered, bool isRowHovered, bool isColHovered,
            bool isCellSelected, bool isRowSelected, bool isColSelected, 
            Color accentColor)
        {
            var rect = cell.Bounds;
            var item = cell.Item;

            // Background with row/column selection highlighting
            Color bgColor = (cell.RowIndex % 2 == 0) ? Color.White : Color.FromArgb(248, 250, 252);
            
            if (isCellSelected)
                bgColor = Color.FromArgb(60, accentColor);
            else if (isRowSelected || isColSelected)
                bgColor = Color.FromArgb((isRowSelected && isColSelected) ? 45 : 30, accentColor);
            else if (isCellHovered)
                bgColor = Color.FromArgb(35, accentColor);
            else if (isRowHovered || isColHovered)
                bgColor = Color.FromArgb((isRowHovered && isColHovered) ? 25 : 15, accentColor);

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Border
            using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
            {
                g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
            }

            // Content - check if it's a boolean indicator or text
            if (item == null) return;

            int iconSize = 20;
            int iconX = rect.Left + (rect.Width - iconSize) / 2;
            int iconY = rect.Top + (rect.Height - iconSize) / 2;

            // Check IsEnabled for checkmark/X, or show text
            if (item.IsEnabled)
            {
                // Draw checkmark
                DrawCheckmark(g, iconX, iconY, iconSize, accentColor);
            }
            else if (!string.IsNullOrEmpty(item.Text) || !string.IsNullOrEmpty(item.Name))
            {
                // If there's text, show it instead of X
                string text = item.Text ?? item.Name ?? "";
                if (text.ToLower() == "x" || text == "-" || text.ToLower() == "no" || text.ToLower() == "false")
                {
                    DrawXMark(g, iconX, iconY, iconSize);
                }
                else
                {
                    // Show the actual text value
                    using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
                    using (var brush = new SolidBrush(Color.FromArgb(71, 85, 105)))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString(text, font, brush, rect, sf);
                    }
                }
            }
            else
            {
                // Draw X mark for disabled items
                DrawXMark(g, iconX, iconY, iconSize);
            }
        }

        private void DrawCheckmark(Graphics g, int x, int y, int size, Color color)
        {
            // Circle background
            using (var brush = new SolidBrush(Color.FromArgb(30, color)))
            {
                g.FillEllipse(brush, x, y, size, size);
            }

            // Checkmark
            using (var pen = new Pen(color, 2.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, x + 5, y + size / 2, x + 8, y + size / 2 + 4);
                g.DrawLine(pen, x + 8, y + size / 2 + 4, x + size - 5, y + 6);
            }
        }

        private void DrawXMark(Graphics g, int x, int y, int size)
        {
            Color xColor = Color.FromArgb(239, 68, 68);

            // Circle background
            using (var brush = new SolidBrush(Color.FromArgb(30, xColor)))
            {
                g.FillEllipse(brush, x, y, size, size);
            }

            // X mark
            using (var pen = new Pen(xColor, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, x + 6, y + 6, x + size - 6, y + size - 6);
                g.DrawLine(pen, x + size - 6, y + 6, x + 6, y + size - 6);
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

        private GraphicsPath CreateTopRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);

            path.CloseFigure();
            return path;
        }

        /// <inheritdoc/>
        public void OnCellSelected(VerticalTableLayoutHelper layout, SimpleItem? item, int columnIndex, int rowIndex) { }

        /// <inheritdoc/>
        public void OnCellHoverChanged(VerticalTableLayoutHelper layout, int columnIndex, int rowIndex) { }
    }
}
