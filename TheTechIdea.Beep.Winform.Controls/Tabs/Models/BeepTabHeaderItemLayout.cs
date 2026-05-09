using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// Layout data for one tab header item.
    /// Carries both core bounds (text, close) and Phase 2 adornment bounds
    /// (icon, subtext, badge, dirty marker, busy indicator).
    /// </summary>
    public sealed class BeepTabHeaderItemLayout
    {
        // ── Core ─────────────────────────────────────────────────────────────
        public BeepTabItem Item { get; set; } = new BeepTabItem();
        public Rectangle Bounds { get; set; } = Rectangle.Empty;
        public Rectangle TextBounds { get; set; } = Rectangle.Empty;
        public Rectangle CloseButtonBounds { get; set; } = Rectangle.Empty;
        public bool HasCloseButton { get; set; }

        // ── Phase 2: adornment bounds ────────────────────────────────────────

        /// <summary>Bounds for the leading icon glyph. Empty when no icon.</summary>
        public Rectangle IconBounds { get; set; } = Rectangle.Empty;

        /// <summary>Bounds for the secondary subtext label. Empty when no subtext.</summary>
        public Rectangle SubTextBounds { get; set; } = Rectangle.Empty;

        /// <summary>Bounds for the badge chip (count, dot, status). Empty when no badge.</summary>
        public Rectangle BadgeBounds { get; set; } = Rectangle.Empty;

        /// <summary>Bounds for the dirty-dot marker (unsaved indicator). Empty when not dirty.</summary>
        public Rectangle DirtyMarkerBounds { get; set; } = Rectangle.Empty;

        /// <summary>Bounds for the busy/spinner indicator. Empty when not busy.</summary>
        public Rectangle BusyIndicatorBounds { get; set; } = Rectangle.Empty;

        // ── Hit testing ───────────────────────────────────────────────────────

        public bool HitTest(Point point) => Bounds.Contains(point);

        public bool HitTestCloseButton(Point point) =>
            HasCloseButton && CloseButtonBounds.Contains(point);

        public bool HitTestIcon(Point point) =>
            !IconBounds.IsEmpty && IconBounds.Contains(point);

        public bool HitTestBadge(Point point) =>
            !BadgeBounds.IsEmpty && BadgeBounds.Contains(point);
    }
}