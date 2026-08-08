using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers
{
    /// <summary>
    /// The colour-resolution seam for progress bars: slot-direct from the theme's
    /// ProgressBar* family, one slot one return, custom overrides as Empty-passthrough.
    /// The control's colour property GETTERS resolve through here on every read, so the
    /// painters (which read owner properties per paint) always see the live theme and an
    /// explicit caller colour survives theme changes. High contrast overrides both.
    /// </summary>
    public static class ProgressBarThemeHelpers
    {
        private static IBeepTheme T(IBeepTheme theme) => theme ?? BeepThemesManager.CurrentTheme;

        private static bool HC => SystemInformation.HighContrast;

        /// <summary>Track/background fill.</summary>
        public static Color GetProgressBarBackColor(IBeepTheme theme, Color? customColor = null)
        {
            if (HC) return SystemColors.Control;
            if (customColor is { } c && c != Color.Empty) return c;
            return T(theme).ProgressBarBackColor;
        }

        /// <summary>Progress fill.</summary>
        public static Color GetProgressBarForeColor(IBeepTheme theme, Color? customColor = null)
        {
            if (HC) return SystemColors.Highlight;
            if (customColor is { } c && c != Color.Empty) return c;
            return T(theme).ProgressBarForeColor;
        }

        /// <summary>Ink for text drawn inside the bar.</summary>
        public static Color GetProgressBarTextColor(IBeepTheme theme, Color? customColor = null)
        {
            if (HC) return SystemColors.ControlText;
            if (customColor is { } c && c != Color.Empty) return c;
            return T(theme).ProgressBarInsideTextColor;
        }

        /// <summary>Outline.</summary>
        public static Color GetProgressBarBorderColor(IBeepTheme theme, Color? customColor = null)
        {
            if (HC) return SystemColors.WindowFrame;
            if (customColor is { } c && c != Color.Empty) return c;
            return T(theme).ProgressBarBorderColor;
        }

        /// <summary>Success state fill (auto-colour mode / ProgressState).</summary>
        public static Color GetProgressBarSuccessColor(IBeepTheme theme, Color? customColor = null)
        {
            if (HC) return SystemColors.Highlight;
            if (customColor is { } c && c != Color.Empty) return c;
            return T(theme).ProgressBarSuccessColor;
        }

        /// <summary>
        /// Warning state fill. The theme has NO ProgressBarWarningColor slot — the old
        /// helper probed for one by reflection on every call; the semantic slot is the source.
        /// </summary>
        public static Color GetProgressBarWarningColor(IBeepTheme theme, Color? customColor = null)
        {
            if (HC) return SystemColors.Highlight;
            if (customColor is { } c && c != Color.Empty) return c;
            return T(theme).WarningColor;
        }

        /// <summary>Error state fill.</summary>
        public static Color GetProgressBarErrorColor(IBeepTheme theme, Color? customColor = null)
        {
            if (HC) return SystemColors.Highlight;
            if (customColor is { } c && c != Color.Empty) return c;
            return T(theme).ProgressBarErrorColor;
        }

        /// <summary>Secondary/buffer progress: an alpha veil of the secondary slot.</summary>
        public static Color GetProgressBarSecondaryColor(IBeepTheme theme, Color? customColor = null, int opacity = 50)
        {
            if (customColor is { } c && c != Color.Empty) return c;
            return Color.FromArgb(opacity, T(theme).SecondaryColor);
        }

        /// <summary>Hover fill for interactive painter areas.</summary>
        public static Color GetProgressBarHoverBackColor(IBeepTheme theme)
        {
            if (HC) return SystemColors.HotTrack;
            return T(theme).ProgressBarHoverBackColor;
        }

        /// <summary>Hover ink for interactive painter areas.</summary>
        public static Color GetProgressBarHoverForeColor(IBeepTheme theme)
        {
            if (HC) return SystemColors.HighlightText;
            return T(theme).ProgressBarHoverForeColor;
        }

        /// <summary>Hover outline for interactive painter areas.</summary>
        public static Color GetProgressBarHoverBorderColor(IBeepTheme theme)
        {
            if (HC) return SystemColors.Highlight;
            return T(theme).ProgressBarHoverBorderColor;
        }
    }
}
