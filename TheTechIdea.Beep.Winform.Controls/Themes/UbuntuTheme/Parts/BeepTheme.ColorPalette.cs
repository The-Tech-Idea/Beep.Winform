using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyColorPalette()
        {
            // Ubuntu theme - Ubuntu Linux desktop aesthetic
            // Aligned with FormStyle.Ubuntu: Background gradient #E95420 → #7F2A81, orange accent
            this.ForeColor = Color.FromArgb(44, 44, 44);  // Dark grey text
            this.BackColor = Color.FromArgb(242, 242, 245);  // Light grey background
            this.BackgroundColor = Color.FromArgb(242, 242, 245);
            this.SurfaceColor = Color.FromArgb(255, 255, 255);  // White
            this.PanelBackColor = Color.FromArgb(242, 242, 245);
            this.PanelGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.PanelGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.PanelGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Ubuntu borders
            this.BorderColor = Color.FromArgb(218, 218, 222);  // Light grey
            this.ActiveBorderColor = Color.FromArgb(233, 84, 32);  // Ubuntu orange (#E95420)
            this.InactiveBorderColor = Color.FromArgb(192, 192, 196);  // Medium grey
            
            // Ubuntu palette - Ubuntu brand colors
            this.PrimaryColor = Color.FromArgb(233, 84, 32);  // Ubuntu orange (#E95420)
            this.SecondaryColor = Color.FromArgb(119, 33, 111);  // Ubuntu purple (#77216F / #7F2A81)
            this.AccentColor = Color.FromArgb(244, 197, 66);  // Ubuntu yellow
            
            // Ubuntu status colors
            this.ErrorColor = Color.FromArgb(192, 28, 40);  // Ubuntu red
            this.WarningColor = Color.FromArgb(229, 165, 10);  // Orange
            this.SuccessColor = Color.FromArgb(42, 168, 118);  // Green
            
           
            this.OnPrimaryColor = Color.White;  // White on orange
            this.OnBackgroundColor = Color.FromArgb(44, 44, 44);  // Dark on light
            this.FocusIndicatorColor = Color.FromArgb(233, 84, 32);  // Ubuntu orange focus
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
    }
}
