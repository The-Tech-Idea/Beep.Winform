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
            if (customColor.HasValue && customColor.Value != Color.Empty)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                var property = typeof(IBeepTheme).GetProperty("StepperCompletedColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                if (theme.SuccessColor != Color.Empty)
                    return theme.SuccessColor;
                if (theme.PrimaryColor != Color.Empty)
                    return theme.PrimaryColor;
            }

            return Color.FromArgb(34, 197, 94);
        }

        public static Color GetStepActiveColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)
        {
            if (customColor.HasValue && customColor.Value != Color.Empty)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                var property = typeof(IBeepTheme).GetProperty("StepperActiveColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                if (theme.PrimaryColor != Color.Empty)
                    return theme.PrimaryColor;
                if (theme.AccentColor != Color.Empty)
                    return theme.AccentColor;
            }

            return Color.FromArgb(59, 130, 246);
        }

        public static Color GetStepPendingColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)
        {
            if (customColor.HasValue && customColor.Value != Color.Empty)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                var property = typeof(IBeepTheme).GetProperty("StepperPendingColor");
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                if (theme.DisabledBackColor != Color.Empty)
                    return theme.DisabledBackColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
            }

            return Color.FromArgb(209, 213, 219);
        }

        /// <summary>
        /// Get color for error steps
        /// </summary>
        public static Color GetStepErrorColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)
        {
            if (customColor.HasValue && customColor.Value != Color.Empty)
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
            return Color.Red;
        }

        /// <summary>
        /// Get color for warning steps
        /// </summary>
        public static Color GetStepWarningColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)
        {
            if (customColor.HasValue && customColor.Value != Color.Empty)
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
            return Color.Goldenrod;
        }

        #endregion

        #region Connector Line Colors

        /// <summary>
        /// Get color for connector lines based on step state
        /// </summary>
        public static Color GetConnectorLineColor(IBeepTheme theme, bool useThemeColors, StepState state, Color? customColor = null)
        {
            if (customColor.HasValue && customColor.Value != Color.Empty)
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
                ? Color.FromArgb(34, 197, 94)
                : Color.FromArgb(209, 213, 219);
        }

        #endregion

        #region Text Colors

        public static Color GetStepTextColor(IBeepTheme theme, bool useThemeColors, StepState state, Color? customColor = null)
        {
            if (customColor.HasValue && customColor.Value != Color.Empty)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                string propertyName = state == StepState.Active
                    ? "StepperActiveTextColor"
                    : "StepperPendingTextColor";

                var property = typeof(IBeepTheme).GetProperty(propertyName);
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                if (state == StepState.Active)
                {
                    if (theme.PrimaryTextColor != Color.Empty)
                        return theme.PrimaryTextColor;
                    if (theme.ButtonForeColor != Color.Empty)
                        return theme.ButtonForeColor;
                }
                else if (state == StepState.Completed)
                {
                    if (theme.ButtonForeColor != Color.Empty)
                        return theme.ButtonForeColor;
                }
                else
                {
                    if (theme.SecondaryTextColor != Color.Empty)
                        return theme.SecondaryTextColor;
                    if (theme.ForeColor != Color.Empty)
                        return theme.ForeColor;
                }
            }

            return state == StepState.Active || state == StepState.Completed
                ? Color.White
                : Color.FromArgb(107, 114, 128);
        }

        public static Color GetStepLabelColor(IBeepTheme theme, bool useThemeColors, StepState state, Color? customColor = null)
        {
            if (customColor.HasValue && customColor.Value != Color.Empty)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                string propertyName = state == StepState.Active
                    ? "StepperActiveLabelColor"
                    : "StepperPendingLabelColor";

                var property = typeof(IBeepTheme).GetProperty(propertyName);
                if (property != null && property.GetValue(theme) is Color themeColor && themeColor != Color.Empty)
                    return themeColor;

                if (state == StepState.Active)
                {
                    if (theme.CardTitleForeColor != Color.Empty)
                        return theme.CardTitleForeColor;
                    if (theme.ForeColor != Color.Empty)
                        return theme.ForeColor;
                }
                else
                {
                    if (theme.CardSubTitleForeColor != Color.Empty)
                        return theme.CardSubTitleForeColor;
                    if (theme.SecondaryTextColor != Color.Empty)
                        return theme.SecondaryTextColor;
                }
            }

            return state == StepState.Active
                ? Color.FromArgb(31, 41, 55)
                : Color.FromArgb(107, 114, 128);
        }

        #endregion

        #region Background and Border Colors

        /// <summary>
        /// Get background color for stepper control
        /// </summary>
        public static Color GetStepBackgroundColor(IBeepTheme theme, bool useThemeColors, Color? customColor = null)
        {
            if (customColor.HasValue && customColor.Value != Color.Empty)
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
            if (customColor.HasValue && customColor.Value != Color.Empty)
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

            // Default fallback
            return state == StepState.Active
                ? Color.FromArgb(59, 130, 246)
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

