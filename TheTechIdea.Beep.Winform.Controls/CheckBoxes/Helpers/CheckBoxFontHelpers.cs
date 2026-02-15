using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in checkbox controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class CheckBoxFontHelpers
    {
        /// <summary>
        /// Gets the font for checkbox text
        /// </summary>
        public static Font GetCheckBoxFont(
            BeepControlStyle controlStyle, float dpiScale = 1.0f)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            // Scale font size
            float scaledSize = DpiScalingHelper.ScaleValue(baseSize, dpiScale);
            FontStyle fontStyle = FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            // string primaryFont = fontFamily.Split(',')[0].Trim(); // BeepFontManager handles comma-separated list now likely, or we keep as is
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, scaledSize, fontStyle);
        }

        /// <summary>
        /// Applies font theme to checkbox control
        /// Updates the control's Font property based on ControlStyle
        /// </summary>
        public static void ApplyFontTheme(
            Control control,
            BeepControlStyle controlStyle,
            float dpiScale)
        {
            if (control == null) return;
            control.Font = GetCheckBoxFont(controlStyle, dpiScale);
        }
    }
}
