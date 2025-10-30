using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyColorPalette()
        {
            // Tokyo Night theme - inspired by Tokyo Night VSCode theme
            // Aligned with FormStyle.Tokyo: Background #1A1B27, Accent cyan glow #7AA2F7, Text #A9B1D6
            this.ForeColor = Color.FromArgb(169, 177, 214);  // Light purple-blue (#A9B1D6)
            this.BackColor = Color.FromArgb(26, 27, 38);  // Dark purple (#1A1B27)
            this.PanelBackColor = Color.FromArgb(26, 27, 38);
            this.PanelGradiantStartColor = Color.FromArgb(36, 40, 59);
            this.PanelGradiantEndColor = Color.FromArgb(36, 40, 59);
            this.PanelGradiantMiddleColor = Color.FromArgb(26, 27, 38);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Tokyo Night borders
            this.BorderColor = Color.FromArgb(86, 95, 137);  // #56617F
            this.ActiveBorderColor = Color.FromArgb(122, 162, 247);  // Tokyo cyan (#7AA2F7)
            this.InactiveBorderColor = Color.FromArgb(65, 72, 104);  // #414868
            
            // Tokyo Night palette - cyan accent colors
            this.PrimaryColor = Color.FromArgb(122, 162, 247);  // Tokyo cyan (#7AA2F7)
            this.SecondaryColor = Color.FromArgb(42, 195, 222);  // Bright cyan
            this.AccentColor = Color.FromArgb(187, 154, 247);  // Purple (#BB9AF7)
            this.BackgroundColor = Color.FromArgb(26, 27, 38);  // #1A1B27
            this.SurfaceColor = Color.FromArgb(36, 40, 59);  // #24283B
            
            // Status colors
            this.ErrorColor = Color.FromArgb(247, 118, 142);  // Pink-red (#F7768E)
            this.WarningColor = Color.FromArgb(224, 175, 104);  // Yellow (#E0AF68)
            this.SuccessColor = Color.FromArgb(158, 206, 106);  // Green (#9ECE6A)
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(26, 27, 38);  // Dark on cyan
            this.OnBackgroundColor = Color.FromArgb(169, 177, 214);  // Light on dark
            this.FocusIndicatorColor = Color.FromArgb(122, 162, 247);  // Tokyo cyan focus
        }
    }
}