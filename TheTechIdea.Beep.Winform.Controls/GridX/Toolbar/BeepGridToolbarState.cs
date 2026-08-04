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
        public bool ShowFilterButton { get; set; } = true;

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

            // ============================================================================
            // RIGHT CLUSTER - laid out right-to-left, anchored to the toolbar's right edge.
            // ============================================================================
            //
            // Anchoring is the whole point. This used to be positioned by the same running x that
            // the title and search advanced, with only a computed "reserved width" keeping them out
            // of its way. Once the flexible sections hit their minimums that reservation was simply
            // exceeded and the cluster walked off the end of the control: measured on a 200px
            // toolbar, the advanced button occupied 186..204 and the overflow chevron sat at x=210,
            // both outside the control and therefore invisible.
            //
            // Placing it backwards from the right edge makes overrunning impossible. Whatever
            // survives is the middle's budget, and if that is not enough the middle collapses
            // rather than the toolbar bleeding past its own bounds.
            int right = bounds.Right - margin;

            // Every button in the cluster gets the same box: icon-wide, full band height. The width
            // stays at the icon because a previous pass tried a 28px minimum and it padded the whole
            // strip out; the height is now the band for all of them. They used to differ - export
            // buttons were 18x32 while filter and advanced were 18x18 - so adjacent icons had hit
            // targets of different heights and read as vertically misaligned even though each was
            // individually centred.
            Rectangle Slot(int rightEdge) =>
                new Rectangle(rightEdge - iconSize, bandY, iconSize, bandHeight);

            if (reserveOverflow)
            {
                OverflowButtonRect = Slot(right);
                right = OverflowButtonRect.Left - buttonGap;

                Separator3X = right - separatorWidth;
                right -= separatorWidth + buttonGap;
            }

            // How many export buttons fit, counted from the start of the list. Placement below is
            // right-to-left, but the decision of *which* survive has to run left-to-right: taking
            // them from the right would keep the last-declared buttons and push the first ones into
            // the chevron, which is backwards. A toolbar overflows from its tail.
            var exportsToPlace = ExportButtons.Where(b => b.IsVisible).ToList();
            int slotPitch = iconSize + buttonGap;
            int exportBudget = Math.Max(0, right - (bounds.Left + margin + minSearchWidth));
            int exportsThatFit = Math.Max(0, Math.Min(exportsToPlace.Count, exportBudget / Math.Max(1, slotPitch)));

            for (int i = 0; i < exportsToPlace.Count; i++)
            {
                exportsToPlace[i].IsOverflow = i >= exportsThatFit;
                exportsToPlace[i].Bounds = Rectangle.Empty;
            }

            // Now place the survivors right-to-left, so the first declared ends up left-most.
            int exportRight = right;
            for (int i = exportsThatFit - 1; i >= 0; i--)
            {
                var slot = Slot(exportRight);
                exportsToPlace[i].Bounds = slot;
                exportRight = slot.Left - buttonGap;
            }

            var placedExports = ExportButtons.Where(b => b.IsVisible && !b.IsOverflow).ToList();
            if (placedExports.Count > 0)
            {
                ExportSectionRect = Rectangle.FromLTRB(
                    placedExports.Min(b => b.Bounds.Left), bandY,
                    placedExports.Max(b => b.Bounds.Right), bandY + bandHeight);

                right = exportRight;
                Separator2X = right - separatorWidth;
                right -= separatorWidth + buttonGap;
            }
            else
            {
                ExportSectionRect = Rectangle.Empty;
            }

            // Filter cluster, right-to-left: clear-filter chip (only while a filter is active),
            // advanced, then the optional filter button. Left-to-right they read filter, advanced,
            // clear.
            int filterRight = right;

            if (IsFilterActive)
            {
                ClearFilterRect = Slot(filterRight);
                filterRight = ClearFilterRect.Left - buttonGap;
            }

            AdvancedButtonRect = Slot(filterRight);
            filterRight = AdvancedButtonRect.Left - buttonGap;

            if (ShowFilterButton)
            {
                FilterButtonRect = Slot(filterRight);
                filterRight = FilterButtonRect.Left - buttonGap;
            }

            FilterSectionRect = Rectangle.FromLTRB(
                filterRight + buttonGap, bandY, right, bandY + bandHeight);
            right = filterRight;

            Separator1X = right - separatorWidth;
            right -= separatorWidth + buttonGap;

            // The active-filter badge sits over whichever button the user reads as "the filter
            // button", clamped inside the band so it cannot paint outside the toolbar.
            var badgeAnchor = ShowFilterButton ? FilterButtonRect : AdvancedButtonRect;
            int badgeSize = (int)(14 * DpiScale);
            int badgeTop = badgeAnchor.Top + (bandHeight - iconSize) / 2 - badgeSize / 3;
            BadgeRect = ClampToBand(
                new Rectangle(badgeAnchor.Right - badgeSize / 2, badgeTop, badgeSize, badgeSize),
                bounds);

            // Whatever survives is the middle's to divide.
            int rightLimit = Math.Max(bounds.Left + margin, right);

            // ============================================================================
            // MIDDLE - title, actions, then the search box right-aligned against the cluster.
            // ============================================================================
            int x = bounds.Left + margin;

            if (ShowGridTitle && !string.IsNullOrEmpty(GridTitle))
            {
                // Measured with the font the painter draws with, so the reservation and the drawn
                // text cannot disagree about how much room the title needs.
                var titleSize = TextRenderer.MeasureText(GridTitle, TitleFont);

                int minTitle = (int)(MinTitleLogicalWidth * DpiScale);
                int titleBudget = Math.Max(0, rightLimit - x - minSearchWidth - margin);

                // The title yields before the search box does, but never sits below a floor at
                // which it would be unreadable - past that it is dropped outright rather than
                // shown as two characters and an ellipsis.
                int titleWidth = Math.Min(titleSize.Width + margin, titleBudget);
                if (titleWidth < minTitle)
                    titleWidth = titleBudget >= minTitle ? minTitle : 0;

                if (titleWidth > 0)
                {
                    TitleSectionRect = new Rectangle(x, bandY, titleWidth, bandHeight);
                    x += titleWidth + margin;
                }
            }

            int actionsBudget = Math.Max(0, rightLimit - x - minSearchWidth - margin);
            bool showActionLabels = actionsBudget >= (int)(LabelCollapseLogicalWidth * DpiScale);

            int actionsStartX = x;
            LayoutButtonList(ActionButtons, ref x, bandY, iconSize, bandHeight, buttonGap,
                             x + actionsBudget, showActionLabels, labelFont);
            int actionsEndX = x;
            ActionsSectionRect = ActionButtons.Any(b => b.IsVisible && !b.IsOverflow)
                ? new Rectangle(actionsStartX, bandY, actionsEndX - actionsStartX, bandHeight)
                : Rectangle.Empty;

            // === SEARCH BOX ===
            // Right-aligned against the cluster rather than stretched across the band: a full-width
            // field dominates the toolbar and pushes the title into a corner, where commercial grids
            // keep a modest field grouped with the other tools.
            //
            // The icon sits INSIDE the box, occupying the same inset the painter and the on-demand
            // editor both apply to their text. Painted outside it, that inset was empty space and
            // the placeholder sat visibly out of line with the icon.
            int maxSearch = (int)(MaxSearchLogicalWidth * DpiScale);
            int available = Math.Max(0, rightLimit - x);
            SearchIconWidth = SearchIconLogicalInset;   // logical; painter/editor scale it themselves

            if (available < (int)(MinSearchCollapseLogicalWidth * DpiScale))
            {
                // Not even a stub of a field fits. Drawing one anyway is what produced a search box
                // overlapping the buttons beside it.
                SearchBoxRect = Rectangle.Empty;
                SearchIconRect = Rectangle.Empty;
                SearchSectionRect = Rectangle.Empty;
            }
            else
            {
                int searchWidth = Math.Min(maxSearch, available);
                int searchX = rightLimit - searchWidth;
                SearchBoxRect = new Rectangle(searchX, bandY, searchWidth, bandHeight);
                SearchIconRect = new Rectangle(
                    SearchBoxRect.Left + (int)(6 * DpiScale),
                    CenterY(bandY, bandHeight, iconSize),
                    iconSize, iconSize);
                SearchSectionRect = SearchBoxRect;
            }
        }

        /// <summary>Minimum search box width in logical pixels before other sections must give way.</summary>
        private const int MinSearchLogicalWidth = 120;

        /// <summary>
        /// Below this much free space the search box is dropped entirely rather than shown as a
        /// stub. A field too narrow to type into is worse than none, and drawing one anyway is what
        /// made it overlap the buttons beside it.
        /// </summary>
        private const int MinSearchCollapseLogicalWidth = 72;

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
