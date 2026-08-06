namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models
{
    /// <summary>
    /// A secondary severity message shown inside a dialog that already has one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The element none of the dialog forms could produce, and the most consequential gap in stage 08.
    /// `dialog3.png` uses it for <i>"By deleting this media <b>8 connected hotspots</b> will also be
    /// deleted."</i> — which is the whole reason the pattern exists. It turns "are you sure?" into
    /// "here is specifically what else you are about to destroy".
    /// </para>
    /// <para>
    /// A callout carries <b>its own</b> severity, not the dialog's: a warning callout inside an error
    /// dialog is the case `dialog3.png` actually shows, and collapsing the two would lose the
    /// distinction the pattern is for.
    /// </para>
    /// </remarks>
    public sealed class DialogCallout
    {
        /// <summary>What this callout means, independent of the dialog's own severity.</summary>
        public DialogSeverity Severity { get; set; } = DialogSeverity.Warning;

        /// <summary>Optional heading — "Warning" in `dialog3.png`. Omitted when empty.</summary>
        public string? Label { get; set; }

        /// <summary>The message body.</summary>
        public string Text { get; set; } = string.Empty;

        public DialogCallout() { }

        public DialogCallout(DialogSeverity severity, string text, string? label = null)
        {
            Severity = severity;
            Text = text;
            Label = label;
        }
    }
}
