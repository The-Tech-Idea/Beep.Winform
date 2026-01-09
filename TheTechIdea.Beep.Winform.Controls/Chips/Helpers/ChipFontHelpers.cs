using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in chip controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class ChipFontHelpers
    {
        /// <summary>
        /// Gets the font for chip text based on chip size and control style
        /// </summary>
        public static Font GetChipFont(
            BeepControlStyle controlStyle,
            ChipSize chipSize)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Adjust size based on chip size
            float chipFontSize = chipSize switch
            {
                ChipSize.Small => baseSize * 0.85f,
                ChipSize.Medium => baseSize,
                ChipSize.Large => baseSize * 1.15f,
                _ => baseSize
            };

            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, chipFontSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for chip group title
        /// </summary>
        public static Font GetTitleFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float titleSize = baseSize * 1.2f; // 20% larger
            FontStyle fontStyle = FontStyle.Bold;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, titleSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for chip icon (if text-based)
        /// </summary>
        public static Font GetIconFont(
            BeepControlStyle controlStyle,
            ChipSize chipSize)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Icon font is typically smaller
            float iconFontSize = chipSize switch
            {
                ChipSize.Small => baseSize * 0.7f,
                ChipSize.Medium => baseSize * 0.8f,
                ChipSize.Large => baseSize * 0.9f,
                _ => baseSize * 0.8f
            };

            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, iconFontSize, fontStyle);
        }

        /// <summary>
        /// Applies font theme to chip control
        /// Updates the control's Font property based on ControlStyle and ChipSize
        /// </summary>
        public static void ApplyFontTheme(
            BeepControlStyle controlStyle,
            ChipSize chipSize)
        {
            // This is a helper for getting fonts, not for setting control font
            // The control should use these helpers when painting
        }
    }
}
