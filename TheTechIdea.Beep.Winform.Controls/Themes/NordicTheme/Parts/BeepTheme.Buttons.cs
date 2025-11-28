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
            this.ButtonHoverBackColor = PanelGradiantMiddleColor;
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Icy blue border
            
            // Selected: Icy blue background
            this.ButtonSelectedBackColor = PrimaryColor;  // Icy blue
            this.ButtonSelectedForeColor = OnPrimaryColor;  // White text
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = PrimaryColor; // use PrimaryColor for selected hover
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = PrimaryColor;
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = PanelBackColor;
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
