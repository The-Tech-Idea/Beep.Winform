using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyButtons()
        {
            // Dracula buttons - dark purple with pink accents
            // Default: Dark with cream text
            this.ButtonBackColor = BackgroundColor;  // Dark background
            this.ButtonForeColor = ForeColor;  // Cream text
            this.ButtonBorderColor = BorderColor;  // Gray border
            
            // Hover: Slightly lighter purple background
            this.ButtonHoverBackColor = SurfaceColor;
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = PrimaryColor;  // Purple border
            
            // Selected: Purple background
            this.ButtonSelectedBackColor = SurfaceColor;
            this.ButtonSelectedForeColor = AccentColor;  // Pink text
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Medium purple
            this.ButtonSelectedHoverBackColor = Color.FromArgb(76, 79, 98);
            this.ButtonSelectedHoverForeColor = AccentColor;
            this.ButtonSelectedHoverBorderColor = PrimaryColor;
            
            // Pressed: Darker
            this.ButtonPressedBackColor = Color.FromArgb(32, 34, 44);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = OnPrimaryColor;  // Cream/white text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
