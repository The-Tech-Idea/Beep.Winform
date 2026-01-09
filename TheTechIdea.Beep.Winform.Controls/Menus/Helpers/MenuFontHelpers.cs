using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in menu bar controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class MenuFontHelpers
    {
        /// <summary>
        /// Gets the font for menu item text
        /// Uses BeepFontManager with ControlStyle-aware sizing
        /// </summary>
        public static Font GetMenuItemFont(
            BeepControlStyle controlStyle,
            IBeepTheme theme = null)
        {
            // Priority 1: Theme font if available
            if (theme != null)
            {
                if (theme.MenuItemUnSelectedFont != null)
                {
                    try
                    {
                        // Use the same approach as BeepMenuBar.cs
                        return FontListHelper.CreateFontFromTypography(theme.MenuItemUnSelectedFont);
                    }
                    catch
                    {
                        // Fall through to default
                    }
                }
                if (theme.LabelFont != null)
                {
                    try
                    {
                        // Use the same approach as BeepMenuBar.cs
                        return FontListHelper.CreateFontFromTypography(theme.LabelFont);
                    }
                    catch
                    {
                        // Fall through to default
                    }
                }
            }

            // Priority 2: ControlStyle-based font
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, baseSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for menu bar (base font)
        /// </summary>
        public static Font GetMenuBarFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, baseSize, fontStyle);
        }

        /// <summary>
        /// Applies font theme to menu bar control
        /// Updates the control's Font property based on ControlStyle and theme
        /// </summary>
        public static void ApplyFontTheme(
            BeepControlStyle controlStyle,
            IBeepTheme theme = null)
        {
            // This is a helper for getting fonts, not for setting control font
            // The control should use these helpers when painting
        }
    }
}
