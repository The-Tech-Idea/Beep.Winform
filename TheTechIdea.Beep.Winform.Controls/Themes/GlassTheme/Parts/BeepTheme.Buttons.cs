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
            this.ButtonBackColor = BackgroundColor;  // Light blue
            this.ButtonForeColor = ForeColor;  // Dark gray
            this.ButtonBorderColor = BorderColor;  // Visible border
            
            // Hover: Slightly darker blue
            this.ButtonHoverBackColor = Color.FromArgb(216, 234, 250);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Indigo border
            
            // Selected: Medium blue
            this.ButtonSelectedBackColor = Color.FromArgb(190, 220, 245);
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedBorderColor = ActiveBorderColor;
            
            // Selected hover: Darker blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(170, 210, 240);
            this.ButtonSelectedHoverForeColor = ForeColor;
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(250, 252, 255);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
