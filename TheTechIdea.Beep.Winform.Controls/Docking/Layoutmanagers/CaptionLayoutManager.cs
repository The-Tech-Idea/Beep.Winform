using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Managers;
using TheTechIdea.Beep.Winform.Controls.Docking.Helpers;
using System.Windows.Forms;

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

        /// <summary>
        /// The font tab text is measured and drawn with.
        /// </summary>
        /// <remarks>
        /// One definition, used by both this layout and <c>CaptionRenderer</c>. Measuring with a
        /// different font than the one that draws is the defect that produces tabs whose text is
        /// clipped or adrift by a few pixels, and it is invisible until someone changes the theme
        /// font.
        /// </remarks>
        internal static Font CaptionFont => BeepFontManager.DefaultFont ?? SystemFonts.DefaultFont;

        /// <summary>
        /// Natural width of a tab: its text, plus the room the renderer reserves for the icon,
        /// padding and dirty marker.
        /// </summary>
        internal static int MeasureTabWidth(CaptionTabModel tab)
        {
            if (tab == null) return 0;

            // GDI+, matching the renderer's Graphics.DrawString. Measuring with TextRenderer (GDI)
            // instead reports a narrower string, and the tab is then laid out fractionally too
            // small - so every title draws with an ellipsis. Measured: "Explorer" came back as
            // "Expl...". The measurement has to use the same technology, font and format as the
            // draw, not merely a similar one.
            SizeF text;
            lock (MeasureLock)
            {
                using var g = Graphics.FromImage(MeasureSurface);
                text = g.MeasureString(tab.Title ?? "Panel", CaptionFont, int.MaxValue, TabFormat);
            }

            int chrome = DockingCaptionPainter.GetTabContentLeft(
                             DockingCaptionPainter.HasTabIcon(tab.IconPath))
                         + DockingCaptionPainter.TabTextPadding;

            // The dirty dot is drawn inside the tab's right edge; without room for it the text
            // runs underneath.
            if (tab.IsDirty)
                chrome += DirtyMarkerAllowance;

            return (int)Math.Ceiling(text.Width) + chrome;
        }

        /// <summary>Room reserved at a tab's right edge for the unsaved-changes dot.</summary>
        private const int DirtyMarkerAllowance = 12;

        // A 1x1 surface is enough to measure against; it exists only to supply a Graphics with the
        // same text rendering as the one that paints.
        private static readonly Bitmap MeasureSurface = new Bitmap(1, 1);
        private static readonly object MeasureLock = new object();

        /// <summary>
        /// Format tab text is measured and drawn with. Shared with <c>CaptionRenderer</c> so the
        /// two cannot drift - trimming and wrapping both change the width a string needs.
        /// </summary>
        internal static readonly StringFormat TabFormat = new StringFormat
        {
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.NoWrap
        };

        /// <summary>
        /// Lays tabs out at their natural width, shrinking them evenly only when they do not fit.
        /// </summary>
        /// <remarks>
        /// Previously every tab got <c>tabsWidth / count</c> clamped to <see cref="MaxTabWidth"/>,
        /// so all tabs were the same width whatever their label: a three-letter title occupied the
        /// same 160px as a long one and carried a block of dead space. Every reference product -
        /// VS Code, Visual Studio, Rider - sizes a tab to its title and only compresses when the
        /// strip runs out of room, which is what this does.
        /// <para>
        /// Compression is proportional rather than truncating the last tab, so the strip degrades
        /// evenly instead of one tab absorbing the whole shortfall.
        /// </para>
        /// </remarks>
        private void LayoutVisibleHorizontalTabs(IReadOnlyList<CaptionTabModel> tabs, int tabsWidth, int height)
        {
            int count = tabs.Count;
            if (count == 0) return;

            var natural = new int[count];
            long total = 0;
            for (int i = 0; i < count; i++)
            {
                natural[i] = Math.Max(MinTabWidth, Math.Min(MaxTabWidth, MeasureTabWidth(tabs[i])));
                total += natural[i];
            }

            // Shrink only if they genuinely do not fit, and never below the overflow floor - past
            // that the strip should be overflowing rather than showing unreadable stubs.
            double scale = total > tabsWidth && total > 0 ? tabsWidth / (double)total : 1.0;

            int tx = 0;
            for (int i = 0; i < count; i++)
            {
                if (tx >= tabsWidth) break;

                int w = scale < 1.0
                    ? Math.Max(Math.Min(OverflowMinTabWidth, natural[i]), (int)Math.Floor(natural[i] * scale))
                    : natural[i];

                w = Math.Min(w, tabsWidth - tx);
                if (w <= 0) break;

                _tabRects.Add(new KeyValuePair<CaptionTabModel, Rectangle>(
                    tabs[i], new Rectangle(tx, 0, w, height)));
                tx += w;
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
