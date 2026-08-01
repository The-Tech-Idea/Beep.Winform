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

            // Mirror for right-to-left before anything consumes the snapshot. BeepTabRtlLayoutHelper
            // was complete and correct but referenced only by itself, so RightToLeft did nothing
            // measurable on this control. Mirroring the snapshot is enough on its own: painting and
            // hit-testing both read these bounds, so the mirrored rectangles are simultaneously
            // where the tabs are drawn and where clicks land. (That is why the helper's FlipPoint
            // is not used here — flipping the pointer as well would mirror twice and cancel out.)
            int width = TabsOwner?.Width ?? 0;
            if (Helpers.BeepTabRtlLayoutHelper.ShouldMirror(RightToLeftForLayout, width))
            {
                Helpers.BeepTabRtlLayoutHelper.MirrorSnapshot(LayoutSnapshot, width);
            }

            LayoutActionSlots();
            ApplyItemState();
        }

        /// <summary>
        /// The RTL setting that governs layout. Resolved from the owning <see cref="BeepTabs"/>
        /// rather than this host, because <see cref="RightToLeft.Inherit"/> on the host would
        /// otherwise report the framework default instead of what the application set.
        /// </summary>
        private System.Windows.Forms.RightToLeft RightToLeftForLayout =>
            TabsOwner?.RightToLeft == System.Windows.Forms.RightToLeft.Inherit
                ? (TabsOwner?.Parent?.RightToLeft ?? System.Windows.Forms.RightToLeft.No)
                : (TabsOwner?.RightToLeft ?? System.Windows.Forms.RightToLeft.No);

        public void RefreshSnapshot()
        {
            SyncSnapshot();
        }
    }
}