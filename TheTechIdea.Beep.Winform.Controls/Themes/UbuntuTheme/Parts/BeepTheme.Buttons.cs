using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyButtons()
        {
            // Ubuntu buttons - Ubuntu Linux desktop aesthetic
            // Default: Light grey background with dark text
            this.ButtonBackColor = BackgroundColor;  // Light grey
            this.ButtonForeColor = ForeColor;  // Dark grey text
            this.ButtonBorderColor = BorderColor;  // Light grey border
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(230, 230, 235);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Ubuntu orange
            
            // Selected: Ubuntu orange background
            this.ButtonSelectedBackColor = PrimaryColor;  // Ubuntu orange
            this.ButtonSelectedForeColor = OnPrimaryColor;  // White text
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Darker orange
            this.ButtonSelectedHoverBackColor = Color.FromArgb(213, 64, 12);
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(213, 64, 12);
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(255, 255, 255);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Ubuntu red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
