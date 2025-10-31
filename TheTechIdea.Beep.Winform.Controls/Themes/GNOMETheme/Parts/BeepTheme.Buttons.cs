using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyButtons()
        {
            // GNOME buttons - clean modern aesthetic
            // Default: White background with dark text
            this.ButtonBackColor = Color.FromArgb(255, 255, 255);  // White
            this.ButtonForeColor = Color.FromArgb(35, 38, 41);  // Dark gray
            this.ButtonBorderColor = Color.FromArgb(200, 200, 200);  // Light gray
            
            // Hover: Light gray background
            this.ButtonHoverBackColor = Color.FromArgb(240, 240, 240);
            this.ButtonHoverForeColor = Color.FromArgb(35, 38, 41);
            this.ButtonHoverBorderColor = Color.FromArgb(50, 50, 50);  // Darker border
            
            // Selected: Medium gray background
            this.ButtonSelectedBackColor = Color.FromArgb(220, 220, 220);
            this.ButtonSelectedForeColor = Color.FromArgb(35, 38, 41);
            this.ButtonSelectedBorderColor = Color.FromArgb(50, 50, 50);
            
            // Selected hover: Darker gray
            this.ButtonSelectedHoverBackColor = Color.FromArgb(200, 200, 200);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(35, 38, 41);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(50, 50, 50);
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(250, 250, 250);
            this.ButtonPressedForeColor = Color.FromArgb(35, 38, 41);
            this.ButtonPressedBorderColor = Color.FromArgb(200, 200, 200);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(237, 21, 21);  // Red
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(180, 0, 0);
        }
    }
}