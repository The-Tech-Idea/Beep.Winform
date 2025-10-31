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
            this.ButtonBackColor = Color.FromArgb(15, 15, 25);  // Deep navy
            this.ButtonForeColor = Color.FromArgb(0, 255, 255);  // Cyan text
            this.ButtonBorderColor = Color.FromArgb(0, 255, 255);  // Cyan glow
            
            // Hover: Slightly lighter with brighter cyan
            this.ButtonHoverBackColor = Color.FromArgb(25, 25, 40);
            this.ButtonHoverForeColor = Color.FromArgb(100, 255, 255);  // Brighter cyan
            this.ButtonHoverBorderColor = Color.FromArgb(0, 255, 200);  // Cyan border
            
            // Selected: Cyan background
            this.ButtonSelectedBackColor = Color.FromArgb(0, 255, 255);  // Cyan
            this.ButtonSelectedForeColor = Color.FromArgb(15, 15, 25);  // Dark text on cyan
            this.ButtonSelectedBorderColor = Color.FromArgb(0, 255, 255);
            
            // Selected hover: Lighter cyan
            this.ButtonSelectedHoverBackColor = Color.FromArgb(100, 255, 255);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(15, 15, 25);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(100, 255, 255);
            
            // Pressed: Darker navy
            this.ButtonPressedBackColor = Color.FromArgb(10, 12, 20);
            this.ButtonPressedForeColor = Color.FromArgb(0, 255, 255);
            this.ButtonPressedBorderColor = Color.FromArgb(0, 255, 255);
            
            // Error button: Pink-red background with dark text
            this.ButtonErrorBackColor = Color.FromArgb(255, 64, 160);  // Pink
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on pink
            this.ButtonErrorBorderColor = Color.FromArgb(255, 100, 200);
        }
    }
}