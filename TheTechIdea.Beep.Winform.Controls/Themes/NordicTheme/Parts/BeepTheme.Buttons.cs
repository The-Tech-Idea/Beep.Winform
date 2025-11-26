using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyButtons()
        {
            // Nordic buttons - Scandinavian minimalist design
            // Default: Light gray background with dark text
            this.ButtonBackColor = BackgroundColor;  // Light gray-blue
            this.ButtonForeColor = ForeColor;  // Dark gray text
            this.ButtonBorderColor = BorderColor;  // Light gray border
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(228, 232, 238);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Icy blue border
            
            // Selected: Icy blue background
            this.ButtonSelectedBackColor = PrimaryColor;  // Icy blue
            this.ButtonSelectedForeColor = OnPrimaryColor;  // White text
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(156, 212, 228);
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(156, 212, 228);
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(248, 250, 252);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
