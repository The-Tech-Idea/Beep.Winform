using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime.DragDrop
{
    /// <summary>
    /// Mutable state for one in-flight caption/tab drag. Created on mouse-down (as a candidate),
    /// promoted to <see cref="Started"/> once the drag threshold is crossed, and discarded on
    /// commit/cancel. Records enough origin state to fully restore the layout on cancel.
    /// </summary>
    internal sealed class DockDragSession
    {
        public DockDragSession(DockPanel panel, Point originScreen)
        {
            Panel = panel;
            OriginScreen = originScreen;
            OriginGroup = panel?.Group;
            OriginState = panel?.State ?? DockPanelState.Docked;
            OriginPosition = panel?.DockPosition ?? DockPosition.Left;
        }

        /// <summary>The panel being dragged.</summary>
        public DockPanel Panel { get; }

        /// <summary>Cursor position (screen) when the candidate drag began.</summary>
        public Point OriginScreen { get; }

        /// <summary>Group the panel belonged to at drag start (for cancel/restore).</summary>
        public DockGroup OriginGroup { get; }

        /// <summary>Panel state at drag start (for cancel/restore).</summary>
        public DockPanelState OriginState { get; }

        /// <summary>Dock position at drag start (for cancel/restore).</summary>
        public DockPosition OriginPosition { get; }

        /// <summary>True once movement exceeded the drag threshold and the drag really started.</summary>
        public bool Started { get; set; }

        /// <summary>Latest resolved drop target (updated on each move).</summary>
        public DockDropResult Current { get; set; }

        /// <summary>
        /// Cursor position (screen) the latest <see cref="Current"/> was resolved at.
        /// </summary>
        /// <remarks>
        /// Kept because the drop target alone cannot say <i>where within</i> a group the cursor was.
        /// Deciding which two tabs a drop fell between needs the point, and reading
        /// <c>Cursor.Position</c> at commit time would answer a slightly different question - where
        /// the mouse is now, rather than where the drop was resolved.
        /// </remarks>
        public Point CurrentScreen { get; set; }
    }
}
