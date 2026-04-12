// DocumentHostTokens.cs
// Centralised design tokens for the DocumentHost subsystem.
// All geometry, timing and sizing constants are sourced from here.
// Update this file to globally adjust spacing / animation without hunting the codebase.
// ─────────────────────────────────────────────────────────────────────────────────────────

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens
{
    /// <summary>
    /// Design token constants for the <see cref="BeepDocumentHost"/> subsystem.
    /// Based on Material Design 3, Fluent UI 2 and Figma 2026 tab-component alignment.
    /// </summary>
    public static class DocTokens
    {
        // ─────────────────────────────────────────────────────────────────────
        // Tab strip — geometry
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Default tab strip height (px). MD3 NavigationBar item height.</summary>
        public const int TabHeight = 36;

        /// <summary>Compact density tab height (px).</summary>
        public const int TabHeightCompact = 28;

        /// <summary>Dense density tab height (px).</summary>
        public const int TabHeightDense = 22;

        /// <summary>Horizontal padding inside each tab (px). Fluent 2 standard.</summary>
        public const int TabPaddingH = 16;

        /// <summary>Vertical padding inside each tab (px).</summary>
        public const int TabPaddingV = 8;

        /// <summary>Minimum tab width (px).</summary>
        public const int TabMinWidth = 80;

        /// <summary>Maximum tab width in Fixed/Equal modes (px). Matches Chrome 2025.</summary>
        public const int TabMaxWidth = 240;

        /// <summary>Width of a pinned tab (icon-only, px).</summary>
        public const int PinnedTabWidth = 48;

        /// <summary>Tab icon size (px). Same as small-icon size in Windows shell.</summary>
        public const int TabIconSize = 16;

        /// <summary>Corner radius for Chrome-style tabs (px). Matches Edge 2025.</summary>
        public const int TabCornerRadius = 6;

        /// <summary>Corner radius for Pill-style tabs (px). Half-height rounding.</summary>
        public const int TabPillCornerRadius = 14;

        /// <summary>Active indicator bar thickness (underline/VSCode styles) in px.</summary>
        public const int IndicatorThickness = 3;

        /// <summary>Width of the close × button hit area (px).</summary>
        public const int CloseButtonSize = 18;

        /// <summary>Gap between close button and right edge of tab (px).</summary>
        public const int CloseButtonMarginRight = 6;

        /// <summary>Gap between icon and title text (px).</summary>
        public const int IconTextGap = 6;

        // ─────────────────────────────────────────────────────────────────────
        // Tab badge
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Badge pill diameter (px).</summary>
        public const int BadgeSize = 16;

        /// <summary>Badge font size (pt).</summary>
        public const int BadgeFontSize = 10;

        /// <summary>Badge right-edge offset from tab right (px).</summary>
        public const int BadgeMarginRight = 4;

        /// <summary>Badge top-edge offset from tab top (px).</summary>
        public const int BadgeMarginTop = 4;

        // ─────────────────────────────────────────────────────────────────────
        // Strip border
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Bottom border of the tab strip (separator line) thickness (px).</summary>
        public const int StripBorderWidth = 1;

        /// <summary>Add (+) button width (px).</summary>
        public const int AddButtonWidth = 28;

        /// <summary>Overflow (⋯) dropdown button width (px).</summary>
        public const int OverflowButtonWidth = 28;

        // ─────────────────────────────────────────────────────────────────────
        // Splitter
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Splitter bar width/height between document groups (px).</summary>
        public const int SplitterBarThickness = 5;

        /// <summary>Minimum pane size so a group is never squashed below visibility (px).</summary>
        public const int SplitterMinPaneSize = 80;

        // ─────────────────────────────────────────────────────────────────────
        // Status bar
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Document status bar height (px). Matches VS Code status bar height.</summary>
        public const int StatusBarHeight = 22;

        /// <summary>Status bar font size (pt).</summary>
        public const int StatusBarFontSize = 10;

        // ─────────────────────────────────────────────────────────────────────
        // Auto-hide strip
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Width/height of the auto-hide strip tab buttons (px).</summary>
        public const int AutoHideStripSize = 26;

        // ─────────────────────────────────────────────────────────────────────
        // Float window
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Default float window width (px).</summary>
        public const int FloatDefaultWidth = 800;

        /// <summary>Default float window height (px).</summary>
        public const int FloatDefaultHeight = 600;

        // ─────────────────────────────────────────────────────────────────────
        // Animations (milliseconds)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Active indicator slide animation duration (ms).</summary>
        public const int IndicatorAnimMs = 200;

        /// <summary>Tab close fade-out animation duration (ms).</summary>
        public const int CloseFadeMs = 150;

        /// <summary>Auto-hide panel slide animation duration (ms).</summary>
        public const int SlideOverlayMs = 180;

        /// <summary>Dock guide hover highlight transition (ms).</summary>
        public const int DockGuideHoverMs = 80;

        /// <summary>Group collapse animation duration (ms).</summary>
        public const int GroupCollapseMs = 200;

        /// <summary>Rich tooltip appear delay (ms). Matches VS Code tooltip delay.</summary>
        public const int TooltipDelayMs = 800;

        /// <summary>Rich tooltip fade-out on mouse leave (ms).</summary>
        public const int TooltipFadeOutMs = 100;

        // ─────────────────────────────────────────────────────────────────────
        // Responsive breakpoints (strip width in px)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Above this width: full tab (icon + title + close).</summary>
        public const int ResponsiveNormal = 480;

        /// <summary>480..this width: compact tab (title + close-on-hover).</summary>
        public const int ResponsiveCompact = 240;

        /// <summary>240..this width: icon-only + overflow.</summary>
        public const int ResponsiveIconOnly = 120;

        // ─────────────────────────────────────────────────────────────────────
        // Alpha / opacity
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Opacity of inactive tab text (0–255). Gives visual hierarchy.</summary>
        public const int InactiveTabTextAlpha = 178;   // ≈ 70 %

        /// <summary>Opacity of pill background on inactive tabs (0–255). MD3 standard.</summary>
        public const int InactivePillAlpha = 38;        // ≈ 15 %

        /// <summary>Hover fill overlay opacity (0–255). Light tint on hover.</summary>
        public const int HoverOverlayAlpha = 26;        // ≈ 10 %

        // ─────────────────────────────────────────────────────────────────────
        // Docking guide
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Size of each direction arrow button in the dock compass (px).</summary>
        public const int DockGuideButtonSize = 32;

        /// <summary>Gap between compass buttons (px).</summary>
        public const int DockGuideButtonGap = 4;

        /// <summary>Dock guide opacity (0–255) when companion window is dragged near.</summary>
        public const int DockGuideOpacity = 220;

        /// <summary>Drop preview rectangle fill alpha (0–255).</summary>
        public const int DockPreviewAlpha = 48;

        // ─────────────────────────────────────────────────────────────────────
        // Thumbnail preview
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Thumbnail preview width inside rich tooltip (px).</summary>
        public const int ThumbnailWidth = 200;

        /// <summary>Thumbnail preview height inside rich tooltip (px).</summary>
        public const int ThumbnailHeight = 120;

        /// <summary>Corner radius applied to thumbnail (px).</summary>
        public const int ThumbnailCornerRadius = 4;

        // ─────────────────────────────────────────────────────────────────────
        // Missing tokens added by Task 1.8
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Preferred (ideal) tab width before overflow kicks in (px). Between min and max.</summary>
        public const int TabPreferredWidth = 180;

        /// <summary>Icon size inside a pinned (icon-only) tab (px).</summary>
        public const int PinnedTabIconSize = 16;

        /// <summary>Extended hit-test hotspot width for the splitter bar (px). Wider than visual thickness.</summary>
        public const int SplitterHotspotWidth = 8;

        /// <summary>Visual size of each dock-zone arrow button in the overlay compass (px).</summary>
        public const int DockZoneSize = 32;

        /// <summary>Stroke thickness of dock-guide arrow lines (px).</summary>
        public const int DockGuideThickness = 2;

        /// <summary>Minimum badge diameter — enforces readability at small sizes (px).</summary>
        public const int BadgeMinSize = 14;

        /// <summary>Horizontal padding inside a badge pill between digit and edge (px).</summary>
        public const int BadgePadding = 3;

        /// <summary>Maximum width of the rich tooltip popup (px).</summary>
        public const int TooltipMaxWidth = 320;
    }
}
