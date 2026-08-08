using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers
{
    /// <summary>
    /// Helper class for accessibility features in BeepBreadcrump control
    /// Provides methods for ARIA attributes, high contrast mode, reduced motion, and WCAG compliance
    /// </summary>
    public static class BreadcrumbAccessibilityHelpers
    {
        #region System Detection

        /// <summary>
        /// Checks if Windows high contrast mode is enabled
        /// </summary>
        public static bool IsHighContrastMode()
        {
            return SystemInformation.HighContrast;
        }

        /// <summary>
        /// Checks if reduced motion is enabled in Windows accessibility settings
        /// </summary>
        public static bool IsReducedMotionEnabled()
        {
            try
            {
                int animation = 0;
                SystemParametersInfo(SPI_GETANIMATION, 0, ref animation, 0);
                return animation == 0;
            }
            catch (Exception ex)
            {
                // SystemParametersInfo failed - animations stay on rather than silently off.
                BeepLog.WarnOnce("Breadcrumb.reducedMotion", null, "query reduced-motion setting", ex.Message);
                return false;
            }
        }

        #endregion

        #region ARIA Attributes

        /// <summary>
        /// Applies accessibility settings to the breadcrumb control
        /// Sets AccessibleName, AccessibleDescription, and AccessibleRole
        /// </summary>
        public static void ApplyAccessibilitySettings(
            Control control,
            string accessibleName = null,
            string accessibleDescription = null)
        {
            if (control == null) return;

            // Set default accessible name if not provided
            if (string.IsNullOrEmpty(accessibleName))
            {
                accessibleName = "Breadcrumb navigation";
            }
            control.AccessibleName = accessibleName;

            // Set default accessible description if not provided
            if (string.IsNullOrEmpty(accessibleDescription))
            {
                accessibleDescription = "Navigate through the breadcrumb trail";
            }
            control.AccessibleDescription = accessibleDescription;

            // Set accessible role
            control.AccessibleRole = AccessibleRole.MenuBar;
        }

        /// <summary>
        /// Generates an accessible name for a breadcrumb item
        /// </summary>
        public static string GenerateItemAccessibleName(
            Models.SimpleItem item,
            int index,
            int totalItems,
            bool isLast)
        {
            if (item == null) return string.Empty;

            string itemText = item.Text ?? item.Name ?? "Item";
            string position = $"Breadcrumb {index + 1} of {totalItems}";
            string state = isLast ? " (Current page)" : "";

            return $"{itemText} ({position}){state}";
        }

        /// <summary>
        /// Generates an accessible description for a breadcrumb item
        /// </summary>
        public static string GenerateItemAccessibleDescription(
            Models.SimpleItem item,
            int index,
            int totalItems,
            bool isLast)
        {
            if (item == null) return string.Empty;

            string description = $"Navigate to {item.Text ?? item.Name ?? "item"}";
            if (isLast)
            {
                description += ". This is the current page.";
            }
            else
            {
                description += $". Position {index + 1} of {totalItems} in the breadcrumb trail.";
            }

            return description;
        }

        /// <summary>
        /// Gets the accessible state description for a breadcrumb item
        /// </summary>
        public static string GetAccessibleStateDescription(bool isHovered, bool isSelected, bool isLast)
        {
            if (isLast)
            {
                return "Current page";
            }
            if (isSelected)
            {
                return "Selected";
            }
            if (isHovered)
            {
                return "Hovered";
            }
            return "Available";
        }

        #endregion

        #region Reduced Motion

        /// <summary>
        /// Checks if animations should be disabled based on user preferences
        /// </summary>
        public static bool ShouldDisableAnimations()
        {
            return IsReducedMotionEnabled();
        }

        #endregion

        #region Accessible Sizing

        /// <summary>
        /// Gets the minimum accessible font size (12pt minimum)
        /// </summary>
        public static float GetAccessibleFontSize(float currentSize)
        {
            return Math.Max(12f, currentSize);
        }

        /// <summary>
        /// Gets the minimum accessible size for breadcrumb items (44x44px minimum for touch targets)
        /// </summary>
        public static Size GetAccessibleMinimumSize(Size currentSize)
        {
            return new Size(
                Math.Max(44, currentSize.Width),
                Math.Max(44, currentSize.Height)
            );
        }

        /// <summary>
        /// Gets accessible padding for breadcrumb items
        /// </summary>
        public static Padding GetAccessiblePadding(Padding currentPadding)
        {
            if (IsHighContrastMode())
            {
                // Increase padding in high contrast mode
                return new Padding(
                    Math.Max(8, currentPadding.Left),
                    Math.Max(8, currentPadding.Top),
                    Math.Max(8, currentPadding.Right),
                    Math.Max(8, currentPadding.Bottom)
                );
            }
            return currentPadding;
        }

        /// <summary>
        /// Gets accessible item spacing (increases spacing for better accessibility)
        /// </summary>
        public static int GetAccessibleItemSpacing(int currentSpacing)
        {
            if (IsHighContrastMode())
            {
                // Increase spacing in high contrast mode
                return Math.Max(12, currentSpacing);
            }
            return currentSpacing;
        }

        /// <summary>
        /// Gets accessible border width (thicker borders in high contrast mode)
        /// </summary>
        public static int GetAccessibleBorderWidth(int currentWidth)
        {
            if (IsHighContrastMode())
            {
                // Minimum 2px border in high contrast mode
                return Math.Max(2, currentWidth);
            }
            return currentWidth;
        }

        #endregion

        #region Windows API

        private const int SPI_GETANIMATION = 0x0048;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(
            uint uiAction,
            uint uiParam,
            ref int pvParam,
            uint fWinIni);
        #endregion
    }
}
