using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Filtering.Painters
{
    /// <summary>
    /// Painter for SidebarPanel filter style - vertical sidebar with collapsible categories
    /// E-commerce/faceted search style with category sections
    /// Features: collapsible sections, checkboxes, count badges, Apply/Clear buttons
    /// </summary>
    public class SidebarPanelFilterPainter : BaseFilterPainter
    {
        private const int SectionHeaderHeight = 36;
        private const int SectionItemHeight = 28;
        private const int CheckboxSize = 16;
        private const int Padding = 12;
        private const int IndentWidth = 8;
        private const int ExpandToggleSize = 12;
        private const int BadgeSize = 20;
        private const int ButtonHeight = 36;
        private const int SectionSpacing = 4;

        /// <summary>Gets the filter style this painter implements.</summary>
        public override FilterStyle FilterStyle => FilterStyle.SidebarPanel;
        
        /// <summary>Gets whether this painter supports drag-drop reordering.</summary>
        public override bool SupportsDragDrop => false;

        /// <summary>Calculates layout positions for sidebar sections and items.</summary>
        public override FilterLayoutInfo CalculateLayout(BeepFilter owner, Rectangle availableRect)
        {
            var layout = new FilterLayoutInfo
            {
                ContainerRect = availableRect,
                ContentRect = availableRect
            };

            var config = owner.ActiveFilter;
            int currentY = availableRect.Y + Padding;
            int panelWidth = availableRect.Width - Padding * 2;

            var rowRects = new List<Rectangle>();

            // Group criteria by column name to create sections
            if (config != null && config.Criteria.Count > 0)
            {
                var sections = config.Criteria
                    .GroupBy(c => c.ColumnName)
                    .ToList();

                foreach (var section in sections)
                {
                    // Section header
                    Rectangle headerRect = new Rectangle(
                        availableRect.X + Padding,
                        currentY,
                        panelWidth,
                        SectionHeaderHeight
                    );
                    rowRects.Add(headerRect);
                    currentY += SectionHeaderHeight + SectionSpacing;

                    // Section items (filter values)
                    foreach (var criterion in section)
                    {
                        Rectangle itemRect = new Rectangle(
                            availableRect.X + Padding + IndentWidth,
                            currentY,
                            panelWidth - IndentWidth,
                            SectionItemHeight
                        );
                        rowRects.Add(itemRect);
                        currentY += SectionItemHeight + 2;
                    }

                    currentY += SectionSpacing;
                }

                layout.RowRects = rowRects.ToArray();
            }

            // Action buttons at bottom
            currentY = Math.Max(currentY, availableRect.Bottom - ButtonHeight * 2 - Padding * 3);
            
            layout.AddFilterButtonRect = new Rectangle(
                availableRect.X + Padding,
                availableRect.Bottom - ButtonHeight * 2 - Padding * 2,
                panelWidth,
                ButtonHeight
            );

            layout.AddGroupButtonRect = new Rectangle(
                availableRect.X + Padding,
                availableRect.Bottom - ButtonHeight - Padding,
                panelWidth,
                ButtonHeight
            );

            return layout;
        }

        /// <summary>Paints the sidebar panel filter UI.</summary>
        public override void Paint(Graphics g, BeepFilter owner, FilterLayoutInfo layout)
        {
            if (g == null || owner == null) return;

            var colors = GetStyleColors(owner.ControlStyle);
            var config = owner.ActiveFilter;

            // Panel background
            using (var brush = new SolidBrush(Color.FromArgb(250, colors.background)))
            {
                g.FillRectangle(brush, layout.ContainerRect);
            }

            // Panel border
            using (var pen = new Pen(colors.border, 1f))
            {
                g.DrawRectangle(pen, layout.ContainerRect);
            }

            // Paint sections and items
            if (config != null && config.Criteria.Count > 0)
            {
                var sections = config.Criteria
                    .GroupBy(c => c.ColumnName)
                    .ToList();

                int rectIndex = 0;
                foreach (var section in sections)
                {
                    if (rectIndex >= layout.RowRects.Length) break;

                    // Paint section header
                    int selectedCount = section.Count(c => c.IsEnabled);
                    PaintSectionHeader(g, layout.RowRects[rectIndex], section.Key, selectedCount, section.Count(), true, colors);
                    rectIndex++;

                    // Paint section items
                    foreach (var criterion in section)
                    {
                        if (rectIndex >= layout.RowRects.Length) break;
                        PaintSectionItem(g, layout.RowRects[rectIndex], criterion.Value?.ToString() ?? "", criterion.IsEnabled, colors);
                        rectIndex++;
                    }
                }
            }

            // Paint action buttons
            if (owner.ShowActionButtons)
            {
                if (layout.AddFilterButtonRect != Rectangle.Empty)
                {
                    PaintActionButton(g, layout.AddFilterButtonRect, "Apply Filters", colors, true);
                }

                if (layout.AddGroupButtonRect != Rectangle.Empty)
                {
                    PaintActionButton(g, layout.AddGroupButtonRect, "Clear All", colors, false);
                }
            }
        }

        private void PaintSectionHeader(
            Graphics g,
            Rectangle rect,
            string title,
            int selectedCount,
            int totalCount,
            bool isExpanded,
            (Color background, Color border, Color text, Color accent) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Background
            using (var brush = new SolidBrush(Color.FromArgb(245, colors.background)))
            {
                g.FillRectangle(brush, rect);
            }

            // Expand/collapse toggle
            int toggleX = rect.X + 8;
            int toggleY = rect.Y + (rect.Height - ExpandToggleSize) / 2;
            PaintExpandToggle(g, new Rectangle(toggleX, toggleY, ExpandToggleSize, ExpandToggleSize), isExpanded, colors);

            // Title text
            using (var font = new Font("Segoe UI", 9f, FontStyle.Bold))
            {
                Rectangle textRect = new Rectangle(
                    toggleX + ExpandToggleSize + 8,
                    rect.Y,
                    rect.Width - ExpandToggleSize - 70,
                    rect.Height
                );
                TextRenderer.DrawText(g, title, font, textRect, colors.text,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            // Count badge
            if (selectedCount > 0)
            {
                string badgeText = selectedCount.ToString();
                Rectangle badgeRect = new Rectangle(
                    rect.Right - BadgeSize - 8,
                    rect.Y + (rect.Height - BadgeSize) / 2,
                    BadgeSize,
                    BadgeSize
                );
                PaintCountBadge(g, badgeRect, badgeText, colors);
            }
        }

        private void PaintExpandToggle(Graphics g, Rectangle rect, bool isExpanded, (Color background, Color border, Color text, Color accent) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (var pen = new Pen(colors.text, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = rect.X + rect.Width / 2;
                int centerY = rect.Y + rect.Height / 2;

                if (isExpanded)
                {
                    // Down arrow
                    g.DrawLine(pen, centerX - 4, centerY - 2, centerX, centerY + 2);
                    g.DrawLine(pen, centerX, centerY + 2, centerX + 4, centerY - 2);
                }
                else
                {
                    // Right arrow
                    g.DrawLine(pen, centerX - 2, centerY - 4, centerX + 2, centerY);
                    g.DrawLine(pen, centerX + 2, centerY, centerX - 2, centerY + 4);
                }
            }
        }

        private void PaintSectionItem(
            Graphics g,
            Rectangle rect,
            string label,
            bool isChecked,
            (Color background, Color border, Color text, Color accent) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Checkbox
            Rectangle checkRect = new Rectangle(
                rect.X,
                rect.Y + (rect.Height - CheckboxSize) / 2,
                CheckboxSize,
                CheckboxSize
            );

            // Checkbox background
            Color checkBgColor = isChecked ? colors.accent : colors.background;
            using (var brush = new SolidBrush(checkBgColor))
            {
                g.FillRectangle(brush, checkRect);
            }

            // Checkbox border
            using (var pen = new Pen(isChecked ? colors.accent : colors.border, 1f))
            {
                g.DrawRectangle(pen, checkRect);
            }

            // Checkmark
            if (isChecked)
            {
                using (var pen = new Pen(Color.White, 1.5f))
                {
                    var points = new Point[]
                    {
                        new Point(checkRect.X + 3, checkRect.Y + CheckboxSize / 2),
                        new Point(checkRect.X + CheckboxSize / 2 - 1, checkRect.Y + CheckboxSize - 4),
                        new Point(checkRect.Right - 3, checkRect.Y + 3)
                    };
                    g.DrawLines(pen, points);
                }
            }

            // Label text
            using (var font = new Font("Segoe UI", 8.5f))
            {
                Rectangle textRect = new Rectangle(
                    checkRect.Right + 8,
                    rect.Y,
                    rect.Width - CheckboxSize - 8,
                    rect.Height
                );
                TextRenderer.DrawText(g, label, font, textRect, colors.text,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        private void PaintCountBadge(Graphics g, Rectangle rect, string count, (Color background, Color border, Color text, Color accent) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Circle background
            using (var brush = new SolidBrush(colors.accent))
            {
                g.FillEllipse(brush, rect);
            }

            // Count text
            using (var font = new Font("Segoe UI", 7.5f, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, count, font, rect, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        private void PaintActionButton(
            Graphics g,
            Rectangle rect,
            string text,
            (Color background, Color border, Color text, Color accent) colors,
            bool isPrimary)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            Color bgColor = isPrimary ? colors.accent : Color.FromArgb(240, colors.background);
            Color textColor = isPrimary ? Color.White : colors.text;

            // Background
            using (var brush = new SolidBrush(bgColor))
            {
                using (var path = CreateRoundedRectanglePath(rect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            // Border
            if (!isPrimary)
            {
                using (var pen = new Pen(colors.border, 1f))
                {
                    using (var path = CreateRoundedRectanglePath(rect, 4))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Text
            using (var font = new Font("Segoe UI", 9f, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, text, font, rect, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        /// <summary>Hit tests for section headers, checkboxes, and action buttons.</summary>
        public override FilterHitArea? HitTest(Point point, FilterLayoutInfo layout)
        {
            // Check action buttons at bottom
            if (layout.AddFilterButtonRect.Contains(point))
                return new FilterHitArea { Name = "ApplyFilters", Bounds = layout.AddFilterButtonRect, Type = FilterHitAreaType.ApplyButton };

            if (layout.AddGroupButtonRect.Contains(point))
                return new FilterHitArea { Name = "ClearAll", Bounds = layout.AddGroupButtonRect, Type = FilterHitAreaType.ClearAllButton };

            // Check sections and items
            // We need to track whether we're in a header or item row
            // This requires the painter to maintain state about section structure
            // For now, we'll check all rows generically
            for (int i = 0; i < layout.RowRects.Length; i++)
            {
                Rectangle rowRect = layout.RowRects[i];
                
                if (rowRect.Height == SectionHeaderHeight)
                {
                    // Section header - check expand toggle
                    Rectangle toggleRect = new Rectangle(rowRect.X + 8, rowRect.Y + (rowRect.Height - ExpandToggleSize) / 2, ExpandToggleSize, ExpandToggleSize);
                    if (toggleRect.Contains(point))
                        return new FilterHitArea { Name = $"SectionToggle_{i}", Bounds = toggleRect, Type = FilterHitAreaType.CollapseButton, Tag = i };

                    // Check count badge
                    Rectangle badgeRect = new Rectangle(rowRect.Right - BadgeSize - 8, rowRect.Y + (rowRect.Height - BadgeSize) / 2, BadgeSize, BadgeSize);
                    if (badgeRect.Contains(point))
                        return new FilterHitArea { Name = $"SectionBadge_{i}", Bounds = badgeRect, Type = FilterHitAreaType.FilterTag, Tag = i };

                    // Entire header
                    if (rowRect.Contains(point))
                        return new FilterHitArea { Name = $"SectionHeader_{i}", Bounds = rowRect, Type = FilterHitAreaType.CollapseButton, Tag = i };
                }
                else if (rowRect.Height == SectionItemHeight)
                {
                    // Section item - check checkbox
                    Rectangle checkRect = new Rectangle(rowRect.X, rowRect.Y + (rowRect.Height - CheckboxSize) / 2, CheckboxSize, CheckboxSize);
                    if (checkRect.Contains(point))
                        return new FilterHitArea { Name = $"Checkbox_{i}", Bounds = checkRect, Type = FilterHitAreaType.ValueInput, Tag = i };

                    // Entire item
                    if (rowRect.Contains(point))
                        return new FilterHitArea { Name = $"Item_{i}", Bounds = rowRect, Type = FilterHitAreaType.ValueInput, Tag = i };
                }
            }

            return null;
        }
    }
}
