using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyButtons()
        {
            // KDE buttons - clean Linux aesthetic
            // Default: Light background with dark text
            this.ButtonBackColor = Color.FromArgb(248, 249, 250);  // Light gray
            this.ButtonForeColor = Color.FromArgb(33, 37, 41);  // Dark gray
            this.ButtonBorderColor = Color.FromArgb(222, 226, 230);  // Medium gray
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(230, 235, 240);
            this.ButtonHoverForeColor = Color.FromArgb(33, 37, 41);
            this.ButtonHoverBorderColor = Color.FromArgb(61, 174, 233);  // KDE blue
            
            // Selected: KDE blue background
            this.ButtonSelectedBackColor = Color.FromArgb(61, 174, 233);  // KDE blue
            this.ButtonSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.ButtonSelectedBorderColor = Color.FromArgb(61, 174, 233);
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(81, 194, 253);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(255, 255, 255);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(81, 194, 253);
            
            // Pressed: Lighter gray
            this.ButtonPressedBackColor = Color.FromArgb(250, 252, 255);
            this.ButtonPressedForeColor = Color.FromArgb(33, 37, 41);
            this.ButtonPressedBorderColor = Color.FromArgb(222, 226, 230);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(220, 53, 69);  // Bootstrap red
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(180, 0, 0);
        }
    }
}