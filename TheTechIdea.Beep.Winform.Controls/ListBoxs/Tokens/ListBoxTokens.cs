namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens
{
    /// <summary>
    /// Design-token constants for BeepListBox.
    /// All painters MUST reference these instead of magic numbers.
    /// Values are logical pixels; scale via DpiScalingHelper.ScaleValue(value, control).
    /// </summary>
    public static class ListBoxTokens
    {
        // ── Item heights ─────────────────────────────────────────────────────────

        /// <summary>Comfortable density — enough breathing room for title + sub-text.</summary>
        public const int ItemHeightComfortable = 52;

        /// <summary>Compact density — title only, modest padding.</summary>
        public const int ItemHeightCompact = 40;

        /// <summary>Dense density — minimal padding, data-grid-like rows.</summary>
        public const int ItemHeightDense = 28;

        // ── Group headers / separators ────────────────────────────────────────────

        public const int GroupHeaderHeight = 28;
        public const int SeparatorHeight   = 20;

        // ── Avatars / icons / checkboxes ──────────────────────────────────────────

        public const int AvatarSize    = 36;
        public const int IconSize      = 20;
        public const int CheckboxSize  = 18;

        // ── Badges ────────────────────────────────────────────────────────────────

        public const int BadgePillRadius    = 10;   // px corner radius of badge pill
        public const int BadgeFontSize      = 9;    // pt
        public const int BadgeMinWidth      = 20;   // px — avoid tiny oval for single digit

        // ── Selection / focus visuals ─────────────────────────────────────────────

        public const int FocusRingThickness    = 2;
        public const int PinnedAccentBarWidth  = 3;
        public const int SelectionCornerRadius = 6;

        // ── Opacity / alpha channels (0-255) ──────────────────────────────────────

        public const int SubTextAlpha       = 140;   // ≈55 % — secondary text
        public const int DisabledAlpha      = 100;   // ≈39 % — greyed-out items
        public const int HoverOverlayAlpha  = 18;    // ≈7 %  — subtle hover fill
        public const int ActiveOverlayAlpha = 38;    // ≈15 % — pressed/active fill
        public const int SkeletonAlpha      = 30;    // shimmer base layer

        // ── Spacing ───────────────────────────────────────────────────────────────

        public const int ItemPaddingH = 12;   // horizontal padding inside row
        public const int ItemPaddingV = 6;    // vertical padding inside row
        public const int IconTextGap  = 10;   // gap between icon and title
        public const int SubTextGap   = 2;    // gap between title and sub-text

        // ── Animation timing (milliseconds) ───────────────────────────────────────

        public const int HoverFadeDurationMs  = 200;
        public const int SkeletonCycleDurationMs = 1500;
        public const int TypeAheadClearMs        = 800;

        // ── Minimum touch target (WCAG 2.5.5) ────────────────────────────────────

        public const int MinTouchTargetPx = 44;

        // ── Scrollbar ─────────────────────────────────────────────────────────────

        public const int ScrollbarTrackWidth   = 6;
        public const int ScrollbarThumbMinLen  = 24;
        public const int ScrollbarAutoHideMs   = 1500;

        // ── Search bar ────────────────────────────────────────────────────────────

        public const int SearchBarHeight   = 40;
        public const int SearchCornerRadius = 8;
        public const int SearchIconSize    = 16;

        // ── Empty state ───────────────────────────────────────────────────────────

        public const int EmptyStateIconSize    = 48;
        public const int EmptyStateHeadlinePt  = 13;
        public const int EmptyStateSubTextPt   = 10;

        // ── Hierarchy / tree ──────────────────────────────────────────────────────

        /// <summary>Horizontal indent per nesting level (px).</summary>
        public const int IndentStepPerLevel = 24;

        /// <summary>Expand/collapse chevron icon size (px).</summary>
        public const int ChevronSize = 16;

        /// <summary>Hit-target around chevron for click detection (px).</summary>
        public const int ChevronHitTarget = 28;

        /// <summary>Maximum supported nesting depth.</summary>
        public const int MaxHierarchyDepth = 10;

        // ── Rich list type tokens (Phase 7) ──────────────────────────────────────

        public const int ChatRowHeight = 72;
        public const int ChatAvatarSize = 52;
        public const int ContactRowHeight = 72;
        public const int ContactAvatarSize = 48;
        public const int ThreeLineRowHeight = 88;
        public const int ThreeLineImageSize = 48;
        public const int ThreeLineImageRadius = 4;
        public const int NotificationRowHeight = 80;
        public const int NotificationIconSize = 40;
        public const int ProfileCardRowHeight = 120;
        public const int ProfileCardAvatarSize = 64;
        public const int RichListHMargin = 16;
        public const int AvatarTextGap = 16;
        public const int ChatTrailingWidth = 64;
        public const int NotificationTrailingWidth = 56;
    }
}
