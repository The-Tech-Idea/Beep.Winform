using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyButtons()
        {
            // Glass buttons - frosted glass effect
            // Default: Light blue background with dark text
            this.ButtonBackColor = Color.FromArgb(236, 244, 255);  // Light blue
            this.ButtonForeColor = Color.FromArgb(17, 24, 39);  // Dark gray
            this.ButtonBorderColor = Color.FromArgb(200, 220, 240);  // Visible border
            
            // Hover: Slightly darker blue
            this.ButtonHoverBackColor = Color.FromArgb(216, 234, 250);
            this.ButtonHoverForeColor = Color.FromArgb(17, 24, 39);
            this.ButtonHoverBorderColor = Color.FromArgb(99, 102, 241);  // Indigo border
            
            // Selected: Medium blue
            this.ButtonSelectedBackColor = Color.FromArgb(190, 220, 245);
            this.ButtonSelectedForeColor = Color.FromArgb(17, 24, 39);
            this.ButtonSelectedBorderColor = Color.FromArgb(99, 102, 241);
            
            // Selected hover: Darker blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(170, 210, 240);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(17, 24, 39);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(99, 102, 241);
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(250, 252, 255);
            this.ButtonPressedForeColor = Color.FromArgb(17, 24, 39);
            this.ButtonPressedBorderColor = Color.FromArgb(200, 220, 240);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(239, 68, 68);  // Red
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(180, 0, 0);
        }
    }
}