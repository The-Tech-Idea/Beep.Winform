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
    /// Style 8: Metric comparison with progress bars and visual indicators.
    /// Shows numeric values as horizontal bars for easy visual comparison.
    /// </summary>
    public class VerticalTableStyle8Painter : IVerticalTablePainter
    {
        private const int CornerRadius = 8;
        private int _labelWidth = 180;

        // Colors for different columns
        private readonly Color[] _columnColors = new Color[]
        {
            Color.FromArgb(59, 130, 246),   // Blue
            Color.FromArgb(16, 185, 129),   // Emerald
            Color.FromArgb(245, 158, 11),   // Amber
            Color.FromArgb(239, 68, 68),    // Red
            Color.FromArgb(139, 92, 246),   // Violet
            Color.FromArgb(236, 72, 153),   // Pink
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
            int headerHeight = layout.Columns.Count > 0 ? layout.Columns[0].HeaderBounds.Height : 60;
            int rowHeight = layout.Columns.Count > 0 && layout.Columns[0].Cells.Count > 0
                ? layout.Columns[0].Cells[0].Bounds.Height : 56;

            // White background
            using (var brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, bounds);
            }

            // Draw metric labels
            DrawMetricLabels(g, columns, padding, headerHeight, rowHeight, layout);

            // Draw column headers with color indicators
            for (int i = 0; i < layout.Columns.Count; i++)
            {
                var col = layout.Columns[i];
                var column = col.Column;
                if (column == null) continue;

                bool isColumnHovered = (layout.HoverColumnIndex == col.ColumnIndex);
                bool isColumnSelected = (layout.SelectedColumnIndex == col.ColumnIndex);
                Color columnColor = _columnColors[i % _columnColors.Length];

                DrawColumnHeader(g, col, column, columnColor, isColumnHovered, isColumnSelected);

                // Draw metric bars for each row
                foreach (var cell in col.Cells)
                {
                    bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                    bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                    bool isColHovered = layout.IsColumnHovered(col.ColumnIndex);
                    bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                    bool isRowSelected = layout.IsRowSelected(cell.RowIndex);
                    bool isColSelected = layout.IsColumnSelected(col.ColumnIndex);

                    // Find max value for this row across all columns
                    double maxValue = GetMaxValueForRow(columns, cell.RowIndex);

                    DrawMetricBar(g, cell, columnColor, isCellHovered, isRowHovered, isColHovered, isCellSelected, isRowSelected, isColSelected, maxValue);
                }
            }
        }

        private double GetMaxValueForRow(BindingList<SimpleItem> columns, int rowIndex)
        {
            double maxVal = 0;
            foreach (var col in columns)
            {
                if (col.Children != null && rowIndex < col.Children.Count)
                {
                    var item = col.Children[rowIndex];
                    if (item?.Value != null && double.TryParse(item.Value.ToString(), out double val))
                    {
                        maxVal = Math.Max(maxVal, val);
                    }
                }
            }
            return maxVal > 0 ? maxVal : 100;
        }

        private void DrawMetricLabels(Graphics g, BindingList<SimpleItem> columns, int padding, int headerHeight, int rowHeight, VerticalTableLayoutHelper layout)
        {
            // Header
            var headerRect = new Rectangle(padding, padding, _labelWidth, headerHeight);
            
            using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(30, 41, 59)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString("Metrics", font, brush, new Rectangle(headerRect.Left + 12, headerRect.Top, headerRect.Width - 24, headerRect.Height), sf);
            }

            // Separator
            using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
            {
                g.DrawLine(pen, headerRect.Left, headerRect.Bottom, headerRect.Right, headerRect.Bottom);
            }

            // Metric labels
            if (columns.Count > 0 && columns[0].Children != null)
            {
                int y = padding + headerHeight;
                for (int i = 0; i < columns[0].Children.Count; i++)
                {
                    var item = columns[0].Children[i];
                    var rowRect = new Rectangle(padding, y, _labelWidth, rowHeight);

                    bool isRowHovered = (layout.HoverRowIndex == i);

                    // Row background on hover
                    if (isRowHovered)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                        {
                            g.FillRectangle(brush, new Rectangle(0, y, layout.Owner.ClientSize.Width, rowHeight));
                        }
                    }

                    // Alternating subtle stripe
                    if (i % 2 == 1 && !isRowHovered)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(249, 250, 251)))
                        {
                            g.FillRectangle(brush, new Rectangle(0, y, layout.Owner.ClientSize.Width, rowHeight));
                        }
                    }

                    // Label
                    using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
                    using (var brush = new SolidBrush(Color.FromArgb(51, 65, 85)))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                        g.DrawString(item.Text ?? item.Name ?? $"Metric {i + 1}", font, brush, 
                            new Rectangle(rowRect.Left + 12, rowRect.Top, rowRect.Width - 24, rowRect.Height), sf);
                    }

                    y += rowHeight;
                }
            }
        }

        private void DrawColumnHeader(Graphics g, VerticalColumnLayout col, SimpleItem column, Color color, bool isHovered, bool isSelected)
        {
            var rect = col.HeaderBounds;

            // Hover/select background
            if (isSelected)
            {
                using (var brush = new SolidBrush(Color.FromArgb(30, color)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(15, color)))
                {
                    g.FillRectangle(brush, rect);
                }
            }

            // Color indicator bar at top
            var indicatorRect = new Rectangle(rect.Left + 12, rect.Top + 8, rect.Width - 24, 4);
            using (var path = CreateRoundedRectPath(indicatorRect, 2))
            using (var brush = new SolidBrush(color))
            {
                g.FillPath(brush, path);
            }

            // Column name
            using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(30, 41, 59)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(column.Text ?? column.Name ?? "", font, brush, 
                    new Rectangle(rect.Left, rect.Top + 16, rect.Width, rect.Height - 16), sf);
            }

            // Bottom separator
            using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
            {
                g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
            }
        }

        private void DrawMetricBar(Graphics g, VerticalCellLayout cell, Color color, 
            bool isCellHovered, bool isRowHovered, bool isColHovered,
            bool isCellSelected, bool isRowSelected, bool isColSelected, 
            double maxValue)
        {
            var rect = cell.Bounds;
            var item = cell.Item;
            if (item == null) return;

            int padding = 12;
            int barHeight = 20;
            int barY = rect.Top + (rect.Height - barHeight) / 2;

            // Selection/hover highlight with row/column support
            if (isCellSelected)
            {
                using (var brush = new SolidBrush(Color.FromArgb(50, color)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isRowSelected || isColSelected)
            {
                int alpha = (isRowSelected && isColSelected) ? 35 : 22;
                using (var brush = new SolidBrush(Color.FromArgb(alpha, color)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isCellHovered || isRowHovered || isColHovered)
            {
                int alpha = isCellHovered ? 20 : 12;
                using (var brush = new SolidBrush(Color.FromArgb(alpha, color)))
                {
                    g.FillRectangle(brush, rect);
                }
            }

            // Get value
            string valueText = item.Value?.ToString() ?? item.Text ?? "";
            double value = 0;
            bool isNumeric = double.TryParse(valueText, out value);

            if (isNumeric && maxValue > 0)
            {
                // Draw bar background
                var barBgRect = new Rectangle(rect.Left + padding, barY, rect.Width - padding * 2 - 60, barHeight);
                using (var path = CreateRoundedRectPath(barBgRect, barHeight / 2))
                using (var brush = new SolidBrush(Color.FromArgb(241, 245, 249)))
                {
                    g.FillPath(brush, path);
                }

                // Draw filled bar
                double percent = Math.Min(1.0, value / maxValue);
                int filledWidth = (int)(barBgRect.Width * percent);
                if (filledWidth > 0)
                {
                    var barFillRect = new Rectangle(barBgRect.Left, barBgRect.Top, Math.Max(barHeight, filledWidth), barHeight);
                    using (var path = CreateRoundedRectPath(barFillRect, barHeight / 2))
                    {
                        bool anyHover = isCellHovered || isRowHovered || isColHovered;
                        Color fillColor = anyHover ? Color.FromArgb(Math.Min(255, color.R + 20), Math.Min(255, color.G + 20), Math.Min(255, color.B + 20)) : color;
                        using (var brush = new LinearGradientBrush(barFillRect, fillColor, color, 0f))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                }

                // Value text
                using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
                using (var brush = new SolidBrush(color))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                    g.DrawString(valueText, font, brush, new Rectangle(rect.Right - 60, rect.Top, 48, rect.Height), sf);
                }
            }
            else
            {
                // Non-numeric: just show text centered
                using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
                using (var brush = new SolidBrush(Color.FromArgb(71, 85, 105)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(valueText, font, brush, rect, sf);
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
