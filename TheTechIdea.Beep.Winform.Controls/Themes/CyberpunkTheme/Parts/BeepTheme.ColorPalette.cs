using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyColorPalette()
        {
            // Cyberpunk theme - neon aesthetic with dark background
            // Aligned with FormStyle.Cyberpunk: Background #080B14, Accent Cyan #00FFFF
            this.ForeColor = Color.FromArgb(0, 255, 255);  // Cyan text
            this.BackColor = Color.FromArgb(10, 10, 20);  // Very dark navy (#0A0A14)
            this.PanelBackColor = Color.FromArgb(20, 20, 40);
            this.PanelGradiantStartColor = Color.FromArgb(32, 12, 48);
            this.PanelGradiantEndColor = Color.FromArgb(16, 32, 64);
            this.PanelGradiantMiddleColor = Color.FromArgb(20, 24, 40);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            
            // Neon borders - cyan glow
            this.BorderColor = Color.FromArgb(0, 255, 255);  // Cyan borders
            this.ActiveBorderColor = Color.FromArgb(100, 255, 255);  // Bright cyan
            this.InactiveBorderColor = Color.FromArgb(80, 100, 120);  // Muted for inactive
            
            // Vibrant neon palette
            this.PrimaryColor = Color.FromArgb(0, 255, 255);  // Cyan
            this.SecondaryColor = Color.FromArgb(255, 0, 128);  // Magenta
            this.AccentColor = Color.FromArgb(255, 230, 0);  // Yellow
            this.BackgroundColor = Color.FromArgb(10, 10, 20);  // Dark background
            this.SurfaceColor = Color.FromArgb(22, 20, 40);  // Slightly lighter surface
            
            // Neon status colors
            this.ErrorColor = Color.FromArgb(255, 64, 64);  // Red
            this.WarningColor = Color.FromArgb(255, 191, 0);  // Yellow
            this.SuccessColor = Color.FromArgb(0, 255, 140);  // Green
            
            // On-colors for neon
            this.OnPrimaryColor = Color.FromArgb(0, 0, 0);  // Black on cyan
            this.OnBackgroundColor = Color.FromArgb(0, 255, 255);  // Cyan on dark
            this.FocusIndicatorColor = Color.FromArgb(0, 255, 255);  // Cyan focus ring
        }
    }
}