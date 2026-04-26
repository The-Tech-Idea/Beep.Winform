using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    public static class RibbonThemeMapper
    {
        public static RibbonTheme Map(IBeepTheme? theme, bool darkMode)
        {
            return Map(theme, darkMode, BeepThemesManager.CurrentStyle);
        }

        public static RibbonTheme Map(IBeepTheme? theme, bool darkMode, FormStyle formStyle)
        {
            var t = new RibbonTheme();
            var preset = GetPreset(formStyle, darkMode);
            if (theme == null)
            {
                if (darkMode)
                {
                    ApplyDark(t);
                }
                ApplyFormStyleFlavor(t, formStyle, darkMode, null);
                ApplyPresetFlavor(t, preset, darkMode);
                return t;
            }

            if (darkMode)
            {
                ApplyDark(t);
                // keep text from theme if available
                t.Text = theme.AppBarTitleForeColor;
                t.IconColor = theme.AppBarButtonForeColor;
                t.HoverBack = ShiftLuminance(theme.AppBarBackColor, .08f);
                t.PressedBack = ShiftLuminance(theme.AppBarBackColor, .16f);
                t.SelectionBack = theme.ButtonSelectedBackColor;
                t.FocusBorder = theme.FocusIndicatorColor;
                t.Separator = theme.BorderColor;
                t.CornerRadius = Math.Max(2, theme.BorderRadius);
                t.GroupSpacing = Math.Max(2, theme.PaddingMedium);
                t.ItemSpacing = Math.Max(1, theme.PaddingSmall);
                t.DisabledBack = theme.DisabledBackColor;
                t.DisabledText = theme.DisabledForeColor;
                t.DisabledBorder = theme.DisabledBorderColor;
                t.ElevationColor = WithAlpha(theme.ShadowColor, theme.ShadowOpacity, 42);
                t.ElevationLevel = Math.Clamp(theme.BorderSize, 0, 4);
                t.ElevationStrongLevel = Math.Clamp(t.ElevationLevel + 1, 1, 6);
                t.FocusBorderThickness = Math.Max(1f, theme.BorderSize);
                t.TabTypography = CloneTypography(theme.TabFont, t.TabTypography);
                t.GroupTypography = CloneTypography(theme.MenuTitleFont, t.GroupTypography);
                t.CommandTypography = CloneTypography(theme.ButtonFont, t.CommandTypography);
                t.ContextHeaderTypography = CloneTypography(theme.AppBarTitleStyle, t.ContextHeaderTypography);
                ApplyFormStyleFlavor(t, formStyle, darkMode, theme);
                ApplyPresetFlavor(t, preset, darkMode);
                return t;
            }

            // Light mapping from Beep theme
            t.Background = theme.AppBarBackColor;
            t.TabActiveBack = theme.ButtonBackColor;
            t.TabInactiveBack = ShiftLuminance(theme.AppBarBackColor, .1f);
            t.TabBorder = theme.BorderColor;
            t.GroupBack = ShiftLuminance(theme.AppBarBackColor, .15f);
            t.GroupBorder = ShiftLuminance(theme.BorderColor, -0.25f);
            t.HoverBack = theme.ButtonHoverBackColor;
            t.PressedBack = theme.ButtonPressedBackColor;
            t.SelectionBack = theme.ButtonSelectedBackColor;
            t.FocusBorder = theme.FocusIndicatorColor;
            t.Separator = theme.BorderColor;
            t.CornerRadius = Math.Max(2, theme.BorderRadius);
            t.GroupSpacing = Math.Max(2, theme.PaddingMedium);
            t.ItemSpacing = Math.Max(1, theme.PaddingSmall);
            t.Text = theme.AppBarTitleForeColor;
            t.IconColor = theme.ButtonForeColor;
            t.QuickAccessBack = ShiftLuminance(theme.AppBarBackColor, .2f);
            t.QuickAccessBorder = ShiftLuminance(theme.BorderColor, -0.25f);
            t.DisabledBack = theme.DisabledBackColor;
            t.DisabledText = theme.DisabledForeColor;
            t.DisabledBorder = theme.DisabledBorderColor;
            t.ElevationColor = WithAlpha(theme.ShadowColor, theme.ShadowOpacity, 34);
            t.ElevationLevel = Math.Clamp(theme.BorderSize, 0, 3);
            t.ElevationStrongLevel = Math.Clamp(t.ElevationLevel + 1, 1, 5);
            t.FocusBorderThickness = Math.Max(1f, theme.BorderSize);
            t.TabTypography = CloneTypography(theme.TabFont, t.TabTypography);
            t.GroupTypography = CloneTypography(theme.MenuTitleFont, t.GroupTypography);
            t.CommandTypography = CloneTypography(theme.ButtonFont, t.CommandTypography);
            t.ContextHeaderTypography = CloneTypography(theme.AppBarTitleStyle, t.ContextHeaderTypography);
            ApplyFormStyleFlavor(t, formStyle, darkMode, theme);
            ApplyPresetFlavor(t, preset, darkMode);
            return t;
        }

        public static RibbonStylePreset GetPreset(FormStyle formStyle, bool darkMode)
        {
            if (IsHighContrastStyle(formStyle))
            {
                return RibbonStylePreset.HighContrast;
            }

            if (IsFluentFamily(formStyle))
            {
                return darkMode ? RibbonStylePreset.FluentDark : RibbonStylePreset.FluentLight;
            }

            return darkMode ? RibbonStylePreset.OfficeDark : RibbonStylePreset.OfficeLight;
        }

        private static void ApplyDark(RibbonTheme t)
        {
            t.Background = ColorUtils.MapSystemColor(SystemColors.Control);
            t.TabActiveBack = Color.FromArgb(45, 45, 45);
            t.TabInactiveBack = ColorUtils.ShiftLuminance(t.Background, 0.06f);
            t.TabBorder = ColorUtils.MapSystemColor(SystemColors.ControlDark);
            t.GroupBack = ColorUtils.ShiftLuminance(t.Background, 0.04f);
            t.GroupBorder = ColorUtils.ShiftLuminance(t.TabBorder, 0.1f);
            t.HoverBack = ColorUtils.ShiftLuminance(t.Background, 0.12f);
            t.PressedBack = ColorUtils.ShiftLuminance(t.Background, 0.18f);
            t.FocusBorder = Color.FromArgb(120, 170, 230);
            t.Separator = ColorUtils.ShiftLuminance(t.TabBorder, 0.05f);
            t.Text = ColorUtils.MapSystemColor(SystemColors.WindowText);
            t.IconColor = ColorUtils.MapSystemColor(SystemColors.WindowText);
            t.QuickAccessBack = ColorUtils.ShiftLuminance(t.Background, 0.15f);
            t.QuickAccessBorder = ColorUtils.ShiftLuminance(t.TabBorder, 0.15f);
            t.DisabledBack = ColorUtils.ShiftLuminance(t.Background, 0.2f);
            t.DisabledText = Color.FromArgb(128, 128, 128);
            t.DisabledBorder = ColorUtils.ShiftLuminance(t.TabBorder, 0.05f);
            t.SelectionBack = Color.FromArgb(66, 102, 142);
            t.ElevationColor = Color.FromArgb(52, 0, 0, 0);
            t.ElevationLevel = 1;
            t.ElevationStrongLevel = 2;
            t.FocusBorderThickness = 1.2f;
        }

        private static void ApplyFormStyleFlavor(RibbonTheme target, FormStyle formStyle, bool darkMode, IBeepTheme? sourceTheme)
        {
            var controlStyle = BeepStyling.GetControlStyle(formStyle);
            var styleBack = StyleColors.GetBackground(controlStyle);
            var styleSurface = StyleColors.GetSecondary(controlStyle);
            var styleText = StyleColors.GetForeground(controlStyle);
            var styleBorder = StyleColors.GetBorder(controlStyle);
            var styleHover = StyleColors.GetHover(controlStyle);
            var styleSelection = StyleColors.GetSelection(controlStyle);
            var stylePrimary = StyleColors.GetPrimary(controlStyle);

            if (!darkMode)
            {
                target.Background = Blend(target.Background, styleBack, 0.42f);
                target.TabActiveBack = Blend(target.TabActiveBack, styleSurface, 0.55f);
                target.TabInactiveBack = Blend(target.TabInactiveBack, styleBack, 0.55f);
                target.GroupBack = Blend(target.GroupBack, styleSurface, 0.60f);
            }
            else
            {
                var darkBack = ToneForDark(styleBack);
                var darkSurface = ToneForDark(styleSurface);
                target.Background = Blend(target.Background, darkBack, 0.42f);
                target.TabActiveBack = Blend(target.TabActiveBack, darkSurface, 0.45f);
                target.TabInactiveBack = Blend(target.TabInactiveBack, darkBack, 0.35f);
                target.GroupBack = Blend(target.GroupBack, darkSurface, 0.45f);
                styleText = Color.WhiteSmoke;
            }

            var accent = stylePrimary;
            if (sourceTheme != null)
            {
                accent = Blend(stylePrimary, sourceTheme.AccentColor, 0.40f);
            }

            target.FocusBorder = accent;
            target.TabBorder = Blend(target.TabBorder, styleBorder, 0.50f);
            target.GroupBorder = Blend(target.GroupBorder, styleBorder, 0.55f);
            target.Separator = Blend(target.Separator, styleBorder, 0.45f);
            target.HoverBack = Blend(target.HoverBack, styleHover, darkMode ? 0.40f : 0.65f);
            target.PressedBack = Blend(target.PressedBack, styleSelection, darkMode ? 0.45f : 0.70f);
            target.SelectionBack = Blend(target.SelectionBack, styleSelection, darkMode ? 0.55f : 0.75f);
            target.QuickAccessBack = Blend(target.QuickAccessBack, styleSurface, darkMode ? 0.30f : 0.55f);
            target.QuickAccessBorder = Blend(target.QuickAccessBorder, accent, 0.55f);
            target.Text = Blend(target.Text, styleText, 0.35f);
            target.IconColor = Blend(target.IconColor, styleText, 0.30f);
            target.DisabledBack = Blend(target.DisabledBack, styleSurface, darkMode ? 0.25f : 0.45f);
            target.DisabledText = Blend(target.DisabledText, styleText, darkMode ? 0.40f : 0.28f);
            target.DisabledBorder = Blend(target.DisabledBorder, styleBorder, 0.45f);
            target.ElevationColor = Blend(target.ElevationColor, styleBorder, darkMode ? 0.12f : 0.08f);

            ApplyStyleMetrics(target, formStyle);
        }

        private static void ApplyStyleMetrics(RibbonTheme target, FormStyle formStyle)
        {
            switch (formStyle)
            {
                case FormStyle.Minimal:
                case FormStyle.Shadcn:
                case FormStyle.NextJS:
                case FormStyle.Linear:
                    target.CornerRadius = 2;
                    target.GroupSpacing = Math.Max(2, target.GroupSpacing - 2);
                    target.ItemSpacing = Math.Max(1, target.ItemSpacing - 1);
                    target.ElevationLevel = 0;
                    target.ElevationStrongLevel = 1;
                    break;
                case FormStyle.Modern:
                case FormStyle.Fluent:
                case FormStyle.Material:
                case FormStyle.MaterialYou:
                case FormStyle.iOS:
                    target.CornerRadius = Math.Max(target.CornerRadius, 8);
                    target.ElevationLevel = Math.Max(target.ElevationLevel, 1);
                    target.ElevationStrongLevel = Math.Max(target.ElevationStrongLevel, 2);
                    break;
                case FormStyle.MacOS:
                case FormStyle.Glass:
                case FormStyle.Glassmorphism:
                case FormStyle.NeoMorphism:
                    target.CornerRadius = Math.Max(target.CornerRadius, 10);
                    target.ElevationLevel = Math.Max(target.ElevationLevel, 2);
                    target.ElevationStrongLevel = Math.Max(target.ElevationStrongLevel, 3);
                    break;
                case FormStyle.Brutalist:
                    target.CornerRadius = 0;
                    target.GroupSpacing = Math.Max(8, target.GroupSpacing);
                    target.ItemSpacing = Math.Max(4, target.ItemSpacing);
                    target.ElevationLevel = 0;
                    target.ElevationStrongLevel = 0;
                    break;
                case FormStyle.Terminal:
                    target.CornerRadius = 0;
                    target.GroupSpacing = Math.Max(2, target.GroupSpacing - 2);
                    target.ItemSpacing = Math.Max(1, target.ItemSpacing - 1);
                    target.ElevationLevel = 0;
                    target.ElevationStrongLevel = 0;
                    break;
            }
        }

        private static void ApplyPresetFlavor(RibbonTheme target, RibbonStylePreset preset, bool darkMode)
        {
            switch (preset)
            {
                case RibbonStylePreset.OfficeLight:
                case RibbonStylePreset.OfficeDark:
                    target.CornerRadius = Math.Clamp(target.CornerRadius, 4, 6);
                    target.GroupSpacing = Math.Max(6, target.GroupSpacing);
                    target.ItemSpacing = Math.Max(2, target.ItemSpacing);
                    target.ElevationLevel = Math.Max(target.ElevationLevel, 1);
                    target.ElevationStrongLevel = Math.Max(target.ElevationStrongLevel, 2);
                    target.FocusBorderThickness = Math.Max(1f, target.FocusBorderThickness);
                    target.HoverBack = Blend(target.HoverBack, target.SelectionBack, darkMode ? 0.22f : 0.16f);
                    break;

                case RibbonStylePreset.FluentLight:
                case RibbonStylePreset.FluentDark:
                    target.CornerRadius = Math.Max(8, target.CornerRadius);
                    target.GroupSpacing = Math.Max(8, target.GroupSpacing);
                    target.ItemSpacing = Math.Max(3, target.ItemSpacing);
                    target.ElevationLevel = Math.Max(target.ElevationLevel, 1);
                    target.ElevationStrongLevel = Math.Max(target.ElevationStrongLevel, 3);
                    target.FocusBorderThickness = Math.Max(1.2f, target.FocusBorderThickness);
                    target.HoverBack = Blend(target.HoverBack, target.SelectionBack, darkMode ? 0.30f : 0.22f);
                    target.PressedBack = Blend(target.PressedBack, target.FocusBorder, darkMode ? 0.32f : 0.24f);
                    target.QuickAccessBack = Blend(target.QuickAccessBack, target.Background, darkMode ? 0.40f : 0.20f);
                    break;

                case RibbonStylePreset.HighContrast:
                    target.Background = darkMode ? Color.Black : Color.White;
                    target.TabActiveBack = darkMode ? Color.Black : Color.White;
                    target.TabInactiveBack = darkMode ? Color.Black : Color.White;
                    target.GroupBack = darkMode ? Color.Black : Color.White;
                    target.Text = darkMode ? Color.White : Color.Black;
                    target.IconColor = target.Text;
                    target.TabBorder = target.Text;
                    target.GroupBorder = target.Text;
                    target.Separator = target.Text;
                    target.FocusBorder = darkMode ? Color.Yellow : Color.Blue;
                    target.HoverBack = Blend(target.Background, target.FocusBorder, 0.22f);
                    target.PressedBack = Blend(target.Background, target.FocusBorder, 0.35f);
                    target.SelectionBack = Blend(target.Background, target.FocusBorder, 0.30f);
                    target.DisabledBack = darkMode ? Color.FromArgb(28, 28, 28) : Color.FromArgb(230, 230, 230);
                    target.DisabledText = darkMode ? Color.FromArgb(180, 180, 180) : Color.FromArgb(92, 92, 92);
                    target.DisabledBorder = target.Text;
                    target.CornerRadius = 0;
                    target.ElevationLevel = 0;
                    target.ElevationStrongLevel = 0;
                    target.FocusBorderThickness = Math.Max(2f, target.FocusBorderThickness);
                    break;
            }
        }

        private static bool IsHighContrastStyle(FormStyle formStyle)
        {
            return formStyle is FormStyle.Terminal;
        }

        private static bool IsFluentFamily(FormStyle formStyle)
        {
            return formStyle is FormStyle.Fluent or FormStyle.Modern or FormStyle.Minimal or FormStyle.Metro or FormStyle.Metro2 or FormStyle.Shadcn or FormStyle.RadixUI or FormStyle.NextJS or FormStyle.Linear;
        }

        private static Color ToneForDark(Color color)
        {
            return color.GetBrightness() > 0.45f ? Blend(color, Color.Black, 0.62f) : color;
        }

        private static Color Blend(Color baseColor, Color overlay, float amount)
        {
            float t = Math.Clamp(amount, 0f, 1f);
            byte a = (byte)Math.Clamp(baseColor.A + ((overlay.A - baseColor.A) * t), 0, 255);
            byte r = (byte)Math.Clamp(baseColor.R + ((overlay.R - baseColor.R) * t), 0, 255);
            byte g = (byte)Math.Clamp(baseColor.G + ((overlay.G - baseColor.G) * t), 0, 255);
            byte b = (byte)Math.Clamp(baseColor.B + ((overlay.B - baseColor.B) * t), 0, 255);
            return Color.FromArgb(a, r, g, b);
        }

        private static TypographyStyle CloneTypography(TypographyStyle? source, TypographyStyle fallback)
        {
            var s = source ?? fallback;
            return new TypographyStyle
            {
                FontFamily = string.IsNullOrWhiteSpace(s.FontFamily) ? fallback.FontFamily : s.FontFamily,
                FontSize = s.FontSize <= 0 ? fallback.FontSize : s.FontSize,
                LineHeight = s.LineHeight,
                LetterSpacing = s.LetterSpacing,
                FontWeight = s.FontWeight,
                FontStyle = s.FontStyle,
                TextColor = s.TextColor,
                IsUnderlined = s.IsUnderlined,
                IsStrikeout = s.IsStrikeout
            };
        }

        private static Color WithAlpha(Color color, float opacity, byte fallbackAlpha)
        {
            byte alpha;
            if (opacity > 0f && opacity <= 1f)
            {
                alpha = (byte)Math.Clamp((int)(opacity * 255f), 0, 255);
            }
            else
            {
                alpha = fallbackAlpha;
            }

            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        private static Color ShiftLuminance(Color color, float amount)
        {
            return ColorUtils.ShiftLuminance(color, amount);
        }
    }
}
