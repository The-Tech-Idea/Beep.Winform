using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Toolbar
{
    /// <summary>
    /// Represents a single toolbar button with layout metadata.
    /// </summary>
    public class ToolbarButtonItem
    {
        public string Key { get; set; }
        public string IconPath { get; set; }
        public string Label { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsOverflow { get; set; }
        public Rectangle Bounds { get; set; }
        public bool IsSeparator { get; set; }
    }

    /// <summary>
    /// Enhanced state model for the unified grid toolbar with responsive layout,
    /// text labels for primary actions, and overflow menu support.
    /// </summary>
    public class BeepGridToolbarState
    {
        public string SearchText { get; set; } = string.Empty;
        public bool SearchHasFocus { get; set; }
        public int ActiveFilterCount { get; set; }
        public bool IsFilterActive { get; set; }
        public float DpiScale { get; set; } = 1f;

        // Grid title
        public string GridTitle { get; set; } = "Grid";
        public bool ShowGridTitle { get; set; } = true;

        // Section rects
        public Rectangle TitleSectionRect { get; private set; }
        public Rectangle ActionsSectionRect { get; private set; }
        public Rectangle SearchSectionRect { get; private set; }
        public Rectangle FilterSectionRect { get; private set; }
        public Rectangle ExportSectionRect { get; private set; }
        public Rectangle OverflowButtonRect { get; private set; }

        // Element rects
        public Rectangle SearchIconRect { get; private set; }
        public Rectangle SearchBoxRect { get; private set; }
        public Rectangle FilterButtonRect { get; private set; }
        public Rectangle AdvancedButtonRect { get; private set; }
        public Rectangle ClearFilterRect { get; private set; }
        public Rectangle BadgeRect { get; private set; }

        // Button items for responsive layout
        public List<ToolbarButtonItem> ActionButtons { get; } = new();
        public List<ToolbarButtonItem> ExportButtons { get; } = new();

        // Separator positions
        public int Separator1X { get; private set; }
        public int Separator2X { get; private set; }
        public int Separator3X { get; private set; }

        // Hover/pressed tracking
        public string? HoveredButtonKey { get; set; }
        public string? PressedButtonKey { get; set; }

        // Overflow state
        public bool HasOverflowItems => ExportButtons.Exists(b => b.IsOverflow);

        public void CalculateLayout(Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                ResetLayout();
                return;
            }

            int margin = (int)(8 * DpiScale);
            int iconSize = (int)(18 * DpiScale);
            int buttonGap = (int)(4 * DpiScale);
            int height = (int)(32 * DpiScale);
            int y = bounds.Top + (bounds.Height - height) / 2;
            int separatorWidth = (int)(1 * DpiScale);

            // Build button lists
            BuildButtonLists(iconSize, height, y);

            int x = bounds.Left + margin;

            // === TITLE SECTION (optional, left-most) ===
            if (ShowGridTitle && !string.IsNullOrEmpty(GridTitle))
            {
                var titleFont = new Font(SystemFonts.DefaultFont.FontFamily, 9.5f, FontStyle.Bold);
                try
                {
                    var titleSize = TextRenderer.MeasureText(GridTitle, titleFont);
                    int titleWidth = Math.Min(titleSize.Width + margin, bounds.Width / 4);
                    TitleSectionRect = new Rectangle(x, y, titleWidth, height);
                    x += titleWidth + margin;
                }
                finally { titleFont.Dispose(); }
            }
            else
            {
                TitleSectionRect = Rectangle.Empty;
            }

            // === ACTIONS SECTION (text labels for primary CRUD) ===
            int actionsStartX = x;
            CalculateButtonLayout(ActionButtons, ref x, y, iconSize, height, buttonGap, bounds.Width, margin, true);
            ActionsSectionRect = new Rectangle(actionsStartX, y - (height - iconSize) / 2, x - actionsStartX, height);

            // === SEARCH SECTION (flexible, fills remaining space) ===
            int searchIconX = x;
            SearchIconRect = new Rectangle(searchIconX, y, iconSize, iconSize);
            x += iconSize + (int)(4 * DpiScale);

            // Reserve space for filter + export sections
            int filterSectionWidth = iconSize * 2 + buttonGap * 2 + margin * 2 + (int)(16 * DpiScale); // badge
            int exportVisibleCount = ExportButtons.Count(b => !b.IsOverflow);
            int exportSectionWidth = exportVisibleCount * (iconSize + buttonGap) + margin;
            int overflowWidth = HasOverflowItems ? (iconSize + margin) : 0;
            int reservedRight = filterSectionWidth + exportSectionWidth + overflowWidth + separatorWidth * 2 + margin;

            int searchWidth = bounds.Right - x - reservedRight;
            searchWidth = Math.Max(100, searchWidth); // minimum 100px for search box

            SearchBoxRect = new Rectangle(x, y, searchWidth, height);
            x = SearchBoxRect.Right + margin;
            SearchSectionRect = new Rectangle(ActionsSectionRect.Right, y - (height - iconSize) / 2, x - ActionsSectionRect.Right, height);

            // === FILTER SECTION ===
            x += separatorWidth;
            Separator1X = x - separatorWidth;
            FilterButtonRect = new Rectangle(x, y, iconSize, iconSize);
            x += iconSize + buttonGap;
            AdvancedButtonRect = new Rectangle(x, y, iconSize, iconSize);
            x += iconSize + buttonGap;

            BadgeRect = new Rectangle(FilterButtonRect.Right - 4, y - 6, (int)(16 * DpiScale), (int)(16 * DpiScale));
            ClearFilterRect = new Rectangle(AdvancedButtonRect.Right + buttonGap, y, iconSize, iconSize);

            FilterSectionRect = new Rectangle(SearchSectionRect.Right, y - (height - iconSize) / 2, x - SearchSectionRect.Right, height);

            // === EXPORT SECTION (right) ===
            x += separatorWidth;
            Separator2X = x - separatorWidth;

            int exportStartX = x;
            foreach (var btn in ExportButtons)
            {
                if (!btn.IsVisible) continue;
                if (btn.IsOverflow) continue; // hidden in overflow
                btn.Bounds = new Rectangle(x, y, iconSize, iconSize);
                x += iconSize + buttonGap;
            }
            ExportSectionRect = new Rectangle(exportStartX, y - (height - iconSize) / 2, x - exportStartX, height);

            // === OVERFLOW BUTTON ===
            if (HasOverflowItems)
            {
                Separator3X = x;
                x += separatorWidth;
                OverflowButtonRect = new Rectangle(x, y, iconSize, iconSize);
            }
            else
            {
                OverflowButtonRect = Rectangle.Empty;
            }
        }

        private void BuildButtonLists(int iconSize, int height, int y)
        {
            // Primary action buttons with text labels
            ActionButtons.Clear();
            ActionButtons.Add(new ToolbarButtonItem { Key = "add", IconPath = "plus", Label = "New", IsVisible = true });
            ActionButtons.Add(new ToolbarButtonItem { Key = "edit", IconPath = "edit", Label = "Edit", IsVisible = true });
            ActionButtons.Add(new ToolbarButtonItem { Key = "delete", IconPath = "trash", Label = "Delete", IsVisible = true });

            // Export buttons (icon-only, overflow-capable)
            ExportButtons.Clear();
            ExportButtons.Add(new ToolbarButtonItem { Key = "import", IconPath = "file_upload", Label = "Import", IsVisible = true });
            ExportButtons.Add(new ToolbarButtonItem { Key = "export", IconPath = "download", Label = "Export", IsVisible = true });
            ExportButtons.Add(new ToolbarButtonItem { Key = "print", IconPath = "print", Label = "Print", IsVisible = true });
        }

        private void CalculateButtonLayout(List<ToolbarButtonItem> buttons, ref int x, int y, int iconSize, int height, int buttonGap, int totalWidth, int margin, bool showLabels)
        {
            foreach (var btn in buttons)
            {
                if (!btn.IsVisible) continue;

                int btnWidth;
                if (showLabels && !string.IsNullOrEmpty(btn.Label))
                {
                    // Measure text width for label
                    var labelSize = TextRenderer.MeasureText(btn.Label, SystemFonts.DefaultFont);
                    btnWidth = iconSize + (int)(6 * DpiScale) + labelSize.Width + (int)(8 * DpiScale);
                }
                else
                {
                    btnWidth = iconSize;
                }

                // Check if button fits; if not, mark as overflow
                if (x + btnWidth > totalWidth - margin * 2)
                {
                    btn.IsOverflow = true;
                    btn.Bounds = Rectangle.Empty;
                    continue;
                }

                btn.IsOverflow = false;
                btn.Bounds = new Rectangle(x, y, btnWidth, height);
                x += btnWidth + buttonGap;
            }
        }

        private void ResetLayout()
        {
            TitleSectionRect = ActionsSectionRect = SearchSectionRect = FilterSectionRect = ExportSectionRect = Rectangle.Empty;
            OverflowButtonRect = Rectangle.Empty;
            SearchIconRect = SearchBoxRect = FilterButtonRect = AdvancedButtonRect = Rectangle.Empty;
            ClearFilterRect = BadgeRect = Rectangle.Empty;
            Separator1X = Separator2X = Separator3X = 0;
            foreach (var btn in ActionButtons) btn.Bounds = Rectangle.Empty;
            foreach (var btn in ExportButtons) btn.Bounds = Rectangle.Empty;
        }

        public string? HitTest(Point p)
        {
            // Check action buttons
            foreach (var btn in ActionButtons)
            {
                if (btn.IsVisible && !btn.IsOverflow && btn.Bounds.Contains(p))
                    return btn.Key;
            }

            // Check export buttons
            foreach (var btn in ExportButtons)
            {
                if (btn.IsVisible && !btn.IsOverflow && btn.Bounds.Contains(p))
                    return btn.Key;
            }

            // Check overflow button
            if (OverflowButtonRect.Contains(p))
                return "overflow";

            // Check clear filter
            if (ClearFilterRect.Contains(p) && IsFilterActive)
                return "clearfilter";

            return null;
        }

        public bool HitTestSearchBox(Point p) => SearchBoxRect.Contains(p);
        public bool HitTestFilterButton(Point p) => FilterButtonRect.Contains(p);
        public bool HitTestAdvancedButton(Point p) => AdvancedButtonRect.Contains(p);

        /// <summary>
        /// Gets the list of overflow items for the dropdown menu.
        /// </summary>
        public List<ToolbarButtonItem> GetOverflowItems()
        {
            var items = new List<ToolbarButtonItem>();
            items.AddRange(ActionButtons.FindAll(b => b.IsOverflow));
            items.AddRange(ExportButtons.FindAll(b => b.IsOverflow));
            return items;
        }
    }
}
