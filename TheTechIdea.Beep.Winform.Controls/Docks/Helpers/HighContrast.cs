using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Helpers
{
    /// <summary>
    /// Substitutes system colours when Windows high-contrast mode is on.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A dock is entirely painted, so it renders its own palette regardless of the accessibility
    /// settings a user has chosen. High contrast exists precisely to override an application's
    /// colours, and a control that paints through it is unreadable for the people who turned it on.
    /// </para>
    /// <para>
    /// <c>BeepDock</c> already had <c>IsHighContrastMode()</c> and <c>GetHighContrastColor()</c> -
    /// private, on the control, and called by nothing. They could not have worked where they were:
    /// the colours are chosen by the painters, which have no access to the control. This is the same
    /// shape as the theme layer that had no callers, and the same fix - put it where the resolvers
    /// can reach it.
    /// </para>
    /// </remarks>
    public static class HighContrast
    {
        /// <summary>True when Windows is in a high-contrast theme.</summary>
        public static bool IsActive => SystemInformation.HighContrast;

        /// <summary>Background to paint, given what the style or theme chose.</summary>
        public static Color Background(Color proposed)
            => IsActive ? ColorUtils.MapSystemColor(SystemColors.Control) : proposed;

        /// <summary>Foreground to paint, given what the style or theme chose.</summary>
        public static Color Foreground(Color proposed)
            => IsActive ? ColorUtils.MapSystemColor(SystemColors.ControlText) : proposed;

        /// <summary>Selection/highlight fill.</summary>
        public static Color Selection(Color proposed)
            => IsActive ? ColorUtils.MapSystemColor(SystemColors.Highlight) : proposed;

        /// <summary>Text drawn on a selection fill.</summary>
        public static Color SelectionText(Color proposed)
            => IsActive ? ColorUtils.MapSystemColor(SystemColors.HighlightText) : proposed;

        /// <summary>Border/outline colour.</summary>
        public static Color Border(Color proposed)
            => IsActive ? ColorUtils.MapSystemColor(SystemColors.WindowFrame) : proposed;

        /// <summary>
        /// WCAG relative-luminance contrast ratio between two colours, 1.0 to 21.0.
        /// </summary>
        /// <remarks>
        /// Lives here rather than in the harness so the assertion and the implementation share one
        /// definition. A contrast check that computes the ratio differently from the code it is
        /// checking measures the difference between two formulas.
        /// </remarks>
        public static double ContrastRatio(Color a, Color b)
        {
            double la = RelativeLuminance(a);
            double lb = RelativeLuminance(b);
            double lighter = Math.Max(la, lb);
            double darker = Math.Min(la, lb);
            return (lighter + 0.05) / (darker + 0.05);
        }

        private static double RelativeLuminance(Color c)
        {
            static double Channel(int v)
            {
                double s = v / 255.0;
                return s <= 0.03928 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
            }

            return 0.2126 * Channel(c.R) + 0.7152 * Channel(c.G) + 0.0722 * Channel(c.B);
        }
    }
}
