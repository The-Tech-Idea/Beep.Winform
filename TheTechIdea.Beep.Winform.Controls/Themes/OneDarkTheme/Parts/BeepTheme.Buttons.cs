using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyButtons()
        {
            // One Dark buttons - popular dark theme
            // Default: Dark background with warm grey text
            this.ButtonBackColor = Color.FromArgb(40, 44, 52);  // Dark background
            this.ButtonForeColor = Color.FromArgb(171, 178, 191);  // Warm grey text
            this.ButtonBorderColor = Color.FromArgb(60, 66, 82);  // #3C4252
            
            // Hover: Slightly lighter
            this.ButtonHoverBackColor = Color.FromArgb(47, 51, 61);
            this.ButtonHoverForeColor = Color.FromArgb(97, 175, 239);  // One Dark blue
            this.ButtonHoverBorderColor = Color.FromArgb(97, 175, 239);  // Blue border
            
            // Selected: One Dark blue background
            this.ButtonSelectedBackColor = Color.FromArgb(97, 175, 239);  // One Dark blue
            this.ButtonSelectedForeColor = Color.FromArgb(40, 44, 52);  // Dark text on blue
            this.ButtonSelectedBorderColor = Color.FromArgb(97, 175, 239);
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(117, 195, 255);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(40, 44, 52);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(117, 195, 255);
            
            // Pressed: Darker
            this.ButtonPressedBackColor = Color.FromArgb(32, 36, 44);
            this.ButtonPressedForeColor = Color.FromArgb(171, 178, 191);
            this.ButtonPressedBorderColor = Color.FromArgb(60, 66, 82);
            
            // Error button: Red background with dark text
            this.ButtonErrorBackColor = Color.FromArgb(224, 108, 117);  // Red
            this.ButtonErrorForeColor = Color.FromArgb(40, 44, 52);  // Dark text on red
            this.ButtonErrorBorderColor = Color.FromArgb(200, 0, 0);
        }
    }
}