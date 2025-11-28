using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyColorPalette()
        {
            // Metro2 theme - Windows Metro with accent stripe
            // Aligned with FormStyle.Metro2: Border #0078D7, Background #FFFFFF
            this.ForeColor = Color.FromArgb(0, 0, 0);  // Black text
            this.BackColor = Color.FromArgb(255, 255, 255);  // White background
            this.PanelBackColor = Color.FromArgb(255, 255, 255);
            this.PanelGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.PanelGradiantEndColor = Color.FromArgb(250, 250, 250);
            this.PanelGradiantMiddleColor = Color.FromArgb(252, 252, 252);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Metro2 borders - blue accent
            this.BorderColor = Color.FromArgb(0, 120, 215);  // Metro blue (#0078D7)
            this.ActiveBorderColor = Color.FromArgb(0, 120, 215);  // Metro blue
            this.InactiveBorderColor = Color.FromArgb(200, 200, 200);  // Light gray
            
            // Metro2 palette - Windows Metro with accent stripe
            this.PrimaryColor = Color.FromArgb(0, 120, 215);  // Metro blue (#0078D7)
            this.SecondaryColor = Color.FromArgb(255, 255, 255);  // White
            this.AccentColor = Color.FromArgb(0, 120, 215);  // Metro blue accent stripe
            this.BackgroundColor = Color.FromArgb(255, 255, 255);
            this.SurfaceColor = Color.FromArgb(255, 255, 255);
            
            // Status colors
            this.ErrorColor = Color.FromArgb(232, 17, 35);  // Red
            this.WarningColor = Color.FromArgb(255, 185, 0);  // Orange
            this.SuccessColor = Color.FromArgb(0, 153, 51);  // Green
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(255, 255, 255);  // White on blue
            this.OnBackgroundColor = Color.FromArgb(0, 0, 0);  // Black on white
            this.FocusIndicatorColor = Color.FromArgb(0, 120, 215);  // Metro blue focus
            // subtle shadow for floating elements
            this.ShadowColor = Color.FromArgb(25, 0, 0, 0);  // subtle black alpha
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
    }
}