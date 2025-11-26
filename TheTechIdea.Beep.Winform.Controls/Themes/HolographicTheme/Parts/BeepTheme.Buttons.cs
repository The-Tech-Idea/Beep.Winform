using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyButtons()
        {
            // Holographic buttons - futuristic with neon accents
            // Default: Dark purple background with light purple text
            this.ButtonBackColor = BackgroundColor;  // Very dark purple
            this.ButtonForeColor = ForeColor;  // Light purple
            this.ButtonBorderColor = BorderColor;  // Purple glow (#8A6FFF)
            
            // Hover: Slightly lighter with cyan border
            this.ButtonHoverBackColor = Color.FromArgb(50, 40, 70);
            this.ButtonHoverForeColor = AccentColor;  // Pink text
            this.ButtonHoverBorderColor = SecondaryColor;  // Cyan glow
            
            // Selected: Medium purple with cyan accent
            this.ButtonSelectedBackColor = Color.FromArgb(60, 50, 90);
            this.ButtonSelectedForeColor = SecondaryColor;  // Cyan text
            this.ButtonSelectedBorderColor = SecondaryColor;
            
            // Selected hover: Lighter purple
            this.ButtonSelectedHoverBackColor = Color.FromArgb(75, 60, 110);
            this.ButtonSelectedHoverForeColor = SecondaryColor;
            this.ButtonSelectedHoverBorderColor = SecondaryColor;
            
            // Pressed: Darker
            this.ButtonPressedBackColor = Color.FromArgb(20, 15, 28);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Pink-red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Pink-red
            this.ButtonErrorForeColor = OnPrimaryColor;  // Dark text on pink
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
