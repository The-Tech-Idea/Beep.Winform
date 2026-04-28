namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal static class ComboBoxPopupScrollHelper
    {
        internal static int ComputeAdjustedScrollOffset(int currentOffset, int itemTop, int itemHeight, int viewportHeight, int maxOffset)
        {
            if (viewportHeight <= 0 || itemHeight <= 0)
            {
                return currentOffset;
            }

            int itemBottom = itemTop + itemHeight;
            int next = currentOffset;

            if (itemTop < currentOffset)
            {
                next = itemTop;
            }
            else if (itemBottom > currentOffset + viewportHeight)
            {
                next = itemBottom - viewportHeight;
            }

            if (next < 0)
            {
                return 0;
            }

            return next > maxOffset ? maxOffset : next;
        }
    }
}
