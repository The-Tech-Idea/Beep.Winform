using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyColorPalette()
        {
            // Glass theme - frosted glass aesthetic with transparency
            // Aligned with FormStyle.Glass: Light acrylic with semi-transparency
            this.ForeColor = Color.FromArgb(17, 24, 39);  // Dark gray text
            this.BackColor = Color.FromArgb(236, 244, 255);  // Light blue (#ECF4FF)
            this.PanelBackColor = Color.FromArgb(220, 235, 250);
            this.PanelGradiantStartColor = Color.FromArgb(240, 248, 255);
            this.PanelGradiantEndColor = Color.FromArgb(220, 235, 250);
            this.PanelGradiantMiddleColor = Color.FromArgb(230, 242, 255);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Glass borders - subtle with transparency effect
            this.BorderColor = Color.FromArgb(200, 220, 240);  // Visible gray-blue
            this.ActiveBorderColor = Color.FromArgb(99, 102, 241);  // Indigo accent
            this.InactiveBorderColor = Color.FromArgb(148, 163, 184);  // Medium gray
            
            // Glass palette - clean and modern
            this.PrimaryColor = Color.FromArgb(99, 102, 241);  // Indigo
            this.SecondaryColor = Color.FromArgb(56, 189, 248);  // Sky blue
            this.AccentColor = Color.FromArgb(16, 185, 129);  // Teal
            this.BackgroundColor = Color.FromArgb(236, 244, 255);
            this.SurfaceColor = Color.FromArgb(250, 252, 255);  // Very light blue
            
            // Status colors
            this.ErrorColor = Color.FromArgb(239, 68, 68);  // Red
            this.WarningColor = Color.FromArgb(245, 158, 11);  // Orange
            this.SuccessColor = Color.FromArgb(16, 185, 129);  // Teal
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(255, 255, 255);  // White on indigo
            this.OnBackgroundColor = Color.FromArgb(17, 24, 39);  // Dark on light
            this.FocusIndicatorColor = Color.FromArgb(99, 102, 241);  // Indigo focus
        }
    }
}