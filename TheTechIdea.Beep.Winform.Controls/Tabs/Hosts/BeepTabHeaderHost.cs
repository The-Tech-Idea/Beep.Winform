using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    [ToolboxItem(false)]
    [Category("Beep Controls")]
    [DisplayName("Beep Tab Header Host")]
    [Description("Custom premium tab header host that replaces stock header rendering over time.")]
    public partial class BeepTabHeaderHost : BaseControl
    {
        private int _hoveredTabIndex = -1;
        private int _hoveredCloseTabIndex = -1;
        private int _pressedTabIndex = -1;
        private int _pressedCloseTabIndex = -1;
        private int _draggingTabIndex = -1;
        private bool _hasActivePointerInteraction;
        private Point _pointerDownLocation = Point.Empty;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTabs? TabsOwner { get; private set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTabHeaderLayoutSnapshot LayoutSnapshot { get; private set; } = new BeepTabHeaderLayoutSnapshot();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int HoveredTabIndex => _hoveredTabIndex;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PressedTabIndex => _pressedTabIndex;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int DraggingTabIndex => _draggingTabIndex;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTabHeaderDragFeedback DragFeedback { get; private set; } = BeepTabHeaderDragFeedback.Empty;

        public BeepTabHeaderHost()
        {
            AccessibleRole = AccessibleRole.PageTabList;
            AccessibleName = "Beep Tab Header Host";
            TabStop = true;
        }

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