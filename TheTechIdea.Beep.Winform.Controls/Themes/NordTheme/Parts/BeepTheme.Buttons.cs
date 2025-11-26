using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyButtons()
        {
            // Nord buttons - Arctic-inspired dark theme
            // Default: Dark blue-gray background with light text
            this.ButtonBackColor = BackgroundColor;  // Dark blue-gray
            this.ButtonForeColor = ForeColor;  // Light gray-blue
            this.ButtonBorderColor = BorderColor;  // #434C5E
            
            // Hover: Slightly lighter
            this.ButtonHoverBackColor = Color.FromArgb(59, 66, 82);  // #3B4252
            this.ButtonHoverForeColor = PrimaryColor;  // Nord cyan
            this.ButtonHoverBorderColor = PrimaryColor;  // Nord cyan
            
            // Selected: Nord cyan background
            this.ButtonSelectedBackColor = PrimaryColor;  // Nord cyan
            this.ButtonSelectedForeColor = OnPrimaryColor;  // Dark text on cyan
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Lighter cyan
            this.ButtonSelectedHoverBackColor = Color.FromArgb(156, 212, 228);
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(156, 212, 228);
            
            // Pressed: Darker
            this.ButtonPressedBackColor = Color.FromArgb(38, 44, 56);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Nord red background with dark text
            this.ButtonErrorBackColor = ErrorColor;  // Nord red
            this.ButtonErrorForeColor = OnPrimaryColor;  // Light text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
