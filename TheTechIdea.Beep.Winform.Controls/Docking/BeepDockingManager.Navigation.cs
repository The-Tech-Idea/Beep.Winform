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
    /// Most-recently-used panel tracking, and the Ctrl+Tab navigator it drives.
    /// </summary>
    /// <remarks>
    /// Extracted from <c>BeepDockingManager.cs</c>, which had grown to 3,317 lines with no region
    /// markers at all. The MRU list, the host-form key hooks that drive it, and the navigator
    /// overlay it presents are one concern and now sit together.
    /// </remarks>
    public partial class BeepDockingManager
    {
        private void PushMrPanel(string panelKey)
        {
            if (string.IsNullOrEmpty(panelKey)) return;
            _mruList.Remove(panelKey);
            _mruList.AddFirst(panelKey);
        }

        internal void RemoveMrPanel(string panelKey)
        {
            _mruList.Remove(panelKey);
        }

        private string GetNextMrPanel(bool forward)
        {
            if (_mruList.Count == 0) return null;

            string active = _mruList.First?.Value;
            if (active == null) return null;

            var current = _mruList.Find(active);
            if (current == null) return _mruList.First.Value;

            if (forward)
            {
                var next = current.Next ?? _mruList.First;
                return next?.Value;
            }
            else
            {
                var prev = current.Previous ?? _mruList.Last;
                return prev?.Value;
            }
        }

        // ── Keyboard handling (Ctrl+Tab navigator, Ctrl+F4, Escape) ────────────

        private void OnHostFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled || _disposed) return;

            if (e.Control && !e.Shift && e.KeyCode == Keys.Tab)
            {
                if (_navigator == null || _navigator.IsDisposed)
                {
                    ShowNavigator();
                }
                else
                {
                    _navigator.SelectNext();
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.Tab)
            {
                if (_navigator == null || _navigator.IsDisposed)
                {
                    ShowNavigator();
                    _navigator?.SelectPrevious();
                }
                else
                {
                    _navigator.SelectPrevious();
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.Control && (e.KeyCode == Keys.F4 || e.KeyCode == Keys.W))
            {
                // Only consume the key when we actually have a panel to close — otherwise
                // let TextBox/other controls see Ctrl+W (delete word) and Ctrl+F4 as usual.
                string activeKey = GetActivePanelKey();
                if (!string.IsNullOrEmpty(activeKey))
                {
                    ClosePanel(activeKey);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Control && e.Shift && !e.Alt && e.KeyCode == Keys.Left)
            {
                // !e.Alt matters: Ctrl+Alt+Shift+Left is the edge-resize binding below, and this
                // is an else-if chain - a guard loose enough to match it would consume the key,
                // decline to act, and leave the resize branch unreachable.
                if (MoveActivePanel(-1))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Control && e.Shift && !e.Alt && e.KeyCode == Keys.Right)
            {
                // !e.Alt matters: Ctrl+Alt+Shift+Right is the edge-resize binding below, and this
                // is an else-if chain - a guard loose enough to match it would consume the key,
                // decline to act, and leave the resize branch unreachable.
                if (MoveActivePanel(1))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Alt && !e.Control && !e.Shift &&
                     e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D9)
            {
                // Rider's Alt+1..9. Indexes the docked panels in a stable order so the same key
                // reaches the same panel between presses.
                if (FocusPanelByIndex(e.KeyCode - Keys.D1))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.F12)
            {
                // Rider's Ctrl+Shift+F12 - maximise the active panel, or restore it.
                string activeKey = GetActivePanelKey();
                if (!string.IsNullOrEmpty(activeKey) && ToggleMaximise(activeKey))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Control && e.Alt && e.KeyCode == Keys.Z)
            {
                // VS Code binds zen to the Ctrl+K Z chord; WinForms key handling has no chord
                // support, so this is the single-stroke equivalent.
                string activeKey = GetActivePanelKey();
                if (!string.IsNullOrEmpty(activeKey) && ToggleZenMode(activeKey))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Control && e.Alt && !e.Shift &&
                     (e.KeyCode == Keys.Right || e.KeyCode == Keys.Down))
            {
                // Rider's Split Right / Split Down, driving the same CommitGroupEdge the drag path
                // uses rather than a second split implementation.
                string activeKey = GetActivePanelKey();
                bool split = !string.IsNullOrEmpty(activeKey) &&
                             (e.KeyCode == Keys.Right
                              ? SplitPanelRight(activeKey)
                              : SplitPanelDown(activeKey));
                if (split)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Control && e.Alt && e.Shift &&
                     (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right ||
                      e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
            {
                // Resize the active panel's edge without a pointer. Ctrl+Shift+arrows is already
                // taken by MoveActivePanel, so resize takes the Shift-extended chord.
                int delta = (e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
                    ? -KeyboardResizeStep
                    : KeyboardResizeStep;

                bool horizontalKey = e.KeyCode == Keys.Left || e.KeyCode == Keys.Right;
                if (ResizeActivePanelEdge(delta, horizontalKey))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                bool handled = false;
                if (_navigator != null && !_navigator.IsDisposed)
                {
                    _navigator.Cancel();
                    handled = true;
                }
                else if (_dragController != null && _dragController.IsDragging)
                {
                    _dragController.Cancel();
                    foreach (var dockspace in GetManagedDockspaces())
                        dockspace.CancelDrag();
                    handled = true;
                }
                else if (IsMaximised)
                {
                    // Last of the three, not first: cancelling an in-flight drag or a navigator
                    // is the more urgent reading of Escape, and a maximise is still on screen to
                    // dismiss afterwards.
                    RestoreFromMaximise();
                    handled = true;
                }

                // Only consume Escape if we actually had something to cancel; otherwise let
                // the focused control (e.g. a TextBox) see the key and clear its content.
                if (handled)
                    e.Handled = true;
            }
        }

        /// <summary>Pixels an edge moves per keyboard resize keystroke.</summary>
        private const int KeyboardResizeStep = 16;

        /// <summary>
        /// Focuses the docked panel at <paramref name="index"/> in a stable order — Rider's
        /// <c>Alt+1..9</c>.
        /// </summary>
        /// <remarks>
        /// Ordered by dock position then key rather than by registration order, so the same
        /// keystroke reaches the same panel every time. Registration order would shuffle the
        /// bindings as panels are closed and reopened, which is worse than no binding.
        /// </remarks>
        public bool FocusPanelByIndex(int index)
        {
            if (index < 0)
                return false;

            var ordered = _panelsByKey.Values
                .Where(p => p != null && p.State == DockPanelState.Docked)
                .OrderBy(p => (int)p.DockPosition)
                .ThenBy(p => p.Key, StringComparer.Ordinal)
                .ToList();

            if (index >= ordered.Count)
                return false;

            return ActivatePanel(ordered[index].Key);
        }

        /// <summary>
        /// Moves the active panel's edge divider by <paramref name="deltaPx"/>, the keyboard
        /// equivalent of dragging its splitter.
        /// </summary>
        /// <remarks>
        /// The delta is in screen-axis terms: positive moves the divider right or down. That means
        /// the arrow key moves the <b>divider</b>, not the panel's size — pressing Right widens a
        /// Left-docked panel and narrows a Right-docked one, which is what dragging the same
        /// splitter does and what Visual Studio and Rider both do.
        /// <para>
        /// A keystroke on the wrong axis is refused rather than applied to the other one: Up/Down
        /// does nothing to a Left-docked panel, so the key falls through to the focused control
        /// instead of silently resizing something the user was not pointing at.
        /// </para>
        /// </remarks>
        /// <param name="deltaPx">Pixels to move the divider; positive is right or down.</param>
        /// <param name="horizontal">
        /// True for a Left/Right arrow, false for Up/Down. The edge is only resized when its axis
        /// matches.
        /// </param>
        public bool ResizeActivePanelEdge(int deltaPx, bool horizontal)
        {
            if (deltaPx == 0 || _layoutController == null)
                return false;

            string key = GetActivePanelKey();
            if (string.IsNullOrEmpty(key))
                return false;

            var panel = GetPanel(key);
            if (panel?.Group == null)
                return false;

            // Walk to the root edge group: that is the one carrying the splitter ratio.
            var group = panel.Group;
            while (group.Parent != null && group.Parent != _layoutTree.Root)
                group = group.Parent;

            if (group.Position == DockPosition.Fill)
                return false;

            bool horizontalEdge = group.Position == DockPosition.Left ||
                                  group.Position == DockPosition.Right;
            if (horizontalEdge != horizontal)
                return false;

            _layoutController.DragSplitter(group.Id, deltaPx);
            ApplyLayout();
            return true;
        }

        private void OnHostFormKeyUp(object sender, KeyEventArgs e)
        {
            if (_disposed) return;

            // Releasing Ctrl while the navigator is open commits the highlighted entry
            // (matches Visual Studio's Ctrl+Tab UX).
            if (e.KeyCode == Keys.ControlKey)
            {
                if (_navigator != null && !_navigator.IsDisposed)
                {
                    CommitNavigatorSelection();
                }
            }
        }

        private void ShowNavigator()
        {
            if (_hostForm == null || _hostForm.IsDisposed) return;

            var dockedPanels = _mruList
                .Select(k => _panelsByKey.TryGetValue(k, out var p) ? p : null)
                .Where(p => p != null && p.State == DockPanelState.Docked)
                .ToList();

            if (dockedPanels.Count == 0) return;

            Point screenCenter = _hostForm.PointToScreen(
                new Point(_hostForm.ClientSize.Width / 2, _hostForm.ClientSize.Height / 2));

            _navigator = new BeepDockingNavigator(dockedPanels, _themeColors, screenCenter);
            _navigator.FormClosed += (_, _) => _navigator = null;
            _navigator.Show(_hostForm);
        }

        private void CommitNavigatorSelection()
        {
            if (_navigator == null || _navigator.IsDisposed) return;

            string key = _navigator.SelectedPanelKey;
            if (!string.IsNullOrEmpty(key))
            {
                ActivatePanel(key);
            }
            _navigator.Close();
            _navigator = null;
        }
    }
}
