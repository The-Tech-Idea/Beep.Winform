// BeepDockManager.CrossHost.cs
// Cross-host dock-panel transfer — partial class extension.
//
// When a floating dock-panel window is dragged over a different
// BeepDocumentHost, the manager detects the hover and, when the float window
// is closed by the user, re-parents the panel to the target host's manager
// (creating one if none exists on that host's form).
//
// How it works:
//   1. A static registry maps each live BeepDockManager to its Host.
//   2. BuildFloatWindow hooks LocationChanged: detect hover → title hint.
//   3. BuildFloatWindow hooks FormClosing: if cursor is over a different host
//      → call TransferPanel() to move the descriptor to the target host's manager.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDockManager
    {
        // ─────────────────────────────────────────────────────────────────────
        // Static registry
        // ─────────────────────────────────────────────────────────────────────

        private static readonly List<WeakReference<BeepDockManager>> _registry =
            new();

        private static void RegisterManager(BeepDockManager mgr)
        {
            lock (_registry)
                _registry.Add(new WeakReference<BeepDockManager>(mgr));
        }

        private static void UnregisterManager(BeepDockManager mgr)
        {
            lock (_registry)
                _registry.RemoveAll(wr =>
                    !wr.TryGetTarget(out var m) || ReferenceEquals(m, mgr));
        }

        /// <summary>
        /// Returns all live <see cref="BeepDockManager"/> instances whose <see cref="Host"/>
        /// is <paramref name="host"/>. Used by serialisation helpers.
        /// </summary>
        internal static IReadOnlyList<BeepDockManager> FindManagersForHost(BeepDocumentHost host)
        {
            var result = new List<BeepDockManager>();
            lock (_registry)
            {
                for (int i = _registry.Count - 1; i >= 0; i--)
                {
                    if (!_registry[i].TryGetTarget(out var mgr))
                    {
                        _registry.RemoveAt(i);
                        continue;
                    }
                    if (ReferenceEquals(mgr._host, host))
                        result.Add(mgr);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the <see cref="BeepDockManager"/> whose <see cref="Host"/> is
        /// <paramref name="host"/>, or null if none is registered.
        /// </summary>
        private static BeepDockManager? FindManagerForHost(BeepDocumentHost host)
        {
            lock (_registry)
            {
                for (int i = _registry.Count - 1; i >= 0; i--)
                {
                    if (!_registry[i].TryGetTarget(out var mgr))
                    {
                        _registry.RemoveAt(i);
                        continue;
                    }
                    if (ReferenceEquals(mgr._host, host))
                        return mgr;
                }
            }
            return null;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Hook points called from the main BeepDockManager constructor / Dispose
        // ─────────────────────────────────────────────────────────────────────

        partial void InitCrossHost()  => RegisterManager(this);
        partial void CleanupCrossHost() => UnregisterManager(this);

        // ─────────────────────────────────────────────────────────────────────
        // Augmented float window — adds cross-host drag awareness
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Called from <see cref="BuildFloatWindow"/> to attach cross-host drag
        /// behaviour to a newly created float window.
        /// </summary>
        private void AttachCrossHostDrag(Form win, DockPanelDescriptor desc)
        {
            string id = desc.Id;

            win.LocationChanged += (_, _) =>
            {
                var pt         = Cursor.Position;
                var targetHost = BeepDocumentDragManager.FindHostAtPoint(pt, _host);
                // Give the user a visual hint in the title bar
                win.Text = targetHost is not null
                    ? $"{desc.Title}  [drop to dock here]"
                    : desc.Title;
            };

            win.FormClosing += (_, fce) =>
            {
                // Only intercept user-initiated closes (title bar X), not
                // programmatic closes from DockPanel() / HidePanel().
                if (fce.CloseReason != CloseReason.UserClosing) return;

                var pt         = Cursor.Position;
                var targetHost = BeepDocumentDragManager.FindHostAtPoint(pt, _host);
                if (targetHost is null) return;                 // normal close, dock back to this host

                // Cancel the close — we will re-dock programmatically
                fce.Cancel = true;

                // Remove content from the float window before it is destroyed
                if (desc.Content is not null && win.Controls.Contains(desc.Content))
                    win.Controls.Remove(desc.Content);

                // Transfer to the target
                TransferPanel(desc, targetHost);
            };
        }

        // ─────────────────────────────────────────────────────────────────────
        // Transfer logic
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Moves <paramref name="desc"/> from this manager to the manager attached
        /// to <paramref name="targetHost"/> (creating one if none exists).
        /// The panel is docked on the same edge at the target host in pinned state.
        /// </summary>
        private void TransferPanel(DockPanelDescriptor desc, BeepDocumentHost targetHost)
        {
            string id = desc.Id;

            // Remove from this manager (no rail events — we're doing this programmatically)
            _panels.ListChanged -= OnPanelListChanged;
            try
            {
                _panels.Remove(desc);
                // Clean up this manager's rail entry for this panel
                GetRail(desc.Edge)?.HidePanel(id);
                if (_floatWindows.TryGetValue(id, out var win))
                {
                    _floatWindows.Remove(id);
                    win.Dispose();
                }
            }
            finally
            {
                _panels.ListChanged += OnPanelListChanged;
            }

            // Find or create a manager on the target host
            var targetMgr = FindManagerForHost(targetHost);
            if (targetMgr is null)
            {
                // Create a transient manager for the target host (no designer owner)
                targetMgr = new BeepDockManager { Host = targetHost };
            }

            // Add the descriptor to the target manager and show it pinned
            desc.IsAutoHidden = false;
            desc.IsVisible    = true;
            targetMgr._panels.Add(desc);
            targetMgr.EnsureRail(desc.Edge).ShowPinned(desc);
        }
    }
}
