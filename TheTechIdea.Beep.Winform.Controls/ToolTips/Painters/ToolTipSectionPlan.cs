namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Painters
{
    /// <summary>
    /// Which sections a tooltip draws, derived from its <see cref="ToolTipLayoutVariant"/>.
    /// <para>
    /// The variant used to have no effect at all: <c>ToolTipPainterFactory</c> mapped only Preview,
    /// Tour and Glass to their own painters, and the painter handling the remaining four —
    /// Simple, Rich, Card and Shortcut — never read the enum. Layout was inferred from which
    /// fields happened to be populated, so a caller who set <c>LayoutVariant = Card</c> got exactly
    /// what they would have got with <c>Simple</c>, and setting a Title silently turned a "Simple"
    /// tooltip into a Rich one.
    /// </para>
    /// <para>
    /// Making the variant authoritative means each one has a stated contract, which is also what
    /// makes them testable — "all seven render distinctly" is only meaningful if each one decides
    /// something.
    /// </para>
    /// </summary>
    internal readonly struct ToolTipSectionPlan
    {
        /// <summary>Render <see cref="ToolTipConfig.Title"/> as a heading.</summary>
        public bool ShowTitle { get; init; }

        /// <summary>Draw a rule between the heading and the body (Card).</summary>
        public bool ShowHeaderDivider { get; init; }

        /// <summary>Render keyboard shortcut badges.</summary>
        public bool ShowShortcuts { get; init; }

        /// <summary>
        /// Place the badges on the body's own baseline rather than in a footer — the compact
        /// single-row Shortcut layout.
        /// </summary>
        public bool ShortcutsInline { get; init; }

        /// <summary>
        /// Resolves the plan for a config.
        /// </summary>
        public static ToolTipSectionPlan For(ToolTipConfig config) => config.LayoutVariant switch
        {
            // One line of text. A title is deliberately ignored: "Simple" means simple, and
            // silently upgrading to a two-line layout because a Title happened to be set is the
            // inference behaviour this replaces.
            ToolTipLayoutVariant.Simple => new ToolTipSectionPlan
            {
                ShowTitle = false,
                ShowHeaderDivider = false,
                ShowShortcuts = false,
                ShortcutsInline = false
            },

            // Title + body (Material rich tooltip).
            ToolTipLayoutVariant.Rich => new ToolTipSectionPlan
            {
                ShowTitle = true,
                ShowHeaderDivider = false,
                ShowShortcuts = false,
                ShortcutsInline = false
            },

            // Header / divider / body / footer (DevExpress SuperTip).
            ToolTipLayoutVariant.Card => new ToolTipSectionPlan
            {
                ShowTitle = true,
                ShowHeaderDivider = true,
                ShowShortcuts = true,
                ShortcutsInline = false
            },

            // Compact single row: label left, key caps right, same baseline.
            ToolTipLayoutVariant.Shortcut => new ToolTipSectionPlan
            {
                ShowTitle = false,
                ShowHeaderDivider = false,
                ShowShortcuts = true,
                ShortcutsInline = true
            },

            // Preview / Tour / Glass have dedicated painters; if one of them reaches this painter
            // (an unmapped future variant, say) fall back to the permissive behaviour rather than
            // rendering nothing.
            _ => new ToolTipSectionPlan
            {
                ShowTitle = true,
                ShowHeaderDivider = false,
                ShowShortcuts = true,
                ShortcutsInline = false
            }
        };
    }
}
