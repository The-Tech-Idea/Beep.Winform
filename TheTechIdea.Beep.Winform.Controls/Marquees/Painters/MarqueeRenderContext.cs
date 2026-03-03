using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Marquees;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Painters
{
    /// <summary>
    /// Sprint 2 — Immutable rendering context passed from <see cref="BeepMarquee"/>
    /// to every <see cref="IMarqueeItemRenderer"/>.
    /// </summary>
    public class MarqueeRenderContext
    {
        // ── Theme ────────────────────────────────────────────────────
        public IBeepTheme Theme           { get; set; }
        public bool       UseThemeColors  { get; set; } = true;
        public Color      DefaultForeColor { get; set; } = Color.Black;
        public Color      DefaultBackColor { get; set; } = Color.White;

        // ── Typography ────────────────────────────────────────────────
        public Font       ItemFont        { get; set; }

        // ── DPI ───────────────────────────────────────────────────────
        /// <summary>Owner control; used for DPI-aware scaling.</summary>
        public Control    OwnerControl    { get; set; }

        // ── Layout ───────────────────────────────────────────────────
        public MarqueeScrollDirection Direction { get; set; } = MarqueeScrollDirection.RightToLeft;
        public int        ItemHeight      { get; set; } = 36;
        public int        Padding         { get; set; } = 8;
        public int        ImageSize       { get; set; } = 20;

        // ── Interaction ──────────────────────────────────────────────
        /// <summary>Zero-based index of the currently hovered item (-1 = none).</summary>
        public int        HoveredItemIndex { get; set; } = -1;

        /// <summary>Current item index being rendered (set by control each Draw call).</summary>
        public int        CurrentItemIndex { get; set; } = -1;

        // ── Animation ────────────────────────────────────────────────
        /// <summary>Alpha 0–1 used by NewsTicker mode for fade transitions.</summary>
        public float      NewsAlpha        { get; set; } = 1f;

        // ── Fade ─────────────────────────────────────────────────────
        public bool       FadeEdges        { get; set; }
        public int        FadeWidth        { get; set; } = 40;
    }
}
