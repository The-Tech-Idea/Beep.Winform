using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyButtons()
        {
            // Metro2 buttons - Windows Metro with accent stripe
            // Default: White background with dark text
            this.ButtonBackColor = Color.FromArgb(255, 255, 255);  // White
            this.ButtonForeColor = Color.FromArgb(0, 0, 0);  // Black text
            this.ButtonBorderColor = Color.FromArgb(0, 120, 215);  // Metro blue border
            
            // Hover: Light gray background
            this.ButtonHoverBackColor = Color.FromArgb(240, 240, 240);
            this.ButtonHoverForeColor = Color.FromArgb(0, 0, 0);
            this.ButtonHoverBorderColor = Color.FromArgb(0, 120, 215);  // Metro blue
            
            // Selected: Metro blue background
            this.ButtonSelectedBackColor = Color.FromArgb(0, 120, 215);  // Metro blue
            this.ButtonSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.ButtonSelectedBorderColor = Color.FromArgb(0, 120, 215);
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(0, 150, 255);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(255, 255, 255);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(0, 150, 255);
            
            // Pressed: Lighter gray
            this.ButtonPressedBackColor = Color.FromArgb(250, 250, 250);
            this.ButtonPressedForeColor = Color.FromArgb(0, 0, 0);
            this.ButtonPressedBorderColor = Color.FromArgb(0, 120, 215);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(196, 30, 58);  // Metro red
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(160, 0, 30);
        }
    }
}