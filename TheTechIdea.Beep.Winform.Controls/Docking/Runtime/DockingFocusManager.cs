using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// Centralized focus routing for docking surfaces. Mirrors DockPanelSuite's focus discipline:
    /// activating a panel should focus the first focusable child of its content (or the panel
    /// itself if no nested focusable exists), and the auto-hide slide-in should call the same
    /// route so behavior is consistent.
    ///
    /// The manager owns one instance and uses it in:
    ///   <list type="bullet">
    ///     <item><see cref="AutoHideStrip.SlideShown"/> handling (auto-hide slide-in)</item>
    ///     <item><see cref="BeepDockingManager.ActivatePanel"/> (mouse / programmatic activate)</item>
    ///     <item><see cref="BeepDockspace"/> tab clicks (user activates a hidden tab)</item>
    ///   </list>
    ///
    /// Hosts that need custom focus discipline (e.g., don't focus text boxes automatically) can
    /// replace the instance via <see cref="BeepDockingManager.FocusManager"/>.
    /// </summary>
    public class DockingFocusManager
    {
        private readonly BeepDockingManager _manager;

        /// <summary>Defaults to true: focuses the panel and any first focusable child.</summary>
        public bool FocusOnActivate { get; set; } = true;

        /// <summary>Defaults to true: brings the panel to the front of its parent before focusing.</summary>
        public bool BringToFrontOnFocus { get; set; } = true;

        public DockingFocusManager(BeepDockingManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Focuses <paramref name="panel"/> per the configured policy: bring-to-front (when
        /// enabled), then focus the deepest focusable descendant of the panel's content (or the
        /// panel itself). No-op when <paramref name="panel"/> is null, disposed, or hidden.
        /// </summary>
        public void Focus(DockPanel panel)
        {
            if (panel == null || panel.IsDisposed || !panel.Visible || !FocusOnActivate)
                return;

            if (BringToFrontOnFocus)
            {
                try { panel.BringToFront(); }
                catch { /* parenting may have changed; safe to skip */ }
            }

            // If the panel hosts user content, find the first focusable child inside it.
            Control content = panel.Content ?? panel;
            Control focusTarget = FindFirstFocusable(content) ?? content;
            try
            {
                if (focusTarget.CanFocus)
                    focusTarget.Focus();
                else if (panel.CanFocus)
                    panel.Focus();
            }
            catch
            {
                // Some host controls (e.g. WebBrowser) refuse Focus() during paint or layout.
            }
        }

        /// <summary>
        /// Recursively searches <paramref name="root"/> for the first focusable control. Returns
        /// the first TabStop + Visible + Enabled child found, or null. Skips the root itself
        /// (we want a descendant that can actually accept text input).
        /// </summary>
        public static Control FindFirstFocusable(Control root)
        {
            if (root == null) return null;
            foreach (Control child in root.Controls)
            {
                if (child == null) continue;
                if (child.Visible && child.Enabled && child.TabStop && child.CanFocus)
                    return child;
                Control nested = FindFirstFocusable(child);
                if (nested != null)
                    return nested;
            }
            return null;
        }
    }
}
