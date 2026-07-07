using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using System.ComponentModel;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Painters
{
    /// <summary>
    /// Style 11 — PricingCard: Card-based pricing-table layout with shadows,
    /// equal-height columns, and a gradient "Featured" accent column.
    /// Inspired by Stripe and GitHub Primer pricing patterns.
    /// </summary>
    public class VerticalTableStyle11Painter : IVerticalTablePainter
    {
        private const int CornerRadius = 16;
        private const int ShadowOffset = 6;
        private const int HoverElevation = 10;
        private const int SelectedElevation = 16;

        public void CalculateLayout(BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, int headerHeight, int rowHeight, int columnWidth, int padding, bool showImage)
        {
            if (layout == null) return;
            var layoutColumns = new List<VerticalColumnLayout>();
            if (columns == null || columns.Count == 0)
            {
                layout.SetColumns(layoutColumns);
                return;
            }

            int x = padding + ShadowOffset;
            int clientHeight = layout.Owner.ClientSize.Height;

            for (int colIdx = 0; colIdx < columns.Count; colIdx++)
            {
                var column = columns[colIdx];
                if (column == null || !column.IsVisible) continue;

                // Calculate total column height for equal-height card layout
                int maxRows = 0;
                foreach (var c in columns)
                    if (c?.Children != null)
                        maxRows = Math.Max(maxRows, c.Children.Count);

                int totalColumnHeight = Math.Min(clientHeight - padding * 2 - ShadowOffset * 2,
                    headerHeight + maxRows * rowHeight + padding);

                var colLayout = new VerticalColumnLayout
                {
                    Column = column,
                    ColumnIndex = colIdx,
                    HeaderBounds = new Rectangle(x, padding + ShadowOffset, columnWidth, headerHeight),
                    ColumnBounds = new Rectangle(x, padding + ShadowOffset, columnWidth, totalColumnHeight)
                };

                int y = padding + ShadowOffset + headerHeight;
                if (column.Children != null)
                {
                    for (int rowIdx = 0; rowIdx < maxRows; rowIdx++)
                    {
                        // Pad shorter columns with empty cells
                        SimpleItem? rowItem = rowIdx < column.Children.Count ? column.Children[rowIdx] : null;
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
                x += columnWidth + padding + ShadowOffset;
            }

            layout.SetColumns(layoutColumns);
        }

        public void Paint(Graphics g, Rectangle bounds, BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, object owner)
        {
            if (g == null || layout == null || columns == null) return;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Get theme
            IBeepTheme? theme = null;
            bool useThemeColors = false;
            BeepControlStyle controlStyle = BeepControlStyle.Material3;
            int highlightedCol = -1;

            if (owner is BeepVerticalTable table)
            {
                theme = table._currentTheme ?? (table.UseThemeColors ? BeepThemesManager.CurrentTheme : null);
                useThemeColors = table.UseThemeColors;
                controlStyle = table.ControlStyle;
                highlightedCol = table.HighlightedColumnIndex;
            }

            // Background
            Color tableBg = VerticalTableThemeHelpers.GetTableBackgroundColor(theme, useThemeColors);
            using (var backBrush = new SolidBrush(tableBg))
                g.FillRectangle(backBrush, bounds);

            // Draw each column as a raised card
            foreach (var col in layout.Columns)
            {
                var column = col.Column;
                if (column == null) continue;

                bool isColumnHovered = layout.HoverColumnIndex == col.ColumnIndex;
                bool isColumnSelected = layout.SelectedColumnIndex == col.ColumnIndex;
                bool isFeatured = col.ColumnIndex == highlightedCol || column.IsSelected;

                DrawPricingCard(g, col, column, isColumnHovered, isColumnSelected, isFeatured, highlightedCol, layout, theme, useThemeColors, controlStyle);
            }
        }

        private void DrawPricingCard(Graphics g, VerticalColumnLayout col, SimpleItem column,
            bool isHovered, bool isSelected, bool isFeatured, int highlightedCol,
            VerticalTableLayoutHelper layout, IBeepTheme? theme, bool useThemeColors, BeepControlStyle controlStyle)
        {
            var cardRect = col.ColumnBounds;

            int elevation = isFeatured ? SelectedElevation : (isHovered ? HoverElevation : ShadowOffset);

            // Draw shadow
            Color shadowColor = VerticalTableThemeHelpers.GetShadowColor(theme, useThemeColors, elevation);
            DrawCardShadow(g, cardRect, elevation, isFeatured, shadowColor);

            // Card background
            Color cardBg = VerticalTableThemeHelpers.GetCellBackgroundColor(theme, useThemeColors, isHovered, isSelected);
            Color borderColor = VerticalTableThemeHelpers.GetBorderColor(theme, useThemeColors, isSelected, isFeatured);
            int borderWidth = isFeatured ? 3 : 1;

            using (var path = CreateRoundedRectPath(cardRect, CornerRadius))
            {
                using (var brush = new SolidBrush(cardBg))
                    g.FillPath(brush, path);
                using (var pen = new Pen(borderColor, borderWidth))
                    g.DrawPath(pen, path);
            }

            // Featured gradient accent at top
            if (isFeatured)
            {
                Color accentColor = theme != null && useThemeColors && theme.AccentColor != Color.Empty
                    ? theme.AccentColor : Color.FromArgb(99, 102, 241);
                DrawFeaturedAccent(g, cardRect, accentColor);
            }

            // Draw header
            DrawCardHeader(g, col, column, isFeatured, theme, useThemeColors, controlStyle);

            // Draw cells
            foreach (var cell in col.Cells)
            {
                bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                bool isColHovered = layout.IsColumnHovered(col.ColumnIndex);
                bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                bool isRowSelected = layout.IsRowSelected(cell.RowIndex);
                bool isColSelected = layout.IsColumnSelected(col.ColumnIndex);

                DrawFeatureCell(g, cell, isCellHovered || isRowHovered || isColHovered,
                    isCellSelected || isRowSelected || isColSelected,
                    isFeatured, theme, useThemeColors, controlStyle);
            }
        }

        private void DrawFeaturedAccent(Graphics g, Rectangle cardRect, Color accentColor)
        {
            int accentHeight = 6;
            var accentRect = new Rectangle(cardRect.X, cardRect.Y, cardRect.Width, CornerRadius);
            using (var path = CreateTopRoundedRectPath(accentRect, CornerRadius))
            using (var brush = new LinearGradientBrush(accentRect, accentColor, Color.FromArgb(180, accentColor), 90f))
                g.FillPath(brush, path);
        }

        private void DrawCardHeader(Graphics g, VerticalColumnLayout col, SimpleItem column, bool isFeatured,
            IBeepTheme? theme, bool useThemeColors, BeepControlStyle controlStyle)
        {
            var rect = col.HeaderBounds;
            int padding = 16;
            int yOffset = rect.Top + 20;

            // Featured badge
            if (isFeatured)
            {
                DrawFeaturedBadge(g, col, "Most Popular", theme, useThemeColors, controlStyle);
                yOffset += 28;
            }

            // Plan name
            Color textColor = VerticalTableThemeHelpers.GetHeaderTextColor(theme, useThemeColors, isFeatured, isFeatured);
            using (var titleFont = VerticalTableFontHelpers.GetHeaderFont(controlStyle, isFeatured))
            {
                var titleRect = new Rectangle(rect.Left + padding, yOffset, rect.Width - padding * 2, 28);
                using (var brush = new SolidBrush(textColor))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                    g.DrawString(column.Text ?? column.Name ?? "", titleFont, brush, titleRect, sf);
                }
                yOffset += 32;
            }

            // Description
            if (!string.IsNullOrEmpty(column.Description))
            {
                using (var descFont = VerticalTableFontHelpers.GetSubtextFont(controlStyle))
                {
                    var descRect = new Rectangle(rect.Left + padding, yOffset, rect.Width - padding * 2, 20);
                    Color descColor = isFeatured ? Color.FromArgb(220, 255, 255, 255) : Color.FromArgb(140, 150, 170);
                    using (var brush = new SolidBrush(descColor))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                        g.DrawString(column.Description, descFont, brush, descRect, sf);
                    }
                    yOffset += 22;
                }
            }

            // Price
            string? priceValue = column.Value?.ToString();
            if (!string.IsNullOrEmpty(priceValue))
            {
                using (var priceFont = VerticalTableFontHelpers.GetPriceFont(controlStyle, isFeatured))
                {
                    var priceRect = new Rectangle(rect.Left + padding, yOffset, rect.Width - padding * 2, 48);
                    using (var brush = new SolidBrush(textColor))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                        g.DrawString(priceValue, priceFont, brush, priceRect, sf);
                    }
                }
            }

            // Separator line below header
            int sepY = rect.Bottom;
            using (var pen = new Pen(Color.FromArgb(60, 200, 200, 200), 1))
                g.DrawLine(pen, rect.Left + padding, sepY, rect.Right - padding, sepY);
        }

        private void DrawFeaturedBadge(Graphics g, VerticalColumnLayout col, string badgeText,
            IBeepTheme? theme, bool useThemeColors, BeepControlStyle controlStyle)
        {
            int badgeHeight = 26;
            using (var badgeFont = VerticalTableFontHelpers.GetBadgeFont(controlStyle))
            {
                var badgeWidth = (int)g.MeasureString(badgeText, badgeFont).Width + 24;
                var badgeRect = new Rectangle(
                    col.HeaderBounds.Left + (col.HeaderBounds.Width - badgeWidth) / 2,
                    col.HeaderBounds.Top - badgeHeight / 2,
                    badgeWidth, badgeHeight);

                Color accentColor = theme != null && useThemeColors && theme.AccentColor != Color.Empty
                    ? theme.AccentColor : Color.FromArgb(99, 102, 241);

                using (var path = CreateRoundedRectPath(badgeRect, badgeHeight / 2))
                using (var brush = new SolidBrush(accentColor))
                    g.FillPath(brush, path);

                using (var brush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(badgeText, badgeFont, brush, badgeRect, sf);
                }
            }
        }

        private void DrawFeatureCell(Graphics g, VerticalCellLayout cell, bool isHovered, bool isSelected,
            bool isColumnFeatured, IBeepTheme? theme, bool useThemeColors, BeepControlStyle controlStyle)
        {
            var rect = cell.Bounds;
            var item = cell.Item;
            int padding = 16;

            // Background
            Color bgColor = VerticalTableThemeHelpers.GetCellBackgroundColor(theme, useThemeColors, isHovered, isSelected);
            if (!isHovered && !isSelected && cell.RowIndex % 2 == 1)
                bgColor = VerticalTableThemeHelpers.GetCellBackgroundColor(theme, useThemeColors, false, false, true);

            using (var brush = new SolidBrush(bgColor))
                g.FillRectangle(brush, rect);

            if (item == null)
            {
                // Empty cell (shorter column) — subtle gray dash
                int dashWidth = 20;
                int dashY = rect.Top + rect.Height / 2;
                using (var pen = new Pen(Color.FromArgb(200, 220, 230), 1.5f))
                    g.DrawLine(pen, rect.Left + (rect.Width - dashWidth) / 2, dashY, rect.Left + (rect.Width + dashWidth) / 2, dashY);
                return;
            }

            // Separator line
            using (var pen = new Pen(Color.FromArgb(30, 200, 200, 200), 1))
                g.DrawLine(pen, rect.Left + padding, rect.Bottom - 1, rect.Right - padding, rect.Bottom - 1);

            // Feature indicator icon
            int iconSize = 20;
            int iconX = rect.Left + padding;
            int iconY = rect.Top + (rect.Height - iconSize) / 2;

            string? text = item.Text ?? item.Name;
            bool isPositive = item.IsEnabled;
            bool isNegative = !string.IsNullOrEmpty(text) &&
                (text?.ToLower() is "x" or "false" or "no" or "-" or "—" or "n/a");

            Color accentColor = isColumnFeatured
                ? (theme != null && useThemeColors && theme.AccentColor != Color.Empty
                    ? theme.AccentColor : Color.FromArgb(99, 102, 241))
                : Color.FromArgb(16, 185, 129);

            if (isNegative)
            {
                // Red X
                using (var pen = new Pen(Color.FromArgb(239, 68, 68), 2.5f))
                {
                    pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, iconX + 4, iconY + 4, iconX + iconSize - 4, iconY + iconSize - 4);
                    g.DrawLine(pen, iconX + iconSize - 4, iconY + 4, iconX + 4, iconY + iconSize - 4);
                }
            }
            else if (isPositive && (string.IsNullOrEmpty(text) || text == "-" || text == "✓"))
            {
                // Green checkmark
                using (var pen = new Pen(accentColor, 2.5f))
                {
                    pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, iconX + 3, iconY + iconSize / 2, iconX + iconSize / 2 - 1, iconY + iconSize - 4);
                    g.DrawLine(pen, iconX + iconSize / 2 - 1, iconY + iconSize - 4, iconX + iconSize - 2, iconY + 3);
                }
            }
            else
            {
                // Text value
                var textRect = new Rectangle(rect.Left + padding, rect.Top, rect.Width - padding * 2, rect.Height);
                Color textColor = VerticalTableThemeHelpers.GetCellTextColor(theme, useThemeColors, isSelected);
                using (var font = VerticalTableFontHelpers.GetCellFont(controlStyle, isSelected))
                using (var brush = new SolidBrush(textColor))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(text ?? "", font, brush, textRect, sf);
                }
            }
        }

        private void DrawCardShadow(Graphics g, Rectangle rect, int elevation, bool isAccented, Color shadowColor)
        {
            if (elevation <= 0) return;

            for (int i = elevation; i > 0; i -= 2)
            {
                int alpha = (int)(25 * ((float)i / elevation));
                var shadowRect = new Rectangle(rect.X + (elevation - i) / 2 + 2,
                    rect.Y + (elevation - i) / 2 + 4, rect.Width, rect.Height);

                using (var path = CreateRoundedRectPath(shadowRect, CornerRadius + 3))
                using (var brush = new SolidBrush(Color.FromArgb(alpha, shadowColor)))
                    g.FillPath(brush, path);
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

        public void OnCellSelected(VerticalTableLayoutHelper layout, SimpleItem? item, int columnIndex, int rowIndex) { }
        public void OnCellHoverChanged(VerticalTableLayoutHelper layout, int columnIndex, int rowIndex) { }
    }
}
