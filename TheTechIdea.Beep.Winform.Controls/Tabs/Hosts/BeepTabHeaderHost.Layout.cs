using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    public partial class BeepTabHeaderHost
    {
        private void ApplyItemState()
        {
            if (LayoutSnapshot == null)
            {
                return;
            }

            foreach (Tabs.Models.BeepTabHeaderItemLayout itemLayout in LayoutSnapshot.Items)
            {
                itemLayout.Item.IsHovered = itemLayout.Item.Index == _hoveredTabIndex;
                itemLayout.Item.IsPressed = itemLayout.Item.Index == _pressedTabIndex;
                itemLayout.Item.IsCloseButtonHovered = itemLayout.Item.Index == _hoveredCloseTabIndex && itemLayout.HasCloseButton;
                itemLayout.Item.IsCloseButtonPressed = itemLayout.Item.Index == _pressedCloseTabIndex && itemLayout.HasCloseButton;
                itemLayout.Item.IsDragging = itemLayout.Item.Index == _draggingTabIndex;
            }
        }

        public void SyncSnapshot()
        {
            LayoutSnapshot = TabsOwner?.CreateRuntimeLayoutSnapshot() ?? new Tabs.Models.BeepTabHeaderLayoutSnapshot();
            LayoutActionSlots();
            ApplyItemState();
        }

        public void RefreshSnapshot()
        {
            SyncSnapshot();
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RefreshSnapshot();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RefreshSnapshot();
        }
    }
}