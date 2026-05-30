using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Layoutmanagers
{
    /// <summary>
    /// Caption chrome buttons shared by the panel caption and the dockspace header.
    /// Order in the buttons list = right-to-left placement (first entry sits rightmost).
    /// </summary>
    internal enum CaptionButtonKind
    {
        Close,
        Float,
        AutoHide,
        Pin,
        DropDown
    }

    /// <summary>
    /// Immutable description of one tab to lay out in a caption/header strip.
    /// </summary>
    internal sealed class CaptionTabModel
    {
        public string Key { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string IconPath { get; init; } = string.Empty;
        public bool IsDirty { get; init; }
        public bool IsActive { get; init; }

        /// <summary>Optional back-reference so callers can map a hit-tested tab to their model.</summary>
        public object Tag { get; init; }
    }

    /// <summary>
    /// Computes tab and caption-button geometry for a caption strip and resolves hit-testing.
    /// This is the single source of layout for both <c>DockPanel</c> and <c>BeepDockspace</c>
    /// captions. Per house style the layout manager <b>detects</b> (geometry + hit-test); the
    /// matching <c>CaptionRenderer</c> only paints the resolved layout.
    /// </summary>
    internal sealed class CaptionLayoutManager
    {
        public int ButtonSize { get; init; } = 18;
        public int ButtonSpacing { get; init; } = 4;
        public int MinTabWidth { get; init; } = 1;
        public int MaxTabWidth { get; init; } = 160;
        public int RightMargin { get; init; } = 4;
        public int TabsRightGap { get; init; } = 2;

        /// <summary>Smallest readable tab width; below this the surplus tabs collapse into the chevron.</summary>
        public int OverflowMinTabWidth { get; init; } = 64;

        private readonly Dictionary<CaptionButtonKind, Rectangle> _buttonRects = new();
        private readonly List<KeyValuePair<CaptionTabModel, Rectangle>> _tabRects = new();
        private readonly List<CaptionTabModel> _overflowTabs = new();

        /// <summary>Computed tab rectangles in paint order (left to right).</summary>
        public IReadOnlyList<KeyValuePair<CaptionTabModel, Rectangle>> TabRects => _tabRects;

        /// <summary>Strip height used by the last <see cref="Compute"/> call.</summary>
        public int StripHeight { get; private set; }

        /// <summary>True when not all tabs fit and a chevron is shown.</summary>
        public bool HasOverflow { get; private set; }

        /// <summary>Chevron rect (empty when <see cref="HasOverflow"/> is false).</summary>
        public Rectangle OverflowButtonRect { get; private set; }

        /// <summary>Tabs that did not fit and are reachable only through the chevron dropdown.</summary>
        public IReadOnlyList<CaptionTabModel> OverflowTabs => _overflowTabs;

        /// <summary>
        /// Computes button rects (right-aligned in the supplied order) and tab rects (left-aligned,
        /// sharing the remaining width). When the tabs cannot all fit at <see cref="OverflowMinTabWidth"/>,
        /// only those that fit are laid out — the active tab is always kept visible — and a chevron is
        /// reserved on the right of the tab area. Call from paint and from resize/hit-test code paths.
        /// </summary>
        public void Compute(int width, int height, IReadOnlyList<CaptionTabModel> tabs, IReadOnlyList<CaptionButtonKind> buttons)
        {
            _buttonRects.Clear();
            _tabRects.Clear();
            _overflowTabs.Clear();
            HasOverflow = false;
            OverflowButtonRect = Rectangle.Empty;
            StripHeight = height;

            int y = (height - ButtonSize) / 2;
            int x = width - RightMargin;

            if (buttons != null)
            {
                foreach (var kind in buttons)
                {
                    x -= ButtonSize + ButtonSpacing;
                    _buttonRects[kind] = new Rectangle(x, y, ButtonSize, ButtonSize);
                }
            }

            int buttonLeft = FirstButtonLeft(width);
            int tabsWidth = Math.Max(0, buttonLeft - TabsRightGap);

            int count = tabs?.Count ?? 0;
            if (count == 0 || tabsWidth <= 0)
                return;

            // How many tabs fit at a readable width? If all fit, share the width evenly (capped).
            int chevronSlot = ButtonSize + ButtonSpacing;
            int fitCount = Math.Max(1, tabsWidth / OverflowMinTabWidth);

            if (count <= fitCount)
            {
                LayoutVisibleTabs(tabs, tabsWidth, height);
                return;
            }

            // Overflow: reserve a chevron, keep the active tab visible, lay out the first
            // (visibleCount) tabs in order and push the remainder into the dropdown.
            HasOverflow = true;
            int tabsArea = Math.Max(OverflowMinTabWidth, tabsWidth - chevronSlot);
            int visibleCount = Math.Max(1, tabsArea / OverflowMinTabWidth);
            if (visibleCount >= count) visibleCount = count - 1;   // guarantee at least one overflow

            var visible = new List<CaptionTabModel>(visibleCount);
            for (int i = 0; i < count; i++)
                visible.Add(tabs[i]);

            // Ensure the active tab is in the visible window; if not, swap it into the last slot.
            int activeIndex = -1;
            for (int i = 0; i < count; i++)
                if (tabs[i].IsActive) { activeIndex = i; break; }

            var shown = new List<CaptionTabModel>(visibleCount);
            for (int i = 0; i < visibleCount; i++)
                shown.Add(tabs[i]);

            if (activeIndex >= visibleCount && visibleCount > 0)
                shown[visibleCount - 1] = tabs[activeIndex];

            for (int i = 0; i < count; i++)
            {
                if (!shown.Contains(tabs[i]))
                    _overflowTabs.Add(tabs[i]);
            }

            LayoutVisibleTabs(shown, tabsArea, height);

            OverflowButtonRect = new Rectangle(tabsArea + TabsRightGap, y, ButtonSize, ButtonSize);
        }

        private void LayoutVisibleTabs(IReadOnlyList<CaptionTabModel> tabs, int tabsWidth, int height)
        {
            int count = tabs.Count;
            if (count == 0) return;

            int tabWidth = Math.Max(MinTabWidth, Math.Min(MaxTabWidth, tabsWidth / count));
            int tx = 0;
            foreach (var tab in tabs)
            {
                if (tx >= tabsWidth)
                    break;
                var rect = new Rectangle(tx, 0, Math.Min(tabWidth, tabsWidth - tx), height);
                _tabRects.Add(new KeyValuePair<CaptionTabModel, Rectangle>(tab, rect));
                tx += rect.Width;
            }
        }

        /// <summary>Returns true if the point is on the overflow chevron.</summary>
        public bool HitTestOverflow(Point p) => HasOverflow && OverflowButtonRect.Contains(p);

        /// <summary>Returns the rect for a button, or <see cref="Rectangle.Empty"/> if not shown.</summary>
        public Rectangle GetButtonRect(CaptionButtonKind kind) =>
            _buttonRects.TryGetValue(kind, out var r) ? r : Rectangle.Empty;

        /// <summary>Enumerates the computed button rects.</summary>
        public IReadOnlyDictionary<CaptionButtonKind, Rectangle> ButtonRects => _buttonRects;

        /// <summary>Returns the button kind at a point, or null.</summary>
        public CaptionButtonKind? HitTestButton(Point p)
        {
            foreach (var kv in _buttonRects)
            {
                if (kv.Value.Contains(p))
                    return kv.Key;
            }
            return null;
        }

        /// <summary>Returns the tab model at a point, or null.</summary>
        public CaptionTabModel HitTestTab(Point p)
        {
            foreach (var kv in _tabRects)
            {
                if (kv.Value.Contains(p))
                    return kv.Key;
            }
            return null;
        }

        private int FirstButtonLeft(int width)
        {
            int left = width;
            foreach (var r in _buttonRects.Values)
                left = Math.Min(left, r.Left);

            return left == width ? width : Math.Max(0, left - ButtonSpacing);
        }
    }
}
