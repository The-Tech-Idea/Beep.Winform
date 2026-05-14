// WorkspaceManagerDialog.cs
// Design-time dialog for managing named workspace snapshots of a BeepDocumentHost.
// Operations available at design time: list, save-current-as, rename, delete.
// (Load/Switch is a runtime operation handled by BeepDocumentHost.SwitchWorkspace.)
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs
{
    /// <summary>
    /// Design-time dialog that lists the named workspace snapshots stored with a
    /// <see cref="BeepDocumentHost"/> and allows the developer to rename or delete them,
    /// or to capture the current layout as a new named workspace.
    /// </summary>
    internal sealed class WorkspaceManagerDialog : Form
    {
        private readonly BeepDocumentHost _host;
        private readonly ListView         _list;
        private readonly Button           _renameButton;
        private readonly Button           _deleteButton;
        private readonly Button           _saveCurrentButton;
        private readonly Button           _closeButton;

        internal WorkspaceManagerDialog(BeepDocumentHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));

            Text            = "Manage Workspaces";
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimumSize     = new Size(560, 360);
            Size            = new Size(680, 460);

            var root = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 1,
                RowCount    = 3,
                Padding     = new Padding(10),
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var info = new Label
            {
                AutoSize = true,
                Text     = "Saved workspace layouts for this document host.",
                Margin   = new Padding(0, 0, 0, 8),
            };

            _list = new ListView
            {
                Dock          = DockStyle.Fill,
                View          = View.Details,
                FullRowSelect = true,
                MultiSelect   = false,
                HideSelection = false,
                GridLines     = true,
            };
            _list.Columns.Add("Name",        200);
            _list.Columns.Add("Description", 230);
            _list.Columns.Add("Saved",       160);
            _list.SelectedIndexChanged += (_, _) => UpdateButtonStates();

            var bar = new FlowLayoutPanel
            {
                Dock            = DockStyle.Fill,
                FlowDirection   = FlowDirection.RightToLeft,
                AutoSize        = true,
                Margin          = new Padding(0, 8, 0, 0),
            };

            _closeButton       = new Button { Text = "Close",               Width = 100, DialogResult = DialogResult.OK };
            _deleteButton      = new Button { Text = "Delete",              Width = 100, Enabled = false };
            _renameButton      = new Button { Text = "Rename\u2026",        Width = 100, Enabled = false };
            _saveCurrentButton = new Button { Text = "Save Current As\u2026", Width = 140 };

            _renameButton.Click      += OnRename;
            _deleteButton.Click      += OnDelete;
            _saveCurrentButton.Click += OnSaveCurrent;

            bar.Controls.Add(_closeButton);
            bar.Controls.Add(_deleteButton);
            bar.Controls.Add(_renameButton);
            bar.Controls.Add(_saveCurrentButton);

            root.Controls.Add(info,  0, 0);
            root.Controls.Add(_list, 0, 1);
            root.Controls.Add(bar,   0, 2);
            Controls.Add(root);

            AcceptButton = _closeButton;
            CancelButton = _closeButton;

            Load += (_, _) => PopulateList();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void PopulateList()
        {
            _list.Items.Clear();
            foreach (var ws in _host.GetAllWorkspaces())
            {
                var item = new ListViewItem(ws.Name);
                item.SubItems.Add(ws.Description ?? string.Empty);
                item.SubItems.Add(ws.SavedAt.ToLocalTime().ToString("g"));
                item.Tag = ws.Name;
                _list.Items.Add(item);
            }
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool sel = _list.SelectedItems.Count > 0;
            _renameButton.Enabled = sel;
            _deleteButton.Enabled = sel;
        }

        private string? SelectedName()
            => _list.SelectedItems.Count > 0
                ? _list.SelectedItems[0].Tag as string
                : null;

        // ── Rename ────────────────────────────────────────────────────────────

        private void OnRename(object? sender, EventArgs e)
        {
            string? old = SelectedName();
            if (old == null) return;

            using var dlg = new WorkspaceNameDialog(old, "Rename Workspace", "New name:");
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            string fresh = dlg.EnteredName.Trim();
            if (string.IsNullOrEmpty(fresh) || fresh == old) return;

            try
            {
                _host.RenameWorkspace(old, fresh);
                PopulateList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Rename Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── Delete ────────────────────────────────────────────────────────────

        private void OnDelete(object? sender, EventArgs e)
        {
            string? name = SelectedName();
            if (name == null) return;

            var r = MessageBox.Show(
                $"Delete workspace \"{name}\"?\n\nThis cannot be undone.",
                "Delete Workspace",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (r != DialogResult.Yes) return;

            _host.DeleteWorkspace(name);
            PopulateList();
        }

        // ── Save current layout as new workspace ──────────────────────────────

        private void OnSaveCurrent(object? sender, EventArgs e)
        {
            using var dlg = new WorkspaceNameDialog(
                string.Empty, "Save Current Layout", "Workspace name:");
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            string name = dlg.EnteredName.Trim();
            if (string.IsNullOrEmpty(name)) return;

            try
            {
                _host.SaveWorkspace(name);
                PopulateList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    // Inline helper — single text-field prompt
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Minimal single-field input dialog used by <see cref="WorkspaceManagerDialog"/>
    /// for rename and new-workspace-name prompts.
    /// </summary>
    internal sealed class WorkspaceNameDialog : Form
    {
        private readonly TextBox _textBox;

        /// <summary>The text the user confirmed with OK.</summary>
        public string EnteredName => _textBox.Text;

        internal WorkspaceNameDialog(string initial, string title, string prompt)
        {
            Text            = title;
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox     = false;
            MaximizeBox     = false;
            ClientSize      = new Size(400, 130);

            var label = new Label
            {
                Text     = prompt,
                AutoSize = true,
                Location = new Point(16, 18),
            };

            _textBox = new TextBox
            {
                Text     = initial,
                Location = new Point(16, 42),
                Width    = 368,
            };

            var ok = new Button
            {
                Text         = "OK",
                DialogResult = DialogResult.OK,
                Location     = new Point(222, 88),
                Width        = 80,
            };
            var cancel = new Button
            {
                Text         = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location     = new Point(308, 88),
                Width        = 80,
            };

            _textBox.KeyDown += (_, ke) =>
            {
                if (ke.KeyCode == Keys.Return) { DialogResult = DialogResult.OK;     Close(); }
                if (ke.KeyCode == Keys.Escape) { DialogResult = DialogResult.Cancel; Close(); }
            };

            Controls.Add(label);
            Controls.Add(_textBox);
            Controls.Add(ok);
            Controls.Add(cancel);

            AcceptButton = ok;
            CancelButton = cancel;

            Load += (_, _) => { _textBox.SelectAll(); _textBox.Focus(); };
        }
    }
}
