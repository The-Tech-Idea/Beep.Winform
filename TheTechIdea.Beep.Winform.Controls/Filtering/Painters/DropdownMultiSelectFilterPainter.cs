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
    /// Painter for DropdownMultiSelect filter style - checkbox list with search
    /// Based on sample image 5 - shows dropdown with searchable checkbox list
    /// Features: (All) option, search box, selected count display, Clear/Close buttons
    /// </summary>
    public class DropdownMultiSelectFilterPainter : BaseFilterPainter
    {
        private const int DropdownButtonHeight = 36;
        private const int SearchBoxHeight = 32;
        private const int CheckboxItemHeight = 28;
        private const int CheckboxSize = 16;
        private const int ItemPadding = 8;
        private const int MaxVisibleItems = 10;
        private const int ButtonHeight = 32;
        private const int DropdownWidth = 280;

        /// <summary>Gets the filter style this painter implements.</summary>
        public override FilterStyle FilterStyle => FilterStyle.DropdownMultiSelect;
        
        /// <summary>Gets whether this painter supports drag-drop reordering.</summary>
        public override bool SupportsDragDrop => false;

        /// <summary>Calculates layout positions for dropdown and expanded list.</summary>
        public override FilterLayoutInfo CalculateLayout(BeepFilter owner, Rectangle availableRect)
        {
            var layout = new FilterLayoutInfo
            {
                ContainerRect = availableRect,
                ContentRect = availableRect
            };

            var config = owner.ActiveFilter;
            int padding = 8;

            // Collapsed dropdown button
            layout.AddFilterButtonRect = new Rectangle(
                availableRect.X + padding,
                availableRect.Y + padding,
                DropdownWidth,
                DropdownButtonHeight
            );

            // If expanded, show the dropdown panel
            // For now, we'll show it as collapsed by default
            // The expanded state would be handled by the control's state

            return layout;
        }

        /// <summary>Paints the dropdown multi-select UI.</summary>
        public override void Paint(Graphics g, BeepFilter owner, FilterLayoutInfo layout)
        {
            if (g == null || owner == null) return;

            var colors = GetStyleColors(owner.ControlStyle);
            var config = owner.ActiveFilter;

            // Paint collapsed dropdown button
            if (layout.AddFilterButtonRect != Rectangle.Empty)
            {
                PaintDropdownButton(g, layout.AddFilterButtonRect, config, colors, owner);
            }

            // TODO: Paint expanded dropdown panel when control state indicates it's open
            // This would show the search box, checkbox list, and action buttons

            // Phase 1: Paint filter count badge (inside dropdown button)
            if (owner.ShowFilterCountBadge && config.Criteria.Count > 0)
            {
                var badgeLocation = new Point(
                    layout.AddFilterButtonRect.Right - 35,
                    layout.AddFilterButtonRect.Y + (layout.AddFilterButtonRect.Height - 20) / 2
                );
                var accentColor = owner._currentTheme?.AccentColor ?? Color.FromArgb(33, 150, 243);
                PaintFilterCountBadge(g, config.Criteria.Count, badgeLocation, accentColor);
            }
        }

        private void PaintDropdownButton(
            Graphics g,
            Rectangle rect,
            FilterConfiguration config,
            (Color BackColor, Color ForeColor, Color BorderColor, Color AccentColor) colors,
            BeepFilter owner)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Background
            using (var brush = new SolidBrush(colors.BackColor))
            {
                using (var path = CreateRoundedRectanglePath(rect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            // Border
            using (var pen = new Pen(colors.BorderColor, 1f))
            {
                using (var path = CreateRoundedRectanglePath(rect, 4))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Text showing selected count
            string displayText = GetDisplayText(config);
            using (var font = new Font("Segoe UI", 9f))
            {
                Rectangle textRect = new Rectangle(rect.X + ItemPadding, rect.Y, rect.Width - 40, rect.Height);
                TextRenderer.DrawText(g, displayText, font, textRect, colors.ForeColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            // Dropdown arrow
            int arrowSize = 8;
            int arrowX = rect.Right - arrowSize - 12;
            int arrowY = rect.Y + rect.Height / 2;
            
            using (var pen = new Pen(colors.ForeColor, 1.5f))
            {
                g.DrawLine(pen, arrowX - arrowSize / 2, arrowY - 2, arrowX, arrowY + 3);
                g.DrawLine(pen, arrowX, arrowY + 3, arrowX + arrowSize / 2, arrowY - 2);
            }
        }

        /// <summary>
        /// Paints the expanded dropdown panel with search and checkboxes.
        /// This would be called when the dropdown is in expanded state.
        /// </summary>
        public void PaintExpandedPanel(
            Graphics g,
            Rectangle panelRect,
            FilterConfiguration config,
            string[] availableValues,
            HashSet<string> selectedValues,
            string searchText,
            (Color BackColor, Color ForeColor, Color BorderColor, Color AccentColor) colors)
        {
            if (g == null || panelRect.Width <= 0 || panelRect.Height <= 0) return;

            // Panel background with shadow
            using (var shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
            {
                Rectangle shadowRect = panelRect;
                shadowRect.Offset(2, 2);
                g.FillRectangle(shadowBrush, shadowRect);
            }

            // Panel background
            using (var brush = new SolidBrush(colors.BackColor))
            {
                g.FillRectangle(brush, panelRect);
            }

            // Panel border
            using (var pen = new Pen(colors.BorderColor, 1f))
            {
                g.DrawRectangle(pen, panelRect);
            }

            int currentY = panelRect.Y + ItemPadding;

            // Search box
            Rectangle searchRect = new Rectangle(
                panelRect.X + ItemPadding,
                currentY,
                panelRect.Width - ItemPadding * 2,
                SearchBoxHeight
            );
            PaintSearchBox(g, searchRect, searchText, colors);
            currentY += SearchBoxHeight + ItemPadding;

            // "(All)" checkbox
            Rectangle allCheckRect = new Rectangle(
                panelRect.X + ItemPadding,
                currentY,
                panelRect.Width - ItemPadding * 2,
                CheckboxItemHeight
            );
            bool allSelected = availableValues.Length > 0 && availableValues.All(v => selectedValues.Contains(v));
            PaintCheckboxItem(g, allCheckRect, "(All)", allSelected, colors, true);
            currentY += CheckboxItemHeight + 2;

            // Separator
            using (var pen = new Pen(Color.FromArgb(200, colors.BorderColor), 1f))
            {
                g.DrawLine(pen, panelRect.X + ItemPadding, currentY, panelRect.Right - ItemPadding, currentY);
            }
            currentY += ItemPadding;

            // Filter values based on search
            var filteredValues = string.IsNullOrEmpty(searchText)
                ? availableValues
                : availableValues.Where(v => v.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();

            // Checkbox list (with scrolling support)
            int maxItems = Math.Min(filteredValues.Length, MaxVisibleItems);
            for (int i = 0; i < maxItems; i++)
            {
                string value = filteredValues[i];
                Rectangle itemRect = new Rectangle(
                    panelRect.X + ItemPadding,
                    currentY,
                    panelRect.Width - ItemPadding * 2,
                    CheckboxItemHeight
                );
                PaintCheckboxItem(g, itemRect, value, selectedValues.Contains(value), colors, false);
                currentY += CheckboxItemHeight + 2;
            }

            // Show "X more items" if there are more
            if (filteredValues.Length > MaxVisibleItems)
            {
                currentY += 4;
                string moreText = $"+ {filteredValues.Length - MaxVisibleItems} more items...";
                using (var font = new Font("Segoe UI", 8f, FontStyle.Italic))
                {
                    Rectangle moreRect = new Rectangle(panelRect.X + ItemPadding, currentY, panelRect.Width - ItemPadding * 2, 20);
                    TextRenderer.DrawText(g, moreText, font, moreRect, Color.FromArgb(150, colors.ForeColor),
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                }
                currentY += 20 + ItemPadding;
            }
            else
            {
                currentY += ItemPadding;
            }

            // Action buttons at bottom
            int buttonWidth = (panelRect.Width - ItemPadding * 3) / 2;
            Rectangle clearButtonRect = new Rectangle(
                panelRect.X + ItemPadding,
                panelRect.Bottom - ButtonHeight - ItemPadding,
                buttonWidth,
                ButtonHeight
            );
            Rectangle closeButtonRect = new Rectangle(
                clearButtonRect.Right + ItemPadding,
                panelRect.Bottom - ButtonHeight - ItemPadding,
                buttonWidth,
                ButtonHeight
            );

            PaintButton(g, clearButtonRect, "Clear Filter", colors, false);
            PaintButton(g, closeButtonRect, "Close", colors, true);
        }

        private void PaintSearchBox(Graphics g, Rectangle rect, string searchText, (Color BackColor, Color ForeColor, Color BorderColor, Color AccentColor) colors)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Background
            using (var brush = new SolidBrush(Color.FromArgb(250, colors.BackColor)))
            {
                g.FillRectangle(brush, rect);
            }

            // Border
            using (var pen = new Pen(colors.BorderColor, 1f))
            {
                g.DrawRectangle(pen, rect);
            }

            // Search icon
            int iconSize = 14;
            Rectangle iconRect = new Rectangle(rect.X + 8, rect.Y + (rect.Height - iconSize) / 2, iconSize, iconSize);
            using (var pen = new Pen(Color.FromArgb(150, colors.ForeColor), 1.5f))
            {
                // Circle
                g.DrawEllipse(pen, iconRect.X, iconRect.Y, iconSize - 4, iconSize - 4);
                // Handle
                g.DrawLine(pen, iconRect.Right - 3, iconRect.Bottom - 3, iconRect.Right, iconRect.Bottom);
            }

            // Text
            string displayText = string.IsNullOrEmpty(searchText) ? "Search..." : searchText;
            Color textColor = string.IsNullOrEmpty(searchText) ? Color.FromArgb(150, colors.ForeColor) : colors.ForeColor;
            
            using (var font = new Font("Segoe UI", 8.5f))
            {
                Rectangle textRect = new Rectangle(rect.X + iconSize + 16, rect.Y, rect.Width - iconSize - 24, rect.Height);
                TextRenderer.DrawText(g, displayText, font, textRect, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        private void PaintCheckboxItem(Graphics g, Rectangle rect, string text, bool isChecked, (Color BackColor, Color ForeColor, Color BorderColor, Color AccentColor) colors, bool isBold)
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
            Color checkBgColor = isChecked ? colors.AccentColor : colors.BackColor;
            using (var brush = new SolidBrush(checkBgColor))
            {
                g.FillRectangle(brush, checkRect);
            }

            // Checkbox border
            using (var pen = new Pen(isChecked ? colors.AccentColor : colors.BorderColor, 1f))
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

            // Text label
            FontStyle fontStyle = isBold ? FontStyle.Bold : FontStyle.Regular;
            using (var font = new Font("Segoe UI", 8.5f, fontStyle))
            {
                Rectangle textRect = new Rectangle(checkRect.Right + 8, rect.Y, rect.Width - CheckboxSize - 8, rect.Height);
                TextRenderer.DrawText(g, text, font, textRect, colors.ForeColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        private void PaintButton(Graphics g, Rectangle rect, string text, (Color BackColor, Color ForeColor, Color BorderColor, Color AccentColor) colors, bool isPrimary)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            Color bgColor = isPrimary ? colors.AccentColor : Color.FromArgb(240, colors.BackColor);
            Color textColor = isPrimary ? Color.White : colors.ForeColor;

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
                using (var pen = new Pen(colors.BorderColor, 1f))
                {
                    using (var path = CreateRoundedRectanglePath(rect, 4))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Text
            using (var font = new Font("Segoe UI", 8.5f, FontStyle.Regular))
            {
                TextRenderer.DrawText(g, text, font, rect, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        private string GetDisplayText(FilterConfiguration config)
        {
            if (config == null || config.Criteria.Count == 0)
                return "Select values...";

            int selectedCount = config.Criteria.Count;
            if (selectedCount == 1)
                return config.Criteria[0].Value?.ToString() ?? "1 selected";

            return $"{selectedCount} selected";
        }

        /// <summary>Hit tests for dropdown button and expanded panel elements.</summary>
        public override FilterHitArea? HitTest(Point point, FilterLayoutInfo layout)
        {
            // Check dropdown button
            if (layout.AddFilterButtonRect.Contains(point))
            {
                // Check dropdown arrow area on right
                Rectangle arrowRect = new Rectangle(
                    layout.AddFilterButtonRect.Right - 30,
                    layout.AddFilterButtonRect.Y,
                    30,
                    layout.AddFilterButtonRect.Height
                );
                if (arrowRect.Contains(point))
                    return new FilterHitArea { Name = "DropdownArrow", Bounds = arrowRect, Type = FilterHitAreaType.CollapseButton };

                return new FilterHitArea { Name = "DropdownButton", Bounds = layout.AddFilterButtonRect, Type = FilterHitAreaType.FieldDropdown };
            }

            // TODO: When expanded panel is implemented, add hit testing for:
            // - Search box
            // - (All) checkbox
            // - Individual value checkboxes
            // - Clear Filter button
            // - Close button

            return null;
        }
    }
}
