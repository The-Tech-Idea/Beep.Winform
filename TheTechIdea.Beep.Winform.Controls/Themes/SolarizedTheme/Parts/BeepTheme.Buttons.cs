using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyButtons()
        {
            // Solarized buttons - scientifically crafted color palette
            // Default: Dark blue-green background with light beige text
            this.ButtonBackColor = BackgroundColor;  // Dark blue-green
            this.ButtonForeColor = ForeColor;  // Light beige text
            this.ButtonBorderColor = BorderColor;  // #586E75
            
            // Hover: Slightly lighter surface
            this.ButtonHoverBackColor = SurfaceColor;  // #073642
            this.ButtonHoverForeColor = SecondaryColor;  // Cyan
            this.ButtonHoverBorderColor = SecondaryColor;  // Cyan
            
            // Selected: Orange background
            this.ButtonSelectedBackColor = AccentColor;  // Orange
            this.ButtonSelectedForeColor = OnPrimaryColor;  // Light text
            this.ButtonSelectedBorderColor = AccentColor;
            
            // Selected hover: Lighter orange derived from Accent
            this.ButtonSelectedHoverBackColor = ThemeUtil.Lighten(AccentColor, 0.08);
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = this.ButtonSelectedHoverBackColor;
            
            // Pressed: Slightly darker than background
            this.ButtonPressedBackColor = ThemeUtil.Darken(BackgroundColor, 0.08);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with light text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = OnPrimaryColor;  // Light text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
