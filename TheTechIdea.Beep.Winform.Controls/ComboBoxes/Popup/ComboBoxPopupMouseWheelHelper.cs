namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal static class ComboBoxPopupMouseWheelHelper
    {
        internal static int ComputeNextOffsetFromWheel(int currentOffset, int wheelDelta, int maxOffset, int stepDivisor = 4)
        {
            if (maxOffset <= 0)
            {
                return 0;
            }

            int divisor = stepDivisor <= 0 ? 4 : stepDivisor;
            int delta = -wheelDelta / divisor;
            int next = currentOffset + delta;
            if (next < 0)
            {
                return 0;
            }

            return next > maxOffset ? maxOffset : next;
        }
    }
}
