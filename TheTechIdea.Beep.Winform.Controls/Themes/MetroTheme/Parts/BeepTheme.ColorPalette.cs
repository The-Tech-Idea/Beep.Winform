using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyColorPalette()
        {
            // Metro theme - Windows Metro design language
            // Aligned with FormStyle.Metro: Caption #0078D7, Background #FFFFFF
            this.ForeColor = Color.FromArgb(0, 0, 0);  // Black text
            this.BackColor = Color.FromArgb(255, 255, 255);  // White background
            this.PanelBackColor = Color.FromArgb(255, 255, 255);
            this.PanelGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.PanelGradiantEndColor = Color.FromArgb(250, 250, 250);
            this.PanelGradiantMiddleColor = Color.FromArgb(252, 252, 252);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Metro borders
            this.BorderColor = Color.FromArgb(180, 180, 180);  // Light gray
            this.ActiveBorderColor = Color.FromArgb(0, 120, 215);  // Metro blue
            this.InactiveBorderColor = Color.FromArgb(200, 198, 196);  // Light gray
            
            // Metro palette - Windows Metro colors
            this.PrimaryColor = Color.FromArgb(0, 120, 215);  // Metro blue (#0078D7)
            this.SecondaryColor = Color.FromArgb(16, 124, 16);  // Green
            this.AccentColor = Color.FromArgb(255, 185, 0);  // Orange
            this.BackgroundColor = Color.FromArgb(255, 255, 255);
            this.SurfaceColor = Color.FromArgb(255, 255, 255);
            
            // Status colors
            this.ErrorColor = Color.FromArgb(196, 30, 58);  // Red
            this.WarningColor = Color.FromArgb(255, 185, 0);  // Orange
            this.SuccessColor = Color.FromArgb(16, 124, 16);  // Green
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(255, 255, 255);  // White on blue
            this.OnBackgroundColor = Color.FromArgb(0, 0, 0);  // Black on white
            this.FocusIndicatorColor = Color.FromArgb(0, 120, 215);  // Metro blue focus
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
    }
}