using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyButtons()
        {
            // Paper buttons - Material Design aesthetic
            // Default: Light grey background with dark text
            this.ButtonBackColor = Color.FromArgb(230, 236, 245); // Light paper
            this.ButtonForeColor = ForeColor;  // Dark grey text
            this.ButtonBorderColor = BorderColor;  // Light grey border
            
            // Hover: Darker background
            this.ButtonHoverBackColor = PanelGradiantStartColor;
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Material blue
            
            // Selected: Material blue background
            this.ButtonSelectedBackColor = PrimaryColor;  // Material blue
            this.ButtonSelectedForeColor = OnPrimaryColor;  // White text
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Darker blue
            this.ButtonSelectedHoverBackColor = PrimaryColor;
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = PrimaryColor;
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = ThemeUtil.Darken(SurfaceColor, 0.08);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}










