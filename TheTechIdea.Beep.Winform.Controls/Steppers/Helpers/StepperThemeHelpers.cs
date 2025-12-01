using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Helpers
{
    /// <summary>
    /// Centralized theme color management for Stepper controls
    /// Provides consistent color retrieval based on theme and step state
    /// </summary>
    public static class StepperThemeHelpers
    {
        #region Step State Colors

        /// <summary>
        /// Get color for completed steps
        /// </summary>
        public static Color GetStepCompletedColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                var property = typeof(IBeepTheme).GetProperty("StepperCompletedColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Fallback to success color
                return theme.SuccessColor;
            }

            // Default fallback
            return Color.FromArgb(34, 197, 94); // Green
        }

        /// <summary>
        /// Get color for active/current step
        /// </summary>
        public static Color GetStepActiveColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                var property = typeof(IBeepTheme).GetProperty("StepperActiveColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Fallback to primary color
                return theme.PrimaryColor;
            }

            // Default fallback
            return Color.FromArgb(59, 130, 246); // Blue
        }

        /// <summary>
        /// Get color for pending/future steps
        /// </summary>
        public static Color GetStepPendingColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                var property = typeof(IBeepTheme).GetProperty("StepperPendingColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Fallback to disabled color
                return theme.DisabledBackColor;
            }

            // Default fallback
            return Color.FromArgb(156, 163, 175); // Gray
        }

        /// <summary>
        /// Get color for error steps
        /// </summary>
        public static Color GetStepErrorColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                var property = typeof(IBeepTheme).GetProperty("StepperErrorColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Fallback to error color
                return theme.ErrorColor;
            }

            // Default fallback
            return Color.FromArgb(239, 68, 68); // Red
        }

        /// <summary>
        /// Get color for warning steps
        /// </summary>
        public static Color GetStepWarningColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                var property = typeof(IBeepTheme).GetProperty("StepperWarningColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Fallback to warning color
                return theme.WarningColor;
            }

            // Default fallback
            return Color.FromArgb(245, 158, 11); // Orange
        }

        #endregion

        #region Connector Line Colors

        /// <summary>
        /// Get color for connector lines based on step state
        /// </summary>
        public static Color GetConnectorLineColor(IBeepTheme theme, bool useThemeColors, StepState state, Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                string propertyName = state == StepState.Completed 
                    ? "StepperConnectorCompletedColor" 
                    : "StepperConnectorPendingColor";
                
                var property = typeof(IBeepTheme).GetProperty(propertyName);
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Fallback based on state
                return state == StepState.Completed 
                    ? GetStepCompletedColor(theme, useThemeColors)
                    : GetStepPendingColor(theme, useThemeColors);
            }

            // Default fallback
            return state == StepState.Completed
                ? Color.FromArgb(34, 197, 94) // Green
                : Color.FromArgb(156, 163, 175); // Gray
        }

        #endregion

        #region Text Colors

        /// <summary>
        /// Get color for step text (numbers, icons) based on step state
        /// </summary>
        public static Color GetStepTextColor(IBeepTheme theme, bool useThemeColors, StepState state, Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                string propertyName = state == StepState.Active
                    ? "StepperActiveTextColor"
                    : "StepperPendingTextColor";

                var property = typeof(IBeepTheme).GetProperty(propertyName);
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Fallback based on state
                if (state == StepState.Active)
                    return theme.PrimaryTextColor;
                else if (state == StepState.Completed)
                    return Color.White; // White text on colored background
                else
                    return theme.SecondaryTextColor;
            }

            // Default fallback - white for active/completed, gray for pending
            return state == StepState.Active || state == StepState.Completed
                ? Color.White
                : Color.FromArgb(107, 114, 128); // Gray
        }

        /// <summary>
        /// Get color for step labels based on step state
        /// </summary>
        public static Color GetStepLabelColor(IBeepTheme theme, bool useThemeColors, StepState state, Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                string propertyName = state == StepState.Active
                    ? "StepperActiveLabelColor"
                    : "StepperPendingLabelColor";

                var property = typeof(IBeepTheme).GetProperty(propertyName);
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Fallback based on state
                if (state == StepState.Active)
                    return theme.CardTitleForeColor;
                else
                    return theme.CardSubTitleForeColor;
            }

            // Default fallback
            return state == StepState.Active
                ? Color.Black
                : Color.Gray;
        }

        #endregion

        #region Background and Border Colors

        /// <summary>
        /// Get background color for stepper control
        /// </summary>
        public static Color GetStepBackgroundColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                var property = typeof(IBeepTheme).GetProperty("StepperBackColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Fallback to card background
                return theme.CardBackColor;
            }

            // Default fallback
            return Color.Transparent;
        }

        /// <summary>
        /// Get border color for steps based on step state
        /// </summary>
        public static Color GetStepBorderColor(IBeepTheme theme, bool useThemeColors, StepState state, Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                // Try theme-specific property first
                var property = typeof(IBeepTheme).GetProperty("StepperBorderColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                // Fallback to border color
                return theme.BorderColor;
            }

            // Default fallback - white border for active step, transparent for others
            return state == StepState.Active
                ? Color.White
                : Color.Transparent;
        }

        #endregion

        #region Theme Color Application

        /// <summary>
        /// Get all theme colors for a specific step state
        /// </summary>
        public static (Color fillColor, Color textColor, Color labelColor, Color borderColor, Color connectorColor) GetThemeColors(
            IBeepTheme theme, 
            bool useThemeColors, 
            StepState state)
        {
            Color fillColor = state switch
            {
                StepState.Completed => GetStepCompletedColor(theme, useThemeColors),
                StepState.Active => GetStepActiveColor(theme, useThemeColors),
                StepState.Error => GetStepErrorColor(theme, useThemeColors),
                StepState.Warning => GetStepWarningColor(theme, useThemeColors),
                _ => GetStepPendingColor(theme, useThemeColors)
            };

            Color textColor = GetStepTextColor(theme, useThemeColors, state);
            Color labelColor = GetStepLabelColor(theme, useThemeColors, state);
            Color borderColor = GetStepBorderColor(theme, useThemeColors, state);
            Color connectorColor = GetConnectorLineColor(theme, useThemeColors, state);

            return (fillColor, textColor, labelColor, borderColor, connectorColor);
        }

        /// <summary>
        /// Apply theme colors to a stepper control
        /// Updates all color properties based on theme
        /// </summary>
        public static void ApplyThemeColors(dynamic stepper, IBeepTheme theme, bool useThemeColors)
        {
            if (stepper == null || theme == null)
                return;

            try
            {
                // Apply step state colors if properties exist
                if (HasProperty(stepper, "CompletedStepColor"))
                    stepper.CompletedStepColor = GetStepCompletedColor(theme, useThemeColors, GetPropertyValue<Color?>(stepper, "CompletedStepColor"));

                if (HasProperty(stepper, "ActiveStepColor"))
                    stepper.ActiveStepColor = GetStepActiveColor(theme, useThemeColors, GetPropertyValue<Color?>(stepper, "ActiveStepColor"));

                if (HasProperty(stepper, "PendingStepColor"))
                    stepper.PendingStepColor = GetStepPendingColor(theme, useThemeColors, GetPropertyValue<Color?>(stepper, "PendingStepColor"));

                if (HasProperty(stepper, "ErrorStepColor"))
                    stepper.ErrorStepColor = GetStepErrorColor(theme, useThemeColors, GetPropertyValue<Color?>(stepper, "ErrorStepColor"));

                if (HasProperty(stepper, "WarningStepColor"))
                    stepper.WarningStepColor = GetStepWarningColor(theme, useThemeColors, GetPropertyValue<Color?>(stepper, "WarningStepColor"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[StepperThemeHelpers] Error applying theme colors: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        private static bool HasProperty(dynamic obj, string propertyName)
        {
            try
            {
                var property = obj.GetType().GetProperty(propertyName);
                return property != null;
            }
            catch
            {
                return false;
            }
        }

        private static T GetPropertyValue<T>(dynamic obj, string propertyName)
        {
            try
            {
                var property = obj.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    var value = property.GetValue(obj);
                    if (value is T typedValue)
                        return typedValue;
                }
            }
            catch
            {
                // Ignore
            }
            return default(T);
        }

        #endregion
    }
}

