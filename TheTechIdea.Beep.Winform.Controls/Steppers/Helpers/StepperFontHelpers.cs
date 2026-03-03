using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Helpers
{
    public enum StepperFontElement
    {
        StepNumber,
        StepLabel,
        StepText,
        Connector
    }

    public static class StepperFontHelpers
    {
        public static Font GetStepNumberFont(dynamic stepper, BeepControlStyle controlStyle, Font textFont = null, Control ownerControl = null)
        {
            IBeepTheme theme = ResolveTheme(stepper);
            TypographyStyle style =
                ResolveTypography(theme, "StepperSelectedFont")
                ?? ResolveTypography(theme, "StepperItemFont")
                ?? ResolveTypography(theme, "LabelMedium");

            return ResolveFont(style, ownerControl, textFont);
        }

        public static Font GetStepLabelFont(dynamic stepper, BeepControlStyle controlStyle, StepState state = StepState.Pending, Font textFont = null, Control ownerControl = null)
        {
            IBeepTheme theme = ResolveTheme(stepper);
            TypographyStyle style = state == StepState.Active
                ? ResolveTypography(theme, "StepperSelectedFont")
                : ResolveTypography(theme, "StepperUnSelectedFont");

            style ??= ResolveTypography(theme, "StepperTitleFont");
            style ??= ResolveTypography(theme, "StepperItemFont");
            style ??= ResolveTypography(theme, "BodyMedium");

            return ResolveFont(style, ownerControl, textFont);
        }

        public static Font GetStepTextFont(dynamic stepper, BeepControlStyle controlStyle, Font textFont = null, Control ownerControl = null)
        {
            IBeepTheme theme = ResolveTheme(stepper);
            TypographyStyle style =
                ResolveTypography(theme, "StepperSubTitleFont")
                ?? ResolveTypography(theme, "BodySmall");

            return ResolveFont(style, ownerControl, textFont);
        }

        public static Font GetStepperFont(dynamic stepper, BeepControlStyle controlStyle, Font textFont = null)
        {
            IBeepTheme theme = ResolveTheme(stepper);
            TypographyStyle style =
                ResolveTypography(theme, "StepperItemFont")
                ?? ResolveTypography(theme, "BodyMedium");

            return ResolveFont(style, stepper as Control, textFont);
        }

        public static int GetFontSizeForElement(BeepControlStyle controlStyle, StepperFontElement element, Control ownerControl = null)
        {
            int baseSize = element switch
            {
                StepperFontElement.StepNumber => 10,
                StepperFontElement.StepLabel => 9,
                StepperFontElement.StepText => 9,
                StepperFontElement.Connector => 8,
                _ => 9
            };

            int minSize = ownerControl != null ? DpiScalingHelper.ScaleValue(8, ownerControl) : 8;
            int maxSize = ownerControl != null ? DpiScalingHelper.ScaleValue(16, ownerControl) : 16;
            return Math.Max(minSize, Math.Min(maxSize, baseSize));
        }

        public static FontStyle GetFontStyleForElement(BeepControlStyle controlStyle, StepperFontElement element)
        {
            return element == StepperFontElement.StepNumber ? FontStyle.Bold : FontStyle.Regular;
        }

        public static void ApplyFontTheme(dynamic stepper, BeepControlStyle controlStyle, Font textFont = null)
        {
            // Intentionally no-op.
            // Fonts are resolved in control ApplyTheme and used through local theme-font fields.
        }

        private static Font ResolveFont(TypographyStyle style, Control ownerControl, Font fallback)
        {
            if (style != null)
            {
                return ownerControl != null
                    ? BeepThemesManager.ToFontForControl(style, ownerControl)
                    : BeepThemesManager.ToFont(style);
            }

            return fallback ?? BeepThemesManager.ToFont((TypographyStyle)null) ?? SystemFonts.DefaultFont;
        }

        private static IBeepTheme ResolveTheme(dynamic stepper)
        {
            if (stepper == null)
            {
                return null;
            }

            object currentThemeObj = GetPropertyValue<object>(stepper, "_currentTheme")
                ?? GetPropertyValue<object>(stepper, "CurrentTheme");
            return currentThemeObj as IBeepTheme;
        }

        private static TypographyStyle ResolveTypography(IBeepTheme theme, string propertyName)
        {
            if (theme == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            var property = theme.GetType().GetProperty(propertyName);
            if (property != null && property.GetValue(theme) is TypographyStyle style)
            {
                return style;
            }

            var interfaceProperty = typeof(IBeepTheme).GetProperty(propertyName);
            if (interfaceProperty != null && interfaceProperty.GetValue(theme) is TypographyStyle interfaceStyle)
            {
                return interfaceStyle;
            }

            return null;
        }

        private static T GetPropertyValue<T>(dynamic obj, string propertyName)
        {
            try
            {
                var property = obj.GetType().GetProperty(propertyName,
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic);
                if (property != null)
                {
                    var value = property.GetValue(obj);
                    if (value is T typedValue)
                    {
                        return typedValue;
                    }
                }
            }
            catch
            {
                // Ignore and use fallback
            }

            return default;
        }
    }
}

