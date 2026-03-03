// BeepDocumentHost.Events.cs
// Tab-strip event handlers for BeepDocumentHost, plus the nested
// BeepDocumentFloatWindow that hosts detached document panels.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    // ── Tab-strip event handlers ──────────────────────────────────────────────

    public partial class BeepDocumentHost
    {
        private void OnTabSelected(object? sender, TabEventArgs e)
            => SetActiveDocument(e.Tab.Id);

        /// <summary>
        /// Fires before actual close — gives the host consumer a chance to cancel
        /// (e.g., "Save changes?" dialog).
        /// </summary>
        private void OnTabClosing(object? sender, TabClosingEventArgs e)
        {
            if (!_panels.TryGetValue(e.Tab.Id, out var panel)) return;

            // Raise host-level cancellable event
            DocumentClosing?.Invoke(this, e);
            // If consumed code sets e.Cancel = true, the strip will not fire TabCloseRequested
        }

        private void OnTabCloseRequested(object? sender, TabEventArgs e)
            => CloseDocument(e.Tab.Id);

        private void OnAddButtonClicked(object? sender, EventArgs e)
            => NewDocumentRequested?.Invoke(this, EventArgs.Empty);

        private void OnTabReordered(object? sender, TabReorderArgs e)
        {
            // Visual reorder only — dictionary keys are document ids, order is irrelevant.
        }

        private void OnTabFloatRequested(object? sender, TabEventArgs e)
        {
            // Try transferring to another registered host when the feature is enabled
            if (_allowDragBetweenHosts)
            {
                var cursor = System.Windows.Forms.Cursor.Position;
                var targetHost = BeepDocumentDragManager.FindHostAtPoint(cursor, exclude: this);
                if (targetHost != null && targetHost._allowDragBetweenHosts)
                {
                    var args = new DocumentTransferEventArgs(e.Tab.Id, targetHost);
                    DocumentDetaching?.Invoke(this, args);
                    if (!args.Cancel)
                    {
                        targetHost.AttachExternalDocument(this, e.Tab.Id);
                        return; // skip floating
                    }
                }
            }

            FloatDocument(e.Tab.Id);
        }

        private void OnTabPinToggled(object? sender, TabEventArgs e)
        {
            if (!_panels.TryGetValue(e.Tab.Id, out var panel)) return;
            panel.CanClose = !e.Tab.IsPinned;
            DocumentPinChanged?.Invoke(this,
                new DocumentEventArgs(e.Tab.Id, panel.DocumentTitle));
        }

        // ── Host-level keyboard shortcut routing ──────────────────────────────
        // Forward to tab strip when any child control is focused
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            if (_keyboardShortcutsEnabled)
            {
                // Ctrl+Shift+T — reopen last closed document
                if (keyData == (Keys.Control | Keys.Shift | Keys.T))
                {
                    ReopenLastClosed();
                    return true;
                }

                // Ctrl+P — quick-switch document picker
                if (keyData == (Keys.Control | Keys.P))
                {
                    ShowQuickSwitch();
                    return true;
                }

                if (_tabStrip.HandleShortcut(keyData))
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    // ── BeepDocumentFloatWindow ───────────────────────────────────────────────

    /// <summary>
    /// Themed floating window that hosts a single <see cref="BeepDocumentPanel"/>
    /// when it has been dragged out of the <see cref="BeepDocumentHost"/>.
    /// </summary>
    internal class BeepDocumentFloatWindow : BeepiFormPro
    {
        private BeepDocumentPanel? _panel;

        // ── Win32 message constants ───────────────────────────────────────────
        private const int WM_MOVE        = 0x0003;
        private const int WM_NCLBUTTONUP = 0x00A2;
        private const int HTCAPTION      = 2;

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Raised when the user requests to dock this window back into the host.</summary>
        public event EventHandler? DockBack;

        /// <summary>
        /// Raised whenever the OS moves this window (includes title-bar drag).
        /// The event argument is the current top-left screen position.
        /// </summary>
        public event EventHandler<Point>? WindowMoved;

        /// <summary>
        /// Raised when the user releases the left mouse button on the title bar
        /// (WM_NCLBUTTONUP / HTCAPTION).  Use this to finalise a dock-drop.
        /// </summary>
        public event EventHandler? TitleBarMouseUp;

        // ── Constructor ───────────────────────────────────────────────────────

        public BeepDocumentFloatWindow(BeepDocumentPanel panel, string themeName)
        {
            _panel = panel ?? throw new ArgumentNullException(nameof(panel));

            Text           = panel.DocumentTitle;
            Size           = new Size(800, 600);
            StartPosition  = FormStartPosition.CenterParent;

            if (!string.IsNullOrEmpty(themeName))
                ApplyTheme(themeName);   // BeepiFormPro exposes ApplyTheme(string)

            panel.Dock = DockStyle.Fill;
            Controls.Add(panel);
        }

        // ── WndProc ───────────────────────────────────────────────────────────

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_MOVE)
                WindowMoved?.Invoke(this, new Point(Left, Top));

            else if (m.Msg == WM_NCLBUTTONUP && m.WParam.ToInt32() == HTCAPTION)
                TitleBarMouseUp?.Invoke(this, EventArgs.Empty);
        }

        // ── Public helpers ────────────────────────────────────────────────────

        /// <summary>
        /// Removes the hosted panel from this window without disposing it,
        /// so the host can re-adopt it.
        /// </summary>
        public BeepDocumentPanel DetachPanel()
        {
            if (_panel == null)
                throw new InvalidOperationException("Panel already detached.");

            Controls.Remove(_panel);
            _panel.Dock = DockStyle.None;
            var detached = _panel;
            _panel = null;
            return detached;
        }

        /// <summary>Call from a toolbar button or menu item to trigger dock-back.</summary>
        protected void OnDockBackRequested()
            => DockBack?.Invoke(this, EventArgs.Empty);
    }
}
