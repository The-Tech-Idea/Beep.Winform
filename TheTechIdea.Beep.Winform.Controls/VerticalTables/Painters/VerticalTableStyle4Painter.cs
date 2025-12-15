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
    /// Style 4: Dark theme with gradient headers and vibrant accent colors.
    /// Modern dark UI with colored dots/indicators for feature availability.
    /// </summary>
    public class VerticalTableStyle4Painter : IVerticalTablePainter
    {
        private const int CornerRadius = 12;
        private int _featureLabelWidth = 220;

        // Gradient color pairs for column headers
        private readonly (Color Start, Color End)[] _gradients = new (Color, Color)[]
        {
            (Color.FromArgb(99, 102, 241), Color.FromArgb(139, 92, 246)),    // Indigo to Violet
            (Color.FromArgb(236, 72, 153), Color.FromArgb(244, 114, 182)),   // Pink
            (Color.FromArgb(245, 158, 11), Color.FromArgb(251, 191, 36)),    // Amber
            (Color.FromArgb(14, 165, 233), Color.FromArgb(56, 189, 248)),    // Sky
            (Color.FromArgb(34, 197, 94), Color.FromArgb(74, 222, 128)),     // Green
            (Color.FromArgb(239, 68, 68), Color.FromArgb(248, 113, 113)),    // Red
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

        /// <inheritdoc/>
        public void Paint(Graphics g, Rectangle bounds, BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, object owner)
        {
            if (g == null || layout == null || columns == null) return;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int padding = 8;
            int headerHeight = layout.Columns.Count > 0 ? layout.Columns[0].HeaderBounds.Height : 100;
            int rowHeight = layout.Columns.Count > 0 && layout.Columns[0].Cells.Count > 0
                ? layout.Columns[0].Cells[0].Bounds.Height : 50;

            // Dark background
            using (var backBrush = new SolidBrush(Color.FromArgb(15, 23, 42)))
            {
                g.FillRectangle(backBrush, bounds);
            }

            // Draw feature labels column
            DrawFeatureLabelColumn(g, columns, padding, headerHeight, rowHeight, layout);

            // Draw main table area with rounded corners
            if (layout.Columns.Count > 0)
            {
                var tableRect = new Rectangle(
                    layout.Columns[0].ColumnBounds.Left - 1,
                    padding,
                    layout.Columns[layout.Columns.Count - 1].ColumnBounds.Right - layout.Columns[0].ColumnBounds.Left + 2,
                    layout.Columns[0].ColumnBounds.Height
                );

                using (var path = CreateRoundedRectPath(tableRect, CornerRadius))
                using (var brush = new SolidBrush(Color.FromArgb(30, 41, 59)))
                {
                    g.FillPath(brush, path);
                }
            }

            // Draw columns
            for (int i = 0; i < layout.Columns.Count; i++)
            {
                var col = layout.Columns[i];
                var column = col.Column;
                if (column == null) continue;

                bool isColumnHovered = (layout.HoverColumnIndex == col.ColumnIndex);
                bool isColumnSelected = (layout.SelectedColumnIndex == col.ColumnIndex);
                var gradient = _gradients[i % _gradients.Length];

                DrawColumnHeader(g, col, column, gradient, isColumnHovered, isColumnSelected, i == 0, i == layout.Columns.Count - 1);

                foreach (var cell in col.Cells)
                {
                    bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                    bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                    bool isColHovered = layout.IsColumnHovered(col.ColumnIndex);
                    bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                    bool isRowSelected = layout.IsRowSelected(cell.RowIndex);
                    bool isColSelected = layout.IsColumnSelected(col.ColumnIndex);

                    DrawCell(g, cell, layout, isCellHovered, isRowHovered, isColHovered, isCellSelected, isRowSelected, isColSelected, gradient.Start);
                }
            }
        }

        private void DrawFeatureLabelColumn(Graphics g, BindingList<SimpleItem> columns, int padding, int headerHeight, int rowHeight, VerticalTableLayoutHelper layout)
        {
            // Header area for feature column
            var headerRect = new Rectangle(padding, padding, _featureLabelWidth - padding, headerHeight);

            using (var path = CreateLeftRoundedRectPath(headerRect, CornerRadius))
            using (var brush = new SolidBrush(Color.FromArgb(30, 41, 59)))
            {
                g.FillPath(brush, path);
            }

            using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                var textRect = new Rectangle(headerRect.Left + 20, headerRect.Top, headerRect.Width - 40, headerRect.Height);
                g.DrawString("PRICING TABLE", font, brush, textRect, sf);
            }

            // Feature rows
            if (columns.Count > 0 && columns[0].Children != null)
            {
                int y = padding + headerHeight;
                for (int i = 0; i < columns[0].Children.Count; i++)
                {
                    var item = columns[0].Children[i];
                    var rowRect = new Rectangle(padding, y, _featureLabelWidth - padding, rowHeight);

                    // Row background
                    Color rowBg = (i % 2 == 0) ? Color.FromArgb(30, 41, 59) : Color.FromArgb(51, 65, 85);
                    
                    bool isRowHovered = (layout.HoverRowIndex == i);
                    if (isRowHovered)
                        rowBg = Color.FromArgb(71, 85, 105);

                    using (var brush = new SolidBrush(rowBg))
                    {
                        g.FillRectangle(brush, rowRect);
                    }

                    // Row number badge
                    int badgeSize = 24;
                    int badgeX = rowRect.Left + 12;
                    int badgeY = rowRect.Top + (rowRect.Height - badgeSize) / 2;

                    using (var path = CreateRoundedRectPath(new Rectangle(badgeX, badgeY, badgeSize, badgeSize), 6))
                    {
                        var gradient = _gradients[i % _gradients.Length];
                        using (var brush = new LinearGradientBrush(
                            new Rectangle(badgeX, badgeY, badgeSize, badgeSize),
                            gradient.Start, gradient.End, 45f))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
                    using (var brush = new SolidBrush(Color.White))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString((i + 1).ToString("00"), font, brush, new Rectangle(badgeX, badgeY, badgeSize, badgeSize), sf);
                    }

                    // Feature text
                    var textRect = new Rectangle(badgeX + badgeSize + 12, rowRect.Top, rowRect.Width - badgeSize - 36, rowRect.Height);
                    using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
                    using (var brush = new SolidBrush(Color.FromArgb(203, 213, 225)))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                        g.DrawString(item.Text ?? item.Name ?? $"Feature {i + 1}", font, brush, textRect, sf);
                    }

                    y += rowHeight;
                }
            }
        }

        private void DrawColumnHeader(Graphics g, VerticalColumnLayout col, SimpleItem column, (Color Start, Color End) gradient, bool isHovered, bool isSelected, bool isFirst, bool isLast)
        {
            var rect = col.HeaderBounds;

            // Create path based on position
            GraphicsPath? path;
            if (isFirst && isLast)
                path = CreateRoundedRectPath(rect, CornerRadius);
            else if (isFirst)
                path = CreateLeftRoundedRectPath(rect, CornerRadius);
            else if (isLast)
                path = CreateRightRoundedRectPath(rect, CornerRadius);
            else
                path = new GraphicsPath();

            if (path.PointCount == 0)
            {
                path.AddRectangle(rect);
            }

            // Gradient fill
            using (path)
            {
                using (var brush = new LinearGradientBrush(rect, gradient.Start, gradient.End, 90f))
                {
                    g.FillPath(brush, path);
                }

                // Selection/hover overlay
                if (isSelected)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                    {
                        g.FillPath(brush, path);
                    }
                    using (var pen = new Pen(Color.White, 3))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(20, 255, 255, 255)))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }

            int yOffset = rect.Top + 15;

            // Plan name
            using (var font = new Font("Segoe UI", 13, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                g.DrawString(column.Text ?? column.Name ?? "", font, brush, new Rectangle(rect.Left, yOffset, rect.Width, 26), sf);
            }
            yOffset += 30;

            // Price
            string? priceValue = column.Value?.ToString();
            if (!string.IsNullOrEmpty(priceValue))
            {
                using (var font = new Font("Segoe UI", 28, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                    g.DrawString(priceValue, font, brush, new Rectangle(rect.Left, yOffset, rect.Width, 38), sf);
                }
                yOffset += 38;
            }

            // Description
            if (!string.IsNullOrEmpty(column.Description))
            {
                using (var font = new Font("Segoe UI", 8, FontStyle.Regular))
                using (var brush = new SolidBrush(Color.FromArgb(180, 255, 255, 255)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                    g.DrawString(column.Description.ToUpper(), font, brush, new Rectangle(rect.Left, yOffset, rect.Width, 16), sf);
                }
            }
        }

        private void DrawCell(Graphics g, VerticalCellLayout cell, VerticalTableLayoutHelper layout,
            bool isCellHovered, bool isRowHovered, bool isColHovered,
            bool isCellSelected, bool isRowSelected, bool isColSelected,
            Color accentColor)
        {
            var rect = cell.Bounds;
            var item = cell.Item;

            // Background with row/column selection highlighting
            Color bgColor = (cell.RowIndex % 2 == 0) ? Color.FromArgb(30, 41, 59) : Color.FromArgb(51, 65, 85);
            
            if (isCellSelected)
                bgColor = Color.FromArgb(100, 116, 139); // Brightest for exact cell
            else if (isRowSelected || isColSelected)
                bgColor = (isRowSelected && isColSelected) ? Color.FromArgb(85, 100, 120) : Color.FromArgb(71, 85, 105);
            else if (isCellHovered)
                bgColor = Color.FromArgb(65, 75, 95);
            else if (isRowHovered || isColHovered)
                bgColor = Color.FromArgb(55, 65, 81);

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }

            if (item == null) return;

            // Content - show text or indicator
            string? text = item.Text ?? item.Name;

            if (!string.IsNullOrEmpty(text) && text != "-" && text.ToLower() != "x")
            {
                // Show text value
                using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(text, font, brush, rect, sf);
                }
            }
            else
            {
                // Show indicator dot
                int dotSize = 14;
                int dotX = rect.Left + (rect.Width - dotSize) / 2;
                int dotY = rect.Top + (rect.Height - dotSize) / 2;

                Color dotColor = item.IsEnabled ? accentColor : Color.FromArgb(239, 68, 68);

                using (var brush = new SolidBrush(dotColor))
                {
                    g.FillEllipse(brush, dotX, dotY, dotSize, dotSize);
                }

                if (item.IsEnabled)
                {
                    // Checkmark inside dot
                    using (var pen = new Pen(Color.White, 2f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        g.DrawLine(pen, dotX + 3, dotY + 7, dotX + 6, dotY + 10);
                        g.DrawLine(pen, dotX + 6, dotY + 10, dotX + 11, dotY + 4);
                    }
                }
                else
                {
                    // X inside dot
                    using (var pen = new Pen(Color.White, 2f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        g.DrawLine(pen, dotX + 4, dotY + 4, dotX + dotSize - 4, dotY + dotSize - 4);
                        g.DrawLine(pen, dotX + dotSize - 4, dotY + 4, dotX + 4, dotY + dotSize - 4);
                    }
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

        private GraphicsPath CreateLeftRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            path.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private GraphicsPath CreateRightRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            var arc = new Rectangle(rect.Right - diameter, rect.Top, diameter, diameter);

            path.AddLine(rect.Left, rect.Top, rect.Right - radius, rect.Top);
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top);

            path.CloseFigure();
            return path;
        }

        /// <inheritdoc/>
        public void OnCellSelected(VerticalTableLayoutHelper layout, SimpleItem? item, int columnIndex, int rowIndex) { }

        /// <inheritdoc/>
        public void OnCellHoverChanged(VerticalTableLayoutHelper layout, int columnIndex, int rowIndex) { }
    }
}
