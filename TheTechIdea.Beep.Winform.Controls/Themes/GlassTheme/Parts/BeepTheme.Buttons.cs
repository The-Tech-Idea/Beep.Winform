using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyButtons()
        {
            // Glass buttons - frosted glass effect
            // Default: Light blue background with dark text
            this.ButtonBackColor = Color.FromArgb(224, 236, 250); // Light surface
            this.ButtonForeColor = ForeColor;  // Dark gray
            this.ButtonBorderColor = InactiveBorderColor;  // Visible border
            
            // Hover: Slightly darker blue
            this.ButtonHoverBackColor = PanelGradiantStartColor;
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Indigo border
            
            // Selected: Medium blue
            this.ButtonSelectedBackColor = PrimaryColor;
            this.ButtonSelectedForeColor = OnPrimaryColor;
            this.ButtonSelectedBorderColor = ActiveBorderColor;
            
            // Selected hover: Darker blue
            this.ButtonSelectedHoverBackColor = PanelGradiantStartColor;
            this.ButtonSelectedHoverForeColor = ForeColor;
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = PanelGradiantMiddleColor;
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = InactiveBorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}









