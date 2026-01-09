using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in dock controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class DockFontHelpers
    {
        /// <summary>
        /// Gets the font for dock item labels
        /// Uses BeepFontManager with ControlStyle-aware sizing
        /// </summary>
        public static Font GetDockItemFont(
            BeepControlStyle controlStyle,
            bool isHovered = false)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Hovered items can use slightly larger font
            float itemSize = isHovered ? baseSize * 1.1f : baseSize;
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, itemSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for dock control (base font)
        /// </summary>
        public static Font GetDockFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, baseSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for badge/notification text
        /// Typically smaller than item font
        /// </summary>
        public static Font GetBadgeFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float badgeSize = baseSize * 0.75f; // 75% of base size
            FontStyle fontStyle = FontStyle.Bold;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, badgeSize, fontStyle);
        }

        /// <summary>
        /// Applies font theme to dock control
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
