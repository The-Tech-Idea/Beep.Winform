using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyButtons()
        {
            // MacOS buttons - clean macOS aesthetic
            // Default: Light gray background with dark text
            this.ButtonBackColor = Color.FromArgb(236, 236, 238); // Light gray
            this.ButtonForeColor = ForeColor;  // Dark gray
            this.ButtonBorderColor = BorderColor;  // Medium gray
            
            // Hover: Darker background
            this.ButtonHoverBackColor = PanelGradiantMiddleColor;
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Dark gray border
            
            // Selected: Dark gray background
            this.ButtonSelectedBackColor = ThemeUtil.Darken(PanelBackColor, 0.06);
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedBorderColor = ActiveBorderColor;
            
            // Selected hover: Lighter gray
            this.ButtonSelectedHoverBackColor = PanelGradiantMiddleColor;
            this.ButtonSelectedHoverForeColor = ForeColor;
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = ThemeUtil.Darken(SurfaceColor, 0.08);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // macOS red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}









