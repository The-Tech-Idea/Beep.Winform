using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyButtons()
        {
            // Solarized buttons - scientifically crafted color palette
            // Default: Dark blue-green background with light beige text
            this.ButtonBackColor = Color.FromArgb(0, 43, 54);  // Dark blue-green
            this.ButtonForeColor = Color.FromArgb(238, 232, 213);  // Light beige text
            this.ButtonBorderColor = Color.FromArgb(88, 110, 117);  // #586E75
            
            // Hover: Slightly lighter
            this.ButtonHoverBackColor = Color.FromArgb(7, 54, 66);  // #073642
            this.ButtonHoverForeColor = Color.FromArgb(42, 161, 152);  // Cyan
            this.ButtonHoverBorderColor = Color.FromArgb(42, 161, 152);  // Cyan
            
            // Selected: Orange background
            this.ButtonSelectedBackColor = Color.FromArgb(203, 75, 22);  // Orange
            this.ButtonSelectedForeColor = Color.FromArgb(238, 232, 213);  // Light text
            this.ButtonSelectedBorderColor = Color.FromArgb(203, 75, 22);
            
            // Selected hover: Lighter orange
            this.ButtonSelectedHoverBackColor = Color.FromArgb(223, 95, 42);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(238, 232, 213);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(223, 95, 42);
            
            // Pressed: Darker
            this.ButtonPressedBackColor = Color.FromArgb(0, 36, 46);
            this.ButtonPressedForeColor = Color.FromArgb(238, 232, 213);
            this.ButtonPressedBorderColor = Color.FromArgb(88, 110, 117);
            
            // Error button: Red background with light text
            this.ButtonErrorBackColor = Color.FromArgb(220, 50, 47);  // Red
            this.ButtonErrorForeColor = Color.FromArgb(238, 232, 213);  // Light text on red
            this.ButtonErrorBorderColor = Color.FromArgb(180, 0, 0);
        }
    }
}