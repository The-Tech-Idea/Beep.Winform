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
    /// Style 5: Elevated cards with gradient price banner and icons.
    /// Clean white cards with colored header strip, plan icons, and feature list.
    /// </summary>
    public class VerticalTableStyle5Painter : IVerticalTablePainter
    {
        private const int CornerRadius = 20;
        private const int CardGap = 24;

        // Gradient pairs for price banners
        private readonly (Color Start, Color End)[] _gradients = new (Color, Color)[]
        {
            (Color.FromArgb(96, 165, 250), Color.FromArgb(59, 130, 246)),   // Blue
            (Color.FromArgb(167, 139, 250), Color.FromArgb(139, 92, 246)), // Purple
            (Color.FromArgb(251, 146, 60), Color.FromArgb(249, 115, 22)),  // Orange
            (Color.FromArgb(52, 211, 153), Color.FromArgb(16, 185, 129)),  // Emerald
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

            int x = padding;
            int clientHeight = layout.Owner.ClientSize.Height;

            for (int colIdx = 0; colIdx < columns.Count; colIdx++)
            {
                var column = columns[colIdx];
                if (column == null || !column.IsVisible) continue;

                int rowsHeight = (column.Children?.Count ?? 0) * rowHeight;
                int cardHeight = Math.Min(clientHeight - padding * 2, headerHeight + rowsHeight + 20);

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

            // Light gray background
            using (var backBrush = new SolidBrush(Color.FromArgb(241, 245, 249)))
            {
                g.FillRectangle(backBrush, bounds);
            }

            for (int i = 0; i < layout.Columns.Count; i++)
            {
                var col = layout.Columns[i];
                var column = col.Column;
                if (column == null) continue;

                bool isColumnHovered = (layout.HoverColumnIndex == col.ColumnIndex);
                bool isColumnSelected = (layout.SelectedColumnIndex == col.ColumnIndex);
                bool isFeatured = column.IsSelected;
                var gradient = _gradients[i % _gradients.Length];

                DrawCard(g, col, column, gradient, isColumnHovered, isColumnSelected, isFeatured, layout);
            }
        }

        private void DrawCard(Graphics g, VerticalColumnLayout col, SimpleItem column, (Color Start, Color End) gradient, bool isHovered, bool isSelected, bool isFeatured, VerticalTableLayoutHelper layout)
        {
            var cardRect = col.ColumnBounds;

            // Elevation shadow
            DrawShadow(g, cardRect, isHovered || isSelected || isFeatured ? 16 : 8);

            // Card background
            using (var path = CreateRoundedRectPath(cardRect, CornerRadius))
            {
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillPath(brush, path);
                }

                if (isSelected || isFeatured)
                {
                    using (var pen = new Pen(gradient.Start, 3))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    using (var pen = new Pen(Color.FromArgb(100, gradient.Start), 2))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Draw header with icon and gradient price banner
            DrawHeader(g, col, column, gradient, isFeatured);

            // Draw feature rows
            foreach (var cell in col.Cells)
            {
                bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                bool isColHovered = layout.IsColumnHovered(col.ColumnIndex);
                bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                bool isRowSelected = layout.IsRowSelected(cell.RowIndex);
                bool isColSelected = layout.IsColumnSelected(col.ColumnIndex);

                DrawCell(g, cell, isCellHovered, isRowHovered, isColHovered, isCellSelected, isRowSelected, isColSelected, gradient.Start);
            }
        }

        private void DrawShadow(Graphics g, Rectangle rect, int elevation)
        {
            for (int i = elevation; i > 0; i -= 2)
            {
                var shadowRect = new Rectangle(
                    rect.X + (elevation - i) / 3,
                    rect.Y + (elevation - i) / 2 + 2,
                    rect.Width + i / 2,
                    rect.Height + i / 3
                );

                int alpha = (int)(20 * ((float)i / elevation));
                using (var path = CreateRoundedRectPath(shadowRect, CornerRadius + 4))
                using (var brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private void DrawHeader(Graphics g, VerticalColumnLayout col, SimpleItem column, (Color Start, Color End) gradient, bool isFeatured)
        {
            var rect = col.HeaderBounds;
            int padding = 20;
            int yOffset = rect.Top + padding;

            // Plan name
            using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(30, 41, 59)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                g.DrawString(column.Text ?? column.Name ?? "", font, brush, new Rectangle(rect.Left, yOffset, rect.Width, 28), sf);
            }
            yOffset += 32;

            // Description under plan name
            if (!string.IsNullOrEmpty(column.SubText))
            {
                using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
                using (var brush = new SolidBrush(Color.FromArgb(100, 116, 139)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                    g.DrawString(column.SubText, font, brush, new Rectangle(rect.Left + padding, yOffset, rect.Width - padding * 2, 40), sf);
                }
                yOffset += 45;
            }
            else
            {
                yOffset += 10;
            }

            // Gradient price banner
            int bannerHeight = 56;
            int bannerMargin = 16;
            var bannerRect = new Rectangle(
                rect.Left + bannerMargin,
                yOffset,
                rect.Width - bannerMargin * 2,
                bannerHeight
            );

            using (var path = CreateRoundedRectPath(bannerRect, bannerHeight / 2))
            {
                using (var brush = new LinearGradientBrush(bannerRect, gradient.Start, gradient.End, 0f))
                {
                    g.FillPath(brush, path);
                }
            }

            // Price on banner
            string? priceValue = column.Value?.ToString();
            string period = column.Description ?? "";

            if (!string.IsNullOrEmpty(priceValue))
            {
                using (var priceFont = new Font("Segoe UI", 22, FontStyle.Bold))
                using (var periodFont = new Font("Segoe UI", 11, FontStyle.Regular))
                {
                    var priceSize = g.MeasureString(priceValue, priceFont);
                    var periodSize = g.MeasureString(period, periodFont);
                    float totalWidth = priceSize.Width + periodSize.Width + 4;
                    float startX = bannerRect.Left + (bannerRect.Width - totalWidth) / 2;

                    using (var brush = new SolidBrush(Color.White))
                    {
                        g.DrawString(priceValue, priceFont, brush, startX, bannerRect.Top + (bannerHeight - priceSize.Height) / 2);
                        g.DrawString(period, periodFont, brush, startX + priceSize.Width + 4, bannerRect.Top + (bannerHeight - periodSize.Height) / 2 + 4);
                    }
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
            if (item == null) return;

            int padding = 20;

            // Hover/selection background with row/column highlighting
            if (isCellSelected)
            {
                using (var brush = new SolidBrush(Color.FromArgb(50, accentColor)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isRowSelected || isColSelected)
            {
                int alpha = (isRowSelected && isColSelected) ? 35 : 22;
                using (var brush = new SolidBrush(Color.FromArgb(alpha, accentColor)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isCellHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(245, 248, 252)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isRowHovered || isColHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                {
                    g.FillRectangle(brush, rect);
                }
            }

            // Separator line
            using (var pen = new Pen(Color.FromArgb(241, 245, 249), 1))
            {
                g.DrawLine(pen, rect.Left + padding, rect.Bottom - 1, rect.Right - padding, rect.Bottom - 1);
            }

            // Checkmark or X
            int iconSize = 20;
            int iconX = rect.Left + padding;
            int iconY = rect.Top + (rect.Height - iconSize) / 2;

            if (item.IsEnabled)
            {
                // Filled circle with checkmark
                using (var brush = new SolidBrush(accentColor))
                {
                    g.FillEllipse(brush, iconX, iconY, iconSize, iconSize);
                }
                using (var pen = new Pen(Color.White, 2.5f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, iconX + 5, iconY + 10, iconX + 8, iconY + 14);
                    g.DrawLine(pen, iconX + 8, iconY + 14, iconX + 15, iconY + 6);
                }
            }
            else
            {
                // Empty circle with X
                using (var pen = new Pen(Color.FromArgb(203, 213, 225), 2))
                {
                    g.DrawEllipse(pen, iconX, iconY, iconSize, iconSize);
                }
                using (var pen = new Pen(Color.FromArgb(203, 213, 225), 2f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, iconX + 6, iconY + 6, iconX + iconSize - 6, iconY + iconSize - 6);
                    g.DrawLine(pen, iconX + iconSize - 6, iconY + 6, iconX + 6, iconY + iconSize - 6);
                }
            }

            // Text
            var textRect = new Rectangle(iconX + iconSize + 12, rect.Top, rect.Width - padding * 2 - iconSize - 12, rect.Height);
            Color textColor = item.IsEnabled ? Color.FromArgb(51, 65, 85) : Color.FromArgb(148, 163, 184);

            using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
            using (var brush = new SolidBrush(textColor))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text ?? item.Name ?? "", font, brush, textRect, sf);
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
