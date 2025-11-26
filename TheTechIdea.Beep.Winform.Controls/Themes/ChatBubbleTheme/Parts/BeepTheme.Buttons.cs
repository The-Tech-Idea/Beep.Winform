using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyButtons()
        {
            // ChatBubble buttons - soft blue theme
            // Default: Cyan background with black text
            this.ButtonBackColor = SurfaceColor;  // Light cyan/white surface
            this.ButtonForeColor = ForeColor;  // Black
            this.ButtonBorderColor = BorderColor;  // Light gray
            
            // Hover: Slightly darker blue
            this.ButtonHoverBackColor = Color.FromArgb(210, 240, 250);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Blue border
            
            // Selected: Medium blue
            this.ButtonSelectedBackColor = Color.FromArgb(180, 230, 250);
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedBorderColor = ActiveBorderColor;
            
            // Selected hover: Darker blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(160, 220, 250);
            this.ButtonSelectedHoverForeColor = ForeColor;
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(250, 255, 255);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = BorderColor;
        }
    }
}
