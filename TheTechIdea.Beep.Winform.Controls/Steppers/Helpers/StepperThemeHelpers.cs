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

        // One slot, one return (the settled end-state; user directive). This file previously probed
        // the theme BY REFLECTION for properties that do not exist on IBeepTheme
        // ("StepperCompletedColor", "StepperConnectorPendingColor", ...) on every colour of every
        // paint - the probe never once hit, and every call fell through guard chains to a hardcoded
        // Tailwind palette. The REAL slots are the StepperItem* family below. customColor stays: an
        // explicit caller override is data.

        private static IBeepTheme T(IBeepTheme theme) => theme ?? BeepThemesManager.CurrentTheme;

        public static Color GetStepCompletedColor(IBeepTheme theme, Color? customColor = null)
            => customColor is { } c && c != Color.Empty ? c : T(theme).StepperItemCheckedBoxBackColor;

        public static Color GetStepActiveColor(IBeepTheme theme, Color? customColor = null)
            => customColor is { } c && c != Color.Empty ? c : T(theme).StepperItemSelectedBackColor;

        public static Color GetStepPendingColor(IBeepTheme theme, Color? customColor = null)
            => customColor is { } c && c != Color.Empty ? c : T(theme).DisabledBackColor;

        public static Color GetStepErrorColor(IBeepTheme theme, Color? customColor = null)
            => customColor is { } c && c != Color.Empty ? c : T(theme).ErrorColor;

        public static Color GetStepWarningColor(IBeepTheme theme, Color? customColor = null)
            => customColor is { } c && c != Color.Empty ? c : T(theme).WarningColor;

        public static Color GetConnectorLineColor(IBeepTheme theme, StepState state, Color? customColor = null)
        {
            if (customColor is { } c && c != Color.Empty) return c;
            var t = T(theme);
            return state == StepState.Completed ? t.StepperItemCheckedBoxBackColor : t.StepperBorderColor;
        }

        #endregion

        #region Text Colors

        public static Color GetStepTextColor(IBeepTheme theme, StepState state, Color? customColor = null)
        {
            if (customColor is { } c && c != Color.Empty) return c;
            var t = T(theme);
            return state switch
            {
                StepState.Active => t.StepperItemSelectedForeColor,
                StepState.Completed => t.StepperItemCheckedBoxForeColor,
                _ => t.StepperItemForeColor,
            };
        }

        public static Color GetStepLabelColor(IBeepTheme theme, StepState state, Color? customColor = null)
        {
            if (customColor is { } c && c != Color.Empty) return c;
            var t = T(theme);
            return state == StepState.Active ? t.StepperForeColor : t.DisabledForeColor;
        }

        #endregion

        #region Background and Border Colors

        public static Color GetStepBackgroundColor(IBeepTheme theme, Color? customColor = null)
            => customColor is { } c && c != Color.Empty ? c : T(theme).StepperBackColor;

        public static Color GetStepBorderColor(IBeepTheme theme, StepState state, Color? customColor = null)
        {
            if (customColor is { } c && c != Color.Empty) return c;
            var t = T(theme);
            return state switch
            {
                StepState.Active => t.StepperItemSelectedBorderColor,
                StepState.Error => t.ErrorColor,
                _ => t.StepperItemBorderColor,
            };
        }

        #endregion

    }
}

