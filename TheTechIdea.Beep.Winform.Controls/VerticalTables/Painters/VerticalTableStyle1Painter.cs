using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using System.ComponentModel;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Painters
{
    /// <summary>
    /// Style 1 painter for vertical table: Card-style columns with hover/selection effects.
    /// Similar to modern pricing table layout with elevation and visual feedback.
    /// Enhanced with theme integration.
    /// </summary>
    public class VerticalTableStyle1Painter : IVerticalTablePainter
    {
        private const int CornerRadius = 12;
        private const int ShadowOffset = 4;
        private const int HoverElevation = 8;
        private const int SelectedElevation = 12;

        public void CalculateLayout(BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, int headerHeight, int rowHeight, int columnWidth, int padding, bool showImage)
        {
            if (layout == null) return;
            var layoutColumns = new List<VerticalColumnLayout>();
            if (columns == null || columns.Count == 0)
            {
                layout.SetColumns(layoutColumns);
                return;
            }

            int x = padding + ShadowOffset; // Account for shadow
            int clientHeight = layout.Owner.ClientSize.Height;

            for (int colIdx = 0; colIdx < columns.Count; colIdx++)
            {
                var column = columns[colIdx];
                if (column == null || !column.IsVisible) continue;

                // Calculate total column height based on rows
                int rowsHeight = 0;
                if (column.Children != null)
                    rowsHeight = column.Children.Count * rowHeight;

                int totalColumnHeight = Math.Min(clientHeight - padding * 2 - ShadowOffset * 2, headerHeight + rowsHeight + padding);

                var colLayout = new VerticalColumnLayout
                {
                    Column = column,
                    ColumnIndex = colIdx,
                    HeaderBounds = new Rectangle(x, padding + ShadowOffset, columnWidth, headerHeight),
                    ColumnBounds = new Rectangle(x, padding + ShadowOffset, columnWidth, totalColumnHeight)
                };

                // Layout rows (children) within this column
                int y = padding + ShadowOffset + headerHeight;
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
                x += columnWidth + padding + ShadowOffset;
            }

            layout.SetColumns(layoutColumns);
        }

        public void Paint(Graphics g, Rectangle bounds, BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, object owner)
        {
            if (g == null || layout == null || columns == null) return;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Get theme information from owner
            IBeepTheme? theme = null;
            bool useThemeColors = false;
            BeepControlStyle controlStyle = BeepControlStyle.Material3;

            if (owner is BeepVerticalTable table)
            {
                theme = table._currentTheme ?? (table.UseThemeColors ? BeepThemesManager.CurrentTheme : null);
                useThemeColors = table.UseThemeColors;
                controlStyle = table.ControlStyle;
            }

            // Background using theme helpers
            Color tableBg = VerticalTableThemeHelpers.GetTableBackgroundColor(theme, useThemeColors);
            using (var backBrush = new SolidBrush(tableBg))
            {
                g.FillRectangle(backBrush, bounds);
            }

            // Draw each column as a card
            foreach (var col in layout.Columns)
            {
                var column = col.Column;
                if (column == null) continue;

                // Determine column state - check if ANY part of this column is hovered/selected
                bool isColumnHovered = (layout.HoverColumnIndex == col.ColumnIndex);
                bool isColumnSelected = (layout.SelectedColumnIndex == col.ColumnIndex);
                bool isFeatured = column.IsSelected; // Use IsSelected on column item itself for "featured" state

                DrawColumnCard(g, col, column, isColumnHovered, isColumnSelected, isFeatured, layout, owner);
            }
        }

        private void DrawColumnCard(Graphics g, VerticalColumnLayout col, SimpleItem column, bool isHovered, bool isSelected, bool isFeatured, VerticalTableLayoutHelper layout, object owner)
        {
            var cardRect = col.ColumnBounds;

            // Get theme information from owner
            IBeepTheme? theme = null;
            bool useThemeColors = false;
            BeepControlStyle controlStyle = BeepControlStyle.Material3;

            if (owner is BeepVerticalTable table)
            {
                theme = table._currentTheme ?? (table.UseThemeColors ? BeepThemesManager.CurrentTheme : null);
                useThemeColors = table.UseThemeColors;
                controlStyle = table.ControlStyle;
            }

            // Determine elevation based on state
            int elevation = ShadowOffset;
            if (isSelected || isFeatured) elevation = SelectedElevation;
            else if (isHovered) elevation = HoverElevation;

            // Draw shadow (elevation effect) using theme helpers
            Color shadowColor = VerticalTableThemeHelpers.GetShadowColor(theme, useThemeColors, elevation);
            DrawCardShadow(g, cardRect, elevation, isSelected || isFeatured, shadowColor);

            // Determine card colors using theme helpers
            Color cardBg = VerticalTableThemeHelpers.GetCellBackgroundColor(theme, useThemeColors, isHovered, isSelected);
            Color headerBg = VerticalTableThemeHelpers.GetHeaderBackgroundColor(theme, useThemeColors, isSelected, isFeatured);
            Color borderColor = VerticalTableThemeHelpers.GetBorderColor(theme, useThemeColors, isSelected, isFeatured);
            int borderWidth = VerticalTableStyleHelpers.GetBorderWidth(VerticalTablePainterStyle.Style1, controlStyle, isSelected, isFeatured);

            // Draw card background with rounded corners
            using (var path = CreateRoundedRectPath(cardRect, CornerRadius))
            {
                using (var brush = new SolidBrush(cardBg))
                {
                    g.FillPath(brush, path);
                }
                using (var pen = new Pen(borderColor, borderWidth))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Draw header section
            DrawColumnHeader(g, col, column, isHovered, isSelected, isFeatured, headerBg, theme, useThemeColors, controlStyle);

            // Draw "Featured" or "Most Popular" badge if applicable
            if (isFeatured && !string.IsNullOrEmpty(column.SubText))
            {
                DrawFeaturedBadge(g, col, column.SubText, theme, useThemeColors, controlStyle);
            }

            // Draw rows (cells) within the column
            foreach (var cell in col.Cells)
            {
                var item = cell.Item;
                if (item == null) continue;

                // Use new helper methods for cross-selection detection
                bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                bool isColHovered = layout.IsColumnHovered(col.ColumnIndex);
                bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                bool isRowSelected = layout.IsRowSelected(cell.RowIndex);
                bool isColSelected = layout.IsColumnSelected(col.ColumnIndex);

                DrawCell(g, cell, item, isCellHovered, isRowHovered, isColHovered, isCellSelected, isRowSelected, isColSelected, isSelected || isFeatured, theme, useThemeColors, controlStyle);
            }
        }

        private void DrawCardShadow(Graphics g, Rectangle rect, int elevation, bool isAccented, Color? shadowColor = null)
        {
            if (elevation <= 0) return;

            Color actualShadowColor = shadowColor ?? (isAccented
                ? Color.FromArgb(40, Color.Black)
                : Color.FromArgb(30, 0, 0, 0));

            // Draw multiple layers for soft shadow
            for (int i = elevation; i > 0; i -= 2)
            {
                int alpha = (int)(30 * ((float)i / elevation));
                var shadowRect = new Rectangle(
                    rect.X + (elevation - i) / 2 + 2,
                    rect.Y + (elevation - i) / 2 + 2,
                    rect.Width,
                    rect.Height
                );

                using (var path = CreateRoundedRectPath(shadowRect, CornerRadius + 2))
                using (var brush = new SolidBrush(Color.FromArgb(alpha, actualShadowColor)))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private void DrawColumnHeader(Graphics g, VerticalColumnLayout col, SimpleItem column, bool isHovered, bool isSelected, bool isFeatured, Color headerBg, IBeepTheme? theme = null, bool useThemeColors = false, BeepControlStyle controlStyle = BeepControlStyle.Material3)
        {
            var rect = col.HeaderBounds;

            // Create header path with top rounded corners only
            using (var path = CreateTopRoundedRectPath(rect, CornerRadius))
            {
                // Fill header with gradient for featured/selected
                if (isFeatured || isSelected)
                {
                    Color accentColor = theme != null && useThemeColors && theme.AccentColor != Color.Empty
                        ? theme.AccentColor
                        : Color.FromArgb(52, 168, 83);
                    using (var brush = new LinearGradientBrush(rect, accentColor, Color.FromArgb(200, accentColor), 90f))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(headerBg))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }

            // Draw header content using theme helpers
            Color textColor = VerticalTableThemeHelpers.GetHeaderTextColor(theme, useThemeColors, isSelected, isFeatured);
            int padding = 12;
            int yOffset = rect.Top + padding;

            // Plan name (title) using font helpers
            using (var titleFont = VerticalTableFontHelpers.GetHeaderFont(controlStyle, isFeatured))
            {
                var titleRect = new Rectangle(rect.Left + padding, yOffset, rect.Width - padding * 2, 28);
                using (var brush = new SolidBrush(textColor))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                    g.DrawString(column.Text ?? column.Name ?? "", titleFont, brush, titleRect, sf);
                }
                yOffset += 30;
            }

            // Price (if Value is set, use it as price)
            string? priceValue = column.Value?.ToString();
            if (!string.IsNullOrEmpty(priceValue))
            {
                using (var priceFont = VerticalTableFontHelpers.GetPriceFont(controlStyle, isFeatured))
                using (var suffixFont = VerticalTableFontHelpers.GetSubtextFont(controlStyle))
                {
                    var priceRect = new Rectangle(rect.Left + padding, yOffset, rect.Width - padding * 2, 40);
                    using (var brush = new SolidBrush(textColor))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                        g.DrawString(priceValue, priceFont, brush, (RectangleF)priceRect, sf);
                    }
                    yOffset += 42;

                    // Period suffix (e.g., "/month")
                    if (!string.IsNullOrEmpty(column.Description))
                    {
                        var suffixRect = new Rectangle(rect.Left + padding, yOffset, rect.Width - padding * 2, 20);
                        Color suffixColor = (isFeatured || isSelected) ? Color.FromArgb(200, 255, 255, 255) : Color.FromArgb(120, 130, 150);
                        using (var brush = new SolidBrush(suffixColor))
                        {
                            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                            g.DrawString(column.Description, suffixFont, brush, suffixRect, sf);
                        }
                    }
                }
            }
        }

        private void DrawFeaturedBadge(Graphics g, VerticalColumnLayout col, string badgeText, IBeepTheme? theme = null, bool useThemeColors = false, BeepControlStyle controlStyle = BeepControlStyle.Material3)
        {
            var badgeHeight = 24;
            using (var badgeFont = VerticalTableFontHelpers.GetBadgeFont(controlStyle))
            {
                var badgeWidth = (int)g.MeasureString(badgeText, badgeFont).Width + 20;
                var badgeRect = new Rectangle(
                    col.HeaderBounds.Left + (col.HeaderBounds.Width - badgeWidth) / 2,
                    col.HeaderBounds.Top - badgeHeight / 2,
                    badgeWidth,
                    badgeHeight
                );

                    using (var path = CreateRoundedRectPath(badgeRect, badgeHeight / 2))
                {
                    Color accentColor = theme != null && useThemeColors && theme.AccentColor != Color.Empty
                        ? theme.AccentColor
                        : Color.FromArgb(52, 168, 83);
                    using (var brush = new SolidBrush(accentColor))
                    {
                        g.FillPath(brush, path);
                    }
                }

                using (var brush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(badgeText, badgeFont, brush, badgeRect, sf);
                }
            }
        }

        private void DrawCell(Graphics g, VerticalCellLayout cell, SimpleItem item, 
            bool isCellHovered, bool isRowHovered, bool isColHovered,
            bool isCellSelected, bool isRowSelected, bool isColSelected, 
            bool isColumnFeatured, IBeepTheme? theme = null, bool useThemeColors = false, BeepControlStyle controlStyle = BeepControlStyle.Material3)
        {
            var rect = cell.Bounds;
            int padding = 12;

            // Get cell background color using theme helpers
            Color cellBg = VerticalTableThemeHelpers.GetCellBackgroundColor(
                theme, 
                useThemeColors, 
                isCellHovered || isRowHovered || isColHovered,
                isCellSelected || isRowSelected || isColSelected,
                false); // Alternate row not implemented yet

            // Determine background based on selection/hover priority:
            // 1. Exact cell selected (intersection) - strongest
            // 2. Row selected OR column selected - medium highlight
            // 3. Exact cell hovered - hover effect
            // 4. Row hovered OR column hovered - light hover
            if (isCellSelected)
            {
                // Intersection of row and column selection - strongest highlight
                Color accentColor = theme != null && useThemeColors && theme.AccentColor != Color.Empty
                    ? theme.AccentColor
                    : Color.FromArgb(52, 168, 83);
                using (var brush = new SolidBrush(Color.FromArgb(60, accentColor)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isRowSelected || isColSelected)
            {
                // Row or column is selected - medium highlight
                Color accentColor = theme != null && useThemeColors && theme.AccentColor != Color.Empty
                    ? theme.AccentColor
                    : Color.FromArgb(52, 168, 83);
                int alpha = (isRowSelected && isColSelected) ? 40 : 25;
                using (var brush = new SolidBrush(Color.FromArgb(alpha, accentColor)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isCellHovered)
            {
                using (var brush = new SolidBrush(cellBg))
                {
                    g.FillRectangle(brush, rect);
                }
            }
            else if (isRowHovered || isColHovered)
            {
                using (var brush = new SolidBrush(cellBg))
                {
                    g.FillRectangle(brush, rect);
                }
            }

            // Draw separator line
            using (var pen = new Pen(Color.FromArgb(40, 200, 200, 200), 1))
            {
                g.DrawLine(pen, rect.Left + padding, rect.Bottom - 1, rect.Right - padding, rect.Bottom - 1);
            }

            // Draw checkmark or bullet
            int iconSize = 16;
            int iconX = rect.Left + padding;
            int iconY = rect.Top + (rect.Height - iconSize) / 2;

            Color checkColor = isColumnFeatured
                ? (theme != null && useThemeColors && theme.AccentColor != Color.Empty ? theme.AccentColor : Color.FromArgb(52, 168, 83))
                : Color.FromArgb(100, 180, 100);

            // Draw checkmark circle
            using (var brush = new SolidBrush(Color.FromArgb(30, checkColor)))
            {
                g.FillEllipse(brush, iconX, iconY, iconSize, iconSize);
            }
            using (var pen = new Pen(checkColor, 2))
            {
                // Draw checkmark
                g.DrawLine(pen, iconX + 4, iconY + 8, iconX + 7, iconY + 11);
                g.DrawLine(pen, iconX + 7, iconY + 11, iconX + 12, iconY + 5);
            }

            // Draw text using theme helpers
            var textRect = new Rectangle(iconX + iconSize + 8, rect.Top, rect.Width - padding * 2 - iconSize - 8, rect.Height);
            Color textColor = VerticalTableThemeHelpers.GetCellTextColor(theme, useThemeColors, isCellSelected || isRowSelected || isColSelected);

            using (var font = VerticalTableFontHelpers.GetCellFont(controlStyle, isCellSelected || isRowSelected || isColSelected))
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

            // Top-left
            path.AddArc(arc, 180, 90);
            // Top-right
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            // Bottom-right
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            // Bottom-left
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

            // Top-left
            path.AddArc(arc, 180, 90);
            // Top-right
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            // Bottom-right (no curve)
            path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);

            path.CloseFigure();
            return path;
        }

        public void OnCellSelected(VerticalTableLayoutHelper layout, SimpleItem? item, int columnIndex, int rowIndex) { }
        public void OnCellHoverChanged(VerticalTableLayoutHelper layout, int columnIndex, int rowIndex) { }
    }
}
