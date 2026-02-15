using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Helpers
{
    /// <summary>
    /// Helper class for notification style properties and recommendations
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class NotificationStyleHelpers
    {
        /// <summary>
        /// Gets recommended border radius for notifications based on ControlStyle
        /// </summary>
        public static int GetRecommendedBorderRadius(BeepControlStyle controlStyle)
        {
            return BeepStyling.GetRadius(controlStyle);
        }

        /// <summary>
        /// Gets recommended padding for notification layout
        /// </summary>
        public static int GetRecommendedPadding(NotificationLayout layout, Control ownerControl = null)
        {
            int baseValue = layout switch
            {
                NotificationLayout.Compact => 8,
                NotificationLayout.Toast => 10,
                NotificationLayout.Banner => 6,
                _ => 12 // Standard, Prominent
            };
            return ownerControl != null ? DpiScalingHelper.ScaleValue(baseValue, ownerControl) : baseValue;
        }

        /// <summary>
        /// Gets recommended icon size for notification layout
        /// </summary>
        public static int GetRecommendedIconSize(NotificationLayout layout, Control ownerControl = null)
        {
            int baseValue = layout switch
            {
                NotificationLayout.Compact => 16,
                NotificationLayout.Toast => 20,
                NotificationLayout.Prominent => 32,
                NotificationLayout.Banner => 24,
                _ => 24 // Standard
            };
            return ownerControl != null ? DpiScalingHelper.ScaleValue(baseValue, ownerControl) : baseValue;
        }

        /// <summary>
        /// Gets recommended minimum width for notification
        /// </summary>
        public static int GetRecommendedMinWidth(NotificationLayout layout, Control ownerControl = null)
        {
            int baseValue = layout switch
            {
                NotificationLayout.Compact => 200,
                NotificationLayout.Toast => 250,
                NotificationLayout.Banner => 400,
                _ => 280 // Standard, Prominent
            };
            return ownerControl != null ? DpiScalingHelper.ScaleValue(baseValue, ownerControl) : baseValue;
        }

        /// <summary>
        /// Gets recommended maximum width for notification
        /// </summary>
        public static int GetRecommendedMaxWidth(NotificationLayout layout, Control ownerControl = null)
        {
            int baseValue = layout switch
            {
                NotificationLayout.Compact => 320,
                NotificationLayout.Toast => 380,
                NotificationLayout.Banner => 600,
                _ => 420 // Standard, Prominent
            };
            return ownerControl != null ? DpiScalingHelper.ScaleValue(baseValue, ownerControl) : baseValue;
        }

        /// <summary>
        /// Gets recommended duration based on priority
        /// </summary>
        public static int GetRecommendedDuration(NotificationPriority priority)
        {
            return priority switch
            {
                NotificationPriority.Low => 3000,
                NotificationPriority.Normal => 5000,
                NotificationPriority.High => 8000,
                NotificationPriority.Critical => 0, // No auto-dismiss
                _ => 5000
            };
        }

        /// <summary>
        /// Gets recommended spacing between notification elements
        /// </summary>
        public static int GetRecommendedSpacing(NotificationLayout layout, Control ownerControl = null)
        {
            int baseValue = layout switch
            {
                NotificationLayout.Compact => 4,
                NotificationLayout.Toast => 6,
                _ => 8 // Standard, Prominent, Banner
            };
            return ownerControl != null ? DpiScalingHelper.ScaleValue(baseValue, ownerControl) : baseValue;
        }
    }
}
