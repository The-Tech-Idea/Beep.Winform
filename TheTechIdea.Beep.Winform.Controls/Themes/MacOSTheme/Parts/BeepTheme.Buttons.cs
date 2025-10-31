using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyButtons()
        {
            // MacOS buttons - clean macOS aesthetic
            // Default: Light gray background with dark text
            this.ButtonBackColor = Color.FromArgb(250, 250, 252);  // Light gray
            this.ButtonForeColor = Color.FromArgb(28, 28, 30);  // Dark gray
            this.ButtonBorderColor = Color.FromArgb(229, 229, 234);  // Medium gray
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(235, 235, 242);
            this.ButtonHoverForeColor = Color.FromArgb(28, 28, 30);
            this.ButtonHoverBorderColor = Color.FromArgb(80, 80, 80);  // Dark gray border
            
            // Selected: Dark gray background
            this.ButtonSelectedBackColor = Color.FromArgb(220, 220, 230);
            this.ButtonSelectedForeColor = Color.FromArgb(28, 28, 30);
            this.ButtonSelectedBorderColor = Color.FromArgb(80, 80, 80);
            
            // Selected hover: Lighter gray
            this.ButtonSelectedHoverBackColor = Color.FromArgb(230, 230, 240);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(28, 28, 30);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(80, 80, 80);
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(255, 255, 255);
            this.ButtonPressedForeColor = Color.FromArgb(28, 28, 30);
            this.ButtonPressedBorderColor = Color.FromArgb(229, 229, 234);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(255, 69, 58);  // macOS red
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(220, 0, 0);
        }
    }
}