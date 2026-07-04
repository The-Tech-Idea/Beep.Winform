using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models
{
    /// <summary>
    /// Phase 14 — typed button model replacing the flat
    /// <see cref="Vis.Modules.BeepDialogButtons"/> enum array.
    /// Each button carries its own label, styling hints, and optional
    /// click callback. Each dialog form applies styled buttons.
    /// </summary>
    public class DialogButton
    {
        /// <summary>Display text. Maps to <see cref="BeepButton.Text"/>.</summary>
        public string Text { get; set; }

        /// <summary>
        /// Stable identifier ("ok", "cancel", "delete", "yes", "no", "save", ...).
        /// Used to match legacy <see cref="Vis.Modules.BeepDialogButtons"/> values
        /// during conversion so that ReturnValue
        /// and the dialog result still surface the right semantic.
        /// </summary>
        public string Id { get; set; }

        /// <summary>When false the button is drawn but cannot be clicked.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>When false the button is hidden entirely.</summary>
        public bool Visible { get; set; } = true;

        /// <summary>Primary styling — filled accent or severity colour.</summary>
        public bool IsPrimary { get; set; }

        /// <summary>Ghost styling — flat, no background fill.</summary>
        public bool IsGhost { get; set; }

        /// <summary>Danger styling — red background for destructive actions.</summary>
        public bool IsDanger { get; set; }

        /// <summary>Tooltip shown on hover.</summary>
        public string? ToolTip { get; set; }

        /// <summary>
        /// Optional inline handler. Fires before the dialog closes.
        /// Return <c>false</c> to prevent the dialog from closing
        /// (useful for validation or async operations).
        /// </summary>
        public Func<Form, bool>? OnClick { get; set; }

        /// <summary>Returns a fully configured OK button.</summary>
        public static DialogButton Ok(string text = "OK") => new()
        {
            Text = text, Id = "ok", IsPrimary = true, IsGhost = false
        };

        /// <summary>Returns a fully configured Cancel (ghost) button.</summary>
        public static DialogButton Cancel(string text = "Cancel") => new()
        {
            Text = text, Id = "cancel", IsPrimary = false, IsGhost = true
        };

        /// <summary>Returns a fully configured Yes (primary) button.</summary>
        public static DialogButton Yes(string text = "Yes") => new()
        {
            Text = text, Id = "yes", IsPrimary = true
        };

        /// <summary>Returns a fully configured No (ghost) button.</summary>
        public static DialogButton No(string text = "No") => new()
        {
            Text = text, Id = "no", IsPrimary = false, IsGhost = true
        };

        /// <summary>Returns a Danger-styled destructive button.</summary>
        public static DialogButton Delete(string text = "Delete") => new()
        {
            Text = text, Id = "delete", IsPrimary = true, IsDanger = true
        };

        /// <summary>Returns a Ghost-styled "Don't Save" button.</summary>
        public static DialogButton DontSave(string text = "Don't Save") => new()
        {
            Text = text, Id = "dontsave", IsPrimary = false, IsGhost = true
        };

        /// <summary>Returns a primary "Save" button.</summary>
        public static DialogButton Save(string text = "Save") => new()
        {
            Text = text, Id = "save", IsPrimary = true
        };

        /// <summary>Converts a legacy enum to the equivalent <see cref="DialogButton"/>.</summary>
        internal static DialogButton FromLegacy(Vis.Modules.BeepDialogButtons legacy, string? customLabel = null)
        {
            var label = customLabel ?? legacy.ToString();
            return legacy switch
            {
                Vis.Modules.BeepDialogButtons.Ok       => Ok(label),
                Vis.Modules.BeepDialogButtons.Cancel   => Cancel(label),
                Vis.Modules.BeepDialogButtons.Yes      => Yes(label),
                Vis.Modules.BeepDialogButtons.No       => No(label),
                Vis.Modules.BeepDialogButtons.Abort    => new DialogButton { Text = label, Id = "abort",   IsPrimary = false, IsGhost = true },
                Vis.Modules.BeepDialogButtons.Retry    => new DialogButton { Text = label, Id = "retry",   IsPrimary = true },
                Vis.Modules.BeepDialogButtons.Ignore   => new DialogButton { Text = label, Id = "ignore",  IsPrimary = false, IsGhost = true },
                Vis.Modules.BeepDialogButtons.Close    => new DialogButton { Text = label, Id = "close",   IsPrimary = false, IsGhost = true },
                Vis.Modules.BeepDialogButtons.Help     => new DialogButton { Text = label, Id = "help",    IsPrimary = false, IsGhost = true },
                Vis.Modules.BeepDialogButtons.Continue => new DialogButton { Text = label, Id = "continue", IsPrimary = true },
                _ => new DialogButton { Text = label, Id = legacy.ToString().ToLowerInvariant(), IsPrimary = true }
            };
        }
    }
}
