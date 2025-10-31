using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyColorPalette()
        {
            // Holographic theme - futuristic magenta/cyan gradient aesthetic
            // Aligned with FormStyle.Holographic: Background #191123, magenta ↔ cyan gradients
            this.ForeColor = Color.FromArgb(200, 150, 255);  // Light purple/magenta (#C896FF)
            this.BackColor = Color.FromArgb(25, 20, 35);  // Very dark purple (#191A23)
            this.PanelBackColor = Color.FromArgb(40, 30, 60);  // Dark purple
            this.PanelGradiantStartColor = Color.FromArgb(255, 122, 217);  // Pink
            this.PanelGradiantEndColor = Color.FromArgb(122, 252, 255);  // Cyan
            this.PanelGradiantMiddleColor = Color.FromArgb(176, 141, 255);  // Purple
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            
            // Holographic borders - glowing purple/cyan
            this.BorderColor = Color.FromArgb(138, 70, 255);  // #8A6FFF
            this.ActiveBorderColor = Color.FromArgb(150, 200, 255);  // Cyan glow
            this.InactiveBorderColor = Color.FromArgb(80, 60, 120);  // Dark purple
            
            // Holographic palette - vibrant gradients
            this.PrimaryColor = Color.FromArgb(255, 122, 217);  // Pink (#FF7AD9)
            this.SecondaryColor = Color.FromArgb(122, 252, 255);  // Cyan (#7AF CFF)
            this.AccentColor = Color.FromArgb(176, 141, 255);  // Purple (#B08DFF)
            this.BackgroundColor = Color.FromArgb(25, 20, 35);  // #191A23
            this.SurfaceColor = Color.FromArgb(40, 30, 60);  // Dark purple
            
            // Vibrant status colors
            this.ErrorColor = Color.FromArgb(255, 138, 167);  // Pink-red
            this.WarningColor = Color.FromArgb(255, 212, 126);  // Orange-yellow
            this.SuccessColor = Color.FromArgb(159, 255, 169);  // Green-cyan
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(25, 20, 35);  // Dark on colored
            this.OnBackgroundColor = Color.FromArgb(200, 150, 255);  // Light on dark
            this.FocusIndicatorColor = Color.FromArgb(122, 252, 255);  // Cyan focus
        }
    }
}