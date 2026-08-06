using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    /// <summary>
    /// Renders each configured callout as an inset panel with a left accent bar.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Built from controls rather than painted: a two-column <see cref="TableLayoutPanel"/> whose
    /// first column is a narrow accent bar and whose second holds the label and text. Nothing here
    /// draws.
    /// </para>
    /// <para>
    /// The accent bar is the one colour assignment, and it is the same exception the destructive
    /// button takes — the colour <i>is</i> the message. It comes from the theme through the severity
    /// resolver, never from a literal, so a callout follows a theme change like everything else.
    /// </para>
    /// </remarks>
    internal static class DialogCallouts
    {
        /// <summary>Column 1 is the text column; column 0 is the icon gutter.</summary>
        private const int TextColumn = 1;

        /// <summary>Width of the accent bar, before DPI scaling.</summary>
        private const int AccentWidth = 4;

        public static void Apply(Form dialog, DialogConfig config, IBeepTheme? theme)
        {
            if (dialog == null || config == null) return;

            IReadOnlyList<DialogCallout>? callouts = config.Callouts;
            if (callouts == null || callouts.Count == 0) return;

            if (dialog.Controls.Count == 0 || dialog.Controls[0] is not TableLayoutPanel shell) return;

            TableLayoutPanel? footer = FindFooter(shell);
            if (footer == null) return;

            // Inserted in order, immediately above the actions: a callout qualifies the decision the
            // buttons are about, so it belongs next to them rather than under the message.
            int row = shell.GetCellPosition(footer).Row;

            foreach (var callout in callouts)
            {
                InsertRowAt(shell, row, Build(callout, theme, dialog.DeviceDpi));
                row++;
            }
        }

        private static Control Build(DialogCallout callout, IBeepTheme? theme, int dpi)
        {
            Color accent = DialogStyleAdapter.GetSeverityAccent(callout.Severity, theme);
            Color surface = theme?.BackColor is { IsEmpty: false } b
                ? b
                : ColorUtils.MapSystemColor(SystemColors.Window);

            var panel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 8, 0, 8),
                Padding = new Padding(0),
                BackColor = DialogStyleAdapter.GetSeverityWash(callout.Severity, theme, surface, 0.10),
            };

            int barWidth = Math.Max(2, (int)Math.Round(AccentWidth * (dpi / 96f)));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, barWidth));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var bar = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                BackColor = accent,
            };

            var body = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 0,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Padding = new Padding(10, 8, 10, 8),
                BackColor = panel.BackColor,
            };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            if (!string.IsNullOrWhiteSpace(callout.Label))
            {
                body.RowCount++;
                body.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                body.Controls.Add(new BeepLabel
                {
                    Text = callout.Label,
                    Dock = DockStyle.Fill,
                    AutoSize = false,
                    Height = 20,
                    IsChild = true,
                    IsFrameless = true,
                    AccessibleName = callout.Label,
                }, 0, body.RowCount - 1);
            }

            body.RowCount++;
            body.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            body.Controls.Add(new BeepLabel
            {
                Text = callout.Text,
                Dock = DockStyle.Fill,
                AutoSize = false,
                Height = 36,
                WordWrap = true,
                Multiline = true,
                IsChild = true,
                IsFrameless = true,
                AccessibleName = callout.Text,
            }, 0, body.RowCount - 1);

            panel.Controls.Add(bar, 0, 0);
            panel.Controls.Add(body, 1, 0);
            return panel;
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
