using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyButtons()
        {
            // Nord buttons - Arctic-inspired dark theme
            // Default: Dark blue-gray background with light text
            this.ButtonBackColor = Color.FromArgb(46, 52, 64);  // Dark blue-gray
            this.ButtonForeColor = Color.FromArgb(216, 222, 233);  // Light gray-blue
            this.ButtonBorderColor = Color.FromArgb(67, 76, 94);  // #434C5E
            
            // Hover: Slightly lighter
            this.ButtonHoverBackColor = Color.FromArgb(59, 66, 82);  // #3B4252
            this.ButtonHoverForeColor = Color.FromArgb(136, 192, 208);  // Nord cyan
            this.ButtonHoverBorderColor = Color.FromArgb(136, 192, 208);  // Nord cyan
            
            // Selected: Nord cyan background
            this.ButtonSelectedBackColor = Color.FromArgb(136, 192, 208);  // Nord cyan
            this.ButtonSelectedForeColor = Color.FromArgb(46, 52, 64);  // Dark text on cyan
            this.ButtonSelectedBorderColor = Color.FromArgb(136, 192, 208);
            
            // Selected hover: Lighter cyan
            this.ButtonSelectedHoverBackColor = Color.FromArgb(156, 212, 228);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(46, 52, 64);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(156, 212, 228);
            
            // Pressed: Darker
            this.ButtonPressedBackColor = Color.FromArgb(38, 44, 56);
            this.ButtonPressedForeColor = Color.FromArgb(216, 222, 233);
            this.ButtonPressedBorderColor = Color.FromArgb(67, 76, 94);
            
            // Error button: Nord red background with dark text
            this.ButtonErrorBackColor = Color.FromArgb(191, 97, 106);  // Nord red
            this.ButtonErrorForeColor = Color.FromArgb(46, 52, 64);  // Dark text on red
            this.ButtonErrorBorderColor = Color.FromArgb(170, 0, 0);
        }
    }
}