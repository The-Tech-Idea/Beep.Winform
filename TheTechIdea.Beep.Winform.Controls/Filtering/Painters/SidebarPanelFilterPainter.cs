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

            layout.DpiScale = Helpers.DpiScalingHelper.GetDpiScaleFactor(owner);

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
                        Helpers.DpiScalingHelper.ScaleValue(SectionHeaderHeight, owner)
                    );
                    rowRects.Add(headerRect);
                    currentY += Helpers.DpiScalingHelper.ScaleValue(SectionHeaderHeight, owner) + SectionSpacing;

                    // Section items (filter values)
                    foreach (var criterion in section)
                    {
                        Rectangle itemRect = new Rectangle(
                            availableRect.X + Padding + IndentWidth,
                            currentY,
                            panelWidth - IndentWidth,
                            Helpers.DpiScalingHelper.ScaleValue(SectionItemHeight, owner)
                        );
                        rowRects.Add(itemRect);
                        currentY += Helpers.DpiScalingHelper.ScaleValue(SectionItemHeight, owner) + 2;
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

            var colors = GetStyleColors(owner, owner.ControlStyle);
            var config = owner.ActiveFilter;

            // Panel background
            g.FillRectangle(GetBrush(Color.FromArgb(250, colors.background)), layout.ContainerRect);

            // Panel border
            g.DrawRectangle(GetPen(colors.border, Helpers.DpiScalingHelper.ScaleValue(1, owner)), layout.ContainerRect);

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
                    PaintSectionHeader(g, layout.RowRects[rectIndex], section.Key, selectedCount, section.Count(), true, colors, owner);
                    rectIndex++;

                    // Paint section items
                    foreach (var criterion in section)
                    {
                        if (rectIndex >= layout.RowRects.Length) break;
                        PaintSectionItem(g, layout.RowRects[rectIndex], criterion.Value?.ToString() ?? "", criterion.IsEnabled, colors, owner);
                        rectIndex++;
                    }
                }
            }

            // Paint action buttons
            if (owner.ShowActionButtons)
            {
                if (layout.AddFilterButtonRect != Rectangle.Empty)
                {
                    PaintActionButton(g, layout.AddFilterButtonRect, "Apply Filters", colors, true, owner);
                }

                if (layout.AddGroupButtonRect != Rectangle.Empty)
                {
                    PaintActionButton(g, layout.AddGroupButtonRect, "Clear All", colors, false, owner);
                }
            }

            // Phase 1: Paint filter count badge (top of sidebar)
            if (owner.ShowFilterCountBadge && config != null && config.Criteria.Count > 0)
            {
                var badgeLocation = new Point(
                    layout.ContainerRect.Right - Helpers.DpiScalingHelper.ScaleValue(40, owner),
                    layout.ContainerRect.Top + Helpers.DpiScalingHelper.ScaleValue(12, owner)
                );
                var accentColor = owner._currentTheme?.AccentColor ?? Color.FromArgb(33, 150, 243);
                PaintFilterCountBadge(g, config.Criteria.Count, badgeLocation, accentColor, owner);
            }
        }

        private void PaintSectionHeader(
            Graphics g,
            Rectangle rect,
            string title,
            int selectedCount,
            int totalCount,
            bool isExpanded,
            (Color background, Color border, Color text, Color accent) colors,
            BeepFilter owner)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Background
            g.FillRectangle(GetBrush(Color.FromArgb(245, colors.background)), rect);

            // Expand/collapse toggle
            int toggleX = rect.X + 8;
            int toggleY = rect.Y + (rect.Height - Helpers.DpiScalingHelper.ScaleValue(ExpandToggleSize, owner)) / 2;
            PaintExpandToggle(g, new Rectangle(toggleX, toggleY, Helpers.DpiScalingHelper.ScaleValue(ExpandToggleSize, owner), Helpers.DpiScalingHelper.ScaleValue(ExpandToggleSize, owner)), isExpanded, colors, owner);

            // Title text
            {
                var font = GetFont(9f, FontStyle.Bold);
                Rectangle textRect = new Rectangle(
                    toggleX + Helpers.DpiScalingHelper.ScaleValue(ExpandToggleSize, owner) + 8,
                    rect.Y,
                    rect.Width - Helpers.DpiScalingHelper.ScaleValue(ExpandToggleSize, owner) - 70,
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
                    rect.Right - Helpers.DpiScalingHelper.ScaleValue(BadgeSize, owner) - 8,
                    rect.Y + (rect.Height - Helpers.DpiScalingHelper.ScaleValue(BadgeSize, owner)) / 2,
                    Helpers.DpiScalingHelper.ScaleValue(BadgeSize, owner),
                    Helpers.DpiScalingHelper.ScaleValue(BadgeSize, owner)
                );
                PaintCountBadge(g, badgeRect, badgeText, colors);
            }
        }

        private void PaintExpandToggle(Graphics g, Rectangle rect, bool isExpanded, (Color background, Color border, Color text, Color accent) colors, BeepFilter owner)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (var pen = new Pen(colors.text, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = rect.X + rect.Width / 2;
                int centerY = rect.Y + rect.Height / 2;

                int a = Helpers.DpiScalingHelper.ScaleValue(4, owner);
                int b = Helpers.DpiScalingHelper.ScaleValue(2, owner);

                if (isExpanded)
                {
                    // Down arrow
                    g.DrawLine(pen, centerX - a, centerY - b, centerX, centerY + b);
                    g.DrawLine(pen, centerX, centerY + b, centerX + a, centerY - b);
                }
                else
                {
                    // Right arrow
                    g.DrawLine(pen, centerX - b, centerY - a, centerX + b, centerY);
                    g.DrawLine(pen, centerX + b, centerY, centerX - b, centerY + a);
                }
            }
        }

        private void PaintSectionItem(
            Graphics g,
            Rectangle rect,
            string label,
            bool isChecked,
            (Color background, Color border, Color text, Color accent) colors,
            BeepFilter owner)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Checkbox
            Rectangle checkRect = new Rectangle(
                rect.X,
                rect.Y + (rect.Height - Helpers.DpiScalingHelper.ScaleValue(CheckboxSize, owner)) / 2,
                Helpers.DpiScalingHelper.ScaleValue(CheckboxSize, owner),
                Helpers.DpiScalingHelper.ScaleValue(CheckboxSize, owner)
            );

            // Checkbox background
            Color checkBgColor = isChecked ? colors.accent : colors.background;
            g.FillRectangle(GetBrush(checkBgColor), checkRect);

            // Checkbox border
            g.DrawRectangle(GetPen(isChecked ? colors.accent : colors.border, Helpers.DpiScalingHelper.ScaleValue(1, owner)), checkRect);

            // Checkmark
            if (isChecked)
            {
                var points = new Point[]
                {
                    new Point(checkRect.X + 3, checkRect.Y + Helpers.DpiScalingHelper.ScaleValue(CheckboxSize, owner) / 2),
                    new Point(checkRect.X + Helpers.DpiScalingHelper.ScaleValue(CheckboxSize, owner) / 2 - 1, checkRect.Y + Helpers.DpiScalingHelper.ScaleValue(CheckboxSize, owner) - 4),
                    new Point(checkRect.Right - 3, checkRect.Y + 3)
                };
                g.DrawLines(GetPen(Color.White, 1.5f), points);
            }

            // Label text
            {
                var font = GetFont(8.5f);
                int labelGap = Helpers.DpiScalingHelper.ScaleValue(8, owner);
                Rectangle textRect = new Rectangle(
                    checkRect.Right + labelGap,
                    rect.Y,
                    rect.Width - Helpers.DpiScalingHelper.ScaleValue(CheckboxSize, owner) - labelGap,
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
            g.FillEllipse(GetBrush(colors.accent), rect);

            // Count text
            {
                var font = GetFont(7.5f, FontStyle.Bold);
                TextRenderer.DrawText(g, count, font, rect, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        private void PaintActionButton(
            Graphics g,
            Rectangle rect,
            string text,
            (Color background, Color border, Color text, Color accent) colors,
            bool isPrimary,
            BeepFilter owner)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            Color bgColor = isPrimary ? colors.accent : Color.FromArgb(240, colors.background);
            Color textColor = isPrimary ? Color.White : colors.text;

            int cr = Helpers.DpiScalingHelper.ScaleValue(4, owner);

            // Background
            using (var path = CreateRoundedRectanglePath(rect, cr))
            {
                g.FillPath(GetBrush(bgColor), path);
            }

            // Border
            if (!isPrimary)
            {
                using (var path = CreateRoundedRectanglePath(rect, cr))
                {
                    g.DrawPath(GetPen(colors.border, Helpers.DpiScalingHelper.ScaleValue(1, owner)), path);
                }
            }

            // Text
            {
                var font = GetFont(9f, FontStyle.Bold);
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
                
                if (rowRect.Height == Helpers.DpiScalingHelper.ScaleValue(SectionHeaderHeight, layout.DpiScale))
                {
                    // Section header - check expand toggle
                    Rectangle toggleRect = new Rectangle(rowRect.X + 8, rowRect.Y + (rowRect.Height - Helpers.DpiScalingHelper.ScaleValue(ExpandToggleSize, layout.DpiScale)) / 2, Helpers.DpiScalingHelper.ScaleValue(ExpandToggleSize, layout.DpiScale), Helpers.DpiScalingHelper.ScaleValue(ExpandToggleSize, layout.DpiScale));
                    if (toggleRect.Contains(point))
                        return new FilterHitArea { Name = $"SectionToggle_{i}", Bounds = toggleRect, Type = FilterHitAreaType.CollapseButton, Tag = i };

                    // Check count badge
                    Rectangle badgeRect = new Rectangle(rowRect.Right - Helpers.DpiScalingHelper.ScaleValue(BadgeSize, layout.DpiScale) - 8, rowRect.Y + (rowRect.Height - Helpers.DpiScalingHelper.ScaleValue(BadgeSize, layout.DpiScale)) / 2, Helpers.DpiScalingHelper.ScaleValue(BadgeSize, layout.DpiScale), Helpers.DpiScalingHelper.ScaleValue(BadgeSize, layout.DpiScale));
                    if (badgeRect.Contains(point))
                        return new FilterHitArea { Name = $"SectionBadge_{i}", Bounds = badgeRect, Type = FilterHitAreaType.FilterTag, Tag = i };

                    // Entire header
                    if (rowRect.Contains(point))
                        return new FilterHitArea { Name = $"SectionHeader_{i}", Bounds = rowRect, Type = FilterHitAreaType.CollapseButton, Tag = i };
                }
                else if (rowRect.Height == Helpers.DpiScalingHelper.ScaleValue(SectionItemHeight, layout.DpiScale))
                {
                    // Section item - check checkbox
                    Rectangle checkRect = new Rectangle(rowRect.X, rowRect.Y + (rowRect.Height - Helpers.DpiScalingHelper.ScaleValue(CheckboxSize, layout.DpiScale)) / 2, Helpers.DpiScalingHelper.ScaleValue(CheckboxSize, layout.DpiScale), Helpers.DpiScalingHelper.ScaleValue(CheckboxSize, layout.DpiScale));
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
