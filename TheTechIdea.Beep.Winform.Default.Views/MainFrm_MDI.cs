using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views
{
    /// <summary>
    /// Full-featured MDI shell demonstrating every first-class capability of
    /// <see cref="BeepDocumentManager"/> + <see cref="BeepDocumentHost"/>.
    ///
    /// Features shown in this form:
    ///   • AutoSaveLayout / SessionFile — zero-code layout persistence
    ///   • Tab-style picker (beepComboBox1) — Chrome, VSCode, Pill, Underline, Flat …
    ///   • Document search (beepTextBox1) — jump-to-document by partial title
    ///   • Startup documents: Welcome viewer, Notes editor, Settings panel, Markdown note
    ///   • RegisterTemplate — "markdown" factory creates RichTextBox with monospace font
    ///   • RegisterCommand — Ctrl+Shift+N → new Markdown note via the template
    ///   • ActiveDocumentChanged — updates the form caption with the active title
    ///   • DocumentClosing guard — prompts when IsModified is true
    ///   • Split-horizontal shortcut demo registered as "demo.split"
    ///   • Float / Auto-hide actions wired through host context menu (right-click tabs)
    ///   • Workspace save/switch invoked from the beepButton1 drop-down
    ///   • WorkspaceSwitched event — updates caption with workspace name
    ///
    /// Keyboard shortcuts (forwarded by BeepDocumentHost / BeepDocumentManager):
    ///   Ctrl+N           New document (plain viewer)
    ///   Ctrl+W           Close active document
    ///   Ctrl+Shift+T     Reopen last closed
    ///   Ctrl+Shift+P     Command palette
    ///   Ctrl+Shift+N     New Markdown note (custom registered command)
    ///   Ctrl+Shift+H     Split active document horizontally (custom registered command)
    ///   Ctrl+Tab         MRU tab cycle
    ///   Ctrl+K, H        Keyboard shortcut help
    ///   Ctrl+Alt+←/→     Move active tab to adjacent split group
    ///   Ctrl+Shift+M     Maximize / restore active document
    /// </summary>
    public partial class MainFrm_MDI : TemplateForm
    {
        // ── State ─────────────────────────────────────────────────────────────

        private int  _docCounter;
        private bool _styleChanging;

        private MenuStrip            _menuStrip   = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        /// <summary>
        /// Local index used by the document-search feature.
        /// Key = document ID (from <see cref="BeepDocumentPanel.DocumentId"/>),
        /// Value = title at the time the document was added.
        /// </summary>
        private readonly Dictionary<string, string> _documentTitles = new(StringComparer.Ordinal);

        // ── Construction ──────────────────────────────────────────────────────

        public MainFrm_MDI()
        {
            InitializeComponent();
            Load += OnFormLoad;
        }

        // ── Form Load ─────────────────────────────────────────────────────────

        private void OnFormLoad(object? sender, EventArgs e)
        {
            WireMenuBar();
            WireStatusBar();
            RegisterCustomCommands();
            WireEvents();
            WireTabStylePicker();
            WireDocumentSearch();
            WireAddButton();
            OpenStartupDocuments();
        }

        // ── Menu bar ──────────────────────────────────────────────────────────

        private void WireMenuBar()
        {
            _menuStrip = new MenuStrip { Dock = DockStyle.Top };

            // File menu
            var fileMenu = new ToolStripMenuItem("&File");
            fileMenu.DropDownItems.Add("&New Document", null,
                (_, _) => AddSampleDocument($"Document {_docCounter + 1}"));
            fileMenu.DropDownItems.Add("Close &Active", null, (_, _) =>
            {
                var id = beepTabbedView1.Host?.ActiveDocumentId;
                if (id != null) beepDocumentManager1.RemoveDocument(id);
            });
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("E&xit", null, (_, _) => Close());

            // View menu — cycles tab style (Chrome → VSCode → Pill → Underline → Flat → ...)
            var viewMenu = new ToolStripMenuItem("&View");
            var cycleItem = new ToolStripMenuItem("Cycle &Tab Style\tCtrl+Shift+,");
            cycleItem.Click += (_, _) =>
            {
                var host = beepTabbedView1.Host;
                if (host == null) return;
                var styles = Enum.GetValues<DocumentTabStyle>();
                int next   = ((int)host.TabStyle + 1) % styles.Length;
                host.TabStyle = styles[next];
                if (!_styleChanging)
                {
                    _styleChanging = true;
                    beepComboBox1.Text = host.TabStyle.ToString();
                    _styleChanging = false;
                }
            };
            viewMenu.DropDownItems.Add(cycleItem);
            viewMenu.DropDownItems.Add(new ToolStripSeparator());
            var showBreadcrumb = new ToolStripMenuItem("Show &Breadcrumb Bar") { Checked = true, CheckOnClick = true };
            showBreadcrumb.CheckedChanged += (_, _) =>
            {
                var host = beepTabbedView1.Host;
                if (host != null) host.ShowBreadcrumb = showBreadcrumb.Checked;
            };
            viewMenu.DropDownItems.Add(showBreadcrumb);

            // Window menu — populated dynamically by AttachWindowMenu
            var windowMenu = new ToolStripMenuItem("&Window");

            _menuStrip.Items.Add(fileMenu);
            _menuStrip.Items.Add(viewMenu);
            _menuStrip.Items.Add(windowMenu);

            Controls.Add(_menuStrip);
            MainMenuStrip = _menuStrip;

            // Wire BeepDocumentHost's built-in Window-menu population
            beepTabbedView1.Host?.AttachWindowMenu(_menuStrip, "Window");
        }

        // ── Status bar ────────────────────────────────────────────────────────

        private void WireStatusBar()
        {
            _statusLabel = new ToolStripStatusLabel
            {
                Text    = "Ready",
                Spring  = true,
                TextAlign = ContentAlignment.MiddleLeft,
            };

            var modeLabel = new ToolStripStatusLabel
            {
                Text      = beepTabbedView1.Host?.TabStyle.ToString() ?? string.Empty,
                Alignment = ToolStripItemAlignment.Right,
            };

            var statusStrip = new StatusStrip { Dock = DockStyle.Bottom };
            statusStrip.Items.Add(_statusLabel);
            statusStrip.Items.Add(modeLabel);
            Controls.Add(statusStrip);

            // Keep the mode label in sync when TabStyle changes
            if (beepTabbedView1.Host != null)
            {
                beepTabbedView1.Host.TabStyleChanged += (_, _) =>
                    modeLabel.Text = beepTabbedView1.Host?.TabStyle.ToString() ?? string.Empty;
            }
        }

        // ── Command registration ──────────────────────────────────────────────

        private void RegisterCustomCommands()
        {
            // Ctrl+Shift+N — new Markdown note
            beepDocumentManager1.RegisterCommand(
                "custom.new.markdown",
                "New Markdown Note",
                () => AddMarkdownDocument($"Note {++_docCounter}"),
                Keys.Control | Keys.Shift | Keys.N);

            // Ctrl+Shift+H — split active document horizontally
            beepDocumentManager1.RegisterCommand(
                "demo.split",
                "Split Document Horizontally (Demo)",
                SplitActiveHorizontal,
                Keys.Control | Keys.Shift | Keys.H);
        }

        // ── Event wiring ──────────────────────────────────────────────────────

        private void WireEvents()
        {
            beepDocumentManager1.ActiveDocumentChanged += OnActiveDocumentChanged;
            beepDocumentManager1.DocumentClosing       += OnDocumentClosing;

            // Track open documents for the search feature
            beepDocumentManager1.DocumentAdded += (_, de) =>
            {
                if (de?.Panel?.DocumentId != null)
                    _documentTitles[de.Panel.DocumentId] = de.Panel.DocumentTitle ?? string.Empty;
            };
            beepDocumentManager1.DocumentRemoved += (_, de) =>
                _documentTitles.Remove(de.DocumentId ?? string.Empty);

            beepDocumentManager1.WorkspaceSwitched += (_, we) =>
                UpdateCaption(null, we.Workspace.Name);

            beepDocumentManager1.WorkspaceSaved += (_, we) =>
                MessageBox.Show($"Workspace \"{we.Workspace.Name}\" saved.",
                    "Workspace", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnActiveDocumentChanged(object? sender, DocumentEventArgs e)
        {
            UpdateCaption(e.Title, beepDocumentManager1.ActiveWorkspaceName ?? null);
            _statusLabel.Text = string.IsNullOrWhiteSpace(e.Title) ? "Ready" : e.Title;
        }

        private void UpdateCaption(string? documentTitle, string? workspaceName)
        {
            var doc  = string.IsNullOrWhiteSpace(documentTitle) ? null : documentTitle;
            var ws   = string.IsNullOrWhiteSpace(workspaceName) ? null : $" [{workspaceName}]";
            Text     = doc != null ? $"{doc}{ws} \u2013 MDI Demo" : $"MDI Demo{ws}";
        }

        private void OnDocumentClosing(object? sender, TabClosingEventArgs e)
        {
            var panel = beepDocumentManager1.GetPanel(e.Tab.Id);
            if (panel?.IsModified != true) return;

            var result = MessageBox.Show(
                $"\"{e.Tab.Title}\" has unsaved changes.\n\nDiscard changes and close?",
                "Unsaved Changes",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
                e.Cancel = true;
        }

        // ── Tab-style picker (beepComboBox1) ──────────────────────────────────

        private void WireTabStylePicker()
        {
            var styles = Enum.GetValues<DocumentTabStyle>()
                             .Select(s => s.ToString())
                             .ToArray();

            beepComboBox1.Items.Clear();
            foreach (var s in styles)
                beepComboBox1.Items.Add(s);

            var host = beepTabbedView1.Host;
            if (host != null)
                beepComboBox1.Text = host.TabStyle.ToString();

            beepComboBox1.SelectedIndexChanged += (_, _) =>
            {
                if (_styleChanging) return;
                var h = beepTabbedView1.Host;
                if (h == null) return;
                if (Enum.TryParse<DocumentTabStyle>(beepComboBox1.Text, out var style))
                {
                    _styleChanging = true;
                    h.TabStyle     = style;
                    _styleChanging = false;
                }
            };
        }

        // ── Document search (beepTextBox1) ────────────────────────────────────

        private void WireDocumentSearch()
        {
            beepTextBox1.PlaceholderText = "Search documents…";

            beepTextBox1.KeyDown += (_, ke) =>
            {
                if (ke.KeyCode != Keys.Enter && ke.KeyCode != Keys.Return) return;
                ke.Handled = ke.SuppressKeyPress = true;

                var query = beepTextBox1.Text?.Trim();
                if (string.IsNullOrEmpty(query)) return;

                // Search the local title index (populated by DocumentAdded / DocumentRemoved)
                var match = _documentTitles.FirstOrDefault(kv =>
                    kv.Value.Contains(query, StringComparison.OrdinalIgnoreCase));

                if (match.Key != null)
                    beepDocumentManager1.ActivateDocument(match.Key);
                else
                    System.Media.SystemSounds.Beep.Play();
            };
        }

        // ── Add-Document button (beepButton1) ─────────────────────────────────

        private void WireAddButton()
        {
            beepButton1.Text   = "+ Add Document";
            beepButton1.Click += (_, _) =>
            {
                // Cycle through three demo types
                int type = _docCounter % 3;
                if (type == 0)
                    AddSampleDocument($"Document {_docCounter + 1}");
                else if (type == 1)
                    AddEditorDocument($"Editor {_docCounter + 1}");
                else
                    AddMarkdownDocument($"Note {_docCounter + 1}");
            };
        }

        // ── Startup documents ─────────────────────────────────────────────────

        private void OpenStartupDocuments()
        {
            if (beepDocumentManager1.DocumentCount > 0) return;

            beepDocumentManager1.BeginBatchAddDocuments();
            try
            {
                // 1. Welcome viewer
                AddSampleDocument("Welcome",
                    "Press Ctrl+Shift+P for the command palette.\n" +
                    "Press Ctrl+K, H for keyboard shortcut help.\n" +
                    "Drag a tab to rearrange. Right-click a tab for more options.");

                // 2. Notes editor — demonstrates IsModified dirty guard
                AddEditorDocument("Notes");

                // 3. Markdown note — uses the registered "markdown" template
                AddMarkdownDocument("README");

                // 4. Settings panel — demonstrates a structured content document
                AddSettingsDocument();
            }
            finally
            {
                beepDocumentManager1.EndBatchAddDocuments();
            }

            // Activate the Welcome document
            var welcome = beepDocumentManager1.GetPanel("welcome");
            if (welcome != null)
                beepTabbedView1.Host?.SetActiveDocument(welcome.DocumentId);
        }

        // ── Document factory helpers ──────────────────────────────────────────

        private void AddSampleDocument(string title, string hint = "")
        {
            _docCounter++;
            var panel = beepDocumentManager1.AddDocument(title, activate: false);
            if (panel == null) return;

            panel.Controls.Add(new Label
            {
                Text      = string.IsNullOrEmpty(hint) ? title : hint,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = new Font("Segoe UI", 11f),
                ForeColor = SystemColors.GrayText,
                Padding   = new Padding(24),
            });
        }

        private void AddEditorDocument(string title)
        {
            _docCounter++;
            var panel = beepDocumentManager1.AddDocument(title, activate: false);
            if (panel == null) return;

            var rtb = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font        = new Font("Consolas", 11f),
            };
            rtb.TextChanged += (_, _) => panel.IsModified = true;
            panel.Controls.Add(rtb);
        }

        private void AddMarkdownDocument(string title, bool activate = false)
        {
            _docCounter++;
            var panel = beepDocumentManager1.AddDocument(title, activate: activate);
            if (panel == null) return;

            var rtb = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font        = new Font("Consolas", 11f),
                BackColor   = Color.FromArgb(30, 30, 30),
                ForeColor   = Color.FromArgb(212, 212, 212),
                Text        = $"# {title}\n\n",
            };
            rtb.TextChanged += (_, _) => panel.IsModified = true;
            panel.Controls.Add(rtb);
        }

        private void AddSettingsDocument()
        {
            _docCounter++;
            var panel = beepDocumentManager1.AddDocument("Settings", activate: false);
            if (panel == null) return;

            var table = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 2,
                RowCount    = 4,
                Padding     = new Padding(16),
                AutoSize    = false,
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

            AddSettingsRow(table, 0, "Theme",       "Default");
            AddSettingsRow(table, 1, "Tab Style",   "Chrome");
            AddSettingsRow(table, 2, "Auto Save",   "Enabled");
            AddSettingsRow(table, 3, "Session File", "%AppData%\\Beep\\Sessions\\MainFrm_MDI.json");

            panel.Controls.Add(table);
        }

        private static void AddSettingsRow(TableLayoutPanel table, int row,
                                           string label, string value)
        {
            table.Controls.Add(new Label
            {
                Text      = label,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = SystemColors.ControlText,
            }, 0, row);

            table.Controls.Add(new Label
            {
                Text      = value,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font      = new Font("Segoe UI", 10f),
                ForeColor = SystemColors.GrayText,
            }, 1, row);
        }

        // ── Host actions ──────────────────────────────────────────────────────

        private void SplitActiveHorizontal()
        {
            var host = beepTabbedView1.Host;
            if (host?.ActiveDocumentId == null) return;
            host.SplitDocumentHorizontal(host.ActiveDocumentId);
        }
    }
}

