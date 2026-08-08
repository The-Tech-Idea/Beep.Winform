using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Helpers
{
    /// <summary>
    /// Switch colour resolution: one slot, one return, from the theme's OWN Switch* family
    /// (the settled end-state). The previous version never touched a single Switch* slot -
    /// it ran the useThemeColors/Empty-guard/literal anti-pattern over GENERIC slots
    /// (SuccessColor as the on-track!) with Material-green fallbacks and banned
    /// ShiftLuminance hover/pressed derivations, when SwitchHoverBackColor exists for
    /// exactly that.
    /// </summary>
    public static class SwitchThemeHelpers
    {
        private static IBeepTheme T(IBeepTheme theme) => theme ?? BeepThemesManager.CurrentTheme;

        public static bool IsOn(SwitchState state) => state is SwitchState.On_Normal
            or SwitchState.On_Hover or SwitchState.On_Pressed
            or SwitchState.On_Disabled or SwitchState.On_Focused;

        public static bool IsHovered(SwitchState state) =>
            state is SwitchState.On_Hover or SwitchState.Off_Hover;

        public static bool IsPressed(SwitchState state) =>
            state is SwitchState.On_Pressed or SwitchState.Off_Pressed;

        public static bool IsDisabled(SwitchState state) =>
            state is SwitchState.On_Disabled or SwitchState.Off_Disabled;

        /// <summary>Track fill. On → SwitchSelectedBackColor; off → SwitchBackColor; hover/press on the off track → SwitchHoverBackColor. Custom overrides are caller data (Empty falls through).</summary>
        public static Color GetTrackColor(IBeepTheme theme, SwitchState state,
            Color? customOn = null, Color? customOff = null)
        {
            var t = T(theme);
            if (IsDisabled(state)) return t.DisabledBackColor;
            if (IsOn(state))
                return customOn is { } on && on != Color.Empty ? on : t.SwitchSelectedBackColor;
            if (IsHovered(state) || IsPressed(state)) return t.SwitchHoverBackColor;
            return customOff is { } off && off != Color.Empty ? off : t.SwitchBackColor;
        }

        public static Color GetTrackBorderColor(IBeepTheme theme, SwitchState state)
        {
            var t = T(theme);
            if (IsOn(state)) return t.SwitchSelectedBorderColor;
            if (IsHovered(state)) return t.SwitchHoverBorderColor;
            return t.SwitchBorderColor;
        }

        /// <summary>Thumb/knob fill: the on-track ink slot; disabled thumbs go flat.</summary>
        public static Color GetThumbColor(IBeepTheme theme, bool isOn, bool isHovered, bool isDisabled)
        {
            var t = T(theme);
            if (isDisabled) return t.DisabledBackColor;
            return t.SwitchSelectedForeColor;
        }

        /// <summary>
        /// On/Off caption ink. The ACTIVE side of an ON switch carries the selected-back
        /// accent so the current state reads at a glance; inactive captions are muted.
        /// </summary>
        public static Color GetLabelTextColor(IBeepTheme theme, bool isOn, bool isActive, bool isDisabled)
        {
            var t = T(theme);
            if (isDisabled) return t.DisabledForeColor;
            if (!isActive) return t.DisabledForeColor;
            return isOn ? t.SwitchSelectedBackColor : t.SwitchForeColor;
        }

        /// <summary>Elevation shadow - alpha veil of the theme's shadow slot.</summary>
        public static Color GetThumbShadowColor(IBeepTheme theme, int elevation = 2)
            => Color.FromArgb(Math.Min(255, elevation * 20), T(theme).ShadowColor);

        /// <summary>Focus ring - alpha veil of the primary slot.</summary>
        public static Color GetFocusRingColor(IBeepTheme theme)
            => Color.FromArgb(80, T(theme).PrimaryColor);
    }
}
