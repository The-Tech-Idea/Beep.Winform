namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// What the header does when the tab run no longer fits.
    /// </summary>
    /// <remarks>
    /// This enum previously also declared <c>ScrollButtons</c>, <c>ShrinkToFit</c> and
    /// <c>Multiline</c>. Nothing anywhere honoured them — zero references across the assembly — so
    /// selecting one behaved like <see cref="None"/> except worse: tabs were dropped from the run
    /// *and* no overflow menu appeared, because the menu is gated on <see cref="OverflowMenu"/>.
    /// They were offered in the designer's property grid as though they worked. Removed rather than
    /// left as a promise the control does not keep.
    /// </remarks>
    public enum BeepTabOverflowPolicy
    {
        /// <summary>
        /// Lay every tab out regardless of available width. Tabs that do not fit are clipped and
        /// unreachable — appropriate only when the caller guarantees the tabs fit.
        /// </summary>
        None,

        /// <summary>
        /// Show as many tabs as fit and put the rest behind an overflow menu. Pinned tabs and the
        /// selected tab are always kept in the visible run.
        /// </summary>
        OverflowMenu
    }
}
