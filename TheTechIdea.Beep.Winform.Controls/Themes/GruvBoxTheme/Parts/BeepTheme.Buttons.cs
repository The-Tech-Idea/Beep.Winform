using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyButtons()
        {
            // GruvBox buttons - warm retro colors
            // Default: Dark background with beige text
            this.ButtonBackColor = Color.FromArgb(40, 40, 40);  // Dark gray
            this.ButtonForeColor = Color.FromArgb(235, 219, 178);  // Beige text
            this.ButtonBorderColor = Color.FromArgb(80, 73, 69);  // Muted brown
            
            // Hover: Slightly lighter background
            this.ButtonHoverBackColor = Color.FromArgb(50, 48, 47);
            this.ButtonHoverForeColor = Color.FromArgb(250, 189, 47);  // Yellow text
            this.ButtonHoverBorderColor = Color.FromArgb(254, 128, 25);  // Orange border
            
            // Selected: Medium brown background
            this.ButtonSelectedBackColor = Color.FromArgb(60, 56, 54);  // #3C3836
            this.ButtonSelectedForeColor = Color.FromArgb(254, 128, 25);  // Orange text
            this.ButtonSelectedBorderColor = Color.FromArgb(254, 128, 25);
            
            // Selected hover: Lighter brown
            this.ButtonSelectedHoverBackColor = Color.FromArgb(68, 64, 62);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(254, 128, 25);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(254, 128, 25);
            
            // Pressed: Darker
            this.ButtonPressedBackColor = Color.FromArgb(30, 30, 30);
            this.ButtonPressedForeColor = Color.FromArgb(235, 219, 178);
            this.ButtonPressedBorderColor = Color.FromArgb(80, 73, 69);
            
            // Error button: Red background with dark text
            this.ButtonErrorBackColor = Color.FromArgb(251, 73, 52);  // Red
            this.ButtonErrorForeColor = Color.FromArgb(235, 219, 178);  // Beige text on red
            this.ButtonErrorBorderColor = Color.FromArgb(200, 0, 0);
        }
    }
}