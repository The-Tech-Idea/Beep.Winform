using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyButtons()
        {
            // iOS buttons - clean, modern aesthetic
            // Default: Light gray background with dark text
            this.ButtonBackColor = Color.FromArgb(230, 230, 235); // Light gray
            this.ButtonForeColor = ForeColor;  // Dark gray
            this.ButtonBorderColor = BorderColor;  // Medium gray
            
            // Hover: Darker background
            this.ButtonHoverBackColor = ThemeUtil.Darken(BackgroundColor, 0.06);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // iOS blue
            
            // Selected: iOS blue background
            this.ButtonSelectedBackColor = PrimaryColor;  // iOS blue
            this.ButtonSelectedForeColor = OnPrimaryColor;  // White text
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = ThemeUtil.Lighten(PrimaryColor, 0.06);
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = ThemeUtil.Lighten(PrimaryColor, 0.06);
            
            // Pressed: Lighter gray
            this.ButtonPressedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.02);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // iOS red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}








