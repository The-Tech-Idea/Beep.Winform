using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models
{
    /// <summary>
    /// Phase 13 — multi-page navigation model.
    /// A page represents one step inside a multi-page wizard dialog.
    /// Each page carries its own title, message, icon, buttons, and
    /// lifecycle hooks (<see cref="OnCreated"/>, <see cref="OnDestroyed"/>).
    /// <para>
    /// The dialog form's Navigate method
    /// replaces the current page's content without closing the form.
    /// </para>
    /// </summary>
    public class DialogPage
    {
        /// <summary>Page title shown in the header.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Message body text.</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Optional icon path (overrides the parent dialog's icon).</summary>
        public string? IconPath { get; set; }

        /// <summary>Page-specific button set. Defaults to Next (primary).</summary>
        public BeepDialogButtons[] Buttons { get; set; } = { BeepDialogButtons.Ok };

        /// <summary>
        /// Optional custom button labels for this page.
        /// Maps <see cref="BeepDialogButtons"/> to the display label.
        /// </summary>
        public Dictionary<BeepDialogButtons, string>? CustomButtonLabels { get; set; }

        /// <summary>
        /// Optional validation callback. Return an error string to show
        /// inline validation; return null/empty to allow navigation.
        /// </summary>
        public Func<string, string?>? InputValidator { get; set; }

        /// <summary>Default value for input dialogs on this page.</summary>
        public string? InputDefaultValue { get; set; }

        /// <summary>Items for list-selection dialogs on this page.</summary>
        public IReadOnlyList<SimpleItem>? Items { get; set; }

        /// <summary>
        /// Fired when this page becomes the active page.
        /// Use to initialise data, reset state, or start async operations.
        /// </summary>
        public Action<Form>? OnCreated { get; set; }

        /// <summary>
        /// Fired when the dialog navigates AWAY from this page
        /// (either forward or backward). Use to clean up resources.
        /// </summary>
        public Action<Form>? OnDestroyed { get; set; }

        /// <summary>Arbitrary metadata hook for user-defined state.</summary>
        public object? Tag { get; set; }
    }
}
