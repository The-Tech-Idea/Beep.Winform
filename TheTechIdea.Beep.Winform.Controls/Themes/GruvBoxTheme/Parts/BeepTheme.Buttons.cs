using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyButtons()
        {
            // GruvBox buttons - warm retro colors
            // Default: Dark background with beige text
            this.ButtonBackColor = BackgroundColor;  // Dark gray
            this.ButtonForeColor = ForeColor;  // Beige text
            this.ButtonBorderColor = BorderColor;  // Muted brown
            
            // Hover: Slightly lighter background
            this.ButtonHoverBackColor = Color.FromArgb(50, 48, 47);
            this.ButtonHoverForeColor = SuccessColor;  // Yellow text
            this.ButtonHoverBorderColor = AccentColor;  // Orange border
            
            // Selected: Medium brown background
            this.ButtonSelectedBackColor = Color.FromArgb(60, 56, 54);  // #3C3836
            this.ButtonSelectedForeColor = AccentColor;  // Orange text
            this.ButtonSelectedBorderColor = AccentColor;
            
            // Selected hover: Lighter brown
            this.ButtonSelectedHoverBackColor = Color.FromArgb(68, 64, 62);
            this.ButtonSelectedHoverForeColor = AccentColor;
            this.ButtonSelectedHoverBorderColor = AccentColor;
            
            // Pressed: Darker
            this.ButtonPressedBackColor = Color.FromArgb(30, 30, 30);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with dark text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = OnPrimaryColor;  // Beige text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
