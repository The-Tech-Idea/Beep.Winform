using System;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    /// <summary>
    /// Specifies the reason why a BeepContextMenu was closed.
    /// Mirrors the behavior of ToolStripDropDownCloseReason from WinForms.
    /// </summary>
    public enum BeepContextMenuCloseReason
    {
        /// <summary>
        /// The menu closed due to application focus change
        /// </summary>
        AppFocusChange = 0,

        /// <summary>
        /// The menu closed because the user clicked outside of it
        /// </summary>
        AppClicked = 1,

        /// <summary>
        /// The menu closed because a menu item was clicked
        /// </summary>
        ItemClicked = 2,

        /// <summary>
        /// The menu closed due to keyboard action (ESC, Alt, etc.)
        /// </summary>
        Keyboard = 3,

        /// <summary>
        /// The menu closed because Close() was called programmatically
        /// </summary>
        CloseCalled = 4
    }
}
