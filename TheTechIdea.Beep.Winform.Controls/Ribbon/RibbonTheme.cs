using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class RibbonTheme
    {
        public Color Background { get; set; }
        public Color TabActiveBack { get; set; }
        public Color TabInactiveBack { get; set; }
        public Color TabBorder { get; set; }
        public Color GroupBack { get; set; }
        public Color GroupBorder { get; set; }
        public Color HoverBack { get; set; }
        public Color PressedBack { get; set; }
        public Color FocusBorder { get; set; }
        public Color Separator { get; set; }
        public Color Text { get; set; }
        public Color IconColor { get; set; }
        public Color QuickAccessBack { get; set; }
        public Color QuickAccessBorder { get; set; }
        public Color DisabledBack { get; set; }
        public Color DisabledText { get; set; }
        public Color DisabledBorder { get; set; }
        public Color SelectionBack { get; set; }
        public Color ElevationColor { get; set; }

        // Layout tokens
        public int CornerRadius { get; set; }
        public int GroupSpacing { get; set; }
        public int ItemSpacing { get; set; }
        public int ElevationLevel { get; set; }
        public int ElevationStrongLevel { get; set; }
        public float FocusBorderThickness { get; set; }

        // Typography tokens consumed through BeepThemesManager.ToFont(...)
        public TypographyStyle TabTypography { get; set; }
        public TypographyStyle GroupTypography { get; set; }
        public TypographyStyle CommandTypography { get; set; }
        public TypographyStyle ContextHeaderTypography { get; set; }

        public RibbonTheme()
        {
            ApplyFallbackDefaults();
            TryApplyCurrentThemeDefaults();
        }

        private void ApplyFallbackDefaults()
        {
            Background = Helpers.ColorUtils.MapSystemColor(SystemColors.ControlLight);
            Text = Helpers.ColorUtils.MapSystemColor(SystemColors.ControlText);
            IconColor = Text;

            TabActiveBack = Color.White;
            TabInactiveBack = Color.FromArgb(235, 235, 235);
            TabBorder = Color.FromArgb(180, 180, 180);

            GroupBack = Helpers.ColorUtils.ShiftLuminance(Background, 0.15f);
            GroupBorder = Helpers.ColorUtils.ShiftLuminance(TabBorder, -0.1f);

            HoverBack = Color.FromArgb(236, 246, 255);
            PressedBack = Color.FromArgb(217, 234, 252);
            FocusBorder = Color.FromArgb(0, 120, 215);
            Separator = Color.FromArgb(210, 210, 210);

            QuickAccessBack = Helpers.ColorUtils.ShiftLuminance(Background, 0.2f);
            QuickAccessBorder = Helpers.ColorUtils.ShiftLuminance(TabBorder, -0.2f);

            DisabledBack = Color.FromArgb(232, 232, 232);
            DisabledText = Color.FromArgb(156, 156, 156);
            DisabledBorder = Color.FromArgb(194, 194, 194);
            SelectionBack = Color.FromArgb(206, 227, 252);
            ElevationColor = Color.FromArgb(42, 0, 0, 0);

            CornerRadius = 6;
            GroupSpacing = 8;
            ItemSpacing = 4;
            ElevationLevel = 1;
            ElevationStrongLevel = 2;
            FocusBorderThickness = 1f;

            TabTypography = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 9f,
                FontWeight = FontWeight.SemiBold
            };
            GroupTypography = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 8f,
                FontWeight = FontWeight.Medium
            };
            CommandTypography = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 8.5f,
                FontWeight = FontWeight.Normal
            };
            ContextHeaderTypography = new TypographyStyle
            {
                FontFamily = "Segoe UI",
                FontSize = 8.5f,
                FontWeight = FontWeight.SemiBold
            };
        }

        private void TryApplyCurrentThemeDefaults()
        {
            try
            {
                var current = BeepThemesManager.CurrentTheme;
                if (current == null)
                {
                    return;
                }

                Background = current.AppBarBackColor;
                TabActiveBack = current.ButtonBackColor;
                TabInactiveBack = current.InactiveTabBackColor;
                TabBorder = current.TabBorderColor;
                GroupBack = current.MenuBackColor;
                GroupBorder = current.MenuBorderColor;
                HoverBack = current.ButtonHoverBackColor;
                PressedBack = current.ButtonPressedBackColor;
                FocusBorder = current.FocusIndicatorColor;
                Separator = current.BorderColor;
                Text = current.AppBarTitleForeColor;
                IconColor = current.ButtonForeColor;
                QuickAccessBack = current.AppBarBackColor;
                QuickAccessBorder = current.BorderColor;
                DisabledBack = current.DisabledBackColor;
                DisabledText = current.DisabledForeColor;
                DisabledBorder = current.DisabledBorderColor;
                SelectionBack = current.ButtonSelectedBackColor;
                ElevationColor = Color.FromArgb(42, current.ShadowColor);

                CornerRadius = Math.Max(2, current.BorderRadius);
                GroupSpacing = Math.Max(2, current.PaddingMedium);
                ItemSpacing = Math.Max(1, current.PaddingSmall);
                ElevationLevel = Math.Clamp(current.BorderSize, 0, 4);
                ElevationStrongLevel = Math.Clamp(ElevationLevel + 1, 1, 6);
                FocusBorderThickness = Math.Max(1f, current.BorderSize);

                TabTypography = CloneTypography(current.TabFont, TabTypography);
                GroupTypography = CloneTypography(current.MenuTitleFont, GroupTypography);
                CommandTypography = CloneTypography(current.ButtonFont, CommandTypography);
                ContextHeaderTypography = CloneTypography(current.AppBarTitleStyle, ContextHeaderTypography);
            }
            catch
            {
                // keep fallback defaults if theme manager is unavailable during early startup
            }
        }

        private static TypographyStyle CloneTypography(TypographyStyle? source, TypographyStyle fallback)
        {
            var s = source ?? fallback;
            return new TypographyStyle
            {
                FontFamily = string.IsNullOrWhiteSpace(s.FontFamily) ? fallback.FontFamily : s.FontFamily,
                FontSize = s.FontSize <= 0 ? fallback.FontSize : s.FontSize,
                FontWeight = s.FontWeight,
                FontStyle = s.FontStyle,
                IsUnderlined = s.IsUnderlined,
                IsStrikeout = s.IsStrikeout,
                TextColor = s.TextColor,
                LineHeight = s.LineHeight,
                LetterSpacing = s.LetterSpacing
            };
        }
    }
}
