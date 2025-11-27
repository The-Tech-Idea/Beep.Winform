using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyColorPalette()
        {
            // Nordic theme - Scandinavian minimalist design
            // Aligned with FormStyle.Nordic: Background #F2F5F8, icy blue accent
            this.ForeColor = Color.FromArgb(60, 60, 60);  // Dark gray text
            this.BackColor = Color.FromArgb(242, 245, 248);  // Light gray-blue (#F2F5F8)
            this.BackgroundColor = Color.FromArgb(242, 245, 248);
            this.SurfaceColor = Color.FromArgb(255, 255, 255);  // White
            this.PanelBackColor = Color.FromArgb(242, 245, 248);
            this.PanelGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.03);
            this.PanelGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.PanelGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Nordic borders
            this.BorderColor = Color.FromArgb(220, 220, 220);  // Light gray
            this.ActiveBorderColor = Color.FromArgb(136, 192, 208);  // Icy blue
            this.InactiveBorderColor = Color.FromArgb(203, 213, 225);  // Medium gray
            
            // Nordic palette - Scandinavian colors
            this.PrimaryColor = Color.FromArgb(136, 192, 208);  // Icy blue
            this.SecondaryColor = Color.FromArgb(100, 100, 100);  // Medium gray
            this.AccentColor = Color.FromArgb(216, 122, 52);  // Orange
            
            // Status colors
            this.ErrorColor = Color.FromArgb(220, 38, 38);  // Red
            this.WarningColor = Color.FromArgb(245, 158, 11);  // Orange
            this.SuccessColor = Color.FromArgb(16, 157, 108);  // Green
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(255, 255, 255);  // White on blue
            this.OnBackgroundColor = Color.FromArgb(60, 60, 60);  // Dark on light
            this.FocusIndicatorColor = Color.FromArgb(136, 192, 208);  // Icy blue focus
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
    }
}
