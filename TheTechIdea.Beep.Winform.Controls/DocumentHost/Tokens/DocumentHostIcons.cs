// DocumentHostIcons.cs
// Semantic icon name → SvgsUI constant mapping for the DocumentHost subsystem.
// Consumers reference these properties instead of raw SvgsUI string paths,
// keeping icon wiring decoupled from the SVG asset library.
// ─────────────────────────────────────────────────────────────────────────────

using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens
{
    /// <summary>
    /// Maps DocumentHost semantic icon names to resolved <see cref="SvgsUI"/> asset paths.
    /// </summary>
    public static class DocumentHostIcons
    {
        // ── Tab chrome ────────────────────────────────────────────────────────

        /// <summary>Close (×) button on each tab.</summary>
        public static string Close => SvgsUI.CircleX;

        /// <summary>Add new document button (+).</summary>
        public static string Add => SvgsUI.Plus;

        /// <summary>Pin tab to keep it visible.</summary>
        public static string Pin => SvgsUI.Pin;

        /// <summary>Unpin (already-pinned) tab.</summary>
        public static string Unpin => SvgsUI.Pinned;

        // ── Navigation / overflow ─────────────────────────────────────────────

        /// <summary>Scroll / navigate left in the tab strip.</summary>
        public static string ChevronLeft => SvgsUI.CircleChevronLeft;

        /// <summary>Scroll / navigate right in the tab strip.</summary>
        public static string ChevronRight => SvgsUI.ChevronRight;

        /// <summary>Overflow dropdown arrow (⋯ / ∨).</summary>
        public static string ChevronDown => SvgsUI.ChevronDown;

        // ── Document state ────────────────────────────────────────────────────

        /// <summary>Generic document icon used in the empty-state illustration.</summary>
        public static string Document => SvgsUI.FileText;

        /// <summary>Dirty / modified indicator — shown on unsaved tabs.</summary>
        public static string Modified => SvgsUI.CircleX;   // closest available; swap when a dot icon is added

        // ── Split / float ──────────────────────────────────────────────────────

        /// <summary>Float document into its own window.</summary>
        public static string Float => SvgsUI.LayoutBoardSplit;

        /// <summary>Split view horizontally.</summary>
        public static string SplitHorizontal => SvgsUI.LayoutBoardSplit;

        /// <summary>Split view vertically.</summary>
        public static string SplitVertical => SvgsUI.LayoutBoardSplit;

        // ── Dock compass directions ───────────────────────────────────────────

        /// <summary>Dock to left half.</summary>
        public static string DockLeft => SvgsUI.CircleChevronLeft;

        /// <summary>Dock to right half.</summary>
        public static string DockRight => SvgsUI.CircleChevronRight;

        /// <summary>Dock to top half.</summary>
        public static string DockTop => SvgsUI.CircleChevronUp;

        /// <summary>Dock to bottom half.</summary>
        public static string DockBottom => SvgsUI.CircleChevronDown;

        /// <summary>Dock to centre (merge / re-dock).</summary>
        public static string DockCenter => SvgsUI.CirclePlus;

        // ── Tooling ───────────────────────────────────────────────────────────

        /// <summary>Command palette / quick-switch search icon.</summary>
        public static string Search => SvgsUI.Search;

        /// <summary>Keyboard shortcuts hint icon.</summary>
        public static string Keyboard => SvgsUI.Keyboard;
    }
}
