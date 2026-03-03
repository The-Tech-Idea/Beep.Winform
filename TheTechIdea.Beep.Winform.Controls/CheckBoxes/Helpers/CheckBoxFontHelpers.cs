using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

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
            IBeepTheme theme,
            BeepControlStyle controlStyle,
            Control control = null)
        {
            if (theme?.CheckBoxFont != null)
            {
                return control == null
                    ? BeepThemesManager.ToFont(theme.CheckBoxFont, true)
                    : BeepThemesManager.ToFontForControl(theme.CheckBoxFont, control);
            }

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();
            float fontSize = StyleTypography.GetFontSize(controlStyle);

            return BeepThemesManager.ToFont(primaryFont, fontSize, FontWeight.Regular, FontStyle.Regular);
        }

        /// <summary>
        /// Applies font theme to checkbox control
        /// Updates the TextFont property for checkbox controls using theme typography.
        /// </summary>
        public static void ApplyFontTheme(
            Control control,
            BeepControlStyle controlStyle,
            float dpiScale,
            IBeepTheme theme = null)
        {
            if (control == null) return;

            Font font = GetCheckBoxFont(theme ?? BeepThemesManager.GetDefaultTheme(), controlStyle, control);
            if (theme?.CheckBoxFont == null && dpiScale > 0f && Math.Abs(dpiScale - 1f) > 0.001f)
            {
                string family = StyleTypography.GetFontFamily(controlStyle).Split(',')[0].Trim();
                float size = DpiScalingHelper.ScaleValue(StyleTypography.GetFontSize(controlStyle), dpiScale);
                font = BeepThemesManager.ToFont(family, size, FontWeight.Regular, FontStyle.Regular);
            }
            var textFontProperty = control.GetType().GetProperty("TextFont");
            if (textFontProperty != null && textFontProperty.PropertyType == typeof(Font) && textFontProperty.CanWrite)
            {
                textFontProperty.SetValue(control, font);
            }
        }
    }
}
