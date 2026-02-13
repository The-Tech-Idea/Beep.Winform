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
            this.ButtonBackColor = Color.FromArgb(82, 86, 108); // Dark background
            this.ButtonForeColor = ForeColor;  // Cream text
            this.ButtonBorderColor = BorderColor;  // Gray border
            
            // Hover: Slightly lighter purple background
            this.ButtonHoverBackColor = ThemeUtil.Lighten(SurfaceColor, 0.06);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = PrimaryColor;  // Purple border
            
            // Selected: Purple background
            this.ButtonSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.06);
            this.ButtonSelectedForeColor = AccentColor;  // Pink text
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Medium purple
            this.ButtonSelectedHoverBackColor = PanelGradiantMiddleColor;
            this.ButtonSelectedHoverForeColor = AccentColor;
            this.ButtonSelectedHoverBorderColor = PrimaryColor;
            
            // Pressed: Darker
            this.ButtonPressedBackColor = PanelGradiantStartColor;
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = OnPrimaryColor;  // Cream/white text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}










