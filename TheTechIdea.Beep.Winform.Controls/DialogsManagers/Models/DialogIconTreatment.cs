namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models
{
    /// <summary>
    /// How a dialog icon's glyph is presented, drawn by <c>BeepImage</c> rather
    /// than the dialog folder — a circle around a glyph is drawing, and drawing
    /// belongs to the control.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The four shapes come straight from the reference images:
    /// <c>CircleOutline</c> is `dialog1.png`'s outlined severity glyph,
    /// <c>Bare</c> is `dialog2.png`'s containerless warning triangle,
    /// <c>CircleFilled</c> is `dialog4.png`/`dialog5.png`'s filled circle with a
    /// contrast glyph, and <c>Oversized</c> is `dialog6.png`'s enlarged bare mark
    /// (no container; size is a presentation concern).
    /// </para>
    /// <para>
    /// The glyph colour always resolves from the severity accent; a filled circle
    /// inverts the glyph to the contrast colour so it stays legible on the fill.
    /// </para>
    /// </remarks>
    public enum DialogIconTreatment
    {
        /// <summary>The glyph alone, severity-coloured, with no container.</summary>
        Bare,

        /// <summary>An outlined circle in the severity colour, glyph in the same colour.</summary>
        CircleOutline,

        /// <summary>A filled circle in the severity colour, glyph in the contrast colour.</summary>
        CircleFilled,

        /// <summary>The bare glyph, enlarged — the circle is replaced by size.</summary>
        Oversized
    }
}
