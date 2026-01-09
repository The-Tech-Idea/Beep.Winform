using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in accordion menu controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class AccordionFontHelpers
    {
        /// <summary>
        /// Gets the font for accordion header/title
        /// </summary>
        public static Font GetHeaderFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float headerSize = baseSize * 1.2f; // 20% larger
            FontStyle fontStyle = FontStyle.Bold;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, headerSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for menu items
        /// </summary>
        public static Font GetItemFont(
            BeepControlStyle controlStyle,
            bool isSelected = false)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            FontStyle fontStyle = isSelected ? FontStyle.Bold : FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, baseSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for child menu items (smaller than parent items)
        /// </summary>
        public static Font GetChildItemFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float childSize = baseSize * 0.9f; // 90% of base size
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, childSize, fontStyle);
        }

        /// <summary>
        /// Applies font theme to accordion control
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
