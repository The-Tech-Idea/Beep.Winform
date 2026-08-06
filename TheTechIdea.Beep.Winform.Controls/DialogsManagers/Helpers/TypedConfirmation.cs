using System;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    /// <summary>
    /// The confirmation step a destructive dialog promises: type the keyword before the action unlocks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>RequireTypedConfirmation</c> and <c>ConfirmationKeyword</c> had <b>zero readers</b>.
    /// <c>DialogConfig.CreateDestructive</c> names Linear and Vercel, sets the flag those products'
    /// patterns are built on, and produced a dialog whose destructive button was live from the moment
    /// it opened. A caller asking for the strongest confirmation the API offers received the weakest —
    /// which is why this is separated from the other dead-property findings: the rest make a dialog
    /// less capable, this one made it less safe than its author believed.
    /// </para>
    /// <para>
    /// The typed step exists to defeat muscle memory, and its whole value is that it cannot be
    /// completed without reading. So the field is never pre-filled and the comparison is exact —
    /// ordinal, no trimming, no case folding. A confirmation that accepts "DELETE" for "delete" is one
    /// the user can complete without looking at it.
    /// </para>
    /// </remarks>
    internal static class TypedConfirmation
    {
        /// <summary>
        /// Inserts the confirmation row and gates the primary action behind it.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The dialog asks for a typed confirmation without saying what to type. Treating that as "no
        /// confirmation needed" is exactly how this class of defect returns, so it is loud.
        /// </exception>
        public static void Apply(Form dialog, DialogConfig config)
        {
            if (dialog == null || config == null) return;
            if (!config.RequireTypedConfirmation) return;

            if (string.IsNullOrEmpty(config.ConfirmationKeyword))
            {
                throw new InvalidOperationException(
                    $"Dialog '{config.Title}' sets RequireTypedConfirmation but leaves ConfirmationKeyword " +
                    "empty, which would render a dialog whose primary action can never be enabled. " +
                    "Set ConfirmationKeyword to the word the user must type.");
            }

            if (dialog.Controls.Count == 0 || dialog.Controls[0] is not TableLayoutPanel shell) return;
            if (dialog.AcceptButton is not Control primary) return;

            TableLayoutPanel? footer = FindFooter(shell);
            if (footer == null) return;

            int footerRow = shell.GetCellPosition(footer).Row;

            var prompt = new BeepLabel
            {
                Text = $"Type {config.ConfirmationKeyword} to confirm",
                Dock = DockStyle.Fill,
                AutoSize = false,
                Margin = new Padding(0, 8, 0, 4),
                IsChild = true,          // the dialog surface colours it, like every other label
                IsFrameless = true,
            };

            var field = new BeepTextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 4),
                Text = string.Empty,     // never pre-filled: the point is that it must be typed
                AccessibleName = $"Type {config.ConfirmationKeyword} to confirm",
            };

            // Two rows ahead of the actions, so the field reads as the last thing before committing.
            InsertRowAt(shell, footerRow, prompt);
            InsertRowAt(shell, footerRow + 1, field);

            primary.Enabled = false;
            field.TextChanged += (_, _) =>
                primary.Enabled = string.Equals(field.Text, config.ConfirmationKeyword, StringComparison.Ordinal);

            // Focus the field, not the destructive action - stage 01's rule, and the reason this
            // dialog has a field at all.
            //
            // This is load-bearing, despite looking like something the tab order would do anyway:
            // deleting it turns the guarding check red. An earlier break test appeared to show it was
            // redundant, but that run left the handler subscribed and only removed the Focus() call,
            // so focus arrived by another route.
            void FocusField(object? sender, EventArgs e)
            {
                dialog.Shown -= FocusField;
                if (!field.IsDisposed) field.Focus();
            }

            dialog.Shown += FocusField;
        }

        /// <summary>
        /// Adds a row at <paramref name="row"/> in the text column, pushing what follows down.
        /// </summary>
        /// <remarks>
        /// The grid is declared in each dialog's designer file. This is the one thing a designer file
        /// cannot express, because whether the row exists at all depends on the config.
        /// </remarks>
        private static void InsertRowAt(TableLayoutPanel shell, int row, Control content)
        {
            shell.SuspendLayout();

            foreach (Control c in shell.Controls)
            {
                var pos = shell.GetCellPosition(c);
                if (pos.Row >= row) shell.SetCellPosition(c, new TableLayoutPanelCellPosition(pos.Column, pos.Row + 1));
            }

            shell.RowCount++;
            shell.RowStyles.Insert(row, new RowStyle(SizeType.AutoSize));

            // Column 1 is the text column in every dialog: column 0 is the icon gutter.
            shell.Controls.Add(content, 1, row);
            shell.ResumeLayout(true);
        }

        private static TableLayoutPanel? FindFooter(TableLayoutPanel shell)
        {
            foreach (Control c in shell.Controls)
            {
                if (c is TableLayoutPanel panel && panel.Controls.Cast<Control>().Any(x => x is IButtonControl))
                {
                    return panel;
                }
            }

            return null;
        }
    }
}
