using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyColorPalette()
        {
            // Dracula theme - dark purple aesthetic
            // Aligned with FormStyle.Dracula: Background #282A36, Caption text #F8F8F2
            this.ForeColor = Color.FromArgb(248, 248, 242);  // Light cream text (#F8F8F2)
            this.BackColor = Color.FromArgb(40, 42, 54);  // Dark purple (#282A36)
            this.PanelBackColor = Color.FromArgb(40, 42, 54);
            this.PanelGradiantStartColor = Color.FromArgb(68, 71, 90);  // #44475A
            this.PanelGradiantEndColor = Color.FromArgb(52, 55, 72);
            this.PanelGradiantMiddleColor = Color.FromArgb(60, 63, 82);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            
            // Purple borders
            this.BorderColor = Color.FromArgb(68, 71, 90);  // #44475A
            this.ActiveBorderColor = Color.FromArgb(189, 147, 249);  // Purple accent
            this.InactiveBorderColor = Color.FromArgb(98, 114, 164);  // Blue-gray
            
            // Dracula palette - pink/purple accents
            this.PrimaryColor = Color.FromArgb(189, 147, 249);  // Purple (#BD93F9)
            this.SecondaryColor = Color.FromArgb(139, 233, 253);  // Cyan (#8BE9FD)
            this.AccentColor = Color.FromArgb(255, 121, 198);  // Pink (#FF79C6)
            this.BackgroundColor = Color.FromArgb(40, 42, 54);  // #282A36
            this.SurfaceColor = Color.FromArgb(68, 71, 90);  // #44475A
            
            // Bright status colors
            this.ErrorColor = Color.FromArgb(255, 85, 85);  // Red (#FF5555)
            this.WarningColor = Color.FromArgb(241, 250, 140);  // Yellow (#F1FA8C)
            this.SuccessColor = Color.FromArgb(80, 250, 123);  // Green (#50FA7B)
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(40, 42, 54);  // Dark on purple
            this.OnBackgroundColor = Color.FromArgb(248, 248, 242);  // Light on dark
            this.FocusIndicatorColor = Color.FromArgb(139, 233, 253);  // Cyan focus
        }
    }
}