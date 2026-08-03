using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Helpers
{
    /// <summary>
    /// Resolves a Beep theme into the palette the docking painters read.
    /// </summary>
    /// <remarks>
    /// The docking controls derive from <see cref="System.Windows.Forms.Panel"/>,
    /// <see cref="System.Windows.Forms.Control"/> and <see cref="System.Windows.Forms.Form"/> rather
    /// than from <c>BaseControl</c>, so they do not inherit its <c>ApplyTheme</c>. Each therefore
    /// exposes the same <c>ApplyTheme</c> surface and forwards here, so the rule for turning a theme
    /// into docking colours exists <b>once</b> rather than five times.
    /// <para>
    /// Every path goes through <see cref="BeepThemesManager"/>: a control asked to apply "the
    /// current theme" must get the same answer as every other control in the application.
    /// </para>
    /// </remarks>
    internal static class DockingThemeResolver
    {
        /// <summary>The theme currently in force, never null.</summary>
        public static IBeepTheme Current
            => BeepThemesManager.CurrentTheme ?? BeepThemesManager.GetDefaultTheme();

        /// <summary>Resolves a named theme, falling back to the current one when unknown.</summary>
        public static IBeepTheme ByName(string themeName)
            => (string.IsNullOrWhiteSpace(themeName)
                    ? null
                    : BeepThemesManager.GetTheme(themeName))
               ?? Current;

        /// <summary>Docking palette for a theme.</summary>
        public static DockingThemeColors Palette(IBeepTheme theme, bool useThemeColors)
            => DockingThemeColors.FromTheme(theme ?? Current, useThemeColors);
    }
}
