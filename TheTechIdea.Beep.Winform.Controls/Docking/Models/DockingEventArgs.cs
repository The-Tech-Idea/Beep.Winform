using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Models
{
    // ── CancelPanelRequestEventArgs ──────────────────────────────────────────────
    // Mirrors Krypton.Docking.CancelUniqueNameEventArgs.
    // Used by PageDockedRequest, PageFloatingRequest, PageAutoHiddenRequest.

    /// <summary>
    /// Cancel-able event arguments for panel state-transition requests.
    /// Set <see cref="CancelEventArgs.Cancel"/> to <c>true</c> to prevent the transition.
    /// Mirrors Krypton's <c>CancelUniqueNameEventArgs</c>.
    /// </summary>
    public class CancelPanelRequestEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="CancelPanelRequestEventArgs"/> class.
        /// </summary>
        /// <param name="uniqueName">Unique key of the panel being acted upon.</param>
        /// <param name="panel">The panel being acted upon (may be <c>null</c>).</param>
        public CancelPanelRequestEventArgs(string uniqueName, DockPanel? panel = null)
        {
            UniqueName = uniqueName;
            Panel      = panel;
        }

        /// <summary>Gets the unique key of the panel.</summary>
        public string UniqueName { get; }

        /// <summary>Gets the panel (may be <c>null</c> if not yet created).</summary>
        public DockPanel? Panel { get; }
    }

    // ── PanelCloseRequestEventArgs ───────────────────────────────────────────────
    // Mirrors Krypton.Docking.CloseRequestEventArgs.
    // Used by PageCloseRequest.

    /// <summary>
    /// Provides data for the <see cref="BeepDockingManager.PageCloseRequest"/> event.
    /// Set <see cref="CancelEventArgs.Cancel"/> to <c>true</c> to prevent close.
    /// Set <see cref="CloseRequest"/> to control what happens after close is confirmed.
    /// Mirrors Krypton's <c>CloseRequestEventArgs</c>.
    /// </summary>
    public class PanelCloseRequestEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="PanelCloseRequestEventArgs"/> class.
        /// </summary>
        /// <param name="uniqueName">Unique key of the panel to be closed.</param>
        /// <param name="panel">The panel to be closed (may be <c>null</c>).</param>
        /// <param name="closeRequest">Default close action.</param>
        public PanelCloseRequestEventArgs(string uniqueName,
                                          DockPanel? panel = null,
                                          DockingCloseRequest closeRequest = DockingCloseRequest.HidePanel)
        {
            UniqueName   = uniqueName;
            Panel        = panel;
            CloseRequest = closeRequest;
        }

        /// <summary>Gets the unique key of the panel to be closed.</summary>
        public string UniqueName { get; }

        /// <summary>Gets the panel to be closed (may be <c>null</c>).</summary>
        public DockPanel? Panel { get; }

        /// <summary>
        /// Gets or sets the action to perform when close is confirmed.
        /// Defaults to <see cref="DockingCloseRequest.HidePanel"/>.
        /// </summary>
        public DockingCloseRequest CloseRequest { get; set; }
    }

    // ── PanelContextMenuEventArgs ────────────────────────────────────────────────
    // Mirrors Krypton.Docking.ContextPageEventArgs.
    // Used by ShowPanelContextMenu.

    /// <summary>
    /// Provides data for the <see cref="BeepDockingManager.ShowPanelContextMenu"/> event.
    /// Assign <see cref="ContextMenu"/> to replace the built-in Float / Auto-Hide / Close menu.
    /// Mirrors Krypton's <c>ContextPageEventArgs</c>.
    /// </summary>
    public class PanelContextMenuEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="PanelContextMenuEventArgs"/> class.
        /// </summary>
        /// <param name="panel">The panel for which the menu is being shown.</param>
        /// <param name="screenPosition">Screen coordinates where the menu should appear.</param>
        public PanelContextMenuEventArgs(DockPanel panel, Point screenPosition)
        {
            Panel          = panel;
            ScreenPosition = screenPosition;
        }

        /// <summary>Gets the panel for which the context menu is being shown.</summary>
        public DockPanel Panel { get; }

        /// <summary>Gets the screen position at which the context menu should appear.</summary>
        public Point ScreenPosition { get; }

        /// <summary>
        /// Gets or sets the context menu to display.
        /// Leave <c>null</c> to show the built-in docking caption menu.
        /// </summary>
        public ContextMenuStrip? ContextMenu { get; set; }
    }

    // ── FloatingWindowEventArgs ──────────────────────────────────────────────────
    // Mirrors Krypton.Docking.FloatingWindowEventArgs.
    // Used by FloatingWindowAdding and FloatingWindowRemoved.

    /// <summary>
    /// Provides data for <see cref="BeepDockingManager.FloatingWindowAdding"/> and
    /// <see cref="BeepDockingManager.FloatingWindowRemoved"/> events.
    /// Mirrors Krypton's <c>FloatingWindowEventArgs</c>.
    /// </summary>
    public class FloatingWindowEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="FloatingWindowEventArgs"/> class.
        /// </summary>
        /// <param name="floatWindow">The float window being added or removed.</param>
        /// <param name="panel">The panel being floated (may be <c>null</c> on removal).</param>
        public FloatingWindowEventArgs(FloatWindow floatWindow, DockPanel? panel)
        {
            FloatingWindow = floatWindow;
            Panel          = panel;
        }

        /// <summary>Gets a reference to the <see cref="FloatWindow"/>.</summary>
        public FloatWindow FloatingWindow { get; }

        /// <summary>Gets the panel associated with the floating window.</summary>
        public DockPanel? Panel { get; }
    }

    // ── AutoHiddenGroupEventArgs ─────────────────────────────────────────────────
    // Mirrors Krypton.Docking.AutoHiddenGroupEventArgs.
    // Used by AutoHiddenGroupAdding and AutoHiddenGroupRemoved.

    /// <summary>
    /// Provides data for <see cref="BeepDockingManager.AutoHiddenGroupAdding"/> and
    /// <see cref="BeepDockingManager.AutoHiddenGroupRemoved"/> events.
    /// Mirrors Krypton's <c>AutoHiddenGroupEventArgs</c>.
    /// </summary>
    public class AutoHiddenGroupEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="AutoHiddenGroupEventArgs"/> class.
        /// </summary>
        /// <param name="panel">The panel joining or leaving the auto-hide strip.</param>
        /// <param name="edge">The edge at which the auto-hide strip lives.</param>
        public AutoHiddenGroupEventArgs(DockPanel panel, DockPosition edge)
        {
            Panel = panel;
            Edge  = edge;
        }

        /// <summary>Gets the panel joining or leaving the auto-hide strip.</summary>
        public DockPanel Panel { get; }

        /// <summary>Gets the edge at which the auto-hide strip lives.</summary>
        public DockPosition Edge { get; }
    }

    // ── DockspaceEventArgs ───────────────────────────────────────────────────────
    // Mirrors Krypton.Docking.DockspaceEventArgs.
    // Used by DockspaceAdding and DockspaceRemoved.

    /// <summary>
    /// Provides data for <see cref="BeepDockingManager.DockspaceAdding"/> and
    /// <see cref="BeepDockingManager.DockspaceRemoved"/> events.
    /// Mirrors Krypton's <c>DockspaceEventArgs</c>.
    /// </summary>
    public class DockspaceEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="DockspaceEventArgs"/> class.
        /// </summary>
        /// <param name="panel">The panel whose dockspace is being created or destroyed.</param>
        public DockspaceEventArgs(DockPanel panel) => Panel = panel;

        /// <summary>Gets the panel whose dockspace is being created or destroyed.</summary>
        public DockPanel Panel { get; }
    }

    // ── SeparatorResizeEventArgs ─────────────────────────────────────────────────
    // Mirrors Krypton separator resize event args.
    // Used by DockspaceSeparatorResize and AutoHiddenSeparatorResize.

    /// <summary>
    /// Provides data for <see cref="BeepDockingManager.DockspaceSeparatorResize"/> and
    /// <see cref="BeepDockingManager.AutoHiddenSeparatorResize"/> events.
    /// Mirrors Krypton's separator resize event args pattern.
    /// </summary>
    public class SeparatorResizeEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="SeparatorResizeEventArgs"/> class.
        /// </summary>
        /// <param name="panel">The panel adjacent to the separator being resized.</param>
        /// <param name="resizeRect">Rectangle that limits the resize operation.</param>
        public SeparatorResizeEventArgs(DockPanel? panel, Rectangle resizeRect)
        {
            Panel      = panel;
            ResizeRect = resizeRect;
        }

        /// <summary>Gets the panel adjacent to the separator being resized.</summary>
        public DockPanel? Panel { get; }

        /// <summary>Gets or sets the rectangle that limits the resize operation.</summary>
        public Rectangle ResizeRect { get; set; }
    }

    // ── DoDragDropEnd / DoDragDropQuit use plain EventArgs — no class needed ────

}
