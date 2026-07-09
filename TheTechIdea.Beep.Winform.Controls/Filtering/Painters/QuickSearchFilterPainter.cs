using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Filtering.Painters
{
    /// <summary>
    /// Painter for QuickSearch filter style - single search bar with instant filtering
    /// Clean, simple interface with column selector and filter count badge
    /// </summary>
    public class QuickSearchFilterPainter : BaseFilterPainter
    {
        private const int SearchBarHeight = 36;
        private const int ColumnSelectorWidth = 120;
        private const int BadgeSize = 20;
        private const int ItemSpacing = 8;

        public override FilterStyle FilterStyle => FilterStyle.QuickSearch;
        public override bool SupportsDragDrop => false;

        public override FilterLayoutInfo CalculateLayout(BeepFilter owner, Rectangle availableRect)
        {
            var layout = new FilterLayoutInfo
            {
                ContainerRect = availableRect,
                ContentRect = availableRect
            };

            int padding = 8;
            int currentX = availableRect.X + padding;
            int currentY = availableRect.Y + padding;
            int height = Math.Min(SearchBarHeight, Math.Max(24, availableRect.Height - (padding * 2)));

            // Column selector dropdown (left)
            int columnWidth = Math.Min(ColumnSelectorWidth, Math.Max(80, availableRect.Width / 3));
            var columnRect = new Rectangle(
                currentX,
                currentY,
                columnWidth,
                height
            );
            currentX += columnWidth + ItemSpacing;

            // Search input (center - takes remaining space minus badge)
            int searchWidth = availableRect.Width - (currentX - availableRect.X) - padding - BadgeSize - ItemSpacing * 2;
            searchWidth = Math.Max(120, searchWidth);
            var searchRect = new Rectangle(
                currentX,
                currentY,
                searchWidth,
                height
            );

            if (searchRect.Right > availableRect.Right - padding)
            {
                searchRect.Width = Math.Max(80, (availableRect.Right - padding) - searchRect.X);
            }

            layout.SearchInputRect = searchRect;
            currentX += searchWidth + ItemSpacing;

            // Active filter count badge (right)
            var badgeRect = new Rectangle(
                currentX,
                currentY + (height - BadgeSize) / 2,
                BadgeSize,
                BadgeSize
            );
            layout.CountBadgeRect = badgeRect;

            // Store column selector in a row rect for hit testing
            layout.RowRects = new Rectangle[] { columnRect };

            return layout;
        }

        /// <summary>Paints the filter UI elements.</summary>
        public override void Paint(Graphics g, BeepFilter owner, FilterLayoutInfo layout)
        {
            if (layout == null)
                return;

            var config = owner.ActiveFilter;

            // Paint column selector
            if (layout.RowRects.Length > 0)
            {
                string columnText = "All Columns";
                if (config?.Criteria.Count > 0)
                {
                    columnText = config.Criteria[0].ColumnName;
                }
                PaintColumnSelector(g, layout.RowRects[0], columnText, owner);
            }

            // Paint search input
            if (!layout.SearchInputRect.IsEmpty)
            {
                string searchText = "";
                if (config?.Criteria.Count > 0)
                {
                    searchText = config.Criteria[0].Value?.ToString() ?? "";
                }
                PaintSearchInput(g, layout.SearchInputRect, searchText, owner);
            }

            // Phase 1: Paint filter count badge (using base class method)
            if (owner.ShowFilterCountBadge)
            {
                int count = config?.Criteria.Count(c => c.IsEnabled) ?? 0;
                if (count > 0 && layout.RowRects.Length > 0)
                {
                    var badgeLocation = new Point(
                        layout.SearchInputRect.Right + ItemSpacing,
                        layout.SearchInputRect.Y + (layout.SearchInputRect.Height - 20) / 2
                    );
                    var accentColor = owner._currentTheme?.AccentColor ?? Color.FromArgb(33, 150, 243);
                    PaintFilterCountBadge(g, count, badgeLocation, accentColor, owner);
                }
            }
        }

        private void PaintColumnSelector(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner, owner.ControlStyle);

            int crColSel = Helpers.DpiScalingHelper.ScaleValue(4, owner);

            // Background
            using (var path = CreateRoundedRectanglePath(rect, crColSel))
            {
                g.FillPath(GetBrush(Color.White), path);
            }

            // Border
            using (var path = CreateRoundedRectanglePath(rect, crColSel))
            {
                g.DrawPath(GetPen(colors.border, Helpers.DpiScalingHelper.ScaleValue(1, owner)), path);
            }

            // Text
            int textPad = Helpers.DpiScalingHelper.ScaleValue(10, owner);
            int textRight = Helpers.DpiScalingHelper.ScaleValue(28, owner);
            var textRect = new Rectangle(rect.X + textPad, rect.Y, rect.Width - textRight, rect.Height);
            TextRenderer.DrawText(g, text, GetFont(9f), textRect, colors.text,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            // Dropdown arrow
            int arrowX = rect.Right - Helpers.DpiScalingHelper.ScaleValue(14, owner);
            int arrowY = rect.Y + rect.Height / 2;
            var arrowPen = GetPen(colors.text, 1.5f);
            int arrowHalf = Helpers.DpiScalingHelper.ScaleValue(4, owner);
            g.DrawLine(arrowPen, arrowX - arrowHalf, arrowY - 2, arrowX, arrowY + 2);
            g.DrawLine(arrowPen, arrowX, arrowY + 2, arrowX + arrowHalf, arrowY - 2);
        }

        private void PaintSearchInput(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner, owner.ControlStyle);

            int crSrch = Helpers.DpiScalingHelper.ScaleValue(4, owner);

            // Background
            using (var path = CreateRoundedRectanglePath(rect, crSrch))
            {
                g.FillPath(GetBrush(Color.White), path);
            }

            // Border
            using (var path = CreateRoundedRectanglePath(rect, crSrch))
            {
                g.DrawPath(GetPen(colors.border, Helpers.DpiScalingHelper.ScaleValue(1, owner)), path);
            }

            // Search icon (left)
            int iconInset = Helpers.DpiScalingHelper.ScaleValue(12, owner);
            int iconX = rect.X + iconInset;
            int iconY = rect.Y + rect.Height / 2;
            int iconSize = Helpers.DpiScalingHelper.ScaleValue(14, owner);

            var iconPen = GetPen(Color.FromArgb(150, colors.text), 1.5f);
            // Circle
            g.DrawEllipse(iconPen, iconX - iconSize / 2 + 2, iconY - iconSize / 2 + 2, iconSize - 4, iconSize - 4);
            // Handle
            g.DrawLine(iconPen, iconX + iconSize / 2 - 2, iconY + iconSize / 2 - 2, iconX + iconSize / 2 + 1, iconY + iconSize / 2 + 1);

            // Text or placeholder
            var displayText = string.IsNullOrEmpty(text) ? "Search..." : text;
            var textColor = string.IsNullOrEmpty(text)
                ? Color.FromArgb(150, colors.text)
                : colors.text;

            int textLeft = Helpers.DpiScalingHelper.ScaleValue(32, owner);
            int textRight = Helpers.DpiScalingHelper.ScaleValue(48, owner);
            var textRect = new Rectangle(rect.X + textLeft, rect.Y, rect.Width - textRight, rect.Height);
            TextRenderer.DrawText(g, displayText, GetFont(10f), textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            // Clear button (X) if there's text
            if (!string.IsNullOrEmpty(text))
            {
                int clearX = rect.Right - Helpers.DpiScalingHelper.ScaleValue(20, owner);
                int clearY = rect.Y + rect.Height / 2;
                int clearSize = Helpers.DpiScalingHelper.ScaleValue(12, owner);

                using (var pen = new Pen(Color.FromArgb(150, colors.text), 1.5f))
                {
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    g.DrawLine(pen, clearX - clearSize / 2, clearY - clearSize / 2, clearX + clearSize / 2, clearY + clearSize / 2);
                    g.DrawLine(pen, clearX + clearSize / 2, clearY - clearSize / 2, clearX - clearSize / 2, clearY + clearSize / 2);
                }
            }
        }

        private void PaintBadge(Graphics g, Rectangle rect, int count, BeepFilter owner)
        {
            var colors = GetStyleColors(owner, owner.ControlStyle);

            // Background circle
            g.FillEllipse(GetBrush(colors.accent), rect);

            // Text
            string countText = count > 99 ? "99+" : count.ToString();
            TextRenderer.DrawText(g, countText, GetFont(8f, FontStyle.Bold), rect, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        /// <summary>Hit tests for search input, column selector, clear button, and badge.</summary>
        public override FilterHitArea? HitTest(Point point, FilterLayoutInfo layout)
        {
            if (layout.RowRects.Length > 0 && layout.RowRects[0].Contains(point))
            {
                return new FilterHitArea { Name = "ColumnSelector", Bounds = layout.RowRects[0], Type = FilterHitAreaType.FieldDropdown };
            }

            if (layout.SearchInputRect.Contains(point))
            {
                // Check clear button first (if visible, would be on right side)
                Rectangle clearRect = new Rectangle(
                    layout.SearchInputRect.Right - 30,
                    layout.SearchInputRect.Y + (layout.SearchInputRect.Height - 16) / 2,
                    16, 16
                );
                if (clearRect.Contains(point))
                    return new FilterHitArea { Name = "ClearSearch", Bounds = clearRect, Type = FilterHitAreaType.RemoveButton };

                // Search input area
                return new FilterHitArea { Name = "Search", Bounds = layout.SearchInputRect, Type = FilterHitAreaType.SearchInput };
            }

            // Check count badge
            if (layout.CountBadgeRect.Contains(point))
                return new FilterHitArea { Name = "CountBadge", Bounds = layout.CountBadgeRect, Type = FilterHitAreaType.FilterTag };

            return null;
        }
    }
}
