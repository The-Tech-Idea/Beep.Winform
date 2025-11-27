using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyButtons()
        {
            // Metro2 buttons - Windows Metro with accent stripe
            // Default: White background with dark text
            this.ButtonBackColor = BackgroundColor;  // White
            this.ButtonForeColor = ForeColor;  // Black text
            this.ButtonBorderColor = BorderColor;  // Metro blue border
            
            // Hover: Light gray background
            this.ButtonHoverBackColor = PanelGradiantMiddleColor;
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Metro blue
            
            // Selected: Metro blue background
            this.ButtonSelectedBackColor = PrimaryColor;  // Metro blue
            this.ButtonSelectedForeColor = OnPrimaryColor;  // White text
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = PrimaryColor;
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = PrimaryColor;
            
            // Pressed: Lighter gray
            this.ButtonPressedBackColor = PanelGradiantEndColor;
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Metro red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
