using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Helpers
{
    /// <summary>
    /// Rating colour resolution: one slot, one return (the settled end-state). The theme has a
    /// dedicated 10-slot StarRating* family - the previous version probed it by REFLECTION
    /// (properties that sit right on IBeepTheme), fell through guard chains to a Gold/Gray
    /// palette, and its ApplyThemeColors passed the control's own colours as the always-winning
    /// custom override - so the control never followed the theme at all.
    /// customColor stays: an explicit caller override is data (Empty falls through to the slot).
    /// </summary>
    public static class RatingThemeHelpers
    {
        private static IBeepTheme T(IBeepTheme theme) => theme ?? BeepThemesManager.CurrentTheme;

        // Windows high-contrast is a SYSTEM accessibility override, resolved per paint so
        // toggling it applies on the next repaint. It outranks explicit custom colours.
        private static bool HC => RatingAccessibilityHelpers.IsHighContrastMode();

        /// <summary>Fill for selected ratings. Hearts/thumbs keep their semantic identity via semantic slots.</summary>
        public static Color GetFilledRatingColor(IBeepTheme theme, RatingStyle style, Color? customColor = null)
        {
            if (HC) return SystemColors.Highlight;
            if (customColor is { } c && c != Color.Empty) return c;
            var t = T(theme);
            return style switch
            {
                RatingStyle.Heart => t.ErrorColor,
                RatingStyle.Thumb or RatingStyle.Circle => t.PrimaryColor,
                _ => t.StarRatingFillColor,
            };
        }

        public static Color GetEmptyRatingColor(IBeepTheme theme, RatingStyle style, Color? customColor = null)
            => HC ? SystemColors.ControlDark
             : customColor is { } c && c != Color.Empty ? c : T(theme).StarRatingBackColor;

        public static Color GetHoverRatingColor(IBeepTheme theme, RatingStyle style, Color? customColor = null)
            => HC ? SystemColors.HotTrack
             : customColor is { } c && c != Color.Empty ? c : T(theme).StarRatingHoverForeColor;

        public static Color GetRatingBorderColor(IBeepTheme theme, RatingStyle style, Color? customColor = null)
            => HC ? SystemColors.WindowFrame
             : customColor is { } c && c != Color.Empty ? c : T(theme).StarRatingBorderColor;

        public static Color GetRatingLabelColor(IBeepTheme theme, Color? customColor = null)
            => HC ? SystemColors.WindowText
             : customColor is { } c && c != Color.Empty ? c : T(theme).StarRatingForeColor;
    }
}
