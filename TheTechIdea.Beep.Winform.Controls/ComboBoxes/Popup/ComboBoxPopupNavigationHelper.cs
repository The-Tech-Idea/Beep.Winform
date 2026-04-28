namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal static class ComboBoxPopupNavigationHelper
    {
        internal static int ClampIndex(int index, int count)
        {
            if (count <= 0)
            {
                return -1;
            }

            if (index < 0)
            {
                return 0;
            }

            return index >= count ? count - 1 : index;
        }

        internal static int ShiftIndex(int currentIndex, int delta, int count)
        {
            if (count <= 0)
            {
                return -1;
            }

            int start = currentIndex >= 0 ? currentIndex : 0;
            return ClampIndex(start + delta, count);
        }
    }
}
