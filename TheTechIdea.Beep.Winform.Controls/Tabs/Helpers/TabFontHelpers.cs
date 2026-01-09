using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in tab controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class TabFontHelpers
    {
        /// <summary>
        /// Gets the font for tab text
        /// Uses BeepFontManager with ControlStyle-aware sizing
        /// </summary>
        public static Font GetTabFont(
            BeepControlStyle controlStyle,
            bool isSelected = false)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Selected tabs are typically bold
            float tabSize = baseSize;
            FontStyle fontStyle = isSelected ? FontStyle.Bold : FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, tabSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for tab subtext/description
        /// </summary>
        public static Font GetTabSubtextFont(
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
        /// Applies font theme to tab control
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
