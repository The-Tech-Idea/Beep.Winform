using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyButtons()
        {
            // Metro buttons - Windows Metro design
            // Default: Light gray background with dark text
            this.ButtonBackColor = Color.FromArgb(243, 242, 241);  // Light gray
            this.ButtonForeColor = Color.FromArgb(32, 31, 30);  // Dark text
            this.ButtonBorderColor = Color.FromArgb(225, 225, 225);  // Light gray
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(220, 220, 220);
            this.ButtonHoverForeColor = Color.FromArgb(32, 31, 30);
            this.ButtonHoverBorderColor = Color.FromArgb(0, 120, 215);  // Metro blue
            
            // Selected: Metro blue background
            this.ButtonSelectedBackColor = Color.FromArgb(0, 120, 215);  // Metro blue
            this.ButtonSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.ButtonSelectedBorderColor = Color.FromArgb(0, 120, 215);
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(0, 140, 235);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(255, 255, 255);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(0, 140, 235);
            
            // Pressed: Lighter gray
            this.ButtonPressedBackColor = Color.FromArgb(250, 250, 250);
            this.ButtonPressedForeColor = Color.FromArgb(32, 31, 30);
            this.ButtonPressedBorderColor = Color.FromArgb(225, 225, 225);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(196, 30, 58);  // Metro red
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(160, 0, 30);
        }
    }
}