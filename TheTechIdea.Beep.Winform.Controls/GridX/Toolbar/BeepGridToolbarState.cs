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
        public string Key { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Tooltip { get; set; } = string.Empty;
        public Keys Shortcut { get; set; } = Keys.None;
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
        // ===== Well-known button keys (single source of truth) =====
        public const string KeyAdd = "add";
        public const string KeyEdit = "edit";
        public const string KeyDelete = "delete";
        public const string KeyImport = "import";
        public const string KeyExport = "export";
        public const string KeyPrint = "print";
        public const string KeyClearFilter = "clearfilter";
        public const string KeyOverflow = "overflow";
        public const string KeyFilter = "filter";
        public const string KeyAdvanced = "advanced";
        public const string KeySearchBox = "searchbox";

        // ===== Public mutable state (set by host or renderer pre-paint) =====
        public string SearchText { get; set; } = string.Empty;
        public bool SearchHasFocus { get; set; }
        public int ActiveFilterCount { get; set; }
        public bool IsFilterActive { get; set; }
        public float DpiScale { get; set; } = 1f;

        public string GridTitle { get; set; } = "Grid";
        public bool ShowGridTitle { get; set; } = true;

        /// <summary>
        /// When false (the default), the toolbar omits the quick Filter
        /// button.  The Advanced button covers all multi-criteria
        /// cases, and the column-header filter icons cover the
        /// single-column case, so the standalone Filter button was
        /// redundant in the default UI.  Hosts that want the old
        /// behaviour — a funnel that opens the quick column=value
        /// dialog — can set this to <c>true</c> from the property
        /// grid or from code.
        /// </summary>
        public bool ShowFilterButton { get; set; } = false;

        /// <summary>
        /// Font used to measure and draw the grid title (one size
        /// larger than <see cref="LabelFont"/>, bold).  Set by the
        /// painter after the cache is built so layout's
        /// <see cref="MeasureGridTitle"/> and the painter's
        /// <see cref="BeepGridToolbarPainter"/> use the same font
        /// instance and avoid the per-paint allocation of a
        /// <c>new Font(...)</c>.
        /// </summary>
        public Font TitleFont { get; set; } = SystemFonts.DefaultFont;

        /// <summary>
        /// Font used to measure and draw toolbar labels.  Set by the
        /// painter before each layout pass so the label width matches
        /// the painted width even when the host overrides the grid font.
        /// Falls back to <see cref="SystemFonts.DefaultFont"/> when the
        /// host has not yet provided a font (design-time scenarios).
        /// </summary>
        public Font LabelFont { get; set; } = SystemFonts.DefaultFont;

        /// <summary>
        /// Pixel width of the search icon, used as the text-pad offset
        /// inside the search box.  Both the painter and the on-demand
        /// search editor read this so the rendered text is exactly
        /// aligned with the painted placeholder.
        /// </summary>
        public int SearchIconWidth { get; set; } = 24;

        // ===== Layout (recalculated by CalculateLayout) =====
        public Rectangle TitleSectionRect { get; private set; }
        public Rectangle ActionsSectionRect { get; private set; }
        public Rectangle SearchSectionRect { get; private set; }
        public Rectangle FilterSectionRect { get; private set; }
        public Rectangle ExportSectionRect { get; private set; }
        public Rectangle OverflowButtonRect { get; private set; }

        public Rectangle SearchIconRect { get; private set; }
        public Rectangle SearchBoxRect { get; private set; }
        public Rectangle FilterButtonRect { get; private set; }
        public Rectangle AdvancedButtonRect { get; private set; }
        public Rectangle ClearFilterRect { get; private set; }
        public Rectangle BadgeRect { get; private set; }

        public List<ToolbarButtonItem> ActionButtons { get; } = new();
        public List<ToolbarButtonItem> ExportButtons { get; } = new();

        public int Separator1X { get; private set; }
        public int Separator2X { get; private set; }
        public int Separator3X { get; private set; }

        public string? HoveredButtonKey { get; set; }
        public string? PressedButtonKey { get; set; }

        public bool HasOverflowItems =>
            ActionButtons.Any(b => b.IsOverflow) || ExportButtons.Any(b => b.IsOverflow);

        public BeepGridToolbarState()
        {
            BuildButtonLists();
        }

        /// <summary>
        /// Resets layout rectangles and overflow flags.  Called from
        /// <see cref="CalculateLayout"/> when the bounds are empty, and
        /// also from the constructor so callers can read the lists
        /// before the first paint.
        /// </summary>
        public void Reset()
        {
            TitleSectionRect = ActionsSectionRect = SearchSectionRect = FilterSectionRect = ExportSectionRect = Rectangle.Empty;
            OverflowButtonRect = Rectangle.Empty;
            SearchIconRect = SearchBoxRect = FilterButtonRect = AdvancedButtonRect = Rectangle.Empty;
            ClearFilterRect = BadgeRect = Rectangle.Empty;
            Separator1X = Separator2X = Separator3X = 0;
            ResetButtonList(ActionButtons);
            ResetButtonList(ExportButtons);
        }

        private static void ResetButtonList(List<ToolbarButtonItem> buttons)
        {
            foreach (var btn in buttons)
            {
                btn.Bounds = Rectangle.Empty;
                btn.IsOverflow = false;
            }
        }

        /// <summary>
        /// Recomputes every section rect and every button's <see cref="ToolbarButtonItem.Bounds"/>
        /// from the supplied <paramref name="bounds"/> rectangle and
        /// <see cref="DpiScale"/>.  When the toolbar is wider than the
        /// minimum, action and export buttons go in the toolbar itself;
        /// when narrower, surplus buttons are flagged <see cref="ToolbarButtonItem.IsOverflow"/>
        /// so the painter can hide them and the input helper can
        /// surface them in a chevron menu.
        /// </summary>
        public void CalculateLayout(Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                Reset();
                return;
            }

            int margin = (int)(8 * DpiScale);
            int iconSize = (int)(18 * DpiScale);
            int buttonGap = (int)(4 * DpiScale);
            int height = (int)(32 * DpiScale);
            int y = bounds.Top + (bounds.Height - height) / 2;
            int separatorWidth = Math.Max(1, (int)Math.Round(DpiScale));
            Font labelFont = LabelFont;

            Reset();
            int x = bounds.Left + margin;

            // === TITLE SECTION (optional, left-most) ===
            if (ShowGridTitle && !string.IsNullOrEmpty(GridTitle))
            {
                // The painter populates TitleFont once per cache so this
                // measurement reuses the same font instance the painter
                // draws with.  The previous code allocated a new Font
                // every paint via "using var titleFont" — the allocation
                // was visible in the toolbar's paint profile.
                var titleFont = TitleFont;
                var titleSize = TextRenderer.MeasureText(GridTitle, titleFont);
                int titleWidth = Math.Min(titleSize.Width + margin, bounds.Width / 4);
                TitleSectionRect = new Rectangle(x, y, titleWidth, height);
                x += titleWidth + margin;
            }

            // === ACTIONS SECTION (text labels for primary CRUD) ===
            int actionsStartX = x;
            LayoutButtonList(ActionButtons, ref x, y, iconSize, height, buttonGap, bounds.Width, margin, showLabels: true, labelFont);
            int actionsEndX = x;
            bool hasVisibleActions = ActionButtons.Any(b => b.IsVisible && !b.IsOverflow);
            ActionsSectionRect = hasVisibleActions
                ? new Rectangle(actionsStartX, y - (height - iconSize) / 2, actionsEndX - actionsStartX, height)
                : Rectangle.Empty;

            // === SEARCH SECTION (flexible, fills remaining space) ===
            int searchIconX = x;
            SearchIconRect = new Rectangle(searchIconX, y, iconSize, iconSize);
            x += iconSize + (int)(4 * DpiScale);

            int filterSectionWidth = iconSize * 2 + buttonGap * 2 + margin * 2 + (int)(16 * DpiScale);
            int exportVisibleCount = ExportButtons.Count(b => b.IsVisible && !b.IsOverflow);
            int exportSectionWidth = exportVisibleCount * (iconSize + buttonGap) + margin;
            int overflowWidth = HasOverflowItems ? (iconSize + margin) : 0;
            int separatorCount = 0;
            if (hasVisibleActions) separatorCount++;
            if (exportVisibleCount > 0) separatorCount++;
            if (HasOverflowItems) separatorCount++;
            int reservedRight = filterSectionWidth + exportSectionWidth + overflowWidth + separatorWidth * separatorCount + margin;

            int searchWidth = Math.Max(80, bounds.Right - x - reservedRight);
            SearchBoxRect = new Rectangle(x, y, searchWidth, height);
            x = SearchBoxRect.Right + margin;
            SearchSectionRect = new Rectangle(SearchIconRect.Left, y - (height - iconSize) / 2, x - SearchIconRect.Left, height);

            // === FILTER SECTION ===
            x += separatorWidth;
            Separator1X = x - separatorWidth;
            if (ShowFilterButton)
            {
                FilterButtonRect = new Rectangle(x, y, iconSize, iconSize);
                x += iconSize + buttonGap;
            }
            else
            {
                FilterButtonRect = Rectangle.Empty;
            }
            AdvancedButtonRect = new Rectangle(x, y, iconSize, iconSize);
            x += iconSize + buttonGap;

            // The active-filter badge sits over the Advanced button when
            // the standalone Filter button is hidden (the default), and
            // over the Filter button when it is shown.  Either way the
            // badge announces the active-filter count on whichever
            // button the user perceives as "the filter button".
            var badgeAnchor = ShowFilterButton ? FilterButtonRect : AdvancedButtonRect;
            BadgeRect = new Rectangle(badgeAnchor.Right - 4, y - 6, (int)(16 * DpiScale), (int)(16 * DpiScale));
            ClearFilterRect = new Rectangle(AdvancedButtonRect.Right + buttonGap, y, iconSize, iconSize);
            FilterSectionRect = new Rectangle(SearchSectionRect.Right, y - (height - iconSize) / 2, x - SearchSectionRect.Right, height);

            // === EXPORT SECTION (right) ===
            x += separatorWidth;
            Separator2X = x - separatorWidth;

            int exportStartX = x;
            LayoutButtonList(ExportButtons, ref x, y, iconSize, height, buttonGap, bounds.Width, margin, showLabels: false, labelFont);
            int exportEndX = x;
            ExportSectionRect = exportVisibleCount > 0
                ? new Rectangle(exportStartX, y - (height - iconSize) / 2, exportEndX - exportStartX, height)
                : Rectangle.Empty;

            // === OVERFLOW BUTTON ===
            if (HasOverflowItems)
            {
                Separator3X = x;
                x += separatorWidth;
                OverflowButtonRect = new Rectangle(x, y, iconSize, iconSize);
            }
        }

        /// <summary>
        /// Lays out a button list left-to-right, flagging items that don't
        /// fit as overflow.  The width budget is derived from the
        /// available right-side real estate so the search box never gets
        /// squeezed below 80 px.
        /// </summary>
        private void LayoutButtonList(List<ToolbarButtonItem> buttons, ref int x, int y,
            int iconSize, int height, int buttonGap, int totalWidth, int margin, bool showLabels, Font labelFont)
        {
            int availableWidth = totalWidth - margin * 4;

            foreach (var btn in buttons)
            {
                if (!btn.IsVisible) continue;

                int btnWidth = showLabels && !string.IsNullOrEmpty(btn.Label)
                    ? MeasureLabeledButtonWidth(btn, iconSize, labelFont)
                    : iconSize;

                if (x + btnWidth > availableWidth)
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

        private int MeasureLabeledButtonWidth(ToolbarButtonItem btn, int iconSize, Font labelFont)
        {
            // Use the supplied label font (which is the toolbar's resolved
            // font, not the global SystemFonts.DefaultFont) so measured width
            // matches the actual paint width even when the host overrides
            // _grid.Font.  The previous version used SystemFonts.DefaultFont
            // and produced clipped labels with custom fonts.
            var labelSize = TextRenderer.MeasureText(btn.Label, labelFont);
            return iconSize + (int)(6 * DpiScale) + labelSize.Width + (int)(8 * DpiScale);
        }

        /// <summary>
        /// Returns the keyboard key for the button with the given <paramref name="key"/>,
        /// or <see cref="Keys.None"/> if the button has no shortcut.
        /// </summary>
        public Keys GetShortcutForButton(string key)
        {
            var btn = FindButton(key);
            return btn?.Shortcut ?? Keys.None;
        }

        /// <summary>
        /// Returns the tooltip text for the button with the given <paramref name="key"/>,
        /// or the empty string if the button has no tooltip.
        /// </summary>
        public string GetTooltipForButton(string key)
        {
            var btn = FindButton(key);
            return btn?.Tooltip ?? string.Empty;
        }

        /// <summary>Locates a button by key across action + export lists.</summary>
        public ToolbarButtonItem? FindButton(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            return ActionButtons.Find(b => b.Key == key)
                ?? ExportButtons.Find(b => b.Key == key);
        }

        /// <summary>
        /// Returns the first button whose <see cref="ToolbarButtonItem.Shortcut"/>
        /// matches <paramref name="key"/>.  Used by the keyboard handler
        /// to fire toolbar shortcuts like Insert (Add), F2 (Edit), and
        /// Delete (Delete).
        /// </summary>
        public ToolbarButtonItem? FindButtonByShortcut(Keys key)
        {
            return ActionButtons.FirstOrDefault(b => b.Shortcut == key)
                ?? ExportButtons.FirstOrDefault(b => b.Shortcut == key);
        }

        /// <summary>Populates the action and export button lists with defaults.</summary>
        private void BuildButtonLists()
        {
            ActionButtons.Clear();
            ActionButtons.Add(new ToolbarButtonItem
            {
                Key = KeyAdd,
                IconPath = "plus",
                Label = "New",
                Tooltip = "Add a new row",
                Shortcut = Keys.Insert,
            });
            ActionButtons.Add(new ToolbarButtonItem
            {
                Key = KeyEdit,
                IconPath = "edit",
                Label = "Edit",
                Tooltip = "Edit the active cell",
                Shortcut = Keys.F2,
            });
            ActionButtons.Add(new ToolbarButtonItem
            {
                Key = KeyDelete,
                IconPath = "trash",
                Label = "Delete",
                Tooltip = "Delete the active row",
                Shortcut = Keys.Delete,
            });

            ExportButtons.Clear();
            ExportButtons.Add(new ToolbarButtonItem
            {
                Key = KeyImport,
                IconPath = "file_upload",
                Label = "Import",
                Tooltip = "Import data from a file",
            });
            ExportButtons.Add(new ToolbarButtonItem
            {
                Key = KeyExport,
                IconPath = "download",
                Label = "Export",
                Tooltip = "Export the grid to a file",
            });
            ExportButtons.Add(new ToolbarButtonItem
            {
                Key = KeyPrint,
                IconPath = "print",
                Label = "Print",
                Tooltip = "Print the grid",
            });
        }

        /// <summary>
        /// Returns the topmost button at <paramref name="p"/>, or <c>null</c>
        /// if no button is at that point.  Checks action buttons, export
        /// buttons, the overflow button, the clear-filter chip, then
        /// the named element rects (search, filter, advanced).
        /// </summary>
        public string? HitTest(Point p)
        {
            // Action and export buttons (visible and not overflowed)
            var fromList = HitTestButtonList(ActionButtons, p)
                       ?? HitTestButtonList(ExportButtons, p);
            if (fromList != null) return fromList;

            // Named element hit-tests (order matters: more specific first).
            // The Filter button is opt-in (ShowFilterButton = true); when
            // its rect is empty (the default), HitTestFilterButton
            // returns false and the click falls through to the toolbar
            // background, which is the correct behaviour.
            if (HitTestFilterButton(p)) return KeyFilter;
            if (HitTestAdvancedButton(p)) return KeyAdvanced;
            if (OverflowButtonRect.Contains(p)) return KeyOverflow;
            if (ClearFilterRect.Contains(p) && IsFilterActive) return KeyClearFilter;
            if (HitTestSearchBox(p)) return KeySearchBox;

            return null;
        }

        private static string? HitTestButtonList(List<ToolbarButtonItem> buttons, Point p)
        {
            foreach (var btn in buttons)
            {
                if (btn.IsVisible && !btn.IsOverflow && btn.Bounds.Contains(p))
                {
                    return btn.Key;
                }
            }
            return null;
        }

        // Named element hit-tests used by HitTest().  Inlined as
        // private helpers — keeping them out of the public surface
        // avoids duplicate paths through the state when callers can
        // simply use HitTest() to learn the key under the cursor.
        private bool HitTestSearchBox(Point p) => SearchBoxRect.Contains(p);
        private bool HitTestFilterButton(Point p) => FilterButtonRect.Contains(p);
        private bool HitTestAdvancedButton(Point p) => AdvancedButtonRect.Contains(p);

        /// <summary>
        /// Returns the buttons that were flagged as overflow by
        /// <see cref="CalculateLayout"/>.  The caller is expected to
        /// display these in a context menu so the user can still
        /// access them when the toolbar is narrow.
        /// </summary>
        public List<ToolbarButtonItem> GetOverflowItems()
        {
            var items = new List<ToolbarButtonItem>();
            items.AddRange(ActionButtons.FindAll(b => b.IsVisible && b.IsOverflow));
            items.AddRange(ExportButtons.FindAll(b => b.IsVisible && b.IsOverflow));
            return items;
        }
    }
}
