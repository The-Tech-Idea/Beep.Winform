using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyColorPalette()
        {
            // Cartoon theme - playful purple aesthetic
            // Aligned with FormStyle.Cartoon: Background #FFF0FF, Border #9664C8, Text purple
            this.ForeColor = Color.FromArgb(80, 0, 120);  // Deep purple text
            this.BackColor = Color.FromArgb(255, 240, 255);  // Pink-tinted white
            this.PanelBackColor = Color.FromArgb(255, 240, 255);
            this.PanelGradiantStartColor = Color.FromArgb(255, 250, 255);
            this.PanelGradiantEndColor = Color.FromArgb(255, 230, 250);
            this.PanelGradiantMiddleColor = Color.FromArgb(255, 240, 255);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            
            // Playful borders - purple accents
            this.BorderColor = Color.FromArgb(150, 100, 200);  // #9664C8
            this.ActiveBorderColor = Color.FromArgb(120, 0, 180);  // Brighter purple
            this.InactiveBorderColor = Color.FromArgb(200, 150, 230);  // Lighter purple
            
            // Vibrant cartoon palette
            this.PrimaryColor = Color.FromArgb(80, 0, 120);  // Deep purple
            this.SecondaryColor = Color.FromArgb(150, 100, 200);  // Medium purple
            this.AccentColor = Color.FromArgb(255, 107, 157);  // Pink accent
            this.BackgroundColor = Color.FromArgb(255, 240, 255);  // Light pink background
            this.SurfaceColor = Color.FromArgb(255, 250, 255);  // Almost white surface
            
            // Bright status colors
            this.ErrorColor = Color.FromArgb(255, 69, 180);  // Pink-red
            this.WarningColor = Color.FromArgb(255, 215, 0);  // Yellow
            this.SuccessColor = Color.FromArgb(72, 199, 142);  // Green
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(255, 255, 255);  // White on purple
            this.OnBackgroundColor = Color.FromArgb(80, 0, 120);  // Purple on white
            this.FocusIndicatorColor = Color.FromArgb(120, 0, 180);  // Purple focus
        }
    }
}