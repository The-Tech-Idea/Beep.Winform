using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
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
            catch
            {
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

        #region High Contrast Support

        /// <summary>
        /// Gets high contrast colors for breadcrumb items
        /// Returns (textColor, hoverBackColor, separatorColor, borderColor)
        /// </summary>
        public static (Color textColor, Color hoverBackColor, Color separatorColor, Color borderColor) GetHighContrastColors()
        {
            if (!IsHighContrastMode())
            {
                // Return default colors if high contrast is not enabled
                return (Color.Black, Color.LightGray, Color.Gray, Color.Black);
            }

            // Use system colors for high contrast mode
            return (
                SystemColors.WindowText,
                SystemColors.Highlight,
                SystemColors.WindowFrame,
                SystemColors.WindowFrame
            );
        }

        /// <summary>
        /// Adjusts colors for high contrast mode
        /// </summary>
        public static (Color textColor, Color hoverBackColor, Color separatorColor, Color borderColor) AdjustColorsForHighContrast(
            Color textColor,
            Color hoverBackColor,
            Color separatorColor,
            Color borderColor)
        {
            if (!IsHighContrastMode())
            {
                return (textColor, hoverBackColor, separatorColor, borderColor);
            }

            var (hcTextColor, hcHoverBackColor, hcSeparatorColor, hcBorderColor) = GetHighContrastColors();
            return (hcTextColor, hcHoverBackColor, hcSeparatorColor, hcBorderColor);
        }

        /// <summary>
        /// Applies high contrast adjustments to the breadcrumb control
        /// </summary>
        public static void ApplyHighContrastAdjustments(
            Control control,
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (control == null || !IsHighContrastMode()) return;

            // Apply system colors
            if (control is BaseControl baseControl)
            {
                // Use system colors for high contrast
                baseControl.BackColor = SystemColors.Window;
                baseControl.ForeColor = SystemColors.WindowText;
            }
        }

        #endregion

        #region WCAG Compliance

        /// <summary>
        /// Calculates the contrast ratio between two colors (WCAG formula)
        /// Returns a value between 1 (no contrast) and 21 (maximum contrast)
        /// WCAG AA requires 4.5:1 for normal text, 3:1 for large text
        /// </summary>
        public static double CalculateContrastRatio(Color color1, Color color2)
        {
            double l1 = GetRelativeLuminance(color1);
            double l2 = GetRelativeLuminance(color2);

            double lighter = Math.Max(l1, l2);
            double darker = Math.Min(l1, l2);

            return (lighter + 0.05) / (darker + 0.05);
        }

        /// <summary>
        /// Calculates the relative luminance of a color (WCAG formula)
        /// Returns a value between 0 (black) and 1 (white)
        /// </summary>
        public static double GetRelativeLuminance(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            // Convert to linear RGB
            r = r <= 0.03928 ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
            g = g <= 0.03928 ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
            b = b <= 0.03928 ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);

            // Calculate relative luminance
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        /// <summary>
        /// Ensures a color meets the minimum contrast ratio requirement
        /// </summary>
        public static bool EnsureContrastRatio(Color foreground, Color background, double minimumRatio = 4.5)
        {
            double ratio = CalculateContrastRatio(foreground, background);
            return ratio >= minimumRatio;
        }

        /// <summary>
        /// Adjusts a color to meet the minimum contrast ratio requirement
        /// </summary>
        public static Color AdjustForContrast(Color foreground, Color background, double minimumRatio = 4.5)
        {
            double ratio = CalculateContrastRatio(foreground, background);
            if (ratio >= minimumRatio)
            {
                return foreground;
            }

            // Calculate target luminance
            double bgLuminance = GetRelativeLuminance(background);
            double targetLuminance = bgLuminance > 0.5
                ? (bgLuminance + 0.05) / minimumRatio - 0.05  // Darker foreground
                : (bgLuminance + 0.05) * minimumRatio - 0.05;  // Lighter foreground

            // Clamp target luminance
            targetLuminance = Math.Max(0, Math.Min(1, targetLuminance));

            // Convert back to RGB
            int grayValue = bgLuminance > 0.5
                ? (int)(targetLuminance * 255)
                : (int)(targetLuminance * 255);

            // Ensure we have enough contrast
            if (bgLuminance > 0.5)
            {
                // Background is light, use darker foreground
                grayValue = Math.Max(0, Math.Min(255, grayValue));
                return Color.FromArgb(grayValue, grayValue, grayValue);
            }
            else
            {
                // Background is dark, use lighter foreground
                grayValue = Math.Max(0, Math.Min(255, grayValue));
                return Color.FromArgb(grayValue, grayValue, grayValue);
            }
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
