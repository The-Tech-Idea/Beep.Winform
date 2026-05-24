using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// No-op painter used at design time so that <see cref="BeepDockingManager._painter"/>
    /// is never <c>null</c>.  All draw methods do nothing; color/size properties return
    /// safe system defaults.  Safe to use in the Visual Studio designer process.
    /// </summary>
    internal sealed class NullDockingPainter : IDockingPainter
    {
        public static readonly NullDockingPainter Instance = new();

        private NullDockingPainter() { }

        // ── Colors ───────────────────────────────────────────────────────────────
        public Color BackgroundColor => SystemColors.Control;
        public Color ForegroundColor => SystemColors.ControlText;
        public Color BorderColor     => SystemColors.ControlDark;
        public Color HoverColor      => SystemColors.ControlLight;
        public Color SelectedColor   => SystemColors.Highlight;
        public Color DisabledColor   => SystemColors.GrayText;

        // ── Fonts ────────────────────────────────────────────────────────────────
        public Font UIFont  => SystemFonts.DefaultFont;
        public Font TabFont => SystemFonts.DefaultFont;

        // ── Metrics ──────────────────────────────────────────────────────────────
        public int TabStripHeight => 24;
        public int SplitterWidth  => 4;

        // ── Draw methods (all no-ops) ─────────────────────────────────────────────
        public void DrawTabStrip(Graphics graphics, Rectangle bounds, TabInfo[] tabs,
                                 int activeTabIndex, Action<int> onTabClicked) { }

        public void DrawTab(Graphics graphics, Rectangle bounds, TabInfo tab,
                            bool isActive, bool isHovered) { }

        public void DrawPanelChrome(Graphics graphics, Rectangle bounds, string title,
                                    Image icon, bool isDirty) { }

        public void DrawSplitter(Graphics graphics, Rectangle bounds,
                                 SplitterOrientation orientation, bool isHovered) { }

        public void DrawDockingGuide(Graphics graphics, Rectangle bounds, DockPosition position) { }

        // ── Query methods ─────────────────────────────────────────────────────────
        public Size GetTabStripPreferredSize(TabInfo[] tabs, int availableWidth)
            => new(availableWidth, TabStripHeight);

        public int GetTabAtPoint(Point point, Rectangle bounds, TabInfo[] tabs) => -1;

        public Rectangle GetTabCloseButtonRect(Rectangle tabBounds, TabInfo tab) => Rectangle.Empty;

        public void InvalidateCache() { }

        public void Dispose() { }
    }
}
