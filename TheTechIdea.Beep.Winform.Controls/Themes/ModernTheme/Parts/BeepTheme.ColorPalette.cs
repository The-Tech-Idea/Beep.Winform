using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplyColorPalette()
        {
            // Modern theme - clean, modern aesthetic
            // Aligned with FormStyle.Modern: Background #FFFFFF, light gray caption
            this.ForeColor = Color.FromArgb(0, 0, 0);  // Black text
            this.BackColor = Color.FromArgb(255, 255, 255);  // White background
            this.PanelBackColor = Color.FromArgb(255, 255, 255);
            this.PanelGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.PanelGradiantEndColor = Color.FromArgb(250, 250, 252);
            this.PanelGradiantMiddleColor = Color.FromArgb(253, 254, 255);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Modern borders
            this.BorderColor = Color.FromArgb(200, 200, 200);  // Light gray
            this.ActiveBorderColor = Color.FromArgb(99, 102, 241);  // Indigo
            this.InactiveBorderColor = Color.FromArgb(150, 163, 184);  // Medium gray
            
            // Modern palette - indigo accents
            this.PrimaryColor = Color.FromArgb(99, 102, 241);  // Indigo
            this.SecondaryColor = Color.FromArgb(100, 100, 100);  // Medium gray
            this.AccentColor = Color.FromArgb(245, 158, 11);  // Orange
            this.BackgroundColor = Color.FromArgb(255, 255, 255);
            this.SurfaceColor = Color.FromArgb(248, 250, 252);  // Light gray
            
            // Status colors
            this.ErrorColor = Color.FromArgb(239, 68, 68);  // Red
            this.WarningColor = Color.FromArgb(245, 158, 11);  // Orange
            this.SuccessColor = Color.FromArgb(16, 185, 129);  // Teal
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(255, 255, 255);  // White on indigo
            this.OnBackgroundColor = Color.FromArgb(0, 0, 0);  // Black on white
            this.FocusIndicatorColor = Color.FromArgb(99, 102, 241);  // Indigo focus
        }
    }
}