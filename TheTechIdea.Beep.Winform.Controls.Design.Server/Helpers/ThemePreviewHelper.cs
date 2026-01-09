using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers
{
    /// <summary>
    /// Helper for previewing how controls look with different themes
    /// Allows temporary theme application at design-time for comparison
    /// </summary>
    public static class ThemePreviewHelper
    {
        /// <summary>
        /// Preview a control with a specific theme (temporarily)
        /// </summary>
        /// <param name="control">The control to preview</param>
        /// <param name="themeName">Name of the theme to apply</param>
        /// <returns>Original theme name for restoration</returns>
        public static string PreviewWithTheme(BaseControl control, string themeName)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            var originalTheme = control.Theme;
            var theme = BeepThemesManager.GetTheme(themeName);

            if (theme != null)
            {
                control.Theme = themeName;
                control.ApplyTheme();
                control.Invalidate();
            }

            return originalTheme;
        }

        /// <summary>
        /// Restore a control to its original theme
        /// </summary>
        /// <param name="control">The control to restore</param>
        /// <param name="originalThemeName">Original theme name</param>
        public static void RestoreTheme(BaseControl control, string originalThemeName)
        {
            if (control == null)
                return;

            control.Theme = originalThemeName;
            control.ApplyTheme();
            control.Invalidate();
        }

        /// <summary>
        /// Get all available themes
        /// </summary>
        /// <returns>List of available theme names</returns>
        public static List<string> GetAvailableThemes()
        {
            // Get all registered themes from BeepThemesManager
            // This would require access to BeepThemesManager's internal theme registry
            // For now, return common theme names
            return new List<string>
            {
                "LinearTheme",
                "MaterialYouTheme",
                "NextJSTheme",
                "RadixUITheme",
                "ShadcnTheme"
            };
        }

        /// <summary>
        /// Compare how a control looks with different themes
        /// </summary>
        /// <param name="control">The control to compare</param>
        /// <param name="themeNames">List of theme names to compare</param>
        /// <returns>Dictionary mapping theme names to preview images (if implemented)</returns>
        public static Dictionary<string, Bitmap> CompareThemes(BaseControl control, List<string> themeNames)
        {
            var previews = new Dictionary<string, Bitmap>();

            if (control == null || !control.IsHandleCreated)
                return previews;

            var originalTheme = control.Theme;

            foreach (var themeName in themeNames)
            {
                try
                {
                    PreviewWithTheme(control, themeName);
                    control.Update();

                    // Capture preview (if control is visible and has size)
                    if (control.Width > 0 && control.Height > 0 && control.Visible)
                    {
                        var bitmap = new Bitmap(control.Width, control.Height);
                        control.DrawToBitmap(bitmap, new Rectangle(0, 0, control.Width, control.Height));
                        previews[themeName] = bitmap;
                    }
                }
                catch
                {
                    // Skip themes that fail to apply
                }
            }

            // Restore original theme
            RestoreTheme(control, originalTheme);

            return previews;
        }

        /// <summary>
        /// Get theme recommendations based on control style
        /// </summary>
        /// <param name="control">The control to get recommendations for</param>
        /// <returns>List of recommended theme names</returns>
        public static List<string> GetThemeRecommendations(BaseControl control)
        {
            if (control == null)
                return new List<string>();

            var recommendations = new List<string>();

            // Recommend themes based on control style
            var controlStyle = control.ControlStyle;
            
            // This is a simplified recommendation logic
            // In a full implementation, this would analyze the control's style
            // and recommend compatible themes
            recommendations.Add("MaterialYouTheme");
            recommendations.Add("ShadcnTheme");
            recommendations.Add("LinearTheme");

            return recommendations;
        }

        /// <summary>
        /// Check if a theme is compatible with a control
        /// </summary>
        /// <param name="control">The control to check</param>
        /// <param name="themeName">Theme name to check</param>
        /// <returns>True if theme is compatible</returns>
        public static bool IsThemeCompatible(BaseControl control, string themeName)
        {
            if (control == null || string.IsNullOrWhiteSpace(themeName))
                return false;

            try
            {
                var theme = BeepThemesManager.GetTheme(themeName);
                return theme != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
