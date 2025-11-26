using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyColorPalette()
        {
            // Fluent theme - Microsoft Fluent Design System
            // Aligned with FormStyle.Fluent: Light, modern, clean
            this.ForeColor = Color.FromArgb(32, 32, 32);  // Dark gray text
            this.BackColor = Color.FromArgb(245, 246, 248);  // Light gray-blue (#F5F6F8)
            this.BackgroundColor = Color.FromArgb(245, 246, 248);
            this.SurfaceColor = Color.FromArgb(255, 255, 255);  // Pure white
            this.PanelBackColor = Color.FromArgb(245, 246, 248);
            this.PanelGradiantStartColor = Color.FromArgb(245, 246, 248);
            this.PanelGradiantEndColor = Color.FromArgb(235, 237, 240);
            this.PanelGradiantMiddleColor = Color.FromArgb(240, 242, 245);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Fluent borders
            this.BorderColor = Color.FromArgb(218, 223, 230);  // Light gray
            this.ActiveBorderColor = Color.FromArgb(0, 120, 215);  // Fluent blue
            this.InactiveBorderColor = Color.FromArgb(180, 186, 196);  // Darker gray
            
            // Fluent palette - Microsoft colors
            this.PrimaryColor = Color.FromArgb(0, 120, 215);  // Fluent blue
            this.SecondaryColor = Color.FromArgb(0, 153, 188);  // Cyan
            this.AccentColor = Color.FromArgb(255, 185, 0);  // Yellow/orange
            
            // Fluent status colors
            this.ErrorColor = Color.FromArgb(196, 30, 58);  // Red
            this.WarningColor = Color.FromArgb(255, 185, 0);  // Yellow
            this.SuccessColor = Color.FromArgb(16, 124, 16);  // Green
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(255, 255, 255);  // White on blue
            this.OnBackgroundColor = Color.FromArgb(32, 32, 32);  // Dark on light
            this.FocusIndicatorColor = Color.FromArgb(0, 120, 215);  // Blue focus
        }
    }
}
