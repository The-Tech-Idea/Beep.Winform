using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyButtons()
        {
            // iOS buttons - clean, modern aesthetic
            // Default: Light gray background with dark text
            this.ButtonBackColor = Color.FromArgb(242, 242, 247);  // Light gray
            this.ButtonForeColor = Color.FromArgb(28, 28, 30);  // Dark gray
            this.ButtonBorderColor = Color.FromArgb(198, 198, 207);  // Medium gray
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(220, 220, 230);
            this.ButtonHoverForeColor = Color.FromArgb(28, 28, 30);
            this.ButtonHoverBorderColor = Color.FromArgb(10, 132, 255);  // iOS blue
            
            // Selected: iOS blue background
            this.ButtonSelectedBackColor = Color.FromArgb(10, 132, 255);  // iOS blue
            this.ButtonSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.ButtonSelectedBorderColor = Color.FromArgb(10, 132, 255);
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(20, 142, 255);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(255, 255, 255);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(20, 142, 255);
            
            // Pressed: Lighter gray
            this.ButtonPressedBackColor = Color.FromArgb(250, 250, 255);
            this.ButtonPressedForeColor = Color.FromArgb(28, 28, 30);
            this.ButtonPressedBorderColor = Color.FromArgb(198, 198, 207);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(255, 69, 58);  // iOS red
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(220, 0, 0);
        }
    }
}