using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyColorPalette()
        {
            // Paper theme - Material Design paper aesthetic
            // Aligned with FormStyle.Paper: Background #FAFAF8, white highlight, clean aesthetic
            this.ForeColor = Color.FromArgb(33, 33, 33);  // Dark grey text
            this.BackColor = Color.FromArgb(250, 250, 250);  // Light paper (#FAFAF8)
            this.PanelBackColor = Color.FromArgb(250, 250, 250);
            this.PanelGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.PanelGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.03);
            this.PanelGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Paper borders
            this.BorderColor = Color.FromArgb(224, 224, 224);  // Light grey
            this.ActiveBorderColor = Color.FromArgb(33, 150, 243);  // Material blue
            this.InactiveBorderColor = Color.FromArgb(230, 230, 230);  // Slightly darker grey
            
            // Paper palette - Material Design colors
            this.PrimaryColor = Color.FromArgb(33, 150, 243);  // Material blue (#2196F3)
            this.SecondaryColor = Color.FromArgb(0, 150, 136);  // Teal
            this.AccentColor = Color.FromArgb(255, 193, 7);  // Amber
            this.BackgroundColor = Color.FromArgb(250, 250, 250);
            this.SurfaceColor = Color.FromArgb(255, 255, 255);  // Pure white
            
            // Material status colors
            this.ErrorColor = Color.FromArgb(244, 67, 54);  // Red
            this.WarningColor = Color.FromArgb(255, 160, 0);  // Orange
            this.SuccessColor = Color.FromArgb(76, 175, 80);  // Green
            
            // On-colors for readability
            this.OnPrimaryColor = Color.White;  // White on blue
            this.OnBackgroundColor = Color.FromArgb(33, 33, 33);  // Dark on light
            this.FocusIndicatorColor = Color.FromArgb(33, 150, 243);  // Material blue focus
        }
    }
}