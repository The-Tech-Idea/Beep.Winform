using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in switch controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class SwitchFontHelpers
    {
        /// <summary>
        /// Gets the font for switch labels (On/Off)
        /// Uses BeepFontManager with ControlStyle-aware sizing
        /// </summary>
        public static Font GetLabelFont(
            BeepControlStyle controlStyle,
            bool isActive = false)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Active labels can be slightly larger or bold
            float labelSize = isActive ? baseSize : baseSize;
            FontStyle fontStyle = isActive ? FontStyle.Regular : FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, labelSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for switch control (base font)
        /// </summary>
        public static Font GetSwitchFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, baseSize, fontStyle);
        }

        /// <summary>
        /// Applies font theme to switch control
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
