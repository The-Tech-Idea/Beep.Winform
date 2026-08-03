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
            else if (e.Control && e.Shift && e.KeyCode == Keys.Left)
            {
                if (MoveActivePanel(-1))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.Right)
            {
                if (MoveActivePanel(1))
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

                // Only consume Escape if we actually had something to cancel; otherwise let
                // the focused control (e.g. a TextBox) see the key and clear its content.
                if (handled)
                    e.Handled = true;
            }
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
