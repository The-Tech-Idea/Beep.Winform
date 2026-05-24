using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// A floating tool-window form that hosts a single <see cref="DockPanel"/>.
    ///
    /// Design notes (follows DockPanelSuite FloatWindow + Krypton KryptonFloatingWindow):
    /// - FormBorderStyle = SizableToolWindow    (DockPanelSuite pattern)
    /// - ShowInTaskbar   = false                (both references)
    /// - Owner           = host form            (so minimise cascades)
    /// - TopLevel        = true
    /// - Double-click title bar re-docks the panel (WM_NCLBUTTONDBLCLK intercept)
    /// - Closing the window calls manager.ClosePanel or fires PanelRedocked depending on
    ///   CanClose / re-dock intent.
    ///
    /// Reference files:
    ///   dockpanelsuite-master\WinFormsUI\Docking\FloatWindow.cs
    ///   Krypton.Docking\Control Docking\KryptonFloatingWindow.cs
    /// </summary>
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    public class FloatWindow : Form
    {
        // Win32 message constants — same values used in DockPanelSuite FloatWindow
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;
        private const int HTCAPTION = 2;
        private const int WM_NCHITTEST = 0x0084;

        private DockPanel _panel;
        private bool _redocking;   // set true when re-dock is in progress so Closing handler knows

        // ── Events ──────────────────────────────────────────────────────────

        /// <summary>
        /// Raised when the user double-clicks the title bar to re-dock the panel.
        /// The manager subscribed in FloatPanel() will call DockFloatingPanel() on receipt.
        /// Mirrors DockPanelSuite VisibleNestedPanes / redock flow.
        /// </summary>
        public event EventHandler<DockPanel> PanelRedocked;

        // ── Constructors ─────────────────────────────────────────────────────

        /// <summary>
        /// Creates a FloatWindow at the default Windows position.
        /// </summary>
        /// <param name="panel">The DockPanel to float.</param>
        /// <param name="owner">The application's main/host form (sets Owner).</param>
        public FloatWindow(DockPanel panel, Form owner)
            : this(panel, owner, Rectangle.Empty) { }

        /// <summary>
        /// Creates a FloatWindow at an explicit screen rectangle.
        /// </summary>
        /// <param name="panel">The DockPanel to float.</param>
        /// <param name="owner">The application's main/host form.</param>
        /// <param name="initialBounds">Screen rectangle; Empty = Windows default position.</param>
        public FloatWindow(DockPanel panel, Form owner, Rectangle initialBounds)
        {
            _panel = panel ?? throw new ArgumentNullException(nameof(panel));

            // ── Window chrome — DockPanelSuite pattern ───────────────────────
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            ShowInTaskbar   = false;
            TopLevel        = true;
            ShowIcon        = false;
            Text            = panel.Title;

            if (owner != null)
                Owner = owner;

            // ── Position ────────────────────────────────────────────────────
            if (initialBounds.IsEmpty)
            {
                StartPosition = FormStartPosition.WindowsDefaultLocation;
                Size          = new Size(
                    panel.PreferredWidth  > 0 ? panel.PreferredWidth  + 16 : 300,
                    panel.PreferredHeight > 0 ? panel.PreferredHeight + 40 : 200);
            }
            else
            {
                StartPosition = FormStartPosition.Manual;
                Bounds        = initialBounds;
            }

            // ── Host the panel as the form's content ─────────────────────────
            // Remove the panel from its current parent so it can be re-parented here.
            // DockPanelSuite does the same when moving a DockPane into a FloatWindow.
            panel.Parent?.Controls.Remove(panel);
            panel.Dock = DockStyle.Fill;
            Controls.Add(panel);
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>The DockPanel currently hosted in this float window.</summary>
        public DockPanel Panel => _panel;

        // ── WndProc — intercept double-click on caption to re-dock ───────────

        /// <summary>
        /// Intercepts WM_NCLBUTTONDBLCLK on the caption bar to trigger re-dock.
        /// Mirrors DockPanelSuite FloatWindow WndProc handling.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                // Hit-test to confirm the click is on the caption
                var ht = (int)SendMessage(Handle, WM_NCHITTEST, IntPtr.Zero, m.LParam);
                if (ht == HTCAPTION)
                {
                    TriggerRedock();
                    return;   // swallow — don't pass to base (would maximise the tool window)
                }
            }
            base.WndProc(ref m);
        }

        // ── FormClosing ──────────────────────────────────────────────────────

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.Cancel) return;

            if (_redocking)
                return;  // redock path handles panel removal itself

            // User closed the float window — hide the panel (keep it alive for reopen)
            if (_panel != null)
            {
                _panel.Dock  = DockStyle.None;
                _panel.State = DockPanelState.Closed;
                _panel.OnClosed();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Do NOT dispose _panel — the manager owns it; only remove from our Controls
                if (_panel != null && Controls.Contains(_panel))
                    Controls.Remove(_panel);
                _panel = null;
            }
            base.Dispose(disposing);
        }

        // ── Private helpers ──────────────────────────────────────────────────

        private void TriggerRedock()
        {
            if (_panel == null) return;
            _redocking = true;

            // Remove the panel from the float window before the manager re-docks it
            _panel.Dock = DockStyle.None;
            Controls.Remove(_panel);

            PanelRedocked?.Invoke(this, _panel);

            Close();
        }

        // P/Invoke for WM_NCHITTEST — same interop used in DockPanelSuite
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
