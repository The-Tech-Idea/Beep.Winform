using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyButtons()
        {
            // Paper buttons - Material Design aesthetic
            // Default: Light grey background with dark text
            this.ButtonBackColor = Color.FromArgb(250, 250, 250);  // Light paper
            this.ButtonForeColor = Color.FromArgb(33, 33, 33);  // Dark grey text
            this.ButtonBorderColor = Color.FromArgb(224, 224, 224);  // Light grey border
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(240, 240, 240);
            this.ButtonHoverForeColor = Color.FromArgb(33, 33, 33);
            this.ButtonHoverBorderColor = Color.FromArgb(33, 150, 243);  // Material blue
            
            // Selected: Material blue background
            this.ButtonSelectedBackColor = Color.FromArgb(33, 150, 243);  // Material blue
            this.ButtonSelectedForeColor = Color.White;  // White text
            this.ButtonSelectedBorderColor = Color.FromArgb(33, 150, 243);
            
            // Selected hover: Darker blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(20, 140, 233);
            this.ButtonSelectedHoverForeColor = Color.White;
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(20, 140, 233);
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(255, 255, 255);
            this.ButtonPressedForeColor = Color.FromArgb(33, 33, 33);
            this.ButtonPressedBorderColor = Color.FromArgb(224, 224, 224);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(244, 67, 54);  // Red
            this.ButtonErrorForeColor = Color.White;  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(200, 0, 0);
        }
    }
}