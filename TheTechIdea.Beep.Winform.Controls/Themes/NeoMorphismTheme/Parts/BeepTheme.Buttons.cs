using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyButtons()
        {
            // NeoMorphism buttons - soft neomorphic effect
            // Default: Light gray background with dark text
            this.ButtonBackColor = Color.FromArgb(240, 240, 245);  // Light gray-blue
            this.ButtonForeColor = Color.FromArgb(50, 50, 60);  // Dark gray text
            this.ButtonBorderColor = Color.FromArgb(220, 220, 225);  // Soft border
            
            // Hover: Slightly darker for recessed effect
            this.ButtonHoverBackColor = Color.FromArgb(228, 228, 233);
            this.ButtonHoverForeColor = Color.FromArgb(50, 50, 60);
            this.ButtonHoverBorderColor = Color.FromArgb(76, 110, 245);  // Blue border
            
            // Selected: Blue background
            this.ButtonSelectedBackColor = Color.FromArgb(76, 110, 245);  // Blue
            this.ButtonSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.ButtonSelectedBorderColor = Color.FromArgb(76, 110, 245);
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(90, 125, 255);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(255, 255, 255);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(90, 125, 255);
            
            // Pressed: Lighter for embossed effect
            this.ButtonPressedBackColor = Color.FromArgb(245, 245, 250);
            this.ButtonPressedForeColor = Color.FromArgb(50, 50, 60);
            this.ButtonPressedBorderColor = Color.FromArgb(220, 220, 225);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(231, 76, 60);  // Red
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(200, 0, 0);
        }
    }
}