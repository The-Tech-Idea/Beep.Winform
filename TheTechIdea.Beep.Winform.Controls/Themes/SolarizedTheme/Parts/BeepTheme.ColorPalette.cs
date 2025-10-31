using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyColorPalette()
        {
            // Solarized theme - scientifically crafted color palette
            // Aligned with FormStyle.Solarized: Background #002B36, Text #EEE8D5, Accent #CB4B16
            this.ForeColor = Color.FromArgb(238, 232, 213);  // Light beige (#EEE8D5)
            this.BackColor = Color.FromArgb(0, 43, 54);  // Dark blue-green (#002B36)
            this.PanelBackColor = Color.FromArgb(0, 43, 54);
            this.PanelGradiantStartColor = Color.FromArgb(0, 43, 54);
            this.PanelGradiantEndColor = Color.FromArgb(0, 43, 54);
            this.PanelGradiantMiddleColor = Color.FromArgb(7, 54, 66);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Solarized borders
            this.BorderColor = Color.FromArgb(88, 110, 117);  // #586E75
            this.ActiveBorderColor = Color.FromArgb(203, 75, 22);  // Orange (#CB4B16)
            this.InactiveBorderColor = Color.FromArgb(101, 123, 131);  // #657B83
            
            // Solarized palette - scientific colors
            this.PrimaryColor = Color.FromArgb(38, 139, 210);  // Blue (#268BD2)
            this.SecondaryColor = Color.FromArgb(42, 161, 152);  // Cyan (#2AA198)
            this.AccentColor = Color.FromArgb(203, 75, 22);  // Orange (#CB4B16)
            this.BackgroundColor = Color.FromArgb(0, 43, 54);  // #002B36
            this.SurfaceColor = Color.FromArgb(7, 54, 66);  // #073642
            
            // Status colors
            this.ErrorColor = Color.FromArgb(220, 50, 47);  // Red (#DC322F)
            this.WarningColor = Color.FromArgb(181, 137, 0);  // Yellow (#B58900)
            this.SuccessColor = Color.FromArgb(133, 153, 0);  // Green (#859900)
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(238, 232, 213);  // Light on blue
            this.OnBackgroundColor = Color.FromArgb(238, 232, 213);  // Light on dark
            this.FocusIndicatorColor = Color.FromArgb(42, 161, 152);  // Cyan focus
        }
    }
}