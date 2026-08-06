namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models
{
    /// <summary>
    /// What an action <i>is</i>, which decides how it is styled.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Destructive"/> is a role of its own rather than "primary in red". The references are
    /// explicit about this: `dialog2.png` and `dialog3.png` both put the dangerous action in saturated
    /// red and the safe one in muted grey, which is what Apple HIG and Material 3 specify — make the
    /// consequential action unmistakable rather than hiding it.
    /// </para>
    /// <para>
    /// Separating the roles is what lets a destructive action be the visually dominant control and
    /// still never take initial focus (stage 01). Collapsed into "primary with a different colour",
    /// there is nothing left to test that rule against.
    /// </para>
    /// </remarks>
    public enum DialogButtonRole
    {
        /// <summary>The action the dialog is asking for. Filled in the dialog's severity colour.</summary>
        Primary,

        /// <summary>An alternative action. Outlined or muted.</summary>
        Secondary,

        /// <summary>Commits something that cannot be undone. Always filled in the error colour.</summary>
        Destructive,

        /// <summary>Backs out. Muted, and always the safe landing place for Escape and initial focus.</summary>
        Cancel
    }

    /// <summary>Where a button's icon sits relative to its caption.</summary>
    /// <remarks>`dialog3.png`'s trash glyph trails its caption; leading is the more common default.</remarks>
    public enum DialogButtonIconPlacement
    {
        Leading,
        Trailing
    }

    /// <summary>
    /// Button corner treatment.
    /// </summary>
    /// <remarks>
    /// Shape varies by design rather than by role — `dialog1.png` uses rounded rectangles while
    /// `dialog3.png`, `dialog4.png` and `dialog5.png` use full pills. It is a property of the dialog,
    /// not of any one action.
    /// </remarks>
    public enum DialogButtonShape
    {
        /// <summary>The theme's own corner radius, so buttons match the rest of the library.</summary>
        Rounded,

        /// <summary>Fully rounded ends: radius is half the button height.</summary>
        Pill
    }
}
