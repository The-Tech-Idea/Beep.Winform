using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Layoutmanagers
{
    internal enum CaptionButtonKind
    {
        Close,
        Float,
        AutoHide,
        Pin,
        DropDown
    }

    internal sealed class CaptionTabModel
    {
        public string Key { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string IconPath { get; init; } = string.Empty;
        public bool IsDirty { get; init; }
        public bool IsActive { get; init; }
        public object Tag { get; init; }
    }

    internal sealed class CaptionLayoutManager
    {
        public int ButtonSize { get; init; } = 18;
        public int ButtonSpacing { get; init; } = 4;
        public int MinTabWidth { get; init; } = 1;
        public int MaxTabWidth { get; init; } = 160;
        public int RightMargin { get; init; } = 4;
        public int TabsRightGap { get; init; } = 2;
        public int OverflowMinTabWidth { get; init; } = 64;

        public bool IsVertical { get; set; }
        public bool IsFlipped { get; set; }

        public int VerticalIconSize { get; init; } = 16;
        public int VerticalPadding { get; init; } = 4;

        private readonly Dictionary<CaptionButtonKind, Rectangle> _buttonRects = new();
        private readonly List<KeyValuePair<CaptionTabModel, Rectangle>> _tabRects = new();
        private readonly List<CaptionTabModel> _overflowTabs = new();

        public IReadOnlyList<KeyValuePair<CaptionTabModel, Rectangle>> TabRects => _tabRects;
        public int StripHeight { get; private set; }
        public bool HasOverflow { get; private set; }
        public Rectangle OverflowButtonRect { get; private set; }
        public IReadOnlyList<CaptionTabModel> OverflowTabs => _overflowTabs;

        /// <summary>The tab currently under the mouse pointer, set by the owning control's OnMouseMove.</summary>
        public CaptionTabModel HoveredTab { get; set; }

        public void Compute(int width, int height, IReadOnlyList<CaptionTabModel> tabs, IReadOnlyList<CaptionButtonKind> buttons)
        {
            _buttonRects.Clear();
            _tabRects.Clear();
            _overflowTabs.Clear();
            HasOverflow = false;
            OverflowButtonRect = Rectangle.Empty;
            StripHeight = height;

            if (IsVertical)
                ComputeVertical(width, height, tabs, buttons);
            else
                ComputeHorizontal(width, height, tabs, buttons);
        }

        private void ComputeHorizontal(int width, int height, IReadOnlyList<CaptionTabModel> tabs, IReadOnlyList<CaptionButtonKind> buttons)
        {
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

            int chevronSlot = ButtonSize + ButtonSpacing;
            int fitCount = Math.Max(1, tabsWidth / OverflowMinTabWidth);

            if (count <= fitCount)
            {
                LayoutVisibleHorizontalTabs(tabs, tabsWidth, height);
                return;
            }

            HasOverflow = true;
            int tabsArea = Math.Max(OverflowMinTabWidth, tabsWidth - chevronSlot);
            int visibleCount = Math.Max(1, tabsArea / OverflowMinTabWidth);
            if (visibleCount >= count) visibleCount = count - 1;

            var shown = new List<CaptionTabModel>(visibleCount);
            for (int i = 0; i < visibleCount; i++)
                shown.Add(tabs[i]);

            int activeIndex = -1;
            for (int i = 0; i < count; i++)
                if (tabs[i].IsActive) { activeIndex = i; break; }

            if (activeIndex >= visibleCount && visibleCount > 0)
                shown[visibleCount - 1] = tabs[activeIndex];

            for (int i = 0; i < count; i++)
            {
                if (!shown.Contains(tabs[i]))
                    _overflowTabs.Add(tabs[i]);
            }

            LayoutVisibleHorizontalTabs(shown, tabsArea, height);
            OverflowButtonRect = new Rectangle(tabsArea + TabsRightGap, y, ButtonSize, ButtonSize);
        }

        private void ComputeVertical(int width, int height, IReadOnlyList<CaptionTabModel> tabs, IReadOnlyList<CaptionButtonKind> buttons)
        {
            if (tabs == null || tabs.Count == 0) return;

            int bx = (width - ButtonSize) / 2;
            int by = height - RightMargin;
            if (buttons != null)
            {
                foreach (var kind in buttons)
                {
                    by -= ButtonSize + ButtonSpacing;
                    _buttonRects[kind] = new Rectangle(bx, by, ButtonSize, ButtonSize);
                }
            }

            int buttonRegionTop = FirstButtonTop(height);
            int tabsHeight = Math.Max(0, buttonRegionTop - VerticalPadding * 2);

            int count = tabs.Count;
            if (count == 0 || tabsHeight <= 0) return;

            int tabHeight = Math.Max(MinTabWidth, Math.Min(MaxTabWidth, tabsHeight / count));
            int ty = VerticalPadding;
            for (int i = 0; i < count; i++)
            {
                int h = Math.Min(tabHeight, tabsHeight - ty + VerticalPadding);
                if (h <= 0) break;
                var rect = new Rectangle(0, ty, width, h);
                _tabRects.Add(new KeyValuePair<CaptionTabModel, Rectangle>(tabs[i], rect));
                ty += h;
            }
        }

        private void LayoutVisibleHorizontalTabs(IReadOnlyList<CaptionTabModel> tabs, int tabsWidth, int height)
        {
            int count = tabs.Count;
            if (count == 0) return;

            int tabWidth = Math.Max(MinTabWidth, Math.Min(MaxTabWidth, tabsWidth / count));
            int tx = 0;
            foreach (var tab in tabs)
            {
                if (tx >= tabsWidth) break;
                var rect = new Rectangle(tx, 0, Math.Min(tabWidth, tabsWidth - tx), height);
                _tabRects.Add(new KeyValuePair<CaptionTabModel, Rectangle>(tab, rect));
                tx += rect.Width;
            }
        }

        public bool HitTestOverflow(Point p) => HasOverflow && OverflowButtonRect.Contains(p);

        public Rectangle GetButtonRect(CaptionButtonKind kind) =>
            _buttonRects.TryGetValue(kind, out var r) ? r : Rectangle.Empty;

        public IReadOnlyDictionary<CaptionButtonKind, Rectangle> ButtonRects => _buttonRects;

        public CaptionButtonKind? HitTestButton(Point p)
        {
            foreach (var kv in _buttonRects)
                if (kv.Value.Contains(p)) return kv.Key;
            return null;
        }

        public CaptionTabModel HitTestTab(Point p)
        {
            foreach (var kv in _tabRects)
                if (kv.Value.Contains(p)) return kv.Key;
            return null;
        }

        private int FirstButtonLeft(int width)
        {
            int left = width;
            foreach (var r in _buttonRects.Values)
                left = Math.Min(left, r.Left);
            return left == width ? width : Math.Max(0, left - ButtonSpacing);
        }

        private int FirstButtonTop(int height)
        {
            int top = height;
            foreach (var r in _buttonRects.Values)
                top = Math.Min(top, r.Top);
            return top == height ? height : Math.Max(0, top - ButtonSpacing);
        }

        /// <summary>Builds <see cref="CaptionTabModel"/> list from a set of <see cref="Models.DockPanel"/> instances,
        /// marking the panel that matches <paramref name="active"/> as active.</summary>
        internal static List<CaptionTabModel> BuildTabModels(IReadOnlyList<Models.DockPanel> panels, Models.DockPanel active)
        {
            var tabs = new List<CaptionTabModel>(panels.Count);
            foreach (var panel in panels)
            {
                tabs.Add(new CaptionTabModel
                {
                    Key = panel.Key,
                    Title = panel.Title,
                    IconPath = panel.IconPath,
                    IsDirty = panel.IsDirty,
                    IsActive = ReferenceEquals(panel, active),
                    Tag = panel
                });
            }
            return tabs;
        }
    }
}
