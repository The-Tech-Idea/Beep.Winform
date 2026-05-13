using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views
{
    /// <summary>
    /// Sample MDI-style shell form that demonstrates <see cref="BeepDocumentHost"/> in action.
    ///
    /// Features shown:
    ///   • Multiple documents opened at startup (demonstrating AddDocument API)
    ///   • AutoSaveLayout / SessionFile — layout persists across restarts
    ///   • AttachWindowMenu — dynamic "Window" menu wired to the document host
    ///   • DocumentActivated event — updates form title
    ///   • Add Document button — calls AddDocument with incrementing counter
    ///
    /// Keyboard shortcuts (all processed by the host):
    ///   Ctrl+N          New document
    ///   Ctrl+W          Close active document
    ///   Ctrl+Shift+T    Reopen last closed
    ///   Ctrl+Alt+←/→    Move active document to adjacent split group
    ///   Ctrl+Shift+W    Close all tabs to the right
    ///   Ctrl+Shift+M    Maximize / restore active document
    ///   Ctrl+K, H       Show keyboard shortcut help
    /// </summary>
    public partial class MainFrm_MDI : TemplateForm
    {
        // Counter for auto-generated document titles
        private int _docCounter;

        public MainFrm_MDI()
        {
            InitializeComponent();
            Load    += OnFormLoad;
            FormClosing += OnFormClosing;
        }

        // ── Startup ──────────────────────────────────────────────────────────

        private void OnFormLoad(object? sender, EventArgs e)
        {
            ConfigureDocumentHost();
            OpenStartupDocuments();
            WireButton();
        }

        private void ConfigureDocumentHost()
        {
            // Persist the layout so split groups, tab order, and sizes survive restarts.
            var sessionDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Beep", "Sessions");
            Directory.CreateDirectory(sessionDir);

            beepDocumentHost1.AutoSaveLayout = true;
            beepDocumentHost1.SessionFile    = Path.Combine(sessionDir, "MainFrm_MDI.json");

            // Keep the form title in sync with the active document.
            beepDocumentHost1.ActiveDocumentChanged += (_, de) =>
            {
                Text = string.IsNullOrWhiteSpace(de.Title)
                    ? "MDI Host"
                    : $"{de.Title} – MDI Host";
            };
        }

        private void OpenStartupDocuments()
        {
            // If a saved session was restored, the host already has documents.
            if (beepDocumentHost1.DocumentCount > 0) return;

            beepDocumentHost1.BeginBatchAddDocuments();
            try
            {
                AddSampleDocument("Welcome",          "Press Ctrl+N to open a new document.");
                AddSampleDocument("Getting Started",  "Use Ctrl+K, H to view all keyboard shortcuts.");
                AddSampleDocument("Notes",            "Drag a tab down to float it as a separate window.");
            }
            finally
            {
                beepDocumentHost1.EndBatchAddDocuments();
            }
        }

        /// <summary>Creates a document with a centred read-only label as placeholder content.</summary>
        private void AddSampleDocument(string title, string hint = "")
        {
            _docCounter++;
            var panel = beepDocumentHost1.AddDocument(title, activate: false);

            var lbl = new Label
            {
                Text      = string.IsNullOrEmpty(hint) ? title : hint,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = new Font("Segoe UI", 12f),
                ForeColor = SystemColors.GrayText,
            };
            panel.Controls.Add(lbl);
        }

        private void WireButton()
        {
            beepButton1.Text   = "+ Add Document";
            beepButton1.Click += (_, _) =>
                AddSampleDocument($"Document {_docCounter + 1}");
        }

        // ── Shutdown ─────────────────────────────────────────────────────────

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            // AutoSaveLayout handles persistence; nothing extra needed here.
            beepDocumentHost1.Dispose();
        }
    }
}
