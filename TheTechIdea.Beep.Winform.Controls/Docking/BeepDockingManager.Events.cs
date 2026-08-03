using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Raises the manager's events. Every notification the docking system emits passes through here.
    /// </summary>
    /// <remarks>
    /// Extracted from <c>BeepDockingManager.cs</c>, which had grown to 3,317 lines with no region
    /// markers at all. The split is by concern, established by reading the file in order rather
    /// than assumed: the Filtering decomposition began with a hypothesis that reading disproved.
    /// </remarks>
    public partial class BeepDockingManager
    {
        /// <summary>Raises the <see cref="PanelActivated"/> event.</summary>
        protected virtual void OnPanelActivated(DockPanel panel) =>
            PanelActivated?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelAdded"/> event.</summary>
        protected virtual void OnPanelAdded(DockPanel panel) =>
            PanelAdded?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelRemoved"/> event.</summary>
        protected virtual void OnPanelRemoved(DockPanel panel) =>
            PanelRemoved?.Invoke(this, panel);

        /// <summary>Raises the <see cref="ThemeChanged"/> event.</summary>
        protected virtual void OnThemeChangedRaised() =>
            ThemeChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>Raises the <see cref="PanelFloated"/> event.</summary>
        protected virtual void OnPanelFloated(DockPanel panel) =>
            PanelFloated?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelAutoHidden"/> event.</summary>
        protected virtual void OnPanelAutoHidden(DockPanel panel) =>
            PanelAutoHidden?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelHidden"/> event.</summary>
        protected virtual void OnPanelHidden(DockPanel panel) =>
            PanelHidden?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelShown"/> event.</summary>
        protected virtual void OnPanelShown(DockPanel panel) =>
            PanelShown?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelClosed"/> event.</summary>
        protected virtual void OnPanelClosed(DockPanel panel) =>
            PanelClosed?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelReopened"/> event.</summary>
        protected virtual void OnPanelReopened(DockPanel panel) =>
            PanelReopened?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelMovedBetweenGroups"/> event.</summary>
        protected virtual void OnPanelMovedBetweenGroups(PanelMovedBetweenGroupsEventArgs e) =>
            PanelMovedBetweenGroups?.Invoke(this, e);

        /// <summary>Raises the <see cref="PanelStateChanging"/> event. Returns true when the
        /// change is allowed (no subscriber canceled it).</summary>
        protected virtual bool RaiseStateChanging(DockPanel panel,
            DockPanelState currentState, DockPanelState requestedState)
        {
            if (panel == null || PanelStateChanging == null) return true;
            var args = new PanelStateChangingEventArgs(panel, currentState, requestedState);
            PanelStateChanging.Invoke(this, args);
            return !args.Cancel;
        }

        /// <summary>Raises the <see cref="PageCloseRequest"/> event.</summary>
        protected virtual void OnPageCloseRequest(PanelCloseRequestEventArgs e) =>
            PageCloseRequest?.Invoke(this, e);

        /// <summary>Raises the <see cref="PageDockedRequest"/> event.</summary>
        protected virtual void OnPageDockedRequest(CancelPanelRequestEventArgs e) =>
            PageDockedRequest?.Invoke(this, e);

        /// <summary>Raises the <see cref="PageAutoHiddenRequest"/> event.</summary>
        protected virtual void OnPageAutoHiddenRequest(CancelPanelRequestEventArgs e) =>
            PageAutoHiddenRequest?.Invoke(this, e);

        /// <summary>Raises the <see cref="PageFloatingRequest"/> event.</summary>
        protected virtual void OnPageFloatingRequest(CancelPanelRequestEventArgs e) =>
            PageFloatingRequest?.Invoke(this, e);

        /// <summary>Raises the <see cref="ShowPanelContextMenu"/> event.</summary>
        protected virtual void OnShowPanelContextMenu(PanelContextMenuEventArgs e) =>
            ShowPanelContextMenu?.Invoke(this, e);

        /// <summary>Raises the <see cref="FloatingWindowAdding"/> event.</summary>
        protected virtual void OnFloatingWindowAdding(FloatingWindowEventArgs e) =>
            FloatingWindowAdding?.Invoke(this, e);

        /// <summary>Raises the <see cref="FloatingWindowRemoved"/> event.</summary>
        protected virtual void OnFloatingWindowRemoved(FloatingWindowEventArgs e) =>
            FloatingWindowRemoved?.Invoke(this, e);

        /// <summary>Raises the <see cref="AutoHiddenGroupAdding"/> event.</summary>
        protected virtual void OnAutoHiddenGroupAdding(AutoHiddenGroupEventArgs e) =>
            AutoHiddenGroupAdding?.Invoke(this, e);

        /// <summary>Raises the <see cref="AutoHiddenGroupRemoved"/> event.</summary>
        protected virtual void OnAutoHiddenGroupRemoved(AutoHiddenGroupEventArgs e) =>
            AutoHiddenGroupRemoved?.Invoke(this, e);

        /// <summary>Raises the <see cref="DockspaceAdding"/> event.</summary>
        protected virtual void OnDockspaceAdding(DockspaceEventArgs e) =>
            DockspaceAdding?.Invoke(this, e);

        /// <summary>Raises the <see cref="DockspaceRemoved"/> event.</summary>
        protected virtual void OnDockspaceRemoved(DockspaceEventArgs e) =>
            DockspaceRemoved?.Invoke(this, e);

        /// <summary>Raises the <see cref="DockspaceSeparatorResize"/> event.</summary>
        protected virtual void OnDockspaceSeparatorResize(SeparatorResizeEventArgs e) =>
            DockspaceSeparatorResize?.Invoke(this, e);

        /// <summary>Raises the <see cref="AutoHiddenSeparatorResize"/> event.</summary>
        protected virtual void OnAutoHiddenSeparatorResize(SeparatorResizeEventArgs e) =>
            AutoHiddenSeparatorResize?.Invoke(this, e);

        /// <summary>Raises the <see cref="DoDragDropEnd"/> event.</summary>
        protected virtual void OnDoDragDropEnd() =>
            DoDragDropEnd?.Invoke(this, EventArgs.Empty);

        /// <summary>Raises the <see cref="DoDragDropQuit"/> event.</summary>
        protected virtual void OnDoDragDropQuit() =>
            DoDragDropQuit?.Invoke(this, EventArgs.Empty);

        // ── MRU (Most Recently Used) panel ordering ─────────────────────────

    }
}
