using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Panel maximise and zen mode — the reversible takeover every reference product provides
    /// (VS Code <c>Ctrl+K Ctrl+M</c> / <c>Ctrl+K Z</c>, Rider <c>Ctrl+Shift+F12</c>, Visual Studio's
    /// double-click a document tab, Blender's <c>Ctrl+Space</c>).
    /// </summary>
    /// <remarks>
    /// The division of labour matters here. <see cref="DockingLayoutController.MaximisedPanelKey"/>
    /// owns the <b>geometry</b>: while it is set, layout allocates the whole container to one panel.
    /// This file owns the <b>controls</b>: which panels, dockspaces and rails are visible, which is
    /// something the layout controller deliberately never touches.
    /// <para>
    /// Neither side mutates the layout tree. Groups, split ratios, active tabs and
    /// <see cref="DockPanel.State"/> are all left exactly as they were, so
    /// <see cref="RestoreFromMaximise"/> is a clear-and-relayout rather than a reconstruction.
    /// Panels are concealed with <see cref="Control.Visible"/> alone — moving them to
    /// <see cref="DockPanelState.Hidden"/> would rewrite the very state a restore has to return.
    /// </para>
    /// </remarks>
    public partial class BeepDockingManager
    {
        // Dockspace tab positions captured when zen mode suppressed them, so the exact prior
        // header arrangement comes back rather than a guessed default.
        private readonly Dictionary<BeepDockspace, Models.HeaderPosition> _zenTabPositions =
            new Dictionary<BeepDockspace, Models.HeaderPosition>();

        // Captions suppressed by zen mode, keyed by panel, for the same reason.
        private readonly Dictionary<DockPanel, bool> _zenCaptions = new Dictionary<DockPanel, bool>();

        private bool _zenMode;

        /// <summary>Key of the maximised panel, or <c>null</c> when the layout is arranged normally.</summary>
        public string MaximisedPanelKey => _layoutController?.MaximisedPanelKey;

        /// <summary>True while any panel is maximised.</summary>
        public bool IsMaximised => _layoutController?.IsMaximised == true;

        /// <summary>
        /// True while <paramref name="panelKey"/> is the maximised panel. Caption painters read this
        /// to draw the restore affordance in place of the maximise one.
        /// </summary>
        public bool IsPanelMaximised(string panelKey)
            => !string.IsNullOrEmpty(panelKey) &&
               string.Equals(MaximisedPanelKey, panelKey, StringComparison.Ordinal);

        /// <summary>
        /// True while zen mode is active: a maximised panel plus suppressed chrome — dockspace
        /// headers, panel captions and auto-hide rails.
        /// </summary>
        public bool IsZenMode => _zenMode;

        /// <summary>Raised after a panel is maximised.</summary>
        public event EventHandler<DockPanel> PanelMaximised;

        /// <summary>Raised after a maximised panel is restored to the previous arrangement.</summary>
        public event EventHandler<DockPanel> PanelRestored;

        /// <summary>
        /// Gives <paramref name="panelKey"/> the whole docking host. Other docked panels are
        /// concealed, not closed and not moved.
        /// </summary>
        /// <returns>
        /// False when the panel is unknown, or is not <see cref="DockPanelState.Docked"/> — a
        /// floating, auto-hidden or hidden panel has no place in the docked layout to take over.
        /// </returns>
        public bool MaximisePanel(string panelKey)
        {
            if (_layoutController == null)
                return false;

            var panel = GetPanel(panelKey);
            if (panel == null || panel.State != DockPanelState.Docked)
                return false;

            if (IsPanelMaximised(panelKey))
                return true;

            _layoutController.MaximisedPanelKey = panel.Key;
            ApplyMaximiseVisibility();
            RecalculateLayout();

            OnPanelMaximised(panel);
            return true;
        }

        /// <summary>
        /// Restores the arrangement that was in place before the current maximise. Also leaves zen
        /// mode, since zen without a maximised panel is not a state this exposes.
        /// </summary>
        /// <returns>False when nothing was maximised.</returns>
        public bool RestoreFromMaximise()
        {
            if (_layoutController == null || !_layoutController.IsMaximised)
                return false;

            var panel = GetPanel(_layoutController.MaximisedPanelKey);

            if (_zenMode)
                RestoreChrome();

            _layoutController.MaximisedPanelKey = null;
            ApplyMaximiseVisibility();
            RecalculateLayout();

            if (panel != null)
                OnPanelRestored(panel);

            return true;
        }

        /// <summary>
        /// Maximises <paramref name="panelKey"/>, or restores if it is already maximised. This is
        /// what a caption double-click and the maximise keystroke both invoke.
        /// </summary>
        public bool ToggleMaximise(string panelKey)
            => IsPanelMaximised(panelKey) ? RestoreFromMaximise() : MaximisePanel(panelKey);

        /// <summary>
        /// Enters zen mode: maximises <paramref name="panelKey"/> and suppresses chrome — dockspace
        /// headers, the panel caption and the auto-hide rails.
        /// </summary>
        /// <remarks>
        /// Separate from <see cref="MaximisePanel"/> because users reach for the two differently:
        /// maximise is "show me more of this", zen is "show me only this".
        /// </remarks>
        public bool EnterZenMode(string panelKey)
        {
            if (!MaximisePanel(panelKey))
                return false;

            if (_zenMode)
                return true;

            SuppressChrome();
            _zenMode = true;
            RecalculateLayout();
            return true;
        }

        /// <summary>
        /// Leaves zen mode but keeps the panel maximised, restoring the chrome zen suppressed.
        /// </summary>
        /// <returns>False when zen mode was not active.</returns>
        public bool ExitZenMode()
        {
            if (!_zenMode)
                return false;

            RestoreChrome();
            RecalculateLayout();
            return true;
        }

        /// <summary>Enters zen mode for <paramref name="panelKey"/>, or leaves it if already in it.</summary>
        public bool ToggleZenMode(string panelKey)
            => _zenMode && IsPanelMaximised(panelKey) ? ExitZenMode() : EnterZenMode(panelKey);

        /// <summary>
        /// Shows only the maximised panel, or everything when nothing is maximised.
        /// </summary>
        /// <remarks>
        /// Hiding is by <see cref="Control.Visible"/> so the layout tree and every panel's
        /// <see cref="DockPanel.State"/> survive untouched. Dockspaces not hosting the maximised
        /// panel are hidden as a unit: a docked-but-invisible control reserves no space in the
        /// WinForms layout engine, so the hosting dockspace expands to the whole client area without
        /// its <see cref="Control.Dock"/> needing to change.
        /// </remarks>
        private void ApplyMaximiseVisibility()
        {
            string maximisedKey = _layoutController?.MaximisedPanelKey;
            bool maximised = !string.IsNullOrEmpty(maximisedKey);
            var maximisedPanel = maximised ? GetPanel(maximisedKey) : null;

            foreach (var panel in _panelsByKey.Values)
            {
                if (panel == null || panel.IsDisposed || panel.State != DockPanelState.Docked)
                    continue;

                panel.Visible = !maximised || ReferenceEquals(panel, maximisedPanel);
            }

            var hostDockspace = maximisedPanel?.Parent as BeepDockspace;
            foreach (var dockspace in GetManagedDockspaces())
            {
                if (dockspace == null || dockspace.IsDisposed)
                    continue;

                dockspace.Visible = !maximised || ReferenceEquals(dockspace, hostDockspace);
            }

            // Splitters are deliberately not touched here. A maximised layout result carries no
            // splitters, so SyncSplitters disposes them as orphans on the very next pass and
            // recreates them from the restored result — hiding them first would be redundant, and
            // would imply they survive the maximise when they do not.
        }

        /// <summary>Hides the chrome zen mode suppresses, recording what it changed.</summary>
        private void SuppressChrome()
        {
            foreach (var dockspace in GetManagedDockspaces())
            {
                if (dockspace == null || dockspace.IsDisposed ||
                    _zenTabPositions.ContainsKey(dockspace))
                    continue;

                _zenTabPositions[dockspace] = dockspace.TabPosition;
                dockspace.TabPosition = Models.HeaderPosition.None;
            }

            foreach (var panel in _panelsByKey.Values)
            {
                if (panel == null || panel.IsDisposed || _zenCaptions.ContainsKey(panel))
                    continue;

                _zenCaptions[panel] = panel.ShowCaption;
                panel.ShowCaption = false;
            }

            foreach (var strip in _autoHideStrips.Values)
            {
                if (strip != null && !strip.IsDisposed)
                    strip.Visible = false;
            }
        }

        /// <summary>Puts back exactly the chrome <see cref="SuppressChrome"/> took away.</summary>
        private void RestoreChrome()
        {
            foreach (var kv in _zenTabPositions)
            {
                if (kv.Key != null && !kv.Key.IsDisposed)
                    kv.Key.TabPosition = kv.Value;
            }
            _zenTabPositions.Clear();

            foreach (var kv in _zenCaptions)
            {
                if (kv.Key != null && !kv.Key.IsDisposed)
                    kv.Key.ShowCaption = kv.Value;
            }
            _zenCaptions.Clear();

            foreach (var strip in _autoHideStrips.Values)
            {
                if (strip != null && !strip.IsDisposed)
                    strip.Visible = true;
            }

            _zenMode = false;
        }

        /// <summary>
        /// Restores the arrangement first when an operation would remove the maximised panel.
        /// </summary>
        /// <remarks>
        /// Closing, hiding, floating or auto-hiding the panel that currently owns the whole
        /// container must not leave the layout maximised around a panel that is no longer there:
        /// every other panel would stay concealed with nothing occupying the space. Restoring first
        /// means the operation then runs against the normal arrangement, exactly as it would have if
        /// the panel had never been maximised.
        /// </remarks>
        private void RestoreIfMaximised(string panelKey)
        {
            if (IsPanelMaximised(panelKey))
                RestoreFromMaximise();
        }

        private void OnPanelMaximised(DockPanel panel)
            => PanelMaximised?.Invoke(this, panel);

        private void OnPanelRestored(DockPanel panel)
            => PanelRestored?.Invoke(this, panel);
    }
}
