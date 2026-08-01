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

            // Two passes so nothing is reserved that will not be drawn. The first assumes every
            // button fits; if any overflowed, the second reserves the chevron slot and lays out
            // again. Reserving it up front left a permanent gap on toolbars that never overflow.
            LayoutPass(bounds, reserveOverflow: false);
            if (HasOverflowItems)
                LayoutPass(bounds, reserveOverflow: true);
        }

        private void LayoutPass(Rectangle bounds, bool reserveOverflow)
        {
            int margin = (int)(8 * DpiScale);
            int iconSize = (int)(18 * DpiScale);
            int buttonGap = (int)(4 * DpiScale);
            int bandHeight = (int)(32 * DpiScale);
            int bandY = bounds.Top + (bounds.Height - bandHeight) / 2;
            int separatorWidth = Math.Max(1, (int)Math.Round(DpiScale));
            int minSearchWidth = (int)(MinSearchLogicalWidth * DpiScale);
            Font labelFont = LabelFont;

            Reset();

            // The right-hand sections are fixed width, so budget them first: everything flexible
            // (title, actions, search) then lays out against a known right limit instead of
            // discovering the overrun afterwards and overlapping.
            //
            // Only what the painter will actually draw gets reserved — the clear-filter chip only
            // appears while a filter is active, and the chevron only when something overflowed.
            int exportVisibleCount = ExportButtons.Count(b => b.IsVisible);
            int filterButtonCount = 1                            // advanced
                                    + (ShowFilterButton ? 1 : 0)
                                    + (IsFilterActive ? 1 : 0);  // clear-filter chip
            int filterSectionWidth = filterButtonCount * (iconSize + buttonGap) + margin;
            int exportSectionWidth = exportVisibleCount > 0
                ? exportVisibleCount * (iconSize + buttonGap) + margin
                : 0;
            int overflowWidth = reserveOverflow ? iconSize + margin : 0;

            int separatorCount = 1                                     // before the filter section
                                 + (exportVisibleCount > 0 ? 1 : 0)
                                 + (reserveOverflow ? 1 : 0);
            int reservedRight = filterSectionWidth + exportSectionWidth + overflowWidth
                                + separatorWidth * separatorCount + margin;
            int rightLimit = Math.Max(bounds.Left + margin, bounds.Right - reservedRight);

            int x = bounds.Left + margin;

            // === TITLE SECTION (optional, left-most) ===
            if (ShowGridTitle && !string.IsNullOrEmpty(GridTitle))
            {
                // The painter populates TitleFont once per cache so this measurement reuses the
                // same font instance the painter draws with.
                var titleSize = TextRenderer.MeasureText(GridTitle, TitleFont);

                // Take what the title needs, but keep the search box above its minimum. The title
                // still gets a floor so a set title never disappears entirely — it ellipsizes
                // instead, and the search box gives up the difference.
                int minTitle = (int)(MinTitleLogicalWidth * DpiScale);
                int titleBudget = Math.Max(0, rightLimit - x - minSearchWidth - margin);
                int titleWidth = Math.Min(titleSize.Width + margin, Math.Max(titleBudget, minTitle));
                titleWidth = Math.Min(titleWidth, Math.Max(0, rightLimit - x - margin));
                if (titleWidth > 0)
                {
                    TitleSectionRect = new Rectangle(x, bandY, titleWidth, bandHeight);
                    x += titleWidth + margin;
                }
            }

            // === ACTIONS SECTION ===
            // Collapse order under pressure: labels go first, then whole buttons move to overflow.
            int actionsBudget = rightLimit - x - minSearchWidth - margin;
            bool showActionLabels = actionsBudget >= (int)(LabelCollapseLogicalWidth * DpiScale);

            int actionsStartX = x;
            LayoutButtonList(ActionButtons, ref x, bandY, iconSize, bandHeight, buttonGap,
                             x + Math.Max(0, actionsBudget), showActionLabels, labelFont);
            int actionsEndX = x;
            bool hasVisibleActions = ActionButtons.Any(b => b.IsVisible && !b.IsOverflow);
            ActionsSectionRect = hasVisibleActions
                ? new Rectangle(actionsStartX, bandY, actionsEndX - actionsStartX, bandHeight)
                : Rectangle.Empty;

            // === SEARCH SECTION (flexible, fills what is left) ===
            // The box is laid out first and the icon sits INSIDE it. The painter and the on-demand
            // search editor both inset their text by SearchIconWidth, so the icon has to occupy
            // that inset — previously it was painted to the left of the box and the inset was
            // empty space, leaving placeholder and typed text visibly out of line with the icon.
            // Cap the search box and right-align it against the filter/export cluster instead of
            // stretching it across the whole toolbar. A full-width search field dominated the band
            // and pushed the title into a corner; commercial grids keep a modest field grouped with
            // the other tools on the right, with open space after the title.
            int maxSearch = (int)(MaxSearchLogicalWidth * DpiScale);
            int available = Math.Max(0, rightLimit - x - margin);
            int searchWidth = Math.Max(minSearchWidth, Math.Min(maxSearch, available));
            int searchX = Math.Max(x, rightLimit - margin - searchWidth);
            SearchBoxRect = new Rectangle(searchX, bandY, searchWidth, bandHeight);
            SearchIconWidth = SearchIconLogicalInset;   // logical; painter/editor scale it themselves
            SearchIconRect = new Rectangle(
                SearchBoxRect.Left + (int)(6 * DpiScale),
                CenterY(bandY, bandHeight, iconSize),
                iconSize, iconSize);
            x = SearchBoxRect.Right + margin;
            SearchSectionRect = new Rectangle(SearchBoxRect.Left, bandY, x - SearchBoxRect.Left, bandHeight);

            // === FILTER SECTION ===
            x += separatorWidth;
            Separator1X = x - separatorWidth;
            int filterSectionStart = x;
            if (ShowFilterButton)
            {
                FilterButtonRect = new Rectangle(x, CenterY(bandY, bandHeight, iconSize), iconSize, iconSize);
                x += iconSize + buttonGap;
            }
            else
            {
                FilterButtonRect = Rectangle.Empty;
            }

            AdvancedButtonRect = new Rectangle(x, CenterY(bandY, bandHeight, iconSize), iconSize, iconSize);
            x += iconSize + buttonGap;

            // The clear-filter chip is only painted while a filter is active, so it only takes a
            // slot then — otherwise it left a permanent hole between Advanced and the exports.
            if (IsFilterActive)
            {
                ClearFilterRect = new Rectangle(x, CenterY(bandY, bandHeight, iconSize), iconSize, iconSize);
                x += iconSize + buttonGap;
            }
            else
            {
                ClearFilterRect = Rectangle.Empty;
            }

            // The active-filter badge sits over whichever button the user perceives as "the filter
            // button", clamped inside the band so it cannot paint outside the toolbar.
            var badgeAnchor = ShowFilterButton ? FilterButtonRect : AdvancedButtonRect;
            int badgeSize = (int)(14 * DpiScale);
            BadgeRect = ClampToBand(
                new Rectangle(badgeAnchor.Right - badgeSize / 2, badgeAnchor.Top - badgeSize / 3, badgeSize, badgeSize),
                bounds);
            FilterSectionRect = new Rectangle(filterSectionStart, bandY, x - filterSectionStart, bandHeight);

            // === EXPORT SECTION (right) ===
            x += separatorWidth;
            Separator2X = x - separatorWidth;

            int exportStartX = x;
            LayoutButtonList(ExportButtons, ref x, bandY, iconSize, bandHeight, buttonGap,
                             bounds.Right - overflowWidth - margin, showLabels: false, labelFont);
            int exportEndX = x;
            ExportSectionRect = ExportButtons.Any(b => b.IsVisible && !b.IsOverflow)
                ? new Rectangle(exportStartX, bandY, exportEndX - exportStartX, bandHeight)
                : Rectangle.Empty;

            // === OVERFLOW BUTTON ===
            if (HasOverflowItems)
            {
                Separator3X = x;
                x += separatorWidth;
                OverflowButtonRect = new Rectangle(x, CenterY(bandY, bandHeight, iconSize), iconSize, iconSize);
            }
        }

        /// <summary>Minimum search box width in logical pixels before other sections must give way.</summary>
        private const int MinSearchLogicalWidth = 120;

        /// <summary>Search box never grows past this; the surplus stays as space after the title.</summary>
        private const int MaxSearchLogicalWidth = 300;

        /// <summary>Floor for the title so a configured title ellipsizes rather than disappearing.</summary>
        private const int MinTitleLogicalWidth = 70;

        /// <summary>Below this much free space (logical px) action buttons drop their text labels.</summary>
        private const int LabelCollapseLogicalWidth = 260;

        /// <summary>
        /// Logical inset from the left of the search box to its text — the width the search icon
        /// occupies. Kept logical because both the painter and <c>FilterEditorHelper</c> scale it
        /// by DPI themselves; storing a scaled value here would double-scale it.
        /// </summary>
        private const int SearchIconLogicalInset = 24;

        /// <summary>Top coordinate that vertically centres <paramref name="itemHeight"/> in the band.</summary>
        private static int CenterY(int bandY, int bandHeight, int itemHeight)
            => bandY + Math.Max(0, (bandHeight - itemHeight) / 2);

        /// <summary>Keeps a rect inside the toolbar bounds so nothing paints outside the band.</summary>
        private static Rectangle ClampToBand(Rectangle rect, Rectangle bounds)
        {
            int top = Math.Max(bounds.Top, Math.Min(rect.Top, bounds.Bottom - rect.Height));
            int left = Math.Max(bounds.Left, Math.Min(rect.Left, bounds.Right - rect.Width));
            return new Rectangle(left, top, rect.Width, rect.Height);
        }

        /// <summary>
        /// Lays out a button list left-to-right, flagging anything past
        /// <paramref name="rightLimit"/> as overflow.
        /// </summary>
        /// <remarks>
        /// <paramref name="rightLimit"/> is an absolute X coordinate, not a width. The previous
        /// version compared the running <c>x</c> against a width derived from <c>bounds.Width</c>,
        /// which only agreed with reality when the toolbar started at x=0 — anywhere else buttons
        /// either overflowed early or ran past the sections to their right.
        ///
        /// Every button gets the full band height so labelled and icon-only buttons present the
        /// same hit target; the painter centres the icon within those bounds.
        /// </remarks>
        private void LayoutButtonList(List<ToolbarButtonItem> buttons, ref int x, int bandY,
            int iconSize, int bandHeight, int buttonGap, int rightLimit, bool showLabels, Font labelFont)
        {
            foreach (var btn in buttons)
            {
                if (!btn.IsVisible)
                {
                    btn.IsOverflow = false;
                    btn.Bounds = Rectangle.Empty;
                    continue;
                }

                // Icon-only buttons keep their original icon-width footprint; widening them to a
                // 28px minimum padded the whole strip out and made the toolbar look coarser.
                int btnWidth = showLabels && !string.IsNullOrEmpty(btn.Label)
                    ? MeasureLabeledButtonWidth(btn, iconSize, labelFont)
                    : iconSize;

                if (x + btnWidth > rightLimit)
                {
                    btn.IsOverflow = true;
                    btn.Bounds = Rectangle.Empty;
                    continue;
                }

                btn.IsOverflow = false;
                btn.Bounds = new Rectangle(x, bandY, btnWidth, bandHeight);
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
            // CRUD buttons (Add/Edit/Delete) are hidden by default.
            // Enable them via ShowAddButton / ShowEditButton / ShowDeleteButton
            // or by calling SetToolbarButtonVisible("add"/"edit"/"delete", true).
            ActionButtons.Add(new ToolbarButtonItem
            {
                Key = KeyAdd,
                IconPath = "plus",
                Label = "New",
                Tooltip = "Add a new row",
                Shortcut = Keys.Insert,
                IsVisible = false,
            });
            ActionButtons.Add(new ToolbarButtonItem
            {
                Key = KeyEdit,
                IconPath = "edit",
                Label = "Edit",
                Tooltip = "Edit the active cell",
                Shortcut = Keys.F2,
                IsVisible = false,
            });
            ActionButtons.Add(new ToolbarButtonItem
            {
                Key = KeyDelete,
                IconPath = "trash",
                Label = "Delete",
                Tooltip = "Delete the active row",
                Shortcut = Keys.Delete,
                IsVisible = false,
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
