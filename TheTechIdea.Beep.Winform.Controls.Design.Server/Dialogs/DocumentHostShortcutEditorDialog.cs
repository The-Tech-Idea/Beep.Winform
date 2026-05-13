using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs
{
    internal sealed class DocumentHostShortcutEditorDialog : Form
    {
        private readonly BeepCommandRegistry _registry;
        private readonly DataGridView _grid;
        private readonly Button _okButton;
        private readonly Button _cancelButton;

        public DocumentHostShortcutEditorDialog(BeepCommandRegistry registry)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));

            Text = "Customize Keyboard Shortcuts";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimumSize = new Size(720, 460);
            Size = new Size(860, 560);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var info = new Label
            {
                AutoSize = true,
                Text = "Edit shortcut text for each command. Example: Ctrl+Shift+T",
                Margin = new Padding(0, 0, 0, 8)
            };

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false,
                RowHeadersVisible = false
            };
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Category",
                HeaderText = "Category",
                DataPropertyName = "Category",
                ReadOnly = true,
                Width = 150
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Title",
                HeaderText = "Command",
                DataPropertyName = "Title",
                ReadOnly = true,
                Width = 300,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Shortcut",
                HeaderText = "Shortcut",
                DataPropertyName = "Shortcut",
                ReadOnly = false,
                Width = 200
            });

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            _okButton = new Button { Text = "OK", Width = 100, DialogResult = DialogResult.OK };
            _cancelButton = new Button { Text = "Cancel", Width = 100, DialogResult = DialogResult.Cancel };
            var resetButton = new Button { Text = "Clear All", Width = 100 };
            resetButton.Click += (_, _) =>
            {
                foreach (DataGridViewRow row in _grid.Rows)
                    row.Cells["Shortcut"].Value = string.Empty;
            };

            actions.Controls.Add(_okButton);
            actions.Controls.Add(_cancelButton);
            actions.Controls.Add(resetButton);

            root.Controls.Add(info, 0, 0);
            root.Controls.Add(_grid, 0, 1);
            root.Controls.Add(actions, 0, 2);
            Controls.Add(root);

            AcceptButton = _okButton;
            CancelButton = _cancelButton;

            Load += (_, _) => LoadRows();
            FormClosing += OnFormClosing;
        }

        private void LoadRows()
        {
            _grid.Rows.Clear();
            foreach (var cmd in _registry.GetAll())
            {
                _grid.Rows.Add(cmd.Category, cmd.Title, cmd.Shortcut ?? string.Empty);
            }
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                return;

            var commandRows = _registry.GetAll();
            if (_grid.Rows.Count != commandRows.Count)
                return;

            for (int i = 0; i < commandRows.Count; i++)
            {
                var shortcutValue = _grid.Rows[i].Cells["Shortcut"].Value?.ToString()?.Trim();
                commandRows[i].Shortcut = string.IsNullOrWhiteSpace(shortcutValue) ? null : shortcutValue;
            }
        }
    }
}
