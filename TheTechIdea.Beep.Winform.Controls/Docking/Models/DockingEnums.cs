namespace TheTechIdea.Beep.Winform.Controls.Docking.Models
{
    /// <summary>
    /// Flags that control which dock positions and states are permitted for a panel.
    /// Mirrors the DockAreas concept in DockPanelSuite and Krypton.
    /// </summary>
    [Flags]
    public enum DockAreas
    {
        /// <summary>No docking allowed.</summary>
        None = 0,

        /// <summary>Can be docked to the left edge.</summary>
        Left = 1 << 0,

        /// <summary>Can be docked to the right edge.</summary>
        Right = 1 << 1,

        /// <summary>Can be docked to the top edge.</summary>
        Top = 1 << 2,

        /// <summary>Can be docked to the bottom edge.</summary>
        Bottom = 1 << 3,

        /// <summary>Can fill the document/center area.</summary>
        Fill = 1 << 4,

        /// <summary>Can float in a separate window.</summary>
        Float = 1 << 5,

        /// <summary>Can be auto-hidden to an edge tab strip.</summary>
        AutoHide = 1 << 6,

        /// <summary>All dock areas permitted (default).</summary>
        All = Left | Right | Top | Bottom | Fill | Float | AutoHide,

        /// <summary>All docked edge positions (no float, no autohide, no fill).</summary>
        Docked = Left | Right | Top | Bottom,
    }


    /// <summary>
    /// Defines the six possible docking positions where a panel can be anchored.
    /// </summary>
    public enum DockPosition
    {
        /// <summary>Docked on the left edge of the host.</summary>
        Left = 0,

        /// <summary>Docked on the right edge of the host.</summary>
        Right = 1,

        /// <summary>Docked on the top edge of the host.</summary>
        Top = 2,

        /// <summary>Docked on the bottom edge of the host.</summary>
        Bottom = 3,

        /// <summary>Fills the entire remaining space (document area).</summary>
        Fill = 4,

        /// <summary>Floating (not docked to any edge).</summary>
        Floating = 5
    }

    /// <summary>
    /// Defines the possible states of a docking panel.
    /// </summary>
    public enum DockPanelState
    {
        /// <summary>Panel is docked to the host and visible.</summary>
        Docked = 0,

        /// <summary>Panel is floating in a separate window.</summary>
        Floating = 1,

        /// <summary>Panel is auto-hidden (collapsed to a tab on the edge).</summary>
        AutoHidden = 2,

        /// <summary>Panel is closed but can be reopened (persisted in layout).</summary>
        Closed = 3
    }

    /// <summary>
    /// Defines how split groups are oriented within a docking container.
    /// </summary>
    public enum SplitOrientation
    {
        /// <summary>Groups are split horizontally (left-right).</summary>
        Horizontal = 0,

        /// <summary>Groups are split vertically (top-bottom).</summary>
        Vertical = 1
    }

    /// <summary>
    /// Defines the tab display style for grouped panels.
    /// </summary>
    public enum TabStyle
    {
        /// <summary>Tabs displayed at the top of the group.</summary>
        Top = 0,

        /// <summary>Tabs displayed at the bottom of the group.</summary>
        Bottom = 1,

        /// <summary>Tabs displayed on the left side of the group.</summary>
        Left = 2,

        /// <summary>Tabs displayed on the right side of the group.</summary>
        Right = 3
    }

    /// <summary>
    /// Defines how a <see cref="TheTechIdea.Beep.Winform.Controls.Docking.BeepDockspace"/>
    /// draws its owned header/tab strip.
    /// </summary>
    public enum DockspaceHeaderStyle
    {
        /// <summary>Flat rectangular tabs, close to the default Visual Studio tool window strip.</summary>
        Flat = 0,

        /// <summary>Document-style tabs with a stronger active page surface.</summary>
        Document = 1,

        /// <summary>Lightweight tabs with an active underline indicator.</summary>
        Underline = 2,

        /// <summary>Separated button-like tabs with small spacing between pages.</summary>
        Buttons = 3
    }

    /// <summary>
    /// Specifies the current docking location of a panel.
    /// Mirrors Krypton's <c>DockingLocation</c> enum.
    /// </summary>
    public enum DockingLocation
    {
        /// <summary>Panel is auto-hidden against a control edge.</summary>
        AutoHidden,

        /// <summary>Panel is docked against a control edge.</summary>
        Docked,

        /// <summary>Panel is inside a floating window.</summary>
        Floating,

        /// <summary>Panel is inside a standalone workspace / fill area.</summary>
        Workspace,

        /// <summary>Panel is not inside the docking hierarchy.</summary>
        None
    }

    /// <summary>
    /// Specifies the action taken when a docking close is requested.
    /// Mirrors Krypton's <c>DockingCloseRequest</c> enum.
    /// </summary>
    public enum DockingCloseRequest
    {
        /// <summary>No action.</summary>
        None,

        /// <summary>Remove the panel from the docking hierarchy.</summary>
        RemovePanel,

        /// <summary>Remove the panel from the hierarchy and dispose it.</summary>
        RemovePanelAndDispose,

        /// <summary>Hide the panel (keep it in the hierarchy but invisible).</summary>
        HidePanel
    }

    /// <summary>
    /// Specifies the sliding state of an auto-hidden panel.
    /// Mirrors Krypton's <c>DockingAutoHiddenShowState</c> enum.
    /// </summary>
    public enum DockingAutoHiddenShowState
    {
        /// <summary>Auto-hidden panel is fully hidden.</summary>
        Hidden,

        /// <summary>Auto-hidden panel is sliding out into view.</summary>
        SlidingOut,

        /// <summary>Auto-hidden panel is sliding back to hidden.</summary>
        SlidingIn,

        /// <summary>Auto-hidden panel is fully showing.</summary>
        Showing
    }

    /// <summary>
    /// Specifies a propagated action sent down the docking hierarchy.
    /// Mirrors Krypton's <c>DockingPropogateAction</c> enum.
    /// </summary>
    public enum DockingPropogateAction
    {
        /// <summary>Null / no-op.</summary>
        Null,

        /// <summary>A multi-part update is starting — suspend recalculation.</summary>
        StartUpdate,

        /// <summary>A multi-part update has ended — resume recalculation.</summary>
        EndUpdate,

        /// <summary>Show all display elements of the named panels.</summary>
        ShowPages,

        /// <summary>Show all display elements of all panels.</summary>
        ShowAllPages,

        /// <summary>Hide all display elements of the named panels.</summary>
        HidePages,

        /// <summary>Hide all display elements of all panels.</summary>
        HideAllPages,

        /// <summary>Replace named panels with position placeholders.</summary>
        StorePages,

        /// <summary>Replace all panels with position placeholders.</summary>
        StoreAllPages,

        /// <summary>Restore position placeholders with actual panels.</summary>
        RestorePages,

        /// <summary>Remove auto-hidden store pages for the named panels.</summary>
        ClearAutoHiddenStoredPages,

        /// <summary>Remove docked store pages for the named panels.</summary>
        ClearDockedStoredPages,

        /// <summary>Remove floating store pages for the named panels.</summary>
        ClearFloatingStoredPages,

        /// <summary>Remove all stored pages for the named panels.</summary>
        ClearStoredPages,

        /// <summary>Remove all stored pages across all panels.</summary>
        ClearAllStoredPages,

        /// <summary>Remove all details of the named panels.</summary>
        RemovePages,

        /// <summary>Remove all details of the named panels and dispose them.</summary>
        RemoveAndDisposePages,

        /// <summary>Remove all details of all panels.</summary>
        RemoveAllPages,

        /// <summary>Remove all details of all panels and dispose them.</summary>
        RemoveAndDisposeAllPages,

        /// <summary>A loading operation is about to begin.</summary>
        Loading,

        /// <summary>Named string property has been updated.</summary>
        StringChanged
    }
}
