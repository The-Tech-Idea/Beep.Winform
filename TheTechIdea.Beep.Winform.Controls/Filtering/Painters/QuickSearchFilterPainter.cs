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

            // Column selector dropdown (left)
            var columnRect = new Rectangle(
                currentX,
                currentY,
                ColumnSelectorWidth,
                SearchBarHeight
            );
            currentX += ColumnSelectorWidth + ItemSpacing;

            // Search input (center - takes remaining space minus badge)
            int searchWidth = availableRect.Width - (currentX - availableRect.X) - padding - BadgeSize - ItemSpacing * 2;
            var searchRect = new Rectangle(
                currentX,
                currentY,
                searchWidth,
                SearchBarHeight
            );
            layout.SearchInputRect = searchRect;
            currentX += searchWidth + ItemSpacing;

            // Active filter count badge (right)
            var badgeRect = new Rectangle(
                currentX,
                currentY + (SearchBarHeight - BadgeSize) / 2,
                BadgeSize,
                BadgeSize
            );

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

            // Paint filter count badge
            if (owner.ShowCountBadge)
            {
                int count = config?.Criteria.Count(c => c.IsEnabled) ?? 0;
                if (count > 0 && layout.RowRects.Length > 0)
                {
                    var badgeRect = new Rectangle(
                        layout.SearchInputRect.Right + ItemSpacing,
                        layout.SearchInputRect.Y + (layout.SearchInputRect.Height - BadgeSize) / 2,
                        BadgeSize,
                        BadgeSize
                    );
                    PaintBadge(g, badgeRect, count, owner);
                }
            }
        }

        private void PaintColumnSelector(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner.ControlStyle);

            // Background
            using (var brush = new SolidBrush(Color.White))
            using (var path = CreateRoundedRectanglePath(rect, 4))
            {
                g.FillPath(brush, path);
            }

            // Border
            using (var pen = new Pen(colors.border, 1))
            using (var path = CreateRoundedRectanglePath(rect, 4))
            {
                g.DrawPath(pen, path);
            }

            // Text
            var textRect = new Rectangle(rect.X + 10, rect.Y, rect.Width - 28, rect.Height);
            using (var font = new Font("Segoe UI", 9f))
            using (var brush = new SolidBrush(colors.text))
            {
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                g.DrawString(text, font, brush, textRect, sf);
            }

            // Dropdown arrow
            int arrowX = rect.Right - 14;
            int arrowY = rect.Y + rect.Height / 2;
            using (var pen = new Pen(colors.text, 1.5f))
            {
                g.DrawLine(pen, arrowX - 4, arrowY - 2, arrowX, arrowY + 2);
                g.DrawLine(pen, arrowX, arrowY + 2, arrowX + 4, arrowY - 2);
            }
        }

        private void PaintSearchInput(Graphics g, Rectangle rect, string text, BeepFilter owner)
        {
            var colors = GetStyleColors(owner.ControlStyle);

            // Background
            using (var brush = new SolidBrush(Color.White))
            using (var path = CreateRoundedRectanglePath(rect, 4))
            {
                g.FillPath(brush, path);
            }

            // Border
            using (var pen = new Pen(colors.border, 1))
            using (var path = CreateRoundedRectanglePath(rect, 4))
            {
                g.DrawPath(pen, path);
            }

            // Search icon (left)
            int iconX = rect.X + 12;
            int iconY = rect.Y + rect.Height / 2;
            int iconSize = 14;
            
            using (var pen = new Pen(Color.FromArgb(150, colors.text), 1.5f))
            {
                // Circle
                g.DrawEllipse(pen, iconX - iconSize / 2 + 2, iconY - iconSize / 2 + 2, iconSize - 4, iconSize - 4);
                // Handle
                g.DrawLine(pen, iconX + iconSize / 2 - 2, iconY + iconSize / 2 - 2, iconX + iconSize / 2 + 1, iconY + iconSize / 2 + 1);
            }

            // Text or placeholder
            var displayText = string.IsNullOrEmpty(text) ? "Search..." : text;
            var textColor = string.IsNullOrEmpty(text) 
                ? Color.FromArgb(150, colors.text) 
                : colors.text;

            var textRect = new Rectangle(rect.X + 32, rect.Y, rect.Width - 48, rect.Height);
            using (var font = new Font("Segoe UI", 10f))
            using (var brush = new SolidBrush(textColor))
            {
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                g.DrawString(displayText, font, brush, textRect, sf);
            }

            // Clear button (X) if there's text
            if (!string.IsNullOrEmpty(text))
            {
                int clearX = rect.Right - 20;
                int clearY = rect.Y + rect.Height / 2;
                int clearSize = 12;

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
            var colors = GetStyleColors(owner.ControlStyle);

            // Background circle
            using (var brush = new SolidBrush(colors.accent))
            {
                g.FillEllipse(brush, rect);
            }

            // Text
            string countText = count > 99 ? "99+" : count.ToString();
            using (var font = new Font("Segoe UI", 8f, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };
                g.DrawString(countText, font, brush, rect, sf);
            }
        }

        /// <summary>Hit tests for search input, column selector, clear button, and badge.</summary>
        public override FilterHitArea? HitTest(Point point, FilterLayoutInfo layout)
        {
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

                // Column selector (left side)
                Rectangle columnRect = new Rectangle(
                    layout.SearchInputRect.X,
                    layout.SearchInputRect.Y,
                    ColumnSelectorWidth,
                    layout.SearchInputRect.Height
                );
                if (columnRect.Contains(point))
                    return new FilterHitArea { Name = "ColumnSelector", Bounds = columnRect, Type = FilterHitAreaType.FieldDropdown };

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
