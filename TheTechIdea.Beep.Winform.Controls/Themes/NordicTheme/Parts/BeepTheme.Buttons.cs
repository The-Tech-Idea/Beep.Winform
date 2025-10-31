using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyButtons()
        {
            // Nordic buttons - Scandinavian minimalist design
            // Default: Light gray background with dark text
            this.ButtonBackColor = Color.FromArgb(242, 245, 248);  // Light gray-blue
            this.ButtonForeColor = Color.FromArgb(60, 60, 60);  // Dark gray text
            this.ButtonBorderColor = Color.FromArgb(220, 220, 220);  // Light gray border
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(228, 232, 238);
            this.ButtonHoverForeColor = Color.FromArgb(60, 60, 60);
            this.ButtonHoverBorderColor = Color.FromArgb(136, 192, 208);  // Icy blue border
            
            // Selected: Icy blue background
            this.ButtonSelectedBackColor = Color.FromArgb(136, 192, 208);  // Icy blue
            this.ButtonSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.ButtonSelectedBorderColor = Color.FromArgb(136, 192, 208);
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(156, 212, 228);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(255, 255, 255);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(156, 212, 228);
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(248, 250, 252);
            this.ButtonPressedForeColor = Color.FromArgb(60, 60, 60);
            this.ButtonPressedBorderColor = Color.FromArgb(220, 220, 220);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(220, 38, 38);  // Red
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(180, 0, 0);
        }
    }
}