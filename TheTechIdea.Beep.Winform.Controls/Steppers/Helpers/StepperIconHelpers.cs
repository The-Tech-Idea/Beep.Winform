using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Helpers
{
    /// <summary>
    /// Centralized icon management for Stepper controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// </summary>
    public static class StepperIconHelpers
    {
        #region Icon Path Resolution

        /// <summary>
        /// Get recommended check icon path for completed steps
        /// </summary>
        public static string GetCheckIconPath()
        {
            // Try common check icon paths
            // Note: SvgsUI, SvgsDatasources, Svgs may not have direct properties
            // Using string literals for common icon names
            string[] checkPaths = {
                "check.svg",
                "checkmark.svg",
                "check-circle.svg",
                "done.svg"
            };

            foreach (var path in checkPaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "check.svg"; // Default fallback
        }

        /// <summary>
        /// Get recommended icon path for a step based on item and state
        /// </summary>
        public static string GetStepIconPath(SimpleItem item, StepState state, StepDisplayMode displayMode)
        {
            // Priority 1: Item's ImagePath or IconPath
            if (item != null)
            {
                if (!string.IsNullOrEmpty(item.ImagePath))
                    return item.ImagePath;
                
                if (!string.IsNullOrEmpty(item.ImagePath))
                    return item.ImagePath;
            }

            // Priority 2: State-based default icons
            if (displayMode == StepDisplayMode.SvgIcon || displayMode == StepDisplayMode.CheckImage)
            {
                return state switch
                {
                    StepState.Completed => GetCheckIconPath(),
                    StepState.Error => GetErrorIconPath(),
                    StepState.Warning => GetWarningIconPath(),
                    StepState.Active => GetActiveIconPath(),
                    _ => GetPendingIconPath()
                };
            }

            // Priority 3: Default check icon
            return GetCheckIconPath();
        }

        /// <summary>
        /// Get error icon path
        /// </summary>
        public static string GetErrorIconPath()
        {
            string[] errorPaths = {
                "error.svg",
                "close.svg",
                "x.svg",
                "cancel.svg"
            };

            foreach (var path in errorPaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "error.svg";
        }

        /// <summary>
        /// Get warning icon path
        /// </summary>
        public static string GetWarningIconPath()
        {
            string[] warningPaths = {
                "warning.svg",
                "alert.svg",
                "exclamation.svg"
            };

            foreach (var path in warningPaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "warning.svg";
        }

        /// <summary>
        /// Get active icon path
        /// </summary>
        public static string GetActiveIconPath()
        {
            string[] activePaths = {
                "circle.svg",
                "dot.svg",
                "radio-button.svg"
            };

            foreach (var path in activePaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "circle.svg";
        }

        /// <summary>
        /// Get pending icon path
        /// </summary>
        public static string GetPendingIconPath()
        {
            string[] pendingPaths = {
                "circle-outline.svg",
                "circle-empty.svg"
            };

            foreach (var path in pendingPaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "circle-outline.svg";
        }

        /// <summary>
        /// Resolve icon path from multiple sources
        /// </summary>
        public static string ResolveIconPath(string iconPath, StepDisplayMode displayMode, StepState state, SimpleItem item = null)
        {
            // If explicit path provided, use it
            if (!string.IsNullOrEmpty(iconPath))
                return iconPath;

            // Try to get from item
            if (item != null)
            {
                if (!string.IsNullOrEmpty(item.ImagePath))
                    return item.ImagePath;
               
            }

            // Get based on state and display mode
            return GetStepIconPath(item, state, displayMode);
        }

        /// <summary>
        /// Get recommended icon for a step state and display mode
        /// </summary>
        public static string GetRecommendedIcon(StepState state, StepDisplayMode displayMode)
        {
            if (displayMode == StepDisplayMode.CheckImage || displayMode == StepDisplayMode.SvgIcon)
            {
                return state switch
                {
                    StepState.Completed => GetCheckIconPath(),
                    StepState.Error => GetErrorIconPath(),
                    StepState.Warning => GetWarningIconPath(),
                    StepState.Active => GetActiveIconPath(),
                    _ => GetPendingIconPath()
                };
            }

            return null; // No icon for StepNumber mode
        }

        #endregion

        #region Icon Color Management

        /// <summary>
        /// Get icon color based on theme and step state
        /// </summary>
        public static Color GetIconColor(IBeepTheme theme, bool useThemeColors, StepState state, Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                // Use text color from theme helpers for consistency
                return StepperThemeHelpers.GetStepTextColor(theme, useThemeColors, state);
            }

            // Default: white for completed/active, gray for pending
            return state == StepState.Completed || state == StepState.Active
                ? Color.White
                : Color.FromArgb(156, 163, 175); // Gray
        }

        #endregion

        #region Icon Size Management

        /// <summary>
        /// Get icon size based on button size and step state
        /// </summary>
        public static Size GetIconSize(Size buttonSize, StepState state)
        {
            // Icon should be 60-70% of button size
            int baseSize = Math.Min(buttonSize.Width, buttonSize.Height);
            int iconSize = (int)(baseSize * 0.65f);

            // Active steps can have slightly larger icons
            if (state == StepState.Active)
            {
                iconSize = (int)(iconSize * 1.1f);
            }

            // Clamp between 12px and 32px
            iconSize = Math.Max(12, Math.Min(32, iconSize));

            return new Size(iconSize, iconSize);
        }

        /// <summary>
        /// Calculate icon bounds within step rectangle
        /// </summary>
        public static Rectangle CalculateIconBounds(Rectangle stepRect, Size iconSize)
        {
            int x = stepRect.Left + (stepRect.Width - iconSize.Width) / 2;
            int y = stepRect.Top + (stepRect.Height - iconSize.Height) / 2;
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }

        #endregion

        #region Icon Painting

        /// <summary>
        /// Paint icon using StyledImagePainter
        /// </summary>
        public static void PaintIcon(
            Graphics g,
            Rectangle bounds,
            string iconPath,
            Color tint,
            float opacity = 1f,
            StepState state = StepState.Pending)
        {
            if (string.IsNullOrEmpty(iconPath) || bounds.IsEmpty)
                return;

            try
            {
                // Create circular path for step icons
                using (var path = GraphicsExtensions.CreateCircle(
                    bounds.X + bounds.Width / 2f,
                    bounds.Y + bounds.Height / 2f,
                    Math.Min(bounds.Width, bounds.Height) / 2f))
                {
                    // Use StyledImagePainter with tint and opacity
                    StyledImagePainter.PaintWithTint(g, path, iconPath, tint, opacity);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[StepperIconHelpers] Error painting icon '{iconPath}': {ex.Message}");
            }
        }

        /// <summary>
        /// Paint icon in a circle
        /// </summary>
        public static void PaintIconInCircle(
            Graphics g,
            float centerX,
            float centerY,
            float radius,
            string iconPath,
            Color tint,
            float opacity = 1f)
        {
            if (string.IsNullOrEmpty(iconPath))
                return;

            try
            {
                using (var path = GraphicsExtensions.CreateCircle(centerX, centerY, radius))
                {
                    StyledImagePainter.PaintWithTint(g, path, iconPath, tint, opacity);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[StepperIconHelpers] Error painting icon in circle: {ex.Message}");
            }
        }

        /// <summary>
        /// Paint icon with a custom GraphicsPath
        /// </summary>
        public static void PaintIconWithPath(
            Graphics g,
            GraphicsPath path,
            string iconPath,
            Color tint,
            float opacity = 1f)
        {
            if (string.IsNullOrEmpty(iconPath) || path == null)
                return;

            try
            {
                StyledImagePainter.PaintWithTint(g, path, iconPath, tint, opacity);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[StepperIconHelpers] Error painting icon with path: {ex.Message}");
            }
        }

        /// <summary>
        /// Paint checkmark icon for completed steps
        /// </summary>
        public static void PaintCheckmarkIcon(
            Graphics g,
            Rectangle bounds,
            Color tint,
            float opacity = 1f)
        {
            string checkPath = GetCheckIconPath();
            PaintIcon(g, bounds, checkPath, tint, opacity, StepState.Completed);
        }

        /// <summary>
        /// Paint step-specific icon based on item and state
        /// </summary>
        public static void PaintStepIcon(
            Graphics g,
            Rectangle bounds,
            SimpleItem item,
            StepState state,
            StepDisplayMode displayMode,
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (bounds.IsEmpty)
                return;

            // Get icon path
            string iconPath = GetStepIconPath(item, state, displayMode);
            if (string.IsNullOrEmpty(iconPath))
                return;

            // Get icon color
            Color iconColor = GetIconColor(theme, useThemeColors, state);

            // Get icon size
            Size iconSize = GetIconSize(new Size(bounds.Width, bounds.Height), state);
            Rectangle iconBounds = CalculateIconBounds(bounds, iconSize);

            // Paint icon
            PaintIcon(g, iconBounds, iconPath, iconColor, 1f, state);
        }

        #endregion
    }
}

