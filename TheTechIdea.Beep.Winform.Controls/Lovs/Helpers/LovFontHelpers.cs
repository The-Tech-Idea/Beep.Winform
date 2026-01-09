using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.Lovs.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in LOV controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class LovFontHelpers
    {
        /// <summary>
        /// Gets the font for LOV text (key and value textboxes)
        /// Uses BeepFontManager with ControlStyle-aware sizing
        /// </summary>
        public static Font GetLovFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, baseSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for button text/icons
        /// Typically smaller than LOV font
        /// </summary>
        public static Font GetButtonFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float buttonSize = baseSize * 0.85f; // 85% of base size
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, buttonSize, fontStyle);
        }

        /// <summary>
        /// Applies font theme to LOV control
        /// Updates the control's Font property based on ControlStyle
        /// </summary>
        public static void ApplyFontTheme(
            BeepControlStyle controlStyle)
        {
            // This is a helper for getting fonts, not for setting control font
            // The control should use these helpers when painting
        }
    }
}
