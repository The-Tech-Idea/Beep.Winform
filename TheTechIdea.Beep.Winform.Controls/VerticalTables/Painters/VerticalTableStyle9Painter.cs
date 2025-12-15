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
    /// Style 9: Rating/Score comparison with star ratings and score displays.
    /// Shows values as scores, ratings, or percentages with visual indicators.
    /// </summary>
    public class VerticalTableStyle9Painter : IVerticalTablePainter
    {
        private const int CornerRadius = 16;
        private const int CardGap = 16;

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

            // Gradient background
            using (var brush = new LinearGradientBrush(bounds, Color.FromArgb(248, 250, 252), Color.FromArgb(241, 245, 249), 135f))
            {
                g.FillRectangle(brush, bounds);
            }

            for (int i = 0; i < layout.Columns.Count; i++)
            {
                var col = layout.Columns[i];
                var column = col.Column;
                if (column == null) continue;

                bool isColumnHovered = (layout.HoverColumnIndex == col.ColumnIndex);
                bool isColumnSelected = (layout.SelectedColumnIndex == col.ColumnIndex);
                bool isWinner = column.IsSelected; // Mark winner with IsSelected

                DrawComparisonCard(g, col, column, isColumnHovered, isColumnSelected, isWinner, layout, columns);
            }
        }

        private void DrawComparisonCard(Graphics g, VerticalColumnLayout col, SimpleItem column, bool isHovered, bool isSelected, bool isWinner, VerticalTableLayoutHelper layout, BindingList<SimpleItem> allColumns)
        {
            var cardRect = col.ColumnBounds;

            // Shadow
            DrawShadow(g, cardRect, isHovered || isSelected || isWinner ? 12 : 6);

            // Card background
            Color bgColor = isWinner ? Color.FromArgb(254, 252, 232) : Color.White;
            using (var path = CreateRoundedRectPath(cardRect, CornerRadius))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                Color borderColor = isWinner ? Color.FromArgb(250, 204, 21) :
                                    isSelected ? Color.FromArgb(59, 130, 246) :
                                    isHovered ? Color.FromArgb(203, 213, 225) :
                                    Color.FromArgb(226, 232, 240);
                int borderWidth = (isWinner || isSelected) ? 2 : 1;

                using (var pen = new Pen(borderColor, borderWidth))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Winner badge
            if (isWinner)
            {
                DrawWinnerBadge(g, cardRect);
            }

            // Header
            DrawHeader(g, col, column, isWinner);

            // Attribute rows
            foreach (var cell in col.Cells)
            {
                bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                bool isColHovered = layout.IsColumnHovered(col.ColumnIndex);
                bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                bool isRowSelected = layout.IsRowSelected(cell.RowIndex);
                bool isColSelected = layout.IsColumnSelected(col.ColumnIndex);

                DrawAttributeRow(g, cell, isCellHovered, isRowHovered, isColHovered, isCellSelected, isRowSelected, isColSelected, isWinner, allColumns);
            }
        }

        private void DrawShadow(Graphics g, Rectangle rect, int elevation)
        {
            for (int i = elevation; i > 0; i -= 2)
            {
                var shadowRect = new Rectangle(rect.X, rect.Y + i / 2 + 1, rect.Width, rect.Height);
                int alpha = (int)(15 * ((float)i / elevation));
                using (var path = CreateRoundedRectPath(shadowRect, CornerRadius + 2))
                using (var brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private void DrawWinnerBadge(Graphics g, Rectangle cardRect)
        {
            int badgeWidth = 80;
            int badgeHeight = 24;
            var badgeRect = new Rectangle(cardRect.Left + (cardRect.Width - badgeWidth) / 2, cardRect.Top - badgeHeight / 2, badgeWidth, badgeHeight);

            using (var path = CreateRoundedRectPath(badgeRect, badgeHeight / 2))
            {
                using (var brush = new LinearGradientBrush(badgeRect, Color.FromArgb(250, 204, 21), Color.FromArgb(245, 158, 11), 0f))
                {
                    g.FillPath(brush, path);
                }
            }

            using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(113, 63, 18)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("★ BEST", font, brush, badgeRect, sf);
            }
        }

        private void DrawHeader(Graphics g, VerticalColumnLayout col, SimpleItem column, bool isWinner)
        {
            var rect = col.HeaderBounds;
            int padding = 16;

            // Overall score/rating display
            string? scoreText = column.Value?.ToString();
            if (!string.IsNullOrEmpty(scoreText))
            {
                int scoreSize = 64;
                int scoreX = rect.Left + (rect.Width - scoreSize) / 2;
                int scoreY = rect.Top + 16;

                // Score circle
                Color circleColor = isWinner ? Color.FromArgb(250, 204, 21) : Color.FromArgb(59, 130, 246);
                using (var path = CreateRoundedRectPath(new Rectangle(scoreX, scoreY, scoreSize, scoreSize), scoreSize / 2))
                {
                    using (var brush = new SolidBrush(Color.FromArgb(30, circleColor)))
                    {
                        g.FillPath(brush, path);
                    }
                    using (var pen = new Pen(circleColor, 3))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                using (var font = new Font("Segoe UI", 18, FontStyle.Bold))
                using (var brush = new SolidBrush(circleColor))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(scoreText, font, brush, new Rectangle(scoreX, scoreY, scoreSize, scoreSize), sf);
                }
            }

            // Item name
            int textY = rect.Top + 90;
            using (var font = new Font("Segoe UI", 13, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(30, 41, 59)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                g.DrawString(column.Text ?? column.Name ?? "", font, brush, new Rectangle(rect.Left, textY, rect.Width, 26), sf);
            }

            // Subtitle/description
            if (!string.IsNullOrEmpty(column.Description))
            {
                using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
                using (var brush = new SolidBrush(Color.FromArgb(100, 116, 139)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                    g.DrawString(column.Description, font, brush, new Rectangle(rect.Left + padding, textY + 28, rect.Width - padding * 2, 20), sf);
                }
            }
        }

        private void DrawAttributeRow(Graphics g, VerticalCellLayout cell, 
            bool isCellHovered, bool isRowHovered, bool isColHovered,
            bool isCellSelected, bool isRowSelected, bool isColSelected,
            bool isWinner, BindingList<SimpleItem> allColumns)
        {
            var rect = cell.Bounds;
            var item = cell.Item;
            if (item == null) return;

            int padding = 16;

            // Hover/selection background with row/column support
            if (isCellSelected)
            {
                using (var brush = new SolidBrush(Color.FromArgb(199, 224, 254)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isRowSelected || isColSelected)
            {
                int alpha = (isRowSelected && isColSelected) ? 45 : 28;
                using (var brush = new SolidBrush(Color.FromArgb(alpha, 59, 130, 246)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isCellHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(241, 247, 253)))
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

            // Separator
            using (var pen = new Pen(Color.FromArgb(241, 245, 249), 1))
            {
                g.DrawLine(pen, rect.Left + padding, rect.Top, rect.Right - padding, rect.Top);
            }

            // Attribute name
            using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
            using (var brush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text ?? item.Name ?? "", font, brush, new Rectangle(rect.Left + padding, rect.Top, rect.Width / 2 - padding, rect.Height), sf);
            }

            // Value - check if it's a rating (0-5 or 0-10)
            string? valueText = item.Value?.ToString() ?? "";
            bool isRating = false;
            double ratingValue = 0;

            if (double.TryParse(valueText, out ratingValue) && ratingValue >= 0 && ratingValue <= 10)
            {
                isRating = true;
            }

            if (isRating)
            {
                // Draw star rating or score bar
                DrawRatingStars(g, rect, ratingValue, isWinner);
            }
            else
            {
                // Plain text value
                using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.FromArgb(51, 65, 85)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                    g.DrawString(valueText, font, brush, new Rectangle(rect.Left, rect.Top, rect.Width - padding, rect.Height), sf);
                }
            }
        }

        private void DrawRatingStars(Graphics g, Rectangle rect, double rating, bool isWinner)
        {
            int padding = 16;
            int starSize = 14;
            int starGap = 2;
            int maxStars = 5;

            // Convert 0-10 scale to 0-5 if needed
            double normalizedRating = rating > 5 ? rating / 2 : rating;

            int totalWidth = (starSize + starGap) * maxStars - starGap;
            int startX = rect.Right - padding - totalWidth;
            int startY = rect.Top + (rect.Height - starSize) / 2;

            Color starColor = isWinner ? Color.FromArgb(250, 204, 21) : Color.FromArgb(251, 191, 36);
            Color emptyColor = Color.FromArgb(226, 232, 240);

            for (int i = 0; i < maxStars; i++)
            {
                int x = startX + i * (starSize + starGap);
                Color color = (i < normalizedRating) ? starColor : emptyColor;

                // Draw star (simplified as filled circle)
                using (var brush = new SolidBrush(color))
                {
                    DrawStar(g, x, startY, starSize, brush);
                }
            }

            // Draw numeric value too
            using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(71, 85, 105)))
            {
                g.DrawString(rating.ToString("0.0"), font, brush, startX - 30, startY + 1);
            }
        }

        private void DrawStar(Graphics g, int x, int y, int size, Brush brush)
        {
            // Simplified 5-point star
            var points = new PointF[10];
            double outerRadius = size / 2.0;
            double innerRadius = outerRadius * 0.4;
            double centerX = x + outerRadius;
            double centerY = y + outerRadius;

            for (int i = 0; i < 10; i++)
            {
                double radius = (i % 2 == 0) ? outerRadius : innerRadius;
                double angle = Math.PI / 2 + i * Math.PI / 5;
                points[i] = new PointF(
                    (float)(centerX + radius * Math.Cos(angle)),
                    (float)(centerY - radius * Math.Sin(angle))
                );
            }

            g.FillPolygon(brush, points);
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
