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
    }
}
