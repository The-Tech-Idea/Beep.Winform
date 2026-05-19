using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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



        // ── Construction ──────────────────────────────────────────────────────

        public MainFrm_MDI()
        {
            InitializeComponent();
         
        }

        // ── Form Load ─────────────────────────────────────────────────────────

    }
}

