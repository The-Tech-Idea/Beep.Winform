using System.Drawing;
using System.Windows.Forms; // Required for Control, if needed; though mainly using static helpers
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in chip controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class ChipFontHelpers
    {
        // Resolves a safe font family: prefer theme ButtonFont family, then style typography,
        // fall back to "Segoe UI" if the resolved name is not installed.
        private static string ResolveSafeFamily(BeepControlStyle controlStyle)
        {
            string family = BeepThemesManager.CurrentTheme?.ButtonFont?.FontFamily;
            if (string.IsNullOrWhiteSpace(family))
                family = StyleTypography.GetFontFamily(controlStyle)?.Split(',')[0].Trim();
            if (string.IsNullOrWhiteSpace(family) || !BeepFontManager.IsFontAvailable(family))
                family = "Segoe UI";
            return family;
        }

        public static Font GetChipFont(
            BeepControlStyle controlStyle,
            ChipSize chipSize,
            float dpiScale = 1.0f)
        {
            return GetChipFont(controlStyle, chipSize, dpiScale, null, null);
        }

        public static Font GetChipFont(
            BeepControlStyle controlStyle,
            ChipSize chipSize,
            float dpiScale,
            FontWeight? fontWeightOverride,
            FontStyle? fontStyleOverride)
        {
            float baseSize = BeepThemesManager.CurrentTheme?.ButtonFont?.FontSize > 0
                ? BeepThemesManager.CurrentTheme.ButtonFont.FontSize
                : StyleTypography.GetFontSize(controlStyle);

            float size = chipSize switch
            {
                ChipSize.Small  => baseSize * 0.85f,
                ChipSize.Large  => baseSize * 1.15f,
                _               => baseSize
            };
            size = Math.Max(6f, size);

            FontStyle fs = fontStyleOverride ?? FontStyle.Regular;
            if ((fontWeightOverride ?? FontWeight.Normal) >= FontWeight.Bold)
                fs |= FontStyle.Bold;

            return BeepFontManager.GetFont(ResolveSafeFamily(controlStyle), size, fs)
                   ?? BeepFontManager.DefaultFont;
        }

        public static Font GetTitleFont(
            BeepControlStyle controlStyle,
            float dpiScale = 1.0f)
        {
            float baseSize = BeepThemesManager.CurrentTheme?.TitleMedium?.FontSize > 0
                ? BeepThemesManager.CurrentTheme.TitleMedium.FontSize
                : StyleTypography.GetFontSize(controlStyle) * 1.2f;

            return BeepFontManager.GetFont(ResolveSafeFamily(controlStyle), Math.Max(6f, baseSize), FontStyle.Bold)
                   ?? BeepFontManager.DefaultFont;
        }

        public static Font GetAvatarFont(
            BeepControlStyle controlStyle,
            float targetFontSize,
            FontWeight fontWeight = FontWeight.Bold,
            FontStyle fontStyle = FontStyle.Bold)
        {
            float size = Math.Max(6f, targetFontSize);
            FontStyle fs = fontStyle;
            if (fontWeight >= FontWeight.Bold) fs |= FontStyle.Bold;

            return BeepFontManager.GetFont(ResolveSafeFamily(controlStyle), size, fs)
                   ?? BeepFontManager.DefaultFont;
        }

        public static Font GetIconFont(
            BeepControlStyle controlStyle,
            ChipSize chipSize,
            float dpiScale = 1.0f)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float size = chipSize switch
            {
                ChipSize.Small  => baseSize * 0.7f,
                ChipSize.Large  => baseSize * 0.9f,
                _               => baseSize * 0.8f
            };

            return BeepFontManager.GetFont(ResolveSafeFamily(controlStyle), Math.Max(6f, size), FontStyle.Regular)
                   ?? BeepFontManager.DefaultFont;
        }

        public static void ApplyFontTheme(
            Control control,
            BeepControlStyle controlStyle,
            ChipSize chipSize,
            float dpiScale = 1.0f)
        {
            if (control == null) return;
            control.Font = GetChipFont(controlStyle, chipSize, dpiScale);
        }
    }
}

