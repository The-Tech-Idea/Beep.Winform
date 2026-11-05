using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal partial class GridRenderHelper
    {
        internal void DrawFooter(Graphics g)
        {
            var footerRect = _grid.Layout.FooterRect;
            if (footerRect.IsEmpty || footerRect.Width <= 0 || footerRect.Height <= 0)
                return;

            var theme = Theme;
            Color backColor = theme?.GridHeaderBackColor is { } headerBack && headerBack != Color.Empty
                ? headerBack
                : (IsDarkTheme ? Color.FromArgb(45, 55, 72) : Color.FromArgb(245, 247, 250));
            Color foreColor = theme?.GridHeaderForeColor is { } headerFore && headerFore != Color.Empty
                ? headerFore
                : (IsDarkTheme ? Color.FromArgb(229, 231, 235) : Color.FromArgb(31, 41, 55));
            Color lineColor = theme?.GridLineColor is { } line && line != Color.Empty
                ? line
                : (IsDarkTheme ? Color.FromArgb(70, 80, 95) : Color.FromArgb(210, 214, 220));

            using (var bg = new SolidBrush(backColor))
                g.FillRectangle(bg, footerRect);
            using (var pen = new Pen(lineColor))
            {
                g.DrawLine(pen, footerRect.Left, footerRect.Top, footerRect.Right, footerRect.Top);
                g.DrawLine(pen, footerRect.Left, footerRect.Bottom - 1, footerRect.Right, footerRect.Bottom - 1);
            }

            var columns = _grid.Data.Columns
                .Select((c, index) => new { Column = c, Index = index })
                .Where(x => x.Column.Visible)
                .OrderBy(x => x.Column.DisplayOrder)
                .ToList();

            var sticky = columns.Where(x => x.Column.Sticked).ToList();
            var scrolling = columns.Where(x => !x.Column.Sticked).ToList();

            int stickyWidth = Math.Min(
                sticky.Sum(x => Math.Max(BeepLayoutMetrics.GridColumnMinW, x.Column.Width)),
                footerRect.Width);

            int stickyX = footerRect.Left;
            foreach (var item in sticky)
            {
                int width = Math.Max(BeepLayoutMetrics.GridColumnMinW, item.Column.Width);
                var cellRect = new Rectangle(stickyX, footerRect.Top, width, footerRect.Height);
                DrawFooterCell(g, item.Column, cellRect, foreColor, lineColor);
                stickyX += width;
            }

            var scrollingRegion = new Rectangle(
                footerRect.Left + stickyWidth,
                footerRect.Top,
                Math.Max(0, footerRect.Width - stickyWidth),
                footerRect.Height);

            if (scrollingRegion.Width > 0 && scrolling.Count > 0)
            {
                var state = g.Save();
                g.SetClip(scrollingRegion);
                int x = scrollingRegion.Left - _grid.Scroll.HorizontalOffset;
                foreach (var item in scrolling)
                {
                    int width = Math.Max(BeepLayoutMetrics.GridColumnMinW, item.Column.Width);
                    if (x + width > scrollingRegion.Left && x < scrollingRegion.Right)
                    {
                        var cellRect = new Rectangle(x, footerRect.Top, width, footerRect.Height);
                        DrawFooterCell(g, item.Column, cellRect, foreColor, lineColor);
                    }
                    x += width;
                    if (x > scrollingRegion.Right) break;
                }
                g.Restore(state);
            }
        }

        private void DrawFooterCell(Graphics g, BeepColumnConfig column, Rectangle rect,
            Color foreColor, Color lineColor)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            var aggregateType = _grid.Footer.GetAggregate(column.ColumnName);
            if (aggregateType == AggregationType.None)
            {
                if (ShowGridLines)
                {
                    using var separator = new Pen(lineColor);
                    g.DrawLine(separator, rect.Right - 1, rect.Top + 2, rect.Right - 1, rect.Bottom - 2);
                }
                return;
            }

            object? value = _grid.Footer.GetValue(_grid, column.ColumnName);
            string text = value == null ? "—" : Convert.ToString(value, System.Globalization.CultureInfo.CurrentCulture) ?? string.Empty;

            var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
            if (IsNumericAggregate(aggregateType))
                flags |= TextFormatFlags.Right;
            else
                flags |= TextFormatFlags.Left;

            TextRenderer.DrawText(g, text, GetSafeCellFont(), rect, foreColor, flags);

            if (ShowGridLines)
            {
                using var separator = new Pen(lineColor);
                g.DrawLine(separator, rect.Right - 1, rect.Top + 2, rect.Right - 1, rect.Bottom - 2);
            }
        }

        private static bool IsNumericAggregate(AggregationType type)
            => type is AggregationType.Sum
                or AggregationType.Average
                or AggregationType.Min
                or AggregationType.Max
                or AggregationType.Count
                or AggregationType.DistinctCount;
    }
}
