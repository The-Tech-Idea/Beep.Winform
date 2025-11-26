using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyButtons()
        {
            // Cartoon buttons - playful with purple accents
            // Default: Light background with purple text
            this.ButtonBackColor = SurfaceColor;  // Pink-tinted background
            this.ButtonForeColor = ForeColor;  // Purple text
            this.ButtonBorderColor = BorderColor;  // Purple border
            
            // Hover: Slightly darker purple background
            this.ButtonHoverBackColor = Color.FromArgb(240, 220, 255);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;
            
            // Selected: Light purple background
            this.ButtonSelectedBackColor = Color.FromArgb(220, 180, 255);
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedBorderColor = ActiveBorderColor;
            
            // Selected hover: Medium purple
            this.ButtonSelectedHoverBackColor = Color.FromArgb(200, 160, 255);
            this.ButtonSelectedHoverForeColor = ForeColor;
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;
            
            // Pressed: Darker for feedback
            this.ButtonPressedBackColor = Color.FromArgb(255, 250, 255);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Pink background with white text
            this.ButtonErrorBackColor = ErrorColor;
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on pink
            this.ButtonErrorBorderColor = BorderColor;
        }
    }
}
