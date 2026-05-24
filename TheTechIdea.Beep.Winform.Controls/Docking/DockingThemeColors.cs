using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    internal sealed class DockingThemeColors
    {
        public Color HeaderBackColor { get; set; }
        public Color HeaderForeColor { get; set; }
        public Color HeaderButtonBackColor { get; set; }
        public Color HeaderButtonForeColor { get; set; }
        public Color PanelBackColor { get; set; }
        public Color PanelForeColor { get; set; }
        public Color ActiveTabBackColor { get; set; }
        public Color ActiveTabForeColor { get; set; }
        public Color InactiveTabBackColor { get; set; }
        public Color InactiveTabForeColor { get; set; }
        public Color TabBorderColor { get; set; }
        public Color HoverBackColor { get; set; }
        public Color SplitterBackColor { get; set; }
        public Color AutoHideStripBackColor { get; set; }
        public Color AutoHideTabBackColor { get; set; }
        public Color AutoHideActiveTabBackColor { get; set; }
        public Color SlidePanelBackColor { get; set; }
        public Color BorderColor { get; set; }
        public Color AccentColor { get; set; }

        public static DockingThemeColors Default => new DockingThemeColors
        {
            HeaderBackColor = Color.FromArgb(37, 37, 38),
            HeaderForeColor = Color.White,
            HeaderButtonBackColor = Color.Transparent,
            HeaderButtonForeColor = Color.White,
            PanelBackColor = SystemColors.Control,
            PanelForeColor = SystemColors.ControlText,
            ActiveTabBackColor = Color.FromArgb(0, 122, 204),
            ActiveTabForeColor = Color.White,
            InactiveTabBackColor = Color.FromArgb(45, 45, 48),
            InactiveTabForeColor = Color.White,
            TabBorderColor = Color.FromArgb(63, 63, 70),
            HoverBackColor = Color.FromArgb(62, 62, 66),
            SplitterBackColor = Color.FromArgb(60, 60, 65),
            AutoHideStripBackColor = Color.FromArgb(45, 45, 48),
            AutoHideTabBackColor = Color.FromArgb(60, 60, 65),
            AutoHideActiveTabBackColor = Color.FromArgb(0, 122, 204),
            SlidePanelBackColor = SystemColors.Control,
            BorderColor = Color.FromArgb(63, 63, 70),
            AccentColor = Color.FromArgb(0, 122, 204)
        };

        public static DockingThemeColors FromTheme(IBeepTheme theme, bool useThemeColors)
        {
            var fallback = Default;
            if (!useThemeColors || theme == null)
                return fallback;

            Color panelBack = FirstValid(
                fallback.PanelBackColor,
                theme.PanelBackColor,
                theme.SurfaceColor,
                theme.BackColor,
                theme.BackgroundColor);

            Color panelFore = EnsureReadable(FirstValid(
                GetReadableTextColor(panelBack),
                theme.ForeColor,
                theme.OnBackgroundColor), panelBack);

            Color headerBack = FirstValid(
                fallback.HeaderBackColor,
                theme.AppBarBackColor,
                theme.TabBackColor,
                theme.NavigationBackColor,
                theme.PanelBackColor,
                theme.SurfaceColor);

            Color activeTabBack = FirstValid(
                fallback.ActiveTabBackColor,
                theme.ActiveTabBackColor,
                theme.TabSelectedBackColor,
                theme.NavigationSelectedBackColor,
                theme.PrimaryColor,
                theme.AccentColor,
                theme.HighlightBackColor);

            Color inactiveTabBack = FirstValid(
                fallback.InactiveTabBackColor,
                theme.InactiveTabBackColor,
                theme.TabBackColor,
                headerBack);

            Color border = FirstValid(
                fallback.BorderColor,
                theme.TabBorderColor,
                theme.ActiveBorderColor,
                theme.BorderColor);

            Color hover = FirstValid(
                fallback.HoverBackColor,
                theme.TabHoverBackColor,
                theme.ButtonHoverBackColor,
                theme.NavigationHoverBackColor,
                theme.HighlightBackColor);

            Color headerFore = EnsureReadable(FirstValid(
                GetReadableTextColor(headerBack),
                theme.AppBarForeColor,
                theme.AppBarTitleForeColor,
                theme.TabForeColor,
                theme.ForeColor), headerBack);

            Color headerButtonFore = EnsureReadable(FirstValid(
                GetReadableTextColor(headerBack),
                theme.AppBarButtonForeColor,
                headerFore), headerBack);

            Color activeTabFore = EnsureReadable(FirstValid(
                GetReadableTextColor(activeTabBack),
                theme.ActiveTabForeColor,
                theme.TabSelectedForeColor), activeTabBack);

            Color inactiveTabFore = EnsureReadable(FirstValid(
                GetReadableTextColor(inactiveTabBack),
                theme.InactiveTabForeColor,
                theme.TabForeColor), inactiveTabBack);

            return new DockingThemeColors
            {
                HeaderBackColor = headerBack,
                HeaderForeColor = headerFore,
                HeaderButtonBackColor = FirstValid(Color.Transparent, theme.AppBarButtonBackColor),
                HeaderButtonForeColor = headerButtonFore,
                PanelBackColor = panelBack,
                PanelForeColor = panelFore,
                ActiveTabBackColor = activeTabBack,
                ActiveTabForeColor = activeTabFore,
                InactiveTabBackColor = inactiveTabBack,
                InactiveTabForeColor = inactiveTabFore,
                TabBorderColor = border,
                HoverBackColor = hover,
                SplitterBackColor = FirstValid(fallback.SplitterBackColor, theme.InactiveBorderColor, border),
                AutoHideStripBackColor = headerBack,
                AutoHideTabBackColor = inactiveTabBack,
                AutoHideActiveTabBackColor = activeTabBack,
                SlidePanelBackColor = panelBack,
                BorderColor = border,
                AccentColor = activeTabBack
            };
        }

        public static DockingThemeColors FromExplicit(Color background, Color foreground, Color border, Color hover, Color accent)
        {
            var fallback = Default;
            Color panelBack = FirstValid(fallback.PanelBackColor, background);
            Color panelFore = EnsureReadable(FirstValid(GetReadableTextColor(panelBack), foreground), panelBack);
            Color headerBack = FirstValid(fallback.HeaderBackColor, background);
            Color activeBack = FirstValid(fallback.ActiveTabBackColor, accent);
            Color inactiveBack = FirstValid(fallback.InactiveTabBackColor, Blend(panelBack, headerBack, 0.5f));
            Color resolvedBorder = FirstValid(fallback.BorderColor, border);
            Color headerButtonFore = GetReadableTextColor(headerBack);

            return new DockingThemeColors
            {
                HeaderBackColor = headerBack,
                HeaderForeColor = EnsureReadable(panelFore, headerBack),
                HeaderButtonBackColor = Color.Transparent,
                HeaderButtonForeColor = headerButtonFore,
                PanelBackColor = panelBack,
                PanelForeColor = panelFore,
                ActiveTabBackColor = activeBack,
                ActiveTabForeColor = EnsureReadable(panelFore, activeBack),
                InactiveTabBackColor = inactiveBack,
                InactiveTabForeColor = EnsureReadable(panelFore, inactiveBack),
                TabBorderColor = resolvedBorder,
                HoverBackColor = FirstValid(fallback.HoverBackColor, hover),
                SplitterBackColor = resolvedBorder,
                AutoHideStripBackColor = headerBack,
                AutoHideTabBackColor = inactiveBack,
                AutoHideActiveTabBackColor = activeBack,
                SlidePanelBackColor = panelBack,
                BorderColor = resolvedBorder,
                AccentColor = activeBack
            };
        }

        public static Color GetReadableTextColor(Color background)
        {
            if (!IsUsableColor(background))
                return SystemColors.ControlText;

            double luminance = (0.299 * background.R + 0.587 * background.G + 0.114 * background.B) / 255.0;
            return luminance > 0.55 ? Color.FromArgb(32, 32, 32) : Color.White;
        }

        private static Color FirstValid(Color fallback, params Color[] candidates)
        {
            foreach (Color candidate in candidates)
            {
                if (IsUsableColor(candidate))
                    return candidate;
            }

            return fallback;
        }

        private static bool IsUsableColor(Color color) =>
            !color.IsEmpty && color.A > 0;

        private static Color EnsureReadable(Color foreground, Color background)
        {
            if (!IsUsableColor(foreground))
                return GetReadableTextColor(background);

            if (ContrastRatio(foreground, background) < 3.0)
                return GetReadableTextColor(background);

            return foreground;
        }

        private static double ContrastRatio(Color foreground, Color background)
        {
            double l1 = RelativeLuminance(foreground) + 0.05;
            double l2 = RelativeLuminance(background) + 0.05;
            return Math.Max(l1, l2) / Math.Min(l1, l2);
        }

        private static double RelativeLuminance(Color color)
        {
            double Channel(byte value)
            {
                double v = value / 255.0;
                return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
            }

            return 0.2126 * Channel(color.R) + 0.7152 * Channel(color.G) + 0.0722 * Channel(color.B);
        }

        private static Color Blend(Color a, Color b, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            return Color.FromArgb(
                (int)(a.R + (b.R - a.R) * amount),
                (int)(a.G + (b.G - a.G) * amount),
                (int)(a.B + (b.B - a.B) * amount));
        }
    }
}
