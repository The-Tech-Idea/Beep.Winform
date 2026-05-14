// IdeShellDemoForm.cs
// Phase 08 A — Polished demo of BeepDocumentManager as an orchestrator.
//
// Demonstrates:
//   • BeepDocumentManager + BeepTabbedView + BeepDocumentHost wired in code
//   • ActiveDocumentChanged → title bar + StatusStrip label
//   • DocumentClosing dirty-document guard
//   • Editor document that flips IsModified on first keystroke
//   • Properties document (PropertyGrid bound to the manager)
//   • Output document (log ListBox that echoes document-activation events)
//   • Workspaces menu: "Save Current As…" + dynamic switch entries
//   • AutoSaveLayout / SessionFile — layout persisted across runs
//
// NOTE: Phase 05 (BeepDockManager) is not yet implemented, so Properties and
//       Output appear as ordinary document tabs rather than docked tool windows.
//       Replace with BeepDockManager calls when Phase 05 ships.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;

namespace TheTechIdea.Beep.Winform.Default.Views
{
    /// <summary>
    /// A code-driven demo form that shows every headline feature of
    /// <see cref="Controls.DocumentHost.BeepDocumentManager"/>.
    /// </summary>
    public sealed class IdeShellDemoForm : Form
    {
        // ── Non-visual components (would sit in the component tray if designer-based)

        private readonly BeepDocumentManager _manager;
        private readonly BeepTabbedView      _tabbedView;

        // ── Visual document host

        private readonly BeepDocumentHost _host;

        // ── Chrome

        private readonly MenuStrip             _menuStrip;
        private readonly ToolStripMenuItem     _workspacesMenu;
        private readonly StatusStrip           _statusStrip;
        private readonly ToolStripStatusLabel  _lblDoc;
        private readonly ToolStripStatusLabel  _lblModified;

        // ── State

        private int _docCounter;

        // ═════════════════════════════════════════════════════════════════════
        // Construction
        // ═════════════════════════════════════════════════════════════════════

        public IdeShellDemoForm()
        {
            Text         = "IDE Shell Demo – BeepDocumentManager";
            Size         = new Size(1200, 760);
            MinimumSize  = new Size(800, 500);
            StartPosition = FormStartPosition.CenterScreen;

            // ── Status strip ────────────────────────────────────────────────
            _lblDoc      = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft, Text = "Ready" };
            _lblModified = new ToolStripStatusLabel { Text = "" };
            _statusStrip = new StatusStrip { Dock = DockStyle.Bottom };
            _statusStrip.Items.Add(_lblDoc);
            _statusStrip.Items.Add(_lblModified);

            // ── Document host (visual) ───────────────────────────────────────
            _host = new BeepDocumentHost { Dock = DockStyle.Fill };

            // ── File menu ───────────────────────────────────────────────────
            var fileMenu = new ToolStripMenuItem("&File");
            fileMenu.DropDownItems.Add("&New Document",  null, (_, _) => AddGenericDocument($"Document {++_docCounter}"));
            fileMenu.DropDownItems.Add("Re&open Layout", null, (_, _) => _manager.LoadLayout());
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("E&xit",          null, (_, _) => Close());

            // ── Workspaces menu (populated lazily on open) ──────────────────
            _workspacesMenu = new ToolStripMenuItem("&Workspaces");
            _workspacesMenu.DropDownOpening += OnWorkspacesMenuOpening;

            // ── Help menu ───────────────────────────────────────────────────
            var helpMenu = new ToolStripMenuItem("&Help");
            helpMenu.DropDownItems.Add("About BeepDocumentManager", null, OnAbout);

            // ── Menu strip ──────────────────────────────────────────────────
            _menuStrip = new MenuStrip { Dock = DockStyle.Top };
            _menuStrip.Items.Add(fileMenu);
            _menuStrip.Items.Add(_workspacesMenu);
            _menuStrip.Items.Add(helpMenu);
            MainMenuStrip = _menuStrip;

            // ── Wire non-visual components ───────────────────────────────────
            _tabbedView = new BeepTabbedView { Host = _host };

            _manager = new BeepDocumentManager();
            _manager.BeginInit();
            _manager.View           = _tabbedView;
            _manager.AutoSaveLayout = true;
            _manager.SessionFile    = @"%AppData%\TheTechIdea\Beep\IdeShellDemoLayout.json";
            _manager.EndInit();

