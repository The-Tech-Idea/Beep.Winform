using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyColorPalette()
        {
            // Brutalist theme - bold, high-contrast black and white
            // Aligned with FormStyle.Brutalist: Background #FFFFFF, Border #000000
            this.ForeColor = Color.FromArgb(0, 0, 0);  // Black text
            this.BackColor = Color.FromArgb(255, 255, 255);  // Pure white background
            this.PanelBackColor = BackColor;
            this.PanelGradiantStartColor = BackColor;
            this.PanelGradiantEndColor = BackColor;
            this.PanelGradiantMiddleColor = BackColor;
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Bold borders - pure black
            this.BorderColor = Color.FromArgb(0, 0, 0);
            this.ActiveBorderColor = Color.FromArgb(0, 0, 0);
            this.InactiveBorderColor = Color.FromArgb(0, 0, 0);
            
            // High contrast palette
            this.PrimaryColor = Color.FromArgb(0, 0, 0);  // Black primary
            this.SecondaryColor = Color.FromArgb(100, 100, 100);  // Medium gray
            this.AccentColor = Color.FromArgb(255, 208, 0);  // Yellow accent for emphasis
            this.BackgroundColor = BackColor;  // White
            this.SurfaceColor = Color.FromArgb(250, 250, 250);  // Very light gray for elevated surfaces
            
            // Bold status colors
            this.ErrorColor = Color.FromArgb(220, 0, 0);  // Bright red
            this.WarningColor = Color.FromArgb(255, 140, 0);  // Orange
            this.SuccessColor = Color.FromArgb(0, 200, 70);  // Green
            
            // High contrast on-colors
            this.OnPrimaryColor = Color.FromArgb(255, 255, 255);  // White on black
            this.OnBackgroundColor = Color.FromArgb(0, 0, 0);  // Black on white
            this.FocusIndicatorColor = Color.FromArgb(0, 0, 0);  // Black focus ring
        }
    }
}
