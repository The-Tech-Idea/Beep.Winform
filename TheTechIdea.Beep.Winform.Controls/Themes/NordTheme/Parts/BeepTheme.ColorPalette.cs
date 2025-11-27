using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyColorPalette()
        {
            // Nord theme - Arctic-inspired dark theme
            // Aligned with FormStyle.Nord: Background #2E3440, Accent #88C0D0, Text #D8DEE9
            this.ForeColor = Color.FromArgb(216, 222, 233);  // Light gray-blue (#D8DEE9)
            this.BackColor = Color.FromArgb(46, 52, 64);  // Dark blue-gray (#2E3440)
            this.PanelBackColor = Color.FromArgb(46, 52, 64);
            this.PanelGradiantStartColor = Color.FromArgb(59, 66, 82);
            this.PanelGradiantEndColor = Color.FromArgb(59, 66, 82);
            this.PanelGradiantMiddleColor = Color.FromArgb(46, 52, 64);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Nord borders
            this.BorderColor = Color.FromArgb(67, 76, 94);  // #434C5E
            this.ActiveBorderColor = Color.FromArgb(136, 192, 208);  // Nord cyan (#88C0D0)
            this.InactiveBorderColor = Color.FromArgb(129, 161, 193);  // #81A1C1
            
            // Nord palette - Arctic colors
            this.PrimaryColor = Color.FromArgb(136, 192, 208);  // Nord cyan (#88C0D0)
            this.SecondaryColor = Color.FromArgb(94, 129, 172);  // #5E81AC
            this.AccentColor = Color.FromArgb(235, 203, 139);  // Nord yellow (#EBCB8B)
            this.BackgroundColor = Color.FromArgb(46, 52, 64);  // #2E3440
            this.SurfaceColor = Color.FromArgb(59, 66, 82);  // #3B4252
            
            // Status colors
            this.ErrorColor = Color.FromArgb(191, 97, 106);  // Nord red (#BF616A)
            this.WarningColor = Color.FromArgb(235, 203, 139);  // Nord yellow
            this.SuccessColor = Color.FromArgb(163, 190, 140);  // Nord green (#A3BE8C)
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(46, 52, 64);  // Dark on cyan
            this.OnBackgroundColor = Color.FromArgb(216, 222, 233);  // Light on dark
            this.FocusIndicatorColor = Color.FromArgb(136, 192, 208);  // Nord cyan focus
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
    }
}