            // ── Event hooks ─────────────────────────────────────────────────
            _manager.ActiveDocumentChanged += OnActiveDocumentChanged;
            _manager.DocumentClosing        += OnDocumentClosing;

            // ── Assemble form ────────────────────────────────────────────────
            Controls.Add(_host);
            Controls.Add(_menuStrip);
            Controls.Add(_statusStrip);

            Load   += OnLoad;
        }

        // ═════════════════════════════════════════════════════════════════════
        // Form events
        // ═════════════════════════════════════════════════════════════════════

        private void OnLoad(object? sender, EventArgs e)
        {
            if (_manager.DocumentCount > 0) return;   // Layout was restored — don't re-open.

            _manager.BeginBatchAddDocuments();
            try
            {
                OpenWelcomeDocument();
                OpenEditorDocument("Editor");
                OpenPropertiesDocument();
                OpenOutputDocument();
            }
            finally
            {
                _manager.EndBatchAddDocuments();
            }
        }

        private void OnActiveDocumentChanged(object? sender, DocumentEventArgs e)
        {
            string title = e.Title ?? string.Empty;
            Text         = string.IsNullOrWhiteSpace(title) ? "IDE Shell Demo" : $"{title} \u2013 IDE Shell Demo";
            _lblDoc.Text = string.IsNullOrWhiteSpace(title) ? "Ready" : title;

            var panel = _manager.GetPanel(e.DocumentId);
            _lblModified.Text = panel?.IsModified == true ? "\u25cf Modified" : string.Empty;
        }

