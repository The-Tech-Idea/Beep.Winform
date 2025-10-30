using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyButtons()
        {
            // Fluent buttons - clean light theme
            // Default: Light background with dark text
            this.ButtonBackColor = Color.FromArgb(245, 246, 248);  // Light gray-blue
            this.ButtonForeColor = Color.FromArgb(32, 32, 32);  // Dark gray
            this.ButtonBorderColor = Color.FromArgb(218, 223, 230);  // Light gray border
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(230, 235, 240);
            this.ButtonHoverForeColor = Color.FromArgb(32, 32, 32);
            this.ButtonHoverBorderColor = Color.FromArgb(0, 120, 215);  // Blue border
            
            // Selected: Light blue background
            this.ButtonSelectedBackColor = Color.FromArgb(220, 230, 245);
            this.ButtonSelectedForeColor = Color.FromArgb(0, 80, 160);  // Darker blue
            this.ButtonSelectedBorderColor = Color.FromArgb(0, 120, 215);
            
            // Selected hover: Medium blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(200, 220, 240);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(0, 80, 160);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(0, 120, 215);
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(250, 251, 253);
            this.ButtonPressedForeColor = Color.FromArgb(32, 32, 32);
            this.ButtonPressedBorderColor = Color.FromArgb(218, 223, 230);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(196, 30, 58);  // Fluent red
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(160, 0, 30);
        }
    }
}