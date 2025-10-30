using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyColorPalette()
        {
            // Arc Linux dark theme - aligned with FormStyle.ArcLinux from FormPainterMetrics
            // Background: #383C4A (56, 60, 74) / #404552 (64, 69, 82) - using the darker base
            this.ForeColor = Color.FromArgb(211, 218, 227);  // Light text for dark background
            this.BackColor = Color.FromArgb(56, 60, 74);  // Main background (darker variant)
            this.PanelBackColor = Color.FromArgb(64, 69, 82);  // Slightly lighter panel
            this.PanelGradiantStartColor = Color.FromArgb(64, 69, 82);
            this.PanelGradiantEndColor = Color.FromArgb(56, 60, 74);
            this.PanelGradiantMiddleColor = Color.FromArgb(60, 65, 78);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Border colors - subtle variations
            this.BorderColor = Color.FromArgb(64, 69, 82);
            this.ActiveBorderColor = Color.FromArgb(82, 148, 226);  // Accent blue for active elements
            this.InactiveBorderColor = Color.FromArgb(56, 60, 74);
            
            // Primary palette - Arc Linux signature colors
            this.PrimaryColor = Color.FromArgb(82, 148, 226);  // Arc blue
            this.SecondaryColor = Color.FromArgb(118, 182, 248);  // Lighter blue accent
            this.AccentColor = Color.FromArgb(82, 148, 226);
            this.BackgroundColor = Color.FromArgb(56, 60, 74);  // Dark background
            this.SurfaceColor = Color.FromArgb(64, 69, 82);  // Elevated surfaces
            
            // Status colors
            this.ErrorColor = Color.FromArgb(244, 67, 54);
            this.WarningColor = Color.FromArgb(255, 193, 7);
            this.SuccessColor = Color.FromArgb(76, 175, 80);
            
            // On-color variants for readability
            this.OnPrimaryColor = Color.FromArgb(211, 218, 227);  // Light text on primary
            this.OnBackgroundColor = Color.FromArgb(211, 218, 227);  // Light text on background
            this.FocusIndicatorColor = Color.FromArgb(82, 148, 226);  // Blue focus ring
        }
    }
}
