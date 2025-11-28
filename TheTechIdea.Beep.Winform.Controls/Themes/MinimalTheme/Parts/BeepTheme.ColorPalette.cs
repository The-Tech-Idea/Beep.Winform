using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyColorPalette()
        {
            // Minimal theme - clean, minimal aesthetic
            // Aligned with FormStyle.Minimal: Background #FFFFFF, Border #C8C8C8, subtle highlighting
            this.ForeColor = Color.FromArgb(30, 30, 30);  // Dark grey text
            this.BackColor = Color.FromArgb(255, 255, 255);  // Pure white (#FFFFFF)
            this.PanelBackColor = Color.FromArgb(255, 255, 255);
            this.PanelGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.PanelGradiantEndColor = Color.FromArgb(255, 255, 255);
            this.PanelGradiantMiddleColor = Color.FromArgb(255, 255, 255);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Minimal borders
            this.BorderColor = Color.FromArgb(200, 200, 200);  // #C8C8C8
            this.ActiveBorderColor = Color.FromArgb(45, 45, 45);  // Dark grey for active
            this.InactiveBorderColor = Color.FromArgb(220, 220, 220);  // Light grey
            
            // Minimal palette - clean and simple
            this.PrimaryColor = Color.FromArgb(20, 20, 20);  // Almost black
            this.SecondaryColor = Color.FromArgb(100, 100, 100);  // Medium grey
            this.AccentColor = Color.FromArgb(210, 210, 210);  // Light grey
            this.BackgroundColor = Color.FromArgb(255, 255, 255);
            this.SurfaceColor = Color.FromArgb(245, 245, 245);  // Very light grey
            
            // Status colors
            this.ErrorColor = Color.FromArgb(239, 68, 68);  // Red
            this.WarningColor = Color.FromArgb(245, 158, 11);  // Orange
            this.SuccessColor = Color.FromArgb(16, 185, 129);  // Green
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(255, 255, 255);  // White on dark
            this.OnBackgroundColor = Color.FromArgb(30, 30, 30);  // Dark on white
            this.FocusIndicatorColor = Color.FromArgb(45, 45, 45);  // Dark grey focus
            this.ShadowColor = Color.FromArgb(25, 0, 0, 0);  // Subtle black alpha
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }

        private static Color MinimalMinimizeColor => Color.FromArgb(253, 224, 71);
        private static Color MinimalMaximizeColor => Color.FromArgb(134, 239, 172);
        private static Color MinimalCloseColor => Color.FromArgb(248, 113, 113);
        private static Color MinimalMutedTextColor => Color.FromArgb(120, 120, 120);
    }
}
