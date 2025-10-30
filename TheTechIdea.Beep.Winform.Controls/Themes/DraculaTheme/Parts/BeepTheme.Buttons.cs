using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyButtons()
        {
            // Dracula buttons - dark purple with pink accents
            // Default: Dark with cream text
            this.ButtonBackColor = Color.FromArgb(40, 42, 54);  // Dark background
            this.ButtonForeColor = Color.FromArgb(248, 248, 242);  // Cream text
            this.ButtonBorderColor = Color.FromArgb(68, 71, 90);  // Gray border
            
            // Hover: Slightly lighter purple background
            this.ButtonHoverBackColor = Color.FromArgb(52, 55, 72);
            this.ButtonHoverForeColor = Color.FromArgb(248, 248, 242);
            this.ButtonHoverBorderColor = Color.FromArgb(189, 147, 249);  // Purple border
            
            // Selected: Purple background
            this.ButtonSelectedBackColor = Color.FromArgb(68, 71, 90);
            this.ButtonSelectedForeColor = Color.FromArgb(255, 121, 198);  // Pink text
            this.ButtonSelectedBorderColor = Color.FromArgb(189, 147, 249);
            
            // Selected hover: Medium purple
            this.ButtonSelectedHoverBackColor = Color.FromArgb(76, 79, 98);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(255, 121, 198);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(189, 147, 249);
            
            // Pressed: Darker
            this.ButtonPressedBackColor = Color.FromArgb(32, 34, 44);
            this.ButtonPressedForeColor = Color.FromArgb(248, 248, 242);
            this.ButtonPressedBorderColor = Color.FromArgb(68, 71, 90);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(255, 85, 85);  // Red
            this.ButtonErrorForeColor = Color.FromArgb(248, 248, 242);  // Cream text on red
            this.ButtonErrorBorderColor = Color.FromArgb(200, 0, 0);
        }
    }
}