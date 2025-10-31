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
            this.ButtonBackColor = Color.FromArgb(25, 20, 35);  // Very dark purple
            this.ButtonForeColor = Color.FromArgb(200, 150, 255);  // Light purple
            this.ButtonBorderColor = Color.FromArgb(138, 70, 255);  // Purple glow (#8A6FFF)
            
            // Hover: Slightly lighter with cyan border
            this.ButtonHoverBackColor = Color.FromArgb(50, 40, 70);
            this.ButtonHoverForeColor = Color.FromArgb(255, 122, 217);  // Pink text
            this.ButtonHoverBorderColor = Color.FromArgb(150, 200, 255);  // Cyan glow
            
            // Selected: Medium purple with cyan accent
            this.ButtonSelectedBackColor = Color.FromArgb(60, 50, 90);
            this.ButtonSelectedForeColor = Color.FromArgb(122, 252, 255);  // Cyan text
            this.ButtonSelectedBorderColor = Color.FromArgb(122, 252, 255);
            
            // Selected hover: Lighter purple
            this.ButtonSelectedHoverBackColor = Color.FromArgb(75, 60, 110);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(122, 252, 255);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(122, 252, 255);
            
            // Pressed: Darker
            this.ButtonPressedBackColor = Color.FromArgb(20, 15, 28);
            this.ButtonPressedForeColor = Color.FromArgb(200, 150, 255);
            this.ButtonPressedBorderColor = Color.FromArgb(138, 70, 255);
            
            // Error button: Pink-red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(255, 138, 167);  // Pink-red
            this.ButtonErrorForeColor = Color.FromArgb(25, 20, 35);  // Dark text on pink
            this.ButtonErrorBorderColor = Color.FromArgb(255, 80, 120);
        }
    }
}