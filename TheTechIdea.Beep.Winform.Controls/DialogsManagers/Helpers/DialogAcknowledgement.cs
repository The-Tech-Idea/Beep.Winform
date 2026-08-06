using System;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    /// <summary>
    /// The acknowledgement checkbox: "don't show this again", and the gate that can depend on it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>VerificationText</c>, <c>VerificationChecked</c> and <c>DisablePrimaryUntilAcknowledged</c>
    /// all had zero readers, and <c>DialogReturn.WasVerificationChecked</c> existed for a value
    /// nothing produced. Three declarations and a result field describing a control that was never
    /// built.
    /// </para>
    /// <para>
    /// It belongs to the dialog rather than the caller for a practical reason: a caller cannot add a
    /// control to a dialog it did not lay out.
    /// </para>
    /// </remarks>
    internal static class DialogAcknowledgement
    {
        /// <summary>Column 1 is the text column; column 0 is the icon gutter.</summary>
        private const int TextColumn = 1;

        /// <summary>The checkbox, so the result can report its state after the dialog closes.</summary>
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Form, BeepCheckBoxBool> _boxes = new();

        public static void Apply(Form dialog, DialogConfig config)
        {
            if (dialog == null || config == null) return;

            bool wantsCheckbox = !string.IsNullOrWhiteSpace(config.VerificationText);
            if (!wantsCheckbox && !config.DisablePrimaryUntilAcknowledged) return;

            if (dialog.Controls.Count == 0 || dialog.Controls[0] is not TableLayoutPanel shell) return;
            if (dialog.AcceptButton is not Control primary) return;

            TableLayoutPanel? footer = FindFooter(shell);
            if (footer == null) return;

            // An acknowledgement gate with nothing to acknowledge would disable the primary action
            // permanently. Asking for the gate is therefore asking for the checkbox.
            string caption = wantsCheckbox ? config.VerificationText! : "I understand";

            var box = new BeepCheckBoxBool
            {
                Text = caption,
                CurrentValue = config.VerificationChecked,
                AutoSize = false,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 8, 0, 4),
                AccessibleName = caption,
            };

            InsertRowAt(shell, shell.GetCellPosition(footer).Row, box);
            _boxes.AddOrUpdate(dialog, box);

            if (config.DisablePrimaryUntilAcknowledged)
            {
                primary.Enabled = box.CurrentValue;

                // Re-evaluated on every change rather than latched, so unchecking disables it again.
                box.CheckedChanged += (_, _) => primary.Enabled = box.CurrentValue;
            }
        }

        /// <summary>The checkbox's state, for <c>DialogReturn.WasVerificationChecked</c>.</summary>
        public static bool WasChecked(Form dialog)
        {
            if (dialog == null) return false;
            return _boxes.TryGetValue(dialog, out var box) && !box.IsDisposed && box.CurrentValue;
        }

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
            shell.Controls.Add(content, TextColumn, row);

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
