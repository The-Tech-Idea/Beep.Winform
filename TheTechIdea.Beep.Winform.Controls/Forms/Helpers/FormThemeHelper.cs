using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Helper class for managing theme integration with different UI styles
    /// </summary>
    public class FormThemeHelper
    {
        private readonly BeepFormAdvanced _form;
        private readonly FormComponentsAccessor _components;
        
        public FormThemeHelper(BeepFormAdvanced form, FormComponentsAccessor components)
        {
            _form = form;
            _components = components;
        }

        public FormColorScheme ApplyThemeToStyle(FormUIStyle style, IBeepTheme theme)
        {
            if (theme == null) return GetDefaultColorScheme(style);

            var colorScheme = new FormColorScheme();

            switch (style)
            {
                case FormUIStyle.Mac:
                    ApplyThemeToMacStyle(theme, colorScheme);
                    break;
                case FormUIStyle.Modern:
                    ApplyThemeToModernStyle(theme, colorScheme);
                    break;
                case FormUIStyle.Classic:
                    ApplyThemeToClassicStyle(theme, colorScheme);
                    break;
                case FormUIStyle.Minimal:
                    ApplyThemeToMinimalStyle(theme, colorScheme);
                    break;
                case FormUIStyle.Material:
                    ApplyThemeToMaterialStyle(theme, colorScheme);
                    break;
                case FormUIStyle.Fluent:
                    ApplyThemeToFluentStyle(theme, colorScheme);
                    break;
                case FormUIStyle.Ribbon:
                    ApplyThemeToRibbonStyle(theme, colorScheme);
                    break;
                case FormUIStyle.Mobile:
                    ApplyThemeToMobileStyle(theme, colorScheme);
                    break;
                case FormUIStyle.Console:
                    ApplyThemeToConsoleStyle(theme, colorScheme);
                    break;
                case FormUIStyle.Floating:
                    ApplyThemeToFloatingStyle(theme, colorScheme);
                    break;
                default:
                    ApplyThemeToModernStyle(theme, colorScheme);
                    break;
            }

            return colorScheme;
        }

        public void ApplyThemeToButtons(FormUIStyle style, IBeepTheme theme, string themeName)
        {
            if (style == FormUIStyle.Mac)
            {
                // Mac: Keep traffic light colors unchanged
                return;
            }
            
            if (_components.BtnMin != null && _components.BtnMin is BeepButton minBeepButton)
            {
                minBeepButton.Theme = themeName;
                if (style != FormUIStyle.Console)
                {
                    minBeepButton.ForeColor = theme.ButtonForeColor;
                    minBeepButton.HoverBackColor = theme.ButtonHoverBackColor;
                }
            }
            
            if (_components.BtnMax != null && _components.BtnMax is BeepButton maxBeepButton)
            {
                maxBeepButton.Theme = themeName;
                if (style != FormUIStyle.Console)
                {
                    maxBeepButton.ForeColor = theme.ButtonForeColor;
                    maxBeepButton.HoverBackColor = theme.ButtonHoverBackColor;
                }
            }
            
            if (_components.BtnClose != null && _components.BtnClose is BeepButton closeBeepButton)
            {
                closeBeepButton.Theme = themeName;
                if (style != FormUIStyle.Console && style != FormUIStyle.Mac)
                {
                    closeBeepButton.ForeColor = theme.ButtonForeColor;
                    closeBeepButton.HoverBackColor = theme.ErrorColor;
                }
            }
        }

        public void ApplyThemeToControls(IBeepTheme theme, string themeName)
        {
            if (_components.TitleBar != null && _components.TitleBar is BeepPanel titleBeepPanel)
            {
                titleBeepPanel.Theme = themeName;
            }
            if (_components.TitleLabel != null && _components.TitleLabel is BeepLabel titleBeepLabel)
            {
                titleBeepLabel.Theme = themeName;
            }
            if (_components.ContentHost != null && _components.ContentHost is BeepPanel contentBeepPanel)
            {
                contentBeepPanel.Theme = themeName;
            }
            if (_components.StatusBar != null && _components.StatusBar is BeepPanel statusBeepPanel)
            {
                statusBeepPanel.Theme = themeName;
            }
            if (_components.StatusLabel != null && _components.StatusLabel is BeepLabel statusBeepLabel)
            {
                statusBeepLabel.Theme = themeName;
            }
        }

        #region Theme Application Methods
        private void ApplyThemeToMacStyle(IBeepTheme theme, FormColorScheme colorScheme)
        {
            colorScheme.TitleBarColor = theme.BackgroundColor;
            colorScheme.TitleBarGradientStart = theme.GradientStartColor;
            colorScheme.TitleBarGradientEnd = theme.GradientEndColor;
            colorScheme.TitleTextColor = theme.PrimaryTextColor;
            colorScheme.ContentBackColor = theme.SurfaceColor;
            colorScheme.StatusBarColor = theme.BackgroundColor;
            colorScheme.BorderColor = theme.BorderColor;
        }

        private void ApplyThemeToModernStyle(IBeepTheme theme, FormColorScheme colorScheme)
        {
            colorScheme.TitleBarColor = theme.BackgroundColor;
            colorScheme.TitleBarGradientStart = theme.GradientStartColor;
            colorScheme.TitleBarGradientEnd = theme.GradientEndColor;
            colorScheme.TitleTextColor = theme.OnBackgroundColor;
            colorScheme.ContentBackColor = theme.SurfaceColor;
            colorScheme.StatusBarColor = theme.BackgroundColor;
            colorScheme.BorderColor = theme.BorderColor;
        }

        private void ApplyThemeToClassicStyle(IBeepTheme theme, FormColorScheme colorScheme)
        {
            colorScheme.TitleBarColor = theme.PrimaryColor;
            colorScheme.TitleBarGradientStart = theme.GradientStartColor;
            colorScheme.TitleBarGradientEnd = theme.GradientEndColor;
            colorScheme.TitleTextColor = theme.OnPrimaryColor;
            colorScheme.ContentBackColor = theme.SurfaceColor;
            colorScheme.StatusBarColor = theme.BackgroundColor;
            colorScheme.BorderColor = theme.BorderColor;
        }

        private void ApplyThemeToMinimalStyle(IBeepTheme theme, FormColorScheme colorScheme)
        {
            colorScheme.TitleBarColor = theme.SurfaceColor;
            colorScheme.TitleBarGradientStart = theme.SurfaceColor;
            colorScheme.TitleBarGradientEnd = theme.BackgroundColor;
            colorScheme.TitleTextColor = theme.PrimaryTextColor;
            colorScheme.ContentBackColor = theme.SurfaceColor;
            colorScheme.StatusBarColor = theme.BackgroundColor;
            colorScheme.BorderColor = theme.BorderColor;
        }

        private void ApplyThemeToMaterialStyle(IBeepTheme theme, FormColorScheme colorScheme)
        {
            colorScheme.TitleBarColor = theme.PrimaryColor;
            colorScheme.TitleBarGradientStart = theme.PrimaryColor;
            colorScheme.TitleBarGradientEnd = theme.AccentColor;
            colorScheme.TitleTextColor = theme.OnPrimaryColor;
            colorScheme.ContentBackColor = theme.SurfaceColor;
            colorScheme.StatusBarColor = theme.BackgroundColor;
            colorScheme.BorderColor = theme.BorderColor;
        }

        private void ApplyThemeToFluentStyle(IBeepTheme theme, FormColorScheme colorScheme)
        {
            colorScheme.TitleBarColor = theme.BackgroundColor;
            colorScheme.TitleBarGradientStart = theme.SurfaceColor;
            colorScheme.TitleBarGradientEnd = theme.BackgroundColor;
            colorScheme.TitleTextColor = theme.PrimaryTextColor;
            colorScheme.ContentBackColor = theme.SurfaceColor;
            colorScheme.StatusBarColor = theme.BackgroundColor;
            colorScheme.BorderColor = theme.BorderColor;
        }

        private void ApplyThemeToRibbonStyle(IBeepTheme theme, FormColorScheme colorScheme)
        {
            colorScheme.TitleBarColor = theme.SurfaceColor;
            colorScheme.TitleBarGradientStart = theme.SurfaceColor;
            colorScheme.TitleBarGradientEnd = theme.BackgroundColor;
            colorScheme.TitleTextColor = theme.PrimaryTextColor;
            colorScheme.ContentBackColor = theme.SurfaceColor;
            colorScheme.StatusBarColor = theme.BackgroundColor;
            colorScheme.BorderColor = theme.BorderColor;
        }

        private void ApplyThemeToMobileStyle(IBeepTheme theme, FormColorScheme colorScheme)
        {
            colorScheme.TitleBarColor = theme.SurfaceColor;
            colorScheme.TitleBarGradientStart = theme.SurfaceColor;
            colorScheme.TitleBarGradientEnd = theme.BackgroundColor;
            colorScheme.TitleTextColor = theme.PrimaryTextColor;
            colorScheme.ContentBackColor = theme.SurfaceColor;
            colorScheme.StatusBarColor = theme.BackgroundColor;
            colorScheme.BorderColor = theme.BorderColor;
        }

        private void ApplyThemeToConsoleStyle(IBeepTheme theme, FormColorScheme colorScheme)
        {
            colorScheme.TitleBarColor = theme.IsDarkTheme ? theme.BackgroundColor : Color.FromArgb(37, 37, 38);
            colorScheme.TitleBarGradientStart = theme.IsDarkTheme ? theme.GradientStartColor : Color.FromArgb(45, 45, 48);
            colorScheme.TitleBarGradientEnd = theme.IsDarkTheme ? theme.GradientEndColor : Color.FromArgb(30, 30, 30);
            colorScheme.TitleTextColor = theme.IsDarkTheme ? theme.OnBackgroundColor : Color.FromArgb(220, 220, 220);
            colorScheme.ContentBackColor = theme.IsDarkTheme ? theme.SurfaceColor : Color.FromArgb(30, 30, 30);
            colorScheme.StatusBarColor = theme.AccentColor;
            colorScheme.BorderColor = theme.BorderColor;
        }

        private void ApplyThemeToFloatingStyle(IBeepTheme theme, FormColorScheme colorScheme)
        {
            colorScheme.TitleBarColor = theme.SurfaceColor;
            colorScheme.TitleBarGradientStart = theme.SurfaceColor;
            colorScheme.TitleBarGradientEnd = theme.BackgroundColor;
            colorScheme.TitleTextColor = theme.SecondaryTextColor;
            colorScheme.ContentBackColor = theme.SurfaceColor;
            colorScheme.StatusBarColor = theme.SurfaceColor;
            colorScheme.BorderColor = theme.BorderColor;
        }

        private FormColorScheme GetDefaultColorScheme(FormUIStyle style)
        {
            var colorScheme = new FormColorScheme();
            
            switch (style)
            {
                case FormUIStyle.Mac:
                    colorScheme.TitleBarColor = Color.FromArgb(236, 236, 236);
                    colorScheme.TitleBarGradientStart = Color.FromArgb(246, 246, 246);
                    colorScheme.TitleBarGradientEnd = Color.FromArgb(226, 226, 226);
                    colorScheme.TitleTextColor = Color.FromArgb(51, 51, 51);
                    colorScheme.ContentBackColor = Color.FromArgb(248, 248, 248);
                    colorScheme.StatusBarColor = Color.FromArgb(240, 240, 240);
                    colorScheme.BorderColor = Color.FromArgb(204, 204, 204);
                    break;
                    
                case FormUIStyle.Classic:
                    colorScheme.TitleBarColor = Color.FromArgb(58, 110, 165);
                    colorScheme.TitleBarGradientStart = Color.FromArgb(78, 130, 185);
                    colorScheme.TitleBarGradientEnd = Color.FromArgb(38, 90, 145);
                    colorScheme.TitleTextColor = Color.White;
                    colorScheme.ContentBackColor = Color.FromArgb(236, 233, 216);
                    colorScheme.StatusBarColor = Color.FromArgb(226, 223, 206);
                    colorScheme.BorderColor = Color.FromArgb(113, 111, 100);
                    break;
                    
                case FormUIStyle.Console:
                    colorScheme.TitleBarColor = Color.FromArgb(37, 37, 38);
                    colorScheme.TitleBarGradientStart = Color.FromArgb(45, 45, 48);
                    colorScheme.TitleBarGradientEnd = Color.FromArgb(30, 30, 30);
                    colorScheme.TitleTextColor = Color.FromArgb(220, 220, 220);
                    colorScheme.ContentBackColor = Color.FromArgb(30, 30, 30);
                    colorScheme.StatusBarColor = Color.FromArgb(0, 122, 204);
                    colorScheme.BorderColor = Color.FromArgb(63, 63, 70);
                    break;
                    
                default: // Modern and others
                    colorScheme.TitleBarColor = Color.FromArgb(32, 32, 32);
                    colorScheme.TitleBarGradientStart = Color.FromArgb(48, 48, 48);
                    colorScheme.TitleBarGradientEnd = Color.FromArgb(24, 24, 24);
                    colorScheme.TitleTextColor = Color.White;
                    colorScheme.ContentBackColor = Color.FromArgb(250, 250, 250);
                    colorScheme.StatusBarColor = Color.FromArgb(240, 240, 240);
                    colorScheme.BorderColor = Color.FromArgb(64, 64, 64);
                    break;
            }
            
            return colorScheme;
        }
        #endregion
    }

    /// <summary>
    /// Data structure to hold color scheme information
    /// </summary>
    public class FormColorScheme
    {
        public Color TitleBarColor { get; set; }
        public Color TitleBarGradientStart { get; set; }
        public Color TitleBarGradientEnd { get; set; }
        public Color TitleTextColor { get; set; }
        public Color ContentBackColor { get; set; }
        public Color StatusBarColor { get; set; }
        public Color BorderColor { get; set; }
    }
}