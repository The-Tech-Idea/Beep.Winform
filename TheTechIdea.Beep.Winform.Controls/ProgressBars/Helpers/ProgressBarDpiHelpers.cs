using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers
{
    internal static class ProgressBarDpiHelpers
    {
        public static int Scale(BeepProgressBar owner, int value)
        {
            if (owner == null)
            {
                return value;
            }

            return Math.Max(1, DpiScalingHelper.ScaleValue(value, owner));
        }

        public static float Scale(BeepProgressBar owner, float value)
        {
            if (owner == null)
            {
                return value;
            }

            return DpiScalingHelper.ScaleValue(value, owner);
        }

        public static Size Scale(BeepProgressBar owner, Size value)
        {
            if (owner == null)
            {
                return value;
            }

            return DpiScalingHelper.ScaleSize(value, owner);
        }
    }
}
