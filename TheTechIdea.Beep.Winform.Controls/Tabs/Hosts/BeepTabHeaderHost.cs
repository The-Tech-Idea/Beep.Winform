using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    /// <summary>
    /// Pure helper class that owns header layout, hit-testing, and rendering state.
    /// It is NOT a Control and is never added to the window hierarchy.
    /// </summary>
    public partial class BeepTabHeaderHost
    {
        private int _hoveredTabIndex = -1;
        private int _hoveredCloseTabIndex = -1;
        private int _pressedTabIndex = -1;
        private int _pressedCloseTabIndex = -1;
        private int _draggingTabIndex = -1;
        private bool _hasActivePointerInteraction;
        private Point _pointerDownLocation = Point.Empty;

        public BeepTabs? TabsOwner { get; private set; }

        public BeepTabHeaderLayoutSnapshot LayoutSnapshot { get; private set; } = new BeepTabHeaderLayoutSnapshot();

        public int HoveredTabIndex => _hoveredTabIndex;

        public int PressedTabIndex => _pressedTabIndex;

        public int DraggingTabIndex => _draggingTabIndex;

        public BeepTabHeaderDragFeedback DragFeedback { get; private set; } = BeepTabHeaderDragFeedback.Empty;

        /// <summary>Delegates to the owner control's font, or the default UI font.</summary>
        public Font? Font => TabsOwner?.Font;

        /// <summary>Delegates to the owner control's foreground colour.</summary>
        public Color ForeColor => TabsOwner?.ForeColor ?? SystemColors.ControlText;

        public void AttachTabsOwner(BeepTabs tabsOwner)
        {
            TabsOwner = tabsOwner;
            RefreshSnapshot();
        }

        public void DetachTabsOwner()
        {
            TabsOwner = null;
            LayoutSnapshot = new BeepTabHeaderLayoutSnapshot();
        }
    }
}