using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyButtons()
        {
            // Neon buttons - vibrant neon aesthetic
            // Default: Dark navy background with cyan text
            this.ButtonBackColor = BackgroundColor;  // Deep navy
            this.ButtonForeColor = ForeColor;  // Cyan text
            this.ButtonBorderColor = BorderColor;  // Cyan glow
            
            // Hover: Slightly lighter with brighter cyan
            this.ButtonHoverBackColor = Color.FromArgb(25, 25, 40);
            this.ButtonHoverForeColor = Color.FromArgb(100, 255, 255);  // Brighter cyan
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Cyan border
            
            // Selected: Cyan background
            this.ButtonSelectedBackColor = PrimaryColor;  // Cyan
            this.ButtonSelectedForeColor = OnPrimaryColor;  // Dark text on cyan
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Lighter cyan
            this.ButtonSelectedHoverBackColor = Color.FromArgb(100, 255, 255);
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(100, 255, 255);
            
            // Pressed: Darker navy
            this.ButtonPressedBackColor = Color.FromArgb(10, 12, 20);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Pink-red background with dark text
            this.ButtonErrorBackColor = ErrorColor;  // Pink
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on pink
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
