using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in numeric controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class NumericFontHelpers
    {
        /// <summary>
        /// Gets the font for numeric value text
        /// Uses BeepFontManager with ControlStyle-aware sizing
        /// </summary>
        public static Font GetValueFont(
            BeepControlStyle controlStyle,
            bool isEditing = false)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Editing mode can use slightly larger font for better visibility
            float valueSize = isEditing ? baseSize + 0.5f : baseSize;
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, valueSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for numeric control (base font)
        /// </summary>
        public static Font GetNumericFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, baseSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for button icons/labels
        /// Typically smaller than value font
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
        /// Gets the font for prefix/suffix/unit text
        /// Typically smaller and lighter than value font
        /// </summary>
        public static Font GetPrefixSuffixFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float prefixSize = baseSize * 0.9f; // 90% of base size
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, prefixSize, fontStyle);
        }

        /// <summary>
        /// Applies font theme to numeric control
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
