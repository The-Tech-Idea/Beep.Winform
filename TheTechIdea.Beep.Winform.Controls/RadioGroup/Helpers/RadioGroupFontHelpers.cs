using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in radio group controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class RadioGroupFontHelpers
    {
        /// <summary>
        /// Gets the font for radio item text
        /// Uses BeepFontManager with ControlStyle-aware sizing
        /// </summary>
        public static Font GetItemFont(
            BeepControlStyle controlStyle,
            bool isSelected = false)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Selected items can be slightly larger or bold
            float itemSize = isSelected ? baseSize : baseSize;
            FontStyle fontStyle = isSelected ? FontStyle.Regular : FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, itemSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for subtext/description
        /// </summary>
        public static Font GetSubtextFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float subtextSize = Math.Max(8f, baseSize - 2f); // Smaller for subtext
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, subtextSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for group label/header
        /// </summary>
        public static Font GetLabelFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float labelSize = baseSize + 1f; // Slightly larger for labels
            FontStyle fontStyle = FontStyle.Bold;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, labelSize, fontStyle);
        }

        /// <summary>
        /// Applies font theme to radio group control
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
