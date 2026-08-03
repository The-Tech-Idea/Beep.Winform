using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Bulk panel operations, the store/restore set, and the state-change request API.
    /// </summary>
    /// <remarks>
    /// Extracted from <c>BeepDockingManager.cs</c>. These are the coarse-grained entry points a host
    /// application calls - show or hide many panels at once, park panels and bring them back, or ask
    /// the manager to move a panel between docked, floating and auto-hidden. They share a shape:
    /// each is a request that may be refused, as distinct from the direct operations that simply act.
    /// </remarks>
    public partial class BeepDockingManager
    {
        /// <summary>Shows multiple panels by key in a single layout pass.</summary>
        public void ShowPanels(IReadOnlyList<string> panelKeys)
        {
            ArgumentNullException.ThrowIfNull(panelKeys);
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in panelKeys)
                ShowPanel(key);
        }

        /// <summary>Hides multiple panels by key in a single layout pass.</summary>
        public void HidePanels(IReadOnlyList<string> panelKeys)
        {
            ArgumentNullException.ThrowIfNull(panelKeys);
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in panelKeys)
                HidePanel(key);
        }

        /// <summary>Shows all panels in a single layout pass.</summary>
        public void ShowAllPanels()
        {
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in _panelsByKey.Keys.ToList())
                ShowPanel(key);
        }

        /// <summary>Hides all panels in a single layout pass.</summary>
        public void HideAllPanels()
        {
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in _panelsByKey.Keys.ToList())
                HidePanel(key);
        }

        // ── Store / restore (mirrors Krypton StorePage / ClearStoredPage) ─────────────

        /// <summary>
        /// Closes a panel and stores it so it can be restored later.
        /// Equivalent to Krypton's <c>StorePage</c> — the panel is not disposed.
        /// </summary>
        public void StorePanel(string panelKey) => ClosePanel(panelKey);

        /// <summary>
        /// Restores a previously stored panel.  Equivalent to Krypton's <c>ClearStoredPage</c>
        /// combined with re-adding the page.
        /// </summary>
        public void RestoreStoredPanel(string panelKey) => ReopenPanel(panelKey);

        /// <summary>Restores all stored panels in a single layout pass.</summary>
        public void RestoreAllStoredPanels()
        {
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in _closedPanels.Keys.ToList())
                ReopenPanel(key);
        }

        /// <summary>Closes all live panels into the closed store in a single layout pass.</summary>
        public void StoreAllPanels()
        {
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in _panelsByKey.Keys.ToList())
                ClosePanel(key);
        }

        // ── Lookup / state helpers (mirrors Krypton Contains/FindPageLocation) ────────

        /// <summary>Returns true if the panel key exists in the live registry.</summary>
        public bool ContainsPanel(string panelKey)
        {
            if (string.IsNullOrWhiteSpace(panelKey))
                throw new ArgumentNullException(nameof(panelKey));
            return _panelsByKey.ContainsKey(panelKey);
        }

        /// <summary>
        /// Returns the current <see cref="DockPanelState"/> of the named panel,
        /// or <see cref="DockPanelState.Closed"/> if not found.
        /// Mirrors Krypton's <c>FindPageLocation</c>.
        /// </summary>
        public DockPanelState FindPanelLocation(string panelKey)
        {
            if (string.IsNullOrWhiteSpace(panelKey))
                throw new ArgumentNullException(nameof(panelKey));

            if (_panelsByKey.TryGetValue(panelKey, out var panel))
                return panel.State;

            if (_closedPanels.ContainsKey(panelKey))
                return DockPanelState.Closed;

            return DockPanelState.Closed;
        }

        // ── Cancel-able request entry points (mirrors Krypton Make*Request) ──────────

        /// <summary>
        /// Raises <see cref="PageDockedRequest"/> and, unless cancelled, docks the panel.
        /// Mirrors Krypton's <c>MakeDockedRequest</c>.
        /// </summary>
        public virtual void MakeDockedRequest(string panelKey)
        {
            var panel = GetPanel(panelKey);
            var args = new CancelPanelRequestEventArgs(panelKey, panel);
            OnPageDockedRequest(args);
            if (args.Cancel) return;

            if (panel?.State == DockPanelState.Floating)
                DockFloatingPanel(panelKey, panel.DockPosition);
            else if (panel?.State == DockPanelState.AutoHidden)
                RestoreAutoHiddenPanel(panelKey);
            else if (panel?.State == DockPanelState.Hidden)
                ShowPanel(panelKey);
            else if (_closedPanels.ContainsKey(panelKey))
                ReopenPanel(panelKey);
        }

        /// <summary>
        /// Raises <see cref="PageFloatingRequest"/> and, unless cancelled, floats the panel.
        /// Mirrors Krypton's <c>MakeFloatingRequest</c>.
        /// </summary>
        public virtual void MakeFloatingRequest(string panelKey)
        {
            var panel = GetPanel(panelKey);
            if (panel == null) return;

            var args = new CancelPanelRequestEventArgs(panelKey, panel);
            OnPageFloatingRequest(args);
            if (args.Cancel) return;
            FloatPanel(panelKey);
        }

        /// <summary>
        /// Raises <see cref="PageAutoHiddenRequest"/> and, unless cancelled, auto-hides the panel.
        /// Mirrors Krypton's <c>MakeAutoHiddenRequest</c>.
        /// </summary>
        public virtual void MakeAutoHiddenRequest(string panelKey)
        {
            var panel = GetPanel(panelKey);
            if (panel == null) return;

            var args = new CancelPanelRequestEventArgs(panelKey, panel);
            OnPageAutoHiddenRequest(args);
            if (args.Cancel) return;
            AutoHidePanel(panelKey);
        }

        /// <summary>
        /// Raises <see cref="PageCloseRequest"/> and, unless cancelled, closes the panel.
        /// Mirrors Krypton's <c>CloseRequest</c>.
        /// </summary>
        public virtual void CloseRequest(IReadOnlyList<string> panelKeys)
        {
            ArgumentNullException.ThrowIfNull(panelKeys);
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in panelKeys.ToList())
                CloseRequest(key);
        }

        /// <summary>
        /// Raises <see cref="PageCloseRequest"/> for a single panel and, unless cancelled,
        /// closes it. This is the entry point for user-initiated closes (caption close button,
        /// middle-click, context menu) so handlers can veto the close.
        /// </summary>
        public virtual void CloseRequest(string panelKey)
        {
            var panel = GetPanel(panelKey);
            if (panel == null)
                return;

            var args = new PanelCloseRequestEventArgs(panelKey, panel);
            OnPageCloseRequest(args);
            if (args.Cancel)
                return;

            switch (args.CloseRequest)
            {
                case DockingCloseRequest.None:
                    break;
                case DockingCloseRequest.RemovePanel:
                    RemovePanel(panelKey);
                    break;
                case DockingCloseRequest.RemovePanelAndDispose:
                    RemovePanel(panelKey);
                    break;
                case DockingCloseRequest.HidePanel:
                    HidePanel(panelKey);
                    break;
                default:
                    ClosePanel(panelKey);  // routes through HideOnClose, stores in _closedPanels
                    break;
            }
        }

        /// <summary>
        /// Raises <see cref="ShowPanelContextMenu"/> and, if a custom menu was supplied, shows it.
        /// Returns <c>true</c> when a custom menu was shown (built-in menu should be skipped).
        /// </summary>
        internal bool TryShowPanelContextMenu(DockPanel panel, Point clientLocation)
        {
            if (panel == null)
                return false;

            var screen = panel.PointToScreen(clientLocation);
            var args = new PanelContextMenuEventArgs(panel, screen);
            OnShowPanelContextMenu(args);

            if (args.ContextMenu == null)
                return false;

            args.ContextMenu.Show(panel, clientLocation);
            return true;
        }
    }
}
