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
    /// Style 3: Minimal card style with subtle shadows, clean lines, and CTA buttons at bottom.
    /// Light theme with accent color badges and rounded cards.
    /// </summary>
    public class VerticalTableStyle3Painter : IVerticalTablePainter
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
            int clientHeight = layout.Owner.ClientSize.Height;

            for (int colIdx = 0; colIdx < columns.Count; colIdx++)
            {
                var column = columns[colIdx];
                if (column == null || !column.IsVisible) continue;

                int rowsHeight = 0;
                if (column.Children != null)
                    rowsHeight = column.Children.Count * rowHeight;

                int cardHeight = Math.Min(clientHeight - padding * 2, headerHeight + rowsHeight + 60); // 60 for CTA button

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

            // Light background
            using (var backBrush = new SolidBrush(Color.FromArgb(249, 250, 251)))
            {
                g.FillRectangle(backBrush, bounds);
            }

            foreach (var col in layout.Columns)
            {
                var column = col.Column;
                if (column == null) continue;

                bool isColumnHovered = (layout.HoverColumnIndex == col.ColumnIndex);
                bool isColumnSelected = (layout.SelectedColumnIndex == col.ColumnIndex);
                bool isFeatured = column.IsSelected;

                DrawCard(g, col, column, isColumnHovered, isColumnSelected, isFeatured, layout);
            }
        }

        private void DrawCard(Graphics g, VerticalColumnLayout col, SimpleItem column, bool isHovered, bool isSelected, bool isFeatured, VerticalTableLayoutHelper layout)
        {
            var cardRect = col.ColumnBounds;

            // Shadow
            DrawShadow(g, cardRect, isHovered || isSelected || isFeatured);

            // Card background
            using (var path = CreateRoundedRectPath(cardRect, CornerRadius))
            {
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillPath(brush, path);
                }

                // Border
                Color borderColor = isFeatured ? BeepStyling.GetThemeColor("accent") :
                                    isSelected ? BeepStyling.GetThemeColor("accent") :
                                    isHovered ? Color.FromArgb(180, 180, 190) :
                                    Color.FromArgb(229, 231, 235);
                int borderWidth = (isFeatured || isSelected) ? 2 : 1;

                using (var pen = new Pen(borderColor, borderWidth))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Draw header
            DrawHeader(g, col, column, isFeatured);

            // Draw rows
            foreach (var cell in col.Cells)
            {
                bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                bool isColHovered = layout.IsColumnHovered(col.ColumnIndex);
                bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                bool isRowSelected = layout.IsRowSelected(cell.RowIndex);
                bool isColSelected = layout.IsColumnSelected(col.ColumnIndex);

                DrawCell(g, cell, isCellHovered, isRowHovered, isColHovered, isCellSelected, isRowSelected, isColSelected, isFeatured);
            }

            // Draw CTA button at bottom
            DrawCTAButton(g, col, column, isHovered, isFeatured);
        }

        private void DrawShadow(Graphics g, Rectangle rect, bool elevated)
        {
            int shadowLayers = elevated ? 6 : 3;
            int shadowOffset = elevated ? 4 : 2;

            for (int i = shadowLayers; i > 0; i--)
            {
                var shadowRect = new Rectangle(
                    rect.X + shadowOffset - i,
                    rect.Y + shadowOffset,
                    rect.Width + i * 2,
                    rect.Height + i
                );

                int alpha = (int)(15 * ((float)i / shadowLayers));
                using (var path = CreateRoundedRectPath(shadowRect, CornerRadius + 2))
                using (var brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private void DrawHeader(Graphics g, VerticalColumnLayout col, SimpleItem column, bool isFeatured)
        {
            var rect = col.HeaderBounds;
            int padding = 20;
            int yOffset = rect.Top + padding;

            // Featured badge
            if (isFeatured && !string.IsNullOrEmpty(column.SubText))
            {
                DrawBadge(g, rect, column.SubText);
                yOffset += 30;
            }

            // Plan name
            using (var font = new Font("Segoe UI", 18, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(17, 24, 39)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                g.DrawString(column.Text ?? column.Name ?? "", font, brush, new Rectangle(rect.Left, yOffset, rect.Width, 30), sf);
            }
            yOffset += 35;

            // Price with period
            string? priceValue = column.Value?.ToString();
            if (!string.IsNullOrEmpty(priceValue))
            {
                using (var priceFont = new Font("Segoe UI", 36, FontStyle.Bold))
                using (var periodFont = new Font("Segoe UI", 14, FontStyle.Regular))
                {
                    var priceSize = g.MeasureString(priceValue, priceFont);
                    string period = column.Description ?? "";
                    var periodSize = g.MeasureString(period, periodFont);

                    float totalWidth = priceSize.Width + periodSize.Width;
                    float startX = rect.Left + (rect.Width - totalWidth) / 2;

                    using (var brush = new SolidBrush(Color.FromArgb(17, 24, 39)))
                    {
                        g.DrawString(priceValue, priceFont, brush, startX, yOffset);
                    }

                    using (var brush = new SolidBrush(Color.FromArgb(107, 114, 128)))
                    {
                        g.DrawString(period, periodFont, brush, startX + priceSize.Width, yOffset + 20);
                    }
                }
            }
        }

        private void DrawBadge(Graphics g, Rectangle headerRect, string text)
        {
            var accent = BeepStyling.GetThemeColor("accent");
            using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
            {
                var textSize = g.MeasureString(text, font);
                int badgeWidth = (int)textSize.Width + 16;
                int badgeHeight = 22;
                var badgeRect = new Rectangle(
                    headerRect.Left + (headerRect.Width - badgeWidth) / 2,
                    headerRect.Top + 10,
                    badgeWidth,
                    badgeHeight
                );

                using (var path = CreateRoundedRectPath(badgeRect, badgeHeight / 2))
                {
                    using (var brush = new SolidBrush(Color.FromArgb(30, accent)))
                    {
                        g.FillPath(brush, path);
                    }
                }

                using (var brush = new SolidBrush(accent))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(text.ToUpper(), font, brush, badgeRect, sf);
                }
            }
        }

        private void DrawCell(Graphics g, VerticalCellLayout cell, 
            bool isCellHovered, bool isRowHovered, bool isColHovered,
            bool isCellSelected, bool isRowSelected, bool isColSelected, 
            bool isFeatured)
        {
            var rect = cell.Bounds;
            var item = cell.Item;
            if (item == null) return;

            int padding = 16;
            var accent = BeepStyling.GetThemeColor("accent");

            // Hover/selection background with row/column highlighting
            if (isCellSelected)
            {
                using (var brush = new SolidBrush(Color.FromArgb(50, accent)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isRowSelected || isColSelected)
            {
                int alpha = (isRowSelected && isColSelected) ? 35 : 22;
                using (var brush = new SolidBrush(Color.FromArgb(alpha, accent)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isCellHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(245, 247, 250)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isRowHovered || isColHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(249, 250, 251)))
                {
                    g.FillRectangle(brush, rect);
                }
            }

            // Icon
            int iconSize = 18;
            int iconX = rect.Left + padding;
            int iconY = rect.Top + (rect.Height - iconSize) / 2;

            Color checkColor = isFeatured ? BeepStyling.GetThemeColor("accent") : Color.FromArgb(34, 197, 94);

            if (item.IsEnabled)
            {
                // Checkmark
                using (var pen = new Pen(checkColor, 2.5f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, iconX + 2, iconY + iconSize / 2, iconX + 6, iconY + iconSize / 2 + 4);
                    g.DrawLine(pen, iconX + 6, iconY + iconSize / 2 + 4, iconX + iconSize - 2, iconY + 4);
                }
            }
            else
            {
                // X mark or dash
                using (var pen = new Pen(Color.FromArgb(239, 68, 68), 2f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, iconX + 4, iconY + 4, iconX + iconSize - 4, iconY + iconSize - 4);
                    g.DrawLine(pen, iconX + iconSize - 4, iconY + 4, iconX + 4, iconY + iconSize - 4);
                }
            }

            // Text
            var textRect = new Rectangle(iconX + iconSize + 10, rect.Top, rect.Width - padding * 2 - iconSize - 10, rect.Height);
            using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
            {
                Color textColor = item.IsEnabled ? Color.FromArgb(55, 65, 81) : Color.FromArgb(156, 163, 175);
                using (var brush = new SolidBrush(textColor))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                    g.DrawString(item.Text ?? item.Name ?? "", font, brush, textRect, sf);
                }
            }
        }

        private void DrawCTAButton(Graphics g, VerticalColumnLayout col, SimpleItem column, bool isHovered, bool isFeatured)
        {
            var cardRect = col.ColumnBounds;
            int buttonHeight = 44;
            int buttonMargin = 16;
            int buttonWidth = cardRect.Width - buttonMargin * 2;

            var buttonRect = new Rectangle(
                cardRect.Left + buttonMargin,
                cardRect.Bottom - buttonHeight - buttonMargin,
                buttonWidth,
                buttonHeight
            );

            var accent = BeepStyling.GetThemeColor("accent");
            Color bgColor, textColor, borderColor;

            if (isFeatured)
            {
                bgColor = accent;
                textColor = Color.White;
                borderColor = accent;
            }
            else
            {
                bgColor = Color.White;
                textColor = accent;
                borderColor = accent;
            }

            using (var path = CreateRoundedRectPath(buttonRect, buttonHeight / 2))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(borderColor, 2))
                {
                    g.DrawPath(pen, path);
                }
            }

            string buttonText = column.Tag?.ToString() ?? "Get Started";
            using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
            using (var brush = new SolidBrush(textColor))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(buttonText, font, brush, buttonRect, sf);
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
