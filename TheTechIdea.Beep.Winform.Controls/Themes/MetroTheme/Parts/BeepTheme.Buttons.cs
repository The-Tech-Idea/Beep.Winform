using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyButtons()
        {
            // Metro buttons - Windows Metro design
            // Default: Light gray background with dark text
            this.ButtonBackColor = BackgroundColor;  // Light gray
            this.ButtonForeColor = ForeColor;  // Dark text
            this.ButtonBorderColor = BorderColor;  // Light gray
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(220, 220, 220);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Metro blue
            
            // Selected: Metro blue background
            this.ButtonSelectedBackColor = PrimaryColor;  // Metro blue
            this.ButtonSelectedForeColor = OnPrimaryColor;  // White text
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(0, 140, 235);
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(0, 140, 235);
            
            // Pressed: Lighter gray
            this.ButtonPressedBackColor = Color.FromArgb(250, 250, 250);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Metro red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