        private void OnDocumentClosing(object? sender, TabClosingEventArgs e)
        {
            var panel = _manager.GetPanel(e.Tab.Id);
            if (panel?.IsModified != true) return;

            var r = MessageBox.Show(
                $"\"{e.Tab.Title}\" has unsaved changes.\n\nDiscard changes and close?",
                "Unsaved Changes",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (r != DialogResult.Yes)
                e.Cancel = true;
        }

        // ═════════════════════════════════════════════════════════════════════
        // Workspace menu
        // ═════════════════════════════════════════════════════════════════════

        private void OnWorkspacesMenuOpening(object? sender, EventArgs e)
        {
            _workspacesMenu.DropDownItems.Clear();
            _workspacesMenu.DropDownItems.Add("Save Current As\u2026", null, OnSaveCurrentWorkspace);
            _workspacesMenu.DropDownItems.Add(new ToolStripSeparator());

            var all = _host.GetAllWorkspaces().ToList();
            if (all.Count == 0)
            {
                _workspacesMenu.DropDownItems.Add(new ToolStripMenuItem("(No saved workspaces)") { Enabled = false });
                return;
            }

            foreach (var ws in all)
            {
                string name = ws.Name;
                _workspacesMenu.DropDownItems.Add(name, null, (_, _) => _host.SwitchWorkspace(name));
            }
        }

        private void OnSaveCurrentWorkspace(object? sender, EventArgs e)
        {
            using var dlg = new IdeShellWorkspaceNameDialog();
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            string name = dlg.WorkspaceName.Trim();
            if (string.IsNullOrEmpty(name)) return;

            try
            {
                _host.SaveWorkspace(name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Workspace Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // Help
        // ═════════════════════════════════════════════════════════════════════

        private void OnAbout(object? sender, EventArgs e)
        {
            MessageBox.Show(
                "BeepDocumentManager — Phase 08 A demo form.\n\n" +
                "Sources: TheTechIdea.Beep.Winform.Controls.DocumentHost\n" +
                "Phases completed: 01–07 A–B1, F1–F2",
                "About IdeShellDemoForm",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // ═════════════════════════════════════════════════════════════════════
        // Document factories
        // ═════════════════════════════════════════════════════════════════════

        private void OpenWelcomeDocument()
        {
            var panel = _manager.AddDocument("Welcome", activate: true);
            if (panel == null) return;

            panel.Controls.Add(new Label
            {
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = new Font("Segoe UI", 11f),
                ForeColor = SystemColors.GrayText,
                Text      =
                    "Welcome to IdeShellDemoForm\n\n" +
                    "  \u2022  BeepDocumentManager + BeepTabbedView + BeepDocumentHost\n" +
                    "  \u2022  Dirty-document guard — type in the Editor tab, then try to close it\n" +
                    "  \u2022  Workspace save / switch — Workspaces menu\n" +
                    "  \u2022  AutoSaveLayout — layout is restored on next run\n\n" +
                    "Keyboard shortcuts (Ctrl+N, Ctrl+W, Ctrl+Tab, etc.) are\n" +
                    "processed automatically by BeepDocumentHost.",
            });
        }

        private void OpenEditorDocument(string title)
        {
            var panel = _manager.AddDocument(title, activate: false);
            if (panel == null) return;

            var rtb = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font        = new Font("Consolas", 11f),
                Text        = "// Type here.\n// Closing this tab without \"saving\" triggers the dirty-document guard.\n",
            };

            rtb.TextChanged += (_, _) =>
            {
                panel.IsModified  = true;
                _lblModified.Text = "\u25cf Modified";
            };

            panel.Controls.Add(rtb);
        }

        private void OpenPropertiesDocument()
        {
            var panel = _manager.AddDocument("Properties", activate: false);
            if (panel == null) return;

            // NOTE: When Phase 05 (BeepDockManager) ships, this becomes a docked
            //       tool window rather than a document tab.
            panel.Controls.Add(new PropertyGrid
            {
                Dock           = DockStyle.Fill,
                SelectedObject = _manager,     // Exposes all manager properties.
            });
        }

        private void OpenOutputDocument()
        {
            var panel = _manager.AddDocument("Output", activate: false);
            if (panel == null) return;

            var log = new ListBox
            {
                Dock        = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font        = new Font("Consolas", 10f),
            };

            void Append(string msg)
            {
                if (log.IsHandleCreated)
                    log.BeginInvoke((Action)(() => { log.Items.Add(msg); log.TopIndex = log.Items.Count - 1; }));
                else
                    log.Items.Add(msg);
            }

            Append($"[{DateTime.Now:HH:mm:ss}] IdeShellDemoForm loaded.");
            Append($"[{DateTime.Now:HH:mm:ss}] AutoSaveLayout = {_manager.AutoSaveLayout}");

            _manager.ActiveDocumentChanged += (_, de) =>
                Append($"[{DateTime.Now:HH:mm:ss}] Activated: {de.Title}");

            panel.Controls.Add(log);
        }

        private void AddGenericDocument(string title)
        {
            var panel = _manager.AddDocument(title, activate: true);
            if (panel == null) return;

            panel.Controls.Add(new Label
            {
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = new Font("Segoe UI", 12f),
                ForeColor = SystemColors.GrayText,
                Text      = title,
            });
        }

        // ═════════════════════════════════════════════════════════════════════
        // Disposal
        // ═════════════════════════════════════════════════════════════════════

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _manager.Dispose();
                _tabbedView.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Inline workspace name prompt (private to this compilation unit)
    // ─────────────────────────────────────────────────────────────────────────

    internal sealed class IdeShellWorkspaceNameDialog : Form
    {
        private readonly TextBox _tb;

        public string WorkspaceName => _tb.Text;

        internal IdeShellWorkspaceNameDialog()
        {
            Text            = "Save Current Workspace";
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox     = false;
            MaximizeBox     = false;
            ClientSize      = new Size(360, 112);

            var lbl = new Label { Text = "Workspace name:", AutoSize = true, Location = new Point(12, 14) };

            _tb = new TextBox { Location = new Point(12, 38), Width = 336 };
            _tb.KeyDown += (_, ke) =>
            {
                if (ke.KeyCode == Keys.Return) { DialogResult = DialogResult.OK;     Close(); }
                if (ke.KeyCode == Keys.Escape) { DialogResult = DialogResult.Cancel; Close(); }
            };

            var ok     = new Button { Text = "OK",     DialogResult = DialogResult.OK,     Location = new Point(182, 76), Width = 80 };
            var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(268, 76), Width = 80 };

            Controls.AddRange(new Control[] { lbl, _tb, ok, cancel });
            AcceptButton = ok;
            CancelButton = cancel;

            Load += (_, _) => { _tb.SelectAll(); _tb.Focus(); };
        }
    }
}
