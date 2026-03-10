using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Vis.Modules;

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
            bool isSelected = false,
            IBeepTheme theme = null,
            Control ownerControl = null)
        {
            var textTypography = theme?.BodyMedium ?? theme?.LabelMedium;
            if (textTypography != null)
            {
                return BeepThemesManager.ToFont(
                    textTypography.FontFamily,
                    textTypography.FontSize,
                    isSelected ? FontWeight.Medium : FontWeight.Regular,
                    FontStyle.Regular);
            }

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            if (ownerControl != null)
            {
                baseSize = DpiScalingHelper.ScaleValue(baseSize, ownerControl);
            }
            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();
            return BeepThemesManager.ToFont(
                primaryFont,
                baseSize,
                isSelected ? FontWeight.Medium : FontWeight.Regular,
                FontStyle.Regular);
        }

        /// <summary>
        /// Gets the font for subtext/description
        /// </summary>
        public static Font GetSubtextFont(
            BeepControlStyle controlStyle,
            IBeepTheme theme = null,
            Control ownerControl = null)
        {
            var subTypography = theme?.BodySmall ?? theme?.LabelSmall;
            if (subTypography != null)
            {
                return BeepThemesManager.ToFont(
                    subTypography.FontFamily,
                    subTypography.FontSize,
                    FontWeight.Regular,
                    FontStyle.Regular);
            }

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            if (ownerControl != null)
            {
                baseSize = DpiScalingHelper.ScaleValue(baseSize, ownerControl);
            }
            float subtextSize = Math.Max(8f, baseSize - 2f); // Smaller for subtext
            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();
            return BeepThemesManager.ToFont(primaryFont, subtextSize, FontWeight.Regular, FontStyle.Regular);
        }

        /// <summary>
        /// Gets the font for group label/header
        /// </summary>
        public static Font GetLabelFont(
            BeepControlStyle controlStyle,
            IBeepTheme theme = null,
            Control ownerControl = null)
        {
            var labelTypography = theme?.LabelLarge ?? theme?.TitleSmall;
            if (labelTypography != null)
            {
                return BeepThemesManager.ToFont(
                    labelTypography.FontFamily,
                    labelTypography.FontSize,
                    FontWeight.Bold,
                    FontStyle.Bold);
            }

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            if (ownerControl != null)
            {
                baseSize = DpiScalingHelper.ScaleValue(baseSize, ownerControl);
            }
            float labelSize = baseSize + 1f; // Slightly larger for labels
            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();
            return BeepThemesManager.ToFont(primaryFont, labelSize, FontWeight.Bold, FontStyle.Bold);
        }

        /// <summary>
        /// Applies font theme to radio group control
        /// Updates the control's Font property based on ControlStyle
        /// </summary>
        public static void ApplyFontTheme(
            BeepRadioGroup radioGroup,
            BeepControlStyle controlStyle,
            IBeepTheme theme = null)
        {
            if (radioGroup == null)
            {
                return;
            }

            var itemFont = GetItemFont(controlStyle, isSelected: false, theme);
            if (itemFont != null)
            {
                radioGroup.TextFont = itemFont;
            }
        }
    }
}
