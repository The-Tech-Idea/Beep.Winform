using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    /// <summary>
    /// Makes the body scroll when the content does not fit, so the actions stay reachable.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The requirement, not an optimisation: a dialog whose buttons have scrolled out of reach is
    /// unusable, and that is the failure users hit on small screens and at high zoom. The title stays
    /// put, the actions stay put, and only the content between them scrolls.
    /// </para>
    /// <para>
    /// <b>This only works because sizing has one owner.</b> An earlier attempt bounded the height here
    /// and collapsed the dialog to 26px wide, because <c>DialogHelpers.FitFormToContent</c> re-measured
    /// on <c>Load</c> and a scrollable body reports a tiny preferred width. That method is now the sole
    /// authority and honours the bounds the manager states, including a minimum width — so the body
    /// can be made scrollable without the form shrinking to it.
    /// </para>
    /// <para>
    /// This is a runtime change to a designer-declared grid, which the rest of this folder avoids. It
    /// earns the exception by being conditional: whether content overflows is not knowable when the
    /// designer file is written, and nothing happens at all when it does not.
    /// </para>
    /// </remarks>
    internal static class DialogOverflow
    {
        private const int TitleRow = 0;
        private const int TextColumn = 1;

        public static void Apply(Form dialog, DialogConfig config)
        {
            if (dialog == null || config == null) return;
            if (dialog.Controls.Count == 0 || dialog.Controls[0] is not TableLayoutPanel shell) return;

            TableLayoutPanel? footer = FindFooter(shell);
            if (footer == null) return;

            int footerRow = shell.GetCellPosition(footer).Row;
            if (footerRow <= TitleRow + 1) return;               // nothing between title and actions

            var bounds = DialogHelpers.BoundsFor(dialog);
            if (bounds.MaxHeight <= 0) return;

            // Only when it actually overflows. An always-on scrollbar in a two-line confirmation reads
            // as broken, so a dialog that fits is left exactly as its designer file declared it.
            int needed = shell.GetPreferredSize(new Size(Math.Max(1, shell.Width), 0)).Height;
            if (needed <= bounds.MaxHeight) return;

            Collapse(shell, footer, footerRow);

            // Take the height the bound allows. Without this the scrollable body's small preferred
            // height wins and the dialog opens cramped, making the user scroll further than needed.
            DialogHelpers.SetSizeBounds(dialog, bounds.MaxHeight, bounds.MinWidth, bounds.MaxHeight);
        }

        /// <summary>
        /// Moves every content row into one scrollable row, leaving the title and actions in place.
        /// </summary>
        private static void Collapse(TableLayoutPanel shell, TableLayoutPanel footer, int footerRow)
        {
            shell.SuspendLayout();

            // Taken in row order, so the body reads the same after the move as before it.
            var content = new List<Control>();
            for (int row = TitleRow + 1; row < footerRow; row++)
            {
                var occupant = shell.GetControlFromPosition(TextColumn, row);
                if (occupant != null) content.Add(occupant);
            }

            if (content.Count == 0) { shell.ResumeLayout(true); return; }

            var body = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = content.Count,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,          // Top, not Fill: a filled child cannot overflow, and
                Margin = new Padding(0),       // without overflow the scroll panel has nothing to do
                Padding = new Padding(0),
            };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            var scroller = new Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Padding = new Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 0),
            };

            for (int i = 0; i < content.Count; i++)
            {
                shell.Controls.Remove(content[i]);
                body.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                body.Controls.Add(content[i], 0, i);
            }

            scroller.Controls.Add(body);

            // Drop the now-empty content rows, then give the scroller a single row of its own.
            for (int row = footerRow - 1; row > TitleRow; row--)
            {
                shell.RowStyles.RemoveAt(row);
                shell.RowCount--;
            }

            shell.RowCount++;
            shell.RowStyles.Insert(TitleRow + 1, new RowStyle(SizeType.Percent, 100f));
            shell.Controls.Add(scroller, TextColumn, TitleRow + 1);
            shell.SetCellPosition(footer, new TableLayoutPanelCellPosition(0, TitleRow + 2));

            // The icon spanned the old content rows; there is one row now.
            var icon = shell.GetControlFromPosition(0, TitleRow);
            if (icon != null) shell.SetRowSpan(icon, 2);

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
