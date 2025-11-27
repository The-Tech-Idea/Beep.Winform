using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyColorPalette()
        {
            // Neon theme - deep navy with pink/cyan glow
            // Aligned with FormStyle.Neon: Background deep navy, Cyan #00FFFF glow
            this.ForeColor = Color.FromArgb(0, 255, 255);  // Cyan text (#00FFFF)
            this.BackColor = Color.FromArgb(15, 15, 25);  // Deep navy (#0F0F19)
            this.PanelBackColor = Color.FromArgb(20, 20, 30);
            this.PanelGradiantStartColor = Color.FromArgb(20, 24, 38);
            this.PanelGradiantEndColor = Color.FromArgb(15, 18, 30);
            this.PanelGradiantMiddleColor = Color.FromArgb(10, 12, 20);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Neon borders - glowing cyan
            this.BorderColor = Color.FromArgb(0, 255, 255);  // Cyan glow (#00FFFF)
            this.ActiveBorderColor = Color.FromArgb(0, 255, 255);  // Bright cyan
            this.InactiveBorderColor = Color.FromArgb(80, 180, 200);  // Dimmed cyan
            
            // Neon palette - vibrant neon colors
            this.PrimaryColor = Color.FromArgb(255, 64, 160);  // Pink (#FF40A0)
            this.SecondaryColor = Color.FromArgb(64, 224, 255);  // Cyan (#40E0FF)
            this.AccentColor = Color.FromArgb(180, 86, 255);  // Purple (#B456FF)
            this.BackgroundColor = Color.FromArgb(15, 15, 25);  // #0F0F19
            this.SurfaceColor = Color.FromArgb(20, 24, 38);
            
            // Bright status colors
            this.ErrorColor = Color.FromArgb(255, 77, 109);  // Pink-red
            this.WarningColor = Color.FromArgb(255, 208, 0);  // Yellow
            this.SuccessColor = Color.FromArgb(57, 255, 20);  // Green
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(15, 15, 25);  // Dark on neon
            this.OnBackgroundColor = Color.FromArgb(0, 255, 255);  // Cyan on dark
            this.FocusIndicatorColor = Color.FromArgb(0, 255, 255);  // Cyan focus
            // Validate contrast and apply autofixes
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
    }
}