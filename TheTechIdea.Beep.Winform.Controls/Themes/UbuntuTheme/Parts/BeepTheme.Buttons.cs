using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyButtons()
        {
            // Ubuntu buttons - Ubuntu Linux desktop aesthetic
            // Default: Light grey background with dark text
            this.ButtonBackColor = Color.FromArgb(242, 242, 245);  // Light grey
            this.ButtonForeColor = Color.FromArgb(44, 44, 44);  // Dark grey text
            this.ButtonBorderColor = Color.FromArgb(218, 218, 222);  // Light grey border
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(230, 230, 235);
            this.ButtonHoverForeColor = Color.FromArgb(44, 44, 44);
            this.ButtonHoverBorderColor = Color.FromArgb(233, 84, 32);  // Ubuntu orange
            
            // Selected: Ubuntu orange background
            this.ButtonSelectedBackColor = Color.FromArgb(233, 84, 32);  // Ubuntu orange
            this.ButtonSelectedForeColor = Color.White;  // White text
            this.ButtonSelectedBorderColor = Color.FromArgb(233, 84, 32);
            
            // Selected hover: Darker orange
            this.ButtonSelectedHoverBackColor = Color.FromArgb(213, 64, 12);
            this.ButtonSelectedHoverForeColor = Color.White;
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(213, 64, 12);
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(255, 255, 255);
            this.ButtonPressedForeColor = Color.FromArgb(44, 44, 44);
            this.ButtonPressedBorderColor = Color.FromArgb(218, 218, 222);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(192, 28, 40);  // Ubuntu red
            this.ButtonErrorForeColor = Color.White;  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(150, 0, 0);
        }
    }
}