using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyButtons()
        {
            // One Dark buttons - popular dark theme
            // Default: Dark background with warm grey text
            this.ButtonBackColor = BackgroundColor;  // Dark background
            this.ButtonForeColor = ForeColor;  // Warm grey text
            this.ButtonBorderColor = BorderColor;  // #3C4252
            
            // Hover: Slightly lighter
            this.ButtonHoverBackColor = PanelGradiantMiddleColor;
            this.ButtonHoverForeColor = PrimaryColor;  // One Dark blue
            this.ButtonHoverBorderColor = PrimaryColor;  // Blue border
            
            // Selected: One Dark blue background
            this.ButtonSelectedBackColor = PrimaryColor;  // One Dark blue
            this.ButtonSelectedForeColor = OnPrimaryColor;  // Dark text on blue
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = ThemeUtil.Lighten(PrimaryColor, 0.3);
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;
            
            // Pressed: Darker
            this.ButtonPressedBackColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with dark text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = OnPrimaryColor;  // Light text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
