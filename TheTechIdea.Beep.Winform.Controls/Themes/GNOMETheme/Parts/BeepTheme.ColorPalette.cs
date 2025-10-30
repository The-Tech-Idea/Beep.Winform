using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyColorPalette()
        {
            // GNOME theme - clean Linux desktop aesthetic
            // Aligned with FormStyle.GNOME: Background #FFFFFF, Caption #F5F5F5, dark buttons
            this.ForeColor = Color.FromArgb(35, 38, 41);  // #232629 dark text
            this.BackColor = Color.FromArgb(255, 255, 255);  // Pure white (#FFFFFF)
            this.PanelBackColor = Color.FromArgb(255, 255, 255);
            this.PanelGradiantStartColor = Color.FromArgb(245, 245, 245);  // #F5F5F5
            this.PanelGradiantEndColor = Color.FromArgb(239, 240, 241);
            this.PanelGradiantMiddleColor = Color.FromArgb(242, 243, 244);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // GNOME borders - subtle gray
            this.BorderColor = Color.FromArgb(200, 200, 200);  // #C8C8C8
            this.ActiveBorderColor = Color.FromArgb(50, 50, 50);  // Dark for contrast
            this.InactiveBorderColor = Color.FromArgb(180, 186, 196);  // Light gray
            
            // GNOME palette - clean and minimal
            this.PrimaryColor = Color.FromArgb(50, 50, 50);  // Dark gray (#323232)
            this.SecondaryColor = Color.FromArgb(100, 100, 100);  // Medium gray
            this.AccentColor = Color.FromArgb(61, 174, 233);  // GNOME blue (#3DAEE9)
            this.BackgroundColor = Color.FromArgb(255, 255, 255);
            this.SurfaceColor = Color.FromArgb(245, 245, 245);  // #F5F5F5
            
            // Status colors
            this.ErrorColor = Color.FromArgb(237, 21, 21);  // Red (#ED1515)
            this.WarningColor = Color.FromArgb(252, 175, 62);  // Orange
            this.SuccessColor = Color.FromArgb(57, 255, 20);  // Green
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(255, 255, 255);  // White on dark
            this.OnBackgroundColor = Color.FromArgb(35, 38, 41);  // Dark on light
            this.FocusIndicatorColor = Color.FromArgb(61, 174, 233);  // GNOME blue focus
        }
    }
}