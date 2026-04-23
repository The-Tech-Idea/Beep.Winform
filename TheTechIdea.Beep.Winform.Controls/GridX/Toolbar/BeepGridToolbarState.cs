using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Toolbar
{
    /// <summary>
    /// State model for the unified grid toolbar (actions + search + filter + export).
    /// Calculates proportional layout and provides hit testing.
    /// </summary>
    public class BeepGridToolbarState
    {
        public string SearchText { get; set; } = string.Empty;
        public bool SearchHasFocus { get; set; }
        public int ActiveFilterCount { get; set; }
        public bool IsFilterActive { get; set; }
        public float DpiScale { get; set; } = 1f;

        // Section rects
        public Rectangle ActionsSectionRect { get; private set; }
        public Rectangle SearchSectionRect { get; private set; }
        public Rectangle FilterSectionRect { get; private set; }
        public Rectangle ExportSectionRect { get; private set; }

        // Element rects
        public Rectangle SearchIconRect { get; private set; }
        public Rectangle SearchBoxRect { get; private set; }
        public Rectangle FilterButtonRect { get; private set; }
        public Rectangle AdvancedButtonRect { get; private set; }
        public Rectangle ClearFilterRect { get; private set; }
        public Rectangle BadgeRect { get; private set; }

        public Rectangle AddButtonRect { get; private set; }
        public Rectangle EditButtonRect { get; private set; }
        public Rectangle DeleteButtonRect { get; private set; }
        public Rectangle ImportButtonRect { get; private set; }
        public Rectangle ExportButtonRect { get; private set; }
        public Rectangle PrintButtonRect { get; private set; }

        // Separator positions
        public int Separator1X { get; private set; }
        public int Separator2X { get; private set; }

        // Hover/pressed tracking
        public string? HoveredButtonKey { get; set; }
        public string? PressedButtonKey { get; set; }

        public void CalculateLayout(Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                ActionsSectionRect = SearchSectionRect = FilterSectionRect = ExportSectionRect = Rectangle.Empty;
                SearchIconRect = SearchBoxRect = FilterButtonRect = AdvancedButtonRect = Rectangle.Empty;
                ClearFilterRect = BadgeRect = Rectangle.Empty;
                AddButtonRect = EditButtonRect = DeleteButtonRect = Rectangle.Empty;
                ImportButtonRect = ExportButtonRect = PrintButtonRect = Rectangle.Empty;
                return;
            }

            int margin = (int)(6 * DpiScale);
            int iconSize = (int)(18 * DpiScale);
            int buttonGap = (int)(4 * DpiScale);
            int height = (int)(32 * DpiScale);
            int y = bounds.Top + (bounds.Height - height) / 2;

            // === ACTIONS SECTION (left) ===
            int x = bounds.Left + margin;
            AddButtonRect = new Rectangle(x, y, iconSize, iconSize);
            x += iconSize + buttonGap;
            EditButtonRect = new Rectangle(x, y, iconSize, iconSize);
            x += iconSize + buttonGap;
            DeleteButtonRect = new Rectangle(x, y, iconSize, iconSize);
            x += iconSize + margin;
            ActionsSectionRect = new Rectangle(bounds.Left, y - (height - iconSize) / 2, x - bounds.Left - margin, height);

            // === SEARCH SECTION (flexible) ===
            int searchIconX = x;
            SearchIconRect = new Rectangle(searchIconX, y, iconSize, iconSize);
            x += iconSize + (int)(4 * DpiScale);

            // Reserve space for filter + export sections
            int filterSectionWidth = iconSize * 2 + buttonGap + margin * 2;
            int exportSectionWidth = iconSize * 3 + buttonGap * 2 + margin * 2;
            int separatorWidth = (int)(2 * DpiScale);
            int reservedRight = filterSectionWidth + exportSectionWidth + separatorWidth * 2 + margin;

            int searchWidth = bounds.Right - x - reservedRight;
            searchWidth = Math.Max(80, searchWidth);

            SearchBoxRect = new Rectangle(x, y, searchWidth, height);
            x = SearchBoxRect.Right + margin;
            SearchSectionRect = new Rectangle(ActionsSectionRect.Right, y - (height - iconSize) / 2, x - ActionsSectionRect.Right, height);

            // === FILTER SECTION ===
            x += separatorWidth;
            Separator1X = x - separatorWidth;
            FilterButtonRect = new Rectangle(x, y, iconSize, iconSize);
            x += iconSize + buttonGap;
            AdvancedButtonRect = new Rectangle(x, y, iconSize, iconSize);
            x += iconSize + margin;

            BadgeRect = new Rectangle(FilterButtonRect.Right - 4, y - 6, (int)(16 * DpiScale), (int)(16 * DpiScale));
            ClearFilterRect = new Rectangle(AdvancedButtonRect.Right + buttonGap, y, iconSize, iconSize);

            FilterSectionRect = new Rectangle(SearchSectionRect.Right, y - (height - iconSize) / 2, x - SearchSectionRect.Right, height);

            // === EXPORT SECTION (right) ===
            x += separatorWidth;
            Separator2X = x - separatorWidth;
            ImportButtonRect = new Rectangle(x, y, iconSize, iconSize);
            x += iconSize + buttonGap;
            ExportButtonRect = new Rectangle(x, y, iconSize, iconSize);
            x += iconSize + buttonGap;
            PrintButtonRect = new Rectangle(x, y, iconSize, iconSize);
            ExportSectionRect = new Rectangle(FilterSectionRect.Right, y - (height - iconSize) / 2, bounds.Right - FilterSectionRect.Right, height);
        }

        public string? HitTest(Point p)
        {
            if (AddButtonRect.Contains(p)) return "add";
            if (EditButtonRect.Contains(p)) return "edit";
            if (DeleteButtonRect.Contains(p)) return "delete";
            if (ImportButtonRect.Contains(p)) return "import";
            if (ExportButtonRect.Contains(p)) return "export";
            if (PrintButtonRect.Contains(p)) return "print";
            if (ClearFilterRect.Contains(p) && IsFilterActive) return "clearfilter";
            return null;
        }

        public bool HitTestSearchBox(Point p) => SearchBoxRect.Contains(p);
        public bool HitTestFilterButton(Point p) => FilterButtonRect.Contains(p);
        public bool HitTestAdvancedButton(Point p) => AdvancedButtonRect.Contains(p);
    }
}
