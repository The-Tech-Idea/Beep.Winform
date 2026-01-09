using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in vertical table controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class VerticalTableFontHelpers
    {
        /// <summary>
        /// Gets the font for column headers
        /// Uses BeepFontManager with ControlStyle-aware sizing
        /// </summary>
        public static Font GetHeaderFont(
            BeepControlStyle controlStyle,
            bool isFeatured = false)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Featured headers are slightly larger and bold
            float headerSize = isFeatured ? baseSize + 2f : baseSize;
            FontStyle fontStyle = isFeatured ? FontStyle.Bold : FontStyle.Bold;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, headerSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for cell text
        /// </summary>
        public static Font GetCellFont(
            BeepControlStyle controlStyle,
            bool isSelected = false)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Selected cells can be slightly larger
            float cellSize = isSelected ? baseSize : baseSize - 1f;
            FontStyle fontStyle = isSelected ? FontStyle.Regular : FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, cellSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for price/value text (typically in SubText2)
        /// </summary>
        public static Font GetPriceFont(
            BeepControlStyle controlStyle,
            bool isFeatured = false)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Price text is typically larger and bold
            float priceSize = isFeatured ? baseSize + 4f : baseSize + 2f;
            FontStyle fontStyle = FontStyle.Bold;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, priceSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for badge/featured text
        /// </summary>
        public static Font GetBadgeFont(
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float badgeSize = Math.Max(8f, baseSize - 2f); // Smaller for badges
            FontStyle fontStyle = FontStyle.Bold;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, badgeSize, fontStyle);
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
        /// Applies font theme to vertical table control
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
