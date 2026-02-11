using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Base implementation for top filter panel painters.
    /// Handles common geometry, glyph drawing, text rendering, and hit-map registration.
    /// </summary>
    public abstract class BaseFilterPanelPainter : IGridFilterPanelPainter
    {
        public abstract navigationStyle Style { get; }
        public abstract string StyleName { get; }

        public virtual int CalculateFilterPanelHeight(BeepGridPro grid)
        {
            return 34;
        }

        public virtual void PaintFilterPanel(
            Graphics g,
            Rectangle panelRect,
            BeepGridPro grid,
            IBeepTheme? theme,
            Dictionary<int, Rectangle> filterCellRects,
            Dictionary<int, Rectangle> clearIconRects)
        {
            if (g == null || grid?.Layout == null || panelRect.Width <= 0 || panelRect.Height <= 0)
            {
                return;
            }

            var tokens = CreateStyleTokens(theme);

            using (var panelBrush = new SolidBrush(tokens.PanelBackColor))
            using (var borderPen = new Pen(tokens.PanelBorderColor))
            {
                g.FillRectangle(panelBrush, panelRect);
                g.DrawLine(borderPen, panelRect.Left, panelRect.Top, panelRect.Right, panelRect.Top);
                g.DrawLine(borderPen, panelRect.Left, panelRect.Bottom - 1, panelRect.Right, panelRect.Bottom - 1);
            }

            var state = g.Save();
            g.SetClip(panelRect);

            try
            {
                int columnCount = grid.Data.Columns.Count;
                for (int i = 0; i < columnCount; i++)
                {
                    var column = grid.Data.Columns[i];
                    if (!column.Visible || column.IsSelectionCheckBox || column.IsRowNumColumn || column.IsRowID)
                    {
                        filterCellRects.Remove(i);
                        clearIconRects.Remove(i);
                        continue;
                    }

                    if (i >= grid.Layout.HeaderCellRects.Length)
                    {
                        continue;
                    }

                    var headerCellRect = grid.Layout.HeaderCellRects[i];
                    if (headerCellRect.IsEmpty || headerCellRect.Width <= 0)
                    {
                        continue;
                    }

                    var filterCellRect = new Rectangle(headerCellRect.X, panelRect.Top, headerCellRect.Width, panelRect.Height);
                    if (filterCellRect.Right < panelRect.Left || filterCellRect.Left > panelRect.Right)
                    {
                        continue;
                    }

                    DrawFilterCell(g, column, i, filterCellRect, theme, tokens, filterCellRects, clearIconRects);
                }
            }
            finally
            {
                g.Restore(state);
            }
        }

        protected virtual FilterPanelStyleTokens CreateStyleTokens(IBeepTheme? theme)
        {
            Color headerBack = theme?.GridHeaderBackColor ?? SystemColors.Control;
            Color gridBack = theme?.GridBackColor ?? SystemColors.Window;
            Color headerFore = theme?.GridHeaderForeColor ?? SystemColors.ControlText;
            Color gridLine = theme?.GridLineColor ?? SystemColors.ControlDark;
            Color focus = (theme?.FocusIndicatorColor ?? Color.Empty) != Color.Empty
                ? theme!.FocusIndicatorColor
                : SystemColors.Highlight;

            var tokens = new FilterPanelStyleTokens
            {
                PanelBackColor = BlendColors(headerBack, gridBack, 0.15f),
                PanelBorderColor = gridLine,
                InactiveChipBackColor = Color.FromArgb(90, Color.White),
                ActiveChipBackColor = Color.FromArgb(40, focus),
                InactiveChipBorderColor = Color.FromArgb(150, gridLine),
                ActiveChipBorderColor = Color.FromArgb(190, focus),
                InactiveTextColor = Color.FromArgb(150, headerFore),
                ActiveTextColor = headerFore,
                InactiveGlyphColor = Color.FromArgb(140, headerFore),
                ActiveGlyphColor = focus,
                ClearGlyphColor = Color.FromArgb(170, headerFore),
                OuterPadding = 4,
                CornerRadius = 8,
                BorderWidthInactive = 1f,
                BorderWidthActive = 1.5f
            };

            switch (Style)
            {
                case navigationStyle.Material:
                    tokens.PanelBackColor = BlendColors(headerBack, gridBack, 0.22f);
                    tokens.CornerRadius = 10;
                    tokens.BorderWidthActive = 1.8f;
                    break;
                case navigationStyle.Compact:
                    tokens.OuterPadding = 3;
                    tokens.CornerRadius = 5;
                    break;
                case navigationStyle.Minimal:
                    tokens.PanelBackColor = BlendColors(headerBack, gridBack, 0.10f);
                    tokens.InactiveChipBackColor = Color.FromArgb(55, Color.White);
                    tokens.CornerRadius = 6;
                    break;
                case navigationStyle.Bootstrap:
                    tokens.CornerRadius = 6;
                    tokens.PanelBackColor = BlendColors(headerBack, gridBack, 0.18f);
                    break;
                case navigationStyle.Fluent:
                    tokens.CornerRadius = 11;
                    tokens.InactiveChipBackColor = Color.FromArgb(120, Color.White);
                    break;
                case navigationStyle.AntDesign:
                    tokens.CornerRadius = 7;
                    tokens.BorderWidthInactive = 1.2f;
                    break;
                case navigationStyle.Telerik:
                    tokens.CornerRadius = 9;
                    tokens.PanelBackColor = BlendColors(headerBack, gridBack, 0.20f);
                    break;
                case navigationStyle.AGGrid:
                    tokens.CornerRadius = 4;
                    tokens.OuterPadding = 3;
                    break;
                case navigationStyle.DataTables:
                    tokens.CornerRadius = 4;
                    tokens.OuterPadding = 3;
                    tokens.InactiveChipBackColor = Color.FromArgb(80, Color.White);
                    break;
                case navigationStyle.Card:
                    tokens.CornerRadius = 12;
                    tokens.PanelBackColor = BlendColors(headerBack, gridBack, 0.25f);
                    break;
                case navigationStyle.Tailwind:
                    tokens.CornerRadius = 6;
                    tokens.PanelBackColor = BlendColors(headerBack, gridBack, 0.14f);
                    tokens.InactiveChipBackColor = Color.FromArgb(105, Color.White);
                    break;
            }

            return tokens;
        }

        private void DrawFilterCell(
            Graphics g,
            BeepColumnConfig column,
            int columnIndex,
            Rectangle cellRect,
            IBeepTheme? theme,
            FilterPanelStyleTokens tokens,
            Dictionary<int, Rectangle> filterCellRects,
            Dictionary<int, Rectangle> clearIconRects)
        {
            int outerPadding = Math.Max(2, tokens.OuterPadding);
            int radius = Math.Max(2, tokens.CornerRadius);

            var chipRect = new Rectangle(
                cellRect.X + outerPadding,
                cellRect.Y + outerPadding,
                Math.Max(1, cellRect.Width - (outerPadding * 2)),
                Math.Max(1, cellRect.Height - (outerPadding * 2)));

            bool hasActiveFilter = column.IsFiltered && !string.IsNullOrWhiteSpace(column.Filter);
            Color chipBack = hasActiveFilter ? tokens.ActiveChipBackColor : tokens.InactiveChipBackColor;
            Color chipBorder = hasActiveFilter ? tokens.ActiveChipBorderColor : tokens.InactiveChipBorderColor;
            Color textColor = hasActiveFilter ? tokens.ActiveTextColor : tokens.InactiveTextColor;
            Color glyphColor = hasActiveFilter ? tokens.ActiveGlyphColor : tokens.InactiveGlyphColor;
            float borderWidth = hasActiveFilter ? tokens.BorderWidthActive : tokens.BorderWidthInactive;

            using (var path = CreateRoundedRectangle(chipRect, radius))
            using (var chipBrush = new SolidBrush(chipBack))
            using (var chipPen = new Pen(chipBorder, borderWidth))
            {
                g.FillPath(chipBrush, path);
                g.DrawPath(chipPen, path);
            }

            int iconSize = Math.Max(10, chipRect.Height - 10);
            var filterIconRect = new Rectangle(
                chipRect.X + 6,
                chipRect.Y + (chipRect.Height - iconSize) / 2,
                iconSize,
                iconSize);
            DrawFilterGlyph(g, filterIconRect, glyphColor, hasActiveFilter);

            Rectangle clearRect = Rectangle.Empty;
            if (hasActiveFilter)
            {
                int clearSize = Math.Max(10, iconSize - 1);
                clearRect = new Rectangle(
                    chipRect.Right - clearSize - 6,
                    chipRect.Y + (chipRect.Height - clearSize) / 2,
                    clearSize,
                    clearSize);
                DrawClearGlyph(g, clearRect, tokens.ClearGlyphColor);
                clearIconRects[columnIndex] = clearRect;
            }
            else
            {
                clearIconRects.Remove(columnIndex);
            }

            string caption = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            string valueText = string.IsNullOrWhiteSpace(column.Filter)
                ? $"Filter {caption}"
                : column.Filter!.Trim();

            var font = (theme?.GridHeaderFont != null ? BeepThemesManager.ToFont(theme.GridHeaderFont) : null) ?? SystemFonts.DefaultFont;
            var textRect = new Rectangle(
                filterIconRect.Right + 6,
                chipRect.Y + 1,
                Math.Max(1, chipRect.Width - (filterIconRect.Width + 12 + (clearRect.IsEmpty ? 0 : clearRect.Width + 8))),
                Math.Max(1, chipRect.Height - 2));

            TextRenderer.DrawText(
                g,
                valueText,
                font,
                textRect,
                textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            filterCellRects[columnIndex] = chipRect;
        }

        private static void DrawFilterGlyph(Graphics g, Rectangle rect, Color color, bool active)
        {
            using var pen = new Pen(color, active ? 1.6f : 1.25f);
            using var brush = new SolidBrush(Color.FromArgb(active ? 140 : 90, color));

            int padding = 1;
            int funnelHeight = rect.Height - (padding * 2);
            Point[] funnel =
            {
                new Point(rect.X + padding, rect.Y + padding),
                new Point(rect.Right - padding, rect.Y + padding),
                new Point(rect.X + rect.Width / 2 + 2, rect.Y + funnelHeight / 2),
                new Point(rect.X + rect.Width / 2 + 2, rect.Bottom - padding),
                new Point(rect.X + rect.Width / 2 - 2, rect.Bottom - padding),
                new Point(rect.X + rect.Width / 2 - 2, rect.Y + funnelHeight / 2)
            };

            g.FillPolygon(brush, funnel);
            g.DrawPolygon(pen, funnel);
        }

        private static void DrawClearGlyph(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 1.4f);
            g.DrawLine(pen, rect.Left + 3, rect.Top + 3, rect.Right - 3, rect.Bottom - 3);
            g.DrawLine(pen, rect.Right - 3, rect.Top + 3, rect.Left + 3, rect.Bottom - 3);
        }

        protected static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 1 || rect.Height <= 1 || radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected static Color BlendColors(Color baseColor, Color blendColor, float blendFactor)
        {
            blendFactor = Math.Max(0f, Math.Min(1f, blendFactor));
            int r = (int)(baseColor.R + ((blendColor.R - baseColor.R) * blendFactor));
            int gr = (int)(baseColor.G + ((blendColor.G - baseColor.G) * blendFactor));
            int b = (int)(baseColor.B + ((blendColor.B - baseColor.B) * blendFactor));
            return Color.FromArgb(255, r, gr, b);
        }

        protected sealed class FilterPanelStyleTokens
        {
            public Color PanelBackColor { get; set; }
            public Color PanelBorderColor { get; set; }
            public Color InactiveChipBackColor { get; set; }
            public Color ActiveChipBackColor { get; set; }
            public Color InactiveChipBorderColor { get; set; }
            public Color ActiveChipBorderColor { get; set; }
            public Color InactiveTextColor { get; set; }
            public Color ActiveTextColor { get; set; }
            public Color InactiveGlyphColor { get; set; }
            public Color ActiveGlyphColor { get; set; }
            public Color ClearGlyphColor { get; set; }
            public int OuterPadding { get; set; }
            public int CornerRadius { get; set; }
            public float BorderWidthInactive { get; set; }
            public float BorderWidthActive { get; set; }
        }
    }
}
