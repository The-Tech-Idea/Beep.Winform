using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers;
using System.ComponentModel;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Painters
{
    /// <summary>
    /// Style 14 — DarkTerminal: Dark background with neon borders and glow effects
    /// on hover/selection. Cyberpunk / holographic theme.
    /// </summary>
    public class VerticalTableStyle14Painter : IVerticalTablePainter
    {
        // Dark terminal color palette
        private static readonly Color DarkBg = Color.FromArgb(10, 12, 20);
        private static readonly Color DarkSurface = Color.FromArgb(18, 22, 36);
        private static readonly Color DarkHeaderBg = Color.FromArgb(14, 16, 30);
        private static readonly Color NeonCyan = Color.FromArgb(0, 240, 255);
        private static readonly Color NeonMagenta = Color.FromArgb(255, 0, 128);
        private static readonly Color NeonGreen = Color.FromArgb(0, 255, 136);
        private static readonly Color NeonBlue = Color.FromArgb(80, 120, 255);
        private static readonly Color GridLine = Color.FromArgb(30, 40, 70);
        private static readonly Color TextPrimary = Color.FromArgb(220, 230, 245);
        private static readonly Color TextDim = Color.FromArgb(140, 150, 175);
        private static readonly Color HoverGlow = Color.FromArgb(20, 180, 255, 50);
        private static readonly Color SelectedGlow = Color.FromArgb(35, 0, 200, 255);

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
                x += columnWidth + padding;
            }

            layout.SetColumns(layoutColumns);
        }

        public void Paint(Graphics g, Rectangle bounds, BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, object owner)
        {
            if (g == null || layout == null || columns == null) return;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            IBeepTheme? theme = null;
            bool useThemeColors = false;
            int highlightedCol = -1;

            if (owner is BeepVerticalTable table)
            {
                theme = table._currentTheme ?? (table.UseThemeColors ? BeepThemesManager.CurrentTheme : null);
                useThemeColors = table.UseThemeColors;
                highlightedCol = table.HighlightedColumnIndex;
            }

            int padding = 8;

            // Full dark background
            using (var backBrush = new SolidBrush(DarkBg))
                g.FillRectangle(backBrush, bounds);

            // Subtle grid pattern
            DrawGridPattern(g, bounds);

            // Draw scanner line effect at top
            DrawScannerLine(g, bounds);

            // Draw columns
            foreach (var col in layout.Columns)
            {
                var column = col.Column;
                if (column == null) continue;

                bool isColumnHovered = layout.HoverColumnIndex == col.ColumnIndex;
                bool isColumnSelected = layout.SelectedColumnIndex == col.ColumnIndex;
                bool isFeatured = col.ColumnIndex == highlightedCol;

                DrawTerminalColumn(g, col, column, isColumnHovered, isColumnSelected, isFeatured,
                    layout, theme, useThemeColors);
            }
        }

        private void DrawGridPattern(Graphics g, Rectangle bounds)
        {
            // Subtle hexagonal or dot grid
            using (var dotBrush = new SolidBrush(Color.FromArgb(8, 255, 255, 255)))
            {
                for (int x = bounds.Left; x < bounds.Right; x += 20)
                    for (int y = bounds.Top; y < bounds.Bottom; y += 20)
                        g.FillEllipse(dotBrush, x, y, 2, 2);
            }
        }

        private void DrawScannerLine(Graphics g, Rectangle bounds)
        {
            // Thin scanning line effect at top
            using (var brush = new LinearGradientBrush(
                new Rectangle(0, 0, bounds.Width, 2),
                Color.FromArgb(0, NeonCyan),
                Color.FromArgb(40, NeonCyan),
                0f))
            {
                g.FillRectangle(brush, 0, 0, bounds.Width, 2);
            }
        }

        private void DrawTerminalColumn(Graphics g, VerticalColumnLayout col, SimpleItem column,
            bool isHovered, bool isSelected, bool isFeatured,
            VerticalTableLayoutHelper layout, IBeepTheme? theme, bool useThemeColors)
        {
            // Column background
            var colRect = col.ColumnBounds;
            using (var brush = new SolidBrush(DarkSurface))
                g.FillRectangle(brush, colRect);

            // Border with optional glow
            Color borderColor = GridLine;
            int borderWidth = 1;

            if (isFeatured)
            {
                borderColor = NeonCyan;
                borderWidth = 2;
                DrawNeonGlow(g, colRect, NeonCyan, 8);
            }
            else if (isSelected)
            {
                borderColor = NeonBlue;
                borderWidth = 2;
                DrawNeonGlow(g, colRect, NeonBlue, 6);
            }
            else if (isHovered)
            {
                borderColor = Color.FromArgb(60, 80, 140);
                borderWidth = 1;
            }

            using (var pen = new Pen(borderColor, borderWidth))
                g.DrawRectangle(pen, colRect);

            // Header
            DrawTerminalHeader(g, col, column, isFeatured, isSelected, theme, useThemeColors);

            // Cells
            foreach (var cell in col.Cells)
            {
                bool isCellHovered = layout.IsCellHovered(col.ColumnIndex, cell.RowIndex);
                bool isRowHovered = layout.IsRowHovered(cell.RowIndex);
                bool isCellSelected = layout.IsCellSelected(col.ColumnIndex, cell.RowIndex);
                bool isRowSelected = layout.IsRowSelected(cell.RowIndex);
                bool isColSelected = layout.IsColumnSelected(col.ColumnIndex);

                DrawTerminalCell(g, cell, isCellHovered || isRowHovered,
                    isCellSelected || isRowSelected || isColSelected, isFeatured);
            }
        }

        private void DrawNeonGlow(Graphics g, Rectangle rect, Color glowColor, int spread)
        {
            // Outer glow effect via multiple semi-transparent lines
            for (int i = spread; i > 0; i -= 2)
            {
                int alpha = (int)(15 * ((float)i / spread));
                var glowRect = new Rectangle(rect.X - i / 2, rect.Y - i / 2, rect.Width + i, rect.Height + i);
                using (var pen = new Pen(Color.FromArgb(alpha, glowColor), i))
                    g.DrawRectangle(pen, glowRect);
            }
        }

        private void DrawTerminalHeader(Graphics g, VerticalColumnLayout col, SimpleItem column,
            bool isFeatured, bool isSelected, IBeepTheme? theme, bool useThemeColors)
        {
            var rect = col.HeaderBounds;

            // Header background
            using (var brush = new SolidBrush(DarkHeaderBg))
                g.FillRectangle(brush, rect);

            // Bottom border
            Color accentColor = isFeatured ? NeonCyan : NeonMagenta;
            using (var pen = new Pen(accentColor, 2))
                g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);

            int yOffset = rect.Top + 10;

            // Title with monospace/terminal style
            string title = column.Text ?? column.Name ?? "";
            using (var font = new Font("Consolas", 11, FontStyle.Bold))
            {
                var titleRect = new Rectangle(rect.Left + 12, yOffset, rect.Width - 24, 24);
                using (var brush = new SolidBrush(isFeatured ? NeonCyan : TextPrimary))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                    g.DrawString(title, font, brush, titleRect, sf);
                }
                yOffset += 26;
            }

            // Price with neon green
            string? priceValue = column.Value?.ToString();
            if (!string.IsNullOrEmpty(priceValue))
            {
                using (var font = new Font("Consolas", 16, FontStyle.Bold))
                {
                    var priceRect = new Rectangle(rect.Left + 12, yOffset, rect.Width - 24, 30);
                    using (var brush = new SolidBrush(isFeatured ? NeonGreen : TextPrimary))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                        g.DrawString(priceValue, font, brush, priceRect, sf);
                    }
                }
                yOffset += 32;
            }

            // Description
            if (!string.IsNullOrEmpty(column.Description))
            {
                using (var font = new Font("Consolas", 8, FontStyle.Regular))
                {
                    var descRect = new Rectangle(rect.Left + 12, yOffset, rect.Width - 24, 18);
                    using (var brush = new SolidBrush(TextDim))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                        g.DrawString(column.Description, font, brush, descRect, sf);
                    }
                }
            }

            // Featured badge
            if (isFeatured)
            {
                DrawTerminalBadge(g, col, "/// OPTIMAL ///", NeonCyan);
            }
        }

        private void DrawTerminalBadge(Graphics g, VerticalColumnLayout col, string badgeText, Color glowColor)
        {
            int badgeHeight = 22;
            using (var font = new Font("Consolas", 8, FontStyle.Bold))
            {
                var badgeWidth = (int)g.MeasureString(badgeText, font).Width + 20;
                var badgeRect = new Rectangle(
                    col.HeaderBounds.Left + (col.HeaderBounds.Width - badgeWidth) / 2,
                    col.HeaderBounds.Top - badgeHeight / 2,
                    badgeWidth, badgeHeight);

                // Glow
                DrawNeonGlow(g, badgeRect, glowColor, 6);

                // Badge background
                using (var brush = new SolidBrush(DarkHeaderBg))
                    g.FillRectangle(brush, badgeRect);

                // Border
                using (var pen = new Pen(glowColor, 1))
                    g.DrawRectangle(pen, badgeRect);

                // Text
                using (var brush = new SolidBrush(glowColor))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(badgeText, font, brush, badgeRect, sf);
                }
            }
        }

        private void DrawTerminalCell(Graphics g, VerticalCellLayout cell, bool isHovered, bool isSelected, bool isFeatured)
        {
            var rect = cell.Bounds;
            var item = cell.Item;
            if (item == null) return;

            // Cell background: alternating subtly different dark surfaces
            Color cellBg = (cell.RowIndex % 2 == 0) ? DarkSurface : Color.FromArgb(22, 26, 44);

            if (isSelected)
            {
                // Strong glow effect
                DrawNeonGlow(g, rect, NeonBlue, 6);
                cellBg = SelectedGlow;
            }
            else if (isHovered)
            {
                cellBg = HoverGlow;
            }

            using (var brush = new SolidBrush(cellBg))
                g.FillRectangle(brush, rect);

            // Horizontal separator
            using (var pen = new Pen(GridLine, 1))
                g.DrawLine(pen, rect.Left + 12, rect.Bottom - 1, rect.Right - 12, rect.Bottom - 1);

            string? text = item.Text ?? item.Name;
            bool isNegative = !string.IsNullOrEmpty(text) &&
                (text?.ToLower() is "x" or "false" or "no" or "-" or "—");

            int iconSize = 16;
            int iconX = rect.Left + (rect.Width - iconSize) / 2;
            int iconY = rect.Top + (rect.Height - iconSize) / 2;

            if (item.IsEnabled && string.IsNullOrEmpty(text))
            {
                // Neon green checkmark
                Color checkColor = isFeatured ? NeonCyan : NeonGreen;
                using (var pen = new Pen(checkColor, 2f))
                {
                    pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, iconX + 2, iconY + 8, iconX + 6, iconY + 12);
                    g.DrawLine(pen, iconX + 6, iconY + 12, iconX + 14, iconY + 4);

                    // Glow overlay
                    using (var glowPen = new Pen(Color.FromArgb(60, checkColor), 4f))
                    {
                        glowPen.StartCap = LineCap.Round; glowPen.EndCap = LineCap.Round;
                        g.DrawLine(glowPen, iconX + 2, iconY + 8, iconX + 6, iconY + 12);
                        g.DrawLine(glowPen, iconX + 6, iconY + 12, iconX + 14, iconY + 4);
                    }
                }
            }
            else if (isNegative)
            {
                // Neon red X
                using (var pen = new Pen(Color.FromArgb(255, 60, 80), 2f))
                {
                    pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, iconX + 3, iconY + 3, iconX + iconSize - 3, iconY + iconSize - 3);
                    g.DrawLine(pen, iconX + iconSize - 3, iconY + 3, iconX + 3, iconY + iconSize - 3);
                }
            }
            else if (!string.IsNullOrEmpty(text))
            {
                // Monospace text
                using (var font = new Font("Consolas", 10, FontStyle.Regular))
                {
                    Color textColor = isFeatured ? NeonCyan : (isSelected ? Color.White : TextPrimary);
                    using (var brush = new SolidBrush(textColor))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString(text, font, brush, rect, sf);
                    }
                }
            }
            else
            {
                // Dash
                using (var pen = new Pen(TextDim, 1.5f))
                    g.DrawLine(pen, iconX + 2, iconY + iconSize / 2, iconX + iconSize - 2, iconY + iconSize / 2);
            }

            // Selection borders
            if (isSelected)
            {
                using (var pen = new Pen(NeonBlue, 1.5f))
                    g.DrawRectangle(pen, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3);
            }
        }

        public void OnCellSelected(VerticalTableLayoutHelper layout, SimpleItem? item, int columnIndex, int rowIndex) { }
        public void OnCellHoverChanged(VerticalTableLayoutHelper layout, int columnIndex, int rowIndex) { }
    }
}
